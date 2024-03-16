using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class VisuaCosmeticItem
    {
        public int /* gbid */ GbId;

        public void Parse(GameBitBuffer buffer)
        {
            GbId = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, GbId);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VisualCosmeticItem:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: 0x" + GbId.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
