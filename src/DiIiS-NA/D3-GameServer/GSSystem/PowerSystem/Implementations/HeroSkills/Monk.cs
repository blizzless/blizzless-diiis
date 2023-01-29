using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	//Complete
	#region DeadlyReach
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.DeadlyReach)]
	public class MonkDeadlyReach : ComboSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (HasBuff<Epiphany.EpiphanyBuff>(User))
			{
				if (Target != null)
				{
					if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
						if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						{
							var dashBuff = new EpiphanyDashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
							AddBuff(User, dashBuff);
							yield return dashBuff.Timeout;

							if (Target != null && Target.World != null)
								User.TranslateFacing(Target.Position, true);

							yield return WaitSeconds(0.5f);
						}
				}
			}

			float reachRadius;
			float reachDegrees;

			switch (ComboIndex)
			{
				case 0:
					reachRadius = ScriptFormula(5);
					reachDegrees = ScriptFormula(9);
					break;
				case 1:
					reachRadius = ScriptFormula(4);
					reachDegrees = ScriptFormula(3);
					break;
				case 2:
					reachRadius = ScriptFormula(20);
					reachDegrees = ScriptFormula(19);
					break;
				default:
					yield break;
			}

			float KnockbackChance = 0.5f;
			if (Rune_B > 0) KnockbackChance = 0.66f;

			bool hitAnything = false;
			AttackPayload attack = new AttackPayload(this);
			if (Rune_C > 0 && ComboIndex == 2)      //Scattered blows
			{
				attack.Targets = GetEnemiesInRadius(User.Position, 10f, (int)ScriptFormula(8));
				//User.PlayEffectGroup(141288);
			}
			else
				attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, reachRadius, reachDegrees);

			attack.AddWeaponDamage(ScriptFormula(0), Rune_C > 0 ? DamageType.Lightning : DamageType.Physical);
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				if (ComboIndex == 2 && Rand.NextDouble() < KnockbackChance)
					if (!HasBuff<KnockbackBuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new KnockbackBuff(10f));

				if (Rune_D > 0 && hitPayload.IsCriticalHit)         //Strike from Beyond
					GeneratePrimaryResource(ScriptFormula(7));
			};
			attack.Apply();

			if (hitAnything)
			{
				GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
				if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
					GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

				if (Rune_A > 0 && ComboIndex == 2)      //Foresight
					if (!HasBuff<ReachForesightBuff>(User))
						AddBuff(User, new ReachForesightBuff());

				if (Rune_E > 0 && ComboIndex == 2)      //Keen Eye
					if (!HasBuff<ReachArmorBuff>(User))
						AddBuff(User, new ReachArmorBuff());

				if ((User as Player).SkillSet.HasPassive(218415)) //CombinationStrike (Monk)
					if (!HasBuff<CombinationDeadlyReachBuff>(User))
						AddBuff(User, new CombinationDeadlyReachBuff());

				(User as Player)._SpiritGeneratorHit++;
			}
			yield break;
		}

		[ImplementsPowerBuff(1, true)]
		class ReachForesightBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(14));
				MaxStackCount = (int)ScriptFormula(12);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				_AddAmp();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= StackCount * ScriptFormula(13);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= StackCount * ScriptFormula(13);
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}

			private void _AddAmp()
			{
				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += ScriptFormula(13);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(13);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(2)]
		class ReachArmorBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(21));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(22);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(22);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region FistsOfThunder
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.FistsOfThunder)]
	public class MonkFistsOfThunder : ComboSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				
				if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)// && PowerMath.Distance2D(User.Position, Target.Position) < 30f)         //max distance handled by client
					if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						//User.Teleport(TargetPosition);
						User.Position = TargetPosition;
						float facingAngle = MovementHelpers.GetFacingAngle(User, Target);

						var _mover = new ActorMover(User);
						float speed = Target.Attributes[GameAttribute.Running_Rate_Total] * 9f;
						_mover.Move(Target.Position, 3.5f, new ACDTranslateNormalMessage
						{
							SnapFacing = true,
							MoveFlags = 0x9206, // alt: 0x920e, not sure what this param is for.
												//AnimationTag = 69808, // dashing strike attack animation
							AnimationTag = 69810,
							WalkInPlaceTicks = 6, // ticks to wait before playing attack animation
						});
						/*
						(User as Player).InGameClient.SendMessage(new ACDTranslateSnappedMessage()
						{
							ActorId = (int)User.DynamicID(User as Player),
							Position = User.Position,
							Angle = facingAngle,
							Field3 = true,
							Field4 = 2304,
							CameraSmoothingTime = 3000,
							Field6 = 0xD3C

						});
						(User as Player).InGameClient.SendMessage(new ACDTranslateSyncMessage()
						{
							ActorId = User.DynamicID(User as Player),
							Position = User.Position,
							Snap = true,
							Field3 = 0x15DB
						});
						//*/
						//User.PlayEffectGroup(170232);
						yield return (WaitSeconds(0.5f));
					}
				
			}

			if (ComboIndex == 0 || ComboIndex == 1)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetBestMeleeEnemy();
				attack.AddWeaponDamage(Rune_A > 0 ? ScriptFormula(7) : ScriptFormula(0), DamageType.Lightning);
				attack.OnHit = hitPayload =>
				{
					GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
					if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
						GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

					if (Rune_A > 0)     //Thunderclap
						WeaponDamage(GetEnemiesInRadius(hitPayload.Target.Position, ScriptFormula(6)), ScriptFormula(7), DamageType.Lightning);

					if (Rune_C > 0 && !HasBuff<StaticChargeDebuff>(hitPayload.Target))      //Static Charge
						AddBuff(hitPayload.Target, new StaticChargeDebuff(ScriptFormula(15), WaitSeconds(ScriptFormula(14))));

					if (Rune_D > 0 && hitPayload.IsCriticalHit)     //Quickening
						GeneratePrimaryResource(ScriptFormula(16));

					if (Rune_E > 0)     //Lightning Flash
						AddBuff(User, new TFists_LightningBuff());
				};
				attack.Apply();

				yield break;
			}

			if (ComboIndex == 2)
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(6));

				bool hitAnything = false;
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(TargetPosition, 5f);
				attack.AddWeaponDamage(Rune_A > 0 ? ScriptFormula(7) : ScriptFormula(0), DamageType.Lightning);
				attack.OnHit = hitPayload =>
				{
					hitAnything = true;
					if (Rune_A > 0)     //Thunderclap
						Knockback(hitPayload.Target, ScriptFormula(21));

					if (Rune_C > 0 && !HasBuff<StaticChargeDebuff>(hitPayload.Target))      //Static Charge
						AddBuff(hitPayload.Target, new StaticChargeDebuff(ScriptFormula(15), WaitSeconds(ScriptFormula(14))));

					if (Rune_D > 0 && hitPayload.IsCriticalHit)     //Quickening
						GeneratePrimaryResource(ScriptFormula(16));
				};
				attack.Apply();

				if (hitAnything)
				{
					GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
					if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
						GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

					if (Rune_B > 0)         //Bounding Light
					{
						Actor lastTarget = User;
						for (int i = 0; i < 3; i++)
						{
							var targets = GetEnemiesInRadius(lastTarget.Position, 10f).Actors.Where(a => a != lastTarget).ToList();
							if (targets.Count > 0)
							{
								lastTarget.AddRopeEffect(83875, targets.First());
								WeaponDamage(targets.First(), ScriptFormula(11), DamageType.Lightning);

								if (targets.First() != null && targets.First().World != null)
									lastTarget = targets.First();
								else break;
							}
							else break;
						}
					}

					if (Rune_E > 0)     //Lightning Flash
						AddBuff(User, new TFists_LightningBuff());

					if ((User as Player).SkillSet.HasPassive(218415)) //CombinationStrike (Monk)
						if (!HasBuff<CombinationFistsOfThunderBuff>(User))
							AddBuff(User, new CombinationFistsOfThunderBuff());

					(User as Player)._SpiritGeneratorHit++;
				}
				yield break;
			}

			yield break;
		}

		[ImplementsPowerBuff(2)]
		public class StaticChargeDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public StaticChargeDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload &&
					(payload as HitPayload).IsWeaponDamage && (payload as HitPayload).AutomaticHitEffects &&
					Rand.NextDouble() < 0.3f)
				{
					var targets = GetEnemiesInRadius(Target.Position, 30f).Actors.Where(a => HasBuff<StaticChargeDebuff>(a)).ToList();
					foreach (var target in targets)
					{
						if (Target == null || Target.World == null) break;
						if (target == null || target.World == null) break;
						if (target == Target) continue;

						target.AddRopeEffect(83875, Target);
						AttackPayload charge = new AttackPayload(this);
						charge.SetSingleTarget(target);
						charge.AutomaticHitEffects = false;
						charge.AddWeaponDamage(Percentage, DamageType.Lightning);
						charge.Apply();
					}
				}
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(3, true)]
		class TFists_LightningBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(19));
				MaxStackCount = (int)ScriptFormula(17);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				_AddAmp();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= StackCount * ScriptFormula(18);
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}

			private void _AddAmp()
			{
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(18);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(7)]
		class ComboStage3Buff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(0.5f / EvalTag(PowerKeys.ComboAttackSpeed3));
			}
		}
	}
	#endregion

	//Complete
	#region SevenSidedStrike
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.SevenSidedStrike)]
	public class MonkSevenSidedStrike : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			int _enemiesDamaged = 0;

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f));

			var groundEffect = SpawnProxy(TargetPosition, WaitSeconds(5f));
			groundEffect.PlayEffectGroup(145041);

			if (Rune_A > 0)     //Sudden Assault
				if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					User.PlayEffectGroup(170232);
					yield return (WaitSeconds(0.2f));
					User.Teleport(TargetPosition);
					User.PlayEffectGroup(170232);
					yield return (WaitSeconds(0.2f));
				}

			var targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(6), (int)ScriptFormula(5)).FilterByType<Monster>().Actors;
			foreach (var target in targets)
			{
				if (target == null) continue;
				SpawnEffect(ActorSno._monk_7sidedstrike, target.Position, -1);
				_enemiesDamaged++;

				yield return WaitSeconds(0.1f);
				WeaponDamage(target, ScriptFormula(0), Rune_A > 0 ? DamageType.Lightning : (Rune_E > 0 ? DamageType.Holy : DamageType.Physical));

				if (Rune_C > 0 && Rand.NextDouble() < 0.5f)     //Pandemonium
					if (!HasBuff<DebuffStunned>(target))
						AddBuff(target, new DebuffStunned(WaitSeconds(ScriptFormula(8))));

				if (Rune_E > 0)     //Fulminating Onslaught
				{
					target.PlayEffectGroup(99098);
					var splashTargets = GetEnemiesInRadius(target.Position, 7f);
					splashTargets.Actors.Remove(target); // don't hit target with splash
					WeaponDamage(splashTargets, ScriptFormula(0), DamageType.Holy);
				}
			}

			groundEffect.Destroy();

			if (_enemiesDamaged >= 7)
				(User as Player).GrantAchievement(74987243307547);
			yield break;
		}
	}
	#endregion

	//Complete
	#region CripplingWave
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.CripplingWave)]
	public class MonkCripplingWave : ComboSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (HasBuff<Epiphany.EpiphanyBuff>(User))
			{
				if (Target != null)
				{
					if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
						if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						{
							var dashBuff = new EpiphanyDashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
							AddBuff(User, dashBuff);
							yield return dashBuff.Timeout;

							if (Target != null && Target.World != null)
								User.TranslateFacing(Target.Position, true);

							yield return WaitSeconds(0.5f);
						}
				}
			}

			int effectSNO;
			switch (ComboIndex)
			{
				case 0:
					effectSNO = 18987;
					break;
				case 1:
					effectSNO = 18988;
					break;
				case 2:
					effectSNO = 96519;
					break;
				default:
					yield break;
			}

			User.PlayEffectGroup(effectSNO);

			bool hitAnything = false;
			AttackPayload attack = new AttackPayload(this);
			if (ComboIndex != 2)
				attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(5), ScriptFormula(6));
			else
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(8));
			//dmg all enemies around you, why is there Angle SF(9) - EDIT: oh because it adds them together 180+180=360

			//if (Rune_B > 0 && ComboIndex == 2)
			//User.PlayEffectGroup(147928);

			attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : (Rune_B > 0 ? DamageType.Cold : DamageType.Physical));
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				if (ComboIndex == 2)
					if (!HasBuff<DazeDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DazeDebuff(ScriptFormula(4), WaitSeconds(ScriptFormula(3))));

				if (Rune_C > 0)     //Concussion
					if (!HasBuff<DamageReduceDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DamageReduceDebuff(ScriptFormula(14), WaitSeconds(ScriptFormula(13))));

				if (Rune_D > 0 && hitPayload.IsCriticalHit)     //Rising Tide
					GeneratePrimaryResource(ScriptFormula(16));

				if (Rune_E > 0)     //Breaking Wave
					if (!HasBuff<BreakingWaveDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new BreakingWaveDebuff(ScriptFormula(18), WaitSeconds(ScriptFormula(17))));
			};
			attack.Apply();

			if (hitAnything)
			{
				GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
				if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
					GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

				if ((User as Player).SkillSet.HasPassive(218415)) //CombinationStrike (Monk)
					if (!HasBuff<CombinationCripplingWaveBuff>(User))
						AddBuff(User, new CombinationCripplingWaveBuff());

				(User as Player)._SpiritGeneratorHit++;
			}

			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class DazeDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public DazeDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.WalkSpeed *= (1f - Percentage);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= Percentage;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.WalkSpeed /= (1f - Percentage);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += Percentage;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(3)]
		public class BreakingWaveDebuff : PowerBuff
		{
			public float Percentage;
			public BreakingWaveDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == Target)
					(payload as HitPayload).TotalDamage *= 1 + Percentage;
			}
		}
	}
	#endregion

	//Complete
	#region ExplodingPalm
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.ExplodingPalm)]
	public class MonkExplodingPalm : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (HasBuff<Epiphany.EpiphanyBuff>(User))
			{
				if (Target != null)
				{
					if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
						if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						{
							var dashBuff = new EpiphanyDashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
							AddBuff(User, dashBuff);
							yield return dashBuff.Timeout;

							if (Target != null && Target.World != null)
								User.TranslateFacing(Target.Position, true);

							yield return WaitSeconds(0.5f);
						}
				}
			}

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			var target = GetBestMeleeEnemy();
			if (target.Actors.Count > 0)
			{
				if (!HasBuff<MainDebuff>(target.Actors[0]))
					AddBuff(target.Actors[0], new MainDebuff());

				if (Rune_B > 0 && !HasBuff<DebuffSlowed>(target.Actors[0]))     //Creeping Demise
					AddBuff(target.Actors[0], new DebuffSlowed(ScriptFormula(18), WaitSeconds(ScriptFormula(3))));

				if (Rune_C > 0 && !HasBuff<RuneCDebuff>(target.Actors[0]))          //The Flesh is Weak
					AddBuff(target.Actors[0], new RuneCDebuff());
			}

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class MainDebuff : PowerBuff
		{
			const float _damageRate = 1.0f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(3));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Bleeding] = true;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Bleeding] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null)
					_damageTimer = WaitSeconds(_damageRate);    //first hit only after a second

				if (_damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(Rune_A > 0 ? 1.8f : (Rune_E > 0 ? 1.8f : 1.31f), Rune_E > 0 ? DamageType.Fire : DamageType.Physical);
					attack.AutomaticHitEffects = false;
					attack.Apply();
				}
				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is DeathPayload)
				{
					var targets = GetEnemiesInRadius(Target.Position, 7f);  //yep, the radius was too large
					targets.Actors.Remove(Target);

					if (Rune_E > 0)         //Essence Burn
					{
						foreach (var tgt in targets.Actors)
							AddBuff(tgt, new BurnDebuff());
					}
					else
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = targets;
						attack.AddDamage(ScriptFormula(9) * Target.Attributes[GameAttribute.Hitpoints_Max_Total], 0f, DamageType.Physical);

						if (Rune_D > 0)     //Strong Spirit
						{
							attack.OnHit = (hitPayload) =>
							{
								GeneratePrimaryResource(10f);
							};
						}
						attack.AutomaticHitEffects = false;
						attack.Apply();
					}

					SpawnProxy(Target.Position).PlayEffectGroup(18991);
				}
			}
		}

		[ImplementsPowerBuff(1)]
		class RuneCDebuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(25));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(15);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(15);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(2, true)]
		class BurnDebuff : PowerBuff
		{
			TickTimer _damageTimer = null;
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(23));
				MaxStackCount = 5;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Bleeding] = true;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Bleeding] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(1f);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(0.86f * StackCount, DamageType.Fire);
					attack.AutomaticHitEffects = false;
					attack.Apply();
				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region SweepingWind
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.SweepingWind)]
	public class MonkSweepingWind : Skill
	{
		//Buff0: Spirit Per second
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if (!HasBuff<SweepBuff>(User))
				AddBuff(User, new SweepBuff());

			yield break;
		}

		[ImplementsPowerBuff(0, true)]
		class SweepBuff : PowerBuff
		{
			const float _damageRate = 0.25f;
			TickTimer _damageTimer = null;
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(8));
				MaxStackCount = (int)ScriptFormula(10);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				AddBuff(User, new VortexBuff());
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == Target && payload is HitPayload)
				{
					if (!(payload as HitPayload).AutomaticHitEffects) return;
					AddBuff(User, new SweepBuff());
				}
			}

			public override void Remove()
			{
				base.Remove();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(13));
					attack.AddWeaponDamage((ScriptFormula(6) * StackCount) / 4f, Rune_B > 0 ? DamageType.Fire : (Rune_C > 0 ? DamageType.Lightning : (Rune_E > 0 ? DamageType.Cold : DamageType.Physical)));
					//we divide by four because this is by second, and tick-intervals = 0.25
					attack.AutomaticHitEffects = false;
					attack.Apply();
				}
				return false;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				AddBuff(User, new VortexBuff());

				if (StackCount == MaxStackCount)
					if (!HasBuff<VortexFullBuff>(Target))
						AddBuff(Target, new VortexFullBuff());

				return true;
			}
		}

		[ImplementsPowerBuff(1)]
		class VortexBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(8));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(3)]
		class VortexFullBuff : PowerBuff
		{
			private ActorMover _tornadoMover;
			const float _cycloneRate = 0.25f;
			TickTimer _cycloneTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(8));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_D > 0)     //Inner Storm
				{
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += ScriptFormula(23);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == Target && payload is HitPayload)
				{
					//Cyclone, changed to not create twisters from any of the sweeping wind's effects, only from other skills
					if (Rune_C > 0 && (payload as HitPayload).AutomaticHitEffects && (payload as HitPayload).IsCriticalHit)
					{
						if (_cycloneTimer == null || _cycloneTimer.TimedOut)
						{
							_cycloneTimer = WaitSeconds(_cycloneRate);

							var tornado = new EffectActor(this, ActorSno._monk_sweepingwind_tornado, Target.Position);
							tornado.Timeout = WaitSeconds(ScriptFormula(21));
							tornado.Scale = 1f;
							tornado.Spawn();
							tornado.UpdateDelay = 1f; // attack every half-second
							tornado.OnUpdate = () =>
							{
								AttackPayload attack = new AttackPayload(this);
								attack.Targets = GetEnemiesInRadius(tornado.Position, 5f);
								attack.AddWeaponDamage(ScriptFormula(29), DamageType.Lightning);
								attack.AutomaticHitEffects = false;
								attack.Apply();
							};
							var closetarget = GetEnemiesInRadius(Target.Position, 15f).GetClosestTo(Target.Position);
							_tornadoMover = new ActorMover(tornado);
							_tornadoMover.Move(RandomDirection(closetarget.Position, 0f, 10f), ScriptFormula(29), new ACDTranslateNormalMessage
							{
								SnapFacing = true,
								AnimationTag = 69728,
							});
						}
					}
				}
			}

			public override void Remove()
			{
				base.Remove();

				if (Rune_D > 0)
				{
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= ScriptFormula(23);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}
		}
	}
	#endregion

	//Complete
	#region WayOfTheHundredFists
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.WayOfTheHundredFists)]
	public class MonkWayOfTheHundredFists : ComboSkill
	{
		TickTimer WindForceTimer = null;
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_A <= 0 && HasBuff<Epiphany.EpiphanyBuff>(User))
			{
				if (Target != null)
				{
					if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
						if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						{
							var dashBuff = new EpiphanyDashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
							AddBuff(User, dashBuff);
							yield return dashBuff.Timeout;

							if (Target != null && Target.World != null)
								User.TranslateFacing(Target.Position, true);

							yield return WaitSeconds(0.5f);
						}
				}
			}

			if (Rune_A > 0 && Target != null) //Fists of Fury
			{
				if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
					if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						var dashBuff = new DashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
						AddBuff(User, dashBuff);
						yield return dashBuff.Timeout;

						if (Target != null && Target.World != null)
							User.TranslateFacing(Target.Position, true);

						yield return WaitSeconds(0.5f);
					}
			}

			if (Rune_D > 0 && Rand.NextDouble() < ScriptFormula(25))    //Spirited Salvo
				GeneratePrimaryResource(ScriptFormula(24));

			bool hitAnything = false;

			if (ComboIndex == 0)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, 6f);
				attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Holy : (Rune_E > 0 ? DamageType.Cold : DamageType.Physical));
				attack.OnHit = hitPayload =>
				{
					hitAnything = true;
					if (Rune_A > 0)     //Fists of Fury
						AddBuff(hitPayload.Target, new RuneA_DOT_100Fists());

					if (Rune_C > 0 && hitPayload.IsCriticalHit)     //Blazing Fists
						AddBuff(User, new RuneCbuff());
				};
				attack.Apply();

				if (hitAnything)
				{
					GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
					if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
						GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);
				}
				yield break;
			}

			if (ComboIndex == 1)
			{
				bool hitCrit = false;
				for (int i = 0; i < (Rune_B > 0 ? 10 : 7); i++)
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetBestMeleeEnemy();
					attack.AddWeaponDamage(ScriptFormula(1), Rune_A > 0 ? DamageType.Holy : (Rune_E > 0 ? DamageType.Cold : DamageType.Physical));
					attack.AutomaticHitEffects = false;     //no procs from this stage, would be op
					attack.OnHit = hitPayload =>
					{
						hitAnything = true;
						if (hitPayload.IsCriticalHit) hitCrit = true;
					};
					attack.Apply();
				}

				if (hitAnything)
				{
					GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
					if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
						GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

					if (Rune_C > 0 && hitCrit)      //Blazing Fists
						AddBuff(User, new RuneCbuff());
				}
				yield break;
			}

			if (ComboIndex == 2)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, 6f);
				attack.AddWeaponDamage(ScriptFormula(2), Rune_A > 0 ? DamageType.Holy : (Rune_E > 0 ? DamageType.Cold : DamageType.Physical));
				attack.OnHit = hitPayload =>
				{
					hitAnything = true;
					if (Rune_A > 0)     //Fists of Fury
						AddBuff(hitPayload.Target, new RuneA_DOT_100Fists());

					if (Rune_C > 0 && hitPayload.IsCriticalHit)     //Blazing Fists
						AddBuff(User, new RuneCbuff());
				};
				attack.Apply();

				if (hitAnything)
				{
					GeneratePrimaryResource(EvalTag(PowerKeys.SpiritGained));
					if (HasBuff<BreathOfHeaven.HeavensSpiritBuff>(User))
						GeneratePrimaryResource(User.World.BuffManager.GetFirstBuff<BreathOfHeaven.HeavensSpiritBuff>(User).SpiritAmount);

					if ((User as Player).SkillSet.HasPassive(218415)) //CombinationStrike (Monk)
						if (!HasBuff<CombinationWayOf100FistBuff>(User))
							AddBuff(User, new CombinationWayOf100FistBuff());

					(User as Player)._SpiritGeneratorHit++;
				}

				if (Rune_E > 0)     //Windforce Flurry
				{
					var startPosition = User.Position;
					var proj = new Projectile(this, ActorSno._monk_wayofthehundredfists_alabaster_projectile, startPosition);
					proj.Position.Z += 5f;
					proj.OnUpdate = () =>
					{
						if (PowerMath.Distance2D(proj.Position, startPosition + new Vector3D(0, 0, 5f)) > 40f)
						{
							proj.Destroy();
							return;
						}

						if (WindForceTimer == null || WindForceTimer.TimedOut)
						{
							WindForceTimer = WaitSeconds(0.8f);
							WeaponDamage(GetEnemiesInRadius(proj.Position, 6f), 1.91f, DamageType.Cold);
						}
					};
					proj.Launch(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 20f), 0.6f);
				}
				yield break;
			}

			yield break;
		}
		public override float GetContactDelay()
		{
			return 0f;
		}

		[ImplementsPowerBuff(0)]
		class DashMoverBuff : PowerBuff
		{
			private Vector3D _destination;
			private ActorMover _mover;

			public DashMoverBuff(Vector3D destination)
			{
				_destination = destination;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				// dash speed seems to always be actor speed * 10
				float speed = Target.Attributes[GameAttribute.Running_Rate_Total] * 9f;

				Target.TranslateFacing(_destination, true);
				_mover = new ActorMover(Target);
				_mover.Move(_destination, speed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					MoveFlags = 0x9206, // alt: 0x920e, not sure what this param is for.
					AnimationTag = 90544,
					WalkInPlaceTicks = 6, // ticks to wait before playing attack animation
				});

				// make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
				Timeout = _mover.ArrivalTime;

				Target.Attributes[GameAttribute.Hidden] = true;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Hidden] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				return base.Update();
			}
		}

		[ImplementsPowerBuff(3)]
		class RuneA_DOT_100Fists : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(4f);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null)
					_damageTimer = WaitSeconds(_damageRate);

				if (_damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(Target, 0.2f, DamageType.Holy);
				}

				return false;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
		[ImplementsPowerBuff(1, true)]
		class RuneCbuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(10));
				MaxStackCount = (int)ScriptFormula(9);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				_AddAmp();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= StackCount * ScriptFormula(7);
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= StackCount * ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}

			private void _AddAmp()
			{
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += ScriptFormula(7);
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region DashingStrike
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.DashingStrike)]
	public class MonkDashingStrike : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Max(Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 45f), EvalTag(PowerKeys.WalkingDistanceMin)));

			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

			//StartCooldown(1f);
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			User.Attributes[GameAttribute.Skill_Charges, 312736] = Math.Max(User.Attributes[GameAttribute.Skill_Charges, 312736] - 1, 0);

			AttackPayload dash = new AttackPayload(this);
			dash.Targets = GetEnemiesInBeamDirection(User.Position, TargetPosition, PowerMath.Distance2D(User.Position, TargetPosition), 5f);
			dash.AddWeaponDamage(ScriptFormula(13), Rune_C > 0 ? DamageType.Cold : DamageType.Physical);
			dash.OnHit = hitPayload =>
			{
				if (Rune_E > 0 && Rand.NextDouble() < 0.4f)     //Flying Side Kick
					if (!HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(33))));
			};

			if (Rune_A > 0)     //Barrage
			{
				if (dash.Targets.Actors.Any())
					AddBuff(dash.Targets.GetClosestTo(TargetPosition), new DashingBarrageDotBuff());
			}

			var dashBuff = new DashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
			AddBuff(User, dashBuff);
			yield return dashBuff.Timeout;

			dash.Apply();

			if (Rune_B > 0)         //Way of the Falling Star
				if (!HasBuff<DashingMovementBuff>(User))
					AddBuff(User, new DashingMovementBuff(ScriptFormula(3), WaitSeconds(ScriptFormula(10))));

			if (Rune_C > 0)         //Blinding Speed
				if (!HasBuff<DodgeBuff>(User))
					AddBuff(User, new DodgeBuff());
		}

		[ImplementsPowerBuff(0)]
		class DashMoverBuff : PowerBuff
		{
			private Vector3D _destination;
			private ActorMover _mover;

			public DashMoverBuff(Vector3D destination)
			{
				_destination = destination;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				// dash speed seems to always be actor speed * 10
				float speed = Target.Attributes[GameAttribute.Running_Rate_Total] * 9f;

				Target.TranslateFacing(_destination, true);
				_mover = new ActorMover(Target);
				_mover.Move(_destination, speed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					MoveFlags = 0x9206, // alt: 0x920e, not sure what this param is for.
									 //AnimationTag = 69808, // dashing strike attack animation
					AnimationTag = 69810,
					WalkInPlaceTicks = 6, // ticks to wait before playing attack animation
				});

				// make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
				Timeout = _mover.ArrivalTime;

				//Target.Attributes[GameAttribute.Hidden] = true;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				//Target.Attributes[GameAttribute.Hidden] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				return base.Update();
			}
		}

		[ImplementsPowerBuff(1)]
		class DashingBarrageDotBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(3f);
				_damageTimer = WaitSeconds(_damageRate);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(Target, ScriptFormula(8), DamageType.Physical);
				}

				return false;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		class DodgeBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(19));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(18);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(18);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(3)]
		public class DashingMovementBuff : PowerBuff
		{
			public float Percentage;

			public DashingMovementBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(0)]
		public class DashingStrikeCountBuff : PowerBuff
		{
			public bool CoolDownStarted = false;
			public int Max = 2;

			public override bool Update()
			{
				if (base.Update())
					return true;
				Max = (User.Attributes[GameAttribute.Rune_D, 312736] > 0) ? 3 : 2;


				if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] < Max)
				{
					if (!CoolDownStarted)
					{
						StartCooldownCharges(6f); CoolDownStarted = true;

						Task.Delay(6100).ContinueWith(delegate
						{
							CoolDownStarted = false;
							User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = Math.Min(User.Attributes[GameAttribute.Skill_Charges, PowerSNO] + 1, Max);
							User.Attributes.BroadcastChangedIfRevealed();
						});
					}
				}

				return false;
			}

		}
	}

	[ImplementsPowerSNO(364249)]
	public class DashingStrikePassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new DashingStrikeBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class DashingStrikeBuff : PowerBuff
		{
			public bool CoolDownStarted = false;
			public int Max = 2;

			public override bool Update()
			{
				if (base.Update())
					return true;
				Max = (User.Attributes[GameAttribute.Rune_D, 312736] > 0) ? 3 : 2;


				if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] < Max)
				{
					if (!CoolDownStarted)
					{
						StartCooldownCharges(6f); CoolDownStarted = true;

						Task.Delay(6100).ContinueWith(delegate
						{
							CoolDownStarted = false;
							User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = Math.Min(User.Attributes[GameAttribute.Skill_Charges, PowerSNO] + 1, Max);
							//User.Attributes[GameAttribute.Next_Charge_Gained_time, 75301] = 0;
							User.Attributes.BroadcastChangedIfRevealed();
						});
					}
				}

				return false;
			}

		}
	}
	#endregion

	//Complete
	#region MantraOfEvasion
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.Mantras.MantraOfEvasion)]
	public class MonkMantraOfEvasion : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost) * ((User as Player).SkillSet.HasPassive(156467) ? 0.5f : 1f)); //Chant Of Resonance

			AddBuff(User, new DodgeBuff());
			foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
				AddBuff(ally, new DodgeBuff());

			(User as Player).AddAchievementCounter(74987243307546, 1);

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class DodgeBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(13));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(2);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(2);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(375050)]
	public class MantraOfEvasionPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction); //MantraOfConviction
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing); //MantraOfHealing
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution); //MantraOfRetribution
			AddBuff(User, new MantraOfEvasionBuff());
			yield break;
		}

		[ImplementsPowerBuff(5)]
		public class MantraOfEvasionBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			private float _unityDamageBonus = 0f;
			public bool Backlash = false;
			public bool BacklashTrigger = false;
			bool DivineShield = false;
			public override bool Apply()
			{
				if (!base.Apply()) return false;

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(2);

				if (Target.Attributes[GameAttribute.Rune_A, 375049] > 0)        //Backlash
					Backlash = true;

				if (Target.Attributes[GameAttribute.Rune_B, 375049] > 0)        //Perseverance
					Target.Attributes[GameAttribute.CrowdControl_Reduction] += ScriptFormula(4);

				if (Target.Attributes[GameAttribute.Rune_C, 375049] > 0)        //Hard Target
					Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(5);

				if (Target.Attributes[GameAttribute.Rune_D, 375049] > 0)        //Wind through the Reeds
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(6);

				if (Target.Attributes[GameAttribute.Rune_E, 375049] > 0)        //Divine Protection
					DivineShield = true;

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;

				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(2);

				if (Target.Attributes[GameAttribute.Rune_B, 375049] > 0)
					Target.Attributes[GameAttribute.CrowdControl_Reduction] -= ScriptFormula(4);

				if (Target.Attributes[GameAttribute.Rune_C, 375049] > 0)
					Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(5);

				if (Target.Attributes[GameAttribute.Rune_D, 375049] > 0)
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(6);

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 2f;

				if (_unityDamageBonus > 0)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;
					_unityDamageBonus = 0f;
				}
				Target.Attributes.BroadcastChangedIfRevealed();
				// aura fade effect
				Target.PlayEffectGroup(199677);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null) CheckTimer = WaitSeconds(1f);

				if (CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					foreach (Actor ally in GetAlliesInRadius(Target.Position, ScriptFormula(0)).Actors)
					{
						if (!HasBuff<EvasionAllyBuff>(ally))
							AddBuff(ally, new EvasionAllyBuff(User, Target.Attributes[GameAttribute.Rune_B, 375049] > 0,
									Target.Attributes[GameAttribute.Rune_C, 375049] > 0,
									Target.Attributes[GameAttribute.Rune_D, 375049] > 0,
									WaitSeconds(1.1f)));
						else ally.World.BuffManager.GetFirstBuff<EvasionAllyBuff>(ally).Extend(60);
					}

					if (BacklashTrigger)    //Backlash, done in HitPayload
					{
						if (Rand.NextDouble() < 0.35f)
						{
							Target.PlayEffectGroup(212532);

							AttackPayload attack = new AttackPayload(this);
							attack.Targets = GetEnemiesInRadius(Target.Position, 12f);
							attack.AddWeaponDamage(0.8f, DamageType.Fire);
							attack.AutomaticHitEffects = false;
							attack.Apply();
						}
						BacklashTrigger = false;
					}

					if (Target is Player)
						if ((Target as Player).IsInPvPWorld) return false;

					if ((Target as Player).SkillSet.HasPassive(368899) &&
						 _unityDamageBonus != (0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count())) //Unity
					{
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;

						_unityDamageBonus = 0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count();

						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += _unityDamageBonus;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				}

				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					if (DivineShield)       //Divine Protection
						if (Target.Attributes[GameAttribute.Hitpoints_Cur] <
							Target.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.25f)
						{
							if (!HasBuff<ArmorCDBuff>(Target)) AddBuff(Target, new ArmorBuff());
						}
				}
			}
		}

		[ImplementsPowerBuff(6)]
		class EvasionAllyBuff : PowerBuff
		{
			Actor Caster = null;
			private bool Perseverance = false;
			private bool HardTarget = false;
			private bool Wind = false;
			private bool _unityBonus = false;
			public EvasionAllyBuff(Actor caster, bool perseverance, bool hardTarget, bool wind, TickTimer timeout)
			{
				Caster = caster;
				Perseverance = perseverance;
				HardTarget = hardTarget;
				Wind = wind;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply()) return false;

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += 0.17f;

				if (Perseverance) Target.Attributes[GameAttribute.CrowdControl_Reduction] += 0.2f;
				if (HardTarget) Target.Attributes[GameAttribute.Armor_Bonus_Percent] += 0.2f;
				if (Wind) Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.1f;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
					if ((Target as Player).IsInPvPWorld) return true;

				if (Caster != null && Caster.World != null)
					if ((Caster as Player).SkillSet.HasPassive(368899)) //Unity
					{
						_unityBonus = true;
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.05f;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.05f;
						Target.Attributes.BroadcastChangedIfRevealed();
					}

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= 0.17f;

				if (Perseverance) Target.Attributes[GameAttribute.CrowdControl_Reduction] -= 0.2f;
				if (HardTarget) Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= 0.2f;
				if (Wind) Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.1f;

				if (_unityBonus)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= 0.05f;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= 0.05f;
				}
				Target.Attributes.BroadcastChangedIfRevealed();
				// aura fade effect
				Target.PlayEffectGroup(199677);
			}
		}

		[ImplementsPowerBuff(2)]
		class ArmorCDBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(90f);
			}
		}

		[ImplementsPowerBuff(3)]
		class ArmorBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(3f);
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage)
				{
					(payload as HitPayload).TotalDamage *= 0.2f;
				}
			}

			public override void Remove()
			{
				base.Remove();

				try
				{
					AddBuff(Target, new ArmorCDBuff());
				}
				catch { }
			}
		}
	}
	#endregion

	//Complete
	#region BlindingFlash
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.BlindingFlash)]
	public class MonkBlindingFlash : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			//var UsedPoint = SpawnProxy(User.Position);

			var targets = GetEnemiesInRadius(User.Position, ScriptFormula(1)).Actors;
			foreach (var target in targets)
			{
				TickTimer waitBuffEnd = WaitSeconds(ScriptFormula(0));

				// add main effect buff only if blind debuff took effect
				if (AddBuff(target, new DebuffBlind(waitBuffEnd)))
				{
					AddBuff(target, new MainEffectBuff(waitBuffEnd));
					if (Rune_B > 0) GeneratePrimaryResource(ScriptFormula(14));     //Replenishing Light						
				}

				if (Rune_C > 0)         //Mystifying Light
				{
					if (!HasBuff<DebuffSlowed>(target))
						AddBuff(target, new DebuffSlowed(ScriptFormula(20), WaitSeconds(ScriptFormula(19))));
				}
			}

			if (Rune_A > 0)     //Faith in the Light
			{
				if (!HasBuff<FlashingRuneABuff>(User))
					AddBuff(User, new FlashingRuneABuff(WaitSeconds(ScriptFormula(3))));
			}

			if (Rune_E > 0)     //Soothing Light
			{
				var allies = GetAlliesInRadius(User.Position, ScriptFormula(1)).Actors;
				foreach (var target in allies)
				{
					if (!HasBuff<FlashIndigoBuff>(target))
						AddBuff(target, new FlashIndigoBuff(WaitSeconds(ScriptFormula(23))));
				}
			}
			yield break;
		}

		[ImplementsPowerBuff(5)]
		class MainEffectBuff : PowerBuff
		{
			public MainEffectBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Hit_Chance] -= ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Hit_Chance] += ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(1)]
		class FlashingRuneABuff : PowerBuff
		{
			public FlashingRuneABuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(4);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(4);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(4)]
		class FlashIndigoBuff : PowerBuff
		{
			public float Regen = 0f;
			public FlashIndigoBuff(TickTimer timeout)
			{
				Regen = LifeRegen(Target.Attributes[GameAttribute.Level]);
				Timeout = timeout;
			}
			private float LifeRegen(int level)      //SF is bugged, use our own formula
			{
				return (10 + 0.04f * (float)Math.Pow(level, 3));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += Regen;
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] -= Regen;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(3)]
		class ConfusionDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Team_Override] = 1;
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Team_Override] = 10;
			}
		}
	}
	#endregion

	//Complete
	#region LashingTailKick
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.LashingTailKick)]
	public class LashingTailKick : Skill
	{
		TickTimer TwisterTimer = null;
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_B <= 0 && Rune_C <= 0 && HasBuff<Epiphany.EpiphanyBuff>(User))
			{
				if (Target != null)
				{
					if (PowerMath.Distance2D(User.Position, Target.Position) > 12f)     //Max distance handled by client
						if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						{
							var dashBuff = new EpiphanyDashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
							AddBuff(User, dashBuff);
							yield return dashBuff.Timeout;

							if (Target != null && Target.World != null)
								User.TranslateFacing(Target.Position, true);

							yield return WaitSeconds(0.5f);
						}
				}
			}

			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			var dmgType = DamageType.Physical;
			if (Rune_A > 0 || Rune_B > 0) dmgType = DamageType.Fire;
			if (Rune_C > 0) dmgType = DamageType.Cold;
			if (Rune_E > 0) dmgType = DamageType.Lightning;

			if (Rune_B > 0)     //Spinning Flame Kick
			{
				var startPosition = User.Position;
				var proj = new Projectile(this, ActorSno._monk_lashingtailkick_indigo_projectile, User.Position);
				proj.OnUpdate = () =>
				{
					if (PowerMath.Distance2D(proj.Position, startPosition + new Vector3D(0, 0, 5f)) > 25f)
					{
						proj.PlayEffectGroup(190831);
						proj.Destroy();
						return;
					}

					if (TwisterTimer == null || TwisterTimer.TimedOut)
					{
						TwisterTimer = WaitSeconds(0.8f);

						WeaponDamage(GetEnemiesInRadius(proj.Position, 7f), 6.77f, dmgType);
					}
				};
				proj.Launch(TargetPosition, 0.4f);
				yield break;
			}

			if (Rune_C > 0)     //Hand of Ytar
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f));
				SpawnEffect(ActorSno._monk_lashingtailkick_bigfoot, TargetPosition);
			}

			AttackPayload attack = new AttackPayload(this);
			if (Rune_A > 0) attack.Targets = GetEnemiesInRadius(User.Position, 10f);
			else if (Rune_C > 0) attack.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(12));
			else attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(6), ScriptFormula(7));

			attack.AddWeaponDamage(ScriptFormula(0), dmgType);
			attack.OnHit = hitPayload =>
			{
				if (Rune_C > 0 && !HasBuff<DebuffChilled>(hitPayload.Target))
					AddBuff(hitPayload.Target, new DebuffChilled(ScriptFormula(18), WaitSeconds(ScriptFormula(21))));

				if (Rune_D > 0)     //Sweeping Armada
				{
					if (!HasBuff<KnockbackBuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new KnockbackBuff(5f));
					if (!HasBuff<DebuffSlowed>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(14), WaitSeconds(ScriptFormula(15))));
				}

				if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(11))        //Scorpion Sting
					if (!HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(8))));
			};
			attack.Apply();
			yield break;
		}
	}
	#endregion

	//Complete
	#region TempestRush
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.TempestRush)]
	public class TempestRush : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new TempestEffect());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class TempestEffect : PowerBuff
		{
			private TickTimer _damageTimer = null;
			private float _damageDelay = 0f;
			private float _damageMult = 1f;
			private float _channelCost = 15f;
			public bool _slipStream = false;
			private DamageType _dmgType = DamageType.Physical;

			public override void Init() //resolved all SF for better performance
			{
				_damageMult = Rune_D > 0 ? 4.5f : 2.4f;     //Northern Breeze
				_dmgType = Rune_E > 0 ? DamageType.Cold : (Rune_D > 0 ? DamageType.Holy : (Rune_A > 0 ? DamageType.Fire : DamageType.Physical));
				_damageDelay = Math.Max(1f / (User.Attributes[GameAttribute.Attacks_Per_Second_Total] * 1.3f), 0.3f);
				_channelCost = Rune_D > 0 ? 13f : 15f;
				if (Rune_C > 0) _slipStream = true;     //Slipstream, done in HitPayload
				Timeout = WaitSeconds(_damageDelay);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_B > 0)     //Tailwind
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.25f;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (Rune_B > 0)
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.25f;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageDelay);
					UsePrimaryResource(_channelCost);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, 6f);
					attack.AddWeaponDamage(_damageMult, _dmgType);
					attack.OnHit = (hitPayload) =>
					{
						if (Rune_A > 0)     //Bluster
						{
							if (!HasBuff<KnockbackBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new KnockbackBuff(8f));

							if (!HasBuff<DamageReduceDebuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DamageReduceDebuff(0.2f, WaitSeconds(1f)));
						}

						if (Rune_E > 0)     //Flurry
							if (!HasBuff<DebuffChilled>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffChilled(0.8f, WaitSeconds(3f)));
					};
					attack.Apply();
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region WaveOfLight
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.WaveOfLight)]
	public class WaveOfLight : Skill
	{
		public override int GetCastEffectSNO()
		{
			return base.GetCastEffectSNO();
		}
		public override int GetContactEffectSNO()
		{
			return base.GetContactEffectSNO();
		}

		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			//projectile distance (50)
			if (Rune_B > 0)         //Explosive Light
			{
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, User.Position + new Vector3D(30, 0, 0), 45f, 8);

				//yield return WaitSeconds(0.1f);
				AttackPayload explosion = new AttackPayload(this);
				explosion.Targets = new TargetList();

				foreach (var projTarget in projDestinations)
				{
					var proj = new Projectile(this, ActorSno._waveoflight_projectile_bells, User.Position);
					proj.Launch(projTarget, 1.5f);

					explosion.Targets.Actors.AddRange(GetEnemiesInBeamDirection(User.Position, projTarget, 35f, 3f).Actors);
				}

				explosion.Targets = explosion.Targets.Distinct();
				explosion.AddWeaponDamage(ScriptFormula(23), DamageType.Holy);
				explosion.Apply();

				yield break;
			}

			if (Rune_C > 0)     //Pillar of the Ancients
			{
				Vector3D pillarPos = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 7f);
				//yield return WaitSeconds(0.1f);

				// TODO: check actor sno
				var Column = new EffectActor(this, ActorSno._p1_monk_waveoflight_pillar, pillarPos);
				Column.Timeout = WaitSeconds(2f);
				Column.Scale = 1f;
				Column.Spawn();
				Column.OnTimeout = () =>
				{
					Column.PlayEffectGroup(182676);
					AttackPayload second_attack = new AttackPayload(this);
					second_attack.Targets = GetEnemiesInRadius(pillarPos, 20f);
					second_attack.AddWeaponDamage(ScriptFormula(31), DamageType.Physical);
					second_attack.Apply();
				};

				AttackPayload first_attack = new AttackPayload(this);
				first_attack.Targets = GetEnemiesInRadius(pillarPos, 20f);
				first_attack.AddWeaponDamage(ScriptFormula(17), DamageType.Physical);
				first_attack.Apply();

				yield break;
			}

			Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 7f);

			var bell = new EffectActor(this, RuneSelect(ActorSno._monk_waveoflight_proxy_spirit, ActorSno._monk_waveoflight_proxy_damage, ActorSno.__NONE, ActorSno.__NONE, ActorSno._monk_waveoflight_proxy_spirit, ActorSno._monk_waveoflight_proxy_aoe), inFrontOfUser);
			bell.Timeout = WaitSeconds(0.4f);
			bell.Scale = 1f;
			bell.Spawn(MovementHelpers.GetFacingAngle(User.Position, inFrontOfUser));
			bell.OnTimeout = () =>
			{
				AttackPayload bellRing = new AttackPayload(this);
				bellRing.Targets = GetEnemiesInRadius(inFrontOfUser, 12f);
				bellRing.AddWeaponDamage(ScriptFormula(3), DamageType.Holy);
				bellRing.OnHit = hitPayload =>
				{
					if (Rune_A > 0)     //Wall of Light
						if (!HasBuff<KnockbackBuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new KnockbackBuff(8f));
				};
				bellRing.Apply();
				bell.PlayEffectGroup(145011);
			};

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(inFrontOfUser, 12f);
			attack.AddWeaponDamage(ScriptFormula(14), DamageType.Holy);
			attack.OnHit = hit =>
			{
				if (Rune_E > 0 && hit.IsCriticalHit)        //Blinding Light
					if (!HasBuff<DebuffStunned>(hit.Target))
						AddBuff(hit.Target, new DebuffStunned(WaitSeconds(ScriptFormula(39))));
			};
			attack.Apply();

			yield break;
		}
	}
	#endregion

	//Complete
	#region BreathOfHeaven
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.BreathOfHeaven)]
	public class BreathOfHeaven : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			User.PlayEffectGroup(RuneSelect(136256, 136258, 183337, 101174, 136259, 136257));

			var allies = GetAlliesInRadius(User.Position, ScriptFormula(4)).Actors;
			allies.Add(User);

			foreach (var ally in allies)
			{
				if (!(ally is Player)) continue;

				(ally as Player).AddPercentageHP(Rune_B > 0 ? 60 : 40, (User as Player).SkillSet.HasPassive(156492));

				if (Rune_E > 0 && !HasBuff<MovementBuff>(ally as Player))       //Zephyr
					AddBuff(ally as Player, new MovementBuff(ScriptFormula(14), WaitSeconds(ScriptFormula(13))));
			}

			if (Rune_A > 0)     //Circle of Scorn
				WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(4)), ScriptFormula(0), DamageType.Holy);

			if (Rune_C > 0 && !HasBuff<HeavensDamageBuff>(User))        //Blazing Wrath
				AddBuff(User, new HeavensDamageBuff(WaitSeconds(ScriptFormula(8))));

			if (Rune_D > 0 && !HasBuff<HeavensSpiritBuff>(User))        //Infused with Light
				AddBuff(User, new HeavensSpiritBuff(ScriptFormula(12), WaitSeconds(ScriptFormula(11))));

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class HeavensDamageBuff : PowerBuff
		{
			//firedamage
			public HeavensDamageBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += ScriptFormula(9);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(9);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= ScriptFormula(9);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(9);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(1)]
		public class HeavensSpiritBuff : PowerBuff
		{
			//Placeholder for generating extra spirit from generator skills
			public float SpiritAmount = 0f;
			public HeavensSpiritBuff(float spiritAmount, TickTimer timeout)
			{
				SpiritAmount = spiritAmount;
				Timeout = timeout;
			}
		}
	}
	#endregion

	//Complete
	#region MantraOfRetribution
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution)]
	public class MantraOfRetribution : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost) * ((User as Player).SkillSet.HasPassive(156467) ? 0.5f : 1f)); //Chant Of Resonance

			AddBuff(User, new CastEffect(WaitSeconds(ScriptFormula(1))));

			(User as Player).AddAchievementCounter(74987243307546, 1);

			yield break;
		}

		[ImplementsPowerBuff(1)]
		class CastEffect : PowerBuff
		{
			//masterFX
			public CastEffect(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage &&
					(payload as HitPayload).AutomaticHitEffects)
				{
					AttackPayload retaliate = new AttackPayload(this);
					retaliate.SetSingleTarget(payload.Context.User);
					retaliate.AddWeaponDamage(1.01f, DamageType.Holy);
					retaliate.AutomaticHitEffects = false;  //no procs and looping
					retaliate.Apply();
				}
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
	}

	[ImplementsPowerSNO(375083)]
	public class MantraOfRetributionPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction); //MantraOfConviction
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing); //MantraOfHealing
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfEvasion); //MantraOfEvasion
			AddBuff(User, new MantraOfRetributionBuff());
			yield break;
		}

		[ImplementsPowerBuff(5)]
		class MantraOfRetributionBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			private float _unityDamageBonus = 0f;
			public override bool Apply()
			{
				if (!base.Apply()) return false;

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;

				if (Target.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0)  //Transgression
					Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += ScriptFormula(22);

				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 2f;

				if (Target.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0)
					Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= ScriptFormula(22);

				if (_unityDamageBonus > 0)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;
					_unityDamageBonus = 0f;
				}
				Target.Attributes.BroadcastChangedIfRevealed();
				// aura fade effect
				Target.PlayEffectGroup(199677);
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null) CheckTimer = WaitSeconds(1f);

				if (CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					foreach (Actor ally in GetAlliesInRadius(Target.Position, ScriptFormula(0)).Actors)
					{
						if (!HasBuff<CastGroupBuff>(ally))
							AddBuff(ally, new CastGroupBuff(User, WaitSeconds(1.1f)));
						else ally.World.BuffManager.GetFirstBuff<CastGroupBuff>(ally).Extend(60);
					}

					if (Target is Player)
						if ((Target as Player).IsInPvPWorld) return false;

					if ((Target as Player).SkillSet.HasPassive(368899) && _unityDamageBonus != (0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count())) //Unity
					{
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;

						_unityDamageBonus = 0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count();

						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += _unityDamageBonus;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				}
				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage &&
					(payload as HitPayload).AutomaticHitEffects)
				{
					AttackPayload retaliate = new AttackPayload(this);
					retaliate.SetSingleTarget(payload.Context.User);

					if (Target.Attributes[GameAttribute.Rune_A, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Retaliation
						retaliate.AddWeaponDamage(2.02f, DamageType.Fire);
					else
						retaliate.AddWeaponDamage(1.01f, DamageType.Holy);

					retaliate.AutomaticHitEffects = false;  //no procs and looping
					retaliate.Apply();

					if (Target.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Indignation
						if (Rand.NextDouble() < 0.2f)
							AddBuff(payload.Context.User, new DebuffStunned(WaitSeconds(3f)));

					if (Target.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Against All Odds
						if (Rand.NextDouble() < 0.2f)
							GeneratePrimaryResource(3f);

					if (Target.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Collateral Damage
						if (Rand.NextDouble() < 0.75f)
						{
							SpawnProxy(payload.Context.User.Position, WaitSeconds(1f)).PlayEffectGroup(193348);
							AttackPayload collateral = new AttackPayload(this);
							collateral.Targets = GetEnemiesInRadius(payload.Context.User.Position, 8f);
							collateral.Targets.Actors.Remove(payload.Context.User);

							collateral.AddWeaponDamage(1.01f, DamageType.Holy);
							collateral.AutomaticHitEffects = false; //no procs and looping
							collateral.Apply();
						}
				}
			}
		}

		[ImplementsPowerBuff(6)]
		class CastGroupBuff : PowerBuff
		{
			//grantee
			Actor Caster = null;
			private bool Retaliation = false;
			private bool Transgression = false;
			private bool Indignation = false;
			private bool Collateral = false;
			private bool _unityBonus = false;
			public CastGroupBuff(Actor caster, TickTimer timeout)
			{
				Caster = caster;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Caster.Attributes[GameAttribute.Rune_A, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Retaliation
					Retaliation = true;

				if (Caster.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Transgression
					Transgression = true;

				if (Caster.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Indignation
					Indignation = true;

				if (Caster.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution] > 0) //Collateral damage
					Collateral = true;

				if (Transgression) Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += 0.1f;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
					if ((Target as Player).IsInPvPWorld) return true;

				if (Caster != null && Caster.World != null)
					if ((Caster as Player).SkillSet.HasPassive(368899)) //Unity
					{
						_unityBonus = true;
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.05f;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.05f;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (Transgression) Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= 0.1f;

				if (_unityBonus)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= 0.05f;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= 0.05f;
				}
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage &&
					(payload as HitPayload).AutomaticHitEffects)
				{
					AttackPayload retaliate = new AttackPayload(this);
					retaliate.SetSingleTarget(payload.Context.User);

					if (Retaliation)
						retaliate.AddWeaponDamage(2.02f, DamageType.Fire);
					else
						retaliate.AddWeaponDamage(1.01f, DamageType.Holy);

					retaliate.AutomaticHitEffects = false;  //no procs and looping
					retaliate.Apply();

					if (Indignation && Rand.NextDouble() < 0.2f)
						AddBuff(payload.Context.User, new DebuffStunned(WaitSeconds(3f)));

					if (Collateral && Rand.NextDouble() < 0.75f)
					{
						SpawnProxy(payload.Context.User.Position, WaitSeconds(1f)).PlayEffectGroup(193348);
						AttackPayload collateral = new AttackPayload(this);
						collateral.Targets = GetEnemiesInRadius(payload.Context.User.Position, 8f);
						collateral.Targets.Actors.Remove(payload.Context.User);

						collateral.AddWeaponDamage(1.01f, DamageType.Holy);
						collateral.AutomaticHitEffects = false; //no procs and looping
						collateral.Apply();
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region MantraOfHealing
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.Mantras.MantraOfHealing)]
	public class MantraOfHealing : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost) * ((User as Player).SkillSet.HasPassive(156467) ? 0.5f : 1f)); //Chant Of Resonance

			AddBuff(User, new HealingShield(User.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.3f, WaitSeconds(ScriptFormula(12)), (User as Player).SkillSet.HasPassive(156492)));

			foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(0)).Actors)
			{
				AddBuff(ally, new HealingShield(ally.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.3f, WaitSeconds(ScriptFormula(12)), (User as Player).SkillSet.HasPassive(156492)));
			}

			(User as Player).AddAchievementCounter(74987243307546, 1);

			yield break;
		}
		[ImplementsPowerBuff(3)]
		class HealingShield : PowerBuff
		{
			float HPTreshold = 0;
			bool GuidingLight = false;

			//holyAuraRune_shield.efg
			public HealingShield(float hpThreshold, TickTimer timeout, bool guidingLight = false)
			{
				Timeout = timeout;
				HPTreshold = hpThreshold;
				GuidingLight = guidingLight;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (GuidingLight)       //Guiding Light passive
				{
					float missingHP = (User.Attributes[GameAttribute.Hitpoints_Max_Total] - User.Attributes[GameAttribute.Hitpoints_Cur]) / User.Attributes[GameAttribute.Hitpoints_Max_Total];
					if (!HasBuff<GuidingLightBuff>(User))
						AddBuff(User, new GuidingLightBuff(Math.Min(missingHP, 0.3f), TickTimer.WaitSeconds(World.Game, 10.0f)));
				}

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage && HPTreshold > 0)
				{
					(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
					HPTreshold -= (payload as HitPayload).TotalDamage;
					if (HPTreshold <= 0)
						User.World.BuffManager.RemoveBuff(User, this);
				}
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
	}

	[ImplementsPowerSNO(373154)]
	public class MantraOfHealingPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution); //MantraOfRetribution
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction); //MantraOfConviction
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfEvasion); //MantraOfEvasion
			AddBuff(User, new MantraOfHealingBuff());
			yield break;
		}

		[ImplementsPowerBuff(5)]
		class MantraOfHealingBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			private float Regen = 0f;
			private float LoH = 0f;
			private float _unityDamageBonus = 0f;

			public override void Init()
			{
				Regen = 10 + 0.0125f * (float)Math.Pow(User.Attributes[GameAttribute.Level], 3);
				LoH = 10 + 0.008f * (float)Math.Pow(User.Attributes[GameAttribute.Level], 3);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (User.Attributes[GameAttribute.Rune_A, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0)  //Sustenance
					Regen *= 2f;

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0) //Boon of Inspiration
					Target.Attributes[GameAttribute.Hitpoints_On_Hit] += LoH;

				if (User.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0) //Heavenly Body
					Target.Attributes[GameAttribute.Vitality_Bonus_Percent] += 0.1f;

				if (User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0) //Circular Breathing
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += ScriptFormula(7);

				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0) //Time of Need
					Target.Attributes[GameAttribute.Resistance_Percent_All] += 0.2f;

				Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += Regen;

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;

				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null) CheckTimer = WaitSeconds(1f);

				if (CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					foreach (Actor ally in GetAlliesInRadius(Target.Position, ScriptFormula(0)).Actors)
					{
						if (!HasBuff<CastGroupBuff>(ally))
							AddBuff(ally, new CastGroupBuff(User, Regen, LoH,
									User.Attributes[GameAttribute.Rune_B, 373143] > 0,
									User.Attributes[GameAttribute.Rune_C, 373143] > 0,
									User.Attributes[GameAttribute.Rune_E, 373143] > 0,
									WaitSeconds(1.1f)));
						else ally.World.BuffManager.GetFirstBuff<CastGroupBuff>(ally).Extend(60);
					}

					if (Target is Player)
						if ((Target as Player).IsInPvPWorld) return false;

					if ((Target as Player).SkillSet.HasPassive(368899) && _unityDamageBonus != (0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count())) //Unity
					{
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;

						_unityDamageBonus = 0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count();

						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += _unityDamageBonus;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0)
					Target.Attributes[GameAttribute.Hitpoints_On_Hit] -= LoH;

				if (User.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0)
					Target.Attributes[GameAttribute.Vitality_Bonus_Percent] -= 0.1f;

				if (User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= ScriptFormula(7);

				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing] > 0)
					Target.Attributes[GameAttribute.Resistance_Percent_All] -= 0.2f;

				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 2f;

				if (_unityDamageBonus > 0)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;
					_unityDamageBonus = 0f;
				}

				Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] -= Regen;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(6)]
		class CastGroupBuff : PowerBuff
		{
			//grantee
			Actor Caster = null;
			private bool _unityBonus = false;
			private bool BoonOfInspiration = false;
			private bool HeavenlyBody = false;
			private bool TimeOfNeed = false;
			private float LoH = 0f;
			private float Regen = 0f;

			public CastGroupBuff(Actor caster, float regen, float loh, bool boonOfInspiration, bool heavenlyBody, bool timeOfNeed, TickTimer timeout)
			{
				Caster = caster;
				Regen = regen;
				LoH = loh;
				BoonOfInspiration = boonOfInspiration;
				HeavenlyBody = heavenlyBody;
				TimeOfNeed = timeOfNeed;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (BoonOfInspiration) Target.Attributes[GameAttribute.Hitpoints_On_Hit] += LoH;
				if (HeavenlyBody) Target.Attributes[GameAttribute.Vitality_Bonus_Percent] += 0.1f;
				if (TimeOfNeed) Target.Attributes[GameAttribute.Resistance_Percent_All] += 0.2f;

				Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += Regen;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
					if ((Target as Player).IsInPvPWorld) return true;

				if (Caster != null && Caster.World != null)
					if ((Caster as Player).SkillSet.HasPassive(368899)) //Unity
					{
						_unityBonus = true;
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.05f;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.05f;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (BoonOfInspiration) Target.Attributes[GameAttribute.Hitpoints_On_Hit] -= LoH;
				if (HeavenlyBody) Target.Attributes[GameAttribute.Vitality_Bonus_Percent] -= 0.1f;
				if (TimeOfNeed) Target.Attributes[GameAttribute.Resistance_Percent_All] -= 0.2f;

				Target.Attributes[GameAttribute.Hitpoints_Regen_Bonus_Percent] -= Regen;

				if (_unityBonus)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= 0.05f;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= 0.05f;
				}
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region MantraOfConviction
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.Mantras.MantraOfConviction)]
	public class MantraOfConviction : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost) * ((User as Player).SkillSet.HasPassive(156467) ? 0.5f : 1f)); //Chant Of Resonance
			AddBuff(User, new ActiveBuff(WaitSeconds(3f)));

			(User as Player).AddAchievementCounter(74987243307546, 1);

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ActiveBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			float RedAmount = 0f;
			//AuraBuff
			public ActiveBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override void Init()
			{
				base.Init();
				RedAmount = Rune_A > 0 ? 0.16f : 0.1f;      //Overawe
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null) _damageTimer = WaitSeconds(_damageRate);

				if (_damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					foreach (Actor Enemy in GetEnemiesInRadius(User.Position, 30f).Actors)
					{
						if (!HasBuff<ActiveDeBuff>(Enemy))
							AddBuff(Enemy, new ActiveDeBuff(RedAmount, WaitSeconds(1.1f)));
						else Enemy.World.BuffManager.GetFirstBuff<ActiveDeBuff>(Enemy).Extend(60);
					}
				}

				return false;
			}
		}

		[ImplementsPowerBuff(1)]
		public class ActiveDeBuff : PowerBuff   //done in HitPayload
		{
			public float RedAmount = 0f;

			public ActiveDeBuff(float redAmount, TickTimer timeout)
			{
				RedAmount = redAmount;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				//Done in HitPayload
				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}

	[ImplementsPowerSNO(375089)]
	public class MantraOfConvictionPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfRetribution); //MantraOfRetribution
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfHealing); //MantraOfHealing
			RemoveBuffs(User, SkillsSystem.Skills.Monk.Mantras.MantraOfEvasion); //MantraOfEvasion
			AddBuff(User, new MantraOfConvictionBuff());
			yield break;
		}

		[ImplementsPowerBuff(5)]
		class MantraOfConvictionBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			private float _unityDamageBonus = 0f;
			private float RedAmount = 0f;
			private bool Annihilation = false;

			public override void Init()
			{
				base.Init();
				RedAmount = User.Attributes[GameAttribute.Rune_A, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0 ? 0.16f : 0.1f;
				Annihilation = User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				if ((User as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
				{
					User.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;
					User.Attributes.BroadcastChangedIfRevealed();
				}
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null) _damageTimer = WaitSeconds(_damageRate);

				if (_damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					foreach (Actor Enemy in GetEnemiesInRadius(User.Position, 30f).Actors)
					{
						if (!HasBuff<DeBuff>(Enemy))
							AddBuff(Enemy, new DeBuff(RedAmount, Annihilation, WaitSeconds(1.1f)));
						else Enemy.World.BuffManager.GetFirstBuff<DeBuff>(Enemy).Extend(60);

						if (User.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0) //Dishearten
						{
							if (!HasBuff<DisheartenDebuff>(Enemy))
								AddBuff(Enemy, new DisheartenDebuff(0.8f, WaitSeconds(1f)));
							else Enemy.World.BuffManager.GetFirstBuff<DisheartenDebuff>(Enemy).Extend(60);
						}
					}

					if (Target is Player)
						if ((Target as Player).IsInPvPWorld) return false;

					if ((Target as Player).SkillSet.HasPassive(368899) && _unityDamageBonus != (0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count())) //Unity
					{
						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;

						_unityDamageBonus = 0.05f * Target.GetActorsInRange<Player>(ScriptFormula(0)).Count;

						Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += _unityDamageBonus;
						Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += _unityDamageBonus;
						Target.Attributes.BroadcastChangedIfRevealed();
					}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				if ((Target as Player).SkillSet.HasPassive(156467)) //ChantOfResonance (monk)
				{
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 2f;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				if (_unityDamageBonus > 0)
				{
					Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= _unityDamageBonus;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= _unityDamageBonus;
					_unityDamageBonus = 0f;
				}
			}
		}

		[ImplementsPowerBuff(6)]
		public class DeBuff : PowerBuff
		{

			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public float RedAmount = 0f;
			private bool Annihilation = false;

			public DeBuff(float redAmount, bool annihilation, TickTimer timeout)
			{
				RedAmount = redAmount;
				Annihilation = annihilation;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0) //Intimidation
					Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] += ScriptFormula(10);

				//base effect done in HitPayload
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0) //Submission
				{
					if (_damageTimer == null || _damageTimer.TimedOut)
					{
						_damageTimer = WaitSeconds(_damageRate);

						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(0.38f, DamageType.Holy);
						attack.AutomaticHitEffects = false;
						attack.Apply();
					}
				}

				return false;
			}
			public override void Remove()
			{
				base.Remove();
				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.Monk.Mantras.MantraOfConviction] > 0)
					Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] -= ScriptFormula(10);

				Target.Attributes.BroadcastChangedIfRevealed();
			}

			public override void OnPayload(Payload payload)
			{
				//Rune D Annihilation
				if (Annihilation && payload.Target == Target && payload is DeathPayload)
					foreach (var plr in Target.GetPlayersInRange(30f))
					{
						if (!HasBuff<AnnihilationBuff>(plr))
							AddBuff(plr, new AnnihilationBuff(0.3f, WaitSeconds(3f)));
					}
			}
		}

		[ImplementsPowerBuff(1)]
		public class DisheartenDebuff : PowerBuff
		{
			public float Percentage;

			public DisheartenDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
					return false;

				Target.WalkSpeed *= (1f - Percentage);
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
				{
					if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
						if (FastRandom.Instance.Next(100) < 30)
							(Target as Player).AddPercentageHP(20);
				}
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.WalkSpeed /= (1f - Percentage);
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(3)]
		public class AnnihilationBuff : PowerBuff
		{
			public float Percentage;

			public AnnihilationBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Rune B not complete
	#region Serenity
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.Serenity)]
	public class Serenity : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			RemoveBuffs(User, 101000); // DebuffStunned.pow
			RemoveBuffs(User, 101002); // DebuffFeared.pow
			RemoveBuffs(User, 101003); // DebuffRooted.pow
			RemoveBuffs(User, 100971); // DebuffSlowed.pow

			AddBuff(User, new SerenityBuff(WaitSeconds(ScriptFormula(0))));

			if (Rune_D > 0)         //Tranquility
				foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(12)).Actors)
				{
					if (!HasBuff<SerenityAlliesBuff>(ally))
						AddBuff(ally, new SerenityAlliesBuff(User, ally.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.4f, WaitSeconds(ScriptFormula(11)), true, (User as Player).SkillSet.HasPassive(156492)));
				}

			yield break;
		}
		[ImplementsPowerBuff(0)]
		class SerenityBuff : PowerBuff
		{
			TickTimer _damageTimer = null;

			public SerenityBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0)     //Peaceful Repose
					(Target as Player).AddPercentageHP(30, (User as Player).SkillSet.HasPassive(156492));

				if (Rune_B > 0)     //Instant Karma
				{
				}

				Target.Attributes[GameAttribute.Gethit_Immune] = true;
				Target.Attributes[GameAttribute.Immunity] = true;
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (Rune_E > 0)     //Unwelcome Disturbance
				{
					if (_damageTimer == null || _damageTimer.TimedOut)
					{
						_damageTimer = WaitSeconds(1f);

						User.PlayEffectGroup(143941);
						WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(5)), ScriptFormula(4), DamageType.Physical);
					}
				}

				return false;
			}
			public override void Remove()
			{
				base.Remove();
				Target.PlayEffectGroup(RuneSelect(129330, 143242, 143243, 143244, 143245, 143246));

				if (Rune_B > 0)
				{
				}

				Target.Attributes[GameAttribute.Gethit_Immune] = false;
				Target.Attributes[GameAttribute.Immunity] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1)]
		class SerenityAlliesBuff : PowerBuff
		{
			Actor Caster = null;
			private float HPTreshold = 0f;
			private bool Redirect = false;
			bool GuidingLight = false;
			public SerenityAlliesBuff(Actor caster, float hpTreshold, TickTimer timeout, bool redirect = false, bool guidingLight = false)
			{
				Caster = caster;
				HPTreshold = hpTreshold;
				Redirect = redirect;
				GuidingLight = guidingLight;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				RemoveBuffs(Target, 101000); // DebuffStunned.pow
				RemoveBuffs(Target, 101002); // DebuffFeared.pow
				RemoveBuffs(Target, 101003); // DebuffRooted.pow
				RemoveBuffs(Target, 100971); // DebuffSlowed.pow

				if (GuidingLight)       //Guiding Light passive
				{
					float missingHP = (Target.Attributes[GameAttribute.Hitpoints_Max_Total] - User.Attributes[GameAttribute.Hitpoints_Cur]) / Target.Attributes[GameAttribute.Hitpoints_Max_Total];
					if (!HasBuff<GuidingLightBuff>(Target))
						AddBuff(Target, new GuidingLightBuff(Math.Min(missingHP, 0.3f), TickTimer.WaitSeconds(World.Game, 10.0f)));
				}

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (!Redirect) return;

				//Tranquility redirecting damage to monk
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage && HPTreshold > 0)
				{
					if (!(payload as HitPayload).IsDodged && !(payload as HitPayload).Blocked)
					{
						if (Caster != null)
						{
							AttackPayload redirect = new AttackPayload(this);
							redirect.SetSingleTarget(Caster);
							redirect.AddDamage(Math.Min((payload as HitPayload).TotalDamage, HPTreshold), 0f, DamageType.Physical);
							redirect.AutomaticHitEffects = false;
							redirect.Apply();
						}

						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= (payload as HitPayload).TotalDamage;
						if (HPTreshold <= 0)
							Target.World.BuffManager.RemoveBuff(Target, this);
					}
				}
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region InnerSanctuary
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.InnerSanctuary)]
	public class InnerSanctuary : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_A > 0)         //Intervene
			{
				PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f));

				if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					User.PlayEffectGroup(170232);
					yield return (WaitSeconds(0.2f));
					User.Teleport(TargetPosition);
					User.PlayEffectGroup(170232);
					yield return (WaitSeconds(0.3f));
				}

				AddBuff(User, new ShieldBuff(User.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.4f, WaitSeconds(ScriptFormula(14)), (User as Player).SkillSet.HasPassive(156492)));
				foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(1)).Actors)
				{
					AddBuff(ally, new ShieldBuff(ally.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.4f, WaitSeconds(ScriptFormula(14)), (User as Player).SkillSet.HasPassive(156492)));
				}
			}

			//var Sanctuary = SpawnEffect(RuneSelect(98557, 98823, 149848, 142312, 98559, 142305), GroundSpot.Position, 0, WaitSeconds(ScriptFormula(0)));
			var Sanctuary = new EffectActor(
				this,
				RuneSelect(
					ActorSno._x1_monk_innersanctuary_proxy,
					ActorSno._x1_monk_innersanctuary_proxy,
					ActorSno._monk_innersanctuaryrune_duration_proxy,
					ActorSno._x1_monk_innersanctuaryrune_protect_proxy,
					ActorSno._x1_monk_innersanctuaryrune_healing_proxy,
					ActorSno._monk_innersanctuaryrune_presanctified_proxy
				),
				User.Position
			);
			Sanctuary.Timeout = WaitSeconds(ScriptFormula(0));
			Sanctuary.Scale = 1f;
			Sanctuary.Spawn();
			Sanctuary.UpdateDelay = 1f;
			Sanctuary.OnUpdate = () =>
			{
				foreach (var actor in GetEnemiesInRadius(Sanctuary.Position, ScriptFormula(1)).Actors)
				{
					if (Rune_B > 0 && !HasBuff<DirectedKnockbackBuff>(actor))    //Sanctified Ground
						AddBuff(actor, new DirectedKnockbackBuff(Sanctuary.Position, 10f));

					if (Rune_E > 0)     //Forbidden Palace
					{
						if (!HasBuff<InnerDebuff>(actor))
							AddBuff(actor, new InnerDebuff(ScriptFormula(22), ScriptFormula(23), WaitSeconds(1f)));
						else actor.World.BuffManager.GetFirstBuff<InnerDebuff>(actor).Extend(60);
					}
				}

				var allies = GetAlliesInRadius(Sanctuary.Position, 11f).Actors;
				if (PowerMath.Distance2D(Sanctuary.Position, User.Position) <= 11f) allies.Add(User);

				foreach (var ally in allies)
				{
					if (!HasBuff<SanctuaryBuff>(ally))
						AddBuff(ally, new SanctuaryBuff(WaitSeconds(1f)));
					else ally.World.BuffManager.GetFirstBuff<SanctuaryBuff>(ally).Extend(60);

					if (Rune_D > 0 && ally is Player)   //Safe Heaven
						(ally as Player).AddPercentageHP(15, (User as Player).SkillSet.HasPassive(156492));
				}
			};
			//outer proxy
			//SpawnEffect(RuneSelect(142719, 142851, 149849, 142788, 142737, 142845), GroundSpot.Position, 0, WaitSeconds(ScriptFormula(7)));
			yield break;
		}
		[ImplementsPowerBuff(6)]
		class ShieldBuff : PowerBuff
		{
			private float HPTreshold = 0f;
			bool GuidingLight = false;
			public ShieldBuff(float hpTreshold, TickTimer timeout, bool guidingLight = false)
			{
				HPTreshold = hpTreshold;
				GuidingLight = guidingLight;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (GuidingLight)       //Guiding Light passive
				{
					float missingHP = (User.Attributes[GameAttribute.Hitpoints_Max_Total] - User.Attributes[GameAttribute.Hitpoints_Cur]) / User.Attributes[GameAttribute.Hitpoints_Max_Total];
					if (!HasBuff<GuidingLightBuff>(User))
						AddBuff(User, new GuidingLightBuff(Math.Min(missingHP, 0.3f), TickTimer.WaitSeconds(World.Game, 10.0f)));
				}

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage && HPTreshold > 0)
					if (!(payload as HitPayload).IsDodged && !(payload as HitPayload).Blocked)
					{
						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= (payload as HitPayload).TotalDamage;
						if (HPTreshold <= 0)
							Target.World.BuffManager.RemoveBuff(Target, this);
					}
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(4)]
		public class InnerDebuff : PowerBuff    //increased damage taken is done in HitPayload
		{
			float SlowPercentage = 0f;
			public float DamagePercentage = 0f;
			public InnerDebuff(float damagePercentage, float slowPercentage, TickTimer timeout)
			{
				DamagePercentage = damagePercentage;
				SlowPercentage = slowPercentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.WalkSpeed *= (1f - SlowPercentage);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= SlowPercentage;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += SlowPercentage;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
				{
					if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
						if (FastRandom.Instance.Next(100) < 30)
							(Target as Player).AddPercentageHP(20);
				}

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.WalkSpeed /= (1f - SlowPercentage);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += SlowPercentage;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= SlowPercentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1)]
		public class SanctuaryBuff : PowerBuff
		{
			public SanctuaryBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage)
					(payload as HitPayload).TotalDamage *= 0.45f;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region MysticAlly
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.MysticAlly)]
	public class MysticAlly : Skill
	{
		TickTimer BoulderTimer = null;
		public override IEnumerable<TickTimer> Main()
		{
			if (!HasBuff<MysticAllyPassive.MysticAllyBuff>(User)) yield break;

			var petAlly = User.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(User).ally;
			if (petAlly == null || petAlly.Dead) yield break;

			StartCooldown(Rune_E > 0 ? 50f : 30f);

			if (Rune_A > 0)     //Fire Ally
			{
				petAlly.PlayEffectGroup(212532);
				WeaponDamage(GetEnemiesInRadius(petAlly.Position, 10f), 3f, DamageType.Fire);
				yield break;
			}

			if (Rune_B > 0)     //Water Ally
			{
				var targets = GetEnemiesInRadius(petAlly.Position, 20f, 7).Actors;
				if (!targets.Any()) yield break;

				foreach (var target in targets)
				{
					if (target == null || target.World == null) continue;
					SpawnProxy(target.Position, WaitSeconds(0.3f)).PlayEffectGroup(171013);
					WeaponDamage(target, 3.5f, DamageType.Cold);
				}
				yield break;
			}

			if (Rune_C > 0)     //Earth Ally
			{
				Actor boulderTarget = GetEnemiesInRadius(petAlly.Position, 20f).GetClosestTo(petAlly.Position);
				if (boulderTarget == null) yield break;

				var startPosition = petAlly.Position;
				var boulder = new Projectile(this, ActorSno._x1_projectile_mystically_runec_boulder, petAlly.Position);
				boulder.OnUpdate = () =>
				{
					if (PowerMath.Distance2D(boulder.Position, startPosition + new Vector3D(0, 0, 5f)) > 30f)
					{
						boulder.PlayEffectGroup(190831);
						boulder.Destroy();
						return;
					}

					if (BoulderTimer == null || BoulderTimer.TimedOut)
					{
						BoulderTimer = WaitSeconds(0.8f);

						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(boulder.Position, 6f);
						attack.AddWeaponDamage(3.5f, DamageType.Physical);
						attack.OnHit = hitPayload =>
						{
							if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DirectedKnockbackBuff(boulder.Position, 10f));
						};
						attack.Apply();
					}
				};
				boulder.Launch(boulderTarget.Position, 0.4f);
				yield break;
			}

			if (Rune_D > 0)     //Air Ally
				GeneratePrimaryResource(ScriptFormula(6));

			if (Rune_E > 0)     //Enduring Ally
			{
				if (HasBuff<MysticAllyPassive.MysticAllyBuff>(User))
					if (User.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(User).ally != null)
					{
						User.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(User).ally.Kill(this);
						User.World.BuffManager.GetFirstBuff<MysticAllyPassive.MysticAllyBuff>(User).ally.Dead = true;
						(User as Player).AddPercentageHP(100);
					}
			}

			yield break;
		}
	}

	[ImplementsPowerSNO(362118)]
	public class MysticAllyPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new MysticAllyBuff());
			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class MysticAllyBuff : PowerBuff
		{
			private static readonly ActorSno[] maleAllys = new ActorSno[]
			{
				ActorSno._monk_male_mystically,
				ActorSno._monk_male_mystically_crimson,
				ActorSno._monk_male_mystically_alabaster,
				ActorSno._monk_male_mystically_obsidian,
				ActorSno._monk_male_mystically_golden,
				ActorSno._monk_male_mystically_indigo
			};
			private static readonly ActorSno[] femaleAllys = new ActorSno[]
			{
				ActorSno._monk_female_mystically,
				ActorSno._monk_female_mystically_crimson,
				ActorSno._monk_female_mystically_alabaster,
				ActorSno._monk_female_mystically_obsidian,
				ActorSno._monk_female_mystically_golden,
				ActorSno._monk_female_mystically_indigo
			};
			public MysticAllyMinion ally = null;
			private ActorSno AllyId = ActorSno.__NONE;
			public bool WaterAlly = false;
			float RegenValue = 0f;

			private float LifeRegen(int level)
			{
				return (10 + 0.8f * (float)Math.Pow(level, 2));
			}

			public override void Init()
			{
				base.Init();
				RegenValue = LifeRegen(User.Attributes[GameAttribute.Level]);
			}
			public override bool Apply()
			{
				if (!base.Apply()) return false;

				if (ally != null) return false;
				if (User.World == null) return false;

				int gender = (User as Player).Toon.Gender;

				var allys = gender == 2 ? femaleAllys : maleAllys;
				AllyId = allys[0];
				if (User.Attributes[GameAttribute.Rune_A, 0x00058676] > 0)  //Crimson
					AllyId = allys[1];
				if (User.Attributes[GameAttribute.Rune_B, 0x00058676] > 0)  //Alabaster
					AllyId = allys[2];
				if (User.Attributes[GameAttribute.Rune_C, 0x00058676] > 0)  //Obsidian
					AllyId = allys[3];
				if (User.Attributes[GameAttribute.Rune_D, 0x00058676] > 0)  //Golden
					AllyId = allys[4];
				if (User.Attributes[GameAttribute.Rune_E, 0x00058676] > 0)  //Indigo
					AllyId = allys[5];

				ally = new MysticAllyMinion(World, this, AllyId);
				ally.Brain.DeActivate();
				ally.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				ally.Attributes[GameAttribute.Untargetable] = true;
				ally.EnterWorld(ally.Position);
				ally.PlayActionAnimation(AnimationSno.mystically_female_spawn2);

				(ally as Minion).Brain.Activate();
				ally.Attributes[GameAttribute.Untargetable] = false;
				ally.Attributes.BroadcastChangedIfRevealed();

				if (User.Attributes[GameAttribute.Rune_A, 0x00058676] > 0)  //Fire Ally
				{
					User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.1f;
					User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.1f;
				}

				if (User.Attributes[GameAttribute.Rune_B, 0x00058676] > 0)  //Water Ally
					WaterAlly = true;   //done in HitPayload

				if (User.Attributes[GameAttribute.Rune_C, 0x00058676] > 0)  //Earth Ally
					User.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] += 0.2f;

				if (User.Attributes[GameAttribute.Rune_D, 0x00058676] > 0)  //Air Ally
					User.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;

				if (User.Attributes[GameAttribute.Rune_E, 0x00058676] > 0)  //Enduring Ally
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += RegenValue;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (User.Attributes[GameAttribute.Rune_A, 0x00058676] > 0)
				{
					User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= 0.1f;
					User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= 0.1f;
				}

				if (User.Attributes[GameAttribute.Rune_C, 0x00058676] > 0)
					User.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] -= 0.2f;

				if (User.Attributes[GameAttribute.Rune_D, 0x00058676] > 0)
					User.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 2f;

				if (User.Attributes[GameAttribute.Rune_E, 0x00058676] > 0)
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] -= RegenValue;

				User.Attributes.BroadcastChangedIfRevealed();

				if (ally != null)
				{
					ally.Destroy();
					ally = null;
				}
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (User.World.Game.TickCounter % 300 == 0)
				{
					if (ally != null && ally.Dead)
					{
						ally = new MysticAllyMinion(World, this, AllyId);
						ally.Brain.DeActivate();
						ally.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
						ally.Attributes[GameAttribute.Untargetable] = true;
						ally.EnterWorld(ally.Position);
						ally.PlayActionAnimation(AnimationSno.mystically_female_spawn2);

						(ally as Minion).Brain.Activate();
						ally.Attributes[GameAttribute.Untargetable] = false;
						ally.Attributes.BroadcastChangedIfRevealed();
					}
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region CycloneStrike
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritSpenders.CycloneStrike)]
	public class CycloneStrike : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			if (Rune_C > 0)     //Soothing Breeze
			{
				(User as Player).AddPercentageHP(30, (User as Player).SkillSet.HasPassive(156492));
				foreach (Actor Ally in GetAlliesInRadius(User.Position, ScriptFormula(19)).Actors)
				{
					if (Ally is Player)
						(Ally as Player).AddPercentageHP(30, (User as Player).SkillSet.HasPassive(156492));
				}
			}

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(2), (int)ScriptFormula(5));
			attack.OnHit = hit =>
			{
				Knockback(hit.Target, -25f, ScriptFormula(1), ScriptFormula(29));
			};
			attack.Apply();

			yield return WaitSeconds(0.5f);
			User.PlayEffectGroup(224247);
			WeaponDamage(GetEnemiesInRadius(User.Position, Rune_B > 0 ? 20f : 10f), ScriptFormula(10), Rune_A > 0 ? DamageType.Fire : (Rune_D > 0 ? DamageType.Lightning : (Rune_E > 0 ? DamageType.Cold : DamageType.Holy)));

			if (Rune_E > 0 && !HasBuff<CycloneDodgeBuff>(User))     //Wall of Wind
				AddBuff(User, new CycloneDodgeBuff(WaitSeconds(ScriptFormula(27))));

			yield break;
		}
		[ImplementsPowerBuff(0)]
		class CycloneDodgeBuff : PowerBuff
		{
			public CycloneDodgeBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(28);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(28);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Epiphany
	[ImplementsPowerSNO(SkillsSystem.Skills.Monk.SpiritGenerator.Epiphany)]
	public class Epiphany : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (!HasBuff<EpiphanyBuff>(User))
				AddBuff(User, new EpiphanyBuff(WaitSeconds(ScriptFormula(0))));

			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class EpiphanyBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			public bool DesertShroud = false;
			private float KillStreakBonus = 0f;
			public EpiphanyBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0) DesertShroud = true;    //Desert Shroud, done in HitPayload

				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017713] = 20f;     //Deadly Reach
				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017837] = 20f;     //Crippling Wave
				if (User.Attributes[GameAttribute.Rune_A, 0x00017B56] <= 0)                     //Way of the Hundred Fists				
					User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017B56] = 15f;

				if (User.Attributes[GameAttribute.Rune_B, 0x0001B43C] <= 0 &&
					User.Attributes[GameAttribute.Rune_C, 0x0001B43C] <= 0)
					User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x0001B43C] = 20f; //Lashing Tail Kick
				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017C30] = 20f;     //Exploding Palm

				User.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 20f;
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null || CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					if (Rune_B > 0)     //Soothing Mist
					{
						User.PlayEffectGroup(358055);
						(User as Player).AddPercentageHP(15);
						foreach (var ally in GetAlliesInRadius(User.Position, 30f).Actors)
						{
							if (ally is Player)
								(ally as Player).AddPercentageHP(15);
						}
					}

					if (Rune_D > 0)     //Inner Fire
					{
						User.PlayEffectGroup(212532);
						AttackPayload burn = new AttackPayload(this);
						burn.Targets = GetEnemiesInRadius(User.Position, 15f);
						burn.AddWeaponDamage(3.53f, DamageType.Fire);
						burn.AutomaticHitEffects = false;
						burn.Apply();
					}

					if (Rune_E > 0)     //Ascendance
					{
						var target = GetEnemiesInRadius(User.Position, 10f).GetClosestTo(User.Position);
						if (target != null)
							if (!HasBuff<DebuffStunned>(target))
							{
								target.PlayEffectGroup(312568);
								AddBuff(target, new DebuffStunned(WaitSeconds(1.5f)));
							}
					}
				}

				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (Rune_C > 0)     //WindWalker
					if (payload.Context.User == User && payload is DeathPayload)
					{
						KillStreakBonus += 0.03f;
						User.Attributes[GameAttribute.Attacks_Per_Second_Percent] += 0.03f;
						User.Attributes.BroadcastChangedIfRevealed();
					}
			}

			public override void Remove()
			{
				base.Remove();

				if (KillStreakBonus > 0f)
				{
					User.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= KillStreakBonus;
					KillStreakBonus = 0f;
				}

				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017713] = 0f;      //Deadly Reach
				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017837] = 0f;      //Crippling Wave
				if (User.Attributes[GameAttribute.Rune_A, 0x00017B56] <= 0)                     //Way of the Hundred Fists
					User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017B56] = 0f;

				if (User.Attributes[GameAttribute.Rune_B, 0x0001B43C] <= 0 &&
					User.Attributes[GameAttribute.Rune_C, 0x0001B43C] <= 0)
					User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x0001B43C] = 0f;  //Lashing Tail Kick
				User.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017C30] = 0f;      //Exploding Palm				

				User.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] -= 20f;
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}

	}
	#endregion
}
