using System;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("conversation", "Starts a conversation. \n Usage: conversation snoConversation", inGameOnly: true)]
public class ConversationCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Conversation(string[] @params, BattleClient invokerClient)
    {
        if (@params.Length != 1)
            return "Invalid arguments. Type 'help conversation' to get help.";

        try
        {
            var conversation = MPQStorage.Data.Assets[SNOGroup.Conversation][int.Parse(@params[0])];
            invokerClient.InGameClient.Player.Conversations.StartConversation(int.Parse(@params[0]));
            return $"Started conversation {conversation.FileName}";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}