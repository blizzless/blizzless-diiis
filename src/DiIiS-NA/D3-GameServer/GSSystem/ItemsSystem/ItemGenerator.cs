//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using NHibernate.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;

//Blizzless Project 2022 
using Scene = DiIiS_NA.GameServer.GSSystem.MapSystem.Scene;
//Blizzless Project 2022 
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
//Blizzless Project 2022 
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public static class ItemGenerator
	{
		public static readonly Logger Logger = LogManager.CreateLogger("D3Core");

		public static readonly ConcurrentDictionary<int, ItemTable> Items = new ConcurrentDictionary<int, ItemTable>();
		public static readonly ConcurrentDictionary<int, ItemTable> AllowedItems = new ConcurrentDictionary<int, ItemTable>();
		public static readonly ConcurrentDictionary<int, ItemTable> AllowedUniqueItems = new ConcurrentDictionary<int, ItemTable>();
		private static readonly ConcurrentDictionary<int, RecipeTable> Recipes = new ConcurrentDictionary<int, RecipeTable>();
		private static readonly ConcurrentDictionary<int, ItemTable> Lore = new ConcurrentDictionary<int, ItemTable>();
		private static readonly List<SocketedEffectTable> GemEffects = new List<SocketedEffectTable>();
		private static readonly List<ParagonBonusesTable> ParagonBonuses = new List<ParagonBonusesTable>();
		private static readonly List<SetItemBonusTable> ItemSetsEffects = new List<SetItemBonusTable>();
		private static readonly ConcurrentDictionary<int, Type> GBIDHandlers = new ConcurrentDictionary<int, Type>();
		private static readonly ConcurrentDictionary<int, Type> TypeHandlers = new ConcurrentDictionary<int, Type>();
		private static readonly HashSet<int> AllowedItemTypes = new HashSet<int>();
		private static readonly List<int> CraftOnlyItems = new List<int>();

		public static int TotalItems
		{
			get { return Items.Count; }
		}

		public static List<int> Tutorials = new List<int>();
		public static Dictionary<int, Quest> Bounties = new Dictionary<int, Quest>();

		static ItemGenerator()
		{
			Player.GeneratePLB();
			Logger.Info("Loading Recipes...");
			Logger.Info("Loading Items...");
			LoadRecipes();
			LoadItems();
			Logger.Info("Loading Paragons...");
			LoadParagonBonuses();
			//LoadAffixes(); //just for checking values
			//LoadPowers();
			//LoadQuests();
			Logger.Info("Loading Tutorials...");
			Tutorials = MPQStorage.Data.Assets[SNOGroup.Tutorial].Keys.OrderBy(i => i).ToList();
			Logger.Info("Loading Bonuses...");
			LoadItemSetBonuses();
			LoadGemBonuses();
			Logger.Info("Loading Handlers...");
			LoadHandlers();
			Logger.Info("Loading Lore...");
			LoadLore(); 
			Logger.Info("Loading Bounties...");
			LoadBounties();
			//LoadConversations();
			//if (Net.GS.Config.Instance.Enabled)

			Logger.Info("Loading Worlds...");
			Scene.PreCacheMarkers();

			SetAllowedTypes();
			System.Threading.Thread.CurrentThread.Name = "ItemGenerator";
		}

		#region loading generator
		private static void LoadBounties()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Quest].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Quest data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Quest;
				if (data != null && data.QuestType == QuestType.Bounty)
				{
					if (data.BountyData0.ActData == BountyData.ActT.Invalid)
					{
						if (asset.Name.Contains("_A1_"))
							data.BountyData0.ActData = BountyData.ActT.A1;
						if (asset.Name.Contains("_A2_"))
							data.BountyData0.ActData = BountyData.ActT.A2;
						if (asset.Name.Contains("_A3_"))
							data.BountyData0.ActData = BountyData.ActT.A3;
						if (asset.Name.Contains("_A4_"))
							data.BountyData0.ActData = BountyData.ActT.A4;
						if (asset.Name.Contains("_A5_"))
							data.BountyData0.ActData = BountyData.ActT.A5;
					}
					//fixes for bugged bounties
					if (data.BountyData0.Type != BountyData.BountyType.CompleteEvent && data.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Any(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.CompleteQuest))
						data.BountyData0.Type = BountyData.BountyType.CompleteEvent;
					if (data.BountyData0.Type == BountyData.BountyType.KillUnique && data.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Any(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillAll))
						data.BountyData0.Type = BountyData.BountyType.ClearDungeon;
					if (data.BountyData0.Type == BountyData.BountyType.KillUnique && !data.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Any(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillAny))
						continue;
					if (data.BountyData0.Type == BountyData.BountyType.KillUnique && data.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Where(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster).Count() > 1)
						continue;

					Bounties.Add(data.Header.SNOId, data);
				}
			}
		}

		private static void LoadHandlers()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(Item))) continue;

				var attributes = (HandledItemAttribute[])type.GetCustomAttributes(typeof(HandledItemAttribute), true);
				if (attributes.Length != 0)
				{
					foreach (var name in attributes.First().Names)
					{
						GBIDHandlers.TryAdd(StringHashHelper.HashItemName(name), type);
					}
				}

				var typeAttributes = (HandledTypeAttribute[])type.GetCustomAttributes(typeof(HandledTypeAttribute), true);
				if (typeAttributes.Length != 0)
				{
					foreach (var typeName in typeAttributes.First().Types)
					{
						TypeHandlers.TryAdd(StringHashHelper.HashItemName(typeName), type);
					}
				}
			}
		}

		private static void LoadItems()
		{
			//Logger.Info("LoadItems()");
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.Items)
				{
					foreach (var itemDefinition in data.Item)
					{
						Items.TryAdd(itemDefinition.Hash, itemDefinition);

						if (itemDefinition.Name.EndsWith("_104") && itemDefinition.Name.Count(i => Char.IsDigit(i)) > 4)
						{
							ObsoleteItems.Add(itemDefinition.Hash);
							//Logger.Debug("flagged as obsolete _104: {0}, {1}", itemDefinition.Name, itemDefinition.Hash);
						}
						if (itemDefinition.Name.EndsWith("_1xx"))
						{
							ObsoleteItems.Add(itemDefinition.Hash);
							//Logger.Debug("flagged as obsolete _1xx: {0}, {1}", itemDefinition.Name, itemDefinition.Hash);
						}
						if (itemDefinition.Name.ToLower().Contains("unique") && !itemDefinition.Name.EndsWith("_x1") &&
							!itemDefinition.Name.EndsWith("_104") && !itemDefinition.Name.EndsWith("_1xx"))
						{
							ObsoleteItems.Add(itemDefinition.Hash);
							//Logger.Debug("flagged as obsolete 1.0.3: {0}, {1}", itemDefinition.Name, itemDefinition.Hash);
						}

						if (
							itemDefinition.SNOActor != 4812 //orb_norm_base_03
							&& !(itemDefinition.Name.ToLower().Contains("stone") && !itemDefinition.Name.ToLower().Contains("spiritstone")) //StoneOfRecall
							&& !itemDefinition.Name.ToLower().StartsWith("ph_") //Kadala items
							&& !itemDefinition.Name.ToLower().Contains("talisman") //TalismanUnlock
							&& !itemDefinition.Name.ToLower().Contains("console") //bugged console items
							&& !itemDefinition.Name.ToLower().Contains("scroll") //any kind of scrolls
							&& !itemDefinition.Name.ToLower().Contains("powerpotion") //PowerPotion
							&& !itemDefinition.Name.ToLower().Contains("cow") //StaffOfCow
							&& !itemDefinition.Name.ToLower().Contains("wings") //AngelWings
							&& !itemDefinition.Name.ToLower().Contains("wodflag") //WoDFlag
							&& !itemDefinition.Name.ToLower().Contains("ptr") //WoDFlag
							&& !itemDefinition.Name.ToLower().Contains("crafting") //recipes and reagents
							&& !itemDefinition.Name.ToLower().Contains("nephalemcube") //NephalemCube
							&& !itemDefinition.Name.ToLower().Contains("oftheancients") //BladeoftheAncients
							&& !itemDefinition.Name.ToLower().Contains("giyuasfang") //Set_001_GiyuasFang
							&& !itemDefinition.Name.ToLower().Contains("thortest") //ThorTest items
							&& !itemDefinition.Name.ToLower().Contains("promo") //Promo items
							&& !itemDefinition.Name.ToLower().Contains("shadowclone") //shadowClone OP weapon
							//&& !(itemDefinition.Name.Contains("_Set_") && itemDefinition.Name.EndsWith("_x1")) //RoS sets
							//&& !(itemDefinition.Name.ToLower().Contains("unique") && (itemDefinition.BonusAffixes + itemDefinition.BonusMajorAffixes + itemDefinition.BonusMinorAffixes == 0) && itemDefinition.LegendaryAffixFamily[0] == -1) //2.1+ items
							&& !(CraftOnlyItems.Contains(itemDefinition.Hash) && itemDefinition.Name.ToLower().Contains("unique")) //crafted
							&& !(CraftOnlyItems.Contains(itemDefinition.Hash) && itemDefinition.Name.ToLower().Contains("unique")) //crafted
							//&& !(itemDefinition.Name.ToLower().Contains("_10") && itemDefinition.Name.ToLower().Contains("_x1")) //"future" items
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Book")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("TreasureBag")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Journal")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Scroll")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Utility")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Unknown")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Dye")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("Shard")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("GreaterShard")
							&& itemDefinition.ItemTypesGBID != StringHashHelper.HashItemName("HealthPotion")
							)
						{
							if (!IsBuggedItem(itemDefinition))
							{
								if (itemDefinition.Name.ToLower().Contains("_set_") ||
									itemDefinition.Name.ToLower().Contains("unique") ||
									itemDefinition.Name.ToLower().StartsWith("p71_ethereal")
									)
								{
									AllowedUniqueItems.TryAdd(itemDefinition.Hash, itemDefinition);
									AllowedItems.TryAdd(itemDefinition.Hash, itemDefinition);
								}
								else
									AllowedItems.TryAdd(itemDefinition.Hash, itemDefinition);
								//Logger.Debug("flagged as allowed item: {0}, {1}", itemDefinition.Name, itemDefinition.Hash);

							}
						}

					}
				}
			}
		}

		private static bool IsBuggedItem(ItemTable definition) 
		{
			switch (definition.Name)
			{
				case "Unique_Axe_1H_101_x1":
				case "Unique_Axe_1H_102_x1":
				case "Unique_Axe_2H_101_x1":
				case "Unique_Axe_2H_102_x1":
				case "Unique_Axe_2H_103_x1":
				case "Unique_Axe_2H_104_x1":
				case "Unique_BarbBelt_102_x1":
				case "Unique_BarbBelt_103_x1":
				case "Unique_BarbBelt_104_x1":
				case "Unique_BarbBelt_105_x1":
				case "Unique_Belt_103_x1":
				case "Unique_Boots_102_x1":
				case "Unique_Boots_103_x1":
				case "Unique_Bow_102_x1":
				case "Unique_Bow_103_x1":
				case "Unique_Bow_104_x1":
				case "Unique_Bracer_105_x1":
				case "Unique_Bracer_108_x1":
				case "Unique_Cloak_102_x1":
				case "Unique_CruShield_104_x1":
				case "Unique_CruShield_108_x1":
				case "Unique_Dagger_101_x1":
				case "Unique_Dagger_102_x1":
				case "Unique_Dagger_103_x1":
				case "Unique_Fist_102_x1":
				case "Unique_Gloves_103_x1":
				case "Unique_HandXBow_102_x1":
				case "Unique_Helm_103_x1":
				case "Unique_Mace_1H_101_x1":
				case "Unique_Mace_2H_104_x1":
				case "Unique_Mighty_1H_101_x1":
				case "Unique_Mighty_1H_102_x1":
				case "Unique_Mighty_1H_103_x1":
				case "Unique_Mighty_1H_104_x1":
				case "Unique_Pants_102_x1":
				case "Unique_Polearm_102_x1":
				case "Unique_Shield_103_x1":
				case "Unique_Shield_104_x1":
				case "Unique_Shield_105_x1":
				case "Unique_Shield_106_x1":
				case "Unique_Shield_107_x1":
				case "Unique_Shoulder_103_x1":
				case "Unique_Spear_102_x1":
				case "Unique_Staff_104_x1":
				case "Unique_Sword_1H_105_x1":
				case "Unique_Sword_1H_106_x1":
				case "Unique_Sword_1H_107_x1":
				case "Unique_Sword_1H_108_x1":
				case "Unique_Sword_1H_110_x1":
				case "Unique_Sword_1H_112_x1":
				case "Unique_Sword_2H_103_x1":
				case "Unique_Amulet_105_x1":
				case "Unique_Amulet_106_x1":
				case "Unique_Ring_105_x1":
				case "Unique_Ring_107_x1":
				case "Unique_Ring_108_x1":
				case "Unique_Ring_109_x1":
				case "Unique_Mojo_101_x1":
				case "Unique_Mojo_104_x1":
				case "Unique_Quiver_104_x1":
					return true;
			}
			return false;
		}
		private static void LoadQuests()
		{
			//Logger.Info("LoadItems()");
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Quest].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Quest data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Quest;
				if (data != null)
				{
					Logger.Info("-------------");
					Logger.Info("Quest: [{0}] {1}", data.Header.SNOId, asset.Name);
					Logger.Info("Type: {0}", data.QuestType);
					Logger.Info("NumberOfSteps: {0}, NumberOfCompletionSteps: {1}", data.NumberOfSteps, data.NumberOfCompletionSteps);
					foreach (var step in data.QuestSteps)
					{
						int nextID = step.StepObjectiveSets.Count() > 0 ? step.StepObjectiveSets.First().FollowUpStepID : -1;
						Logger.Info("Step [{0}] {1} -> {2}", step.ID, step.Name, nextID);
						foreach (var objSet in step.StepObjectiveSets)
							foreach (var obj in objSet.StepObjectives)
								Logger.Info("objective type {0}, I0 {1}, I2 {2}, Target {3}, Sno1 {4}, Sno2 {5}", obj.ObjectiveType, obj.I0, obj.I2, obj.CounterTarget, obj.SNOName1, obj.SNOName2);
					}
				}
			}
		}

		private static void LoadRecipes()
		{
			//Logger.Info("LoadRecipes()");
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.Recipes)
				{
					foreach (var recipeDefinition in data.Recipes)
					{
						Recipes.TryAdd(recipeDefinition.Hash, recipeDefinition);
						if (recipeDefinition.SNORecipe > 0 && MPQStorage.Data.Assets[SNOGroup.Recipe].ContainsKey(recipeDefinition.SNORecipe))
						{
							var reward = (MPQStorage.Data.Assets[SNOGroup.Recipe][recipeDefinition.SNORecipe].Data as DiIiS_NA.Core.MPQ.FileFormats.Recipe).ItemSpecifierData.ItemGBId;
							if (!CraftOnlyItems.Contains(reward))
								CraftOnlyItems.Add(reward);
						}
						/*Logger.Info("-------------");
						Logger.Info("recipe name: {0}, SNOId {1}", recipeDefinition.Name, recipeDefinition.SNORecipe);
						Logger.Info("recipe type: {0}", recipeDefinition.Type);
						Logger.Info("RecipeNeeded: {0}, I1: {1}, Gold: {2}, I3: {3}", recipeDefinition.RecipeNeeded, recipeDefinition.I1, recipeDefinition.Gold, recipeDefinition.I3);
						if (recipeDefinition.SNORecipe > 0)
						{
							var reward = (Mooege.Common.MPQ.FileFormats.Recipe)MPQStorage.Data.Assets[SNOGroup.Recipe][recipeDefinition.SNORecipe].Data;
							string affixes = "";
							for (int i = 0; i<3; i++)
								if (reward.ItemSpecifierData.GBIdAffixes[i] > 0)
									affixes += string.Format("{0}, ", reward.ItemSpecifierData.GBIdAffixes[i]);
							Logger.Info("recipe reward- item {0}, I0 {1}, I1 {2}, I2 {3}, Affixes: {4}", reward.ItemSpecifierData.ItemGBId, reward.ItemSpecifierData.I0, reward.ItemSpecifierData.I1, reward.ItemSpecifierData.I2, affixes);
						}
						for (int i = 0; i<6; i++)
							if (recipeDefinition.Ingredients[i].ItemGBId > 0)
								Logger.Info("Ingredient {0}: {1}, amount of {2}", i+1, recipeDefinition.Ingredients[i].ItemGBId, recipeDefinition.Ingredients[i].Count);*/
					}
				}
			}
		}

		private static void LoadGemBonuses()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.SocketedEffects)
				{
					foreach (var gemBonusDefinition in data.SocketedEffects)
					{
						GemEffects.Add(gemBonusDefinition);
						//Logger.Info("gemBonusDefinition: {0} {1} {2} {3}", gemBonusDefinition.Item, gemBonusDefinition.ItemType, gemBonusDefinition.Attribute[0].AttributeId, gemBonusDefinition.S0);
					}
				}
			}
		}

		private static void LoadParagonBonuses()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.ParagonBonuses)
				{
					foreach (var paragonBonusDefinition in data.ParagonBonusesTables)
					{
						ParagonBonuses.Add(paragonBonusDefinition);
					}
				}
			}
		}

		private static void LoadItemSetBonuses()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.SetItemBonuses)
				{
					foreach (var itemSetBonusDefinition in data.SetItemBonus)
					{
						ItemSetsEffects.Add(itemSetBonusDefinition);
						/*Logger.Info("ItemSetsEffectDefinition: {0}, set {1}, {2} bonuses:", itemSetBonusDefinition.Name, itemSetBonusDefinition.Set, itemSetBonusDefinition.Count);
						for (int i = 0; i < itemSetBonusDefinition.Count; i++)
							Logger.Info("{0}, {1}", itemSetBonusDefinition.Attribute[i].AttributeId, itemSetBonusDefinition.Attribute[i].SNOParam);*/
					}
				}
			}
		}

		private static void LoadLore()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Actor].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Actor data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Actor;
				if (data != null && data.TagMap.ContainsKey(ActorKeys.Lore))
				{
					if (Lore.ContainsKey(data.TagMap[ActorKeys.Lore].Id)) continue;
					var item = Items.Where(i => i.Value.SNOActor == data.Header.SNOId).ToList();
					if (item.Count == 0) continue;
					Lore.TryAdd(data.TagMap[ActorKeys.Lore].Id, item.First().Value);
					//Logger.Info("LoreActor: {0}, Lore {1}, GbId {2}", data.Header.SNOId, data.TagMap[ActorKeys.Lore], item.First().Value.Hash);
				}
			}
		}

		private static void LoadPowers()
		{
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Actor].Values)
			{
				Mooege.Common.MPQ.FileFormats.Actor data = asset.Data as Mooege.Common.MPQ.FileFormats.Actor;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("asset type: {0}", data.Type);
				if (data.TagMap.ContainsKey(ActorKeys.GizmoGroup))
					Logger.Info("asset group: {0}", (GizmoGroup)data.TagMap[ActorKeys.GizmoGroup]);
				Logger.Info("MonsterSNO: {0}", data.MonsterSNO);
				Logger.Info("I0: {0}, I1: {1}, I2: {2}, I3: {3}, I4: {4}", data.Int0, data.Int1, data.Int2, data.Int3, data.Int4);
				Logger.Info("Type: {0}", data.Type);
				foreach (var tag in data.TagMap)
					Logger.Info("TagMap tag: {0}", tag.ToString());
			}*/
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Worlds].Values)
			{
				Mooege.Common.MPQ.FileFormats.World data = asset.Data as Mooege.Common.MPQ.FileFormats.World;
				if (asset.Name.ToLower().StartsWith("x1_") && data.IsGenerated && !asset.Name.ToLower().Contains("_lr_"))
				{
					Logger.Info("World {0} - {1}", asset.SNOId, asset.Name);
					foreach (var scene_asset in MPQStorage.Data.Assets[SNOGroup.LevelArea].Values)
						if (scene_asset.Name.ToLower().Contains(asset.Name.ToLower()))
							Logger.Info("LevelArea {0} - {1}", scene_asset.SNOId, scene_asset.Name);
					foreach (var scene_asset in MPQStorage.Data.Assets[SNOGroup.Weather].Values)
						if (scene_asset.Name.ToLower().Contains(asset.Name.ToLower()))
							Logger.Info("Weather {0} - {1}", scene_asset.SNOId, scene_asset.Name);
					foreach (var scene_asset in MPQStorage.Data.Assets[SNOGroup.Scene].Values)
						if (scene_asset.Name.ToLower().StartsWith("x1_" + asset.Name.ToLower().Split("_")[1]))
						{
							if (LookupCommand.GetExitBits(scene_asset) == 0 && !scene_asset.Name.ToLower().Contains("filler")) continue;
							//Logger.Info("Scene {0} - {1}", scene_asset.SNOId, scene_asset.Name);
							int type = scene_asset.Name.Contains("Filler") ? 401 : (scene_asset.Name.Contains("Entrance") ? 200 : (scene_asset.Name.Contains("Exit") ? 300 : 100));
							Logger.Info("INSERT INTO \"TileInfo\" VALUES ({0}, {1}, {2}, 100, 'True','True',0,0,0,'True',0,0,NULL); // {3}", LookupCommand.GetExitBits(scene_asset), type, scene_asset.SNOId, scene_asset.Name);
						}
					Logger.Info("----------------");
				}
				
				//Logger.Info("INSERT OR IGNORE INTO \"TOC\" VALUES ('Worlds',{0},'{1}');", asset.SNOId, asset.Name);
				/*Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("Int1: {0}, Int2: {1}", data.Int1, data.Int2);
				Logger.Info("Int4: {0}, Int5: {1}", data.Int4, data.Int5);
				Logger.Info("IsGenerated: {0}", data.IsGenerated);*/
			//}
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.QuestRange].Values)
			{
				Mooege.Common.MPQ.FileFormats.QuestRange data = asset.Data as Mooege.Common.MPQ.FileFormats.QuestRange;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("I0: {0}", data.I0);
				foreach (var range in data.Entries)
				{
					Logger.Info("---Entry---");
					Logger.Info("Start: quest {0}, step: {1}", range.Start.SNOQuest, range.Start.StepID);
					Logger.Info("End: quest {0}, step: {1}", range.End.SNOQuest, range.End.StepID);
				}
			}*/
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Act].Values)
			{
				Mooege.Common.MPQ.FileFormats.Act data = asset.Data as Mooege.Common.MPQ.FileFormats.Act;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				foreach (var waypoint in data.WayPointInfo)
				{
					Logger.Info("---Waypoint---");
					Logger.Info("GameModeFlags: {0}", waypoint.GameModeFlags);
					Logger.Info("World: {0}", waypoint.SNOWorld);
					Logger.Info("LevelArea: {0}", waypoint.SNOLevelArea);
					Logger.Info("I0: {0}", waypoint.I0);
					Logger.Info("I1: {0}", waypoint.I1);
					Logger.Info("I2: {0}", waypoint.I2);
					Logger.Info("QuestRange: {0}", waypoint.SNOQuestRange);
					Logger.Info("I3: {0}", waypoint.I3);
					Logger.Info("V0: X {0}, Y {1}", waypoint.V0.X, waypoint.V0.Y);
				}
			}*/
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Scene].Values)
			{
				Mooege.Common.MPQ.FileFormats.Scene data = asset.Data as Mooege.Common.MPQ.FileFormats.Scene;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("squaresCount: X {0}, Y {1}, NavMesh {2}", data.NavMesh.SquaresCountX, data.NavMesh.SquaresCountY, data.NavMesh.NavMeshSquareCount);
				Logger.Info("Int0: {0}, Int1: {1}", data.Int0, data.Int1);
				Logger.Info("HasGeodata: {0}", data.NavMesh.HasGeodata);
				Logger.Info("AABB Min: {0} {1} {2}", data.AABBBounds.Min.X, data.AABBBounds.Min.Y, data.AABBBounds.Min.Z);
				Logger.Info("AABB Max: {0} {1} {2}", data.AABBBounds.Max.X, data.AABBBounds.Max.Y, data.AABBBounds.Max.Z);
			}*/
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Monster].Values)
			{
				Mooege.Common.MPQ.FileFormats.Monster data = asset.Data as Mooege.Common.MPQ.FileFormats.Monster;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("ActorSNO: {0}", data.ActorSNO);
				Logger.Info("I0: {0}, I1: {1}, I2: {2}, I3: {3}, I4: {4}, I5: {5}, I6: {6}, I7: {7}, I8: {8}, I9: {9}, I10: {10}, I11: {11}", data.I0, data.I1, data.I2, data.I3, data.I4, data.I5, data.I6, data.I7, data.I8, data.I9, data.I10, data.I11);
				Logger.Info("Type: {0}", data.Type);
				Logger.Info("MonsterDef: {0} - {1} - {2} - {3} - {4}", data.Monsterdef.F0, data.Monsterdef.F1, data.Monsterdef.F2, data.Monsterdef.F3, data.Monsterdef.I0);
				foreach (var skill in data.SkillDeclarations)
					Logger.Info("SkillDeclaration: {0} - {1}", skill.SNOPower, skill.I0);
				foreach (var mskill in data.MonsterSkillDeclarations)
					Logger.Info("MonsterSkillDeclaration: {0} - {1} - {2} - {3}", mskill.F0, mskill.F1, mskill.I0, mskill.F2);
				foreach (var summon in data.SNOSummonActor)
					Logger.Info("SNOSummonActor: {0}", summon);
				foreach (var tag in data.TagMap)
				Logger.Info("TagMap tag: {0}", tag.ToString());
			for (int i = 0; i<138; i++)
				Logger.Info("Float{0}: {1}", i, data.Floats[i]);
			}*/
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Power].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Power data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Power;
				Logger.Info("{0} => array(\"name\" => \"{1}\", \"desc\" => \"{2}\"),", data.Header.SNOId, asset.Name, data.LuaName);
				/*Logger.Info("---------------------------------------------------------");
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("SNOID: {0}", data.Header.SNOId);
				Logger.Info("LuaName: {0}", data.LuaName);
				Logger.Info("I0: {0}, I1: {1}, I3: {2}", data.I0, data.I1, data.i3);
				Logger.Info("String1: {0}", data.Chararray2);
				Logger.Info("SNOQuestMetaData: {0}", data.SNOQuestMetaData);
				Logger.Info("CompiledScript: {0}", data.CompiledScript);
				Logger.Info("Power Definition I0: {0}", data.Powerdef.I0);
				foreach (var anim in data.Powerdef.PVPGeneralTagMap)
					Logger.Info("PVPGeneralTagMap tag: {0}", anim.ToString());
				foreach (var anim in data.Powerdef.GeneralTagMap)
					Logger.Info("GeneralTagMap tag: {0}", anim.ToString());
				foreach (var anim in data.Powerdef.ContactTagMap0)
					Logger.Info("ContactTagMap0 tag: {0}", anim.ToString());
				foreach (var anim in data.Powerdef.ContactTagMap1)
					Logger.Info("ContactTagMap1 tag: {0}", anim.ToString());
				foreach (var anim in data.Powerdef.ContactTagMap2)
					Logger.Info("ContactTagMap2 tag: {0}", anim.ToString());
				foreach (var anim in data.Powerdef.ContactTagMap3)
					Logger.Info("ContactTagMap3 tag: {0}", anim.ToString());
				Logger.Info("Power Scripts:");
				foreach (var script in data.ScriptFormulaDetails)
				{
					Logger.Info("---------");
					Logger.Info("Script CharArray1: {0}", script.CharArray1);
					Logger.Info("Script CharArray2: {0}", script.CharArray2);
					Logger.Info("Script I0: {0}", script.I0);
					Logger.Info("Script I1: {0}", script.I1);
				}*/
			}

			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Tutorial].Values)
			{
				Mooege.Common.MPQ.FileFormats.Tutorial data = asset.Data as Mooege.Common.MPQ.FileFormats.Tutorial;
				Logger.Info("---------------------------------------------------------");
				Logger.Info("asset id: {0}", asset.SNOId);
				Logger.Info("asset name: {0}", asset.Name);
				Logger.Info("Int0: {0}, Int1: {1}", data.I0, data.I1);
				Logger.Info("Int2: {0}, Int3: {1}", data.I2, data.I3);
			}*/
		}

		private static void LoadAffixes()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				/*if (data != null && data.Type == BalanceType.AffixList)
				{
					foreach (var affixDefinition in data.Affixes)
					{
						Logger.Info("---------------------------------------------------------");
						Logger.Info("affix GBid: {0}", affixDefinition.Hash);
						Logger.Info("affix name: {0}", affixDefinition.Name);
						Logger.Info("affix Level: {0}, item Level: {1}, level min: {2}, level max: {3}", affixDefinition.AffixLevel, affixDefinition.ItemLevel, affixDefinition.MinLevel, affixDefinition.MaxLevel);
						Logger.Info("I201: {0}", affixDefinition.I201);
						Logger.Info("I202: {0}", affixDefinition.I202);
						Logger.Info("I203: {0}", affixDefinition.I203);
						Logger.Info("I204: {0}", affixDefinition.I204);
						Logger.Info("I205: {0}", affixDefinition.I205);
						Logger.Info("I206: {0}", affixDefinition.I206);
						Logger.Info("I207: {0}", affixDefinition.I207);
						Logger.Info("I208: {0}", affixDefinition.I208);
						Logger.Info("I209: {0}", affixDefinition.I209);
						Logger.Info("I210: {0}", affixDefinition.I210);
						Logger.Info("I211: {0}", affixDefinition.I211);
						Logger.Info("I212: {0}", affixDefinition.I212);
						Logger.Info("Type-1: {0}, Type-2: {1}, Class: {2}, QualityMask: {3}, SupMask: {4}", affixDefinition.AffixType1, affixDefinition.AffixType2, affixDefinition.Class, affixDefinition.QualityMask, affixDefinition.SupMask);
						Logger.Info("I0: {0}, I3: {1}, I6: {2}", affixDefinition.I0, affixDefinition.I3, affixDefinition.I6);
						Logger.Info("ItemGroups: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}", affixDefinition.ItemGroup[0], affixDefinition.ItemGroup[1], affixDefinition.ItemGroup[2], affixDefinition.ItemGroup[3], affixDefinition.ItemGroup[4], affixDefinition.ItemGroup[5], affixDefinition.ItemGroup[6], affixDefinition.ItemGroup[7], affixDefinition.ItemGroup[8], affixDefinition.ItemGroup[9], affixDefinition.ItemGroup[10], affixDefinition.ItemGroup[11], affixDefinition.ItemGroup[12], affixDefinition.ItemGroup[13], affixDefinition.ItemGroup[14], affixDefinition.ItemGroup[15]);
						Logger.Info("I10: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}", affixDefinition.I10[0], affixDefinition.I10[1], affixDefinition.I10[2], affixDefinition.I10[3], affixDefinition.I10[4], affixDefinition.I10[5], affixDefinition.I10[6], affixDefinition.I10[7], affixDefinition.I10[8], affixDefinition.I10[9], affixDefinition.I10[10], affixDefinition.I10[11], affixDefinition.I10[12], affixDefinition.I10[13], affixDefinition.I10[14], affixDefinition.I10[15]);
						Logger.Info("AffixFamily0: {0}", affixDefinition.AffixFamily0);
						Logger.Info("AffixFamily1: {0}", affixDefinition.AffixFamily1);
						Logger.Info("AssociatedAffix: {0}", affixDefinition.AssociatedAffix);
						Logger.Info("ExclusionCategory: {0}", affixDefinition.ExclusionCategory);
						Logger.Info("Price: {0}", affixDefinition.Price);
						foreach (var attribute in affixDefinition.AttributeSpecifier)
						{
							float result;
							float minValue;
							float maxValue;
							if (FormulaScript.Evaluate(attribute.Formula.ToArray(), new ItemRandomHelper(35674658), out result, out minValue, out maxValue))
								Logger.Info("AttributeSpecifier - attribute {0}, param {1}, value {2}-{3}", GameAttribute.Attributes[attribute.AttributeId].Name, attribute.SNOParam, minValue, maxValue);
						}
					}
				}*/
				/*if (data != null && data.Type == BalanceType.SocketedEffects)
				{
					foreach (var affixDefinition in data.SocketedEffects)
					{
						Logger.Info("affix: {1},	{2}	{3},	AffixName: {0}", affixDefinition.Name, affixDefinition.S0, affixDefinition.Item, affixDefinition.ItemType);
						foreach(var attr in affixDefinition.Attribute)
						{
							float result;
							if (attr.AttributeId > 0)
								if (FormulaScript.Evaluate(attr.Formula.ToArray(), new ItemRandomHelper(35674658), out result))
									Logger.Info("attr: {0},	{1}	{2}", attr.AttributeId, attr.SNOParam, result);
						}
						foreach(var attr in affixDefinition.ReqAttribute)
						{
							float result;
							if (attr.AttributeId > 0)
								if (FormulaScript.Evaluate(attr.Formula.ToArray(), new ItemRandomHelper(35674658), out result))
									Logger.Info("reqAttr: {0},	{1}	{2}", attr.AttributeId, attr.SNOParam, result);
						}
					}
				}*/
				/*if (data != null && data.Type == BalanceType.ItemTypes)
				{
					foreach (var itemType in data.ItemType)
					{
						Logger.Info("item type: {1},	parent {2},	I0: {3},	Flags {4},	Type0 {5},	Type1{6},	Type2 {7},	Type3 {8},	array: {9} {10} {11} {12},	TypeName: {0}", itemType.Name, itemType.Hash, itemType.ParentType, itemType.I0, itemType.Flags, itemType.Type0, itemType.Type1, itemType.Type2, itemType.Type3, itemType.Array[0], itemType.Array[1], itemType.Array[2], itemType.Array[3]);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.MonsterAffixes)
				{
					foreach (var affixDefinition in data.MonsterAffixes)
					{
						Logger.Info("monster affix: {1},	{2}	{3}	{4}	{5}	{6}	A1: {7}	A2:{8}	Type: {9},	I4: {10},	I5: {11},	SNOPower: {12},	AffixName: {0}", affixDefinition.Name, affixDefinition.Hash, affixDefinition.I0, affixDefinition.I1, affixDefinition.I2, affixDefinition.I3, affixDefinition.Attributes[0].AttributeId, affixDefinition.Attributes[1].AttributeId, affixDefinition.MonsterAffix, affixDefinition.AffixType, affixDefinition.I4, affixDefinition.I5, affixDefinition.SNOOnSpawnPowerChampion);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.ExperienceTable)
				{
					Logger.Info("---------------------------------------------------------");
					Logger.Info("asset name: {0}", asset.Name);
					Logger.Info("SNOID: {0}", data.Header.SNOId);
					foreach (var row in data.Experience)
					{
						Logger.Info("Experience: {0}", row.Exp);
						Logger.Info("I201: {0}", row.I201);
						Logger.Info("Multiplier: {0}", row.Multiplier);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.ParagonBonuses)
				{
					Logger.Info("---------------------------------------------------------");
					Logger.Info("asset name: {0}", asset.Name);
					Logger.Info("SNOID: {0}", data.Header.SNOId);
					foreach (var row in data.ParagonBonuses)
					{
						Logger.Info("Name: {0}", row.Name);
						Logger.Info("Hash: {0}", row.Hash);
						Logger.Info("IconName: {0}", row.IconName);
						Logger.Info("I1: {0}", row.I1);
						Logger.Info("I2: {0}", row.I2);
						Logger.Info("Limit: {0}", row.Limit);
						Logger.Info("Category: {0}", row.Category);
						Logger.Info("Index: {0}", row.Index);
						Logger.Info("Class: {0}", row.Class);
						foreach(var attr in row.Attribute)
						{
							if (attr.AttributeId > 0)
								Logger.Info("Attr: {0},	{1}	- {2}", attr.AttributeId, attr.SNOParam, FormulaScript.ToString(attr.Formula.ToArray()));
						}
					}
				}*/
				/*if (data != null && data.Type == BalanceType.MonsterLevels)
				{
					Logger.Info("---------------------------------------------------------");
					Logger.Info("asset name: {0}", asset.Name);
					Logger.Info("SNOID: {0}", data.Header.SNOId);
					foreach (var row in data.MonsterLevel)
					{
						Logger.Info("I0: {0}", row.I0);
						Logger.Info("F0: {0}", row.F0);
						Logger.Info("F1: {0}", row.F1);
						Logger.Info("F2: {0}", row.F2);
						Logger.Info("F3: {0}", row.F3);
						Logger.Info("F4: {0}", row.F4);
						Logger.Info("F5: {0}", row.F5);
						Logger.Info("F6: {0}", row.F6);
						Logger.Info("F7: {0}", row.F7);
						Logger.Info("F8: {0}", row.F8);
						Logger.Info("F9: {0}", row.F9);
						Logger.Info("F10: {0}", row.F10);
						Logger.Info("F11: {0}", row.F11);
						Logger.Info("F12: {0}", row.F12);
						Logger.Info("F13: {0}", row.F13);
						Logger.Info("F14: {0}", row.F14);
						Logger.Info("F15: {0}", row.F15);
						Logger.Info("F16: {0}", row.F16);
						Logger.Info("F17: {0}", row.F17);
						Logger.Info("F18: {0}", row.F18);
						Logger.Info("F19: {0}", row.F19);
						Logger.Info("F20: {0}", row.F20);
						Logger.Info("F21: {0}", row.F21);
						Logger.Info("F22: {0}", row.F22);
						Logger.Info("F23: {0}", row.F23);
						Logger.Info("F24: {0}", row.F24);
						Logger.Info("F25: {0}", row.F25);
						Logger.Info("F26: {0}", row.F26);
						Logger.Info("F27: {0}", row.F27);
						Logger.Info("F28: {0}", row.F28);
						Logger.Info("F29: {0}", row.F29);
						Logger.Info("F30: {0}", row.F30);
						Logger.Info("F31: {0}", row.F31);
						Logger.Info("F32: {0}", row.F32);
						Logger.Info("F33: {0}", row.F33);
						Logger.Info("F34: {0}", row.F34);
						Logger.Info("F35: {0}", row.F35);
						Logger.Info("F36: {0}", row.F36);
						Logger.Info("F37: {0}", row.F37);
						Logger.Info("F38: {0}", row.F38);
						Logger.Info("F39: {0}", row.F39);
						Logger.Info("F40: {0}", row.F40);
						Logger.Info("F41: {0}", row.F41);
						Logger.Info("F42: {0}", row.F42);
						Logger.Info("F43: {0}", row.F43);
						Logger.Info("F44: {0}", row.F44);
						Logger.Info("F45: {0}", row.F45);
						Logger.Info("F46: {0}", row.F46);
						Logger.Info("F47: {0}", row.F47);
						Logger.Info("F48: {0}", row.F48);
						Logger.Info("F49: {0}", row.F49);
						Logger.Info("F50: {0}", row.F50);
						Logger.Info("F51: {0}", row.F51);
					}
				}*/
				if (data != null && data.Type == BalanceType.Heros)
				{
					Logger.Info("---------------------------------------------------------");
					Logger.Info("asset name: {0}", asset.Name);
					Logger.Info("SNOID: {0}", data.Header.SNOId);
					/*
					foreach (var row in data.Heros)
					{
						Logger.Info("Name: {0}", row.Name);
						Logger.Info("Hash: {0}", row.Hash);
						Logger.Info("I20: {0}", row.I20);
						Logger.Info("I21: {0}", row.I21);
						Logger.Info("SNOMaleActor: {0}", row.SNOMaleActor);
						Logger.Info("SNOFemaleActor: {0}", row.SNOFemaleActor);
						Logger.Info("SNOInventory: {0}", row.SNOInventory);
						Logger.Info("I0: {0}", row.I0);
						Logger.Info("SNOStartingLMBSkill: {0}", row.SNOStartingLMBSkill);
						Logger.Info("SNOStartingRMBSkill: {0}", row.SNOStartingRMBSkill);
						Logger.Info("SNOSKillKit0: {0}", row.SNOSKillKit0);
						Logger.Info("SNOSKillKit1: {0}", row.SNOSKillKit1);
						Logger.Info("SNOSKillKit2: {0}", row.SNOSKillKit2);
						Logger.Info("SNOSKillKit3: {0}", row.SNOSKillKit3);
						Logger.Info("PrimaryResource: {0}", row.PrimaryResource);
						Logger.Info("SecondaryResource: {0}", row.SecondaryResource);
						Logger.Info("CoreAttribute: {0}", row.CoreAttribute);
						Logger.Info("F0: {0}", row.F0);
						Logger.Info("I1: {0}", row.I1);
						Logger.Info("HitpointsMax: {0}", row.HitpointsMax);
						Logger.Info("HitpointsFactorLevel: {0}", row.HitpointsFactorLevel);
						Logger.Info("F3: {0}", row.F3);
						Logger.Info("PrimaryResourceMax: {0}", row.PrimaryResourceMax);
						Logger.Info("PrimaryResourceFactorLevel: {0}", row.PrimaryResourceFactorLevel);
						Logger.Info("PrimaryResourceRegenPerSecond: {0}", row.PrimaryResourceRegenPerSecond);
						Logger.Info("SecondaryResourceMax: {0}", row.SecondaryResourceMax);
						Logger.Info("SecondaryResourceFactorLevel: {0}", row.SecondaryResourceFactorLevel);
						Logger.Info("SecondaryResourceRegenPerSecond: {0}", row.SecondaryResourceRegenPerSecond);
						Logger.Info("F10: {0}", row.F10);
						Logger.Info("F201: {0}", row.F201);
						Logger.Info("F11: {0}", row.F11);
						Logger.Info("CritPercentCap: {0}", row.CritPercentCap);
						Logger.Info("F13: {0}", row.F13);
						Logger.Info("F14: {0}", row.F14);
						Logger.Info("WalkingRate: {0}", row.WalkingRate);
						Logger.Info("RunningRate: {0}", row.RunningRate);
						Logger.Info("F17: {0}", row.F17);
						Logger.Info("F18: {0}", row.F18);
						Logger.Info("F19: {0}", row.F19);
						Logger.Info("F20: {0}", row.F20);
						Logger.Info("F21: {0}", row.F21);
						Logger.Info("F22: {0}", row.F22);
						Logger.Info("F23: {0}", row.F23);
						Logger.Info("F24: {0}", row.F24);
						Logger.Info("F25: {0}", row.F25);
						Logger.Info("F26: {0}", row.F26);
						Logger.Info("F27: {0}", row.F27);
						Logger.Info("F28: {0}", row.F28);
						Logger.Info("F29: {0}", row.F29);
						Logger.Info("F30: {0}", row.F30);
						Logger.Info("F31: {0}", row.F31);
						Logger.Info("F32: {0}", row.F32);
						Logger.Info("F33: {0}", row.F33);
						Logger.Info("F34: {0}", row.F34);
						Logger.Info("Strength: {0}", row.Strength);
						Logger.Info("Dexterity: {0}", row.Dexterity);
						Logger.Info("Vitality: {0}", row.Vitality);
						Logger.Info("Intelligence: {0}", row.Intelligence);
						Logger.Info("GetHitMaxBase: {0}", row.GetHitMaxBase);
						Logger.Info("GetHitMaxPerLevel: {0}", row.GetHitMaxPerLevel);
						Logger.Info("GetHitRecoveryBase: {0}", row.GetHitRecoveryBase);
						Logger.Info("GetHitRecoveryPerLevel: {0}", row.GetHitRecoveryPerLevel);
						Logger.Info("F35: {0}", row.F35);
						Logger.Info("F201: {0}", row.F201);
						Logger.Info("I203: {0}", row.I203);
					}
					//*/
				}
				/*if (data != null && data.Type == BalanceType.MonsterNames)
				{
					foreach (var affixDefinition in data.RareMonsterNames)
					{
						Logger.Info("affix: {1},	{2}	{3},	AffixName: {0}", affixDefinition.Name, affixDefinition.Hash, affixDefinition.AffixType, affixDefinition.S0);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.HandicapLevels)
				{
					foreach (var diffDefinition in data.HandicapLevels)
					{
						Logger.Info("Difficulty:	{1},	{2},	{3},	{4},	{5},	{6},	{7},	{8}, Name: {0}", 
							diffDefinition.Name,
							diffDefinition.I0,
							diffDefinition.I1,
							diffDefinition.HPMod,
							diffDefinition.DmgMod,
							diffDefinition.F2,
							diffDefinition.XPMod,
							diffDefinition.GoldMod,
							diffDefinition.F5 );
					}
				}*/
				/*if (data != null && data.Type == BalanceType.RareItemNames)
				{
					foreach (var affixDefinition in data.RareItemNames)
					{
						Logger.Info("RareItemName: {1},	{2}	{3},	AffixName: {0}", affixDefinition.Name, affixDefinition.Hash, affixDefinition.Type, affixDefinition.AffixType);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.SetItemBonuses)
				{
					foreach (var affixDefinition in data.SetItemBonus)
					{
						Logger.Info("SetItemBonus: {1},	{2}	{3},	SetItemBonusName: {0}", affixDefinition.Name, affixDefinition.Count, affixDefinition.Set, affixDefinition.Attribute[0].AttributeId);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.Heros)
				{
					foreach (var affixDefinition in data.Heros)
					{
						Logger.Info("Heros: {1},	{2}	{3},	HeroName: {0}", affixDefinition.Name, affixDefinition.SNOMaleActor, affixDefinition.I0, affixDefinition.F0);
					}
				}*/
				/*if (data != null && data.Type == BalanceType.ItemEnhancements)
				{
					foreach (var enchantDefinition in data.ItemEnhancement)
					{
						Logger.Info("Enchant: {1},	{2}	{3}	{4}	{5}	{6}	ItemEnhancement: {0}", enchantDefinition.Name, enchantDefinition.Hash, enchantDefinition.I0, enchantDefinition.I1, enchantDefinition.I2, enchantDefinition.I3, enchantDefinition.Attribute[0].AttributeId);
					}
				}*/
			}
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Lore].Values)
			{
				Mooege.Common.MPQ.FileFormats.Lore data = asset.Data as Mooege.Common.MPQ.FileFormats.Lore;
				Logger.Info("Lore: snoid {1},	Type: {2},	I0 {3},	I1 {4}	I2 {5}	I3 {6}, ConvId {7},	SNOName: {0}", asset.Name, data.Header.SNOId, data.Category, data.I0, data.I1, data.I2, data.I3, data.SNOConversation);
			}*/
			/*foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Condition].Values)
			{
				Mooege.Common.MPQ.FileFormats.Condition data = asset.Data as Mooege.Common.MPQ.FileFormats.Condition;
				Logger.Info("Condition: snoid {1},	I0 {2},	I1 {3},	LoreCond1 {4}-{5},	LoreCond2 {6}-{7},	LoreCond3 {8}-{9},	SNOName: {0}", asset.Name, data.Header.SNOId, data.I0, data.I1, data.LoreCondition[0].SNOLore, data.LoreCondition[0].I0, data.LoreCondition[1].SNOLore, data.LoreCondition[1].I0, data.LoreCondition[2].SNOLore, data.LoreCondition[2].I0);
			}*/
		}

		private static void LoadConversations()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Conversation].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Conversation data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Conversation;
				/*Logger.Info("Conversation: [{8}]{0} I4: {1},	Type {9}, NPC {10}	Act {2}, SNOConvUnlocks {3}, {4}, {5}, I5: {6}, I6: {7}", 
					asset.Name, 
					data.I4, 
					data.I201, 
					data.SNOConvUnlocks[0], 
					data.SNOConvUnlocks[1], 
					data.SNOConvUnlocks[2], 
					data.I5, 
					data.I6, 
					data.Header.SNOId, 
					data.ConversationType, 
					data.SNOPrimaryNpc);*/
			}
		}

		private static void SetAllowedTypes()
		{
			foreach (int hash in ItemGroup.SubTypesToHashList("Weapon"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("Armor"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("Offhand"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("Jewelry"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("Utility"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("CraftingPlan"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in ItemGroup.SubTypesToHashList("Jewel"))
				AllowedItemTypes.Add(hash);
			foreach (int hash in TypeHandlers.Keys)
			{
				if (AllowedItemTypes.Contains(hash))
				{
					// already added structure
					continue;
				}
				foreach (int subhash in ItemGroup.SubTypesToHashList(ItemGroup.FromHash(hash).Name))
				{
					if (AllowedItemTypes.Contains(subhash))
					{
						// already added structure
						continue;
					}
					AllowedItemTypes.Add(subhash);
				}
			}

		}

		#endregion
		#region generating items
		// generates a random item.
		public static Item GenerateRandom(ActorSystem.Actor owner)
		{
			var itemDefinition = GetRandom(Items.Values
				.Where(def => def.ItemLevel == owner.Attributes[GameAttribute.Level]).ToList());
			return CreateItem(owner, itemDefinition);
		}

		public static Item GenerateLegOrSetRandom(ActorSystem.Actor owner)
		{
			var itemDefinition = GetLegOrSetRandom(AllowedUniqueItems.Values
				.Where(def => def.ItemLevel == owner.Attributes[GameAttribute.Level]).ToList());
			return CreateItem(owner, itemDefinition);
		}

		public static List<int> ObsoleteItems = new List<int>();

		// generates a random equip item (for vendors)
		public static Item GenerateRandomEquip(ActorSystem.Actor owner, int level, int minQuality = 1, int maxQuality = -1, ItemTypeTable type = null, ToonClass owner_class = ToonClass.Unknown, bool crafted = false)
		{
			if (level < 0) level = owner.Attributes[GameAttribute.Level];
			int quality = minQuality;
			//if (quality > 7)
			//	quality -= 5;
			if (maxQuality > -1)
				quality = RandomHelper.Next(minQuality, maxQuality);

			if (quality > 8)        //Unique items level scaling
			{
				var legaDefinition = GetRandom(AllowedItems.Values
					.Where(def =>
						def.ItemLevel <= Math.Min(level + 2, 73)
						&& !ObsoleteItems.Contains(def.Hash)
						&& UniqueItems.UniqueItemStats.ContainsKey(def.Hash)
						&& def.Quality != ItemTable.ItemQuality.Special
						&& (type == null ? true : ItemGroup.HierarchyToHashList(ItemGroup.FromHash(def.ItemTypesGBID)).Contains(type.Hash))
						&& (quality > 2 ? true : !ItemGroup.HierarchyToHashList(ItemGroup.FromHash(def.ItemTypesGBID)).Contains(-740765630)) //not jewelry
						&& (owner_class == ToonClass.Unknown ? true :
								(owner_class == ToonClass.Barbarian ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Barbarian) :
								(owner_class == ToonClass.Crusader ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Crusader) :
								(owner_class == ToonClass.Monk ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Monk) :
								(owner_class == ToonClass.Necromancer ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Necromancer) :
								(owner_class == ToonClass.Wizard ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Wizard) :
								(owner_class == ToonClass.DemonHunter ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.DemonHunter) :
								(owner_class == ToonClass.WitchDoctor ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.WitchDoctor) : true)))))))
							)
						).ToList()
					, (quality > 8));

				legaDefinition.ItemLevel = level;
				legaDefinition.RequiredLevel = level;
				legaDefinition.CrafterRequiredLevel = level;
				for (int i = 0; i < 6; i++)
					legaDefinition.MaxAffixLevel[i] = level;
				return CreateItem(owner, legaDefinition, quality, crafted);
			}

			var itemDefinition = GetRandom(AllowedItems.Values
				.Where(def =>
						//def.ItemLevel == owner.World.Game.InitialLevel //owner.Attributes[GameAttribute.Level]
						def.ItemLevel >= Math.Max(Math.Min(level - 3, 60), 1)
						&& def.ItemLevel <= Math.Min(level + 3, 73)

						&& !ObsoleteItems.Contains(def.Hash) //obsolete 1.0.3 items
						&& (type == null ? true : ItemGroup.HierarchyToHashList(ItemGroup.FromHash(def.ItemTypesGBID)).Contains(type.Hash))
						&& (quality > 2 ? true : !ItemGroup.HierarchyToHashList(ItemGroup.FromHash(def.ItemTypesGBID)).Contains(-740765630)) //not jewelry
						&& (owner_class == ToonClass.Unknown ? true :
								(owner_class == ToonClass.Barbarian ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Barbarian) :
								owner_class == ToonClass.Crusader ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Crusader) :
								(owner_class == ToonClass.Monk ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Monk) :
								(owner_class == ToonClass.Wizard ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Wizard) :
								(owner_class == ToonClass.DemonHunter ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.DemonHunter) :
								(owner_class == ToonClass.Necromancer ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.Necromancer) :
								(owner_class == ToonClass.WitchDoctor ? ItemGroup.FromHash(def.ItemTypesGBID).Usable.HasFlag(ItemFlags.WitchDoctor) :
								true
								)))))))
						).ToList()
					, false//(quality > 8)
					);

			return CreateItem(owner, itemDefinition, quality, crafted);
		}

		// generates a random dye (for vendors)
		public static Item GenerateRandomDye(ActorSystem.Actor owner)
		{
			var itemDefinition = GetRandom(Items.Values
				.Where(def =>
						def.ItemTypesGBID == StringHashHelper.HashItemName("Dye") &&
						!def.Name.Contains("CE")
						).ToList()
					);
			return CreateItem(owner, itemDefinition);
		}

		// generates a random potion (for innkeepers)
		public static Item GenerateRandomPotion(ActorSystem.Actor owner)
		{
			return Cook((owner as Player), "HealthPotionBottomless");
		}

		public static Item GenerateRandomCraftItem(ActorSystem.Actor player, int level, bool dropRecipe = false)
		{
			if (level < 0) level = player.Attributes[GameAttribute.Level];
			ItemTable itemDefinition = null;
			if (dropRecipe && FastRandom.Instance.Next(100) < 2)
				itemDefinition = GetRandom(Items.Values
					.Where(def =>
							def.ItemLevel <= (level + 3) &&
							!def.Name.Contains("StaffofCow") &&
							def.Name.Contains("CraftingPlan") &&
							!def.Name.Contains("CraftingPlan_Jeweler")
							).ToList()
						);

			if (itemDefinition == null) return null;
			return CreateItem(player, itemDefinition);
		}

		public static List<string> GemNames = new List<string>()
		{
			"x1_Topaz",
			"x1_Ruby",
			"x1_Amethyst",
			"x1_Diamond",
			"x1_Emerald"
		};

		public static Item GenerateRandomUniqueGem(ActorSystem.Actor player)
		{
			string baseN = "Unique_";
			string gemName = "Gem";

			int gem_grade = RandomHelper.Next(1,23);
			gemName += string.Format("_0{0:00}", gem_grade) + "_x1";
			return Cook((player as Player), baseN + gemName);
		}

		public static Item GenerateRandomGem(ActorSystem.Actor player, int level, bool is_goblin)
		{
			string gemName = GemNames[FastRandom.Instance.Next(GemNames.Count)];

			int lvl = Math.Max(player.Attributes[GameAttribute.Level], 20);
			int gem_grade = ((lvl - 10) / 8) + 1;
			if (is_goblin) gem_grade += 2;
			gemName += string.Format("_{0:00}", gem_grade);

			return Cook((player as Player), gemName);
		}

		// generates a random item from given type category.
		// we can also set a difficulty mode parameter here, but it seems current db doesnt have nightmare or hell-mode items with valid snoId's /raist.
		public static Item GenerateRandom(ActorSystem.Actor player, ItemTypeTable type)
		{
			var itemDefinition = GetRandom(Items.Values
				.Where(def => ItemGroup.HierarchyToHashList(ItemGroup.FromHash(def.ItemTypesGBID)).Contains(type.Hash)).ToList());
			return CreateItem(player, itemDefinition);
		}

		private static ItemTable GetRandom(List<ItemTable> pool, bool isUnique = false)
		{
			//var found = false;
			//ItemTable itemDefinition = null;

			if (pool.Count() == 0) return null;
			List<ItemTable> pool_filtered = pool.Where(it =>
				it.SNOActor != -1 &&
				it.WeaponDamageMin != 100.0f &&
				!it.Name.ToLower().Contains("lootrun") &&  //TieredLootrunKey
				!it.Name.ToLower().Contains("gold") &&
				!it.Name.ToLower().Contains("Retro") &&
				!it.Name.ToLower().Contains("healthglobe") &&
				!it.Name.ToLower().Contains("consumable") &&
				!it.Name.ToLower().Contains("pvp") &&
				!it.Name.ToLower().Contains("test") &&
				!it.Name.ToLower().Contains("cosmetic") &&
				!it.Name.ToLower().Contains("transmog") &&
				!it.Name.ToLower().Contains("reagent") &&
				!it.Name.ToLower().ToLower().Contains("retro") &&
				!it.Name.ToLower().Contains("pet") &&
				!it.Name.ToLower().Contains("set") &&
				!it.Name.ToLower().Contains("pvp") &&
				(isUnique ? it.Name.ToLower().Contains("unique") : !it.Name.ToLower().Contains("unique")) &&
				!it.Name.ToLower().Contains("crafted") &&
				!it.Name.ToLower().StartsWith("p71_ethereal") &&
				!it.Name.ToLower().Contains("debug") &&
				!it.Name.ToLower().Contains("promo") &&
				!it.Name.ToLower().Contains("powerpotion") &&
				!((it.ItemTypesGBID == StringHashHelper.HashItemName("Book")) && (it.Cost == 0)) && // i hope it catches all lore with npc spawned /xsochor
				!(!GBIDHandlers.ContainsKey(it.Hash) && !AllowedItemTypes.Contains(it.ItemTypesGBID))
			).ToList();
			/*
			List<ItemTable> pool_filtered = pool.Where(it =>
				it.SNOActor != -1 &&
				it.WeaponDamageMin != 100.0f &&
				!it.Name.ToLower().Contains("lootrun") &&  //TieredLootrunKey
				!it.Name.ToLower().Contains("gold") &&
				!it.Name.ToLower().Contains("healthglobe") &&
				!it.Name.ToLower().Contains("consumable") &&
				!it.Name.ToLower().Contains("pvp") &&
				!it.Name.ToLower().Contains("test") &&
				!it.Name.ToLower().Contains("cosmetic") &&
				!it.Name.ToLower().Contains("pet") &&
				!it.Name.ToLower().Contains("set") &&
				!it.Name.ToLower().Contains("norm") &&
				!it.Name.ToLower().Contains("unique") &&
				//(!isUnique) &&
				!it.Name.ToLower().Contains("crafted") &&
				!it.Name.ToLower().Contains("debug") &&
				!it.Name.ToLower().Contains("promo") &&
				!it.Name.ToLower().Contains("powerpotion") &&
				!((it.ItemTypesGBID == StringHashHelper.HashItemName("Book")) && (it.Cost == 0)) && // i hope it catches all lore with npc spawned /xsochor
				!(//!GBIDHandlers.ContainsKey(it.Hash) &&
				!AllowedItemTypes.Contains(it.ItemTypesGBID))
				).ToList();
			//*/

			if (pool_filtered.Count() == 0) return null;


			ItemTable selected = pool_filtered[FastRandom.Instance.Next(0, pool_filtered.Count() - 1)];
			return selected;
		}
		private static ItemTable GetLegOrSetRandom(List<ItemTable> pool, bool isUnique = false)
		{
			//var found = false;
			//ItemTable itemDefinition = null;

			if (pool.Count() == 0) return null;
			List<ItemTable> pool_filtered = pool.Where(it =>
				it.SNOActor != -1 &&
				it.WeaponDamageMin != 100.0f &&
				!it.Name.ToLower().Contains("lootrun") &&  //TieredLootrunKey
				!it.Name.ToLower().Contains("gold") &&
				!it.Name.ToLower().Contains("Retro") &&
				!it.Name.ToLower().Contains("healthglobe") &&
				!it.Name.ToLower().Contains("consumable") &&
				!it.Name.ToLower().Contains("pvp") &&
				!it.Name.ToLower().Contains("test") &&
				!it.Name.ToLower().Contains("cosmetic") &&
				!it.Name.ToLower().Contains("transmog") &&
				!it.Name.ToLower().Contains("reagent") &&
				!it.Name.ToLower().ToLower().Contains("retro") &&
				!it.Name.ToLower().Contains("pet") &&
				!it.Name.ToLower().Contains("set") &&
				!it.Name.ToLower().Contains("pvp") &&
				(it.Name.ToLower().Contains("unique") || it.Name.ToLower().Contains("set")) &&
				!it.Name.ToLower().Contains("crafted") &&
				!it.Name.ToLower().StartsWith("p71_ethereal") &&
				!it.Name.ToLower().Contains("debug") &&
				!it.Name.ToLower().Contains("promo") &&
				!it.Name.ToLower().Contains("powerpotion") &&
				!((it.ItemTypesGBID == StringHashHelper.HashItemName("Book")) && (it.Cost == 0)) && // i hope it catches all lore with npc spawned /xsochor
				!(!GBIDHandlers.ContainsKey(it.Hash) && !AllowedItemTypes.Contains(it.ItemTypesGBID))
			).ToList();
			if (pool_filtered.Count() == 0) return null;

			ItemTable selected = pool_filtered[FastRandom.Instance.Next(0, pool_filtered.Count() - 1)];
			return selected;
		}
		#endregion
		#region misc
		public static Type GetItemClass(ItemTable definition)
		{
			Type type = typeof(Item);

			if (GBIDHandlers.ContainsKey(definition.Hash))
			{
				type = GBIDHandlers[definition.Hash];
			}
			else
			{
				foreach (var hash in ItemGroup.HierarchyToHashList(ItemGroup.FromHash(definition.ItemTypesGBID)))
				{
					if (TypeHandlers.ContainsKey(hash))
					{
						type = TypeHandlers[hash];
						break;
					}
				}
			}

			return type;
		}

		public static int GetItemHash(string name)
		{
			var item = Items.Where(i => i.Value.Name == name).FirstOrDefault();
			return (item.Value == null ? -1 : item.Key);
		}

		public static Item CloneItem(Item originalItem)
		{
			Item clonedItem = CreateItem(originalItem.Owner, originalItem.ItemDefinition, originalItem.Attributes[GameAttribute.Item_Quality_Level], originalItem.Attributes[GameAttribute.IsCrafted], originalItem.Attributes[GameAttribute.Seed]);
			//clonedItem.AffixList = originalItem.AffixList;
			//clonedItem.Attributes = originalItem.Attributes;

			AffixGenerator.CloneIntoItem(originalItem, clonedItem);
			clonedItem.Attributes[GameAttribute.ItemStackQuantityLo] = originalItem.Attributes[GameAttribute.ItemStackQuantityLo];
			clonedItem.RareItemName = originalItem.RareItemName;
			clonedItem.Unidentified = originalItem.Unidentified;
			return clonedItem;
		}

		public static Item GetRandomItemOfType(Player player, ItemTypeTable itemType)
		{
			int minQuality = 1;
			if (ItemGroup.HierarchyToHashList(itemType).Contains(-740765630)) //jewelry
				minQuality = 3;

			Item item = GenerateRandomEquip(player, player.Level, minQuality, 10, itemType);

			item.Unidentified = false;
			return item;
		}

		// Creates an item based on supplied definition.
		public static Item CreateItem(ActorSystem.Actor owner, ItemTable definition, int forceQuality = -1, bool crafted = false, int seed = -1)
		{
			// Logger.Trace("Creating item: {0} [sno:{1}, gbid {2}]", definition.Name, definition.SNOActor, StringHashHelper.HashItemName(definition.Name));

			if (definition == null) return null;

			Type type = GetItemClass(definition);

			var item = (Item)Activator.CreateInstance(type, new object[] { owner.World, definition, forceQuality, crafted, seed });
			if (forceQuality == 9)
				item.Attributes[GameAttribute.Item_Quality_Level] = 9;
			return item;
		}

		// Allows cooking a custom item.
		public static Item Cook(Player player, string name)
		{
			int hash = StringHashHelper.HashItemName(name);
			ItemTable definition = Items[hash];

			//Unique items level scaling
			if (definition.Name.ToLower().Contains("unique") ||
				definition.Quality == ItemTable.ItemQuality.Legendary ||
				definition.Quality == ItemTable.ItemQuality.Special ||
				definition.Quality == ItemTable.ItemQuality.Set)
			{
				definition.ItemLevel = player.Attributes[GameAttribute.Level];
				definition.RequiredLevel = player.Attributes[GameAttribute.Level];
				definition.CrafterRequiredLevel = player.Attributes[GameAttribute.Level];
				for (int i = 0; i < 6; i++)
					definition.MaxAffixLevel[i] = player.Attributes[GameAttribute.Level];
			}
			return CookFromDefinition(player.World, definition);
		}

		// Allows cooking a custom item.
		public static Item CookFromDefinition(World world, ItemTable definition, int forceQuality = -1, bool binded = false, bool crafted = false, int seed = -1)
		{
			Type type = GetItemClass(definition);

			var item = (Item)Activator.CreateInstance(type, new object[] { world, definition, forceQuality, crafted, seed });

			item.Attributes[GameAttribute.Item_Quality_Level] = Math.Min(item.Attributes[GameAttribute.Item_Quality_Level], 9);

			if (binded)
				item.Attributes[GameAttribute.Item_Binding_Level_Override] = 1;

			return item;
		}

		public static ItemTable GetItemDefinition(int gbid)
		{
			return (Items.ContainsKey(gbid)) ? Items[gbid] : null;
		}

		public static List<ParagonBonusesTable> GetParagonBonusTable(ToonClass toon_class)
		{
			Class gb_class = Class.None;
			if (toon_class == ToonClass.Barbarian) gb_class = Class.Barbarian;
			if (toon_class == ToonClass.Crusader) gb_class = Class.Crusader;
			if (toon_class == ToonClass.DemonHunter) gb_class = Class.DemonHunter;
			if (toon_class == ToonClass.Monk) gb_class = Class.Monk;
			if (toon_class == ToonClass.WitchDoctor) gb_class = Class.Witchdoctor;
			if (toon_class == ToonClass.Wizard) gb_class = Class.Wizard;
			if (toon_class == ToonClass.Necromancer) gb_class = Class.Necromancer;
			return ParagonBonuses.Where(b => b.HeroClass == gb_class || b.HeroClass == Class.None).ToList();
		}

		public static RecipeTable GetRecipeDefinition(int gbid)
		{
			return (Recipes.ContainsKey(gbid)) ? Recipes[gbid] : null;
		}

		public static RecipeTable GetRecipeDefinition(string name)
		{
			var recipe = Recipes.Where(r => r.Value.Name == name).FirstOrDefault();
			return (recipe.Value == null) ? null : recipe.Value;
		}

		public static SocketedEffectTable GetGemEffectDefinition(int gem_gbid, int item_type)
		{
			var effects = GemEffects.Where(ge => ge.Item == gem_gbid && ge.ItemType == item_type).ToList();
			return (effects.Count == 1) ? effects.First() : null;
		}

		public static SetItemBonusTable GetItemSetEffectDefinition(int set_gbid, int count)
		{
			var effects = ItemSetsEffects.Where(ge => ge.Set == set_gbid && ge.Count == count).ToList();
			return (effects.Count == 1) ? effects.First() : null;
		}

		public static Item CreateGold(Player player, int amount)
		{
			string item_name = "Gold1";
			if (amount > 10) item_name = "Gold2";
			if (amount > 100) item_name = "Gold3";
			if (amount > 500) item_name = "Gold4";
			if (amount > 1000) item_name = "Gold5";
			var item = Cook(player, item_name);
			item.Attributes[GameAttribute.Gold] = amount;

			return item;
		}

		public static Item CreateBloodShards(Player player, int amount)
		{
			var item = Cook(player, "HoradricRelic");
			item.Attributes[GameAttribute.ItemStackQuantityLo] = amount;

			return item;
		}

		public static Item CreateHealthGlobe(Player player, int amount)
		{
			if (amount > 10)
				amount = 10 + ((amount - 10) * 5);

			var item = Cook(player, "HealthGlobe" + amount);
			item.Attributes[GameAttribute.Health_Globe_Bonus_Health] = amount;

			return item;
		}

		public static Item CreateArcaneGlobe(Player player)
		{
			var item = Cook(player, "ArcaneGlobe");

			return item;
		}

		public static Item CreatePowerGlobe(Player player)
		{
			var item = Cook(player, "PowerGlobe_v2_x1_NoFlippy");

			return item;
		}

		public static Item CreateLore(Player player, int loreId)
		{

			var loreDefinition = Lore[loreId];
			if (loreDefinition == null) return null;
			return CookFromDefinition(player.World, loreDefinition);
		}

		public static bool IsValidItem(string name)
		{
			return Items.ContainsKey(StringHashHelper.HashItemName(name));
		}

		#endregion
		#region Database handling
		public static void SaveToDB(Item item)
		{
			//var timestart = DateTime.Now;

			var affixSer = SerializeAffixList(item.AffixList);
			var attributesSer = item.Attributes.Serialize();

			item.DBInventory.Affixes = affixSer;
			item.DBInventory.DyeType = item.Attributes[GameAttribute.DyeType];
			item.DBInventory.Count = item.Attributes[GameAttribute.ItemStackQuantityLo];
			item.DBInventory.Durability = item.Attributes[GameAttribute.Durability_Cur];
			item.DBInventory.Binding = item.Attributes[GameAttribute.Item_Binding_Level_Override];
			item.DBInventory.Rating = item.Rating;
			item.DBInventory.RareItemName = (item.RareItemName == null ? null : item.RareItemName.ToByteArray());
			item.DBInventory.Quality = item.Attributes[GameAttribute.Item_Quality_Level];
			item.DBInventory.Attributes = attributesSer;
			item.DBInventory.GbId = item.GBHandle.GBID;
			item.DBInventory.Version = 2;
			item.DBInventory.TransmogGBID = item.Attributes[GameAttribute.TransmogGBID];

			//Logger.Info("ItemFlags: {0}", (int)item.ItemType.Flags);

			//var timeTaken = DateTime.Now - timestart;
			//Logger.Debug("Save item instance #{0}, took {1} msec", item.DBInventory.Id, timeTaken.TotalMilliseconds);

		}

		public static Item LoadFromDB(Player owner, DBInventory instance)//  int dbID, int gbid, string attributesSer, string affixesSer)
		{
			var table = Items[instance.GbId];
			var itm = new Item(owner.World, table, DeSerializeAffixList(instance.Affixes), instance.Attributes, instance.Count);
			if (itm.Attributes[GameAttribute.Durability_Max] > 0)
				itm.Attributes[GameAttribute.Durability_Cur] = instance.Durability;
			itm.Attributes[GameAttribute.DyeType] = instance.DyeType;
			itm.Attributes[GameAttribute.TransmogGBID] = instance.TransmogGBID;
			itm.Unidentified = instance.Unidentified;
			itm.DBInventory = instance;
			if (instance.Version == 1)
				itm.Attributes[GameAttribute.IsCrafted] = true;

			if (!owner.World.DbItems.ContainsKey(owner.World))
				owner.World.DbItems.Add(owner.World, new List<Item>());
			if (!owner.World.DbItems[owner.World].Contains(itm))
				owner.World.DbItems[owner.World].Add(itm);

			owner.World.CachedItems[instance.Id] = itm;

			if (instance.FirstGem != -1) itm.Gems.Add(ItemGenerator.CookFromDefinition(owner.World, ItemGenerator.GetItemDefinition(instance.FirstGem), 1));
			if (instance.SecondGem != -1) itm.Gems.Add(ItemGenerator.CookFromDefinition(owner.World, ItemGenerator.GetItemDefinition(instance.SecondGem), 1));
			if (instance.ThirdGem != -1) itm.Gems.Add(ItemGenerator.CookFromDefinition(owner.World, ItemGenerator.GetItemDefinition(instance.ThirdGem), 1));

			for (int i = 0; i < itm.Gems.Count; i++)
			{
				itm.Gems[i].Owner = itm;
				itm.Gems[i].SetInventoryLocation(20, 0, i);
			}

			itm.Attributes[GameAttribute.Sockets_Filled] = itm.Gems.Count;

			if (instance.RareItemName != null) itm.RareItemName = D3.Items.RareItemName.ParseFrom(instance.RareItemName);

			return itm;
		}

		public static string SerializeAffixList(List<Affix> affixList)
		{
			var affixgbIdList = affixList.Select(af => af.AffixGbid);
			var affixSer = affixgbIdList.Aggregate(",", (current, affixId) => current + (affixId + ",")).Trim(new[] { ',' });
			return affixSer;
		}

		public static List<Affix> DeSerializeAffixList(string serializedAffixList)
		{
			var affixListStr = serializedAffixList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var affixList = affixListStr.Select(int.Parse).Select(affixId => new Affix(affixId)).ToList();
			return affixList;
		}

		public static int GetSeed(string attributesList)
		{
			if (attributesList == "")
			{
				return 0;
			}
			var pairs = attributesList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var pair in pairs)
			{
				try
				{
					var pairParts = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					if (pairParts.Length != 2)
					{
						continue;
					}

					var keyData = pairParts[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					var attributeId = int.Parse(keyData[0].Trim());

					if (attributeId != 394)
					{
						continue;
					}


					var values = pairParts[1].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					var valueI = int.Parse(values[0].Trim());

					return valueI;

				}
				catch
				{
					return 0;
				}
			}

			return 0;
		}

		public static bool IsCrafted(string attributesList)
		{
			if (attributesList == "")
			{
				return false;
			}
			var pairs = attributesList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var pair in pairs)
			{
				try
				{
					var pairParts = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					if (pairParts.Length != 2)
					{
						continue;
					}

					var keyData = pairParts[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					var attributeId = int.Parse(keyData[0].Trim());

					if (attributeId != 395)
					{
						continue;
					}

					var values = pairParts[1].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					var valueI = int.Parse(values[0].Trim());

					return (valueI == 1);

				}
				catch
				{
					return false;
				}
			}

			return false;
		}

		#endregion
	}
}
 