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
    public class LearnedLore
    {
        public int Count;
        // MaxLength = 520
        
        public int /* sno */[] m_snoLoreLearned;

        public void Parse(GameBitBuffer buffer)
        {
            Count = buffer.ReadInt(32);
            m_snoLoreLearned = new int /* sno */[512];
            for (int i = 0; i < m_snoLoreLearned.Length; i++)
                m_snoLoreLearned[i] = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Count);
            for (int i = 0; i < 512; i++) buffer.WriteInt(32, m_snoLoreLearned[i]);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LearnedLore:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Count: 0x" + Count.ToString("X8") + " (" + Count + ")");
            b.Append(' ', pad);
            b.AppendLine("m_snoLoreLearned:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < m_snoLoreLearned.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < m_snoLoreLearned.Length; j++, i++)
                {
                    b.Append("0x" + m_snoLoreLearned[i].ToString("X8") + ", ");
                }
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
