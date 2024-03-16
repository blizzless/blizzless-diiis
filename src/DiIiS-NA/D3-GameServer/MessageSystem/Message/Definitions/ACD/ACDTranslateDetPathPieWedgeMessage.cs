using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateDetPathPieWedgeMessage)]
    public class ACDTranslateDetPathPieWedgeMessage : GameMessage
    {
        public int ann;
        public Vector3D StartPos;
        public Vector3D FirstTagetPos;
        public int MoveFlags;
        public int AnimTag;
        public DPathPieData PieData;
        public float Field6;

        public ACDTranslateDetPathPieWedgeMessage() : base(Opcodes.ACDTranslateDetPathPieWedgeMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ann = buffer.ReadInt(32);
            StartPos = new Vector3D();
            StartPos.Parse(buffer);
            FirstTagetPos = new Vector3D();
            FirstTagetPos.Parse(buffer);
            MoveFlags = buffer.ReadInt(32);
            AnimTag = buffer.ReadInt(32);
            PieData = new DPathPieData();
            PieData.Parse(buffer);
            Field6 = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ann);
            StartPos.Encode(buffer);
            FirstTagetPos.Encode(buffer);
            buffer.WriteInt(32, MoveFlags);
            buffer.WriteInt(32, AnimTag);
            PieData.Encode(buffer);
            buffer.WriteFloat32(Field6);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //b.Append(' ', pad);
            //b.AppendLine("ACDTranslateDetPathPieWedgeMessage:");
            //b.Append(' ', pad++);
            //b.AppendLine("{");
            //b.Append(' ', pad); b.AppendLine("ann: 0x" + ann.ToString("X8"));
            //StartPos.AsText(b, pad);
            //FirstTagetPos.AsText(b, pad);
            //b.Append(' ', pad); b.AppendLine("MoveFlags: 0x" + MoveFlags.ToString("X8") + " (" + MoveFlags + ")");
            //b.Append(' ', pad); b.AppendLine("AnimTag: 0x" + AnimTag.ToString("X8") + " (" + AnimTag + ")");
            //PieData.AsText(b, pad);
            //b.Append(' ', pad); b.AppendLine("Field6: 0x" + Field6.ToString("X8") + " (" + Field6 + ")");
            //b.Append(' ', --pad);
            //b.AppendLine("}");
        }


    }
}
