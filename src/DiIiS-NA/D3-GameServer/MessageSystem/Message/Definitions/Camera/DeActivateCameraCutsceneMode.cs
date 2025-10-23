using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.DeactivateCameraCutsceneMessage, Consumers.Player)]
    public class DeActivateCameraCutsceneMode : GameMessage
    {
        public bool Activate;
        public DeActivateCameraCutsceneMode() : base(Opcodes.DeactivateCameraCutsceneMessage)
        {

        }
        public override void Parse(GameBitBuffer buffer)
        {
            Activate = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Activate);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BossZoomMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
