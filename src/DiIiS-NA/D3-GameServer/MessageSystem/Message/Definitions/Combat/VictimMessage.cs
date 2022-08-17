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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.VictimMessage)]
    public class VictimMessage : GameMessage
    {
        public int PlayerVictimIndex;
        public int KillerLevel;
        public int KillerPlayerIndex;
        public int KillerMonsterRarity;
        public int /* sno */ snoKillerActor;
        public int KillerTeam;
        // MaxLength = 2
        public int /* gbid */[] KillerRareNameGBIDs;
        public int /* sno */ snoPowerDmgSource;

        public VictimMessage() : base(Opcodes.VictimMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerVictimIndex = buffer.ReadInt(3);
            KillerLevel = buffer.ReadInt(7);
            KillerPlayerIndex = buffer.ReadInt(4) + (-1);
            KillerMonsterRarity = buffer.ReadInt(4) + (-1);
            snoKillerActor = buffer.ReadInt(32);
            KillerTeam = buffer.ReadInt(5) + (-1);
            KillerRareNameGBIDs = new int /* gbid */[2];
            for (int i = 0; i < KillerRareNameGBIDs.Length; i++) KillerRareNameGBIDs[i] = buffer.ReadInt(32);
            snoPowerDmgSource = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerVictimIndex);
            buffer.WriteInt(7, KillerLevel);
            buffer.WriteInt(4, KillerPlayerIndex - (-1));
            buffer.WriteInt(4, KillerMonsterRarity - (-1));
            buffer.WriteInt(32, snoKillerActor);
            buffer.WriteInt(5, KillerTeam - (-1));
            for (int i = 0; i < KillerRareNameGBIDs.Length; i++) buffer.WriteInt(32, KillerRareNameGBIDs[i]);
            buffer.WriteInt(32, snoPowerDmgSource);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VictimMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerVictimIndex: 0x" + PlayerVictimIndex.ToString("X8") + " (" + PlayerVictimIndex + ")");
            b.Append(' ', pad); b.AppendLine("KillerLevel: 0x" + KillerLevel.ToString("X8") + " (" + KillerLevel + ")");
            b.Append(' ', pad); b.AppendLine("KillerPlayerIndex: 0x" + KillerPlayerIndex.ToString("X8") + " (" + KillerPlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("KillerMonsterRarity: 0x" + KillerMonsterRarity.ToString("X8") + " (" + KillerMonsterRarity + ")");
            b.Append(' ', pad); b.AppendLine("snoKillerActor: 0x" + snoKillerActor.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("KillerTeam: 0x" + KillerTeam.ToString("X8") + " (" + KillerTeam + ")");
            b.Append(' ', pad); b.AppendLine("KillerRareNameGBIDs:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < KillerRareNameGBIDs.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < KillerRareNameGBIDs.Length; j++, i++) { b.Append("0x" + KillerRareNameGBIDs[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("snoPowerDmgSource: 0x" + snoPowerDmgSource.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
