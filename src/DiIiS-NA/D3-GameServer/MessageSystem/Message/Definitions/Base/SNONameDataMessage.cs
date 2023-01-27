using DiIiS_NA.GameServer.Core.Types.SNO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(Opcodes.PrefetchMessage)]
    public class PrefetchMessage : GameMessage
    {
        public SNOHandle Name;

        public PrefetchMessage() : base(Opcodes.PrefetchMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Name = new SNOHandle();
            Name.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Name.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PrefetchMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Name.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
