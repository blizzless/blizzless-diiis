//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.AffixMessage)]
    public class AffixMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID
        public int Field1; // 1 == Identified Affixes, 2 == Unidentified Affixes
        // MaxLength = 32
        public int /* gbid */[] aAffixGBIDs;

        public AffixMessage() : base(Opcodes.AffixMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Field1 = buffer.ReadInt(2);
            aAffixGBIDs = new int /* gbid */[buffer.ReadInt(6)];
            for (int i = 0; i < aAffixGBIDs.Length; i++) aAffixGBIDs[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(2, Field1);
            buffer.WriteInt(6, aAffixGBIDs.Length);
            for (int i = 0; i < aAffixGBIDs.Length; i++) buffer.WriteInt(32, aAffixGBIDs[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AffixMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad); b.AppendLine("aAffixGBIDs:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < aAffixGBIDs.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < aAffixGBIDs.Length; j++, i++) { b.Append("0x" + aAffixGBIDs[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
