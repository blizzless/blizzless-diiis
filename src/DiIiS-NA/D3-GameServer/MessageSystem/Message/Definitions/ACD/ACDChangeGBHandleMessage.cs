using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDChangeGBHandleMessage)]
    public class ACDChangeGBHandleMessage : GameMessage
    {
        public int Field0;
        public GBHandle Field1;

        public ACDChangeGBHandleMessage() : base(Opcodes.ACDChangeGBHandleMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = new GBHandle();
            Field1.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            Field1.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDChangeGBHandleMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            Field1.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
