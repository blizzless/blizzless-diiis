//Blizzless Project 2022
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Globals)]
    public class Globals : FileFormat
    {
        public Header Header { get; private set; }
        public List<GlobalServerData> ServerData { get; private set; }
        public int I0 { get; private set; }
        public Dictionary<int, StartLocationName> StartLocationNames { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public float F3 { get; private set; }

        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public float F4 { get; private set; }
        public float F5 { get; private set; }

        public int I3 { get; private set; }

        public float F6 { get; private set; }
        public float F7 { get; private set; }
        public float F8 { get; private set; }
        public float F9 { get; private set; }
        public float F10 { get; private set; }

        public float F11 { get; private set; }
        public float F12 { get; private set; }
        public float F13 { get; private set; }
        public float F14 { get; private set; }

        public int I4 { get; private set; }

        public int[] I6 { get; private set; }
        public BannerParams BannerParams { get; private set; }

        public int I5 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }
        public float F15 { get; private set; }
        public int I10 { get; private set; }

        public float F16 { get; private set; }
        public float F17 { get; private set; }
        public float F18 { get; private set; }
        public float F19 { get; private set; }
        public float F20 { get; private set; }

        public float F21 { get; private set; }
        public float F22 { get; private set; }
        public float F23 { get; private set; }
        public float F24 { get; private set; }
        public float F25 { get; private set; }

        public float F26 { get; private set; }
        public float F27 { get; private set; }
        public float F28 { get; private set; }
        public float F29 { get; private set; }
        public float F30 { get; private set; }

        public float F31 { get; private set; }
        public float F32 { get; private set; }
        public float F33 { get; private set; }
        public float F34 { get; private set; }
        public float F35 { get; private set; }

        public float F36 { get; private set; }
        public int I11 { get; private set; }

        public float F37 { get; private set; }
        public int I12 { get; private set; }
        public int I13 { get; private set; }

        public float F38 { get; private set; }
        public float F39 { get; private set; }
        public float F40 { get; private set; }
        public float F41 { get; private set; }
        public float F42 { get; private set; }

        public float F43 { get; private set; }
        public float F44 { get; private set; }

        public List<AssetList> AssetLists { get; private set; }

        public float F45 { get; private set; }
        public float F46 { get; private set; }
        public float F47 { get; private set; }
        public float F48 { get; private set; }
        public float F49 { get; private set; }

        public float F50 { get; private set; }

        public int I14 { get; private set; }
        public int I15 { get; private set; }

        public float F51 { get; private set; }
        public float F52 { get; private set; }
        public float F53 { get; private set; }
        public float F54 { get; private set; }
        public float F55 { get; private set; }

        public float F56 { get; private set; }
        public float F57 { get; private set; }
        public float F58 { get; private set; }
        public float F59 { get; private set; }
        public float F60 { get; private set; }

        public int I16 { get; private set; }
        public int I17 { get; private set; }
        public int I18 { get; private set; }
        public int I19 { get; private set; }
        public int I20 { get; private set; }

        public int I21 { get; private set; }

        public Globals(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            stream.Position += (3 * 4);
            this.ServerData = stream.ReadSerializedData<GlobalServerData>();

            stream.Position += 4;
            this.I0 = stream.ReadValueS32(); //32
            stream.Position += 12;
            this.StartLocationNames = new Dictionary<int, FileFormats.StartLocationName>();
            foreach (var startLocation in stream.ReadSerializedData<StartLocationName>())
                StartLocationNames.Add(startLocation.I0, startLocation);

            this.F0 = stream.ReadValueF32(); //56
            this.F1 = stream.ReadValueF32(); //60
            this.F2 = stream.ReadValueF32(); //64
            this.F3 = stream.ReadValueF32(); //68

            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.F4 = stream.ReadValueF32();
            this.F5 = stream.ReadValueF32();
            this.I3 = stream.ReadValueS32();
            this.F6 = stream.ReadValueF32();
            this.F7 = stream.ReadValueF32();
            this.F8 = stream.ReadValueF32();
            this.F9 = stream.ReadValueF32();
            this.F10 = stream.ReadValueF32();
            this.I4 = stream.ReadValueS32();
            this.I6 = new int[4];
            for (int i = 0; i < 4; i++)
                this.I6[i] = stream.ReadValueS32();
            stream.Position += 4;
            this.BannerParams = new BannerParams(stream);
            this.I5 = stream.ReadValueS32();
            this.I7 = stream.ReadValueS32();
            this.I8 = stream.ReadValueS32();
            this.I9 = stream.ReadValueS32();
            this.F11 = stream.ReadValueF32();
            this.F12 = stream.ReadValueF32();
            this.F13 = stream.ReadValueF32();
            this.F14 = stream.ReadValueF32();
            this.F15 = stream.ReadValueF32();
            this.F16 = stream.ReadValueF32();
            this.F17 = stream.ReadValueF32();
            this.F18 = stream.ReadValueF32();
            stream.Close();
        }
    }

    public class DifficultyTuningParams
    {
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public float F3 { get; private set; }
        public float F4 { get; private set; }
        public float F5 { get; private set; }
        public float F6 { get; private set; }
        public float F7 { get; private set; }
        public float F8 { get; private set; }
        public float F9 { get; private set; }

        public DifficultyTuningParams(MpqFileStream stream)
        {
            this.F0 = stream.ReadValueF32();
            this.F1 = stream.ReadValueF32();
            this.F2 = stream.ReadValueF32();
            this.F3 = stream.ReadValueF32();
            this.F4 = stream.ReadValueF32();
            this.F5 = stream.ReadValueF32();
            this.F6 = stream.ReadValueF32();
            this.F7 = stream.ReadValueF32();
            this.F8 = stream.ReadValueF32();
            this.F9 = stream.ReadValueF32();
        }
    }

    public class ActorGroup : ISerializableData
    {
        public int UHash { get; private set; }
        public string S0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.UHash = stream.ReadValueS32();
            this.S0 = stream.ReadString(64, true);
        }
    }

    public class StartLocationName : ISerializableData
    {
        public int I0 { get; private set; }
        public string S0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.S0 = stream.ReadString(64, true);
        }
    }

    public class GlobalScriptVariable : ISerializableData
    {
        public int UHash { get; private set; }
        public string S0 { get; private set; }
        public float F0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.UHash = stream.ReadValueS32();
            this.S0 = stream.ReadString(32, true);
            this.F0 = stream.ReadValueF32();
        }
    }

    public class GlobalServerData : ISerializableData
    {
        public Dictionary<int, ActorGroup> ActorGroups { get; private set; }
        public List<GlobalScriptVariable> ScriptGlobalVars { get; private set; }
        public DifficultyTuningParams[] TuningParams { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public float F3 { get; private set; }
        public float F4 { get; private set; }

        public float F5 { get; private set; }
        public float F6 { get; private set; }
        public float F7 { get; private set; }

        public int I0 { get; private set; }

        public float F8 { get; private set; }
        public float F9 { get; private set; }
        public float F10 { get; private set; }
        public float F11 { get; private set; }
        public float F12 { get; private set; }

        public float F13 { get; private set; }
        public float F14 { get; private set; }
        public float F15 { get; private set; }
        public float F16 { get; private set; }
        public float F17 { get; private set; }

        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }

        public float F18 { get; private set; }
        public float F19 { get; private set; }
        public float F20 { get; private set; }
        public float F21 { get; private set; }
        public float F22 { get; private set; }

        public float F23 { get; private set; }
        public float F24 { get; private set; }

        public int I4 { get; private set; }

        public float F25 { get; private set; }

        public int I5 { get; private set; }
        public int I6 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }

        public int I10 { get; private set; }
        public int I11 { get; private set; }
        public int I12 { get; private set; }
        public int I13 { get; private set; }

        public float F26 { get; private set; }
        public float F27 { get; private set; }
        public float[] F28 { get; private set; }
        public float F29 { get; private set; }
        public float F30 { get; private set; }

        public int I14 { get; private set; }
        public int I15 { get; private set; }
        public int I16 { get; private set; }

        public float F31 { get; private set; }
        public int I17 { get; private set; }

        public float F32 { get; private set; }
        public float F33 { get; private set; }
        public float F34 { get; private set; }
        public float F35 { get; private set; }
        public float F36 { get; private set; }

        public float F37 { get; private set; }
        public int I18 { get; private set; }

        public float F38 { get; private set; }
        public float F39 { get; private set; }
        public float F40 { get; private set; }
        public float F41 { get; private set; }

        public int I19 { get; private set; }

        public float F42 { get; private set; }
        public float F43 { get; private set; }
        public float F44 { get; private set; }
        public float F45 { get; private set; }
        public float F46 { get; private set; }

        public float[] F47 { get; private set; }

        public float F48 { get; private set; }
        public float F49 { get; private set; }
        public float F50 { get; private set; }
        public float F51 { get; private set; }
        public float F52 { get; private set; }

        public float F53 { get; private set; }
        public float F54 { get; private set; }
        public float F55 { get; private set; }

        public int I20 { get; private set; }
        public int I21 { get; private set; }
        public int I22 { get; private set; }
        public int I23 { get; private set; }
        public int I24 { get; private set; }

        public int I25 { get; private set; }

        public float F56 { get; private set; }
        public float F57 { get; private set; }
        public float F58 { get; private set; }
        public float F59 { get; private set; }
        public float F60 { get; private set; }

        public float F61 { get; private set; }

        public float[] F62 { get; private set; }

        public float F63 { get; private set; }
        public float F64 { get; private set; }

        public int I26 { get; private set; }
        public int I27 { get; private set; }
        public int I28 { get; private set; }
        public int I29 { get; private set; }
        public int I30 { get; private set; }

        public int I31 { get; private set; }
        public int I32 { get; private set; }
        public int I33 { get; private set; }
        public int I34 { get; private set; }
        public int I35 { get; private set; }

        public int I36 { get; private set; }
        public int I37 { get; private set; }
        public int I38 { get; private set; }
        public int I39 { get; private set; }
        public int I40 { get; private set; }

        public int I41 { get; private set; }
        public int I42 { get; private set; }
        public float F65 { get; private set; }
        public void Read(MpqFileStream stream)
        {
            this.ActorGroups = new Dictionary<int, FileFormats.ActorGroup>();
            foreach (var group in stream.ReadSerializedData<ActorGroup>()) //166
                this.ActorGroups.Add(group.UHash, group);

            stream.Position += 8;
            this.ScriptGlobalVars = stream.ReadSerializedData<GlobalScriptVariable>();
            stream.Position += 8;
            this.TuningParams = new DifficultyTuningParams[4];
            for (int i = 0; i < 4; i++)
                this.TuningParams[i] = new DifficultyTuningParams(stream);
            this.F0 = stream.ReadValueF32();
            this.F1 = stream.ReadValueF32();
            this.F2 = stream.ReadValueF32();
            this.F3 = stream.ReadValueF32();
            this.F4 = stream.ReadValueF32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.F5 = stream.ReadValueF32();
            this.F6 = stream.ReadValueF32();
            this.F7 = stream.ReadValueF32();
            this.F8 = stream.ReadValueF32();
            this.F20 = stream.ReadValueF32();
            this.F21 = stream.ReadValueF32();
            this.F22 = stream.ReadValueF32();
            this.I5 = stream.ReadValueS32();
            this.F23 = stream.ReadValueF32();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.I4 = stream.ReadValueS32();
            this.F9 = stream.ReadValueF32();
            this.F10 = stream.ReadValueF32();
            this.F11 = stream.ReadValueF32();
            this.F12 = stream.ReadValueF32();
            this.F13 = stream.ReadValueF32();
            this.F14 = stream.ReadValueF32();
            this.F15 = stream.ReadValueF32();
            this.F16 = stream.ReadValueF32();
            this.F17 = stream.ReadValueF32();
            this.F18 = stream.ReadValueF32();
            this.F28 = new float[17];
            for (var i = 0; i < 17; i++)
                this.F28[i] = stream.ReadValueF32();
        }
    }

    public class BannerParams
    {
        //Total Length: 232
        public List<BannerTexturePair> TexBackgrounds { get; private set; }
        public int I0 { get; private set; }
        public List<BannerTexturePair> TexPatterns { get; private set; }
        public List<BannerTexturePair> TexMainSigils { get; private set; }
        public List<BannerTexturePair> TexVariantSigils { get; private set; }
        public List<BannerTexturePair> TexSigilAccents { get; private set; }
        public List<BannerColorSet> ColorSets { get; private set; }
        public List<BannerSigilPlacement> SigilPlacements { get; private set; }
        public List<int> SNOActorBases { get; private set; }
        public List<int> SNOActorCaps { get; private set; }
        public List<int> SNOActorPoles { get; private set; }
        public List<int> SNOActorRibbons { get; private set; }
        public List<EpicBannerDescription> EpicBannerDescriptions { get; private set; }

        public BannerParams(MpqFileStream stream)
        {
            stream.Position += 8;
            this.TexBackgrounds = stream.ReadSerializedData<BannerTexturePair>();
            this.I0 = stream.ReadValueS32(); //16
            stream.Position += 12;
            this.TexPatterns = stream.ReadSerializedData<BannerTexturePair>();
            this.I0 = stream.ReadValueS32(); //40
            stream.Position += 12;
            this.TexMainSigils = stream.ReadSerializedData<BannerTexturePair>();
            stream.Position += 8;
            this.TexVariantSigils = stream.ReadSerializedData<BannerTexturePair>();
            this.I0 = stream.ReadValueS32(); //80
            stream.Position += 12;
            this.TexSigilAccents = stream.ReadSerializedData<BannerTexturePair>();
            this.I0 = stream.ReadValueS32(); //104
            stream.Position += 12;
            this.ColorSets = stream.ReadSerializedData<BannerColorSet>();
            stream.Position += 8;
            this.SigilPlacements = stream.ReadSerializedData<BannerSigilPlacement>();
            stream.Position += 8;
            this.SNOActorBases = stream.ReadSerializedInts();
            stream.Position += 8;
            this.SNOActorCaps = stream.ReadSerializedInts();
            stream.Position += 8;
            this.SNOActorPoles = stream.ReadSerializedInts();
            stream.Position += 8;
            this.SNOActorRibbons = stream.ReadSerializedInts();
            stream.Position += 8;
            this.EpicBannerDescriptions = stream.ReadSerializedData<EpicBannerDescription>();
            stream.Position += 8;
        }
    }

    public class BannerTexturePair : ISerializableData
    {
        public int SNOTexture { get; private set; }
        public int I0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOTexture = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            stream.Position += 4;
        }
    }

    public class BannerColorSet : ISerializableData
    {
        public RGBAColor[] Color { get; private set; }
        public string String1 { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.Color = new RGBAColor[2];
            for (int i = 0; i < 2; i++)
                this.Color[i] = new RGBAColor(stream);
            this.String1 = stream.ReadString(64, true);
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            stream.Position += 4;
        }
    }

    public class BannerSigilPlacement : ISerializableData
    {
        public string S0 { get; private set; }
        public int I0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.S0 = stream.ReadString(64, true);
            this.I0 = stream.ReadValueS32();
        }
    }

    public class EpicBannerDescription : ISerializableData
    {
        public int SNOBannerShape { get; private set; }
        public int SNOBannerBase { get; private set; }
        public int SNOBannerPole { get; private set; }
        public int I3 { get; private set; }
        public string S0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOBannerShape = stream.ReadValueS32();
            this.SNOBannerBase = stream.ReadValueS32();
            this.SNOBannerPole = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.S0 = stream.ReadString(128, true);
        }
    }

    public class AssetList : ISerializableData
    {
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
        }
    }
}
