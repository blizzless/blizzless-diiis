//Blizzless Project 2022 
using D3.Quests;
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
    [Message(Opcodes.QuestStepCompleteMessage)]
    public class QuestStepCompleteMessage : GameMessage
    {

        public QuestStepComplete QuestStepComplete;

        public QuestStepCompleteMessage() : base(Opcodes.QuestStepCompleteMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            QuestStepComplete = QuestStepComplete.ParseFrom(buffer.ReadBlob(32));
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, QuestStepComplete.ToByteArray());
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("QuestStepCompleteMessage:");
            b.Append(' ', pad++);
            b.Append(QuestStepComplete.ToString());
        }
    }
}
