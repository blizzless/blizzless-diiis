using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Conversation
{
    [Message(Opcodes.AdvanceConvMessage, Consumers.Conversations)]
    class AdvanceConvMessage : GameMessage
    {
        public int SNOConversation;
        public int PlayLineParamsId;

        public override void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
            PlayLineParamsId = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
            buffer.WriteInt(32, PlayLineParamsId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AdvanceConvMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOConversation: 0x" + SNOConversation.ToString("X8") + " (" + SNOConversation + ")");
            b.Append(' ', pad); b.AppendLine("PlayLineParamsId: 0x" + PlayLineParamsId.ToString("X8") + " (" + PlayLineParamsId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
