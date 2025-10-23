using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Conversation
{
    [Message(Opcodes.EndConversationMessage)]
    public class EndConversationMessage : GameMessage
    {
        public int SNOConversation;
        public uint ActorId;
        public int Field2;

        public EndConversationMessage() : base(Opcodes.EndConversationMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
            ActorId = buffer.ReadUInt(32);
            Field2 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
            buffer.WriteUInt(32, ActorId);
            buffer.WriteInt(32, Field2);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EndConversationMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOConversation: 0x" + SNOConversation.ToString("X8") + " (" + SNOConversation + ")");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
