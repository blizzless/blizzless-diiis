using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

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
        var level = @params[1].ToLower();
        Account.UserLevels userLevel;

        var account = AccountManager.GetAccountByEmail(email);

        if (account == null)
            return $"No account with email '{email}' exists.";

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
        return $"Updated user level for account {email} [user-level: {userLevel}].";
    }
}