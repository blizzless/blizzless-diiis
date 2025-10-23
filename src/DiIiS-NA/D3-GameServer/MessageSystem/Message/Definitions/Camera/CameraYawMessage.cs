using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.CameraYawMessage)]
    public class CameraYawMessage : GameMessage
    {
        public float /* angle */ Angle;
        public bool Snap;
        public float Duration;

        public CameraYawMessage() : base(Opcodes.CameraYawMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Angle = buffer.ReadFloat32();
            Snap = buffer.ReadBool();
            Duration = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Angle);
            buffer.WriteBool(Snap);
            buffer.WriteFloat32(Duration);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CameraYawMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Snap: " + (Snap ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Duration: " + Duration.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
