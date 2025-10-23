using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("heal", "Heals yourself", Account.UserLevels.Tester, inGameOnly: true)]
public class HealCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Heal(string[] @params, BattleClient invokerClient)
    {
        invokerClient.InGameClient.Player.Heal();
        return "You have been healed";
    }
}