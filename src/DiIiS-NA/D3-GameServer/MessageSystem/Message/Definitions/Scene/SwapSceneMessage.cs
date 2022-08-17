//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Scene
{
    [Message(Opcodes.SwapSceneMessage)]
    public class SwapSceneMessage : GameMessage
    {
        public int Field0;
        public int idSceneNew;
        public int idSceneOld;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            idSceneNew = buffer.ReadInt(32);
            idSceneOld = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, idSceneNew);
            buffer.WriteInt(32, idSceneOld);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SwapSceneMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("idSceneNew: 0x" + idSceneNew.ToString("X8") + " (" + idSceneNew + ")");
            b.Append(' ', pad); b.AppendLine("idSceneOld: 0x" + idSceneOld.ToString("X8") + " (" + idSceneOld + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
