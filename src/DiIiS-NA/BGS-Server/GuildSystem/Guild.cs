//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.ChannelSystem;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DateTime = System.DateTime;

namespace DiIiS_NA.LoginServer.GuildSystem
{
	public class Guild
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// D3.GameMessage.GuildId encoded guild Id.
		/// </summary>
		public D3.GameMessage.GuildId GuildId { get; protected set; }

		public ulong PersistentId { get; protected set; }

		public DBGuild DBGuild
		{
			get
			{
				return DBSessions.SessionGet<DBGuild>(this.PersistentId);
			}
		}

		/// <summary>
		/// Max number of members.
		/// </summary>
		public uint MaxMembers { get; set; }

		/// <summary>
		/// Name of the guild
		/// </summary>
		public string Name { get; set; }

		public string FullName
		{
			get
			{
				//return this.IsClan ? string.Format("{0} [{1}]", this.Name, this.ParagonRatings.Values.ToList().OrderByDescending(x => x).Take(12).Sum()) : this.Name;
				return this.Name;
			}
		}

		public uint Flags
		{
			get
			{
				var flags = GuildFlags.None;
				if (!this.Disbanded) flags |= GuildFlags.Enabled;
				if (this.IsLFM) flags |= GuildFlags.LookingForMembers;
				if (this.IsInviteRequired) flags |= GuildFlags.InviteOnly;
				return (uint)flags;
			}
		}

		/// <summary>
		/// Prefix of the guild (clans)
		/// </summary>
		private string _Prefix = "";

		public string Prefix
		{
			get
			{
				return this._Prefix;
			}
			set
			{
				this._Prefix = value;
			}
		}

		public string Description { get; set; }
		public string MOTD { get; set; }

		/// <summary>
		/// Category of the guild (clans are always 0)
		/// </summary>
		public uint Category { get; set; }
		public uint Language { get; set; }

		public Dictionary<uint, RankDescriptor> Ranks { get; set; }

		public byte[] BnetRanks
		{
			get
			{
				var builder = D3.Guild.RankList.CreateBuilder();
				foreach (var rank in this.Ranks)
				{
					builder.AddRanks(D3.Guild.Rank.CreateBuilder().SetRankId(rank.Key).SetRankOrder(rank.Key).SetName(rank.Value.RankName).SetPermissions(rank.Value.Privileges));
				}
				return builder.Build().ToByteString().ToByteArray();
			}
		}

		/// <summary>
		/// Guild channel (common one)
		/// </summary>
		public Channel Channel { get; set; }
		public Channel GroupChatChannel { get; set; }

		/// <summary>
		/// Dictionary of guild members + their ranks.
		/// </summary>
		public readonly Dictionary<GameAccount, DBGuildMember> Members = new Dictionary<GameAccount, DBGuildMember>();
		public List<D3.Guild.Invite> GuildSuggestions = new List<D3.Guild.Invite>();

		/// <summary>
		/// Guild owner.
		/// </summary>
		public GameAccount Owner { get; set; }

		public bool IsClan { get; set; }
		public bool IsLFM { get; set; }
		public bool IsInviteRequired { get; set; }
		public Dictionary<GameAccount, int> ParagonRatings { get; set; }
		public int RatingPoints { get; set; }

		public ulong NewsTime { get; set; }
		public ulong InviteTime { get; set; }

		public bool Disbanded { get; set; }

		/// <summary>
		/// Creates a new guild instance
		/// </summary>
		public Guild(DBGuild dbGuild)
		{
			this.PersistentId = dbGuild.Id;
			this.GuildId = D3.GameMessage.GuildId.CreateBuilder().SetGuildId_(dbGuild.Id).Build();
			this.IsClan = (dbGuild.Category == 0);
			this.IsLFM = dbGuild.IsLFM;
			this.IsInviteRequired = dbGuild.IsInviteRequired;
			this.Prefix = dbGuild.Tag;
			this.Name = dbGuild.Name;
			this.Description = dbGuild.Description;
			this.MOTD = dbGuild.MOTD;
			this.RatingPoints = dbGuild.Rating;
			var ranks = D3.Guild.RankList.ParseFrom(dbGuild.Ranks);
			this.Ranks = new Dictionary<uint, RankDescriptor>();
			foreach (var rank in ranks.RanksList)
				this.Ranks.Add(rank.RankId, new RankDescriptor() { RankName = rank.Name, Privileges = rank.Permissions });
			this.NewsTime = 0;
			this.InviteTime = 0;
			this.Owner = GameAccountManager.GetGameAccountByDBGameAccount(dbGuild.Creator);
			this.MaxMembers = this.IsClan ? (16U) : 100U;
			this.Category = (uint)dbGuild.Category;
			this.Language = (uint)dbGuild.Language;
			this.Disbanded = dbGuild.Disbanded;
			this.Channel = ChannelManager.CreateGuildChannel(this);
			this.GroupChatChannel = ChannelManager.CreateGuildGroupChannel(this);
			this.ParagonRatings = new Dictionary<GameAccount, int>();
		}

