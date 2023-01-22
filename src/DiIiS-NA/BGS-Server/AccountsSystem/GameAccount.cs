//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using bgs.protocol.presence.v1;
//Blizzless Project 2022 
using D3.Account;
//Blizzless Project 2022 
using D3.Achievements;
//Blizzless Project 2022 
using D3.Client;
//Blizzless Project 2022 
using D3.OnlineService;
//Blizzless Project 2022 
using D3.PartyMessage;
//Blizzless Project 2022 
using D3.Profile;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.ChannelSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GuildSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using static DiIiS_NA.LoginServer.Toons.Toon;

namespace DiIiS_NA.LoginServer.AccountsSystem
{
	public class GameAccount : PersistentRPCObject
	{
		private Account _owner;

		public Account Owner
		{
			get
			{
				if (_owner == null)
					_owner = AccountManager.GetAccountByPersistentID(AccountId);
				return _owner;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.DBAccount = value.DBAccount;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public ulong AccountId = 0;

		public DBGameAccount DBGameAccount
		{
			get
			{
				return DBSessions.SessionGet<DBGameAccount>(this.PersistentID);
			}
			set { }
		}

		public D3.OnlineService.EntityId D3GameAccountId
		{
			get
			{
				return D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(BnetEntityId.High).SetIdLow(PersistentID).Build();
			}
		}

		public ByteStringPresenceField<BannerConfiguration> BannerConfigurationField
		{
			get
			{
				return new ByteStringPresenceField<BannerConfiguration>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 1, 0, BannerConfiguration);
			}
		}


		public ByteStringPresenceField<EntityId> LastPlayedHeroIdField
		{
			get
			{
				var val = new ByteStringPresenceField<EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 2, 0)
				{
					Value = LastPlayedHeroId
				};
				return val;
			}
		}

		public IntPresenceField ActivityField
		{
			get
			{
				return new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 3, 0, this.CurrentActivity);
			}
		}

