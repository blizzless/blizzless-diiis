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
    [Message(Opcodes.IdentifyItemMessage, Consumers.Inventory)]
    public class InventoryIdentifyItemMessage : GameMessage
    {
        public uint ItemID; // Item's DynamicID
        //TODO: find out about unknown data
       
        public override void Parse(GameBitBuffer buffer)
        {
            ItemID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ItemID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryIdentifyItemMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ItemID: 0x" + ItemID.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
    [Message(Opcodes.UseIdentifyItemMessage, Consumers.Inventory)]
    public class InventoryUseIdentifyItemMessage : GameMessage
    {
        public uint ItemID; // Item's DynamicID

        public override void Parse(GameBitBuffer buffer)
        {
            ItemID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ItemID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryUseIdentifyItemMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ItemID: 0x" + ItemID.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
