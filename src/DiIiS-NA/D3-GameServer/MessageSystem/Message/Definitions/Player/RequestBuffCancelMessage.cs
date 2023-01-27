using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.RequestBuffCancelMessage, Consumers.Player)]
    public class RequestBuffCancelMessage : GameMessage
    {
        public int /* sno */ PowerSNOId; // SNO of the power that activated the buff to be canceled
        public int BuffIndex; // Might be ActorID, might be number of stacks to clear off?

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNOId = buffer.ReadInt(32);
            BuffIndex = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNOId);
            buffer.WriteInt(32, BuffIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RequestBuffCancelMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNOId: 0x" + PowerSNOId.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("BuffIndex: 0x" + BuffIndex.ToString("X8") + " (" + BuffIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
