//Blizzless Project 2022
using System.Collections.Generic;
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.SkillKit)]
    public class SkillKit : FileFormat
    {
        public Header Header { get; private set; }
        public List<TraitEntry> TraitEntries { get; private set; }
        public List<ActiveSkillEntry> ActiveSkillEntries { get; private set; }

        public SkillKit(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            stream.Position += 12;
            TraitEntries = stream.ReadSerializedData<TraitEntry>();
            stream.Position += 8;
            ActiveSkillEntries = stream.ReadSerializedData<ActiveSkillEntry>();

            stream.Close();
        }
    }

    public class TraitEntry : ISerializableData
    {
        public int SNOPower { get; private set; }
        public int Category { get; private set; }
        public int ReqLevel { get; private set; }
        public int I0 { get; private set; }
        public void Read(MpqFileStream stream)
        {
            SNOPower = stream.ReadValueS32();
            Category = stream.ReadValueS32();
            ReqLevel = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    public class ActiveSkillEntry : ISerializableData
    {
        public int SNOPower { get; private set; }
        public ActiveSkillCategory Category { get; private set; }
        public int SkillGroup { get; private set; }  // TODO: possible to make an enum for this, like Category has?
        public int ReqLevel { get; private set; }
        public int RuneNone_ReqLevel { get; private set; }
        public int RuneA_ReqLevel { get; private set; }
        public int RuneB_ReqLevel { get; private set; }
        public int RuneC_ReqLevel { get; private set; }
        public int RuneD_ReqLevel { get; private set; }
        public int RuneE_ReqLevel { get; private set; }
        public int I0 { get; private set; }
        public int[] I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            SNOPower = stream.ReadValueS32();
            Category = (ActiveSkillCategory)stream.ReadValueS32();
            SkillGroup = stream.ReadValueS32();
            ReqLevel = stream.ReadValueS32();
            RuneNone_ReqLevel = stream.ReadValueS32();
            RuneA_ReqLevel = stream.ReadValueS32();
            RuneB_ReqLevel = stream.ReadValueS32();
            RuneC_ReqLevel = stream.ReadValueS32();
            RuneD_ReqLevel = stream.ReadValueS32();
            RuneE_ReqLevel = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = new int[5];
            for (int i = 0; i < I1.Length; i++)
                I1[i] = stream.ReadValueS32();
        }
    }

    public enum ActiveSkillCategory
    {
        FuryGenerator = 0,
        FurySpender,
        Situational,
        Signature,
        Offensive,
        Utility,
        PhysicalRealm,
        SpiritRealm,
        Support,
        HatredGenerator,
        HatredSpender,
        Discipline,
        SpiritGenerator,
        SpiritSpender,
        Mantras,
    }
}
