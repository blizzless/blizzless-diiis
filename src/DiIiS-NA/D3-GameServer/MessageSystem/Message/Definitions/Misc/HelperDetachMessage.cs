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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(new[] { Opcodes.HelperDetachMessage })]
    public class HelperDetachMessage : GameMessage
    {
        public uint annHelper;

        public HelperDetachMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            annHelper = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, annHelper);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HelperDetachMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annHelper: 0x" + annHelper.ToString("X8") + " (" + annHelper + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
