using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Helpers;
using DiIiS_NA.LoginServer.Objects;
using NHibernate.Linq;

namespace DiIiS_NA.LoginServer.Toons
{
	public class Toon : PersistentRPCObject
	{
        #region Cosmetics
        public int Cosmetic1 { get { return this.DBToon.Cosmetic1; } set { lock (this.DBToon) { var dbToon = this.DBToon; dbToon.Cosmetic1 = value; DBSessions.SessionUpdate(dbToon); } } }
		public int Cosmetic2 { get { return this.DBToon.Cosmetic2; } set { lock (this.DBToon) { var dbToon = this.DBToon; dbToon.Cosmetic2 = value; DBSessions.SessionUpdate(dbToon); } } }
		public int Cosmetic3 { get { return this.DBToon.Cosmetic3; } set { lock (this.DBToon) { var dbToon = this.DBToon; dbToon.Cosmetic3 = value; DBSessions.SessionUpdate(dbToon); } } }
		public int Cosmetic4 { get { return this.DBToon.Cosmetic4; } set { lock (this.DBToon) { var dbToon = this.DBToon; dbToon.Cosmetic4 = value; DBSessions.SessionUpdate(dbToon); } } }
        #endregion

        public DBToon DBToon
		{
			get;
				/*
			{
				if (LoginServer.Config.Instance.Enabled)
				{
					if (this.GameAccount.IsOnline)
						this.CachedDBToon = DBSessions.SessionGet<DBToon>(this.PersistentID);
					return this.CachedDBToon;
				}
				else
					return DBSessions.SessionGet<DBToon>(this.PersistentID);
			}
			//*/
			set;
		}

		private DBToon CachedDBToon { get; set; }

		public bool IsHardcore { get; set; }

		public DBActiveSkills DBActiveSkills
		{
			get
			{
				return DBSessions.SessionQuerySingle<DBActiveSkills>(s => s.DBToon.Id == this.PersistentID);
			}
			set { }
		}

