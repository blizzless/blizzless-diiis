//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
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
