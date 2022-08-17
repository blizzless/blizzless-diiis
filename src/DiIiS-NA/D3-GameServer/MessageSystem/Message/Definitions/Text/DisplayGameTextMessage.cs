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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Text
{
    [Message(new[] { Opcodes.DisplayGameTextMessage, Opcodes.DisplayGameChatTextMessage })]
    public class DisplayGameTextMessage : GameMessage
    {
        public string Message;
        public int? Param1;
        public int? Param2;
        public DisplayGameTextMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Message = buffer.ReadCharArray(1024);
            if (buffer.ReadBool())
            {
                Param1 = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                Param2 = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteCharArray(1024, Message);
            buffer.WriteBool(Param1.HasValue);
            if (Param1.HasValue)
            {
                buffer.WriteInt(32, Param1.Value);
            }
            buffer.WriteBool(Param2.HasValue);
            if (Param2.HasValue)
            {
                buffer.WriteInt(32, Param2.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DisplayGameTextMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Message: \"" + Message + "\"");
            if (Param1.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Param1.Value: 0x" + Param1.Value.ToString("X8") + " (" + Param1.Value + ")");
            }
            if (Param2.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Param2.Value: 0x" + Param2.Value.ToString("X8") + " (" + Param2.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
