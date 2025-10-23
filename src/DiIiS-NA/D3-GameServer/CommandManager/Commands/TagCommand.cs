using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("tag", "Switch private Tag for connect", inGameOnly: true)]
class TagCommand : CommandGroup
{
    [DefaultCommand(Account.UserLevels.User, inGameOnly: true)]
    public string Tag(string[] @params, BattleClient invokerClient)
    {
        if(@params == null)
            return "Wrong game tag. Example: !tag mytag";
        if (@params.Length != 1)
            return "Invalid arguments. Enter one string tag.";

        string Tag = @params[0];
        invokerClient.GameTeamTag = Tag;
			
        return string.Format("New Game Tag - " + Tag );
    }
}