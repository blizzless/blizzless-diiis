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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter
{
    [Message(new[] { Opcodes.BossJoinEncounterMessage, Opcodes.BossStartEncounterMessage }, Consumers.Player)]
    public class BossEncounterMessage : GameMessage
    {
        public int PlayerIndex;
        public int /* sno */ snoEncounter;
        public BossEncounterMessage(Opcodes id) : base(id)
        { 
        }
        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(32);
            snoEncounter = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PlayerIndex);
            buffer.WriteInt(32, snoEncounter);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BossEncounterMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("snoEncounter: 0x" + snoEncounter.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
