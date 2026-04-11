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
using Spectre.Console;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	/// <summary>
	/// Second stage of the damage pipeline. Spawned per-target by
	/// <see cref="AttackPayload.Apply"/>; its constructor computes the
	/// raw-to-final damage number while <see cref="Apply"/> actually
	/// subtracts HP, handles dodge/block, spawns hit visuals, and kicks
	/// off a <see cref="DeathPayload"/> if the hit kills the target.
	///
	/// <para><b>Damage formula (happens in the constructor)</b>, in order:</para>
	/// <list type="number">
	///   <item><description>Roll each <see cref="AttackPayload.DamageEntry"/>
	///     into a float: weapon-based entries read the caster's weapon
	///     damage attributes, flat entries roll <c>min + rand()*delta</c>.</description></item>
	///   <item><description>Apply caster's <c>Damage_Type_Percent_Bonus</c>
	///     and <c>Damage_Dealt_Percent_Bonus</c> per element.</description></item>
	///   <item><description>Apply target's <c>Immunity</c> /
	///     <c>Damage_Percent_Reduction_From_Type</c> /
	///     <c>ReductionFromResistance</c>.</description></item>
	///   <item><description>Apply crit multiplier (<c>1 + Crit_Damage_Percent</c>).</description></item>
	///   <item><description>Apply <see cref="ReductionFromArmor"/>.</description></item>
	///   <item><description>Apply misc caster/target modifiers
	///     (<c>Damage_Done_Reduction_Percent</c>, <c>Power_Damage_Percent_Bonus</c>,
	///     melee/ranged reduction).</description></item>
	///   <item><description>Apply per-class "offensive" passives — Glass
	///     Cannon, Single Out, etc.</description></item>
	///   <item><description>Apply per-class "defensive" passives and the
	///     hard-coded <c>TotalDamage *= 0.1f</c> safety multiplier for
	///     players and minions (this is the primary "players are
	///     unkillable" balance hack — see Battle.md).</description></item>
	/// </list>
	///
	/// <para><b>Main balance dials in this file:</b></para>
	/// <list type="bullet">
	///   <item><description><c>TotalDamage *= 0.1f</c> (line ~553 and ~569):
	///     global player and minion damage-taken multiplier. Raise this to
	///     make players / pets more fragile.</description></item>
	///   <item><description>Class DR multipliers: Monk/Barb/Crusader get
	///     <c>TotalDamage *= 0.7f</c> baseline. Change these to rebalance
	///     class tankiness.</description></item>
	///   <item><description><see cref="ReductionFromArmor"/> /
	///     <see cref="ReductionFromResistance"/> formulas.</description></item>
	/// </list>
	/// </summary>
	public class HitPayload : Payload
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>Final damage amount applied to the target (post all modifiers).</summary>
		public float TotalDamage { get; set; }

		/// <summary>Element that contributed the largest share of damage (used for FX).</summary>
		public DamageType DominantDamageType { get; set; }

		/// <summary>Breakdown of per-element damage before the final aggregate.</summary>
		public Dictionary<DamageType, float> ElementDamages { get; set; }

		/// <summary>True if the hit rolled a crit in <see cref="AttackPayload"/>.</summary>
		public bool IsCriticalHit { get; set; }

		/// <summary>Set on the target's side if it dodged the hit (sets damage to 0).</summary>
		public bool IsDodged { get; set; }

		/// <summary>True if at least one damage entry was weapon-based.</summary>
		public bool IsWeaponDamage { get; set; }

		/// <summary>
		/// False if the hit was skipped (world paused, target dead /
		/// invisible, player not revealed, etc.). <see cref="Apply"/> bails
		/// out early when this is false.
		/// </summary>
		public bool Successful { get; set; }

		/// <summary>True if the target blocked (inside <see cref="Apply"/>).</summary>
		public bool Blocked { get; set; }

		/// <summary>
		/// Inherited from the originating <see cref="AttackPayload"/>.
		/// When false, hit VFX / on-hit procs / death callbacks are
		/// suppressed (used for pure damage DoTs).
		/// </summary>
		public bool AutomaticHitEffects = true;

		/// <summary>
		/// Optional kill callback inherited from the originating
		/// <see cref="AttackPayload"/>, passed through to
		/// <see cref="DeathPayload"/> when the hit is lethal.
		/// </summary>
		public Action<DeathPayload> OnDeath = null;

		/// <summary>
		/// Busy-wait on a <see cref="TickTimer"/>. Used by the hit-recover
		/// animation code to temporarily freeze a monster's walk speed for
		/// 0.3s without blocking the world tick.
		/// </summary>
		private bool WaitTo(TickTimer timer)
		{
			while (timer.TimedOut != true) ;
			return true;
		}

		/// <summary>
		/// Computes the full damage amount for a single target hit. This
		/// is where the bulk of the balance math lives — all class
		/// passives, armor, resistance, crit damage, melee/ranged
		/// reduction, etc. are folded into <see cref="TotalDamage"/> here.
		///
		/// <para>Sets <see cref="Successful"/> to false if the hit should
		/// be discarded (world paused, target dead, stealth, etc.).</para>
		/// </summary>
		public HitPayload(AttackPayload attackPayload, bool criticalHit, Actor target)
			: base(attackPayload.Context, target)
		{
			IsCriticalHit = criticalHit;
			IsDodged = false;
			IsWeaponDamage = (attackPayload.DamageEntries.Count > 0 && attackPayload.DamageEntries.First().IsWeaponBasedDamage);

			Context.User ??= target;
			Target ??= target;

			// Early-out for non-applicable world / target states.
			if (Target?.World == null ||
			    !Target.World.Game.Working ||
			    Target.World.Game.Paused ||
			    !Target.Visible ||
			    Target.Dead)
				return;

			// Monsters can't hit players that haven't revealed them yet —
			// prevents out-of-sight damage.
			if (Context.User is Monster && Context.Target is Player)
				if (!Context.User.IsRevealedToPlayer(Context.Target as Player))
					return;

			Successful = true;

			//float weaponMinDamage = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			//float weaponDamageDelta = this.Context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			// ---- Stage 1: build per-element damage dictionary ----
			// Each damage entry rolls into a typed bucket; players read
			// from Damage_Weapon_Min/Delta_Total; minions scale via the
			// master's weapon damage and the pet's DamageCoefficient;
			// everything else just uses the caster's raw weapon stats.
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
						// Minions inherit their master's weapon damage,
						// scaled by the pet's own DamageCoefficient.
						var master = (Context.User as Minion).Master;
						var dmg_mul = (Context.User as Minion).DamageCoefficient;

						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * dmg_mul * (
							master.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttributes.Damage_Weapon_Min_Total, (int)entry.DamageType.HitEffect] : 0) +
							((float)PowerContext.Rand.NextDouble() * (master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] + ((int)entry.DamageType.HitEffect > 0 ? master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, (int)entry.DamageType.HitEffect] : 0)))
						);
						break;
					default:
						// Monsters and everything else: raw weapon stats.
						ElementDamages[entry.DamageType] += entry.WeaponDamageMultiplier * (Context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0] + ((float)PowerContext.Rand.NextDouble() * Context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0]));
						break;
				}

				// Caster's +X% per element damage bonuses.
				ElementDamages[entry.DamageType] *= 1f + Context.User.Attributes[GameAttributes.Damage_Type_Percent_Bonus, (int)entry.DamageType.HitEffect] + Context.User.Attributes[GameAttributes.Damage_Dealt_Percent_Bonus, (int)entry.DamageType.HitEffect];

				if (Target.Attributes[GameAttributes.Immunity, (int)entry.DamageType.HitEffect] == true) ElementDamages[entry.DamageType] = 0f; //Immunity

				// Per-element resistance and damage-type-reduction on the target.
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
						// Minions borrow their master's resistances.
						ElementDamages[entry.DamageType] *= ReductionFromResistance((Target as Minion).Master.Attributes[GameAttributes.Resistance_Total, (int)entry.DamageType.HitEffect], Context.User.Attributes[GameAttributes.Level]);
						break;
				}
			}

			// ---- Stage 2: collapse elements into TotalDamage ----
			TotalDamage = ElementDamages.Sum(kv => kv.Value);

			// /god mode toggle.
			if (Context.User.Attributes[GameAttributes.God] == true)
				TotalDamage = 0f;

			// Crit multiplier. Wizards also regenerate a bit of resource
			// on crit per their Resource_On_Crit affix.
			if (criticalHit)
			{
				TotalDamage *= (1f + Context.User.Attributes[GameAttributes.Crit_Damage_Percent]);
				if (Context.User is Player player && player.Toon.Class == ToonClass.Wizard && player.Attributes[GameAttributes.Resource_On_Crit, 1] > 0)
					if (FastRandom.Instance.NextDouble() < Context.GetProcCoefficient())
						(Context.User as Player).GeneratePrimaryResource(Context.User.Attributes[GameAttributes.Resource_On_Crit, 1]);
			}

			// Armor reduction — see ReductionFromArmor at the bottom of
			// the file for the exact formula.
			var targetArmor = target.Attributes[GameAttributes.Armor_Total];
			var attackerLevel = attackPayload.Context.User.Attributes[GameAttributes.Level];

			TotalDamage *= ReductionFromArmor(targetArmor, attackerLevel);

			//this.TotalDamage *= 1f - target.Attributes[GameAttribute.Armor_Bonus_Percent];
			//this.TotalDamage *= 1f + target.Attributes[GameAttribute.Amplify_Damage_Percent];
			//this.TotalDamage *= 1f + attackPayload.Context.User.Attributes[GameAttribute.Multiplicative_Damage_Percent_Bonus_No_Pets];
			TotalDamage *= 1f - attackPayload.Context.User.Attributes[GameAttributes.Damage_Done_Reduction_Percent];
			TotalDamage *= 1f + Context.User.Attributes[GameAttributes.Power_Damage_Percent_Bonus, attackPayload.Context.PowerSNO];

			// Melee vs. ranged reduction — distance under 6 tiles counts
			// as melee for DR purposes.
			if (PowerMath.Distance2D(Context.User.Position, Target.Position) < 6f)
				TotalDamage *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Melee];
			else
				TotalDamage *= 1f - Target.Attributes[GameAttributes.Damage_Percent_Reduction_From_Ranged];

			// Pick dominant type for visual effect / element-matching
			// bonuses further down.
			DominantDamageType = ElementDamages.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
			if (DominantDamageType == null) DominantDamageType = DamageType.Physical;

			// ---- Stage 3: offensive per-class passives & bonuses ----
			// The following switch expression folds in dozens of class
			// passives (Glass Cannon, Single Out, Elemental Exposure,
			// Judgment, etc.) and on-hit resource generation. Each class
			// block is self-contained; edit these to rebuild a class's
			// offensive balance.
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

						// Monster-type and elite damage bonuses come from
						// gear affixes (e.g. "+X% dmg vs. demons") —
						// applied here so they scale over every other
						// offensive modifier above.
						if (Target is Monster monster)
						{
							TotalDamage *= 1 + plr.Attributes[GameAttributes.Damage_Percent_Bonus_Vs_Monster_Type, monster.MonsterType];

							if (monster.Quality > 0)
								TotalDamage *= 1 + plr.Attributes[GameAttributes.Damage_Percent_Bonus_Vs_Elites];

							// On-hit CC proc rolls (fear / stun / blind /
							// freeze / chill / slow / knockback). Only
							// fire for single-target hits with automatic
							// hit effects enabled — prevents AoE spam.
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
					// Minion damage scales with its primary attribute
					// and the master's APS (so attack-speed stats on the
					// master benefit pets).
					TotalDamage *= (1 + (mn.PrimaryAttribute / 100f));
					TotalDamage *= mn.Master.Attributes[GameAttributes.Attacks_Per_Second_Total];

					if (mn.Master is Player mstr)
					{
                        try
                        {
                            if (mstr.SkillSet.HasPassive(209041) && mn is CorpseSpider or CorpseSpiderQueen)
                                mstr.World.BuffManager.AddBuff(mstr, mstr, new VisionQuestBuff());

                            if (mn.SNO == ActorSno._dh_companion_spider)
                                if (!Context.Target.World.BuffManager.HasBuff<Companion.SpiderWebbedDebuff>(
                                        Context.Target))
                                    Context.Target.World.BuffManager.AddBuff(Context.Target, Context.Target,
                                        new Companion.SpiderWebbedDebuff());

                            if (Context.Target.World.BuffManager.HasBuff<Fragile.Rune_D_Buff>(Context.Target))
                                TotalDamage *= 1.15f;
                        }
                        catch (Exception ex)
                        {
							Logger.MethodTrace($"Error: $[red3_1]${ex.Message.EscapeMarkup()}$[/]$");
                        }
                    }
					break;
			}


			// ---- Stage 4: defensive per-class passives on the target ----
			// Each class branch applies its own DR passives and then the
			// global "TotalDamage *= 0.1f" safety multiplier that makes
			// players (and their pets) effectively 10× tankier. This is
			// the primary knob for global player survivability; raise it
			// to 1.0f for "vanilla" balance, lower for god-mode testing.
			switch (Target)
			{
				//check for passives here (incoming damage)
				case Player playerTarget:
				{
					// Dodge roll — caps at DodgeChance.
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

					// *** Global player damage-taken multiplier. ***
					// This is the main "players are unkillable" safety
					// hack — scales ALL incoming player damage to 10% of
					// what the formula would otherwise produce. Tune this
					// to change global survivability (see Battle.md).
					TotalDamage *= 0.1f;
					break;
				}
				//check for passives here (incoming damage, minions)
				case Minion { Master: Player playerOwner }:
				{
					var plr = playerOwner;

					// Minions pull armor from their master so that gearing
					// the player tanks the pet.
					var masterArmor = plr.Attributes[GameAttributes.Armor_Total];
					var attackLevel = attackPayload.Context.User.Attributes[GameAttributes.Level];

					TotalDamage *= ReductionFromArmor(masterArmor, attackLevel);

					if (plr.SkillSet.HasPassive(217968)) //JungleFortitude (WD)
						TotalDamage *= 0.85f;

					// Same 10× tankiness multiplier as players — keeps
					// pets from getting one-shot at high difficulties.
					TotalDamage *= 0.1f; //hack for unkillable minions
					break;
				}
			}
		}

		/// <summary>
		/// Resistance formula: <c>1 - (resist / (5*level + resist))</c>.
		/// Classic diminishing-returns curve — matches the vanilla
		/// client's reduction preview numbers.
		/// </summary>
		private static float ReductionFromResistance(float resistance, int attackerLevel) => 1f - (resistance / ((5 * attackerLevel) + resistance));

		/// <summary>
		/// Armor formula: <c>1 - (armor / (50*level + armor))</c>. Same
		/// shape as resistance but 10× the break-even slope. Tuning either
		/// the constant or the curve here is the cleanest way to make
		/// armor matter more or less across the whole game.
		/// </summary>
		private static float ReductionFromArmor(float armor, int attackerLevel) => 1f - (armor / ((50 * attackerLevel) + armor));

		/// <summary>
		/// On-hit visual proc check for a pair of hard-coded item
		/// passive SNOs. Fires a 20% chance visual effect when the player
		/// lands a hit.
		/// </summary>
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

		/// <summary>
		/// Applies the (already-computed) damage to the target:
		/// <list type="number">
		///   <item><description>Early-out for dead/invisible/paused actors.</description></item>
		///   <item><description>Handle invulnerability → draw "Immune" float.</description></item>
		///   <item><description>Player dodge / block rolls.</description></item>
		///   <item><description>Destructible container → 100% damage.</description></item>
		///   <item><description>Forward the payload through the buff
		///     manager (for on-hit buffs like Thorns).</description></item>
		///   <item><description>Lifesteal, hitpoints-on-hit, crit-based
		///     resource generation for the caster.</description></item>
		///   <item><description>Spawn the damage floating number in the
		///     right colour (red for player damage, white for monster).</description></item>
		///   <item><description>Play hit-effect group and sound.</description></item>
		///   <item><description>Subtract HP and broadcast the change.</description></item>
		///   <item><description>If HP hit zero, spawn a <see cref="DeathPayload"/>
		///     and fire <see cref="OnDeath"/>.</description></item>
		///   <item><description>Otherwise maybe play a "get hit" animation
		///     at 33% chance, freezing walk speed for 0.3s.</description></item>
		/// </list>
		/// </summary>
		public void Apply()
		{
			if (Target == null) return;

			if (!Target.World.Game.Working) return;

			if (Target.World.Game.Paused) return;

			if (!Target.Visible)
				return;

			// Invulnerable / immune → draw "Immune" float and bail.
			if ((Target.Attributes[GameAttributes.Invulnerable] || Target.Attributes[GameAttributes.Immunity]) && Target.World != null)
			{
				Logger.Trace("HitPayload.Apply: target {0} is Invulnerable/Immune — damage {1:F1} ignored (power {2})",
					Target.SNO, TotalDamage, Context?.PowerSNO ?? -1);
				if (Target is not Minion)
					Target.World.BroadcastIfRevealed(plr => new FloatingNumberMessage()
					{
						ActorID = Target.DynamicID(plr),
						Number = 0f,
						Type = FloatingNumberMessage.FloatType.Immune
					}, Target);
				return;
			}
			// Recursion safeguard against Thorns / reflect loops.
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

					// Dodge float + achievement tracking.
					if (IsDodged)
					{
						Logger.Trace("Player {0} dodged hit from power {1} (dodges-in-a-row: {2})",
							plr.Toon?.Name ?? "<unknown>", Context?.PowerSNO ?? -1, plr.DodgesInARow + 1);
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

					// Block roll — subtracts a random amount between
					// Block_Amount_Min and Block_Amount_Max.
					if (FastRandom.Instance.NextDouble() < playerActor.Attributes[GameAttributes.Block_Chance_Capped_Total])
					{
						float preBlock = TotalDamage;
						TotalDamage -= (float)FastRandom.Instance.NextDouble((double)playerActor.Attributes[GameAttributes.Block_Amount_Total_Min], (double)playerActor.Attributes[GameAttributes.Block_Amount_Total_Max]);
						if (TotalDamage < 0f) TotalDamage = 0f;
						Logger.Trace("Player {0} blocked {1:F1} → {2:F1} (power {3})",
							plr.Toon?.Name ?? "<unknown>", preBlock - TotalDamage, TotalDamage, Context?.PowerSNO ?? -1);
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
					// Destructibles take 100 hardcoded damage regardless
					// of the computed TotalDamage — they just break.
					container.ReceiveDamage(container, 100);
					if (Context.User is Player plrAddAchievement
					    && Context.PowerSNO == 96296)
						plrAddAchievement.AddAchievementCounter(74987243307049, 1);
					return;
				}
			}

			// Let any buffs on the target react (Thorns, damage reflect,
			// shield-on-hit procs, etc.).
			Target.World?.BuffManager?.SendTargetPayload(Target, this);
			if (Context.User != null)
				Target.World?.BuffManager?.SendTargetPayload(Context.User, this);

			if (Target?.World == null) return;   //in case Target was killed in OnPayload

			// Lifesteal / HP-on-hit for players.
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

			// Lifesteal / HP-on-hit for hirelings.
			if (Context.User is Hireling hireling)
			{
				if (hireling.Attributes[GameAttributes.Steal_Health_Percent] > 0)
					hireling.AddHP(TotalDamage * hireling.Attributes[GameAttributes.Steal_Health_Percent]);
				if (hireling.Attributes[GameAttributes.Hitpoints_On_Hit] > 0)
					hireling.AddHP(hireling.Attributes[GameAttributes.Hitpoints_On_Hit]);
			}

			// Make player damage red, all other damage white; critical
			// hits get the Golden / RedCritical float variants.
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
				// Play the override hit effect if the power's tagmap
				// specifies one, otherwise the default per-element hit
				// effect for this dominant damage type.
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
					// Override hitsound if any, otherwise just default to
					// playing the metal-weapon hit for now.
					int overridenSound = Context.EvalTag(PowerKeys.HitsoundOverride);
					int hitSound = overridenSound != -1 ? overridenSound : 1;
					if (hitSound > 0)
						Target.PlayEffect(Effect.Hit, hitSound);
				}
			}

			// ---- Apply HP damage and broadcast the attribute change ----
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

			// If HP hit zero, spawn the death payload.
			if (newHp <= 0f)
			{
				Logger.Debug("Lethal hit: {0} killed {1} with power {2} ({3:F1} dmg, crit: {4})",
					Context?.User?.SNO.ToString() ?? "<null>",
					Target.SNO,
					Context?.PowerSNO ?? -1,
					TotalDamage,
					IsCriticalHit);

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
					catch (Exception ex)
					{
						Logger.WarnException(ex, "HitPayload.OnDeath callback threw for power {0} on target {1}",
							Context?.PowerSNO ?? -1, Target.SNO);
					}
					deathPayload.Apply();
				}
				else
				{
					Logger.Trace("DeathPayload not Successful — {0} saved from death (power {1})",
						Target.SNO, Context?.PowerSNO ?? -1);
				}
			}
			else if (AutomaticHitEffects && Target.World != null && Target is not Player)
			{
				// Target didn't die → maybe play a "get hit" animation.
				if (Target.World.BuffManager.GetFirstBuff<KnockbackBuff>(Target) == null &&
					Target.AnimationSet != null)
				{
					if (Target.AnimationSet.TagMapAnimDefault.ContainsKey(AnimationSetKeys.GetHit) && FastRandom.Instance.Next(100) < 33)
					{
						var hitAni = (AnimationSno)Target.AnimationSet.TagMapAnimDefault[AnimationSetKeys.GetHit];
						if (hitAni != AnimationSno._NONE)
						{
							// HACK: hardcoded animation speed/ticks, need
							// to base those off hit recovery speed.
							Target.PlayAnimation(6, hitAni, 1.0f, 40);
							foreach (var plr in Target.World.Players.Values)
							{
								if (Target.IsRevealedToPlayer(plr))
								{
									// Freeze monster walk for 0.3s
									// during the get-hit animation.
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
