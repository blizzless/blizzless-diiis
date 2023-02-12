using System.Linq;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("world", "World commands", Account.UserLevels.Tester)]
public class WorldCommand : CommandGroup
{
    [Command("info", "Current World Info")]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not {} player)
            return "You are not in game";
        
        if (player.World == null)
            return "You are not in world";
        
        var world = player.World;
        return $"[{world.SNO.ToString()}] - {world.SNO}\n{world.Players.Count} players\n" +
               $"{world.Monsters.Count(s=>!s.Dead)} of {world.Monsters.Count} monsters alive\n" +
               $"~ {world.Monsters.Average(s=>s.Attributes[GameAttributes.Level]).ToString("F1")} avg. monsters level\n" +
               $"~ {world.Monsters.Average(s=>s.Attributes[GameAttributes.Hitpoints_Max]).ToString("F1")} avg. monsters HP\n" +
               $"{world.Portals.Count} portal(s)\n" +
               $"{world.GetAllDoors().Length} door(s)\n" +
               $"{world.Actors.Count(s=>s.Value is Door)} door(s)\n" +
               $"{(world.Game.ActiveNephalemPortal ? "Nephalem portal is active" : "Nephalem portal is inactive")}\n" +
               $"{world.Game.ActiveNephalemProgress} nephalem progress";
    }
}