using System.Linq;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("speed", "Modify speed walk of you character.\nUsage: !speed <value>\nReset: !speed")]
public class SpeedCommand : CommandGroup
{
    [DefaultCommand]
    public string ModifySpeed(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient == null)
            return "This command can only be used in-game.";

        if (@params == null)
            return
                "Change the movement speed. Min 0 (Base), Max 2.\n You can use decimal values like 1.3 for example.";
        float speedValue;

        const float maxSpeed = 2;
        const float baseSpeed = 0.36f;

        if (@params.Any())
        {
            if (!float.TryParse(@params[0], out speedValue) || speedValue is < 0 or > maxSpeed)
                return "Invalid speed value. Must be a number between 0 and 2.";
        }
        else
        {
            speedValue = 0;
        }

        var playerSpeed = invokerClient.InGameClient.Player.Attributes;

        if (playerSpeed.FixedMap.Contains(FixedAttribute.Speed))
            playerSpeed.FixedMap.Remove(FixedAttribute.Speed);

        if (speedValue <= baseSpeed) // Base Run Speed [Necrosummon]
        {
            playerSpeed[GameAttributes.Running_Rate] = baseSpeed;
            return $"Speed reset to Base Speed ({baseSpeed:0.000}).";
        }
        playerSpeed.FixedMap.Add(FixedAttribute.Speed, attr => attr[GameAttributes.Running_Rate] = speedValue);
        playerSpeed.BroadcastChangedIfRevealed();
        return $"Speed changed to {speedValue}";
    }
}