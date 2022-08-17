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
