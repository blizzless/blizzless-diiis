using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Tick
{
    [Message(Opcodes.EndOfTickMessage)]
    public class EndOfTickMessage : GameMessage
    {
        public int TickEnding;
        public int TickNext;

        public EndOfTickMessage() : base(Opcodes.EndOfTickMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            TickEnding = buffer.ReadInt(32);
            TickNext = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, TickEnding);
            buffer.WriteInt(32, TickNext);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EndOfTickMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("TickEnding: 0x" + TickEnding.ToString("X8") + " (" + TickEnding + ")");
            b.Append(' ', pad); b.AppendLine("TickNext: 0x" + TickNext.ToString("X8") + " (" + TickNext + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
