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
    [Message(new[] { Opcodes.LowGameDifficulty }, Consumers.Game)]
    public class LowGameDifficulty : GameMessage
    {
        public int PlayerIndex;
        public LowGameDifficulty() : base(Opcodes.LowGameDifficulty) { }

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerIndexMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("RaiseDifficultyMessage");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}