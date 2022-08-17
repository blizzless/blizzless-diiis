//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GuildSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.LoginServer.ChannelSystem
{
	public class Channel : RPCObject
	{
		/// <summary>
		/// D3.OnlineService.EntityId encoded channel Id.
		/// </summary>
		public D3.OnlineService.EntityId D3EntityId { get; protected set; }

		/// <summary>
		/// Channel PrivacyLevel.
		/// </summary>
		public bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel PrivacyLevel { get; set; }

		public Dictionary<string, bgs.protocol.Attribute> Attributes = new Dictionary<string, bgs.protocol.Attribute>();

		/// <summary>
		/// Max number of members.
		/// </summary>
		public uint MaxMembers { get; set; }

		/// <summary>
		/// Minimum number of members.
		/// </summary>
		public uint MinMembers { get; set; }

		/// <summary>
		/// BucketIndex of channel(only for chat)
		/// </summary>
		public uint BucketIndex { get; set; }

		/// <summary>
		/// Name of the channel(for public chats).
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Maximum invitations.
		/// </summary>
		public uint MaxInvitations { get; set; }

		/// <summary>
		/// List of channel members.
		/// </summary>
		public readonly ConcurrentDictionary<BattleClient, Member> Members = new ConcurrentDictionary<BattleClient, Member>();

		public readonly Dictionary<ulong, bgs.protocol.Invitation> Invitations = new Dictionary<ulong, bgs.protocol.Invitation>();

		/// <summary>
		/// Channel owner.
		/// </summary>
		public BattleClient Owner { get; protected set; }

		public bool IsGameChannel { get; set; }

		public bool IsChatChannel { get; set; }

		public bool IsGuildChannel { get; set; }
		public bool IsGuildChatChannel { get; set; }

		public Guild Guild { get; set; }

		/// <summary>
		/// Creates a new channel for given client with supplied remote object-id.
		/// </summary>
		/// <param name="client">The client channels is created for</param>
		/// <param name="remoteObjectId">The remove object-id of the client.</param>
		public Channel(BattleClient client, bool isGameChannel = false, ulong remoteObjectId = 0, bool isChatChannel = false)
		{
			this.BnetEntityId = bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.ChannelId).SetLow(0x0000000100000000L + this.DynamicId).Build();
			this.D3EntityId = D3.OnlineService.EntityId.CreateBuilder().SetIdHigh((ulong)EntityIdHelper.HighIdType.ChannelId).SetIdLow(this.DynamicId).Build();
			this.PrivacyLevel = isChatChannel ? bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN : bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN_INVITATION;
			this.MinMembers = isChatChannel ? 0U : 1U;
			this.MaxMembers = isChatChannel ? 100U : 4U;
			this.Name = "";
			this.MaxInvitations = 12000;
			this.IsGameChannel = isGameChannel;
			this.IsChatChannel = isChatChannel;
			this.IsGuildChannel = false;
			this.IsGuildChatChannel = false;

			if ((client != null) && (remoteObjectId != 0))
				client.MapLocalObjectID(this.DynamicId, remoteObjectId);

			if (this.IsChatChannel)
				Program.Watchdog.AddTask(10, new Action(() =>
				{
					if (this == null || this._dissolved) throw new Exception("Channel is null");
					foreach (var member in this.Members.Keys.ToArray())
						if (member.Account == null || !member.Account.IsOnline)
							Members.TryRemove(member, out _);
				}));

		}

		#region common methods

		public bool HasUser(BattleClient client)
		{
			return this.Members.Any(pair => pair.Key == client);
		}

		public bool HasMember(GameAccount gameAccount) //check if a given game account is already channels member
		{
			return this.Members.Any(pair => pair.Value.Identity.AccountId.Low == gameAccount.BnetEntityId.Low);
		}

		public Member GetMember(BattleClient client)
		{
			return this.Members[client];
		}

		private bool _dissolved = false;
		public void Dissolve()
		{
			_dissolved = true;
			ChannelManager.DissolveChannel(this.DynamicId);
		}

		#endregion

		#region owner functionality

		public void SetOwner(BattleClient client)
		{
			if (client == this.Owner)
				return;
			this.Owner = client;
		}

		public void RemoveOwner()
		{
			if (this.Owner == null) return;

			this.NotifyRoles();

			var owner = this.Members.Keys.FirstOrDefault(c => c != this.Owner && c.SocketConnection != null);

			if (owner != null)
			{
				this.SetOwner(owner);
				this.NotifyRoles();
			}
			else
				this.Owner = null;
		}

		public void NotifyRole(BattleClient client)
		{
			//if (this.Members.Count <= 1) return;
			var channelMember = bgs.protocol.channel.v1.Member.CreateBuilder();
			var state = bgs.protocol.channel.v1.MemberState.CreateBuilder();

			state.AddRole((uint)(client == this.Owner ? 2 : 0));

			var identity = bgs.protocol.Identity.CreateBuilder().SetGameAccountId(client.Account.GameAccount.BnetEntityId);

			channelMember.SetIdentity(identity);
			channelMember.SetState(state);

			var notification = bgs.protocol.channel.v1.UpdateMemberStateNotification.CreateBuilder()
				.AddStateChange(channelMember)
				.Build();

			client.MakeTargetedRPC(this, (lid) =>
				bgs.protocol.channel.v1.ChannelListener.CreateStub(client).OnUpdateMemberState(new HandlerController() { ListenerId = lid } , notification, callback => { }));
		}

		public void NotifyRoles()
		{
			if (this.Members.Count <= 1) return;
			foreach (var member in this.Members.Keys)
			{
				var channelMember = bgs.protocol.channel.v1.Member.CreateBuilder();
				var state = bgs.protocol.channel.v1.MemberState.CreateBuilder();

				state.AddRole((uint)(member == this.Owner ? 2 : 0));

				var identity = bgs.protocol.Identity.CreateBuilder().SetGameAccountId(member.Account.GameAccount.BnetEntityId);

				channelMember.SetIdentity(identity);
				channelMember.SetState(state);

				var notification = bgs.protocol.channel.v1.UpdateMemberStateNotification.CreateBuilder()
					.AddStateChange(channelMember)
					.Build();

				//Notify all Channel members
				foreach (var n_member in this.Members.Keys)
				{
					n_member.MakeTargetedRPC(this, (lid) =>
						bgs.protocol.channel.v1.ChannelListener.CreateStub(n_member).OnUpdateMemberState(new HandlerController() { ListenerId = lid }, notification, callback => { }));
				}
			}
		}

		#endregion

		#region member functinality

		public void Join(BattleClient client, ulong remoteObjectId)
		{
			client.MapLocalObjectID(this.DynamicId, remoteObjectId);
			this.AddMember(client);
		}

		public void AddMember(BattleClient client)
		{
			if (HasUser(client))
			{
				Logger.Warn("Attempted to add client {0} to channel when it was already a member of the channel", client.SocketConnection.RemoteAddress.ToString());
				return;
			}

			bool isOwner = client == this.Owner;

			// Cache the built state and member
			var channelState = this.State.ToBuilder();

			var addedMember = new Member(this, client.Account.GameAccount, isOwner ? Member.Privilege.Creator : Member.Privilege.JoinedMember);
			addedMember.AddRole((isOwner) ? Member.Role.ChannelCreator : Member.Role.ChannelMember);

			this.Members.TryAdd(client, addedMember);

			var members = this.Members.Select(member => member.Value.BnetMember).ToList();

			if (this.IsGuildChannel)
				members = this.Guild.Members.Keys.ToList().Select(account => new Member(this, account, Member.Privilege.JoinedMember).BnetMember).ToList();
			var joinNotification = bgs.protocol.channel.v1.JoinNotification.CreateBuilder()
				.SetChannelState(channelState.Build())
				.SetChannelId(bgs.protocol.channel.v1.ChannelId.CreateBuilder()
				.SetId((uint)this.D3EntityId.IdLow)
				.SetType(1)
				.SetHost(bgs.protocol.ProcessId.CreateBuilder().SetLabel(4041445648).SetEpoch(DateTime.Today.ToUnixTime())))
				.SetSelf(addedMember.BnetMember)
				.AddRangeMember(members)
				.Build();

			client.MakeTargetedRPC(this, (lid) =>
				bgs.protocol.channel.v1.ChannelListener.CreateStub(client).OnJoin(new HandlerController() { ListenerId = lid }, joinNotification, callback => { }));


			//client.SendServerWhisper("!PvpDisable");
			if (!this.IsChatChannel)
			{
				if (this.IsGameChannel)
					client.GameChannel = this;
				else
					client.PartyChannel = this;

				client.CurrentChannel = this;
				if (this.Members.Count < 2) return;
			}
			
			var addNotification = bgs.protocol.channel.v1.MemberAddedNotification.CreateBuilder()
				.SetMember(addedMember.BnetMember).SetChannelId(bgs.protocol.channel.v1.ChannelId.CreateBuilder()
				.SetId((uint)this.D3EntityId.IdLow)
				.SetType(1)
				.SetHost(bgs.protocol.ProcessId.CreateBuilder().SetLabel(4041445648).SetEpoch(DateTime.Today.ToUnixTime()))
				//.SetChannelState(channelState.Build())
				//.SetSelf(addedMember.BnetMember)
				).Build();

			foreach (var pair in this.Members.Where(pair => pair.Value != addedMember)) // only send this to previous members of the channel.
			{
				pair.Key.MakeTargetedRPC(this, (lid) =>
					bgs.protocol.channel.v1.ChannelListener.CreateStub(pair.Key).OnMemberAdded(new HandlerController() { ListenerId = lid }, addNotification, callback => { }));
			}

			if (!this.IsChatChannel) this.NotifyRole(client);
		}

		public void RemoveAllMembers()
		{
			if (!_dissolved)
			{
				Dissolve();
				return;
			}

			foreach (var member in this.Members)
			{
				RemoveMember(member.Key, RemoveReason.Dissolved);
				System.Threading.Tasks.Task.Delay(100).Wait();
			}
		}

		public void RemoveMemberByID(bgs.protocol.EntityId memberId, RemoveReason reason)
		{
			var client = this.Members.FirstOrDefault(pair => pair.Value.Identity.AccountId == memberId).Key;
			RemoveMember(client, reason);
		}

		public void RemoveMember(BattleClient client, RemoveReason reason, bool wentOffline = false)
		{
			if (client == null || client.Account == null || client.Account.GameAccount == null)
			{
				if (client != null)
					Logger.Warn("Could not remove client {0} from channel {1}.", client.SocketConnection.RemoteAddress.ToString(), this.ToString());
				return;
			}
			else if (!HasUser(client))
			{
				Logger.Warn("Attempted to remove non-member client {0} from channel {1}.", client.SocketConnection.RemoteAddress.ToString(), this.ToString());
				return;
			}
			else if (!client.Channels.ContainsValue(this) && !client.ChatChannels.Contains(this))
			{
				Logger.Warn("Client {0} being removed from a channel ({1}) he's not associated with.", client.SocketConnection.RemoteAddress.ToString(), this.ToString());
			}

			lock (this.Members)
			{
				var memberId = this.Members[client].Identity.GameAccountId;
				var leaveMessage = bgs.protocol.channel.v1.LeaveNotification.CreateBuilder()
					//.SetAgentId(memberId)
					//.SetChannelId(bgs.protocol.channel.v1.ChannelId.CreateBuilder() .SetId((uint)this.D3EntityId.IdLow) .SetType(1) .SetHost(bgs.protocol.ProcessId.CreateBuilder().SetLabel(4041445648).SetEpoch(DateTime.Today.ToUnixTime())))
					.SetMemberId(memberId)
					//.SetReason((uint)reason)
					.Build();

				var removeMessage = bgs.protocol.channel.v1.MemberRemovedNotification.CreateBuilder()
					.SetAgentId(memberId)
					.SetMemberId(memberId)
					.SetReason((uint)reason).Build();

				if (this.Members.Count <= 2 && !_dissolved && !this.IsChatChannel)
				{
					Dissolve();
					return;
				}
				if (client == this.Owner)
					this.RemoveOwner();

				foreach (var pair in this.Members)
				{
					if (pair.Key != client)
						pair.Key.MakeTargetedRPC(this, (lid) =>
							bgs.protocol.channel.v1.ChannelListener.CreateStub(pair.Key).OnMemberRemoved(new HandlerController() { ListenerId = lid }, removeMessage, callback => { }));
				}
				client.MakeTargetedRPC(this, (lid) => bgs.protocol.channel.v1.ChannelListener.CreateStub(client).OnLeave(new HandlerController() { ListenerId = lid }, leaveMessage, callback => 
					{ 
						client.UnmapLocalObjectId(this.DynamicId); }));

				this.Members.TryRemove(client, out _);
				//client.UnmapLocalObjectId(this.DynamicId);

				if (!this.IsChatChannel) client.CurrentChannel = null;
				if (this.IsGameChannel)
				{
					client.GameChannel = null;
					Logger.Warn("Client {0} left game channel {1}.", client, this);
				}
				else if (this.IsChatChannel)
				{
					client.ChatChannels.Remove(this);
					Logger.Warn("Client {0} left chat channel {1}.", client, this);
				}
				else
				{
					client.PartyChannel = null;
					Logger.Warn("Client {0} left party channel {1}.", client, this);
				}
			}
		}

		#endregion

		#region invitation functionality
		/*public void AddInvitation(bgs.protocol.Invitation invitation)
		{
			this.Invitations.Add(invitation.Id, invitation);
		}*/

		public void SetOpen()
		{
			this.PrivacyLevel = bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN;
		}

		/*public void RemoveInvitation(bgs.protocol.Invitation invitation)
		{
			if (this.Invitations.ContainsKey(invitation.Id))
			{
				this.Invitations.Remove(invitation.Id);
			}
			else
			{
				Logger.Warn("Tried to removed unmapped invitation {0} from channel {1}.", invitation.Id, this);
			}
		}*/

		#endregion

		#region channel-messaging

		public void SendMessage(BattleClient client, bgs.protocol.channel.v1.Message message)
		{
			if (this.Name != "" && !this.IsGuildChannel && client.Account.MuteTime > DateTime.Now.ToUnixTime())
			{
				client.SendServerWhisper(string.Format("Your have been muted in public chat channels by Moderator.\nRemained time: {0} s.", client.Account.MuteTime - DateTime.Now.ToUnixTime()));
				return;
			}
			GameServer.CommandManager.CommandManager.TryParse(message.AttributeList[0].Value.StringValue, client); // try parsing it as a command and respond it if so.

			var notification =
				bgs.protocol.channel.v1.SendMessageNotification.CreateBuilder()
					.SetAgentId(client.Account.GameAccount.BnetEntityId)
					.SetBattleTag(client.Account.BattleTag)
					.SetRequiredPrivileges(0)
					.SetMessage(message)
					.Build();

			if (this.Name == "")
				Logger.ChatMessage("[Group][{0}]: {1}", client.Account.BattleTagName, message.AttributeList[0].Value.StringValue);
			else
				Logger.ChatMessage("[Public][{0}#{1}][{2}]: {3}", this.Name, this.BucketIndex, client.Account.BattleTagName, message.AttributeList[0].Value.StringValue);

			foreach (var pair in this.Members) // send to all members of channel even to the actual one that sent the message else he'll not see his own message.
			{
				if (pair.Key.Account.IgnoreIds.Contains(client.Account.PersistentID)) continue;
				pair.Key.MakeTargetedRPC(this, (lid) =>
					bgs.protocol.channel.v1.ChannelListener.CreateStub(pair.Key).OnSendMessage(new HandlerController() { ListenerId = lid }, notification, callback => { }));
			}
		}

		#endregion

		#region channel state messages

		public void AddAttribute(bgs.protocol.Attribute attribute)
		{
			if (this.Attributes.ContainsKey(attribute.Name))
				this.Attributes[attribute.Name] = attribute;
			else
				this.Attributes.Add(attribute.Name, attribute);
		}

		public void ClearAttribute(bgs.protocol.Attribute attribute)
		{
			if (this.Attributes.ContainsKey(attribute.Name))
				this.Attributes.Remove(attribute.Name);
		}

		/// <summary>
		/// bgs.protocol.channel.v1.ChannelState message.
		/// </summary>
		public bgs.protocol.channel.v1.ChannelState State
		{
			get
			{
				if (this.IsGuildChannel)
					return this.IsGuildChatChannel ? this.Guild.GroupChatChannelState : this.Guild.ChannelState;
				var state = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
					.SetMinMembers(this.MinMembers)
					.SetMaxMembers(this.MaxMembers)
					.SetPrivacyLevel(this.PrivacyLevel)
					.SetName(this.Name)
					.SetChannelType("default")
					.SetProgram(17459)
					.SetSubscribeToPresence(true)
					//.SetName(string.Format("{0}#{1}", this.Name, this.DynamicId))
					;
				if (this.IsChatChannel)
				{
					var chatState = bgs.protocol.channel.v1.ChatChannelState.CreateBuilder().SetIdentity(string.Format("#{0}", this.DynamicId)).SetLocale(0x55527572).SetBucketIndex(this.BucketIndex).SetPublic(true).Build();
					state.SetExtension(bgs.protocol.channel.v1.ChatChannelState.ChannelState_, chatState);
				}

				foreach (var attr in this.Attributes.Values)
					state.AddAttribute(attr);

				return state.Build();
			}
		}

		/// <summary>
		/// bgs.protocol.channel.v1.ChannelDescription message.
		/// </summary>
		public bgs.protocol.channel.v1.ChannelDescription Description
		{
			get
			{
				var builder = bgs.protocol.channel.v1.ChannelDescription.CreateBuilder() // NOTE: Can have extensions
					.SetChannelId(this.BnetEntityId)
					.SetState(this.State);

				if (this.Members.Count > 0) // No reason to set a value that defaults to 0
					builder.SetCurrentMembers((uint)this.Members.Count);
				return builder.Build();
			}
		}

		/// <summary>
		/// bgs.protocol.channel.v1.ChannelInfo message.
		/// </summary>
		public bgs.protocol.channel.v1.ChannelInfo Info
		{
			get
			{
				var builder = bgs.protocol.channel.v1.ChannelInfo.CreateBuilder() // NOTE: Can have extensions
					.SetDescription(this.Description);

				foreach (var pair in this.Members)
				{
					builder.AddMember(pair.Value.BnetMember);
				}

				return builder.Build();
			}
		}

		#endregion

		#region remove-reason helpers

		// Reasons the client tries to remove a member - // TODO: Need more data to complete this		
		public enum RemoveRequestReason : uint
		{
			RequestedBySelf = 0x00,   // Default; generally when the client quits or leaves a channel (for example, when switching toons)
			Kicked = 0x01,
			Dissolved = 0x02,
			// Kick is probably 0x01 or somesuch
		}

		// Reasons a member was removed (sent in NotifyRemove)
		public enum RemoveReason : uint
		{
			Left = 0x00,
			Kicked = 0x01,         // The member was kicked
			Dissolved = 0x02           // The channel was dissolved
		}

		public static RemoveReason GetRemoveReasonForRequest(RemoveRequestReason reqreason)
		{
			switch (reqreason)
			{
				case RemoveRequestReason.RequestedBySelf:
					return RemoveReason.Left;
				case RemoveRequestReason.Kicked:
					return RemoveReason.Kicked;
				case RemoveRequestReason.Dissolved:
					return RemoveReason.Dissolved;
				default:
					Logger.Warn("No RemoveReason for given RemoveRequestReason: {0}", Enum.GetName(typeof(RemoveRequestReason), reqreason));
					break;
			}
			return RemoveReason.Left;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("{{ Channel: [id: {0}] [owner: {1}] }}", this.DynamicId, this.Owner != null ? this.Owner.Account.GameAccount.CurrentToon.ToString() : "N/A");
		}
	}
}
