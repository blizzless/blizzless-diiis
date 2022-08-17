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
    [Message(Opcodes.CraftingResultsMessage)]
    public class CraftingResultsMessage : GameMessage
    {
        public uint annItem;
        public int /* gbid */ GBIDItem;
        public int IQL;

        public override void Parse(GameBitBuffer buffer)
        {
            annItem = buffer.ReadUInt(32);
            GBIDItem = buffer.ReadInt(32);
            IQL = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, annItem);
            buffer.WriteInt(32, GBIDItem);
            buffer.WriteInt(32, IQL);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CraftingResultsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("annItem: 0x" + annItem.ToString("X8") + " (" + annItem + ")");
            b.Append(' ', pad); b.AppendLine("GBIDItem: 0x" + GBIDItem.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("IQL: 0x" + IQL.ToString("X8") + " (" + IQL + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
