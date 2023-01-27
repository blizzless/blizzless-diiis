using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class AnimPreplayData
    {
        public int ServerTimeAnimStarted;
        public int SynceedSeed; 
        public int Field2;

        public void Parse(GameBitBuffer buffer)
        {
            ServerTimeAnimStarted = buffer.ReadInt(32);
            SynceedSeed = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ServerTimeAnimStarted);
            buffer.WriteInt(32, SynceedSeed);
            buffer.WriteInt(32, Field2);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AnimPreplayData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("ServerTimeAnimStarted: 0x" + ServerTimeAnimStarted.ToString("X8") + " (" + ServerTimeAnimStarted + ")");
            b.Append(' ', pad);
            b.AppendLine("SynceedSeed: 0x" + SynceedSeed.ToString("X8") + " (" + SynceedSeed + ")");
            b.Append(' ', pad);
            b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
