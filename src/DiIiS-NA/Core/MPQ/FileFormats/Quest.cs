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
    [FileFormat(SNOGroup.Quest)]
    public class Quest : FileFormat
    {
        public Header Header { get; private set; }
        public QuestType QuestType { get; private set; }
        public int NumberOfSteps { get; private set; }
        public int NumberOfCompletionSteps { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int I5 { get; private set; }
        public QuestUnassignedStep QuestUnassignedStep { get; private set; }
        public List<QuestStep> QuestSteps { get; private set; }
        public List<QuestCompletionStep> QuestCompletionSteps { get; private set; }
        public int[] SNOs { get; private set; }
        public int WorldSNO { get; private set; }
        public QuestMode QuestMode { get; private set; }
        public BountyData BountyData0 { get; private set; }

        public Quest(MpqFile file)
        {
            MpqFileStream stream = file.Open();

            Header = new Header(stream);
            //+16

            QuestType = (QuestType)stream.ReadValueS32(); //12 - 28
            NumberOfSteps = stream.ReadValueS32(); //16 - 32
            NumberOfCompletionSteps = stream.ReadValueS32(); //20 - 36
            I2 = stream.ReadValueS32(); //24 - 40
            I3 = stream.ReadValueS32(); //28 - 44
            I4 = stream.ReadValueS32(); //32 - 48
            I5 = stream.ReadValueS32(); //36 - 52

            QuestUnassignedStep = new QuestUnassignedStep(stream); //40 - 56
            stream.Position += 8;
            QuestSteps = stream.ReadSerializedData<QuestStep>(); //88 - 104
            stream.Position += 8;
            QuestCompletionSteps = stream.ReadSerializedData<QuestCompletionStep>(); //104 - 120

            SNOs = new int[18]; //128
            for (int i = 0; i < SNOs.Length; i++)
                SNOs[i] = stream.ReadValueS32();
            WorldSNO = stream.ReadValueS32(); //184 - 200
            QuestMode = (QuestMode)stream.ReadValueS32(); //188 - 204
            BountyData0 = new BountyData(stream); //192 - 208

            stream.Close();
        }
    }


    public interface IQuestStep
    {
        int ID { get; }
        List<QuestStepObjectiveSet> StepObjectiveSets { get; }
    }

    public class QuestUnassignedStep : IQuestStep
    {
        public int ID { get; private set; }
        public int I0 { get; private set; }
        public List<QuestStepObjectiveSet> StepObjectiveSets { get; private set; }
        public List<QuestStepFailureConditionSet> StepFailureConditionSets { get; private set; }

        public QuestUnassignedStep(MpqFileStream stream)
        {
            //+56

            ID = stream.ReadValueS32(); //0 - 56
            I0 = stream.ReadValueS32(); //4 - 60
            stream.Position += (2 * 4);
            StepObjectiveSets = stream.ReadSerializedData<QuestStepObjectiveSet>(); //16 - 72
            stream.Position += (2 * 4);
            StepFailureConditionSets = stream.ReadSerializedData<QuestStepFailureConditionSet>(); //32 - 88
        }
    }

    public class QuestStepObjectiveSet : ISerializableData
    {
        public int FollowUpStepID { get; private set; }
        public List<QuestStepObjective> StepObjectives { get; private set; }

        public void Read(MpqFileStream stream)
        {
            FollowUpStepID = stream.ReadValueS32();
            stream.Position += (3 * 4);
            StepObjectives = stream.ReadSerializedData<QuestStepObjective>();
        }
    }

    public class QuestStepObjective : ISerializableData
    {
        public int I0 { get; private set; }
        public QuestStepObjectiveType ObjectiveType { get; private set; }
        public int I2 { get; private set; }
        public int CounterTarget { get; private set; }
        public SNOHandle SNOName1 { get; private set; }
        public SNOHandle SNOName2 { get; private set; }
        public int GBID1 { get; private set; }
        public int GBID2 { get; private set; }
        public string Group1Name { get; private set; }
        public string Unknown2 { get; private set; }
        public int I4 { get; private set; }              // min = 0, max = 1 unless i know what it is im not making it a bool
        public int I5 { get; private set; }
        public int GBIDItemToShow { get; private set; }

        public void Read(MpqFileStream stream)
        {
            //352
            I0 = stream.ReadValueS32();
            ObjectiveType = (QuestStepObjectiveType)stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            CounterTarget = stream.ReadValueS32();
            SNOName1 = new SNOHandle(stream);
            SNOName2 = new SNOHandle(stream);
            GBID1 = stream.ReadValueS32();
            GBID2 = stream.ReadValueS32(); //36
            stream.Position += 8;
            Group1Name = stream.ReadString(8, true); //48
            stream.Position += 8;
            Unknown2 = stream.ReadString(8, true); //64
            I4 = stream.ReadValueS32(); //72
            I5 = stream.ReadValueS32(); //76
            GBIDItemToShow = stream.ReadValueS32(); //80
            stream.Position += 4;
            //440
        }
    }

    public class QuestStepFailureConditionSet : ISerializableData
    {
        public List<QuestStepFailureCondition> QuestStepFailureConditions { get; private set; }

        public void Read(MpqFileStream stream)
        {
            stream.Position += 8;
            QuestStepFailureConditions = stream.ReadSerializedData<QuestStepFailureCondition>();
        }
    }

    public class QuestStepFailureCondition : ISerializableData
    {
        public QuestStepFailureConditionType ConditionType { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public SNOHandle SNOName1 { get; private set; }
        public SNOHandle SNOName2 { get; private set; }
        public int GBID1 { get; private set; }
        public int GBID2 { get; private set; }
        public string Unknown1 { get; private set; }
        public string Unknown2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            ConditionType = (QuestStepFailureConditionType)stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            SNOName1 = new SNOHandle(stream);
            SNOName2 = new SNOHandle(stream);
            GBID1 = stream.ReadValueS32();
            GBID2 = stream.ReadValueS32(); //32
            //36
            stream.Position += 12;
            Unknown1 = stream.ReadString(8, true); //48
            stream.Position += 8;
            Unknown2 = stream.ReadString(8, true); //64
        }
    }

    public class QuestLevelRange
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public QuestLevelRange(MpqFileStream stream)
        {
            Min = stream.ReadValueS32();
            Max = stream.ReadValueS32();
        }
    }

    public class BountyData
    {
        public ActT ActData { get; set; }
        public BountyType Type { get; set; }
        public int I0 { get; private set; }
        public int LeveAreaSNO0 { get; private set; }
        public int WorldSNO0 { get; private set; }
        public int QuestSNO0 { get; private set; }
        public int WorldSNO1 { get; private set; }
        public int ActorSNO0 { get; private set; }
        public int WorldSNO2 { get; private set; }
        public int LeveAreaSNO1 { get; private set; }
        public int SceneSNO0 { get; private set; }
        public int WorldSNO3 { get; private set; }
        public int LabelsGBID0 { get; private set; }
        public int AdventureSNO0 { get; private set; }
        public int WorldSNO4 { get; private set; }
        public int LeveAreaSNO2 { get; private set; }
        public int EncounterSNO { get; private set; }
        public int SceneSNO1 { get; private set; }
        public int WorldSNO5 { get; private set; }
        public int LabelsGBID1 { get; private set; }
        public int SNO { get; private set; }
        public int BossEncounterSNO { get; private set; }
        public float F0 { get; private set; }

        public BountyData(MpqFileStream stream)
        {
            ActData = (ActT)stream.ReadValueS32();
            Type = (BountyType)stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            LeveAreaSNO0 = stream.ReadValueS32();
            WorldSNO0 = stream.ReadValueS32();
            QuestSNO0 = stream.ReadValueS32();
            WorldSNO1 = stream.ReadValueS32();
            ActorSNO0 = stream.ReadValueS32();
            WorldSNO2 = stream.ReadValueS32();
            LeveAreaSNO1 = stream.ReadValueS32();
            SceneSNO0 = stream.ReadValueS32();
            WorldSNO3 = stream.ReadValueS32();
            LabelsGBID0 = stream.ReadValueS32();
            AdventureSNO0 = stream.ReadValueS32();
            WorldSNO4 = stream.ReadValueS32();
            LeveAreaSNO2 = stream.ReadValueS32();
            EncounterSNO = stream.ReadValueS32();
            SceneSNO1 = stream.ReadValueS32();
            WorldSNO5 = stream.ReadValueS32();
            LabelsGBID1 = stream.ReadValueS32();
            SNO = stream.ReadValueS32();
            BossEncounterSNO = stream.ReadValueS32();
            F0 = stream.ReadValueF32();
        }

        public enum ActT
        {
            Invalid = -1,
            A1 = 0,
            A2 = 100,
            A3 = 200,
            A4 = 300,
            A5 = 400,
            OpenWorld = 3000,
            Test = 1000
        }
        public enum Mode
        {
            NoItem = 0,
            SharedRecipe = 1,
            ClassRecipe = 2,
            TreasureClass = 3
        }

        public enum BountyType
        {
            None = -1,
            KillUnique = 0,
            KillBoss = 1,
            CompleteEvent = 2,
            ClearDungeon = 3
        }
    }

    public class QuestStep : ISerializableData, IQuestStep
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public int I1 { get; private set; }
        public Enum1 Enum1 { get; private set; }
        public int[] SNORewardRecipe = new int[7];
        public int SNORewardTreasureClass { get; private set; }

        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public Enum1 Enum2 { get; private set; }
        public int[] SNOReplayRewardRecipe = new int[7];
        public int SNOReplayRewardTreasureClass { get; private set; }

        public int I5 { get; private set; }
        public int I6 { get; private set; }
        public int SNOPowerGranted { get; private set; }
        public int[] SNOWaypointLevelAreas = new int[2];

        public List<QuestStepObjectiveSet> StepObjectiveSets { get; private set; }
        public List<QuestStepBonusObjectiveSet> StepBonusObjectiveSets { get; private set; }
        public List<QuestStepFailureConditionSet> StepFailureConditionSets { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Name = stream.ReadString(16, true); //+600 - 
            ID = stream.ReadValueS32(); //616
            I1 = stream.ReadValueS32(); //620
            Enum1 = (Enum1)stream.ReadValueS32();
            for (int i = 0; i < SNORewardRecipe.Length; i++)
                SNORewardRecipe[i] = stream.ReadValueS32();
            SNORewardTreasureClass = stream.ReadValueS32(); //656

            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            Enum2 = (Enum1)stream.ReadValueS32(); //68
            for (int i = 0; i < SNOReplayRewardRecipe.Length; i++)
                SNOReplayRewardRecipe[i] = stream.ReadValueS32();
            SNOReplayRewardTreasureClass = stream.ReadValueS32(); //700

            I5 = stream.ReadValueS32();
            I6 = stream.ReadValueS32();
            SNOPowerGranted = stream.ReadValueS32();
            for (int i = 0; i < SNOWaypointLevelAreas.Length; i++)
                SNOWaypointLevelAreas[i] = stream.ReadValueS32();

            stream.Position += 4;      // unnacounted for in the xml
            stream.Position += 8;
            StepObjectiveSets = stream.ReadSerializedData<QuestStepObjectiveSet>(); //736
            stream.Position += 8;
            StepBonusObjectiveSets = stream.ReadSerializedData<QuestStepBonusObjectiveSet>();
            stream.Position += 8;
            StepFailureConditionSets = stream.ReadSerializedData<QuestStepFailureConditionSet>();
        }
    }

    public class QuestStepBonusObjectiveSet : ISerializableData
    {
        //public int[] I0 = new int[4];
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int I5 { get; private set; }
        public List<QuestStepObjective> StepBonusObjectives { get; private set; }

        public void Read(MpqFileStream stream)
        {
            //for (int i = 0; i < I0.Length; i++)
            //    I0[i] = stream.ReadValueS32();

            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            I5 = stream.ReadValueS32();

            stream.Position += 12;
            StepBonusObjectives = stream.ReadSerializedData<QuestStepObjective>();
        }
    }

    public class QuestCompletionStep : ISerializableData, IQuestStep
    {
        public string Unknown { get; private set; }
        public int ID { get; private set; }
        public int I2 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Unknown = stream.ReadString(16, true);
            ID = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
        }

        public List<QuestStepObjectiveSet> StepObjectiveSets
        {
            get { return new List<QuestStepObjectiveSet>(); }
        }
    }


    public enum Enum1
    {
        NoItem = 0,
        SharedRecipe = 1,
        ClassRecipe = 2,
        TreasureClass = 3
    }

    public enum QuestStepFailureConditionType
    {
        MonsterDied = 0,
        PlayerDied = 1,
        ActorDied = 2,
        TimedEventExpired = 3,
        ItemUsed = 4,
        GameFlagSet = 5,
        PlayerFlagSet = 6,
        EventReceived = 7
    }

    public enum QuestStepObjectiveType
    {
        HadConversation = 0,
        PossessItem = 1,
        KillMonster = 2,
        InteractWithActor = 3,
        EnterLevelArea = 4,
        EnterScene = 5,
        EnterWorld = 6,
        EnterTrigger = 7,
        CompleteQuest = 8,
        PlayerFlagSet = 9,
        TimedEventExpired = 10,
        KillGroup = 11,
        GameFlagSet = 12,
        EventReceived = 13,
        MonsterFromGroup = 14,
        MonsterFromFamily = 15,
        KillElite = 16,
        KillAny = 17,
        KillAll = 18
    }

    public enum QuestMode
    {
        None = -1,
        TimedDungeon = 0,
        WaveFight = 1,
        Horde = 2,
        Zapper = 3,
        GoblinHunt = 4
    }

    public enum QuestType
    {
        MainQuest = 0,
        Event = 2,
        Challenge = 4,
        Bounty = 5,
        HoradricQuest = 6,
        SetDungeon = 7,
        SetDungeonBonus = 8,
        SetDungeonMastery = 9,
        SetDungeonTracker = 10
    }
}
