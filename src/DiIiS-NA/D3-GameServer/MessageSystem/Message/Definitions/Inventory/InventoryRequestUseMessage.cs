//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
    [Message(Opcodes.InventoryRequestUseMessage, Consumers.Inventory)]
    public class InventoryRequestUseMessage : GameMessage
    {
        public uint UsedItem;
        public int Type;
        public uint UsedOnItem;
        public WorldPlace Location;

        public override void Parse(GameBitBuffer buffer)
        {
            UsedItem = buffer.ReadUInt(32);
            Type = buffer.ReadInt(3) + (-1);
            UsedOnItem = buffer.ReadUInt(32);
            Location = new WorldPlace();
            Location.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, UsedItem);
            buffer.WriteInt(3, Type - (-1));
            buffer.WriteUInt(32, UsedOnItem);
            Location.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryRequestUseMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("UsedItem: 0x" + UsedItem.ToString("X8") + " (" + UsedItem + ")");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("UsedOnItem: 0x" + UsedOnItem.ToString("X8") + " (" + UsedOnItem + ")");
            Location.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
