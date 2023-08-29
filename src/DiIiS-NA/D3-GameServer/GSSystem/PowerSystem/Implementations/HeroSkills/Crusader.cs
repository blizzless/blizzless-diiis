using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	//Complete
	#region Punish
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.Punish)]
	public class CrusaderPunish : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			var damageType = DamageType.Physical;
			if (Rune_A > 0) damageType = DamageType.Holy;
			if (Rune_D > 0) damageType = DamageType.Fire;
			if (Rune_E > 0) damageType = DamageType.Lightning;
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetBestMeleeEnemy();
			attack.AddWeaponDamage(ScriptFormula(0), damageType);
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				//User.PlayEffectGroup(RuneSelect(289607, 348581, 345432, 345433, 345912, 348580));	//effects are looping
				AddBuff(User, new BlockBuff());     //buff slot 0
			};
			attack.Apply();

			if (hitAnything)
			{
				GeneratePrimaryResource(ScriptFormula(18));
				if ((User as Player).SkillSet.HasPassive(356147))       //Righteousness passive
					GeneratePrimaryResource(3f);
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class BlockBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttributes.Block_Chance] += ScriptFormula(3);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			private float LifeRegen(int level)      //SF10 is bugged, use our own formula
			{
				return (10 + 0.04f * (float)Math.Pow(level, 3));
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == User && (payload as HitPayload).Blocked)
				{
					//User.PlayEffectGroup(311051);		//"pulse"?
					if (Rune_A > 0)     //Retaliate
						if (PowerMath.Distance2D(User.Position, payload.Context.User.Position) < 10f)
						{
							WeaponDamage(payload.Context.User, ScriptFormula(6), DamageType.Holy);
						}
					if ((Rune_B > 0) && !HasBuff<CeleritySpeedBuff>(User))      //Celerity (buff slot 3)
					{
						AddBuff(User, new CeleritySpeedBuff(ScriptFormula(8), WaitSeconds(ScriptFormula(9))));
					}
					if ((Rune_C > 0) && !HasBuff<RebirthHitPointRegenBuff>(User))       //Rebirth (buff slot 1)
					{
						AddBuff(User, new RebirthHitPointRegenBuff(LifeRegen(User.Attributes[GameAttributes.Level]), WaitSeconds(ScriptFormula(11))));
					}
					if (Rune_D > 0)     //Roar
					{
						SpawnEffect(ActorSno._x1_crusader_punish_explosion_nova, User.Position);
						WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(14)), ScriptFormula(16), DamageType.Fire);
					}
					if ((Rune_E > 0) && !HasBuff<FuryChCBuff>(User))        //Fury (buff slot 2)
					{
						AddBuff(User, new FuryChCBuff(ScriptFormula(17), WaitSeconds(10f)));
					}
				}
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttributes.Block_Chance] -= ScriptFormula(3);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(2)]
		public class FuryChCBuff : PowerBuff        //Fury, done in attackPayload
		{
			public float Percentage;

			public FuryChCBuff(float percentage, TickTimer timeout)
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

			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region Slash
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.Slash)]
	public class CrusaderSlash : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			var DmgType = DamageType.Fire;
			if (Rune_A > 0) DmgType = DamageType.Holy;      //Zeal
			if (Rune_B > 0) DmgType = DamageType.Lightning;     //Electrify
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, 12f, Rune_D > 0 ? 120f : 90f); //Carve
			if (Rune_C > 0) attack.ChcBonus = ScriptFormula(14);    //Crush
			attack.AddWeaponDamage(ScriptFormula(0), DmgType);
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				if (Rune_A > 0) AddBuff(User, new ZealBuff());      //Zeal (buff slot 0)
				if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(8))     //Electrify
					AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(9))));
				if (Rune_E > 0) AddBuff(User, new GuardBuff());     //Guard	(buff slot 1)
			};
			attack.Apply();

			if (hitAnything)
			{
				GeneratePrimaryResource(ScriptFormula(18));
				if ((User as Player).SkillSet.HasPassive(356147))       //Righteousness passive
					GeneratePrimaryResource(3f);
			}

			yield break;
		}

		[ImplementsPowerBuff(0, true)]
		public class ZealBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(6));
				MaxStackCount = (int)ScriptFormula(5);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Attacks_Per_Second_Bonus] -= StackCount * ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Attacks_Per_Second_Bonus] += ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1, true)]
		public class GuardBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(6));
				MaxStackCount = (int)ScriptFormula(16);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= StackCount * ScriptFormula(15);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] += ScriptFormula(15);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Smite
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.Smite)]
	public class CrusaderSmite : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;

			User.PlayEffectGroup(RuneSelect(290264, 335909, 335910, 335911, 335912, 345216));
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInBeamDirection(User.Position, TargetPosition, EvalTag(PowerKeys.AttackRadius), 5f);     //Rune E (surge) here
			attack.AddWeaponDamage(ScriptFormula(0), DamageType.Holy);
			attack.OnHit = hitPayload =>
			{
				hitPayload.Target.PlayEffectGroup(RuneSelect(288396, 338180, 338180, 337165, 342996, 345217));
				hitAnything = true;
				if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(26))    //Shackle (buff slot 2)
					if (!HasBuff<DebuffRooted>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffRooted(WaitSeconds(ScriptFormula(1))));

				if (Rune_D > 0)     //Reaping (buff slot 1)
					AddBuff(User, new ReapingBuff());
			};
			attack.Apply();

			if (hitAnything)
			{
				GeneratePrimaryResource(ScriptFormula(30));
				if ((User as Player).SkillSet.HasPassive(356147))       //Righteousness passive
					GeneratePrimaryResource(3f);
			}

			yield return WaitSeconds(0.2f);

			var additionalTargets = GetEnemiesInRadius(User.Position, ScriptFormula(3)).Actors
				.OrderBy(actor => PowerMath.Distance2D(actor.Position, User.Position))
				.Take((int)ScriptFormula(4))
				.ToArray();

			foreach (var target in additionalTargets)
			{
				target.PlayEffectGroup(RuneSelect(336292, 338256, 338255, 338254, 343105, 336292));
				AttackPayload additionalAttack = new AttackPayload(this);
				additionalAttack.SetSingleTarget(target);
				additionalAttack.AddWeaponDamage(ScriptFormula(9), DamageType.Holy);
				additionalAttack.Apply();
				if (Rune_A > 0)         //Shared fates (buff slot 0)
					foreach (var otherTarget in additionalTargets)
						if (otherTarget != target)
							if (PowerMath.Distance2D(otherTarget.Position, target.Position) > 10f)      //for now
								if (!HasBuff<DebuffStunned>(otherTarget))
									AddBuff(otherTarget, new DebuffStunned(WaitSeconds(ScriptFormula(32))));

				if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(26))    //Shackle (buff slot 2)
					if (!HasBuff<DebuffRooted>(target))
						AddBuff(target, new DebuffRooted(WaitSeconds(ScriptFormula(1))));

				if (Rune_C > 0)         //Shatter
				{
					AttackPayload explosionAttack = new AttackPayload(this);
					explosionAttack.Targets = GetEnemiesInRadius(target.Position, ScriptFormula(23));
					explosionAttack.AddWeaponDamage(ScriptFormula(6), DamageType.Holy);
					explosionAttack.Apply();
				}
			}
		}

		[ImplementsPowerBuff(1, true)]
		public class ReapingBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(16));
				MaxStackCount = (int)ScriptFormula(17);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			private float LifeRegen(int level) 
			{
				return (10 + 0.02f * (float)Math.Pow(level, 3));
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] -= StackCount * LifeRegen(User.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] += LifeRegen(User.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//rune C hammer of pursuit not complete
	#region Justice
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.Justice)]
	public class CrusaderJustice : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_C > 0) yield break;

			bool hitAnything = false;
			var proj = new Projectile(
				this,
				RuneSelect(
					ActorSno._x1_crusader_justice_projectile, 
					ActorSno._x1_crusader_justice_projectile_exploding, 
					ActorSno._x1_crusader_justice_projectile, 
					ActorSno._x1_crusader_justice_projectile_seeking, 
					ActorSno._x1_crusader_justice_projectile_exploding, 
					ActorSno._x1_crusader_justice_projectile_holybolt
				),
				User.Position
			);
			proj.Position.Z += 2f;  // fix height
			proj.OnCollision = (hit) =>
			{
				if (Rune_D > 0) hit.PlayEffectGroup(345210);
				else hit.PlayEffectGroup(325529);
				if (!hitAnything)
				{
					GeneratePrimaryResource(ScriptFormula(0));
					if ((User as Player).SkillSet.HasPassive(356147))       //Righteousness passive
						GeneratePrimaryResource(3f);
				}
				hitAnything = true;

				AttackPayload attack = new AttackPayload(this);
				if (Rune_D > 0)         //Burst
				{
					attack.Targets = GetEnemiesInRadius(proj.Position, ScriptFormula(10));
					attack.AddWeaponDamage(ScriptFormula(25), DamageType.Lightning);
				}
				else
				{
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage(ScriptFormula(22), DamageType.Holy);
				}

				attack.OnHit = hitPayload =>
				{
					if (Rune_A > 0) AddBuff(User, new SwordSpeedBuff());            //Sword of justice (buff slot 0)
					if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(12))        //Crack
					{
						var additional_targets = GetEnemiesInRadius(hitPayload.Target.Position, 10f).Actors;
						additional_targets.Remove(hitPayload.Target);
						foreach (var target in additional_targets.Take((int)ScriptFormula(8)))
						{
							var secondaryProj = new Projectile(this, ActorSno._x1_crusader_justice_projectile_split, hitPayload.Target.Position);
							secondaryProj.Position.Z += 5f;
							secondaryProj.OnCollision = secondaryHit =>
							{
								if (secondaryHit == hitPayload.Target) return;
								secondaryHit.PlayEffectGroup(325529);
								AttackPayload additional_attack = new AttackPayload(this);
								additional_attack.SetSingleTarget(secondaryHit);
								additional_attack.AddWeaponDamage(ScriptFormula(16), DamageType.Holy);
								additional_attack.Apply();
								secondaryProj.Destroy();
							};
							secondaryProj.Launch(target.Position, 1.5f);
						}
					}
					if (Rune_D > 0 && Rand.NextDouble() < ScriptFormula(20))            //Burst
						if (!HasBuff<DebuffStunned>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(15))));

					if (Rune_E > 0 && User is Player)               //Holy Bolt
					{
						(User as Player).AddHP(Healing(User.Attributes[GameAttributes.Level]));
						foreach (Actor ally in GetAlliesInRadius(User.Position, 10f).Actors)
							if (ally is Player)
								(ally as Player).AddHP(Healing(User.Attributes[GameAttributes.Level]));
					}
				};
				attack.Apply();
				proj.Destroy();
			};
			proj.Launch(TargetPosition, 1.5f);

			if (Rune_C > 0)                     //Hammer of pursuit
			{

			}
			yield break;
		}

		private float Healing(int level)
		{
			return (5 + 0.01f * (float)Math.Pow(level, 3));
		}

		[ImplementsPowerBuff(0, true)]
		public class SwordSpeedBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(7));
				MaxStackCount = (int)ScriptFormula(1);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Movement_Bonus_Run_Speed] -= StackCount * ScriptFormula(13);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Movement_Bonus_Run_Speed] += ScriptFormula(13);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Provoke
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.Provoke)]
	public class CrusaderProvoke : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			GeneratePrimaryResource(ScriptFormula(3));

			SpawnEffect(
				RuneSelect(
					ActorSno._x1_crusader_provoke_ringgeo,
					ActorSno._x1_crusader_provoke_ringgeo_life, 
					ActorSno._x1_crusader_provoke_ringgeo_fear,
					ActorSno._x1_crusader_provoke_ringgeo_slow,
					ActorSno._x1_crusader_provoke_ringgeo_lightning,
					ActorSno._x1_crusader_provoke_ringgeo_block
				),
				User.Position
			);

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, 15f);
			attack.OnHit = (hitPayload) =>
			{
				if (Rune_B > 0)     //Flee fool (buff slot 0)
				{
					GeneratePrimaryResource(ScriptFormula(19) * 0.3f); //Nerf Wrath gen for this rune, too OP with taunt replaced by fear
					if (!HasBuff<DebuffFeared>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffFeared(WaitSeconds(ScriptFormula(18))));
				}
				else if (!HasBuff<DebuffTaunted>(hitPayload.Target))
				{
					GeneratePrimaryResource(ScriptFormula(19));
					AddBuff(hitPayload.Target, new DebuffTaunted(WaitSeconds(ScriptFormula(1))));
					if (Rune_A > 0) AddBuff(User, new CleanseLoHBuff());    //Cleanse (buff slot 1)
					if (Rune_C > 0 && !HasBuff<TooScaredSpeedDebuff>(hitPayload.Target))        //Too scared to run (buff slot ?)
						AddBuff(hitPayload.Target, new TooScaredSpeedDebuff(ScriptFormula(6), ScriptFormula(5), WaitSeconds(ScriptFormula(1))));
				}
			};
			attack.Apply();

			if (Rune_D > 0 && !HasBuff<ChargedUpDamageBuff>(User))          //Charged Up (buff slot 3)
				AddBuff(User, new ChargedUpDamageBuff(ScriptFormula(16), WaitSeconds(ScriptFormula(15))));

			if (Rune_E > 0 && !HasBuff<HitMeBlockBuff>(User))           //Hit Me (buff slot 2)
				AddBuff(User, new HitMeBlockBuff(ScriptFormula(9), WaitSeconds(ScriptFormula(8))));

			yield break;
		}

		[ImplementsPowerBuff(1, true)]
		public class CleanseLoHBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(11));
				MaxStackCount = 5;                  //no limit for this on skill tooltip, seems OP
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			private float LoH(int level)        //SF13 table seems bugged, use our own formula
			{
				return (5 + 0.003f * (float)Math.Pow(level, 3));
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Hitpoints_On_Hit] -= StackCount * LoH(Target.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Hitpoints_On_Hit] += LoH(Target.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(4)]
		public class TooScaredSpeedDebuff : SimpleBooleanStatusDebuff
		{
			public float PercentageAps;
			public float PercentageMove;

			public TooScaredSpeedDebuff(float percentageAps, float percentageMove, TickTimer timeout)
				: base(GameAttributes.Slow, GameAttributes.Slowdown_Immune)
			{
				PercentageAps = percentageAps;
				PercentageMove = percentageMove;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply() || Target.Attributes[GameAttributes.Immunity] == true)
					return false;
				Target.WalkSpeed *= (1f - PercentageMove);
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] -= PercentageAps;
				Target.Attributes[GameAttributes.Movement_Scalar_Reduction_Percent] += PercentageMove;
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
				Target.WalkSpeed /= (1f - PercentageMove);
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] += PercentageAps;
				Target.Attributes[GameAttributes.Movement_Scalar_Reduction_Percent] -= PercentageMove;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(3)]
		public class ChargedUpDamageBuff : PowerBuff        //bugged for now
		{
			public float Percentage;
			public ChargedUpDamageBuff(float percentage, TickTimer timeout)
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
			/*public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == User && (payload as HitPayload).IsWeaponDamage)
					WeaponDamage(GetEnemiesInRadius(User.Position, 10f).GetClosestTo(User.Position), Percentage, DamageType.Lightning);
			}*/
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		class HitMeBlockBuff : PowerBuff
		{
			public float Percentage;
			public HitMeBlockBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttributes.Block_Chance] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Block_Chance] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region AkaratChampion
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithGenerators.AkaratChampion)]
	public class CrusaderAkaratChampion : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (!HasBuff<AkaratBuff>(User))
				AddBuff(User, new AkaratBuff(ScriptFormula(15), ScriptFormula(25), WaitSeconds(ScriptFormula(2))));
			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class AkaratBuff : PowerBuff
		{
			public float PercentageAps;
			public float PercentageWrathRegen;
			public bool CDRActive = false;          //done in HitPayload
			public bool resurrectActive = true;     //done in HitPayload
			public bool wrathBlast = false;         //done in Player
			const float fearRate = 1f;
			TickTimer fearTimer = null;
			public AkaratBuff(float percentageAps, float percentageWrathRegen, TickTimer timeout)
			{
				PercentageAps = percentageAps;
				PercentageWrathRegen = percentageWrathRegen;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_C > 0) CDRActive = true;       //Rally
				if (Rune_D > 0)                         //Prophet
					Target.Attributes[GameAttributes.Armor_Bonus_Percent] += ScriptFormula(3);
				if (Rune_E > 0)     //Hasteful
					Target.Attributes[GameAttributes.Movement_Bonus_Run_Speed] += ScriptFormula(17);
				Target.Attributes[GameAttributes.Attacks_Per_Second_Bonus] += PercentageAps;
				Target.Attributes[GameAttributes.Resource_Regen_Bonus_Percent] += PercentageWrathRegen;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (Rune_A > 0)         //Fire Starter
				{
					if (wrathBlast)
					{
						//visual effect here?
						WeaponDamage(GetEnemiesInRadius(User.Position, 10f), ScriptFormula(40), DamageType.Fire);
						wrathBlast = false;
					}
				}
				if (Rune_B > 0)         //Embodiment of power
				{
					if (fearTimer == null || fearTimer.TimedOut)
					{
						fearTimer = WaitSeconds(fearRate);

						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(26));
						attack.OnHit = (hitPayload) =>
						{
							if (Rand.NextDouble() < ScriptFormula(37) && !HasBuff<DebuffFeared>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffFeared(WaitSeconds(ScriptFormula(27))));
						};
						attack.Apply();
					}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_D > 0)                         //Prophet
					Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= ScriptFormula(3);
				if (Rune_E > 0)     //Hasteful
					Target.Attributes[GameAttributes.Movement_Bonus_Run_Speed] -= ScriptFormula(17);
				Target.Attributes[GameAttributes.Attacks_Per_Second_Bonus] -= PercentageAps;
				Target.Attributes[GameAttributes.Resource_Regen_Bonus_Percent] -= PercentageWrathRegen;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region ShieldBash
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.ShieldBash)]
	public class CrusaderShieldBash : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(1f);
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			Target = GetEnemiesInRadius(TargetPosition, 15f).GetClosestTo(TargetPosition);
			if (Target != null)
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, Target.Position, User.Position, Math.Max(Math.Min(PowerMath.Distance2D(User.Position, Target.Position), 30f), EvalTag(PowerKeys.WalkingDistanceMin)));

				var dashBuff = new DashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
				AddBuff(User, dashBuff);
				yield return dashBuff.Timeout;

				if (Target == null || Target.World == null) yield break;
				yield return WaitSeconds(0.1f);

				if (Rune_E > 0 && !HasBuff<DebuffRooted>(Target))   //One on One
					AddBuff(Target, new DebuffRooted(WaitSeconds(ScriptFormula(34))));
			}

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, Rune_B > 0 ? 12f : 5f);
			attack.Targets.Actors.AddRange(GetEnemiesInArcDirection(User.Position, TargetPosition,
				 Rune_D > 0 ? 8f : 20f, 120f).Actors.Where(a => !attack.Targets.Actors.Contains(a)));

			if (Rune_B > 0) attack.AddWeaponDamage((3.25f + (User.Attributes[GameAttributes.Block_Chance] * 3f)), DamageType.Holy);      //Shattered Shield
			else if (Rune_D > 0) attack.AddWeaponDamage((5.5f + (User.Attributes[GameAttributes.Block_Chance] * 1.5f)), DamageType.Holy);    //Pound
			else attack.AddWeaponDamage((3.25f + (User.Attributes[GameAttributes.Block_Chance] * 3f)), DamageType.Holy);

			attack.OnHit = (hitPayload) =>
			{
				if (Rune_A > 0)     //Crumble
					attack.OnDeath = (deathPayload) =>
					{
						SpawnEffect(ActorSno._x1_crusader_shieldbash_glowsphere_explode, deathPayload.Target.Position);
						AttackPayload secondaryAttack = new AttackPayload(this);
						secondaryAttack.Targets = GetEnemiesInRadius(deathPayload.Target.Position, 7f);
						secondaryAttack.AddWeaponDamage(ScriptFormula(27), DamageType.Holy);
						secondaryAttack.OnHit = (secHitPayload) =>
						{
							if (!HasBuff<DirectedKnockbackBuff>(secHitPayload.Target))
								AddBuff(secHitPayload.Target, new DirectedKnockbackBuff(deathPayload.Target.Position, 12f));
						};
						secondaryAttack.Apply();
					};

				if (Rune_C > 0)     //Shield Cross
				{
					Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, User.Position + new Vector3D(30, 0, 0), 90f, 4);
					//SpawnEffect(352871, User.Position);
					foreach (var projTarget in projDestinations)
					{
						// FIXME: check actor
						var proj = new Projectile(this, ActorSno._x1_crusader_shieldbash_shieldcross, User.Position);
						proj.Timeout = new SecondsTickTimer(World.Game, 1f);
						proj.OnCollision = (hit) =>
						{
							if ((Target != null) && (hit == Target)) return;
							WeaponDamage(hit, 1.35f + (User.Attributes[GameAttributes.Block_Chance] * 1f), DamageType.Holy);
						};
						proj.Launch(projTarget, 0.5f);
					}
				}
				if ((Rune_E > 0) && (Target != null))       //One on One
				{
					if (!HasBuff<KnockbackBuff>(hitPayload.Target) && !(hitPayload.Target == Target))
						AddBuff(hitPayload.Target, new KnockbackBuff(12f));
				}
			};
			attack.Apply();
			yield break;
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

				float speed = User.Attributes[GameAttributes.Running_Rate_Total] * EvalTag(PowerKeys.WalkingSpeedMultiplier);

				User.TranslateFacing(_destination, true);
				_mover = new ActorMover(User);
				_mover.Move(_destination, speed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = 69728,
				});

				// make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
				//TickTimer minDashWait = WaitSeconds(0.15f);
				Timeout = _mover.ArrivalTime;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
		}
	}
	#endregion

	//Complete
	#region SweepAttack
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.SweepAttack)]
	public class CrusaderSweepAttack : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			//bool hitAnything = false;
			var DmgType = DamageType.Physical;
			if (Rune_B > 0) DmgType = DamageType.Fire;
			if (Rune_D > 0) DmgType = DamageType.Lightning;
			if (Rune_E > 0) DmgType = DamageType.Cold;
			var LoH = LifeOnHit(User.Attributes[GameAttributes.Level]);
			int heals = 0;

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(26), 150f);
			attack.AddWeaponDamage(ScriptFormula(16) //* 10000f
				, DmgType);
			attack.OnHit = hitPayload =>
			{
				if (Rune_A > 0 && !HasBuff<KnockbackBuff>(hitPayload.Target))       //Gathering sweep
					AddBuff(hitPayload.Target, new KnockbackBuff(-15f));
				if (Rune_B > 0)                                             //Blazing Sweep (buff slot 1)
					AddBuff(hitPayload.Target, new BlazingSweepDoTBuff());
				if (Rune_C > 0 && heals < 5)        //Holy Shock, nerfed to max 5 heals per swing for now
				{
					(User as Player).AddHP(LoH);
					heals++;
				}
				if (Rune_D > 0 && Rand.NextDouble() < ScriptFormula(13))        //Trip Attack
					if (!HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(7))));
				if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(19))        //Frozen Sweep
					if (!HasBuff<DebuffChilled>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffChilled(0.3f, WaitSeconds(ScriptFormula(6))));
			};
			attack.Apply();

			yield break;
		}

		private float LifeOnHit(int level) 
		{
			return (5 + 0.00025f * (float)Math.Pow(level, 4));
		}

		//Retail allows multiple burn buffs on this rune, so we make it into 1 stacking buff here to balance it
		[ImplementsPowerBuff(1, true)]
		class BlazingSweepDoTBuff : PowerBuff
		{
			public float dps;
			public float time;
			public float damage;
			const float DotRate = 1f;
			TickTimer DotTimer = null;
			public override void Init()
			{
				damage = ScriptFormula(17);
				time = ScriptFormula(5);
				Timeout = WaitSeconds(time);
				if (time > 0) dps = damage / time;
				else dps = damage;
				MaxStackCount = 5;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DotTimer == null || DotTimer.TimedOut)
				{
					DotTimer = WaitSeconds(DotRate);
					WeaponDamage(Target, dps * StackCount, DamageType.Fire);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//not complete
	/*
	{c_gold}Cost:{/c_gold} {c_green}{Resource Cost}{/c_green} Wrath

	Summon a blessed hammer that spins around you, dealing {c_green}[{Script Formula 12}*100]%{/c_green}
	 weapon damage as Holy to all enemies hit.

	 rune A Burning wrath
	The hammer is engulfed in fire and has a {c_green}[{Script Formula 8}*100]%{/c_green} chance to scorch 
	the ground over which it passes. Enemies who pass through the scorched ground take
	 {c_green}[{Script Formula 9}*100]%{/c_green} weapon damage as Fire per second. 

	rune B Thunderstruck
	The hammer is charged with lightning that occasionally arcs between you and the hammer as it spirals through 
	the air, dealing {c_green}[{Script Formula 11}*100]%{/c_green} weapon damage as Lightning to enemies caught in the arcs. 

	rune C Limitless
	When the hammer hits an enemy there is a {c_green}[{Script Formula 18}*100]%{/c_green} chance that a new 
	hammer will be created at the location of the enemy hit. This can only occur once per hammer.

	rune D Icebound hammer
	The hammer is made of ice, chilling enemies it passes through and has a {c_green}[{Script Formula 30}*100]%{/c_green}
	 chance to explode on impact, dealing {c_green}[{Script Formula 3}*100]%{/c_green} weapon damage as Cold and
	  Freezing enemies within {c_green}{Script Formula 20}{/c_green} yards for {c_green}{Script Formula 0}{/c_green} seconds.

	rune E Dominion
	The Hammers now orbit you as you move.
	*/
	#region BlessedHammer
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.BlessedHammer)]
	public class CrusaderBlessedHammer : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][PowerSNO].Data;
			
			var proj = new Projectile(this, ActorSno._x1_crusader_holyhammer_hammer, User.Position);
			proj.Position.Z += 2f;
			//proj.Launch(TargetPosition, 0.2f);
			proj.EnterWorld(User.Position);

			//World.PlayPieAnimation(proj, User, PowerSNO, TargetPosition);
			World.PlayCircleAnimation(proj, User, PowerSNO, new Vector3D(User.Position.X + 5f, User.Position.Y + 5f, User.Position.Z));
			//World.PlaySpiralAnimation(proj, User, PowerSNO, TargetPosition);

			yield break;
		}
	}
	#endregion

	//Complete
	#region BlessedShield
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.BlessedShield)]
	public class CrusaderBlessedShield : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_E > 0)             //Piercing shield
			{
				var proj1 = new Projectile(this, ActorSno._x1_crusader_blessedshield_piercing_shieldprojectile, User.Position);
				proj1.Position.Z += 5f;  // fix height
				proj1.RadiusMod = 5f;
				proj1.OnCollision = (hit) =>
				{
					//hit.PlayEffectGroup(RuneSelect(353682, 353683, 353684, 353685, 353686, 353687));
					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage((ScriptFormula(3) + (User.Attributes[GameAttributes.Block_Chance] * ScriptFormula(17))), DamageType.Holy);
					attack.OnHit = (hitPayload) =>
					{
						if (Rand.NextDouble() < ScriptFormula(26))
							if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DirectedKnockbackBuff(proj1.Position, 10f));
					};
					attack.Apply();
				};
				proj1.Launch(TargetPosition, 0.9f);
				yield break;
			}

			if (Rune_D > 0)             //Shattering throw
			{
				var proj2 = new Projectile(this, ActorSno._x1_crusader_blessedshield_piercing_shieldprojectile, User.Position);
				proj2.Position.Z += 5f;
				proj2.OnCollision = (hit) =>
				{
					//hit.PlayEffectGroup(RuneSelect(353682, 353683, 353684, 353685, 353686, 353687));	
					WeaponDamage(hit, ScriptFormula(38) + (User.Attributes[GameAttributes.Block_Chance] * ScriptFormula(17)), DamageType.Holy);
					var additional_targets = GetEnemiesInRadius(hit.Position, 15f).Actors.Take((int)ScriptFormula(4));
					foreach (var target in additional_targets)
					{
						var proj3 = new Projectile(this, ActorSno._x1_crusader_blessedshield_split_shieldprojectile_small, hit.Position);
						proj3.LaunchChain(hit, target.Position, ShieldOnHitSecondary, 1f, (int)ScriptFormula(12) + 1, 15f);
					}
					proj2.Destroy();
				};
				proj2.Launch(TargetPosition, 1.5f);
				yield break;
			}

			//base effect
			var proj = new Projectile(
				this,
				RuneSelect(
					ActorSno._x1_crusader_blessedshield_shieldprojectile,
					ActorSno._x1_crusader_blessedshield_shieldprojectile_stun,
					ActorSno._x1_crusader_blessedshield_firey_shieldprojectile,
					ActorSno._x1_crusader_blessedshield_shieldprojectile,
					ActorSno._x1_crusader_blessedshield_piercing_shieldprojectile,
					ActorSno._x1_crusader_blessedshield_piercing_shieldprojectile
				), 
				User.Position
			);
			proj.LaunchChain(User, TargetPosition, ShieldOnHit, 1f, (int)ScriptFormula(12) + 1, 15f);
			yield break;
		}

		private void ShieldOnHit(Actor hit, int iteration)
		{
			var damageType = DamageType.Holy;
			if (Rune_A > 0) damageType = DamageType.Lightning;
			if (Rune_B > 0) damageType = DamageType.Fire;

			//hit.PlayEffectGroup(RuneSelect(353682, 353683, 353684, 353685, 353686, 353687));
			AttackPayload attack = new AttackPayload(this);
			attack.SetSingleTarget(hit);
			attack.AddWeaponDamage((ScriptFormula(3) + (User.Attributes[GameAttributes.Block_Chance] * ScriptFormula(17))), damageType);
			attack.OnHit = (hitPayload) =>
			{
				if (Rune_A > 0 && Rand.NextDouble() < (ScriptFormula(33) - iteration * ScriptFormula(19)))  //Staggering Shield
					if (!HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(18))));
				if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(29))        //Combust
				{
					SpawnEffect(ActorSno._x1_crusader_blessedshield_fire_damagewave, hitPayload.Target.Position);
					WeaponDamage(GetEnemiesInRadius(hitPayload.Target.Position, ScriptFormula(34)), ScriptFormula(32), DamageType.Fire);
				}
				if (Rune_C > 0) AddBuff(User, new AegisBuff());     //Divine Aegis (buff slot 2)
			};
			attack.Apply();
		}

		private void ShieldOnHitSecondary(Actor hit, int iteration)     //Boost: added block chance to damage, same as base effect
		{
			//hit.PlayEffectGroup(RuneSelect(353682, 353683, 353684, 353685, 353686, 353687));
			WeaponDamage(hit, ScriptFormula(38) + (User.Attributes[GameAttributes.Block_Chance] * ScriptFormula(17)), DamageType.Holy);
		}

		[ImplementsPowerBuff(2, true)]
		public class AegisBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(21));
				MaxStackCount = 2;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= StackCount * ScriptFormula(22);
				Target.Attributes[GameAttributes.Hitpoints_Regen_Bonus_Percent] -= StackCount * ScriptFormula(23);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] += ScriptFormula(22);
				Target.Attributes[GameAttributes.Hitpoints_Regen_Bonus_Percent] += ScriptFormula(23);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region FistOfTheHeavens
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.FistOfTheHeavens)]
	public class CrusaderFistOfTheHeavens : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if (Rune_E > 0)     //Retribution
			{
				var proj = new Projectile(this, ActorSno._x1_crusader_fistofheavens_chargedbolt_piercing, User.Position);
				proj.Position.Z += 5f;
				proj.OnCollision = (hit) =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage(ScriptFormula(34), DamageType.Holy);
					attack.Apply();
					if (hit == Target)
					{
						SpawnEffect(ActorSno._x1_crusader_fistofheavens_chargedbolt_piercing_explosion, hit.Position);
						AttackPayload secondaryAttack = new AttackPayload(this);
						secondaryAttack.Targets = GetEnemiesInRadius(hit.Position, ScriptFormula(9));
						secondaryAttack.AddWeaponDamage(ScriptFormula(23), DamageType.Holy);
						secondaryAttack.Apply();

						Vector3D[] boltDestinations = PowerMath.GenerateSpreadPositions(hit.Position, hit.Position + new Vector3D(30, 0, 0), 60f, 6);
						foreach (var boltTarget in boltDestinations)
						{
							var secondaryProj = new Projectile(this, ActorSno._x1_crusader_fistofheavens_chargedbolt_lightningrod, hit.Position);
							secondaryProj.Position.Z += 5f;
							secondaryProj.OnCollision = (hit) =>
							{
								if (hit == Target) return;
								AttackPayload boltAttack = new AttackPayload(this);
								boltAttack.SetSingleTarget(hit);
								boltAttack.AddWeaponDamage(ScriptFormula(33), DamageType.Holy);
								boltAttack.Apply();
							};
							secondaryProj.Launch(boltTarget, 1.5f);
						}
						proj.Destroy();
					}
				};
				proj.Launch(TargetPosition, 1.5f);
				yield break;
			}

			//Base effect			
			var fistPoint = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 60f)));
			fistPoint.PlayEffectGroup(RuneSelect(341131, 341091, 342928, 341131, 324044, -1));  //347125
			AttackPayload column = new AttackPayload(this);
			column.Targets = GetEnemiesInRadius(fistPoint.Position, ScriptFormula(9));
			column.AddWeaponDamage(ScriptFormula(30), Rune_D > 0 ? DamageType.Holy : DamageType.Lightning);
			column.OnHit = (hitPayload) =>
			{
				if (Rune_B > 0)     //Reverberation
				{
					fistPoint.PlayEffectGroup(342928);
					if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DirectedKnockbackBuff(fistPoint.Position, 5f));
					if (!HasBuff<DebuffSlowed>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(17), WaitSeconds(ScriptFormula(22))));
				}
			};
			column.Apply();
			yield return WaitSeconds(0.3f);

			if (Rune_A > 0)     //Heaven's Tempest
			{
				//This aoe's stacking is allowed in retail
				var tempest = SpawnEffect(ActorSno._x1_crusader_fistofheavens_teslacoil_stormcloud, fistPoint.Position, 0, WaitSeconds(ScriptFormula(10) + User.Attributes[GameAttributes.Power_Duration_Increase, 30680]));
				tempest.UpdateDelay = 1f;
				tempest.OnUpdate = () =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(tempest.Position, ScriptFormula(13));
					attack.AddWeaponDamage(ScriptFormula(12), DamageType.Lightning);
					attack.Apply();
				};
				yield break;
			}
			if (Rune_C > 0)     //Fissure
			{
				var fissure = SpawnEffect(ActorSno._x1_crusader_fistofheavens_teslacoil, fistPoint.Position, 0, WaitSeconds(ScriptFormula(18) + User.Attributes[GameAttributes.Power_Duration_Increase, 30680]));
				fissure.UpdateDelay = 0.8f;
				fissure.OnUpdate = () =>
				{
					//Another fissure
					var fissures = fissure.GetActorsInRange<EffectActor>(40f).Where(i => i.SNO == ActorSno._x1_crusader_fistofheavens_teslacoil);
					foreach (var fiss in fissures)
					{
						if (fiss == fissure) continue;

						if (PowerMath.Distance2D(fissure.Position, fiss.Position) <= 40f)
						{
							fissure.AddRopeEffect(0x78c0, fiss);
							AttackPayload arc = new AttackPayload(this);
							arc.Targets = GetEnemiesInBeamDirection(fissure.Position, fiss.Position, PowerMath.Distance2D(fissure.Position, fiss.Position), 5f);
							arc.AddWeaponDamage(ScriptFormula(19) * 0.25f, DamageType.Lightning);
							arc.Apply();
						}
					}

					var targets = GetEnemiesInRadius(fissure.Position, 20f).Actors;
					foreach (var target in targets)
					{
						var proj = new Projectile(this, ActorSno._x1_crusader_fistofheavens_chargedbolt_lightningrod, fissure.Position);
						proj.Position.Z += 5f;
						proj.OnCollision = (hit) =>
						{
							if (hit == fissure) return;
							WeaponDamage(hit, ScriptFormula(25), DamageType.Lightning);
							proj.Destroy();
						};
						proj.Launch(target.Position, 1f);
					}
				};
				yield break;
			}

			fistPoint.PlayEffectGroup(RuneSelect(341688, -1, -1, -1, 323721, -1));
			Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(fistPoint.Position, fistPoint.Position + new Vector3D(30, 0, 0), 60f, 6);
			var runeActorSno = RuneSelect(
				ActorSno._x1_crusader_fistofheavens_chargedbolt,
				ActorSno._x1_crusader_fistofheavens_chargedbolt,
				ActorSno._x1_crusader_fistofheavens_chargedbolt_knockback,
				ActorSno._x1_crusader_fistofheavens_chargedbolt,
				ActorSno._x1_crusader_fistofheavens_chargedbolt_wellofretribution,
				ActorSno._x1_crusader_fistofheavens_chargedbolt
			);
			foreach (var projTarget in projDestinations)
			{
                var proj = new Projectile(this, runeActorSno, fistPoint.Position);
				proj.Position.Z += 5f;
				if (Rune_D > 0) proj.RadiusMod = ScriptFormula(28);     //Divine Well
				proj.OnCollision = (hit) =>
				{
					if (hit == Target) return;      //To prevent single-target dps exploit
					AttackPayload secondaryAttack = new AttackPayload(this);
					secondaryAttack.SetSingleTarget(hit);
					secondaryAttack.AddWeaponDamage(Rune_D > 0 ? ScriptFormula(26) : ScriptFormula(1), Rune_D > 0 ? DamageType.Holy : DamageType.Lightning);
					secondaryAttack.Apply();
				};
				proj.Launch(projTarget, 1.5f);
			}

			yield break;
		}
	}
	#endregion

	//Complete
	#region Phalanx
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.Phalanx)]
	public class CrusaderPhalanx : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float damageMult = 1f;
			if ((User as Player).SkillSet.HasPassive(348741)) damageMult = 1.2f;    //Lord Commander passive

			if (Rune_A > 0)     //Bowmen
			{
				StartCooldown(WaitSeconds(ScriptFormula(27)));
				int maxAvatars = 4;
				List<Actor> avatars = new List<Actor>();

				for (int i = 0; i < maxAvatars; i++)
				{
					var avatar = new AvatarRanged(World, this, i, ScriptFormula(8) * damageMult, WaitSeconds(ScriptFormula(35) + 2f));
					avatar.Brain.DeActivate();
					avatar.Position = RandomDirection(User.Position, 3f, 8f);
					avatar.Attributes[GameAttributes.Untargetable] = true;

					avatar.EnterWorld(avatar.Position);
					avatars.Add(avatar);
					yield return WaitSeconds(0.2f);
				}
				yield return WaitSeconds(0.5f);

				foreach (Actor avatar in avatars)
				{
					(avatar as Minion).Brain.Activate();
					avatar.Attributes[GameAttributes.Untargetable] = false;
					avatar.Attributes.BroadcastChangedIfRevealed();
				}
				yield break;
			}

			if (Rune_D > 0)     //Shield Bearers
			{
				UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
				StartCooldown(WaitSeconds(ScriptFormula(25)));
				SpawnEffect(ActorSno._x1_crusader_phalanx3_blocker, User.Position, 0, WaitSeconds(5f));
				yield break;
			}

			if (Rune_E > 0)     //Bodyguard
			{
				UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
				StartCooldown(WaitSeconds(ScriptFormula(32)));
				int maxAvatars = 2;
				List<Actor> avatars = new List<Actor>();

				for (int i = 0; i < maxAvatars; i++)
				{
					var avatar = new AvatarMelee(World, this, i, ScriptFormula(46) * damageMult, WaitSeconds(ScriptFormula(36) + 2f));
					avatar.Brain.DeActivate();
					avatar.Position = RandomDirection(User.Position, 3f, 8f);
					avatar.Attributes[GameAttributes.Untargetable] = true;

					avatar.EnterWorld(avatar.Position);
					avatars.Add(avatar);
					yield return WaitSeconds(0.2f);
				}
				yield return WaitSeconds(0.5f);

				foreach (Actor avatar in avatars)
				{
					(avatar as Minion).Brain.Activate();
					avatar.Attributes[GameAttributes.Untargetable] = false;
					avatar.Attributes.BroadcastChangedIfRevealed();
				}
				yield break;
			}

			//base effect
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 30f));

			var projSNO = ActorSno._x1_crusader_phalanx3_projectile;
			if (Rune_B > 0) projSNO = ActorSno._x1_crusader_phalanx3_projectile_chargers;
			if (Rune_C > 0) projSNO = ActorSno._x1_crusader_phalanx3_projectile_horse;

			for (int i = -1; i < 2; i++)
			{
				float angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition);
				var proj = new Projectile(this, projSNO, User.Position + new Vector3D(i * 5 * (float)Math.Sin(angle), i * 5 * (float)Math.Cos(angle), 0));
				proj.RadiusMod = 2.5f;
				proj.OnCollision = (hit) =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage(ScriptFormula(18), DamageType.Physical);
					attack.OnHit = (hitPayload) =>
					{
						if (Rune_C > 0 && Rand.NextDouble() < ScriptFormula(23))    //Stampede
							if (!HasBuff<DebuffStunned>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(24))));
					};
					attack.Apply();
				};
				proj.OnTimeout = () =>
				{
					if (Rune_B > 0)         //Shield charge
					{
						proj.PlayEffectGroup(375441);
						WeaponDamage(GetEnemiesInRadius(proj.Position, 7f), ScriptFormula(20), DamageType.Physical);
					}
				};
				proj.Launch(TargetPosition + new Vector3D(i * 5 * (float)Math.Sin(angle), i * 5 * (float)Math.Cos(angle), 0), 0.4f);
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region FallingSword
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.FaithSpenders.FallingSword)]
	public class CrusaderFallingSword : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			var proxyLifetime = 2f;
			if (Rune_A > 0) proxyLifetime = ScriptFormula(11) + 1f;     //to compensate for activation delay
			if (Rune_B > 0) proxyLifetime = ScriptFormula(19) + 2f;
			var dropPoint = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 60f)), WaitSeconds(proxyLifetime));

			if (!User.World.CheckLocationForFlag(dropPoint.Position, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			User.PlayEffectGroup(RuneSelect(241760, 353616, 324779, 353105, 354259, 354419));   //launch
			dropPoint.PlayEffectGroup(RuneSelect(265543, 353540, 324791, 353106, 354266, 354546));  //pending
			var animation1 = ((User as Player).Toon.Gender == 2) ? AnimationSno.x1_crusader_female_hth_attack_fallingsword_01 : AnimationSno.x1_crusader_male_hth_attack_fallingsword_01;
			User.PlayActionAnimation(animation1, 1, 12);
			yield return WaitTicks(12);

			User.Teleport(dropPoint.Position);

			var animation2 = ((User as Player).Toon.Gender == 2) ? AnimationSno.x1_crusader_female_hth_attack_fallingsword_02 : AnimationSno.x1_crusader_male_hth_attack_fallingsword_02;
			User.PlayActionAnimation(animation2, 1, 50);
			yield return WaitTicks(20);
			dropPoint.PlayEffectGroup(RuneSelect(241761, 353634, 324826, 353109, 354245, 353851));  //impact
			dropPoint.PlayEffectGroup(RuneSelect(275347, 353814, 324832, 353108, 354254, 354632));  //impactLightning
																									//yield return WaitTicks(30);

			var dmgType = DamageType.Physical;
			if (Rune_A > 0) dmgType = DamageType.Fire;
			if (Rune_B > 0 || Rune_D > 0) dmgType = DamageType.Lightning;
			if (Rune_E > 0) dmgType = DamageType.Holy;
			int CDRcount = 0;

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(3));
			attack.AddWeaponDamage(ScriptFormula(35), dmgType);
			attack.OnHit = (hitPayload) =>
			{
				if (!HasBuff<KnockbackBuff>(hitPayload.Target))
					AddBuff(hitPayload.Target, new KnockbackBuff(5f));
				if (Rune_D > 0)         //Rapid descent
					foreach (var cdBuff in User.World.BuffManager.GetBuffs<CooldownBuff>(User))
						if (cdBuff.TargetPowerSNO == 239137 && CDRcount < 10)
						{
							cdBuff.Reduce(60);
							CDRcount++;
						}
			};
			attack.Apply();

			if (Rune_A > 0)         //Superheated
			{
				dropPoint.PlayEffectGroup(361061);
				dropPoint.UpdateDelay = 1f;
				dropPoint.OnUpdate = () =>
				{
					AttackPayload burn = new AttackPayload(this);
					burn.Targets = GetEnemiesInRadius(dropPoint.Position, ScriptFormula(3) - 3f);
					burn.AddWeaponDamage(ScriptFormula(16), DamageType.Fire);
					burn.Apply();
				};
			}
			if (Rune_B > 0)     //Part the clouds
			{
				dropPoint.UpdateDelay = 1f;
				dropPoint.OnUpdate = () =>
				{
					var targets = GetEnemiesInRadius(dropPoint.Position, ScriptFormula(3) * 1.2f);
					if (!targets.Actors.Any()) return;

					AttackPayload shock = new AttackPayload(this);
					shock.SetSingleTarget(targets.Actors.PickRandom());
					shock.Targets.Actors.First().PlayEffectGroup(312568);
					shock.AddWeaponDamage(ScriptFormula(21), DamageType.Lightning);
					shock.OnHit = (hitPayload) =>
					{
						if (!HasBuff<DebuffStunned>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(23))));
					};
					shock.Apply();
				};
			}
			if (Rune_C > 0)         //Rise Brothers
			{
				int maxAvatars = (int)ScriptFormula(14);
				List<Actor> avatars = new List<Actor>();

				for (int i = 0; i < maxAvatars; i++)
				{
					var avatar = new AvatarMelee(World, this, i, ScriptFormula(49), WaitSeconds(ScriptFormula(28) + 2f));
					avatar.Brain.DeActivate();
					avatar.Position = RandomDirection(User.Position, 3f, 8f);
					avatar.Attributes[GameAttributes.Untargetable] = true;

					avatar.EnterWorld(avatar.Position);
					avatars.Add(avatar);
					yield return WaitSeconds(0.2f);
				}
				yield return WaitSeconds(0.5f);

				foreach (Actor avatar in avatars)
				{
					(avatar as Minion).Brain.Activate();
					avatar.Attributes[GameAttributes.Untargetable] = false;
					avatar.Attributes.BroadcastChangedIfRevealed();
				}
			}
			if (Rune_E > 0)         //Flurry
			{
				var proxy = SpawnEffect(ActorSno._x1_crusader_fallingsword_swordnadorig_spawner, TargetPosition, 0, WaitSeconds(ScriptFormula(41) + User.Attributes[GameAttributes.Power_Duration_Increase, 30680]));
				proxy.UpdateDelay = 1f;
				proxy.OnUpdate = () =>
				{
					AttackPayload slash = new AttackPayload(this);
					slash.Targets = GetEnemiesInRadius(proxy.Position, ScriptFormula(3));
					slash.AddWeaponDamage(ScriptFormula(45), DamageType.Holy);
					slash.OnHit = (hitPayload) =>
					{
						if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DirectedKnockbackBuff(proxy.Position, -10f));
					};
					slash.Apply();
				};
			}
			yield break;
		}
	}
	#endregion

	//Complete	
	#region ShieldGlare
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.ShieldGlare)]
	public class CrusaderShieldGlare : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(0), 120f);
			attack.OnHit = (hitPayload) =>
			{
				if (Rune_B > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(8))       //Uncertainty
						AddBuff(hitPayload.Target, new GlareCharmDebuff());
				}
				else if (!HasBuff<DebuffBlind>(hitPayload.Target))
				{
					AddBuff(hitPayload.Target, new DebuffBlind(WaitSeconds(ScriptFormula(10))));
					if (Rune_A > 0)         //Divine Verdict
						AddBuff(hitPayload.Target, new VerdictDebuff(ScriptFormula(2), WaitSeconds(ScriptFormula(5))));

					if (Rune_C > 0)         //Emblazoned shield
						if (hitPayload.Target.Attributes[GameAttributes.Hitpoints_Cur] < hitPayload.Target.Attributes[GameAttributes.Hitpoints_Max_Total] * ScriptFormula(15))
							if (Rand.NextDouble() < ScriptFormula(14))
							{
								AttackPayload blast = new AttackPayload(this);
								blast.Targets = GetEnemiesInRadius(hitPayload.Target.Position, ScriptFormula(17));
								blast.AddWeaponDamage(ScriptFormula(21), DamageType.Physical);
								blast.Apply();
							}
					if (Rune_D > 0) GeneratePrimaryResource(ScriptFormula(9));      //Zealous Glare
					if (Rune_E > 0 && !HasBuff<DebuffSlowed>(hitPayload.Target))    //Subdue
						AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(23), WaitSeconds(ScriptFormula(22))));
				}
			};
			attack.Apply();
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class VerdictDebuff : PowerBuff
		{
			public float Percentage;
			public VerdictDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttributes.Debuff_Duration_Reduction_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Debuff_Duration_Reduction_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(2)]
		class GlareCharmDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(16));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttributes.Team_Override] = 1;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Team_Override] = 10;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region IronSkin
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.IronSkin)]
	public class CrusaderIronSkin : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			AddBuff(User, new IronSkinBuff(ScriptFormula(1), Rune_B > 0 ? WaitSeconds(ScriptFormula(10)) : WaitSeconds(ScriptFormula(0))));
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class IronSkinBuff : PowerBuff
		{
			const float shockRate = 1f;
			TickTimer shockTimer = null;
			public float Percentage;
			public IronSkinBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_D > 0)     //Reflective skin
					Target.Attributes[GameAttributes.Thorns_Fixed] *= ScriptFormula(15);
				Target.Attributes[GameAttributes.Damage_Absorb_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (Rune_A > 0)         //Charged Up
				{
					if (shockTimer == null || shockTimer.TimedOut)
					{
						shockTimer = WaitSeconds(shockRate);
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(6));
						attack.OnHit = (hitPayload) =>
						{
							if (Rand.NextDouble() < ScriptFormula(7) && !HasBuff<DebuffStunned>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(20))));
						};
						attack.Apply();
					}
				}
				return false;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					if (Rune_E > 0 && !HasBuff<FlashSpeedBuff>(User))       //Flash (buff slot 3)
						AddBuff(User, new FlashSpeedBuff(ScriptFormula(17), WaitSeconds(ScriptFormula(22))));
			}

			public override void Remove()
			{
				base.Remove();

				if (Rune_D > 0)     //Reflective skin
					Target.Attributes[GameAttributes.Thorns_Fixed] /= ScriptFormula(15);
				Target.Attributes[GameAttributes.Damage_Absorb_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Rune_C > 0)     //Explosive skin
				{
					User.PlayEffectGroup(349877);
					AttackPayload blast = new AttackPayload(this);
					blast.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(13));
					blast.AddWeaponDamage(ScriptFormula(12), DamageType.Physical);
					blast.Apply();
				}
			}
		}
	}
	#endregion

	//Complete
	#region Consecration
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.Consecration)]
	public class CrusaderConsecration : Skill
	{
		private float LifeRegen(int level)
		{
			return (10 + 0.1f * (float)Math.Pow(level, 3));
		}
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			float duration = Rune_A > 0 ? ScriptFormula(10) : ScriptFormula(1);
			float radius = Rune_C > 0 ? ScriptFormula(11) : ScriptFormula(3);       //Bathed in Light
			float healing = LifeRegen(User.Attributes[GameAttributes.Level]);
			if (Rune_C > 0) healing *= 1.5f;

			var proxy = SpawnEffect(
				RuneSelect(
					ActorSno._x1_crusader_consecration_proxy, 
					ActorSno._x1_crusader_consecration_proxy, 
					ActorSno._x1_crusader_consecration_proxy_frozen, 
					ActorSno._x1_crusader_consecration_proxy, 
					ActorSno._x1_crusader_consecration_proxy_shatteredground, 
					ActorSno._x1_crusader_consecration_proxy_fear
				),
				User.Position,
				0,
				WaitSeconds(duration + User.Attributes[GameAttributes.Power_Duration_Increase, 30680])
			);
			if (Rune_A > 0) SpawnEffect(ActorSno._x1_crusader_consecration_wall, User.Position);         //Aegis Purgatory	
			proxy.UpdateDelay = 0.9f;
			proxy.OnUpdate = () =>
			{
				var targets = GetAlliesInRadius(proxy.Position, radius).Actors;
				//targets.AddRange(proxy.GetActorsInRange<Player>(radius));
				targets.Add(User);
				foreach (Actor target in targets)
				{
					if (!HasBuff<ConsecHealBuff>(target)) 
						AddBuff(target, new ConsecHealBuff(healing, WaitSeconds(1f)));
					else target.World.BuffManager.GetFirstBuff<ConsecHealBuff>(target).Extend(60);
				}

				if (Rune_A > 0)         //Aegis Purgatory
				{
					AttackPayload push = new AttackPayload(this);
					push.Targets = GetEnemiesInRadius(proxy.Position, radius);
					push.OnHit = (hitPayload) =>
					{
						if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DirectedKnockbackBuff(proxy.Position, 10f));
					};
					push.Apply();
				}

				if (Rune_B > 0)     //Frozen Ground
				{
					AttackPayload freeze = new AttackPayload(this);
					freeze.Targets = GetEnemiesInRadius(proxy.Position, radius);
					freeze.OnHit = (hitPayload) =>
					{
						if (Rand.NextDouble() < ScriptFormula(20))
						{
							if (!HasBuff<DebuffFrozen>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(21))));
						}
						else
						{
							if (!HasBuff<DebuffChilled>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffChilled(ScriptFormula(15), WaitSeconds(1f)));
							else hitPayload.Target.World.BuffManager.GetFirstBuff<DebuffChilled>(hitPayload.Target).Extend(60);
						}
					};
					freeze.Apply();
				}
				if (Rune_D > 0)     //Shattered Ground
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(proxy.Position, radius);
					attack.AddWeaponDamage(ScriptFormula(13), DamageType.Fire);
					attack.Apply();
				}
				if (Rune_E > 0)     //Fearful
				{
					AttackPayload fear = new AttackPayload(this);
					fear.Targets = GetEnemiesInRadius(proxy.Position, radius);
					fear.OnHit = (hitPayload) =>
					{
						if (Rand.NextDouble() < ScriptFormula(18) && !HasBuff<DebuffFeared>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffFeared(WaitSeconds(ScriptFormula(19))));
					};
					fear.Apply();
				}
			};
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ConsecHealBuff : PowerBuff
		{
			public float Percentage;
			public ConsecHealBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Judgment
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.Judgment)]
	public class CrusaderJudgment : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			var castPoint = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 60f)));
			castPoint.PlayEffectGroup(RuneSelect(343622, 343680, 343666, 343652, 343623, 345410));

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(castPoint.Position, ScriptFormula(0));
			if (Rune_B > 0)         //Mass Verdict
			{
				foreach (var target in attack.Targets.Actors)
					if (!HasBuff<DirectedKnockbackBuff>(target)) AddBuff(target, new DirectedKnockbackBuff(castPoint.Position, -20f));
				yield return WaitSeconds(1f);
			}
			attack.OnHit = (hitPayload) =>
			{
				if (!HasBuff<JudgedDebuffRooted>(hitPayload.Target))
					AddBuff(hitPayload.Target, new JudgedDebuffRooted());
				if (Rune_A > 0) AddBuff(User, new PenitenceRegenBuff());    //Penitence (buff slot 1)			
			};
			attack.Apply();
			yield break;
		}

		[ImplementsPowerBuff(3)]
		public class JudgedDebuffRooted : PowerBuff
		{
			public float bonusChC = 0;
			public bool conversion = false;
			public override void Init()
			{
				Timeout = Rune_C > 0 ? WaitSeconds(ScriptFormula(1)) : WaitSeconds(ScriptFormula(7));
				if (Rune_D > 0) bonusChC = ScriptFormula(14);       //Resolved
				if (Rune_E > 0) conversion = true;      //Conversion, done in deathPayload
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttributes.Stunned] = true;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (Target is Player)
				{
					if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
						if (FastRandom.Instance.Next(100) < 30)
							(Target as Player).AddPercentageHP(20);
					if ((Target as Player).SkillSet.HasPassive(209813)) //Provocation (Monk)
						AddBuff(Target, new ProvocationBuff());
				}
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Stunned] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1, true)]
		public class PenitenceRegenBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(4));
				MaxStackCount = 4;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			private float LifeRegen(int level) 
			{
				return (10 + 0.008f * (float)Math.Pow(level, 3));
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] -= StackCount * LifeRegen(Target.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] += LifeRegen(Target.Attributes[GameAttributes.Level]);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Condemn
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.Condemn)]
	public class CrusaderCondemn : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_E > 0)     //Unleashed
			{
				User.PlayEffectGroup(RuneSelect(277780, 352671, 352957, 352971, 352707, 352857));
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(8));
				attack.AddWeaponDamage(ScriptFormula(1), DamageType.Holy);
				attack.Apply();
				yield break;
			}
			if (Rune_A > 0 && !HasBuff<CondemnTempBuff>(User))          //Reciprocate
				AddBuff(User, new CondemnTempBuff(ScriptFormula(4), WaitSeconds(ScriptFormula(9) + 1f)));

			if (Rune_B > 0)             //Vacuum
			{
				int delay = (int)ScriptFormula(9);
				int i = 2;
				while (delay > 0)
				{
					AttackPayload pull = new AttackPayload(this);
					pull.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(8), i);
					pull.OnHit = (hitPayload) =>
					{
						if (!HasBuff<KnockbackBuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new KnockbackBuff(-15f));
					};
					pull.Apply();

					i += 2;
					yield return (WaitSeconds(1f));
					delay--;
					if (i > 12) break;      //don't trust SF)
				}
			}
			else yield return WaitSeconds(ScriptFormula(9));

			User.PlayEffectGroup(RuneSelect(277780, 352671, 352957, 352971, 352707, 352857));
			int CDRcount = 0;
			AttackPayload blast = new AttackPayload(this);
			blast.Targets = GetEnemiesInRadius(User.Position, Rune_D > 0 ? ScriptFormula(5) : ScriptFormula(8));
			blast.AddWeaponDamage(ScriptFormula(1), DamageType.Holy);
			if (Rune_A > 0)
				blast.AddDamage(User.World.BuffManager.GetFirstBuff<CondemnTempBuff>(User).DamageTaken, 0f, DamageType.Fire);
			blast.OnHit = (hitPayload) =>
			{
				if (Rune_C > 0)         //Eternal Retaliation
					foreach (var cdBuff in (User as Player).World.BuffManager.GetBuffs<CooldownBuff>(User))
						if (cdBuff.TargetPowerSNO == 266627 && CDRcount < 7)    //Nerf: cannot CDR more than 7 times
						{
							cdBuff.Reduce(60);
							CDRcount++;
						}
			};
			blast.Apply();

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class CondemnTempBuff : PowerBuff
		{
			public float DamageTaken = 0;
			private float Multiplier = 0;
			public CondemnTempBuff(float multiplier, TickTimer timeout)
			{
				Timeout = timeout;
				Multiplier = multiplier;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					if (DamageTaken < 1000000f)         //Nerf: cap this damage for now
						DamageTaken += (payload as HitPayload).TotalDamage * Multiplier;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region SteedCharge
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.SteedCharge)]
	public class CrusaderSteedCharge : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float cooldown = 25f;
			if ((User as Player).SkillSet.HasPassive(348741)) cooldown -= 5f;       //Lord Commander passive
			StartCooldown(cooldown);

			if (!HasBuff<PonyBuff>(User))
				AddBuff(User, new PonyBuff(ScriptFormula(13), ScriptFormula(18), ScriptFormula(20), ScriptFormula(31), WaitSeconds(Rune_B > 0 ? ScriptFormula(0) : ScriptFormula(14))));
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class PonyBuff : PowerBuff
		{
			const float DamageRate = 0.5f;
			TickTimer DamageTimer = null;
			private float MultiplierA = 0f;
			private float MultiplierC = 0f;
			private float MultiplierD = 0f;
			private float MultiplierE = 0f;
			public PonyBuff(float multiplierA, float multiplierC, float multiplierD, float multiplierE, TickTimer timeout)
			{
				Timeout = timeout;
				MultiplierA = multiplierA;
				MultiplierC = multiplierC;
				MultiplierD = multiplierD;
				MultiplierE = multiplierE;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] += 1f;
				User.Attributes[GameAttributes.Walk_Passability_Power_SNO] = 243853;
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DamageTimer == null || DamageTimer.TimedOut)
				{
					DamageTimer = WaitSeconds(DamageRate);
					if (Rune_A > 0)     //Ramming Speed
					{
						AttackPayload ram = new AttackPayload(this);
						ram.Targets = GetEnemiesInRadius(User.Position, 6f);
						ram.AddWeaponDamage(MultiplierA * 0.5f, DamageType.Physical);
						ram.OnHit = (hitPayload) =>
						{
							if (!HasBuff<KnockbackBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new KnockbackBuff(5f));
						};
						ram.Apply();
					}
					if (Rune_C > 0)     //Rejuvenation
					{
						(User as Player).AddPercentageHP((int)(MultiplierC * 0.5f * 100));
					}
					if (Rune_D > 0)     //Nightmare
					{
						var firePool = SpawnEffect(ActorSno._x1_crusader_steedcharge_firepool, User.Position, 0, WaitSeconds(4f));
						firePool.UpdateDelay = 0.9f;
						firePool.OnUpdate = () =>
						{
							AttackPayload attack = new AttackPayload(this);
							attack.Targets = GetEnemiesInRadius(firePool.Position, 4f);
							attack.OnHit = (hitPayload) =>
							{
								if (!HasBuff<FirePoolDmgBuff>(hitPayload.Target))
									AddBuff(hitPayload.Target, new FirePoolDmgBuff(MultiplierD * 0.5f, WaitSeconds(1f)));
								else hitPayload.Target.World.BuffManager.GetFirstBuff<FirePoolDmgBuff>(hitPayload.Target).Extend(60);
							};
							attack.Apply();
						};
					}
					if (Rune_E > 0)     //Draw and Quarter
					{
						AttackPayload drag = new AttackPayload(this);
						drag.Targets = GetEnemiesInRadius(User.Position, 15f, 5);
						drag.AddWeaponDamage(MultiplierE * 0.5f, DamageType.Physical);
						drag.OnHit = (hitPayload) =>
						{
							if (!HasBuff<KnockbackBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new KnockbackBuff(-15f));
						};
						drag.Apply();
					}
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] -= 1f;
				User.Attributes[GameAttributes.Walk_Passability_Power_SNO] = -1;
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(4)]
		class FirePoolDmgBuff : PowerBuff
		{
			const float DamageRate = 1f;
			TickTimer DamageTimer = null;
			private float Multiplier = 0;
			public FirePoolDmgBuff(float multiplier, TickTimer timeout)
			{
				Timeout = timeout;
				Multiplier = multiplier;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DamageTimer == null || DamageTimer.TimedOut)
				{
					DamageTimer = WaitSeconds(DamageRate);
					WeaponDamage(Target, Multiplier, DamageType.Fire);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region LawsOfValor
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.LawsOfValor)]
	public class CrusaderLawsOfValor : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float durationBonus = 0f;
			if ((User as Player).SkillSet.HasPassive(310678)) durationBonus = 5f;   //Long Arm of the Law passive

			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (!HasBuff<LawsApsBuff>(User))
				AddBuff(User, new LawsApsBuff(ScriptFormula(4) - ScriptFormula(2), WaitSeconds(ScriptFormula(5) + durationBonus), true));

			foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
				if (!HasBuff<LawsApsBuff>(ally))
					AddBuff(ally, new LawsApsBuff(ScriptFormula(4) - ScriptFormula(2), WaitSeconds(ScriptFormula(5) + durationBonus), false));

			if (Rune_B > 0)     //Frozen in Terror
			{
				AttackPayload stun = new AttackPayload(this);
				stun.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(9));
				stun.OnHit = (hitPayload) =>
				{
					if (Rand.NextDouble() < ScriptFormula(8) && !HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(10) + durationBonus)));
				};
				stun.Apply();
			}
			yield break;
		}

		[ImplementsPowerBuff(6)]
		public class LawsApsBuff : PowerBuff
		{
			public float Percentage;
			public bool Primary = false;
			public bool Glory = false;
			public LawsApsBuff(float percentage, TickTimer timeout, bool primary = false)
			{
				Percentage = percentage;
				Primary = primary;
				Timeout = timeout;
			}

			private float LoH(int level) 

			{
				return (5 + 0.06f * (float)Math.Pow(level, 3));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0 && Primary)          //Invincible
					Target.Attributes[GameAttributes.Hitpoints_On_Hit] += LoH(Target.Attributes[GameAttributes.Level]);
				if (Rune_C > 0 && Primary)          //Critical
					Target.Attributes[GameAttributes.Crit_Damage_Percent] += ScriptFormula(13);
				if (Rune_D > 0 && Primary)          //Unstoppable Force
					Target.Attributes[GameAttributes.Resource_Cost_Reduction_Percent] += ScriptFormula(15);
				if (Rune_E > 0 && Primary)          //Answered Prayer, done in DeathPayload
					Glory = true;
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_A > 0 && Primary)
					Target.Attributes[GameAttributes.Hitpoints_On_Hit] -= LoH(Target.Attributes[GameAttributes.Level]);
				if (Rune_C > 0 && Primary)
					Target.Attributes[GameAttributes.Crit_Damage_Percent] -= ScriptFormula(13);
				if (Rune_D > 0 && Primary)
					Target.Attributes[GameAttributes.Resource_Cost_Reduction_Percent] -= ScriptFormula(15);
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(342284)]
	public class LawsOfValorPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfHope);
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfJustice);
			AddBuff(User, new LawsApsPassiveBuff(ScriptFormula(4), null, true));
			yield break;
		}

		[ImplementsPowerBuff(2)]
		class LawsApsPassiveBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			public float Percentage;
			public bool IsAura = false;
			public LawsApsPassiveBuff(float percentage, TickTimer timeout, bool isAura = false)
			{
				Percentage = percentage;
				IsAura = isAura;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null || CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					if (IsAura)
						foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
						{
							if (!HasBuff<LawsApsPassiveBuff>(ally))
								AddBuff(ally, new LawsApsPassiveBuff(ScriptFormula(4), WaitSeconds(1.1f), false));
							else ally.World.BuffManager.GetFirstBuff<LawsApsPassiveBuff>(ally).Extend(60);
						}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region LawsOfJustice
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.LawsOfJustice)]
	public class CrusaderLawsOfJustice : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float durationBonus = 0f;
			if ((User as Player).SkillSet.HasPassive(310678)) durationBonus = 5f;   //Long Arm of the Law passive

			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (!HasBuff<LawsResBuff>(User))
				AddBuff(User, new LawsResBuff(ScriptFormula(2) - ScriptFormula(1), WaitSeconds(ScriptFormula(3) + durationBonus), true, Rune_A > 0 ? true : false, User));

			foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
				if (!HasBuff<LawsResBuff>(ally))
					AddBuff(ally, new LawsResBuff(ScriptFormula(2) - ScriptFormula(1), WaitSeconds(ScriptFormula(3) + durationBonus), false, Rune_A > 0 ? true : false, User));

			yield break;
		}

		[ImplementsPowerBuff(6)]
		public class LawsResBuff : PowerBuff
		{
			public float Allres;
			public bool Primary = false;
			public bool Redirect = false;
			public Actor Protector = null;
			public float HPTreshold = 0;
			public LawsResBuff(float allres, TickTimer timeout, bool primary = false, bool redirect = false, Actor protector = null)
			{
				Allres = allres;
				Primary = primary;
				Redirect = redirect;
				if (Redirect) Protector = protector;
				Timeout = timeout;
			}

			private float ShieldTreshold(int level)   
			{
				return (10 + 0.001f * (float)Math.Pow(level, 4));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_B > 0)         //Immovable Object
					Target.Attributes[GameAttributes.Armor_Bonus_Item] += ScriptFormula(5);
				if (Rune_C > 0) HPTreshold = ShieldTreshold(User.Attributes[GameAttributes.Level]);      //Faith's Armor
				if (Rune_E > 0)         //Bravery
					Target.Attributes[GameAttributes.Stun_Immune] = true;
				Target.Attributes[GameAttributes.Resistance_All] += Allres;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (Redirect)   //Protect the Innocent, also done in HitPayload. 0.25f is correct here, as incoming damage is already reduced by 20%
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
						WeaponDamage(Protector, (payload as HitPayload).TotalDamage * 0.25f, DamageType.Physical);

				if (Rune_C > 0 && HPTreshold > 0)       //Faith's Armor
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					{
						float dmg = (payload as HitPayload).TotalDamage;
						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= dmg;
						if (HPTreshold < 0) HPTreshold = -1f;
					}

				if (Rune_D > 0)         //Decaying Strength
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
						AddBuff(payload.Context.User, new DecayingDebuff());
			}
			public override void Remove()
			{
				base.Remove();

				if (Rune_B > 0)
					Target.Attributes[GameAttributes.Armor_Bonus_Item] -= ScriptFormula(5);
				if (Rune_E > 0)
					Target.Attributes[GameAttributes.Stun_Immune] = false;
				Target.Attributes[GameAttributes.Resistance_All] -= Allres;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1, true)]
		public class DecayingDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(9));
				MaxStackCount = (int)ScriptFormula(8);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddAmp();
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Damage_Dealt_Percent_Bonus] += StackCount * ScriptFormula(7);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			private void _AddAmp()
			{
				Target.Attributes[GameAttributes.Damage_Dealt_Percent_Bonus] -= ScriptFormula(7);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(342286)]
	public class LawsOfJusticePassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfHope);
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfValor);
			AddBuff(User, new LawsResPassiveBuff(ScriptFormula(2), null, true));
			yield break;
		}

		[ImplementsPowerBuff(2)]
		class LawsResPassiveBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			public float Allres;
			public bool IsAura = false;
			public LawsResPassiveBuff(float allres, TickTimer timeout, bool isAura = false)
			{
				Allres = allres;
				IsAura = isAura;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttributes.Resistance_All] += Allres;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null || CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					if (IsAura)
						foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
						{
							if (!HasBuff<LawsResPassiveBuff>(ally))
								AddBuff(ally, new LawsResPassiveBuff(ScriptFormula(2), WaitSeconds(1.1f), false));
							else ally.World.BuffManager.GetFirstBuff<LawsResPassiveBuff>(ally).Extend(60);
						}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Resistance_All] -= Allres;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Rune E not complete
	/*
	rune E Stop Time
	{c_gold}Active:{/c_gold} Empowering the Law also causes all health loss and regeneration to pause 
	for {c_green}{Script Formula 18}{/c_green} seconds.
	*/
	#region LawsOfHope
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.LawsOfHope)]
	public class CrusaderLawsOfHope : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float durationBonus = 0f;
			if ((User as Player).SkillSet.HasPassive(310678)) durationBonus = 5f;   //Long Arm of the Law passive

			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (!HasBuff<LawsShieldBuff>(User))
				AddBuff(User, new LawsShieldBuff(ShieldTreshold(User.Attributes[GameAttributes.Level]), WaitSeconds(ScriptFormula(19) + durationBonus), true));

			foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
				if (!HasBuff<LawsShieldBuff>(ally))
					AddBuff(ally, new LawsShieldBuff(ShieldTreshold(User.Attributes[GameAttributes.Level]), WaitSeconds(ScriptFormula(19) + durationBonus), false));
			yield break;
		}

		private float ShieldTreshold(int level)  
		{
			return (10 + 0.005f * (float)Math.Pow(level, 4));
		}

		[ImplementsPowerBuff(7)]
		public class LawsShieldBuff : PowerBuff
		{
			public bool Primary = false;
			public float HPTreshold = 0;
			public bool HealPerWrath = false;
			public LawsShieldBuff(float hpTreshold, TickTimer timeout, bool primary = false)
			{
				HPTreshold = hpTreshold;
				Primary = primary;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0)         //Wings of Angels
				{
					Target.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(4);
					Target.Attributes[GameAttributes.Walk_Passability_Power_SNO] = 342279;
				}
				if (Rune_B > 0)         //Eternal hope
					Target.Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus] += ScriptFormula(8);
				if (Rune_C > 0)         //Hopeful cry
				{
					AttackPayload globeDrop = new AttackPayload(this);
					globeDrop.Targets = GetEnemiesInRadius(User.Position, 15f);
					globeDrop.OnHit = (hitPayload) =>
					{
						if (Rand.NextDouble() < ScriptFormula(13))
							User.World.SpawnHealthGlobe(hitPayload.Target, (User as Player), hitPayload.Target.Position);
					};
					globeDrop.Apply();
				}
				if (Rune_D > 0 && Primary)      //Faith's reward (done in Player)
					HealPerWrath = true;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (HPTreshold > 0)
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					{
						float dmg = (payload as HitPayload).TotalDamage;
						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= dmg;
						if (HPTreshold < 0) HPTreshold = -1f;
					}
			}
			public override void Remove()
			{
				base.Remove();

				if (Rune_A > 0)
				{
					Target.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(4);
					Target.Attributes[GameAttributes.Walk_Passability_Power_SNO] = -1;
				}
				if (Rune_B > 0)
					Target.Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus] -= ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(342299)]
	public class LawsOfHopePassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfJustice);
			RemoveBuffs(User, SkillsSystem.Skills.Crusader.Situational.LawsOfValor);
			AddBuff(User, new LawsRegenPassiveBuff(LifeRegen(User.Attributes[GameAttributes.Level]), null, true));
			yield break;
		}

		private float LifeRegen(int level)     
		{
			return (10 + 0.03f * (float)Math.Pow(level, 3));
		}

		[ImplementsPowerBuff(2)]
		class LawsRegenPassiveBuff : PowerBuff
		{
			TickTimer CheckTimer = null;
			public float Regen;
			public bool IsAura = false;
			public LawsRegenPassiveBuff(float regen, TickTimer timeout, bool isAura = false)
			{
				Regen = regen;
				IsAura = isAura;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] += Regen;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CheckTimer == null || CheckTimer.TimedOut)
				{
					CheckTimer = WaitSeconds(1f);

					if (IsAura)
						foreach (Actor ally in GetAlliesInRadius(User.Position, 60f).Actors)
						{
							if (!HasBuff<LawsRegenPassiveBuff>(ally))
								AddBuff(ally, new LawsRegenPassiveBuff(Regen, WaitSeconds(1.1f), false));
							else ally.World.BuffManager.GetFirstBuff<LawsRegenPassiveBuff>(ally).Extend(60);
						}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second_Bonus] -= Regen;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region HeavenFury
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.HeavenFury)]
	public class CrusaderHeavenFury : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_E > 0)         //Fires of Heaven
			{
				UsePrimaryResource(ScriptFormula(17));
				User.PlayEffectGroup(350406);
				yield return WaitSeconds(0.2f);
				WeaponDamage(GetEnemiesInBeamDirection(User.Position, TargetPosition, 40f, 8f), ScriptFormula(16), DamageType.Holy);
				yield break;
			}

			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			var groundZero = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f)));

			if (!User.World.CheckLocationForFlag(groundZero.Position, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

			if (Rune_C > 0)         //Split Fury (beams should not stack and intersect)
			{
				EffectActor[] beams = new EffectActor[3] { null, null, null };
				Vector3D[] beamPoints = PowerMath.GenerateSpreadPositions(groundZero.Position, groundZero.Position + new Vector3D(12, 0, 0), 120f, 3);

				if (User.World.CheckLocationForFlag(beamPoints[0], DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					beams[0] = SpawnEffect(ActorSno._x1_crusader_godray_proxy_wander_small, beamPoints[0], 0, WaitSeconds(ScriptFormula(0) + 1f));
					beams[0].UpdateDelay = 0.9f;
					beams[0].OnUpdate = () =>
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(beams[0].Position, 7f);
						attack.OnHit = (hitPayload) =>
						{
							if (!HasBuff<SplitDmgBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new SplitDmgBuff(ScriptFormula(11), WaitSeconds(1f)));
							else hitPayload.Target.World.BuffManager.GetFirstBuff<SplitDmgBuff>(hitPayload.Target).Extend(60);
						};
						attack.Apply();
					};
				}
				if (User.World.CheckLocationForFlag(beamPoints[1], DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					beams[1] = SpawnEffect(ActorSno._x1_crusader_godray_proxy_wander_small, beamPoints[1], 0, WaitSeconds(ScriptFormula(0) + 1f));
					beams[1].UpdateDelay = 0.9f;
					beams[1].OnUpdate = () =>
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(beams[1].Position, 7f);
						attack.OnHit = (hitPayload) =>
						{
							if (!HasBuff<SplitDmgBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new SplitDmgBuff(ScriptFormula(11), WaitSeconds(1f)));
							else hitPayload.Target.World.BuffManager.GetFirstBuff<SplitDmgBuff>(hitPayload.Target).Extend(60);
						};
						attack.Apply();
					};
				}
				if (User.World.CheckLocationForFlag(beamPoints[2], DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					beams[2] = SpawnEffect(ActorSno._x1_crusader_godray_proxy_wander_small, beamPoints[2], 0, WaitSeconds(ScriptFormula(0) + 1f));
					beams[2].UpdateDelay = 0.9f;
					beams[2].OnUpdate = () =>
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(beams[2].Position, 7f);
						attack.OnHit = (hitPayload) =>
						{
							if (!HasBuff<SplitDmgBuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new SplitDmgBuff(ScriptFormula(11), WaitSeconds(1f)));
							else hitPayload.Target.World.BuffManager.GetFirstBuff<SplitDmgBuff>(hitPayload.Target).Extend(60);
						};
						attack.Apply();
					};
				}

				var commandCenter = SpawnProxy(groundZero.Position, WaitSeconds(ScriptFormula(0)));
				commandCenter.UpdateDelay = 1f;
				commandCenter.OnUpdate = () =>
				{
					Actor[] assignedTargets = new Actor[3] { null, null, null };
					Actor moveTarget = null;
					for (int i = 0; i < 3; i++)
					{
						if (beams[i] == null) continue;

						moveTarget = GetEnemiesInRadius(beams[i].Position, 30f).GetClosestTo(beams[i].Position);
						if (moveTarget == null) continue;

						if (!(assignedTargets.Contains(moveTarget)))
						{
							bool assign = true;
							var proxies = moveTarget.GetActorsInRange<EffectActor>(7f);
							foreach (var proxy in proxies)
								if (proxy.SNO == ActorSno._x1_crusader_godray_proxy_wander_small) assign = false;

							if (assign) assignedTargets[i] = moveTarget;
						}
					}

					for (int i = 0; i < 3; i++)
						if (!(beams[i] == null) && !(assignedTargets[i] == null))
						{
							//Logger.Debug("Assigned Target {0} to beam {1} - {2}", assignedTargets[i].ActorSNO.Name, i, beams[i].ActorSNO.Name);
							beams[i].Move(assignedTargets[i].Position, MovementHelpers.GetFacingAngle(beams[i].Position, assignedTargets[i].Position));
						}
				};

				yield break;
			}

			//base effect
			var beam = SpawnEffect(
				RuneSelect(
					ActorSno._x1_crusader_godray_proxy_wander,
					ActorSno._x1_crusader_godray_proxy_wander_large,
					ActorSno._x1_crusader_godray_proxy_wander_dot,
					ActorSno._x1_crusader_godray_proxy_wander_small,
					ActorSno._x1_crusader_godray_proxy_wander_blocker,
					ActorSno.__NONE
				),
				groundZero.Position,
				0,
				WaitSeconds(ScriptFormula(0))
			);
			beam.UpdateDelay = 1f;
			beam.OnUpdate = () =>
			{
				AttackPayload laser = new AttackPayload(this);
				laser.Targets = GetEnemiesInRadius(beam.Position, Rune_A > 0 ? ScriptFormula(19) : 8f);
				laser.AddWeaponDamage(Rune_A > 0 ? ScriptFormula(20) : ScriptFormula(1), DamageType.Holy);
				laser.Apply();

				if (Rune_B > 0)     //Blessed Ground
				{
					var spawn = true;
					var proxies = beam.GetActorsInRange<EffectActor>(5f);
					foreach (var proxy in proxies)
						if (proxy.SNO == ActorSno._x1_crusader_heavensfury_groundpool) spawn = false;

					if (spawn)
					{
						var blessedPool = SpawnEffect(ActorSno._x1_crusader_heavensfury_groundpool, beam.Position, 0, WaitSeconds(ScriptFormula(7)));
						blessedPool.UpdateDelay = 0.9f;
						blessedPool.OnUpdate = () =>
						{
							AttackPayload attack = new AttackPayload(this);
							attack.Targets = GetEnemiesInRadius(blessedPool.Position, 4f);
							attack.OnHit = (hitPayload) =>
							{
								if (!HasBuff<PoolDmgBuff>(hitPayload.Target))
									AddBuff(hitPayload.Target, new PoolDmgBuff(ScriptFormula(6), WaitSeconds(1f)));
								else hitPayload.Target.World.BuffManager.GetFirstBuff<PoolDmgBuff>(hitPayload.Target).Extend(60);
							};
							attack.Apply();
						};
					}
				}

				if (Rune_D > 0)     //Thou Shalt Not Pass
				{
					var spawn = true;
					var proxies = beam.GetActorsInRange<EffectActor>(7f);
					foreach (var proxy in proxies)
						if (proxy.SNO == ActorSno._x1_crusader_heavensfury_groundpool) spawn = false;

					if (spawn)
					{
						var blessedPool = SpawnEffect(ActorSno._x1_crusader_heavensfury_groundpool, beam.Position, 0, WaitSeconds(ScriptFormula(7)));
						blessedPool.UpdateDelay = 1f;
						blessedPool.OnUpdate = () =>
						{
							AttackPayload push = new AttackPayload(this);
							push.Targets = GetEnemiesInRadius(blessedPool.Position, 4f);
							push.OnHit = (hitPayload) =>
							{
								if (!HasBuff<DirectedKnockbackBuff>(hitPayload.Target))
									AddBuff(hitPayload.Target, new DirectedKnockbackBuff(blessedPool.Position, 7f));
							};
							push.Apply();
						};
					}
				}

				var moveTarget = GetEnemiesInRadius(beam.Position, 30f).GetClosestTo(beam.Position);
				if (moveTarget == null) return;
				beam.Move(moveTarget.Position, MovementHelpers.GetFacingAngle(beam.Position, moveTarget.Position));
			};
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class SplitDmgBuff : PowerBuff
		{
			const float DamageRate = 1f;
			TickTimer DamageTimer = null;
			private float Multiplier = 0;
			public SplitDmgBuff(float multiplier, TickTimer timeout)
			{
				Timeout = timeout;
				Multiplier = multiplier;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DamageTimer == null || DamageTimer.TimedOut)
				{
					DamageTimer = WaitSeconds(DamageRate);
					WeaponDamage(Target, Multiplier, DamageType.Holy);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		class PoolDmgBuff : PowerBuff
		{
			const float DamageRate = 1f;
			TickTimer DamageTimer = null;
			private float Multiplier = 0;
			public PoolDmgBuff(float multiplier, TickTimer timeout)
			{
				Timeout = timeout;
				Multiplier = multiplier;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DamageTimer == null || DamageTimer.TimedOut)
				{
					DamageTimer = WaitSeconds(DamageRate);
					WeaponDamage(Target, Multiplier, DamageType.Holy);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region Bombardment
	[ImplementsPowerSNO(SkillsSystem.Skills.Crusader.Situational.Bombardment)]
	public class CrusaderBombardment : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			float cooldown = 60f;
			if ((User as Player).SkillSet.HasPassive(348741)) cooldown -= 20f;      //Lord Commander passive
			StartCooldown(cooldown);

			var groundZero = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 50f)), WaitSeconds(ScriptFormula(32)));

			if (Rune_D > 0)     //Impactful bombardment
			{
				var pending = SpawnProxy(groundZero.Position, WaitSeconds(1f));
				pending.PlayEffectGroup(357241);
				yield return WaitSeconds(0.8f);

				var impact = SpawnProxy(groundZero.Position, WaitSeconds(1f));
				impact.PlayEffectGroup(357272);
				yield return WaitSeconds(0.5f);

				WeaponDamage(GetEnemiesInRadius(groundZero.Position, 18f), ScriptFormula(3), DamageType.Physical);
				yield break;
			}

			if (Rune_E > 0)     //Targeted
			{
				for (int i = 0; i < 5; i++)
				{
					var singleTarget = GetEnemiesInRadius(groundZero.Position, 24f).GetClosestTo(groundZero.Position);
					if (singleTarget == null) yield break;

					var pending = SpawnProxy(singleTarget.Position, WaitSeconds(1f));
					pending.PlayEffectGroup(357434);
					yield return WaitSeconds(0.8f);

					var impact = SpawnProxy(singleTarget.Position, WaitSeconds(1f));
					impact.PlayEffectGroup(357421);
					yield return WaitSeconds(0.5f);

					WeaponDamage(GetEnemiesInRadius(singleTarget.Position, 6f), ScriptFormula(3), DamageType.Physical);
				}
				yield break;
			}

			Vector3D[] spread = PowerMath.GenerateSpreadPositions(groundZero.Position, groundZero.Position + new Vector3D(12, 0, 0), 72f, 5);
			if (Rune_C > 0)             //Mine Field
			{
				foreach (var point in spread)
				{
					var impact = SpawnProxy(point, WaitSeconds(1f));
					impact.PlayEffectGroup(327812);  
					yield return WaitSeconds(0.5f);

					Vector3D[] mineSpread = PowerMath.GenerateSpreadPositions(point, point + new Vector3D(5, 0, 0), 180f, 2);
					foreach (var minePoint in mineSpread)
					{
						var mine = new EffectActor(this, ActorSno._x1_crusader_trebuchet_mine, minePoint);
						mine.Timeout = WaitSeconds(8f);
						mine.Scale = 1f;
						mine.UpdateDelay = 1.5f;
						mine.OnUpdate = () =>
						{
							if (mine.UpdateDelay == 1.5f)   //2 sec delay after spawn
							{
								mine.UpdateDelay = 2f;
								return;
							}
							if (mine.UpdateDelay == 2f) mine.UpdateDelay = 1f;

							var targets = GetEnemiesInRadius(mine.Position, 5f).FilterByType<Monster>();
							if (targets.Actors.Count > 0 && targets != null)
							{
								mine.PlayEffectGroup(357450);
								WeaponDamage(GetEnemiesInRadius(mine.Position, ScriptFormula(26)), ScriptFormula(25), DamageType.Physical);
								mine.Destroy();
							}
						};
						mine.Spawn();
					}
				}

				yield break;
			}

			//base effect
			foreach (var point in spread)
			{
				var pending = SpawnProxy(point, WaitSeconds(1f));
				pending.PlayEffectGroup(RuneSelect(327812, 356717, 357944, -1, -1, -1));
				yield return WaitSeconds(0.8f);
				var impact = SpawnProxy(point, WaitSeconds(1f));
				impact.PlayEffectGroup(RuneSelect(293316, 356927, 293316, -1, -1, -1));
				yield return WaitSeconds(0.5f);
				if (Rune_A > 0) SpawnEffect(ActorSno._x1_crusader_trebuchet_tarpit, impact.Position, 0, WaitSeconds(2f));

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(point, 12f);
				attack.AddWeaponDamage(ScriptFormula(3), DamageType.Physical);
				if (Rune_B > 0)     //Annihilate
					attack.ChcBonus = 1f;       //will be capped to 85% anyway
				attack.OnHit = (hitPayload) =>
				{
					if (Rune_A > 0)     //Barrels of tar
						if (!HasBuff<DebuffSlowed>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(16), WaitSeconds(5f)));
				};
				attack.Apply();
			}
			yield break;
		}
	}
	#endregion
}
