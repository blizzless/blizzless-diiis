//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using Actor = DiIiS_NA.GameServer.GSSystem.ActorSystem.Actor;
//Blizzless Project 2022 
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
//Blizzless Project 2022 
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public class Item : Actor
	{
		public DBInventory DBInventory = null;

		private static readonly Logger Logger = LogManager.CreateLogger();
		public bool ItemHasChanges { get; private set; }//needed in Future, set this to true if Item affixes or item attributes have changed.


		public override ActorType ActorType { get { return ActorType.Item; } }

		public Actor Owner { get; set; } // Only set when the _actor_ has the item in its inventory. /fasbat

		public ItemTable ItemDefinition
		{
			get
			{
				return ItemGenerator.GetItemDefinition(this.GBHandle.GBID);
			}
		}

		public ItemTypeTable ItemType
		{
			get
			{
				return ItemGroup.FromHash(this.ItemDefinition.ItemTypesGBID);
			}
		}

		public bool Unidentified = false;

		public int EquipGemType
		{
			get
			{
				if (IsWeapon(this.ItemType)) return 485534122;
				if (IsHelm(this.ItemType)) return 3851110;
				return 109305;
			}
		}

		public ItemRandomHelper RandomGenerator { get; set; }
		public int ItemLevel { get; private set; }

		public List<Item> Gems = new List<Item>();

		public int Rating
		{
			get
			{
				return this.AffixList.Select(a => a.Rating).Sum() + (int)this.Gems.Select(g => g.ItemDefinition.Cost * 6f).Sum();
			}
			set { }
		}

		public D3.Items.RareItemName RareItemName = null;

		public ItemState CurrentState { get; set; }

		public int EquipmentSlot { get; private set; }
		public Vector2D InventoryLocation { get; private set; } // Column, row; NOTE: Call SetInventoryLocation() instead of setting fields on this

		public override int Quality
		{
			get
			{
				return Attributes[GameAttribute.Item_Quality_Level];
			}
			set
			{
				Attributes[GameAttribute.Item_Quality_Level] = value;
			}
		}

		public SNOHandle SnoFlippyActory
		{
			get
			{
				return ActorData.TagMap.ContainsKey(ActorKeys.Flippy) ? ActorData.TagMap[ActorKeys.Flippy] : null;
			}
		}

		public SNOHandle SnoFlippyParticle
		{
			get
			{
				return ActorData.TagMap.ContainsKey(ActorKeys.FlippyParticle) ? ActorData.TagMap[ActorKeys.FlippyParticle] : null;
			}
		}

		public override bool HasWorldLocation
		{
			get { return this.Owner == null; }
		}

		public override InventoryLocationMessageData InventoryLocationMessage(Player plr)
		{
			return new InventoryLocationMessageData
			{
				OwnerID = (this.Owner != null) ? this.Owner.DynamicID(plr) : 0,
				EquipmentSlot = this.EquipmentSlot,
				InventoryLocation = this.InventoryLocation
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
				OwnerID = (this.Owner != null) ? this.Owner.DynamicID(plr) : 0,
				EquipmentSlot = this.EquipmentSlot,
				Row = this.InventoryLocation.Y,
				Column = this.InventoryLocation.X
			};
		}

		public List<int> AffixFamilies = new List<int>();

		public Item(MapSystem.World world, ItemTable definition, IEnumerable<Affix> affixList, string serializedGameAttributeMap, int count = 1)
			: base(world, definition.SNOActor)
		{
			this.GBHandle.GBID = definition.Hash; 
			SetInitialValues(definition);
			this.Attributes.FillBySerialized(serializedGameAttributeMap);
			if (this.Attributes[GameAttribute.Seed] == 0)
			{
				this.Attributes[GameAttribute.Seed] = DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next();
				//this.Attributes[GameAttribute.Seed] = 0;
				this.Attributes[GameAttribute.Item_Quality_Level] = 1;
				this.Attributes[GameAttribute.TeamID] = 0;
			}
			//for (int i = 50; i < 60; i++)
			//Attributes[GameAttribute.Requirement, 57] = 10;

			

			this.Attributes[GameAttribute.ItemStackQuantityLo] = count;
			Attributes[GameAttribute.Loot_2_0_Drop] = true;
			this.AffixList.Clear();
			this.AffixList.AddRange(affixList);
			//this.Attributes[GameAttribute.EnchantAffix] = -758203990;
			//this.Attributes[GameAttribute.EnchantAffix, 0] = -758203990;
			//this.Attributes[GameAttribute.EnchantAffix, 1] = -758203990;
			//this.Attributes[GameAttribute.EnchantRangeVal] = 1;
			//*
			if (Item.IsArmor(this.ItemType) || Item.IsWeapon(this.ItemType) || Item.IsOffhand(this.ItemType) || (Item.IsPotion(this.ItemType) && this.ItemDefinition.Name.Contains("Legendary")) || Item.IsAccessory(this.ItemType))
			{
				//Attributes[GameAttribute.Requirement, 64] = 0;
				var reqLevel = (definition.RequiredLevel % 10 != 0) ? definition.RequiredLevel - 1 : definition.RequiredLevel;
				var level = Math.Max(this.AffixList.Any() ? this.AffixList.Select(a => a.ItemLevel).Max() : 0, reqLevel);
				Attributes[GameAttribute.Requirement, 57] = Math.Max(level - Attributes[GameAttribute.Item_Level_Requirement_Reduction], 0);
			}

			if (AffixList.Count > 0)
			{
				if (Attributes[GameAttribute.Requirement, 57] != AffixList[0].Definition.OverrideLevelReq && AffixList[0].Definition.OverrideLevelReq != 0)
					Attributes[GameAttribute.Requirement, 57] = AffixList[0].Definition.OverrideLevelReq;
				foreach (var affix in AffixList)
				{
					if (affix.Definition.OverrideLevelReq > Attributes[GameAttribute.Requirement, 57])
						Attributes[GameAttribute.Requirement, 57] = affix.Definition.OverrideLevelReq;
				}
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
			this.ItemLevel = definition.ItemLevel;
			this.GBHandle.Type = (int)ActorType.Gizmo;
			this.EquipmentSlot = 0;
			this.InventoryLocation = new Vector2D { X = 0, Y = 0 };
			this.Scale = 1f;
			this.RotationW = 0.0f;
			this.RotationAxis.Set(0.0f, 0.0f, 1.0f);
			this.CurrentState = ItemState.Normal;
			//flags: 0x01 - socketable, 0x20 - blinking
			this.Field2 = 0x1f;
			this.Field7 = -1;
			this.CollFlags = 0;
			this.NameSNOId = -1;
			this.Field10 = 0x00;

			this.Attributes[GameAttribute.TeamID] = 0;
		}

		public Item(MapSystem.World world, ItemTable definition, int ForceQualityLevel = -1, bool crafted = false, int seed = -1)
			: base(world, definition.SNOActor)
		{
			this.GBHandle.GBID = definition.Hash;
			SetInitialValues(definition);
			this.ItemHasChanges = true;
			Attributes[GameAttribute.IsCrafted] = crafted;
			Attributes[GameAttribute.Item_Quality_Level] = 1;

			Attributes[GameAttribute.Loot_2_0_Drop] = true;

			if (Item.IsArmor(this.ItemType) || Item.IsWeapon(this.ItemType) || Item.IsOffhand(this.ItemType) || Item.IsAccessory(this.ItemType) || Item.IsShard(this.ItemType))
				Attributes[GameAttribute.Item_Quality_Level] = RandomHelper.Next(8);
			if (this.ItemType.Usable.HasFlag(ItemFlags.AtLeastMagical) && Attributes[GameAttribute.Item_Quality_Level] < 3)
				Attributes[GameAttribute.Item_Quality_Level] = 3;
			if (definition.Name.ToLower().Contains("unique") || definition.Quality == ItemTable.ItemQuality.Legendary)
				Attributes[GameAttribute.Item_Quality_Level] = 9;
			if (ForceQualityLevel > -1)
				Attributes[GameAttribute.Item_Quality_Level] = ForceQualityLevel;
			if (definition.SNOSet != -1)
			{
				Attributes[GameAttribute.Item_Quality_Level] = 9;
			}
			if (this.ItemDefinition.Name.ToLower().Contains("unique_gem"))
			{
				Attributes[GameAttribute.Item_Quality_Level] = 9;
				if (!this.Attributes.Contains(GameAttribute.Jewel_Rank))
					Attributes[GameAttribute.Jewel_Rank] = 1;
				//Attributes[GameAttribute.Jewel_Rank] = 1;
			}
			if (this.ItemDefinition.Name.ToLower().Contains("norm_season"))
			{
				Attributes[GameAttribute.Item_Quality_Level] = 9;
			}
			if (this.ItemDefinition.Name.ToLower().StartsWith("p71_ethereal"))
			{

				Attributes[GameAttribute.Item_Quality_Level] = 9;
				Attributes[GameAttribute.Attacks_Per_Second_Item] += 1.1f;

				Attributes[GameAttribute.Damage_Weapon_Min, 0] = 15 + (this.World.Game.InitialMonsterLevel * 1.7f);
				if (this.World.Game.InitialMonsterLevel > 70)
					Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 20f;
				else if (this.World.Game.InitialMonsterLevel > 60)
					Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 14f;
				Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 15;



			}


			Attributes[GameAttribute.ItemStackQuantityLo] = 1;
			if (seed == -1)
				Attributes[GameAttribute.Seed] = DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(); //unchecked((int)2286800181);
			else
				Attributes[GameAttribute.Seed] = seed;

			//Attributes[GameAttribute.Seed] = 0;
			//Attributes[GameAttribute.Item_Quality_Level] = 1;

			this.RandomGenerator = new ItemRandomHelper(Attributes[GameAttribute.Seed]);
			this.RandomGenerator.Next();
			if (Item.IsArmor(this.ItemType))
			{
				if (!crafted)
					this.RandomGenerator.Next();
				if (Attributes[GameAttribute.Item_Quality_Level] >= 5 && Attributes[GameAttribute.Item_Quality_Level] <= 7)
					this.RandomGenerator.Next();
			}
			this.RandomGenerator.ReinitSeed();

			if (Item.IsWeapon(this.ItemType) && !crafted)
			{
				this.RandomGenerator.Next();
				this.RandomGenerator.Next();
			}

			ApplyWeaponSpecificOptions(definition);
			ApplyArmorSpecificOptions(definition);
			ApplyDurability(definition);
			ApplySkills(definition);
			ApplyAttributeSpecifier(definition);

			int affixNumber = 1;
			if (Attributes[GameAttribute.Item_Quality_Level] >= 3)
				affixNumber = Attributes[GameAttribute.Item_Quality_Level] - 2;

			if (ItemDefinition.Name.Contains("Shard"))
				affixNumber = 1;
			if (ItemDefinition.Name.Contains("GreaterShard"))
				affixNumber = 3;

			if (!crafted)
				if (Attributes[GameAttribute.Item_Quality_Level] > 1)
					AffixGenerator.Generate(this, affixNumber);

			if (Item.IsShard(this.ItemType))
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
			if (Attributes[GameAttribute.Item_Quality_Level] == 9)
			{
				this.Attributes[GameAttribute.MinimapActive] = true;
			}

			if (Item.IsArmor(this.ItemType) || Item.IsWeapon(this.ItemType) || Item.IsOffhand(this.ItemType) || (Item.IsPotion(this.ItemType) && this.ItemDefinition.Name.Contains("Legendary")) || Item.IsAccessory(this.ItemType))
			{

				var a = Attributes[GameAttribute.Requirement, 57];
				var reqLevel = (definition.RequiredLevel % 10 != 0) ? definition.RequiredLevel - 1 : definition.RequiredLevel;
				var level = Math.Max(this.AffixList.Any() ? this.AffixList.Select(a => a.ItemLevel).Max() : 0, reqLevel);
				Attributes[GameAttribute.Requirement, 57] = Math.Max(level - Attributes[GameAttribute.Item_Level_Requirement_Reduction], 0);
				a = Attributes[GameAttribute.Requirement, 57];
			}

			//Жесткая перепись требуемого уровня для легендарного оружия, в случае его бага на 70 лвл.
			if (Attributes[GameAttribute.Item_Quality_Level] > 8)
				if (Attributes[GameAttribute.Requirement, 57] == 0)
					Attributes[GameAttribute.Item_Level_Requirement_Override] = 1;
				else
					Attributes[GameAttribute.Item_Level_Requirement_Override] = (int)Attributes[GameAttribute.Requirement, 57];

			if (this.ItemDefinition.Name.ToLower().StartsWith("p71_ethereal"))
			{
				AffixGenerator.AddAffix(this, 1661455571, true); //1661455571
			}

		}


		public void Identify()
		{
			this.Unidentified = false;
			this.DBInventory.Unidentified = false;
			this.Attributes[GameAttribute.Unidentified] = false;

			this.Owner.World.Game.GameDBSession.SessionUpdate(this.DBInventory);
			if (this.Owner is Player)
			{
				this.Unreveal(this.Owner as Player);
				this.Reveal(this.Owner as Player);
				if (this.ItemDefinition.Name.Contains("Unique"))
				{
					(this.Owner as Player).UniqueItemIdentified(this.DBInventory.Id);
					//if (Program.MaxLevel == 70)
					(this.Owner as Player).UnlockTransmog(this.ItemDefinition.Hash);
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

			int hash = definition.Hash;
			if (definition.Name.Contains("_104"))
				hash = ItemGenerator.GetItemHash(definition.Name.Substring(0, definition.Name.Length - 4));

			if (UniqueItems.UniqueItemStats.ContainsKey(hash))
			{
				Attributes[GameAttribute.Attacks_Per_Second_Item] += UniqueItems.GetDPS(hash);
				Attributes[GameAttribute.Damage_Weapon_Min, 0] += UniqueItems.GetWeaponDamageMin(hash);
				Attributes[GameAttribute.Damage_Weapon_Delta, 0] += UniqueItems.GetWeaponDamageDelta(hash);

				if (IsWeapon(this.ItemType))
				{
					if (Attributes[GameAttribute.Damage_Weapon_Min, 0] == 0)
						Attributes[GameAttribute.Damage_Weapon_Min, 0] = 34;
					if (Attributes[GameAttribute.Damage_Weapon_Delta, 0] == 0)
						Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 34;
				}
				float scaleCapMin = 0f;
				float scaleCapDelta = 0f;
				switch (definition.ItemTypesGBID)
				{
					case 109694:            //Axe
						scaleCapMin = 249f;
						scaleCapDelta = 461f - scaleCapMin;
						break;
					case -262576534:        //Dagger
						scaleCapMin = 107f;
						scaleCapDelta = 321f - scaleCapMin;
						break;
					case 4026134:           //Mace
						scaleCapMin = 316f;
						scaleCapDelta = 585f - scaleCapMin;
						break;
					case 140519163:         //Spear
						scaleCapMin = 357f;
						scaleCapDelta = 526f - scaleCapMin;
						break;
					case 140782159:         //Sword
						scaleCapMin = 168f;
						scaleCapDelta = 392f - scaleCapMin;
						break;
					case -199811863:        //Ceremonial Knife
						scaleCapMin = 117f;
						scaleCapDelta = 469f - scaleCapMin;
						break;
					case -2094596416:       //Fist Weapon
						scaleCapMin = 168f;
						scaleCapDelta = 392f - scaleCapMin;
						break;
					case -1363671135:       //Flail
						scaleCapMin = 192f;
						scaleCapDelta = 355f - scaleCapMin;
						break;
					case -1488678091:       //Mighty Weapon
						scaleCapMin = 249f;
						scaleCapDelta = 461f - scaleCapMin;
						break;
					case 763102523:         //Hand Crossbow
						scaleCapMin = 126f;
						scaleCapDelta = 714f - scaleCapMin;
						break;
					case 4385866:           //Wand
						scaleCapMin = 197f;
						scaleCapDelta = 357f - scaleCapMin;
						break;
					case 110504:            //Bow
						scaleCapMin = 143f;
						scaleCapDelta = 815f - scaleCapMin;
						break;
					case -1338851342:       //Crossbow
						scaleCapMin = 779f;
						scaleCapDelta = 945f - scaleCapMin;
						break;
					case 119458520:         //2H Axe
						scaleCapMin = 1384f;
						scaleCapDelta = 1685f - scaleCapMin;
						break;
					case 89494384:          //2H Mace
						scaleCapMin = 1737f;
						scaleCapDelta = 1912f - scaleCapMin;
						break;
					case -1203595600:       //2H Polearm
						scaleCapMin = 1497f;
						scaleCapDelta = 1823f - scaleCapMin;
						break;
					case 140658708:         //2H Staff
						scaleCapMin = 1229f;
						scaleCapDelta = 1839f - scaleCapMin;
						break;
					case -1307049751:       //2H Sword
						scaleCapMin = 1137f;
						scaleCapDelta = 1702f - scaleCapMin;
						break;
					case -1620551894:       //2H Daibo
						scaleCapMin = 994f;
						scaleCapDelta = 1845f - scaleCapMin;
						break;
					case -1363671102:       //2H Flail
						scaleCapMin = 1351f;
						scaleCapDelta = 1486f - scaleCapMin;
						break;
					case -1488678058:       //2H Mighty Weapon
						scaleCapMin = 1462f;
						scaleCapDelta = 1609f - scaleCapMin;
						break;
				}

				if (scaleCapMin > 5 && scaleCapDelta > 5)
				{
					float ratio = (float)Math.Pow(definition.ItemLevel, 2f) / 4900f;
					if (ratio < 0.01f) ratio = 0.01f;
					if (ratio > 1f) ratio = 1f;
					Attributes[GameAttribute.Damage_Weapon_Min, 0] += Math.Abs(scaleCapMin * ratio - Attributes[GameAttribute.Damage_Weapon_Min, 0]);
					Attributes[GameAttribute.Damage_Weapon_Delta, 0] += Math.Abs(scaleCapDelta * ratio - Attributes[GameAttribute.Damage_Weapon_Delta, 0]);
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
				var Armor_Item_Total = Attributes[GameAttribute.Armor_Item_Total];
			}

			int hash = definition.Hash;
			if (definition.Name.Contains("_104"))
				hash = ItemGenerator.GetItemHash(definition.Name.Substring(0, definition.Name.Length - 4));

			if (UniqueItems.UniqueItemStats.ContainsKey(hash))
			{
				
				Attributes[GameAttribute.Armor_Item] += UniqueItems.GetArmor(hash);
				//Unique items level scaling
				if (IsArmor(this.ItemType))
					if (Attributes[GameAttribute.Armor_Item] == 0)
						Attributes[GameAttribute.Armor_Item] = 30;

				if (Attributes[GameAttribute.Armor_Item] < 100) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 6;
				else if (Attributes[GameAttribute.Armor_Item] < 200) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 5;
				else if (Attributes[GameAttribute.Armor_Item] < 300) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 4;
				else if (Attributes[GameAttribute.Armor_Item] < 400) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 3;
				else if (Attributes[GameAttribute.Armor_Item] < 500) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel * 2;
				else if (Attributes[GameAttribute.Armor_Item] < 600) Attributes[GameAttribute.Armor_Item] += definition.ItemLevel;

				if (definition.ItemTypesGBID == 332825721 || definition.ItemTypesGBID == 602099538)     //Shield and CruShield
				{
					float scaleCapMin = 14000f;
					float scaleCapDelta = 21000f - scaleCapMin;
					float ratio = (float)Math.Pow(definition.ItemLevel, 2f) / 4900f;
					if (ratio < 0.01f) ratio = 0.01f;
					if (ratio > 1f) ratio = 1f;
					Attributes[GameAttribute.Block_Amount_Item_Min] += Math.Abs(scaleCapMin * ratio - Attributes[GameAttribute.Block_Amount_Item_Min, 0]);
					Attributes[GameAttribute.Block_Amount_Item_Delta] += Math.Abs(scaleCapDelta * ratio - Attributes[GameAttribute.Block_Amount_Item_Delta, 0]);
				}
			}
		}

		private void ApplyDurability(ItemTable definition)
		{
			if (definition.BaseDurability > 0)
			{
				int durability = (definition.BaseDurability * 2) + RandomHelper.Next(definition.DurabilityVariance);
				Attributes[GameAttribute.Durability_Max] = durability;
				Attributes[GameAttribute.Durability_Cur] = durability;
			}
		}

		public void UpdateDurability(int newDurability)
		{
			Attributes[GameAttribute.Durability_Cur] = newDurability;
			this.DBInventory.Durability = newDurability;
			this.Owner.World.Game.GameDBSession.SessionUpdate(this.DBInventory);
		}

		public void UpdateTransmog(int newTransmogGBID)
		{
			Attributes[GameAttribute.TransmogGBID] = newTransmogGBID;
			this.DBInventory.TransmogGBID = newTransmogGBID;
			this.Owner.World.Game.GameDBSession.SessionUpdate(this.DBInventory);
		}

		public void SaveAttributes()
		{
			this.DBInventory.Attributes = this.Attributes.Serialize();
			this.Owner.World.Game.GameDBSession.SessionUpdate(this.DBInventory);
		}

		public void UpdateStackCount(int newCount)
		{
			if (newCount > 0)
			{
				if (this.DBInventory == null) return;
				this.Attributes[GameAttribute.ItemStackQuantityLo] = newCount;
				this.Attributes.SendChangedMessage((Owner as Player).InGameClient);

				this.DBInventory.Count = newCount;
				this.Owner.World.Game.GameDBSession.SessionUpdate(this.DBInventory);
			}
		}

		private void ApplySkills(ItemTable definition)
		{
			if (definition.SNOSkill0 != -1)
			{
				Attributes[GameAttribute.Skill, definition.SNOSkill0] = 1;
			}
			if (definition.SNOSkill1 != -1)
			{
				Attributes[GameAttribute.Skill, definition.SNOSkill1] = 1;
			}
			if (definition.SNOSkill2 != -1)
			{
				Attributes[GameAttribute.Skill, definition.SNOSkill2] = 1;
			}
			if (definition.SNOSkill3 != -1)
			{
				Attributes[GameAttribute.Skill, definition.SNOSkill3] = 1;
			}
		}

		private void ApplyAttributeSpecifier(ItemTable definition)
		{
			foreach (var effect in definition.Attribute)
			{
				float result;
				if (FormulaScript.Evaluate(effect.Formula.ToArray(), this.RandomGenerator, out result))
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
				GbId = (this.Attributes[GameAttribute.TransmogGBID] == -1 ? this.GBHandle.GBID : this.Attributes[GameAttribute.TransmogGBID]),
				DyeType = this.Attributes[GameAttribute.DyeType],
				ItemEffectType = 0,//Mooege.Common.Helpers.Math.FastRandom.Instance.Next(1, 14),
				EffectLevel = -1//Mooege.Common.Helpers.Math.FastRandom.Instance.Next(1, 30)
			};
		}

		//TODO: Move to proper D3.Hero.Visual item classes
		public D3.Hero.VisualItem GetVisualItem()
		{
			var visualItem = D3.Hero.VisualItem.CreateBuilder()
				.SetGbid((this.Attributes[GameAttribute.TransmogGBID] == -1 ? this.GBHandle.GBID : this.Attributes[GameAttribute.TransmogGBID]))
				.SetDyeType(Attributes[GameAttribute.DyeType])
				.SetEffectLevel(0)
				.SetItemEffectType(-1)
				.Build();
			return visualItem;
		}

		public int GetPrice()
		{
			int price = this.ItemDefinition.Cost;
			//if (this.AffixList.Count == 0)
			//	price *= (1 + this.ItemDefinition.BuyCostMultiplier);
			foreach (var affix in this.AffixList)
			{
				price += affix.Price;
			}
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
			this.EquipmentSlot = equipmentSlot;
			this.InventoryLocation.X = column;
			this.InventoryLocation.Y = row;
			if (this.Owner is PlayerSystem.Player)
			{
				var player = (this.Owner as PlayerSystem.Player);
				if (!this.Reveal(player))
				{
					player.InGameClient.SendMessage(this.ACDInventoryPositionMessage(player));
				}
			}
		}

		public void SetNewWorld(World world)
		{
			if (this.World == world)
				return;

			this.World = world;
		}

		public void Drop(Player owner, Vector3D position)
		{
			this.Owner = owner;
			this.EnterWorld(position);
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			player.Inventory.RefreshInventoryToClient();

			switch (this.ActorSNO.Id)
			{
				case 408416:
					player.Toon.BigPortalKey++;
					this.Destroy();
					break;
				default:
					player.Inventory.PickUp(this);
					break;
			}

			var Moneys = D3.Items.CurrencySavedData.CreateBuilder();
			D3.Items.CurrencyData GoldData = D3.Items.CurrencyData.CreateBuilder().SetId(0).SetCount((long)player.Inventory.GetGoldAmount()).Build();
			D3.Items.CurrencyData BloodShardData = D3.Items.CurrencyData.CreateBuilder().SetId(1).SetCount(player.InGameClient.BnetClient.Account.GameAccount.BloodShards).Build();
			D3.Items.CurrencyData PlatinumData = D3.Items.CurrencyData.CreateBuilder().SetId(2).SetCount(player.InGameClient.BnetClient.Account.GameAccount.Platinum).Build();
			D3.Items.CurrencyData Craft1Data = D3.Items.CurrencyData.CreateBuilder().SetId(3).SetCount(player.Toon.CraftItem1).Build();
			D3.Items.CurrencyData Craft2Data = D3.Items.CurrencyData.CreateBuilder().SetId(4).SetCount(player.Toon.CraftItem2).Build();
			D3.Items.CurrencyData Craft3Data = D3.Items.CurrencyData.CreateBuilder().SetId(5).SetCount(player.Toon.CraftItem3).Build();
			D3.Items.CurrencyData Craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(player.Toon.CraftItem4).Build();
			D3.Items.CurrencyData Craft5Data = D3.Items.CurrencyData.CreateBuilder().SetId(7).SetCount(player.Toon.CraftItem5).Build();
			D3.Items.CurrencyData Craft6Data = D3.Items.CurrencyData.CreateBuilder().SetId(16).SetCount(player.Toon.LeorikKey).Build(); //Leorik Regret
			D3.Items.CurrencyData Craft7Data = D3.Items.CurrencyData.CreateBuilder().SetId(20).SetCount(player.Toon.BigPortalKey).Build(); //Big Portal Key
			D3.Items.CurrencyData Horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8).SetCount(player.Toon.HoradricA1Res).Build();
			D3.Items.CurrencyData Horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9).SetCount(player.Toon.HoradricA2Res).Build();
			D3.Items.CurrencyData Horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10).SetCount(player.Toon.HoradricA3Res).Build();
			D3.Items.CurrencyData Horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11).SetCount(player.Toon.HoradricA4Res).Build();
			D3.Items.CurrencyData Horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12).SetCount(player.Toon.HoradricA5Res).Build();
			//CraftItemLegendary - 2073430088

			Moneys.AddCurrency(GoldData);
			Moneys.AddCurrency(BloodShardData);
			Moneys.AddCurrency(PlatinumData);
			Moneys.AddCurrency(Craft1Data);
			Moneys.AddCurrency(Craft2Data);
			Moneys.AddCurrency(Craft3Data);
			Moneys.AddCurrency(Craft4Data);
			Moneys.AddCurrency(Craft5Data);
			Moneys.AddCurrency(Craft6Data);
			Moneys.AddCurrency(Craft7Data);
			Moneys.AddCurrency(Horadric1Data);
			Moneys.AddCurrency(Horadric2Data);
			Moneys.AddCurrency(Horadric3Data);
			Moneys.AddCurrency(Horadric4Data);
			Moneys.AddCurrency(Horadric5Data);
			player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.GenericBlobMessage(Opcodes.CurrencyDataFull) { Data = Moneys.Build().ToByteArray() });

		}

		public virtual void OnRequestUse(Player player, Item target, int actionId, WorldPlace worldPlace)
		{
			if (IsPotion(this.ItemType)) //if item is health potion
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

			if (IsRecipe(this.ItemType)) //if item is crafting recipe
			{
				Logger.Trace("Learning recipe...");
				for (int i = 0; i < 10; i++)
				{
					if (this.ItemDefinition.RecipeToGrant[i] != -1)
						player.LearnRecipe(player.ArtisanInteraction, this.ItemDefinition.RecipeToGrant[i]);
					else
						break;
				}
				for (int i = 0; i < 8; i++)
				{
					if (this.ItemDefinition.TransmogsToGrant[i] != -1)
						 player.UnlockTransmog(this.ItemDefinition.TransmogsToGrant[i]);
					else
						break;
				}
				if (this.GBHandle.GBID == 1549850924) //Arma Haereticorum additional transmog
				{
					player.UnlockTransmog(974107120);
					return;
				}

				if (this.Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
					player.Inventory.DestroyInventoryItem(this); // No more recipes!
				else
				{
					this.UpdateStackCount(--this.Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
					this.Attributes.SendChangedMessage(player.InGameClient);
				}
				return;
			}

			if (IsTreasureBag(this.ItemType))
			{
				Logger.Warn("Treasure Bag system v0.2");
				string[] items = new string[1];
				int Base = 0;
				switch (player.Toon.Class)
				{
					case LoginServer.Toons.ToonClass.Crusader: Base = 2; break;
					case LoginServer.Toons.ToonClass.DemonHunter: Base = 4; break;
					case LoginServer.Toons.ToonClass.Monk: Base = 6; break;
					case LoginServer.Toons.ToonClass.Necromancer: Base = 8; break;
					case LoginServer.Toons.ToonClass.WitchDoctor: Base = 10; break;
					case LoginServer.Toons.ToonClass.Wizard: Base = 12; break;
				} //0 - Варвар, 2 - Крестоносец, 4 - Охотник, 6 - Монах, 8 - Некромант, 10 - Колдун, 12 - Волшебник
				string it = "";
				#region Калькуляция баланса
				var Moneys = D3.Items.CurrencySavedData.CreateBuilder();
				D3.Items.CurrencyData GoldData = D3.Items.CurrencyData.CreateBuilder().SetId(0).SetCount((long)player.Inventory.GetGoldAmount()).Build();
				D3.Items.CurrencyData BloodShardData = D3.Items.CurrencyData.CreateBuilder().SetId(1).SetCount(player.InGameClient.BnetClient.Account.GameAccount.BloodShards).Build();
				D3.Items.CurrencyData PlatinumData = D3.Items.CurrencyData.CreateBuilder().SetId(2).SetCount(player.InGameClient.BnetClient.Account.GameAccount.Platinum).Build();
				D3.Items.CurrencyData Craft1Data = D3.Items.CurrencyData.CreateBuilder().SetId(3).SetCount(player.Toon.CraftItem1).Build();
				D3.Items.CurrencyData Craft2Data = D3.Items.CurrencyData.CreateBuilder().SetId(4).SetCount(player.Toon.CraftItem2).Build();
				D3.Items.CurrencyData Craft3Data = D3.Items.CurrencyData.CreateBuilder().SetId(5).SetCount(player.Toon.CraftItem3).Build();
				D3.Items.CurrencyData Craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(player.Toon.CraftItem4).Build();
				D3.Items.CurrencyData Craft5Data = D3.Items.CurrencyData.CreateBuilder().SetId(7).SetCount(player.Toon.CraftItem5).Build();
				D3.Items.CurrencyData Craft6Data = D3.Items.CurrencyData.CreateBuilder().SetId(16).SetCount(player.Toon.LeorikKey).Build(); //Leorik Regret
				D3.Items.CurrencyData Craft7Data = D3.Items.CurrencyData.CreateBuilder().SetId(20).SetCount(player.Toon.BigPortalKey).Build(); //Big Portal Key
				D3.Items.CurrencyData Horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8).SetCount(player.Toon.HoradricA1Res).Build();
				D3.Items.CurrencyData Horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9).SetCount(player.Toon.HoradricA2Res).Build();
				D3.Items.CurrencyData Horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10).SetCount(player.Toon.HoradricA3Res).Build();
				D3.Items.CurrencyData Horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11).SetCount(player.Toon.HoradricA4Res).Build();
				D3.Items.CurrencyData Horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12).SetCount(player.Toon.HoradricA5Res).Build();

				//CraftItemLegendary - 2073430088

				Moneys.AddCurrency(GoldData);
				Moneys.AddCurrency(BloodShardData);
				Moneys.AddCurrency(PlatinumData);
				Moneys.AddCurrency(Craft1Data);
				Moneys.AddCurrency(Craft2Data);
				Moneys.AddCurrency(Craft3Data);
				Moneys.AddCurrency(Craft4Data);
				Moneys.AddCurrency(Craft5Data);
				Moneys.AddCurrency(Craft6Data);
				Moneys.AddCurrency(Craft7Data);
				#endregion
				switch (this.GBHandle.GBID)
				{
                    #region Дар Хедрига
                    case -1249067449:
					    items = new string[] {	"Unique_Helm_Set_15_x1", "Unique_Gloves_Set_15_x1",
												"Unique_Helm_Set_12_x1", "Unique_Gloves_Set_12_x1",
												"Unique_Helm_Set_14_x1", "Unique_Gloves_Set_14_x1",
												"Unique_Helm_Set_11_x1", "Unique_Gloves_Set_11_x1",
												"P6_Necro_Set_3_Helm",	 "P6_Necro_Set_3_Gloves",
												"Unique_Helm_Set_09_x1", "Unique_Gloves_Set_09_x1",
												"Unique_Helm_Set_06_x1", "Unique_Gloves_Set_06_x1"};
						switch (player.Toon.Class)
						{
							case LoginServer.Toons.ToonClass.Crusader:		Base = 2; break;
							case LoginServer.Toons.ToonClass.DemonHunter:	Base = 4; break;
							case LoginServer.Toons.ToonClass.Monk:			Base = 6; break;
							case LoginServer.Toons.ToonClass.Necromancer:	Base = 8; break;
							case LoginServer.Toons.ToonClass.WitchDoctor:	Base = 10; break;
							case LoginServer.Toons.ToonClass.Wizard:		Base = 12; break;
						}
						it = items[RandomHelper.Next(Base, Base+1)]; player.Inventory.PickUp(ItemGenerator.Cook(player, it));
						break;
					case -1249067448:
						items = new string[] {  "Unique_Shoulder_Set_15_x1", "Unique_Boots_Set_15_x1",
												"Unique_Shoulder_Set_12_x1", "Unique_Boots_Set_12_x1",
												"Unique_Shoulder_Set_14_x1", "Unique_Boots_Set_14_x1",
												"Unique_Shoulder_Set_11_x1", "Unique_Boots_Set_11_x1",
												"P6_Necro_Set_3_Shoulders",   "P6_Necro_Set_3_Boots",
												"Unique_Shoulder_Set_09_x1", "Unique_Boots_Set_09_x1",
												"Unique_Shoulder_Set_06_x1", "Unique_Boots_Set_06_x1"};
						it = items[RandomHelper.Next(Base, Base + 1)]; player.Inventory.PickUp(ItemGenerator.Cook(player, it));
						break;
					case -1249067447:
						items = new string[] {  "Unique_Chest_Set_15_x1", "Unique_Pants_Set_15_x1",
												"Unique_Chest_Set_12_x1", "Unique_Pants_Set_12_x1",
												"Unique_Chest_Set_14_x1", "Unique_Pants_Set_14_x1",
												"Unique_Chest_Set_11_x1", "Unique_Pants_Set_11_x1",
												"P6_Necro_Set_3_Chest",   "P6_Necro_Set_3_Pants",
												"Unique_Chest_Set_09_x1", "Unique_Pants_Set_09_x1",
												"Unique_Chest_Set_06_x1", "Unique_Pants_Set_06_x1"};
						it = items[RandomHelper.Next(Base, Base + 1)]; player.Inventory.PickUp(ItemGenerator.Cook(player, it));
						break;
					#endregion
					#region Сокровище Хорадримов
					case -1575654862: //Сокровища 1 Акта
						player.Toon.HoradricA1Res += RandomHelper.Next(1, 5);
						player.Toon.CraftItem4 += RandomHelper.Next(2, 4);
						Horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8).SetCount(player.Toon.HoradricA1Res).Build();
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
						player.World.SpawnGold(player, player, 5000);
						player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
						break;
					case -1575654861: //Сокровища 2 Акта
						player.Toon.HoradricA2Res += RandomHelper.Next(1, 5);
						player.Toon.CraftItem4 += RandomHelper.Next(2, 4);
						Horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9).SetCount(player.Toon.HoradricA2Res).Build();
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
						player.World.SpawnGold(player, player, 5000);
						player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
						break;
					case -1575654860: //Сокровища 3 Акта
						player.Toon.HoradricA3Res += RandomHelper.Next(1, 5);
						player.Toon.CraftItem4 += RandomHelper.Next(2, 4);
						Horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10).SetCount(player.Toon.HoradricA3Res).Build();
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
						player.World.SpawnGold(player, player, 5000);
						player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
						break;
					case -1575654859: //Сокровища 4 Акта
						player.Toon.HoradricA4Res += RandomHelper.Next(1, 5);
						player.Toon.CraftItem4 += RandomHelper.Next(2, 4);
						Horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11).SetCount(player.Toon.HoradricA4Res).Build();
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
						player.World.SpawnGold(player, player, 5000);
						player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
						break;
					case -1575654858: //Сокровища 5 Акта
						player.Toon.HoradricA5Res += RandomHelper.Next(1, 5);
						player.Toon.CraftItem4 += RandomHelper.Next(2, 4);
						Horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12).SetCount(player.Toon.HoradricA5Res).Build();
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(3, 8));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(5, 9));
						player.World.SpawnRandomEquip(player, player, RandomHelper.Next(7, 9));
						player.World.SpawnGold(player, player, 5000);
						player.World.SpawnBloodShards(player, player, RandomHelper.Next(10, 25));
						break;
					#endregion
					default: 
						Logger.Warn("This treasure bag - not implemented"); break;
				}
				Craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(player.Toon.CraftItem4).Build();
				Moneys.AddCurrency(Horadric1Data);
				Moneys.AddCurrency(Horadric2Data);
				Moneys.AddCurrency(Horadric3Data);
				Moneys.AddCurrency(Horadric4Data);
				Moneys.AddCurrency(Horadric5Data);
				player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.GenericBlobMessage(Opcodes.CurrencyDataFull) { Data = Moneys.Build().ToByteArray() });

				player.Inventory.DestroyInventoryItem(this);
				return;
			}

			if (this.GBHandle.GBID == 237118774) //AngelWings_Blue
			{
				SwitchWingsBuff(player, 208706);
				return;
			}

			if (this.GBHandle.GBID == -1424453175) //AngelWings_Red
			{
				SwitchWingsBuff(player, 317139);
				return;
			}

			if (this.GBHandle.GBID == -1736870778) //BugWings
			{
				SwitchWingsBuff(player, 255336);
				return;
			}

			if (this.GBHandle.GBID == -1364948604) //x1_AngelWings_Imperius
			{
				SwitchWingsBuff(player, 378292);
				return;
			}

			if (this.GBHandle.GBID == -762694428) //WoDFlag
			{
				SwitchWingsBuff(player, 375412);
				return;
			}

			if (IsDye(this.ItemType)) //if item is dye
			{
				if (target == null) return;
				target.Attributes[GameAttribute.DyeType] = this.Attributes[GameAttribute.DyeType];
				target.Attributes.BroadcastChangedIfRevealed();
				target.DBInventory.DyeType = this.Attributes[GameAttribute.DyeType];

				player.World.Game.GameDBSession.SessionUpdate(target.DBInventory);

				player.Inventory.SendVisualInventory(player);

				player.GrantAchievement(74987243307154);

				var colors = new List<int>(player.Inventory.GetEquippedItems().Where(i => i.Attributes[GameAttribute.DyeType] > 0).Select(i => i.Attributes[GameAttribute.DyeType]));
				if (colors.Count >= 6)
				{
					if (new HashSet<int>(colors).Count == 1)
						player.GrantAchievement(74987243307156);
					if (new HashSet<int>(colors).Count >= 6)
						player.GrantAchievement(74987243307157);
				}

				switch ((uint)this.GBHandle.GBID)
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

				if (this.GBHandle.GBID == 1866876233 || this.GBHandle.GBID == 1866876234) return; //CE dyes

				if (this.Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
					player.Inventory.DestroyInventoryItem(this); // No more dyes!
				else
				{
					this.UpdateStackCount(--this.Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
					this.Attributes.SendChangedMessage(player.InGameClient);
				}



				return;
			}

			Logger.Warn("OnRequestUse(): gbid {0} not implemented", this.GBHandle.GBID);
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

			bool activated = (player.Attributes[GameAttribute.Buff_Exclusive_Type_Active, powerId] == true);

			player.CurrentWingsPowerId = activated ? -1 : powerId;

			player.Attributes[GameAttribute.Buff_Exclusive_Type_Active, powerId] = !activated;
			player.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, powerId] = !activated;
			player.Attributes[GameAttribute.Buff_Icon_Start_Tick0, powerId] = 0;
			player.Attributes[GameAttribute.Buff_Icon_End_Tick0, powerId] = (activated ? 0 : 100);
			player.Attributes[GameAttribute.Buff_Icon_Count0, powerId] = (activated ? 0 : 1);
			player.Attributes.BroadcastChangedIfRevealed();
			player.Inventory.SendVisualInventory(player);
			var dbToon = player.Toon.DBToon;
			dbToon.WingsActive = player.CurrentWingsPowerId;
			player.World.Game.GameDBSession.SessionUpdate(dbToon);
			return;
		}

		public override bool Reveal(Player player)
		{
			if (this.CurrentState == ItemState.PickingUp && HasWorldLocation)
				return false;

			foreach (var gplayer in player.World.Game.Players.Values)
			{
				if (gplayer.GroundItems.ContainsKey(this.GlobalID) && gplayer != player)
					return false;
			}

			if (!base.Reveal(player))
				return false;

			if (AffixList.Count > 0)
			{
				var affixGbis = new int[AffixList.Count];
				for (int i = 0; i < AffixList.Count; i++)
				{
					affixGbis[i] = AffixList[i].AffixGbid;
				}

				player.InGameClient.SendMessage(new AffixMessage()
				{
					ActorID = DynamicID(player),
					Field1 = (this.Unidentified ? 0x00000002 : 0x00000001),
					aAffixGBIDs = affixGbis,
				});
			}

			foreach (var gem in this.Gems)
				gem.Reveal(player);

			if (this.RareItemName != null)
				player.InGameClient.SendMessage(new RareItemNameMessage()
				{
					ann = DynamicID(player),
					RareItemName = new RareItemName()
					{
						Field0 = this.RareItemName.ItemNameIsPrefix,
						snoAffixStringList = this.RareItemName.SnoAffixStringList,
						AffixStringListIndex = this.RareItemName.AffixStringListIndex,
						ItemStringListIndex = this.RareItemName.ItemStringListIndex
					}
				});
			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (CurrentState == ItemState.PickingUp)// && player == Owner)
			{
				return false;
			}

			foreach (var gem in this.Gems)
				gem.Unreveal(player);

			return base.Unreveal(player);
		}

		private bool ZPositionCorrected = false;

		public override void OnPlayerApproaching(Player player)
		{
			if (PowerMath.Distance2D(player.Position, this.Position) < 3f && !this.ZPositionCorrected)
			{
				foreach (var gplayer in player.World.Game.Players.Values)
				{
					if (gplayer.GroundItems.ContainsKey(this.GlobalID) && gplayer != player)
						return;
				}

				this.ZPositionCorrected = true;
				this.Teleport(new Vector3D(this.Position.X, this.Position.Y, player.Position.Z));
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
