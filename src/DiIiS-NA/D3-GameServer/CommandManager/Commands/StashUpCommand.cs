using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("stashup", "Upgrade Stash.", Account.UserLevels.Tester, inGameOnly: true)]
public class StashUpCommand : CommandGroup
{
    [DefaultCommand]
    public string Stashup(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;

        player.Inventory.OnBuySharedStashSlots(null);

        return "Stash Upgraded";
    }
}