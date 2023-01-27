using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
    [Message(Opcodes.GoldTransferMessage, Consumers.Inventory)]
    public class GoldTransferMessage : GameMessage
    {
        public uint annPlayer;
        public ulong Amount;

        public override void Parse(GameBitBuffer buffer)
        {
            annPlayer = buffer.ReadUInt(32);
            Amount = buffer.ReadUInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, annPlayer);
            buffer.WriteUInt64(64, Amount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GoldTransferMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annPlayer: 0x" + annPlayer.ToString("X8") + " (" + annPlayer + ")");
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
