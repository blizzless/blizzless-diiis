//Blizzless Project 2022
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
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
            this.Header = new Header(stream); //0
            this.Pull = stream.ReadValueS32(); //12 + 16 = 28
            this.I1 = stream.ReadValueS32(); //12 + 16 = 28
            this.I2 = stream.ReadValueS32(); //12 + 16 = 28
            this.I3 = stream.ReadValueS32(); //12 + 16 = 28
            this.I4 = stream.ReadValueS32(); //12 + 16 = 28
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
            this.SNOQuest = stream.ReadValueS32();
            this.StepID = stream.ReadValueS32();
        }

    }
}
