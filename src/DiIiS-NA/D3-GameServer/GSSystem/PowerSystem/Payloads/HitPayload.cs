//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
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

		private bool WaitTo(TickerSystem.TickTimer timer)
		{
			while (timer.TimedOut != true)
			{

			}
			return true;
		}

		public HitPayload(AttackPayload attackPayload, bool criticalHit, Actor target)
			: base(attackPayload.Context, target)
		{
			this.IsCriticalHit = criticalHit;
			this.IsDodged = false;
			this.IsWeaponDamage = (attackPayload.DamageEntries.Count > 0 ? attackPayload.DamageEntries.First().IsWeaponBasedDamage : false);

			if (this.Context.User == null)
				this.Context.User = target;

			

			if (this.Target == null)
				this.Target = target;

			if (this.Target == null) return;

			if (this.Target.World == null) return;

			if (!this.Target.World.Game.Working) return;

			if (this.Target.World.Game.Paused) return;

			if (!this.Target.Visible) return;

			if (this.Target.Dead) return;

			if (this.Context.User is Monster && this.Context.Target is Player)
				if (!this.Context.User.IsRevealedToPlayer(this.Context.Target as Player))
					return;
			
			this.Successful = true;

			//float weaponMinDamage = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			//float weaponDamageDelta = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			// calculate and add up damage amount for each element type
			this.ElementDamages = new Dictionary<DamageType, float>();

			foreach (var entry in attackPayload.DamageEntries)
			{
				if (!this.ElementDamages.ContainsKey(entry.DamageType))
					this.ElementDamages[entry.DamageType] = 0f;

				switch (this.Context.User)
				{
					case Player:
						if (entry.IsWeaponBasedDamage)
						{
							this.ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier
								* (
									this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0)
									+
									((float)PowerContext.Rand.NextDouble() * (this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
								);
						}
						else
							this.ElementDamages[entry.DamageType] += entry.MinDamage + (float)PowerContext.Rand.NextDouble() * entry.DamageDelta;
						break;
					case Minion:
						var master = (this.Context.User as Minion).Master;
						var dmg_mul = (this.Context.User as Minion).DamageCoefficient;

						this.ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * dmg_mul * (
							master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttribute.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0) +
							((float)PowerContext.Rand.NextDouble() * (master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
						);
						break;
					default:
						this.ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * (this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] + ((float)PowerContext.Rand.NextDouble() * this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0]));
						break;
				}
				
				this.ElementDamages[entry.DamageType] *= 1f + this.Context.User.Attributes[GameAttribute.Damage_Type_Percent_Bonus, (int)entry.DamageType.HitEffect] + this.Context.User.Attributes[GameAttribute.Damage_Dealt_Percent_Bonus, (int)entry.DamageType.HitEffect];

				if (this.Target.Attributes[GameAttribute.Immunity, (int)entry.DamageType.HitEffect] == true) this.ElementDamages[entry.DamageType] = 0f; //Immunity

				switch (this.Target)
				{
					case Player:
						this.ElementDamages[entry.DamageType] *= HitPayload.ReductionFromResistance(this.Target.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], this.Context.User.Attributes[GameAttribute.Level]);
						this.ElementDamages[entry.DamageType] *= 1f - this.Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + this.Target.Attributes[GameAttribute.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						if ((this.Target as Player).SkillSet.HasPassive(205491) && (int)entry.DamageType.HitEffect != 0)
							this.ElementDamages[entry.DamageType] *= 0.8f;
						if((Target as Player).SkillSet.HasSkill(462239))
							foreach (var skill in (Target as Player).SkillSet.ActiveSkills)
								if (skill.snoSkill == 462239 && skill.snoRune == 2)
										TotalDamage *= 1f - (Target as Player).Revived.Count * 0.03f;
						break;
					case Hireling:
						this.ElementDamages[entry.DamageType] *= HitPayload.ReductionFromResistance(this.Target.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], this.Context.User.Attributes[GameAttribute.Level]);
						this.ElementDamages[entry.DamageType] *= 1f - this.Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, (int)entry.DamageType.HitEffect] + this.Target.Attributes[GameAttribute.Amplify_Damage_Type_Percent, (int)entry.DamageType.HitEffect];
						break;
					case Minion:
						this.ElementDamages[entry.DamageType] *= HitPayload.ReductionFromResistance((this.Target as Minion).Master.Attributes[GameAttribute.Resistance_Total, (int)entry.DamageType.HitEffect], this.Context.User.Attributes[GameAttribute.Level]);
						break;
				}
			}

			this.TotalDamage = this.ElementDamages.Sum(kv => kv.Value);

			if (this.Context.User.Attributes[GameAttribute.God] == true)
				this.TotalDamage = 0f;

			// apply critical damage boost
			if (criticalHit)
			{
				this.TotalDamage *= (1f + this.Context.User.Attributes[GameAttribute.Crit_Damage_Percent]);
				if (this.Context.User is Player && (this.Context.User as Player).Toon.Class == ToonClass.Wizard && this.Context.User.Attributes[GameAttribute.Resource_On_Crit, 1] > 0)
					if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
						(this.Context.User as Player).GeneratePrimaryResource(this.Context.User.Attributes[GameAttribute.Resource_On_Crit, 1]);
			}

			var targetArmor = target.Attributes[GameAttribute.Armor_Total];
			var attackerLevel = attackPayload.Context.User.Attributes[GameAttribute.Level];

			this.TotalDamage *= HitPayload.ReductionFromArmor(targetArmor, attackerLevel);

			//this.TotalDamage *= 1f - target.Attributes[GameAttribute.Armor_Bonus_Percent];
			//this.TotalDamage *= 1f + target.Attributes[GameAttribute.Amplify_Damage_Percent];
			//this.TotalDamage *= 1f + attackPayload.Context.User.Attributes[GameAttribute.Multiplicative_Damage_Percent_Bonus_No_Pets];
			this.TotalDamage *= 1f - attackPayload.Context.User.Attributes[GameAttribute.Damage_Done_Reduction_Percent];
			this.TotalDamage *= 1f + this.Context.User.Attributes[GameAttribute.Power_Damage_Percent_Bonus, attackPayload.Context.PowerSNO];

			if (PowerMath.Distance2D(this.Context.User.Position, this.Target.Position) < 6f)
				this.TotalDamage *= 1f - this.Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee];
			else
				this.TotalDamage *= 1f - this.Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged];

			this.DominantDamageType = this.ElementDamages.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			if (this.DominantDamageType == null) this.DominantDamageType = DamageType.Physical;

			switch (this.Context.User)
			{
				case Player:
					if (this.IsWeaponDamage)
					{
						var plr = this.Context.User as Player;
						this.TotalDamage = TotalDamage * (1 + (plr.PrimaryAttribute / 100f));
						if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
							plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_On_Hit]);

						switch (plr.Toon.Class)
						{
							case ToonClass.WitchDoctor:
								if (plr.SkillSet.HasPassive(217826) && this.ElementDamages.ContainsKey(DamageType.Poison) && this.ElementDamages[DamageType.Poison] > 0f) //BadMedicine (wd)
									plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new DamageReduceDebuff(0.2f, TickTimer.WaitSeconds(plr.World.Game, 3f)));

								if (plr.SkillSet.HasPassive(208628)) 
									this.TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(209041) &&
									(
									attackPayload.Context.PowerSNO == 103181 ||
									attackPayload.Context.PowerSNO == 67567 || 
									attackPayload.Context.PowerSNO == 106465
									)) 
									plr.World.BuffManager.AddBuff(plr, plr, new VisionQuestBuff());

								if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
									plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_On_Hit, 0]);
								break;
							case ToonClass.Barbarian:
								if (plr.SkillSet.HasPassive(205187))
									if (plr.Attributes[GameAttribute.Resource_Max_Total, 2] == plr.Attributes[GameAttribute.Resource_Cur, 2])
										this.TotalDamage *= 1.25f;

								if (plr.SkillSet.HasPassive(205133))
									if (plr.GetObjectsInRange<Monster>(8f).Count >= 3)
										this.TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(205175))
									if (this.Target.Attributes[GameAttribute.Hitpoints_Cur] < (this.Target.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.3f))
										this.TotalDamage *= 1.4f;
								break;
							case ToonClass.DemonHunter:
								if (plr.SkillSet.HasPassive(164363))
									if (plr.GetObjectsInRange<Monster>(10f).Count == 0) 
										this.TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(352920)) 
									if (this.Target.Attributes[GameAttribute.Hitpoints_Cur] > (this.Target.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.75f))
										this.TotalDamage *= 1.4f;

								if (plr.SkillSet.HasPassive(218350) && criticalHit) 
									if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
										plr.GenerateSecondaryResource(1f);

								if (plr.SkillSet.HasPassive(155721) && this.Target.Attributes[GameAttribute.Slow] == true)
									this.TotalDamage *= 1.20f;

								if (plr.SkillSet.HasPassive(155725))
									plr.World.BuffManager.AddBuff(plr, plr, new SpeedBuff(0.2f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (plr.SkillSet.HasPassive(211225) && plr.World.BuffManager.GetFirstBuff<Implementations.ThrillOfTheHuntCooldownBuff>(plr) == null) //ThrillOfTheHunt (DH)
								{
									if (!plr.World.BuffManager.HasBuff<DebuffStunned>(this.Target))
										plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 3f)));
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
								if (plr.SkillSet.HasPassive(208477) && this.ElementDamages.ContainsKey(DamageType.Arcane) && this.ElementDamages[DamageType.Arcane] > 0f) //TemporalFlux (wizard)
									if (!plr.World.BuffManager.HasBuff<DebuffSlowed>(this.Target))
										plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new DebuffSlowed(0.8f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (plr.SkillSet.HasPassive(226348) && this.ElementDamages.ContainsKey(DamageType.Lightning) && this.ElementDamages[DamageType.Lightning] > 0f) //Paralysis (wizard)
									if (this.AutomaticHitEffects && !plr.World.BuffManager.HasBuff<DebuffStunned>(this.Target))
										if (FastRandom.Instance.NextDouble() < 0.15f * this.Context.GetProcCoefficient())
											plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (plr.SkillSet.HasPassive(218044) && this.ElementDamages.ContainsKey(DamageType.Fire) && this.ElementDamages[DamageType.Fire] > 0f) //Conflagration (wizard)
									plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new ArmorReduceDebuff(0.1f, TickTimer.WaitSeconds(plr.World.Game, 3f)));

								if (plr.SkillSet.HasPassive(226301)) //ColdBlooded (Wizard)
									if (this.Target.Attributes[GameAttribute.Frozen] || this.Target.Attributes[GameAttribute.Chilled])
										this.TotalDamage *= 1.1f;

								if (plr.SkillSet.HasPassive(208471)) //GlassCannon (Wizard)
									this.TotalDamage *= 1.15f;

								if (this.Target.World.BuffManager.HasBuff<EnergyTwister.GaleForceDebuff>(this.Target))      //Wizard -> Gale Force
									if (this.DominantDamageType == DamageType.Fire)
										this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<EnergyTwister.GaleForceDebuff>(this.Target).Percentage));

								if (this.Target.World.BuffManager.HasBuff<WizardWaveOfForce.StaticPulseDebuff>(this.Target))        //Wizard -> Static Pulse
									if (this.DominantDamageType == DamageType.Lightning)
										this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<WizardWaveOfForce.StaticPulseDebuff>(this.Target).Percentage));

								if (this.Target.World.BuffManager.HasBuff<WizardRayOfFrost.SnowBlastDebuff>(this.Target))       //Wizard -> Snow Blast
									if (this.DominantDamageType == DamageType.Cold)
										this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<WizardRayOfFrost.SnowBlastDebuff>(this.Target).Percentage));

								if (this.Target.World.BuffManager.HasBuff<WizardDisintegrate.IntensifyDebuff>(this.Target))     //Wizard -> Intensify
									if (this.DominantDamageType == DamageType.Arcane)
										this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<WizardDisintegrate.IntensifyDebuff>(this.Target).Percentage));

								if (plr.World.BuffManager.HasBuff<WizardSpectralBlade.FlameBuff>(plr))      //Wizard -> Flame Blades
									if (this.DominantDamageType == DamageType.Fire)
										this.TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardSpectralBlade.FlameBuff>(plr).StackCount * 0.01f));

								if (plr.World.BuffManager.HasBuff<ArcaneOrb.OrbShockBuff>(plr))     //Wizard -> Spark
									if (this.DominantDamageType == DamageType.Lightning)
										this.TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<ArcaneOrb.OrbShockBuff>(plr).StackCount * 0.02f));

								if (plr.World.BuffManager.HasBuff<WizardWaveOfForce.AttuneBuff>(plr))       //Wizard -> Arcane Attunement
									if (this.DominantDamageType == DamageType.Arcane)
										this.TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardWaveOfForce.AttuneBuff>(plr).StackCount * 0.04f));

								if (plr.World.BuffManager.HasBuff<WizardBlackHole.ColdBuff>(plr))       //Wizard -> Absolute Zero
									if (this.DominantDamageType == DamageType.Cold)
										this.TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardBlackHole.ColdBuff>(plr).StackCount * 0.03f));

								if (plr.World.BuffManager.HasBuff<WizardBlackHole.DamageBuff>(plr))     //Wizard -> SpellSteal
									this.TotalDamage *= (1f + (plr.World.BuffManager.GetFirstBuff<WizardBlackHole.DamageBuff>(plr).StackCount * 0.03f));

								if (plr.World.BuffManager.HasBuff<DynamoBuff>(plr))     //Wizard -> Arcane Dynamo
									if (plr.World.BuffManager.GetFirstBuff<DynamoBuff>(plr).StackCount >= 5)
										if (this.Context.PowerSNO != 0x00007818 && this.Context.PowerSNO != 0x0000783F &&
											this.Context.PowerSNO != 0x0001177C && this.Context.PowerSNO != 0x000006E5) //non-signature
										{
											this.TotalDamage *= 1.6f;
											plr.World.BuffManager.RemoveBuffs(plr, 208823);
										}

								if (plr.SkillSet.HasPassive(341540)) //Audacity (Wiz)
									if (PowerMath.Distance2D(plr.Position, this.Target.Position) <= 15f)
										this.TotalDamage *= 1.15f;

								if (plr.SkillSet.HasPassive(342326)) //Elemental Exposure (Wiz)
								{
									var dmgElement = (int)this.DominantDamageType.HitEffect;
									if (dmgElement == 1 || dmgElement == 2 || dmgElement == 3 || dmgElement == 5)
									{
										if (this.Target.World.BuffManager.HasBuff<ElementalExposureBuff>(this.Target))
										{
											if (this.Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(this.Target).LastDamageType != dmgElement)
											{
												this.Target.World.BuffManager.AddBuff(plr, this.Target, new ElementalExposureBuff());
												this.Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(this.Target).LastDamageType = dmgElement;
											}
										}
										else
										{
											this.Target.World.BuffManager.AddBuff(plr, this.Target, new ElementalExposureBuff());
											this.Target.World.BuffManager.GetFirstBuff<ElementalExposureBuff>(this.Target).LastDamageType = dmgElement;
										}
									}
								}
								break;
							case ToonClass.Monk:
								if (plr.World.BuffManager.HasBuff<MysticAllyPassive.MysticAllyBuff>(plr))       //Monk -> Water Ally
									if (plr.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(plr).WaterAlly)
										if (!plr.World.BuffManager.HasBuff<DebuffSlowed>(this.Target))
											plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new DebuffSlowed(0.8f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (this.Target.World.BuffManager.HasBuff<MantraOfConviction.ActiveDeBuff>(this.Target))        //Monk -> Mantra of Conviction Active effect
									this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<MantraOfConviction.ActiveDeBuff>(this.Target).RedAmount));

								if (this.Target.World.BuffManager.HasBuff<MantraOfConvictionPassive.DeBuff>(this.Target))       //Monk -> Mantra of Conviction Passive effect
									this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<MantraOfConvictionPassive.DeBuff>(this.Target).RedAmount));

								if (this.Target.World.BuffManager.HasBuff<InnerSanctuary.InnerDebuff>(this.Target))     //Monk -> Forbidden Palace
									this.TotalDamage *= (1f + (this.Target.World.BuffManager.GetFirstBuff<InnerSanctuary.InnerDebuff>(this.Target).DamagePercentage));

								if (plr.SkillSet.HasPassive(211581)) //Resolve (Monk)
									if (!plr.World.BuffManager.HasBuff<DamageReduceDebuff>(this.Target))
										plr.World.BuffManager.AddBuff(this.Context.User, this.Target, new DamageReduceDebuff(0.20f, TickTimer.WaitSeconds(plr.World.Game, 2.5f)));
								break;
							case ToonClass.Crusader:
								if (plr.SkillSet.HasPassive(310804))        //Crusader -> HolyCause
									if (this.IsWeaponDamage)
										if (this.DominantDamageType == DamageType.Holy)
											plr.AddPercentageHP(1);

								if (plr.SkillSet.HasPassive(348773))        //Crusader -> Blunt
									if (attackPayload.Context.PowerSNO == 325216 || //Justice
										attackPayload.Context.PowerSNO == 266766)   //Blessed Hammer
										this.TotalDamage *= 1.2f;

								if (plr.SkillSet.HasPassive(348741))        //Crusader -> Lord Commander
									if (attackPayload.Context.PowerSNO == 330729)       //Phalanx
										this.TotalDamage *= 1.2f;

								if (plr.World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(plr))              //AkaratChampion -> Rally
									if (plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).CDRActive)
										if (FastRandom.Instance.NextDouble() < 0.5f * this.Context.GetProcCoefficient())
											foreach (var cdBuff in plr.World.BuffManager.GetBuffs<PowerSystem.Implementations.CooldownBuff>(plr))
												if (!(cdBuff.TargetPowerSNO == 269032))         //do not CDR AkaratChampionBuff
													cdBuff.Reduce(60);
								break;
						}
						
						if (this.Target is Monster) 
						{
							this.TotalDamage *= 1 + plr.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, (this.Target as Monster).MonsterType];

							if ((this.Target as Monster).Quality > 0) 
								this.TotalDamage *= 1 + plr.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Elites];

							if (attackPayload.Targets.Actors.Count == 1 && !(attackPayload.Context is Buff) && attackPayload.AutomaticHitEffects)
							{
								float procCoeff = this.Context.GetProcCoefficient();

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Fear_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffFeared(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Stun_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffStunned(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Blind_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffBlind(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Freeze_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffFrozen(TickTimer.WaitSeconds(plr.World.Game, 1.5f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Chill_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffChilled(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Slow_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new DebuffSlowed(0.3f, TickTimer.WaitSeconds(plr.World.Game, 2f)));

								if (FastRandom.Instance.NextDouble() < plr.Attributes[GameAttribute.On_Hit_Knockback_Proc_Chance] * procCoeff)
									plr.World.BuffManager.AddBuff(plr, this.Target, new KnockbackBuff(3f));
							}

						}
					}
					break;
				case Minion:
					var mn = this.Context.User as Minion;
					this.TotalDamage *= (1 + (mn.PrimaryAttribute / 100f));
					this.TotalDamage *= mn.Master.Attributes[GameAttribute.Attacks_Per_Second_Total];

					if (mn.Master is Player)
					{
						var mstr = mn.Master as Player;

						if (mstr.SkillSet.HasPassive(209041) && (mn is CorpseSpider || mn is CorpseSpiderQueen))
							mstr.World.BuffManager.AddBuff(mstr, mstr, new VisionQuestBuff());

						if (mn.ActorSNO.Id == 173827)
							if (!this.Context.Target.World.BuffManager.HasBuff<Companion.SpiderWebbedDebuff>(this.Context.Target))
								this.Context.Target.World.BuffManager.AddBuff(this.Context.Target, this.Context.Target, new Companion.SpiderWebbedDebuff());

						if (this.Context.Target.World.BuffManager.HasBuff<Fragile.Rune_D_Buff>(this.Context.Target))
							this.TotalDamage *= 1.15f;
					}
					break;
			}


			if (this.Target is Player) //check for passives here (incoming damage)
			{
				var plr = this.Target as Player;

				if (!plr.Attributes[GameAttribute.Cannot_Dodge] && FastRandom.Instance.NextDouble() < plr.DodgeChance)
					this.IsDodged = true;

				if (plr.Toon.Class == ToonClass.Monk)       //Monk defensive passives
				{
					this.TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.World.BuffManager.HasBuff<TempestRush.TempestEffect>(plr))      //Tempest rush -> Slipstream
						if (plr.World.BuffManager.GetFirstBuff<TempestRush.TempestEffect>(plr)._slipStream)
							this.TotalDamage *= 0.8f;

					if (plr.World.BuffManager.HasBuff<Epiphany.EpiphanyBuff>(plr))      //Epiphany -> Desert Shroud
						if (plr.World.BuffManager.GetFirstBuff<Epiphany.EpiphanyBuff>(plr).DesertShroud)
							this.TotalDamage *= 0.5f;

					if (this.IsDodged)      //Mantra of Evasion -> Backlash
						if (plr.World.BuffManager.HasBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr))
							if (plr.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr).Backlash)
								plr.World.BuffManager.GetFirstBuff<MantraOfEvasionPassive.MantraOfEvasionBuff>(plr).BacklashTrigger = true;
				}

				if (plr.Toon.Class == ToonClass.Barbarian)      //Barb defensive passives
				{
					this.TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.SkillSet.HasPassive(205491) && PowerMath.Distance2D(this.Context.User.Position, plr.Position) > 6f) //Superstition (barbarian)
						if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
							plr.GeneratePrimaryResource(2f);

					if (plr.SkillSet.HasPassive(205398) && (plr.Attributes[GameAttribute.Hitpoints_Cur] - this.TotalDamage) < (plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.2f)) //Relentless (barbarian)
						this.TotalDamage *= 0.5f;
				}

				if (plr.Toon.Class == ToonClass.Wizard)     //Wizard defensive passives
				{
					if (plr.SkillSet.HasPassive(208471)) //GlassCannon (Wizard)
						this.TotalDamage *= 1.1f;

					if (plr.SkillSet.HasPassive(208547) && this.TotalDamage > (plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.15f)) //Illusionist (Wizard)
					{
						foreach (var cdBuff in plr.World.BuffManager.GetBuffs<PowerSystem.Implementations.CooldownBuff>(plr))
							if (cdBuff.TargetPowerSNO == 1769 || cdBuff.TargetPowerSNO == 168344)
								cdBuff.Remove();
					}

					if (plr.SkillSet.HasPassive(208474) && (plr.Attributes[GameAttribute.Hitpoints_Cur] - this.TotalDamage) <= 0) //UnstableAnomaly (wizard)
					{
						if (plr.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.UnstableAnomalyCooldownBuff>(plr) == null)
						{
							plr.AddPercentageHP(45);
							plr.World.BuffManager.AddBuff(plr, plr, new UnstableAnomalyCooldownBuff());
							plr.World.PowerManager.RunPower(plr, 30796);
							plr.GenerateSecondaryResource(25f);
							foreach (var cdBuff in plr.World.BuffManager.GetBuffs<PowerSystem.Implementations.CooldownBuff>(plr))
								if (cdBuff.TargetPowerSNO == 30796)
									cdBuff.Remove();
						}
					}
				}

				if (plr.Toon.Class == ToonClass.WitchDoctor)        //Witch Doctor defensive passives
				{
					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						this.TotalDamage *= 0.85f;
				}

				if (plr.Toon.Class == ToonClass.DemonHunter)        //DH defensive passives				
				{
					if (plr.SkillSet.HasPassive(210801) && plr.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.BroodingCooldownBuff>(plr) == null) //Brooding (DH)
						plr.World.BuffManager.AddBuff(plr, plr, new BroodingCooldownBuff());
				}

				if (plr.Toon.Class == ToonClass.Crusader)       //Crusader defensive passives
				{
					this.TotalDamage *= 0.7f;       //Class damage reduction bonus

					if (plr.SkillSet.HasPassive(310626))        //Vigilant
						if (!(this.DominantDamageType == DamageType.Physical))
							this.TotalDamage *= 0.95f;

					if (plr.World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(plr))  //AkaratChampion resurrect once
						if (plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).resurrectActive)
							if ((plr.Attributes[GameAttribute.Hitpoints_Cur] - this.TotalDamage) <= 0)
							{
								plr.World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(plr).resurrectActive = false;
								plr.AddPercentageHP(100);
							}

					if (plr.World.BuffManager.HasBuff<CrusaderLawsOfJustice.LawsResBuff>(plr))      //Protect the Innocent
						if (!plr.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(plr).Primary)
							if (plr.World.BuffManager.GetFirstBuff<CrusaderLawsOfJustice.LawsResBuff>(plr).Redirect)
								this.TotalDamage *= 0.8f;
				}

				this.TotalDamage *= 0.1f;
			}
			else if (this.Target is Minion) //check for passives here (incoming damage, minions)
			{
				var minion = this.Target as Minion;
				if (minion.Master != null && minion.Master is Player)
				{
					var plr = minion.Master as Player;

					var masterArmor = plr.Attributes[GameAttribute.Armor_Total];
					var attackLevel = attackPayload.Context.User.Attributes[GameAttribute.Level];

					this.TotalDamage *= HitPayload.ReductionFromArmor(masterArmor, attackLevel);

					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						this.TotalDamage *= 0.85f;

					this.TotalDamage *= 0.1f; //hack for unkillable minions
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
			if (this.Target == null) return;

			if (!this.Target.World.Game.Working) return;

			if (this.Target.World.Game.Paused) return;

			if (!this.Target.Visible)
				return;

			if ((this.Target.Attributes[GameAttribute.Invulnerable] == true || this.Target.Attributes[GameAttribute.Immunity] == true) && this.Target.World != null)
			{
				if (!(this.Target is Minion))
					this.Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = this.Target.DynamicID(plr),
						Number = 0f,
						Type = FloatingNumberMessage.FloatType.Immune
					}, this.Target);
				return;
			}
			if (new System.Diagnostics.StackTrace().FrameCount > 35) // some arbitrary limit
			{
				Logger.Error("StackOverflowException prevented!: {0}", System.Environment.StackTrace);
				return;
			}

			if (this.Target is Player)
			{
				var plr = (this.Target as Player);
				if (plr.Dead) return;

				if (this.IsDodged)
				{
					this.Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = this.Target.DynamicID(plr),
						Number = 0f,
						Type = FloatingNumberMessage.FloatType.Dodge
					}, this.Target);
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

				if (FastRandom.Instance.NextDouble() < this.Target.Attributes[GameAttribute.Block_Chance_Capped_Total])
				{
					this.TotalDamage -= (float)FastRandom.Instance.NextDouble((double)this.Target.Attributes[GameAttribute.Block_Amount_Total_Min], (double)this.Target.Attributes[GameAttribute.Block_Amount_Total_Max]);
					if (this.TotalDamage < 0f) this.TotalDamage = 0f;
					this.Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = this.Target.DynamicID(plr),
						Number = this.TotalDamage,
						Type = FloatingNumberMessage.FloatType.Block
					}, this.Target);

					this.Blocked = true;
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
			if (this.Target is DesctructibleLootContainer)
			{
				(this.Target as DesctructibleLootContainer).ReceiveDamage(this.Target, 100);
				if (this.Context.PowerSNO == 96296)
					(this.Context.User as Player).AddAchievementCounter(74987243307049, 1);
				return;
			}

			if (this.Target.World != null)
				this.Target.World.BuffManager.SendTargetPayload(this.Target, this);
			if (this.Context.User != null)
				this.Target.World.BuffManager.SendTargetPayload(this.Context.User, this);

			if (this.Target == null || this.Target.World == null) return;   //in case Target was killed in OnPayload

			if (this.Context.User is Player)
			{
				this.CheckItemProcs(this.Context.User as Player);
				if (this.Context.User.Attributes[GameAttribute.Steal_Health_Percent] > 0)
					(this.Context.User as Player).AddHP(this.TotalDamage * this.Context.User.Attributes[GameAttribute.Steal_Health_Percent]);
				if (this.Context.User.Attributes[GameAttribute.Hitpoints_On_Hit] > 0)
					(this.Context.User as Player).AddHP(this.Context.User.Attributes[GameAttribute.Hitpoints_On_Hit]);
				if (this.IsCriticalHit)
					if ((this.Context.User as Player).Toon.Class == ToonClass.Wizard)
						if (FastRandom.Instance.NextDouble() < this.Context.GetProcCoefficient())
							(this.Context.User as Player).GeneratePrimaryResource(this.Context.User.Attributes[GameAttribute.Resource_On_Hit, 1]);
			}

			if (this.Context.User is Hireling)
			{
				if (this.Context.User.Attributes[GameAttribute.Steal_Health_Percent] > 0)
					(this.Context.User as Hireling).AddHP(this.TotalDamage * this.Context.User.Attributes[GameAttribute.Steal_Health_Percent]);
				if (this.Context.User.Attributes[GameAttribute.Hitpoints_On_Hit] > 0)
					(this.Context.User as Hireling).AddHP(this.Context.User.Attributes[GameAttribute.Hitpoints_On_Hit]);
			}

			
			// floating damage number
			if (this.Target.World != null)
			{
				this.Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage
				{
					ActorID = this.Target.DynamicID(plr),
					Number = this.TotalDamage,
					// make player damage red, all other damage white
					Type = this.IsCriticalHit ?
						(this.Target is Player) ? FloatingNumberMessage.FloatType.RedCritical : FloatingNumberMessage.FloatType.Golden
											  :
						(this.Target is Player) ? FloatingNumberMessage.FloatType.Red : FloatingNumberMessage.FloatType.White
				}, this.Target);
			}

			if (this.AutomaticHitEffects)
			{
				// play override hit effect it power context has one
				if (this.Context.EvalTag(PowerKeys.OverrideHitEffects) > 0)
				{
					int efg = this.Context.EvalTag(PowerKeys.HitEffect);
					if (efg != -1)
						this.Target.PlayEffectGroup(efg);
				}
				else
				{
					this.Target.PlayHitEffect((int)this.DominantDamageType.HitEffect, this.Context.User);
				}

				if (this.TotalDamage > 0f)
				{
					// play override hitsound if any, otherwise just default to playing metal weapon hit for now
					int overridenSound = this.Context.EvalTag(PowerKeys.HitsoundOverride);
					int hitsound = overridenSound != -1 ? overridenSound : 1;
					if (hitsound > 0)
						this.Target.PlayEffect(Effect.Hit, hitsound);
				}
			}

			// update hp
			float new_hp = Math.Max(this.Target.Attributes[GameAttribute.Hitpoints_Cur] - this.TotalDamage, 0f);
			this.Target.Attributes[GameAttribute.Hitpoints_Cur] = new_hp;
			this.Target.Attributes.BroadcastChangedIfRevealed();

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
				var deathload = new DeathPayload(this.Context, this.DominantDamageType, this.Target, this.Target.HasLoot);
				deathload.AutomaticHitEffects = this.AutomaticHitEffects;

				if (deathload.Successful)
				{
					this.Target.Dead = true;
					try
					{
						if (OnDeath != null && this.AutomaticHitEffects)
							OnDeath(deathload);
					}
					catch { }
					deathload.Apply();
				}
			}
			else if (this.AutomaticHitEffects && this.Target.World != null && !(this.Target is Player))
			{
				// target didn't die, so play hit animation if the actor has one
				if (this.Target.World.BuffManager.GetFirstBuff<Implementations.KnockbackBuff>(this.Target) == null &&
					this.Target.AnimationSet != null)
				{
					if (this.Target.AnimationSet.TagMapAnimDefault.ContainsKey(AnimationSetKeys.GetHit) && FastRandom.Instance.Next(100) < 33)
					{
						int hitAni = this.Target.AnimationSet.TagMapAnimDefault[AnimationSetKeys.GetHit];
						if (hitAni != -1)
						{
							// HACK: hardcoded animation speed/ticks, need to base those off hit recovery speed
							this.Target.PlayAnimation(6, hitAni, 1.0f, 40);
							foreach (var plr in this.Target.World.Players.Values)
							{
								if (this.Target.IsRevealedToPlayer(plr))
								{
									float BackSpeed = this.Target.WalkSpeed;
									this.Target.WalkSpeed = 0f;
									TickerSystem.TickTimer Timeout = new TickerSystem.SecondsTickTimer(this.Target.World.Game, 0.3f);
									var Boom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitTo(Timeout));
									Boom.ContinueWith(delegate
									{
										this.Target.WalkSpeed = BackSpeed;
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
