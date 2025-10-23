using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class LoadoutItemData
    {
        public int annItem;
        public int[] Field1;
        public int field2;

        public void Parse(GameBitBuffer buffer)
        {
            annItem = buffer.ReadInt(32);
            Field1 = new int[buffer.ReadInt(2)];
            for (int i = 0; i < Field1.Length; i++) Field1[i] = buffer.ReadInt(32);
            field2 = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, annItem);
            buffer.WriteInt(2, Field1.Length);
            for (int i = 0; i < Field1.Length; i++) buffer.WriteInt(32, Field1[i]);
            field2 = buffer.ReadInt(32);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LoadoutItemData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
           
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
