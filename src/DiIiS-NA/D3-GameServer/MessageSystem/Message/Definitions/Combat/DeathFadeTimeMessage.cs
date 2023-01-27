using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.SetDeathFadeTimeMessage)]
    public class DeathFadeTimeMessage : GameMessage
    {
        public uint Field0;
        public int Field1;
        public int Field2;
        public bool Field3;

        public DeathFadeTimeMessage() : base(Opcodes.SetDeathFadeTimeMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadUInt(32);
            Field1 = buffer.ReadInt(11) + (-1);
            Field2 = buffer.ReadInt(11);
            Field3 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, Field0);
            buffer.WriteInt(11, Field1 - (-1));
            buffer.WriteInt(11, Field2);
            buffer.WriteBool(Field3);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SetDeathFadeTimeMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', pad); b.AppendLine("Field3: " + (Field3 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
