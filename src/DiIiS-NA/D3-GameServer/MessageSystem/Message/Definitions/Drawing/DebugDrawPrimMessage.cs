using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Drawing
{
    [Message(Opcodes.DebugDrawPrimMessage)]
    public class DebugDrawPrimMessage : GameMessage
    {
        public int Field0;
        public int Field1;
        public int Field2;
        public Vector3D Field3;
        public Vector3D Field4;
        public float Field5;
        public float Field6;
        public int Field7;
        public RGBAColor Field8;
        public string Field9;

        public DebugDrawPrimMessage()
            : base(Opcodes.DebugDrawPrimMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
            Field3 = new Vector3D();
            Field3.Parse(buffer);
            Field4 = new Vector3D();
            Field4.Parse(buffer);
            Field5 = buffer.ReadFloat32();
            Field6 = buffer.ReadFloat32();
            Field7 = buffer.ReadInt(32);
            Field8 = new RGBAColor();
            Field8.Parse(buffer);
            Field9 = buffer.ReadCharArray(128);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(32, Field1);
            buffer.WriteInt(32, Field2);
            Field3.Encode(buffer);
            Field4.Encode(buffer);
            buffer.WriteFloat32(Field5);
            buffer.WriteFloat32(Field6);
            buffer.WriteInt(32, Field7);
            Field8.Encode(buffer);
            buffer.WriteCharArray(128, Field9);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DebugDrawPrimMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            Field3.AsText(b, pad);
            Field4.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Field5: " + Field5.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field6: " + Field6.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field7: 0x" + Field7.ToString("X8") + " (" + Field7 + ")");
            Field8.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Field9: \"" + Field9 + "\"");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
