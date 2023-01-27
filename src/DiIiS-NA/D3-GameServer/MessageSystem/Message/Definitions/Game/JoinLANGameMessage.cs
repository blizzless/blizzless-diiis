using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.JoinLANGameMessage)]
    public class JoinLANGameMessage : GameMessage
    {
        public int idGame;
        public string Account;
        public string Hero;
        public int Locale;

        public JoinLANGameMessage() : base(Opcodes.JoinLANGameMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            idGame = buffer.ReadInt(32);
            Account = buffer.ReadCharArray(128);
            Hero = buffer.ReadCharArray(49);
            Locale = buffer.ReadInt(5) + (2);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, idGame);
            buffer.WriteCharArray(128, Account);
            buffer.WriteCharArray(49, Hero);
            buffer.WriteInt(5, Locale - (2));
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("JoinLANGameMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("idGame: 0x" + idGame.ToString("X8") + " (" + idGame + ")");
            b.Append(' ', pad); b.AppendLine("Account: \"" + Account + "\"");
            b.Append(' ', pad); b.AppendLine("Hero: \"" + Hero + "\"");
            b.Append(' ', pad); b.AppendLine("Locale: 0x" + Locale.ToString("X8") + " (" + Locale + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
