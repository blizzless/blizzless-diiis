//Blizzless Project 2022
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
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
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.I4 = stream.ReadValueS32();
            this.I5 = stream.ReadValueS32();
            this.I6 = stream.ReadValueS32();
            this.I7 = stream.ReadValueS32();
            this.I8 = stream.ReadValueS32();
            this.I9 = stream.ReadValueS32();
            this.I10 = stream.ReadValueS32();
            this.I11 = stream.ReadValueS32();
            this.F0 = stream.ReadValueF32();
            this.SNOQuestRange = stream.ReadValueS32();
            this.Worlds = new int[4];
            for (int i = 0; i < 4; i++)
                this.Worlds[i] = stream.ReadValueS32();
            this.Scripts = new int[3];
            for (int i = 0; i < 3; i++)
                this.Scripts[i] = stream.ReadValueS32();
            this.LevelAreaSNO = stream.ReadValueS32();
            this.ActorSNO = stream.ReadValueS32();
            stream.Close();
        }
    }
}
