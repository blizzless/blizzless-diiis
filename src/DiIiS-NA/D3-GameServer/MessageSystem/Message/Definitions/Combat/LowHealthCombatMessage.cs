using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.LowHealthCombatMessage)]
    public class LowHealthCombatMessage : GameMessage
    {
        public float Field0;
        public int Field1;

        public LowHealthCombatMessage() : base(Opcodes.LowHealthCombatMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadFloat32();
            Field1 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Field0);
            buffer.WriteInt(32, Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LowHealthCombatMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + Field0.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
