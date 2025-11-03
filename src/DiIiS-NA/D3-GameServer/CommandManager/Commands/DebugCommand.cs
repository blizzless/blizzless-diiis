using System;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.Utilities;
using Spectre.Console;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("debug", "Debug Mode Only (makes you powerful, invulnerable and high speed)",
#if DEBUG
    Account.UserLevels.Tester,
#elif RELEASE
    Account.UserLevels.GM,
#endif
    inGameOnly: true)]
public class DebugCommand : CommandGroup
{
    private readonly Logger _logger = LogManager.CreateLogger<DebugCommand>();

    [DefaultCommand(Account.UserLevels.GM, true)]
    public string Debug(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return InGameOnlyMessage;

        var containsPowerful = player.Attributes.FixedMap.Contains(FixedAttribute.Powerful);
        var containsInvulnerability = player.Attributes.FixedMap.Contains(FixedAttribute.Invulnerable);
        var containsSpeed = player.Attributes.FixedMap.Contains(FixedAttribute.Speed);

        if (containsPowerful)
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Speed);
            player.Attributes.BroadcastChangedIfRevealed();
        }

        if (containsInvulnerability)
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Invulnerable);
            player.Attributes.BroadcastChangedIfRevealed();
        }

        if (containsSpeed)
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Speed);
            player.Attributes.BroadcastChangedIfRevealed();
        }

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Dev))
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Dev);
            player.Attributes.BroadcastChangedIfRevealed();
            return "Debug mode deactivated.";
        }

        player.Attributes.FixedMap.Add(FixedAttribute.Dev, (attributes) =>
        {
            // powerful - should be reset when not fixed anymore.
            attributes[GameAttributes.Damage_Delta, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Min, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Weapon_Delta, 0] = float.MaxValue;
            attributes[GameAttributes.Damage_Weapon_Min, 0] = float.MaxValue;

            // invulnerable
            attributes[GameAttributes.Invulnerable] = true;
            // max speed
            attributes[GameAttributes.Running_Rate] = SpeedCommand.MaxSpeedValue;
        }, removedAction: (attributes) =>
        {
            attributes[GameAttributes.Invulnerable] = false;
            attributes[GameAttributes.Running_Rate] = SpeedCommand.NormalSpeedValue;
        });
        player.Attributes.BroadcastChangedIfRevealed();

        return $"You are now invulnerable, powerful and max speed ({SpeedCommand.MaxSpeedValue}).";
    }
}