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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.CurrencyCounterUpdateMessage)]
    public class CurrencyCounterUpdateMessage : GameMessage
    {
        public int AmountCollected;
        public int CurrencyType;
        public bool Expired;
        public CurrencyCounterUpdateMessage() : base(Opcodes.CurrencyCounterUpdateMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            AmountCollected = buffer.ReadInt(32);
            CurrencyType = buffer.ReadInt(32);
            Expired = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, AmountCollected);
            buffer.WriteInt(32, CurrencyType);
            buffer.WriteBool(Expired);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CurrencyCounterUpdateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("AmountCollected: 0x" + AmountCollected.ToString("X8") + " (" + AmountCollected + ")");
            b.Append(' ', pad); b.AppendLine("CurrencyType: 0x" + CurrencyType.ToString("X8") + " (" + CurrencyType + ")");
            //b.Append(' ', pad); b.AppendLine("Field2: 0x" + " (" + Field2 + ")");
            //b.Append(' ', pad); b.AppendLine("Field3:");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
