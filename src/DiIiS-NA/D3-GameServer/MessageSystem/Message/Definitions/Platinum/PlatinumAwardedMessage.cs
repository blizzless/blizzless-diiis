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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum
{
    [Message(Opcodes.PlatinumAwardedMessage)]
    public class PlatinumAwardedMessage : GameMessage
    {
        public long CurrentPlatinum;
        public long PlatinumIncrement;

        public PlatinumAwardedMessage() : base(Opcodes.PlatinumAwardedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            CurrentPlatinum = buffer.ReadInt64(64);
            PlatinumIncrement = buffer.ReadInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt64(64, CurrentPlatinum);
            buffer.WriteInt64(64, PlatinumIncrement);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlatinumAwardedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("CurrentPlatinum: 0x" + CurrentPlatinum.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("PlatinumIncrement: 0x" + PlatinumIncrement.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
