//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.FlippyMessage)]
    public class FlippyMessage : GameMessage
    {
        public int ActorID;             // Effect is created at the actors location
        public int SNOParticleEffect;   // SNO for a particle effect or 0x6d82 (default_flippy) for an appearance effect
        public int SNOFlippyActor;      // -1 for a particle effect or ActorSNO for the actor to use during flipping
        public Vector3D Destination;

        public FlippyMessage() : base(Opcodes.FlippyMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadInt(32);
            SNOParticleEffect = buffer.ReadInt(32);
            SNOFlippyActor = buffer.ReadInt(32);
            Destination = new Vector3D();
            Destination.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorID);
            buffer.WriteInt(32, SNOParticleEffect);
            buffer.WriteInt(32, SNOFlippyActor);
            Destination.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FlippyMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("SNOParticleEffect: 0x" + SNOParticleEffect.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("SNOFlippyActor: 0x" + SNOFlippyActor.ToString("X8"));
            Destination.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
