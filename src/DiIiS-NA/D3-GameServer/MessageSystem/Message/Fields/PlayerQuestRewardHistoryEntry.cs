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
    public class PlayerQuestRewardHistoryEntry
    {
        public int /* sno */ snoQuest;
        public int Field1;

        public enum Difficulty
        {
            Normal = 0,
            Nightmare = 1,
            Hell = 2,
            Inferno = 3,
        }

        public Difficulty Field2;

        public void Parse(GameBitBuffer buffer)
        {
            snoQuest = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = (Difficulty)buffer.ReadInt(8);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoQuest);
            buffer.WriteInt(32, Field1);
            buffer.WriteInt(8, (int)Field2);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerQuestRewardHistoryEntry:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("snoQuest: 0x" + snoQuest.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad);
            b.AppendLine("Field2: " + Field2.ToString());
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
