using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.PlayCutsceneMessage)]
    public class PlayCutsceneMessage : GameMessage
    {
        public int Index;

        public PlayCutsceneMessage() : base(Opcodes.PlayCutsceneMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Index = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Index);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayCutsceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Index: 0x" + Index.ToString("X8") + " (" + Index + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
