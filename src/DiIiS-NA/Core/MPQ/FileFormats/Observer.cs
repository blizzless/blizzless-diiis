//Blizzless Project 2022
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.Core.MPQ.FileFormats.Types;


namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Observer)]
    public class Observer : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }

        public float F0 { get; private set; }
        public float Angle0 { get; private set; }

        public float F1 { get; private set; }
        public float Velocity { get; private set; }
        public float F8 { get; private set; }

        public float Angle1 { get; private set; }
        public float Angle2 { get; private set; }

        public float F2 { get; private set; }

        public Vector3D V0 { get; private set; }
        public Vector3D V1 { get; private set; }

        public float F3 { get; private set; }
        public float F4 { get; private set; }
        public float F5 { get; private set; }
        public float F6 { get; private set; }
        public float F7 { get; private set; }

        public Observer(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.F0 = stream.ReadValueF32();
            this.Angle0 = stream.ReadValueF32();
            this.F1 = stream.ReadValueF32();
            this.Velocity = stream.ReadValueF32();
            this.F8 = stream.ReadValueF32();
            this.Angle1 = stream.ReadValueF32();
            this.Angle2 = stream.ReadValueF32();
            this.F2 = stream.ReadValueF32();
            this.V0 = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.V1 = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            this.F3 = stream.ReadValueF32();
            this.F4 = stream.ReadValueF32();
            this.F5 = stream.ReadValueF32();
            this.F6 = stream.ReadValueF32();
            this.F7 = stream.ReadValueF32();
            stream.Close();
        }
    }
}
