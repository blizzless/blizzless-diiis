using System;
using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("identify", "Identifies all items in your inventory.", Account.UserLevels.Tester, inGameOnly: true)]
public class IdentifyCommand
{
    [DefaultCommand(inGameOnly: true)]
    public string Identify(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;
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