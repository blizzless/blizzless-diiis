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
    [Message(new[] { Opcodes.MonsterEngagedMessage, Opcodes.EliteKilledMessage })]
    public class CombatEngagementMessage : GameMessage
    {
        public int PlayerIndex;
        public int /* sno */ snoActorMonster;
        public int MonsterRarity;
        // MaxLength = 2
        public int /* gbid */[] RareNameGBIDs;

        public CombatEngagementMessage() : base(Opcodes.MonsterEngagedMessage) { }
        public CombatEngagementMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(4) + (-1);
            snoActorMonster = buffer.ReadInt(32);
            MonsterRarity = buffer.ReadInt(4) + (-1);
            RareNameGBIDs = new int /* gbid */[2];
            for (int i = 0; i < RareNameGBIDs.Length; i++) RareNameGBIDs[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, PlayerIndex - (-1));
            buffer.WriteInt(32, snoActorMonster);
            buffer.WriteInt(4, MonsterRarity - (-1));
            for (int i = 0; i < RareNameGBIDs.Length; i++) buffer.WriteInt(32, RareNameGBIDs[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CombatEngagementMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("snoActorMonster: 0x" + snoActorMonster.ToString("X8") + " (" + snoActorMonster + ")");
            b.Append(' ', pad); b.AppendLine("MonsterRarity: 0x" + MonsterRarity.ToString("X8") + " (" + MonsterRarity + ")");
            b.Append(' ', pad); b.AppendLine("RareNameGBIDs:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < RareNameGBIDs.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < RareNameGBIDs.Length; j++, i++) { b.Append("0x" + RareNameGBIDs[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
