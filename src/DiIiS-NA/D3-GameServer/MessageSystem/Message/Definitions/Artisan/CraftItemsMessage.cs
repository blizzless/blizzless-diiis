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
    [Message(Opcodes.CraftItemsMessage, Consumers.Inventory)]
    public class CraftItemsMessage : GameMessage
    {
        public int /* gbid */ GBIDRecipe;
        public int Count;

        public override void Parse(GameBitBuffer buffer)
        {
            GBIDRecipe = buffer.ReadInt(32);
            Count = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, GBIDRecipe);
            buffer.WriteInt(32, Count);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CraftItemsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("GBIDRecipe: 0x" + GBIDRecipe.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Count: 0x" + Count.ToString("X8") + " (" + Count + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
