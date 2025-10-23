using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.CameraZoomMessage)]
    public class CameraZoomMessage : GameMessage
    {
        public float Zoom;
        public bool Snap;
        public float Duration;

        public CameraZoomMessage() : base(Opcodes.CameraZoomMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Zoom = buffer.ReadFloat32();
            Snap = buffer.ReadBool();
            Duration = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Zoom);
            buffer.WriteBool(Snap);
            buffer.WriteFloat32(Duration);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CameraZoomMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Zoom: " + Zoom.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Snap: " + (Snap ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Duration: " + Duration.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