		public IntPresenceField HeroClassField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 1, 0, this.ClassID);
				return val;
			}
		}

		public IntPresenceField HeroLevelField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 2, 0, this.Level);
				return val;
			}
		}

		public IntPresenceField HeroFlagsField
		{
			get
			{
				var tFlags = this.Flags;
				if (this.IsHardcore) tFlags |= ToonFlags.Hardcore;
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 4, 0, (int)tFlags);
				return val;
			}
		}

		public IntPresenceField HighestUnlockedAct = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 6, 0, 3000);

		public IntPresenceField HeroParagonLevelField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 8, 0, this.ParagonLevel);
				return val;
			}
		}

		public StringPresenceField HeroNameField
		{ get { return new StringPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 5, 0, this._heroName); } }

		private D3.Hero.VisualEquipment _visualEquipment = null;
		public bool _visualEquipmentChanged = true;

		public ByteStringPresenceField<D3.Hero.VisualEquipment> HeroVisualEquipmentField
		{
			get
			{
				if (this._visualEquipmentChanged)
				{
					var visualItems = new[]
				{
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Head
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Chest
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Feet
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Hands
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Weapon (1)
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Weapon (2)
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Shoulders
					D3.Hero.VisualItem.CreateBuilder().SetEffectLevel(0).Build(), // Legs
				};
					var CosmeticItems = new[]
					{
					D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(Cosmetic1).Build(), // Wings
					D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(Cosmetic2).Build(), // Flag
					D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(Cosmetic3).Build(), // Pet
					D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(Cosmetic4).Build(), // Frame
				};
					var visibleEquipment = DBSessions.SessionQueryWhere<DBInventory>(inv => inv.DBToon.Id == this.PersistentID && inv.EquipmentSlot > 0 && inv.EquipmentSlot < 15 && inv.ForSale == false);

					foreach (var inv in visibleEquipment)
					{
						var slot = inv.EquipmentSlot;
						if (!visualToSlotMapping.ContainsKey(slot))
							continue;
						// decode vislual slot from equipment slot
						slot = visualToSlotMapping[slot];
						visualItems[slot] = D3.Hero.VisualItem.CreateBuilder()
							.SetGbid(inv.TransmogGBID == -1 ? inv.GbId : inv.TransmogGBID)
							.SetDyeType(inv.DyeType)
							.SetEffectLevel(0)
							.Build();
					}

					this._visualEquipment = D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems).AddRangeCosmeticItem(CosmeticItems).Build();
					this._visualEquipmentChanged = false;
				}
				return new ByteStringPresenceField<D3.Hero.VisualEquipment>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 3, 0, _visualEquipment);
			}
		}

		public IntPresenceField HighestUnlockedDifficulty
		{ get { return new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 7, 0, 9); } }

		/// <summary>
		/// D3 EntityID encoded id.
		/// </summary>
		public D3.OnlineService.EntityId D3EntityID { get; private set; }

		/// <summary>
		/// True if toon has been recently deleted;
		/// </summary>
		public bool Deleted
		{
			get { return this.DBToon.Deleted; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.Deleted = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool isSeassoned
		{
			get;
			set;
		}

		public int SeasonCreated
		{
			get { return this.DBToon.CreatedSeason; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.CreatedSeason = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool StoneOfPortal
		{
			get { return this.DBToon.StoneOfPortal; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.StoneOfPortal = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool Dead
		{
			get { return this.DBToon.Dead; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.Dead = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// True if toon has been recently archieved to Hall of Fallen;
		/// </summary>
		public bool Archieved
		{
			get { return this.DBToon.Archieved; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.Archieved = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool LevelingBoosted
		{
			get
			{
				var _editions = DBSessions.SessionQueryWhere<DBBonusSets>(dbi => dbi.SetId == 6 && dbi.ClaimedToon.Id == this.PersistentID);
				return _editions.Count() > 0;
			}

			set { }
		}

		/// <summary>
		/// Toon handle struct.
		/// </summary>
		public ToonHandleHelper ToonHandle { get; private set; }

		/// <summary>
		/// Toon's name.
		/// </summary>
		public string Name
		{
			get
			{
				//return this.IsHardcore ? string.Format("{{c_green}}{0}{{/c}}", this._heroName) : this._heroName;
				return this._heroName;//this.IsHardcore ? this.isSeassoned ? string.Format("{{c_yellow}}{0}{{/c}}", this._heroName) : string.Format("{{c_red}}{0}{{/c}}", this._heroName) : this.isSeassoned ? string.Format("{{c_green}}{0}{{/c}}", this._heroName) : this._heroName;
			}
			set
			{
				this._heroName = value;
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.Name = value;
					DBSessions.SessionUpdate(dbToon);
				}

				this.HeroNameField.Value = value;
			}
		}

		private string _heroName = "";

		/*
		/// <summary>
		/// Toon's hash-code.
		/// </summary>
		public int HashCode { get; set; }
		*/

		public ulong GameAccountId = 0;

		/// <summary>
		/// Toon's owner account.
		/// </summary>
		public GameAccount GameAccount
		{
			get
			{
				return GameAccountManager.GetAccountByPersistentID(this.GameAccountId);
			}
			set
			{
				this.GameAccountId = value.PersistentID;
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.DBGameAccount = value.DBGameAccount;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Toon's class.
		/// </summary>
		private ToonClass _toonClass = ToonClass.Unknown;

		public ToonClass Class
		{
			get
			{
				return _toonClass;
			}
			private set
			{
				/*var dbToon = this.DBToon;
				dbToon.Class = value;
				DBSessions.SessionUpdate(dbToon);*/ //not needed

				_toonClass = value;

				switch (_toonClass)
				{
					case ToonClass.Barbarian:
						this.HeroClassField.Value = 0x4FB91EE2;
						break;
					case ToonClass.Crusader:
						this.HeroClassField.Value = unchecked((int)0xBE27DC19);
						break;
					case ToonClass.DemonHunter:
						this.HeroClassField.Value = unchecked((int)0xC88B9649);
						break;
					case ToonClass.Monk:
						this.HeroClassField.Value = 0x3DAC15;
						break;
					case ToonClass.WitchDoctor:
						this.HeroClassField.Value = 0x343C22A;
						break;
					case ToonClass.Wizard:
						this.HeroClassField.Value = 0x1D4681B1;
						break;
					case ToonClass.Necromancer:
						this.HeroClassField.Value = 0x8D4D94ED;//unchecked((int)0x8D4D94ED);
						break;
					default:
						this.HeroClassField.Value = 0x0;
						break;
				}
			}
		}

		/// <summary>
		/// Toon's flags.
		/// </summary>
		public ToonFlags Flags
		{
			get
			{
				return this._flags;// | ToonFlags.AllUnknowns;
			}
			set
			{
				this._flags = value;
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.Flags = value;
					DBSessions.SessionUpdate(dbToon);
				}
				// | ToonFlags.AllUnknowns;
				//this.HeroFlagsField.Value = (int)(value | ToonFlags.AllUnknowns);
			}
		}

		private ToonFlags _flags;


		private byte _cachedLevel = 0;
		private bool _levelChanged = true;
		/// <summary>
		/// Toon's level.
		/// </summary>
		public byte Level
		{
			get
			{
				if (_levelChanged || !LoginServer.Config.Instance.Enabled)
				{
					_cachedLevel = this.DBToon.Level;
					_levelChanged = false;
				}
				return this._cachedLevel;
			}
			private set
			{
				lock (this.DBToon)
				{
					_cachedLevel = value;
					var dbToon = this.DBToon;
					dbToon.Level = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		private int _cachedParagonLevel = 0;
		public bool _paragonLevelChanged = true;

		/// <summary>
		/// Toon's Paragon level.
		/// </summary>
		public int ParagonLevel
		{
			get
			{
				if (_paragonLevelChanged || !LoginServer.Config.Instance.Enabled)
				{
					this._cachedParagonLevel = this.GameAccount.DBGameAccount.ParagonLevel;
					_paragonLevelChanged = false;
				}
				return this._cachedParagonLevel;
			}
			private set
			{
				lock (this.GameAccount.DBGameAccount)
				{
					this._cachedParagonLevel = value;
					var dbGAcc = this.GameAccount.DBGameAccount;
					if (this.IsHardcore)
						dbGAcc.ParagonLevelHardcore = value;
					else
						dbGAcc.ParagonLevel = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		/// <summary>
		/// Experience to next level
		/// </summary>
		public long ExperienceNext
		{
			get { return (this.Level >= 70 ? this.ParagonExperienceNext : this.DBToon.Experience); }
			set
			{
				if (this.Level >= 70)
					this.ParagonExperienceNext = value;
				else
				{
					lock (this.DBToon)
					{
						var dbToon = this.DBToon;
						dbToon.Experience = value;
						DBSessions.SessionUpdate(dbToon);
					}
				}
			}
		}

		public long ParagonExperienceNext
		{
			get { return (this.IsHardcore ? this.GameAccount.DBGameAccount.ExperienceHardcore : this.GameAccount.DBGameAccount.Experience); }
			set
			{
				lock (this.GameAccount.DBGameAccount)
				{
					var dbGAcc = this.GameAccount.DBGameAccount;
					if (this.IsHardcore)
						dbGAcc.ExperienceHardcore = value;
					else
						dbGAcc.Experience = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public int CurrentAct
		{
			get { return this.DBToon.CurrentAct; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.CurrentAct = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentQuestId
		{
			get { return this.DBToon.CurrentQuestId; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.CurrentQuestId = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int PvERating
		{
			get { return this.DBToon.PvERating; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.PvERating = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentQuestStepId
		{
			get { return this.DBToon.CurrentQuestStepId; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.CurrentQuestStepId = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentDifficulty
		{
			get { return this.DBToon.CurrentDifficulty; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.CurrentDifficulty = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Killed monsters(total for account)
		/// </summary>
		public ulong TotalKilled {
			get {
				return this.GameAccount.DBGameAccount.TotalKilled;
			}
			set {
				var dbGA = this.GameAccount.DBGameAccount;
				lock (dbGA) {
					dbGA.TotalKilled = value;
					DBSessions.SessionUpdate(dbGA);
				}
			}
		}
		/// <summary>
		/// Killed elites(total for account)
		/// </summary>
		public ulong ElitesKilled {
			get {
				return this.GameAccount.DBGameAccount.ElitesKilled;
			}
			set {
				var dbGA = this.GameAccount.DBGameAccount;
				lock (dbGA) {
					dbGA.ElitesKilled = value;
					DBSessions.SessionUpdate(dbGA);
				}
			}
		}

		/// <summary>
		/// Bounties completed(total for account)
		/// </summary>
		public int TotalBounties {
			get {
				if (this.IsHardcore) {
					return this.GameAccount.DBGameAccount.TotalBountiesHardcore;
				}
				else {
					return this.GameAccount.DBGameAccount.TotalBounties;
				}
			}
			set {
				var dbGA = this.GameAccount.DBGameAccount;
				lock (dbGA) {
					if (this.IsHardcore) {
						dbGA.TotalBountiesHardcore = value;
					}
					else {
						dbGA.TotalBounties = value;
					}
					DBSessions.SessionUpdate(dbGA);
				}
			}
		}

		/// <summary>
		/// KDR kills(seasonal)
		/// </summary>
		public int SeasonalKills
		{
			get { return this.DBToon.Kills; }
			set
			{
				var dbToon = this.DBToon;
				lock (dbToon)
				{
					dbToon.Kills = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Total collected Gold(total for account)
		/// </summary>
		public ulong CollectedGold {
			get {
				if (this.IsHardcore) {
					return this.GameAccount.DBGameAccount.HardTotalGold;
				}
				else {
					return this.GameAccount.DBGameAccount.TotalGold;
				}
			}
			set {
				var dbGAcc = this.GameAccount.DBGameAccount;
				lock (dbGAcc) {
					if (this.IsHardcore) {
						dbGAcc.HardTotalGold = value;
					}
					else {
						dbGAcc.TotalGold = value;
					}
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		/// <summary>
		/// Total collected Gold(season)
		/// </summary>
		public int CollectedGoldSeasonal
		{
			get { return this.DBToon.GoldGained; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.GoldGained = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Total time played for toon.
		/// </summary>
		public int TimePlayed
		{
			get { return this.DBToon.TimePlayed; }
			set
			{
				lock (this.DBToon)
				{
					var dbToon = this.DBToon;
					dbToon.TimePlayed = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Last login time for toon.
		/// </summary>
		public uint LoginTime { get; set; }

		/// <summary>
		/// Database handler for this toon
		/// </summary>
		public GameDBSession DBSession { get; set; }

		/// <summary>
		/// Settings for toon.
		/// </summary>
		private D3.Client.ToonSettings _settings = D3.Client.ToonSettings.CreateBuilder().Build();
		public D3.Client.ToonSettings Settings
		{
			get
			{
				return D3.Client.ToonSettings.CreateBuilder().SetUiFlags(0xFFFFFFFF).Build();//this._settings;
			}
			set
			{
				this._settings = value;
			}
		}

		/// <summary>
		/// Toon digest.
		/// </summary>

		public D3.Hero.Digest Digest
		{
			get
			{
				var dbToon = this.DBToon;
				if (this.IsHardcore) dbToon.Flags |= ToonFlags.Hardcore;
				//var isSeason = Convert.ToUInt16(isSeassoned);

				var digest = D3.Hero.Digest.CreateBuilder().SetVersion(905)
								.SetHeroId(this.D3EntityID)
								.SetHeroName(this.Name)
								.SetGbidClass((int)this.ClassID)
								.SetLevel(this.Level)
								//deprecated //.SetAltLevel(dbToon.ParagonLevel)
								.SetPlayerFlags((uint)dbToon.Flags)// + isSeason)
								.SetSeasonCreated((uint)this.SeasonCreated)
								
								.SetVisualEquipment(this.HeroVisualEquipmentField.Value)
								.SetLastPlayedAct(dbToon.CurrentAct)
								.SetHighestUnlockedAct(3000)
								//deprecated //.SetLastPlayedDifficulty(dbToon.CurrentDifficulty)
								
								//deprecated //.SetHighestCompletedDifficulty(0)
								.SetHighestSoloRiftCompleted(3)
								.SetLastPlayedQuest(dbToon.CurrentQuestId)
								.SetLastPlayedQuestStep(dbToon.CurrentQuestStepId)
								.SetTimePlayed((uint)dbToon.TimePlayed);

				if (!this.IsHardcore)
				{
					foreach (var quest in _allQuests)
					{
						digest.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetSnoQuest(quest));
					}
				}
				else
				{
					IEnumerable<DBQuestHistory> _dbQuests = null;
					_dbQuests = DBSessions.SessionQueryWhere<DBQuestHistory>(dbi => dbi.DBToon.Id == this.PersistentID);
#if DEBUG
					digest
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(87700))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72095))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72221))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72061))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(117779))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72738))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72738))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(73236))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72546))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(72801))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(136656))
						//2 Акт
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(80322))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(93396))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(74128))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(57331))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(78264))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(78266))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(57335))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(57337))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(121792))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(57339))
						//3 Акт
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(93595))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(93684))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(93697))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(203595))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(101756))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(101750))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(101758))
						//4 Акт
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(112498))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(113910))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(114795))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(114901))
						//5 Акт
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(251355))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(284683))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(285098))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(257120))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(263851))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(273790))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(269552))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(273408))
						;
