using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Skill
{
    [Message(new[] { Opcodes.AssignSkillMessage, Opcodes.AssignSkillToSlot, Opcodes.AssignTraitToSlot },  Consumers.Player)]
    public class AssignSkillMessage : GameMessage
    {
        public int /* sno */ SNOSkill;
        public int RuneIndex;
        public int SkillIndex;
        public int Field3;

        public override void Parse(GameBitBuffer buffer)
        {
            SNOSkill = buffer.ReadInt(32);
            RuneIndex = buffer.ReadInt(3) + (-1);
            SkillIndex = buffer.ReadInt(3);
            Field3 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOSkill);
            buffer.WriteInt(3, RuneIndex - (-1));
            buffer.WriteInt(5, SkillIndex);
            buffer.WriteInt(32, Field3);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AssignSkillMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOSkill: 0x" + SNOSkill.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("RuneIndex: 0x" + RuneIndex.ToString("X8") + " (" + RuneIndex + ")");
            b.Append(' ', pad); b.AppendLine("SkillIndex: 0x" + SkillIndex.ToString("X8") + " (" + SkillIndex + ")");
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
