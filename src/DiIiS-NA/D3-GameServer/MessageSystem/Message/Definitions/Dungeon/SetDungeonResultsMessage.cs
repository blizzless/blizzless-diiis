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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Dungeon
{
    [Message(Opcodes.SetDungeonResultsMessage)]
    public class SetDungeonResultsMessage : GameMessage
    {
        public int /* sno */ SNOQuestKill;
        public int QuestKillMonsterCounter;
        public int /* sno */ SNOQuestBonus1;
        public bool QuestBonus1Success;
        public int /* sno */ SNOQuestBonus2;
        public bool QuestBonus2Success;
        public int /* sno */ SNOQuestMastery;
        public bool QuestMasterySuccess;
        public bool QuestKillSuccess;
        public bool ShowTotalTime;
        public int TimeTaken;
        public int TargetTime;

        public SetDungeonResultsMessage() : base(Opcodes.SetDungeonResultsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNOQuestKill = buffer.ReadInt(32);
            QuestKillMonsterCounter = buffer.ReadInt(32);
            SNOQuestBonus1 = buffer.ReadInt(32);
            QuestBonus1Success = buffer.ReadBool();
            SNOQuestBonus2 = buffer.ReadInt(32);
            QuestBonus2Success = buffer.ReadBool();
            SNOQuestMastery = buffer.ReadInt(32);
            QuestMasterySuccess = buffer.ReadBool();
            QuestKillSuccess = buffer.ReadBool();
            ShowTotalTime = buffer.ReadBool();
            TimeTaken = buffer.ReadInt(32);
            TargetTime = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOQuestKill);
            buffer.WriteInt(32, QuestKillMonsterCounter);
            buffer.WriteInt(32, SNOQuestBonus1);
            buffer.WriteBool(QuestBonus1Success);
            buffer.WriteInt(32, SNOQuestBonus2);
            buffer.WriteBool(QuestBonus2Success);
            buffer.WriteInt(32, SNOQuestMastery);
            buffer.WriteBool(QuestMasterySuccess);
            buffer.WriteBool(QuestKillSuccess);
            buffer.WriteBool(ShowTotalTime);
            buffer.WriteInt(32, TimeTaken);
            buffer.WriteInt(32, TargetTime);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}
