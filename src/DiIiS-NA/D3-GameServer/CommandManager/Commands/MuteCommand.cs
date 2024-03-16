using System;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("mute", "Disables chat messages for the account for some defined time span.")]
class MuteCommand : CommandGroup
{
    [DefaultCommand(Account.UserLevels.GM)]
    public string Mute(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length < 2)
            return "Invalid arguments. Type 'help mute' to get help.";

        var bTagName = @params[0];
        int muteTime = 0;
        Int32.TryParse(@params[1], out muteTime);

        var account = AccountManager.GetAccountByName(bTagName);

        if (account == null)
            return $"No account with bTagName '{bTagName}' exists.";

        account.MuteTime = DateTime.Now.ToUnixTime() + (muteTime * 60);

        return string.Format("Done!");
    }
}