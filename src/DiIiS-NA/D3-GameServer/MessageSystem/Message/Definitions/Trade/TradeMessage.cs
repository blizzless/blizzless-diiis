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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Trade
{
    [Message(new[] { Opcodes.ClientTradeMessage, Opcodes.ServerTradeMessage })]
    public class TradeMessage : GameMessage
    {
        public int MessageType;
        public int Data;
        public int Data2;
        public long Data64;
        public int ActorID;
        // MaxLength = 5
        public int[] arrItems;

        public override void Parse(GameBitBuffer buffer)
        {
            MessageType = buffer.ReadInt(4);
            Data = buffer.ReadInt(32);
            Data2 = buffer.ReadInt(32);
            Data64 = buffer.ReadInt64(64);
            ActorID = buffer.ReadInt(32);
            arrItems = new int[5];
            for (int i = 0; i < arrItems.Length; i++) arrItems[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, MessageType);
            buffer.WriteInt(32, Data);
            buffer.WriteInt(32, Data2);
            buffer.WriteInt64(64, Data64);
            buffer.WriteInt(32, ActorID);
            for (int i = 0; i < arrItems.Length; i++) buffer.WriteInt(32, arrItems[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TradeMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("MessageType: 0x" + MessageType.ToString("X8") + " (" + MessageType + ")");
            b.Append(' ', pad); b.AppendLine("Data: 0x" + Data.ToString("X8") + " (" + Data + ")");
            b.Append(' ', pad); b.AppendLine("Data2: 0x" + Data2.ToString("X8") + " (" + Data2 + ")");
            b.Append(' ', pad); b.AppendLine("Data64: 0x" + Data64.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("arrItems:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < arrItems.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < arrItems.Length; j++, i++) { b.Append("0x" + arrItems[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
