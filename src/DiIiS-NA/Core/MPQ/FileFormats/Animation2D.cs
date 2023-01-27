//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Animation2D)]
    public class Animation2D : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public FrameAnim FrameAnim { get; private set; }
        public int SNOSound { get; private set; }
        public int I2 { get; private set; }
        public Anim2DFrame Anim2DFrame { get; private set; }

        public Animation2D(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.FrameAnim = new FrameAnim(stream);
            this.SNOSound = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.Anim2DFrame = new Anim2DFrame(stream);

            stream.Close();
        }
    }

    public class FrameAnim
    {
        public int I0 { get; private set; }
        public float Velocity0 { get; private set; }
        public float Velocity1 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public FrameAnim(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.Velocity0 = stream.ReadValueF32();
            this.Velocity1 = stream.ReadValueF32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
        }
    }
    public class Anim2DFrame
    {
        public string Text { get; private set; }
        public DT_RGBACOLOR DT_RGBACOLOR { get; private set; }

        public Anim2DFrame(MpqFileStream stream)
        {
            this.Text = stream.ReadString(64);
            DT_RGBACOLOR = new DT_RGBACOLOR(stream);
        }
    }
    public class DT_RGBACOLOR
    {
        public byte B0 { get; private set; }
        public byte B1 { get; private set; }
        public byte B2 { get; private set; }
        public byte B3 { get; private set; }

        public DT_RGBACOLOR(MpqFileStream stream)
        {
            this.B0 = stream.ReadValueU8();
            this.B1 = stream.ReadValueU8();
            this.B2 = stream.ReadValueU8();
            this.B3 = stream.ReadValueU8();
        }
    }
}
