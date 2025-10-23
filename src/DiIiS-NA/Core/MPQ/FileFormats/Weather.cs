using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Weather)]
    public class Weather : FileFormat
    {
        public Header Header { get; private set; }
        /*
        public int x00C { get; private set; }
        public int Time { get; private set; }  // DT_TIME
        public VelocityVectorPath x018_VelocityVectorPath { get; private set; }
        public VelocityVectorPath x048_VelocityVectorPath { get; private set; }
        public int x078_Time { get; private set; }  // DT_TIME
        public FloatPath x080_FloatPath { get; private set; }
        public int ParticleSNO { get; private set; } //SNO
        public int ParticleSNO1 { get; private set; } //SNO
        public int SoundSNO { get; private set; } //SNO
        public int ActorSNO { get; private set; } //SNO
        public float x0C0 { get; private set; }
        public int x0C4_Time { get; private set; }  // DT_TIME
        public int x0C8_Time { get; private set; }  // DT_TIME
        public int x0CC_Time { get; private set; }  // DT_TIME
        public int LightSNO { get; private set; } //SNO
        public int EffectGroupSNO { get; private set; } //SNO
        public int SoundSNO1 { get; private set; } //SNO
        public float x0DC { get; private set; }
        public float x0E0 { get; private set; }
        public float x0E4 { get; private set; }
        public WeatherStateParams x0E8_WeatherStateParams { get; private set; }
        public WeatherStateParams x12C_WeatherStateParams { get; private set; }
        public float x170 { get; private set; }
        public ColorCorrectionParams x174_ColorCorrectionParams { get; private set; }
        public int[] SNOs { get; private set; } //public SerializeData x190_SerializeData { get; private set; }
        public int x198 { get; private set; }
        public int x19C { get; private set; }
        */

        public Weather() { }

        public Weather(MpqFile file)
        {
            var stream = file.Open();

            Header = new Header(stream);


            stream.Close();
        }
    }
}
