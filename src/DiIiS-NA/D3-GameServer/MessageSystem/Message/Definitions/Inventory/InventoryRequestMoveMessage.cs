using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
    [Message(new[] { Opcodes.InventoryRequestMoveMessage, Opcodes.InventoryRequestMoveMessage1 }, Consumers.Inventory)]
    public class InventoryRequestMoveMessage : GameMessage
    {
        public uint ItemID; // Item's DynamicID
        public InvLoc Location;

        public override void Parse(GameBitBuffer buffer)
        {
            ItemID = buffer.ReadUInt(32);
            Location = new InvLoc();
            Location.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ItemID);
            Location.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryRequestMoveMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ItemID: 0x" + ItemID.ToString("X8") + " (" + ItemID + ")");
            Location.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
