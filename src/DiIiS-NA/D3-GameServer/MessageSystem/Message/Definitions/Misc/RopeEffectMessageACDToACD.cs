using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.RopeEffectMessageACDToACD)]
    public class RopeEffectMessageACDToACD : GameMessage
    {
        public int /* sno */ RopeSNO;
        public int StartSourceActorId;
        public int Field2;  // always seems to be 4
        public int DestinationActorId;
        public int Field4;  // always seems to be 1
        public bool Field5;

        public RopeEffectMessageACDToACD() : base(Opcodes.RopeEffectMessageACDToACD) { }

        public override void Parse(GameBitBuffer buffer)
        {
            RopeSNO = buffer.ReadInt(32);
            StartSourceActorId = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(5);
            DestinationActorId = buffer.ReadInt(32);
            Field4 = buffer.ReadInt(5);
            Field5 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, RopeSNO);
            buffer.WriteInt(32, StartSourceActorId);
            buffer.WriteInt(5, Field2);
            buffer.WriteInt(32, DestinationActorId);
            buffer.WriteInt(5, Field4);
            buffer.WriteBool(Field5);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RopeEffectMessageACDToACD:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("RopeSNO: 0x" + RopeSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("StartSourceActorId: 0x" + StartSourceActorId.ToString("X8") + " (" + StartSourceActorId + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', pad); b.AppendLine("DestinationActorId: 0x" + DestinationActorId.ToString("X8") + " (" + DestinationActorId + ")");
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
