using System.Collections.Generic;
using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Monster)]
    public class Monster : FileFormat
    {
        public Header Header { get; private set; }
        public int Flags { get; private set; }
        public int ActorSNO { get; private set; }
        public int LookIndex { get; private set; }
        public MonsterRace Race { get; private set; }
        public MonsterSize Size { get; private set; }
        public MonsterType Type { get; private set; }
        public MonsterDef Monsterdef { get; private set; }
        public Resistance Resists { get; private set; }
        public int DefaultCountMin { get; private set; }
        public int DefaultCountDelta { get; private set; }
        public float[] AttributeModifiers { get; private set; }
        public float HPChampion { get; private set; }
        public float HPDeltaChampion { get; private set; }
        public float HPRare { get; private set; }
        public float HPDeltaRare { get; private set; }
        public float HPMinion { get; private set; }
        public float HPDeltaMinion { get; private set; }
        public int SNOInventory { get; private set; }
        public int SNOSecondaryInventory { get; private set; }
        public int SNOLore { get; private set; }
        public int GoldGranted { get; private set; }
        public HealthDropInfo HealthDropNormal { get; private set; }
        public HealthDropInfo HealthDropChampion { get; private set; }
        public HealthDropInfo HealthDropRare { get; private set; }
        public HealthDropInfo HealthDropMinion { get; private set; }
        public int SNOSkillKit { get; private set; }

        public SkillDeclaration[] SkillDeclarations { get; private set; }
        public MonsterSkillDeclaration[] MonsterSkillDeclarations { get; private set; }
        public int SNOTreasureClassFirstKill { get; private set; }
        public int SNOTreasureClass { get; private set; }
        public int SNOTreasureClassRare { get; private set; }
        public int SNOTreasureClassChampion { get; private set; }
        public int SNOTreasureClassChampionLight { get; private set; }
        public float NoDropScalar { get; private set; }
        public float FleeChance { get; private set; }
        public float FleeCooldownMin { get; private set; }
        public float FleeCooldownDelta { get; private set; }
        public int SummonCountPer { get; private set; }
        public float SummonLifetime { get; private set; }
        public int SummonMaxConcurrent { get; private set; }
        public int SummonMaxTotal { get; private set; }
        public int[] AIBehavior { get; private set; }
        public int[] GBIdMovementStyles { get; private set; } // 8
        public int[] SNOSummonActor { get; private set; } //6
        public int RandomAffixes { get; private set; }
        public int[] GBIdAffixes { get; private set; } // 4
        public int[] GBIdDisallowedAffixes { get; private set; } // 6
        public int AITargetStyleNormal { get; private set; }
        public int AITargetStyleChampion { get; private set; }
        public int AITargetStyleRare { get; private set; }
        public MonsterPowerType PowerType { get; private set; }
        public string Name { get; private set; } // 128
        public TagMap TagMap { get; private set; }
        public int MinionSpawnGroupCount { get; private set; }
        public List<MonsterMinionSpawnGroup> MonsterMinionSpawngroup { get; private set; }
        public List<MonsterChampionSpawnGroup> MonsterChampionSpawngroup { get; private set; }
        public int ChampionSpawnGroupCount { get; private set; }
        public int DoesNotDropNecroCorpse { get; private set; }
        public int Pad { get; private set; }
        public int snoAIStateAttackerCapReached { get; private set; }

        public Monster(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            Flags = stream.ReadValueS32(); //12
            ActorSNO = stream.ReadValueS32(); //16
            LookIndex = stream.ReadValueS32(); //20
            Type = (MonsterType)stream.ReadValueS32(); //40 - 24
            Race = (MonsterRace)stream.ReadValueS32(); //44 - 28
            Size = (MonsterSize)stream.ReadValueS32(); //48 - 32
            Monsterdef = new MonsterDef(stream); //52 - 36
            Resists = (Resistance)stream.ReadValueS32(); //56
            DefaultCountMin = stream.ReadValueS32(); //60
            DefaultCountDelta = stream.ReadValueS32(); //64
            AttributeModifiers = new float[146]; //68
            for (int i = 0; i < 146; i++)
            {
                AttributeModifiers[i] = stream.ReadValueF32();
            }
            HPChampion = stream.ReadValueF32(); //652
            HPDeltaChampion = stream.ReadValueF32();
            HPRare = stream.ReadValueF32();
            HPDeltaRare = stream.ReadValueF32();
            HPMinion = stream.ReadValueF32();
            HPDeltaMinion = stream.ReadValueF32(); //672

            GoldGranted = stream.ReadValueS32();
            HealthDropNormal = new HealthDropInfo(stream);
            HealthDropChampion = new HealthDropInfo(stream);
            HealthDropRare = new HealthDropInfo(stream);
            HealthDropMinion = new HealthDropInfo(stream);
            // 716
            SNOSkillKit = stream.ReadValueS32();
            SkillDeclarations = new SkillDeclaration[8];
            for (int i = 0; i < 8; i++)
            {
                SkillDeclarations[i] = new SkillDeclaration(stream);
            }
            MonsterSkillDeclarations = new MonsterSkillDeclaration[8];
            for (int i = 0; i < 8; i++)
            {
                MonsterSkillDeclarations[i] = new MonsterSkillDeclaration(stream);
            }
            // 912
            SNOTreasureClassFirstKill = stream.ReadValueS32();
            SNOTreasureClass = stream.ReadValueS32();
            SNOTreasureClassRare = stream.ReadValueS32();
            SNOTreasureClassChampion = stream.ReadValueS32();
            SNOTreasureClassChampionLight = stream.ReadValueS32();
            // 932
            NoDropScalar = stream.ReadValueF32();
            FleeChance = stream.ReadValueF32();
            FleeCooldownMin = stream.ReadValueF32();
            FleeCooldownDelta = stream.ReadValueF32();
            SummonCountPer = stream.ReadValueS32();
            SummonLifetime = stream.ReadValueF32();
            SummonMaxConcurrent = stream.ReadValueS32();
            SummonMaxTotal = stream.ReadValueS32();
            SNOInventory = stream.ReadValueS32(); //3D0 - 976 + 28 =1004
            SNOSecondaryInventory = stream.ReadValueS32();
            SNOLore = stream.ReadValueS32();
            AIBehavior = new int[6];

            for (int i = 0; i < 6; i++)
            {
                AIBehavior[i] = stream.ReadValueS32();
            }

            GBIdMovementStyles = new int[8];
            for (int i = 0; i < 8; i++)
            {
                GBIdMovementStyles[i] = stream.ReadValueS32();
            }

            SNOSummonActor = new int[6];
            for (int i = 0; i < 6; i++)
            {
                SNOSummonActor[i] = stream.ReadValueS32();
            }

            RandomAffixes = stream.ReadValueS32();

            GBIdAffixes = new int[4];
            for (int i = 0; i < 4; i++)
            {
                GBIdAffixes[i] = stream.ReadValueS32();
            }
            GBIdDisallowedAffixes = new int[6];
            for (int i = 0; i < 6; i++)
            {
                GBIdDisallowedAffixes[i] = stream.ReadValueS32();
            }
            // 1096
            AITargetStyleNormal = stream.ReadValueS32();
            AITargetStyleChampion = stream.ReadValueS32();
            AITargetStyleRare = stream.ReadValueS32();
            PowerType = (MonsterPowerType)stream.ReadValueS32(); //1152
            //0x468
            stream.Position += (3 * 4);
            TagMap = stream.ReadSerializedItem<TagMap>(); //1180
            stream.Position = 1196;
            MinionSpawnGroupCount = stream.ReadValueS32(); //1196
            stream.Position += (3 * 4);
            MonsterMinionSpawngroup = stream.ReadSerializedData<MonsterMinionSpawnGroup>(); //1212
            ChampionSpawnGroupCount = stream.ReadValueS32(); //1220
            MonsterChampionSpawngroup = stream.ReadSerializedData<MonsterChampionSpawnGroup>(); //1236
            Name = stream.ReadString(128, true); //1244
            DoesNotDropNecroCorpse = stream.ReadValueS32(); //1344
            Pad = stream.ReadValueS32(); //1344
            snoAIStateAttackerCapReached = stream.ReadValueS32();


            stream.Close();
        }

        public class MonsterMinionSpawnGroup : ISerializableData
        {
            public float Weight { get; private set; }
            public int SpawnItemCount { get; private set; }
            public List<MonsterMinionSpawnItem> SpawnItems = new List<MonsterMinionSpawnItem>();
            public void Read(MpqFileStream stream)
            {
                Weight = stream.ReadValueF32();
                SpawnItemCount = stream.ReadValueS32();
                stream.Position += 8;
                SpawnItems = stream.ReadSerializedData<MonsterMinionSpawnItem>();
            }
        }

        public class MonsterChampionSpawnGroup : ISerializableData
        {
            public float Weight { get; private set; }
            public int SpawnItemCount { get; private set; }
            public List<MonsterChampionSpawnItem> SpawnItems = new List<MonsterChampionSpawnItem>();
            public void Read(MpqFileStream stream)
            {
                Weight = stream.ReadValueF32();
                SpawnItemCount = stream.ReadValueS32();
                stream.Position += 8;
                SpawnItems = stream.ReadSerializedData<MonsterChampionSpawnItem>();
            }
        }

        public class MonsterMinionSpawnItem : ISerializableData
        {
            public int SNOSpawn { get; private set; }
            public int SpawnCountMin { get; private set; }
            public int SpawnCountMax { get; private set; }
            public int SpawnSpreadMin { get; private set; }
            public int SpawnSpreadMax { get; private set; }

            public void Read(MpqFileStream stream)
            {
                SNOSpawn = stream.ReadValueS32();
                SpawnCountMin = stream.ReadValueS32();
                SpawnCountMax = stream.ReadValueS32();
                SpawnSpreadMin = stream.ReadValueS32();
                SpawnSpreadMax = stream.ReadValueS32();
            }
        }

        public class MonsterChampionSpawnItem : ISerializableData
        {
            public int SnoActor { get; private set; }
            public int SpawnCount { get; private set; }

            public void Read(MpqFileStream stream)
            {
                SnoActor = stream.ReadValueS32();
                SpawnCount = stream.ReadValueS32();
            }
        }

        public class MonsterDef
        {
            public float IdleRadius { get; private set; }
            public float CombatRaidus { get; private set; }
            public float TargetAbandonTime { get; private set; }
            public float WarnOthersRadius { get; private set; }
            public int RequireLOSforAllTargets { get; private set; }

            public MonsterDef(MpqFileStream stream)
            {
                IdleRadius = stream.ReadValueF32();
                CombatRaidus = stream.ReadValueF32();
                TargetAbandonTime = stream.ReadValueF32();
                WarnOthersRadius = stream.ReadValueF32();
                RequireLOSforAllTargets = stream.ReadValueS32();
            }
        }
        public class MonsterSkillDeclaration
        {
            public float UseRangeMin { get; private set; }
            public float UseRangeMax { get; private set; }
            public int Weight { get; private set; }
            public float Timer { get; private set; }

            public MonsterSkillDeclaration(MpqFileStream stream)
            {
                UseRangeMin = stream.ReadValueF32();
                UseRangeMax = stream.ReadValueF32();
                Weight = stream.ReadValueS32();
                Timer = stream.ReadValueF32();
            }
        }
        public class SkillDeclaration
        {
            public int SNOPower { get; private set; }
            public int LevelMod { get; private set; }

            public SkillDeclaration(MpqFileStream stream)
            {
                SNOPower = stream.ReadValueS32();
                LevelMod = stream.ReadValueS32();
            }
        }

        public enum MonsterPowerType // No idea what this is called - DarkLotus
        {
            Mana = 0,
            Arcanum = 1,
            Fury = 2,
            Spirit = 3,
            Power = 4,
            Hatred = 5,
            Discipline = 6,
            Faith = 7,
            Essence = 8
        }

        public class HealthDropInfo
        {
            public float DropChance { get; private set; }
            public int GBID { get; private set; }
            public int HealthDropStyle { get; private set; }

            public HealthDropInfo(MpqFileStream stream)
            {
                DropChance = stream.ReadValueF32();
                GBID = stream.ReadValueS32();
                HealthDropStyle = stream.ReadValueS32();
            }
        }

        public enum Resistance
        {
            Physical = 0,
            Fire = 1,
            Lightning = 2,
            Cold = 3,
            Poison = 4,
            Arcane = 5,
            Holy = 6
        }

        public class Levels
        {
            public int Normal;
            public int Nightmare;
            public int Hell;
            public int Inferno;
        }

        public enum MonsterSize
        {
            Unknown = -1,
            Big = 3,
            Standard = 4,
            Ranged = 5,
            Swarm = 6,
            Boss = 7
        }

        public enum MonsterRace
        {
            Unknown = -1,
            Fallen = 1,
            GoatMen = 2,
            Rogue = 3,
            Skeleton = 4,
            Zombie = 5,
            Spider = 6,
            Triune = 7,
            WoodWraith = 8,
            Human = 9,
            Animal = 10,
            TreasureGoblin = 11,
            CrazedAngel = 12
        }

        public enum MonsterType
        {
            Unknown = -1,
            Undead = 0,
            Demon = 1,
            Beast = 2,
            Human = 3,
            Breakable = 4,
            Scenery = 5,
            Ally = 6,
            Team = 7,
            Helper = 8,
            CorruptedAngel = 9,
            Pandemonium = 10,
            Adria = 11,
            BloodGolem = 12
        }
    }
}