		public void Disband()
		{
			this.RemoveAllMembers();
			this.Disbanded = true;
			lock (this.DBGuild)
			{
				var dbGuild = this.DBGuild;
				dbGuild.Disbanded = true;
				DBSessions.SessionUpdate(dbGuild);
			}
		}

		public void ActivatePremium()
		{
			if (this.Ranks.Count < 4)
				this.Ranks.Add(3, new RankDescriptor() { RankName = "Advanced", Privileges = 212943U });
			lock (this.DBGuild)
			{
				var dbGuild = this.DBGuild;
				dbGuild.Ranks = this.BnetRanks;
				DBSessions.SessionUpdate(dbGuild);
			}
			this.UpdateChannelAttributes();
		}

		#region member functionality

		public bool HasMember(GameAccount account)
		{
			return this.Members.Keys.Any(a => a == account);
		}

		public void AddMember(GameAccount account)
		{
			if (HasMember(account))
			{
				Logger.Warn("Attempted to add account {0} to guild when it was already a member of the guild", account.Owner.BattleTag);
				return;
			}

			if (this.IsClan && account.Clan != null)
			{
				Logger.Warn("Attempted to add account {0} to clan when it was already a member of the another clan", account.Owner.BattleTag);
				return;
			}

			if (this.Members.Count + 1 > this.MaxMembers) return;

			var newDBGuildMember = new DBGuildMember
			{
				DBGuild = this.DBGuild,
				DBGameAccount = account.DBGameAccount,
				Note = "",
				Rank = (account.PersistentID == this.Owner.PersistentID ? 1 : 4)
			};
			DBSessions.SessionSave(newDBGuildMember);

			this.Members.Add(account, newDBGuildMember);
			if (this.IsClan)
			{
				this.ParagonRatings.Add(account, account.DBGameAccount.ParagonLevel);
				this.AddNews(account, 2);
			}
			this.NotifyChannels(account);
		}

		public void RemoveAllMembers()
		{
			List<GameAccount> _members = this.Members.Keys.ToList();
			foreach (var account in _members)
			{
				RemoveMember(account);
			}
		}

		public void RemoveMember(GameAccount account)
		{
			if (account == null) return;

			if (!HasMember(account))
			{
				Logger.Warn("Attempted to remove non-member account {0} from guild {1}#{2}", account.Owner.BattleTag, this.Name, this.PersistentId);
				return;
			}
			else if (account.Clan != this && !account.Communities.Contains(this))
			{
				Logger.Warn("Client {0} being removed from a guild ({1}#{2}) he's not associated with.", account.Owner.BattleTag, this.Name, this.PersistentId);
			}

			this.Members.Remove(account);
			this.Channel.RemoveMember(account.LoggedInClient, Channel.RemoveReason.Left);
			if (this.IsClan)
			{
				this.ParagonRatings.Remove(account);
				this.AddNews(account, 3);
			}
			var dbRow = DBSessions.SessionQuerySingle<DBGuildMember>(m => m.DBGuild.Id == this.PersistentId && m.DBGameAccount.Id == account.PersistentID);
			DBSessions.SessionDelete(dbRow);
		}

		public void AddSuggestion(GameAccount account, GameAccount inviter)
		{
			var invite = D3.Guild.Invite.CreateBuilder()
				.SetAccountId(account.PersistentID)
				.SetInviterId(inviter.PersistentID)
				.SetInviteTime(DateTime.Now.ToUnixTime())
				.SetInviteType(1)
				.SetExpireTime(3600);
			this.GuildSuggestions.Add(invite.Build());

			this.InviteTime = DateTime.Now.ToBlizzardEpoch();
			this.UpdateChannelAttributes();
		}

