using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.RareItemNameMessage)]
    public class RareItemNameMessage : GameMessage
    {
        public uint ann;
        public RareItemName RareItemName;

        public RareItemNameMessage() : base(Opcodes.RareItemNameMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ann = buffer.ReadUInt(32);
            RareItemName = new RareItemName();
            RareItemName.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ann);
            RareItemName.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RareItemNameMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ann: 0x" + ann.ToString("X8") + " (" + ann + ")");
            RareItemName.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
