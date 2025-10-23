using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] { Opcodes.PlayerSetCameraObserverMessage })]
    public class PlayerSetCameraObserverMessage : GameMessage
    {
        public int Field0;
        public WorldPlace Field1;

        public PlayerSetCameraObserverMessage() : base(Opcodes.PlayerSetCameraObserverMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = new WorldPlace();
            if (buffer.ReadBool())
            {
                Field1.Parse(buffer);
            }
            
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteBool(Field1 != null);
            Field1.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSetCameraObserverMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: \"" + Field0 + "\"");
            //Field1.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
