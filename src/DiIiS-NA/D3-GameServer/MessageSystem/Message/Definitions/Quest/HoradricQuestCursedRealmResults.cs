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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(new[] { Opcodes.HoradricQuestCursedRealmResults })]
    public class HoradricQuestCursedRealmResults : GameMessage
    {
        public int ActIndex;
        public int RewardAmount;

        public HoradricQuestCursedRealmResults() : base(Opcodes.HoradricQuestCursedRealmResults) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActIndex = buffer.ReadInt(32);
            RewardAmount = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActIndex);
            buffer.WriteInt(32, RewardAmount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HoradricQuestCursedRealmResults:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActIndex: 0x" + ActIndex.ToString("X8") + " (" + ActIndex + ")");
            b.Append(' ', pad); b.AppendLine("RewardAmount: 0x" + RewardAmount.ToString("X8") + " (" + RewardAmount + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
