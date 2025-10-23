using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.BossZoomMessage)]
    public class BossZoomMessage : GameMessage
    {
        public float Zoom;
        public float ZoomSpeed;

        public override void Parse(GameBitBuffer buffer)
        {
            Zoom = buffer.ReadFloat32();
            ZoomSpeed = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Zoom);
            buffer.WriteFloat32(ZoomSpeed);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BossZoomMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Zoom: " + Zoom.ToString("G"));
            b.Append(' ', pad); b.AppendLine("ZoomSpeed: " + ZoomSpeed.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
