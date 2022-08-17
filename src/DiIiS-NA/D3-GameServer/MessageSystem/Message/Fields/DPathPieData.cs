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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class DPathPieData
    {
        public Vector3D Field0;
        public float Field1;
        public float Field2;
        public int Field3;

        public void Parse(GameBitBuffer buffer)
        {
            Field0 = new Vector3D();
            Field0.Parse(buffer);
            Field1 = buffer.ReadFloat32();
            Field2 = buffer.ReadFloat32();
            Field3 = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            Field0.Encode(buffer);
            buffer.WriteFloat32(Field1);
            buffer.WriteFloat32(Field2);
            buffer.WriteInt(32, Field3);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DPathSinData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            Field0.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("Field1: " + Field1.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Field2: " + Field2.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
