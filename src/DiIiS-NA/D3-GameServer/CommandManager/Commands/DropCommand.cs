using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("drop", "Drops an epic item for your class.\nOptionally specify the number of items: !drop [1-20]", Account.UserLevels.Owner)]
public class DropCommand : CommandGroup
{
    [DefaultCommand]
    public string Drop(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You can only invoke from the client.";

        var amount = 1;
        if (@params != null && @params.Any())
            if (!int.TryParse(@params[0], out amount))
                amount = 1;

        amount = amount switch
        {
            < 1 => 1,
            > 20 => 20,
            _ => amount
        };

        try
        {
            for (var i = 0; i < amount; i++)
                player.World.SpawnRandomEquip(player, player, 11, /*player.Level,*/ toonClass: player.Toon.Class,
                    canBeUnidentified: false);
        }
        catch
        {
            for (var i = 0; i < amount; i++)
                player.World.SpawnRandomEquip(player, player, 8, /*player.Level,*/ toonClass: player.Toon.Class,
                    canBeUnidentified: false);
        }

        return $"Dropped {amount} random epic equipment.";
    }
}