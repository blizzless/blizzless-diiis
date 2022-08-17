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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling
{
    [Message(Opcodes.HirelingHireMessage, Consumers.SelectedNPC)]
    public class HirelingHireMessage : GameMessage
    {
        public HirelingHireMessage()
            : base(Opcodes.HirelingHireMessage)
        {
        }

        public uint HirelingId;

        public override void Parse(GameBitBuffer buffer)
        {
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, HirelingId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingHireMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HirelingId: 0x" + HirelingId.ToString("X8") + " (" + HirelingId + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
