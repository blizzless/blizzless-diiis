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
    public class SavedConversations
    {
        public byte[] Field0 = new byte[70];
        public int[] Field1;

        public void Parse(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 70; i++)
                Field0[i] = (byte)buffer.ReadInt(8);
            Field1 = new int /* sno */[buffer.ReadInt(10)];
            for (int i = 0; i < Field1.Length; i++) Field1[i] = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 70; i++)
                buffer.WriteInt(8, Field0[i]);
            for (int i = 0; i < Field1.Length; i++) buffer.WriteInt(32, Field1[i]);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0:");
            b.Append(' ', pad);
            b.AppendLine("{");

            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
