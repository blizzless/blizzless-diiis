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
    public class UnlockableRandomAffixProperties
    {
        public int field0;
        public bool field1;

        public void Parse(GameBitBuffer buffer)
        {
            field0 = buffer.ReadInt(32);
            field1 = buffer.ReadBool();
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, field0);
            buffer.WriteBool(field1);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("UnlockableRandomAffixProperties:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("field0: 0x" + field0.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("field1: 0x" + field1);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
