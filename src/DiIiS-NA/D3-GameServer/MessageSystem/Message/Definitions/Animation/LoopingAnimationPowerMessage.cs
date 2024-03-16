using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation
{
    [Message(Opcodes.LoopingAnimationPowerMessage, Consumers.Player)]
    class LoopingAnimationPowerMessage : GameMessage
    {
        public int snoPower;
        public int snoData0;
        public int Data;

        public override void Parse(GameBitBuffer buffer)
        {
            snoPower = buffer.ReadInt(32);
            snoData0 = buffer.ReadInt(32);
            Data = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoPower);
            buffer.WriteInt(32, snoData0);
            buffer.WriteInt(32, Data);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LoopingAnimationPowerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoPower: 0x" + snoPower.ToString("X8") + " (" + snoPower + ")");
            b.Append(' ', pad); b.AppendLine("snoData0: 0x" + snoData0.ToString("X8") + " (" + snoData0 + ")");
            b.Append(' ', pad); b.AppendLine("Data: 0x" + Data.ToString("X8") + " (" + Data + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
