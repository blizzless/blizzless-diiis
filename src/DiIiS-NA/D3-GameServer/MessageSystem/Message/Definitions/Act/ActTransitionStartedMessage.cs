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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act
{
    [Message(Opcodes.ActTransitionStartedMessage)]
    public class ActTransitionStartedMessage : GameMessage
    {
        public int ActTransitionTime;
        public int RecreateGameWithParty;
        public int PlayerIndex;

        public ActTransitionStartedMessage() : base(Opcodes.ActTransitionStartedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActTransitionTime = buffer.ReadInt(32);
            RecreateGameWithParty = buffer.ReadInt(32);
            PlayerIndex = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActTransitionTime);
            buffer.WriteInt(32, RecreateGameWithParty);
            buffer.WriteInt(32, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ActTransitionStartedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActTransitionTime: 0x" + ActTransitionTime.ToString("X8") + " (" + ActTransitionTime + ")");
            b.Append(' ', pad); b.AppendLine("RecreateGameWithParty: 0x" + RecreateGameWithParty.ToString("X8") + " (" + RecreateGameWithParty + ")");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
