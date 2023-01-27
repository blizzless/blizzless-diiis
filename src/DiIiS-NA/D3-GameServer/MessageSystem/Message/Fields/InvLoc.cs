using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class InvLoc
    {
        public uint OwnerID; // Owner's DynamicID
        public int EquipmentSlot;
        public int Column;
        public int Row;

        public void Parse(GameBitBuffer buffer)
        {
            OwnerID = buffer.ReadUInt(32);
            //EquipmentSlot = buffer.ReadInt(5) + (-1); // 2.6.10
            EquipmentSlot = buffer.ReadInt(6) + (-1); // 2.7.0
            Column = buffer.ReadInt(32);
            Row = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, OwnerID);
            //buffer.WriteInt(5, EquipmentSlot - (-1)); //2.6.10
            buffer.WriteInt(6, EquipmentSlot - (-1)); //2.7.0
            buffer.WriteInt(32, Column);
            buffer.WriteInt(32, Row);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InvLoc:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("OwnerID: 0x" + OwnerID.ToString("X8") + " (" + OwnerID + ")");
            b.Append(' ', pad);
            b.AppendLine("EquipmentSlot: 0x" + EquipmentSlot.ToString("X8") + " (" + EquipmentSlot + ")");
            b.Append(' ', pad);
            b.AppendLine("Column: 0x" + Column.ToString("X8") + " (" + Column + ")");
            b.Append(' ', pad);
            b.AppendLine("Row: 0x" + Row.ToString("X8") + " (" + Row + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
