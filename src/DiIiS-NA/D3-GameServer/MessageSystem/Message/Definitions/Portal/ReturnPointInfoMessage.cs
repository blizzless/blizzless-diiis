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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal
{
    [Message(Opcodes.ReturnPointInfoMessage)]
    public class ReturnPointInfoMessage : GameMessage
    {
        public int SNO; // Portal's DynamicID

        public ReturnPointInfoMessage() : base(Opcodes.ReturnPointInfoMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNO);
            buffer.WriteInt(32, SNO);
            buffer.WriteInt(32, SNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PortalSpecifierMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNO: 0x" + SNO.ToString("X8") + " (" + SNO + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
