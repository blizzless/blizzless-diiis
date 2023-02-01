using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("stashup", "Upgrade Stash.", Account.UserLevels.Tester)]
public class StashUpCommand : CommandGroup
{
    [DefaultCommand]
    public string Stashup(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;

        player.Inventory.OnBuySharedStashSlots(null);

        return "Stash Upgraded";
    }
}