//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GuildSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	[CommandGroup("account", "Provides account management commands.")]
	public class AccountCommands : CommandGroup
	{
		[Command("show", "Shows information about given account\nUsage: account show <email>", Account.UserLevels.GM)]
		public string Show(string[] @params, BattleClient invokerClient)
		{
			if (!@params.Any())
				return "Invalid arguments. Type 'help account show' to get help.";

			var email = @params[0];
			var account = AccountManager.GetAccountByEmail(email);

			if (account == null)
				return string.Format("No account with email '{0}' exists.", email);

			return string.Format("Email: {0} User Level: {1}", account.Email, account.UserLevel);
		}

		[Command("add", "Allows you to add a new user account.\nUsage: account add <email> <password> <battletag> [userlevel]", Account.UserLevels.GM)]
		public string Add(string[] @params, BattleClient invokerClient)
		{
			if (@params.Count() < 3)
				return "Invalid arguments. Type 'help account add' to get help.";

			var email = @params[0];
			var password = @params[1];
			var battleTagName = @params[2];
			var userLevel = Account.UserLevels.User;

			if (@params.Count() == 4)
			{
				var level = @params[3].ToLower();
				switch (level)
				{
					case "owner":
						userLevel = Account.UserLevels.Owner;
						break;
					case "admin":
						userLevel = Account.UserLevels.Admin;
						break;
					case "gm":
						userLevel = Account.UserLevels.GM;
						break;
					case "tester":
						userLevel = Account.UserLevels.Tester;
						break;
					case "user":
						userLevel = Account.UserLevels.User;
						break;
					default:
						return level + " is not a valid user level.";
				}
			}

			if (!email.Contains('@'))
				return string.Format("'{0}' is not a valid email address.", email);

			if (battleTagName.Contains('#'))
				return "BattleTag must not contain '#' or HashCode.";

			if (password.Length < 8 || password.Length > 16)
				return "Password should be a minimum of 8 and a maximum of 16 characters.";

			if (AccountManager.GetAccountByEmail(email) != null)
				return string.Format("An account already exists for email address {0}.", email);

			var account = AccountManager.CreateAccount(email, password, battleTagName, userLevel);
			var gameAccount = GameAccountManager.CreateGameAccount(account);
			//account.DBAccount.DBGameAccounts.Add(gameAccount.DBGameAccount);
			return string.Format("Created new account {0} [user-level: {1}] Full BattleTag: {2}.", account.Email, account.UserLevel, account.BattleTag);
		}

		[Command("setpassword", "Allows you to set a new password for account\nUsage: account setpassword <email> <password>", Account.UserLevels.GM)]
		public string SetPassword(string[] @params, BattleClient invokerClient)
		{
			if (@params.Count() < 2)
				return "Invalid arguments. Type 'help account setpassword' to get help.";

			var email = @params[0];
			var password = @params[1];

			var account = AccountManager.GetAccountByEmail(email);

			if (account == null)
				return string.Format("No account with email '{0}' exists.", email);

			if (password.Length < 8 || password.Length > 16)
				return "Password should be a minimum of 8 and a maximum of 16 characters.";

			AccountManager.UpdatePassword(account, password);
			return string.Format("Updated password for account {0}.", email);
		}

		[Command("setbtag", "Allows you to change battle tag for account\nUsage: account setbtag <email> <newname>", Account.UserLevels.GM)]
		public string SetBTag(string[] @params, BattleClient invokerClient)
		{
			if (@params.Count() < 2)
				return "Invalid arguments. Type 'help account setbtag' to get help.";

			var email = @params[0];
			var newname = @params[1];

			var account = AccountManager.GetAccountByEmail(email);

			if (account == null)
				return string.Format("No account with email '{0}' exists.", email);

			AccountManager.UpdateBattleTag(account, newname);
			return string.Format("Updated battle tag for account {0}.", email);
		}

		[Command("setuserlevel", "Allows you to set a new user level for account\nUsage: account setuserlevel <email> <user level>.\nAvailable user levels: owner, admin, gm, user.", Account.UserLevels.GM)]
		public string SetLevel(string[] @params, BattleClient invokerClient)
		{
			if (@params.Count() < 2)
				return "Invalid arguments. Type 'help account setuserlevel' to get help.";

			var email = @params[0];
			var level = @params[1].ToLower();
			Account.UserLevels userLevel;

			var account = AccountManager.GetAccountByEmail(email);

			if (account == null)
				return string.Format("No account with email '{0}' exists.", email);

			switch (level)
			{
				case "owner":
					userLevel = Account.UserLevels.Owner;
					break;
				case "admin":
					userLevel = Account.UserLevels.Admin;
					break;
				case "gm":
					userLevel = Account.UserLevels.GM;
					break;
				case "tester":
					userLevel = Account.UserLevels.Tester;
					break;
				case "user":
					userLevel = Account.UserLevels.User;
					break;
				default:
					return level + " is not a valid user level.";
			}
			account.UpdateUserLevel(userLevel);
			return string.Format("Updated user level for account {0} [user-level: {1}].", email, userLevel);
		}
	}

	[CommandGroup("mute", "Disables chat messages for the account for some defined time span.")]
	class MuteCommand : CommandGroup
	{
		[DefaultCommand(Account.UserLevels.GM)]
		public string Mute(string[] @params, BattleClient invokerClient)
		{
			if (@params.Count() < 2)
				return "Invalid arguments. Type 'help mute' to get help.";

			var bTagName = @params[0];
			int muteTime = 0;
			Int32.TryParse(@params[1], out muteTime);

			var account = AccountManager.GetAccountByName(bTagName);

			if (account == null)
				return string.Format("No account with bTagName '{0}' exists.", bTagName);

			account.MuteTime = DateTime.Now.ToUnixTime() + (muteTime * 60);

			return string.Format("Done!");
		}
	}

	[CommandGroup("tag", "Switch private Tag for connect")]
	class TagCommand : CommandGroup
	{
		[DefaultCommand(Account.UserLevels.User)]
		public string Tag(string[] @params, BattleClient invokerClient)
		{
			if(@params == null)
				return "Wrong game tag. Example: !tag mytag";
			if (@params.Count() != 1)
				return "Invalid arguments. Enter one string tag.";

			string Tag = @params[0];
			invokerClient.GameTeamTag = Tag;
			
			return string.Format("New Game Tag - " + Tag );
		}
	}

}
