using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Scene
{
    [Message(Opcodes.RevealSceneMessage)]
    public class RevealSceneMessage : GameMessage
    {
        public uint WorldID;
        public SceneSpecification SceneSpec;
        public uint ChunkID;
        public int /* sno */ SceneSNO;
        public PRTransform Transform;
        public uint ParentChunkID;
        public int /* sno */ SceneGroupSNO;
        // MaxLength = 256
        public int /* gbid */[] arAppliedLabels;
        public int /* sno */[] snoActors;
        public int Vista;

        public RevealSceneMessage() : base(Opcodes.RevealSceneMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
            SceneSpec = new SceneSpecification();
            SceneSpec.Parse(buffer);
            ChunkID = buffer.ReadUInt(32);
            SceneSNO = buffer.ReadInt(32);
            Transform = new PRTransform();
            Transform.Parse(buffer);
            ParentChunkID = buffer.ReadUInt(32);
            SceneGroupSNO = buffer.ReadInt(32);
            arAppliedLabels = new int /* gbid */[buffer.ReadInt(9)];
            for (int i = 0; i < arAppliedLabels.Length; i++) arAppliedLabels[i] = buffer.ReadInt(32);
            snoActors = new int /* sno */[buffer.ReadInt(9)];
            for (int i = 0; i < snoActors.Length; i++) snoActors[i] = buffer.ReadInt(32);
            Vista = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
            SceneSpec.Encode(buffer);
            buffer.WriteUInt(32, ChunkID);
            buffer.WriteInt(32, SceneSNO);
            Transform.Encode(buffer);
            buffer.WriteUInt(32, ParentChunkID);
            buffer.WriteInt(32, SceneGroupSNO);
            buffer.WriteInt(9, arAppliedLabels.Length);
            for (int i = 0; i < arAppliedLabels.Length; i++) buffer.WriteInt(32, arAppliedLabels[i]);
            if(snoActors == null)
                snoActors = new int /* sno */[1];
            buffer.WriteInt(9, snoActors.Length);
            for (int i = 0; i < snoActors.Length; i++) buffer.WriteInt(32, snoActors[i]);
            buffer.WriteInt(32, Vista);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RevealSceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            SceneSpec.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("ChunkID: 0x" + ChunkID.ToString("X8") + " (" + ChunkID + ")");
            b.Append(' ', pad); b.AppendLine("SceneSNO: 0x" + SceneSNO.ToString("X8"));
            Transform.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("ParentChunkID: 0x" + ParentChunkID.ToString("X8") + " (" + ParentChunkID + ")");
            b.Append(' ', pad); b.AppendLine("SceneGroupSNO: 0x" + SceneGroupSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("arAppliedLabels:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < arAppliedLabels.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < arAppliedLabels.Length; j++, i++) { b.Append("0x" + arAppliedLabels[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
