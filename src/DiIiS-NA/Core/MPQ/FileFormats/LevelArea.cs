using System.Collections.Generic;
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.Misc;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.LevelArea)]
    public class LevelArea : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int SNOLevelArea0 { get; private set; }
        public int SNOLevelArea1 { get; private set; }
        public List<LevelAreaServerData> LevelAreaServerData { get; private set; }

        public LevelArea(MpqFile file)
        {

            var stream = file.Open();
            Header = new Header(stream);
            I0 = stream.ReadValueS32();
            SNOLevelArea0 = stream.ReadValueS32();
            SNOLevelArea1 = stream.ReadValueS32();
            stream.Position += 8;
            if (stream.Position + 8 != stream.Length)
                LevelAreaServerData = stream.ReadSerializedData<LevelAreaServerData>(); //32 - 48

            stream.Close();
        }
    }


    public class LevelAreaServerData : ISerializableData
    {
        public int SNOLevelArea0 { get; private set; }
        public GizmoLocSet LocSet { get; private set; }
        public int SNOLevelArea1 { get; private set; }
        public int I0 { get; private set; }
        public List<LevelAreaSpawnPopulation> SpawnPopulation { get; private set; }

        public void Read(MpqFileStream stream)
        {
            SNOLevelArea0 = stream.ReadValueS32();
            LocSet = new GizmoLocSet(stream);
            SNOLevelArea1 = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            SpawnPopulation = stream.ReadSerializedData<LevelAreaSpawnPopulation>();
        }
    }

    public class GizmoLocSet
    {
        public GizmoLocSpawnType[] SpawnType { get; private set; }

        public GizmoLocSet(MpqFileStream stream)
        {
            //stream.Position = 0;
            SpawnType = new GizmoLocSpawnType[52];
            for (int i = 0; i < 52; i++)
                SpawnType[i] = new GizmoLocSpawnType(stream);
        }
    }

    public class GizmoLocSpawnType
    {
        public List<GizmoLocSpawnEntry> SpawnEntry { get; private set; }
        public string Description { get; private set; }
        public string Comment { get; private set; }

        public GizmoLocSpawnType(MpqFileStream stream)
        {
            stream.Position += 8;
            SpawnEntry = stream.ReadSerializedData<GizmoLocSpawnEntry>();
            //this.Description = stream.ReadString(80, true);
            //this.Comment = stream.ReadString(256, true);
        }
    }

    public class GizmoLocSpawnChoice : ISerializableData
    {
        public SNOHandle SNOHandle { get; private set; }
        public int ForceRandomFacing { get; private set; }
        public float Weight { get; private set; }
        public int MaxTimesPicked { get; private set; }


        public void Read(MpqFileStream stream)
        {
            SNOHandle = new SNOHandle(stream);
            ForceRandomFacing = stream.ReadValueS32();
            Weight = stream.ReadValueF32();
            MaxTimesPicked = stream.ReadValueS32();
        }
    }

    public class GizmoLocSpawnEntry : ISerializableData
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        //public SNOHandle SNOHandle { get; private set; }
        public int SnoRequiredQuest { get; private set; }
        public int SnoCondition { get; private set; }
        public int Flags { get; private set; }
        public float HighPrecisionPercent { get; private set; }
        public int ConditionSNO { get; private set; }
        public List<GizmoLocSpawnChoice> GizmoLocSpawnChoices { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Min = stream.ReadValueS32();
            Max = stream.ReadValueS32();
            //this.SNOHandle = new SNOHandle(stream);
            SnoRequiredQuest = stream.ReadValueS32();
            SnoCondition = stream.ReadValueS32();
            Flags = stream.ReadValueS32();
            HighPrecisionPercent = stream.ReadValueF32();
            ConditionSNO = stream.ReadValueS32();
            stream.Position += 8;
            GizmoLocSpawnChoices = stream.ReadSerializedData<GizmoLocSpawnChoice>();
        }
    }

    public class LevelAreaSpawnPopulation : ISerializableData
    {
        [PersistentProperty("Description")]
        public string Description { get; private set; }
        [PersistentProperty("I0")]
        public int I0 { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        [PersistentProperty("I1", 4)]
        public int[] I1 { get; private set; }
        [PersistentProperty("I2", 4)]
        public int[] I2 { get; private set; }
        [PersistentProperty("SpawnGroupsCount")]
        public int SpawnGroupsCount { get; private set; }
        public List<LevelAreaSpawnGroup> SpawnGroup { get; private set; }
        public List<int> SNOs { get; private set; }
        [PersistentProperty("SpawnGroup")]
        public int SNO { get; private set; }
        public void Read(MpqFileStream stream)
        {
            Description = stream.ReadString(64, true);
            I0 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            F1 = stream.ReadValueF32();

            I1 = new int[4];
            for (int i = 0; i < 4; i++)
                I1[i] = stream.ReadValueS32();
            I2 = new int[4];
            for (int i = 0; i < 4; i++)
                I2[i] = stream.ReadValueS32();
            SpawnGroupsCount = stream.ReadValueS32();

            stream.Position += 8;
            SpawnGroup = stream.ReadSerializedData<LevelAreaSpawnGroup>();
            stream.Position += 8;
            SNOs = stream.ReadSerializedInts();

            SNO = stream.ReadValueS32();
        }
    }

    public class LevelAreaSpawnGroup : ISerializableData
    {
        [PersistentProperty("GroupType")]
        public SpawnGroupType GroupType { get; private set; }
        [PersistentProperty("F0")]
        public float F0 { get; private set; }
        [PersistentProperty("F1")]
        public float F1 { get; private set; }
        [PersistentProperty("I0")]
        public int I0 { get; private set; }
        [PersistentProperty("I1")]
        public int SpawnItemsCount { get; private set; }
        [PersistentProperty("SpawnItems")]
        public List<LevelAreaSpawnItem> SpawnItems { get; private set; }
        [PersistentProperty("I2")]
        public int I2 { get; private set; }
        [PersistentProperty("I3")]
        public int I3 { get; private set; }
        public int SNO { get; private set; }
        public void Read(MpqFileStream stream)
        {
            GroupType = (SpawnGroupType)stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            F1 = stream.ReadValueF32();
            I0 = stream.ReadValueS32();
            SpawnItemsCount = stream.ReadValueS32();
            stream.Position += 12;
            SpawnItems = stream.ReadSerializedData<LevelAreaSpawnItem>();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            SNO = stream.ReadValueS32();
        }
    }

    public enum SpawnGroupType : int
    {
        Density = 0,
        Exactly = 1,
    }

    public class LevelAreaSpawnItem : ISerializableData
    {
        [PersistentProperty("SNOHandle")]
        public SNOHandle SNOHandle { get; private set; }
        [PersistentProperty("SpawnType")]
        public SpawnType SpawnType { get; private set; }
        [PersistentProperty("I0")]
        public int I0 { get; private set; }
        [PersistentProperty("I1")]
        public int I1 { get; private set; }
        [PersistentProperty("I2")]
        public int I2 { get; private set; }
        [PersistentProperty("I3")]
        public int I3 { get; private set; }
        public float F0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            SNOHandle = new SNOHandle(stream);
            SpawnType = (SpawnType)stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
        }
    }

    public enum SpawnType : int
    {
        Normal = 0,
        Champion,
        Rare,
        Minion,
        Unique,
        Hireling,
        Clone,
        Boss,
    }
}
