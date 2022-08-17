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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    
    [Message(new[] { Opcodes.LootRunAppearedMessage })]
    public class LootRunAppearedMessage : GameMessage
    {
        public uint snoLevelArea; // Actor's DynamicID

        public LootRunAppearedMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoLevelArea = buffer.ReadUInt(8);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, snoLevelArea);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LootRunAppearedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoLevelArea: 0x" + snoLevelArea.ToString("X8") + " (" + snoLevelArea + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
