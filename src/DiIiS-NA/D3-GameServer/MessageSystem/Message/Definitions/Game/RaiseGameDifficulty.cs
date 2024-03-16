using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(new[] { Opcodes.RaiseGameDifficulty }, Consumers.Game)]
    public class RaiseGameDifficulty : GameMessage
    {
        public int PlayerIndex;
        public RaiseGameDifficulty() : base(Opcodes.RaiseGameDifficulty) { }

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