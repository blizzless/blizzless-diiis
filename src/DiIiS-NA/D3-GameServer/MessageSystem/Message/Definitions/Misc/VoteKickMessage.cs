//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(new[] { Opcodes.StartVoteKick, Opcodes.ServerVoteKickVote })]
    class VoteKickMessage : GameMessage
    {
        public int PlayerIndexToKick;
        public int PlayerIndexInitiating;
        public string Message;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndexToKick = buffer.ReadInt(4) + (-1);
            PlayerIndexInitiating = buffer.ReadInt(4) + (-1);
            Message = buffer.ReadCharArray(1024);
        }   

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, PlayerIndexToKick - (-1));
            buffer.WriteInt(4, PlayerIndexInitiating - (-1));
            buffer.WriteCharArray(1024, Message);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VoteKickMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndexToKick: 0x" + PlayerIndexToKick.ToString("X8") + " (" + PlayerIndexToKick + ")");
            b.Append(' ', pad); b.AppendLine("PlayerIndexInitiating: 0x" + PlayerIndexInitiating.ToString("X8") + " (" + PlayerIndexInitiating + ")");
            b.Append(' ', pad); b.AppendLine("Message: " + Message);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
