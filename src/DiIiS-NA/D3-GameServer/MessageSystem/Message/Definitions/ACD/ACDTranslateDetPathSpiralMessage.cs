using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateDetPathSpiralMessage)]
    public class ACDTranslateDetPathSpiralMessage : GameMessage
    {
        public uint DynamicId;
        public int DPath;
        public Vector3D StartPosition;
        public Vector3D TargetPosition;
        public int MoveFlags;
        public int AnimTag;
        public float Var0;
        public float Var1;
        public DPathSinData SinusData;
        public float SpeedMult;

        public ACDTranslateDetPathSpiralMessage() : base(Opcodes.ACDTranslateDetPathSpiralMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            DynamicId = buffer.ReadUInt(32);
            DPath = buffer.ReadInt(4);
            StartPosition = new Vector3D();
            StartPosition.Parse(buffer);
            TargetPosition = new Vector3D();
            TargetPosition.Parse(buffer);
            MoveFlags = buffer.ReadInt(32);
            AnimTag = buffer.ReadInt(32);
            Var0 = buffer.ReadFloat32();
            Var1 = buffer.ReadFloat32();
            SinusData = new DPathSinData();
            SinusData.Parse(buffer);
            SpeedMult = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, DynamicId);
            buffer.WriteInt(4, DPath);
            StartPosition.Encode(buffer);
            TargetPosition.Encode(buffer);
            buffer.WriteInt(32, MoveFlags);
            buffer.WriteInt(32, AnimTag);
            buffer.WriteFloat32(Var0);
            buffer.WriteFloat32(Var1);
            SinusData.Encode(buffer);
            buffer.WriteFloat32(SpeedMult);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateDetPathSpiralMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
