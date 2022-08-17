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
    [Message(Opcodes.ACDTranslateNormalMessage)]
    public class ACDTranslateNormalMessage : GameMessage
    {
        public uint ActorId;
        public Vector3D Position;  
        public float? Angle;      
        public bool? SnapFacing;       
        public float? MovementSpeed;    
        public int? MoveFlags;
        public int? AnimationTag;  
        public int? WalkInPlaceTicks;
        public int? SnoPowerPassability;
        public int? AckValue;
        public int? MoveType;
        public int? MaxTicksToWalk;

        public ACDTranslateNormalMessage() : base(Opcodes.ACDTranslateNormalMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            if (buffer.ReadBool())
            {
                Position = new Vector3D();
                Position.Parse(buffer);
            }
            if (buffer.ReadBool())
            {
                Angle = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                SnapFacing = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                MovementSpeed = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                MoveFlags = buffer.ReadInt(26);
            }
            if (buffer.ReadBool())
            {
                AnimationTag = buffer.ReadInt(21) + (-1);
            }
            if (buffer.ReadBool())
            {
                WalkInPlaceTicks = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                SnoPowerPassability = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                AckValue = buffer.ReadInt(16);
            }
            if (buffer.ReadBool())
            {
                MoveType = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MaxTicksToWalk = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            buffer.WriteBool(Position != null);
            if (Position != null)
            {
                Position.Encode(buffer);
            }
            buffer.WriteBool(Angle.HasValue);
            if (Angle.HasValue)
            {
                buffer.WriteFloat32(Angle.Value);
            }
            buffer.WriteBool(SnapFacing.HasValue);
            if (SnapFacing.HasValue)
            {
                buffer.WriteBool(SnapFacing.Value);
            }
            buffer.WriteBool(MovementSpeed.HasValue);
            if (MovementSpeed.HasValue)
            {
                buffer.WriteFloat32(MovementSpeed.Value);
            }
            buffer.WriteBool(MoveFlags.HasValue);
            if (MoveFlags.HasValue)
            {
                buffer.WriteInt(26, MoveFlags.Value);
            }
            buffer.WriteBool(AnimationTag.HasValue);
            if (AnimationTag.HasValue)
            {
                buffer.WriteInt(21, AnimationTag.Value - (-1));
            }
            buffer.WriteBool(WalkInPlaceTicks.HasValue);
            if (WalkInPlaceTicks.HasValue)
            {
                buffer.WriteInt(32, WalkInPlaceTicks.Value);
            }
            buffer.WriteBool(SnoPowerPassability.HasValue);
            if (SnoPowerPassability.HasValue)
            {
                buffer.WriteInt(32, SnoPowerPassability.Value);
            }
            buffer.WriteBool(AckValue.HasValue);
            if (AckValue.HasValue)
            {
                buffer.WriteInt(16, AckValue.Value);
            }
            buffer.WriteBool(MoveType.HasValue);
            if (MoveType.HasValue)
            {
                buffer.WriteInt(32, MoveType.Value);
            }
            buffer.WriteBool(MaxTicksToWalk.HasValue);
            if (MaxTicksToWalk.HasValue)
            {
                buffer.WriteInt(32, MaxTicksToWalk.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateNormalMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8"));
            if (Position != null)
            {
                Position.AsText(b, pad);
            }
            if (Angle.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Angle.Value: " + Angle.Value.ToString("G"));
            }
            if (SnapFacing.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("SnapFacing.Value: " + (SnapFacing.Value ? "true" : "false"));
            }
            if (MovementSpeed.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MovementSpeed.Value: " + MovementSpeed.Value.ToString("G"));
            }
            if (MoveFlags.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MoveFlags.Value: 0x" + MoveFlags.Value.ToString("X8") + " (" + MoveFlags.Value + ")");
            }
            if (AnimationTag.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("AnimationTag.Value: 0x" + AnimationTag.Value.ToString("X8") + " (" + AnimationTag.Value + ")");
            }
            if (WalkInPlaceTicks.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("WalkInPlaceTicks.Value: 0x" + WalkInPlaceTicks.Value.ToString("X8") + " (" + WalkInPlaceTicks.Value + ")");
            }
            if (SnoPowerPassability.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("SnoPowerPassability.Value: 0x" + SnoPowerPassability.Value.ToString("X8") + " (" + SnoPowerPassability.Value + ")");
            }
            if (AckValue.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("AckValue.Value: 0x" + AckValue.Value.ToString("X8") + " (" + AckValue.Value + ")");
            }
            if (MoveType.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MoveType.Value: 0x" + MoveType.Value.ToString("X8") + " (" + MoveType.Value + ")");
            }
            if (MaxTicksToWalk.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MaxTicksToWalk.Value: 0x" + MaxTicksToWalk.Value.ToString("X8") + " (" + MaxTicksToWalk.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
