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
    [Message(Opcodes.SavePointInfoMessage)]
    public class SavePointInfoMessage : GameMessage
    {
        public int /* sno */ snoLevelArea;

        public SavePointInfoMessage() : base(Opcodes.SavePointInfoMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoLevelArea = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoLevelArea);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SavePointInfoMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoLevelArea: 0x" + snoLevelArea.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
