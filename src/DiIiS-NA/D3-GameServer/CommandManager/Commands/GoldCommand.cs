using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("gold", "Gold for your character.\nOptionally specify the number of gold: !gold [count]",
    Account.UserLevels.GM, inGameOnly: true)]
public class GoldCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Gold(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;
        var amount = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out amount))
                amount = 1;

        player.Inventory.AddGoldAmount(amount);

        return $"Added Gold {amount}";
    }
}