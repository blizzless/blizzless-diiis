//Blizzless Project 2022
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
            this.Header = new Header(stream); //0
            //+16
            this.I0 = stream.ReadValueS32(); //12 + 16 = 28
            this.I1 = stream.ReadValueS32(); //32
            this.Class = new int[7]; //36
            for (int i = 0; i < 7; i++)
                this.Class[i] = stream.ReadValueS32();

            this.I2 = stream.ReadValueS32(); //48 + 16 = 64
            this.I3 = stream.ReadValueS32(); //68
            this.I4 = stream.ReadValueS32(); //72
            this.I5 = stream.ReadValueS32(); //76

            this.LoreCondition = new LoreSubcondition[3]; //80
            for (int i = 0; i < 3; i++)
                this.LoreCondition[i] = new LoreSubcondition(stream);

            this.QuestCondition = new QuestSubcondition[3]; //104
            for (int i = 0; i < 3; i++)
                this.QuestCondition[i] = new QuestSubcondition(stream);

            this.I6 = stream.ReadValueS32(); //152
            this.I7 = stream.ReadValueS32(); //156
            this.I8 = stream.ReadValueS32(); //160
            this.ItemCondition = new ItemSubcondition[3]; //164
            for (int i = 0; i < 3; i++)
                this.ItemCondition[i] = new ItemSubcondition(stream);

            this.I9 = stream.ReadValueS32();  //212
            this.I10 = stream.ReadValueS32(); //216
            this.I11 = stream.ReadValueS32(); //220
            this.I12 = stream.ReadValueS32(); //224
            this.I13 = stream.ReadValueS32(); //228

            this.I14 = stream.ReadValueS32(); //232
            this.I15 = stream.ReadValueS32(); //236
            this.I16 = stream.ReadValueS32(); //240
            stream.Position += 4;
            this.I17 = stream.ReadValueS32(); //248
            this.I18 = stream.ReadValueS32(); //252

            this.I19 = stream.ReadValueS32(); //256
            this.I20 = stream.ReadValueS32(); //260

            this.SNOCurrentWorld = stream.ReadValueS32(); //264
            this.SNOCurrentLevelArea = stream.ReadValueS32(); //268
            this.SNOQuestRange = stream.ReadValueS32(); //272
            this.FollowerCondition = new FollowerSubcondition(stream); //276

            this.LabelCondition = new LabelSubcondition[3]; //284
            for (int i = 0; i < 3; i++)
                this.LabelCondition[i] = new LabelSubcondition(stream);

            this.SkillCondition = new SkillSubcondition[3]; //308
            for (int i = 0; i < 3; i++)
                this.SkillCondition[i] = new SkillSubcondition(stream);


            this.I21 = stream.ReadValueS32(); //344
            this.I22 = stream.ReadValueS32(); //348
            this.I23 = stream.ReadValueS32(); //352
            this.I24 = stream.ReadValueS32(); //356
            this.I25 = stream.ReadValueS32(); //360

            this.I26 = stream.ReadValueS32(); //364
            this.I27 = stream.ReadValueS32(); //368
            this.I28 = stream.ReadValueS32(); //372
            this.I29 = stream.ReadValueS32(); //376

            this.MonsterCondition = new MonsterSubcondition[15]; //380
            for (int i = 0; i < 15; i++)
                this.MonsterCondition[i] = new MonsterSubcondition(stream);

            this.GameFlagCondition = new GameFlagSubcondition[3]; //440
            for (int i = 0; i < 3; i++)
                this.GameFlagCondition[i] = new GameFlagSubcondition(stream);

            this.PlayerFlagCondition = new PlayerFlagSubcondition[3]; //824
            for (int i = 0; i < 3; i++)
                this.PlayerFlagCondition[i] = new PlayerFlagSubcondition(stream);

            this.BuffSubCondition = new BuffSubcondition[3]; //1208
            for (int i = 0; i < 3; i++)
                this.BuffSubCondition[i] = new BuffSubcondition(stream);

            this.I30 = stream.ReadValueS32(); //1244
            this.I31 = stream.ReadValueS32(); //1248
            this.I32 = stream.ReadValueS32(); //1252

            stream.Close();
        }
    }

    public class LoreSubcondition
    {
        public int SNOLore { get; private set; }
        public int I0 { get; private set; }

        public LoreSubcondition(MpqFileStream stream)
        {
            this.SNOLore = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
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
            this.SNOQuest = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
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
            this.ItemGBId = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
        }
    }

    public class FollowerSubcondition
    {
        public FollowerType Type { get; private set; }
        public int I0 { get; private set; }

        public FollowerSubcondition(MpqFileStream stream)
        {
            this.Type = (FollowerType)stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
        }
    }

    public class LabelSubcondition
    {
        public int LabelGBId { get; private set; }
        public int I0 { get; private set; }

        public LabelSubcondition(MpqFileStream stream)
        {
            this.LabelGBId = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
        }
    }

    public class SkillSubcondition
    {
        public int SNOSkillPower { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public SkillSubcondition(MpqFileStream stream)
        {
            this.SNOSkillPower = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
        }
    }

    public class MonsterSubcondition
    {
        public int SNOMonsterActor { get; private set; }

        public MonsterSubcondition(MpqFileStream stream)
        {
            this.SNOMonsterActor = stream.ReadValueS32();
        }
    }

    public class GameFlagSubcondition
    {
        public string S0 { get; private set; }

        public GameFlagSubcondition(MpqFileStream stream)
        {
            this.S0 = stream.ReadString(128, true);
        }
    }

    public class PlayerFlagSubcondition
    {
        public string S0 { get; private set; }

        public PlayerFlagSubcondition(MpqFileStream stream)
        {
            this.S0 = stream.ReadString(128, true);
        }
    }

    public class BuffSubcondition
    {
        public int SNOPower { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public BuffSubcondition(MpqFileStream stream)
        {
            this.SNOPower = stream.ReadValueS32();
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
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
