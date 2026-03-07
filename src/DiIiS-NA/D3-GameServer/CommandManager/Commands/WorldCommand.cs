using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.Utilities;
using NHibernate.Mapping;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("world", "World commands\nUsage: !world <command>", Account.UserLevels.Tester, inGameOnly: true)]
public class WorldCommand : CommandGroup
{
    private Logger _logger = LogManager.CreateLogger<WorldCommand>();

    [Command("info", "Current World Info", inGameOnly: true)]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not {} player)
            return "You are not in-game.";
        
        if (player.World == null)
            return "You are not in any world.";

        var world = player.World;

        var act = world.Game.GetCurrentActName();

        string questName = "";
        if (invokerClient.InGameClient.Game?.QuestManager is { } questManager)
            questName = "Quest: " + questManager.Game.GetCurrentQuestName() + "\n";
        try
        {
            var openedDoors = world.Actors.Count(s => s.Value is Door { isOpened: true });
            var closedDoors = world.Actors.Count(s => s.Value is Door { isOpened: false });
            StrBuilder builder = new StrBuilder()
                .Append($"[{world.SNO.ToString()}] - {world.SNO}")
                .Append($"{world.Players.Count} players")
                .Append($"{world.Monsters.Count(s => !s.Dead)} of {world.Monsters.Count} monsters alive")
                .Append($"~ {world.Monsters.Average(s => s.Attributes[GameAttributes.Level]):F1} avg. monsters level")
                .Append($"~ {world.Monsters.Average(s => s.Attributes[GameAttributes.Hitpoints_Max]):F1} avg. monsters HP")
                .Append($"{world.Portals.Count} portal(s)")
                .Append($"{openedDoors + closedDoors} door(s) - {openedDoors} open and {closedDoors} closed")
                .Append($"{act} at quest '{questName}' and side-quest {world.Game.CurrentSideQuest}")
                .AppendIf(world.Game.ActiveNephalemPortal, "Nephalem portal is ACTIVE")
                .AppendIf(world.Game.ActiveNephalemPortal, $"{world.Game.ActiveNephalemProgress} nephalem progress");
            return builder.ToString("\n");
        }
        catch (Exception ex)
        {
            _logger.ErrorException(ex, "Error while invoking command !world info: " + ex.Message);
            return "An error occurred while retrieving world info.";
        }
    }

    [Command("quest", "Current world quest information\nUsage: !world quest", inGameOnly: true)]
    public string Quest(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in-game.";

        if (player.World == null)
            return "You are not in any world.";

        var world = player.World;

        var act = world.Game.GetCurrentActName();
        var quest = world.Game.GetCurrentQuestName();
        var step = world.Game.CurrentStep;
        StrBuilder builder = new StrBuilder()
            .Append(act)
            .Append(quest)
            .Bundle(" - ")
            .AppendIf(step != -1, $"Step {step}");
        return builder.ToString("\n");
    }
}