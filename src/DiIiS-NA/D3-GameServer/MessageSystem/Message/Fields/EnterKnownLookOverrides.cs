using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class EnterKnownLookOverrides
    {
        // MaxLength = 6
        public int /* sno */[] Field0;
     

        public void Parse(GameBitBuffer buffer)
        {
            Field0 = new int /* sno */[6];
            for (int i = 0; i < Field0.Length; i++) Field0[i] = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < Field0.Length; i++) buffer.WriteInt(32, Field0[i]);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0:");
            b.Append(' ', pad);
            b.AppendLine("{");
            
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
