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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    //[Message(new[] { Opcodes.TransmogItemMessage, Opcodes.TransmogItemMessage1 })]
    public class TransmogItemMessage : GameMessage
    {
        public int annItem;
        public int /* gbid */ GBIDTransmog;

        public override void Parse(GameBitBuffer buffer)
        {
            annItem = buffer.ReadInt(32);
            GBIDTransmog = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, annItem);
            buffer.WriteInt(32, GBIDTransmog);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TransmogItemMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annItem: 0x" + annItem.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("GBIDTransmog: 0x" + GBIDTransmog.ToString("X8") + " (" + GBIDTransmog + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
