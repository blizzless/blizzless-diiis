using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.GSSystem.SkillsSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DiIiS_NA.LoginServer.Toons.Toon;

namespace DiIiS_NA.LoginServer.Toons
{
	public static class ToonManager
	{
		private static readonly ConcurrentDictionary<ulong, Toon> LoadedToons = new();
		private static readonly Logger Logger = LogManager.CreateLogger("DataBaseSystem");

		private static readonly DBInventory NewbiePants = new DBInventory
		{
			EquipmentSlot = 9,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = -1512732138,
			Durability = 1183,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1623510521|1.1345012E+20;33,:1086324736|6;36,:1086324736|6;37,:1086324736|6;38,:1094713344|12;31,:1086324736|6;381,:1183|1.658E-42;380,:1183|1.658E-42;388,57:0|0"
		};

		private static readonly DBInventory NewbieArmor = new DBInventory
		{
			EquipmentSlot = 2,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = 1612257704,
			Durability = 1115,
			Affixes = "",
			Attributes = "383,:0|0;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1287202746|97074640;33,:1077936128|3;36,:1077936128|3;37,:1077936128|3;38,:1086324736|6;31,:1077936128|3;381,:1115|1.562E-42;380,:1115|1.562E-42;388,57:0|0"
		};

		private static readonly DBInventory NewbieKnife = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = -635269584,
			Durability = 800,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1391170218|5.0588896E+11;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:800|1.121E-42;380,:800|1.121E-42;100,30592:1|1E-45;102,30592:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieAxe = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = 1661412389,
			Durability = 1000,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1941814752|3.0065772E+31;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:1000|1.401E-42;380,:1000|1.401E-42;100,30592:1|1E-45;102,30592:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieBow = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = unchecked((int)-2091504072),
			Durability = 920,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1256226286|7356151;194,:1068708659|1.4;196,:1068708659|1.4;198,:1068708659|1.4;538,:1068708659|1.4;540,:1068708659|1.4;546,:1068708659|1.4;201,:1068708659|1.4;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1065353216|1;224,0:1090519040|8;232,0:1065353216|1;225,0:1090519040|8;226,:1090519040|8;233,:1065353216|1;236,:1083179008|4.5;235,0:1083179008|4.5;542,0:1065353216|1;547,0:1065353216|1;220,0:1065353216|1;216,0:1065353216|1;543,0:0|0;234,0:1083179008|4.5;222,0:1088421888|7;223,0:1088421888|7;227,0:1088421888|7;228,:1088421888|7;544,0:1088421888|7;548,0:1088421888|7;213,0:1088421888|7;545,0:0|0;381,:920|1.289E-42;380,:920|1.289E-42;100,30599:1|1E-45;102,30599:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieFlail = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = -912456881,
			Durability = 1000,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1569321797|1.2425456E+18;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:1000|1.401E-42;380,:1000|1.401E-42;100,30592:1|1E-45;102,30592:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieWand = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = 88665049,
			Durability = 800,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:415523826|5.076573E-24;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:800|1.121E-42;380,:800|1.121E-42;100,30601:1|1E-45;102,30601:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieFist = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = 1236604967,
			Durability = 800,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1941814752|3.0065772E+31;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:1000|1.401E-42;380,:1000|1.401E-42;100,30592:1|1E-45;102,30592:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieNecromancer = new DBInventory
		{
			EquipmentSlot = 4,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = unchecked((int)111732407),
			Durability = 1000,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1007483702|0.008604815;194,:1067030938|1.2;196,:1067030938|1.2;198,:1067030938|1.2;538,:1067030938|1.2;540,:1067030938|1.2;546,:1067030938|1.2;201,:1067030938|1.2;446,:0|0;447,:0|0;448,:0|0;449,:0|0;539,:0|0;541,:0|0;195,:0|0;197,:0|0;231,0:1073741824|2;224,0:1077936128|3;232,0:1073741824|2;225,0:1077936128|3;226,:1077936128|3;233,:1073741824|2;236,:1075838976|2.5;235,0:1075838976|2.5;542,0:1073741824|2;547,0:1073741824|2;220,0:1073741824|2;216,0:1073741824|2;543,0:0|0;234,0:1075838976|2.5;222,0:1065353216|1;223,0:1065353216|1;227,0:1065353216|1;228,:1065353216|1;544,0:1065353216|1;548,0:1065353216|1;213,0:1065353216|1;545,0:0|0;381,:1000|1.401E-42;380,:1000|1.401E-42;100,30592:1|1E-45;102,30592:1|1E-45;388,57:0|0"
		};

