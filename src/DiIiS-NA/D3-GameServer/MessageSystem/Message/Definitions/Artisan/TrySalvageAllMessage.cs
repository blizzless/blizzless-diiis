using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(new[] { Opcodes.TrySalvageAllMessage }, Consumers.Inventory)]
    public class TrySalvageAllMessage : GameMessage
    {
        public int SalvageType;

        public override void Parse(GameBitBuffer buffer)
        {
            SalvageType = buffer.ReadInt(2);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, SalvageType);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TrySalvageAllMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SalvageType: 0x" + SalvageType.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
