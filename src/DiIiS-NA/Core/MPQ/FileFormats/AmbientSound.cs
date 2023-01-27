//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.AmbientSound)]
    public class AmbientSound : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int SoundSNO00 { get; private set; }
        public RandomAmbientSoundParams RandomAmbientSoundParams { get; private set; }
        public int SoundSNO01 { get; private set; }
        public float Time01 { get; private set; }
        public float Time02 { get; private set; }
        public string Text { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public float F3 { get; private set; }
        public float F4 { get; private set; }

        public AmbientSound(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.SoundSNO00 = stream.ReadValueS32();
            this.RandomAmbientSoundParams = new RandomAmbientSoundParams(stream);
            //stream.Position = 76;
            this.SoundSNO01 = stream.ReadValueS32();
            Time01 = stream.ReadValueF32();
            Time02 = stream.ReadValueF32();
            this.Text = stream.ReadString(64);
            F0 = stream.ReadValueF32();
            F1 = stream.ReadValueF32();
            F2 = stream.ReadValueF32();
            F3 = stream.ReadValueF32();
            F4 = stream.ReadValueF32();

            stream.Close();
        }
    }

    public class RandomAmbientSoundParams
    {
        public int SNO { get; private set; }
        public float F0 { get; private set; }
        public float F1 { get; private set; }
        public float Time01 { get; private set; }
        public float Time02 { get; private set; }

        public RandomAmbientSoundParams(MpqFileStream stream)
        {
            SNO = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
            F1 = stream.ReadValueF32();
            Time01 = stream.ReadValueF32();
            Time02 = stream.ReadValueF32();
        }
    }
}
