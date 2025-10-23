using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDDestroyActorMessage)]
    public class ACDDestroyActorMessage : GameMessage
    {
        public uint ActorId; // Actor's DynamicID

        public ACDDestroyActorMessage(uint actorId)
            : base(Opcodes.ACDDestroyActorMessage)
        {
            ActorId = actorId;
        }

        public ACDDestroyActorMessage()
            : base(Opcodes.ACDDestroyActorMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDDestroyActorMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
