using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
    [Message(Opcodes.InventoryRequestSocketMessage, Consumers.Inventory)]
    public class InventoryRequestSocketMessage : GameMessage
    {
        public uint annGem;
        public uint annItemToReceiveGem;

        public override void Parse(GameBitBuffer buffer)
        {
            annGem = buffer.ReadUInt(32);
            annItemToReceiveGem = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, annGem);
            buffer.WriteUInt(32, annItemToReceiveGem);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryRequestSocketMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annGem: 0x" + annGem.ToString("X8") + " (" + annGem + ")");
            b.Append(' ', pad); b.AppendLine("annItemToReceiveGem: 0x" + annItemToReceiveGem.ToString("X8") + " (" + annItemToReceiveGem + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
