using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("eff", "Platinum for your character.\nOptionally specify the number of levels: !eff [count]",
    Account.UserLevels.GM)]
public class PlayEffectGroup : CommandGroup
{
    [DefaultCommand]
    public string PlayEffectCommand(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var id = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out id))
                id = 1;

        player.PlayEffectGroup(id);

        return $"PlayEffectGroup {id}";
    }
}