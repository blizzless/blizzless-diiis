using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class VisualEquipment
    {
        // MaxLength = 8
        public VisualItem[] Equipment;
        // MaxLength = 4
        public VisuaCosmeticItem[] CosmeticEquipment;

        public void Parse(GameBitBuffer buffer)
        {
            Equipment = new VisualItem[8];
            for (int i = 0; i < Equipment.Length; i++)
            {
                Equipment[i] = new VisualItem();
                Equipment[i].Parse(buffer);
            }
            CosmeticEquipment = new VisuaCosmeticItem[4];
            for (int i = 0; i < CosmeticEquipment.Length; i++)
            {
                CosmeticEquipment[i] = new VisuaCosmeticItem();
                CosmeticEquipment[i].Parse(buffer);
            }
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < Equipment.Length; i++)
            {
                Equipment[i].Encode(buffer);
            }
            if (CosmeticEquipment == null)
            {
                CosmeticEquipment = new VisuaCosmeticItem[4];
                for (int i = 0; i < 4; i++)
                {
                    CosmeticEquipment[i] = new VisuaCosmeticItem() { GbId = -1 };
                }
            }
            for (int i = 0; i < CosmeticEquipment.Length; i++)
            {
                CosmeticEquipment[i].Encode(buffer);
            }
            
            
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VisualEquipment:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < Equipment.Length; i++)
            {
                Equipment[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
