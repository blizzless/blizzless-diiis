using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    //[Message(Opcodes.RequestJoinBNetGameMessage)]
    public class RequestJoinBNetGameMessage : GameMessage
    {
        public GameId Field0;
        public EntityId Field1;
        public int _Field2;
        public int Field2 { get { return _Field2; } set { if (value < -1 || value > 22) throw new ArgumentOutOfRangeException(); _Field2 = value; } }

        
        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = new GameId();
            Field0.Parse(buffer);
            Field1 = new EntityId();
            Field1.Parse(buffer);
            Field2 = buffer.ReadInt(5) + (-1);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Field0.Encode(buffer);
            Field1.Encode(buffer);
            buffer.WriteInt(5, Field2 - (-1));
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RequestJoinBNetGameMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Field0.AsText(b, pad);
            Field1.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
