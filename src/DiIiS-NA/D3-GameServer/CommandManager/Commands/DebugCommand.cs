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

[CommandGroup("debug", "Teleports where you click.", Account.UserLevels.GM, shortcut: "d", inGameOnly: true, disabled: true)]
[Obsolete("Does not work properly.")]
public class DebugCommand : CommandGroup
{
    private readonly Logger _logger = LogManager.CreateLogger<TeleportCommand>();
    [DefaultCommand(Account.UserLevels.GM, true)]
    public string Debug(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in-game to use this command.";
        var containsPowerful = player.Attributes.FixedMap.Contains(FixedAttribute.Powerful);
        var containsInvulnerability = player.Attributes.FixedMap.Contains(FixedAttribute.Invulnerable);
        var containsSpeed = player.Attributes.FixedMap.Contains(FixedAttribute.Speed);

        if (containsPowerful)
            player.Attributes.FixedMap.Remove(FixedAttribute.Speed);
        if (containsInvulnerability)
            player.Attributes.FixedMap.Remove(FixedAttribute.Invulnerable);
        if (containsSpeed)
            player.Attributes.FixedMap.Remove(FixedAttribute.Speed);

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

            // invulenrable
            attributes[GameAttributes.Invulnerable] = true;
            // max speed
            attributes[GameAttributes.Running_Rate] = 2;
        }, removedAction: (attributes) =>
        {
            attributes[GameAttributes.Invulnerable] = false;
            attributes[GameAttributes.Running_Rate] = 0.36f;
        });

        return $"DEBUG MODE: You are now invulnerable, powerful and max speed.";
    }
}