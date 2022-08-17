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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.CameraFocusMessage)]
    public class CameraFocusMessage : GameMessage
    {
        public int ActorID;
        public bool Snap;
        public float Duration;
        public CameraFocusMessage() : base(Opcodes.CameraFocusMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadInt(32);
            Snap = buffer.ReadBool();
            Duration = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorID);
            buffer.WriteBool(Snap);
            buffer.WriteFloat32(Duration);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CameraFocusMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Snap: " + (Snap ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Duration: " + Duration.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
