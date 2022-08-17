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
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.EffectItems = stream.ReadSerializedData<EffectItem>();
            this.EffectItemsCount = stream.ReadValueS32();
            stream.Position += 12; // pad 1
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            this.I4 = stream.ReadValueS32();
            this.SnoPower = stream.ReadValueS32();

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
            this.Weight = stream.ReadValueS32();
            this.Hash = stream.ReadString(64, true);
            this.TriggeredEvent.Read(stream);
        }
    }
}
