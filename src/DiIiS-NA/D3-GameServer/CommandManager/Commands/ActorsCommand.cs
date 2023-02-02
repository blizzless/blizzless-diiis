using System;
using System.Linq;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("actors",
    "Actors info (does not include Players, Minions and Monsters). This is useful for testing purposes.",
    Account.UserLevels.Tester)]
public class ActorsCommand : CommandGroup
{
    [Command("all", "Lists all actors.", Account.UserLevels.Tester)]
    public string All(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not {} player)
            return "You are not in game.";

        return $"World [{player.World.SNO}]\nAll actors:" + string.Join("\n", player.World.Actors
            .OrderBy(a =>
            {
                var position = player.Position;
                return a.Value.Position.DistanceSquared(ref position);
            }).Select(a =>
            {
                var position = player.Position;
                var distance = a.Value.Position.DistanceSquared(ref position);
                return $"[{a.Value.GetType().Name}] - {a.Value.SNO}\n" +
                       $" > Distance: {distance}\n" +
                       $" > SnoId: {(int)a.Value.SNO}";
            }));
    }
    
    [Command("revealed", "Lists all revealed actors.", Account.UserLevels.Tester)]
    public string Revealed(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not {} player)
            return "You are not in game.";

        return $"World [{player.World.SNO}]\nVisible actors:" + string.Join("\n", player.World.Actors
            .Where(a => a.Value.IsRevealedToPlayer(player))
            .OrderBy(a =>
            {
                var position = player.Position;
                return a.Value.Position.DistanceSquared(ref position);
            }).Select(a =>
            {
                var position = player.Position;
                var distance = a.Value.Position.DistanceSquared(ref position);
                return $"[{a.Value.GetType().Name}] - {a.Value.SNO}\n" +
                       $" > Distance: {distance}\n" +
                       $" > SnoId: {(int)a.Value.SNO}";
            }));
    }
    
    [Command("setoperable", "Sets all actors operable (not the wisest invention).", Account.UserLevels.Tester)]
    public string Operable(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not {} player)
            return "You are not in game.";
        
        if (@params is { Length: > 0 })
        {
            if (!Enum.TryParse<ActorSno>(@params[0].AsSpan(), out var actorSno))
            {
                return "Invalid actor SNO.";
            }
            
            var actor = player.World.Actors.FirstOrDefault(a => a.Value.SNO == actorSno);
            if (actor.Value is null)
                return "Actor not found.";
            actor.Value.SetVisible(true);
            actor.Value.SetUsable(true);
        }
        var actors = player.World.Actors.Select(s=>s.Value).ToArray();
        foreach (var actor in actors)
        {
            actor.SetVisible(true);
            actor.SetUsable(true);
        }
        
        return $"All {actors.Length} world actors are now operable.";
    }
}