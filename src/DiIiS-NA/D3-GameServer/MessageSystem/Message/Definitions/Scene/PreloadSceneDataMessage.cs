using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Scene
{
    [Message(new[] { Opcodes.PreloadAddSceneMessage, Opcodes.PreloadRemoveSceneMessage })]
    public class PreloadSceneDataMessage : GameMessage
    {
        public uint idSScene;
        public int SNOScene;
        public int[] SnoLevelAreas;

        public PreloadSceneDataMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            idSScene = buffer.ReadUInt(32);
            SNOScene = buffer.ReadInt(32);
            SnoLevelAreas = new int[4];
            for (int i = 0; i < SnoLevelAreas.Length; i++)
                SnoLevelAreas[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, idSScene);
            buffer.WriteInt(32, SNOScene);
            for (int i = 0; i < SnoLevelAreas.Length; i++)
                buffer.WriteInt(32, SnoLevelAreas[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DestroySceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("idSScene: 0x" + idSScene.ToString("X8") + " (" + idSScene + ")");
            b.Append(' ', pad); b.AppendLine("SNOScene: 0x" + SNOScene.ToString("X8") + " (" + SNOScene + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
