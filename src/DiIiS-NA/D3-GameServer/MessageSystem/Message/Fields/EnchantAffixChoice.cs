//Blizzless Project 2022 
using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class EnchantAffixChoice
    {
        public int /* gbid */ GbId;
        public int Seed;

        public void Parse(GameBitBuffer buffer)
        {
            GbId = buffer.ReadInt(32);
            Seed = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, GbId);
            buffer.WriteInt(32, Seed);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VisualItem:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("GbId: 0x" + GbId.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("Seed: 0x" + Seed.ToString("X8") + " (" + Seed + ")");
            b.Append(' ', pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
