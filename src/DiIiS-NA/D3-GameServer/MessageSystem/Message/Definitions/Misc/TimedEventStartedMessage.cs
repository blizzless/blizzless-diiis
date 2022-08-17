//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
    [Message(new[] {Opcodes.TimedEventStartedMessage, Opcodes.TimedEventModifiedMessage})]
    public class TimedEventStartedMessage : GameMessage
    {
        public ActiveEvent Event;
        public TimedEventStartedMessage() : base(Opcodes.TimedEventStartedMessage) { }
        public TimedEventStartedMessage(Opcodes opcode) : base(opcode) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Event = new ActiveEvent();
            Event.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Event.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TimedEventStartedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Event.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
