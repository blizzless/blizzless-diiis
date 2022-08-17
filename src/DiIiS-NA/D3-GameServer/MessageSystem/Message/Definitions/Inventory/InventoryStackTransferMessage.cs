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
    [Message(Opcodes.InventoryStackTransferMessage, Consumers.Inventory)]
    public class InventoryStackTransferMessage : GameMessage
    {
        public uint FromID;
        public uint ToID;
        public ulong Amount;

        public override void Parse(GameBitBuffer buffer)
        {
            FromID = buffer.ReadUInt(32);
            ToID = buffer.ReadUInt(32);
            Amount = buffer.ReadUInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, FromID);
            buffer.WriteUInt(32, ToID);
            buffer.WriteUInt64(64, Amount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventoryStackTransferMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FromID: 0x" + FromID.ToString("X8") + " (" + FromID + ")");
            b.Append(' ', pad); b.AppendLine("ToID: 0x" + ToID.ToString("X8") + " (" + ToID + ")");
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
