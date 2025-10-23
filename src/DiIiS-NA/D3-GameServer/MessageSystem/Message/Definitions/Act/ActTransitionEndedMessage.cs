using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act
{
    [Message(Opcodes.ActTransitionEndedMessage)]
    public class ActTransitionEndedMessage : GameMessage
    {
        public ActTransitionEndedMessage() : base(Opcodes.ActTransitionEndedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SimpleMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
