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
    [Message(Opcodes.ACDTranslateDetPathMessage)]
    public class ACDTranslateDetPathMessage : GameMessage
    {
        public int ActorID;
        public int Seed;
        public int Carry;
        public int Field3;
        public Vector3D Velocity;
        public float /* angle */ Angle;
        public Vector3D StartPos;
        public int MoveFlags;
        public int AnimTag;
        public int Field9;
        public int /* sno */ Field10;
        public float Field11;
        public float Field12;
        public float Field13;
        public float Field14;
        public float Field15;
        public float Field16;

        public ACDTranslateDetPathMessage() : base(Opcodes.ACDTranslateDetPathMessage) { }


        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadInt(32);
            Seed = buffer.ReadInt(4);
            Carry = buffer.ReadInt(32);
            Field3 = buffer.ReadInt(32);
            Velocity = new Vector3D();
            Velocity.Parse(buffer);
            Angle = buffer.ReadFloat32();
            StartPos = new Vector3D();
            StartPos.Parse(buffer);
            MoveFlags = buffer.ReadInt(32);
            AnimTag = buffer.ReadInt(32);
            Field9 = buffer.ReadInt(32);
            Field10 = buffer.ReadInt(32);
            Field11 = buffer.ReadFloat32();
            Field12 = buffer.ReadFloat32();
            Field13 = buffer.ReadFloat32();
            Field14 = buffer.ReadFloat32();
            Field15 = buffer.ReadFloat32();
            Field16 = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorID);
            buffer.WriteInt(4, Seed);
            buffer.WriteInt(32, Carry);
            buffer.WriteInt(32, Field3);
            Velocity.Encode(buffer);
            buffer.WriteFloat32(Angle);
            StartPos.Encode(buffer);
            buffer.WriteInt(32, MoveFlags);
            buffer.WriteInt(32, AnimTag);
            buffer.WriteInt(32, Field9);
            buffer.WriteInt(32, Field10);
            buffer.WriteFloat32(Field11);
            buffer.WriteFloat32(Field12);
            buffer.WriteFloat32(Field13);
            buffer.WriteFloat32(Field14);
            buffer.WriteFloat32(Field15);
            buffer.WriteFloat32(Field16);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateDetPathMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Seed: 0x" + Seed.ToString("X8") + " (" + Seed + ")");
            b.Append(' ', pad); b.AppendLine("Carry: 0x" + Carry.ToString("X8") + " (" + Carry + ")");
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            Velocity.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            StartPos.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("MoveFlags: 0x" + MoveFlags.ToString("X8") + " (" + MoveFlags + ")");
            b.Append(' ', pad); b.AppendLine("AnimTag: 0x" + AnimTag.ToString("X8") + " (" + AnimTag + ")");
            b.Append(' ', pad); b.AppendLine("Field9: 0x" + Field9.ToString("X8") + " (" + Field9 + ")");
            b.Append(' ', pad); b.AppendLine("Field10: 0x" + Field10.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field11: 0x" + Field11.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field12: " + Field12.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field13: " + Field13.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field14: " + Field14.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field15: " + Field15.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field16: " + Field15.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
