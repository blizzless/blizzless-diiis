using System;
using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("identify", "Identifies all items in your inventory.", Account.UserLevels.Tester)]
public class IdentifyCommand
{
    [DefaultCommand()]
    public string Identify(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in game to use this command.";

        var unidentified = player.Inventory.GetBackPackItems().Where(i => i.Unidentified).ToArray();
        var count = unidentified.Length;
        player.StartCasting(60 * 2, new Action(() =>
        {
            foreach (var item in unidentified)
                item.Identify();
        }));
        return $"Identified {count} items.";
    }
}