//Blizzless Project 2022
using System.Collections.Generic;
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Misc;
using DiIiS_NA.GameServer.Core.Types.Scene;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Worlds)]
    public class World : FileFormat
    {
        public Header Header { get; private set; }
        public bool DynamicWorld { get; set; }
        public List<ServerData> ServerData { get; private set; }


        [PersistentProperty("DRLGParams")]
        public List<DRLGParams> DRLGParams { get; set; }
        [PersistentProperty("SceneParams")]
        public SceneParams SceneParams { get; set; }
        [PersistentProperty("SceneClusterSet")]
        public SceneClusterSet SceneClusterSet { get; set; }
        [PersistentProperty("LabelRuleSet")]
        public LabelRuleSet LabelRuleSet { get; set; }
        [PersistentProperty("SNONavMeshFunctions", 4)]
        public int[] SNONavMeshFunctions = new int[4];

        public List<int> MarkerSets = new List<int>();
        public Environment Environment { get; set; }
        public float DeformationScale { get; private set; }
        public int Flags { get; private set; }
        public float PVPMinimapPerPixel { get; private set; }

        public World() { }

        public World(MpqFile file)
        {
            var stream = file.Open();

            Header = new Header(stream);

            DynamicWorld = (stream.ReadValueS32() != 0);
            stream.Position += 8;
            ServerData = stream.ReadSerializedData<ServerData>(); //16
            MarkerSets = stream.ReadSerializedInts(); //40

            Environment = new Environment(stream); //96  - 56
            // - 56
            DeformationScale = stream.ReadValueF32(); //172
            Flags = stream.ReadValueS32(); //176
            PVPMinimapPerPixel = stream.ReadValueF32(); //180

            stream.Close();
        }
        public void CreateNewSceneParams()
        {
            SceneParams = new SceneParams();
            SceneParams.ClearSceneParams();
        }
    }

    #region scene-params

    public class SceneParams : ISerializableData
    {
        [PersistentProperty("SceneChunks")]
        public List<SceneChunk> SceneChunks { get; set; }
        [PersistentProperty("ChunkCount")]
        public int ChunkCount { get; set; }

        public SceneParams() { }

        public void Read(MpqFileStream stream)
        {
            SceneChunks = stream.ReadSerializedData<SceneChunk>();
            ChunkCount = stream.ReadValueS32();
            stream.Position += (3 * 4);
        }

        public void ClearSceneParams()
        {
            SceneChunks = new List<SceneChunk>();
            ChunkCount = 0;
        }
    }

    public class SceneChunk : ISerializableData
    {
        [PersistentProperty("SNOHandle")]
        public SNOHandle SNOHandle { get; set; }
        [PersistentProperty("PRTransform")]
        public PRTransform PRTransform { get; set; }
        [PersistentProperty("SceneSpecification")]
        public SceneSpecification SceneSpecification { get; set; }

        public SceneChunk() { }

        public void Read(MpqFileStream stream)
        {
            SNOHandle = new SNOHandle(stream);
            PRTransform = new PRTransform(stream);
            SceneSpecification = new SceneSpecification(stream);
        }
    }

    #endregion

    #region drlg-params

    public class DRLGParams : ISerializableData
    {
        [PersistentProperty("Tiles")]
        public List<TileInfo> Tiles { get; private set; }

        [PersistentProperty("CommandCount")]
        public int CommandCount { get; private set; }

        [PersistentProperty("LevelArea")]
        public int LevelArea { get; private set; }

        [PersistentProperty("ChunkSize")]
        public int ChunkSize { get; private set; }

        [PersistentProperty("Weather")]
        public int Weather { get; private set; }

        [PersistentProperty("PrevWorld")]
        public int PrevWorld { get; private set; }

        [PersistentProperty("PrevLevelArea")]
        public int PrevLevelArea { get; private set; }

        [PersistentProperty("PrevStartingPoint")]
        public int PrevStartingPoint { get; private set; }

        [PersistentProperty("NextWorld")]
        public int NextWorld { get; private set; }

        [PersistentProperty("NextLevelArea")]
        public int NextLevelArea { get; private set; }

        [PersistentProperty("NextStartingPoint")]
        public int NextStartingPoint { get; private set; }

        [PersistentProperty("Commands")]
        public List<DRLGCommand> Commands { get; private set; }

        [PersistentProperty("ParentIndices")]
        public List<int> ParentIndices { get; private set; }

        [PersistentProperty("TagMap")]
        public TagMap TagMap { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Tiles = stream.ReadSerializedData<TileInfo>();

            stream.Position += (14 * 4);
            CommandCount = stream.ReadValueS32();
            Commands = stream.ReadSerializedData<DRLGCommand>();

            stream.Position += (3 * 4);
            ParentIndices = stream.ReadSerializedInts();

            stream.Position += (2 * 4);
            TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);
        }
    }

    public enum TileExits
    {
        West = 1,
        East = 2,
        North = 4,
        South = 8,
    }

    public enum TileTypes
    {
        Normal = 100,
        EventTile1 = 101, // Jar of souls? more? Deadend?
        EventTile2 = 102, // 1000 dead
        Entrance = 200,
        UEntrance1 = 201, // Defiled crypt what there?
        Exit = 300,
        Filler = 401
    }

    public class TileInfo : ISerializableData
    {
        [PersistentProperty("Int0")]
        public int ExitDirectionBits { get; private set; }

        [PersistentProperty("Int1")]
        public int TileType { get; private set; }

        [PersistentProperty("SNOScene")]
        public int SNOScene { get; private set; }

        [PersistentProperty("Int2")]
        public int Probability { get; private set; }

        [PersistentProperty("TagMap")]
        public TagMap TagMap { get; private set; }

        [PersistentProperty("CustomTileInfo")]
        public CustomTileInfo CustomTileInfo { get; private set; }

        public void Read(MpqFileStream stream)
        {
            ExitDirectionBits = stream.ReadValueS32();
            TileType = stream.ReadValueS32();
            SNOScene = stream.ReadValueS32();
            Probability = stream.ReadValueS32();
            TagMap = stream.ReadSerializedItem<TagMap>();

            stream.Position += (2 * 4);
            CustomTileInfo = new CustomTileInfo(stream);
        }
    }

    public enum CommandType
    {
        Waypoint = 0,
        BridleEntrance = 1,
        AddExit = 2,
        AddHub = 3,
        AddSpoke = 4,
        Group = 9, //used in DRLG to group tiles together
    }

    public class DRLGCommand : ISerializableData
    {
        [PersistentProperty("Name")]
        public string Name { get; private set; }

        [PersistentProperty("I0")]
        public int CommandType { get; private set; }

        [PersistentProperty("TagMap")]
        public TagMap TagMap { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Name = stream.ReadString(128, true);
            CommandType = stream.ReadValueS32();
            TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (3 * 4);
        }
    }

    public class CustomTileInfo
    {
        [PersistentProperty("I0")]
        public int Int0 { get; private set; }

        [PersistentProperty("I1")]
        public int SizeX { get; private set; }

        [PersistentProperty("I2")]
        public int SizeY { get; private set; }

        [PersistentProperty("V0")]
        public Vector2D Anchor { get; private set; }

        [PersistentProperty("CustomTileCells")]
        public List<CustomTileCell> CustomTileCells { get; private set; }

        public CustomTileInfo() { }

        public CustomTileInfo(MpqFileStream stream)
        {
            Int0 = stream.ReadValueS32();
            SizeX = stream.ReadValueS32();
            SizeY = stream.ReadValueS32();
            Anchor = new Vector2D(stream);
            CustomTileCells = stream.ReadSerializedData<CustomTileCell>();
            stream.Position += (3 * 4);
        }
    }

    public class CustomTileCell : ISerializableData 
    {
        [PersistentProperty("I0")]
        public int Int0 { get; private set; }

        [PersistentProperty("I1")]
        public int TileType { get; private set; }

        [PersistentProperty("I2")]
        public int TileSet { get; private set; }

        [PersistentProperty("SNOScene")]
        public int SNOScene { get; private set; }

        [PersistentProperty("I3")]
        public int TileHeight { get; private set; }

        [PersistentProperty("I4", 4)]
        public int[] ExitHeights { get; private set; }

        public CustomTileCell() { }

        public void Read(MpqFileStream stream)
        {
            Int0 = stream.ReadValueS32();
            TileType = stream.ReadValueS32();
            TileSet = stream.ReadValueS32();
            SNOScene = stream.ReadValueS32();
            TileHeight = stream.ReadValueS32();
            ExitHeights = new int[4];
            for (int i = 0; i < ExitHeights.Length; i++)
            {
                ExitHeights[i] = stream.ReadValueS32();
            }
        }
    }

    #endregion

    #region scene-cluster

    public class SceneClusterSet
    {
        [PersistentProperty("ClusterCount")]
        public int ClusterCount { get; set; }
        [PersistentProperty("SceneClusters")]
        public List<SceneCluster> SceneClusters { get; set; }

        public SceneClusterSet() { }

        public SceneClusterSet(MpqFileStream stream)
        {
            ClusterCount = stream.ReadValueS32();
            stream.Position += (4 * 3);
            SceneClusters = stream.ReadSerializedData<SceneCluster>();
        }
    }

    public class SceneCluster : ISerializableData
    {
        [PersistentProperty("Name")]
        public string Name { get; private set; }
        [PersistentProperty("ClusterId")]
        public int ClusterId { get; set; }
        [PersistentProperty("GroupCount")]
        public int GroupCount { get; set; }
        [PersistentProperty("SubSceneGroups")]
        public List<SubSceneGroup> SubSceneGroups { get; private set; }
        public SubSceneGroup Default { get; private set; }

        public SceneCluster()
        {

        }

        public SceneCluster(string name, int ClustId, int GC)
        {
            Name = name;
            ClusterId = ClusterId;
            GroupCount = GC;
            SubSceneGroups = new List<SubSceneGroup> { };
        }

        public void Read(MpqFileStream stream)
        {
            Name = stream.ReadString(128, true);
            ClusterId = stream.ReadValueS32();
            GroupCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            SubSceneGroups = stream.ReadSerializedData<SubSceneGroup>();

            Default = new SubSceneGroup(stream);
        }
    }

    public class SubSceneGroup : ISerializableData
    {
        [PersistentProperty("I0")]
        public int I0 { get; set; }
        [PersistentProperty("SubSceneCount")]
        public int SubSceneCount { get; set; }
        [PersistentProperty("Entries")]
        public List<SubSceneEntry> Entries { get; set; }

        public SubSceneGroup() { }

        public SubSceneGroup(MpqFileStream stream)
        {
            Read(stream);
        }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            SubSceneCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            Entries = stream.ReadSerializedData<SubSceneEntry>();
        }
    }

    public class SubSceneEntry : ISerializableData
    {
        [PersistentProperty("SNOScene")]
        public int SNOScene { get; private set; }
        [PersistentProperty("Probability")]
        public int Probability { get; private set; }
        [PersistentProperty("LabelCount")]
        public int LabelCount { get; set; }
        [PersistentProperty("Labels")]
        public List<SubSceneLabel> Labels { get; set; }

        public SubSceneEntry() { }

        public SubSceneEntry(int SNO, int Prob)
        {
            SNOScene = SNO;
            Probability = Prob;
            LabelCount = 0;
            Labels = new List<SubSceneLabel> { };
        }

        public void Read(MpqFileStream stream)
        {
            SNOScene = stream.ReadValueS32();
            Probability = stream.ReadValueS32();
            stream.Position += (3 * 4);
            LabelCount = stream.ReadValueS32();
            Labels = stream.ReadSerializedData<SubSceneLabel>();
        }
    }

    public class SubSceneLabel : ISerializableData
    {
        [PersistentProperty("GBId")]
        public int GBId { get; set; }
        [PersistentProperty("I0")]
        public int I0 { get; set; }

        public void Read(MpqFileStream stream)
        {
            GBId = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    #endregion

    #region others

    public class LabelRuleSet
    {
        [PersistentProperty("Rulecount")]
        public int Rulecount { get; private set; }
        [PersistentProperty("LabelRules")]
        public List<LabelRule> LabelRules { get; private set; }

        public LabelRuleSet() { }

        public LabelRuleSet(MpqFileStream stream)
        {
            Rulecount = stream.ReadValueS32();
            stream.Position += (3 * 4);
            LabelRules = stream.ReadSerializedData<LabelRule>();
        }
    }

    public class LabelRule : ISerializableData
    {
        [PersistentProperty("Name")]
        public string Name { get; private set; }
        [PersistentProperty("LabelCondition")]
        public LabelCondition LabelCondition { get; private set; }
        [PersistentProperty("Int0")]
        public int NumToChoose { get; private set; }
        [PersistentProperty("LabelCount")]
        public int LabelCount { get; private set; }
        [PersistentProperty("Entries")]
        public List<LabelEntry> Entries { get; private set; }

        public LabelRule() { }

        public void Read(MpqFileStream stream)
        {
            Name = stream.ReadString(128, true);
            LabelCondition = new LabelCondition(stream);
            NumToChoose = stream.ReadValueS32();
            LabelCount = stream.ReadValueS32();
            stream.Position += (2 * 4);
            Entries = stream.ReadSerializedData<LabelEntry>();
        }
    }

    public class LabelEntry : ISerializableData
    {
        [PersistentProperty("GBIdLabel")]
        public int GBIdLabel { get; private set; }
        [PersistentProperty("Int0")]
        public int Int0 { get; private set; }
        [PersistentProperty("Float0")]
        public float Weight { get; private set; }
        [PersistentProperty("Int1")]
        public int ApplyCountMin { get; private set; }
        [PersistentProperty("Int2")]
        public int ApplyCountMax { get; private set; }

        public LabelEntry() { }

        public void Read(MpqFileStream stream)
        {
            GBIdLabel = stream.ReadValueS32();
            Int0 = stream.ReadValueS32();
            Weight = stream.ReadValueF32();
            ApplyCountMin = stream.ReadValueS32();
            ApplyCountMax = stream.ReadValueS32();
        }
    }

    public class PostFXParams
    {
        public float[] I0 { get; private set; }
        public float[] I1 { get; private set; }

        public PostFXParams(MpqFileStream stream)
        {
            //Int0 = stream.ReadValueS32();
            I0 = new float[4];
            I1 = new float[4];

            for (int i = 0; i < I0.Length; i++)
                I0[i] = stream.ReadValueF32();
            for (int i = 0; i < I1.Length; i++)
                I1[i] = stream.ReadValueF32();
        }
    }


    public class LabelCondition
    {
        [PersistentProperty("Enum0")]
        public DT_ENUM0 Enum0 { get; private set; }
        [PersistentProperty("Int0")]
        public int Flags { get; private set; }
        [PersistentProperty("Int1", 4)]
        public int[] ConditonParm { get; private set; }

        public LabelCondition() { }

        public LabelCondition(MpqFileStream stream)
        {
            Enum0 = (DT_ENUM0)stream.ReadValueS32();
            Flags = stream.ReadValueS32();
            ConditonParm = new int[4];

            for (int i = 0; i < ConditonParm.Length; i++)
            {
                ConditonParm[i] = stream.ReadValueS32();
            }
        }
    }

    public enum DT_ENUM0
    {
        Always = 0,
        GameDifficulty = 1,
        LabelAlreadySet = 2,
    }

    public class Environment
    {
        public RGBAColor RGBASkyColor { get; private set; }
        public PostFXParams PostFXParams { get; private set; }

        //public int[] Unknown { get; private set; }
        public int snoSkyBoxActor { get; private set; }
        public int snoMusic { get; private set; }
        public int snoIntroMusic { get; private set; }
        public float IntroMusicCooldownSecsMin { get; private set; }
        public float IntroMusicCooldownSecsMax { get; private set; }
        public int snoAmbient { get; private set; }
        public int snoReverb { get; private set; }
        public int snoWeather { get; private set; }
        public int snoIrradianceTex { get; private set; }
        public int snoIrradianceTexDead { get; private set; }
        public float FarPlaneCap { get; private set; }

        public Environment(MpqFileStream stream)
        {
            stream.Position += 28;
            RGBASkyColor = new RGBAColor(stream);
            PostFXParams = new PostFXParams(stream);

            stream.Position += 28;

            snoSkyBoxActor = stream.ReadValueS32();
            snoMusic = stream.ReadValueS32();
            snoIntroMusic = stream.ReadValueS32();

            IntroMusicCooldownSecsMin = stream.ReadValueF32();
            IntroMusicCooldownSecsMax = stream.ReadValueF32();

            snoAmbient = stream.ReadValueS32();
            snoReverb = stream.ReadValueS32();
            snoWeather = stream.ReadValueS32();
            snoIrradianceTex = stream.ReadValueS32();
            snoIrradianceTexDead = stream.ReadValueS32();
            FarPlaneCap = stream.ReadValueF32();
        }
    }

    #endregion

    public class ServerData : ISerializableData
    {
        public List<DRLGParams> DRLGParams { get; private set; }
        public SceneParams SceneParams { get; private set; }
        public LabelRuleSet LabelRuleSet { get; private set; }
        public int Int1 { get; private set; }
        public SceneClusterSet SceneClusterSet { get; private set; }
        public int[] SNONavMeshFunctions = new int[4];
        public int[] GBIDUnknown = new int[3];
        public int SNOScript { get; private set; }
        public int Int2 { get; private set; }


        public void Read(MpqFileStream stream)
        {
            //stream.Position += 8;
            DRLGParams = stream.ReadSerializedData<DRLGParams>();
            stream.Position += 8;
            SceneParams = stream.ReadSerializedItem<SceneParams>();
            //stream.Position += 8;
            LabelRuleSet = new LabelRuleSet(stream);
            Int1 = stream.ReadValueS32();
            SceneClusterSet = new SceneClusterSet(stream);
            for (int i = 0; i < SNONavMeshFunctions.Length; i++)
            {
                SNONavMeshFunctions[i] = stream.ReadValueS32();
            }
            for (int i = 0; i < GBIDUnknown.Length; i++)
            {
                GBIDUnknown[i] = stream.ReadValueS32();
            }
            SNOScript = stream.ReadValueS32();
            Int2 = stream.ReadValueS32();
        }
    }
}
