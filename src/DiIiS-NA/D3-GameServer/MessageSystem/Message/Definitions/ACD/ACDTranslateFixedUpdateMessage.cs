using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateFixedUpdateMessage)]
    public class ACDTranslateFixedUpdateMessage : GameMessage
    {
        public int Field0;
        public Vector3D vPos;
        public Vector3D vVel;

        public ACDTranslateFixedUpdateMessage() : base(Opcodes.ACDTranslateFixedUpdateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            vPos = new Vector3D();
            vPos.Parse(buffer);
            vVel = new Vector3D();
            vVel.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            vPos.Encode(buffer);
            vVel.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateFixedUpdateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            vPos.AsText(b, pad);
            vVel.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
