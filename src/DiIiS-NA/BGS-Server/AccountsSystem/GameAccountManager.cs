//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.LoginServer.AccountsSystem
{
	class GameAccountManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger("DataBaseSystem");

		public static readonly ConcurrentDictionary<ulong, GameAccount> LoadedGameAccounts = new();

		public static int TotalAccounts => DBSessions.SessionQuery<DBGameAccount>().Count;

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
			if (dbGameAccount == null)
				return null;
			if (LoadedGameAccounts.ContainsKey(dbGameAccount.Id))
			{
				// LoadedGameAccounts[dbGameAccount.Id].DBGameAccount = dbGameAccount;
				return LoadedGameAccounts[dbGameAccount.Id];
			}
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
				StashSize = 700,			// Default stash sizes should be 70 with purchasable upgrades.
				HardcoreStashSize = 700,    // Default stash sizes should be 70 with purchasable upgrades.
				SeasonStashSize = 700,
				HardSeasonStashSize = 700,
				BloodShards = 0,
				HardcoreBloodShards = 0,
				BossProgress = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff },
				SeenTutorials = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
				StashIcons = new byte[] { 0x00, 0x00, 0x00, 0x00 },
				RmtCurrency = 0,
				HardRmtCurrency = 0,
				Platinum = 0,
				HardPlatinum = 0,
				Gold = 0,
				HardcoreGold = 0,
				ElitesKilled = 0,
				HardElitesKilled = 0,
				TotalKilled = 0,
				HardTotalKilled = 0,
				TotalGold = 0,
				HardTotalGold = 0,
				TotalBloodShards = 0,
				HardTotalBloodShards = 0,
				PvPTotalKilled = 0,
				HardPvPTotalKilled = 0,
				PvPTotalWins = 0,
				HardPvPTotalWins = 0,
				PvPTotalGold = 0,
				HardPvPTotalGold = 0,
				CraftItem1 = 0,
				HardCraftItem1 = 0,
				CraftItem2 = 0,
				HardCraftItem2 = 0,
				CraftItem3 = 0,
				HardCraftItem3 = 0,
				CraftItem4 = 0,
				HardCraftItem4 = 0,
				CraftItem5 = 0,
				HardCraftItem5 = 0,
				BigPortalKey = 0,
				HardBigPortalKey = 0,
				LeorikKey = 0,
				HardLeorikKey = 0,
				VialofPutridness = 0,
				HardVialofPutridness = 0,
				IdolofTerror = 0,
				HardIdolofTerror = 0,
				HeartofFright = 0,
				HardHeartofFright = 0,
				HoradricA1 = 0,
				HardHoradricA1 = 0,
				HoradricA2 = 0,
				HardHoradricA2 = 0,
				HoradricA3 = 0,
				HardHoradricA3 = 0,
				HoradricA4 = 0,
				HardHoradricA4 = 0,
				HoradricA5 = 0,
				HardHoradricA5 = 0
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
