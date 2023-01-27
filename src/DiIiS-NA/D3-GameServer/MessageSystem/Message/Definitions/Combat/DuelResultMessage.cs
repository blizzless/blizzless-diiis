using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.DuelResultMessage)]
    public class DuelResultMessage : GameMessage
    {
        public int PlayerVictimIndex;
        public int PlayerKillerIndex;
        public int /* sno */ snoPowerDmgSource;

        public DuelResultMessage() : base(Opcodes.DuelResultMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerVictimIndex = buffer.ReadInt(3);
            PlayerKillerIndex = buffer.ReadInt(4) + (-1);
            snoPowerDmgSource = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerVictimIndex);
            buffer.WriteInt(4, PlayerKillerIndex - (-1));
            buffer.WriteInt(32, snoPowerDmgSource);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DuelResultMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerVictimIndex: 0x" + PlayerVictimIndex.ToString("X8") + " (" + PlayerVictimIndex + ")");
            b.Append(' ', pad); b.AppendLine("PlayerKillerIndex: 0x" + PlayerKillerIndex.ToString("X8") + " (" + PlayerKillerIndex + ")");
            b.Append(' ', pad); b.AppendLine("snoPowerDmgSource: 0x" + snoPowerDmgSource.ToString("X8") + " (" + snoPowerDmgSource + ")");
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
