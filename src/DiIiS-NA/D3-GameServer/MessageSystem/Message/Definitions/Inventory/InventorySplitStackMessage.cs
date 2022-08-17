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
    [Message(Opcodes.InventorySplitStackMessage, Consumers.Inventory)]
    public class InventorySplitStackMessage : GameMessage
    {
        public int FromID;
        public long Amount;
        public InvLoc InvLoc;

        public override void Parse(GameBitBuffer buffer)
        {
            FromID = buffer.ReadInt(32);
            Amount = buffer.ReadInt64(64);
            InvLoc = new InvLoc();
            InvLoc.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, FromID);
            buffer.WriteInt64(64, Amount);
            InvLoc.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("InventorySplitStackMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FromID: 0x" + FromID.ToString("X8") + " (" + FromID + ")");
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X16"));
            InvLoc.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
