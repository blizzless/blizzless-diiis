using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.KillCountMessage)]
    public class KillCountMessage : GameMessage
    {
        public int PlayerIndex;
        public int PlayerKills;
        public int Deaths;
        public int Assists;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(3);
            PlayerKills = buffer.ReadInt(32);
            Deaths = buffer.ReadInt(32);
            Assists = buffer.ReadInt(32);
        }

        public KillCountMessage() : base(Opcodes.KillCountMessage) { }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerIndex);
            buffer.WriteInt(32, PlayerKills);
            buffer.WriteInt(32, Deaths);
            buffer.WriteInt(32, Assists);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("KillCountMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("PlayerKills: 0x" + PlayerKills.ToString("X8") + " (" + PlayerKills + ")");
            b.Append(' ', pad); b.AppendLine("Deaths: 0x" + Deaths.ToString("X8") + " (" + Deaths + ")");
            b.Append(' ', pad); b.AppendLine("Assists: 0x" + Assists.ToString("X8") + " (" + Assists + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
