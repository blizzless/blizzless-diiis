//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Condition)]
    public class Condition : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int[] Class { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }
        public int I4 { get; private set; }
        public int I5 { get; private set; }
        public LoreSubcondition[] LoreCondition { get; private set; }
        public QuestSubcondition[] QuestCondition { get; private set; }
        public int I6 { get; private set; }
        public int I7 { get; private set; }
        public int I8 { get; private set; }
        public ItemSubcondition[] ItemCondition { get; private set; }

        public int I9 { get; private set; }
        public int I10 { get; private set; }
        public int I11 { get; private set; }
        public int I12 { get; private set; }
        public int I13 { get; private set; }

        public int I14 { get; private set; }
        public int I15 { get; private set; }
        public int I16 { get; private set; }
        public int I17 { get; private set; }
        public int I18 { get; private set; }

        public int I19 { get; private set; }
        public int I20 { get; private set; }

        public int SNOCurrentWorld { get; private set; }
        public int SNOCurrentLevelArea { get; private set; }
        public int SNOQuestRange { get; private set; }
        public FollowerSubcondition FollowerCondition { get; private set; }
        public LabelSubcondition[] LabelCondition { get; private set; }
        public SkillSubcondition[] SkillCondition { get; private set; }

        public int I21 { get; private set; }
        public int I22 { get; private set; }
        public int I23 { get; private set; }
        public int I24 { get; private set; }
        public int I25 { get; private set; }

        public int I26 { get; private set; }
        public int I27 { get; private set; }
        public int I28 { get; private set; }
        public int I29 { get; private set; }

        public MonsterSubcondition[] MonsterCondition { get; private set; }
        public GameFlagSubcondition[] GameFlagCondition { get; private set; }
        public PlayerFlagSubcondition[] PlayerFlagCondition { get; private set; }
        public BuffSubcondition[] BuffSubCondition { get; private set; }

        public int I30 { get; private set; }
        public int I31 { get; private set; }
        public int I32 { get; private set; }


        public Condition(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream); //0
            //+16
            I0 = stream.ReadValueS32(); //12 + 16 = 28
            I1 = stream.ReadValueS32(); //32
            Class = new int[7]; //36
            for (int i = 0; i < 7; i++)
                Class[i] = stream.ReadValueS32();

            I2 = stream.ReadValueS32(); //48 + 16 = 64
            I3 = stream.ReadValueS32(); //68
            I4 = stream.ReadValueS32(); //72
            I5 = stream.ReadValueS32(); //76

            LoreCondition = new LoreSubcondition[3]; //80
            for (int i = 0; i < 3; i++)
                LoreCondition[i] = new LoreSubcondition(stream);

            QuestCondition = new QuestSubcondition[3]; //104
            for (int i = 0; i < 3; i++)
                QuestCondition[i] = new QuestSubcondition(stream);

            I6 = stream.ReadValueS32(); //152
            I7 = stream.ReadValueS32(); //156
            I8 = stream.ReadValueS32(); //160
            ItemCondition = new ItemSubcondition[3]; //164
            for (int i = 0; i < 3; i++)
                ItemCondition[i] = new ItemSubcondition(stream);

            I9 = stream.ReadValueS32();  //212
            I10 = stream.ReadValueS32(); //216
            I11 = stream.ReadValueS32(); //220
            I12 = stream.ReadValueS32(); //224
            I13 = stream.ReadValueS32(); //228

            I14 = stream.ReadValueS32(); //232
            I15 = stream.ReadValueS32(); //236
            I16 = stream.ReadValueS32(); //240
            stream.Position += 4;
            I17 = stream.ReadValueS32(); //248
            I18 = stream.ReadValueS32(); //252

            I19 = stream.ReadValueS32(); //256
            I20 = stream.ReadValueS32(); //260

            SNOCurrentWorld = stream.ReadValueS32(); //264
            SNOCurrentLevelArea = stream.ReadValueS32(); //268
            SNOQuestRange = stream.ReadValueS32(); //272
            FollowerCondition = new FollowerSubcondition(stream); //276

            LabelCondition = new LabelSubcondition[3]; //284
            for (int i = 0; i < 3; i++)
                LabelCondition[i] = new LabelSubcondition(stream);

            SkillCondition = new SkillSubcondition[3]; //308
            for (int i = 0; i < 3; i++)
                SkillCondition[i] = new SkillSubcondition(stream);


            I21 = stream.ReadValueS32(); //344
            I22 = stream.ReadValueS32(); //348
            I23 = stream.ReadValueS32(); //352
            I24 = stream.ReadValueS32(); //356
            I25 = stream.ReadValueS32(); //360

            I26 = stream.ReadValueS32(); //364
            I27 = stream.ReadValueS32(); //368
            I28 = stream.ReadValueS32(); //372
            I29 = stream.ReadValueS32(); //376

            MonsterCondition = new MonsterSubcondition[15]; //380
            for (int i = 0; i < 15; i++)
                MonsterCondition[i] = new MonsterSubcondition(stream);

            GameFlagCondition = new GameFlagSubcondition[3]; //440
            for (int i = 0; i < 3; i++)
                GameFlagCondition[i] = new GameFlagSubcondition(stream);

            PlayerFlagCondition = new PlayerFlagSubcondition[3]; //824
            for (int i = 0; i < 3; i++)
                PlayerFlagCondition[i] = new PlayerFlagSubcondition(stream);

            BuffSubCondition = new BuffSubcondition[3]; //1208
            for (int i = 0; i < 3; i++)
                BuffSubCondition[i] = new BuffSubcondition(stream);

            I30 = stream.ReadValueS32(); //1244
            I31 = stream.ReadValueS32(); //1248
            I32 = stream.ReadValueS32(); //1252

            stream.Close();
        }
    }

    public class LoreSubcondition
    {
        public int SNOLore { get; private set; }
        public int I0 { get; private set; }

        public LoreSubcondition(MpqFileStream stream)
        {
            SNOLore = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    public class QuestSubcondition
    {
        public int SNOQuest { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public QuestSubcondition(MpqFileStream stream)
        {
            SNOQuest = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
        }
    }

    public class ItemSubcondition
    {
        public int ItemGBId { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public ItemSubcondition(MpqFileStream stream)
        {
            ItemGBId = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
        }
    }

    public class FollowerSubcondition
    {
        public FollowerType Type { get; private set; }
        public int I0 { get; private set; }

        public FollowerSubcondition(MpqFileStream stream)
        {
            Type = (FollowerType)stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    public class LabelSubcondition
    {
        public int LabelGBId { get; private set; }
        public int I0 { get; private set; }

        public LabelSubcondition(MpqFileStream stream)
        {
            LabelGBId = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
        }
    }

    public class SkillSubcondition
    {
        public int SNOSkillPower { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public SkillSubcondition(MpqFileStream stream)
        {
            SNOSkillPower = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
        }
    }

    public class MonsterSubcondition
    {
        public int SNOMonsterActor { get; private set; }

        public MonsterSubcondition(MpqFileStream stream)
        {
            SNOMonsterActor = stream.ReadValueS32();
        }
    }

    public class GameFlagSubcondition
    {
        public string S0 { get; private set; }

        public GameFlagSubcondition(MpqFileStream stream)
        {
            S0 = stream.ReadString(128, true);
        }
    }

    public class PlayerFlagSubcondition
    {
        public string S0 { get; private set; }

        public PlayerFlagSubcondition(MpqFileStream stream)
        {
            S0 = stream.ReadString(128, true);
        }
    }

    public class BuffSubcondition
    {
        public int SNOPower { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public BuffSubcondition(MpqFileStream stream)
        {
            SNOPower = stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
        }
    }

    public enum FollowerType
    {
        None = 0,
        Templar,
        Scoundrel,
        Enchantress,
    }
}
