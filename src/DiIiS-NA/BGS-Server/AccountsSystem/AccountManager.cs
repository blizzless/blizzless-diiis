//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Crypthography;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.AccountsSystem
{
	public static class AccountManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger("DataBaseSystem");

		public static int TotalAccounts
		{
			get { return DBSessions.SessionQuery<DBAccount>().Count(); }
		}

		public static readonly ConcurrentDictionary<ulong, Account> LoadedAccounts = new ConcurrentDictionary<ulong, Account>();

		public static void PreLoadAccounts()
		{
			Logger.Info("Loading accounts...");
			List<DBAccount> all_accounts = DBSessions.SessionQuery<DBAccount>();
			foreach (var account in all_accounts)
			{
				LoadedAccounts.TryAdd(account.Id, new Account(account));
			}
		}

		#region AccountGetter
		public static Account CreateAccount(string email, string password, string battleTag, Account.UserLevels userLevel = Account.UserLevels.User)
		{
			if (password.Length > 16) password = password.Substring(0, 16); // make sure the password does not exceed 16 chars.
			var hashCode = GetRandomHashCodeForBattleTag();
			var salt = SRP6a.GetRandomBytes(32);
			var passwordVerifier = SRP6a.CalculatePasswordVerifierForAccount(email, password, salt);
			var saltedticket = password + " asa " + email;

			var newDBAccount = new DBAccount
			{
				Email = email,
				Banned = false,
				Salt = salt,
				PasswordVerifier = passwordVerifier,
				SaltedTicket = saltedticket,
				BattleTagName = battleTag,
				UserLevel = userLevel,
				HashCode = hashCode
			};

			DBSessions.SessionSave(newDBAccount);

			//GenerateReferralCode(email);

			Logger.Warn("Created account {0}", email);
			return GetAccountByEmail(email);
		}

		public static bool GenerateReferralCode(string email)
		{
			try
			{
				var account = GetAccountByEmail(email);
				account.DBAccount.ReferralCode = ((int)(account.DBAccount.Id * 17) + Core.Helpers.Math.FastRandom.Instance.Next(16));
				DBSessions.SessionUpdate(account.DBAccount);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool SetInvitee(string email, int invitee_code)
		{
			try
			{
				var invitee_account = DBSessions.SessionQuerySingle<DBAccount>(dba => dba.ReferralCode == invitee_code);
				var account = GetAccountByEmail(email);
				account.DBAccount.InviteeAccount = invitee_account;
				DBSessions.SessionUpdate(account.DBAccount);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static Account GetAccountByEmail(string email)
		{
			if (LoadedAccounts.Any(a => a.Value.Email.ToLower() == email.ToLower()))
				return LoadedAccounts[LoadedAccounts.Single(a => a.Value.Email.ToLower() == email.ToLower()).Key];
			else
			{
				List<DBAccount> dbAcc = DBSessions.SessionQueryWhere<DBAccount>(dba => dba.Email == email);
				if (dbAcc.Count() == 0)
				{
					Logger.Warn("GetAccountByEmail {0}: DBAccount is null!", email);
					return null;
				}
				if (dbAcc.First() == null)
				{
					Logger.Warn("GetAccountByEmail {0}: DBAccount id is null!", email);
					return null;
				}
				else
					Logger.Debug("GetAccountByEmail id -  \"{0}\"", dbAcc.First().Id);
				return GetAccountByDBAccount(dbAcc.First());
			}
		}

		public static Account GetAccountByBattletag(string battletag)
		{
			string[] tagparts = battletag.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
			int taghash = Convert.ToInt32(tagparts[1], 10);

			if (tagparts[0].StartsWith(" {icon"))
				tagparts[0] = tagparts[0].Substring(13);
			//Logger.Debug("trying GetAccountByBattletag {0}", battletag);
			if (tagparts[0].StartsWith("{c_legendary"))
			{
				tagparts[0] = tagparts[0].Substring(13);
				tagparts[0] = tagparts[0].Split('{')[0];
			}
			List<DBAccount> dbAcc = DBSessions.SessionQueryWhere<DBAccount>(dba => dba.BattleTagName == tagparts[0] && dba.HashCode == taghash);
			if (dbAcc.Count() == 0)
			{
				Logger.Warn("GetAccountByBattletag {0}: DBAccount is null!", battletag);
				return null;
			}
			//else
			//Logger.Debug("GetAccountByBattletag \"{0}\"", battletag);
			return GetAccountByDBAccount(dbAcc.First());
		}

		public static Account GetAccountByName(string btname) //pretty bad to use it
		{
			List<DBAccount> dbAcc = DBSessions.SessionQueryWhere<DBAccount>(dba => dba.BattleTagName == btname);
			if (dbAcc.Count() == 0)
			{
				Logger.Warn("GetAccountByName {0}: DBAccount is null!", btname);
				return null;
			}
			return GetAccountByDBAccount(dbAcc.First());
		}

		public static Account GetAccountByPersistentID(ulong persistentId)
		{
			if (LoadedAccounts.ContainsKey(persistentId))
				return LoadedAccounts[persistentId];
			else
			{
				return GetAccountByDBAccount(DBSessions.SessionGet<DBAccount>(persistentId));
			}
		}

		public static Account GetAccountByDBAccount(DBAccount dbAccount)
		{
			if (LoadedAccounts.ContainsKey(dbAccount.Id))
				return LoadedAccounts[dbAccount.Id];
			else
			{
				var account = new Account(dbAccount);
				LoadedAccounts.TryAdd(dbAccount.Id, account);
				return account;
			}
		}
		#endregion

		#region Managing Functions, also extending Account

		public static bool UpdatePassword(this Account account, string newPassword)
		{
			try
			{
				account.PasswordVerifier = SRP6a.CalculatePasswordVerifierForAccount(account.Email, newPassword, account.Salt);
				return true;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UpdatePassword()");
				return false;
			}
		}

		public static bool UpdateBattleTag(this Account account, string newName)
		{
			Logger.Info("Renaming account \"{0}\"", account.Email);
			try
			{
				if (account.DBAccount.HasRename == false) return false;
				int newHash = GetRandomHashCodeForBattleTag();
				Logger.RenameAccount("{0}#{1} -> {2}#{3}", account.DBAccount.BattleTagName, account.DBAccount.HashCode, newName, newHash);
				account.DBAccount.BattleTagName = newName;
				account.DBAccount.HashCode = newHash;
				account.DBAccount.HasRename = false;
				account.DBAccount.RenameCooldown = DateTime.Now.ToExtendedEpoch() + 2592000000000;
				DBSessions.SessionUpdate(account.DBAccount);
				
				return true;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UpdatePassword()");
				return false;
			}
		}

		public static bool UnlockRename(this Account account)
		{
			Logger.Info("Rename unlock for account \"{0}\"", account.Email);
			try
			{
				account.DBAccount.HasRename = true;
				DBSessions.SessionUpdate(account.DBAccount);
				return true;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UnlockRename()");
				return false;
			}
		}

		public static Account GetAccountBySaltTicket(string ticket)
		{
			if (DBSessions.SessionQueryWhere<DBAccount>(dba => dba.SaltedTicket == ticket).Count() > 0)
				return LoadedAccounts[LoadedAccounts.Single(a => a.Value.SaltedTicket == ticket).Key];
			return null;
		}

		public static bool AddMoney(this Account account, int money)
		{
			try
			{
				if (money <= 0) return false;
				account.DBAccount.Money += (ulong)money;
				DBSessions.SessionUpdate(account.DBAccount);
				return true;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "AddMoney()");
				return false;
			}
		}

		public static bool SubMoney(this Account account, int money)
		{
			try
			{
				if (money <= 0) return false;
				if (account.DBAccount.Money < (ulong)money) return false;
				account.DBAccount.Money -= (ulong)money;
				DBSessions.SessionUpdate(account.DBAccount);
				return true;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "SubMoney()");
				return false;
			}
		}

		public static void UpdateUserLevel(this Account account, Account.UserLevels userLevel)
		{
			try
			{
				account.UserLevel = userLevel;
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UpdateUserLevel()");
			}
		}
		#endregion



		private static int GetRandomHashCodeForBattleTag()
		{
			var rnd = new Random();
			return rnd.Next(1, 10000);
		}
	}
}
