using System.Collections.Generic;
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.QuestRange)]
    public class QuestRange : FileFormat
    {
        public Header Header { get; private set; }
        public int Pull { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public int I3 { get; private set; }
        public int I4 { get; private set; }

        public QuestTimeEntity[] Enitys { get; private set; }
        public int[] TestMass { get; private set; }


        public QuestRange(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream); //0
            Pull = stream.ReadValueS32(); //12 + 16 = 28
            I1 = stream.ReadValueS32(); //12 + 16 = 28
            I2 = stream.ReadValueS32(); //12 + 16 = 28
            I3 = stream.ReadValueS32(); //12 + 16 = 28
            I4 = stream.ReadValueS32(); //12 + 16 = 28
            Enitys = new QuestTimeEntity[20];
            for (int i = 0; stream.Position < stream.Length; i++)
            {
                Enitys[i] = new QuestTimeEntity(stream);
            }
            stream.Close();
        }
    }
    public class QuestTimeEntity
    {
        public QuestTime Start { get; private set; }
        public QuestTime End { get; private set; }

        public QuestTimeEntity(MpqFileStream stream)
        {
            Start = new QuestTime(stream);
            End = new QuestTime(stream);
        }
    }


    public class QuestTime
    {
        public int SNOQuest { get; private set; }
        public int StepID { get; private set; }

        public QuestTime(MpqFileStream stream)
        {
            SNOQuest = stream.ReadValueS32();
            StepID = stream.ReadValueS32();
        }

    }
}
