using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateSyncMessage)]
    public class ACDTranslateSyncMessage : GameMessage
    {
        public uint ActorId;
        public Vector3D Position;
        public bool? Snap;
        public int? Field3;

        public ACDTranslateSyncMessage() : base(Opcodes.ACDTranslateSyncMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            Position = new Vector3D();
            Position.Parse(buffer);
            if (buffer.ReadBool())
            {
                Snap = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                Field3 = buffer.ReadInt(16);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            Position.Encode(buffer);
            buffer.WriteBool(Snap.HasValue);
            if (Snap.HasValue)
            {
                buffer.WriteBool(Snap.Value);
            }
            buffer.WriteBool(Field3.HasValue);
            if (Field3.HasValue)
            {
                buffer.WriteInt(16, Field3.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateSyncMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            Position.AsText(b, pad);
            if (Snap.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Snap.Value: " + (Snap.Value ? "true" : "false"));
            }
            if (Field3.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Field3.Value: 0x" + Field3.Value.ToString("X8") + " (" + Field3.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
