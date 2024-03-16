using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class HotbarButtonData
    {
        public int /* sno */ SNOSkill;
        public int RuneType;
        public int /* gbid */ ItemGBId;
        public int ItemAnn;


        public void Parse(GameBitBuffer buffer)
        {
            SNOSkill = buffer.ReadInt(32);
            RuneType = buffer.ReadInt(3) + (-1);
            ItemGBId = buffer.ReadInt(32);
            ItemAnn = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOSkill);
            buffer.WriteInt(3, RuneType - (-1));
            buffer.WriteInt(32, ItemGBId);
            buffer.WriteInt(32, ItemAnn);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HotbarButtonData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("m_snoPower: 0x" + SNOSkill.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("RuneType: " + RuneType);
            b.Append(' ', pad);
            b.AppendLine("m_gbidItem: 0x" + ItemGBId.ToString("X8"));
            b.AppendLine("ItemAnn: " + ItemAnn);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
