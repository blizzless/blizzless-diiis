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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.GameSetupMessage)]
    public class GameSetupMessage : GameMessage
    {
        public int FirstHeartBeat;

        public GameSetupMessage() : base(Opcodes.GameSetupMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            FirstHeartBeat = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, FirstHeartBeat);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GameSetupMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FirstHeartBeat: 0x" + FirstHeartBeat.ToString("X8") + " (" + FirstHeartBeat + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
