using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("gold", "Gold for your character.\nOptionally specify the number of gold: !gold [count]",
    Account.UserLevels.GM)]
public class GoldCommand : CommandGroup
{
    [DefaultCommand]
    public string Gold(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var amount = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out amount))
                amount = 1;

        player.Inventory.AddGoldAmount(amount);

        return $"Added Gold {amount}";
    }
}