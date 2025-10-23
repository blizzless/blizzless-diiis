using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDInventoryPositionMessage)]
    public class ACDInventoryPositionMessage : GameMessage
    {
        public uint ItemId;                                    
        public InventoryLocationMessageData InventoryLocation; 
        public int LocType;                                    

        public ACDInventoryPositionMessage()
            : base(Opcodes.ACDInventoryPositionMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ItemId = buffer.ReadUInt(32);
            InventoryLocation = new InventoryLocationMessageData();
            InventoryLocation.Parse(buffer);
            LocType = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ItemId);
            if (InventoryLocation != null)
            {
                InventoryLocation.Encode(buffer);
            }
            buffer.WriteInt(32, LocType);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDInventoryPositionMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ItemId: 0x" + ItemId.ToString("X8") + " (" + ItemId + ")");
            if (InventoryLocation != null)
            {
                InventoryLocation.AsText(b, pad);
            }
            b.Append(' ', pad); b.AppendLine("LocType: 0x" + LocType.ToString("X8") + " (" + LocType + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
