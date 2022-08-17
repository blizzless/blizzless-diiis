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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(new[] {Opcodes.PrefetchWorldMessage, Opcodes.PrefetchSceneMessage, Opcodes.PrefetchLevelAreaMessage })]
    public class PrefetchDataMessage : GameMessage
    {
        public int SNO; // Actor's DynamicID
       
        public PrefetchDataMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PrefetchDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNO: 0x" + SNO.ToString("X8") + " (" + SNO + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
