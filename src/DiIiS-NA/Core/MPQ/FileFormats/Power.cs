//Blizzless Project 2022
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using System.Collections.Generic;
using DiIiS_NA.GameServer.Core.Types.TagMap;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Power)]
    public class Power : FileFormat
    {
        public Header Header { get; private set; }
        public string LuaName { get; private set; }
        public PowerDef Powerdef { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public string Chararray2 { get; private set; }
        public List<ScriptFormulaDetails> ScriptFormulaDetails = new List<ScriptFormulaDetails>();
        public int i3 { get; private set; }
        public byte[] b0 { get; private set; }
        public int SNOQuestMetaData { get; private set; }

        public string CompiledScript { get; private set; }

        public Power(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            LuaName = stream.ReadString(64, true); //28
            stream.Position += 4; // 
            Powerdef = new PowerDef(stream); //108
            stream.Position = 824; // Seems like theres a bit of a gap - DarkLotus
            I0 = stream.ReadValueS32(); //824
            I1 = stream.ReadValueS32(); //828
            Chararray2 = stream.ReadString(256, true); //832
            //ScriptFormulaCount = stream.ReadValueS32();
            ScriptFormulaDetails = stream.ReadSerializedData<ScriptFormulaDetails>(); //1088
            stream.Position += (2 * 4);
            i3 = stream.ReadValueS32(); //1104
            stream.Position += 4;
            CompiledScript = System.Text.Encoding.ASCII.GetString(stream.ReadSerializedByteArray());// b0 = stream.ReadSerializedByteArray(); //1112
            stream.Position += (2 * 4);
            SNOQuestMetaData = stream.ReadValueS32(); //1228
            stream.Position += 4;
            stream.Close();

            //CompiledScript = System.Text.Encoding.ASCII.GetString(stream.ReadSerializedByteArray());
        }
    }

    public class PowerDef
    {
        public TagMap TagMap { get; private set; }
        public TagMap GeneralTagMap { get; private set; }
        public TagMap PVPGeneralTagMap { get; private set; }
        public TagMap ContactTagMap0 { get; private set; }
        public TagMap ContactTagMap1 { get; private set; }
        public TagMap ContactTagMap2 { get; private set; }
        public TagMap ContactTagMap3 { get; private set; }
        public TagMap PVPContactTagMap0 { get; private set; }
        public TagMap PVPContactTagMap1 { get; private set; }
        public TagMap PVPContactTagMap2 { get; private set; }
        public TagMap PVPContactTagMap3 { get; private set; }
        public int I0 { get; private set; }
        public ActorCollisionFlags ActColFlags1 { get; private set; }
        public ActorCollisionFlags ActColFlags2 { get; private set; }
        public List<BuffDef> Buffs = new List<BuffDef>(); //32

        public PowerDef(MpqFileStream stream)
        {
            TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);
            GeneralTagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);
            PVPGeneralTagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);
            ContactTagMap0 = stream.ReadSerializedItem<TagMap>();
            ContactTagMap1 = stream.ReadSerializedItem<TagMap>();
            ContactTagMap2 = stream.ReadSerializedItem<TagMap>();
            ContactTagMap3 = stream.ReadSerializedItem<TagMap>();
            stream.Position += (8 * 4);
            PVPContactTagMap0 = stream.ReadSerializedItem<TagMap>();
            PVPContactTagMap1 = stream.ReadSerializedItem<TagMap>();
            PVPContactTagMap2 = stream.ReadSerializedItem<TagMap>();
            PVPContactTagMap3 = stream.ReadSerializedItem<TagMap>();
            stream.Position += (8 * 4);
            I0 = stream.ReadValueS32();
            ActColFlags1 = new ActorCollisionFlags(stream);
            ActColFlags2 = new ActorCollisionFlags(stream);
            stream.Position += 4;
            for (int i = 0; i < 32; i++)
            {
                Buffs.Add(new BuffDef(stream));
                stream.Position += (2 * 4);
            }
        }

    }


    public class BuffDef
    {
        public List<int> BuffFilterPowers = new List<int>();

        public BuffDef(MpqFileStream stream)
        {
            BuffFilterPowers = stream.ReadSerializedInts();
        }
    }
}
