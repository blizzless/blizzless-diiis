using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(new[] { Opcodes.EnchantAffixMessage }, Consumers.Inventory)]
    public class EnchantAffixMessage : GameMessage
    {
        public int Field0;
        public int /* gbid */ GBIDAffixToReroll;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            GBIDAffixToReroll = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, GBIDAffixToReroll);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EnchantAffixMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("GBIDAffixToReroll: 0x" + GBIDAffixToReroll.ToString("X8") + " (" + GBIDAffixToReroll + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
