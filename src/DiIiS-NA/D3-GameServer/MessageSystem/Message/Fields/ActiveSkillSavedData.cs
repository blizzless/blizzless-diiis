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
    public class ActiveSkillSavedData
    {
        public int snoSkill;
        public int snoRune;

        public void Parse(GameBitBuffer buffer)
        {
            snoSkill = buffer.ReadInt(32);
            snoRune = buffer.ReadInt(3) + (-1);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoSkill);
            buffer.WriteInt(3, snoRune - (-1));
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ActiveSkillSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("snoSkill: 0x" + snoSkill.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("snoRune: " + snoRune);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
