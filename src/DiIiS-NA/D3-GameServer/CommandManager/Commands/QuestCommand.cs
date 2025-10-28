 using System;
 using DiIiS_NA.Core.Logging;
 using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.Utilities;
using FluentNHibernate.Utils;
using Spectre.Console;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("quest",
    "Retrieves information about quest states and manipulates quest progress.\n" +
    "Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]",
    Account.UserLevels.Tester, inGameOnly: true)]
public class QuestCommand : CommandGroup
{
    Logger _logger = LogManager.CreateLogger<QuestCommand>();

    [Command("advance", "Advances a quest by a single step\n Usage: advance", inGameOnly: true)]
    public string Advance(string[] @params, BattleClient invokerClient)
    {
        try
        {
            invokerClient.InGameClient.Game.QuestManager.Advance();
            return "Advancing main quest line";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    [Command("sideadvance", "Advances a side-quest by a single step\n Usage: sideadvance", inGameOnly: true)]
    public string SideAdvance(string[] @params, BattleClient invokerClient)
    {
        try
        {
            invokerClient.InGameClient.Game.QuestManager.SideAdvance();
            return "Advancing side quest line";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    [Command("event", "Launches chosen side-quest by snoID\n Usage: event snoId", inGameOnly: true)]
    public string Event(string[] @params, BattleClient invokerClient)
    {
        if (@params == null)
            return Fallback();

        if (@params.Length != 1)
            return "Invalid arguments. Type 'help text public' to get help.";

        var questId = int.Parse(@params[0]);

        try
        {
            invokerClient.InGameClient.Game.QuestManager.LaunchSideQuest(questId, true);
            return "Advancing side quest line";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    [Command("timer", "Send broadcast text message.\n Usage: public 'message'", inGameOnly: true)]
    public string Timer(string[] @params, BattleClient invokerClient)
    {
        if (@params == null)
            return Fallback();

        if (@params.Length != 2)
            return "Invalid arguments. Type 'help text public' to get help.";

        if (!int.TryParse(@params[0], out var eventId) || !int.TryParse(@params[1], out var duration))
            return "Invalid arguments. Type 'help text public' to get help.";
        
        invokerClient.InGameClient.Game.QuestManager.LaunchQuestTimer(eventId, (float)duration, (_) => { });

        return "Message sent.";
    }
    
    [Command("set", "Advance to a specific quest step.\n Usage: quest to [questId] [step]", inGameOnly: true)]
    public string Set(string[] @params, BattleClient invokerClient)
    {
        if (@params == null)
            return Fallback();

        if (@params.Length != 2)
            return "Invalid arguments. Type 'help quest to' to get help.";

        if (!int.TryParse(@params[0], out var questId) || !int.TryParse(@params[1], out var step))
            return "Invalid arguments. Type 'help quest to' to get help.";

        try
        {
            invokerClient.InGameClient.Game.QuestManager.AdvanceTo(questId, step);
            var questName = invokerClient.InGameClient.Game.GetCurrentQuestName(true);
            _logger.Warn($"Advancing world to $[bold]${questName}$[/]$ quest on step $[bold]${step}$[/]$. $[red3_1]$There may be some $[/]$$[red3_1 bold underline]$unexpected$[/]$$[red3_1]$ behaviour.");

            return $"Advancing to quest {questName} step {step}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }


    [Command("get", "Gets the name of a specific quest id\n Usage: quest get [questId]", inGameOnly: true)]
    public string Get(string[] @params, BattleClient invokerClient)
    {
        if (@params == null)
            return Fallback();

        if (@params.Length != 2)
            return "Invalid arguments. Type 'help quest to' to get help.";

        if (!int.TryParse(@params[0], out var questId) || !int.TryParse(@params[1], out var step))
            return "Invalid arguments. Type 'help quest to' to get help.";

        try
        {
            invokerClient.InGameClient.Game.QuestManager.AdvanceTo(questId, step);
            var questName = invokerClient.InGameClient.Game.GetCurrentQuestName(true, questId);
            _logger.Warn($"Advancing world to $[bold]${questName}$[/]$ quest on step $[bold]${step}$[/]$. $[red3_1]$There may be some $[/]$$[red3_1 bold underline]$unexpected$[/]$$[red3_1]$ behaviour.");

            return $"Advancing to quest {questName} step {step}";
        }
        catch (Exception e)
        {
            _logger.ErrorException(e, e.Message);
            return e.Message;
        }
    }

    [Command("info", "Retrieves information about quest states.\n Usage: info", inGameOnly: true)]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient.InGameClient is null)
            return "You are not in-game.";
        if (invokerClient.InGameClient.Game?.QuestManager is not {} questManager)
            return "No quests found.";

        var act = questManager.Game.GetCurrentActName(true);
        var quest = questManager.Game.GetCurrentQuestName(true);

        //var quest = questManager.Game.CurrentQuest;
        var questStep = questManager.Game.CurrentStep;
        var currentSideQuest = questManager.Game.CurrentSideQuest;
        var currentSideQuestStep = questManager.Game.CurrentSideStep;
        var isValidAct = questManager.Game.IsValidAct();
        StrBuilder builder = new();

        builder.AppendIf(isValidAct && currentSideQuest != -1, $" - Side Quest: {currentSideQuest}");
        builder.AppendIf(isValidAct && currentSideQuestStep != -1, $" - Side Quest Step: {currentSideQuestStep}");
        builder.Append(act);
        builder.AppendIf(isValidAct, $"{quest}");
        builder.AppendIf(isValidAct && questStep != -1, $"Step: {questStep}");
        _logger.Debug($"Quest Info.: {act} {quest}");
        return builder.ToString("\n");
    }
}