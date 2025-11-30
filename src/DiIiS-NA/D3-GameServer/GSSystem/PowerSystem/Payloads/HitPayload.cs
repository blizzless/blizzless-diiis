using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using DiIiS_NA.LoginServer.Toons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public class HitPayload : Payload
	{
		public static readonly Logger Logger = LogManager.CreateLogger();
		public float TotalDamage { get; set; }
		public DamageType DominantDamageType { get; set; }
		public Dictionary<DamageType, float> ElementDamages { get; set; }
		public bool IsCriticalHit { get; set; }
		public bool IsDodged { get; set; }
		public bool IsWeaponDamage { get; set; }

		public bool Successful { get; set; }
		public bool Blocked { get; set; }

		public bool AutomaticHitEffects = true;
		public Action<DeathPayload> OnDeath = null;

		private bool WaitTo(TickTimer timer)
		{
			while (timer.TimedOut != true) ;
			return true;
		}

		public HitPayload(AttackPayload attackPayload, bool criticalHit, Actor target)
			: base(attackPayload.Context, target)
		{
			IsCriticalHit = criticalHit;
			IsDodged = false;
			IsWeaponDamage = (attackPayload.DamageEntries.Count > 0 && attackPayload.DamageEntries.First().IsWeaponBasedDamage);

			Context.User ??= target;
			Target ??= target;

			if (Target?.World == null ||
			    !Target.World.Game.Working ||
			    Target.World.Game.Paused ||
			    !Target.Visible ||
			    Target.Dead)
				return;

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
									Context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? Context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0)
									+
									((float)PowerContext.Rand.NextDouble() * (Context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? Context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
								);
						}
						else
							ElementDamages[entry.DamageType] += entry.MinDamage + (float)PowerContext.Rand.NextDouble() * entry.DamageDelta;
						break;
					case Minion:
						var master = (Context.User as Minion).Master;
						var dmg_mul = (Context.User as Minion).DamageCoefficient;

						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * dmg_mul * (
							master.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttributes.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0) +
							((float)PowerContext.Rand.NextDouble() * (master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
						);
						break;
					default:
						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * (Context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0] + ((float)PowerContext.Rand.NextDouble() * Context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0]));
						break;
				}
				
				ElementDamages[entry.DamageType] *= 1f + Context.User.Attributes[GameAttributes.Damage_Type_Percent_Bonus, (int)entry.DamageType.HitEffect] + Context.User.Attributes[GameAttributes.Damage_Dealt_Percent_Bonus, (int)entry.DamageType.HitEffect];

				if (Target.Attributes[GameAttributes.Immunity, (int)entry.DamageType.HitEffect] == true) ElementDamages[entry.DamageType] = 0f; //Immunity

				switch (Target)
				{
					case Player:
						ElementDamages[entry.DamageType] *= ReductionFromResistance(Target.Attributes[GameAttributes.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttributes.Level]);
						ElementDamages[entry.DamageType] *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + Target.Attributes[GameAttributes.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						if ((Target as Player).SkillSet.HasPassive(205491) && (int)entry.DamageType.HitEffect != 0)
							ElementDamages[entry.DamageType] *= 0.8f;
						if((Target as Player).SkillSet.HasSkill(462239))
							foreach (var skill in (Target as Player).SkillSet.ActiveSkills)
								if (skill.snoSkill == 462239 && skill.snoRune == 2)
										TotalDamage *= 1f - (Target as Player).Revived.Count * 0.03f;
						break;
					case Hireling:
						ElementDamages[entry.DamageType] *= ReductionFromResistance(Target.Attributes[GameAttributes.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttributes.Level]);
						ElementDamages[entry.DamageType] *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + Target.Attributes[GameAttributes.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						break;
					case Minion:
						ElementDamages[entry.DamageType] *= ReductionFromResistance((Target as Minion).Master.Attributes[GameAttributes.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttributes.Level]);
						break;
				}
			}

			TotalDamage = ElementDamages.Sum(kv => kv.Value);

			if (Context.User.Attributes[GameAttributes.God] == true)
				TotalDamage = 0f;

			// apply critical damage boost
			if (criticalHit)
			{
				TotalDamage *= (1f + Context.User.Attributes[GameAttributes.Crit_Damage_Percent]);
				if (Context.User is Player player && player.Toon.Class == ToonClass.Wizard && player.Attributes[GameAttributes.Resource_On_Crit, 1] > 0)
					if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
						(Context.User as Player).GeneratePrimaryResource(Context.User.Attributes[GameAttributes.Resource_On_Crit, 1]);
			}

			var targetArmor = target.Attributes[GameAttributes.Armor_Total];
			var attackerLevel = attackPayload.Context.User.Attributes[GameAttributes.Level];

			TotalDamage *= ReductionFromArmor(targetArmor, attackerLevel);

			//this.TotalDamage *= 1f - target.Attributes[GameAttribute.Armor_Bonus_Percent];
			//this.TotalDamage *= 1f + target.Attributes[GameAttribute.Amplify_Damage_Percent];
			//this.TotalDamage *= 1f + attackPayload.Context.User.Attributes[GameAttribute.Multiplicative_Damage_Percent_Bonus_No_Pets];
			TotalDamage *= 1f - attackPayload.Context.User.Attributes[GameAttributes.Damage_Done_Reduction_Percent];
			TotalDamage *= 1f + Context.User.Attributes[GameAttributes.Power_Damage_Percent_Bonus, attackPayload.Context.PowerSNO];

			if (PowerMath.Distance2D(Context.User.Position, Target.Position) < 6f)
				TotalDamage *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Melee];
			else
				TotalDamage *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Ranged];

			DominantDamageType = ElementDamages.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			if (DominantDamageType == null) DominantDamageType = DamageType.Physical;

			switch (Context.User)
			{
				case Player plr:
					if (IsWeaponDamage)
					{
						TotalDamage = TotalDamage * (1 + (plr.PrimaryAttribute / 100f));
						if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
							plr.GeneratePrimaryResource(plr.Attributes[GameAttributes.Resource_On_Hit]);

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
									plr.GeneratePrimaryResource(plr.Attributes[GameAttributes.Resource_On_Hit, 0]);
								break;
							case ToonClass.Barbarian:
								if (plr.SkillSet.HasPassive(205187))
									if (plr.Attributes[GameAttributes.Resource_Max_Total, 2] == plr.Attributes[GameAttributes.Resource_Cur, 2])
										TotalDamage *= 1.25f;

								if (plr.SkillSet.HasPassive(205133))
									if (plr.GetObjectsInRange<Monster>(8f).Count >= 3)
										TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(205175))
									if (Target.Attributes[GameAttributes.Hitpoints_Cur] < (Target.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.3f))
										TotalDamage *= 1.4f;
								break;
							case ToonClass.DemonHunter:
								if (plr.SkillSet.HasPassive(164363))
									if (plr.GetObjectsInRange<Monster>(10f).Count == 0) 
										TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(352920)) 
									if (Target.Attributes[GameAttributes.Hitpoints_Cur] > (Target.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.75f))
										TotalDamage *= 1.4f;

								if (plr.SkillSet.HasPassive(218350) && criticalHit) 
									if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
										plr.GenerateSecondaryResource(1f);

								if (plr.SkillSet.HasPassive(155721) && Target.Attributes[GameAttributes.Slow])
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
									plr.AddTimedAction(1f, _ => plr.World.BuffManager.RemoveBuffs(plr, 155715));
									plr.AddTimedAction(2f, _ =>
									{
										if (plr.SkillSet.HasPassive(155715))
											plr.World.BuffManager.AddBuff(plr, plr, new SharpshooterBuff());
									});
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
									if (Target.Attributes[GameAttributes.Frozen] || Target.Attributes[GameAttributes.Chilled])
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
											foreach (var cooldownBuff in plr.World.BuffManager.GetBuffs<CooldownBuff>(plr))
												if (cooldownBuff.TargetPowerSNO != 269032)         //do not CDR AkaratChampionBuff
													cooldownBuff.Reduce(60);
								break;
						}
						
						if (Target is Monster monster) 
						{
							TotalDamage *= 1 + plr.Attributes[GameAttributes.Damage_Percent_Bonus_Vs_Monster_Type, monster.MonsterTypeValue];

							if (monster.Quality > 0) 
								TotalDamage *= 1 + plr.Attributes[GameAttributes.Damage_Percent_Bonus_Vs_Elites];

							if (attackPayload.Targets.Actors.Count == 1 && !(attackPayload.Context is Buff) && attackPayload.AutomaticHitEffects)
							{
								float procCoeff = Context.GetProcCoefficient();

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Fear_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffFeared(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Stun_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Blind_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffBlind(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Freeze_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffFrozen(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Chill_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffChilled(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Slow_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new DebuffSlowed(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttributes.On_Hit_Knockback_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, monster, new KnockbackBuff(3f));
							}

						}
					}
					break;
				case Minion mn:
					TotalDamage *= (1 + (mn.PrimaryAttribute / 100f));
					TotalDamage *= mn.Master.Attributes[GameAttributes.Attacks_Per_Second_Total];

					if (mn.Master is Player mstr)
					{
						if (mstr.SkillSet.HasPassive(209041) && mn is CorpseSpider or CorpseSpiderQueen)
							mstr.World.BuffManager.AddBuff(mstr, mstr, new VisionQuestBuff());

						if (mn.SNO == ActorSno._dh_companion_spider)
							if (!Context.Target.World.BuffManager.HasBuff<Companion.SpiderWebbedDebuff>(Context.Target))
								Context.Target.World.BuffManager.AddBuff(Context.Target, Context.Target, new Companion.SpiderWebbedDebuff());

						if (Context.Target.World.BuffManager.HasBuff<Fragile.Rune_D_Buff>(Context.Target))
							TotalDamage *= 1.15f;
					}
					break;
			}


			switch (Target)
			{
				//check for passives here (incoming damage)
				case Player playerTarget:
				{
					if (!playerTarget.Attributes[GameAttributes.Cannot_Dodge] && FastRandom.Instance.NextDouble() < playerTarget.DodgeChance)
						IsDodged = true;

					switch (playerTarget.Toon.Class)
					{
						//Monk defensive passives
						case ToonClass.Monk:
						{
							TotalDamage *= 0.7f;       //Class damage reduction bonus

							if (playerTarget.World.BuffManager.HasBuff<TempestRush.TempestEffect>(playerTarget))      //Tempest rush -> Slipstream
								if (playerTarget.World.BuffManager.GetFirstBuff<TempestRush.TempestEffect>(playerTarget)._slipStream)
									TotalDamage *= 0.8f;

							if (playerTarget.World.BuffManager.HasBuff<Epiphany.EpiphanyBuff>(playerTarget))      //Epiphany -> Desert Shroud
								if (playerTarget.World.BuffManager.GetFirstBuff<Epiphany.EpiphanyBuff>(playerTarget).DesertShroud)
									TotalDamage *= 0.5f;

							if (IsDodged)      //Mantra of Evasion -> Backlash
								if (playerTarget.World.BuffManager.HasBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(playerTarget))
									if (playerTarget.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(playerTarget).Backlash)
										playerTarget.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(playerTarget).BacklashTrigger = true;
							break;
						}
						//Barb defensive passives
						case ToonClass.Barbarian:
						{
							TotalDamage *= 0.7f;       //Class damage reduction bonus

							if (playerTarget.SkillSet.HasPassive(205491) && PowerMath.Distance2D(Context.User.Position, playerTarget.Position) > 6f) //Superstition (barbarian)
								if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
									playerTarget.GeneratePrimaryResource(2f);

							if (playerTarget.SkillSet.HasPassive(205398) && (playerTarget.Attributes[GameAttributes.Hitpoints_Cur] - TotalDamage) < (playerTarget.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.2f)) //Relentless (barbarian)
								TotalDamage *= 0.5f;
							break;
						}
						//Wizard defensive passives
						case ToonClass.Wizard:
						{
							if (playerTarget.SkillSet.HasPassive(208471)) //GlassCannon (Wizard)
								TotalDamage *= 1.1f;

							if (playerTarget.SkillSet.HasPassive(208547) && TotalDamage > (playerTarget.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.15f)) //Illusionist (Wizard)
							{
								foreach (var cdBuff in playerTarget.World.BuffManager.GetBuffs<CooldownBuff>(playerTarget))
									if (cdBuff.TargetPowerSNO == 1769 || cdBuff.TargetPowerSNO == 168344)
										cdBuff.Remove();
							}

							if (playerTarget.SkillSet.HasPassive(208474) && (playerTarget.Attributes[GameAttributes.Hitpoints_Cur] - TotalDamage) <= 0) //UnstableAnomaly (wizard)
							{
								if (playerTarget.World.BuffManager.GetFirstBuff<UnstableAnomalyCooldownBuff>(playerTarget) == null)
								{
									playerTarget.AddPercentageHP(45);
									playerTarget.World.BuffManager.AddBuff(playerTarget, playerTarget, new UnstableAnomalyCooldownBuff());
									playerTarget.World.PowerManager.RunPower(playerTarget, 30796);
									playerTarget.GenerateSecondaryResource(25f);
									foreach (var cdBuff in playerTarget.World.BuffManager.GetBuffs<CooldownBuff>(playerTarget))
										if (cdBuff.TargetPowerSNO == 30796)
											cdBuff.Remove();
								}
							}

							break;
						}
						//Witch Doctor defensive passives
						case ToonClass.WitchDoctor:
						{
							if (playerTarget.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
								TotalDamage *= 0.85f;
							break;
						}
						//DH defensive passives				
						case ToonClass.DemonHunter:
						{
							if (playerTarget.SkillSet.HasPassive(210801) && playerTarget.World.BuffManager.GetFirstBuff<BroodingCooldownBuff>(playerTarget) == null) //Brooding (DH)
								playerTarget.World.BuffManager.AddBuff(playerTarget, playerTarget, new BroodingCooldownBuff());
							break;
						}
						//Crusader defensive passives
						case ToonClass.Crusader:
						{
							TotalDamage *= 0.7f;       //Class damage reduction bonus

							if (playerTarget.SkillSet.HasPassive(310626))        //Vigilant
								if (DominantDamageType != DamageType.Physical)
									TotalDamage *= 0.95f;

							if (playerTarget.World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(playerTarget))  //AkaratChampion resurrect once
								if (playerTarget.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(playerTarget).resurrectActive)
									if ((playerTarget.Attributes[GameAttributes.Hitpoints_Cur] - TotalDamage) <= 0)
									{
										playerTarget.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(playerTarget).resurrectActive = false;
										playerTarget.AddPercentageHP(100);
									}

							if (playerTarget.World.BuffManager.HasBuff<CrusaderLawsOfJustice.LawsResBuff>(playerTarget))      //Protect the Innocent
								if (!playerTarget.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(playerTarget).Primary)
									if (playerTarget.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(playerTarget).Redirect)
										TotalDamage *= 0.8f;
							break;
						}
					}

					TotalDamage *= 0.1f;
					break;
				}
				//check for passives here (incoming damage, minions)
				case Minion { Master: Player playerOwner }:
				{
					var plr = playerOwner;

					var masterArmor = plr.Attributes[GameAttributes.Armor_Total];
					var attackLevel = attackPayload.Context.User.Attributes[GameAttributes.Level];

					TotalDamage *= ReductionFromArmor(masterArmor, attackLevel);

					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						TotalDamage *= 0.85f;

					TotalDamage *= 0.1f; //hack for unkillable minions
					break;
				}
			}
		}

		private static float ReductionFromResistance(float resistance, int attackerLevel) => 1f - (resistance / ((5 * attackerLevel) + resistance));

		private static float ReductionFromArmor(float armor, int attackerLevel) => 1f - (armor / ((50 * attackerLevel) + armor));

		private void CheckItemProcs(Player user)
		{
			if (Math.Abs(user.Attributes[GameAttributes.Item_Power_Passive, 247724] - 1) < Globals.FLOAT_TOLERANCE && FastRandom.Instance.NextDouble() < 0.2)
			{
				user.PlayEffectGroup(247770);
			}
			if (Math.Abs(user.Attributes[GameAttributes.Item_Power_Passive, 245741] - 1) < Globals.FLOAT_TOLERANCE && FastRandom.Instance.NextDouble() < 0.2)
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

			if ((Target.Attributes[GameAttributes.Invulnerable] || Target.Attributes[GameAttributes.Immunity]) && Target.World != null)
			{
				if (Target is not Minion)
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

			switch (Target)
			{
				case Player playerActor:
				{
					var plr = playerActor;
					if (plr.Dead) return;

					if (IsDodged)
					{
						playerActor.World.BroadcastIfRevealed(plr2 => new FloatingNumberMessage()
						{
							ActorID = Target.DynamicID(plr2),
							Number = 0f,
							Type = FloatingNumberMessage.FloatType.Dodge
						}, playerActor);
						plr.DodgesInARow++;
						if (plr.Toon.Class == ToonClass.Monk && plr.DodgesInARow >= 15)
						{
							plr.GrantAchievement(74987243307548);
						}

						else if (plr.Toon.Class == ToonClass.DemonHunter)    //Awareness
						{
							plr.AddTimedAction(1f, _ => plr.World.BuffManager.RemoveBuffs(plr, 324770));
							plr.AddTimedAction(2f, _ =>
							{
								if (plr.SkillSet.HasPassive(324770))
									plr.World.BuffManager.AddBuff(plr, plr, new AwarenessBuff());
							});
						}
						return;
					}
					plr.DodgesInARow = 0;

					if (FastRandom.Instance.NextDouble() < playerActor.Attributes[GameAttributes.Block_Chance_Capped_Total])
					{
						TotalDamage -= (float)FastRandom.Instance.NextDouble((double)playerActor.Attributes[GameAttributes.Block_Amount_Total_Min], (double)playerActor.Attributes[GameAttributes.Block_Amount_Total_Max]);
						if (TotalDamage < 0f) TotalDamage = 0f;
						playerActor.World.BroadcastIfRevealed(plr3 => new FloatingNumberMessage()
						{
							ActorID = Target.DynamicID(plr3),
							Number = TotalDamage,
							Type = FloatingNumberMessage.FloatType.Block
						}, playerActor);

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

					break;
				}
				case DesctructibleLootContainer container:
				{
					container.ReceiveDamage(container, 100);
					if (Context.User is Player plrAddAchievement
					    && Context.PowerSNO == 96296)
						plrAddAchievement.AddAchievementCounter(74987243307049, 1);
					return;
				}
			}

			Target.World?.BuffManager?.SendTargetPayload(Target, this);
			if (Context.User != null)
				Target.World?.BuffManager?.SendTargetPayload(Context.User, this);

			if (Target?.World == null) return;   //in case Target was killed in OnPayload

			if (Context.User is Player player)
			{
				CheckItemProcs(player);
				if (player.Attributes[GameAttributes.Steal_Health_Percent] > 0)
					player.AddHP(TotalDamage * Context.User.Attributes[GameAttributes.Steal_Health_Percent]);
				if (Context.User.Attributes[GameAttributes.Hitpoints_On_Hit] > 0)
					player.AddHP(Context.User.Attributes[GameAttributes.Hitpoints_On_Hit]);
				if (IsCriticalHit)
					if (player.Toon.Class == ToonClass.Wizard)
						if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
							player.GeneratePrimaryResource(Context.User.Attributes[GameAttributes.Resource_On_Hit, 1]);
			}

			if (Context.User is Hireling hireling)
			{
				if (hireling.Attributes[GameAttributes.Steal_Health_Percent] > 0)
					hireling.AddHP(TotalDamage * hireling.Attributes[GameAttributes.Steal_Health_Percent]);
				if (hireling.Attributes[GameAttributes.Hitpoints_On_Hit] > 0)
					hireling.AddHP(hireling.Attributes[GameAttributes.Hitpoints_On_Hit]);
			}
			
			// make player damage red, all other damage white
			var type = Target is Player ? 
				IsCriticalHit ? FloatingNumberMessage.FloatType.RedCritical : FloatingNumberMessage.FloatType.Red :
				IsCriticalHit ? FloatingNumberMessage.FloatType.Golden : FloatingNumberMessage.FloatType.White;
			if (Target.World is { } world)
			{
				world.BroadcastIfRevealed(plr => new FloatingNumberMessage
				{
					ActorID = Target.DynamicID(plr),
					Number = TotalDamage,
					Type = type
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
					int hitSound = overridenSound != -1 ? overridenSound : 1;
					if (hitSound > 0)
						Target.PlayEffect(Effect.Hit, hitSound);
				}
			}

			// update hp
			float newHp = Math.Max(Target.Attributes[GameAttributes.Hitpoints_Cur] - TotalDamage, 0f);
			Target.Attributes[GameAttributes.Hitpoints_Cur] = newHp;
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
			if (newHp <= 0f)
			{
				var deathPayload = new DeathPayload(Context, DominantDamageType, Target, Target.HasLoot)
					{
						AutomaticHitEffects = AutomaticHitEffects
					};

				if (deathPayload.Successful)
				{
					Target.Dead = true;
					try
					{
						if (OnDeath != null && AutomaticHitEffects)
							OnDeath(deathPayload);
					}
					catch { }
					deathPayload.Apply();
				}
			}
			else if (AutomaticHitEffects && Target.World != null && Target is not Player)
			{
				// target didn't die, so play hit animation if the actor has one
				if (Target.World.BuffManager.GetFirstBuff<KnockbackBuff>(Target) == null &&
					Target.AnimationSet != null)
				{
					if (Target.AnimationSet.TagMapAnimDefault.ContainsKey(AnimationSetKeys.GetHit) && FastRandom.Instance.Next(100) < 33)
					{
						var hitAni = (AnimationSno)Target.AnimationSet.TagMapAnimDefault[AnimationSetKeys.GetHit];
						if (hitAni != AnimationSno._NONE)
						{
							// HACK: hardcoded animation speed/ticks, need to base those off hit recovery speed
							Target.PlayAnimation(6, hitAni, 1.0f, 40);
							foreach (var plr in Target.World.Players.Values)
							{
								if (Target.IsRevealedToPlayer(plr))
								{
									float backSpeed = Target.WalkSpeed;
									Target.WalkSpeed = 0f;
									TickTimer timeout = new SecondsTickTimer(Target.World.Game, 0.3f);
									var boom = Task<bool>.Factory.StartNew(() => WaitTo(timeout));
									boom.ContinueWith(_ =>
									{
										Target.WalkSpeed = backSpeed;
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
