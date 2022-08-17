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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection
{
    [Message(Opcodes.LogoutTickTimeMessage)]
    public class LogoutTickTimeMessage : GameMessage
    {
        public bool Field0;
        public int Ticks;
        public int Field2;
        public int Field3;

        public LogoutTickTimeMessage() : base(Opcodes.LogoutTickTimeMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
            Ticks = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
            Field3 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
            buffer.WriteInt(32, Ticks);
            buffer.WriteInt(32, Field2);
            buffer.WriteInt(32, Field3);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LogoutTickTimeMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Ticks: 0x" + Ticks.ToString("X8") + " (" + Ticks + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
