﻿//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public class HitPayload : Payload
	{
		public static readonly Logger Logger = LogManager.CreateLogger();
		public float TotalDamage;
		public DamageType DominantDamageType;
		public Dictionary<DamageType, float> ElementDamages;
		public bool IsCriticalHit;
		public bool IsDodged;
		public bool IsWeaponDamage;

		public bool Successful = false;
		public bool Blocked = false;

		public bool AutomaticHitEffects = true;
		public Action<DeathPayload> OnDeath = null;

		private bool WaitTo(TickTimer timer)
		{
			while (timer.TimedOut != true)
			{

			}
			return true;
		}

		public HitPayload(AttackPayload attackPayload, bool criticalHit, Actor target)
			: base(attackPayload.Context, target)
		{
			IsCriticalHit = criticalHit;
			IsDodged = false;
			IsWeaponDamage = (attackPayload.DamageEntries.Count > 0 ? attackPayload.DamageEntries.First().IsWeaponBasedDamage : false);

			if (Context.User == null)
				Context.User = target;

			

			if (Target == null)
				Target = target;

			if (Target == null) return;

			if (Target.World == null) return;

			if (!Target.World.Game.Working) return;

			if (Target.World.Game.Paused) return;

			if (!Target.Visible) return;

			if (Target.Dead) return;

			if (Context.User is Monster && Context.Target is Player)
				if (!Context.User.IsRevealedToPlayer(Context.Target as Player))
					return;
			
			Successful = true;

			//float weaponMinDamage = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			//float weaponDamageDelta = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			// calculate and add up damage amount for each element type
			ElementDamages = new Dictionary<DamageType, float>();

			foreach (var entry in attackPayload.DamageEntries)
			{
				if (!ElementDamages.ContainsKey(entry.DamageType))
					ElementDamages[entry.DamageType] = 0f;

				switch (Context.User)
				{
					case Player:
						if (entry.IsWeaponBasedDamage)
						{
							ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier
								* (
									Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0)
									+
									((float)PowerContext.Rand.NextDouble() * (Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
								);
						}
						else
							ElementDamages[entry.DamageType] += entry.MinDamage + (float)PowerContext.Rand.NextDouble() * entry.DamageDelta;
						break;
					case Minion:
						var master = (Context.User as Minion).Master;
						var dmg_mul = (Context.User as Minion).DamageCoefficient;

						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * dmg_mul * (
							master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttribute.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0) +
							((float)PowerContext.Rand.NextDouble() * (master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
						);
						break;
					default:
						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * (Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((float)PowerContext.Rand.NextDouble() * Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0]));
						break;
				}
				
				ElementDamages[entry.DamageType] *= 1f + Context.User.Attributes[GameAttribute.Damage_Type_Percent_Bonus, (int)entry.DamageType.HitEffect] + Context.User.Attributes[GameAttribute.Damage_Dealt_Percent_Bonus, (int)entry.DamageType.HitEffect];

				if (Target.Attributes[GameAttribute.Immunity, (int)entry.DamageType.HitEffect] == true) ElementDamages[entry.DamageType] = 0f; //Immunity

				switch (Target)
				{
					case Player:
						ElementDamages[entry.DamageType] *= ReductionFromResistance(Target.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttribute.Level]);
						ElementDamages[entry.DamageType] *= 1f - Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + Target.Attributes[GameAttribute.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						if ((Target as Player).SkillSet.HasPassive(205491) && (int)entry.DamageType.HitEffect != 0)
							ElementDamages[entry.DamageType] *= 0.8f;
						if((Target as Player).SkillSet.HasSkill(462239))
							foreach (var skill in (Target as Player).SkillSet.ActiveSkills)
								if (skill.snoSkill == 462239 && skill.snoRune == 2)
										TotalDamage *= 1f - (Target as Player).Revived.Count * 0.03f;
						break;
					case Hireling:
						ElementDamages[entry.DamageType] *= ReductionFromResistance(Target.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttribute.Level]);
						ElementDamages[entry.DamageType] *= 1f - Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + Target.Attributes[GameAttribute.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						break;
					case Minion:
						ElementDamages[entry.DamageType] *= ReductionFromResistance((Target as Minion).Master.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttribute.Level]);
						break;
				}
			}

			TotalDamage = ElementDamages.Sum(kv => kv.Value);

			if (Context.User.Attributes[GameAttribute.God] == true)
				TotalDamage = 0f;

			// apply critical damage boost
			if (criticalHit)
			{
				TotalDamage *= (1f + Context.User.Attributes[GameAttribute.Crit_Damage_Percent]);
				if (Context.User is Player && (Context.User as Player).Toon.Class == ToonClass.Wizard && Context.User.Attributes[GameAttribute.Resource_On_Crit, 1] > 0)
					if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
						(Context.User as Player).GeneratePrimaryResource(Context.User.Attributes[GameAttribute.Resource_On_Crit, 1]);
			}

			var targetArmor = target.Attributes[GameAttribute.Armor_Total];
			var attackerLevel = attackPayload.Context.User.Attributes[GameAttribute.Level];

			TotalDamage *= ReductionFromArmor(targetArmor, attackerLevel);

			//this.TotalDamage *= 1f - target.Attributes[GameAttribute.Armor_Bonus_Percent];
			//this.TotalDamage *= 1f + target.Attributes[GameAttribute.Amplify_Damage_Percent];
			//this.TotalDamage *= 1f + attackPayload.Context.User.Attributes[GameAttribute.Multiplicative_Damage_Percent_Bonus_No_Pets];
			TotalDamage *= 1f - attackPayload.Context.User.Attributes[GameAttribute.Damage_Done_Reduction_Percent];
			TotalDamage *= 1f + Context.User.Attributes[GameAttribute.Power_Damage_Percent_Bonus, attackPayload.Context.PowerSNO];

			if (PowerMath.Distance2D(Context.User.Position, Target.Position) < 6f)
				TotalDamage *= 1f - Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee];
			else
				TotalDamage *= 1f - Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged];

			DominantDamageType = ElementDamages.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			if (DominantDamageType == null) DominantDamageType = DamageType.Physical;

			switch (Context.User)
			{
				case Player:
					if (IsWeaponDamage)
					{
						var plr = Context.User as Player;
						TotalDamage = TotalDamage * (1 + (plr.PrimaryAttribute / 100f));
						if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
							plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_On_Hit]);

						switch (plr.Toon.Class)
						{
							case ToonClass.WitchDoctor:
								if (plr.SkillSet.HasPassive(217826) && ElementDamages.ContainsKey(DamageType.Poison) && ElementDamages[DamageType.Poison] > 0f) //BadMedicine (wd)
									plr.World.BuffManager.AddBuff(Context.User, Target, new DamageReduceDebuff(0.2f, TickTimer.WaitSeconds(plr.World.Game, 3f)));

								if (plr.SkillSet.HasPassive(208628)) 
									TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(209041) &&
									(
									attackPayload.Context.PowerSNO == 103181 ||
									attackPayload.Context.PowerSNO == 67567 || 
									attackPayload.Context.PowerSNO == 106465
									)) 
									plr.World.BuffManager.AddBuff(plr, plr, new VisionQuestBuff());

								if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
									plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_On_Hit, 0]);
								break;
							case ToonClass.Barbarian:
								if (plr.SkillSet.HasPassive(205187))
									if (plr.Attributes[GameAttribute.Resource_Max_Total, 2] == plr.Attributes[GameAttribute.Resource_Cur, 2])
										TotalDamage *= 1.25f;

								if (plr.SkillSet.HasPassive(205133))
									if (plr.GetObjectsInRange<Monster>(8f).Count >= 3)
										TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(205175))
									if (Target.Attributes[GameAttribute.Hitpoints_Cur] < (Target.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.3f))
										TotalDamage *= 1.4f;
								break;
							case ToonClass.DemonHunter:
								if (plr.SkillSet.HasPassive(164363))
									if (plr.GetObjectsInRange<Monster>(10f).Count == 0) 
										TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(352920)) 
									if (Target.Attributes[GameAttribute.Hitpoints_Cur] > (Target.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.75f))
										TotalDamage *= 1.4f;

								if (plr.SkillSet.HasPassive(218350) && criticalHit) 
									if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
										plr.GenerateSecondaryResource(1f);

								if (plr.SkillSet.HasPassive(155721) && Target.Attributes[GameAttribute.Slow] == true)
									TotalDamage *= 1.20f;

								if (plr.SkillSet.HasPassive(155725))
									plr.World.BuffManager.AddBuff(plr, plr, new SpeedBuff(0.2f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (plr.SkillSet.HasPassive(211225) && plr.World.BuffManager.GetFirstBuff<ThrillOfTheHuntCooldownBuff>(plr) == null) //ThrillOfTheHunt (DH)
								{
									if (!plr.World.BuffManager.HasBuff<DebuffStunned>(Target))
										plr.World.BuffManager.AddBuff(plr, Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 3f)));
									plr.World.BuffManager.AddBuff(plr, plr, new ThrillOfTheHuntCooldownBuff());
								}

								if (criticalHit)
								{
									plr.AddTimedAction(1f, new Action<int>((q) => plr.World.BuffManager.RemoveBuffs(plr, 155715)));
									plr.AddTimedAction(2f, new Action<int>((q) =>
									{
										if (plr.SkillSet.HasPassive(155715))
											plr.World.BuffManager.AddBuff(plr, plr, new SharpshooterBuff());
									}));
								}
								break;
							case ToonClass.Wizard:
								if (plr.SkillSet.HasPassive(208477) && ElementDamages.ContainsKey(DamageType.Arcane) && ElementDamages[DamageType.Arcane] > 0f) //TemporalFlux (wizard)
									if (!plr.World.BuffManager.HasBuff<DebuffSlowed>(Target))
										plr.World.BuffManager.AddBuff(Context.User, Target, new DebuffSlowed(0.8f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (plr.SkillSet.HasPassive(226348) && ElementDamages.ContainsKey(DamageType.Lightning) && ElementDamages[DamageType.Lightning] > 0f) //Paralysis (wizard)
									if (AutomaticHitEffects && !plr.World.BuffManager.HasBuff<DebuffStunned>(Target))
										if (FastRandom.Instance.NextDouble() < 0.15f * Context.GetProcCoefficient())
											plr.World.BuffManager.AddBuff(Context.User, Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (plr.SkillSet.HasPassive(218044) && ElementDamages.ContainsKey(DamageType.Fire) && ElementDamages[DamageType.Fire] > 0f) //Conflagration (wizard)
									plr.World.BuffManager.AddBuff(Context.User, Target, new ArmorReduceDebuff(0.1f, TickTimer.WaitSeconds(plr.World.Game, 3f)));

								if (plr.SkillSet.HasPassive(226301)) //ColdBlooded (Wizard)
									if (Target.Attributes[GameAttribute.Frozen] || Target.Attributes[GameAttribute.Chilled])
										TotalDamage *= 1.1f;

								if (plr.SkillSet.HasPassive(208471)) //GlassCannon (Wizard)
									TotalDamage *= 1.15f;

								if (Target.World.BuffManager.HasBuff<EnergyTwister.GaleForceDebuff>(Target))      //Wizard -> Gale Force
									if (DominantDamageType == DamageType.Fire)
										TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<EnergyTwister.GaleForceDebuff>(Target).Percentage));

								if (Target.World.BuffManager.HasBuff<WizardWaveOfForce.StaticPulseDebuff>(Target))        //Wizard -> Static Pulse
									if (DominantDamageType == DamageType.Lightning)
										TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<WizardWaveOfForce.StaticPulseDebuff>(Target).Percentage));

								if (Target.World.BuffManager.HasBuff<WizardRayOfFrost.SnowBlastDebuff>(Target))       //Wizard -> Snow Blast
									if (DominantDamageType == DamageType.Cold)
										TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<WizardRayOfFrost.SnowBlastDebuff>(Target).Percentage));

								if (Target.World.BuffManager.HasBuff<WizardDisintegrate.IntensifyDebuff>(Target))     //Wizard -> Intensify
									if (DominantDamageType == DamageType.Arcane)
										TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<WizardDisintegrate.IntensifyDebuff>(Target).Percentage));

								if (plr.World.BuffManager.HasBuff<WizardSpectralBlade.FlameBuff>(plr))      //Wizard -> Flame Blades
									if (DominantDamageType == DamageType.Fire)
										TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardSpectralBlade.FlameBuff>(plr).StackCount * 0.01f));

								if (plr.World.BuffManager.HasBuff<ArcaneOrb.OrbShockBuff>(plr))     //Wizard -> Spark
									if (DominantDamageType == DamageType.Lightning)
										TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<ArcaneOrb.OrbShockBuff>(plr).StackCount * 0.02f));

								if (plr.World.BuffManager.HasBuff<WizardWaveOfForce.AttuneBuff>(plr))       //Wizard -> Arcane Attunement
									if (DominantDamageType == DamageType.Arcane)
										TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardWaveOfForce.AttuneBuff>(plr).StackCount * 0.04f));

								if (plr.World.BuffManager.HasBuff<WizardBlackHole.ColdBuff>(plr))       //Wizard -> Absolute Zero
									if (DominantDamageType == DamageType.Cold)
										TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardBlackHole.ColdBuff>(plr).StackCount * 0.03f));

								if (plr.World.BuffManager.HasBuff<WizardBlackHole.DamageBuff>(plr))     //Wizard -> SpellSteal
									TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardBlackHole.DamageBuff>(plr).StackCount * 0.03f));

								if (plr.World.BuffManager.HasBuff<DynamoBuff>(plr))     //Wizard -> Arcane Dynamo
									if (plr.World.BuffManager.GetFirstBuff<DynamoBuff>(plr).StackCount >= 5)
										if (Context.PowerSNO != 0x00007818 && Context.PowerSNO != 0x0000783F &&
											Context.PowerSNO != 0x0001177C && Context.PowerSNO != 0x000006E5) //non-signature
										{
											TotalDamage *= 1.6f;
											plr.World.BuffManager.RemoveBuffs(plr, 208823);
										}

								if (plr.SkillSet.HasPassive(341540)) //Audacity (Wiz)
									if (PowerMath.Distance2D(plr.Position, Target.Position) <= 15f)
										TotalDamage *= 1.15f;

								if (plr.SkillSet.HasPassive(342326)) //Elemental Exposure (Wiz)
								{
									var dmgElement = (int)DominantDamageType.HitEffect;
									if (dmgElement == 1 || dmgElement == 2 || dmgElement == 3 || dmgElement == 5)
									{
										if (Target.World.BuffManager.HasBuff<ElementalExposureBuff>(Target))
										{
											if (Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(Target).LastDamageType != dmgElement)
											{
												Target.World.BuffManager.AddBuff(plr, Target, new ElementalExposureBuff());
												Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(Target).LastDamageType = dmgElement;
											}
										}
										else
										{
											Target.World.BuffManager.AddBuff(plr, Target, new ElementalExposureBuff());
											Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(Target).LastDamageType = dmgElement;
										}
									}
								}
								break;
							case ToonClass.Monk:
								if (plr.World.BuffManager.HasBuff<MysticAllyPassive.MysticAllyBuff>(plr))       //Monk -> Water Ally
									if (plr.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(plr).WaterAlly)
										if (!plr.World.BuffManager.HasBuff<DebuffSlowed>(Target))
											plr.World.BuffManager.AddBuff(Context.User, Target, new DebuffSlowed(0.8f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (Target.World.BuffManager.HasBuff<MantraOfConviction.ActiveDeBuff>(Target))        //Monk -> Mantra of Conviction Active effect
									TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<MantraOfConviction.ActiveDeBuff>(Target).RedAmount));

								if (Target.World.BuffManager.HasBuff<MantraOfConvictionPassive.DeBuff>(Target))       //Monk -> Mantra of Conviction Passive effect
									TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<MantraOfConvictionPassive.DeBuff>(Target).RedAmount));

								if (Target.World.BuffManager.HasBuff<InnerSanctuary.InnerDebuff>(Target))     //Monk -> Forbidden Palace
									TotalDamage *= (1f + (Target.World.BuffManager.GetFirstBuff<InnerSanctuary.InnerDebuff>(Target).DamagePercentage));

								if (plr.SkillSet.HasPassive(211581)) //Resolve (Monk)
									if (!plr.World.BuffManager.HasBuff<DamageReduceDebuff>(Target))
										plr.World.BuffManager.AddBuff(Context.User, Target, new DamageReduceDebuff(0.20f, TickTimer.WaitSeconds(plr.World.Game, 2.5f)));
								break;
							case ToonClass.Crusader:
								if (plr.SkillSet.HasPassive(310804))        //Crusader -> HolyCause
									if (IsWeaponDamage)
										if (DominantDamageType == DamageType.Holy)
											plr.AddPercentageHP(1);

								if (plr.SkillSet.HasPassive(348773))        //Crusader -> Blunt
									if (attackPayload.Context.PowerSNO == 325216 || //Justice
										attackPayload.Context.PowerSNO == 266766)   //Blessed Hammer
										TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(348741))        //Crusader -> Lord Commander
									if (attackPayload.Context.PowerSNO == 330729)       //Phalanx
										TotalDamage *= 1.2f;

								if (plr.World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(plr))              //AkaratChampion -> Rally
									if (plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).CDRActive)
										if (FastRandom.Instance.NextDouble() < 0.5f * Context.GetProcCoefficient())
											foreach (var cdBuff in plr.World.BuffManager.GetBuffs<CooldownBuff>(plr))
												if (!(cdBuff.TargetPowerSNO == 269032))         //do not CDR AkaratChampionBuff
													cdBuff.Reduce(60);
								break;
						}
						
						if (Target is Monster) 
						{
							TotalDamage *= 1 + plr.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, (Target as Monster).MonsterType];

							if ((Target as Monster).Quality > 0) 
								TotalDamage *= 1 + plr.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Elites];

							if (attackPayload.Targets.Actors.Count == 1 && !(attackPayload.Context is Buff) && attackPayload.AutomaticHitEffects)
							{
								float procCoeff = Context.GetProcCoefficient();

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Fear_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffFeared(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Stun_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Blind_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffBlind(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Freeze_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffFrozen(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Chill_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffChilled(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Slow_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new DebuffSlowed(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Knockback_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, Target, new KnockbackBuff(3f));
							}

						}
					}
					break;
				case Minion:
					var mn = Context.User as Minion;
					TotalDamage *= (1 + (mn.PrimaryAttribute / 100f));
					TotalDamage *= mn.Master.Attributes[GameAttribute.Attacks_Per_Second_Total];

					if (mn.Master is Player)
					{
						var mstr = mn.Master as Player;

						if (mstr.SkillSet.HasPassive(209041) && (mn is CorpseSpider || mn is CorpseSpiderQueen))
							mstr.World.BuffManager.AddBuff(mstr, mstr, new VisionQuestBuff());

						if (mn.SNO == ActorSno._dh_companion_spider)
							if (!Context.Target.World.BuffManager.HasBuff<Companion.SpiderWebbedDebuff>(Context.Target))
								Context.Target.World.BuffManager.AddBuff(Context.Target, Context.Target, new Companion.SpiderWebbedDebuff());

						if (Context.Target.World.BuffManager.HasBuff<Fragile.Rune_D_Buff>(Context.Target))
							TotalDamage *= 1.15f;
					}
					break;
			}


			if (Target is Player) //check for passives here (incoming damage)
			{
				var plr = Target as Player;

				if (!plr.Attributes[GameAttribute.Cannot_Dodge] && FastRandom.Instance.NextDouble() < plr.DodgeChance)
					IsDodged = true;

				if (plr.Toon.Class == ToonClass.Monk)       //Monk defensive passives
				{
					TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.World.BuffManager.HasBuff<TempestRush.TempestEffect>(plr))      //Tempest rush -> Slipstream
						if (plr.World.BuffManager.GetFirstBuff<TempestRush.TempestEffect>(plr)._slipStream)
							TotalDamage *= 0.8f;

					if (plr.World.BuffManager.HasBuff<Epiphany.EpiphanyBuff>(plr))      //Epiphany -> Desert Shroud
						if (plr.World.BuffManager.GetFirstBuff<Epiphany.EpiphanyBuff>(plr).DesertShroud)
							TotalDamage *= 0.5f;

					if (IsDodged)      //Mantra of Evasion -> Backlash
						if (plr.World.BuffManager.HasBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr))
							if (plr.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr).Backlash)
								plr.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr).BacklashTrigger = true;
				}

				if (plr.Toon.Class == ToonClass.Barbarian)      //Barb defensive passives
				{
					TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.SkillSet.HasPassive(205491) && PowerMath.Distance2D(Context.User.Position, plr.Position) > 6f) //Superstition (barbarian)
						if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
							plr.GeneratePrimaryResource(2f);

					if (plr.SkillSet.HasPassive(205398) && (plr.Attributes[GameAttribute.Hitpoints_Cur] - TotalDamage) < (plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.2f)) //Relentless (barbarian)
						TotalDamage *= 0.5f;
				}

				if (plr.Toon.Class == ToonClass.Wizard)     //Wizard defensive passives
				{
					if (plr.SkillSet.HasPassive(208471)) //GlassCannon (Wizard)
						TotalDamage *= 1.1f;

					if (plr.SkillSet.HasPassive(208547) && TotalDamage > (plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.15f)) //Illusionist (Wizard)
					{
						foreach (var cdBuff in plr.World.BuffManager.GetBuffs<CooldownBuff>(plr))
							if (cdBuff.TargetPowerSNO == 1769 || cdBuff.TargetPowerSNO == 168344)
								cdBuff.Remove();
					}

					if (plr.SkillSet.HasPassive(208474) && (plr.Attributes[GameAttribute.Hitpoints_Cur] - TotalDamage) <= 0) //UnstableAnomaly (wizard)
					{
						if (plr.World.BuffManager.GetFirstBuff<UnstableAnomalyCooldownBuff>(plr) == null)
						{
							plr.AddPercentageHP(45);
							plr.World.BuffManager.AddBuff(plr, plr, new UnstableAnomalyCooldownBuff());
							plr.World.PowerManager.RunPower(plr, 30796);
							plr.GenerateSecondaryResource(25f);
							foreach (var cdBuff in plr.World.BuffManager.GetBuffs<CooldownBuff>(plr))
								if (cdBuff.TargetPowerSNO == 30796)
									cdBuff.Remove();
						}
					}
				}

				if (plr.Toon.Class == ToonClass.WitchDoctor)        //Witch Doctor defensive passives
				{
					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						TotalDamage *= 0.85f;
				}

				if (plr.Toon.Class == ToonClass.DemonHunter)        //DH defensive passives				
				{
					if (plr.SkillSet.HasPassive(210801) && plr.World.BuffManager.GetFirstBuff<BroodingCooldownBuff>(plr) == null) //Brooding (DH)
						plr.World.BuffManager.AddBuff(plr, plr, new BroodingCooldownBuff());
				}

				if (plr.Toon.Class == ToonClass.Crusader)       //Crusader defensive passives
				{
					TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.SkillSet.HasPassive(310626))        //Vigilant
						if (!(DominantDamageType == DamageType.Physical))
							TotalDamage *= 0.95f;

					if (plr.World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(plr))  //AkaratChampion resurrect once
						if (plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).resurrectActive)
							if ((plr.Attributes[GameAttribute.Hitpoints_Cur] - TotalDamage) <= 0)
							{
								plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).resurrectActive = false;
								plr.AddPercentageHP(100);
							}

					if (plr.World.BuffManager.HasBuff<CrusaderLawsOfJustice.LawsResBuff>(plr))      //Protect the Innocent
						if (!plr.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(plr).Primary)
							if (plr.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(plr).Redirect)
								TotalDamage *= 0.8f;
				}

				TotalDamage *= 0.1f;
			}
			else if (Target is Minion) //check for passives here (incoming damage, minions)
			{
				var minion = Target as Minion;
				if (minion.Master != null && minion.Master is Player)
				{
					var plr = minion.Master as Player;

					var masterArmor = plr.Attributes[GameAttribute.Armor_Total];
					var attackLevel = attackPayload.Context.User.Attributes[GameAttribute.Level];

					TotalDamage *= ReductionFromArmor(masterArmor, attackLevel);

					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						TotalDamage *= 0.85f;

					TotalDamage *= 0.1f; //hack for unkillable minions
				}
			}
		}

		private static float ReductionFromResistance(float resistance, int attackerLevel)
		{
			return 1f - (resistance / ((5 * attackerLevel) + resistance));
		}

		private static float ReductionFromArmor(float armor, int attackerLevel)
		{
			return 1f - (armor / ((50 * attackerLevel) + armor));
		}

		private void CheckItemProcs(Player user)
		{
			if (user.Attributes[GameAttribute.Item_Power_Passive, 247724] == 1 && FastRandom.Instance.NextDouble() < 0.2)
			{
				user.PlayEffectGroup(247770);
			}
			if (user.Attributes[GameAttribute.Item_Power_Passive, 245741] == 1 && FastRandom.Instance.NextDouble() < 0.2)
			{
				user.PlayEffectGroup(245747);
			}
		}

		public void Apply()
		{
			if (Target == null) return;

			if (!Target.World.Game.Working) return;

			if (Target.World.Game.Paused) return;

			if (!Target.Visible)
				return;

			if ((Target.Attributes[GameAttribute.Invulnerable] == true || Target.Attributes[GameAttribute.Immunity] == true) && Target.World != null)
			{
				if (!(Target is Minion))
					Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = Target.DynamicID(plr),
						Number = 0f,
						Type = FloatingNumberMessage.FloatType.Immune
					}, Target);
				return;
			}
			if (new System.Diagnostics.StackTrace().FrameCount > 35) // some arbitrary limit
			{
				Logger.Error("StackOverflowException prevented!: {0}", System.Environment.StackTrace);
				return;
			}

			if (Target is Player)
			{
				var plr = (Target as Player);
				if (plr.Dead) return;

				if (IsDodged)
				{
					Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = Target.DynamicID(plr),
						Number = 0f,
						Type = FloatingNumberMessage.FloatType.Dodge
					}, Target);
					plr.DodgesInARow++;
					if (plr.Toon.Class == ToonClass.Monk && plr.DodgesInARow >= 15)
					{
						plr.GrantAchievement(74987243307548);
					}

					else if (plr.Toon.Class == ToonClass.DemonHunter)    //Awareness
					{
						plr.AddTimedAction(1f, new Action<int>((q) => plr.World.BuffManager.RemoveBuffs(plr, 324770)));
						plr.AddTimedAction(2f, new Action<int>((q) =>
						{
							if (plr.SkillSet.HasPassive(324770))
								plr.World.BuffManager.AddBuff(plr, plr, new AwarenessBuff());
						}));
					}
					return;
				}
				else
				{
					plr.DodgesInARow = 0;
				}

				if (FastRandom.Instance.NextDouble() < Target.Attributes[GameAttribute.Block_Chance_Capped_Total])
				{
					TotalDamage -= (float)FastRandom.Instance.NextDouble((double)Target.Attributes[GameAttribute.Block_Amount_Total_Min], (double)Target.Attributes[GameAttribute.Block_Amount_Total_Max]);
					if (TotalDamage < 0f) TotalDamage = 0f;
					Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = Target.DynamicID(plr),
						Number = TotalDamage,
						Type = FloatingNumberMessage.FloatType.Block
					}, Target);

					Blocked = true;
					plr.BlocksInARow++;
					if (plr.Toon.Class == ToonClass.Barbarian)
					{
						if (plr.BlocksInARow >= 5)
							plr.GrantAchievement(74987243307048);
						if (plr.SkillSet.HasPassive(340877)) //Sword and Board
							if (FastRandom.Instance.NextDouble() < 0.3f)
								plr.GeneratePrimaryResource(6f);
					}
				}
				else
				{
					plr.BlocksInARow = 0;
				}
			}
			if (Target is DesctructibleLootContainer)
			{
				(Target as DesctructibleLootContainer).ReceiveDamage(Target, 100);
				if (Context.PowerSNO == 96296)
					(Context.User as Player).AddAchievementCounter(74987243307049, 1);
				return;
			}

			if (Target.World != null)
				Target.World.BuffManager.SendTargetPayload(Target, this);
			if (Context.User != null)
				Target.World.BuffManager.SendTargetPayload(Context.User, this);

			if (Target == null || Target.World == null) return;   //in case Target was killed in OnPayload

			if (Context.User is Player)
			{
				CheckItemProcs(Context.User as Player);
				if (Context.User.Attributes[GameAttribute.Steal_Health_Percent] > 0)
					(Context.User as Player).AddHP(TotalDamage * Context.User.Attributes[GameAttribute.Steal_Health_Percent]);
				if (Context.User.Attributes[GameAttribute.Hitpoints_On_Hit] > 0)
					(Context.User as Player).AddHP(Context.User.Attributes[GameAttribute.Hitpoints_On_Hit]);
				if (IsCriticalHit)
					if ((Context.User as Player).Toon.Class == ToonClass.Wizard)
						if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
							(Context.User as Player).GeneratePrimaryResource(Context.User.Attributes[GameAttribute.Resource_On_Hit, 1]);
			}

			if (Context.User is Hireling)
			{
				if (Context.User.Attributes[GameAttribute.Steal_Health_Percent] > 0)
					(Context.User as Hireling).AddHP(TotalDamage * Context.User.Attributes[GameAttribute.Steal_Health_Percent]);
				if (Context.User.Attributes[GameAttribute.Hitpoints_On_Hit] > 0)
					(Context.User as Hireling).AddHP(Context.User.Attributes[GameAttribute.Hitpoints_On_Hit]);
			}

			
			// floating damage number
			if (Target.World != null)
			{
				Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage
				{
					ActorID = Target.DynamicID(plr),
					Number = TotalDamage,
					// make player damage red, all other damage white
					Type = IsCriticalHit ?
						(Target is Player) ? FloatingNumberMessage.FloatType.RedCritical : FloatingNumberMessage.FloatType.Golden
											  :
						(Target is Player) ? FloatingNumberMessage.FloatType.Red : FloatingNumberMessage.FloatType.White
				}, Target);
			}

			if (AutomaticHitEffects)
			{
				// play override hit effect it power context has one
				if (Context.EvalTag(PowerKeys.OverrideHitEffects) > 0)
				{
					int efg = Context.EvalTag(PowerKeys.HitEffect);
					if (efg != -1)
						Target.PlayEffectGroup(efg);
				}
				else
				{
					Target.PlayHitEffect((int)DominantDamageType.HitEffect, Context.User);
				}

				if (TotalDamage > 0f)
				{
					// play override hitsound if any, otherwise just default to playing metal weapon hit for now
					int overridenSound = Context.EvalTag(PowerKeys.HitsoundOverride);
					int hitsound = overridenSound != -1 ? overridenSound : 1;
					if (hitsound > 0)
						Target.PlayEffect(Effect.Hit, hitsound);
				}
			}

			// update hp
			float new_hp = Math.Max(Target.Attributes[GameAttribute.Hitpoints_Cur] - TotalDamage, 0f);
			Target.Attributes[GameAttribute.Hitpoints_Cur] = new_hp;
			Target.Attributes.BroadcastChangedIfRevealed();

			//thorns
			//not working for some reason
			/*
			if (this.AutomaticHitEffects)
				if (this.Target.Attributes[GameAttribute.Thorns_Fixed, 0] > 0 && PowerMath.Distance2D(this.Context.User.Position, this.Target.Position) < 12f)
				{
					//Logger.Debug("Thorns: user: {0}, Target: {1}, Damage: {2}", this.Context.User.NameSNOId, this.Target.NameSNOId, this.Target.Attributes[GameAttribute.Thorns_Fixed, 0]);
					PowerContext ThornsContext = this.Context;
					ThornsContext.User = this.Target;
					AttackPayload attack = new AttackPayload(ThornsContext);
					attack.SetSingleTarget(this.Context.User);
					attack.AddDamage(this.Target.Attributes[GameAttribute.Thorns_Fixed, 0], 0f, DamageType.Physical);
					attack.AutomaticHitEffects = false;		//no procs and self-procs from this
					attack.Apply();
				}
			*/

			// if hp=0 do death
			if (new_hp <= 0f)
			{
				var deathload = new DeathPayload(Context, DominantDamageType, Target, Target.HasLoot);
				deathload.AutomaticHitEffects = AutomaticHitEffects;

				if (deathload.Successful)
				{
					Target.Dead = true;
					try
					{
						if (OnDeath != null && AutomaticHitEffects)
							OnDeath(deathload);
					}
					catch { }
					deathload.Apply();
				}
			}
			else if (AutomaticHitEffects && Target.World != null && !(Target is Player))
			{
				// target didn't die, so play hit animation if the actor has one
				if (Target.World.BuffManager.GetFirstBuff<KnockbackBuff>(Target) == null &&
					Target.AnimationSet != null)
				{
					if (Target.AnimationSet.Animations.ContainsKey(AnimationSetKeys.GetHit.ID) && FastRandom.Instance.Next(100) < 33)
					{
						var hitAni = Target.AnimationSet.Animations[AnimationSetKeys.GetHit.ID];
						if (hitAni != AnimationSno._NONE)
						{
							// HACK: hardcoded animation speed/ticks, need to base those off hit recovery speed
							Target.PlayAnimation(6, hitAni, 1.0f, 40);
							foreach (var plr in Target.World.Players.Values)
							{
								if (Target.IsRevealedToPlayer(plr))
								{
									float BackSpeed = Target.WalkSpeed;
									Target.WalkSpeed = 0f;
									TickTimer Timeout = new SecondsTickTimer(Target.World.Game, 0.3f);
									var Boom = Task<bool>.Factory.StartNew(() => WaitTo(Timeout));
									Boom.ContinueWith(delegate
									{
										Target.WalkSpeed = BackSpeed;
									});
								}
							}
						}
					}
				}
			}
		}
	}
}
