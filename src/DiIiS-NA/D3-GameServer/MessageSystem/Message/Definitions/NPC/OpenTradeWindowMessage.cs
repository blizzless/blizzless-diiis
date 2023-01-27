using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.NPC
{
    [Message(Opcodes.OpenTradeWindow)]
    public class OpenTradeWindowMessage : GameMessage
    {
        public int ActorID;

        public OpenTradeWindowMessage() { }
        public OpenTradeWindowMessage(int actorID)
            : base(Opcodes.OpenTradeWindow)
        {
            ActorID = actorID;
        }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("OpenTradeWindowMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

    }
}