		public ByteStringPresenceField<D3.Guild.GuildSummary> ClanIdField
		{
			get
			{
				var val = new ByteStringPresenceField<D3.Guild.GuildSummary>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 7, 0);
				val.Value = this.Clan.Summary;
				return val;
			}
		}

		public StringPresenceField GameVersionField
		{
			get
			{
				return new StringPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.GameAccount, 11, 0, "2.7.1.22044");
			}
		}


		public EntityId LastPlayedHeroId
		{
			get
			{
				if (this.CurrentToon == null)
					return Toons.Count > 0 ? Toons.First().D3EntityID : AccountHasNoToons;
				return this.CurrentToon.D3EntityID;
			}
		}

		public ByteStringPresenceField<bgs.protocol.channel.v1.ChannelId> PartyIdField
		{
			get
			{
				var val = new ByteStringPresenceField<bgs.protocol.channel.v1.ChannelId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Party, 1, 0)
				{
					Value = PartyChannelId
				};
				return val;
			}
		}
		/*
		public ByteStringPresenceField<EntityId> PartyIdField
		{
			get
			{
				var val = new ByteStringPresenceField<EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Party, 1, 0)
				{
					Value = PartyId
				};
				return val;
			}
		}
		//*/

		public bgs.protocol.channel.v1.ChannelId PartyChannelId
		{
			get
			{
				if (this.LoggedInClient != null && this.LoggedInClient.CurrentChannel != null)
				{
					return bgs.protocol.channel.v1.ChannelId.CreateBuilder()
						.SetType(0)
						.SetId((uint)this.LoggedInClient.CurrentChannel.D3EntityId.IdLow)
						.SetHost(bgs.protocol.ProcessId.CreateBuilder().SetLabel(1).SetEpoch(0))
						.Build();
				}
				else
					return null;
			}
			set
			{
				if (value != null)
					this.LoggedInClient.CurrentChannel = ChannelManager.GetChannelByChannelId (value);
			}
		}

		public EntityId PartyId
		{
			get
			{
				if (this.LoggedInClient != null && this.LoggedInClient.CurrentChannel != null)
				{
					return this.LoggedInClient.CurrentChannel.D3EntityId;
				}
				else
					return null;
			}
			set
			{
				if (value != null)
					this.LoggedInClient.CurrentChannel = ChannelManager.GetChannelByEntityId(value);
			}
		}

		public IntPresenceField JoinPermissionField
			= new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Party, 2, 0);

		public FourCCPresenceField ProgramField
			= new FourCCPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 3, 0);

		public StringPresenceField CallToArmsField
		{
			get
			{
				return new StringPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Party, 3, 0, Owner.BattleTagName);
			}
		}

		public StringPresenceField BattleTagField
		{
			get
			{
				return new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 5, 0, Owner.BattleTag);
			}
		}

		public StringPresenceField GameAccountNameField
		{
			get
			{
				return new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 6, 0, Owner.BnetEntityId.Low.ToString() + "#1");
			}
		}

		public EntityIdPresenceField OwnerIdField
		{
			get
			{
				var val = new EntityIdPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 7, 0);
				val.Value = this.Owner.BnetEntityId;
				return val;
			}
		}

		public BoolPresenceField GameAccountStatusField = new BoolPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 1, 0, false);

		public int _currentActivity = 0;

		public int CurrentActivity
		{
			get { return _currentActivity; }
			set
			{
				_currentActivity = value;
				ChangedFields.SetPresenceFieldValue(ActivityField);
			}
		}


		public IntPresenceField LastOnlineField
		{
			get
			{
				return new IntPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.GameAccount, 4, 0, (long)this.LastOnline);
			}
		}

		public ulong LastOnline = 1;

		public FieldKeyHelper.Program Program;


		public BannerConfiguration BannerConfiguration
		{
			get
			{
				if (this._bannerConfiguration != null)
					return this._bannerConfiguration;
				var res = BannerConfiguration.CreateBuilder();
				if (this.DBGameAccount.Banner == null || this.DBGameAccount.Banner.Length < 1)
				{
					res = BannerConfiguration.CreateBuilder()
						.SetBannerShape(189701627)
						.SetSigilMain(1494901005)
						.SetSigilAccent(3399297034)
						.SetPatternColor(1797588777)
						.SetBackgroundColor(1797588777)
						.SetSigilColor(2045456409)
						.SetSigilPlacement(1015980604)
						.SetPattern(4173846786)
						.SetUseSigilVariant(true);
					//.SetEpicBanner((uint)StringHashHelper.HashNormal("Banner_Epic_02_Class_Completion"))
					//.SetEpicBanner((uint)StringHashHelper.HashNormal("Banner_Epic_03_PVP_Class_Completion"))
					//.SetEpicBanner((uint)StringHashHelper.HashNormal("Banner_Epic_01_Hardcore"))

					lock (this.DBGameAccount)
					{
						var dbGAcc = this.DBGameAccount;
						dbGAcc.Banner = res.Build().ToByteArray();
						DBSessions.SessionUpdate(dbGAcc);
					}
				}
				else
					res = BannerConfiguration.CreateBuilder(BannerConfiguration.ParseFrom(this.DBGameAccount.Banner));

				this._bannerConfiguration = res.Build();
				return this._bannerConfiguration;
			}
			set
			{
				this._bannerConfiguration = value;
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.Banner = value.ToByteArray();
					DBSessions.SessionUpdate(dbGAcc);
				}

				ChangedFields.SetPresenceFieldValue(BannerConfigurationField);
			}
		}

		private BannerConfiguration _bannerConfiguration;

		private ScreenStatus _screenstatus = ScreenStatus.CreateBuilder().SetScreen(1).SetStatus(0).Build();

		public ScreenStatus ScreenStatus
		{
			get { return _screenstatus; }
			set
			{
				_screenstatus = value;
				JoinPermissionField.Value = value.Screen;
				ChangedFields.SetPresenceFieldValue(JoinPermissionField);
			}
		}

		/// <summary>
		/// Selected toon for current account.
		/// </summary>

		public string CurrentAHCurrency
		{
			get
			{
				if (this.CurrentToon.IsHardcore)
					return "D3_GOLD_HC";
				else
					return "D3_GOLD";
			}
			set { }
		}
		public bool Logined = false;
		public bool Setted = false;
		public Toon CurrentToon
		{
			get
			{
				if (this._currentToonId == 0) return null;
				return ToonManager.GetToonByLowID(this._currentToonId);
			}
			set
			{
				if (value.GameAccount.PersistentID != this.PersistentID) return; //just in case...
				this._currentToonId = value.PersistentID;
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.LastPlayedHero = value.DBToon;
					DBSessions.SessionUpdate(dbGAcc);
				}

				ChangedFields.SetPresenceFieldValue(LastPlayedHeroIdField);
				ChangedFields.SetPresenceFieldValue(value.HeroClassField);
				ChangedFields.SetPresenceFieldValue(value.HeroLevelField);
				ChangedFields.SetPresenceFieldValue(value.HeroParagonLevelField);
				ChangedFields.SetPresenceFieldValue(value.HeroVisualEquipmentField);
				ChangedFields.SetPresenceFieldValue(value.HeroFlagsField);
				ChangedFields.SetPresenceFieldValue(value.HeroNameField);
				ChangedFields.SetPresenceFieldValue(value.HighestUnlockedAct);
				ChangedFields.SetPresenceFieldValue(value.HighestUnlockedDifficulty);
			}
		}

		private ulong _currentToonId = 0;

		public ulong Gold
		{
			get
			{
				return this.DBGameAccount.Gold;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.Gold = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public ulong HardcoreGold
		{
			get
			{
				return this.DBGameAccount.HardcoreGold;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.HardcoreGold = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public int BloodShards
		{
			get
			{
				return this.DBGameAccount.BloodShards;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.BloodShards = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public int HardcoreBloodShards
		{
			get
			{
				return this.DBGameAccount.HardcoreBloodShards;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.HardcoreBloodShards = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public int TotalBloodShards
		{
			get
			{
				return this.DBGameAccount.TotalBloodShards;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.TotalBloodShards = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}


		public int CraftItem1
		{
			get { return this.DBGameAccount.CraftItem1; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.CraftItem1 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int CraftItem2
		{
			get { return this.DBGameAccount.CraftItem2; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.CraftItem2 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int CraftItem3
		{
			get { return this.DBGameAccount.CraftItem3; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.CraftItem3 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int CraftItem4
		{
			get { return this.DBGameAccount.CraftItem4; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.CraftItem4 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int CraftItem5
		{
			get { return this.DBGameAccount.CraftItem5; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.CraftItem5 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int BigPortalKey
		{
			get { return this.DBGameAccount.BigPortalKey; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.BigPortalKey = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int LeorikKey
		{
			get { return this.DBGameAccount.LeorikKey; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.LeorikKey = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}


		public int VialofPutridness
		{
			get { return this.DBGameAccount.VialofPutridness; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.VialofPutridness = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}

		public int IdolofTerror
		{
			get { return this.DBGameAccount.IdolofTerror; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.IdolofTerror = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}

		public int HeartofFright
		{
			get { return this.DBGameAccount.HeartofFright; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HeartofFright = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}

		public int HoradricA1Res
		{
			get { return this.DBGameAccount.HoradricA1; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HoradricA1 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int HoradricA2Res
		{
			get { return this.DBGameAccount.HoradricA2; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HoradricA2 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int HoradricA3Res
		{
			get { return this.DBGameAccount.HoradricA3; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HoradricA3 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int HoradricA4Res
		{
			get { return this.DBGameAccount.HoradricA4; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HoradricA4 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}
		public int HoradricA5Res
		{
			get { return this.DBGameAccount.HoradricA5; }
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGameAccount = this.DBGameAccount;
					dbGameAccount.HoradricA5 = value;
					DBSessions.SessionUpdate(dbGameAccount);
				}
			}
		}

		public Guild Clan
		{
			get
			{
				return GuildManager.GetClans().Where(g => g.HasMember(this)).FirstOrDefault();
			}
		}
		public List<Guild> Communities
		{
			get
			{
				return GuildManager.GetCommunities().Where(g => g.HasMember(this)).ToList();
			}
		}

		public List<D3.Guild.InviteInfo> GuildInvites = new List<D3.Guild.InviteInfo>();

		public GameAccountSettings Settings
		{
			get
			{
				GameAccountSettings res = null;
				if (this.DBGameAccount.UISettings == null || this.DBGameAccount.UISettings.Length < 1)
				{
					res = GameAccountSettings.CreateBuilder()
						//.SetChatFontSize(8)
						.SetRmtPreferredCurrency("USD")
						.SetRmtLastUsedCurrency("D3_GOLD")
						.AddAutoJoinChannelsDeprecated("D3_GeneralChat")
						.Build();

					lock (this.DBGameAccount)
					{
						var dbGAcc = this.DBGameAccount;
						dbGAcc.UISettings = res.ToByteArray();
						DBSessions.SessionUpdate(dbGAcc);
					}
				}
				else
					res = GameAccountSettings.ParseFrom(this.DBGameAccount.UISettings);

				return res;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.UISettings = value.ToByteArray();
					DBSessions.SessionUpdate(dbGAcc);
				}

				ChangedFields.SetPresenceFieldValue(BannerConfigurationField);
			}
		}

		public Preferences Preferences
		{
			get
			{
				Preferences res = null;
				if (DBGameAccount.UIPrefs == null || DBGameAccount.UIPrefs.Length < 1)
				{
					res = Preferences.CreateBuilder()
						.SetVersion(43)
						//.SetFlags2(0x7FFFFFFF)
						//.SetActionBindingWorldmap(D3.Client.ActionBinding.CreateBuilder().SetKey1(48).SetKey2(-1).SetKeyModifierFlags1(0).SetKeyModifierFlags2(0).Build())
						//.SetActionBindingConsole(D3.Client.ActionBinding.CreateBuilder().SetKey1(48).SetKey2(-1).SetKeyModifierFlags1(0).SetKeyModifierFlags2(0).Build())
						//.SetActionBindingVoiceptt(D3.Client.ActionBinding.CreateBuilder().SetKey1(112).SetKey2(-1).SetKeyModifierFlags1(0).SetKeyModifierFlags2(0).Build())
						.Build();

					lock (this.DBGameAccount)
					{
						var dbGAcc = this.DBGameAccount;
						dbGAcc.UIPrefs = res.ToByteArray();
						DBSessions.SessionUpdate(dbGAcc);
					}
				}
				else
					res = Preferences.ParseFrom(DBGameAccount.UIPrefs);

				return res;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.UIPrefs = value.ToByteArray();
					DBSessions.SessionUpdate(dbGAcc);
				}

				ChangedFields.SetPresenceFieldValue(BannerConfigurationField);
			}
		}

		/// <summary>
		/// Away status
		/// </summary>
		public AwayStatusFlag AwayStatus { get; private set; }

		private List<AchievementUpdateRecord> _achievements = null;
		private List<CriteriaUpdateRecord> _criteria = null;

		public List<AchievementUpdateRecord> Achievements
		{
			get
			{
				if (this._achievements == null)
					this.SetField();
				return this._achievements;
			}
			set
			{
				this._achievements = value;
			}
		}

		public List<CriteriaUpdateRecord> AchievementCriteria
		{
			get
			{
				if (this._criteria == null)
					this.SetField();
				return this._criteria;
			}
			set
			{
				this._criteria = value;
			}
		}

		private ClassInfo GetClassInfo(ToonClass className)
		{
			uint playtime = 0;
			uint highestLevel = 1;
			var _toons = DBSessions.SessionQueryWhere<DBToon>(
					dbi =>
					dbi.DBGameAccount.Id == this.PersistentID
					&& dbi.Class == className).ToList();
			foreach (var toon in _toons)
			{
				playtime += (uint)toon.TimePlayed;
				if (highestLevel < toon.Level) highestLevel = toon.Level;
			}
			return ClassInfo.CreateBuilder()
				.SetPlaytime(playtime)
				.SetHighestLevel(highestLevel)
				//deprecated //.SetHighestDifficulty(highestDifficulty)
				.Build();
		}

		private uint GetHighestHardcoreLevel()
		{
			uint highestLevel = 0;
			var _toons = DBSessions.SessionQueryWhere<DBToon>(
					dbi =>
					dbi.DBGameAccount.Id == this.PersistentID
					&& dbi.isHardcore == true).ToList();
			foreach (var toon in _toons)
			{
				if (highestLevel < toon.Level) highestLevel = toon.Level;
			}
			return highestLevel;
		}

		public bool InviteToGuild(Guild guild, GameAccount inviter)
		{
			if (guild.IsClan && this.Clan != null)
				return false;
			else
			{
				var invite = D3.Guild.InviteInfo.CreateBuilder()
					.SetGuildId(guild.PersistentId)
					.SetGuildName(guild.Name)
					.SetInviterId(inviter.PersistentID)
					.SetCategory(guild.Category)
					.SetInviteType(inviter.PersistentID == this.PersistentID ? 1U : 0U)
					.SetExpireTime(3600);
				if (guild.IsClan) invite.SetGuildTag(guild.Prefix);
				this.GuildInvites.Add(invite.Build());


				var update = D3.Notification.GuildInvitesListUpdate.CreateBuilder();
				update.SetIsRemoved(false).SetGuildId(guild.PersistentId).SetInvite(invite);

				var notification = bgs.protocol.notification.v1.Notification.CreateBuilder();
				notification.SetSenderId(bgs.protocol.EntityId.CreateBuilder().SetHigh(0UL).SetLow(0UL));
				notification.SetTargetAccountId(this.Owner.BnetEntityId);
				notification.SetTargetId(this.BnetEntityId);
				notification.SetType("D3.NotificationMessage");
				notification.AddAttribute(bgs.protocol.Attribute.CreateBuilder()
					.SetName("D3.NotificationMessage.MessageId").SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(0)));
				notification.AddAttribute(bgs.protocol.Attribute.CreateBuilder()
					.SetName("D3.NotificationMessage.Payload").SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(update.Build().ToByteString())));

				this.LoggedInClient.MakeRPC((lid) =>
					bgs.protocol.notification.v1.NotificationListener.CreateStub(this.LoggedInClient).OnNotificationReceived(new HandlerController() { ListenerId = lid 
					}, notification.Build(), callback => { }));
				return true;
			}
		}

		public AccountProfile Profile
		{
			get
			{
				var dbGAcc = this.DBGameAccount;
				var profile = AccountProfile.CreateBuilder()
					.SetParagonLevel((uint)dbGAcc.ParagonLevel)
					.SetDeprecatedBestLadderParagonLevel((uint)dbGAcc.ParagonLevel)
					.SetParagonLevelHardcore((uint)dbGAcc.ParagonLevelHardcore)
					.SetBloodShardsCollected((uint)dbGAcc.TotalBloodShards)
					.SetSeasonId(1)
					.AddSeasons(1)
					.SetElitesKilled(dbGAcc.ElitesKilled)
					.SetGoldCollected(dbGAcc.TotalGold)
					//deprecated //.SetHighestDifficulty(Convert.ToUInt32(progress[0], 10))
					.SetNumFallenHeroes(3)
					.SetParagonLevelHardcore(0)  // Уровень совершенства в гер. режиме
					.SetBountiesCompleted((uint)dbGAcc.TotalBounties) //Выполнено поручений
					.SetLootRunsCompleted(0) //Закрыто нефалемских порталов
					.SetPvpWins(0)
					.SetPvpTakedowns(0)
					.SetPvpDamage(0)
					.SetMonstersKilled(dbGAcc.TotalKilled) //Убито монстров
					.SetElitesKilled(this.DBGameAccount.ElitesKilled) //Убито особых противников
					.SetGoldCollected(this.DBGameAccount.TotalGold) //Собрано золото
					.SetHighestHardcoreLevel(0)     // Максимальный уровень в гер. режиме
					.SetHardcoreMonstersKilled(0) // Убито монстров в гер. режиме

					.SetHighestHardcoreLevel(GetHighestHardcoreLevel())
					.SetMonstersKilled(dbGAcc.TotalKilled)
					.SetClassBarbarian(GetClassInfo(ToonClass.Barbarian))
					.SetClassCrusader(GetClassInfo(ToonClass.Crusader))
					.SetClassDemonhunter(GetClassInfo(ToonClass.DemonHunter))
					.SetClassMonk(GetClassInfo(ToonClass.Monk))
					.SetClassWitchdoctor(GetClassInfo(ToonClass.WitchDoctor))
					.SetClassWizard(GetClassInfo(ToonClass.Wizard))
					.SetClassNecromancer(GetClassInfo(ToonClass.Necromancer));

					
				if (dbGAcc.BossProgress[0] != 0xff) profile.SetHighestBossDifficulty1(dbGAcc.BossProgress[0]);
				if (dbGAcc.BossProgress[1] != 0xff) profile.SetHighestBossDifficulty2(dbGAcc.BossProgress[1]);
				if (dbGAcc.BossProgress[2] != 0xff) profile.SetHighestBossDifficulty3(dbGAcc.BossProgress[2]);
				if (dbGAcc.BossProgress[3] != 0xff) profile.SetHighestBossDifficulty4(dbGAcc.BossProgress[3]);
				if (dbGAcc.BossProgress[4] != 0xff) profile.SetHighestBossDifficulty5(dbGAcc.BossProgress[4]);
				foreach (var hero in Toons)
				{
					profile.AddHeroes(HeroMiniProfile.CreateBuilder()
						.SetHeroName(hero.Name)
						.SetHeroGbidClass((int)hero.ClassID)
						.SetHeroFlags((uint)hero.Flags)
						.SetHeroId((uint)hero.D3EntityID.IdLow)
						.SetHeroLevel(hero.Level)
						.SetHeroVisualEquipment(hero.HeroVisualEquipmentField.Value)
						);
				}
				profile.SetNumFallenHeroes(1);

				return profile.Build();

				//*/
			}
		}

		public static readonly EntityId AccountHasNoToons =
			EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(0).Build();
		//Platinum

		public int Platinum { get { return this.DBGameAccount.Platinum; } set { lock (this.DBGameAccount) { var dbGA = this.DBGameAccount; dbGA.Platinum = value; DBSessions.SessionUpdate(dbGA); } } }

		public List<Toon> Toons
		{
			get { return ToonManager.GetToonsForGameAccount(this); }
		}

		public GameAccount(DBGameAccount dbGameAccount, List<Core.Storage.AccountDataBase.Entities.DBAchievements> achs = null)
			: base(dbGameAccount.Id)
		{
			//DBGameAccount = dbGameAccount;
			this.AccountId = dbGameAccount.DBAccount.Id;
			if (dbGameAccount.LastPlayedHero != null)
				this._currentToonId = dbGameAccount.LastPlayedHero.Id;
			this.LastOnline = dbGameAccount.LastOnline;
			var banner = this.BannerConfiguration; //just pre-loading it

			const ulong bnetGameAccountHigh = ((ulong)EntityIdHelper.HighIdType.GameAccountId) + (0x0100004433);// + (0x0100004433);

			this.BnetEntityId = bgs.protocol.EntityId.CreateBuilder().SetHigh(bnetGameAccountHigh).SetLow(PersistentID).Build();
			this.ProgramField.Value = "D3";
		}

		private void SetField()
		{
			Achievements = new List<AchievementUpdateRecord>();
			AchievementCriteria = new List<CriteriaUpdateRecord>();

			var achs = DBSessions.SessionQueryWhere<Core.Storage.AccountDataBase.Entities.DBAchievements>(dbi => dbi.DBGameAccount.Id == this.PersistentID).ToList();
			foreach (var ach in achs)
			{
				if (ach.AchievementId == 1)
				{
					;
					uint countOfTravels = 0;
					foreach (var criteria in GameServer.AchievementSystem.AchievementManager.UnserializeBytes(ach.Criteria))
					{
						if (criteria == 3367569)
							countOfTravels++;
					}
					AchievementCriteria.Add(CriteriaUpdateRecord.CreateBuilder()
							.SetCriteriaId32AndFlags8(3367569)
							.SetQuantity32(countOfTravels)
							.Build()
							);
				}
				else
				{
					if (ach.CompleteTime != -1)
						Achievements.Add(AchievementUpdateRecord.CreateBuilder()
							.SetAchievementId(ach.AchievementId)//74987243307105)
							.SetCompletion(ach.CompleteTime)//1476016727)
							.Build()
						);

					if (GameServer.AchievementSystem.AchievementManager.UnserializeBytes(ach.Criteria).Count > 0 && ach.CompleteTime == -1)
						foreach (var criteria in GameServer.AchievementSystem.AchievementManager.UnserializeBytes(ach.Criteria))
							AchievementCriteria.Add(CriteriaUpdateRecord.CreateBuilder()
							.SetCriteriaId32AndFlags8(criteria)
							.SetQuantity32(1)
							.Build()
							);

					if (ach.Quantity > 0 && ach.CompleteTime == -1)
						AchievementCriteria.Add(CriteriaUpdateRecord.CreateBuilder()
						.SetCriteriaId32AndFlags8((uint)GameServer.AchievementSystem.AchievementManager.GetMainCriteria(ach.AchievementId))
						.SetQuantity32((uint)ach.Quantity)
						.Build()
						);
				}
			}
		}

		public bool IsOnline
		{
			get { return LoggedInClient != null; }
			set { }
		}

		private BattleClient _loggedInClient;

		public BattleClient LoggedInClient
		{
			get { return _loggedInClient; }
			set
			{
				_loggedInClient = value;

				GameAccountStatusField.Value = IsOnline;

				ulong current_time = (ulong)DateTime.Now.ToExtendedEpoch();


				//checking last online
				var dbAcc = this.Owner.DBAccount;

				ChangedFields.SetPresenceFieldValue(GameAccountStatusField);
				ChangedFields.SetPresenceFieldValue(LastOnlineField);
				ChangedFields.SetPresenceFieldValue(BannerConfigurationField);
				
				//TODO: Remove this set once delegate for set is added to presence field
				//this.Owner.AccountOnlineField.Value = this.Owner.IsOnline;
				//var operation = this.Owner.AccountOnlineField.GetFieldOperation();
				try
				{
					NotifyUpdate();
				}
				catch { }
				//this.UpdateSubscribers(this.Subscribers, new List<bgs.protocol.presence.v1.FieldOperation>() { operation });
			}
		}

		/// <summary>
		/// GameAccount's flags.
		/// </summary>
		public GameAccountFlags Flags
		{
			get
			{
				return (GameAccountFlags)this.DBGameAccount.Flags | GameAccountFlags.HardcoreAdventureModeUnlocked;
			}
			set
			{
				lock (this.DBGameAccount)
				{
					var dbGAcc = this.DBGameAccount;
					dbGAcc.Flags = (int)value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public Digest Digest
		{
			get
			{
				Digest.Builder builder = Digest.CreateBuilder().SetVersion(116)
					// 7447=>99, 7728=> 100, 8801=>102, 8296=>105, 8610=>106, 8815=>106, 8896=>106, 9183=>107
					.SetBannerConfiguration(this.BannerConfiguration)
					//.SetFlags((uint)this.Flags) //1 - Enable Hardcore
					.SetFlags((uint)114)
					.SetLastPlayedHeroId(LastPlayedHeroId)
					.SetRebirthsUsed(0)
					.SetStashTabsRewardedFromSeasons(1)
					.SetSeasonId(1)
					.SetCompletedSoloRift(false)
					.SetChallengeRiftAccountData(D3.ChallengeRifts.AccountData.CreateBuilder()
						.SetLastChallengeRewardEarned(416175).SetLastChallengeTried(416175)
						)
					.AddAltLevels((uint)this.DBGameAccount.ParagonLevel)
					//.AddAltLevels((uint)this.DBGameAccount.ParagonLevelHardcore)
					;
				if (this.Clan != null)
					builder.SetGuildId(this.Clan.PersistentId);

				return builder.Build();
			}
		}

		public uint AchievementPoints
		{
			get
			{
				return (uint)this.Achievements.Where(a => a.Completion != -1).Count() * 10U;
			}
		}

		#region Notifications

		public override void NotifyUpdate()
		{
			var operations = ChangedFields.GetChangedFieldList();
			ChangedFields.ClearChanged();
			base.UpdateSubscribers(Subscribers, operations);
		}

		public override List<FieldOperation> GetSubscriptionNotifications()
		{
			//for now set it here
			GameAccountStatusField.Value = IsOnline;

			var operationList = new List<FieldOperation>();

			//gameaccount
			//D3,GameAccount,1,0 -> D3.DBAccount.BannerConfiguration
			//D3,GameAccount,2,0 -> ToonId
			//D3,GameAccount,3,0 -> Activity
			//D3,Party,1,0 -> PartyId
			//D3,Party,2,0 -> JoinPermission
			//D3,Hero,1,0 -> Hero Class
			//D3,Hero,2,0 -> Hero's current level
			//D3,Hero,3,0 -> D3.Hero.VisualEquipment
			//D3,Hero,4,0 -> Hero's flags
			//D3,Hero,5,0 -> Hero Name
			//D3,Hero,6,0 -> HighestUnlockedAct
			//D3,Hero,7,0 -> HighestUnlockedDifficulty
			//Bnet,GameAccount,1,0 -> GameAccount Online
			//Bnet,GameAccount,3,0 -> FourCC = "D3"
			//Bnet,GameAccount,4,0 -> Unk Int (0 if GameAccount is Offline)
			//Bnet,GameAccount,5,0 -> BattleTag
			//Bnet,GameAccount,6,0 -> DBAccount.Low + "#1"
			//Bnet,GameAccount,7,0 -> DBAccount.EntityId
			operationList.Add(BannerConfigurationField.GetFieldOperation());
			if (LastPlayedHeroId != AccountHasNoToons)
			{
				operationList.Add(LastPlayedHeroIdField.GetFieldOperation());
				if (CurrentToon != null)
					operationList.AddRange(CurrentToon.GetSubscriptionNotifications());
			}

			operationList.Add(GameAccountStatusField.GetFieldOperation());
			operationList.Add(ProgramField.GetFieldOperation());
			operationList.Add(LastOnlineField.GetFieldOperation());
			operationList.Add(BattleTagField.GetFieldOperation());
			operationList.Add(GameAccountNameField.GetFieldOperation());
			operationList.Add(OwnerIdField.GetFieldOperation());
			if (this.Clan != null)
				operationList.Add(ClanIdField.GetFieldOperation());
			operationList.Add(GameVersionField.GetFieldOperation());
			operationList.Add(PartyIdField.GetFieldOperation());
			operationList.Add(JoinPermissionField.GetFieldOperation());
			operationList.Add(CallToArmsField.GetFieldOperation());
			operationList.Add(ActivityField.GetFieldOperation());
			return operationList;
		}

		#endregion

		public void Update(IList<FieldOperation> operations)
		{
			List<FieldOperation> operationsToUpdate = new List<FieldOperation>();
			foreach (var operation in operations)
			{
				switch (operation.Operation)
				{
					case FieldOperation.Types.OperationType.SET:
						var op_build = DoSet(operation.Field);
						if (op_build.HasValue)
						{
							var new_op = operation.ToBuilder();
							new_op.SetField(op_build);
							operationsToUpdate.Add(new_op.Build());
						}
						break;
					case FieldOperation.Types.OperationType.CLEAR:
						DoClear(operation.Field);
						break;
					default:
						Logger.Warn("No operation type.");
						break;
				}
			}
			if (operationsToUpdate.Count > 0)
				base.UpdateSubscribers(Subscribers, operationsToUpdate);
		}

		public void TestUpdate()
		{
			var operations = this.GetSubscriptionNotifications();
			/*
			operations.Add(
				FieldOperation.CreateBuilder()
				.SetOperation(FieldOperation.Types.OperationType.SET)
				.SetField(
					Field.CreateBuilder()
					.SetKey(FieldKey.CreateBuilder().SetGroup(4).SetField(3).SetProgram(17459))
					.SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue("TExt")))
				.Build()
			);
			//*/
			//operations.Add(new StringPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Party, 3, 0, "CallToArms").GetFieldOperation());
			//this.Update(operations);
		}

		private Field.Builder DoSet(Field field)
		{
			FieldOperation.Builder operation = FieldOperation.CreateBuilder();

			Field.Builder returnField = Field.CreateBuilder().SetKey(field.Key);
			if (LoggedInClient == null) return returnField;

			switch ((FieldKeyHelper.Program)field.Key.Program)
			{
				case FieldKeyHelper.Program.D3:
					if (field.Key.Group == 2 && field.Key.Field == 3) //CurrentActivity
					{
						this.CurrentActivity = (int)field.Value.IntValue;
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set CurrentActivity to {1}", this, field.Value.IntValue);
					}
					else if (field.Key.Group == 2 && field.Key.Field == 4) //Unknown bool
					{
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set CurrentActivity to {1}", this, field.Value.BoolValue);
					}
					else if (field.Key.Group == 2 && field.Key.Field == 6) //Flags
					{
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set Flags to {1}", this, field.Value.UintValue);
					}
					else if (field.Key.Group == 2 && field.Key.Field == 8) //?
					{
						returnField.SetValue(field.Value);
					}
					else if (field.Key.Group == 2 && field.Key.Field == 11) //Version
					{
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set Version to {1}", this, field.Value.StringValue);
					}
					else if (field.Key.Group == 4 && field.Key.Field == 1) //PartyId
					{
						if (field.Value.HasMessageValue) //7727 Sends empty SET instead of a CLEAR -Egris
						{
							Channel channel = ChannelManager.GetChannelByChannelId(bgs.protocol.channel.v1.ChannelId.ParseFrom(field.Value.MessageValue));
							//this.PartyId = EntityId.CreateBuilder().SetIdLow(NewChannelID.Id).SetIdHigh(0x600000000000000).Build();
							
							this.PartyChannelId = bgs.protocol.channel.v1.ChannelId.ParseFrom(field.Value.MessageValue);
							LoggedInClient.CurrentChannel = channel;
							var c = bgs.protocol.channel.v1.ChannelId.ParseFrom(field.Value.MessageValue);
							//returnField.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(PartyChannelId.ToByteString()).Build());
							returnField.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(PartyChannelId.ToByteString()).Build());
							//returnField.SetValue(field.Value);



							Logger.Trace("{0} set channel to {1}", this, channel);
						}
						else
						{
							this.PartyId = null;
							//if(PartyChannelId != null)
							//	returnField.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(PartyChannelId.ToByteString()).Build());
							//else
							returnField.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(ByteString.Empty).Build());
							Logger.Trace("Emtpy-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
						}
					}
					else if (field.Key.Group == 4 && field.Key.Field == 2) //JoinPermission
					{
						if (ScreenStatus.Screen != field.Value.IntValue)
						{
							ScreenStatus = ScreenStatus.CreateBuilder().SetScreen((int)field.Value.IntValue).SetStatus(0).Build();
							Logger.Trace("{0} set current screen to {1}.", this, field.Value.IntValue);
						}
						returnField.SetValue(field.Value);
					}
					else if (field.Key.Group == 4 && field.Key.Field == 3) //CallToArmsMessage
					{
						Logger.Trace("CallToArmsMessage: {0}, {1}, {2}", field.Key.Group, field.Key.Field, field.Value);
						returnField.SetValue(field.Value);
					}
					else if (field.Key.Group == 4 && field.Key.Field == 4) //Party IsFull
					{
						returnField.SetValue(field.Value);
					}
					else if (field.Key.Group == 5 && field.Key.Field == 5) //Game IsPrivate
					{
						//returnField.SetValue(Variant.CreateBuilder().SetBoolValue(false).Build());
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set Game IsPrivate {1}.", this, field.Value.ToString());
					}
					else
					{
						Logger.Warn("GameAccount: Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program,
									field.Key.Group, field.Key.Field, field.Value);
					}
					break;
				case FieldKeyHelper.Program.BNet:
					if (field.Key.Group == 2 && field.Key.Field == 2) // SocialStatus
					{
						AwayStatus = (AwayStatusFlag)field.Value.IntValue;
						returnField.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue((long)AwayStatus).Build());
						Logger.Trace("{0} set AwayStatus to {1}.", this, AwayStatus);
					}
					else if (field.Key.Group == 2 && field.Key.Field == 8)// RichPresence
					{
						returnField.SetValue((field.Value));
					}
					else if (field.Key.Group == 2 && field.Key.Field == 10) // AFK
					{
						returnField.SetValue(field.Value);
						Logger.Trace("{0} set AFK to {1}.", this, field.Value.BoolValue);
					}
					else
					{
						Logger.Warn("GameAccount: Unknown set-field: {0}, {1}, {2} := {3}", field.Key.Program,
									field.Key.Group, field.Key.Field, field.Value);
					}
					break;
			}

			//We only update subscribers on fields that actually change values.
			return returnField;
		}

		private void DoClear(Field field)
		{
			switch ((FieldKeyHelper.Program)field.Key.Program)
			{
				case FieldKeyHelper.Program.D3:
					Logger.Warn("GameAccount: Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group,
								field.Key.Field);
					break;
				case FieldKeyHelper.Program.BNet:
					Logger.Warn("GameAccount: Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group,
								field.Key.Field);
					break;
			}
		}

		public Field QueryField(FieldKey queryKey)
		{
			Field.Builder field = Field.CreateBuilder().SetKey(queryKey);

			switch ((FieldKeyHelper.Program)queryKey.Program)
			{
				case FieldKeyHelper.Program.D3:
					if (queryKey.Group == 2 && queryKey.Field == 1) // Banner configuration
					{
						field.SetValue(
							bgs.protocol.Variant.CreateBuilder().SetMessageValue(BannerConfigurationField.Value.ToByteString()).Build
								());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 2) //Hero's EntityId
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(LastPlayedHeroId.ToByteString()).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 4) //Unknown Bool
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(true).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 8)
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(true).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 1) // Hero's class (GbidClass)
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(CurrentToon.ClassID).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 2) // Hero's current level
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(CurrentToon.Level).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 3) // Hero's visible equipment
					{
						field.SetValue(
							bgs.protocol.Variant.CreateBuilder().SetMessageValue(
								CurrentToon.HeroVisualEquipmentField.Value.ToByteString()).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 4) // Hero's flags (gender and such)
					{
						field.SetValue(
							bgs.protocol.Variant.CreateBuilder().SetIntValue(/*1073741821*/(uint)(CurrentToon.Flags | ToonFlags.AllUnknowns)).
								Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 5) // Toon name
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(CurrentToon.Name).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 6) //highest act
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(400).Build());
					}
					else if (queryKey.Group == 3 && queryKey.Field == 7) //highest difficulty
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(9).Build());
					}
					else if (queryKey.Group == 4 && queryKey.Field == 1) // Channel ID if the client is online
					{
						//field.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(PartyChannelId.ToByteString()).Build());
						if (PartyId != null)
							field.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(PartyId.ToByteString()).Build());
						else field.SetValue(bgs.protocol.Variant.CreateBuilder().Build());
					}
					else if (queryKey.Group == 4 && queryKey.Field == 2)
					// Current screen (all known values are just "in-menu"; also see ScreenStatuses sent in ChannelService.UpdateChannelState)
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(ScreenStatus.Screen).Build());
					}
					else if (queryKey.Group == 4 && queryKey.Field == 4) //Unknown Bool
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(false).Build());
					}
					else
					{
						Logger.Warn("GameAccount Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group,
									queryKey.Field);
					}
					break;
				case FieldKeyHelper.Program.BNet:
					if (queryKey.Group == 2 && queryKey.Field == 1) //GameAccount Logged in
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(GameAccountStatusField.Value).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 2) // Away status
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue((long)AwayStatus).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 3) // Program - always D3
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetFourccValue("D3").Build());
						//field.SetValue(bgs.protocol.Variant.CreateBuilder().SetFourccValue("BNet").Build());
						//BNet = 16974,
						//D3 = 17459,
						//S2 = 21298,
						//WoW = 5730135,
					}
					else if (queryKey.Group == 2 && queryKey.Field == 5) // BattleTag
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(Owner.BattleTag).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 7) // DBAccount.EntityId
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(Owner.BnetEntityId).Build());
					}
					else if (queryKey.Group == 2 && queryKey.Field == 10) // AFK
					{
						field.SetValue(
							bgs.protocol.Variant.CreateBuilder().SetBoolValue(AwayStatus != AwayStatusFlag.Available).Build());
					}
					else
					{
						Logger.Warn("GameAccount Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group,
									queryKey.Field);
					}
					break;
			}

			return field.HasValue ? field.Build() : null;
		}

		public override string ToString()
		{
			return String.Format("{{ GameAccount: {0} [lowId: {1}] }}", Owner.BattleTag, BnetEntityId.Low);
		}

		//TODO: figure out what 1 and 3 represent, or if it is a flag since all observed values are powers of 2 so far /dustinconrad
		public enum AwayStatusFlag : uint
		{
			Available = 0x00,
			UnknownStatus1 = 0x01,
			Away = 0x02,
			UnknownStatus2 = 0x03,
			Busy = 0x04
		}

		[Flags]
		public enum GameAccountFlags : uint
		{
			None = 0x00,
			HardcoreUnlocked = 0x01,
			AdventureModeUnlocked = 0x04,
			Paragon100 = 0x08,
			MasterUnlocked = 0x10,
			TormentUnlocked = 0x20,
			AdventureModeTutorial = 0x40,
			HardcoreMasterUnlocked = 0x80,
			HardcoreTormentUnlocked = 0x100,
			HardcoreAdventureModeUnlocked = 0x200
		}
	}
}
