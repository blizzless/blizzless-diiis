//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Adventure)]
    public class Adventure : FileFormat
    {
        public Header Header { get; private set; }
        public int ActorSNO { get; private set; }
        public float F0 { get; private set; }
        public float Angle0 { get; private set; }
        public float Angle1 { get; private set; }
        public float Angle2 { get; private set; }
        public int MarkerSetSNO { get; private set; }

        public Adventure(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.ActorSNO = stream.ReadValueS32();
            this.F0 = stream.ReadValueF32();
            this.Angle0 = stream.ReadValueF32();
            this.Angle1 = stream.ReadValueF32();
            this.Angle2 = stream.ReadValueF32();
            this.MarkerSetSNO = stream.ReadValueS32();
            stream.Close();
        }
    }
}
