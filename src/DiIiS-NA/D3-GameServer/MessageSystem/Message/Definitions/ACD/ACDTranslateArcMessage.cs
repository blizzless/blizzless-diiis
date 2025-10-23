using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateArcMessage)]
    public class ACDTranslateArcMessage : GameMessage
    {
        public int ActorId;                 // DynamicID of the Actor to be moved
        public Vector3D Start;              // Starting position of the movement
        public Vector3D Velocity;           // Velocity vector, added to actor position every tick
        public int Field3;                  // Some sort of move enum, varies depending on type of movement
        public int FlyingAnimationTagID;    // TagID of the flying animation or -1
        public int LandingAnimationTagID;   // TagID of the landing animation or -1
        public float Gravity;               // Gravity, added to Velocity.Z every tick, so it needs to be <0. Smaller values effectively speed up the translate.
        public int /* sno */ PowerSNO;      // PowerSNO, usually whatever skill is activating the translate
        public float Bounce;                // if >0 client will do extra physics bouncing arc translates when actor lands, math isn't known yet
        public float DestinationZ;          // Once Actor.Position.Z reaches this value while landing the translate ends.

        // Some useful math for calculating arcs:
        // TicksToArrival = current_tick + Math.sqrt(2 * height / +Gravity) * 2
        // Velocity.Z = +Gravity * Math.sqrt(2 * height / +Gravity)
        // Actor.Position.Z = 0.5 * Gravity * (ticks*ticks) + Velicity.Z*ticks + Start.Z

        public ACDTranslateArcMessage() : base(Opcodes.ACDTranslateArcMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadInt(32);
            Start = new Vector3D();
            Start.Parse(buffer);
            Velocity = new Vector3D();
            Velocity.Parse(buffer);
            Field3 = buffer.ReadInt(26);
            FlyingAnimationTagID = buffer.ReadInt(21) + (-1);
            LandingAnimationTagID = buffer.ReadInt(21) + (-1);
            Gravity = buffer.ReadFloat32();
            PowerSNO = buffer.ReadInt(32);
            Bounce = buffer.ReadFloat32();
            DestinationZ = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorId);
            Start.Encode(buffer);
            Velocity.Encode(buffer);
            buffer.WriteInt(26, Field3);
            buffer.WriteInt(21, FlyingAnimationTagID - (-1));
            buffer.WriteInt(21, LandingAnimationTagID - (-1));
            buffer.WriteFloat32(Gravity);
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteFloat32(Bounce);
            buffer.WriteFloat32(DestinationZ);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateArcMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            Start.AsText(b, pad);
            Velocity.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            b.Append(' ', pad); b.AppendLine("FlyingAnimationTagID: 0x" + FlyingAnimationTagID.ToString("X8") + " (" + FlyingAnimationTagID + ")");
            b.Append(' ', pad); b.AppendLine("LandingAnimationTagID: 0x" + LandingAnimationTagID.ToString("X8") + " (" + LandingAnimationTagID + ")");
            b.Append(' ', pad); b.AppendLine("Gravity: " + Gravity.ToString("G"));
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Bounce: " + Bounce.ToString("G"));
            b.Append(' ', pad); b.AppendLine("DestinationZ: " + DestinationZ.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
