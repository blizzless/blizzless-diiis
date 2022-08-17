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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal
{
    [Message(Opcodes.HearthPortalInfoMessage)]
    public class HearthPortalInfoMessage : GameMessage
    {
        public int /* sno */ snoLevelArea;
        public int /* sno */ snoUnknown;
        public int Field1;
        public bool Field2;
        public bool Field3;

        public HearthPortalInfoMessage() : base(Opcodes.HearthPortalInfoMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoLevelArea = buffer.ReadInt(32);
            snoUnknown = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = buffer.ReadBool();
            Field3 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoLevelArea);
            buffer.WriteInt(32, snoUnknown);
            buffer.WriteInt(32, Field1);
            buffer.WriteBool(Field2);
            buffer.WriteBool(Field3);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HearthPortalInfoMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoLevelArea: 0x" + snoLevelArea.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
