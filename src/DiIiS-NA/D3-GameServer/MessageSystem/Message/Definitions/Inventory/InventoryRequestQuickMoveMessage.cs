using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
    [Message(Opcodes.InventoryRequestQuickMoveMessage, Consumers.Inventory)]
    public class InventoryRequestQuickMoveMessage : GameMessage
    {
        public uint ItemID;
        public int Field1;
        public int DestEquipmentSlot;
        public int DestRowStart;
        public int DestRowEnd;

        public override void Parse(GameBitBuffer buffer)
        {
            ItemID = buffer.ReadUInt(32);
            Field1 = buffer.ReadInt(32);
            //DestEquipmentSlot = buffer.ReadInt(5) + (-1); //2.6.10
            DestEquipmentSlot = buffer.ReadInt(6) + (-1); //2.7.0
            DestRowStart = buffer.ReadInt(32);
            DestRowEnd = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ItemID);
            buffer.WriteInt(32, Field1);
            //buffer.WriteInt(5, DestEquipmentSlot - (-1)); //2.6.10
            buffer.WriteInt(6, DestEquipmentSlot - (-1)); //2.7.0
            buffer.WriteInt(32, DestRowStart);
            buffer.WriteInt(32, DestRowEnd);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryRequestQuickMoveMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + ItemID.ToString("X8") + " (" + ItemID + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + DestEquipmentSlot.ToString("X8") + " (" + DestEquipmentSlot + ")");
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + DestRowStart.ToString("X8") + " (" + DestRowStart + ")");
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + DestRowEnd.ToString("X8") + " (" + DestRowEnd + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
