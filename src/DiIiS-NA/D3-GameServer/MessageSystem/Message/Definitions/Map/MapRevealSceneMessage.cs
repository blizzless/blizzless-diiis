using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map
{
    [Message(Opcodes.MapRevealSceneMessage)]
    public class MapRevealSceneMessage : GameMessage
    {
        public uint ChunkID;
        public int /* sno */ SceneSNO;
        public PRTransform Transform;
        public uint WorldID;
        public bool MiniMapVisibility;

        public MapRevealSceneMessage() : base(Opcodes.MapRevealSceneMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ChunkID = buffer.ReadUInt(32);
            SceneSNO = buffer.ReadInt(32);
            Transform = new PRTransform();
            Transform.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
            MiniMapVisibility = buffer.ReadBool(); ;
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ChunkID);
            buffer.WriteInt(32, SceneSNO);
            Transform.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
            buffer.WriteBool(MiniMapVisibility);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("MapRevealSceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ChunkID: 0x" + ChunkID.ToString("X8") + " (" + ChunkID + ")");
            b.Append(' ', pad); b.AppendLine("SceneSNO: 0x" + SceneSNO.ToString("X8"));
            Transform.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("MiniMapVisibility: " + (MiniMapVisibility ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
