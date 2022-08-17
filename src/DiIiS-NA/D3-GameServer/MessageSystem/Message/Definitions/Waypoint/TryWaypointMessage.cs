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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint
{
    [Message(Opcodes.TryWaypointMessage, Consumers.Player)]
    public class TryWaypointMessage : GameMessage
    {
        public int ActorDyID; // ann Waypoint
        public int nWaypoint; //  n Waypoint

        public TryWaypointMessage()
            : base(Opcodes.TryWaypointMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorDyID = buffer.ReadInt(32);
            nWaypoint = buffer.ReadInt(7);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorDyID);
            buffer.WriteInt(7, nWaypoint);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TryWaypointMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorDyID: 0x" + ActorDyID.ToString("X8") + " (" + ActorDyID + ")");
            b.Append(' ', pad); b.AppendLine("nWaypoint: 0x" + nWaypoint.ToString("X8") + " (" + nWaypoint + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
