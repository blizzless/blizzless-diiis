//Blizzless Project 2022
using System;
using System.Collections.Generic;
using CrystalMpq;
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.GameBalance)]
    public class GameBalance : FileFormat
    {
        public Header Header { get; private set; }
        public BalanceType Type { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public List<ItemTypeTable> ItemType { get; private set; }
        public List<ItemTable> Item { get; private set; }
        public List<ExperienceTable> Experience { get; private set; }
        public List<ExperienceAltTable> ExperienceAlt { get; private set; }
        public List<HelpCodesTable> HelpCodes { get; private set; }
        public List<MonsterLevelTable> MonsterLevel { get; private set; }
        public List<AffixTable> Affixes { get; private set; }
        public List<HeroTable> Heros { get; private set; }

        [PersistentProperty("MovementStyles")]
        public List<MovementStyle> MovementStyles { get; private set; }
        public List<LabelGBIDTable> Labels { get; private set; }

        [PersistentProperty("LootDistribution")]
        public List<LootDistributionTableEntry> LootDistribution { get; private set; }

        public List<RareItemNamesTable> RareItemNames { get; private set; }

        public List<MonsterAffixesTable> MonsterAffixes { get; private set; }
        public List<MonsterNamesTable> RareMonsterNames { get; private set; }
        public List<SocketedEffectTable> SocketedEffects { get; private set; }

        [PersistentProperty("ItemDropTable")]
        public List<ItemDropTableEntry> ItemDropTable { get; private set; }

        [PersistentProperty("ItemLevelModifiers")]
        public List<ItemLevelModifier> ItemLevelModifiers { get; private set; }

        [PersistentProperty("QualityClasses")]
        public List<QualityClass> QualityClasses { get; private set; }

        public List<HandicapLevelTable> HandicapLevelTables { get; private set; }
        public List<ItemSalvageLevelTable> ItemSalvageLevelTables { get; private set; }

        public List<HirelingTable> Hirelings { get; private set; }
        public List<SetItemBonusTable> SetItemBonus { get; private set; }

        [PersistentProperty("EliteModifiers")]
        public List<EliteModifier> EliteModifiers { get; private set; }

        [PersistentProperty("ItemTiers")]
        public List<ItemTier> ItemTiers { get; private set; }
        public List<PowerFormulaTable> PowerFormula { get; private set; }
        public List<RecipeTable> Recipes { get; private set; }

        [PersistentProperty("ScriptedAchievementEvents")]
        public List<ScriptedAchievementEventsTable> ScriptedAchievementEvents { get; private set; }

        public List<LootRunQuestTierTable> LootRunQuestTierTables { get; private set; }
        public List<ParagonBonusesTable> ParagonBonusesTables { get; private set; }

        public List<LegacyItemConversionTable> LegacyItemConversionTables { get; private set; }
        public List<EnchantItemAffixUseCountCostScalarsTable> EnchantItemAffixUseCountCostScalarsTables { get; private set; }
        public List<TieredLootRunLevelTable> TieredLootRunLevelTables { get; private set; }
        public List<TransmuteRecipesTable> TransmuteRecipesTables { get; private set; }
        public List<CurrencyConversionTable> CurrencyConversionTables { get; private set; }
        public GameBalance() { }
        public GameBalance(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            Type = (BalanceType)stream.ReadValueS32();
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            ItemType = stream.ReadSerializedData<ItemTypeTable>();
            stream.Position += 8;
            Item = stream.ReadSerializedData<ItemTable>();
            stream.Position += 8;
            Experience = stream.ReadSerializedData<ExperienceTable>();
            stream.Position += 8;
            ExperienceAlt = stream.ReadSerializedData<ExperienceAltTable>();
            stream.Position += 8;
            HelpCodes = stream.ReadSerializedData<HelpCodesTable>();
            stream.Position += 8;
            MonsterLevel = stream.ReadSerializedData<MonsterLevelTable>();
            stream.Position += 8;
            Affixes = stream.ReadSerializedData<AffixTable>();
            stream.Position += 8;
            Heros = stream.ReadSerializedData<HeroTable>();
            stream.Position += 8;
            MovementStyles = stream.ReadSerializedData<MovementStyle>();
            stream.Position += 8;
            Labels = stream.ReadSerializedData<LabelGBIDTable>();
            stream.Position += 8;
            LootDistribution = stream.ReadSerializedData<LootDistributionTableEntry>();
            stream.Position += 8;
            RareItemNames = stream.ReadSerializedData<RareItemNamesTable>();
            stream.Position += 8;
            MonsterAffixes = stream.ReadSerializedData<MonsterAffixesTable>();
            stream.Position += 8;
            RareMonsterNames = stream.ReadSerializedData<MonsterNamesTable>();
            stream.Position += 8;
            SocketedEffects = stream.ReadSerializedData<SocketedEffectTable>();
            stream.Position += 8;
            ItemDropTable = stream.ReadSerializedData<ItemDropTableEntry>();
            stream.Position += 8;
            ItemLevelModifiers = stream.ReadSerializedData<ItemLevelModifier>();
            stream.Position += 8;
            QualityClasses = stream.ReadSerializedData<QualityClass>();
            stream.Position += 8;
            HandicapLevelTables = stream.ReadSerializedData<HandicapLevelTable>();
            stream.Position += 8;
            ItemSalvageLevelTables = stream.ReadSerializedData<ItemSalvageLevelTable>();
            stream.Position += 8;
            Hirelings = stream.ReadSerializedData<HirelingTable>();
            stream.Position += 8;
            SetItemBonus = stream.ReadSerializedData<SetItemBonusTable>();
            stream.Position += 8;
            EliteModifiers = stream.ReadSerializedData<EliteModifier>();
            stream.Position += 8;
            ItemTiers = stream.ReadSerializedData<ItemTier>();
            stream.Position += 8;
            PowerFormula = stream.ReadSerializedData<PowerFormulaTable>();
            stream.Position += 8;
            Recipes = stream.ReadSerializedData<RecipeTable>();
            stream.Position += 8;
            ScriptedAchievementEvents = stream.ReadSerializedData<ScriptedAchievementEventsTable>();
            stream.Position += 8;
            LootRunQuestTierTables = stream.ReadSerializedData<LootRunQuestTierTable>();
            stream.Position += 8;
            ParagonBonusesTables = stream.ReadSerializedData<ParagonBonusesTable>();
            stream.Position += 8;
            LegacyItemConversionTables = stream.ReadSerializedData<LegacyItemConversionTable>();
            stream.Position += 8;
            EnchantItemAffixUseCountCostScalarsTables = stream.ReadSerializedData<EnchantItemAffixUseCountCostScalarsTable>();
            stream.Position += 8;
            TieredLootRunLevelTables = stream.ReadSerializedData<TieredLootRunLevelTable>();
            stream.Position += 8;
            TransmuteRecipesTables = stream.ReadSerializedData<TransmuteRecipesTable>();
            stream.Position += 8;
            CurrencyConversionTables = stream.ReadSerializedData<CurrencyConversionTable>();
            stream.Position += 8;

            #region Запись дампа вещей
            /*
            if (Item.Count > 0)
            {
                string writePath = @"D:\__DiIiS-ROS__\GameBalanceData-";//.txt";
                writePath += file.Name;
                writePath += ".txt";
                int i = 0;
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {

                    foreach (var I in Item)
                    {
                        sw.WriteLine(@"Hash: {0}", I.Hash);
                        sw.WriteLine(@"Name: {0}", I.Name);
                        sw.WriteLine(@"GBID: {0}", I.GBID);
                        sw.WriteLine(@"SNOActor: {0}", I.SNOActor);
                        sw.WriteLine(@"ItemTypesGBID: {0}", I.ItemTypesGBID);
                        sw.WriteLine(@"Flags: {0}", I.Flags);
                        sw.WriteLine(@"DyeType: {0}", I.DyeType);
                        sw.WriteLine(@"ItemLevel: {0}", I.ItemLevel);
                        sw.WriteLine(@"ItemAct: {0}", I.ItemAct);
                        sw.WriteLine(@"AffixLevel: {0}", I.AffixLevel);
                        sw.WriteLine(@"BonusAffixes: {0}", I.BonusAffixes);
                        sw.WriteLine(@"BonusMajorAffixes: {0}", I.BonusMajorAffixes);
                        sw.WriteLine(@"BonusMinorAffixes: {0}", I.BonusMinorAffixes);
                        sw.WriteLine(@"MaxSockets: {0}", I.MaxSockets);
                        sw.WriteLine(@"MaxStackSize: {0}", I.MaxStackSize);
                        sw.WriteLine(@"Cost: {0}", I.Cost);
                        sw.WriteLine(@"CostAlt: {0}", I.CostAlt);
                        sw.WriteLine(@"IdentifyCost: {0}", I.IdentifyCost);
                        sw.WriteLine(@"SellOverrideCost: {0}", I.SellOverrideCost);
                        sw.WriteLine(@"RemoveGemCost: {0}", I.RemoveGemCost);
                        sw.WriteLine(@"RequiredLevel: {0}", I.RequiredLevel);
                        sw.WriteLine(@"CrafterRequiredLevel: {0}", I.CrafterRequiredLevel);
                        sw.WriteLine(@"BaseDurability: {0}", I.BaseDurability);
                        sw.WriteLine(@"DurabilityVariance: {0}", I.DurabilityVariance);
                        sw.WriteLine(@"EnchantAffixCost: {0}", I.EnchantAffixCost);
                        sw.WriteLine(@"EnchantAffixCostX1: {0}", I.EnchantAffixCostX1);
                        sw.WriteLine(@"TransmogUnlockCrafterLevel: {0}", I.TransmogUnlockCrafterLevel);
                        sw.WriteLine(@"TransmogCost: {0}", I.TransmogCost);
                        sw.WriteLine(@"SNOBaseItem: {0}", I.SNOBaseItem);
                        sw.WriteLine(@"SNOSet: {0}", I.SNOSet);
                        sw.WriteLine(@"SNOComponentTreasureClass: {0}", I.SNOComponentTreasureClass);
                        sw.WriteLine(@"SNOComponentTreasureClassMagic: {0}", I.SNOComponentTreasureClassMagic);
                        sw.WriteLine(@"SNOComponentTreasureClassRare: {0}", I.SNOComponentTreasureClassRare);
                        sw.WriteLine(@"SNOComponentTreasureClassLegend: {0}", I.SNOComponentTreasureClassLegend);
                        sw.WriteLine(@"StartEffect: {0}", I.StartEffect);
                        sw.WriteLine(@"EndEffect: {0}", I.EndEffect);
                        sw.WriteLine(@"PortraitBkgrnd: {0}", I.PortraitBkgrnd);
                        sw.WriteLine(@"PortraitHPBar: {0}", I.PortraitHPBar);
                        sw.WriteLine(@"PortraitBanner: {0}", I.PortraitBanner);
                        sw.WriteLine(@"PortraitFrame: {0}", I.PortraitFrame);
                        sw.WriteLine(@"Labels: {0}", I.Labels);
                        sw.WriteLine(@"Pad: {0}", I.Pad);
                        sw.WriteLine(@"WeaponDamageMin: {0}", I.WeaponDamageMin);
                        sw.WriteLine(@"WeaponDamageDelta: {0}", I.WeaponDamageDelta);
                        sw.WriteLine(@"DamageMinVariance: {0}", I.DamageMinVariance);
                        sw.WriteLine(@"DamageDeltaVariance: {0}", I.DamageDeltaVariance);
                        sw.WriteLine(@"AttacksPerSecond: {0}", I.AttacksPerSecond);
                        sw.WriteLine(@"Armor: {0}", I.Armor);
                        sw.WriteLine(@"ArmorDelta: {0}", I.ArmorDelta);
                        sw.WriteLine(@"SNOSkill0: {0}", I.SNOSkill0);
                        sw.WriteLine(@"SkillI0: {0}", I.SkillI0);
                        sw.WriteLine(@"SNOSkill1: {0}", I.SNOSkill1);
                        sw.WriteLine(@"SkillI1: {0}", I.SkillI1);
                        sw.WriteLine(@"SNOSkill2: {0}", I.SNOSkill2);
                        sw.WriteLine(@"SkillI2: {0}", I.SkillI2);
                        sw.WriteLine(@"SNOSkill3: {0}", I.SNOSkill3);
                        sw.WriteLine(@"SkillI3: {0}", I.SkillI3);
                        sw.WriteLine(@"Quality: {0}", I.Quality);
                        sw.WriteLine(@"EnhancementToGrant: {0}", I.EnhancementToGrant);
                        sw.WriteLine(@"LegendaryFamily: {0}", I.LegendaryFamily);
                        sw.WriteLine(@"CraftingTier: {0}", I.CraftingTier);
                        sw.WriteLine(@"CostAlt2: {0}", I.CostAlt2);
                        sw.WriteLine(@"------------------------------------------");
                    }
                }
            }
            //*/
            #endregion
        }

        public enum BalanceType : int
        {
            ItemTypes = 1,
            Items = 2,
            ExperienceTable = 3,
            Hirelings = 4,
            MonsterLevels = 5,
            Heros = 7,
            AffixList = 8,
            MovementStyles = 10,
            Labels = 11,
            LootDistribution = 12,
            RareItemNames = 16,
            Scenery = 17,
            MonsterAffixes = 18,
            MonsterNames = 19,
            SocketedEffects = 21,
            HelpCodes = 24,
            ItemDropTable = 25,
            ItemLevelModifiers = 26,
            QualityClasses = 27,
            Handicaps = 28,
            ItemSalvageLevels = 29,
            Recipes = 32,
            SetItemBonuses = 33,
            EliteModifiers = 34,
            ItemTiers = 35,
            PowerFormulaTables = 36,
            ScriptedAchievementEvents = 37,
            LootRunQuestTiers = 39,
            ParagonBonuses = 40,
            LegacyItemConversions = 45,
            EnchantItemAffixUseCountCostScalars = 46,
            TieredLootRunLevels = 49,
            TransmuteRecipes = 50,
            CurrencyConversions = 51
        }
        public class ItemTypeTable : ISerializableData
        {
            public int Hash { get; private set; }
            public string Name { get; private set; }
            public int ParentType { get; private set; }
            public int I0 { get; private set; }
            public int GBID { get; private set; }
            public int LootLevelRange { get; private set; }
            public int ReqCrafterLevelForEnchant { get; private set; }
            public int MaxSockets { get; private set; }
            public ItemFlags Usable { get; private set; }
            public eItemType BodySlot1 { get; private set; }
            public eItemType BodySlot2 { get; private set; }
            public eItemType BodySlot3 { get; private set; }
            public eItemType BodySlot4 { get; private set; }
            public int InheritedAffix0 { get; private set; }
            public int InheritedAffix1 { get; private set; }
            public int InheritedAffix2 { get; private set; }
            public int InheritedAffixFamily0 { get; private set; }
            public int[] Labels { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                Hash = StringHashHelper.HashItemName(Name);
                ParentType = stream.ReadValueS32();
                GBID = stream.ReadValueS32();
                I0 = stream.ReadValueS32();
                LootLevelRange = stream.ReadValueS32();
                ReqCrafterLevelForEnchant = stream.ReadValueS32();
                MaxSockets = stream.ReadValueS32();
                Usable = (ItemFlags)stream.ReadValueS32();
                BodySlot1 = (eItemType)stream.ReadValueS32();
                BodySlot2 = (eItemType)stream.ReadValueS32();
                BodySlot3 = (eItemType)stream.ReadValueS32();
                BodySlot4 = (eItemType)stream.ReadValueS32();
                InheritedAffix0 = stream.ReadValueS32();
                InheritedAffix1 = stream.ReadValueS32();
                InheritedAffix2 = stream.ReadValueS32();
                InheritedAffixFamily0 = stream.ReadValueS32();
                Labels = new int[5];
                for (int i = 0; i < 5; i++)
                    Labels[i] = stream.ReadValueS32();
            }
        }
        public class ItemTable : ISerializableData
        {
            public int Hash { get; private set; }
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }
            public int SNOActor { get; private set; }
            public int ItemTypesGBID { get; private set; }
            public int Flags { get; private set; }
            public int DyeType { get; private set; }
            public int ItemLevel { get; set; }
            public eItemAct ItemAct { get; private set; }
            public int AffixLevel { get; private set; }
            public int BonusAffixes { get; private set; }
            public int BonusMajorAffixes { get; private set; }
            public int BonusMinorAffixes { get; set; }
            public int MaxSockets { get; private set; }
            public int MaxStackSize { get; private set; }
            public int Cost { get; private set; }
            public int CostAlt { get; private set; }
            public int IdentifyCost { get; private set; }
            public int SellOverrideCost { get; private set; }
            public int RemoveGemCost { get; private set; }
            public int RequiredLevel { get; set; }
            public int CrafterRequiredLevel { get; set; }
            public int BaseDurability { get; private set; }
            public int DurabilityVariance { get; private set; }
            public int EnchantAffixCost { get; private set; }
            public int EnchantAffixCostX1 { get; private set; }
            public int TransmogUnlockCrafterLevel { get; private set; }
            public int TransmogCost { get; private set; }


            public int SNOBaseItem { get; private set; }
            public int SNOSet { get; private set; }
            public int SNOComponentTreasureClass { get; private set; }
            public int SNOComponentTreasureClassMagic { get; private set; }
            public int SNOComponentTreasureClassRare { get; private set; }
            public int SNOComponentTreasureClassLegend { get; private set; }
            public int SNORareNamePrefixStringList { get; private set; }
            public int SNORareNameSuffixStringList { get; private set; }
            public int StartEffect { get; private set; }
            public int EndEffect { get; private set; }

            public int PortraitBkgrnd { get; private set; }
            public int PortraitHPBar { get; private set; }
            public int PortraitBanner { get; private set; }
            public int PortraitFrame { get; private set; }
            public int[] Labels { get; private set; }
            public float Pad { get; private set; }
            public float WeaponDamageMin { get; private set; }
            public float WeaponDamageDelta { get; private set; }
            public float DamageMinVariance { get; private set; }
            public float DamageDeltaVariance { get; private set; }
            public float AttacksPerSecond { get; private set; }
            public float Armor { get; private set; }
            public float ArmorDelta { get; private set; }
            public int SNOSkill0 { get; private set; }
            public int SkillI0 { get; private set; }
            public int SNOSkill1 { get; private set; }
            public int SkillI1 { get; private set; }
            public int SNOSkill2 { get; private set; }
            public int SkillI2 { get; private set; }
            public int SNOSkill3 { get; private set; }
            public int SkillI3 { get; private set; }
            public AttributeSpecifier[] Attribute { get; private set; }
            public ItemQuality Quality { get; private set; }
            public int[] RecipeToGrant { get; private set; }
            public int[] TransmogsToGrant { get; private set; }
            public int[] Massive0 { get; private set; }
            public int EnhancementToGrant { get; private set; }
            public int[] LegendaryAffixFamily { get; private set; }
            public int[] MaxAffixLevel { get; private set; }
            public int[] I38 { get; private set; }
            public int LegendaryFamily { get; private set; }
            public GemType GemT { get; private set; }
            public int CraftingTier { get; private set; }
            public Alpha CraftingQuality { get; private set; }

            public int snoActorPageOfFatePortal { get; private set; }
            public int snoWorldPageOfFate1 { get; private set; }
            public int snoWorldPageOfFate2 { get; private set; }
            public int snoLevelAreaPageOfFatePortal { get; private set; }
            public int EnchantAffixIngredientsCount { get; private set; }

            public RecipeIngredient[] EnchantAffixIngredients { get; private set; }
            public int EnchantAffixIngredientsCountX1 { get; private set; }
            public RecipeIngredient[] EnchantAffixIngredientsX1 { get; private set; }
            public int LegendaryPowerItemReplacement { get; private set; }
            public int SeasonRequiredToDrop { get; private set; }
            public AttributeSpecifier[] Attribute1 { get; private set; }

            public int JewelSecondaryEffectUnlockRank { get; private set; }
            public int JewelMaxRank { get; private set; }
            public int MainEffect { get; private set; }
            public int DateReleased { get; private set; }
            public int VacuumPickup { get; private set; }
            public int CostAlt2 { get; private set; }
            public int DynamicCraftCostMagic { get; private set; }
            public int DynamicCraftCostRare { get; private set; }
            public int DynamicCraftAffixCount { get; private set; }
            public int SeasonCacheTreasureClass { get; private set; }


            public int NewI0 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true); //000                
                Hash = StringHashHelper.HashItemName(Name); //256

                GBID = stream.ReadValueS32(); //256
                PAD = stream.ReadValueS32(); //260

                SNOActor = stream.ReadValueS32(); //264
                ItemTypesGBID = stream.ReadValueS32(); //268

                Flags = stream.ReadValueS32(); //272 - 8 + 16 = 24
                DyeType = stream.ReadValueS32(); //276
                //this.NewI0 = stream.ReadValueS32();
                ItemLevel = stream.ReadValueS32(); //280
                ItemAct = (eItemAct)stream.ReadValueS32(); //284 - 28 + 16 = 44

                AffixLevel = stream.ReadValueS32(); //288 - 48
                BonusAffixes = stream.ReadValueS32(); //292 - 52
                BonusMajorAffixes = stream.ReadValueS32(); //296 - 56
                BonusMinorAffixes = stream.ReadValueS32(); //300 - 60
                MaxSockets = stream.ReadValueS32(); //304 - 64
                MaxStackSize = stream.ReadValueS32(); //308 - 68
                Cost = stream.ReadValueS32(); //312 - 72
                CostAlt = stream.ReadValueS32(); //316 - 76
                IdentifyCost = stream.ReadValueS32(); //320 - 80
                SellOverrideCost = stream.ReadValueS32(); //324 - 84
                RemoveGemCost = stream.ReadValueS32(); //328 - 88
                RequiredLevel = stream.ReadValueS32(); //332 - 92
                CrafterRequiredLevel = stream.ReadValueS32(); //336 - 96
                BaseDurability = stream.ReadValueS32(); //340 - 100
                DurabilityVariance = stream.ReadValueS32(); //344 - 104
                EnchantAffixCost = stream.ReadValueS32(); //348 - 108
                EnchantAffixCostX1 = stream.ReadValueS32(); //352 - 112
                TransmogUnlockCrafterLevel = stream.ReadValueS32(); //356 - 116
                TransmogCost = stream.ReadValueS32(); //360 - 120
                SNOBaseItem = stream.ReadValueS32(); //364 - 124
                SNOSet = stream.ReadValueS32(); //368 - 128
                SNOComponentTreasureClass = stream.ReadValueS32(); //372
                SNOComponentTreasureClassMagic = stream.ReadValueS32(); //376
                SNOComponentTreasureClassRare = stream.ReadValueS32(); //380
                SNOComponentTreasureClassLegend = stream.ReadValueS32(); //384
                SNORareNamePrefixStringList = stream.ReadValueS32(); //388
                SNORareNameSuffixStringList = stream.ReadValueS32(); //392 - 152
                StartEffect = stream.ReadValueS32(); //396
                EndEffect = stream.ReadValueS32(); //400 
                PortraitBkgrnd = stream.ReadValueS32(); //404
                PortraitHPBar = stream.ReadValueS32(); //408
                PortraitBanner = stream.ReadValueS32(); //412
                PortraitFrame = stream.ReadValueS32(); //416
                Labels = new int[5]; //420
                for (int i = 0; i < 5; i++)
                    Labels[i] = stream.ReadValueS32();

                //stream.Position += 16;

                Pad = stream.ReadValueS32(); //440 - 200
                WeaponDamageMin = stream.ReadValueF32(); //444 - 204
                WeaponDamageDelta = stream.ReadValueF32(); //448 - 208
                DamageMinVariance = stream.ReadValueF32(); //452 - 212
                DamageDeltaVariance = stream.ReadValueF32(); //456 - 216
                AttacksPerSecond = stream.ReadValueF32(); //460 - 220
                Armor = stream.ReadValueF32(); //464 - 224
                ArmorDelta = stream.ReadValueF32(); //468 - 228

                SNOSkill0 = stream.ReadValueS32(); //472 - 232
                SkillI0 = stream.ReadValueS32(); //476
                SNOSkill1 = stream.ReadValueS32(); //480
                SkillI1 = stream.ReadValueS32(); //484
                SNOSkill2 = stream.ReadValueS32(); //488
                SkillI2 = stream.ReadValueS32(); //492
                SNOSkill3 = stream.ReadValueS32(); //496
                SkillI3 = stream.ReadValueS32(); //500

                Attribute = new AttributeSpecifier[16]; //504
                for (int i = 0; i < 16; i++)
                    Attribute[i] = new AttributeSpecifier(stream);
                Quality = (ItemQuality)stream.ReadValueS32(); // 888 -- 888-608 = 284

                RecipeToGrant = new int[10]; //892
                for (int i = 0; i < 10; i++)
                    RecipeToGrant[i] = stream.ReadValueS32();

                TransmogsToGrant = new int[8]; //932
                for (int i = 0; i < 8; i++)
                    TransmogsToGrant[i] = stream.ReadValueS32();

                Massive0 = new int[9]; // 964
                for (int i = 0; i < 9; i++)
                    Massive0[i] = stream.ReadValueS32();

                LegendaryAffixFamily = new int[6]; //1000
                for (int i = 0; i < 6; i++)
                    LegendaryAffixFamily[i] = stream.ReadValueS32();
                MaxAffixLevel = new int[6];
                for (int i = 0; i < 6; i++)
                    MaxAffixLevel[i] = stream.ReadValueS32(); //1456
                I38 = new int[6]; //1024
                for (int i = 0; i < 6; i++)
                    I38[i] = stream.ReadValueS32();

                LegendaryFamily = stream.ReadValueS32(); //1072
                GemT = (GemType)stream.ReadValueS32(); //1076
                CraftingTier = stream.ReadValueS32(); //1080
                CraftingQuality = (Alpha)stream.ReadValueS32(); //1084
                snoActorPageOfFatePortal = stream.ReadValueS32(); //1088
                snoWorldPageOfFate1 = stream.ReadValueS32(); //1092
                snoWorldPageOfFate2 = stream.ReadValueS32(); //1096
                snoLevelAreaPageOfFatePortal = stream.ReadValueS32(); //1100
                EnchantAffixIngredientsCount = stream.ReadValueS32(); //1104
                EnchantAffixIngredients = new RecipeIngredient[6]; //1108
                for (int i = 0; i < 6; i++)
                    EnchantAffixIngredients[i] = new RecipeIngredient(stream);
                EnchantAffixIngredientsCountX1 = stream.ReadValueS32(); //1156
                EnchantAffixIngredientsX1 = new RecipeIngredient[6]; //1160
                for (int i = 0; i < 6; i++)
                    EnchantAffixIngredientsX1[i] = new RecipeIngredient(stream);
                LegendaryPowerItemReplacement = stream.ReadValueS32(); //1208
                SeasonRequiredToDrop = stream.ReadValueS32(); //1212

                Attribute1 = new AttributeSpecifier[2]; //1216
                for (int i = 0; i < 2; i++)
                    Attribute1[i] = new AttributeSpecifier(stream);

                JewelSecondaryEffectUnlockRank = stream.ReadValueS32(); //1264
                JewelMaxRank = stream.ReadValueS32(); //1268
                MainEffect = stream.ReadValueS32(); //1272
                DateReleased = stream.ReadValueS32(); //1276
                VacuumPickup = stream.ReadValueS32(); //1280

                CostAlt2 = stream.ReadValueS32(); //1284
                DynamicCraftCostMagic = stream.ReadValueS32(); //1288
                DynamicCraftCostRare = stream.ReadValueS32(); //1292
                DynamicCraftAffixCount = stream.ReadValueS32(); //1296
                SeasonCacheTreasureClass = stream.ReadValueS32(); //1300
                if (SNOSet != -1)// & Name.Contains("Unique"))
                { }

            }


            public enum ItemQuality
            {
                Invalid = -1,
                Inferior = 0,
                Normal = 1,
                Superior = 2,
                Magic1 = 3,
                Magic2 = 4,
                Magic3 = 5,
                Rare4 = 6,
                Rare5 = 7,
                Rare6 = 8,
                Legendary = 9,
                Special = 10,
                Set = 11
            }

            [Flags]
            public enum eItemAct
            {
                Invalid = -1,
                A1 = 0,
                A2 = 100,
                A3 = 200,
                A4 = 300,
                A5 = 400,
                Test = 1000,
                OpenWorld = 3000
            }

            public enum GemType : int
            {
                Amethyst = 1,
                Emerald = 2,
                Ruby = 3,
                Topaz = 4,
                Diamond = 5
            }

            public enum Alpha : int
            {
                A = 1,
                B = 2,
                C = 3,
                D = 4
            }
        }
        public class ExperienceTable : ISerializableData
        {
            //Total Length: 224
            public long DeltaXP { get; private set; }
            public int DeltaXPClassic { get; private set; }
            public int PlayerXPValue { get; private set; }
            public float DurabilityLossPct { get; private set; }
            public float LifePerVitality { get; private set; }

            public int QuestRewardTier1 { get; private set; }
            public int QuestRewardTier2 { get; private set; }
            public int QuestRewardTier3 { get; private set; }
            public int QuestRewardTier4 { get; private set; }
            public int QuestRewardTier5 { get; private set; }
            public int QuestRewardTier6 { get; private set; }
            public int QuestRewardTier7 { get; private set; }
            public int QuestRewardGoldTier1 { get; private set; }
            public int QuestRewardGoldTier2 { get; private set; }
            public int QuestRewardGoldTier3 { get; private set; }
            public int QuestRewardGoldTier4 { get; private set; }
            public int QuestRewardGoldTier5 { get; private set; }
            public int QuestRewardGoldTier6 { get; private set; }
            public int LoreRewardTier1 { get; private set; }
            public int LoreRewardTier2 { get; private set; }
            public int LoreRewardTier3 { get; private set; }
            public int TimedDungeonBonusXP { get; private set; }
            public int WaveFightBonusXP { get; private set; }
            public int HordeBonusXP { get; private set; }
            public int ZapperBonusXP { get; private set; }

            public int GoblinHuntBonusXP { get; private set; }
            public int TimedDungeonBonusGold { get; private set; }
            public int WaveFightBonusGold { get; private set; }
            public int HordeBonusGold { get; private set; }
            public int ZapperBonusGold { get; private set; }
            public int GoblinHuntBonusGold { get; private set; }
            public int BountyKillUniqueXP { get; private set; }
            public int BountyKillUniqueGold { get; private set; }
            public int BountyKillBossXP { get; private set; }
            public int BountyKillBossGold { get; private set; }

            public int BountyCompleteEventXP { get; private set; }
            public int BountyCompleteEventGold { get; private set; }
            public int BountyClearDungeonXP { get; private set; }
            public int BountyClearDungeonGold { get; private set; }
            public int BountyCampsXP { get; private set; }
            public int BountyCampsGold { get; private set; }
            public int KillCounterReward { get; private set; }
            public int GenericSkillPoints { get; private set; }

            public float CritMultiplier { get; private set; }
            public float DodgeMultiplier { get; private set; }
            public float LifeStealMultiplier { get; private set; }

            public int DemonHunterStrength { get; private set; }
            public int DemonHunterDexterity { get; private set; }
            public int DemonHunterIntelligence { get; private set; }
            public int DemonHunterVitality { get; private set; }
            public int BarbarianStrength { get; private set; }
            public int BarbarianDexterity { get; private set; }
            public int BarbarianIntelligence { get; private set; }
            public int BarbarianVitality { get; private set; }
            public int WizardStrength { get; private set; }
            public int WizardDexterity { get; private set; }
            public int WizardIntelligence { get; private set; }
            public int WizardVitality { get; private set; }
            public int WitchDoctorStrength { get; private set; }
            public int WitchDoctorDexterity { get; private set; }
            public int WitchDoctorIntelligence { get; private set; }
            public int WitchDoctorVitality { get; private set; }
            public int MonkStrength { get; private set; }
            public int MonkDexterity { get; private set; }
            public int MonkIntelligence { get; private set; }
            public int MonkVitality { get; private set; }
            public int CrusaderStrength { get; private set; }
            public int CrusaderDexterity { get; private set; }
            public int CrusaderIntelligence { get; private set; }
            public int CrusaderVitality { get; private set; }
            public int NecromancerStrength { get; private set; }
            public int NecromancerDexterity { get; private set; }
            public int NecromancerIntelligence { get; private set; }
            public int NecromancerVitality { get; private set; }
            public int TemplarStrength { get; private set; }
            public int TemplarDexterity { get; private set; }
            public int TemplarIntelligence { get; private set; }
            public int TemplarVitality { get; private set; }
            public float TemplarDamageAbsorbPercent { get; private set; }
            public int ScoundrelStrength { get; private set; }
            public int ScoundrelDexterity { get; private set; }
            public int ScoundrelIntelligence { get; private set; }
            public int ScoundrelVitality { get; private set; }
            public float ScoundrelDamageAbsorbPercent { get; private set; }
            public int EnchantressStrength { get; private set; }
            public int EnchantressDexterity { get; private set; }
            public int EnchantressIntelligence { get; private set; }
            public int EnchantressVitality { get; private set; }
            public float EnchantressDamageAbsorbPercent { get; private set; }
            public long PVPXPToLevel { get; private set; }
            public int PVPGoldWin { get; private set; }
            public int PVPGoldWinByRank { get; private set; }
            public int PVPXPWin { get; private set; }
            public int PVPNormalXPWin { get; private set; }
            public int PVPTokensWin { get; private set; }
            public int PVPAltXPWin { get; private set; }
            public int PVPGoldLoss { get; private set; }
            public int PVPGoldLossByRank { get; private set; }
            public int PVPXPLoss { get; private set; }
            public int PVPNormalXPLoss { get; private set; }
            public int PVPTokensLoss { get; private set; }
            public int PVPAltXPLoss { get; private set; }
            public int PVPGoldTie { get; private set; }
            public int PVPGoldTieByRank { get; private set; }
            public int PVPXPTie { get; private set; }
            public int PVPNormalXPTie { get; private set; }
            public int PVPTokensTie { get; private set; }
            public int PVPAltXPTie { get; private set; }
            public float GoldCostLevelScalar { get; private set; }
            public int SidekickPrimaryStatIdeal { get; private set; }
            public int SidekickVitalityIdeal { get; private set; }
            public int SidekickTotalArmorIdeal { get; private set; }
            public int SidekickTotalResistIdeal { get; private set; }
            public int SidekickTargetLifeOnHitIdeal { get; private set; }
            public int SidekickTargetDPSIdeal { get; private set; }
            public float GearXPScalar { get; private set; }

            public void Read(MpqFileStream stream)
            {
                DeltaXP = stream.ReadValueS64();
                DeltaXPClassic = stream.ReadValueS32();
                PlayerXPValue = stream.ReadValueS32();

                DurabilityLossPct = stream.ReadValueF32();
                LifePerVitality = stream.ReadValueF32();

                QuestRewardTier1 = stream.ReadValueS32();
                QuestRewardTier2 = stream.ReadValueS32();
                QuestRewardTier3 = stream.ReadValueS32();
                QuestRewardTier4 = stream.ReadValueS32();
                QuestRewardTier5 = stream.ReadValueS32();
                QuestRewardTier6 = stream.ReadValueS32();
                QuestRewardTier7 = stream.ReadValueS32();
                QuestRewardGoldTier1 = stream.ReadValueS32();
                QuestRewardGoldTier2 = stream.ReadValueS32();
                QuestRewardGoldTier3 = stream.ReadValueS32();
                QuestRewardGoldTier4 = stream.ReadValueS32();
                QuestRewardGoldTier5 = stream.ReadValueS32();
                QuestRewardGoldTier6 = stream.ReadValueS32();
                LoreRewardTier1 = stream.ReadValueS32();
                LoreRewardTier2 = stream.ReadValueS32();
                LoreRewardTier3 = stream.ReadValueS32(); //76
                TimedDungeonBonusXP = stream.ReadValueS32(); //92
                WaveFightBonusXP = stream.ReadValueS32(); //96
                HordeBonusXP = stream.ReadValueS32(); //100
                ZapperBonusXP = stream.ReadValueS32(); //104
                GoblinHuntBonusXP = stream.ReadValueS32(); //108
                TimedDungeonBonusGold = stream.ReadValueS32(); //112
                WaveFightBonusGold = stream.ReadValueS32(); //116
                HordeBonusGold = stream.ReadValueS32(); //120
                ZapperBonusGold = stream.ReadValueS32(); //124
                GoblinHuntBonusGold = stream.ReadValueS32(); //128
                BountyKillUniqueXP = stream.ReadValueS32(); //132
                BountyKillUniqueGold = stream.ReadValueS32(); //136
                BountyKillBossXP = stream.ReadValueS32(); //140
                BountyKillBossGold = stream.ReadValueS32(); //144
                BountyCompleteEventXP = stream.ReadValueS32(); //148
                BountyCompleteEventGold = stream.ReadValueS32(); //152
                BountyClearDungeonXP = stream.ReadValueS32();
                BountyClearDungeonGold = stream.ReadValueS32();
                BountyCampsXP = stream.ReadValueS32();
                BountyCampsGold = stream.ReadValueS32();
                KillCounterReward = stream.ReadValueS32();
                GenericSkillPoints = stream.ReadValueS32();

                CritMultiplier = stream.ReadValueF32();
                DodgeMultiplier = stream.ReadValueF32();
                LifeStealMultiplier = stream.ReadValueF32();

                DemonHunterStrength = stream.ReadValueS32();
                DemonHunterDexterity = stream.ReadValueS32();
                DemonHunterIntelligence = stream.ReadValueS32();
                DemonHunterVitality = stream.ReadValueS32();
                BarbarianStrength = stream.ReadValueS32();
                BarbarianDexterity = stream.ReadValueS32();
                BarbarianIntelligence = stream.ReadValueS32();
                BarbarianVitality = stream.ReadValueS32();
                WizardStrength = stream.ReadValueS32();
                WizardDexterity = stream.ReadValueS32();
                WizardIntelligence = stream.ReadValueS32();
                WizardVitality = stream.ReadValueS32();
                WitchDoctorStrength = stream.ReadValueS32();
                WitchDoctorDexterity = stream.ReadValueS32();
                WitchDoctorIntelligence = stream.ReadValueS32();
                WitchDoctorVitality = stream.ReadValueS32();
                MonkStrength = stream.ReadValueS32();
                MonkDexterity = stream.ReadValueS32();
                MonkIntelligence = stream.ReadValueS32();
                MonkVitality = stream.ReadValueS32();
                CrusaderStrength = stream.ReadValueS32();
                CrusaderDexterity = stream.ReadValueS32();
                CrusaderIntelligence = stream.ReadValueS32();
                CrusaderVitality = stream.ReadValueS32();
                NecromancerStrength = stream.ReadValueS32();
                NecromancerDexterity = stream.ReadValueS32();
                NecromancerIntelligence = stream.ReadValueS32();
                NecromancerVitality = stream.ReadValueS32();

                TemplarStrength = stream.ReadValueS32();
                TemplarDexterity = stream.ReadValueS32();
                TemplarIntelligence = stream.ReadValueS32();
                TemplarVitality = stream.ReadValueS32();
                ScoundrelStrength = stream.ReadValueS32();
                ScoundrelDexterity = stream.ReadValueS32();
                ScoundrelIntelligence = stream.ReadValueS32();
                ScoundrelVitality = stream.ReadValueS32();
                EnchantressStrength = stream.ReadValueS32();
                EnchantressDexterity = stream.ReadValueS32();
                EnchantressIntelligence = stream.ReadValueS32();
                EnchantressVitality = stream.ReadValueS32();

                TemplarDamageAbsorbPercent = stream.ReadValueF32();
                ScoundrelDamageAbsorbPercent = stream.ReadValueF32();
                EnchantressDamageAbsorbPercent = stream.ReadValueF32();
                PVPXPToLevel = stream.ReadValueS64();

                PVPGoldWin = stream.ReadValueS32();
                PVPGoldWinByRank = stream.ReadValueS32();
                PVPXPWin = stream.ReadValueS32();
                PVPNormalXPWin = stream.ReadValueS32();
                PVPTokensWin = stream.ReadValueS32();
                PVPAltXPWin = stream.ReadValueS32();
                PVPGoldLoss = stream.ReadValueS32();
                PVPGoldLossByRank = stream.ReadValueS32();
                PVPXPLoss = stream.ReadValueS32();
                PVPNormalXPLoss = stream.ReadValueS32();

                PVPTokensLoss = stream.ReadValueS32();
                PVPAltXPLoss = stream.ReadValueS32();
                PVPGoldTie = stream.ReadValueS32();
                PVPGoldTieByRank = stream.ReadValueS32();
                PVPXPTie = stream.ReadValueS32();
                PVPNormalXPTie = stream.ReadValueS32();
                PVPTokensTie = stream.ReadValueS32();
                PVPAltXPTie = stream.ReadValueS32();
                GoldCostLevelScalar = stream.ReadValueF32();

                SidekickPrimaryStatIdeal = stream.ReadValueS32();
                SidekickVitalityIdeal = stream.ReadValueS32();
                SidekickTotalArmorIdeal = stream.ReadValueS32();
                SidekickTotalResistIdeal = stream.ReadValueS32();
                SidekickTargetLifeOnHitIdeal = stream.ReadValueS32();
                SidekickTargetDPSIdeal = stream.ReadValueS32();

                GearXPScalar = stream.ReadValueF32();

            }
        }
        public class ExperienceAltTable : ISerializableData
        {
            //Total Length: 128
            public long L0 { get; private set; }
            public int I1 { get; private set; }
            public int I2 { get; private set; }
            public int I3 { get; private set; }
            public int I4 { get; private set; }
            public int I5 { get; private set; }
            public int I6 { get; private set; }
            public int I7 { get; private set; }
            public int I8 { get; private set; }
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
            public int I21 { get; private set; }
            public int I22 { get; private set; }

            public int I23 { get; private set; }
            public int I24 { get; private set; }
            public int I25 { get; private set; }
            public int I26 { get; private set; }
            public int I27 { get; private set; }
            public int I28 { get; private set; }
            public int I29 { get; private set; }
            public int I30 { get; private set; }


            public void Read(MpqFileStream stream)
            {
                L0 = stream.ReadValueS64(); //584

                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
                I4 = stream.ReadValueS32();
                I5 = stream.ReadValueS32();
                I6 = stream.ReadValueS32();
                I7 = stream.ReadValueS32();
                I8 = stream.ReadValueS32();
                I9 = stream.ReadValueS32();
                I10 = stream.ReadValueS32();

                I11 = stream.ReadValueS32();
                I12 = stream.ReadValueS32();
                I13 = stream.ReadValueS32();
                I14 = stream.ReadValueS32();
                I15 = stream.ReadValueS32();
                I16 = stream.ReadValueS32();
                I17 = stream.ReadValueS32();
                I18 = stream.ReadValueS32();
                I19 = stream.ReadValueS32();
                I20 = stream.ReadValueS32();

                I21 = stream.ReadValueS32();
                I22 = stream.ReadValueS32();
                I23 = stream.ReadValueS32();
                I24 = stream.ReadValueS32();
                I25 = stream.ReadValueS32();
                I26 = stream.ReadValueS32();
                I27 = stream.ReadValueS32();
                I28 = stream.ReadValueS32();
                I29 = stream.ReadValueS32();
                I30 = stream.ReadValueS32();
            }
        }
        public class HelpCodesTable : ISerializableData //unused
        {
            //Total Length: 16
            public int S0 { get; private set; }
            public int S1 { get; private set; }
            public long S2 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                S0 = stream.ReadValueS32();
                S1 = stream.ReadValueS32();
                S2 = stream.ReadValueS64();
                // this.S0 = stream.ReadString(256, true);
                // this.S1 = stream.ReadString(256, true);
                // this.S2 = stream.ReadString(128, true);
            }
        }
        public class MonsterLevelTable : ISerializableData
        {
            //Total Length: 240
            public int LvlMin { get; private set; }
            public float Str { get; private set; }
            public float Dex { get; private set; }
            public float Int { get; private set; }
            public float Vit { get; private set; }
            public float HPMin { get; private set; }
            public float HPDelta { get; private set; }
            public float HPRegen { get; private set; }
            public float ResourceBase { get; private set; }
            public float ResourceRegen { get; private set; }
            public float Armor { get; private set; }
            public float Dmg { get; private set; }
            public float DmgDelta { get; private set; }
            public float DmgFire { get; private set; }
            public float DmgDeltaFire { get; private set; }
            public float DmgLightning { get; private set; }
            public float DmgDeltaLightning { get; private set; }
            public float DmgCold { get; private set; }
            public float DmgDeltaCold { get; private set; }
            public float DmgPoison { get; private set; }
            public float DmgDeltaPoison { get; private set; }
            public float DmgArcane { get; private set; }
            public float DmgDeltaArcane { get; private set; }
            public float DmgHoly { get; private set; }
            public float DmgDeltaHoly { get; private set; }
            public float DmgSiege { get; private set; }
            public float DmgDeltaSiege { get; private set; }
            public float HirelingHPMin { get; private set; }
            public float HirelingHPDelta { get; private set; }
            public float HirelingHPRegen { get; private set; }
            public float HirelingDmg { get; private set; }
            public float HirelingDmgRange { get; private set; }
            public float HirelingRetrainCost { get; private set; }
            public float GetHitDamage { get; private set; }
            public float GetHitScalar { get; private set; }
            public float GetHitMax { get; private set; }
            public float GetHitRecovery { get; private set; }
            public float WalkSpd { get; private set; }
            public float RunSpd { get; private set; }
            public float SprintSpd { get; private set; }
            public float StrafeSpd { get; private set; }
            public float AttSpd { get; private set; }
            public float ProjSpd { get; private set; }
            public float Exp { get; private set; }
            public float ResistPhysical { get; private set; }
            public float ResistFire { get; private set; }
            public float ResistLightning { get; private set; }
            public float ResistCold { get; private set; }
            public float ResistPoison { get; private set; }
            public float ResistArcane { get; private set; }
            public float ResistSiege { get; private set; }
            public float ResistChill { get; private set; }
            public float ResistStun { get; private set; }
            public float ConsoleHealthScalar { get; private set; }
            public float ConsoleDamageScalar { get; private set; }
            public float Monster1AffixWeight { get; private set; }
            public float Monster2AffixWeight { get; private set; }
            public float Monster3AffixWeight { get; private set; }
            public float Monster4AffixWeight { get; private set; }
            public int Pad { get; private set; }
            public void Read(MpqFileStream stream)
            {
                LvlMin = stream.ReadValueS32();
                Str = stream.ReadValueF32();
                Dex = stream.ReadValueF32();
                Int = stream.ReadValueF32();
                Vit = stream.ReadValueF32();
                HPMin = stream.ReadValueF32();
                HPDelta = stream.ReadValueF32();
                HPRegen = stream.ReadValueF32();
                ResourceBase = stream.ReadValueF32();
                ResourceRegen = stream.ReadValueF32();
                Armor = stream.ReadValueF32();
                Dmg = stream.ReadValueF32();
                DmgDelta = stream.ReadValueF32();
                DmgFire = stream.ReadValueF32();
                DmgDeltaFire = stream.ReadValueF32();
                DmgLightning = stream.ReadValueF32();
                DmgDeltaLightning = stream.ReadValueF32();
                DmgCold = stream.ReadValueF32();
                DmgDeltaCold = stream.ReadValueF32();
                DmgPoison = stream.ReadValueF32();
                DmgDeltaPoison = stream.ReadValueF32();
                DmgArcane = stream.ReadValueF32();
                DmgDeltaArcane = stream.ReadValueF32();
                DmgHoly = stream.ReadValueF32();
                DmgDeltaHoly = stream.ReadValueF32();
                DmgSiege = stream.ReadValueF32();
                DmgDeltaSiege = stream.ReadValueF32();
                HirelingHPMin = stream.ReadValueF32();
                HirelingHPDelta = stream.ReadValueF32();
                HirelingHPRegen = stream.ReadValueF32();
                HirelingDmg = stream.ReadValueF32();
                HirelingDmgRange = stream.ReadValueF32();
                HirelingRetrainCost = stream.ReadValueF32();
                GetHitDamage = stream.ReadValueF32();
                GetHitScalar = stream.ReadValueF32();
                GetHitMax = stream.ReadValueF32();
                GetHitRecovery = stream.ReadValueF32();
                WalkSpd = stream.ReadValueF32();
                RunSpd = stream.ReadValueF32();
                SprintSpd = stream.ReadValueF32();
                StrafeSpd = stream.ReadValueF32();
                AttSpd = stream.ReadValueF32();
                ProjSpd = stream.ReadValueF32();
                Exp = stream.ReadValueF32();
                ResistPhysical = stream.ReadValueF32();
                ResistFire = stream.ReadValueF32();
                ResistLightning = stream.ReadValueF32();
                ResistCold = stream.ReadValueF32();
                ResistPoison = stream.ReadValueF32();
                ResistArcane = stream.ReadValueF32();
                ResistSiege = stream.ReadValueF32();
                ResistChill = stream.ReadValueF32();
                ResistStun = stream.ReadValueF32();
                ConsoleHealthScalar = stream.ReadValueF32();
                ConsoleDamageScalar = stream.ReadValueF32();
                Monster1AffixWeight = stream.ReadValueF32();
                Monster2AffixWeight = stream.ReadValueF32();
                Monster3AffixWeight = stream.ReadValueF32();
                Monster4AffixWeight = stream.ReadValueF32();
                Pad = stream.ReadValueS32();
            }
        }
        public class AffixTable : ISerializableData
        {
            //Total Length: 544
            public int Hash { get; private set; }
            public string Name { get; private set; }
            public int I0 { get; set; }
            public int AffixLevel { get; set; }
            public int SupMask { get; private set; }
            public int Frequency { get; private set; }
            public int DemonHunterFrequency { get; private set; }
            public int BarbarianFrequency { get; set; }
            public int WizardFrequency { get; private set; }
            public int WitchDoctorFrequency { get; private set; }
            public int MonkFrequency { get; private set; }
            public int CrusaderFrequency { get; private set; }
            public int NecromancerFrequency { get; private set; }
            public int HirelingNoneFrequency { get; private set; }
            public int TemplarFrequency { get; private set; }
            public int ScoundrelFrequency { get; private set; }
            public int EnchantressFrequency { get; private set; }
            public int AffixLevelMin { get; set; }
            public int AffixLevelMax { get; set; }
            public int Cost { get; private set; }
            public int IdentifyCost { get; private set; }
            public int OverrideLevelReq { get; private set; }
            public int CrafterRequiredLevel { get; private set; }
            public DamageAffixType ItemEffectType { get; private set; }
            public int ItemEffectLevel { get; private set; }
            public int ConvertsTo { get; private set; }
            public int LegendaryUprankAffix { get; private set; }
            public int SNORareNamePrefixStringList { get; private set; }
            public int SNORareNameSuffixStringList { get; private set; }
            public int AffixFamily0 { get; set; }
            public int AffixFamily1 { get; private set; }
            public Class PlayerClass { get; private set; }
            public int ExclusionCategory { get; private set; }
            public int[] ExcludedCategories { get; private set; }
            public int[] ItemGroup { get; private set; }
            public int[] LegendaryAllowedTypes { get; private set; }
            public int AllowedQualityLevels { get; private set; }
            public AffixType AffixType { get; private set; }
            public int AssociatedAffix { get; private set; }
            public AttributeSpecifier[] AttributeSpecifier { get; private set; }
            public int AffixGroup { get; private set; }
            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                Hash = StringHashHelper.HashItemName(Name);
                I0 = stream.ReadValueS32();
                AffixLevel = stream.ReadValueS32();
                SupMask = stream.ReadValueS32();
                Frequency = stream.ReadValueS32();
                DemonHunterFrequency = stream.ReadValueS32();
                BarbarianFrequency = stream.ReadValueS32();
                WizardFrequency = stream.ReadValueS32();
                WitchDoctorFrequency = stream.ReadValueS32();
                MonkFrequency = stream.ReadValueS32();
                CrafterRequiredLevel = stream.ReadValueS32();
                NecromancerFrequency = stream.ReadValueS32();
                HirelingNoneFrequency = stream.ReadValueS32();
                TemplarFrequency = stream.ReadValueS32();
                ScoundrelFrequency = stream.ReadValueS32();
                EnchantressFrequency = stream.ReadValueS32();
                AffixLevelMin = stream.ReadValueS32();
                AffixLevelMax = stream.ReadValueS32();
                Cost = stream.ReadValueS32();
                IdentifyCost = stream.ReadValueS32();
                OverrideLevelReq = stream.ReadValueS32();
                CrafterRequiredLevel = stream.ReadValueS32();
                ItemEffectType = (DamageAffixType)stream.ReadValueS32(); //340
                ItemEffectLevel = stream.ReadValueS32();
                ConvertsTo = stream.ReadValueS32();
                LegendaryUprankAffix = stream.ReadValueS32();
                SNORareNamePrefixStringList = stream.ReadValueS32();
                SNORareNameSuffixStringList = stream.ReadValueS32();
                AffixFamily0 = stream.ReadValueS32();
                AffixFamily1 = stream.ReadValueS32();
                PlayerClass = (Class)stream.ReadValueS32();  //372
                ExclusionCategory = stream.ReadValueS32();

                ExcludedCategories = new int[6];
                for (int i = 0; i < 6; i++)
                    ExcludedCategories[i] = stream.ReadValueS32();
                ItemGroup = new int[24];
                for (int i = 0; i < 24; i++)
                    ItemGroup[i] = stream.ReadValueS32();
                LegendaryAllowedTypes = new int[24];
                for (int i = 0; i < 24; i++)
                    LegendaryAllowedTypes[i] = stream.ReadValueS32();

                AllowedQualityLevels = stream.ReadValueS32();
                AffixType = (AffixType)stream.ReadValueS32();  //600
                AssociatedAffix = stream.ReadValueS32();

                AttributeSpecifier = new AttributeSpecifier[4];
                for (int i = 0; i < 4; i++)
                    AttributeSpecifier[i] = new AttributeSpecifier(stream);
                //704
                stream.Position += 72;
                AffixGroup = stream.ReadValueS32();
                stream.Position += 4;
            }
        }
        public class HeroTable : ISerializableData
        {
            //Total Length: 504
            public string Name { get; private set; }

            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int SNOMaleActor { get; private set; }
            public int SNOFemaleActor { get; private set; }
            public int SNOInventory { get; private set; }
            public int MaxTrainableSkills { get; private set; }
            public int SNOStartingLMBSkill { get; private set; }
            public int SNOStartingRMBSkill { get; private set; }
            public int SNOSKillKit0 { get; private set; }
            public int SNOSKillKit1 { get; private set; }
            public int SNOSKillKit2 { get; private set; }
            public int SNOSKillKit3 { get; private set; }
            public Resource PrimaryResource { get; private set; }
            public Resource SecondaryResource { get; private set; }
            public PrimaryAttribute CoreAttribute { get; private set; }
            public float PlayerAwarenessRadius { get; private set; }
            public int IsRanged { get; private set; }

            public float Strength { get; private set; }
            public float Dexterity { get; private set; }
            public float Intelligence { get; private set; }
            public float Vitality { get; private set; }
            public float HitpointsMax { get; private set; }
            public float HitpointsFactorLevel { get; private set; }
            public float HPRegen { get; private set; }
            public float ClassDamageReductionPercent { get; private set; }
            public float ClassDamageReductionPercentPVP { get; private set; }
            public float PrimaryResourceBase { get; private set; }

            public float PrimaryResourceFactorLevel { get; private set; }
            public float PrimaryResourceRegen { get; private set; }
            public float SecondaryResourceBase { get; private set; }
            public float SecondaryResourceFactorLevel { get; private set; }
            public float SecondaryResourceRegen { get; private set; }
            public float Armor { get; private set; }
            public float Dmg { get; private set; }
            public float WalkingRate { get; private set; }
            public float RunningRate { get; private set; }
            public float SprintRate { get; private set; }

            public float ProjRate { get; private set; }
            public float CritDamageCap { get; private set; }
            public float CritPercentBase { get; private set; }
            public float CritPercentCap { get; private set; } //Resistance?
            public float DodgeRatingBase { get; private set; } //ResistanceTotal?
            public float GetHitMaxBase { get; private set; } //CastingSpeed?
            public float GetHitMaxPerLevel { get; private set; }
            public float GetHitRecoveryBase { get; private set; }
            public float GetHitRecoveryPerLevel { get; private set; }
            public float ResistPhysical { get; private set; } //HitChance?

            public float ResistFire { get; private set; }
            public float ResistLightning { get; private set; }
            public float ResistCold { get; private set; }
            public float ResistPoison { get; private set; }
            public float ResistArcane { get; private set; }
            public float ResistChill { get; private set; }
            public float ResistStun { get; private set; }
            public float KnockbackWeight { get; private set; }
            public float OOCHealthRegen { get; private set; }
            public float OOCManaRegen { get; private set; }
            public float PotionDilutionDuration { get; private set; }
            public float PotionDilutionScalar { get; private set; }
            public float DualWieldBothAttackChance { get; private set; }
            public float Freeze_Capacity { get; private set; }
            public float Thaw_Rate { get; private set; }


            public void Read(MpqFileStream stream)
            {
                //stream.Position += 4;
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                SNOMaleActor = stream.ReadValueS32();
                SNOFemaleActor = stream.ReadValueS32();
                SNOInventory = stream.ReadValueS32();
                MaxTrainableSkills = stream.ReadValueS32();
                SNOStartingLMBSkill = stream.ReadValueS32();
                SNOStartingRMBSkill = stream.ReadValueS32();
                SNOSKillKit0 = stream.ReadValueS32();
                SNOSKillKit1 = stream.ReadValueS32();
                SNOSKillKit2 = stream.ReadValueS32();
                SNOSKillKit3 = stream.ReadValueS32();
                PrimaryResource = (Resource)stream.ReadValueS32();
                SecondaryResource = (Resource)stream.ReadValueS32();
                CoreAttribute = (PrimaryAttribute)stream.ReadValueS32();
                PlayerAwarenessRadius = stream.ReadValueF32();
                IsRanged = stream.ReadValueS32(); //320
                //HitpointsMax HitpointsFactorLevel
                Strength = stream.ReadValueF32(); //336
                Dexterity = stream.ReadValueF32(); //340
                Intelligence = stream.ReadValueF32(); //352
                Vitality = stream.ReadValueF32(); //356
                HitpointsMax = stream.ReadValueF32(); //360
                HitpointsFactorLevel = stream.ReadValueF32(); //364
                HPRegen = stream.ReadValueF32(); //372
                ClassDamageReductionPercent = stream.ReadValueF32(); //376
                ClassDamageReductionPercentPVP = stream.ReadValueF32(); //380
                PrimaryResourceBase = stream.ReadValueF32(); //408
                PrimaryResourceFactorLevel = stream.ReadValueF32(); //484
                PrimaryResourceRegen = stream.ReadValueF32(); //488
                SecondaryResourceBase = stream.ReadValueF32(); //492
                SecondaryResourceFactorLevel = stream.ReadValueF32(); //500
                SecondaryResourceRegen = stream.ReadValueF32(); //536
                Armor = stream.ReadValueF32(); //540
                Dmg = stream.ReadValueF32(); //548
                WalkingRate = stream.ReadValueF32(); //584
                RunningRate = stream.ReadValueF32(); //588
                SprintRate = stream.ReadValueF32(); //592
                ProjRate = stream.ReadValueF32(); //596
                CritDamageCap = stream.ReadValueF32(); //600
                CritPercentBase = stream.ReadValueF32(); //604
                CritPercentCap = stream.ReadValueF32(); //612
                DodgeRatingBase = stream.ReadValueF32(); //616
                GetHitMaxBase = stream.ReadValueF32(); //680
                GetHitMaxPerLevel = stream.ReadValueF32(); //692
                GetHitRecoveryBase = stream.ReadValueF32(); //696
                GetHitRecoveryPerLevel = stream.ReadValueF32(); //700
                ResistPhysical = stream.ReadValueF32(); //704
                ResistFire = stream.ReadValueF32(); //720
                ResistLightning = stream.ReadValueF32(); //724
                ResistCold = stream.ReadValueF32(); //728
                ResistPoison = stream.ReadValueF32(); //772
                ResistArcane = stream.ReadValueF32();
                ResistChill = stream.ReadValueF32();
                ResistStun = stream.ReadValueF32();
                KnockbackWeight = stream.ReadValueF32();
                OOCHealthRegen = stream.ReadValueF32();
                OOCManaRegen = stream.ReadValueF32();
                PotionDilutionDuration = stream.ReadValueF32();
                PotionDilutionScalar = stream.ReadValueF32();
                DualWieldBothAttackChance = stream.ReadValueF32();
                Freeze_Capacity = stream.ReadValueF32();
                Thaw_Rate = stream.ReadValueF32();
            }

            public enum Resource : int
            {
                None = -1,
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


        }
        public class MovementStyle : ISerializableData //0 byte file
        {
            //Total Length: 384
            [PersistentPropertyAttribute("Name")]
            public string Name { get; private set; }

            [PersistentPropertyAttribute("I0")]
            public int I0 { get; private set; }

            [PersistentPropertyAttribute("I1")]
            public int I1 { get; private set; }

            [PersistentPropertyAttribute("I2")]
            public int I2 { get; private set; }

            [PersistentPropertyAttribute("I3")]
            public int I3 { get; private set; }

            [PersistentPropertyAttribute("I4")]
            public int I4 { get; private set; }

            [PersistentPropertyAttribute("I5")]
            public int I5 { get; private set; }

            [PersistentPropertyAttribute("I6")]
            public int I6 { get; private set; }

            [PersistentPropertyAttribute("I7")]
            public int I7 { get; private set; }

            [PersistentPropertyAttribute("I8")]
            public int I8 { get; private set; }

            [PersistentPropertyAttribute("I9")]
            public int I9 { get; private set; }

            [PersistentPropertyAttribute("F0")]
            public float F0 { get; private set; }

            [PersistentPropertyAttribute("F1")]
            public float F1 { get; private set; }

            [PersistentPropertyAttribute("F2")]
            public float F2 { get; private set; }

            [PersistentPropertyAttribute("F3")]
            public float F3 { get; private set; }

            [PersistentPropertyAttribute("F4")]
            public float F4 { get; private set; }

            [PersistentPropertyAttribute("F5")]
            public float F5 { get; private set; }

            [PersistentPropertyAttribute("F6")]
            public float F6 { get; private set; }

            [PersistentPropertyAttribute("F7")]
            public float F7 { get; private set; }

            [PersistentPropertyAttribute("F8")]
            public float F8 { get; private set; }

            [PersistentPropertyAttribute("F9")]
            public float F9 { get; private set; }

            [PersistentPropertyAttribute("F10")]
            public float F10 { get; private set; }

            [PersistentPropertyAttribute("F11")]
            public float F11 { get; private set; }

            [PersistentPropertyAttribute("F12")]
            public float F12 { get; private set; }

            [PersistentPropertyAttribute("F13")]
            public float F13 { get; private set; }

            [PersistentPropertyAttribute("F14")]
            public float F14 { get; private set; }

            [PersistentPropertyAttribute("F15")]
            public float F15 { get; private set; }

            [PersistentPropertyAttribute("F16")]
            public float F16 { get; private set; }

            [PersistentPropertyAttribute("F17")]
            public float F17 { get; private set; }

            [PersistentPropertyAttribute("F18")]
            public float F18 { get; private set; }

            [PersistentPropertyAttribute("F19")]
            public float F19 { get; private set; }

            [PersistentPropertyAttribute("F20")]
            public float F20 { get; private set; }

            [PersistentPropertyAttribute("F21")]
            public float F21 { get; private set; }

            [PersistentPropertyAttribute("SNOPowerToBreakObjects")]
            public int SNOPowerToBreakObjects { get; private set; }

            public void Read(MpqFileStream stream)
            {
                stream.Position += 4;
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
                I4 = stream.ReadValueS32();
                I5 = stream.ReadValueS32();
                I6 = stream.ReadValueS32();
                I7 = stream.ReadValueS32();
                I8 = stream.ReadValueS32();
                I9 = stream.ReadValueS32();
                F0 = stream.ReadValueF32();
                F1 = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                F3 = stream.ReadValueF32();
                F4 = stream.ReadValueF32();
                F5 = stream.ReadValueF32();
                F6 = stream.ReadValueF32();
                F7 = stream.ReadValueF32();
                F8 = stream.ReadValueF32();
                F9 = stream.ReadValueF32();
                F10 = stream.ReadValueF32();
                F11 = stream.ReadValueF32();
                F12 = stream.ReadValueF32();
                F13 = stream.ReadValueF32();
                F14 = stream.ReadValueF32();
                F15 = stream.ReadValueF32();
                F16 = stream.ReadValueF32();
                F17 = stream.ReadValueF32();
                F18 = stream.ReadValueF32();
                F19 = stream.ReadValueF32();
                F20 = stream.ReadValueF32();
                F21 = stream.ReadValueF32();
                SNOPowerToBreakObjects = stream.ReadValueS32();
            }
        }
        public class LabelGBIDTable : ISerializableData
        {
            //Total Length: 264
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int I2 { get; private set; }
            public int I3 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                //stream.Position += 4;
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
            }
        }
        public class LootDistributionTableEntry : ISerializableData //0 byte file
        {
            //Total Length: 92
            [PersistentPropertyAttribute("I0")]
            public int I0 { get; private set; }

            [PersistentPropertyAttribute("I1")]
            public int I1 { get; private set; }

            [PersistentPropertyAttribute("I2")]
            public int I2 { get; private set; }

            [PersistentPropertyAttribute("I3")]
            public int I3 { get; private set; }

            [PersistentPropertyAttribute("I4")]
            public int I4 { get; private set; }

            [PersistentPropertyAttribute("I5")]
            public int I5 { get; private set; }

            [PersistentPropertyAttribute("I6")]
            public int I6 { get; private set; }

            [PersistentPropertyAttribute("I7")]
            public int I7 { get; private set; }

            [PersistentPropertyAttribute("I8")]
            public int I8 { get; private set; }

            [PersistentPropertyAttribute("I9")]
            public int I9 { get; private set; }

            [PersistentPropertyAttribute("F0")]
            public float F0 { get; private set; }

            [PersistentPropertyAttribute("F1")]
            public float F1 { get; private set; }

            [PersistentPropertyAttribute("F2")]
            public float F2 { get; private set; }

            [PersistentPropertyAttribute("F3")]
            public float F3 { get; private set; }

            [PersistentPropertyAttribute("F4")]
            public float F4 { get; private set; }

            [PersistentPropertyAttribute("F5")]
            public float F5 { get; private set; }

            [PersistentPropertyAttribute("F6")]
            public float F6 { get; private set; }

            [PersistentPropertyAttribute("F7")]
            public float F7 { get; private set; }

            [PersistentPropertyAttribute("F8")]
            public float F8 { get; private set; }

            [PersistentPropertyAttribute("F9")]
            public float F9 { get; private set; }

            [PersistentPropertyAttribute("F10")]
            public float F10 { get; private set; }

            [PersistentPropertyAttribute("I10")]
            public int I10 { get; private set; }

            [PersistentPropertyAttribute("I11")]
            public int I11 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
                I4 = stream.ReadValueS32();
                I5 = stream.ReadValueS32();
                I6 = stream.ReadValueS32();
                I7 = stream.ReadValueS32();
                I8 = stream.ReadValueS32();
                I9 = stream.ReadValueS32();
                F0 = stream.ReadValueF32();
                F1 = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                F3 = stream.ReadValueF32();
                F4 = stream.ReadValueF32();
                F5 = stream.ReadValueF32();
                F6 = stream.ReadValueF32();
                F7 = stream.ReadValueF32();
                F8 = stream.ReadValueF32();
                F9 = stream.ReadValueF32();
                F10 = stream.ReadValueF32();
                I10 = stream.ReadValueS32();
                I11 = stream.ReadValueS32();
            }
        }
        public class RareItemNamesTable : ISerializableData
        {
            //Total Length: 272
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public BalanceType Type { get; private set; }
            public int RelatedAffixOrItemType { get; private set; }
            public AffixType AffixType { get; private set; }
            public int I2 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                Type = (BalanceType)stream.ReadValueS32();
                RelatedAffixOrItemType = stream.ReadValueS32();
                AffixType = (AffixType)stream.ReadValueS32();
                I2 = stream.ReadValueS32();
            }
        }
        public class MonsterAffixesTable : ISerializableData
        {
            //Total Length: 792
            public string Name { get; private set; }
            public int Hash { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int I2 { get; private set; }
            public int I3 { get; private set; }
            public int I4 { get; private set; }

            public MonsterAffix MonsterAffix { get; private set; }
            public Resistance Resistance { get; private set; }
            public AffixType AffixType { get; private set; }

            public int I5 { get; private set; }
            public int I6 { get; private set; }
            public int I7 { get; private set; }
            public int I8 { get; private set; }

            public AttributeSpecifier[] Attributes { get; private set; }
            public AttributeSpecifier[] MinionAttributes { get; private set; }
            public int SNOOnSpawnPowerMinion { get; private set; }
            public int SNOOnSpawnPowerChampion { get; private set; }
            public int SNOOnSpawnPowerRare { get; private set; }

            public byte[] BS { get; private set; }

            public void Read(MpqFileStream stream)
            {
                //584
                Name = stream.ReadString(256, true);
                Hash = StringHashHelper.HashItemName(Name);
                I0 = stream.ReadValueS32(); //
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
                I4 = stream.ReadValueS32();
                MonsterAffix = (MonsterAffix)stream.ReadValueS32(); //
                Resistance = (Resistance)stream.ReadValueS32();
                AffixType = (AffixType)stream.ReadValueS32();
                I5 = stream.ReadValueS32();
                I6 = stream.ReadValueS32();
                I7 = stream.ReadValueS32();
                I8 = stream.ReadValueS32();

                Attributes = new AttributeSpecifier[10]; //888
                for (int i = 0; i < 10; i++)
                    Attributes[i] = new AttributeSpecifier(stream);

                MinionAttributes = new AttributeSpecifier[10];
                for (int i = 0; i < 10; i++)
                    MinionAttributes[i] = new AttributeSpecifier(stream);
                stream.Position += 4;
                SNOOnSpawnPowerMinion = stream.ReadValueS32(); //804 - 1372
                SNOOnSpawnPowerChampion = stream.ReadValueS32();
                SNOOnSpawnPowerRare = stream.ReadValueS32();

                BS = new byte[99];
                for (int i = 0; i < BS.Length; i++)
                    BS[i] = stream.ReadValueU8();
                //99 byte's
                //1482 + 1 - 899
                stream.Position += 5;
                //1488
            }
        }
        public class MonsterNamesTable : ISerializableData
        {
            //Total Length: 392
            public string Name { get; private set; }
            public int Hash { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public AffixType AffixType { get; private set; }
            public string S0 { get; private set; }
            public int I2 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                Hash = StringHashHelper.HashItemName(Name);
                I0 = stream.ReadValueS32(); //
                I1 = stream.ReadValueS32();
                AffixType = (AffixType)stream.ReadValueS32();
                S0 = stream.ReadString(128, true);
                I2 = stream.ReadValueS32();
            }

        }
        public class SocketedEffectTable : ISerializableData
        {
            //Total Length: 1416
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int Item { get; private set; }
            public int ItemType { get; private set; }
            public AttributeSpecifier[] Attribute { get; private set; }
            public AttributeSpecifier[] ReqAttribute { get; private set; }
            public string S0 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                Item = stream.ReadValueS32();
                ItemType = stream.ReadValueS32();
                Attribute = new AttributeSpecifier[3];
                for (int i = 0; i < 3; i++)
                    Attribute[i] = new AttributeSpecifier(stream);
                ReqAttribute = new AttributeSpecifier[2];
                for (int i = 0; i < 2; i++)
                    ReqAttribute[i] = new AttributeSpecifier(stream);
                S0 = stream.ReadString(1024, true);
            }
        }
        public class ItemDropTableEntry : ISerializableData //0 byte file
        {
            //Total Length: 1140
            [PersistentProperty("Name")]
            public string Name { get; private set; }

            [PersistentProperty("I0", 221)]
            public int[] I0 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = new int[221];
                for (int i = 0; i < 221; i++)
                    I0[i] = stream.ReadValueS32();
            }
        }
        public class ItemLevelModifier : ISerializableData //0 byte file
        {
            //Total Length: 92
            [PersistentPropertyAttribute("I0")]
            public int I0 { get; private set; }

            [PersistentPropertyAttribute("F0")]
            public float F0 { get; private set; }

            [PersistentPropertyAttribute("F1")]
            public float F1 { get; private set; }

            [PersistentPropertyAttribute("F2")]
            public float F2 { get; private set; }

            [PersistentPropertyAttribute("F3")]
            public float F3 { get; private set; }

            [PersistentPropertyAttribute("F4")]
            public float F4 { get; private set; }

            [PersistentPropertyAttribute("F5")]
            public float F5 { get; private set; }

            [PersistentPropertyAttribute("F6")]
            public float F6 { get; private set; }

            [PersistentPropertyAttribute("F7")]
            public float F7 { get; private set; }

            [PersistentPropertyAttribute("F8")]
            public float F8 { get; private set; }

            [PersistentPropertyAttribute("F9")]
            public float F9 { get; private set; }

            [PersistentPropertyAttribute("F10")]
            public float F10 { get; private set; }

            [PersistentPropertyAttribute("I1")]
            public int I1 { get; private set; }

            [PersistentPropertyAttribute("I2")]
            public int I2 { get; private set; }

            [PersistentPropertyAttribute("I3")]
            public int I3 { get; private set; }

            [PersistentPropertyAttribute("I4")]
            public int I4 { get; private set; }

            [PersistentPropertyAttribute("I5")]
            public int I5 { get; private set; }

            [PersistentPropertyAttribute("I6")]
            public int I6 { get; private set; }

            [PersistentPropertyAttribute("I7")]
            public int I7 { get; private set; }

            [PersistentPropertyAttribute("I8")]
            public int I8 { get; private set; }

            [PersistentPropertyAttribute("I9")]
            public int I9 { get; private set; }

            [PersistentPropertyAttribute("I10")]
            public int I10 { get; private set; }

            [PersistentPropertyAttribute("I11")]
            public int I11 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                I3 = stream.ReadValueS32();
                I4 = stream.ReadValueS32();

                I5 = stream.ReadValueS32();
                I6 = stream.ReadValueS32();
                I7 = stream.ReadValueS32();
                I8 = stream.ReadValueS32();
                I9 = stream.ReadValueS32();

                F0 = stream.ReadValueF32();
                F1 = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                F3 = stream.ReadValueF32();
                F4 = stream.ReadValueF32();

                F5 = stream.ReadValueF32();
                F6 = stream.ReadValueF32();
                F7 = stream.ReadValueF32();
                F8 = stream.ReadValueF32();
                F9 = stream.ReadValueF32();

                F10 = stream.ReadValueF32();

                I10 = stream.ReadValueS32();
                I11 = stream.ReadValueS32();
            }
        }
        public class QualityClass : ISerializableData //0 byte file
        {
            //Total Length: 352
            [PersistentProperty("Name")]
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            [PersistentProperty("F0", 22)]
            public float[] F0 { get; private set; }
            public int I2 { get; private set; }
            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                F0 = new float[22];
                for (int i = 0; i < 22; i++)
                    F0[i] = stream.ReadValueF32();
                I2 = stream.ReadValueS32();
            }
        }
        public class HandicapLevelTable : ISerializableData
        {
            public float HPMod { get; private set; }
            public float DmgMod { get; private set; }
            public float F2 { get; private set; }
            public float XPMod { get; private set; }
            public float GoldMod { get; private set; }

            public float F5 { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public void Read(MpqFileStream stream)
            {
                HPMod = stream.ReadValueF32();
                DmgMod = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                XPMod = stream.ReadValueF32();
                GoldMod = stream.ReadValueF32();
                F5 = stream.ReadValueF32();
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
            }
        }
        public class ItemSalvageLevelTable : ISerializableData
        {
            public int TreasureClassSNO0 { get; private set; }
            public int TreasureClassSNO1 { get; private set; }
            public int TreasureClassSNO2 { get; private set; }
            public int TreasureClassSNO3 { get; private set; }
            public void Read(MpqFileStream stream)
            {
                TreasureClassSNO0 = stream.ReadValueS32();
                TreasureClassSNO1 = stream.ReadValueS32();
                TreasureClassSNO2 = stream.ReadValueS32();
                TreasureClassSNO3 = stream.ReadValueS32();
            }
        }
        public class HirelingTable : ISerializableData
        {
            //Total Length: 824
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int SNOActor { get; private set; }
            public int SNOProxy { get; private set; }
            public int SNOInventory { get; private set; }
            public int TreasureClassSNO { get; private set; }
            public PrimaryAttribute Attribute { get; private set; }
            public float F0 { get; private set; }
            public float F1 { get; private set; }
            public float F2 { get; private set; }
            public float F3 { get; private set; }
            public float F4 { get; private set; }

            public float F5 { get; private set; }
            public float F6 { get; private set; }
            public float F7 { get; private set; }
            public float F8 { get; private set; }
            public float F9 { get; private set; }

            public float F10 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                //
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                SNOActor = stream.ReadValueS32();
                SNOProxy = stream.ReadValueS32();
                SNOInventory = stream.ReadValueS32();
                TreasureClassSNO = stream.ReadValueS32();
                Attribute = (PrimaryAttribute)stream.ReadValueS32();
                F0 = stream.ReadValueF32();
                F1 = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                F3 = stream.ReadValueF32();
                F4 = stream.ReadValueF32();
                F5 = stream.ReadValueF32();
                F6 = stream.ReadValueF32();
                F7 = stream.ReadValueF32();
                F8 = stream.ReadValueF32();
                F9 = stream.ReadValueF32();
                F10 = stream.ReadValueF32();
            }

        }
        public class SetItemBonusTable : ISerializableData
        {
            //Total Length: 464
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public int Set { get; private set; }
            public int Count { get; private set; }
            public AttributeSpecifier[] Attribute { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                Set = stream.ReadValueS32();
                Count = stream.ReadValueS32();
                Attribute = new AttributeSpecifier[8];
                for (int i = 0; i < 8; i++)
                    Attribute[i] = new AttributeSpecifier(stream);
            }

        }
        public class EliteModifier : ISerializableData //0 byte file
        {
            //Total Length: 344

            [PersistentProperty("Name")]
            public string Name { get; private set; }

            public float I0 { get; private set; }
            public float I1 { get; private set; }

            [PersistentProperty("F0")]
            public float F0 { get; private set; }

            [PersistentProperty("Time0")]
            public int Time0 { get; private set; }

            [PersistentProperty("F1")]
            public float F1 { get; private set; }

            [PersistentProperty("Time1")]
            public int Time1 { get; private set; }

            [PersistentProperty("F2")]
            public float F2 { get; private set; }

            [PersistentProperty("Time2")]
            public int Time2 { get; private set; }

            [PersistentProperty("F3")]
            public float F3 { get; private set; }

            [PersistentProperty("Time3")]
            public int Time3 { get; private set; }

            [PersistentProperty("F4")]
            public float F4 { get; private set; }

            [PersistentProperty("Time4")]
            public int Time4 { get; private set; }

            [PersistentProperty("F5")]
            public float F5 { get; private set; }

            [PersistentProperty("Time5")]
            public int Time5 { get; private set; }

            [PersistentProperty("F6")]
            public float F6 { get; private set; }

            [PersistentProperty("Time6")]
            public int Time6 { get; private set; }

            [PersistentProperty("F7")]
            public float F7 { get; private set; }

            [PersistentProperty("F8")]
            public float F8 { get; private set; }

            [PersistentProperty("Time7")]
            public int Time7 { get; private set; }

            [PersistentProperty("F9")]
            public float F9 { get; private set; }

            [PersistentProperty("F10")]
            public float F10 { get; private set; }

            [PersistentProperty("F11")]
            public float F11 { get; private set; }

            [PersistentProperty("F12")]
            public float F12 { get; private set; }

            public float I2 { get; private set; }


            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);

                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();

                F0 = stream.ReadValueF32();
                Time0 = stream.ReadValueS32();
                F1 = stream.ReadValueF32();
                Time1 = stream.ReadValueS32();
                F2 = stream.ReadValueF32();
                Time2 = stream.ReadValueS32();
                F3 = stream.ReadValueF32();
                Time3 = stream.ReadValueS32();
                F4 = stream.ReadValueF32();
                Time4 = stream.ReadValueS32();
                F5 = stream.ReadValueF32();
                Time5 = stream.ReadValueS32();
                F6 = stream.ReadValueF32();
                Time6 = stream.ReadValueS32();
                F7 = stream.ReadValueF32();
                F8 = stream.ReadValueF32();
                Time7 = stream.ReadValueS32();
                F9 = stream.ReadValueF32();
                F10 = stream.ReadValueF32();
                F11 = stream.ReadValueF32();
                F12 = stream.ReadValueF32();
                I2 = stream.ReadValueS32();
            }
        }
        public class ItemTier : ISerializableData //0 byte file
        {
            //Total Length: 32

            [PersistentPropertyAttribute("Head")]
            public int Head { get; private set; }

            [PersistentPropertyAttribute("Torso")]
            public int Torso { get; private set; }

            [PersistentPropertyAttribute("Feet")]
            public int Feet { get; private set; }

            [PersistentPropertyAttribute("Hands")]
            public int Hands { get; private set; }

            [PersistentPropertyAttribute("Shoulders")]
            public int Shoulders { get; private set; }

            [PersistentPropertyAttribute("Bracers")]
            public int Bracers { get; private set; }

            [PersistentPropertyAttribute("Belt")]
            public int Belt { get; private set; }

            [PersistentPropertyAttribute("Necessary")]
            public int Necesssary { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Head = stream.ReadValueS32();
                Torso = stream.ReadValueS32();
                Feet = stream.ReadValueS32();
                Hands = stream.ReadValueS32();

                Shoulders = stream.ReadValueS32();
                Bracers = stream.ReadValueS32();
                Belt = stream.ReadValueS32();
                Necesssary = stream.ReadValueS32();
            }
        }
        public class PowerFormulaTable : ISerializableData
        {
            //Total Length: 1268
            public string S0 { get; private set; }
            public float[] F0 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                S0 = stream.ReadString(1024, true);
                F0 = new float[76];
                for (int i = 0; i < 76; i++)
                    F0[i] = stream.ReadValueF32();
            }
        }
        public class RecipeTable : ISerializableData
        {
            //Total Length: 332
            public int Hash { get; private set; }
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }
            public int SNORecipe { get; private set; }
            public RecipeType CrafterType { get; private set; }
            public int Flags { get; private set; }
            public int Level { get; private set; }
            public int Gold { get; private set; }
            public int NumIngredients { get; private set; }
            public RecipeIngredient[] Ingredients { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                Hash = StringHashHelper.HashItemName(Name);
                GBID = stream.ReadValueS32();
                PAD = stream.ReadValueS32();
                SNORecipe = stream.ReadValueS32();
                CrafterType = (RecipeType)stream.ReadValueS32();
                Flags = stream.ReadValueS32();
                Level = stream.ReadValueS32();
                Gold = stream.ReadValueS32();
                NumIngredients = stream.ReadValueS32();
                Ingredients = new RecipeIngredient[6];
                for (int i = 0; i < 6; i++)
                    Ingredients[i] = new RecipeIngredient(stream);
            }
        }
        public class ScriptedAchievementEventsTable : ISerializableData
        {
            //Total Length: 264
            [PersistentPropertyAttribute("Name")]
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                GBID = stream.ReadValueS32();
                PAD = stream.ReadValueS32();
            }
        }
        public class LootRunQuestTierTable : ISerializableData
        {
            public string Name { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public LootRunQuestTierEntry[] LootRunQuestTierEntrys { get; private set; }
            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                LootRunQuestTierEntrys = new LootRunQuestTierEntry[16];
                for (int i = 0; i < 16; i++)
                    LootRunQuestTierEntrys[i] = new LootRunQuestTierEntry(stream);
            }
        }
        public class ParagonBonusesTable : ISerializableData
        {
            public string Name { get; private set; }
            public int Hash { get; private set; }
            public int I1 { get; private set; }
            public int I2 { get; private set; }
            public AttributeSpecifier[] AttributeSpecifiers { get; private set; }
            public int Category { get; private set; }
            public int Index { get; private set; }
            public Class HeroClass { get; private set; }
            public string IconName { get; private set; }
            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                Hash = stream.ReadValueS32();
                if (Hash == 0)
                { 
                    Hash = StringHashHelper.HashItemName(Name);
                }
                I1 = stream.ReadValueS32();
                I2 = stream.ReadValueS32();
                stream.Position += 4;
                AttributeSpecifiers = new AttributeSpecifier[4]; //856
                for (int i = 0; i < 4; i++)
                    AttributeSpecifiers[i] = new AttributeSpecifier(stream);
                Category = stream.ReadValueS32(); //368 + 584 = 952
                Index = stream.ReadValueS32();
                HeroClass = (Class)stream.ReadValueS32();
                IconName = stream.ReadString(256, true);
                stream.Position += 4;
                //640 + 584 = 1224
            }
        }
        public class LegacyItemConversionTable : ISerializableData
        {
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }
            public int OldItemGBID { get; private set; }
            public int NewItemGBID { get; private set; }
            public int ConsoleIgnore { get; private set; }
            public int Pad { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                GBID = stream.ReadValueS32();
                PAD = stream.ReadValueS32();
                OldItemGBID = stream.ReadValueS32();
                NewItemGBID = stream.ReadValueS32();
                ConsoleIgnore = stream.ReadValueS32();
                Pad = stream.ReadValueS32();
            }
        }
        public class EnchantItemAffixUseCountCostScalarsTable : ISerializableData
        {
            public int UseCount { get; private set; }
            public float CostMultiplier { get; private set; }

            public void Read(MpqFileStream stream)
            {
                UseCount = stream.ReadValueS32();
                CostMultiplier = stream.ReadValueF32();
            }
        }
        public class TieredLootRunLevelTable : ISerializableData
        {
            public float F0 { get; private set; }
            public float F1 { get; private set; }
            public float F2 { get; private set; }
            public float F3 { get; private set; }
            public float F4 { get; private set; }
            public float F5 { get; private set; }
            public float F6 { get; private set; }
            public float F7 { get; private set; }
            public int I0 { get; private set; }
            public int I1 { get; private set; }
            public long L0 { get; private set; }
            public float F8 { get; private set; }
            public float F9 { get; private set; }

            public void Read(MpqFileStream stream)
            {
                F0 = stream.ReadValueF32();
                F1 = stream.ReadValueF32();
                F2 = stream.ReadValueF32();
                F3 = stream.ReadValueF32();
                F4 = stream.ReadValueF32();
                F5 = stream.ReadValueF32();
                F6 = stream.ReadValueF32();
                F7 = stream.ReadValueF32();
                I0 = stream.ReadValueS32();
                I1 = stream.ReadValueS32();
                L0 = stream.ReadValueS64();
                F8 = stream.ReadValueF32();
                F9 = stream.ReadValueF32();
            }
        }
        public class TransmuteRecipesTable : ISerializableData
        {
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }
            public TransmuteType TransmuteType { get; private set; }
            public TransmuteRecipeIngredient[] TransmuteRecipeIngredients { get; private set; }
            public int IngredientsCount { get; private set; }
            public int Page { get; private set; }
            public int Hidden { get; private set; }

            public void Read(MpqFileStream stream)
            {
                Name = stream.ReadString(256, true);
                GBID = stream.ReadValueS32();
                PAD = stream.ReadValueS32();
                TransmuteType = (TransmuteType)stream.ReadValueS32();
                TransmuteRecipeIngredients = new TransmuteRecipeIngredient[8];
                for (int i = 0; i < TransmuteRecipeIngredients.Length; i++)
                    TransmuteRecipeIngredients[i] = new TransmuteRecipeIngredient(stream);
                IngredientsCount = stream.ReadValueS32();
                Page = stream.ReadValueS32();
                Hidden = stream.ReadValueS32();
                //stream.Position += 8;
            }
        }
        public class CurrencyConversionTable : ISerializableData
        {
            public string Name { get; private set; }
            public int GBID { get; private set; }
            public int PAD { get; private set; }
            public CurrencyType CurrencyType { get; private set; }
            public int[] LinkedItemsGBIDs { get; private set; }
            public int SortOrder { get; private set; }
            public int Hidden { get; private set; }
            public int AutoPickup { get; private set; }

            public void Read(MpqFileStream stream)
            {

                Name = stream.ReadString(256, true);
                GBID = stream.ReadValueS32();
                PAD = stream.ReadValueS32();
                CurrencyType = (CurrencyType)stream.ReadValueS32();
                LinkedItemsGBIDs = new int[5];
                for (int i = 0; i < LinkedItemsGBIDs.Length; i++)
                    LinkedItemsGBIDs[i] = stream.ReadValueS32();
                SortOrder = stream.ReadValueS32(); //872
                Hidden = stream.ReadValueS32();
                AutoPickup = stream.ReadValueS32();
                stream.Position += 4;
            }
        }


        [Flags]
        public enum ItemFlags
        {
            NotEquipable1 = 0x1,
            AtLeastMagical = 0x2,
            Gem = 0x8,
            NotEquipable2 = 0x40,
            Socketable = 0x80,
            Barbarian = 0x100,
            Wizard = 0x200,
            WitchDoctor = 0x400,
            DemonHunter = 0x800,
            Unknown = 0x1000,
            Monk = 0x2000,
            Unknown1 = 0x4000,
            Unknown2 = 0x8000,
            Unknown3 = 0x10000,
            Crusader = 0x20000,
            Necromancer = 0x100000,
        }
        public enum DamageAffixType
        {
            None = 0,
            Lightning = 1,
            Cold = 2,
            Fire = 3,
            Poison = 4,
            Arcane = 5,
            WitchdoctorDamage = 6,
            LifeSteal = 7,
            ManaSteal = 8,
            MagicFind = 9,
            GoldFind = 10,
            AttackSpeedBonus = 11,
            CastSpeedBonus = 12,
            Holy = 13,
            WizardDamage = 14
        }
        public enum Resistance
        {
            None = -1,
            Physical = 0,
            Fire = 1,
            Lightning = 2,
            Cold = 3,
            Poison = 4,
            Arcane = 5,
            Holy = 6
        }
        public enum Class
        {
            None = -1,
            DemonHunter = 0,
            Barbarian = 1,
            Wizard = 2,
            Witchdoctor = 3,
            Monk = 4,
            Crusader = 5,
            Necromancer = 6
        }
        public enum AffixType
        {
            Prefix = 0,
            Suffix = 1,
            Inherit = 2,
            Title = 5,
            Quality = 6,
            Immunity = 7,
            Random = 9,
            Enhancement = 10,
            SocketEnhancement = 11,
        }
        public enum eItemType
        {
            PlayerBackpack = 0,
            PlayerHead = 1,
            PlayerTorso = 2,
            PlayerRightHand = 3,
            PlayerLeftHand = 4,
            PlayerHands = 5,
            PlayerWaist = 6,
            PlayerFeet = 7,
            PlayerShoulders = 8,
            PlayerLegs = 9,
            PlayerBracers = 10,
            PlayerLeftFinger = 12,
            PlayerRightFinger = 11,
            PlayerNeck = 13,
            Merchant = 18,
            PetRightHand = 20,
            PetLeftHand = 21,
            PetSpecial = 22,
            PetNeck = 23,
            PetRightFinger = 24,
            PetLeftFinger = 25
        }
        public enum MonsterAffix
        {
            All = 0,
            Rares = 1,
            Shooters = 2,
            Champions = 3
        }
        public enum PrimaryAttribute : int
        {
            None = -1,
            Strength = 0,
            Dexterity = 1,
            Intelligence = 2
        }
        public enum RecipeType
        {
            None = -1,
            Blacksmith = 0,
            Jeweler = 1,
            Mystic = 2,
            JewelUpgrade = 4,
            Horadrim = 3
        }
        public enum TransmuteType
        {
            None = -1,
            EXTRACT_LEGENDARY_POWER = 0,
            REFORGE_LEGENDARY = 1,
            UPGRADE_RARE_ITEM = 2,
            CONVERT_SET_ITEM = 3,
            REMOVE_LEVEL_REQ = 4,
            CONVERT_CRAFTING_MATS_FROM_NORMAL = 5,
            CONVERT_CRAFTING_MATS_FROM_MAGIC = 6,
            CONVERT_CRAFTING_MATS_FROM_RARE = 7,
            UPGRADE_ITEM_TO_PLAYER_LEVEL = 9,
            CONVERT_GEMS = 8,
            OPEN_PORTAL_TO_GREED = 10,
            OPEN_PORTAL_TO_COW = 11,
            OPEN_PORTAL_TO_WHIMSEYSHIRE = 12,
            AUGMENT_ARMOR = 13,
            CONVERT_BLACKROCK_PAGES = 14,
            INFUSE_LEGENDARY_POWER = 15
        }
        public enum CurrencyType
        {
            None = -1,
            Gold = 0,
            BloodShards = 1,
            Platinum = 2,
            ReusableParts = 3,
            ArcaneDust = 4,
            VeiledCrystal = 5,
            DeathsBreath = 6,
            ForgottenSoul = 7,
            KhanduranRune = 8,
            CaldeumNightshade = 9,
            ArreatWarTapestry = 10,
            CorruptedAngelFlesh = 11,
            WestmarchHolyWater = 12,
            DemonOrganDiablo = 13,
            DemonOrganGhom = 14,
            DemonOrganSiegeBreaker = 15,
            DemonOrganSkeletonKing = 16,
            DemonOrganEye = 17,
            DemonOrganSpineCord = 18,
            DemonOrganTooth = 19
        }
        public class RecipeIngredient
        {
            //Length: 8
            public int ItemsGBID { get; private set; }
            public int Count { get; private set; }

            public RecipeIngredient(MpqFileStream stream)
            {
                ItemsGBID = stream.ReadValueS32();
                Count = stream.ReadValueS32();
            }
        }
        public class TransmuteRecipeIngredient
        {
            public int Type { get; private set; }
            public int TypeValue { get; private set; }
            public int Quantity { get; private set; }

            public TransmuteRecipeIngredient(MpqFileStream stream)
            {
                Type = stream.ReadValueS32();
                TypeValue = stream.ReadValueS32();
                Quantity = stream.ReadValueS32();
            }
        }
        public class LootRunQuestTierEntry
        {
            public int QuestSNO { get; private set; }
            public float F0 { get; private set; }
            public int GBID { get; private set; }
            public int ItemsGBID { get; private set; }

            public LootRunQuestTierEntry(MpqFileStream stream)
            {
                QuestSNO = stream.ReadValueS32();
                F0 = stream.ReadValueF32();
                GBID = stream.ReadValueS32();
                ItemsGBID = stream.ReadValueS32();
            }
        }
        public class AttributeSpecifier
        {
            //Length: 24
            public int AttributeId { get; private set; }
            public int SNOParam { get; private set; }
            public List<int> Formula { get; private set; }

            public AttributeSpecifier(MpqFileStream stream)
            {
                AttributeId = stream.ReadValueS32();
                SNOParam = stream.ReadValueS32();
                stream.Position += 8;
                Formula = stream.ReadSerializedInts();
            }
        }
    }
}
