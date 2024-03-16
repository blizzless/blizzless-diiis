using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.InterstitialMessage)]
    public class InterstitialMessage : GameMessage
    {
        public int PlayerClass;
        public bool IsMale;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerClass = buffer.ReadInt(4) + (-1);
            IsMale = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, PlayerClass - (-1));
            buffer.WriteBool(IsMale);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InterstitialMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerClass: 0x" + PlayerClass.ToString("X8") + " (" + PlayerClass + ")");
            b.Append(' ', pad); b.AppendLine("IsMale: " + (IsMale ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
