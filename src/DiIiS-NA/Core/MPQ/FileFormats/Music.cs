//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Music)]
    public class Music : FileFormat
    {
        public Header Header { get; private set; }

        public Music()
        {

        }

        public Music(MpqFile file)
        {
            var stream = file.Open();

            this.Header = new Header(stream);


            stream.Close();
        }
    }
}
