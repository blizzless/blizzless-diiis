using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] { Opcodes.PlayerLoadoutTabIconMessage, Opcodes.PlayerLoadoutTabIconMessage1 })]
    public class PlayerLoadoutTabIconMessage : GameMessage
    {
        public int Field0;
        public int TabIcon;
        public PlayerLoadoutTabIconMessage(Opcodes opcode) : base(opcode) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            TabIcon = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, TabIcon);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerLoadoutTabIconMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad++);
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad++);
            b.Append(' ', pad); b.AppendLine("TabIcon: 0x" + TabIcon.ToString("X8") + " (" + TabIcon + ")");
            b.Append(' ', pad++);
            b.AppendLine("}");
            //throw new NotImplementedException();
        }
    }
}
