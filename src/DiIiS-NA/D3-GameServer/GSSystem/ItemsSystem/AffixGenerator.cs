using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	static class AffixGenerator
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		public static List<AffixTable> AllAffix = new List<AffixTable>();
		public static List<AffixTable> FullAffixList = new List<AffixTable>();

		public static List<AffixTable> AffixList = new List<AffixTable>();

		private static List<AffixTable> LegendaryAffixList = new List<AffixTable>();

		static AffixGenerator()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.AffixList)
				{
					foreach (var affixDef in data.Affixes)
					{
						FullAffixList.Add(affixDef);

						if (affixDef.I0 > 8)
							affixDef.I0 -= 8;
						if (affixDef.I0 > 4)
							affixDef.I0 -= 4; //hacks for RoS affixes

						if (affixDef.AffixFamily0 == -1)
							continue;
						if (affixDef.Name.Contains("REQ"))
						{ continue; }// crashes the client // dark0ne

						if (!(affixDef.Name.StartsWith("1xx_") || affixDef.Name.StartsWith("X1_") ||
							affixDef.Name.StartsWith("CriticalD")))
							continue; //skip obsolete affixes except the old Critical Damage

						if (affixDef.Name.Contains("DamConversionHeal")) continue; // not in game
						if (affixDef.Name.Contains("CrushingBlow")) continue; // not in game
						if (affixDef.Name.Contains("LifeS")) continue; // not in game
						if (affixDef.Name.Contains("CooldownReduction")) continue; // not in game
						if (affixDef.Name.Contains("SplashDamage")) continue; // not in game
						if (affixDef.Name.Contains("1xx_Inferior")) continue;
						//!a.Name.Contains("")

						if (affixDef.AttributeSpecifier[0].AttributeId == 50) continue; // Armor_Item_Percent not in game

						if (affixDef.Name.Contains("CriticalD") && (affixDef.AffixLevel == 63)) //add level 70 weapon chd
							affixDef.AffixLevelMax = 73; //same for 1H,2H and secondary affix

						if (affixDef.Name.Contains("Run") && (affixDef.AffixLevel == 71))   //add level 70 movement speed
							affixDef.AffixLevelMax = 73;

						if (affixDef.Name.Contains("Sockets") &&
							(affixDef.AffixLevel == 53 || affixDef.AffixLevel == 60)) //add level 70 sockets
							affixDef.AffixLevelMax = 73;

						if (affixDef.Name.Contains("Sockets Chest")) // hack for chest socket
							affixDef.I0 = 1;

						if (affixDef.Name.Contains("Sockets Helm V")) // hack for weapon socket
						{
							affixDef.ItemGroup[1] = 485534122;
						}

						if (affixDef.Name.Contains("Sockets Bracer")) // hack for legs socket
						{
							affixDef.ItemGroup[0] = 3994699;
						}

						if (affixDef.Name.Contains("Sockets Shield")) // hack for shield socket
						{
							affixDef.ItemGroup[0] = 1440677334;
						}

						if (affixDef.Name.Contains("Kings")) // hack for ApS
							affixDef.I0 = 2;

						AllAffix.Add(affixDef);

						if (affixDef.I0 == 2 || affixDef.I0 == 3)
							LegendaryAffixList.Add(affixDef);

						if (affixDef.AffixLevel > (74) && !affixDef.Name.EndsWith("_Jewel")) continue;

						if (affixDef.I0 == 1 || affixDef.I0 == 3)
							AffixList.Add(affixDef);
					}

				}
			}
		}

		public static void Generate(Item item, int affixesCount, bool isCrafting = false)
		{
			if (!Item.IsWeapon(item.ItemType) &&
				!Item.IsArmor(item.ItemType) &&
				!Item.IsOffhand(item.ItemType) &&
				!Item.IsShard(item.ItemType) &&
				!Item.IsAccessory(item.ItemType))
				return;

			if (Item.IsAccessory(item.ItemType) && affixesCount <= 1) affixesCount = 2;

			bool IsUnique = item.ItemDefinition.Name.Contains("Unique_");

			
			if (IsUnique && !isCrafting) affixesCount = item.ItemDefinition.BonusAffixes + item.ItemDefinition.BonusMajorAffixes + item.ItemDefinition.BonusMinorAffixes;

			if (item.ItemDefinition.Name.ToLower().Contains("p71_ethereal"))
			{
				affixesCount = 8;
				IsUnique = true; 
			}


			if (item.GBHandle.GBID == -4139386) affixesCount = 6; //referral ring
			if (IsUnique)
				affixesCount += 3;

			Class ItemPlayerClass = Class.None;
			if (item.ItemType.Usable.HasFlag(ItemFlags.Barbarian)) ItemPlayerClass = Class.Barbarian;
			if (item.ItemType.Usable.HasFlag(ItemFlags.Crusader)) ItemPlayerClass = Class.Crusader;
			if (item.ItemType.Usable.HasFlag(ItemFlags.Necromancer)) ItemPlayerClass = Class.Necromancer;
			if (item.ItemType.Usable.HasFlag(ItemFlags.DemonHunter)) ItemPlayerClass = Class.DemonHunter;
			if (item.ItemType.Usable.HasFlag(ItemFlags.Wizard)) ItemPlayerClass = Class.Wizard;
			if (item.ItemType.Usable.HasFlag(ItemFlags.WitchDoctor)) ItemPlayerClass = Class.Witchdoctor;
			if (item.ItemType.Usable.HasFlag(ItemFlags.Monk)) ItemPlayerClass = Class.Monk;


			List<int> itemTypes = ItemGroup.HierarchyToHashList(item.ItemType);
			//itemGroups.Add(ItemGroup.GetRootType(item.ItemType));
			int levelToFind = (item.ItemLevel < 5 ? 0 : Math.Min(item.ItemLevel, 70));
			if (item.GBHandle.GBID == -4139386) levelToFind = 10;
			if (itemTypes[0] == 828360981)
				itemTypes.Add(-1028103400);
			else if (itemTypes[0] == -947867741)
				itemTypes.Add(3851110);
			else if (itemTypes[0] == 110504)
				itemTypes.Add(395678127);
			else if (itemTypes[0] == 327487312)
			{ itemTypes.Add(140775496); itemTypes.Add(133016072); itemTypes.Add(485534122); }

			if (item.ItemDefinition.Name.ToLower().Contains("p71_ethereal"))
			{
				itemTypes.Add(133016072);
				itemTypes.Add(133016072);
			}
			IEnumerable<AffixTable> filteredList = null;

			filteredList = AllAffix.Where(a =>
				(a.PlayerClass == ItemPlayerClass || a.PlayerClass == Class.None) &&//(a.PlayerClass == ItemPlayerClass || a.PlayerClass == Class.None) &&
				itemTypes.ContainsAtLeastOne(a.ItemGroup) &&
				(a.AffixLevelMax >= levelToFind) &&
				(a.OverrideLevelReq <= item.ItemDefinition.RequiredLevel)
				//!a.Name.Contains("1xx_Inferior")
				);

			if (IsUnique)
			{
				var restrictedFamily = item.ItemDefinition.LegendaryAffixFamily.Where(af => af != -1).ToHashSet();
				filteredList = filteredList
					.Where(a => !(restrictedFamily.Contains(a.AffixFamily0) || restrictedFamily.Contains(a.AffixFamily1)));

				if (restrictedFamily.Contains(1616088365) ||
				    restrictedFamily.Contains(-1461069734) ||
				    restrictedFamily.Contains(234326220) ||
				    restrictedFamily.Contains(1350281776) ||
				    restrictedFamily.Contains(-812845450) ||
				    restrictedFamily.Contains(1791554648) ||
				    restrictedFamily.Contains(125900958)) //MinMaxDam and ele damage
				{
					filteredList = filteredList
						.Where(a => !a.Name.Contains("FireD") &&
						            !a.Name.Contains("PoisonD") &&
						            !a.Name.Contains("HolyD") &&
						            !a.Name.Contains("ColdD") &&
						            !a.Name.Contains("LightningD") &&
						            !a.Name.Contains("ArcaneD") &&
						            !a.Name.Contains("MinMaxDam") &&
						            isCrafting ? !a.Name.ToLower().Contains("socket") : !a.Name.Contains("ASDHUIOPASDHIOU"));
				}
			}

			if (affixesCount <= 1)
				filteredList = filteredList.Where(
					a =>
					!a.Name.Contains("Secondary")
					);

			if (item.GBHandle.GBID == -4139386)
				filteredList = filteredList.Where(
					a =>
					!a.Name.Contains("Str") &&
					!a.Name.Contains("Dex") &&
					!a.Name.Contains("Int") &&
					!a.Name.Contains("Vit")
					);

			if (affixesCount <= 3)
				filteredList = filteredList.Where(
					a =>
					!a.Name.Contains("Experience") &&
					!a.Name.Contains("Archon")
					);



			Dictionary<int, AffixTable> bestDefinitions = new Dictionary<int, AffixTable>();

			foreach (var affix_group in filteredList.GroupBy(a => a.AffixFamily0))
			{
				if (item.AffixFamilies.Contains(affix_group.First().AffixFamily0)) continue;
				int s = item.ItemDefinition.RequiredLevel;

				bestDefinitions[affix_group.First().AffixFamily0] = affix_group.PickRandom();
			}

			//if (bestDefinitions.Values.Where(a => a.Name.Contains("PoisonD")).Count() > 0) Logger.Debug("PoisonD in bestDefinitions");
			List<AffixTable> selectedGroups = bestDefinitions.Values
				.OrderBy(x => FastRandom.Instance.Next()) //random order
				.GroupBy(x => (x.AffixFamily1 == -1) ? x.AffixFamily0 : x.AffixFamily1)
				.Select(x => x.First()) //only one from group
				.Take(affixesCount) //take needed amount
				.ToList();

			int n = 0;
			foreach (var def in selectedGroups)
			{
				if (def != null)
				{
					//Logger.Debug("Generating affix " + def.Name + " (aLvl:" + def.AffixLevel + ")");
					List<float> Scores = new List<float>();
					foreach (var effect in def.AttributeSpecifier)
					{
						//if (def.Name.Contains("Sockets")) Logger.Info("socket affix attribute: {0}, {1}", effect.AttributeId, effect.SNOParam);
						if (effect.AttributeId > 0)
						{
							float result;
							float minValue;
							float maxValue;
							if (FormulaScript.Evaluate(effect.Formula.ToArray(), item.RandomGenerator, out result, out minValue, out maxValue))
							{
								if (effect.AttributeId == 369) continue; //Durability_Max
								if (effect.AttributeId == 380) //Sockets
								{
									result = Math.Max(result, 1f);
									if (def.Name.Contains("Chest"))
										result = (float)FastRandom.Instance.Next(1, 4);
									if (def.Name.Contains("Bracer"))
										result = (float)FastRandom.Instance.Next(1, 3);
								}
								float score = (minValue == maxValue ? 0.5f : ((result - minValue) / (maxValue - minValue)));
								Scores.Add(score);
								//Logger.Debug("Randomized value for attribute " + GameAttribute.Attributes[effect.AttributeId].Name + "(" + minValue + "," + maxValue + ")" + " is " + result + ", score is " + score);
								//var tmpAttr = GameAttribute.Attributes[effect.AttributeId];
								//var attrName = tmpAttr.Name;
								//


								if (GameAttributes.Attributes[effect.AttributeId] is GameAttributeF)
								{
									
									//
									var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeF;
									if (effect.SNOParam != -1)
										item.Attributes[attr, effect.SNOParam] += result;
									else
										item.Attributes[attr] += result;
								}
								else if (GameAttributes.Attributes[effect.AttributeId] is GameAttributeI)
								{
									var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeI;
									if (effect.SNOParam != -1)
										item.Attributes[attr, effect.SNOParam] += (int)result;
									else
										item.Attributes[attr] += (int)result;
								}
								//Logger.Debug("{0} - Str: {1} ({2})", n, result, item.Attributes[GameAttribute.Attributes[effect.AttributeId] as GameAttributeF]);
							}
						}
					}
					var affix = new Affix(def.Hash);
					affix.Score = (Scores.Count == 0 ? 0 : Scores.Average());
					//Logger.Debug("Affix  " + def.Hash + ", final score is" + affix.Score);
					item.AffixList.Add(affix);

					if (affixesCount > 0 && !IsUnique && !item.ItemDefinition.Name.Contains("StaffOfCow"))
					{
						item.RareItemName = GenerateItemName();
					}
				}
				n++;
			}

			if (IsUnique)
			{
				for (int i = 0; i < 6; i++)
				{
					if (item.ItemDefinition.LegendaryAffixFamily[i] != -1)
						AddAffix(item, FindSuitableAffix(item, item.ItemDefinition.LegendaryAffixFamily[i], item.ItemDefinition.MaxAffixLevel[i], (item.ItemDefinition.I38[i] == 1)));
				}
			}
		}

		public static int FindSuitableAffix(Item item, int affixGroup, int affixLevel, bool extendedFilter)
		{
			if (affixGroup == -1) return -1;
			var allGroup = LegendaryAffixList
				.Where(a => (a.AffixFamily0 == affixGroup || a.AffixFamily1 == affixGroup) && (Item.Is2H(item.ItemType) ? !a.Name.EndsWith("1h") : (!a.Name.Contains("Two-Handed") && !a.Name.EndsWith("2h"))))
				.ToArray();

			if (!allGroup.Any()) return -1;

			bool secondGroup = allGroup.First().AffixFamily1 == affixGroup;

			var suitable = allGroup.Where(a => a.AffixLevel <= affixLevel || affixLevel <= 0).ToArray();

			if (!suitable.Any()) return -1;

			List<int> itemTypes = ItemGroup.HierarchyToHashList(item.ItemType);

			suitable = suitable.Where(a =>
				itemTypes.ContainsAtLeastOne(a.ItemGroup) ||
				(extendedFilter && itemTypes.ContainsAtLeastOne(a.LegendaryAllowedTypes))).ToArray();

			if (!suitable.Any()) return -1;

			int suitableAffixLevel = suitable.MaxBy(a => a.AffixLevel).AffixLevel;
			suitable = suitable.Where(a => a.AffixLevel == suitableAffixLevel).ToArray();

			//if (i18 && !secondGroup)
			//	suitable = suitable.Where(a => a.MaxLevel <= (Program.MaxLevel + 4));

			//if (!ignoreItemTypeFilter)
			//else
			//suitable = suitable.Where(a => itemTypes.ContainsAtLeastOne(a.I10));

			if (!suitable.Any())
				suitable = allGroup.Where(a => a.AffixLevel == 1).ToArray();

			/*int suitableMaxLevel = suitable.OrderByDescending(a => a.AffixLevel).First().MaxLevel;
			int suitableAffixLevel = suitable.OrderByDescending(a => a.AffixLevel).First().AffixLevel;
			
			if (suitableMaxLevel > (Program.MaxLevel + 4) && i18 && !secondGroup)
				suitable = all_group.Where(a => a.AffixLevel == 1);
			else
				suitable = all_group.Where(a => a.AffixLevel == suitableAffixLevel);
			
			if (suitable.Count() > 1 && i18 && !secondGroup && suitable.Where(a => a.Name.Contains("Secondary")).Count() > 0)
				suitable = suitable.Where(a => a.Name.Contains("Secondary"));*/

			if (suitable.TryPickRandom(out var randomAffix))
				return randomAffix.Hash;

			return -1;
		}

		public static void AddAffix(Item item, int affixGbId, bool findFromTotal = false)
		{
			if (affixGbId == -1) return;
			AffixTable definition = null;

			if (findFromTotal)
			{
				definition = AllAffix.FirstOrDefault(def => def.Hash == affixGbId);
				var testc = AllAffix.FirstOrDefault(def => def.ItemGroup[0] == affixGbId || def.ItemGroup[1] == affixGbId);
			}
			else
			{
				definition = AffixList.FirstOrDefault(def => def.Hash == affixGbId);

				if (definition == null)
				{
					definition = LegendaryAffixList.FirstOrDefault(def => def.Hash == affixGbId);

				}
			}

			if (definition == null)
			{
				Logger.Warn("Affix {0} was not found!", affixGbId);
				return;
			}

			item.AffixFamilies.Add(definition.AffixFamily0);
			//if (item.AffixList.Where(a => a.AffixGbid == AffixGbId).ToList().Count > 0) return;
			List<float> Scores = new List<float>();
			foreach (var effect in definition.AttributeSpecifier)
			{
				if (effect.AttributeId <= 0)
					continue;

				float result;
				float minValue;
				float maxValue;
				if (FormulaScript.Evaluate(effect.Formula.ToArray(), item.RandomGenerator, out result, out minValue, out maxValue))
				{
					var attribute = GameAttributes.Attributes[effect.AttributeId];

					if (effect.AttributeId == 369) continue; //Durability_Max
					if (effect.AttributeId == 380) //Sockets
					{
						result = Math.Max(result, 1f);
						if (definition.Name.Contains("Bracer") && item.GBHandle.GBID == 219884787) //Inna's Legs
							result = 2;
					}
					float score = (minValue == maxValue ? 0.5f : ((result - minValue) / (maxValue - minValue)));
					Scores.Add(score);
					if (attribute is GameAttributeF)
					{
						var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeF;
						if (effect.SNOParam != -1)
							item.Attributes[attr, effect.SNOParam] += result;
						else
							item.Attributes[attr] += result;
					}
					else if (GameAttributes.Attributes[effect.AttributeId] is GameAttributeI)
					{
						var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeI;
						if (effect.SNOParam != -1)
							item.Attributes[attr, effect.SNOParam] += (int)result;
						else
							item.Attributes[attr] += (int)result;
					}
					else if (GameAttributes.Attributes[effect.AttributeId] is GameAttributeB)
					{
						var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeB;
						if (result == 1)
							item.Attributes[attr] = true;
						else
							item.Attributes[attr] = false;
					}
				}
			}
			var affix = new Affix(affixGbId);
			affix.Score = (Scores.Count == 0 ? 0 : Scores.Average());
			item.AffixList.Add(affix);
			//item.Attributes[GameAttribute.Item_Quality_Level]++;
		}

		public static void CloneIntoItem(Item source, Item target)
		{
			target.AffixList.Clear();
			foreach (var affix in source.AffixList)
			{
				var newItemAffix = new Affix(affix.AffixGbid);
				target.AffixList.Add(newItemAffix);
			}
			foreach (var affix in target.AffixList)
			{
				var definition = AllAffix.Single(def => def.Hash == affix.AffixGbid);

				foreach (var effect in definition.AttributeSpecifier)
				{
					if (effect.AttributeId <= 0)
						continue;

					var attribute = GameAttributes.Attributes[effect.AttributeId];

					if (attribute.ScriptFunc != null && !attribute.ScriptedAndSettable)
						continue;

					if (attribute is GameAttributeF)
					{
						var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeF;
						if (effect.SNOParam != -1)
							target.Attributes[attr, effect.SNOParam] = source.Attributes[attr, effect.SNOParam];
						else
							target.Attributes[attr] = source.Attributes[attr];
					}
					else if (GameAttributes.Attributes[effect.AttributeId] is GameAttributeI)
					{
						var attr = GameAttributes.Attributes[effect.AttributeId] as GameAttributeI;
						if (effect.SNOParam != -1)
							target.Attributes[attr, effect.SNOParam] = source.Attributes[attr, effect.SNOParam];
						else
							target.Attributes[attr] = source.Attributes[attr];
					}

				}
			}
		}

		private static List<int> PrefixAffixLists = new List<int> { 214115, 1113787, 130726, 130727, 130761, 130748, 130729, 130730, 130731, 130735, 130734, 214211, 214213, 130737, 130749, 130739, 214091, 130742, 130744, 214093, 130745, 222658, 130740, 214136, 222661, 130746, 130733, 214215, 130752, 130753, 130755, 130756, 214120, 214102, 214088, 130758, 130759, 130760, 214189, 130762, 130763, 214108, 214106, 130764, 214202, 214206, 214209, 130765, 130766 };

		private static List<int> SuffixAffixLists = new List<int> { 214116, 213605, 213606, 213608, 213638, 213610, 213611, 213612, 213613, 213614, 213617, 214212, 214214, 213618, 213619, 213621, 214092, 213622, 213623, 214094, 213624, 222659, 213625, 214137, 222660, 213626, 213615, 214216, 213628, 213629, 213631, 213633, 214121, 214103, 214089, 213635, 213636, 213637, 214197, 213639, 213640, 214109, 214107, 213607, 214203, 214208, 214210, 213641, 213642 };

		public static D3.Items.RareItemName GenerateItemName()
		{
			bool itemIsPrefix = (FastRandom.Instance.Next(0, 2) > 0);
			var randomName = D3.Items.RareItemName.CreateBuilder()
				.SetItemNameIsPrefix(itemIsPrefix)
				.SetSnoAffixStringList(itemIsPrefix ? SuffixAffixLists.PickRandom() : PrefixAffixLists.PickRandom())
				.SetAffixStringListIndex(FastRandom.Instance.Next(0, 6))
				.SetItemStringListIndex(FastRandom.Instance.Next(0, 6));
			return randomName.Build();
		}
	}
}
