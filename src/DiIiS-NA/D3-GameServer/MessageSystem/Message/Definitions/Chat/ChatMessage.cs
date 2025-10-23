using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Chat
{
    [Message(Opcodes.ChatMessage)]
    public class ChatMessage : GameMessage
    {
        public int Color;
        public int PlayerIndex;
        public string Message;

        public override void Parse(GameBitBuffer buffer)
        {
            Color = buffer.ReadInt(2);
            PlayerIndex = buffer.ReadInt(4) + (-1);
            Message = buffer.ReadCharArray(1024);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, Color);
            buffer.WriteInt(4, PlayerIndex - (-1));
            buffer.WriteCharArray(1024, Message);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ChatMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Color: 0x" + Color.ToString("X8") + " (" + Color + ")");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("Message: \"" + Message + "\"");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
