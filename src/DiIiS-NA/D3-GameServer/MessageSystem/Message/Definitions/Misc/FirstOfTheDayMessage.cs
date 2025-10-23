using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.FirstOfTheDayMessage)]
    public class FirstOfTheDayMessage : GameMessage
    {
        public int Field0;
        public long Field1;
        public long Field2;

        public FirstOfTheDayMessage() : base(Opcodes.FirstOfTheDayMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = buffer.ReadInt64(64);
            Field2 = buffer.ReadInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt64(64, Field1);
            buffer.WriteInt64(64, Field2);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FirstOfTheDayMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
