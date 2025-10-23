using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(Opcodes.LoreMessage)]
    public class LoreMessage : GameMessage
    {
        public int LoreSNOId;

        public LoreMessage() : base(Opcodes.LoreMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            LoreSNOId = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, LoreSNOId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LoreMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("LoreSNOId: 0x" + LoreSNOId.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
