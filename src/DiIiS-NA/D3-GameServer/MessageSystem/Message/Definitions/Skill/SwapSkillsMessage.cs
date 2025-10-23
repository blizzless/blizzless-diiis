using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Skill
{
    [Message(Opcodes.SwapSkillsMessage)]
    public class SwapSkillsMessage : GameMessage
    {
        public int SkillIndex;
        public int SkillIndex1;

        public override void Parse(GameBitBuffer buffer)
        {
            SkillIndex = buffer.ReadInt(3);
            SkillIndex1 = buffer.ReadInt(3);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(5, SkillIndex);
            buffer.WriteInt(5, SkillIndex1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SwapSkillsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SkillIndex: 0x" + SkillIndex.ToString("X8") + " (" + SkillIndex + ")");
            b.Append(' ', pad); b.AppendLine("SkillIndex1: 0x" + SkillIndex1.ToString("X8") + " (" + SkillIndex1 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
