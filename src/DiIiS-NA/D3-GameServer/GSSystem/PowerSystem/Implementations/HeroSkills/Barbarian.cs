//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	//Complete
	#region Bash
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.Bash)]
	public class BarbarianBash : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			var ShockWavePos = PowerMath.TranslateDirection2D(User.Position, TargetPosition,
															  User.Position,
															  ScriptFormula(23));
			var maxHits = 2;
			for (int i = 0; i < maxHits; ++i)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetBestMeleeEnemy();

				if (i == 0)
					attack.AddWeaponDamage(2.0f, Rune_D > 0 ? DamageType.Fire : DamageType.Physical);
				else
					attack.AddWeaponDamage(3.2f, Rune_D > 0 ? DamageType.Fire : DamageType.Physical);

				attack.OnHit = hitPayload =>
				{
					hitAnything = true;
					if (Rune_D > 0)
					{
						GeneratePrimaryResource(ScriptFormula(10));
					}
					if (Rune_B > 0)
					{
						AddBuff(User, new AddDamageBuff());
					}

					if (Rune_A > 0)
					{
					}
					else if (Rune_C > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(14))
							AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(15))));
					}
					else
					{
						if (Rand.NextDouble() < ScriptFormula(0))
							Knockback(hitPayload.Target, ScriptFormula(5), ScriptFormula(6), ScriptFormula(7));
					}
				};
				attack.Apply();
				yield return WaitSeconds(0.5f);

				if (hitAnything)
					GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit));
			}
			if (Rune_E > 0)
			{
				User.PlayEffectGroup(93867);
				yield return WaitSeconds(0.5f);
				WeaponDamage(GetEnemiesInBeamDirection(ShockWavePos, TargetPosition, 30f, 8f), 1f, DamageType.Fire);
			}
			yield break;
		}

		public override float GetContactDelay()
		{
			// seems to need this custom speed for all attacks
			return 0.5f;
		}

		[ImplementsPowerBuff(0, true)]
		public class AddDamageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(11));
				MaxStackCount = (int)ScriptFormula(4);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddDamage();
				return true;
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddDamage();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= StackCount * ScriptFormula(2);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= StackCount * ScriptFormula(2);
				User.Attributes.BroadcastChangedIfRevealed();
			}

			private void _AddDamage()
			{
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += ScriptFormula(2);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(2);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region LeapAttack
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.LeapAttack)]
	public class BarbarianLeap : Skill
	{
		//there is a changed walking speed multiplier from 8101 patch.
		public override IEnumerable<TickTimer> Main()
		{
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 50f));

			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;
			bool hitAnything = false;
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_C > 0)
			{
				AttackPayload launch = new AttackPayload(this);
				launch.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(31));
				launch.AddWeaponDamage(ScriptFormula(30), DamageType.Physical);
				launch.OnHit = hitPayload =>
				{
					hitAnything = true;
				};
				launch.Apply();
				User.PlayEffectGroup(165924); //Not sure if this is the only effect to be displayed in this case
			}

			ActorMover mover = new ActorMover(User);
			mover.MoveArc(TargetPosition, 10, -0.1f, new ACDTranslateArcMessage
			{
				//Field3 = 303110, // used for male barb leap, not needed?
				FlyingAnimationTagID = AnimationSetKeys.Attack2.ID,
				LandingAnimationTagID = -1,
				PowerSNO = PowerSNO
			});

			// wait for landing
			while (!mover.Update())
				yield return WaitTicks(1);

			// extra wait for leap to finish
			yield return WaitTicks(1);

			// ground smash effect
			User.PlayEffectGroup(162811);
			int enemiesHit = 0;

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(0));
			//ScriptFormula(1) states "% of willpower Damage", perhaps the damage should be calculated that way instead.
			attack.AddWeaponDamage(0.70f, DamageType.Physical);
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				enemiesHit++;
				if (Rune_E > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(37))
					{
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(38))));
					}
				}
			};
			attack.Apply();

			if (Rune_D > 0)
			{
				AddBuff(User, new LeapAttackArmorBuff(enemiesHit));
			}

			if (hitAnything)
				GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit));

			//TODO: Eventually att visuals, and check if the current uber-drag is really intended :P
			if (Rune_A > 0)
			{
				TargetList targets = GetEnemiesInRadius(User.Position, ScriptFormula(3));
				Actor curTarget;
				int affectedTargets = 0;
				while (affectedTargets < ScriptFormula(12)) //SF(11) states  "Min number to Knockback", and is 5, what can that mean?
				{
					curTarget = targets.GetClosestTo(User.Position);
					if (curTarget != null)
					{
						targets.Actors.Remove(curTarget);

						if (curTarget.World != null)
						{
							Knockback(curTarget, -25f, ScriptFormula(9), ScriptFormula(10));
						}
						affectedTargets++;
					}
					else
					{
						break;
					}
				}
			}

			if (Rune_B > 0)
			{
				TargetList targets = GetEnemiesInRadius(User.Position, ScriptFormula(3));
				foreach (Actor curTarget in targets.Actors)
				{
					Knockback(curTarget, ScriptFormula(17), ScriptFormula(18), ScriptFormula(19));
				}
			}
			yield break;
		}
		[ImplementsPowerBuff(2)]
		class LeapAttackArmorBuff : PowerBuff
		{
			int EnemiesHit = 1;
			public LeapAttackArmorBuff(int enemies)
			{
				this.EnemiesHit = enemies;
			}

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(36));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Armor_Bonus_Percent] += (ScriptFormula(33) * this.EnemiesHit);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				User.Attributes[GameAttribute.Armor_Bonus_Percent] -= (ScriptFormula(33) * this.EnemiesHit);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region WhirlWind
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.Whirlwind)]
	public class BarbarianWhirlwind : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new WhirlwindEffect());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class WhirlwindEffect : PowerBuff
		{
			private TickTimer _damageTimer = null;
			private TickTimer _tornadoSpawnTimer = null;
			private TickTimer _pullTimer = null;
			private float _damageDelay = 0f;
			private float _damageMult = 1f;
			private DamageType _dmgType = DamageType.Physical;

			public override void Init() //resolved all SF for better performance
			{
				_damageMult = Rune_A > 0 ? 3.245f : 2.75f;      //Volcanic Eruption
				_dmgType = Rune_A > 0 ? DamageType.Fire : (Rune_D > 0 ? DamageType.Lightning : DamageType.Physical);
				_damageDelay = Math.Max(1f / (User.Attributes[GameAttribute.Attacks_Per_Second_Total] * 1.3f), 0.3f);
				Timeout = WaitSeconds(_damageDelay);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//User.Attributes[GameAttribute.Running_Rate] = User.Attributes[GameAttribute.Running_Rate] * EvalTag(PowerKeys.WalkingSpeedMultiplier);
				User.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += 0.35f;
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 0.35f;
				//User.Attributes[GameAttribute.Running_Rate] = User.Attributes[GameAttribute.Running_Rate] / EvalTag(PowerKeys.WalkingSpeedMultiplier);
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageDelay);
					UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, 6f);
					attack.AddWeaponDamage(_damageMult, _dmgType);
					attack.OnHit = (hitPayload) =>
					{
						if (Rune_D > 0 && Rand.NextDouble() < 0.13f)        //Wind Shear
							GeneratePrimaryResource(1f);

						if (Rune_E > 0 && hitPayload.IsCriticalHit)     //Blood Funnel
							if (User is Player)
								(User as Player).AddPercentageHP(1);
					};
					attack.Apply();
				}

				if (Rune_B > 0)     //Dust Devils
				{
					if (_tornadoSpawnTimer == null)
						_tornadoSpawnTimer = WaitSeconds(0.75f);

					if (_tornadoSpawnTimer.TimedOut)
					{
						_tornadoSpawnTimer = WaitSeconds(0.75f);

						var tornado = new Projectile(this, 162386, User.Position);
						tornado.Timeout = WaitSeconds(3f);
						tornado.OnCollision = (hit) =>
						{
							WeaponDamage(hit, 0.8f, DamageType.Physical);
						};
						tornado.Launch(new Vector3D(User.Position.X + (float)Rand.NextDouble() - 0.5f,
													User.Position.Y + (float)Rand.NextDouble() - 0.5f,
													User.Position.Z), 0.25f);
					}
				}

				if (Rune_C > 0)     //Hurricane
				{
					if (_pullTimer == null)
						_pullTimer = WaitSeconds(1f);

					if (_pullTimer.TimedOut)
					{
						_pullTimer = WaitSeconds(1f);

						foreach (var target in GetEnemiesInRadius(User.Position, 35f).Actors)
							if (!HasBuff<KnockbackBuff>(target))
								AddBuff(target, new KnockbackBuff(-20f));
					}
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region AncientSpear
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.AncientSpear)]
	public class BarbarianAncientSpear : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			bool hitAnything = false;

			if (Rune_B > 0)
			{
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, ScriptFormula(9) / 5f, (int)ScriptFormula(11));

				for (int i = 0; i < projDestinations.Length; i++)
				{
					var proj = new Projectile(this, 161891, User.Position);
					proj.Scale = 3f;
					proj.Timeout = WaitSeconds(0.5f);
					proj.OnCollision = (hit) =>
					{
						hitAnything = true;
						_setupReturnProjectile(hit.Position);

						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(hit);
						attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
						attack.OnHit = (hitPayload) =>
						{
							hitPayload.Target.PlayEffectGroup(79420);
							Knockback(hitPayload.Target, -25f, ScriptFormula(3), ScriptFormula(4));
							if ((User as Player).SkillSet.HasPassive(204725) && hitPayload.IsCriticalHit)
								foreach (var cdBuff in User.World.BuffManager.GetBuffs<PowerSystem.Implementations.CooldownBuff>(User))
									if (cdBuff.TargetPowerSNO == 69979)
										cdBuff.Remove();
						};
						attack.Apply();

						proj.Destroy();
					};
					proj.OnTimeout = () =>
					{
						_setupReturnProjectile(proj.Position);
					};

					proj.Launch(projDestinations[i], ScriptFormula(8));
					User.AddRopeEffect(79402, proj);
				}
			}
			else if (Rune_E > 0)
			{
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, ScriptFormula(9) / 5f, (int)ScriptFormula(11));

				for (int i = 0; i < projDestinations.Length; i++)
				{
					var proj = new Projectile(this, 161894, User.Position);
					proj.Scale = 3f;
					proj.Timeout = WaitSeconds(0.5f);
					proj.OnCollision = (hit) =>
					{
						hitAnything = true;

						_setupReturnProjectile(hit.Position);

						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(hit);
						attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
						attack.OnHit = (hitPayload) =>
						{
							hitPayload.Target.PlayEffectGroup(79420);
							Knockback(hitPayload.Target, 25f, ScriptFormula(3), ScriptFormula(4));
						};
						attack.Apply();

						proj.Destroy();
					};
					proj.OnTimeout = () =>
					{
						_setupReturnProjectile(proj.Position);
					};

					proj.Launch(projDestinations[i], ScriptFormula(8));
					User.AddRopeEffect(79402, proj);
				}
			}
			else if (Rune_A > 0)
			{
				var projectile = new Projectile(this, 161890, User.Position);
				projectile.Scale = 3f;
				projectile.Timeout = WaitSeconds(0.5f);
				projectile.OnCollision = (hit) =>
				{
					hitAnything = true;

					_setupReturnProjectile(hit.Position);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
					attack.OnHit = (hitPayload) =>
					{
						for (int i = 0; i < ScriptFormula(17); ++i)
						{
							hitPayload.Target.PlayEffectGroup(79420);
							Knockback(hitPayload.Target, -25f, ScriptFormula(3), ScriptFormula(4));
						}
					};
					attack.Apply();

				};
				projectile.OnTimeout = () =>
				{
					_setupReturnProjectile(projectile.Position);
				};

				projectile.Launch(TargetPosition, ScriptFormula(8));
				User.AddRopeEffect(79402, projectile);
			}
			else
			{
				var projectile = new Projectile(this, RuneSelect(74636, -1, -1, 161892, 161893, -1), User.Position);
				projectile.Scale = 3f;
				projectile.Timeout = WaitSeconds(0.5f);
				projectile.OnCollision = (hit) =>
				{
					hitAnything = true;

					_setupReturnProjectile(hit.Position);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage(ScriptFormula(0), Rune_C > 0 ? DamageType.Fire : DamageType.Physical);
					attack.OnHit = (hitPayload) =>
					{
						// GET OVER HERE
						//unknown on magnitude/knockback offset?
						hitPayload.Target.PlayEffectGroup(79420);
						Knockback(hitPayload.Target, -25f, ScriptFormula(3), ScriptFormula(4));
						if (Rune_C > 0)
						{
							float healMe = ScriptFormula(10) * hitPayload.TotalDamage;
							if (User is Player)
								(User as Player).AddHP(healMe);
							//User.Attributes[GameAttribute.Hitpoints_Granted] = healMe;
							//User.Attributes.BroadcastChangedIfRevealed();
						}
					};
					attack.Apply();

					projectile.Destroy();
				};
				projectile.OnTimeout = () =>
				{
					_setupReturnProjectile(projectile.Position);
				};

				projectile.Launch(TargetPosition, ScriptFormula(8));
				User.AddRopeEffect(79402, projectile);
			}

			if (hitAnything)
				GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit));

			yield break;
		}

		private void _setupReturnProjectile(Vector3D spawnPosition)
		{
			Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, spawnPosition, User.Position, 5f);

			var return_proj = new Projectile(this, 79400, new Vector3D(spawnPosition.X, spawnPosition.Y, User.Position.Z));
			return_proj.Scale = 3f;
			return_proj.DestroyOnArrival = true;
			return_proj.LaunchArc(inFrontOfUser, 1f, -0.03f);
			User.AddRopeEffect(79402, return_proj);
		}
	}
	#endregion

	//Complete
	#region ThreateningShout
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.ThreateningShout)]
	public class ThreateningShout : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			GeneratePrimaryResource(ScriptFormula(13));
			if ((User as Player).SkillSet.HasPassive(205546))
				foreach (var plr in User.World.Players.Values)
					AddBuff(plr, new InspiringPresenceBuff());

			foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(9)).Actors)
			{
				AddBuff(enemy, new ShoutDeBuff());
				if (Rune_D > 0)
				{
					AddBuff(enemy, new ShoutAttackDeBuff());
				}
				if (Rune_A > 0)
				{
					AddBuff(enemy, new DebuffTaunted(WaitSeconds(ScriptFormula(8))));
				}
				if (Rune_E > 0)
				{
					//Script(10) -> Fear Death Effect Duration? what is this for..
					if (Rand.NextDouble() < ScriptFormula(3))
					{
						AddBuff(enemy, new DebuffFeared(WaitSeconds(Rand.Next((int)ScriptFormula(5), (int)ScriptFormula(5) + (int)ScriptFormula(6)))));
					}
				}
			}

			yield break;
		}
		[ImplementsPowerBuff(0)]
		public class ShoutDeBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(2));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] += ScriptFormula(0);
				if (Rune_B > 0)
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += ScriptFormula(14);
				}
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is DeathPayload && payload.Target == Target)
				{
					if (Rune_C > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(7))
						{
							if (User is Player)
								payload.Target.World.SpawnHealthGlobe(payload.Target, (User as Player), payload.Target.Position);
						}
					}
				}
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Damage_Done_Reduction_Percent] -= ScriptFormula(0);
				if (Rune_B > 0)
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= ScriptFormula(14);
				}
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(4)]
		public class ShoutAttackDeBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(17));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= ScriptFormula(4);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

	}
	#endregion

	//Complete
	#region HammerOfTheAncients
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.HammerOfTheAncients)]
	public class HammerOfTheAncients : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if (Rune_B > 0)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(14), ScriptFormula(15));
				attack.AddWeaponDamage(ScriptFormula(23), DamageType.Physical);
				attack.Apply();
				yield break;
			}
			else
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(11));

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(11));
				attack.AddWeaponDamage(ScriptFormula(4), Rune_C > 0 ? DamageType.Cold : (Rune_E > 0 ? DamageType.Lightning : DamageType.Fire));
				attack.OnHit = hitPayload =>
				{
					if (Rune_D > 0)
					{
						if (hitPayload.IsCriticalHit)
						{
							if (Rand.NextDouble() < ScriptFormula(5))
							{
							}
						}
					}
					if (Rune_C > 0)
					{
						AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(8), WaitSeconds(ScriptFormula(10))));
					}
				};
				attack.OnDeath = DeathPayload =>
				{
					if (Rune_E > 0)
					{
						//if (DeathPayload.Target)? Doesn't above handle this?
						{
							if (Rand.NextDouble() < ScriptFormula(16))
							{
								AttackPayload Stunattack = new AttackPayload(this);
								Stunattack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(18));
								Stunattack.OnHit = stun =>
								{
									AddBuff(stun.Target, new DebuffStunned(WaitSeconds(ScriptFormula(17))));
								};
								Stunattack.Apply();
							}
						}
					}
				};
				attack.Apply();

				if (Rune_C > 0)
				{
					var QuakeHammer = SpawnEffect(159030, User.Position, 0, WaitSeconds(ScriptFormula(10)));
					QuakeHammer.UpdateDelay = 1f;
					QuakeHammer.OnUpdate = () =>
					{
						AttackPayload TremorAttack = new AttackPayload(this);
						TremorAttack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(7));
						TremorAttack.AddWeaponDamage(ScriptFormula(9), DamageType.Physical);
						TremorAttack.Apply();
					};
				}
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region BattleRage
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.BattleRage)]
	public class BattleRage : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new BattleRageEffect());

			if ((User as Player).SkillSet.HasPassive(205546))
				foreach (var plr in User.World.Players.Values)
					AddBuff(plr, new InspiringPresenceBuff());

			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class BattleRageEffect : PowerBuff
		{
			private TickTimer _damageTimer;
			private float ChCbonus = 0f;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//Rune_A
				//Total Damage Bonus
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += ScriptFormula(1);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(1);

				//Crit Chance Bonus
				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] += (int)ScriptFormula(2);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(0.5f);

					if (Rune_D > 0)
					{
						User.Attributes[GameAttribute.Weapon_Crit_Chance] -= ChCbonus;
						ChCbonus = 0.01f * GetEnemiesInRadius(User.Position, 10f).Actors.Count;
						User.Attributes[GameAttribute.Weapon_Crit_Chance] += ChCbonus;
					}
				}

				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == Target)
				{
					if ((payload as HitPayload).IsCriticalHit && (payload as HitPayload).AutomaticHitEffects && Rand.NextDouble() < 0.5)
					{
						if (Rune_B > 0)
						{
							if (Rand.NextDouble() < ScriptFormula(5))
							{
								//ScriptFormula(4) -> extends duration 
								this.Extend((int)ScriptFormula(4) * 60);
							}
						}
						if (Rune_C > 0)
						{
							//or ScriptFormula(15)
							if (Rand.NextDouble() < ScriptFormula(6))
							{
								//drop additional health globes.
								if (Target is Player)
									payload.Target.World.SpawnHealthGlobe(payload.Target, (Target as Player), payload.Target.Position);
							}
						}
						if (Rune_E > 0)
						{
							User.PlayEffectGroup(210321);
							AttackPayload attack = new AttackPayload(this);
							attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(9));
							attack.AddWeaponDamage(ScriptFormula(10), DamageType.Physical);
							attack.AutomaticHitEffects = false;
							attack.Apply();
						}
					}
				}
			}

			public override void Remove()
			{
				base.Remove();
				//Total Damage Bonus
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= ScriptFormula(1);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(1);

				//Crit Chance Bonus
				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] -= (int)ScriptFormula(2);
				User.Attributes[GameAttribute.Weapon_Crit_Chance] -= ChCbonus;
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Cleave
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.Cleave)]
	public class Cleave : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			WeaponDamage(GetBestMeleeEnemy(), ScriptFormula(16), DamageType.Physical);
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(26), ScriptFormula(27));
			attack.AddWeaponDamage(ScriptFormula(16), Rune_A > 0 ? DamageType.Lightning : (Rune_D > 0 ? DamageType.Fire : DamageType.Physical));
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				if (Rune_B > 0)
				{
					AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(20), WaitSeconds(ScriptFormula(14))));
				}
				if (Rune_C > 0)
				{
					if (hitPayload.IsCriticalHit)
					{
						Knockback(hitPayload.Target, ScriptFormula(2), ScriptFormula(3));

						//since its a max number of knockback jumps, but its UP TO that number, we will randomize it.
						int Jumps = Rand.Next((int)ScriptFormula(25));
						for (int i = 0; i < Jumps; ++i)
						{
							WeaponDamage(hitPayload.Target, ScriptFormula(9), DamageType.Physical);
							Knockback(hitPayload.Target, ScriptFormula(7), ScriptFormula(8));
						}
					}
				}
				if (Rune_D > 0)
				{
					GeneratePrimaryResource(ScriptFormula(22));
				}
			};
			if (Rune_E > 0)
			{
				attack.OnDeath = DeathPayload =>
				{
					DeathPayload.Target.PlayEffectGroup(161045);
					AttackPayload explode = new AttackPayload(this);
					explode.Targets = GetEnemiesInRadius(DeathPayload.Target.Position, ScriptFormula(6));
					explode.AddWeaponDamage(ScriptFormula(4), DamageType.Fire);
					explode.Apply();
				};
			}
			attack.Apply();

			if (hitAnything)
				GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit));

			yield break;
		}
	}
	#endregion

	//Complete
	#region IgnorePain
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.IgnorePain)]
	public class IgnorePain : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new IgnorePainBuff());
			if (Rune_C > 0)
			{
				foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(4)).Actors)
					AddBuff(ally, new ObsidianAlliesBuff());
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class IgnorePainBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(2);
				Target.Attributes.BroadcastChangedIfRevealed();
				if (Rune_D > 0)
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(8));
					attack.AddWeaponDamage(ScriptFormula(11), DamageType.Physical);
					attack.OnHit = hitPayload =>
					{
						Knockback(hitPayload.Target, ScriptFormula(6), ScriptFormula(7));
					};
					attack.Apply();
				}
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == Target)
				{
					if (Rune_A > 0)
					{
						Damage(payload.Context.User,
							(((payload as HitPayload).TotalDamage / (1 - ScriptFormula(2))) - (payload as HitPayload).TotalDamage) * ScriptFormula(9),
							0f,
							DamageType.Physical);
					}
				}
			}
			//OnPayload -> Rune_E
			//Calculate Hit Total from mob and
			//float healMe = ScriptFormula(1) * hitPayload.TotalDamage;
			//User.Attributes[GameAttribute.Hitpoints_Granted] = healMe;
			//User.Attributes.BroadcastChangedIfRevealed();



			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(2);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(1)]
		public class ObsidianAlliesBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(3));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] -= Convert.ToInt32(ScriptFormula(10));
				Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] -= Convert.ToInt32(ScriptFormula(10));
				Target.Attributes.BroadcastChangedIfRevealed();


				return true;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] -= Convert.ToInt32(ScriptFormula(10));
				Target.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] -= Convert.ToInt32(ScriptFormula(10));
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region WeaponThrow
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.WeaponThrow)]
	public class WeaponThrow : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			GeneratePrimaryResource(ScriptFormula(18));

			float _resourcePool = User.Attributes[GameAttribute.Resource_Cur, 2];

			var proj = new Projectile(this, RuneSelect(100800, 100839, 166438, 100832, 101057, 100934), User.Position);
			proj.Position.Z += 5f;  // fix height
			proj.OnCollision = (hit) =>
			{
				hit.PlayEffectGroup(RuneSelect(18707, 166333, 16634, 166335, -1, 166339));
				AttackPayload attack = new AttackPayload(this);
				attack.SetSingleTarget(hit);
				attack.AddWeaponDamage(ScriptFormula(15), RuneSelect(DamageType.Physical, DamageType.Lightning, DamageType.Fire, DamageType.Physical, DamageType.Fire, DamageType.Physical));
				attack.OnHit = (hitPayload) =>
				{
					if ((User as Player).SkillSet.HasPassive(204725) && hitPayload.IsCriticalHit)
						GeneratePrimaryResource(15f);
				};
				attack.Apply();

				if (Rune_B > 0)
				{
					//once collision with target, RopeEffect up to ScriptFromula(5) times.
					Actor curSource = hit;
					var enemies = GetEnemiesInRadius(curSource.Position, ScriptFormula(12)).Actors.Where(actor => actor != curSource).ToList();
					Actor curTarget = (enemies.Count > 0 ? enemies[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, enemies.Count)] : null);
					for (int i = 0; i <= ScriptFormula(5); i++)
					{
						if (curTarget == null) break;
						curSource.AddRopeEffect(166450, curTarget);
						curSource = curTarget;

						AttackPayload ricochet_attack = new AttackPayload(this);
						ricochet_attack.AddWeaponDamage(ScriptFormula(15), DamageType.Physical);
						ricochet_attack.SetSingleTarget(curTarget);
						ricochet_attack.Apply();

						var next_enemies = GetEnemiesInRadius(curSource.Position, ScriptFormula(12)).Actors.Where(actor => actor != curSource).ToList();
						curTarget = (next_enemies.Count > 0 ? next_enemies[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, next_enemies.Count)] : null);
					}
				}
				if (Rune_D > 0)
				{
					//Use remaining Fury points. Get Remaining Fury Points, use that to multiply against SF(18), and add that to total damage.

					WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(19)), (ScriptFormula(18) * _resourcePool), DamageType.Physical);
				}
				if (Rune_C > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(6))
					{
						AddBuff(hit, new DebuffStunned(WaitSeconds(ScriptFormula(7))));
					}
				}
				if (Rune_E > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(0))
					{
						AddBuff(hit, new ConfuseDebuff());
						//This will cause a buff on the target hit, changing their attack from the User, to other Mobs.
					}
				}
				proj.Destroy();
			};
			if (Rune_D > 0)
			{
				UsePrimaryResource(_resourcePool);
			}
			proj.Launch(TargetPosition, 1.5f);
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class ConfuseDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(10));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Team_Override] = 1;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Team_Override] = 10;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region GroundStomp
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.GroundStomp)]
	public class GroundStomp : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			User.PlayEffectGroup(RuneSelect(18685, 99685, 159415, 159416, 159397, 18685));
			//Rune_E -> when stun wears off, use slow.efg

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(0));
			attack.AddWeaponDamage(ScriptFormula(6), Rune_E > 0 ? DamageType.Cold : (Rune_A > 0 ? DamageType.Fire : DamageType.Physical));
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				if (Rune_B > 0)
				{
					//push em away!
					Knockback(hitPayload.Target, ScriptFormula(13), ScriptFormula(14));
				}
				if (Rune_C > 0)
				{
					//bring em in!
					Knockback(hitPayload.Target, ScriptFormula(11), ScriptFormula(12));
				}
				AddBuff(hitPayload.Target, new GroundStompStun());
			};
			attack.Apply();
			if (hitAnything)
				GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit) + ScriptFormula(19));

			yield break;
		}
		[ImplementsPowerBuff(0)]
		public class GroundStompStun : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				new DebuffStunned(WaitSeconds(ScriptFormula(1)));

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_E > 0)
				{
					new GroundStompSlow();
				}
			}
		}
		[ImplementsPowerBuff(0)]
		public class GroundStompSlow : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(4));
				Target.PlayEffectGroup(159418);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Slow] = true;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += ScriptFormula(5);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Slow] = false;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= ScriptFormula(5);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Rend
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.Rend)]
	public class Rend : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(4));
			attack.OnHit = hitPayload =>
			{
				AddBuff(hitPayload.Target, new RendDebuff());
				if (Rune_D > 0)
				{
					float healMe = ScriptFormula(8) * hitPayload.TotalDamage;
					if (User is Player) //not sure about it
						(User as Player).AddHP(healMe);
					//User.Attributes[GameAttribute.Hitpoints_Granted] = healMe;
					//User.Attributes.BroadcastChangedIfRevealed();
				}
			};
			//this work? if it dies with rend debuff, infect others.
			attack.Apply();
			yield break;
		}
		[ImplementsPowerBuff(0, true)]
		public class RendDebuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
				MaxStackCount = (int)ScriptFormula(10);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				if (Rand.NextDouble() < ScriptFormula(9))
				{
					//for rend, would we just stack updates?
					Update();
				}
				return true;
			}


			public override void OnPayload(Payload payload)
			{
				if (payload is DeathPayload && payload.Target == Target)
				{
					if (new System.Diagnostics.StackTrace().FrameCount < 15)
						if (Rune_E > 0)
						{
							foreach (Actor newTarget in GetEnemiesInRadius(Target.Position, ScriptFormula(19)).Actors)
								AddBuff(newTarget, new RendDebuff());
						}
				}
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);
					WeaponDamage(Target, ScriptFormula(20), DamageType.Physical);
				}
				return false;
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					Update();

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
	#region Frenzy
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.Frenzy)]
	public class Frenzy : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			bool hitAnything = false;
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetBestMeleeEnemy();
			attack.AddWeaponDamage(2f, Rune_A > 0 ? DamageType.Fire : (Rune_D > 0 ? DamageType.Lightning : DamageType.Physical));
			attack.OnHit = hitPayload =>
			{
				hitAnything = true;
				AddBuff(User, new FrenzyBuff());
				if (Rune_C > 0)
				{
					AddBuff(User, new ObsidianSpeedEffect());
				}
				if (Rune_B > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(17))
					{
						Actor target = GetEnemiesInRadius(User.Position, 15f).GetClosestTo(User.Position);

						var proj = new Projectile(this, RuneSelect(6515, 130073, 215555, -1, 216040, 75650), User.Position);
						proj.Position.Z += 5f;  // fix height
						proj.OnCollision = (hit) =>
						{
							WeaponDamage(hit, ScriptFormula(18), DamageType.Physical);
						};
						proj.Launch(target.Position, 2f);
					}
				}
				if (Rune_D > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(1))
					{
						hitPayload.Target.PlayEffectGroup(163470);
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(0))));
					}
				}
			};
			attack.OnDeath = DeathPayload =>
			{
				if (Rune_E > 0)
				{
					//User.Attributes[GameAttribute.Hitpoints_Granted_Duration] += (int)ScriptFormula(12);
					//User.Attributes[GameAttribute.Hitpoints_Granted] += ScriptFormula(10) * User.Attributes[GameAttribute.Hitpoints_Max_Total];
					if (User is Player) //not sure about it
						(User as Player).AddHP(ScriptFormula(10) * User.Attributes[GameAttribute.Hitpoints_Max_Total]);  //TODO: regen on 6 seconds
					User.Attributes.BroadcastChangedIfRevealed();
				}
			};
			attack.Apply();
			if (hitAnything)
				GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit));
			yield break;
		}
		[ImplementsPowerBuff(0, true)]
		class FrenzyBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
				MaxStackCount = (int)ScriptFormula(3);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddFrenzy();
				return true;
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddFrenzy();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_A > 0)
				{
					User.Attributes[GameAttribute.Amplify_Damage_Percent] -= StackCount * ScriptFormula(11);
				}
				User.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= StackCount * ScriptFormula(6);
				User.Attributes.BroadcastChangedIfRevealed();

			}

			private void _AddFrenzy()
			{
				if (Rune_A > 0)
				{
					User.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(11);
				}
				User.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += ScriptFormula(6);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
		//powerbuff(1) = Healing Over Time buff

		[ImplementsPowerBuff(3)]
		class ObsidianSpeedEffect : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(8);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(8);
				User.Attributes.BroadcastChangedIfRevealed();

			}
		}
	}
	#endregion

	//Complete
	#region Revenge
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.Revenge)]
	public class Revenge : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			Target.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.Barbarian.Situational.Revenge] = 0;
			User.Attributes.BroadcastChangedIfRevealed();

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(1));
			attack.AddWeaponDamage(ScriptFormula(2), Rune_C > 0 ? DamageType.Lightning : (Rune_A > 0 ? DamageType.Fire : DamageType.Physical));
			attack.OnHit = hitPayload =>
			{
				if (User is Player)
					(User as Player).AddHP(User.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(4));

				if (Rune_D > 0)
					GeneratePrimaryResource(ScriptFormula(6));

				if (Rune_C > 0)
					Knockback(hitPayload.Target, ScriptFormula(8));
			};
			attack.Apply();

			if (Rune_E > 0)
				AddBuff(User, new RevengeCritBuff());

			yield break;
		}

		[ImplementsPowerBuff(2)]
		public class RevengeCritBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(20));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Weapon_Crit_Chance] += ScriptFormula(7);
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Weapon_Crit_Chance] -= ScriptFormula(7);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}

		
	}

	[ImplementsPowerSNO(109344)]
	public class RevengeBuff : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new BarbarianRevengeBuff());
			yield break;
		}

		[ImplementsPowerBuff(0, true)]
		public class BarbarianRevengeBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				MaxStackCount = (int)ScriptFormula(3);
			}

			public override void OnPayload(Payload payload)
			{
				base.OnPayload(payload);

				if (payload.Target == User && payload is HitPayload)
				{
					if (Rand.NextDouble() < 0.15)
					{
						User.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.Barbarian.Situational.Revenge] = 1;
						User.Attributes.BroadcastChangedIfRevealed();
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region WarCry
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.WarCry)]
	public class WarCry : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			GeneratePrimaryResource(ScriptFormula(3));

			AddBuff(User, new WarCryBuff());

			if ((User as Player).SkillSet.HasPassive(205546))
				foreach (var plr in User.World.Players.Values)
					AddBuff(plr, new InspiringPresenceBuff());

			foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(11)).Actors)
			{
				AddBuff(ally, new WarCryBuff());
			}

			if ((User as Player).ActiveHireling != null)
				(User as Player).GrantAchievement(74987243307045);

			yield return WaitSeconds(0.5f);
		}
		//4 different powerbuffs, figure out which one is which.
		[ImplementsPowerBuff(0)]
		class WarCryBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(2));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(0);
				if (Rune_B > 0)
				{
					User.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(14);
				}
				if (Rune_C > 0)
				{
					User.Attributes[GameAttribute.Resistance_Percent_All] += ScriptFormula(4);
				}
				if (Rune_E > 0)
				{
					User.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] += ScriptFormula(5);
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(6);
				}
				//User.Attributes[GameAttribute.Defense_Bonus_Percent] += ScriptFormula(0);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				User.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(0);
				if (Rune_B > 0)
				{
					User.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(14);
				}
				if (Rune_C > 0)
				{
					User.Attributes[GameAttribute.Resistance_Percent_All] -= ScriptFormula(4);
				}
				if (Rune_E > 0)
				{
					User.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] -= ScriptFormula(5);
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(6);
				}
				//User.Attributes[GameAttribute.Defense_Bonus_Percent] -= ScriptFormula(0);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//still terrible. needs to be redone.
	#region FuriousCharge
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FuryGenerators.FuriousCharge)]
	public class FuriousCharge : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Max(Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 35f), EvalTag(PowerKeys.WalkingDistanceMin)));

			User.Attributes[GameAttribute.Skill_Charges, PowerSNO] -= 1;

			var dashBuff = new DashMoverBuff(MovementHelpers.GetCorrectPosition(User.Position, TargetPosition, User.World));
			AddBuff(User, dashBuff);
			yield return dashBuff.Timeout;

			GeneratePrimaryResource(EvalTag(PowerKeys.ResourceGainedOnFirstHit)); //since its furygenerator -> this should be fine without being the first hit.

			yield return WaitSeconds(0.1f);
			User.PlayEffectGroup(166193);
			foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(6)).Actors)
			{
				WeaponDamage(enemy, ScriptFormula(14), Rune_A > 0 ? DamageType.Fire : DamageType.Physical);
				Knockback(enemy, ScriptFormula(10));
			}

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class DashMoverBuff : PowerBuff
		{
			const float _damageRate = 0.05f;
			TickTimer _damageTimer = null;

			float cdReduce = 0;
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

				float speed = User.Attributes[GameAttribute.Running_Rate_Total] * EvalTag(PowerKeys.WalkingSpeedMultiplier);

				User.TranslateFacing(_destination, true);
				_mover = new ActorMover(User);
				_mover.Move(_destination, speed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = 69808,
				});

				// make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
				//TickTimer minDashWait = WaitSeconds(0.15f);
				Timeout = _mover.ArrivalTime;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				StartCooldown(EvalTag(PowerKeys.CooldownTime) - cdReduce);
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);
					AttackPayload attack = new AttackPayload(this);
					attack.AddWeaponDamage(ScriptFormula(16), DamageType.Physical);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(6));
					attack.OnHit = hit =>
					{
						Knockback(hit.Target, ScriptFormula(2));
						if (Rune_B > 0)
						{
							if (User is Player)
								(User as Player).AddHP(ScriptFormula(21) * User.Attributes[GameAttribute.Hitpoints_Max_Total]);
						}
						if (Rune_D > 0)
						{
							GeneratePrimaryResource(ScriptFormula(11));
						}
						if (Rune_C > 0)
						{
							if (hit.IsCriticalHit)
							{
								AddBuff(hit.Target, new DebuffStunned(WaitSeconds(ScriptFormula(8))));
							}
						}
						if (Rune_E > 0)
						{
							if (cdReduce < ScriptFormula(10))
								cdReduce += ScriptFormula(12);
						}
					};
					attack.Apply();
				}
				return false;
			}
		}
		[ImplementsPowerBuff(1)]
		public class FuriousChargeCountBuff : PowerBuff
		{
			public bool CoolDownStarted = false;
			public uint Max = 1;

			public override bool Update()
			{
				if (base.Update())
					return true;



				if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] < Max)
				{
					if (!CoolDownStarted)
					{
						StartCooldownCharges(10f); CoolDownStarted = true;

						Task.Delay(10200).ContinueWith(delegate
						{
							CoolDownStarted = false;
							User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = (int)Math.Min(User.Attributes[GameAttribute.Skill_Charges, PowerSNO] + 1, Max);
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
	#region Overpower
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.Overpower)]
	public class Overpower : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			float cooldownReduce = ScriptFormula(27);

			if (Rune_A > 0)
			{
				AddBuff(User, new DurationBuff());
			}
			if (Rune_E > 0)
			{
				AddBuff(User, new ReflectBuff());
			}
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(7));
			attack.AddWeaponDamage(ScriptFormula(6), Rune_C > 0 ? DamageType.Fire : (Rune_A > 0 ? DamageType.Lightning : DamageType.Physical));
			attack.OnHit = HitPayload =>
			{
				if (Rune_D > 0)
				{
					GeneratePrimaryResource(ScriptFormula(28));
				}
				if (HitPayload.IsCriticalHit)
				{
					cooldownReduce -= 1f;
				}
			};
			StartCooldown(cooldownReduce);
			attack.Apply();

			if (Rune_B > 0)
			{
				Vector3D[] targetDirs;
				targetDirs = new Vector3D[(int)ScriptFormula(18)];

				int takenPos = 0;
				foreach (Actor actor in GetEnemiesInRadius(User.Position, ScriptFormula(15)).Actors)
				{
					targetDirs[takenPos] = actor.Position;
					++takenPos;
					if (takenPos >= targetDirs.Length)
						break;
				}
				if (takenPos < targetDirs.Length)
				{
					PowerMath.GenerateSpreadPositions(User.Position, User.Position + new Vector3D(40, 0, 0), 360f / (targetDirs.Length - takenPos), targetDirs.Length - takenPos)
							 .CopyTo(targetDirs, takenPos);
				}

				foreach (Vector3D position in targetDirs)
				{
					var proj = new Projectile(this, 3276, User.Position);
					proj.Position.Z += 5f;  // fix height
					proj.OnCollision = (hit) =>
					{
						WeaponDamage(hit, ScriptFormula(14), DamageType.Physical);
						proj.Destroy();
					};
					proj.Launch(position, ScriptFormula(17));
				}
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class ReflectBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(30));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				base.OnPayload(payload);

				//reflect 35% of melee damage
				if (payload.Target == Target && payload is HitPayload)
				{
					if (Rune_E > 0)
					{
						Damage(payload.Context.User, (payload as HitPayload).TotalDamage * ScriptFormula(31), 0f, DamageType.Physical);
						(payload as HitPayload).TotalDamage *= (1f - ScriptFormula(31));
					}
				}
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
		[ImplementsPowerBuff(1)]
		class DurationBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(10));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] += (int)ScriptFormula(9);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] -= (int)ScriptFormula(9);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region SeismicSlam
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.SiesmicSlam)]
	public class SeismicSlam : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			yield return WaitSeconds(0.5f);
			var proj1 = new Projectile(this, RuneSelect(164708, 164709, 164712, 164710, 164714, 164713), User.Position);
			proj1.Launch(TargetPosition, 1f);
			foreach (Actor target in GetEnemiesInArcDirection(User.Position, TargetPosition, 45f, ScriptFormula(14)).Actors)
			{
				if (Rune_E > 0)
					if (!PowerMath.PointInBeam(target.Position, User.Position, PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(19) * ScriptFormula(23)), 8f))
						continue;

				WeaponDamage(target, ScriptFormula(8), Rune_C > 0 ? DamageType.Lightning : DamageType.Physical);
				Knockback(target, ScriptFormula(0), ScriptFormula(1));

				if (Rune_C > 0)
				{
					if (Rand.NextDouble() < ScriptFormula(6))
					{
						AddBuff(target, new DebuffStunned(WaitSeconds(ScriptFormula(7))));
					}
				}
			}

			yield return WaitSeconds(1f);

			if (Rune_B > 0)
			{
				var aShockproj = new Projectile(this, 164788, User.Position);
				aShockproj.Launch(TargetPosition, 1f);

				foreach (Actor target in GetEnemiesInArcDirection(User.Position, TargetPosition, 45f, ScriptFormula(14)).Actors)
				{
					WeaponDamage(target, ScriptFormula(3), DamageType.Physical);
				}
			}

			yield break;
		}
	}
	#endregion

	//Complete
	#region Earthquake
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.Earthquake)]
	public class Earthquake : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(361661)) //Earthen might
				GeneratePrimaryResource(30f);

			User.PlayEffectGroup(RuneSelect(168303, 168470, 55689, 168506, 55689, 55689));
			WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(0)), ScriptFormula(19), (Rune_C > 0 ? DamageType.Cold : (Rune_D > 0 ? DamageType.Lightning : DamageType.Fire)));
			Vector3D quakepos = new Vector3D(User.Position);
			var Quake = SpawnEffect(168440, quakepos, 0, WaitSeconds(ScriptFormula(1)));
			Quake.UpdateDelay = 0.5f;
			Quake.OnUpdate = () =>
			{
				TargetList enemies = GetEnemiesInRadius(quakepos, ScriptFormula(0));
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = enemies;
				attack.AddWeaponDamage(ScriptFormula(17), (Rune_C > 0 ? DamageType.Cold : (Rune_D > 0 ? DamageType.Lightning : DamageType.Fire)));
				attack.Apply();

				if (Rune_C > 0)
					foreach (var enemy in enemies.Actors)
					{
						if (!Quake.TriggeredActors.Contains(enemy))
							AddBuff(enemy, new DebuffFrozen(WaitSeconds(1f)));
						Quake.TriggeredActors.Add(enemy);
					}


				if (Rune_B > 0)
				{
					User.PlayEffectGroup(18673);
					WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(10)), ScriptFormula(14), DamageType.Fire);
				}

				if (Rune_E > 0)
				{
					User.PlayEffectGroup(99550); //barbarian_cleave_land
					WeaponDamage(GetEnemiesInArcDirection(User.Position, (User.CurrentDestination != null ? User.CurrentDestination : User.Position), 45f, ScriptFormula(10)), ScriptFormula(14), DamageType.Fire);
				}
			};
			//Secondary Tremor stuff..
			yield break;
		}
	}
	#endregion

	//Complete
	#region Sprint
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.FurySpenders.Sprint)]
	public class Sprint : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new MovementBuff());

			if (Rune_B > 0)
			{
				AddBuff(User, new DodgeBuff());
			}
			if (Rune_D > 0)
			{
				foreach (Actor ally in GetAlliesInRadius(User.Position, ScriptFormula(4)).Actors)
					AddBuff(ally, new MovementAlliesBuff());
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class MovementBuff : PowerBuff
		{
			const float _whirlwindRate = 1f;
			TickTimer _whirlwindTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//nothing seems to be working here for attributes
				User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(1);
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_whirlwindTimer == null || _whirlwindTimer.TimedOut)
				{
					_whirlwindTimer = WaitSeconds(_whirlwindRate);
					if (Rune_C > 0)
					{
						var whirlwind = new EffectActor(this, 108868, Target.Position);
						whirlwind.Timeout = WaitSeconds(ScriptFormula(6));
						whirlwind.Scale = 1f;
						whirlwind.Spawn();
						whirlwind.UpdateDelay = 0.33f; // attack every half-second
						whirlwind.OnUpdate = () =>
						{
							WeaponDamage(GetEnemiesInRadius(whirlwind.Position, ScriptFormula(8)), ScriptFormula(5), DamageType.Physical);
						};
					}
				}
				if (Rune_E > 0)
					foreach (var target in GetEnemiesInRadius(Target.Position, 5f).Actors)
					{
						Knockback(Target.Position, target, 10f);
						WeaponDamage(target, ScriptFormula(16), DamageType.Physical);
					}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(1);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(3)]
		class MovementAlliesBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(4)]
		class DodgeBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(2);
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(2);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region WrathOfTheBerserker
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.WrathOfTheBerserker)]
	public class WrathOfTheBerserker : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(204603))
				StartCooldown(Math.Max(EvalTag(PowerKeys.CooldownTime) - 30f, 0));
			else
				StartCooldown(EvalTag(PowerKeys.CooldownTime));

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new BerserkerBuff());
			if (Rune_B > 0)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(20));
				attack.AddWeaponDamage(ScriptFormula(17), DamageType.Physical);
				attack.Apply();
			}

			yield break;
		}
		[ImplementsPowerBuff(0)]
		public class BerserkerBuff : PowerBuff
		{
			public float GainedFury = 0f;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(6));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				if (Rune_A > 0)
				{
					User.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(8);
				}
				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] += (int)ScriptFormula(0);
				User.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += ScriptFormula(2);
				User.Attributes[GameAttribute.Dodge_Chance_Bonus] += ScriptFormula(3);
				User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(1);
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (Rune_D > 0)
					if (GainedFury >= (1 / ScriptFormula(21)))
					{
						GainedFury -= (1 / ScriptFormula(21));
						this.Extend(60);
					}
				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == Target && payload is HitPayload)
					if (Rune_E > 0 && (payload as HitPayload).IsCriticalHit && (payload as HitPayload).AutomaticHitEffects)
						if (Rand.NextDouble() < payload.Context.GetProcCoefficient())
						{
							SpawnProxy(payload.Target.Position).PlayEffectGroup(209488);

							AttackPayload attack = new AttackPayload(this);
							attack.Targets = GetEnemiesInRadius(payload.Target.Position, ScriptFormula(13));
							attack.AddWeaponDamage(ScriptFormula(11), DamageType.Physical);
							attack.AutomaticHitEffects = false;
							attack.Apply();
						}
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_A > 0)
				{
					User.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(8);
				}
				User.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] -= (int)ScriptFormula(0);
				User.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= ScriptFormula(2);
				User.Attributes[GameAttribute.Dodge_Chance_Bonus] -= ScriptFormula(3);
				User.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(1);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Incomplete
	#region CallOfTheAncients
	[ImplementsPowerSNO(SkillsSystem.Skills.Barbarian.Situational.CallOfTheAncients)]
	public class CallOfTheAncients : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(204603))
				StartCooldown(30f);
			else
				StartCooldown(60f);

			List<Actor> ancients = new List<Actor>();
			for (int i = 0; i < 3; i++)
			{
				if (i == 0)
				{
					var ancient = new AncientKorlic(this.World, this, i);
					ancient.Brain.DeActivate();
					ancient.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					ancient.Attributes[GameAttribute.Untargetable] = true;
					ancient.EnterWorld(ancient.Position);
					ancient.PlayActionAnimation(97105);
					ancients.Add(ancient);
					yield return WaitSeconds(0.2f);
				}
				if (i == 1)
				{
					var ancient = new AncientTalic(this.World, this, i);
					ancient.Brain.DeActivate();
					ancient.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					ancient.Attributes[GameAttribute.Untargetable] = true;
					ancient.EnterWorld(ancient.Position);
					ancient.PlayActionAnimation(97109);
					ancients.Add(ancient);
					yield return WaitSeconds(0.2f);
				}
				if (i == 2)
				{
					var ancient = new AncientMawdawc(this.World, this, i);
					ancient.Brain.DeActivate();
					ancient.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					ancient.Attributes[GameAttribute.Untargetable] = true;
					ancient.EnterWorld(ancient.Position);
					ancient.PlayActionAnimation(97107);
					ancients.Add(ancient);
					yield return WaitSeconds(0.2f);
				}
			}
			yield return WaitSeconds(0.8f);

			foreach (Actor ancient in ancients)
			{
				(ancient as Minion).Brain.Activate();
				ancient.Attributes[GameAttribute.Untargetable] = false;
				ancient.Attributes.BroadcastChangedIfRevealed();
			}
			yield break;
		}
	}
	#endregion
}
