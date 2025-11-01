using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("powerful", "Makes your character with absurd amount of damage. Useful for testing.",
    Account.UserLevels.Tester, inGameOnly: true)]
public class PowerfulCommand : CommandGroup
{
    [DefaultCommand(Account.UserLevels.Tester, true)]
    public string Powerful(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Dev))
        {
            return "You cannot change powerfulness while in DEV mode.";
        }

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Powerful))
        {
            // Revert to normal damage values by removing the Damage override to float.MaxValue.

            player.Attributes.FixedMap.Remove(FixedAttribute.Powerful);
            player.Attributes.BroadcastChangedIfRevealed();
            return "You are no longer powerful.";
        }

        player.Attributes.FixedMap.Add(FixedAttribute.Powerful, (attributes) =>
        {
            attributes[GameAttributes.Damage_Delta, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Min, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Weapon_Delta, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Weapon_Min, 0] = float.MaxValue;
        });
        
        player.Attributes.BroadcastChangedIfRevealed();
        return "You are now powerful.";
    }
}