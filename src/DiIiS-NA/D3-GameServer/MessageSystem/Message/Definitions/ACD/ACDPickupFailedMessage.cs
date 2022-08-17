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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDPickupFailedMessage)]
    public class ACDPickupFailedMessage : GameMessage
    {
        public enum Reasons : int
        {
            InventoryFull = 0,                  //and 1, 2, 5, 6, 7  <-- ?
            ItemBelongingToSomeoneElse = 3,
            OnlyOneItemAllowed = 4
        }

        public bool IsCurrency;
        public Reasons Reason;
        //public Reasons Reason;
        public uint ItemID; // Item's DynamicID

        public ACDPickupFailedMessage() : base(Opcodes.ACDPickupFailedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            IsCurrency = buffer.ReadBool();
            Reason = (Reasons)buffer.ReadInt(3);
            ItemID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(IsCurrency);
            buffer.WriteInt(3, (int)Reason);
            buffer.WriteUInt(32, ItemID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDPickupFailedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ItemID: 0x" + ItemID.ToString("X8") + " (" + ItemID + ")");
            b.Append(' ', pad); b.AppendLine("Reason: 0x" + ((int)(Reason)).ToString("X8") + " (" + Reason + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
