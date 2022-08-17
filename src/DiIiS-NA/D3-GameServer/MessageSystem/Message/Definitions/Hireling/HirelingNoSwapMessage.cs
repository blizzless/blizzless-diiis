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
        [Message(new[] { Opcodes.HirelingNoSwapMessage })]
        public class HirelingNoSwapMessage : GameMessage
        {
            public int NewClass;

            public HirelingNoSwapMessage() : base(Opcodes.HirelingNoSwapMessage)
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
