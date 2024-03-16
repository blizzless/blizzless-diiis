using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class RareItemName
    {
        public bool Field0;
        public int /* sno */ snoAffixStringList;
        public int AffixStringListIndex;
        public int ItemStringListIndex;

        public void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
            snoAffixStringList = buffer.ReadInt(32);
            AffixStringListIndex = buffer.ReadInt(32);
            ItemStringListIndex = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
            buffer.WriteInt(32, snoAffixStringList);
            buffer.WriteInt(32, AffixStringListIndex);
            buffer.WriteInt(32, ItemStringListIndex);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RareItemName:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("snoAffixStringList: 0x" + snoAffixStringList.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("AffixStringListIndex: 0x" + AffixStringListIndex.ToString("X8") + " (" + AffixStringListIndex + ")");
            b.Append(' ', pad);
            b.AppendLine("ItemStringListIndex: 0x" + ItemStringListIndex.ToString("X8") + " (" + ItemStringListIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
