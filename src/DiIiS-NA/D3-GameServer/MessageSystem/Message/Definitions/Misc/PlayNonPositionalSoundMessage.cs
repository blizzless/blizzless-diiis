using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(new[] { Opcodes.PlayNonPositionalSoundMessage })]
    public class PlayNonPositionalSoundMessage : GameMessage
    {
        public int SNO; 

        public PlayNonPositionalSoundMessage(Opcodes id) : base(id) { }

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
            b.AppendLine("PlayNonPositionalSoundMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNO: 0x" + SNO.ToString("X8") + " (" + SNO + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
