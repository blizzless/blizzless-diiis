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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class VisualItem
    {
        public int /* gbid */ GbId;
        public int DyeType;
        public int ItemEffectType;
        public int EffectLevel;

        public void Parse(GameBitBuffer buffer)
        {
            GbId = buffer.ReadInt(32);
            DyeType = buffer.ReadInt(5);
            ItemEffectType = buffer.ReadInt(4);
            EffectLevel = buffer.ReadInt(5) + (-1);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, GbId);
            buffer.WriteInt(5, DyeType);
            buffer.WriteInt(4, ItemEffectType);
            buffer.WriteInt(5, EffectLevel - (-1));
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VisualItem:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("GbId: 0x" + GbId.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("DyeType: 0x" + DyeType.ToString("X8") + " (" + DyeType + ")");
            b.Append(' ', pad);
            b.AppendLine("ItemEffectType: 0x" + ItemEffectType.ToString("X8") + " (" + ItemEffectType + ")");
            b.Append(' ', pad);
            b.AppendLine("EffectLevel: 0x" + EffectLevel.ToString("X8") + " (" + EffectLevel + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