		public void RemoveSuggestion(GameAccount account)
		{
			this.GuildSuggestions.RemoveAll(s => s.AccountId == account.PersistentID);
		}

		public uint GetRank(ulong GameAccountId)
		{
			if (this.Members.Keys.Any(m => m.PersistentID == GameAccountId))
			{
				return (uint)this.Members.Single(m => m.Key.PersistentID == GameAccountId).Value.Rank;
			}
			else
			{
				Logger.Warn("Client {0} called GetRank() on a guild ({1}#{2}) he's not associated with.", GameAccountId, this.Name, this.PersistentId);
				return 0;
			}
		}

		public string GetMemberNote(ulong GameAccountId)
		{
			if (this.Members.Keys.Any(m => m.PersistentID == GameAccountId))
			{
				return this.Members.Single(m => m.Key.PersistentID == GameAccountId).Value.Note;
			}
			else
			{
				Logger.Warn("Client {0} called GetMemberNote() on a guild ({1}#{2}) he's not associated with.", GameAccountId, this.Name, this.PersistentId);
				return "";
			}
		}

		public void SetMemberNote(GameAccount account, string note)
		{
			if (!this.HasMember(account)) return;

			this.Members[account].Note = note;
			DBSessions.SessionUpdate(this.Members[account]);
			this.UpdateMemberAttributes(account);
		}

		#endregion

		#region ranks functionality

		public bool HasPermission(GameAccount account, GuildPrivilegeFlags permission)
		{
			if (!this.HasMember(account)) return false;

			var rank = (uint)this.Members[account].Rank;

			if (!this.Ranks.ContainsKey(rank)) return false;

			var permissions = (GuildPrivilegeFlags)this.Ranks[rank].Privileges;

			return permissions.HasFlag(permission);
		}

		public void SetPermissions(uint rank, string name, uint permissions)
		{
			if (!this.Ranks.ContainsKey(rank)) return;

			this.Ranks[rank].RankName = name;
			this.Ranks[rank].Privileges = permissions;

			lock (this.DBGuild)
			{
				var dbGuild = this.DBGuild;
				dbGuild.Ranks = this.BnetRanks;
				DBSessions.SessionUpdate(dbGuild);
			}
			this.UpdateChannelAttributes();
		}

		public void PromoteMember(GameAccount account, bool setLeader = false)
		{
			if (!this.HasMember(account)) return;

			var rank = (uint)this.Members[account].Rank;

			if (!this.Ranks.ContainsKey(rank)) return;
			if (rank <= 2 && !setLeader) return;

			this.Members[account].Rank--;
			if (this.Members[account].Rank == 3)
				this.Members[account].Rank--;

			DBSessions.SessionUpdate(this.Members[account]);
			this.UpdateMemberAttributes(account);
			if (setLeader)
				this.AddNews(account, 7);
			else
				this.AddNews(account, 4);
		}

		public void DemoteMember(GameAccount account)
		{
			if (!this.HasMember(account)) return;

			var rank = (uint)this.Members[account].Rank;

			if (!this.Ranks.ContainsKey(rank)) return;
			if (rank >= this.Ranks.Count) return;

			this.Members[account].Rank++;
			if (this.Members[account].Rank == 3)
				this.Members[account].Rank++;

			DBSessions.SessionUpdate(this.Members[account]);
			this.UpdateMemberAttributes(account);
		}

		#endregion

		#region guild channel

