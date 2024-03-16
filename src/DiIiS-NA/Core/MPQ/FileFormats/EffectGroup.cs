using System.Collections.Generic;
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.EffectGroup)]
    public class EffectGroup : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int EffectItemsCount { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int SnoPower { get; private set; }

        public int[] I5 { get; private set; }
        public List<EffectItem> EffectItems = new List<EffectItem>();

        public EffectGroup(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            I0 = stream.ReadValueS32();
            EffectItems = stream.ReadSerializedData<EffectItem>();
            EffectItemsCount = stream.ReadValueS32();
            stream.Position += 12; // pad 1
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            SnoPower = stream.ReadValueS32();

            I5 = new int[16];
            for (int i = 0; i < I5.Length; i++)
                I5[i] = stream.ReadValueS32();
            //SnoPower = I5[1];
            stream.Close();
        }
    }
    public class EffectItem : ISerializableData
    {
        public int Weight { get; private set; }
        public string Hash { get; private set; } // 64
        public MsgTriggeredEvent TriggeredEvent = new MsgTriggeredEvent();

        public void Read(MpqFileStream stream)
        {
            Weight = stream.ReadValueS32();
            Hash = stream.ReadString(64, true);
            TriggeredEvent.Read(stream);
        }
    }
}
