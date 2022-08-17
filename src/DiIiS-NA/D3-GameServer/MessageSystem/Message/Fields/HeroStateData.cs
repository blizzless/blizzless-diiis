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
    namespace BlizzLess.Net.GS.Message.Fields
    {
        public class HeroStateData
        {
            public int LastPlayedAct;
            public int HighestUnlockedAct;
            public int PlayedFlags;
            public PlayerSavedData PlayerSavedData;
            public PlayerQuestRewardHistoryEntry[] tQuestRewardHistory;

            public void Parse(GameBitBuffer buffer)
            {
                LastPlayedAct = buffer.ReadInt(32);
                HighestUnlockedAct = buffer.ReadInt(32);
                PlayedFlags = buffer.ReadInt(32);
                PlayerSavedData = new PlayerSavedData();
                PlayerSavedData.Parse(buffer);
                tQuestRewardHistory = new PlayerQuestRewardHistoryEntry[buffer.ReadUInt(7)];
                for (int i = 0; i < tQuestRewardHistory.Length; i++)
                {
                    tQuestRewardHistory[i] = new PlayerQuestRewardHistoryEntry();
                    tQuestRewardHistory[i].Parse(buffer);
                }
            }

            public void Encode(GameBitBuffer buffer)
            {
                buffer.WriteInt(32, LastPlayedAct);
                buffer.WriteInt(32, HighestUnlockedAct);
                buffer.WriteInt(32, PlayedFlags);
                PlayerSavedData.Encode(buffer);
                buffer.WriteInt(7, tQuestRewardHistory.Length);
                for (int i = 0; i < tQuestRewardHistory.Length; i++)
                {
                    tQuestRewardHistory[i].Encode(buffer);
                }
            }

            public void AsText(StringBuilder b, int pad)
            {
                b.Append(' ', pad);
                b.AppendLine("HeroStateData:");
                b.Append(' ', pad++);
                b.AppendLine("{");
                b.Append(' ', pad);
                b.AppendLine("LastPlayedAct: 0x" + LastPlayedAct.ToString("X8") + " (" + LastPlayedAct + ")");
                b.Append(' ', pad);
                b.AppendLine("HighestUnlockedAct: 0x" + HighestUnlockedAct.ToString("X8") + " (" + HighestUnlockedAct + ")");
                b.Append(' ', pad);
                b.AppendLine("PlayedFlags: 0x" + PlayedFlags.ToString("X8") + " (" + PlayedFlags + ")");
                b.Append(' ', pad);
                //b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
                b.Append(' ', pad);
                //b.AppendLine("PlayerFlags: 0x" + PlayerFlags.ToString("X8") + " (" + PlayerFlags + ")");
                PlayerSavedData.AsText(b, pad);
                b.Append(' ', pad);
                //b.AppendLine("QuestRewardHistoryEntriesCount: 0x" + QuestRewardHistoryEntriesCount.ToString("X8") + " (" + QuestRewardHistoryEntriesCount + ")");
                b.Append(' ', pad);
                b.AppendLine("tQuestRewardHistory:");
                b.Append(' ', pad);
                b.AppendLine("{");
                for (int i = 0; i < tQuestRewardHistory.Length; i++)
                {
                    tQuestRewardHistory[i].AsText(b, pad + 1);
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
}
