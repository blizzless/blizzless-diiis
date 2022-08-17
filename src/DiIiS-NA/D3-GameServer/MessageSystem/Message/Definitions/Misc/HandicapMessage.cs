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
    [Message(new[] { Opcodes.HandicapMessage })]
    public class HandicapMessage : GameMessage
    {
        public uint Difficulty; // Actor's DynamicID

        public HandicapMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Difficulty = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, Difficulty);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HandicapMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Difficulty: 0x" + Difficulty.ToString("X8") + " (" + Difficulty + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
