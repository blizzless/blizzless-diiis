using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Helpers;
using DiIiS_NA.LoginServer.Objects;
using NHibernate.Linq;

namespace DiIiS_NA.LoginServer.Toons
{
	public class Toon : PersistentRPCObject
	{
		#region Cosmetics

		public int Cosmetic1
		{
			get => DBToon.Cosmetic1;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Cosmetic1 = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int Cosmetic2
		{
			get => DBToon.Cosmetic2;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Cosmetic2 = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int Cosmetic3
		{
			get => DBToon.Cosmetic3;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Cosmetic3 = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int Cosmetic4
		{
			get => DBToon.Cosmetic4;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Cosmetic4 = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

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
			get { return DBSessions.SessionQuerySingle<DBActiveSkills>(s => s.DBToon.Id == PersistentID); }
			set { }
		}

		public IntPresenceField HeroClassField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 1, 0,
					ClassID);
				return val;
			}
		}

		public IntPresenceField HeroLevelField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 2, 0,
					Level);
				return val;
			}
		}

		public IntPresenceField HeroFlagsField
		{
			get
			{
				var tFlags = Flags;
				if (IsHardcore) tFlags |= ToonFlags.Hardcore;
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 4, 0,
					(int)tFlags);
				return val;
			}
		}

		public IntPresenceField HighestUnlockedAct =
			new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 6, 0, 3000);

		public IntPresenceField HeroParagonLevelField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Hero, 8, 0,
					ParagonLevel);
				return val;
			}
		}

		public StringPresenceField HeroNameField => new StringPresenceField(FieldKeyHelper.Program.D3,
			FieldKeyHelper.OriginatingClass.Hero, 5, 0, _heroName);

		private D3.Hero.VisualEquipment _visualEquipment = null;
		public bool _visualEquipmentChanged = true;

		public ByteStringPresenceField<D3.Hero.VisualEquipment> HeroVisualEquipmentField
		{
			get
			{
				if (_visualEquipmentChanged)
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
					var visibleEquipment = DBSessions.SessionQueryWhere<DBInventory>(inv =>
						inv.DBToon.Id == PersistentID && inv.EquipmentSlot > 0 && inv.EquipmentSlot < 15 &&
						inv.ForSale == false);

					foreach (var inv in visibleEquipment)
					{
						var slot = inv.EquipmentSlot;
						if (!_visualToSlotMapping.ContainsKey(slot))
							continue;
						// decode vislual slot from equipment slot
						slot = _visualToSlotMapping[slot];
						visualItems[slot] = D3.Hero.VisualItem.CreateBuilder()
							.SetGbid(inv.TransmogGBID == -1 ? inv.GbId : inv.TransmogGBID)
							.SetDyeType(inv.DyeType)
							.SetEffectLevel(0)
							.Build();
					}

					_visualEquipment = D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems)
						.AddRangeCosmeticItem(CosmeticItems).Build();
					_visualEquipmentChanged = false;
				}

				return new ByteStringPresenceField<D3.Hero.VisualEquipment>(FieldKeyHelper.Program.D3,
					FieldKeyHelper.OriginatingClass.Hero, 3, 0, _visualEquipment);
			}
		}

		public IntPresenceField HighestUnlockedDifficulty => new IntPresenceField(FieldKeyHelper.Program.D3,
			FieldKeyHelper.OriginatingClass.Hero, 7, 0, 9);

		/// <summary>
		/// D3 EntityID encoded id.
		/// </summary>
		public D3.OnlineService.EntityId D3EntityID { get; private set; }

		/// <summary>
		/// True if toon has been recently deleted;
		/// </summary>
		public bool Deleted
		{
			get => DBToon.Deleted;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Deleted = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool IsSeasoned { get; set; }

		public int SeasonCreated
		{
			get => DBToon.CreatedSeason;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.CreatedSeason = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool StoneOfPortal
		{
			get => DBToon.StoneOfPortal;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.StoneOfPortal = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool Dead
		{
			get => DBToon.Dead;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
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
			get => DBToon.Archieved;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Archieved = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public bool LevelingBoosted
		{
			get
			{
				return DBSessions
					.SessionQueryWhere<DBBonusSets>(dbi => dbi.SetId == 6 && dbi.ClaimedToon.Id == PersistentID).Any();
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
			get =>
				//return this.IsHardcore ? string.Format("{{c_green}}{0}{{/c}}", this._heroName) : this._heroName;
				_heroName; //this.IsHardcore ? this.isSeassoned ? string.Format("{{c_yellow}}{0}{{/c}}", this._heroName) : string.Format("{{c_red}}{0}{{/c}}", this._heroName) : this.isSeassoned ? string.Format("{{c_green}}{0}{{/c}}", this._heroName) : this._heroName;
			set
			{
				_heroName = value;
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.Name = value;
					DBSessions.SessionUpdate(dbToon);
				}

				HeroNameField.Value = value;
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
			get => GameAccountManager.GetAccountByPersistentID(GameAccountId);
			set
			{
				GameAccountId = value.PersistentID;
				lock (DBToon)
				{
					var dbToon = DBToon;
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
			get => _toonClass;
			private set
			{
				/*var dbToon = this.DBToon;
				dbToon.Class = value;
				DBSessions.SessionUpdate(dbToon);*/ //not needed

				_toonClass = value;

				switch (_toonClass)
				{
					case ToonClass.Barbarian:
						HeroClassField.Value = 0x4FB91EE2;
						break;
					case ToonClass.Crusader:
						HeroClassField.Value = unchecked((int)0xBE27DC19);
						break;
					case ToonClass.DemonHunter:
						HeroClassField.Value = unchecked((int)0xC88B9649);
						break;
					case ToonClass.Monk:
						HeroClassField.Value = 0x3DAC15;
						break;
					case ToonClass.WitchDoctor:
						HeroClassField.Value = 0x343C22A;
						break;
					case ToonClass.Wizard:
						HeroClassField.Value = 0x1D4681B1;
						break;
					case ToonClass.Necromancer:
						HeroClassField.Value = 0x8D4D94ED; //unchecked((int)0x8D4D94ED);
						break;
					default:
						HeroClassField.Value = 0x0;
						break;
				}
			}
		}

		/// <summary>
		/// Toon's flags.
		/// </summary>
		public ToonFlags Flags
		{
			get => _flags; // | ToonFlags.AllUnknowns;
			set
			{
				_flags = value;
				lock (DBToon)
				{
					var dbToon = DBToon;
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
				if (_levelChanged || !LoginServerConfig.Instance.Enabled)
				{
					_cachedLevel = DBToon.Level;
					_levelChanged = false;
				}

				return _cachedLevel;
			}
			private set
			{
				lock (DBToon)
				{
					_cachedLevel = value;
					var dbToon = DBToon;
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
				if (!_paragonLevelChanged && LoginServerConfig.Instance.Enabled) return _cachedParagonLevel;
				_cachedParagonLevel = GameAccount.DBGameAccount.ParagonLevel;
				_paragonLevelChanged = false;

				return _cachedParagonLevel;
			}
			private set
			{
				lock (GameAccount.DBGameAccount)
				{
					_cachedParagonLevel = value;
					var dbGAcc = GameAccount.DBGameAccount;
					if (IsHardcore)
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
			get => (Level >= 70 ? ParagonExperienceNext : DBToon.Experience);
			set
			{
				if (Level >= 70)
					ParagonExperienceNext = value;
				else
				{
					lock (DBToon)
					{
						var dbToon = DBToon;
						dbToon.Experience = value;
						DBSessions.SessionUpdate(dbToon);
					}
				}
			}
		}

		public long ParagonExperienceNext
		{
			get => (IsHardcore ? GameAccount.DBGameAccount.ExperienceHardcore : GameAccount.DBGameAccount.Experience);
			set
			{
				lock (GameAccount.DBGameAccount)
				{
					var dbGAcc = GameAccount.DBGameAccount;
					if (IsHardcore)
						dbGAcc.ExperienceHardcore = value;
					else
						dbGAcc.Experience = value;
					DBSessions.SessionUpdate(dbGAcc);
				}
			}
		}

		public ActEnum CurrentAct
		{
			get => (ActEnum)DBToon.CurrentAct;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.CurrentAct = (int)value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentQuestId
		{
			get => DBToon.CurrentQuestId;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.CurrentQuestId = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int PvERating
		{
			get => DBToon.PvERating;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.PvERating = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentQuestStepId
		{
			get => DBToon.CurrentQuestStepId;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.CurrentQuestStepId = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		public int CurrentDifficulty
		{
			get => DBToon.CurrentDifficulty;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
					dbToon.CurrentDifficulty = value;
					DBSessions.SessionUpdate(dbToon);
				}
			}
		}

		/// <summary>
		/// Killed monsters(total for account)
		/// </summary>
		public ulong TotalKilled
		{
			get => GameAccount.DBGameAccount.TotalKilled;
			set
			{
				var dbGA = GameAccount.DBGameAccount;
				lock (dbGA)
				{
					dbGA.TotalKilled = value;
					DBSessions.SessionUpdate(dbGA);
				}
			}
		}

		/// <summary>
		/// Killed elites(total for account)
		/// </summary>
		public ulong ElitesKilled
		{
			get => GameAccount.DBGameAccount.ElitesKilled;
			set
			{
				var dbGA = GameAccount.DBGameAccount;
				lock (dbGA)
				{
					dbGA.ElitesKilled = value;
					DBSessions.SessionUpdate(dbGA);
				}
			}
		}

		/// <summary>
		/// Bounties completed(total for account)
		/// </summary>
		public int TotalBounties
		{
			get
			{
				if (IsHardcore)
				{
					return GameAccount.DBGameAccount.TotalBountiesHardcore;
				}
				else
				{
					return GameAccount.DBGameAccount.TotalBounties;
				}
			}
			set
			{
				var dbGA = GameAccount.DBGameAccount;
				lock (dbGA)
				{
					if (IsHardcore)
					{
						dbGA.TotalBountiesHardcore = value;
					}
					else
					{
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
			get => DBToon.Kills;
			set
			{
				var dbToon = DBToon;
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
		public ulong CollectedGold
		{
			get => IsHardcore ? GameAccount.DBGameAccount.HardTotalGold : GameAccount.DBGameAccount.TotalGold;
			set
			{
				var dbGAcc = GameAccount.DBGameAccount;
				lock (dbGAcc)
				{
					if (IsHardcore)
					{
						dbGAcc.HardTotalGold = value;
					}
					else
					{
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
			get => DBToon.GoldGained;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
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
			get => DBToon.TimePlayed;
			set
			{
				lock (DBToon)
				{
					var dbToon = DBToon;
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
			get => D3.Client.ToonSettings.CreateBuilder().SetUiFlags(0xFFFFFFFF).Build(); //this._settings;
			set => _settings = value;
		}

		/// <summary>
		/// Toon digest.
		/// </summary>

		public D3.Hero.Digest Digest
		{
			get
			{
				var dbToon = DBToon;
				if (IsHardcore) dbToon.Flags |= ToonFlags.Hardcore;
				//var isSeason = Convert.ToUInt16(isSeassoned);

				var digest = D3.Hero.Digest.CreateBuilder().SetVersion(905)
					.SetHeroId(D3EntityID)
					.SetHeroName(Name)
					.SetGbidClass((int)ClassID)
					.SetLevel(Level)
					//deprecated //.SetAltLevel(dbToon.ParagonLevel)
					.SetPlayerFlags((uint)dbToon.Flags) // + isSeason)
					.SetSeasonCreated((uint)SeasonCreated)

					.SetVisualEquipment(HeroVisualEquipmentField.Value)
					.SetLastPlayedAct(dbToon.CurrentAct)
					.SetHighestUnlockedAct(3000)
					//deprecated //.SetLastPlayedDifficulty(dbToon.CurrentDifficulty)

					//deprecated //.SetHighestCompletedDifficulty(0)
					.SetHighestSoloRiftCompleted(3)
					.SetLastPlayedQuest(dbToon.CurrentQuestId)
					.SetLastPlayedQuestStep(dbToon.CurrentQuestStepId)
					.SetTimePlayed((uint)dbToon.TimePlayed);

				if (!IsHardcore)
				{
					foreach (var quest in _allQuests)
					{
						digest.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetSnoQuest(quest));
					}
				}
				else
				{
					var dbQuests = DBSessions.SessionQueryWhere<DBQuestHistory>(dbi => dbi.DBToon.Id == PersistentID);
#if DEBUG
					digest
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(87700))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72095))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72221))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72061))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(117779))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72738))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72738))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(73236))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72546))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(72801))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(136656))
						//2 act
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(80322))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(93396))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(74128))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(57331))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(78264))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(78266))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(57335))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(57337))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(121792))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(57339))
						//3 act
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(93595))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(93684))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(93697))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(203595))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(101756))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(101750))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(101758))
						//4 act
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(112498))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(113910))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(114795))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(114901))
						//5 act
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(251355))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(284683))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(285098))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(257120))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(263851))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(273790))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(269552))
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0)
							.SetSnoQuest(273408))
						;
