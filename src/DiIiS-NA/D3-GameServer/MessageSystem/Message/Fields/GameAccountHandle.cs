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
    public class GameAccountHandle
    {
        public uint ID;
        public uint Program;
        public uint Region;

        public void Parse(GameBitBuffer buffer)
        {
            ID = buffer.ReadUInt(32);
            Program = buffer.ReadUInt(32);
            Region = buffer.ReadUInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ID);
            buffer.WriteUInt(32, Program);
            buffer.WriteUInt(32, Region);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EntityId:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: 0x" + ID.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Program.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Field2: 0x" + Region.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
