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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(new[] {
        Opcodes.OpenWorldTrackedSNOMessage

        , Opcodes.TimedEventFinishedMessage
        , Opcodes.TimedEventResetMessage
        //, Opcodes.SNODataMessage4
        , Opcodes.GameEndRequestMessageg
        , Opcodes.ActTransitionRequestMessagge
        , Opcodes.ChallengeStartMessage
        , Opcodes.ChallengeStartedMessage
        , Opcodes.ChallengeAcceptMessage
        , Opcodes.SummonVanityPet
        , Opcodes.DungeonFinderSetTimedEvent
    })]
    public class SNODataMessage : GameMessage
    {
        public int /* sno */ Field0;
        public SNODataMessage(Opcodes opcode) : base(opcode) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SNODataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
