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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Chat
{
    [Message(Opcodes.TryChatMessage)]
    public class TryChatMessage : GameMessage
    {
        public int Destination;
        public int PlayerTarget;
        public string Message;

        public override void Parse(GameBitBuffer buffer)
        {
            Destination = buffer.ReadInt(3);
            PlayerTarget = buffer.ReadInt(4) + (-1);
            Message = buffer.ReadCharArray(1024);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, Destination);
            buffer.WriteInt(4, PlayerTarget - (-1));
            buffer.WriteCharArray(1024, Message);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TryChatMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Destination: 0x" + Destination.ToString("X8") + " (" + Destination + ")");
            b.Append(' ', pad); b.AppendLine("PlayerTarget: 0x" + PlayerTarget.ToString("X8") + " (" + PlayerTarget + ")");
            b.Append(' ', pad); b.AppendLine("Message: \"" + Message + "\"");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
