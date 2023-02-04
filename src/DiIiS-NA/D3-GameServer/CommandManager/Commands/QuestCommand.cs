using System;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("quest",
    "Retrieves information about quest states and manipulates quest progress.\n Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]")]
public class QuestCommand : CommandGroup
{
    [DefaultCommand]
    public string Quest(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        return Info(@params, invokerClient);
    }

    [Command("advance", "Advances a quest by a single step\n Usage: advance")]
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

    [Command("sideadvance", "Advances a side-quest by a single step\n Usage: sideadvance")]
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

    [Command("event", "Launches chosen side-quest by snoID\n Usage: event snoId")]
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

    [Command("timer", "Send broadcasted text message.\n Usage: public 'message'")]
    public string Timer(string[] @params, BattleClient invokerClient)
    {
        if (@params == null)
            return Fallback();

        if (@params.Length != 2)
            return "Invalid arguments. Type 'help text public' to get help.";

        var eventId = int.Parse(@params[0]);
        var duration = int.Parse(@params[1]);

        invokerClient.InGameClient.Game.QuestManager.LaunchQuestTimer(eventId, (float)duration,
            new Action<int>((q) => { }));

        return "Message sent.";
    }

    [Command("info", "Retrieves information about quest states.\n Usage: info")]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Game?.QuestManager is not {} questManager)
            return "You can only invoke this command while in-game.";
            
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