using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Skill
{
    [Message(Opcodes.CancelChanneledSkillMessage, Consumers.Player)]
    public class CancelChanneledSkillMessage : GameMessage
    {
        public int PowerSNO;

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CancelChanneledSkillMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8") + " (" + PowerSNO + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
