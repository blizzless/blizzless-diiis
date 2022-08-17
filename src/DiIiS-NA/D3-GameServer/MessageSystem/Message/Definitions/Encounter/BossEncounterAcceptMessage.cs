//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
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
    [Message(new[] { Opcodes.BossJoinAcceptMessage, Opcodes.BossStartAcceptMessage }, Consumers.Player)]
    //[Message(new[] {Opcodes.BossEncounterMessage1, Opcodes.BossEncounterMessage2})]
    public class BossEncounterAcceptMessage : GameMessage, ISelfHandler
    {
        public int Field0;
        public int /* sno */ snoEncounter;
        public uint ToWorldID;

        public void Handle(GameClient client)
        {
            client.Player.ArtisanInteraction = "QueueAccepted";
            client.Game.AcceptBossEncounter();
        }

        public override void Parse(GameBitBuffer buffer)
        {
            //Field0 = buffer.ReadInt(32);
            //ToWorldID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            //buffer.WriteInt(32, Field0);
            //buffer.WriteInt(32, snoEncounter);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BossEncounterMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("snoEncounter: 0x" + snoEncounter.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
