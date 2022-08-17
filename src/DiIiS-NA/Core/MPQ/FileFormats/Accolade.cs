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
    [FileFormat(SNOGroup.Accolade)]
    public class Accolade : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }

        public Accolade(MpqFile file)
        {
            var stream = file.Open();

            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();

            stream.Close();
        }
    }
}
