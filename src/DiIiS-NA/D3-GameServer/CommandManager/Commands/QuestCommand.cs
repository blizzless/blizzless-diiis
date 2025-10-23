using System;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using FluentNHibernate.Utils;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("quest",
    "Retrieves information about quest states and manipulates quest progress.\n" +
    "Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]",
    Account.UserLevels.Tester, inGameOnly: true)]
public class QuestCommand : CommandGroup
{
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
            return $"Advancing to quest {questId} step {step}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    [Command("info", "Retrieves information about quest states.\n Usage: info", inGameOnly: true)]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient.InGameClient.Game?.QuestManager is not {} questManager)
            return "No quests found.";
            
        var act = questManager.CurrentAct;
        var quest = questManager.Game.CurrentQuest;
        var questStep = questManager.Game.CurrentStep;
        var currentSideQuest = questManager.Game.CurrentSideQuest;
        var currentSideQuestStep = questManager.Game.CurrentSideStep;
        return $"Act: {act}\n" +
               $"Quest: {quest}\n" +
               $"Quest Step: {questStep}\n" +
               $"Side Quest: {currentSideQuest}\n" +
               $"Side Quest Step: {currentSideQuestStep}";
    }
}