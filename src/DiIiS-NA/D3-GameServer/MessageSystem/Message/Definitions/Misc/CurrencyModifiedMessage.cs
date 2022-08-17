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
    [Message(Opcodes.CurrencyModifiedMessage)]
    public class CurrencyModifiedMessage : GameMessage
    {
        public bool PlaySound;
        public int CurrencyType;
        public int Reason;

        public CurrencyModifiedMessage() : base(Opcodes.SavePointInfoMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlaySound = buffer.ReadBool();
            CurrencyType = buffer.ReadInt(5);
            Reason = buffer.ReadInt(5);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(PlaySound);
            buffer.WriteInt(5, CurrencyType);
            buffer.WriteInt(5, Reason);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SavePointInfoMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.Append(' ', pad); b.AppendLine("PlaySound: 0x" + PlaySound.ToString());
            b.Append(' ', pad);
            b.Append(' ', pad); b.AppendLine("CurrencyType: 0x" + CurrencyType.ToString("X8"));
            b.Append(' ', pad);
            b.Append(' ', pad); b.AppendLine("Reason: 0x" + Reason.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
