using System;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("unlockart", "Unlock all artisans: !unlockart", Account.UserLevels.Tester, inGameOnly: true)]
public class UnlockArtCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string UnlockArt(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;

        player.BlacksmithUnlocked = true;
        player.JewelerUnlocked = true;
        player.MysticUnlocked = true;
        player.GrantAchievement(74987243307766); // Blacksmith
        player.GrantAchievement(74987243307780); // Jeweler
        player.GrantAchievement(74987247205955); // Mystic

        player.HirelingTemplarUnlocked = true;
        player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 1 });
        player.GrantAchievement(74987243307073);
        player.HirelingScoundrelUnlocked = true;
        player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 2 });
        player.GrantAchievement(74987243307147);
        player.HirelingEnchantressUnlocked = true;
        player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 3 });
        player.GrantAchievement(74987243307145);

        player.LoadCrafterData();
        player.Toon.GameAccount.NotifyUpdate();
        return "All artisans Unlocked";
    }
}