using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(new[] { Opcodes.PlayerBusyMessage, Opcodes.PlayerLorePlayingMessage, Opcodes.RiftJoinAcceptedMessage, Opcodes.RiftVoteContinueMessage, Opcodes.RiftPaymentAcceptedMessage
    , Opcodes.CameraTriggerFadeToBlackMessage, Opcodes.ClientVoteKickVote, Opcodes.SoftPauseStateMessage, Opcodes.AchievementsAvailability, Opcodes.RequestSoftPauseStateChangeMessage
    , Opcodes.BoolDataMessage15})]
    public class BoolDataMessage : GameMessage
    {
        public bool Field0;
        public BoolDataMessage(Opcodes id) :base(id)
        { }
        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BoolDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
