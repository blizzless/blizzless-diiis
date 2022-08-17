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
    public class HirelingInfo
    {
        public int HirelingIndex;
        public int GbidName;
        public bool Dead;
        public int Skill1SNOId;
        public int Skill2SNOId;
        public int Skill3SNOId;
        public int Skill4SNOId;
        public int annItems;

        public void Parse(GameBitBuffer buffer)
        {
            HirelingIndex = buffer.ReadInt(2);
            GbidName = buffer.ReadInt(32);
            Dead = buffer.ReadBool();
            Skill1SNOId = buffer.ReadInt(32);
            Skill2SNOId = buffer.ReadInt(32);
            Skill3SNOId = buffer.ReadInt(32);
            Skill4SNOId = buffer.ReadInt(32);
            annItems = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, HirelingIndex);
            buffer.WriteInt(32, GbidName);
            buffer.WriteBool(Dead);
            buffer.WriteInt(32, Skill1SNOId);
            buffer.WriteInt(32, Skill2SNOId);
            buffer.WriteInt(32, Skill3SNOId);
            buffer.WriteInt(32, Skill4SNOId);
            buffer.WriteInt(32, annItems);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingInfo:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("HirelingIndex: 0x" + HirelingIndex.ToString("X8") + " (" + HirelingIndex + ")");
            b.Append(' ', pad);
            //b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad);
            //b.AppendLine("Level: 0x" + Level.ToString("X8") + " (" + Level + ")");
            b.Append(' ', pad);
            b.AppendLine("GbidName: 0x" + GbidName.ToString("X8") + " (" + GbidName + ")");
            b.Append(' ', pad);
            b.AppendLine("Dead: " + (Dead ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("Skill1SNOId: 0x" + Skill1SNOId.ToString("X8") + " (" + Skill1SNOId + ")");
            b.Append(' ', pad);
            b.AppendLine("Skill2SNOId: 0x" + Skill2SNOId.ToString("X8") + " (" + Skill2SNOId + ")");
            b.Append(' ', pad);
            b.AppendLine("Skill3SNOId: 0x" + Skill3SNOId.ToString("X8") + " (" + Skill3SNOId + ")");
            b.Append(' ', pad);
            b.AppendLine("Skill4SNOId: 0x" + Skill4SNOId.ToString("X8") + " (" + Skill4SNOId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