		private static readonly DBInventory NewbieShield = new DBInventory
		{
			EquipmentSlot = 3,
			LocationX = 0,
			LocationY = 0,
			Count = 1,
			FirstGem = -1,
			SecondGem = -1,
			ThirdGem = -1,
			TransmogGBID = -1,
			GbId = unchecked((int)1815806856 ),
			Durability = 1138,
			Affixes = "",
			Attributes = "383,:1|1E-45;103,:0|0;406,:0|0;409,:1|1E-45;401,:1|1E-45;405,:1477195667|6.166813E+14;33,:1090519040|8;36,:1090519040|8;37,:1090519040|8;38,:1098907648|16;31,:1090519040|8;381,:1138|1.595E-42;380,:1138|1.595E-42;264,:1041865114|0.15;272,:1088421888|7;273,:1084227584|5;275,:1084227584|5;276,:1084227584|5;388,57:0|0"
		};

		public static void PreLoadToons()
		{
			Logger.Info("Loading Diablo III - Toons...");
			foreach (var toon in DBSessions.SessionQuery<DBToon>())
				LoadedToons.TryAdd(toon.Id, new Toon(toon, null));
		}

		public static Toon GetToonByDBToon(DBToon dbToon, GameDBSession session = null)
		{
			if (dbToon == null) return null;
			if (LoadedToons.ContainsKey(dbToon.Id))
				return LoadedToons[dbToon.Id];
			else
			{
				var toon = new Toon(dbToon, session);
				LoadedToons.TryAdd(dbToon.Id, toon);
				return toon;
			}
		}

		public static Account GetOwnerAccountByToonLowId(ulong id)
		{
			return GetToonByLowId(id).GameAccount.Owner;
		}

		public static GameAccount GetOwnerGameAccountByToonLowId(ulong id)
		{
			return GetToonByLowId(id).GameAccount;
		}

		public static Toon GetToonByLowId(ulong id, GameDBSession session = null)
		{
			if (LoadedToons.ContainsKey(id))
				return LoadedToons[id];
			else
			{
				var dbToon = DBSessions.SessionGet<DBToon>(id);
				return GetToonByDBToon(dbToon, session);
			}
		}

		public static Toon GetDeletedToon(GameAccount account)
		{
			var query = DBSessions.SessionQueryWhere<DBToon>(dbt => dbt.DBGameAccount.Id == account.PersistentID && dbt.Deleted);
			return query.Any() ? GetToonByLowId(query.Last().Id) : null;
		}

		public static List<Toon> GetToonsForGameAccount(GameAccount account) =>
			DBSessions.SessionQueryWhere<DBToon>(t => t.DBGameAccount.Id == account.PersistentID)
				.Select(dbt => GetToonByLowId(dbt.Id)).ToList();

		public static int TotalToons => DBSessions.SessionQuery<DBToon>().Count;


