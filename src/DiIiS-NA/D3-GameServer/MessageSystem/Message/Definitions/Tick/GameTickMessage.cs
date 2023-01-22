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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Tick
{
    [Message(Opcodes.GameTickMessage)]
    public class GameTickMessage : GameMessage
    {
        public int Tick;

        public GameTickMessage() : base(Opcodes.GameTickMessage) { }

        public GameTickMessage(int tick)
            : base(Opcodes.GameTickMessage)
        {
            Tick = tick;
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Tick = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Tick);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            
            b.Append(' ', pad);
            b.AppendLine("GameTickMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            //b.AppendLine("Tick: 0x" + Tick.ToString("X8") + " (" + Tick + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");

        }
    }
}
