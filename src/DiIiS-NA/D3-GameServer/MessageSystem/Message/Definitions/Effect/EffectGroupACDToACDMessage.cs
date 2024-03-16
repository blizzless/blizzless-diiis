using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect
{
    [Message(Opcodes.EffectGroupACDToACDMessage)]
    public class EffectGroupACDToACDMessage : GameMessage
    {
        public int? /* sno */ EffectSNOId; // the effect to play
        public uint ActorID; // where the effect starts
        public uint TargetID; // where the effect will travel to

        public EffectGroupACDToACDMessage() : base(Opcodes.EffectGroupACDToACDMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            EffectSNOId = buffer.ReadInt(32);
            ActorID = buffer.ReadUInt(32);
            TargetID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, EffectSNOId.Value);
            buffer.WriteUInt(32, ActorID);
            buffer.WriteUInt(32, TargetID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EffectGroupACDToACDMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("EffectSNOId: 0x" + EffectSNOId.Value.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("TargetID: 0x" + TargetID.ToString("X8") + " (" + TargetID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
