using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using FluentNHibernate.Utils;

namespace DiIiS_NA.GameServer.CommandManager;

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
            return $"No account with email '{email}' exists.";

        return $"Email: {account.Email} User Level: {account.UserLevel}";
    }

    [Command("add",
        "Allows you to add a new user account.\nUsage: account add <email> <password> <battletag> [userlevel]",
        Account.UserLevels.GM)]
    public string Add(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length < 3)
            return "Invalid arguments. Type 'help account add' to get help.";

        var email = @params[0];
        var password = @params[1];
        var battleTagName = @params[2];
        var userLevel = Account.UserLevels.User;

        if (@params.Length == 4)
        {
            var level = Account.UserLevelsExtensions.FromString(@params[3]);
            if (level == null)
                return "Invalid user level.";
            userLevel = level.Value;
        }

        if (!email.Contains('@'))
            return $"'{email}' is not a valid email address.";

        if (battleTagName.Contains('#'))
            return "BattleTag must not contain '#' or HashCode.";

        if (password.Length < 8 || password.Length > 16)
            return "Password should be a minimum of 8 and a maximum of 16 characters.";

        if (AccountManager.GetAccountByEmail(email) != null)
            return $"An account already exists for email address {email}.";

        var account = AccountManager.CreateAccount(email, password, battleTagName, userLevel);
        var gameAccount = GameAccountManager.CreateGameAccount(account);
        //account.DBAccount.DBGameAccounts.Add(gameAccount.DBGameAccount);
        return
            $"Created new account {account.Email} [user-level: {account.UserLevel}] Full BattleTag: {account.BattleTag}.";
    }

    [Command("setpassword",
        "Allows you to set a new password for account\nUsage: account setpassword <email> <password>",
        Account.UserLevels.GM)]
    public string SetPassword(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length < 2)
            return "Invalid arguments. Type 'help account setpassword' to get help.";

        var email = @params[0];
        var password = @params[1];

        var account = AccountManager.GetAccountByEmail(email);

        if (account == null)
            return $"No account with email '{email}' exists.";

        if (password.Length < 8 || password.Length > 16)
            return "Password should be a minimum of 8 and a maximum of 16 characters.";

        account.UpdatePassword(password);
        return $"Updated password for account {email}.";
    }

    [Command("setbtag", "Allows you to change battle tag for account\nUsage: account setbtag <email> <newname>",
        Account.UserLevels.GM)]
    public string SetBTag(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length < 2)
            return "Invalid arguments. Type 'help account setbtag' to get help.";

        var email = @params[0];
        var newname = @params[1];

        var account = AccountManager.GetAccountByEmail(email);

        if (account == null)
            return $"No account with email '{email}' exists.";

        account.UpdateBattleTag(newname);
        return $"Updated battle tag for account {email}.";
    }

    [Command("setuserlevel",
        "Allows you to set a new user level for account\nUsage: account setuserlevel <email> <user level>.\nAvailable user levels: owner, admin, gm, user.",
        Account.UserLevels.GM)]
    public string SetLevel(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length < 2)
            return "Invalid arguments. Type 'help account setuserlevel' to get help.";

        var email = @params[0];

        var account = AccountManager.GetAccountByEmail(email);

        if (account == null)
            return $"No account with email '{email}' exists.";

        var level = Account.UserLevelsExtensions.FromString(@params[1]);
        if (level == null)
            return "Invalid user level.";
        Account.UserLevels userLevel = level.Value;

        account.UpdateUserLevel(userLevel);
        return $"Updated user level for account {email} [user-level: {userLevel}].";
    }
}