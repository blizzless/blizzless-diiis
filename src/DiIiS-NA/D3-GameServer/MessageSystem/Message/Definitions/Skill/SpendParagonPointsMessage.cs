using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Skill
{
    [Message(Opcodes.SpendParagonPointsMessage, Consumers.Player)]
    public class SpendParagonPointsMessage : GameMessage
    {
        public int /* gbid */ BonusGBID;
        public int Amount;
        public SpendParagonPointsMessage() : base(Opcodes.SpendParagonPointsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            BonusGBID = buffer.ReadInt(32);
            Amount = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, BonusGBID);
            buffer.WriteInt(32, Amount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SpendParagonPointsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + BonusGBID.ToString("X8") + " (" + BonusGBID + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Amount.ToString("X8") + " (" + Amount + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    [Message(Opcodes.ResetParagonPointsMessage, Consumers.Player)]
    public class ResetParagonPointsMessage : GameMessage
    {
        public int Field0;

        public ResetParagonPointsMessage() : base(Opcodes.ResetParagonPointsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ResetParagonPointsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
