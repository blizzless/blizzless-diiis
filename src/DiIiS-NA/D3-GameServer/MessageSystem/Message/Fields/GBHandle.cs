using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class GBHandle
    {
        public int Type; // Probably.
        public int GBID;

        public void Parse(GameBitBuffer buffer)
        {
            Type = buffer.ReadInt(6) + (-2);
            GBID = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(6, Type - (-2));
            buffer.WriteUInt(32, (uint)GBID);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GBHandle:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad);
            b.AppendLine("GBID: 0x" + GBID.ToString("X8") + " (" + GBID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
