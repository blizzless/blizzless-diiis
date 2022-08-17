//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
    [Message(Opcodes.SavePointDataMessage)]
    public class SavePointDataMessage : GameMessage
    {
        public SavePointData Field0;
        public int Field1;

        public SavePointDataMessage() : base(Opcodes.SavePointDataMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            if (Field0 == null)
                Field0 = new SavePointData();
            Field0.Parse(buffer);
            Field1 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Field0.Encode(buffer);
            buffer.WriteInt(32, Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SavePointDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
