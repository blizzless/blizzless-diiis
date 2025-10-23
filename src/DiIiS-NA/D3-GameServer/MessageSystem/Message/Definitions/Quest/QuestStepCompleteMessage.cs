using D3.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
