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

[CommandGroup("teleport", "Teleports where you click.", Account.UserLevels.GM, inGameOnly: true, disabled: true)]
[Obsolete("Does not work properly.")]
public class TeleportCommand : CommandGroup
{
    private readonly Logger _logger = LogManager.CreateLogger<TeleportCommand>();
    [DefaultCommand(Account.UserLevels.Tester, true)]
    public string Teleport(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in-game to use this command.";
        player.IsTeleportActive = !player.IsTeleportActive;
        _logger.Trace(player.IsTeleportActive ? 
            $"Player is now $[deepskyblue1]$teleporting$[/]$." :
            "Player is $[red3_1 bold underline]$NOT$[/]$ $[deepskyblue1]$teleporting$[/]$ anymore.");
        return player.IsTeleportActive
            ? "You will now teleport where you click."
            : "You will no longer teleport where you click.";
    }

    [Command("followers", "Teleport follower to you.", minUserLevel: Account.UserLevels.Tester)]
    public string TeleportFollowers(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in-game to use this command.";

        var gameClient = invokerClient.InGameClient!;
        if (gameClient == null) return "You must be in-game to use this command.";
        var world = player.World;
        var followers = player.GetFollowers();
        int following = 0;
        foreach (var follower in followers)
        {
            var sno = player.Followers[follower];
            if (world.GetActorByDynamicId(follower, out var actor))
            {
                if (actor.World != world)
                {
                    _logger.Trace(
                        $"Actor {sno.GetName().Markup().Color(Color.OrangeRed1)} " +
                        $"is being teleported ".Markup().Color(Color.DarkOliveGreen1) +
                        $"to player " +
                        $"from $[red3_1 underline]$another world$[/]$ " +
                        $"from {actor.World.SNO.GetName().Markup().Color(Color.Plum1)} to {world.SNO.GetName().Markup().Color(Color.Pink1)}");
                    actor.ChangeWorld(world, player.Position);
                    following++;
                }
                else
                {
                    _logger.Trace($"Actor {sno.GetName().Markup().Color(Color.OrangeRed1)} " +
                                  $"is being teleported ".Markup().Color(Color.DarkOliveGreen1) +
                                  $"to player " +
                                  $"from $[purple_1 underline]$the same world$[/]$ " +
                                  $"({world.SNO.GetName().Markup().Color(Color.Plum1)}");
                    actor.Teleport(player.Position);
                    following++;
                }
            }
            else
            {
                _logger.Warn($"The follower {sno.GetName().Markup().Color(Color.Red)} doesn't exist.");
            }
        }

        return $"Your " +
               $"{following}/" +
               $"{followers.Length} " +
               $"follower(s) have been teleported to you.";
    }
}