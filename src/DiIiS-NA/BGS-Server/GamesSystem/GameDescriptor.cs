using bgs.protocol.matchmaking.v1;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.ChannelSystem;
using DiIiS_NA.LoginServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.GamesSystem
{
	public class GameDescriptor : Channel
	{
		public int PlayersCount = 0;
		public bgs.protocol.games.v1.GameHandle GameHandle { get; private set; }

		public D3.OnlineService.GameCreateParams GameCreateParams { get; private set; }

		public string Version = "2.7.4";
		public ulong FactoryID { get; private set; }
		public KeyValuePair<string, BattleBackend.ServerDescriptor> GServer { get; private set; }
		public ulong RequestId { get; private set; }
		public bool Started { get; private set; }
		public bool Public { get; set; }

		public GameDescriptor(BattleClient owner, GameMatchmakingOptions request, ulong requestId)
			: base(owner, true)
		{
			this.Started = false;
			this.Public = false;
			this.Owner = owner;
			this.RequestId = requestId;
			this.GServer = ChooseGameServer();
			this.Owner.GameChannel = this;
			this.BnetEntityId = bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.GameId).SetLow(this.DynamicId)
				.Build();
			this.GameHandle = bgs.protocol.games.v1.GameHandle.CreateBuilder().SetFactoryId(this.FactoryID).SetGameId(this.BnetEntityId).Build();

			foreach (bgs.protocol.v2.Attribute attribute in request.CreationProperties.AttributeList)
			{
				if (attribute.Name == "GameCreateParams")
				{
					this.GameCreateParams = D3.OnlineService.GameCreateParams.ParseFrom(attribute.Value.BlobValue);
					if (this.GameCreateParams.CreationFlags == 256 || this.GameCreateParams.CreationFlags == 262400) this.Public = true;
					lock (owner.Account.GameAccount.CurrentToon.DbToon)
					{
						var toonByClient = owner.Account.GameAccount.CurrentToon.DbToon;
						toonByClient.CurrentAct = this.GameCreateParams.CampaignOrAdventureMode.Act;
						toonByClient.CurrentQuestId = (this.GameCreateParams.CampaignOrAdventureMode.SnoQuest == 0 ? 87700 : this.GameCreateParams.CampaignOrAdventureMode.SnoQuest);
						toonByClient.CurrentQuestStepId = (this.GameCreateParams.CampaignOrAdventureMode.QuestStepId == 0 ? -1 : this.GameCreateParams.CampaignOrAdventureMode.QuestStepId);
						toonByClient.CurrentDifficulty = this.GameCreateParams.CampaignOrAdventureMode.HandicapLevel;
						DBSessions.SessionUpdate(toonByClient);
					}
				}
				else if (attribute.Name == "version")
					this.Version = attribute.Value.StringValue;
			}

			foreach (bgs.protocol.v2.Attribute attribute in request.MatchmakerFilter.AttributeList)
			{
				if (attribute.Name == "version")
					this.Version = attribute.Value.StringValue;
			}
		}

		private static KeyValuePair<string, BattleBackend.ServerDescriptor> ChooseGameServer()
		{
			return Program.BattleBackend.GameServers.First();
		}

		public void StartGame(List<BattleClient> clients, ulong objectId)
		{
			Logger.MethodTrace($"objectId: {objectId}");
			var owner = this.Owner.Account.GameAccount.CurrentToon.DbToon;

			if (Program.BattleBackend.GameServers.Count == 0) return;

			Program.BattleBackend.CreateGame(
				this.GServer.Key,
				(int)this.DynamicId,
				owner.Level,
				owner.CurrentAct,
				owner.CurrentDifficulty,
				owner.CurrentQuestId,
				owner.CurrentQuestStepId,
				owner.isHardcore,
				this.GameCreateParams.GameType,
				owner.isSeasoned
			);

			foreach (var client in clients)
			{
				client.MapLocalObjectId(this.DynamicId, objectId);
				this.SendConnectionInfo(client);
				client.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(0).SetStatus(0).Build();
				client.Account.GameAccount.NotifyUpdate();
			}

			this.Started = true;

		}

		public void JoinGame(List<BattleClient> clients, ulong objectId)
		{
			foreach (var client in clients) 
			{
				client.MapLocalObjectId(this.DynamicId, objectId); 
				this.SendConnectionInfo(client);
				client.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(1).SetStatus(1).Build();
				client.Account.GameAccount.NotifyUpdate();
			}
		}

		public bgs.protocol.games.v2.ConnectInfo GetConnectionInfoForClient(BattleClient client)
		{
			return bgs.protocol.games.v2.ConnectInfo.CreateBuilder()
				.SetAddress(bgs.protocol.Address.CreateBuilder().SetAddress_(this.GServer.Value.GameIp).SetPort((uint)this.GServer.Value.GamePort))
				.SetToken(Google.ProtocolBuffers.ByteString.CopyFrom(new byte[] { 0x31, 0x33, 0x38, 0x38, 0x35, 0x34, 0x33, 0x33, 0x32, 0x30, 0x38, 0x34, 0x30, 0x30, 0x38, 0x38, 0x35, 0x37, 0x39, 0x36 }))
				.AddAttribute(bgs.protocol.Attribute.CreateBuilder().SetName("Token").SetValue(bgs.protocol.Variant.CreateBuilder().SetUintValue(0xee34d06ffe821c43L)))

				.AddAttribute(bgs.protocol.Attribute.CreateBuilder()
					.SetName("SGameId").SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue((long)this.DynamicId).Build()))
				.AddAttribute(bgs.protocol.Attribute.CreateBuilder()
					.SetName("GameCreateParams").SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(this.GameCreateParams.ToByteString()).Build()))
				.Build();
		}

		private void SendConnectionInfo(BattleClient client)
		{
			if (client.CurrentChannel != null)
			{
				var channelStatePrivacyLevel = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
				   .SetPrivacyLevel(bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN).Build();

				var notificationPrivacyLevel = bgs.protocol.channel.v1.UpdateChannelStateNotification.CreateBuilder()
					.SetAgentId(client.Account.GameAccount.BnetEntityId)
					.SetStateChange(channelStatePrivacyLevel)
					.Build();

				var altPrivacyLevel = bgs.protocol.channel.v1.JoinNotification.CreateBuilder()
					.SetChannelState(channelStatePrivacyLevel)
					.Build();

				client.MakeTargetedRpc(client.CurrentChannel, (lid) =>
				   bgs.protocol.channel.v1.ChannelListener.CreateStub(client).OnUpdateChannelState(new HandlerController() { ListenerId = lid }, notificationPrivacyLevel, callback => { }));
		

				var channelStatePartyLock = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
					.AddAttribute(bgs.protocol.Attribute.CreateBuilder()
					.SetName("D3.Party.LockReasons")
					.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(0).Build())
					.Build()).Build();
				

				var notificationPartyLock = bgs.protocol.channel.v1.UpdateChannelStateNotification.CreateBuilder()
					.SetAgentId(client.Account.GameAccount.BnetEntityId)
					.SetStateChange(channelStatePartyLock)
					.Build();

				var altPartyLock = bgs.protocol.channel.v1.JoinNotification.CreateBuilder()
					.SetChannelState(channelStatePartyLock)
					.Build();

				client.MakeTargetedRpc(client.CurrentChannel, (lid) =>
					bgs.protocol.channel.v1.ChannelListener.CreateStub(client).OnUpdateChannelState(new HandlerController() { ListenerId = lid }, notificationPartyLock, callback => { }));
			}
		}
	}
}
