using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("eff", "Platinum for your character.\nOptionally specify the number of levels: !eff [count]",
    Account.UserLevels.GM, inGameOnly: true)]
public class PlayEffectGroup : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string PlayEffectCommand(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;
        var id = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out id))
                id = 1;

        player.PlayEffectGroup(id);

        return $"PlayEffectGroup {id}";
    }
}