#else
										foreach (var inv in _dbQuests)
										{
											// load quests
											var quest = D3.Hero.QuestHistoryEntry.CreateBuilder()
													.SetSnoQuest(inv.QuestId);
													//deprecated //.SetDifficulty(inv.Difficulty);
												if (inv.isCompleted != true)
													quest.SetHighestPlayedQuestStep(inv.QuestStep);
											digest.AddQuestHistory(quest);
										}
					digest
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(80322))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(93595))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(112498))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(251355))
						;

#endif
				}
				return digest.Build();
			}
		}
		//*/
		/// <summary>
		/// Hero Profile.
		/// </summary>
		public D3.Profile.HeroProfile Profile
		{
			get
			{
				var itemList = D3.Items.ItemList.CreateBuilder();
				List<DBInventory> heroInventoryItems = DBSessions.SessionQueryWhere<DBInventory>(dbi => dbi.DBToon.Id == this.PersistentID);
				//*
				foreach (var invItem in heroInventoryItems)
				{
					if ((invItem.EquipmentSlot) == 0 || (invItem.EquipmentSlot == 15)) continue;
					var item = D3.Items.SavedItem.CreateBuilder()
						.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0).SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
						.SetHirelingClass(0)
						.SetItemSlot(272 + (invItem.EquipmentSlot * 16))
						.SetOwnerEntityId(this.D3EntityID)
						.SetSquareIndex(0)
						.SetUsedSocketCount(0);

					var generator = D3.Items.Generator.CreateBuilder()
							.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.GbId).SetGameBalanceType(2))
							.SetStackSize(0)
							.SetDyeType((uint)invItem.DyeType)
							.SetFlags((uint)((invItem.Version == 1 || GameServer.GSSystem.ItemsSystem.ItemGenerator.IsCrafted(invItem.Attributes)) ? 2147483647 : 10633))
							.SetSeed((uint)GameServer.GSSystem.ItemsSystem.ItemGenerator.GetSeed(invItem.Attributes))
							.SetTransmogGbid(invItem.TransmogGBID)
							.SetDurability(509);
					if (invItem.RareItemName != null) generator.SetRareItemName(D3.Items.RareItemName.ParseFrom(invItem.RareItemName));
					List<string> affixes = invItem.Affixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					//if (affixes.Count() > 3 ) generator.SetFlags(10633);
					generator.SetItemQualityLevel(invItem.Quality);
					foreach (string affix in affixes)
					{
						int result = 0;
						Int32.TryParse(affix, out result);
						generator.AddBaseAffixes(result);
					}
					#region gems				
					if (invItem.FirstGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0).SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.FirstGem).SetGameBalanceType(2))
								.SetFlags(2147483647)
								.SetSeed(0)
								.SetDurability(509)
								.SetStackSize(0)
							)
					);

					if (invItem.SecondGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0).SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.SecondGem).SetGameBalanceType(2))
								.SetFlags(2147483647)
								.SetSeed(0)
								.SetDurability(509)
								.SetStackSize(0)
							)
					);

					if (invItem.ThirdGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0).SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.ThirdGem).SetGameBalanceType(2))
								.SetFlags(2147483647)
								.SetSeed(0)
								.SetDurability(509)
								.SetStackSize(0)
							)
					);
					#endregion

					item.SetGenerator(generator);
					itemList.AddItems(item.Build());
				}
				//*/
				var dbToon = this.DBToon;
				string[] stats = dbToon.Stats.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				var profile = D3.Profile.HeroProfile.CreateBuilder()
					.SetHardcore(this.IsHardcore)
					.SetDeathTime(1476016727)
					//deprecated //.SetLife(0)
					.SetSnoKillLocation(71150)
					.SetKillerInfo(D3.Profile.KillerInfo.CreateBuilder().SetSnoKiller(6031).SetRarity(4))
					.SetHeroId(this.D3EntityID)
					//deprecated //.SetHighestDifficulty(0)
					.SetHighestLevel(dbToon.Level)
					//.SetMonstersKilled(111)
					//.SetGoldCollected(999)
					.SetToughness(1)
					.SetMonstersKilled(1)
					.SetElitesKilled(1)
					.SetHealing(1)
					.SetCreateTime(0)
					.SetStrength(Convert.ToUInt32(stats[0], 10))
					.SetDexterity(Convert.ToUInt32(stats[1], 10))
					.SetIntelligence(Convert.ToUInt32(stats[2], 10))
					.SetVitality(Convert.ToUInt32(stats[3], 10))
					//deprecated //.SetArmor(Convert.ToUInt32(stats[4], 10))
					.SetDps(Convert.ToUInt32(stats[5], 10))
					//.SetResistArcane(106)
					//.SetResistFire(107)
					//.SetResistLightning(108)
					//.SetResistPoison(109)
					//.SetResistCold(110)
					//.SetResistPoison(111)
					.SetEquipment(itemList);
				if (this.DBActiveSkills != null)
				{
					var dbActiveSkills = this.DBActiveSkills;
					var skills = new[]{
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill0).SetRuneType(dbActiveSkills.Rune0).Build(),
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill1).SetRuneType(dbActiveSkills.Rune1).Build(),
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill2).SetRuneType(dbActiveSkills.Rune2).Build(),
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill3).SetRuneType(dbActiveSkills.Rune3).Build(),
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill4).SetRuneType(dbActiveSkills.Rune4).Build(),
							D3.Profile.SkillWithRune.CreateBuilder().SetSkill(dbActiveSkills.Skill5).SetRuneType(dbActiveSkills.Rune5).Build()
						};
					var passives = new[]{
							dbActiveSkills.Passive0, dbActiveSkills.Passive1, dbActiveSkills.Passive2, dbActiveSkills.Passive3
						};
					profile.SetSnoActiveSkills(D3.Profile.SkillsWithRunes.CreateBuilder().AddRangeRunes(skills));
					profile.SetSnoTraits(D3.Profile.PassiveSkills.CreateBuilder().AddRangeSnoTraits(passives));
					
				}
				profile.SetLegendaryPowers(D3.Profile.LegendaryPowers.CreateBuilder()
						.AddGbidLegendaryPowers(-1)
						.AddGbidLegendaryPowers(-1)
						.AddGbidLegendaryPowers(-1)
						)
						.SetPvpDamage(0)
					.SetPvpWins(0)
					.SetPvpGlory(0)
					.SetPvpTakedowns(0)
					;
				return profile.Build();
			}
		}

		public bool IsSelected
		{
			get
			{
				if (!this.GameAccount.IsOnline) return false;
				else
				{
					if (this.GameAccount.CurrentToon != null)
						return this.GameAccount.CurrentToon == this;
					else
						return false;
				}
			}
		}

		public int ClassID
		{
			get
			{
				switch (this.Class)
				{
					case ToonClass.Barbarian:
						return 0x4FB91EE2;
					case ToonClass.Crusader:
						return unchecked((int)0xBE27DC19);
					case ToonClass.DemonHunter:
						return unchecked((int)0xC88B9649);
					case ToonClass.Monk:
						return 0x3DAC15;
					case ToonClass.WitchDoctor:
						return 0x343C22A;
					case ToonClass.Wizard:
						return 0x1D4681B1;
					case ToonClass.Necromancer:
						return unchecked((int)0x8D4D94ED);
				}
				return 0x0;
			}
		}

		// Used for Conversations
		public int VoiceClassID
		{
			get
			{
				switch (this.Class)
				{
					case ToonClass.DemonHunter:
						return 0;
					case ToonClass.Barbarian:
						return 1;
					case ToonClass.Wizard:
						return 2;
					case ToonClass.WitchDoctor:
						return 3;
					case ToonClass.Monk:
						return 4;
					case ToonClass.Crusader:
						return 5;
					case ToonClass.Necromancer:
						return 6;
				}
				return 0;
			}
		}

		public int Gender
		{
			get
			{
				return (int)(this.Flags & ToonFlags.Female);
			}
		}

		#region c-tor and setfields

		public readonly Core.MPQ.FileFormats.GameBalance.HeroTable HeroTable;
		private readonly Dictionary<int, int> visualToSlotMapping = new Dictionary<int, int> { { 1, 0 }, { 2, 1 }, { 7, 2 }, { 5, 3 }, { 4, 4 }, { 3, 5 }, { 8, 6 }, { 9, 7 } };
		private static readonly DiIiS_NA.Core.MPQ.FileFormats.GameBalance HeroData = (DiIiS_NA.Core.MPQ.FileFormats.GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19740].Data;

		public Toon(DBToon dbToon, GameDBSession DBSession = null)
			: base(dbToon.Id)
		{
			this.D3EntityID = D3.OnlineService.EntityId.CreateBuilder().SetIdHigh((ulong)EntityIdHelper.HighIdType.ToonId).SetIdLow(this.PersistentID).Build();
			this._heroName = dbToon.Name;
			this._flags = dbToon.Flags;
			this.GameAccountId = dbToon.DBGameAccount.Id;
			this._toonClass = dbToon.Class;

			this.DBToon = dbToon;
			this.DBSession = DBSession;
			this.IsHardcore = dbToon.isHardcore;
			this.isSeassoned = dbToon.isSeasoned;
			this.HeroTable = HeroData.Heros.Find(item => item.Name == this.Class.ToString());
		}

		#endregion

		public void LevelUp()
		{
			this.Level++;
			this.GameAccount.ChangedFields.SetIntPresenceFieldValue(this.HeroLevelField);
		}

		public void ParagonLevelUp()
		{
			this.ParagonLevel++;
			this.GameAccount.ChangedFields.SetIntPresenceFieldValue(this.HeroParagonLevelField);
		}

		private List<int> _allQuests = new List<int>() { 87700, 72095, 72221, 72061, 117779, 72738, 73236, 72546, 72801, 136656, 80322, 93396, 74128, 57331, 78264, 78266, 57335, 57337, 121792, 57339, 93595, 93684, 93697, 203595, 101756, 101750, 101758, 112498, 113910, 114795, 114901, 251355, 284683, 285098, 257120, 263851, 273790, 269552, 273408 };

		public void UnlockAllQuests()
		{
			var questList = DBSessions.SessionQueryWhere<DBQuestHistory>(qh => qh.DBToon.Id == this.PersistentID);

			foreach (var quest in _allQuests)
			{
				if (!questList.Any(qh => qh.QuestId == quest))
				{
					var questHistory = new DBQuestHistory();
					questHistory.DBToon = this.DBToon;
					questHistory.QuestId = quest;
					questHistory.QuestStep = -1;
					questHistory.isCompleted = true;
					DBSessions.SessionSave(questHistory);
				}
				else
				{
					var questHistory = questList.Where(qh => qh.QuestId == quest).First();
					questHistory.isCompleted = true;
					DBSessions.SessionUpdate(questHistory);
				}
			}
		}

		public void StateChanged()
		{
			_levelChanged = true;
			_paragonLevelChanged = true;
			_visualEquipmentChanged = true;
		}
		#region Notifications

		//hero class generated
		//D3,Hero,1,0 -> D3.Hero.GbidClass: Hero Class
		//D3,Hero,2,0 -> D3.Hero.Level: Hero's current level
		//D3,Hero,3,0 -> D3.Hero.VisualEquipment: VisualEquipment
		//D3,Hero,4,0 -> D3.Hero.PlayerFlags: Hero's flags
		//D3,Hero,5,0 -> ?D3.Hero.NameText: Hero's Name
		//D3,Hero,6,0 -> Int64 - HighestUnlockedAct
		//D3,Hero,7,0 -> Int64 - HighestUnlockedDifficulty
		//D3,Hero,8,0 -> Int64 - ParagonLevel

		public override List<bgs.protocol.presence.v1.FieldOperation> GetSubscriptionNotifications()
		{
			var operationList = new List<bgs.protocol.presence.v1.FieldOperation>();
			operationList.Add(this.HeroClassField.GetFieldOperation());
			operationList.Add(this.HeroLevelField.GetFieldOperation());
			operationList.Add(this.HeroParagonLevelField.GetFieldOperation());
			operationList.Add(this.HeroVisualEquipmentField.GetFieldOperation());
			operationList.Add(this.HeroFlagsField.GetFieldOperation());
			operationList.Add(this.HeroNameField.GetFieldOperation());
			operationList.Add(this.HighestUnlockedAct.GetFieldOperation());
			operationList.Add(this.HighestUnlockedDifficulty.GetFieldOperation());

			return operationList;
		}

		#endregion

		public static ToonClass GetClassByID(int classId)
		{
			switch (classId)
			{
				case 0x4FB91EE2:
					return ToonClass.Barbarian;
				case unchecked((int)0xBE27DC19):
					return ToonClass.Crusader;
				case unchecked((int)0xC88B9649):
					return ToonClass.DemonHunter;
				case 0x003DAC15:
					return ToonClass.Monk;
				case 0x0343C22A:
					return ToonClass.WitchDoctor;
				case 0x1D4681B1:
					return ToonClass.Wizard;
				case unchecked((int)0x8D4D94ED):
					return ToonClass.Necromancer;
			}
			return ToonClass.Barbarian;
		}

		public override string ToString()
		{
			return String.Format("{{ Toon: {0} [lowId: {1}] }}", this.Name, this.D3EntityID.IdLow);
		}

	}

	#region Definitions and Enums
	//Order is important as actor voices and saved data is based on enum index
	public enum ToonClass// : uint
	{
		Barbarian,// = 0x4FB91EE2,
		Monk,//= 0x3DAC15,
		DemonHunter,// = 0xC88B9649,
		WitchDoctor,// = 0x343C22A,
		Wizard,// = 0x1D4681B1
		Crusader,// = 0xBE27DC19
		Necromancer,
		Unknown
	}

	[Flags]
	public enum ToonFlags : uint
	{
		Male = 0x00,
		Hardcore = 0x01,
		Female = 0x02,
		Fallen = 0x08,
		NeedRename = 0x20000000,
		// TODO: These two need to be figured out still.. /plash
		//Unknown1 = 0x20,
		Unknown2 = 0x40,
		Unknown3 = 0x80000,
		Unknown4 = 0x2000000,
		AllUnknowns = Unknown2 | Unknown3 | Unknown4
	}
	#endregion
}
