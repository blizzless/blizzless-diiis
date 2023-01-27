//Blizzless Project 2022
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using System.Collections.Generic;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.PhysMesh)]
    public class PhysMesh : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int CollisionMeshCount { get; private set; }
        public List<CollisionMesh> CollisionMeshes { get; private set; }
        public int I1 { get; private set; }
        //public string S0 { get; private set; }
        //public string S1 { get; private set; }

        public PhysMesh(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream); //0
            //+16
            this.I0 = stream.ReadValueS32(); //28
            this.CollisionMeshCount = stream.ReadValueS32(); //32
            this.CollisionMeshes = stream.ReadSerializedData<CollisionMesh>(); //36
            stream.Position += 12;
            this.I1 = stream.ReadValueS32(); //26
            stream.Close();
        }
    }

    public class CollisionMesh : ISerializableData
    {
        public Float3 F0 { get; private set; }
        public Float3 F1 { get; private set; }
        public Float3 F2 { get; private set; }
        public int DominoNodeCount { get; private set; }
        public int VerticeCount { get; private set; }
        public int DominoTriangleCount { get; private set; }
        public int DominoEdgeCount { get; private set; }

        public List<Float4> Vertices { get; private set; }
        public List<MeshTriangle> DominoTriangles { get; private set; }
        public List<MeshNode> DominoNodes { get; private set; }

        public int I6 { get; private set; }
        public int I7 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            //64
            stream.Position += (4 * 6);
            this.F0 = new Float3(stream); //88
            this.F1 = new Float3(stream); //100
            this.F2 = new Float3(stream); //112
            this.DominoNodeCount = stream.ReadValueS32(); //124
            this.VerticeCount = stream.ReadValueS32(); //128
            this.DominoTriangleCount = stream.ReadValueS32(); //132
            this.DominoEdgeCount = stream.ReadValueS32(); //136
            stream.Position += 4;
            this.Vertices = stream.ReadSerializedData<Float4>(); //96 - 160
            this.DominoTriangles = stream.ReadSerializedData<MeshTriangle>(); //104 - 168
            this.DominoNodes = stream.ReadSerializedData<MeshNode>(); //112 - 176

            //stream.Position += 4 * 2;
            this.I6 = stream.ReadValueS32(); //120 - 184
            this.I7 = stream.ReadValueS32(); //124 - 188
            //176
        }
    }

    public class Float4 : ISerializableData
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float K { get; private set; }

        public Float4() { }

        public Float4(MpqFileStream stream)
        {
            X = stream.ReadValueF32();
            Y = stream.ReadValueF32();
            Z = stream.ReadValueF32();
            K = stream.ReadValueF32();
        }

        public void Read(MpqFileStream stream)
        {
            X = stream.ReadValueF32();
            Y = stream.ReadValueF32();
            Z = stream.ReadValueF32();
            K = stream.ReadValueF32();
        }
    }

    public class Float3 : ISerializableData
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public Float3() { }

        public Float3(MpqFileStream stream)
        {
            X = stream.ReadValueF32();
            Y = stream.ReadValueF32();
            Z = stream.ReadValueF32();
        }

        public void Read(MpqFileStream stream)
        {
            X = stream.ReadValueF32();
            Y = stream.ReadValueF32();
            Z = stream.ReadValueF32();
        }
    }

    public class MeshTriangle : ISerializableData
    {
        public int VerticeOneIndex { get; private set; }
        public int VerticeTwoIndex { get; private set; }
        public int VerticeThreeIndex { get; private set; }

        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int I5 { get; private set; }

        public short I6 { get; private set; }
        public short I7 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            VerticeOneIndex = stream.ReadValueS32();
            VerticeTwoIndex = stream.ReadValueS32();
            VerticeThreeIndex = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            I5 = stream.ReadValueS32();
            I6 = stream.ReadValueS16();
            I7 = stream.ReadValueS16();
            // i6 is a word, but struct is 28 bytes - DarkLotus
            //<Field Type="DT_WORD#30" Offset="24" Flags="1" EncodedBits="16" />
            //<Field Offset="28" Flags="0" />

        }
    }

    public class MeshEdge : ISerializableData
    {
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
        }
    }

    public class MeshNode : ISerializableData
    {
        public short B0 { get; private set; }
        public short B1 { get; private set; }

        public short B2 { get; private set; }
        public short B3 { get; private set; }

        public short B4 { get; private set; }
        public short B5 { get; private set; }

        public int I0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            B0 = stream.ReadValueS16();
            B1 = stream.ReadValueS16();
            B2 = stream.ReadValueS16();
            B3 = stream.ReadValueS16();
            B4 = stream.ReadValueS16();
            B5 = stream.ReadValueS16();
            I0 = stream.ReadValueS32();
        }
    }
}
