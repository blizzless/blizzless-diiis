using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class ResolvedPortalDestination
    {
        public int /* sno */ WorldSNO;
        public int StartingPointActorTag;       // in the target world is (should be!) a starting point, that is tagged with this id
        public int /* sno */ DestLevelAreaSNO;

        public void Parse(GameBitBuffer buffer)
        {
            WorldSNO = buffer.ReadInt(32);
            StartingPointActorTag = buffer.ReadInt(32);
            DestLevelAreaSNO = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, WorldSNO);
            buffer.WriteInt(32, StartingPointActorTag);
            buffer.WriteInt(32, DestLevelAreaSNO);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ResolvedPortalDestination:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("WorldSNO: 0x" + WorldSNO.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("StartingPointActorTag: 0x" + StartingPointActorTag.ToString("X8") + " (" + StartingPointActorTag + ")");
            b.Append(' ', pad);
            b.AppendLine("DestLevelAreaSNO: 0x" + DestLevelAreaSNO.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
