using DiIiS_NA.LoginServer.Battle;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.Utilities;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("portals", "Information about all portals in the vicinity. This is useful for testing purposes.", Account.UserLevels.Tester, inGameOnly: true)]
public class PortalsCommand : CommandGroup
{
    [Command("all", "Activate all portals. This is useful for testing purposes.\nUsage: !portals all", Account.UserLevels.Tester, inGameOnly: true)]
    public string OpenAllPortals(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game.";
        var world = player.World;
        var openedPortals = world.OpenAllPortals();
        if (openedPortals.Length == 0)
            return "No portals found.";
        return $"Opened {openedPortals.Length} portals: {string.Join(", ", openedPortals.Select(d => (int)d.SNO + " - " + d.SNO))}";
    }
    
    [Command("near", "Activate all nearby portals in the vicinity. This is useful for testing purposes.\nUsage: !portals near [distance:50]", Account.UserLevels.Tester, inGameOnly: true)]
    public string OpenAllPortalsNear(string[] @params, BattleClient invokerClient)
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

        var openedPortals = player.OpenNearPortals(distance);
        if (openedPortals.Length == 0)
            return "No portals found.";
        return $"Opened {openedPortals.Count()} portals in a distance of {distance:0.0000}: {string.Join(", ", openedPortals)}";
    }
    
    [Command("info", "Retrieve all world's portals in proximity, sorted in descending order.\nUsage: !portals info [distance:50]", Account.UserLevels.Tester, inGameOnly: true)]
    public string InfoPortalsNear(string[] @params, BattleClient invokerClient)
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

        var portals = player.GetNearPortals(distance);
        if (portals.Length == 0)
            return "No portals found.";
        return $"{portals.Length} portals in a distance of {distance:0.0000}: \n{string.Join("\n", portals.Select(s=>
        {
            var position = player.Position;
            return $"{s.Position.DistanceSquared(ref position)} distance - [{s.SNO.GetName()}] id {(int)s.SNO}";;
        }))}";
    }
    
    [DefaultCommand(inGameOnly: true)]
    public string DefaultCommand(string[] @params, BattleClient invokerClient)
    {
        return StrBuilder.From("!portals all - Activate all portals. This is useful for testing purposes.")
            .Append("!portals near [distance:50] - Activate all nearby portals in the vicinity. This is useful for testing purposes.")
            .Append("!portals info [distance:50] - Retrieve all world portals in proximity, sorted in descending order.")
            .ToString(Separator.NewLine);
    }
}