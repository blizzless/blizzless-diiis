using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling
{
    [Message(new[] { Opcodes.HirelingNewUnlocked })]
    public class HirelingNewUnlocked : GameMessage
    {
        public int NewClass;

        public HirelingNewUnlocked() : base(Opcodes.HirelingNewUnlocked)
        {
        }

        public override void Parse(GameBitBuffer buffer)
        {
            NewClass = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, NewClass);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingSwapMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("NewClass: 0x" + NewClass.ToString("X8") + " (" + NewClass + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}