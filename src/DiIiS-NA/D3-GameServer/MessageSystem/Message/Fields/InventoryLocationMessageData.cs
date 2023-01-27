using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class InventoryLocationMessageData
    {
        public uint OwnerID; // Player's DynamicID
        public int EquipmentSlot;
        public Vector2D InventoryLocation; // Row, column

        public void Parse(GameBitBuffer buffer)
        {
            OwnerID = buffer.ReadUInt(32);
            //EquipmentSlot = buffer.ReadInt(5) + (-1); //2.6.10
            EquipmentSlot = buffer.ReadInt(6) + (-1); //2.7.0
            InventoryLocation = new Vector2D();
            InventoryLocation.Parse(buffer);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, OwnerID);
            //buffer.WriteInt(5, EquipmentSlot - (-1)); //2.6.10
            buffer.WriteInt(6, EquipmentSlot - (-1)); //2.7.0
            InventoryLocation.Encode(buffer);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryLocationMessageData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("OwnerID: 0x" + OwnerID.ToString("X8") + " (" + OwnerID + ")");
            b.Append(' ', pad);
            b.AppendLine("EquipmentSlot: 0x" + EquipmentSlot.ToString("X8") + " (" + EquipmentSlot + ")");
            InventoryLocation.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
