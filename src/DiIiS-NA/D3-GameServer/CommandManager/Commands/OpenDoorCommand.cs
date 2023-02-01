using DiIiS_NA.LoginServer.Battle;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.LoginServer.AccountsSystem;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("doors", "Information about all doors in the vicinity. This is useful for testing purposes.. Useful for testing.", Account.UserLevels.Tester)]
public class OpenDoorCommand : CommandGroup
{
    [Command("all", "Activate all doors. This is useful for testing purposes.\nUsage: !open all", Account.UserLevels.Tester)]
    public string OpenAllDoors(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game.";
        var world = player.World;
        var openedDoors = world.OpenAllDoors();
        if (openedDoors.Length == 0)
            return "No doors found.";
        return $"Opened {openedDoors.Length} doors: {string.Join(", ", openedDoors.Select(d => (int)d.SNO + " - " + d.SNO))}";
    }
    
    [Command("near", "Activate all nearby doors in the vicinity. This is useful for testing purposes.\nUsage: !open near [distance:50]", Account.UserLevels.Tester)]
    public string OpenAllDoorsNear(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game.";
        var world = player.World;

        var distance = 50f;
        
        if (@params.Length > 0)
        {
            if (!float.TryParse(@params[0], out distance) || distance < 1)
                return "Invalid distance. Distance must be greater than 1.";
        }

        var openedDoors = player.OpenNearDoors(distance);
        if (openedDoors.Length == 0)
            return "No doors found.";
        return $"Opened {openedDoors.Count()} in a distance of {distance:0.0000} doors: {string.Join(", ", openedDoors)}";
    }
    
    [Command("info", "Retrieve all world doors in proximity, sorted in descending order.\nUsage: !open info [distance:50]", Account.UserLevels.Tester)]
    public string InfoDoorsNear(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game.";
        var world = player.World;
        var distance = 50f;
        
        if (@params.Length > 0)
        {
            if (!float.TryParse(@params[0], out distance) || distance < 1)
                return "Invalid distance. Distance must be greater than 1.";
        }

        var doors = player.GetNearDoors(distance);
        if (doors.Length == 0)
            return "No doors found.";
        return $"{doors.Length} doors in a distance of {distance:0.0000} doors: \n{string.Join("\n", doors.Select(s=>
        {
            var position = player.Position;
            return s.Position.DistanceSquared(ref position) + " distance - [" + (int)s.SNO + "] " + s.SNO;;
        }))}";
    }
    
    [DefaultCommand()]
    public string DefaultCommand(string[] @params, BattleClient invokerClient)
    {
        return "!doors all - Activate all doors. This is useful for testing purposes.\n" +
               "!doors near [distance:50] - Activate all nearby doors in the vicinity. This is useful for testing purposes.\n" +
               "!doors info [distance:50] - Retrieve all world doors in proximity, sorted in descending order.";
    }
}