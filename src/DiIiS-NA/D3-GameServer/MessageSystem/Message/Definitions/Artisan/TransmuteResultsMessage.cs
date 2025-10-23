using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.TransmuteResultsMessage, Consumers.Player)]
    public class TransmuteResultsMessage : GameMessage
    {
        public int annItem;
        public int /* gbid */ GBIDPower;
        public int Type;
        public int /* gbid */ GBIDFakeItem;
        public long FakeItemStackCount;
        public TransmuteResultsMessage() : base(Opcodes.TransmuteResultsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            annItem = buffer.ReadInt(32);
            GBIDPower = buffer.ReadInt(32);
            Type = buffer.ReadInt(32);
            GBIDFakeItem = buffer.ReadInt(32);
            FakeItemStackCount = buffer.ReadInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, annItem);
            buffer.WriteInt(32, GBIDPower);
            buffer.WriteInt(32, Type);
            buffer.WriteInt(32, GBIDFakeItem);
            buffer.WriteInt64(64, FakeItemStackCount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TransmuteResultsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annItem: 0x" + annItem.ToString("X8") + " (" + annItem + ")");
            b.Append(' ', pad); b.AppendLine("GBIDPower: 0x" + GBIDPower.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("GBIDFakeItem: 0x" + GBIDFakeItem.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("FakeItemStackCount: 0x" + FakeItemStackCount.ToString("X8") + " (" + FakeItemStackCount + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
