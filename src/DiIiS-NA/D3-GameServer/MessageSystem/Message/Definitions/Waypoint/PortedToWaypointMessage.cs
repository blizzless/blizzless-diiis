using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint
{
    [Message(Opcodes.PortedToWaypointMessage)]
    public class PortedToWaypointMessage : GameMessage
    {
        public int PlayerIndex;
        public int LevelAreaSNO;

        public PortedToWaypointMessage() : base(Opcodes.PortedToWaypointMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(4) + (-1);
            LevelAreaSNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, PlayerIndex - (-1));
            buffer.WriteInt(32, LevelAreaSNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PortedToWaypointMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
