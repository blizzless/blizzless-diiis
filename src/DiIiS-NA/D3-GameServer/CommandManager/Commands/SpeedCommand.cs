using System;
using System.Linq;
using D3.Account;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.Battle;
using NHibernate.Criterion;
using Account = DiIiS_NA.LoginServer.AccountsSystem.Account;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("speed", $"Modify speed walk of you character.\nUsage: !speed <value>\nReset: !speed\nMax Speed: !speed 2", Account.UserLevels.Tester, inGameOnly: true)]
public class SpeedCommand : CommandGroup
{
    public const float MinSpeedValue = 0;
    public const float NormalSpeedValue = 0.36f;
    public const float MaxSpeedValue = 2;
    
    [DefaultCommand(Account.UserLevels.Tester, inGameOnly: true)]
    public string ModifySpeed(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient.InGameClient?.Player is not { } player)
            return "You are not in game.";

        if (@params == null)
            return $"Change the movement speed. Min {MinSpeedValue} (Base), Max {MaxSpeedValue}.\nYou can use decimal values like 1.3 for example.";

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Dev))
            return "You cannot change speed while in DEV mode.";

        // Determine the speed value to apply
        float speedValue = NormalSpeedValue; // Default to normal speed
        
        if (@params.Any())
        {
            string command = @params[0];
            
            if (!command.CompareWith("reset"))
            {
                // Try to parse as float
                // `speedValue < MinSpeedValue` because it's checked later.
                if (!float.TryParse(command, out speedValue) || speedValue < MinSpeedValue || speedValue > MaxSpeedValue)
                    return $"Invalid speed value. Must be a number between {MinSpeedValue} and {MaxSpeedValue}.";
            }
        }

        var playerSpeed = invokerClient.InGameClient.Player.Attributes;

        // Remove the existing Speed fixed attribute if present
        if (playerSpeed.FixedMap.Contains(FixedAttribute.Speed))
            playerSpeed.FixedMap.Remove(FixedAttribute.Speed);

        // Apply the speed value
        if (speedValue.IsWithinTolerance(NormalSpeedValue) || speedValue.IsZero())
        {
            playerSpeed[GameAttributes.Running_Rate] = NormalSpeedValue;
            playerSpeed.BroadcastChangedIfRevealed();
            return $"Speed reset to Base Speed ({NormalSpeedValue:0.000}).";
        }
        
        playerSpeed.FixedMap.Add(FixedAttribute.Speed, attr => attr[GameAttributes.Running_Rate] = speedValue);
        playerSpeed.BroadcastChangedIfRevealed();
        return $"Speed changed to {speedValue:0.000}";
    }
}