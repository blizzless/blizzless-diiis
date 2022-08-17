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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateFixedMessage)]
    public class ACDTranslateFixedMessage : GameMessage
    {
        public int ActorId;
        public Vector3D Velocity;       // Velocity in game units per game tick
        public int MoveFlags;
        public int AnimationTag;        // Animation used during movement
        public int /* sno */ SNOPowerPassability;

        public ACDTranslateFixedMessage() : base(Opcodes.ACDTranslateFixedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadInt(32);
            Velocity = new Vector3D();
            Velocity.Parse(buffer);
            MoveFlags = buffer.ReadInt(26);
            AnimationTag = buffer.ReadInt(21) + (-1);
            SNOPowerPassability = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorId);
            Velocity.Encode(buffer);
            buffer.WriteInt(26, MoveFlags);
            buffer.WriteInt(21, AnimationTag - (-1));
            buffer.WriteInt(32, SNOPowerPassability);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateFixedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            Velocity.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("MoveFlags: 0x" + MoveFlags.ToString("X8") + " (" + MoveFlags + ")");
            b.Append(' ', pad); b.AppendLine("AnimationTag: 0x" + AnimationTag.ToString("X8") + " (" + AnimationTag + ")");
            b.Append(' ', pad); b.AppendLine("SNOPowerPassability: 0x" + SNOPowerPassability.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
