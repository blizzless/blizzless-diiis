//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.BossEncounter)]
    public class BossEncounter : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int I5 { get; private set; }
        public int I6 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }
        public int I10 { get; private set; }
        public int I11 { get; private set; }
        public float F0 { get; private set; }
        public int SNOQuestRange { get; private set; }
        public int[] Worlds { get; private set; }
        public int[] Scripts { get; private set; }
        public int LevelAreaSNO { get; private set; }
        public int ActorSNO { get; private set; }

        public BossEncounter(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            I5 = stream.ReadValueS32();
            I6 = stream.ReadValueS32();
            I7 = stream.ReadValueS32();
            I8 = stream.ReadValueS32();
            I9 = stream.ReadValueS32();
            I10 = stream.ReadValueS32();
            I11 = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            SNOQuestRange = stream.ReadValueS32();
            Worlds = new int[4];
            for (int i = 0; i < 4; i++)
                Worlds[i] = stream.ReadValueS32();
            Scripts = new int[3];
            for (int i = 0; i < 3; i++)
                Scripts[i] = stream.ReadValueS32();
            LevelAreaSNO = stream.ReadValueS32();
            ActorSNO = stream.ReadValueS32();
            stream.Close();
        }
    }
}
