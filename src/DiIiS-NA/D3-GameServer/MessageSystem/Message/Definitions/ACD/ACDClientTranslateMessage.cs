using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDClientTranslateMessage, Consumers.Player)]
    public class ACDClientTranslateMessage : GameMessage
    {
        public int TickBase;
        public int TicksAhead;
        public Vector3D Position;
        public float Angle;
        public float MovementSpeed;
        public int MoveFlags;
        public int AnimationTag;
        public int? SnoPower;

        public override void Parse(GameBitBuffer buffer)
        {
            TickBase = buffer.ReadInt(32);
            TicksAhead = buffer.ReadInt(4);
            Position = new Vector3D();
            Position.Parse(buffer);
            Angle = buffer.ReadFloat32();
            MovementSpeed = buffer.ReadFloat32();
            MoveFlags = buffer.ReadInt(32);
            AnimationTag = buffer.ReadInt(21) + (-1);
            if (buffer.ReadBool())
                SnoPower = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, TickBase);
            buffer.WriteInt(4, TicksAhead);
            Position.Encode(buffer);
            buffer.WriteFloat32(Angle);
            buffer.WriteFloat32(MovementSpeed);
            buffer.WriteInt(32, MoveFlags);
            buffer.WriteInt(21, AnimationTag - (-1));
            if (SnoPower.HasValue)
                buffer.WriteInt(32, SnoPower.Value);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDClientTranslateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("TickBase: 0x" + TickBase.ToString("X8") + " (" + TickBase + ")");
            b.Append(' ', pad); b.AppendLine("TicksAhead: 0x" + TicksAhead.ToString("X8"));
            Position.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            b.Append(' ', pad); b.AppendLine("MovementSpeed: " + MovementSpeed.ToString("G"));
            b.Append(' ', pad); b.AppendLine("MoveFlags: 0x" + MoveFlags.ToString("X8") + " (" + MoveFlags + ")");
            b.Append(' ', pad); b.AppendLine("AnimationTag: 0x" + AnimationTag.ToString("X8") + " (" + AnimationTag + ")");
            if (SnoPower.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("SnoPower: 0x" + SnoPower.Value.ToString("X8") + " (" + SnoPower.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
