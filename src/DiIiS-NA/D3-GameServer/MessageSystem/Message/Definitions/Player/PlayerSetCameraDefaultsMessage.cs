using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
     [Message(new[] { Opcodes.PlayerSetCameraDefaultsMessage })]
    public class PlayerSetCameraDefaultsMessage : GameMessage
    {

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSetCameraDefaultsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
