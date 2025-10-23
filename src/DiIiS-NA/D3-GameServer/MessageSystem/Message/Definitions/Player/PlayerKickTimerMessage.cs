using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerKickTimerMessage)]
    public class PlayerKickTimerMessage : GameMessage
    {
        public int LastKickAttempt;
        public int LastSuccessfulKick;

        public PlayerKickTimerMessage() : base(Opcodes.PlayerKickTimerMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            LastKickAttempt = buffer.ReadInt(32);
            LastSuccessfulKick = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, LastKickAttempt);
            buffer.WriteInt(32, LastSuccessfulKick);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerKickTimerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("LastKickAttempt: 0x" + LastKickAttempt.ToString("X8") + " (" + LastKickAttempt + ")");
            b.Append(' ', pad); b.AppendLine("LastSuccessfulKick: 0x" + LastSuccessfulKick.ToString("X8") + " (" + LastSuccessfulKick + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
