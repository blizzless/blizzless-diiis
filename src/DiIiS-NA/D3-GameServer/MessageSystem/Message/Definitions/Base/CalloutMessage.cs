using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(Opcodes.CalloutMessage)]
    public class CalloutMessage : GameMessage
    {
        public int Type;
        public float? Amount;
        public int? Time;

        public CalloutMessage() : base(Opcodes.CalloutMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Type = buffer.ReadInt(5);
            if (buffer.ReadBool())
            {
                Amount = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                Time = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(5, Type);
            buffer.WriteBool(Amount.HasValue);
            if (Amount.HasValue)
            {
                buffer.WriteFloat32(Amount.Value);
            }
            buffer.WriteBool(Time.HasValue);
            if (Time.HasValue)
            {
                buffer.WriteInt(32, Time.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CalloutMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
