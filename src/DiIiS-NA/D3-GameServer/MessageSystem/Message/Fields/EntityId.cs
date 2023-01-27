using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class EntityId
    {
        public long High;
        public long Low;

        public void Parse(GameBitBuffer buffer)
        {
            High = buffer.ReadInt64(64);
            Low = buffer.ReadInt64(64);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt64(64, High);
            buffer.WriteInt64(64, Low);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EntityId:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: 0x" + High.ToString("X16"));
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Low.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
