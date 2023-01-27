using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.RareMonsterNamesMessage)]
    public class RareMonsterNamesMessage : GameMessage
    {
        public uint ann;
        // MaxLength = 2
        public int /* gbid */[] RareNames;
        // MaxLength = 8
        public int /* gbid */[] MonsterAffixes;

        public RareMonsterNamesMessage() : base(Opcodes.RareMonsterNamesMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ann = buffer.ReadUInt(32);
            RareNames = new int /* gbid */[2];
            for (int i = 0; i < RareNames.Length; i++) RareNames[i] = buffer.ReadInt(32);
            MonsterAffixes = new int /* gbid */[8];
            for (int i = 0; i < MonsterAffixes.Length; i++) MonsterAffixes[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ann);
            for (int i = 0; i < RareNames.Length; i++) buffer.WriteInt(32, RareNames[i]);
            for (int i = 0; i < MonsterAffixes.Length; i++) buffer.WriteInt(32, MonsterAffixes[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RareMonsterNamesMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ann: 0x" + ann.ToString("X8") + " (" + ann + ")");
            b.Append(' ', pad); b.AppendLine("RareNames:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < RareNames.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < RareNames.Length; j++, i++) { b.Append("0x" + RareNames[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("MonsterAffixes:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < MonsterAffixes.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < MonsterAffixes.Length; j++, i++) { b.Append("0x" + MonsterAffixes[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
