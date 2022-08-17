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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling
{
    [Message(Opcodes.HirelingDeadMessage)]
    public class HirelingInfoUpdateMessage : GameMessage
    {
        public int HirelingIndex;
        public bool Dead;
        public int GbidName;
        public int ValidClass;

        public HirelingInfoUpdateMessage()
            : base(Opcodes.HirelingDeadMessage)
        {
        }

        public override void Parse(GameBitBuffer buffer)
        {
            HirelingIndex = buffer.ReadInt(2);
            Dead = buffer.ReadBool();
            GbidName = buffer.ReadInt(32);
            ValidClass = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, HirelingIndex);
            buffer.WriteBool(Dead);
            buffer.WriteInt(32, GbidName);
            buffer.WriteInt(32, ValidClass);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingInfoUpdateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HirelingIndex: 0x" + HirelingIndex.ToString("X8") + " (" + HirelingIndex + ")");
            b.Append(' ', pad); b.AppendLine("Dead: " + (Dead ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("GbidName: 0x" + GbidName.ToString("X8") + " (" + GbidName + ")");
            b.Append(' ', pad); b.AppendLine("ValidClass: 0x" + ValidClass.ToString("X8") + " (" + ValidClass + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
