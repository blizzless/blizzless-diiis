using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] {
        Opcodes.RessurectionPlayerMessage,
        },Consumers.Player)]
    public class RessurectionPlayerMessage : GameMessage
    {
        public int Choice;
        public RessurectionPlayerMessage(Opcodes opcode) : base(opcode) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Choice = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Choice);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RessurectionPlayerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Choice: 0x" + Choice.ToString("X8") + " (" + Choice + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
