using DiIiS_NA.Core.Logging;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
using Actor = DiIiS_NA.GameServer.GSSystem.ActorSystem.Actor;
using Monster = DiIiS_NA.GameServer.GSSystem.ActorSystem.Monster;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	static class MonsterAffixGenerator
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		private static List<MonsterAffixesTable> AffixList = new List<MonsterAffixesTable>();

		private static List<MonsterNamesTable> NamesList = new List<MonsterNamesTable>();

		static MonsterAffixGenerator()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.MonsterAffixes)
				{
					foreach (var affixDef in data.MonsterAffixes)
					{
						//if ((int)affixDef.AffixType == -1) continue;
						if (affixDef.AffixType == AffixType.Inherit) continue;
						if (affixDef.AffixType == AffixType.Prefix) continue;
						if (affixDef.MonsterAffix == MonsterAffix.Shooters) continue;
						if (affixDef.MonsterAffix == MonsterAffix.Champions) continue;
						if (affixDef.Name.Contains("Shaman")) continue;
						if (affixDef.Name.Contains("Vampiric")) continue;
						if (affixDef.Name.Contains("ArcaneEnchanted")) continue;
						if (affixDef.Name.Contains("Illusionist")) continue;
						if (affixDef.Name.Contains("Molten")) continue;
						if (affixDef.Name.Contains("Mortar")) continue;

						AffixList.Add(affixDef);
					}
				}
				if (data != null && data.Type == BalanceType.MonsterNames)
				{
					foreach (var nameDef in data.RareMonsterNames)
					{
						if (nameDef.Name.Contains("Prefix004")) continue;
						if (nameDef.Name.Contains("Suffix004")) continue;
						if (nameDef.Name == "MultishotPrefix001") continue;
						if (nameDef.Name == "PowerfulPrefix001") continue;
						if (nameDef.Name == "PowerfulPrefix002") continue;
						if (nameDef.Name == "DamageAuraPrefix002") continue;
						if (nameDef.Name == "MultishotPrefix003") continue;
						if (nameDef.Name == "MultishotSuffix") continue;
						if (nameDef.Name == "DamageAuraPrefix003") continue;
						if (nameDef.Name.Contains("LifeLinkPrefix")) continue;
						if (nameDef.Name.Contains("StoneskinPrefix")) continue;
						if (nameDef.Name.Contains("ManaLeech")) continue;
						if (nameDef.Name.Contains("Shaman")) continue;
						if (nameDef.Name.Contains("BallistaSuffix")) continue;
						NamesList.Add(nameDef);
					}
				}
			}
		}

		public static List<Affix> Generate(Actor monster, int affixesCount)
		{
			List<MonsterAffixesTable> selectedGroups = AffixList.Where(a => (int)a.AffixType != -1).OrderBy(x => FastRandom.Instance.Next()).Take(affixesCount).ToList();

			if (monster is Champion)
				selectedGroups.Add(AffixList.Single(a => a.Name == "ChampionBase"));
			if (monster is Unique)
				selectedGroups.Add(AffixList.Single(a => a.Name == "Unique"));
			if (monster is Rare)
				selectedGroups.Add(AffixList.Single(a => a.Name == "Rare"));
			if (monster is Minion)
				selectedGroups.Add(AffixList.Single(a => a.Name == "Minion"));

			monster.AffixList.Clear();
			foreach (var def in selectedGroups)
			{
				if (def != null)
				{
					//Logger.Debug("Generating affix " + def.Name + " (aLvl:" + def.AffixLevel + ")");
					monster.AffixList.Add(new Affix(def.Hash));
					foreach (var effect in def.Attributes)
					{
						//if (def.Name.Contains("Sockets")) Logger.Info("socket affix attribute: {0}, {1}", effect.AttributeId, effect.SNOParam);
						if (effect.AttributeId > 0)
						{
							float result;
							if (FormulaScript.Evaluate(effect.Formula.ToArray(), new ItemRandomHelper(FastRandom.Instance.Next()), out result))
							{
								if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
									if (effect.SNOParam != -1)
										monster.Attributes[attr, effect.SNOParam] += result;
									else
										monster.Attributes[attr] += result;
								}
								else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
									if (effect.SNOParam != -1)
										monster.Attributes[attr, effect.SNOParam] += (int)result;
									else
										monster.Attributes[attr] += (int)result;
								}
							}
						}
					}
					switch (def.SNOOnSpawnPowerChampion)
					{
						case 90566: //Plagued
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(231115);
							break;
						case 90144: //Frozen
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(231149);
							break;
						case 155958: //Teleporter
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(155959);
							break;
						case 221131: //Desecrator
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(156105);
							break;
						case 226293: //Waller
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(226294);
							break;
						case 70650: //Extra Health
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70650);
							break;
						case 226437: //Shielding
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(226438);
							break;
						case 81420: //Electrified
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(81420);
							break;
						case 221132: //Vortex
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(120305);
							break;
						case 222745: //Jailer
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(222744);
							break;
						case 70849: //Fast
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70849);
							break;
						case 70655: //Knockback
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70655);
							break;
					}
				}
			}
			return monster.AffixList;
		}

		public static void CopyAffixes(Actor monster, List<Affix> affixList)
		{
			if (monster == null) return;
			monster.AffixList.Clear();
			foreach (var affix in affixList)
			{
				var def = AffixList.Where(d => d.Hash == affix.AffixGbid).FirstOrDefault();
				if (def != null)
				{
					//Logger.Debug("Generating affix " + def.Name + " (aLvl:" + def.AffixLevel + ")");
					monster.AffixList.Add(new Affix(def.Hash));
					foreach (var effect in def.Attributes)
					{
						//if (def.Name.Contains("Sockets")) Logger.Info("socket affix attribute: {0}, {1}", effect.AttributeId, effect.SNOParam);
						if (effect.AttributeId > 0)
						{
							float result;
							if (FormulaScript.Evaluate(effect.Formula.ToArray(), new ItemRandomHelper(FastRandom.Instance.Next()), out result))
							{
								//Logger.Debug("Randomized value for attribute " + GameAttribute.Attributes[effect.AttributeId].Name + " is " + result);
								//var tmpAttr = GameAttribute.Attributes[effect.AttributeId];
								//var attrName = tmpAttr.Name;

								if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
									if (effect.SNOParam != -1)
										monster.Attributes[attr, effect.SNOParam] += result;
									else
										monster.Attributes[attr] += result;
								}
								else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
									if (effect.SNOParam != -1)
										monster.Attributes[attr, effect.SNOParam] += (int)result;
									else
										monster.Attributes[attr] += (int)result;
								}
							}
						}
					}
					switch (def.SNOOnSpawnPowerChampion)
					{
						case 90566: //Plagued
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(231115);
							break;
						case 90144: //Frozen
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(231149);
							break;
						case 155958: //Teleporter
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(155959);
							break;
						case 221131: //Desecrator
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(156105);
							break;
						case 226293: //Waller
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(226294);
							break;
						case 70650: //Extra Health
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70650);
							break;
						case 226437: //Shielding
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(226438);
							break;
						case 81420: //Electrified
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(81420);
							break;
						case 221132: //Vortex
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(120305);
							break;
						case 222745: //Jailer
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(222744);
							break;
						case 70849: //Fast
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70849);
							break;
						case 70655: //Knockback
							((monster as Monster).Brain as MonsterBrain).AddPresetPower(70655);
							break;
					}
				}
			}
		}

		public static int GeneratePrefixName()
		{
			var randomPrefix = NamesList.Where(n => n.AffixType == AffixType.Prefix).OrderBy(x => RandomHelper.Next()).ToList().First();
			return randomPrefix.Hash;
		}

		public static int GenerateSuffixName()
		{
			var randomSuffix = NamesList.Where(n => n.AffixType == AffixType.Suffix).OrderBy(x => RandomHelper.Next()).ToList().First();
			return randomSuffix.Hash;
		}
	}
}