#else
										foreach (var inv in dbQuests)
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
						.AddQuestHistory(D3.Hero.QuestHistoryEntry.CreateBuilder().SetDifficultyDeprecated(0).SetSnoQuest(251355));
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
				var heroInventoryItems =
					DBSessions.SessionQueryWhere<DBInventory>(dbi => dbi.DBToon.Id == PersistentID).ToImmutableArray();
				//*
				foreach (var invItem in heroInventoryItems)
				{
					if (invItem.EquipmentSlot is 0 or 15) continue;
					var item = D3.Items.SavedItem.CreateBuilder()
						.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0)
							.SetIdHigh(0x3C000002517A293 + invItem.Id))
						.SetHirelingClass(0)
						.SetItemSlot(272 + invItem.EquipmentSlot * 16)
						.SetOwnerEntityId(D3EntityID)
						.SetSquareIndex(0)
						.SetUsedSocketCount(0);

					var generator = D3.Items.Generator.CreateBuilder()
						.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.GbId).SetGameBalanceType(2))
						.SetStackSize(0)
						.SetDyeType((uint)invItem.DyeType)
						.SetFlags((uint)((invItem.Version == 1 ||
						                  GameServer.GSSystem.ItemsSystem.ItemGenerator.IsCrafted(invItem.Attributes))
							? 2147483647
							: 10633))
						.SetSeed((uint)GameServer.GSSystem.ItemsSystem.ItemGenerator.GetSeed(invItem.Attributes))
						.SetTransmogGbid(invItem.TransmogGBID)
						.SetDurability(509);
					if (invItem.RareItemName != null)
						generator.SetRareItemName(D3.Items.RareItemName.ParseFrom(invItem.RareItemName));
					List<string> affixes = invItem.Affixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
						.ToList();
					//if (affixes.Count() > 3 ) generator.SetFlags(10633);
					generator.SetItemQualityLevel(invItem.Quality);
					foreach (string affix in affixes)
					{
						Int32.TryParse(affix, out var result);
						generator.AddBaseAffixes(result);
					}

					#region gems

					if (invItem.FirstGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0)
								.SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.FirstGem)
									.SetGameBalanceType(2))
								.SetFlags(2147483647)
								.SetSeed(0)
								.SetDurability(509)
								.SetStackSize(0)
							)
						);

					if (invItem.SecondGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0)
								.SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.SecondGem)
									.SetGameBalanceType(2))
								.SetFlags(2147483647)
								.SetSeed(0)
								.SetDurability(509)
								.SetStackSize(0)
							)
						);

					if (invItem.ThirdGem != -1)
						generator.AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
							.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0)
								.SetIdHigh(0x3C000002517A293 + (ulong)invItem.Id))
							.SetGenerator(D3.Items.Generator.CreateBuilder()
								.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(invItem.ThirdGem)
									.SetGameBalanceType(2))
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
				var dbToon = DBToon;
				string[] stats = dbToon.Stats.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				var profile = D3.Profile.HeroProfile.CreateBuilder()
					.SetHardcore(IsHardcore)
					.SetDeathTime(1476016727)
					//deprecated //.SetLife(0)
					.SetSnoKillLocation(71150)
					.SetKillerInfo(D3.Profile.KillerInfo.CreateBuilder().SetSnoKiller(6031).SetRarity(4))
					.SetHeroId(D3EntityID)
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
				if (DBActiveSkills != null)
				{
					var dbActiveSkills = DBActiveSkills;
					var skills = new[]
					{
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill0)
							.SetRuneType(dbActiveSkills.Rune0).Build(),
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill1)
							.SetRuneType(dbActiveSkills.Rune1).Build(),
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill2)
							.SetRuneType(dbActiveSkills.Rune2).Build(),
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill3)
							.SetRuneType(dbActiveSkills.Rune3).Build(),
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill4)
							.SetRuneType(dbActiveSkills.Rune4).Build(),
						D3.Profile.SkillWithRune.CreateBuilder()
							.SetSkill(dbActiveSkills.Skill5)
							.SetRuneType(dbActiveSkills.Rune5).Build()
					};
					var passives = new[]
					{
						dbActiveSkills.Passive0, dbActiveSkills.Passive1, dbActiveSkills.Passive2,
						dbActiveSkills.Passive3
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
				if (!GameAccount.IsOnline) return false;
				if (GameAccount.CurrentToon != null)
					return GameAccount.CurrentToon == this;
				return false;
			}
		}

		public int ClassID =>
			Class switch
			{
				ToonClass.Barbarian => 0x4FB91EE2,
				ToonClass.Crusader => unchecked((int)0xBE27DC19),
				ToonClass.DemonHunter => unchecked((int)0xC88B9649),
				ToonClass.Monk => 0x3DAC15,
				ToonClass.WitchDoctor => 0x343C22A,
				ToonClass.Wizard => 0x1D4681B1,
				ToonClass.Necromancer => unchecked((int)0x8D4D94ED),
				_ => 0x0
			};

		// Used for Conversations
		public int VoiceClassID =>
			Class switch
			{
				ToonClass.DemonHunter => 0,
				ToonClass.Barbarian => 1,
				ToonClass.Wizard => 2,
				ToonClass.WitchDoctor => 3,
				ToonClass.Monk => 4,
				ToonClass.Crusader => 5,
				ToonClass.Necromancer => 6,
				_ => 0
			};

		public int Gender => (int)(Flags & ToonFlags.Female);

		#region c-tor and setfields

		public readonly Core.MPQ.FileFormats.GameBalance.HeroTable HeroTable;

		private readonly Dictionary<int, int> _visualToSlotMapping = new() { { 1, 0 }, { 2, 1 }, { 7, 2 }, { 5, 3 }, { 4, 4 }, { 3, 5 }, { 8, 6 }, { 9, 7 } };

		private static readonly Core.MPQ.FileFormats.GameBalance HeroData =
			(Core.MPQ.FileFormats.GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19740].Data;

		public Toon(DBToon dbToon, GameDBSession DBSession = null)
			: base(dbToon.Id)
		{
			D3EntityID = D3.OnlineService.EntityId.CreateBuilder().SetIdHigh((ulong)EntityIdHelper.HighIdType.ToonId)
				.SetIdLow(PersistentID).Build();
			_heroName = dbToon.Name;
			_flags = dbToon.Flags;
			GameAccountId = dbToon.DBGameAccount.Id;
			_toonClass = dbToon.Class;

			DBToon = dbToon;
			this.DBSession = DBSession;
			IsHardcore = dbToon.isHardcore;
			IsSeasoned = dbToon.isSeasoned;
			HeroTable = HeroData.Heros.Find(item => item.Name == Class.ToString());
		}

		#endregion

		public void LevelUp()
		{
			Level++;
			GameAccount.ChangedFields.SetIntPresenceFieldValue(HeroLevelField);
		}

		public void ParagonLevelUp()
		{
			ParagonLevel++;
			GameAccount.ChangedFields.SetIntPresenceFieldValue(HeroParagonLevelField);
		}

		private readonly List<int> _allQuests = new()
		{
			87700, 72095, 72221, 72061, 117779, 72738, 73236, 72546, 72801, 136656, 80322, 93396, 74128, 57331, 78264,
			78266, 57335, 57337, 121792, 57339, 93595, 93684, 93697, 203595, 101756, 101750, 101758, 112498, 113910,
			114795, 114901, 251355, 284683, 285098, 257120, 263851, 273790, 269552, 273408
		};

		public void UnlockAllQuests()
		{
			var questList = DBSessions.SessionQueryWhere<DBQuestHistory>(qh => qh.DBToon.Id == PersistentID);

			foreach (var quest in _allQuests)
			{
				if (questList.All(qh => qh.QuestId != quest))
				{
					var questHistory = new DBQuestHistory
					{
						DBToon = DBToon,
						QuestId = quest,
						QuestStep = -1,
						isCompleted = true
					};
					DBSessions.SessionSave(questHistory);
				}
				else
				{
					var questHistory = questList.First(qh => qh.QuestId == quest);
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
			return new List<bgs.protocol.presence.v1.FieldOperation>
			{
				HeroClassField.GetFieldOperation(),
				HeroLevelField.GetFieldOperation(),
				HeroParagonLevelField.GetFieldOperation(),
				HeroVisualEquipmentField.GetFieldOperation(),
				HeroFlagsField.GetFieldOperation(),
				HeroNameField.GetFieldOperation(),
				HighestUnlockedAct.GetFieldOperation(),
				HighestUnlockedDifficulty.GetFieldOperation()
			};
		}

		#endregion

		public static ToonClass GetClassByID(int classId) =>
			classId switch
			{
				0x4FB91EE2 => ToonClass.Barbarian,
				unchecked((int)0xBE27DC19) => ToonClass.Crusader,
				unchecked((int)0xC88B9649) => ToonClass.DemonHunter,
				0x003DAC15 => ToonClass.Monk,
				0x0343C22A => ToonClass.WitchDoctor,
				0x1D4681B1 => ToonClass.Wizard,
				unchecked((int)0x8D4D94ED) => ToonClass.Necromancer,
				_ => ToonClass.Barbarian
			};

		public override string ToString()
		{
			return $"{{ Toon: {Name} [lowId: {D3EntityID.IdLow}] }}";
		}

	}

	#region Definitions and Enums

	//Order is important as actor voices and saved data is based on enum index
	public enum ToonClass // : uint
	{
		Barbarian, // = 0x4FB91EE2,
		Monk, //= 0x3DAC15,
		DemonHunter, // = 0xC88B9649,
		WitchDoctor, // = 0x343C22A,
		Wizard, // = 0x1D4681B1
		Crusader, // = 0xBE27DC19
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