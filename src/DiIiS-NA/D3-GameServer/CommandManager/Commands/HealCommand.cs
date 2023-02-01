using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("heal", "Heals yourself", Account.UserLevels.Tester)]
public class HealCommand : CommandGroup
{
    [DefaultCommand]
    public string Heal(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game";

        player.Heal();
        return "You have been healed";
    }
}