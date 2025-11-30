using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.CommandManager;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.ChannelSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
	[Service(serviceID: 0x10, serviceName: "bnet.protocol.channel.Channel")]
	public class ChannelService : bgs.protocol.channel.v1.ChannelService, IServerService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		
		public override void Dissolve(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.DissolveRequest request, Action<bgs.protocol.NoData> done)
		{
			throw new NotImplementedException();
		}

		public override void RemoveMember(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.RemoveMemberRequest request, Action<bgs.protocol.NoData> done)
		{
			Logger.MethodTrace("");
			var channel = ChannelManager.GetChannelByDynamicId((((HandlerController) controller).LastCallHeader).ObjectId);
			var gameAccount = GameAccountManager.GetAccountByPersistentID(request.MemberId.Low);

			var builder = bgs.protocol.NoData.CreateBuilder();
			done(builder.Build());

			channel.RemoveMember(gameAccount.LoggedInClient, Channel.GetRemoveReasonForRequest((Channel.RemoveRequestReason)request.Reason));
			if (request.Reason == 0)
			{
				ulong invId = ChannelInvitationManager.FindInvAsForClient(((HandlerController) controller).Client);
				if (invId != UInt64.MaxValue)
					ChannelInvitationManager.AltConnectToJoin(((HandlerController) controller).Client, bgs.protocol.channel.v1.AcceptInvitationRequest.CreateBuilder().SetInvitationId(invId).SetObjectId(0).Build());
				//ServicesSystem.Services.ChannelInvitationService.CreateStub(((HandlerController) controller).Client).AcceptInvitation(controller, bgs.protocol.channel.v1.AcceptInvitationRequest.CreateBuilder().SetInvitationId(invId).SetObjectId(0).Build(), callback => { });
			}
		}

		public override void SendMessage(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.SendMessageRequest request, Action<bgs.protocol.NoData> done)
		{
			var channel = ChannelManager.GetChannelByDynamicId((((HandlerController) controller).LastCallHeader).ObjectId);
			//Logger.Trace("{0} sent a message to channel {1}.", (((HandlerController) controller).Client).Account.GameAccount.CurrentToon, channel);

			var builder = bgs.protocol.NoData.CreateBuilder();
			done(builder.Build());

			if (!request.HasMessage)
				return; // only continue if the request actually contains a message.

			if (request.Message.AttributeCount == 0 || !request.Message.AttributeList.First().HasValue)
				return; // check if it has attributes.

			var parsedAsCommand = CommandManager.TryParse(request.Message.AttributeList[0].Value.StringValue, (((HandlerController) controller).Client)); // try parsing the message as a command

			if (!parsedAsCommand)
				channel.SendMessage((((HandlerController) controller).Client), request.Message); // if it's not parsed as an command - let channel itself to broadcast message to it's members.			  
		}

		public override void UpdateChannelState(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.UpdateChannelStateRequest request, Action<bgs.protocol.NoData> done)
		{
			/*
			if (this._loggedInClient.CurrentChannel != channel)
			{
				var request = bgs.protocol.channel.v1.AcceptInvitationRequest.CreateBuilder().SetInvitationId(channel.BnetEntityId.Low);

				ServicesSystem.Services.ChannelInvitationService.CreateStub(this.LoggedInClient).AcceptInvitation(null, request.Build(), callback => { });
			}
			//*/
			Channel channel = ChannelManager.GetChannelByDynamicId((((HandlerController) controller).LastCallHeader).ObjectId);
			
			Logger.Debug($"Agent ID: {(request.HasAgentId ? request.AgentId.ToString() : "N/A")}, gas state change: {(request.HasStateChange ? request.StateChange.ToString() : "N/A")}");

			foreach (bgs.protocol.Attribute attribute in request.StateChange.AttributeList)
			{
				if (attribute.Name == "D3.Party.GameCreateParams")
				{
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var gameCreateParams = D3.OnlineService.GameCreateParams.ParseFrom(attribute.Value.MessageValue);
						Logger.Debug("$[underline]$D3.Party.GameCreateParams:$[/]$ {0}", gameCreateParams.ToString());
						//D3.OnlineService.EntityId hero = gameCreateParams.Coop.ResumeFromSaveHeroId;
						bool clear_quests = (((HandlerController) controller).Client.GameChannel != null && gameCreateParams.CampaignOrAdventureMode.QuestStepId == -1 &&
								(gameCreateParams.CampaignOrAdventureMode.SnoQuest == 87700 ||
								gameCreateParams.CampaignOrAdventureMode.SnoQuest == 80322 ||
								gameCreateParams.CampaignOrAdventureMode.SnoQuest == 93595 ||
								gameCreateParams.CampaignOrAdventureMode.SnoQuest == 112498 ||
								gameCreateParams.CampaignOrAdventureMode.SnoQuest == 251355));
						var paramsBuilder = D3.OnlineService.GameCreateParams.CreateBuilder(gameCreateParams);
						var Mode = D3.OnlineService.CampaignOrAdventureModeCreateParams.CreateBuilder(gameCreateParams.CampaignOrAdventureMode);

						lock (((HandlerController) controller).Client.Account.GameAccount.CurrentToon.DbToon)
						{
							DBToon toonByClient = (((HandlerController) controller).Client).Account.GameAccount.CurrentToon.DbToon;
							if(toonByClient.CurrentAct == 400)
								toonByClient.CurrentAct = gameCreateParams.CampaignOrAdventureMode.Act;
							if (!clear_quests)
							{
								if (toonByClient.CurrentQuestId == 251355)
								{
									toonByClient.CurrentQuestId = (gameCreateParams.CampaignOrAdventureMode.SnoQuest == 0 ? 87700 : gameCreateParams.CampaignOrAdventureMode.SnoQuest);
									toonByClient.CurrentQuestStepId = (gameCreateParams.CampaignOrAdventureMode.QuestStepId == 0 ? -1 : gameCreateParams.CampaignOrAdventureMode.QuestStepId);
								}
								else 
								{
									Mode.SetAct(toonByClient.CurrentAct)
										.SetSnoQuest(toonByClient.CurrentQuestId)
										.SetQuestStepId(toonByClient.CurrentQuestStepId);
								}
							}
							
							toonByClient.CurrentDifficulty = gameCreateParams.CampaignOrAdventureMode.HandicapLevel;
							DBSessions.SessionUpdate(toonByClient);
						}
						paramsBuilder.SetCampaignOrAdventureMode(Mode);

						
						//paramsBuilder.SetGameType(16);
						//paramsBuilder.SetCreationFlags(0xFFFFFFFF);
						//paramsBuilder.ClearCoop();
						//paramsBuilder.SetPvp(D3.OnlineService.PvPCreateParams.CreateBuilder().SetSnoWorld(79100));

						/*var toon = (((HandlerController) controller).Client).Account.GameAccount.CurrentToon.DBToon;
						paramsBuilder.SetCoop(D3.OnlineService.CoopCreateParams.CreateBuilder()
							.SetDifficultyLevel(toon.CurrentDifficulty)
							.SetAct(toon.CurrentAct)
							.SetSnoQuest(toon.CurrentQuestId)
							.SetQuestStepId(toon.CurrentQuestStepId)
							.SetOpenToFriends(true)
							.SetOpenToFriendsMessage("TestGame")
							);
						*/

						gameCreateParams = paramsBuilder.Build(); //some magic

						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.GameCreateParams")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(gameCreateParams.ToByteString()).Build()).Build();

						channel.AddAttribute(attr);

					}
					else
					{
						var gameCreateParamsBuilder = D3.OnlineService.GameCreateParams.CreateBuilder();
						var toon = (((HandlerController) controller).Client).Account.GameAccount.CurrentToon;
						var dbToon = (((HandlerController) controller).Client).Account.GameAccount.CurrentToon.DbToon;
						gameCreateParamsBuilder.SetGameType(1);
						gameCreateParamsBuilder.SetCreationFlags(0);
						gameCreateParamsBuilder.SetCampaignOrAdventureMode(D3.OnlineService.CampaignOrAdventureModeCreateParams.CreateBuilder()
							.SetHandicapLevel(dbToon.CurrentDifficulty)
							.SetAct(dbToon.CurrentAct)
							.SetSnoQuest(dbToon.CurrentQuestId)
							.SetQuestStepId(dbToon.CurrentQuestStepId)
							.SetResumeFromSaveHeroId(toon.D3EntityId)
							.SetDeprecatedOpenToFriends(true)
							.SetDeprecatedOpenToFriendsMessage("TestGame")
							);
						gameCreateParamsBuilder.SetName(dbToon.Name);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.GameCreateParams")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(gameCreateParamsBuilder.Build().ToByteString()).Build());

						channel.AddAttribute(attr.Build());
					}
				}
				else if (attribute.Name == "D3.Party.SearchForPublicGame.Params")
				{
					// TODO: Find a game that fits the clients params and join /raist.
					var publicGameParams = D3.PartyMessage.SearchForPublicGameParams.ParseFrom(attribute.Value.MessageValue);
					Logger.Debug("$[underline]$SearchForPublicGameParams:$[/]$ {0}", publicGameParams.ToString());
					var attr = bgs.protocol.Attribute.CreateBuilder()
						.SetName("D3.Party.SearchForPublicGame.Params")
						.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(publicGameParams.ToByteString()).Build());

					channel.AddAttribute(attr.Build());
				}
				else if (attribute.Name == "D3.Party.ScreenStatus")
				{
					if (!attribute.HasValue || attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.ScreenStatus")
							.SetValue(bgs.protocol.Variant.CreateBuilder());

						channel.AddAttribute(attr.Build());
						Logger.Debug("$[underline]$D3.Party.ScreenStatus$[/]$ is null");
					}
					else
					{
						var oldScreen = D3.PartyMessage.ScreenStatus.ParseFrom(attribute.Value.MessageValue);
						(((HandlerController) controller).Client).Account.GameAccount.ScreenStatus = oldScreen;

						// TODO: save screen status for use with friends -Egris
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.ScreenStatus")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(oldScreen.ToByteString()));

						channel.AddAttribute(attr.Build());
						Logger.Debug("Client moving to Screen: {0}, with Status: {1}", oldScreen.Screen, oldScreen.Status);
					}
				}
				else if (attribute.Name == "D3.Party.JoinPermissionPreviousToLock")
				{
					// 0 - CLOSED
					// 1 - ASK_TO_JOIN

					var joinPermission = attribute.Value;
					var attr = bgs.protocol.Attribute.CreateBuilder()
						.SetName("D3.Party.JoinPermissionPreviousToLock")
						.SetValue(joinPermission);

					channel.AddAttribute(attr.Build());
					Logger.Debug("$[underline]$D3.Party.JoinPermissionPreviousToLock$[/]$ = {0}", joinPermission.IntValue);
				}
				else if (attribute.Name == "D3.Party.JoinPermissionPreviousToClose")
				{
					// 0 - CLOSED
					// 1 - ASK_TO_JOIN

					var joinPermission = attribute.Value;
					var attr = bgs.protocol.Attribute.CreateBuilder()
						.SetName("D3.Party.JoinPermissionPreviousToClose")
						.SetValue(joinPermission);

					channel.AddAttribute(attr.Build());
					Logger.Debug("$[underline]$D3.Party.JoinPermissionPreviousToClose$[/]$ = {0}", joinPermission.IntValue);
				}
				else if (attribute.Name == "D3.Party.LockReasons")
				{
					// 0 - CREATING_GAME
					// 2 - MATCHMAKER_SEARCHING

					var lockReason = attribute.Value;
					var attr = bgs.protocol.Attribute.CreateBuilder()
						.SetName("D3.Party.LockReasons")
						.SetValue(lockReason);

					channel.AddAttribute(attr.Build());
					Logger.Debug("$[underline]$D3.Party.LockReasons$[/]$ = {0}", lockReason.IntValue);
				}
				else if (attribute.Name == "D3.Party.GameId")
				{
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var gameId = D3.OnlineService.GameId.ParseFrom(attribute.Value.MessageValue);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.GameId")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(gameId.ToByteString()).Build());

						channel.AddAttribute(attr.Build());
						Logger.Debug("$[underline]$D3.Party.GameId$[/]$ = {0}", gameId.GameInstanceId);
					}
					else
						Logger.Debug("$[underline]$D3.Party.GameId$[/]$ is null");

				}
				else if (attribute.Name == "D3.Party.EnterGame.Members")
				{
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var members = D3.PartyMessage.EnterGamePartyMemberList.ParseFrom(attribute.Value.MessageValue);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.EnterGame.Members")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(members.ToByteString()).Build());

						channel.AddAttribute(attr.Build());
						Logger.Debug("$[underline]$D3.Party.EnterGame.Members$[/]$ = {0}", members.ToString());
					}
					else
						Logger.Debug("$[underline]$D3.Party.EnterGame.Members$[/]$ is null");

				}
				else if (attribute.Name == "D3.Party.JoinPermission")
				{
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var permission = D3.PartyMessage.EnterGamePartyMemberList.ParseFrom(attribute.Value.MessageValue);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.JoinPermission")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(permission.ToByteString()).Build());

						channel.AddAttribute(attr.Build());
						Logger.Debug("$[underline]$D3.Party.JoinPermission$[/]$ = {0}", permission.ToString());
					}
					else
						Logger.Debug("$[underline]$D3.Party.JoinPermission$[/]$ is null");

				}
				else if (attribute.Name == "D3.Party.EnterGame.Leader.AtQueueStart")
				{
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var queueStart = D3.PartyMessage.EnterGamePartyMemberList.ParseFrom(attribute.Value.MessageValue);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.Party.EnterGame.Leader.AtQueueStart")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(queueStart.ToByteString()).Build());

						channel.AddAttribute(attr.Build());
						Logger.Debug("$[underline]$D3.Party.EnterGame.Leader.AtQueueStart$[/]$ = {0}", queueStart.ToString());
					}
					else
						Logger.Debug("$[underline]$D3.Party.EnterGame.Leader.AtQueueStart$[/]$ = null");

				}
				else
				{
					Logger.MethodTrace($"Unknown attribute: {attribute.Name}");
				}
			}

			if (request.StateChange.HasPrivacyLevel)
				channel.PrivacyLevel = request.StateChange.PrivacyLevel;

			var builder = bgs.protocol.NoData.CreateBuilder();
			done(builder.Build());

			var notification = bgs.protocol.channel.v1.UpdateChannelStateNotification.CreateBuilder()
				.SetAgentId((((HandlerController) controller).Client).Account.GameAccount.BnetEntityId)
				/*
				.SetChannelId(bgs.protocol.channel.v1.ChannelId.CreateBuilder().SetId((uint)channel.BnetEntityId.Low))
				.SetSubscriber(bgs.protocol.account.v1.Identity.CreateBuilder()
					.SetAccount(bgs.protocol.account.v1.AccountId.CreateBuilder().SetId((uint)(((HandlerController) controller).Client).Account.BnetEntityId.Low))
					.SetGameAccount(bgs.protocol.account.v1.GameAccountHandle.CreateBuilder()
						.SetId((uint)(((HandlerController) controller).Client).Account.GameAccount.BnetEntityId.Low)
						.SetProgram(17459)
						.SetRegion(1))
					.SetProcess(bgs.protocol.ProcessId.CreateBuilder().SetLabel(0).SetEpoch(DateTime.Today.ToUnixTime())))
				//*/
				.SetStateChange(channel.State) //channelState
				.Build();

			var altnotif = bgs.protocol.channel.v1.JoinNotification.CreateBuilder().SetChannelState(channel.State).Build();

			var client = ((HandlerController) controller).Client;
			
			//Notify all Channel members
			foreach (var member in channel.Members.Keys)
			{
				member.MakeTargetedRpc(channel, (lid) => bgs.protocol.channel.v1.ChannelListener.CreateStub(member).OnUpdateChannelState(new HandlerController() { ListenerId = lid }, notification, callback => {  }));
			}
		}

		public override void UpdateMemberState(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.UpdateMemberStateRequest request, Action<bgs.protocol.NoData> done)
		{
			var channel = ChannelManager.GetChannelByDynamicId((((HandlerController) controller).LastCallHeader).ObjectId);
			var builder = bgs.protocol.NoData.CreateBuilder();
			done(builder.Build());

			var channelMember = bgs.protocol.channel.v1.Member.CreateBuilder();
			var state = bgs.protocol.channel.v1.MemberState.CreateBuilder();
			foreach (bgs.protocol.Attribute attribute in request.GetStateChange(0).State.AttributeList)
			{
				if (attribute.Name == "D3.PartyMember.GameId")
					if (attribute.HasValue && !attribute.Value.MessageValue.IsEmpty) //Sometimes not present -Egris
					{
						var gameId = D3.OnlineService.GameId.ParseFrom(attribute.Value.MessageValue);
						var attr = bgs.protocol.Attribute.CreateBuilder()
							.SetName("D3.PartyMember.GameId")
							.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(gameId.ToByteString()).Build());
						state.AddAttribute(attr);
						Logger.Debug("$[underline]$D3.PartyMember.GameId$[/]$ = {0}", gameId.GameInstanceId);
					}
					else
					{
						Logger.Debug("$[underline]$D3.PartyMember.GameId$[/]$ is null");
						channel.RemoveMember(((HandlerController) controller).Client, Channel.GetRemoveReasonForRequest((Channel.RemoveRequestReason)2));
					}
			}

			if (request.GetStateChange(0).State.RoleCount > 0)
			{
				state.AddRangeRole(request.GetStateChange(0).State.RoleList);
			}
			channelMember.SetIdentity(request.GetStateChange(0).Identity);
			channelMember.SetState(state);

			var notification = bgs.protocol.channel.v1.UpdateMemberStateNotification.CreateBuilder()
				.SetAgentId((((HandlerController) controller).Client).Account.GameAccount.BnetEntityId)
				.AddStateChange(channelMember)
				.Build();

			try
			{
				if (request.GetStateChange(0).State.RoleCount > 0 && request.GetStateChange(0).State.GetRole(0) == 2)
				{
					channel.SetOwner(GameAccountManager.GetAccountByPersistentID(request.GetStateChange(0).Identity.GameAccountId.Low).LoggedInClient);
				}
			}
			catch { }

			//Notify all Channel members
			foreach (var member in channel.Members.Keys)
			{
				member.MakeTargetedRpc(channel, (lid) =>
					bgs.protocol.channel.v1.ChannelListener.CreateStub(member).OnUpdateMemberState(new HandlerController() { ListenerId = lid }, notification, callback => { }));
			}
			//*/
		}

	}
}
