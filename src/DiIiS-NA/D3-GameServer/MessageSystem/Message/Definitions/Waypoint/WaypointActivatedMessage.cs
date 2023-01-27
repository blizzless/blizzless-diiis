using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint
{
    [Message(Opcodes.WaypointActivatedMessage)]
    public class WaypointActivatedMessage : GameMessage
    {
        public uint WaypointDyID; //ann Waypoint
        public uint PlayerDyID; //ann Activating Player
        public int /* sno */ SNOLevelArea; // Sno Level Area
        public bool Announce; // Bool Announce

        public override void Parse(GameBitBuffer buffer)
        {
            WaypointDyID = buffer.ReadUInt(32);
            PlayerDyID = buffer.ReadUInt(32);
            SNOLevelArea = buffer.ReadInt(32);
            Announce = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WaypointDyID);
            buffer.WriteUInt(32, PlayerDyID);
            buffer.WriteInt(32, SNOLevelArea);
            buffer.WriteBool(Announce);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WaypointActivatedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WaypointDyID: 0x" + WaypointDyID.ToString("X8") + " (" + WaypointDyID + ")");
            b.Append(' ', pad); b.AppendLine("PlayerDyID: 0x" + PlayerDyID.ToString("X8") + " (" + PlayerDyID + ")");
            b.Append(' ', pad); b.AppendLine("SNOLevelArea: 0x" + SNOLevelArea.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Announce: " + (Announce ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
