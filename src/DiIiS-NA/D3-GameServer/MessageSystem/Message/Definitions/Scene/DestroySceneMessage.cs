using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Scene
{
    [Message(Opcodes.DestroySceneMessage)]
    public class DestroySceneMessage : GameMessage
    {
        public uint WorldID;
        public uint SceneID;

        public DestroySceneMessage() : base(Opcodes.DestroySceneMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
            SceneID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
            buffer.WriteUInt(32, SceneID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DestroySceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("SceneID: 0x" + SceneID.ToString("X8") + " (" + SceneID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
