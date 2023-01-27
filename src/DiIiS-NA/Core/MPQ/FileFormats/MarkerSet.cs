//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Collision;
using DiIiS_NA.GameServer.Core.Types.Math;
using Gibbed.IO;
using System.Collections.Generic;
using DiIiS_NA.GameServer.Core.Types.TagMap;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.MarkerSet)]
    public class MarkerSet : FileFormat
    {
        public Header Header { get; private set; }
        public List<Marker> Markers = new List<Marker>();
        public List<Circle> NoSpawns { get; private set; }
        public AABB AABB { get; private set; }
        public bool ContainsActorLocations { get; private set; }
        public int NLabel { get; private set; }
        public int SpecialIndexCount { get; private set; }
        public List<short> SpecialIndexList { get; private set; }

        public MarkerSet(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);//0
            this.Markers = stream.ReadSerializedData<Marker>(); //28
            stream.Position += 4;
            NoSpawns = stream.ReadSerializedData<Circle>(); //96
            stream.Position += (15 * 4);
            this.AABB = new AABB(stream); //160
            //stream.Position += (14 * 4);
            int i0 = stream.ReadValueS32(); //184

            if (i0 != 0 && i0 != 1)
                //this.ContainsActorLocations = false;
                throw new System.Exception("Farmy thought this field is a bool, but apparently its not");
            else
                this.ContainsActorLocations = i0 == 1;

            this.NLabel = stream.ReadValueS32(); //200
            this.SpecialIndexCount = stream.ReadValueS32(); //204
            this.SpecialIndexList = stream.ReadSerializedShorts(); //208
            stream.Close();
        }
    }

    public class Marker : ISerializableData
    {
        public string Name { get; private set; }
        public MarkerType Type { get; private set; }
        public PRTransform PRTransform { get; private set; }
        public SNOHandle SNOHandle { get; private set; }
        public TagMap TagMap { get; private set; }
        public int MarkerLinksCount { get; private set; }
        public List<MarkerLink> MarkerLinks = new List<MarkerLink>();

        public void Read(MpqFileStream stream)
        {
            this.Name = stream.ReadString(128, true);
            this.Type = (MarkerType)stream.ReadValueS32();
            this.PRTransform = new PRTransform(stream);
            this.SNOHandle = new SNOHandle(stream);
            this.TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += 8;
            this.MarkerLinksCount = stream.ReadValueS32();
            this.MarkerLinks = stream.ReadSerializedData<MarkerLink>();
            stream.Position += (3 * 4);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Name, SNOHandle.Name);
        }
    }

    public class Circle : ISerializableData
    {
        public Vector2F Center { get; private set; }
        public float Radius { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Center = new Vector2F(stream.ReadValueF32(), stream.ReadValueF32());
            Radius = stream.ReadValueF32();
        }
    }

    public class MarkerLink : ISerializableData
    {
        public string String1 { get; private set; }
        public string String2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.String1 = stream.ReadString(128, true);
            this.String2 = stream.ReadString(128, true);
        }
    }

    public enum MarkerType
    {
        Actor = 0,
        Light = 1,
        Camera = 3,
        AudioVolume = 4,
        WeatherVolume = 5,
        AmbientSound = 6,
        Particle = 7,

        Scene = 9,
        Encounter = 10,

        SceneGroup = 12,
        Script = 13,
        Adventure = 14,


        SubScenePosition = 16,

        MinimapMarker = 28,
        Event = 29,
        Conductror = 30,
        EffectGroup = 31,

        GizmoLocationA = 50,
        GizmoLocationB = 51,
        GizmoLocationC = 52,
        GizmoLocationD = 53,
        GizmoLocationE = 54,
        GizmoLocationF = 55,
        GizmoLocationG = 56,
        GizmoLocationH = 57,
        GizmoLocationI = 58,
        GizmoLocationJ = 59,
        GizmoLocationK = 60,
        GizmoLocationL = 61,
        GizmoLocationM = 62,
        GizmoLocationN = 63,
        GizmoLocationO = 64,
        GizmoLocationP = 65,
        GizmoLocationQ = 66,
        GizmoLocationR = 67,
        GizmoLocationS = 68,
        GizmoLocationT = 69,
        GizmoLocationU = 70,
        GizmoLocationV = 71,
        GizmoLocationW = 72,
        GizmoLocationX = 73,
        GizmoLocationY = 74,
        GizmoLocationZ = 75,
    }
}
