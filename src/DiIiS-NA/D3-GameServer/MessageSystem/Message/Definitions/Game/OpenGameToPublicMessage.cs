using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(new[] { Opcodes.OpenGameToPublicMessage, Opcodes.OpenedGameToPublicMessage }//, Consumers.Game 
        )]
    public class OpenGameToPublicMessage : GameMessage
    {
        public string Field0;
        public bool Field1;

        public OpenGameToPublicMessage() : base(Opcodes.OpenGameToPublicMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadCharArray(16);
            Field1 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteCharArray(16, Field0);
            buffer.WriteBool(Field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("OpenGameToPublicMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            //b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