		public static Toon CreateNewToon(string name, int classId, ToonFlags flags, byte level, bool IsHardcore, GameAccount gameAccount, int Season)
		{
			var dbGameAccount = DBSessions.SessionQuerySingle<DBGameAccount>(dba => dba.Id == gameAccount.PersistentID);
			var toonFlags = flags;
			//if (IsHardcore) toonFlags = toonFlags | ToonFlags.Hardcore;
			
			var newDBToon = new DBToon
			{
				Class = @Toon.GetClassById(classId),
				Name = name,
				/*HashCode = GetUnusedHashCodeForToonName(name),*/
				Flags = toonFlags,
				StoneOfPortal = false,
				isHardcore = IsHardcore,
				isSeasoned = Season == 0 ? false : true,
				CreatedSeason = Season,
				Level = level,
				ParagonBonuses = new ushort[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
				Experience = 280,
				CurrentQuestId = 87700,
				CurrentQuestStepId = -1,
				CurrentDifficulty = 0,
				Lore = Array.Empty<byte>(),
				Stats = "0;0;0;0;0;0",
				DBGameAccount = dbGameAccount,
				Cosmetic1 = -1,
				Cosmetic2 = -1,
				Cosmetic3 = -1,
				Cosmetic4 = -1,
			};

			DBSessions.SessionSave(newDBToon);

			Toon createdToon = GetToonByLowId(newDBToon.Id);

			CreateSkillSet(newDBToon);

			CreateStartEquipment(createdToon, IsHardcore);

			CreateHirelingProfile(createdToon, 1);
			CreateHirelingProfile(createdToon, 2);
			CreateHirelingProfile(createdToon, 3);

			LoadedToons.TryAdd(newDBToon.Id, createdToon);
			return createdToon;
		}

		private static void CreateSkillSet(DBToon toon)
		{
			int[] ActiveSkillsList = Skills.GetAllActiveSkillsByClass(toon.Class).Take(1).ToArray();
			var ActiveSkills = new DBActiveSkills
			{
				DBToon = toon,
				Skill0 = ActiveSkillsList[0],
				Skill1 = -1,
				Skill2 = -1,
				Skill3 = -1,
				Skill4 = -1,
				Skill5 = -1,
				Rune0 = -1,
				Rune1 = -1,
				Rune2 = -1,
				Rune3 = -1,
				Rune4 = -1,
				Rune5 = -1,
				Passive0 = -1,
				Passive1 = -1,
				Passive2 = -1,
				Passive3 = -1,
				PotionGBID = -1
			};
			DBSessions.SessionSave(ActiveSkills);
		}

		public static void CreateStartEquipment(Toon toon, bool isHardcore)
		{
			DBInventory pants = NewbiePants;
			pants.DBToon = toon.DbToon;
			pants.DBGameAccount = toon.GameAccount.DBGameAccount;
			pants.isHardcore = isHardcore;
			DBSessions.SessionSave(pants);

			DBInventory armor = NewbieArmor;
			armor.DBToon = toon.DbToon;
			armor.DBGameAccount = toon.GameAccount.DBGameAccount;
			armor.isHardcore = isHardcore;
			DBSessions.SessionSave(armor);

			DBInventory weapon;
			switch (toon.DbToon.Class)
			{
				case ToonClass.Barbarian:
					weapon = NewbieAxe;
					break;
				case ToonClass.Crusader:
					weapon = NewbieFlail;
					break;
				case ToonClass.DemonHunter:
					weapon = NewbieBow;
					break;
				case ToonClass.Monk:
					weapon = NewbieFist;
					break;
				case ToonClass.WitchDoctor:
					weapon = NewbieKnife;
					break;
				case ToonClass.Wizard:
					weapon = NewbieWand;
					break;
				case ToonClass.Necromancer:
					weapon = NewbieNecromancer;
					break;
				default:
					weapon = NewbieKnife;
					break;
			}
			weapon.DBToon = toon.DbToon;
			weapon.DBGameAccount = toon.GameAccount.DBGameAccount;
			weapon.isHardcore = isHardcore;
			DBSessions.SessionSave(weapon);
			if (toon.DbToon.Class == ToonClass.Crusader) //add shield
			{
				weapon = NewbieShield;
				weapon.DBToon = toon.DbToon;
				weapon.DBGameAccount = toon.GameAccount.DBGameAccount;
				weapon.isHardcore = isHardcore;
				DBSessions.SessionSave(weapon);
			}
		}

		public static void CreateHirelingProfile(Toon toon, int type)
		{
			DBSessions.SessionSave(new DBHireling
			{
				Class = type,
				DBToon = toon.DbToon,
				Skill1SNOId = -1,
				Skill2SNOId = -1,
				Skill3SNOId = -1,
				Skill4SNOId = -1
			});
		}

		public static void DeleteToon(Toon toon)
		{
			if (toon == null) return;

			if (toon.GameAccount.DBGameAccount.LastPlayedHero != null && toon.GameAccount.DBGameAccount.LastPlayedHero.Id == toon.PersistentID)
				toon.GameAccount.DBGameAccount.LastPlayedHero = null;

			toon.Deleted = true;
			Logger.Debug("Deleting toon {0}", toon.PersistentID);
		}

		public static DBToon CreateFakeDBToon(string name, DBGameAccount gaccount)
		{
			int class_seed = FastRandom.Instance.Next(100);
			int gender_seed = FastRandom.Instance.Next(100);
			ToonClass class_name = ToonClass.Barbarian;
			if (class_seed > 20)
				class_name = ToonClass.Monk;
			if (class_seed > 40)
				class_name = ToonClass.DemonHunter;
			if (class_seed > 60)
				class_name = ToonClass.WitchDoctor;
			if (class_seed > 80)
				class_name = ToonClass.Wizard;
			DBToon fakeToon = new DBToon
			{
				Name = name,
				Class = class_name,
				Flags = gender_seed > 50 ? ToonFlags.Male : ToonFlags.Female,
				Level = 60,
				Experience = 0,
				PvERating = 0,
				isHardcore = false,
				CurrentAct = 0,
				CurrentQuestId = -1,
				CurrentQuestStepId = -1,
				CurrentDifficulty = 0,
				DBGameAccount = gaccount,
				TimePlayed = 0,
				Stats = "",
				Lore = Array.Empty<byte>(),
				Deleted = false,
				Archieved = false,
				Cosmetic1 = -1,
				Cosmetic2 = -1,
				Cosmetic3 = -1,
				Cosmetic4 = -1
			};
			/*switch (class_name)
			{
				case ToonClass.Barbarian:
					fakeToon.DBActiveSkills = new DBActiveSkills
					{
						Skill0 = 78548,
						Rune0 = 4,
						Skill1 = 96296,
						Rune1 = 1,
						Skill2 = 93409,
						Rune2 = 4,
						Skill3 = 93885,
						Rune3 = 2,
						Skill4 = 79076,
						Rune4 = 1,
						Skill5 = 98878,
						Rune5 = 2,
						Passive0 = 205300,
						Passive1 = 205217,
						Passive2 = 205707,
						PotionGBID = -1
					};
					break;
				case ToonClass.Monk:
					fakeToon.DBActiveSkills = new DBActiveSkills
					{
						Skill0 = 95940,
						Rune0 = 0,
						Skill1 = 97328,
						Rune1 = 1,
						Skill2 = 96215,
						Rune2 = 1,
						Skill3 = 96203,
						Rune3 = 3,
						Skill4 = 192405,
						Rune4 = 2,
						Skill5 = 96694,
						Rune5 = 0,
						Passive0 = 209622,
						Passive1 = 209029,
						Passive2 = 209104,
						PotionGBID = -1
					};
					break;
				case ToonClass.DemonHunter:
					fakeToon.DBActiveSkills = new DBActiveSkills
					{
						Skill0 = 77552,
						Rune0 = 2,
						Skill1 = 131325,
						Rune1 = 4,
						Skill2 = 130830,
						Rune2 = 4,
						Skill3 = 111215,
						Rune3 = 3,
						Skill4 = 77546,
						Rune4 = 0,
						Skill5 = 129214,
						Rune5 = 0,
						Passive0 = 211225,
						Passive1 = 155721,
						Passive2 = 218385,
						PotionGBID = -1
					};
					break;
				case ToonClass.WitchDoctor:
					fakeToon.DBActiveSkills = new DBActiveSkills
					{
						Skill0 = 106465,
						Rune0 = 0,
						Skill1 = 108506,
						Rune1 = 1,
						Skill2 = 69182,
						Rune2 = 2,
						Skill3 = 70455,
						Rune3 = 1,
						Skill4 = 67567,
						Rune4 = 1,
						Skill5 = 67616,
						Rune5 = 0,
						Passive0 = 217968,
						Passive1 = 208568,
						Passive2 = 208569,
						PotionGBID = -1
					};
					break;
				case ToonClass.Wizard:
					fakeToon.DBActiveSkills = new DBActiveSkills
					{
						Skill0 = 30668,
						Rune0 = 4,
						Skill1 = 91549,
						Rune1 = 1,
						Skill2 = 168344,
						Rune2 = 1,
						Skill3 = 86991,
						Rune3 = 0,
						Skill4 = 30680,
						Rune4 = 4,
						Skill5 = 30796,
						Rune5 = 4,
						Passive0 = 208468,
						Passive1 = 208473,
						Passive2 = 208477,
						PotionGBID = -1
					};
					break;
			}*/
			return fakeToon;
		}
	}
}
