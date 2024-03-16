using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(new[] { Opcodes.DungeonFinderClosingMessage })]
    public class DungeonFinderClosingMessage : GameMessage
    {
        public int Field0;
        public int Field1;
        public DungeonFinderClosingMessage() : base(Opcodes.DungeonFinderClosingMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DungeonFinderClosingMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    /*
     public class DungeonFinderClosingMessage : GameMessage
    {
        public int TickClosing;
        public int RiftLevel;

        public override void Parse(GameBitBuffer buffer)
        {
            TickClosing = buffer.ReadInt(32);
            RiftLevel = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, TickClosing);
            buffer.WriteInt(32, RiftLevel);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DungeonFinderClosingMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("TickClosing: 0x" + TickClosing.ToString("X8") + " (" + TickClosing + ")");
            b.Append(' ', pad); b.AppendLine("RiftLevel: 0x" + RiftLevel.ToString("X8") + " (" + RiftLevel + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
     */
}
