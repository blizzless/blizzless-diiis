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

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Encounter)]
    public class Encounter : FileFormat
    {
        public Header Header { get; private set; }
        public int SNOSpawn { get; private set; }
        public List<EncounterSpawnOptions> Spawnoptions = new List<EncounterSpawnOptions>();

        public Encounter(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.SNOSpawn = stream.ReadValueS32();
            stream.Position += (2 * 4);// pad 2 int
            this.Spawnoptions = stream.ReadSerializedData<EncounterSpawnOptions>();
            stream.Close();
        }
    }

    public class EncounterSpawnOptions : ISerializableData
    {
        public int SNOSpawn { get; set; }
        public int Probability { get; set; }
        public int I1 { get; set; }
        public int SNOCondition { get; set; }

        public void Read(MpqFileStream stream)
        {
            this.SNOSpawn = stream.ReadValueS32();
            this.Probability = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.SNOCondition = stream.ReadValueS32();
        }
    }
}
