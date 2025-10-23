using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.TryTransmogItem, Consumers.Inventory)]
    public class TryTransmogItemMessage : GameMessage
    {
        public uint annItem;
        public int /* gbid */ GBIDTransmog;

        public TryTransmogItemMessage() : base(Opcodes.TryTransmogItem) { }

        public override void Parse(GameBitBuffer buffer)
        {
            annItem = buffer.ReadUInt(32);
            GBIDTransmog = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, annItem);
            buffer.WriteInt(32, GBIDTransmog);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TryTransmogItem:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annItem: 0x" + annItem.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("GBIDTransmog: 0x" + GBIDTransmog.ToString("X8") + " (" + GBIDTransmog + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
