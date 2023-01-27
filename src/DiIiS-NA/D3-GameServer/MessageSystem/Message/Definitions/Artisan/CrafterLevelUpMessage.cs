using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.CrafterLevelUpMessage)]
    public class CrafterLevelUpMessage : GameMessage
    {
        public int Type;
        public int AnimTag;
        public int NewIdle;
        public int Level;

        public CrafterLevelUpMessage() : base((int)Opcodes.CrafterLevelUpMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Type = buffer.ReadInt(32);
            AnimTag = buffer.ReadInt(32);
            NewIdle = buffer.ReadInt(32);
            Level = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Type);
            buffer.WriteInt(32, AnimTag);
            buffer.WriteInt(32, NewIdle);
            buffer.WriteInt(32, Level);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CrafterLevelUpMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("AnimTag: 0x" + AnimTag.ToString("X8") + " (" + AnimTag + ")");
            b.Append(' ', pad); b.AppendLine("NewIdle: 0x" + NewIdle.ToString("X8") + " (" + NewIdle + ")");
            b.Append(' ', pad); b.AppendLine("Level: 0x" + Level.ToString("X8") + " (" + Level + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
