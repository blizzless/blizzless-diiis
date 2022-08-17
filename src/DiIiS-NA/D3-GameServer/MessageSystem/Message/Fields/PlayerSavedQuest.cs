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
    public class PlayerSavedQuest
    {
        public int /* sno */ QuestSNO;
        public int CurrentStepUID;
        public int[] ObjectiveState;
        public int[] ConditionState;

        public void Parse(GameBitBuffer buffer)
        {
            QuestSNO = buffer.ReadInt(32);
            CurrentStepUID = buffer.ReadInt(32);
            ObjectiveState = new int[20];
            for (int i = 0; i < ObjectiveState.Length; i++) ObjectiveState[i] = buffer.ReadInt(32);
            ConditionState = new int[20];
            for (int i = 0; i < ConditionState.Length; i++) ConditionState[i] = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, QuestSNO);
            buffer.WriteInt(32, CurrentStepUID);
            for (int i = 0; i < ObjectiveState.Length; i++) buffer.WriteInt(32, ObjectiveState[i]);
            for (int i = 0; i < ConditionState.Length; i++) buffer.WriteInt(32, ConditionState[i]);
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
