//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.LoginServer.AccountsSystem
{
	class GameAccountManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger("DataBaseSystem");

		public static readonly ConcurrentDictionary<ulong, GameAccount> LoadedGameAccounts = new ConcurrentDictionary<ulong, GameAccount>();

		public static int TotalAccounts
		{
			get { return DBSessions.SessionQuery<DBGameAccount>().Count(); }
		}

		public static void PreLoadGameAccounts()
		{
			Logger.Info("Loading game data...");
			List<DBGameAccount> all_accounts = DBSessions.SessionQuery<DBGameAccount>();
			List<DBAchievements> all_achievements = DBSessions.SessionQuery<DBAchievements>();
			foreach (var account in all_accounts)
			{
				var gameAccount = new GameAccount(account, all_achievements);
				LoadedGameAccounts.TryAdd(account.Id, gameAccount);
				gameAccount.Owner.GameAccount = gameAccount;
			}
		}

		public static GameAccount GetGameAccountByDBGameAccount(DBGameAccount dbGameAccount)
		{
			if (LoadedGameAccounts.ContainsKey(dbGameAccount.Id))
				return LoadedGameAccounts[dbGameAccount.Id];
			else
			{
				var account = new GameAccount(dbGameAccount);
				LoadedGameAccounts.TryAdd(dbGameAccount.Id, account);
				return account;
			}
		}

		//Not needed... we emulate only D3, or not?
		/*
		public static Dictionary<ulong, GameAccount> GetGameAccountsForAccountProgram(Account account, FieldKeyHelper.Program program)
		{
			
			return GameAccounts.Where(pair => pair.Value.Owner != null).Where(pair => (pair.Value.Owner.PersistentID == account.PersistentID) && (pair.Value.Program == program)).ToDictionary(pair => pair.Key, pair => pair.Value);
		}
		*/
		public static GameAccount GetAccountByPersistentID(ulong persistentId)
		{
			if (LoadedGameAccounts.ContainsKey(persistentId))
				return LoadedGameAccounts[persistentId];
			else
				return GetGameAccountByDBGameAccount(DBSessions.SessionGet<DBGameAccount>(persistentId));
		}

		public static GameAccount CreateGameAccount(Account account)
		{
			var newDBGameAccount = new DBGameAccount
			{
				DBAccount = DBSessions.SessionGet<DBAccount>(account.PersistentID),
				Flags = 0,
				ParagonLevel = 0,
				ParagonLevelHardcore = 0,
				Experience = 7200000,
				ExperienceHardcore = 7200000,
				StashSize = 700,			// Default stash sizes should be 70 with purchasable upgrades
				HardcoreStashSize = 700,
				SeasonStashSize = 700,
				BloodShards = 0,
				HardcoreBloodShards = 0,
				BossProgress = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff },
				SeenTutorials = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
				StashIcons = new byte[] { 0x00, 0x00, 0x00, 0x00 },
				RmtCurrency = 0,
				Gold = 0,
				HardcoreGold = 0,
				ElitesKilled = 0,
				TotalKilled = 0,
				TotalGold = 0,
				TotalBloodShards = 0,
				PvPTotalKilled = 0,
				PvPTotalWins = 0,
				PvPTotalGold = 0
			};

			DBSessions.SessionSave(newDBGameAccount);

			CreateArtisanProfile(newDBGameAccount, true, false, "Blacksmith");
			CreateArtisanProfile(newDBGameAccount, true, false, "Jeweler");
			CreateArtisanProfile(newDBGameAccount, true, false, "Mystic");

			CreateArtisanProfile(newDBGameAccount, false, false, "Blacksmith");
			CreateArtisanProfile(newDBGameAccount, false, false, "Jeweler");
			CreateArtisanProfile(newDBGameAccount, false, false, "Mystic");

			CreateArtisanProfile(newDBGameAccount, true, true, "Blacksmith");
			CreateArtisanProfile(newDBGameAccount, true, true, "Jeweler");
			CreateArtisanProfile(newDBGameAccount, true, true, "Mystic");

			CreateArtisanProfile(newDBGameAccount, false, true, "Blacksmith");
			CreateArtisanProfile(newDBGameAccount, false, true, "Jeweler");
			CreateArtisanProfile(newDBGameAccount, false, true, "Mystic");

			Logger.Warn("Created gameAccount {0}", account.Email);

			var newGameAccount = GetGameAccountByDBGameAccount(newDBGameAccount);

			account.GameAccount = newGameAccount;
			return newGameAccount;
		}


		public static void CreateArtisanProfile(DBGameAccount dbGAcc, bool hardcore, bool seasoned, string type)
		{
			var crafting = new DBCraft();
			crafting.Artisan = type;
			crafting.DBGameAccount = dbGAcc;
			crafting.isHardcore = hardcore;
			crafting.isSeasoned = seasoned;
			crafting.LearnedRecipes = new byte[0];
			crafting.Level = 1;
			DBSessions.SessionSave(crafting);
		}
	}
}