		/// <summary>
		/// bnet.protocol.channel.ChannelState message.
		/// </summary>
		public bgs.protocol.channel.v1.ChannelState ChannelState
		{
			get
			{
				var state = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
					.SetMinMembers(0)
					.SetMaxMembers(this.MaxMembers)
					.SetName(this.Name)
					.SetPrivacyLevel(bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN)
					.SetChannelType(this.IsClan ? "clan" : "group")
					.SetProgram(17459)
					.SetSubscribeToPresence(true);

				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.GuildId").SetValue(Variant.CreateBuilder().SetUintValue(this.PersistentId)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Name").SetValue(Variant.CreateBuilder().SetStringValue(this.FullName)));
				if (this.IsClan)
					state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Tag").SetValue(Variant.CreateBuilder().SetStringValue(this.Prefix)));
				else
				{
					state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.TotalMembers").SetValue(Variant.CreateBuilder().SetUintValue((ulong)this.Members.Count)));
					state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.TotalMembersInChat").SetValue(Variant.CreateBuilder().SetUintValue((ulong)this.GroupChatChannel.Members.Count)));
				}

				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Ranks").SetValue(Variant.CreateBuilder().SetMessageValue(ByteString.CopyFrom(this.BnetRanks))));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Motd").SetValue(Variant.CreateBuilder().SetStringValue(this.MOTD)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Description").SetValue(Variant.CreateBuilder().SetStringValue(this.Description)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Category").SetValue(Variant.CreateBuilder().SetUintValue(this.Category)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Creator").SetValue(Variant.CreateBuilder().SetUintValue(this.Owner.PersistentID)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Language").SetValue(Variant.CreateBuilder().SetUintValue(this.Language)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.InviteTime").SetValue(Variant.CreateBuilder().SetUintValue(this.InviteTime)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Flags").SetValue(Variant.CreateBuilder().SetUintValue(this.Flags)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.SearchCategory").SetValue(Variant.CreateBuilder().SetUintValue(this.Category)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.PostNewsTime").SetValue(Variant.CreateBuilder().SetUintValue(0)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.NewsTime").SetValue(Variant.CreateBuilder().SetUintValue(this.NewsTime)));

				return state.Build();
			}
		}

		public bgs.protocol.channel.v1.ChannelState GroupChatChannelState
		{
			get
			{
				var state = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
					.SetMinMembers(0)
					.SetMaxMembers(this.MaxMembers)
					.SetName(this.Name)
					.SetPrivacyLevel(bgs.protocol.channel.v1.ChannelState.Types.PrivacyLevel.PRIVACY_LEVEL_OPEN)
					.SetChannelType("group_chat")
					.SetProgram(17459)
					.SetSubscribeToPresence(true);

				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.GuildId").SetValue(Variant.CreateBuilder().SetUintValue(this.PersistentId)));
				state.AddAttribute(Attribute.CreateBuilder().SetName("D3.Guild.Name").SetValue(Variant.CreateBuilder().SetStringValue(this.Name)));


				return state.Build();
			}
		}

		public D3.Guild.GuildSummary Summary
		{
			get
			{
				var summary = D3.Guild.GuildSummary.CreateBuilder()
					.SetGuildId(this.PersistentId)
					.SetGuildName(this.Name)
					.SetGuildTag(this.Prefix)
					.SetGuildFlags(this.Flags);
				return summary.Build();
			}
		}

		public void NotifyChannels(GameAccount account)
		{
			var guildChannelId = bgs.protocol.Attribute.CreateBuilder()
				.SetName("channel_id")
				.SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(
					this.Channel.BnetEntityId
					).Build())
				.Build();

			var notificationBuilder = bgs.protocol.notification.v1.Notification.CreateBuilder()
				.SetTargetId(account.BnetEntityId)
				.SetType("P_SUBSCRIBE_TO_CHANNEL")
				.AddAttribute(guildChannelId)
				.SetSenderAccountId(account.Owner.BnetEntityId)
				.SetTargetAccountId(account.Owner.BnetEntityId)
				.Build();

			account.LoggedInClient.MakeRPC((lid) =>
				bgs.protocol.notification.v1.NotificationListener.CreateStub(account.LoggedInClient).OnNotificationReceived(new HandlerController() { ListenerId = lid 
				}, notificationBuilder, callback => { }));

			if (!this.IsClan)
			{
				var guildChatChannelId = bgs.protocol.Attribute.CreateBuilder()
					.SetName("channel_id")
					.SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(
						this.GroupChatChannel.BnetEntityId
						).Build())
					.Build();

				var chatNotificationBuilder = bgs.protocol.notification.v1.Notification.CreateBuilder()
					.SetTargetId(account.BnetEntityId)
					.SetType("P_SUBSCRIBE_TO_CHANNEL")
					.AddAttribute(guildChatChannelId)
					.SetSenderAccountId(account.Owner.BnetEntityId)
					.SetTargetAccountId(account.Owner.BnetEntityId)
					.Build();

				account.LoggedInClient.MakeRPC((lid) =>
					bgs.protocol.notification.v1.NotificationListener.CreateStub(account.LoggedInClient).OnNotificationReceived(new HandlerController() { ListenerId = lid
					}, chatNotificationBuilder, callback => { }));
			}
		}

		#endregion

		#region guild news

		public void AddNews(GameAccount account, int type, byte[] data = null)
		{
			var newDBGuildNews = new DBGuildNews
			{
				DBGuild = this.DBGuild,
				DBGameAccount = account.DBGameAccount,
				Type = type,
				Time = DateTime.Now.ToBlizzardEpoch(),
				Data = data
			};
			DBSessions.SessionSave(newDBGuildNews);

			this.NewsTime = DateTime.Now.ToBlizzardEpoch();
			this.UpdateChannelAttributes();
		}

		public void UpdateChannelAttributes()
		{
			var notification = bgs.protocol.channel.v1.UpdateChannelStateNotification.CreateBuilder().SetStateChange(this.ChannelState).Build();
			var altnotification = bgs.protocol.channel.v1.JoinNotification.CreateBuilder().SetChannelState(this.ChannelState).Build(); 

			foreach (var member in this.Channel.Members)
				//member.Key.MakeTargetedRPC(this.Channel, (lid) => bgs.protocol.channel.v1.ChannelListener.CreateStub(member.Key).OnUpdateChannelState(new HandlerController() { ListenerId = lid }, notification, callback => { }));
				member.Key.MakeTargetedRPC(this.Channel, (lid) => bgs.protocol.channel.v1.ChannelListener.CreateStub(member.Key).OnJoin(new HandlerController() { ListenerId = lid }, altnotification, callback => { }));
		}

		public void UpdateMemberAttributes(GameAccount member)
		{
			if (!this.HasMember(member)) return;

			var channelMember = bgs.protocol.channel.v1.Member.CreateBuilder();
			var state = bgs.protocol.channel.v1.MemberState.CreateBuilder();

			state.AddAttribute(Attribute.CreateBuilder().SetName("D3.GuildMember.Rank").SetValue(Variant.CreateBuilder().SetIntValue(this.Members[member].Rank)));
			state.AddAttribute(Attribute.CreateBuilder().SetName("D3.GuildMember.Note").SetValue(Variant.CreateBuilder().SetStringValue(this.Members[member].Note)));
			state.AddAttribute(Attribute.CreateBuilder().SetName("D3.GuildMember.Hardcore").SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(true)));
			state.AddAttribute(Attribute.CreateBuilder().SetName("D3.GuildMember.AchievementPoints").SetValue(Variant.CreateBuilder().SetUintValue(member.AchievementPoints)));

			var identity = bgs.protocol.Identity.CreateBuilder().SetGameAccountId(member.BnetEntityId);

			channelMember.SetIdentity(identity);
			channelMember.SetState(state);

			var notification = bgs.protocol.channel.v1.UpdateMemberStateNotification.CreateBuilder()
				.AddStateChange(channelMember)
				.Build();

			foreach (var mbr in this.Channel.Members)
				mbr.Key.MakeTargetedRPC(this.Channel, (lid) =>
					bgs.protocol.channel.v1.ChannelListener.CreateStub(mbr.Key).OnUpdateMemberState(new HandlerController() { ListenerId = lid }, notification, callback => { }));
		}

		#endregion

		[System.Flags]
		public enum GuildFlags : uint
		{
			None = 0x00,
			LookingForMembers = 0x01,
			InviteOnly = 0x02,
			Enabled = 0x04
		}

		[System.Flags]
		public enum GuildPrivilegeFlags : uint
		{
			None = 0x00,
			Enabled = 0x01,
			ClanChatSpeak = 0x02,
			IsOfficer = 0x04,
			OfficerChatSpeak = 0x08,
			Promote = 0x10,
			Demote = 0x20,
			Invite = 0x40,
			Kick = 0x80,
			SetMOTD = 0x100,
			AddNews = 0x200,
			EditMemberNotesOther = 0x400,
			EditMemberNotesSelf = 0x800,
			SeeNotes = 0x1000,
			ModifyPermissions = 0x2000,
			EditInfo = 0x8000,
			SeeInvites = 0x10000,
			CancelInvite = 0x20000,
			SetLanguage = 0x40000,
			SetPrivacy = 0x80000,
		}

		public class RankDescriptor
		{
			public string RankName;
			public uint Privileges;
		}
	}
}
