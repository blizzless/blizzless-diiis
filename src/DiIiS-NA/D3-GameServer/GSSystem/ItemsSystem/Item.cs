
using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.MessageSystem;
using Actor = DiIiS_NA.GameServer.GSSystem.ActorSystem.Actor;
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
    public class Item : Actor
    {
        private static readonly Logger Logger = LogManager.CreateLogger(nameof(Item));

        public DBInventory DBInventory = null;
        public bool ItemHasChanges
        {
            get;
            private set; //needed in Future, set this to true if Item affixes or item attributes have changed.
        }
        
        public override ActorType ActorType => ActorType.Item;

        public Actor Owner { get; set; } // Only set when the _actor_ has the item in its inventory. /fasbat

        public ItemTable ItemDefinition => ItemGenerator.GetItemDefinition(GBHandle.GBID);

        public ItemTypeTable ItemType => ItemGroup.FromHash(ItemDefinition.ItemTypesGBID);

        public bool Unidentified
        {
            get => Attributes[GameAttribute.Unidentified];
            set 
            { 
                Attributes[GameAttribute.Unidentified] = value;
                if (DBInventory is {} dbInventory) dbInventory.Unidentified = value;
            }
        }

        public int EquipGemType
        {
            get
            {
                if (IsWeapon(ItemType)) return 485534122;
                if (IsHelm(ItemType)) return 3851110;
                return 109305;
            }
        }

        public ItemRandomHelper RandomGenerator { get; set; }
        public int ItemLevel { get; private set; }

        public List<Item> Gems = new();

        public int Rating
        {
            get => AffixList.Select(a => a.Rating).Sum() + (int)Gems.Select(g => g.ItemDefinition.Cost * 6f).Sum();
            set => Logger.Warn("Rating is readonly");
        }

        public D3.Items.RareItemName RareItemName = null;

        public ItemState CurrentState { get; set; }

        public int EquipmentSlot { get; private set; }

        public Vector2D InventoryLocation
        {
            get;
            private set;
        } // Column, row; NOTE: Call SetInventoryLocation() instead of setting fields on this

        public override int Quality
        {
            get => Attributes[GameAttribute.Item_Quality_Level];
            set => Attributes[GameAttribute.Item_Quality_Level] = value;
        }

        public SNOHandle SnoFlippyActory =>
            ActorData.TagMap.ContainsKey(ActorKeys.Flippy) ? ActorData.TagMap[ActorKeys.Flippy] : null;

        public SNOHandle SnoFlippyParticle =>
            ActorData.TagMap.ContainsKey(ActorKeys.FlippyParticle)
                ? ActorData.TagMap[ActorKeys.FlippyParticle]
                : null;

        public override bool HasWorldLocation => Owner == null;

        public override InventoryLocationMessageData InventoryLocationMessage(Player plr)
        {
            return new InventoryLocationMessageData
            {
                OwnerID = Owner?.DynamicID(plr) ?? 0,
                EquipmentSlot = EquipmentSlot,
                InventoryLocation = InventoryLocation
            };
        }

        public bool IsStackable()
        {
            return ItemDefinition.MaxStackSize > 1;
        }

        public InvLoc InvLoc(Player plr)
        {
            return new InvLoc
            {
                OwnerID = Owner?.DynamicID(plr) ?? 0,
                EquipmentSlot = EquipmentSlot,
                Row = InventoryLocation.Y,
                Column = InventoryLocation.X
            };
        }

        public List<int> AffixFamilies = new();

        public Item(World world, ItemTable definition, IEnumerable<Affix> affixList, string serializedGameAttributeMap,
            int count = 1)
            : base(world, (ActorSno)definition.SNOActor)
        {
            GBHandle.GBID = definition.Hash;
            SetInitialValues(definition);
            Attributes.FillBySerialized(serializedGameAttributeMap);
            if (Attributes[GameAttribute.Seed] == 0)
            {
                Attributes[GameAttribute.Seed] = FastRandom.Instance.Next();
                //this.Attributes[GameAttribute.Seed] = 0;
                Attributes[GameAttribute.Item_Quality_Level] = 1;
                Attributes[GameAttribute.TeamID] = 0;
            }
            //for (int i = 50; i < 60; i++)
            //Attributes[GameAttribute.Requirement, 57] = 10;


            Attributes[GameAttribute.ItemStackQuantityLo] = count;
            Attributes[GameAttribute.Loot_2_0_Drop] = true;
            AffixList.Clear();
            AffixList.AddRange(affixList);
            //this.Attributes[GameAttribute.EnchantAffix] = -758203990;
            //this.Attributes[GameAttribute.EnchantAffix, 0] = -758203990;
            //this.Attributes[GameAttribute.EnchantAffix, 1] = -758203990;
            //this.Attributes[GameAttribute.EnchantRangeVal] = 1;
            //*
            if (IsArmor(ItemType) || IsWeapon(ItemType) || IsOffhand(ItemType) ||
                (IsPotion(ItemType) && ItemDefinition.Name.Contains("Legendary")) || IsAccessory(ItemType))
            {
                //Attributes[GameAttribute.Requirement, 64] = 0;
                var reqLevel = definition.RequiredLevel % 10 != 0
                    ? definition.RequiredLevel - 1
                    : definition.RequiredLevel;
                var level = Math.Max(AffixList.Any() ? AffixList.Select(a => a.ItemLevel).Max() : 0, reqLevel);
                Attributes[GameAttribute.Requirement, 57] =
                    Math.Max(level - Attributes[GameAttribute.Item_Level_Requirement_Reduction], 0);
            }

            if (AffixList.Count > 0)
            {
                if (Math.Abs(Attributes[GameAttribute.Requirement, 57] - AffixList[0].Definition.OverrideLevelReq) > 0.001 &&
                    AffixList[0].Definition.OverrideLevelReq != 0)
                    Attributes[GameAttribute.Requirement, 57] = AffixList[0].Definition.OverrideLevelReq;
                foreach (var affix in AffixList)
                    if (affix.Definition.OverrideLevelReq > Attributes[GameAttribute.Requirement, 57])
                        Attributes[GameAttribute.Requirement, 57] = affix.Definition.OverrideLevelReq;
            }
            //*/
            /*
            Attributes[GameAttribute.Item_Quality_Level] = 1;
            if (Item.IsArmor(this.ItemType) || Item.IsWeapon(this.ItemType) || Item.IsOffhand(this.ItemType))
                Attributes[GameAttribute.Item_Quality_Level] = RandomHelper.Next(6);
            if (this.ItemType.Flags.HasFlag(ItemFlags.AtLeastMagical) && Attributes[GameAttribute.Item_Quality_Level] < 3)
                Attributes[GameAttribute.Item_Quality_Level] = 3;
            */
            //Attributes[GameAttribute.ItemStackQuantityLo] = 1;
            //Attributes[GameAttribute.Seed] = RandomHelper.Next(); //unchecked((int)2286800181);
        }


        private void SetInitialValues(ItemTable definition)
        {
            ItemLevel = definition.ItemLevel;
            GBHandle.Type = (int)ActorType.Gizmo;
            EquipmentSlot = 0;
            InventoryLocation = new Vector2D { X = 0, Y = 0 };
            Scale = 1f;
            RotationW = 0.0f;
            RotationAxis.Set(0.0f, 0.0f, 1.0f);
            CurrentState = ItemState.Normal;
            //flags: 0x01 - socketable, 0x20 - blinking
            Field2 = 0x1f;
            Field7 = -1;
            CollFlags = 0;
            NameSNO = ActorSno.__NONE;
            Field10 = 0x00;

            Attributes[GameAttribute.TeamID] = 0;
        }

        public Item(World world, ItemTable definition, int forceQualityLevel = -1, bool crafted = false, int seed = -1)
            : base(world, (ActorSno)definition.SNOActor)
        {
            GBHandle.GBID = definition.Hash;
            SetInitialValues(definition);
            ItemHasChanges = true;
            Attributes[GameAttribute.IsCrafted] = crafted;
            Attributes[GameAttribute.Item_Quality_Level] = 1;

            Attributes[GameAttribute.Loot_2_0_Drop] = true;

            if (IsArmor(ItemType) || IsWeapon(ItemType) || IsOffhand(ItemType) || IsAccessory(ItemType) ||
                IsShard(ItemType))
                Attributes[GameAttribute.Item_Quality_Level] = RandomHelper.Next(8);
            if (ItemType.Usable.HasFlag(ItemFlags.AtLeastMagical) && Attributes[GameAttribute.Item_Quality_Level] < 3)
                Attributes[GameAttribute.Item_Quality_Level] = 3;
            if (definition.Name.ToLower().Contains("unique") || definition.Quality == ItemTable.ItemQuality.Legendary)
                Attributes[GameAttribute.Item_Quality_Level] = 9;
            if (forceQualityLevel > -1)
                Attributes[GameAttribute.Item_Quality_Level] = forceQualityLevel;
            if (definition.SNOSet != -1) Attributes[GameAttribute.Item_Quality_Level] = 9;

            if (ItemDefinition.Name.ToLower().Contains("unique_gem"))
            {
                Attributes[GameAttribute.Item_Quality_Level] = 9;
                if (!Attributes.Contains(GameAttribute.Jewel_Rank))
                    Attributes[GameAttribute.Jewel_Rank] = 1;
                //Attributes[GameAttribute.Jewel_Rank] = 1;
            }

            if (ItemDefinition.Name.ToLower().Contains("norm_season")) Attributes[GameAttribute.Item_Quality_Level] = 9;

            if (ItemDefinition.Name.ToLower().StartsWith("p71_ethereal"))
            {
                Attributes[GameAttribute.Item_Quality_Level] = 9;
                Attributes[GameAttribute.Attacks_Per_Second_Item] += 1.1f;

                Attributes[GameAttribute.Damage_Weapon_Min, 0] = 15 + World.Game.InitialMonsterLevel * 1.7f;
                if (World.Game.InitialMonsterLevel > 70)
                    Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 20f;
                else if (World.Game.InitialMonsterLevel > 60)
                    Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 14f;
                Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 15;
            }


            Attributes[GameAttribute.ItemStackQuantityLo] = 1;
            if (seed == -1)
                Attributes[GameAttribute.Seed] = FastRandom.Instance.Next(); //unchecked((int)2286800181);
            else
                Attributes[GameAttribute.Seed] = seed;

            //Attributes[GameAttribute.Seed] = 0;
            //Attributes[GameAttribute.Item_Quality_Level] = 1;

            RandomGenerator = new ItemRandomHelper(Attributes[GameAttribute.Seed]);
            RandomGenerator.Next();
            if (IsArmor(ItemType))
            {
                if (!crafted)
                    RandomGenerator.Next();
                if (Attributes[GameAttribute.Item_Quality_Level] >= 5 &&
                    Attributes[GameAttribute.Item_Quality_Level] <= 7)
                    RandomGenerator.Next();
            }

            RandomGenerator.ReinitSeed();

            if (IsWeapon(ItemType) && !crafted)
            {
                RandomGenerator.Next();
                RandomGenerator.Next();
            }

            ApplyWeaponSpecificOptions(definition);
            ApplyArmorSpecificOptions(definition);
            ApplyDurability(definition);
            ApplySkills(definition);
            ApplyAttributeSpecifier(definition);

            var affixNumber = 1;
            if (Attributes[GameAttribute.Item_Quality_Level] >= 3)
                affixNumber = Attributes[GameAttribute.Item_Quality_Level] - 2;

            if (ItemDefinition.Name.Contains("Shard"))
                affixNumber = 1;
            if (ItemDefinition.Name.Contains("GreaterShard"))
                affixNumber = 3;

            if (!crafted)
                if (Attributes[GameAttribute.Item_Quality_Level] > 1)
                    AffixGenerator.Generate(this, affixNumber);

            if (IsShard(ItemType))
                Attributes[GameAttribute.Item_Quality_Level] = 1;


            Attributes[GameAttribute.Item_Quality_Level] = Math.Min(Attributes[GameAttribute.Item_Quality_Level], 9);
            Attributes[GameAttribute.Durability_Cur] = Attributes[GameAttribute.Durability_Max];
            /*
            if (Attributes[GameAttribute.Item_Quality_Level] > 8)
            {
                this.Unidentified = true;
                if (!this.ItemDefinition.Name.ToLower().StartsWith("p71_ethereal"))
                    this.Attributes[GameAttribute.Unidentified] = true;
            }
            //*/


#if DEBUG
#else
			//if (Attributes[GameAttribute.Item_Quality_Level] > 6)
			//	this.Unidentified = true;
#endif
            if (Attributes[GameAttribute.Item_Quality_Level] == 9) Attributes[GameAttribute.MinimapActive] = true;

            if (IsArmor(ItemType) || IsWeapon(ItemType) || IsOffhand(ItemType) ||
                (IsPotion(ItemType) && ItemDefinition.Name.Contains("Legendary")) || IsAccessory(ItemType))
            {
                var a = Attributes[GameAttribute.Requirement, 57];
                var reqLevel = definition.RequiredLevel % 10 != 0
                    ? definition.RequiredLevel - 1
                    : definition.RequiredLevel;
                var level = Math.Max(AffixList.Any() ? AffixList.Select(a => a.ItemLevel).Max() : 0, reqLevel);
                Attributes[GameAttribute.Requirement, 57] =
                    Math.Max(level - Attributes[GameAttribute.Item_Level_Requirement_Reduction], 0);
                a = Attributes[GameAttribute.Requirement, 57];
            }

            // Hard rewrite of the required level for legendary weapons, in case of its bug on 70 lvls.
            if (Attributes[GameAttribute.Item_Quality_Level] > 8)
                if (Attributes[GameAttribute.Requirement, 57] == 0)
                    Attributes[GameAttribute.Item_Level_Requirement_Override] = 1;
                else
                    Attributes[GameAttribute.Item_Level_Requirement_Override] =
                        (int)Attributes[GameAttribute.Requirement, 57];

            if (ItemDefinition.Name.ToLower().StartsWith("p71_ethereal"))
                AffixGenerator.AddAffix(this, 1661455571, true); //1661455571
        }


        public void Identify()
        {
            Unidentified = false;
            // DBInventory.Unidentified = false;
            Attributes[GameAttribute.Unidentified] = false;

            Owner.World.Game.GameDBSession.SessionUpdate(DBInventory);
            if (Owner is Player player)
            {
                Unreveal(player);
                Reveal(player);
                if (ItemDefinition.Name.Contains("Unique"))
                {
                    player.UniqueItemIdentified(DBInventory.Id);
                    //if (Program.MaxLevel == 70)
                    player.UnlockTransmog(ItemDefinition.Hash);
                }
            }
        }


        private void ApplyWeaponSpecificOptions(ItemTable definition)
        {
            if (definition.WeaponDamageMin > 0)
            {
                Attributes[GameAttribute.Attacks_Per_Second_Item] += definition.AttacksPerSecond;
                Attributes[GameAttribute.Attacks_Per_Second_Item_Percent] = 0;
                Attributes[GameAttribute.Attacks_Per_Second_Item_Bonus] = 0;
                Attributes[GameAttribute.Damage_Weapon_Min, 0] += definition.WeaponDamageMin;
                Attributes[GameAttribute.Damage_Weapon_Delta, 0] += definition.WeaponDamageDelta;
            }

            var hash = definition.Hash;
            if (definition.Name.Contains("_104"))
                hash = ItemGenerator.GetItemHash(definition.Name.Substring(0, definition.Name.Length - 4));

            if (UniqueItems.UniqueItemStats.ContainsKey(hash))
            {
                Attributes[GameAttribute.Attacks_Per_Second_Item] += UniqueItems.GetDPS(hash);
                Attributes[GameAttribute.Damage_Weapon_Min, 0] += UniqueItems.GetWeaponDamageMin(hash);
                Attributes[GameAttribute.Damage_Weapon_Delta, 0] += UniqueItems.GetWeaponDamageDelta(hash);

                if (IsWeapon(ItemType))
                {
                    if (Attributes[GameAttribute.Damage_Weapon_Min, 0] == 0)
                        Attributes[GameAttribute.Damage_Weapon_Min, 0] = 34;
                    if (Attributes[GameAttribute.Damage_Weapon_Delta, 0] == 0)
                        Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 34;
                }

                var scaleCapMin = 0f;
                var scaleCapDelta = 0f;
                switch (definition.ItemTypesGBID)
                {
                    case 109694: //Axe
                        scaleCapMin = 249f;
                        scaleCapDelta = 461f - scaleCapMin;
                        break;
                    case -262576534: //Dagger
                        scaleCapMin = 107f;
                        scaleCapDelta = 321f - scaleCapMin;
                        break;
                    case 4026134: //Mace
                        scaleCapMin = 316f;
                        scaleCapDelta = 585f - scaleCapMin;
                        break;
                    case 140519163: //Spear
                        scaleCapMin = 357f;
                        scaleCapDelta = 526f - scaleCapMin;
                        break;
                    case 140782159: //Sword
                        scaleCapMin = 168f;
                        scaleCapDelta = 392f - scaleCapMin;
                        break;
                    case -199811863: //Ceremonial Knife
                        scaleCapMin = 117f;
                        scaleCapDelta = 469f - scaleCapMin;
                        break;
                    case -2094596416: //Fist Weapon
                        scaleCapMin = 168f;
                        scaleCapDelta = 392f - scaleCapMin;
                        break;
                    case -1363671135: //Flail
                        scaleCapMin = 192f;
                        scaleCapDelta = 355f - scaleCapMin;
                        break;
                    case -1488678091: //Mighty Weapon
                        scaleCapMin = 249f;
                        scaleCapDelta = 461f - scaleCapMin;
                        break;
                    case 763102523: //Hand Crossbow
                        scaleCapMin = 126f;
                        scaleCapDelta = 714f - scaleCapMin;
                        break;
                    case 4385866: //Wand
                        scaleCapMin = 197f;
                        scaleCapDelta = 357f - scaleCapMin;
                        break;
                    case 110504: //Bow
                        scaleCapMin = 143f;
                        scaleCapDelta = 815f - scaleCapMin;
                        break;
                    case -1338851342: //Crossbow
                        scaleCapMin = 779f;
                        scaleCapDelta = 945f - scaleCapMin;
                        break;
                    case 119458520: //2H Axe
                        scaleCapMin = 1384f;
                        scaleCapDelta = 1685f - scaleCapMin;
                        break;
                    case 89494384: //2H Mace
                        scaleCapMin = 1737f;
                        scaleCapDelta = 1912f - scaleCapMin;
                        break;
                    case -1203595600: //2H Polearm
                        scaleCapMin = 1497f;
                        scaleCapDelta = 1823f - scaleCapMin;
                        break;
                    case 140658708: //2H Staff
                        scaleCapMin = 1229f;
                        scaleCapDelta = 1839f - scaleCapMin;
                        break;
                    case -1307049751: //2H Sword
                        scaleCapMin = 1137f;
                        scaleCapDelta = 1702f - scaleCapMin;
                        break;
                    case -1620551894: //2H Daibo
                        scaleCapMin = 994f;
                        scaleCapDelta = 1845f - scaleCapMin;
                        break;
                    case -1363671102: //2H Flail
                        scaleCapMin = 1351f;
                        scaleCapDelta = 1486f - scaleCapMin;
                        break;
                    case -1488678058: //2H Mighty Weapon
                        scaleCapMin = 1462f;
                        scaleCapDelta = 1609f - scaleCapMin;
                        break;
                }

                if (scaleCapMin > 5 && scaleCapDelta > 5)
                {
                    var ratio = (float)Math.Pow(definition.ItemLevel, 2f) / 4900f;
                    if (ratio < 0.01f) ratio = 0.01f;
                    if (ratio > 1f) ratio = 1f;
                    Attributes[GameAttribute.Damage_Weapon_Min, 0] +=
                        Math.Abs(scaleCapMin * ratio - Attributes[GameAttribute.Damage_Weapon_Min, 0]);
                    Attributes[GameAttribute.Damage_Weapon_Delta, 0] +=
                        Math.Abs(scaleCapDelta * ratio - Attributes[GameAttribute.Damage_Weapon_Delta, 0]);
                }
            }
        }

        private void ApplyArmorSpecificOptions(ItemTable definition)
        {
            if (definition.Armor > 0)
            {
                Attributes[GameAttribute.Armor_Item] += definition.Armor;
                //Attributes[GameAttribute.Armor_Bonus_Item] = 0;
                //Attributes[GameAttribute.Armor_Item_Percent] = 0;
                Attributes[GameAttribute.Armor] += definition.Armor;
                var armorItemTotal = Attributes[GameAttribute.Armor_Item_Total];
            }

            var hash = definition.Hash;
            if (definition.Name.Contains("_104"))
                hash = ItemGenerator.GetItemHash(definition.Name.Substring(0, definition.Name.Length - 4));

            if (UniqueItems.UniqueItemStats.ContainsKey(hash))
            {
                Attributes[GameAttribute.Armor_Item] += UniqueItems.GetArmor(hash);
                //Unique items level scaling
                if (IsArmor(ItemType))
                    if (Attributes[GameAttribute.Armor_Item] == 0)
                        Attributes[GameAttribute.Armor_Item] = 30;

                if (Attributes[GameAttribute.Armor_Item] < 100)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 6;
                else if (Attributes[GameAttribute.Armor_Item] < 200)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 5;
                else if (Attributes[GameAttribute.Armor_Item] < 300)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 4;
                else if (Attributes[GameAttribute.Armor_Item] < 400)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 3;
                else if (Attributes[GameAttribute.Armor_Item] < 500)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 2;
                else if (Attributes[GameAttribute.Armor_Item] < 600)
                    Attributes[GameAttribute.Armor_Item] += definition.ItemLevel;

                if (definition.ItemTypesGBID == 332825721 ||
                    definition.ItemTypesGBID == 602099538) //Shield and CruShield
                {
                    var scaleCapMin = 14000f;
                    var scaleCapDelta = 21000f - scaleCapMin;
                    var ratio = (float)Math.Pow(definition.ItemLevel, 2f) / 4900f;
                    if (ratio < 0.01f) ratio = 0.01f;
                    if (ratio > 1f) ratio = 1f;
                    Attributes[GameAttribute.Block_Amount_Item_Min] +=
                        Math.Abs(scaleCapMin * ratio - Attributes[GameAttribute.Block_Amount_Item_Min, 0]);
                    Attributes[GameAttribute.Block_Amount_Item_Delta] += Math.Abs(scaleCapDelta * ratio -
                        Attributes[GameAttribute.Block_Amount_Item_Delta, 0]);
                }
            }
        }

        private void ApplyDurability(ItemTable definition)
        {
            if (definition.BaseDurability > 0)
            {
                var durability = definition.BaseDurability * 2 + RandomHelper.Next(definition.DurabilityVariance);
                Attributes[GameAttribute.Durability_Max] = durability;
                Attributes[GameAttribute.Durability_Cur] = durability;
            }
        }

        public void UpdateDurability(int newDurability)
        {
            Attributes[GameAttribute.Durability_Cur] = newDurability;
            DBInventory.Durability = newDurability;
            Owner.World.Game.GameDBSession.SessionUpdate(DBInventory);
        }

        public void UpdateTransmog(int newTransmogGBID)
        {
            Attributes[GameAttribute.TransmogGBID] = newTransmogGBID;
            DBInventory.TransmogGBID = newTransmogGBID;
            Owner.World.Game.GameDBSession.SessionUpdate(DBInventory);
        }

        public void SaveAttributes()
        {
            DBInventory.Attributes = Attributes.Serialize();
            Owner.World.Game.GameDBSession.SessionUpdate(DBInventory);
        }

        public void UpdateStackCount(int newCount)
        {
            if (newCount > 0)
            {
                if (DBInventory == null) return;
                Attributes[GameAttribute.ItemStackQuantityLo] = newCount;
                Attributes.SendChangedMessage((Owner as Player).InGameClient);

                DBInventory.Count = newCount;
                Owner.World.Game.GameDBSession.SessionUpdate(DBInventory);
            }
        }

        private void ApplySkills(ItemTable definition)
        {
            if (definition.SNOSkill0 != -1) Attributes[GameAttribute.Skill, definition.SNOSkill0] = 1;

            if (definition.SNOSkill1 != -1) Attributes[GameAttribute.Skill, definition.SNOSkill1] = 1;

            if (definition.SNOSkill2 != -1) Attributes[GameAttribute.Skill, definition.SNOSkill2] = 1;

            if (definition.SNOSkill3 != -1) Attributes[GameAttribute.Skill, definition.SNOSkill3] = 1;
        }

        private void ApplyAttributeSpecifier(ItemTable definition)
        {
            foreach (var effect in definition.Attribute)
            {
                float result;
                if (FormulaScript.Evaluate(effect.Formula.ToArray(), RandomGenerator, out result))
                {
                    //Logger.Debug("Randomized value for attribute " + GameAttribute.Attributes[effect.AttributeId].Name + " is " + result);

                    if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
                    {
                        var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
                        if (effect.SNOParam != -1)
                            Attributes[attr, effect.SNOParam] += result;
                        else
                            Attributes[attr] += result;
                    }
                    else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
                    {
                        var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
                        if (effect.SNOParam != -1)
                            Attributes[attr, effect.SNOParam] += (int)result;
                        else
                            Attributes[attr] += (int)result;
                    }
                }
            }
        }

        // There are 2 VisualItemClasses... any way to use the builder to create a D3 Message?
        public VisualItem CreateVisualItem()
        {
            return new VisualItem()
            {
                GbId = Attributes[GameAttribute.TransmogGBID] == -1
                    ? GBHandle.GBID
                    : Attributes[GameAttribute.TransmogGBID],
                DyeType = Attributes[GameAttribute.DyeType],
                ItemEffectType = 0, //Mooege.Common.Helpers.Math.FastRandom.Instance.Next(1, 14),
                EffectLevel = -1 //Mooege.Common.Helpers.Math.FastRandom.Instance.Next(1, 30)
            };
        }

        //TODO: Move to proper D3.Hero.Visual item classes
        public D3.Hero.VisualItem GetVisualItem()
        {
            var visualItem = D3.Hero.VisualItem.CreateBuilder()
                .SetGbid(Attributes[GameAttribute.TransmogGBID] == -1
                    ? GBHandle.GBID
                    : Attributes[GameAttribute.TransmogGBID])
                .SetDyeType(Attributes[GameAttribute.DyeType])
                .SetEffectLevel(0)
                .SetItemEffectType(-1)
                .Build();
            return visualItem;
        }

        public int GetPrice()
        {
            var price = ItemDefinition.Cost;
            //if (this.AffixList.Count == 0)
            //	price *= (1 + this.ItemDefinition.BuyCostMultiplier);
            foreach (var affix in AffixList) price += affix.Price;

            return price;
        }

        #region Is*

        public static bool IsHealthGlobe(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "HealthGlyph");
        }

        public static bool IsGold(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Gold");
        }

        public static bool IsBloodShard(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Collectible_DevilsHand");
        }

        public static bool IsPotion(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Potion");
        }

        public static bool IsMetaItem(ItemTypeTable itemType)
        {
            return itemType.Name.StartsWith("Generic");
        }

        public static bool IsRecipe(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "CraftingPlan");
        }

        public static bool IsTreasureBag(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "TreasureBag");
        }

        public static bool IsAccessory(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Jewelry");
        }

        public static bool IsJournalOrScroll(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Scroll") || ItemGroup.IsSubType(itemType, "Book");
        }

        public static bool IsDye(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Dye");
        }

        public static bool IsGem(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Gem");
        }

        public static bool IsWeapon(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Weapon");
        }

        public static bool IsArmor(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Armor");
        }

        public static bool IsChestArmor(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "ChestArmor");
        }

        public static bool IsOffhand(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Offhand");
        }

        public static bool IsShard(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Jewel");
        }

        public static bool IsBelt(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("Belt");
        }

        public static bool IsHelm(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "Helm");
        }

        public static bool IsAmulet(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("Amulet");
        }

        public static bool IsHandXbow(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("HandXbow");
        }

        public static bool IsShield(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("Shield");
        }

        public static bool IsRing(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("Ring");
        }

        public static bool IsQuiver(ItemTypeTable itemType)
        {
            return itemType.Name.Contains("Quiver");
        }

        public static bool IsBow(ItemTypeTable itemType)
        {
            return ItemGroup.IsSubType(itemType, "GenericBowWeapon");
        }

        public static bool Is2H(ItemTypeTable itemType)
        {
            return ItemGroup.Is2H(itemType);
        }

        #endregion

        public void SetInventoryLocation(int equipmentSlot, int column, int row)
        {
            EquipmentSlot = equipmentSlot;
            InventoryLocation.X = column;
            InventoryLocation.Y = row;
            if (Owner is Player)
            {
                var player = Owner as Player;
                if (!Reveal(player)) player.InGameClient.SendMessage(ACDInventoryPositionMessage(player));
            }
        }

        public void SetNewWorld(World world)
        {
            if (World == world)
                return;

            World = world;
        }

        public void Drop(Player owner, Vector3D position)
        {
            Owner = owner;
            EnterWorld(position);
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.Inventory.RefreshInventoryToClient();
            var playerAcc = player.InGameClient.BnetClient.Account.GameAccount;
            switch (SNO)
            {
                case ActorSno._tieredlootrunkey_0:
                    playerAcc.BigPortalKey++;
                    Destroy();
                    break;
                default:
                    player.Inventory.PickUp(this);
                    break;
            }

            var Moneys = D3.Items.CurrencySavedData.CreateBuilder();
            var GoldData = D3.Items.CurrencyData.CreateBuilder().SetId(0)
                .SetCount((long)player.Inventory.GetGoldAmount()).Build();
            var BloodShardData =
                D3.Items.CurrencyData.CreateBuilder().SetId(1).SetCount(playerAcc.BloodShards).Build();
            var PlatinumData =
                D3.Items.CurrencyData.CreateBuilder().SetId(2).SetCount(playerAcc.Platinum).Build();

            var Craft1Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(3).SetCount(playerAcc.CraftItem1)
                    .Build(); // Reusable Parts.
            var Craft2Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(4).SetCount(playerAcc.CraftItem2).Build(); // Arcanes Dust.
            var Craft3Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(5).SetCount(playerAcc.CraftItem3)
                    .Build(); // Veiled Crystal.
            var Craft4Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(playerAcc.CraftItem4)
                    .Build(); // Death's Breath.
            var Craft5Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(7).SetCount(playerAcc.CraftItem5)
                    .Build(); // Forgotten Soul.

            var Horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8)
                .SetCount(playerAcc.HoradricA1Res).Build(); // Khanduran Rune Bounty itens Act I.
            var Horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9)
                .SetCount(playerAcc.HoradricA2Res).Build(); // Caldeum Nightshade Bounty itens Act II.
            var Horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10)
                .SetCount(playerAcc.HoradricA3Res).Build(); // Arreat War Tapestry Bounty itens Act III.
            var Horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11)
                .SetCount(playerAcc.HoradricA4Res).Build(); // Copputed Angel Flesh Bounty itens Act IV.
            var Horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12)
                .SetCount(playerAcc.HoradricA5Res).Build(); // Westmarch Holy Water Bounty itens Act V.

            var Craft8Data = D3.Items.CurrencyData.CreateBuilder().SetId(13)
                .SetCount(playerAcc.HeartofFright).Build(); // Heart of Fright.
            var Craft9Data = D3.Items.CurrencyData.CreateBuilder().SetId(14)
                .SetCount(playerAcc.VialofPutridness).Build(); // Idol of Terror.
            var Craft10Data = D3.Items.CurrencyData.CreateBuilder().SetId(15)
                .SetCount(playerAcc.IdolofTerror).Build(); // Vail of Putridiness.
            var Craft11Data =
                D3.Items.CurrencyData.CreateBuilder().SetId(16).SetCount(playerAcc.LeorikKey).Build(); // Leorik Regret.

            var Craft7Data = D3.Items.CurrencyData.CreateBuilder().SetId(20)
                .SetCount(playerAcc.BigPortalKey).Build(); // KeyStone Greater Rift.

            D3.Items.CurrencyData[] consumables =
            {
                GoldData, BloodShardData, PlatinumData, Craft1Data, Craft2Data, Craft3Data, Craft4Data, Craft5Data,
                Craft7Data, Horadric1Data, Horadric2Data, Horadric3Data, Horadric4Data, Horadric5Data, Craft8Data,
                Craft9Data, Craft10Data, Craft11Data
            };

            foreach (var consumable in consumables) Moneys.AddCurrency(consumable);

            player.InGameClient.SendMessage(
                new MessageSystem.Message.Definitions.Base.GenericBlobMessage(Opcodes.CurrencyDataFull)
                    { Data = Moneys.Build().ToByteArray() });
        }

        public virtual void OnRequestUse(Player player, Item target, int actionId, WorldPlace worldPlace)
        {
            if (IsPotion(ItemType)) //if item is health potion
            {
                if (player.Attributes[GameAttribute.Hitpoints_Cur] == player.Attributes[GameAttribute.Hitpoints_Max])
                    return;

                player.World.PowerManager.RunPower(player, 30211);

                /* Potions are no longer consumable
                if (this.Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
                    player.Inventory.DestroyInventoryItem(this); // No more potions!
                else
                {
                    this.UpdateStackCount(--this.Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
                    this.Attributes.SendChangedMessage(player.InGameClient);
                }
                */

                return;
            }

            if (IsRecipe(ItemType)) //if item is crafting recipe
            {
                Logger.Trace("Learning recipe...");
                for (var i = 0; i < 10; i++)
                    if (ItemDefinition.RecipeToGrant[i] != -1)
                        player.LearnRecipe(player.CurrentArtisan, ItemDefinition.RecipeToGrant[i]);
                    else
                        break;

                for (var i = 0; i < 8; i++)
                    if (ItemDefinition.TransmogsToGrant[i] != -1)
                        player.UnlockTransmog(ItemDefinition.TransmogsToGrant[i]);
                    else
                        break;

                if (GBHandle.GBID == 1549850924) //Arma Haereticorum additional transmog
                {
                    player.UnlockTransmog(974107120);
                    return;
                }

                if (Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
                {
                    player.Inventory.DestroyInventoryItem(this); // No more recipes!
                }
                else
                {
                    UpdateStackCount(--Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
                    Attributes.SendChangedMessage(player.InGameClient);
                }

                return;
            }

            if (IsTreasureBag(ItemType))
            {
                Logger.Warn("Treasure Bag system v0.2");
                var items = new string[1];
                var @base = 0;
                switch (player.Toon.Class)
                {
                    case LoginServer.Toons.ToonClass.Crusader:
                        @base = 2;
                        break;
                    case LoginServer.Toons.ToonClass.DemonHunter:
                        @base = 4;
                        break;
                    case LoginServer.Toons.ToonClass.Monk:
                        @base = 6;
                        break;
                    case LoginServer.Toons.ToonClass.Necromancer:
                        @base = 8;
                        break;
                    case LoginServer.Toons.ToonClass.WitchDoctor:
                        @base = 10;
                        break;
                    case LoginServer.Toons.ToonClass.Wizard:
                        @base = 12;
                        break;
                } //0 - Barbarian, 2 - Crusader, 4 - Hunter, 6 - Monk, 8 - Necromancer, 10 - Sorcerer, 12 - Wizard

                var it = "";

                #region Balance calculation

                var moneys = D3.Items.CurrencySavedData.CreateBuilder();
                var playerAcc = player.InGameClient.BnetClient.Account.GameAccount;

                var goldData = D3.Items.CurrencyData.CreateBuilder().SetId(0)
                    .SetCount((long)player.Inventory.GetGoldAmount()).Build();
                var bloodShardData = D3.Items.CurrencyData.CreateBuilder().SetId(1)
                    .SetCount(playerAcc.BloodShards).Build();
                var platinumData =
                    D3.Items.CurrencyData.CreateBuilder().SetId(2).SetCount(playerAcc.Platinum).Build();

                var craft1Data = D3.Items.CurrencyData.CreateBuilder().SetId(3)
                    .SetCount(playerAcc.CraftItem1).Build(); // Reusable Parts.
                var craft2Data = D3.Items.CurrencyData.CreateBuilder().SetId(4)
                    .SetCount(playerAcc.CraftItem2).Build(); // Arcanes Dust.
                var craft3Data = D3.Items.CurrencyData.CreateBuilder().SetId(5)
                    .SetCount(playerAcc.CraftItem3).Build(); // Veiled Crystal.
                var craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6)
                    .SetCount(playerAcc.CraftItem4).Build(); // Death's Breath.
                var craft5Data = D3.Items.CurrencyData.CreateBuilder().SetId(7)
                    .SetCount(playerAcc.CraftItem5).Build(); // Forgotten Soul.

                var horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8)
                    .SetCount(playerAcc.HoradricA1Res).Build(); // Khanduran Rune Bounty itens Act I.
                var horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9)
                    .SetCount(playerAcc.HoradricA2Res).Build(); // Caldeum Nightshade Bounty itens Act II.
                var horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10)
                    .SetCount(playerAcc.HoradricA3Res).Build(); // Arreat War Tapestry Bounty itens Act III.
                var horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11)
                    .SetCount(playerAcc.HoradricA4Res).Build(); // Copputed Angel Flesh Bounty itens Act IV.
                var horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12)
                    .SetCount(playerAcc.HoradricA5Res).Build(); // Westmarch Holy Water Bounty itens Act V.

                var craft8Data = D3.Items.CurrencyData.CreateBuilder().SetId(13)
                    .SetCount(playerAcc.HeartofFright).Build(); // Heart of Fright.
                var craft9Data = D3.Items.CurrencyData.CreateBuilder().SetId(14)
                    .SetCount(playerAcc.VialofPutridness).Build(); // Idol of Terror.
                var craft10Data = D3.Items.CurrencyData.CreateBuilder().SetId(15)
                    .SetCount(playerAcc.IdolofTerror).Build(); // Vail of Putridiness.
                var craft11Data = D3.Items.CurrencyData.CreateBuilder().SetId(16)
                    .SetCount(playerAcc.LeorikKey).Build(); // Leorik Regret.

                var craft7Data = D3.Items.CurrencyData.CreateBuilder().SetId(20)
                    .SetCount(playerAcc.BigPortalKey).Build(); // KeyStone Greater Rift.

                D3.Items.CurrencyData[] consumables =
                {
                    goldData, bloodShardData, platinumData, craft1Data, craft2Data, craft3Data, craft4Data, craft5Data,
                    craft7Data, horadric1Data, horadric2Data, horadric3Data, horadric4Data, horadric5Data, craft8Data,
                    craft9Data, craft10Data, craft11Data
                };

                foreach (var consumable in consumables) moneys.AddCurrency(consumable);

                #endregion

                switch (GBHandle.GBID)
                {
                    #region The Gift of Horadric

                    case -1249067449:
                        items = new string[]
                        {
                            "Unique_Helm_Set_15_x1", "Unique_Gloves_Set_15_x1",
                            "Unique_Helm_Set_12_x1", "Unique_Gloves_Set_12_x1",
                            "Unique_Helm_Set_14_x1", "Unique_Gloves_Set_14_x1",
                            "Unique_Helm_Set_11_x1", "Unique_Gloves_Set_11_x1",
                            "P6_Necro_Set_3_Helm", "P6_Necro_Set_3_Gloves",
                            "Unique_Helm_Set_09_x1", "Unique_Gloves_Set_09_x1",
                            "Unique_Helm_Set_06_x1", "Unique_Gloves_Set_06_x1"
                        };
                        switch (player.Toon.Class)
                        {
                            case LoginServer.Toons.ToonClass.Crusader:
                                @base = 2;
                                break;
                            case LoginServer.Toons.ToonClass.DemonHunter:
                                @base = 4;
                                break;
                            case LoginServer.Toons.ToonClass.Monk:
                                @base = 6;
                                break;
                            case LoginServer.Toons.ToonClass.Necromancer:
                                @base = 8;
                                break;
                            case LoginServer.Toons.ToonClass.WitchDoctor:
                                @base = 10;
                                break;
                            case LoginServer.Toons.ToonClass.Wizard:
                                @base = 12;
                                break;
                        }

                        it = items[RandomHelper.Next(@base, @base + 1)];
                        player.Inventory.PickUp(ItemGenerator.Cook(player, it));
                        break;
                    case -1249067448:
                        items = new string[]
                        {
                            "Unique_Shoulder_Set_15_x1", "Unique_Boots_Set_15_x1",
                            "Unique_Shoulder_Set_12_x1", "Unique_Boots_Set_12_x1",
                            "Unique_Shoulder_Set_14_x1", "Unique_Boots_Set_14_x1",
                            "Unique_Shoulder_Set_11_x1", "Unique_Boots_Set_11_x1",
                            "P6_Necro_Set_3_Shoulders", "P6_Necro_Set_3_Boots",
                            "Unique_Shoulder_Set_09_x1", "Unique_Boots_Set_09_x1",
                            "Unique_Shoulder_Set_06_x1", "Unique_Boots_Set_06_x1"
                        };
                        it = items[RandomHelper.Next(@base, @base + 1)];
                        player.Inventory.PickUp(ItemGenerator.Cook(player, it));
                        break;
                    case -1249067447:
                        items = new string[]
                        {
                            "Unique_Chest_Set_15_x1", "Unique_Pants_Set_15_x1",
                            "Unique_Chest_Set_12_x1", "Unique_Pants_Set_12_x1",
                            "Unique_Chest_Set_14_x1", "Unique_Pants_Set_14_x1",
                            "Unique_Chest_Set_11_x1", "Unique_Pants_Set_11_x1",
                            "P6_Necro_Set_3_Chest", "P6_Necro_Set_3_Pants",
                            "Unique_Chest_Set_09_x1", "Unique_Pants_Set_09_x1",
                            "Unique_Chest_Set_06_x1", "Unique_Pants_Set_06_x1"
                        };
                        it = items[RandomHelper.Next(@base, @base + 1)];
                        player.Inventory.PickUp(ItemGenerator.Cook(player, it));
                        break;

                    #endregion

                    #region The Treasure of the Khoradrim

                    case -1575654862: // The Treasure 1 Акта
                        playerAcc.HoradricA1Res += RandomHelper.Next(1, 5);
                        playerAcc.CraftItem4 += RandomHelper.Next(2, 4);
                        horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8).SetCount(playerAcc.HoradricA1Res)
                            .Build();
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
                        player.World.SpawnGold(player, player, 5000);
                        player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
                        break;
                    case -1575654861: // The Treasure 2 Акта
                        playerAcc.HoradricA2Res += RandomHelper.Next(1, 5);
                        playerAcc.CraftItem4 += RandomHelper.Next(2, 4);
                        horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9).SetCount(playerAcc.HoradricA2Res)
                            .Build();
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
                        player.World.SpawnGold(player, player, 5000);
                        player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
                        break;
                    case -1575654860: // The Treasure 3 Акта
                        playerAcc.HoradricA3Res += RandomHelper.Next(1, 5);
                        playerAcc.CraftItem4 += RandomHelper.Next(2, 4);
                        horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10)
                            .SetCount(playerAcc.HoradricA3Res).Build();
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
                        player.World.SpawnGold(player, player, 5000);
                        player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
                        break;
                    case -1575654859: // The Treasure 4 Акта
                        playerAcc.HoradricA4Res += RandomHelper.Next(1, 5);
                        playerAcc.CraftItem4 += RandomHelper.Next(2, 4);
                        horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11)
                            .SetCount(playerAcc.HoradricA4Res).Build();
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
                        player.World.SpawnGold(player, player, 5000);
                        player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
                        break;
                    case -1575654858: // The Treasure 5 Акта
                        playerAcc.HoradricA5Res += RandomHelper.Next(1, 5);
                        playerAcc.CraftItem4 += RandomHelper.Next(2, 4);
                        horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12)
                            .SetCount(playerAcc.HoradricA5Res).Build();
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
                        player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
                        player.World.SpawnGold(player, player, 5000);
                        player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
                        break;

                    #endregion

                    default:
                        Logger.Warn("This treasure bag - not implemented");
                        break;
                }

                craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(playerAcc.CraftItem4).Build();

                D3.Items.CurrencyData[] horadricBoxes = { horadric1Data, horadric2Data, horadric3Data, horadric4Data, horadric5Data };
                foreach (var horadricBoxe in horadricBoxes) moneys.AddCurrency(horadricBoxe);

                player.InGameClient.SendMessage(
                    new MessageSystem.Message.Definitions.Base.GenericBlobMessage(Opcodes.CurrencyDataFull)
                        { Data = moneys.Build().ToByteArray() });

                player.Inventory.DestroyInventoryItem(this);
                return;
            }

            if (GBHandle.GBID == 237118774) //AngelWings_Blue
            {
                SwitchWingsBuff(player, 208706);
                return;
            }

            if (GBHandle.GBID == -1424453175) //AngelWings_Red
            {
                SwitchWingsBuff(player, 317139);
                return;
            }

            if (GBHandle.GBID == -1736870778) //BugWings
            {
                SwitchWingsBuff(player, 255336);
                return;
            }

            if (GBHandle.GBID == -1364948604) //x1_AngelWings_Imperius
            {
                SwitchWingsBuff(player, 378292);
                return;
            }

            if (GBHandle.GBID == -762694428) //WoDFlag
            {
                SwitchWingsBuff(player, 375412);
                return;
            }

            if (IsDye(ItemType)) //if item is dye
            {
                if (target == null) return;
                target.Attributes[GameAttribute.DyeType] = Attributes[GameAttribute.DyeType];
                target.Attributes.BroadcastChangedIfRevealed();
                target.DBInventory.DyeType = Attributes[GameAttribute.DyeType];

                player.World.Game.GameDBSession.SessionUpdate(target.DBInventory);

                player.Inventory.SendVisualInventory(player);

                player.GrantAchievement(74987243307154);

                var colors = new List<int>(player.Inventory.GetEquippedItems()
                    .Where(i => i.Attributes[GameAttribute.DyeType] > 0)
                    .Select(i => i.Attributes[GameAttribute.DyeType]));
                if (colors.Count >= 6)
                {
                    if (new HashSet<int>(colors).Count == 1)
                        player.GrantAchievement(74987243307156);
                    if (new HashSet<int>(colors).Count >= 6)
                        player.GrantAchievement(74987243307157);
                }

                switch ((uint)GBHandle.GBID)
                {
                    case 4060770506:
                        player.GrantCriteria(74987243311599);
                        break;
                    case 4060770531:
                        player.GrantCriteria(74987243312037);
                        break;
                    case 4060770504:
                        player.GrantCriteria(74987243312038);
                        break;
                    case 4060770500:
                        player.GrantCriteria(74987243312039);
                        break;
                    case 4060770499:
                        player.GrantCriteria(74987243312040);
                        break;
                    case 4060770474:
                        player.GrantCriteria(74987243312041);
                        break;
                    case 4060770505:
                        player.GrantCriteria(74987243312042);
                        break;
                    case 4060770503:
                        player.GrantCriteria(74987243312044);
                        break;
                    case 4060770507:
                        player.GrantCriteria(74987243312045);
                        break;
                    case 4060770469:
                        player.GrantCriteria(74987243312046);
                        break;
                    case 4060770498:
                        player.GrantCriteria(74987243312047);
                        break;
                    case 4060770501:
                        player.GrantCriteria(74987243312048);
                        break;
                    case 4060770471:
                        player.GrantCriteria(74987243312050);
                        break;
                    case 4060770468:
                        player.GrantCriteria(74987243312052);
                        break;
                    case 4060770466:
                        player.GrantCriteria(74987243312053);
                        break;
                    case 4060770502:
                        player.GrantCriteria(74987243312054);
                        break;
                    case 4060770473:
                        player.GrantCriteria(74987243312055);
                        break;
                    default:
                        break;
                }

                if (GBHandle.GBID == 1866876233 || GBHandle.GBID == 1866876234) return; //CE dyes

                if (Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
                {
                    player.Inventory.DestroyInventoryItem(this); // No more dyes!
                }
                else
                {
                    UpdateStackCount(--Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
                    Attributes.SendChangedMessage(player.InGameClient);
                }

                return;
            }

            Logger.Warn("OnRequestUse(): gbid {0} not implemented", GBHandle.GBID);
        }

        private void SwitchWingsBuff(Player player, int powerId)
        {
            if (player.CurrentWingsPowerId != -1 && player.CurrentWingsPowerId != powerId) //turning off another wings
            {
                player.Attributes[GameAttribute.Buff_Exclusive_Type_Active, player.CurrentWingsPowerId] = false;
                player.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, player.CurrentWingsPowerId] = false;
                player.Attributes[GameAttribute.Buff_Icon_Start_Tick0, player.CurrentWingsPowerId] = 0;
                player.Attributes[GameAttribute.Buff_Icon_End_Tick0, player.CurrentWingsPowerId] = 0;
                player.Attributes[GameAttribute.Buff_Icon_Count0, player.CurrentWingsPowerId] = 0;
                player.CurrentWingsPowerId = -1;
            }

            var activated = player.Attributes[GameAttribute.Buff_Exclusive_Type_Active, powerId] == true;

            player.CurrentWingsPowerId = activated ? -1 : powerId;

            player.Attributes[GameAttribute.Buff_Exclusive_Type_Active, powerId] = !activated;
            player.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, powerId] = !activated;
            player.Attributes[GameAttribute.Buff_Icon_Start_Tick0, powerId] = 0;
            player.Attributes[GameAttribute.Buff_Icon_End_Tick0, powerId] = activated ? 0 : 100;
            player.Attributes[GameAttribute.Buff_Icon_Count0, powerId] = activated ? 0 : 1;
            player.Attributes.BroadcastChangedIfRevealed();
            player.Inventory.SendVisualInventory(player);
            var dbToon = player.Toon.DBToon;
            dbToon.WingsActive = player.CurrentWingsPowerId;
            player.World.Game.GameDBSession.SessionUpdate(dbToon);
            return;
        }

        public override bool Reveal(Player player)
        {
            if (CurrentState == ItemState.PickingUp && HasWorldLocation)
                return false;

            foreach (var gplayer in player.World.Game.Players.Values)
                if (gplayer.GroundItems.ContainsKey(GlobalID) && gplayer != player)
                    return false;

            if (!base.Reveal(player))
                return false;

            if (AffixList.Count > 0)
            {
                var affixGbis = new int[AffixList.Count];
                for (var i = 0; i < AffixList.Count; i++) affixGbis[i] = AffixList[i].AffixGbid;

                player.InGameClient.SendMessage(new AffixMessage()
                {
                    ActorID = DynamicID(player),
                    Field1 = Unidentified ? 0x00000002 : 0x00000001,
                    aAffixGBIDs = affixGbis
                });
            }

            foreach (var gem in Gems)
                gem.Reveal(player);

            if (RareItemName != null)
                player.InGameClient.SendMessage(new RareItemNameMessage()
                {
                    ann = DynamicID(player),
                    RareItemName = new RareItemName()
                    {
                        Field0 = RareItemName.ItemNameIsPrefix,
                        snoAffixStringList = RareItemName.SnoAffixStringList,
                        AffixStringListIndex = RareItemName.AffixStringListIndex,
                        ItemStringListIndex = RareItemName.ItemStringListIndex
                    }
                });
            return true;
        }

        public override bool Unreveal(Player player)
        {
            if (CurrentState == ItemState.PickingUp) // && player == Owner)
                return false;

            foreach (var gem in Gems)
                gem.Unreveal(player);

            return base.Unreveal(player);
        }

        private bool ZPositionCorrected = false;

        public override void OnPlayerApproaching(Player player)
        {
            if (PowerMath.Distance2D(player.Position, Position) < 3f && !ZPositionCorrected)
            {
                foreach (var gplayer in player.World.Game.Players.Values)
                    if (gplayer.GroundItems.ContainsKey(GlobalID) && gplayer != player)
                        return;

                ZPositionCorrected = true;
                Teleport(new Vector3D(Position.X, Position.Y, player.Position.Z));
            }
        }
    }

    public enum ItemState
    {
        Normal,
        PickingUp,
        Dropping
    }
}