using System.Collections.Generic;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("item", "Spawns an item (with a name or type).\nUsage: item [type <type>|<name>] [amount]",
    Account.UserLevels.GM)]
public class ItemCommand : CommandGroup
{
    [DefaultCommand]
    public string Spawn(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var name = "Dye_02";
        var amount = 1;


        if (@params == null)
            return Fallback();

        name = @params[0];

        if (!ItemGenerator.IsValidItem(name))
            return "You need to specify a valid item name!";


        if (@params.Length == 1 || !int.TryParse(@params[1], out amount))
            amount = 1;

        if (amount > 100) amount = 100;

        for (var i = 0; i < amount; i++)
        {
            var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Z);

            var item = ItemGenerator.Cook(player, name);
            item.EnterWorld(position);
        }

        return $"Spawned {amount} items with name: {name}";
    }

    [Command("type", "Spawns random items of a given type.\nUsage: item type <type> [amount]")]
    public string Type(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var name = "Dye";
        var amount = 1;


        if (@params == null)
            return "You need to specify a item type!";

        name = @params[0];

        var type = ItemGroup.FromString(name);

        if (type == null)
            return "The type given is not a valid item type.";

        if (@params.Length == 1 || !int.TryParse(@params[1], out amount))
            amount = 1;

        if (amount > 100) amount = 100;

        for (var i = 0; i < amount; i++)
        {
            var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Z);

            var item = ItemGenerator.GenerateRandom(player, type);
            item.EnterWorld(position);
        }

        return $"Spawned {amount} items with type: {name}";
    }

    [Command("dropall", "Drops all items in Backpack.\nUsage: item dropall")]
    public string DropAll(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;

        var bpItems = new List<Item>(player.Inventory.GetBackPackItems());


        foreach (var item in bpItems)
        {
            var msg = new InventoryDropItemMessage { ItemID = item.DynamicID(player) };
            player.Inventory.Consume(invokerClient.InGameClient, msg);
        }

        return $"Dropped {bpItems.Count} Items for you";
    }
}