//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
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
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
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
	#region BolaShot //Bola Shot
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.BolaShot)]
	public class DemonHunterBolaShot : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			GeneratePrimaryResource(ScriptFormula(17));
			//StartCooldown(1f); //avoiding unlimited shot speed
			// fire projectile normally, or find targets in arc if RuneB
			Vector3D[] targetDirs;
			if (Rune_B > 0)
			{
				targetDirs = new Vector3D[(int)ScriptFormula(24)];

				int takenPos = 0;
				foreach (Actor actor in GetEnemiesInArcDirection(User.Position, TargetPosition, 75f, ScriptFormula(12)).Actors)
				{
					targetDirs[takenPos] = actor.Position;
					++takenPos;
					if (takenPos >= targetDirs.Length)
						break;
				}

				
				if (takenPos < targetDirs.Length)
				{
					PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 10f, targetDirs.Length - takenPos)
							 .CopyTo(targetDirs, takenPos);
				}
			}
			else
			{
				targetDirs = new Vector3D[] { TargetPosition };
			}

			foreach (Vector3D position in targetDirs)
			{
				var proj = new Projectile(
					this,
					RuneSelect(
						ActorSno._demonhunter_bolashot_projectile,
						ActorSno._demonhunter_bolashotrune_explode_projectile,
						ActorSno._demonhunter_bolashotrune_multi_projectile,
						ActorSno._demonhunter_bolashotrune_stun_projectile,
						ActorSno._demonhunter_bolashotrune_hatred_projectile,
						ActorSno._demonhunter_bolashotrune_delay_projectile
					),
					User.Position
				);
				proj.Position.Z += 5f;  // fix height
				proj.OnCollision = (hit) =>
				{
					// hit effect
					hit.PlayEffectGroup(RuneSelect(77577, 153870, 153872, 153873, 153871, 153869));

					if (hit is DesctructibleLootContainer)
					{
						(hit as DesctructibleLootContainer).Die();
					}
					else
					{
						if (Rune_B > 0)
							WeaponDamage(hit, ScriptFormula(9), DamageType.Poison);
						else
							AddBuff(hit, new ExplosionBuff());

						proj.Destroy();
					}
				};
				proj.Launch(position, ScriptFormula(2));

				if (Rune_B > 0)
					yield return WaitSeconds(ScriptFormula(13));
			}
		}

		[ImplementsPowerBuff(0)]
		class ExplosionBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(4));
			}

			public override bool Update()
			{
				if (Timeout.TimedOut)
				{
					Target.PlayEffectGroup(RuneSelect(77573, 153727, 154073, 154074, 154072, 154070));

					if (Rune_D > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(31))
							GenerateSecondaryResource(ScriptFormula(32));
					}

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(Target.Position, ScriptFormula(20));
					attack.AddWeaponDamage(ScriptFormula(6),
						RuneSelect(DamageType.Fire, DamageType.Fire, DamageType.Poison,
								   DamageType.Lightning, DamageType.Lightning, DamageType.Fire));
					if (Rune_C > 0)
					{
						attack.OnHit = (hitPayload) =>
						{
							if (Rand.NextDouble() < ScriptFormula(28))
								AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(29))));
						};
					}
					attack.Apply();
				}

				return base.Update();
			}

			public override bool Stack(Buff buff)
			{
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region Grenades //Grenades
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.Grenades)]
	public class DemonHunterGrenades : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			GeneratePrimaryResource(ScriptFormula(25));

			float targetDistance = PowerMath.Distance2D(User.Position, TargetPosition);

			// create grenade projectiles with shared detonation timer
			TickTimer timeout = WaitSeconds(ScriptFormula(2));
			Projectile[] grenades = new Projectile[Rune_C > 0 ? 3 : 1];
			for (int i = 0; i < grenades.Length; ++i)
			{
				var projectile = new Projectile(this, Rune_C > 0 ? ActorSno._demonhunter_grenade_projectile_big : ActorSno._demonhunter_grenade_projectile, User.Position);
				projectile.Timeout = timeout;
				grenades[i] = projectile;
			}

			// generate spread positions with distance-scaled spread amount.
			float scaledSpreadOffset = Math.Max(targetDistance - ScriptFormula(14), 0f);
			Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition,
				ScriptFormula(11) - scaledSpreadOffset, grenades.Length);

			// launch and bounce grenades
			yield return WaitTicks(1);  // helps make bounce timings more consistent

			float bounceOffset = 1f;
			float minHeight = ScriptFormula(21);
			float height = minHeight + ScriptFormula(22);
			float bouncePercent = 0.7f; // ScriptFormula(23);
			while (!timeout.TimedOut)
			{
				for (int i = 0; i < grenades.Length; ++i)
				{
					projDestinations[i].Z += 0.5f;
					grenades[i].LaunchArc(PowerMath.TranslateDirection2D(projDestinations[i], User.Position, projDestinations[i],
																		  targetDistance * 0.3f * bounceOffset),
										  height, ScriptFormula(20));
				}

				height *= bouncePercent;
				bounceOffset *= 0.3f;

				yield return grenades[0].ArrivalTime;
				// play "dink dink" grenade bounce sound
				grenades[0].PlayEffect(Effect.Unknown69);
			}

			// damage effects
			foreach (var grenade in grenades)
			{
				var grenadeN = grenade;

				SpawnEffect(RuneSelect(ActorSno._grenadeproxy_norune, ActorSno._grenadeproxy_crimson, ActorSno._grenadeproxy_indigo, ActorSno._grenadeproxy_obsidian, ActorSno._grenadeproxy_golden, ActorSno._grenadeproxy_alabaster), grenade.Position);

				// poison pool effect
				if (Rune_A > 0)
				{
					var pool = SpawnEffect(ActorSno._grenadeproxy_crimson_aoe, grenade.Position, 0, WaitSeconds(ScriptFormula(7)));
					pool.UpdateDelay = 1f;
					pool.OnUpdate = () =>
					{
						WeaponDamage(GetEnemiesInRadius(grenadeN.Position, ScriptFormula(5)), ScriptFormula(6), DamageType.Poison);
					};
				}

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(grenade.Position, ScriptFormula(4));
				attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Poison : (Rune_E > 0 ? DamageType.Lightning : DamageType.Fire));
				attack.OnHit = (hitPayload) =>
				{
					if (Rune_E > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(9))
							AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(10))));
					}
					if (Rune_C > 0)
						Knockback(grenadeN.Position, hitPayload.Target, ScriptFormula(8));
				};
				attack.Apply();
			}

			// clusterbomb hits
			if (Rune_B > 0)
			{
				int damagePulses = (int)ScriptFormula(28);
				for (int pulse = 0; pulse < damagePulses; ++pulse)
				{
					yield return WaitSeconds(ScriptFormula(12) / damagePulses);

					foreach (var grenade in grenades)
					{
						WeaponDamage(GetEnemiesInRadius(grenade.Position, ScriptFormula(4)), ScriptFormula(0), DamageType.Fire);
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region RainOfVengeance //Rain of Vengeance
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.RainOfVengeance)]
	public class DemonHunterRainOfVengeance : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			// ground summon effect for rune c
			if (Rune_C > 0)
				SpawnProxy(TargetPosition).PlayEffectGroup(152294);

			// startup delay all version of skill have
			yield return WaitSeconds(ScriptFormula(3));

			IEnumerable<TickTimer> subScript;
			if (Rune_A > 0)
				subScript = _RuneA();
			else if (Rune_B > 0)
				subScript = _RuneB();
			else if (Rune_C > 0)
				subScript = _RuneC();
			else if (Rune_D > 0)
				subScript = _RuneD();
			else if (Rune_E > 0)
				subScript = _RuneE();
			else
				subScript = _NoRune();

			foreach (var timeout in subScript)
				yield return timeout;
		}

		IEnumerable<TickTimer> _RuneA()     //Shade
		{
			_CreateArrowPool(ActorSno._demonhunter_rainofarrows_alabaster_discipline, new Vector3D(User.Position), ScriptFormula(6), ScriptFormula(7));
			yield break;
		}

		IEnumerable<TickTimer> _NoRune()
		{
			_CreateArrowPool(ActorSno._demonhunter_rainofarrows, new Vector3D(User.Position), ScriptFormula(6), ScriptFormula(7));
			yield break;
		}

		IEnumerable<TickTimer> _RuneB()     //Dark Cloud
		{
			for (int i = 0; i < 8; i++)
			{
				var targets = GetEnemiesInRadius(User.Position, ScriptFormula(18)).Actors;
				Actor target = null;

				if (targets.Count() > 0)
					target = targets[Rand.Next(targets.Count())];

				var position = target == null ? RandomDirection(User.Position, 1f, 15f) : target.Position;
				_CreateArrowPool(ActorSno._demonhunter_rainofarrows_indigo_buff, position, ScriptFormula(16), 2f);

				yield return WaitSeconds(1f);
			}
			yield break;
		}

		void _CreateArrowPool(ActorSno actorSNO, Vector3D position, float duration, float radius)
		{
			var pool = SpawnEffect(actorSNO, position, 0, Rune_B > 0 ? WaitSeconds(1f) : WaitSeconds(duration));
			pool.UpdateDelay = (1.0f / EvalTag(PowerKeys.AttackSpeed));
			if (pool.UpdateDelay < 0.25f) pool.UpdateDelay = 0.25f;
			pool.OnUpdate = () =>
			{
				TargetList targets = GetEnemiesInRadius(position, radius);
				//targets.Actors.RemoveAll((actor) => Rand.NextDouble() > ScriptFormula(10));
				//targets.ExtraActors.RemoveAll((actor) => Rand.NextDouble() > ScriptFormula(10));

				WeaponDamage(targets, ScriptFormula(0) / (duration * pool.UpdateDelay), Rune_A > 0 ? DamageType.Lightning : DamageType.Physical);

				// rewrite delay every time for variation: base wait time * variation * user attack speed
				//pool.UpdateDelay = (ScriptFormula(5) + (float)Rand.NextDouble() * ScriptFormula(2)) * (1.0f / EvalTag(PowerKeys.AttackSpeed));
			};
		}

		IEnumerable<TickTimer> _RuneC()         //Anathema
		{
			var demon = new Projectile(this, ActorSno._dh_rainofarrows_grenade_launcher, TargetPosition);
			demon.Timeout = WaitSeconds(ScriptFormula(30));

			TickTimer grenadeTimer = null;
			demon.OnUpdate = () =>
			{
				if (grenadeTimer == null || grenadeTimer.TimedOut)
				{
					grenadeTimer = WaitSeconds(ScriptFormula(31));

					demon.PlayEffect(Effect.Sound, 215621);

					var grenade = new Projectile(this, ActorSno._dh_rainofarrows_shadowbeast_projectile, demon.Position);
					grenade.Position.Z += 18f;  // make it spawn near demon's cannon
					grenade.Timeout = WaitSeconds(ScriptFormula(33));
					grenade.OnTimeout = () =>
					{
						grenade.PlayEffectGroup(154020);
						WeaponDamage(GetEnemiesInRadius(grenade.Position, ScriptFormula(32)), ScriptFormula(0), DamageType.Fire);
					};
					grenade.LaunchArc(demon.Position, 0.1f, -0.1f, 0.6f);  // parameters not based on anything, just picked to look good
				}
			};

			bool firstLaunch = true;
			while (!demon.Timeout.TimedOut)
			{
				demon.Launch(RandomDirection(TargetPosition, 0f, ScriptFormula(7)), 0.2f);
				if (firstLaunch)
				{
					demon.PlayEffectGroup(165237);
					firstLaunch = false;
				}
				yield return demon.ArrivalTime;
			}
		}

		IEnumerable<TickTimer> _RuneD()         //Flying Strike
		{
			int flyerCount = (int)ScriptFormula(14);
			for (int n = 0; n < flyerCount; ++n)
			{
				var flyerPosition = RandomDirection(TargetPosition, 0f, ScriptFormula(7));
				var flyer = SpawnEffect(ActorSno._demonhunter_rainofarrows_crash_land, flyerPosition, 0f, WaitSeconds(ScriptFormula(5)));
				flyer.OnTimeout = () =>
				{
					flyer.PlayEffectGroup(200516);
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(flyerPosition, ScriptFormula(13));
					attack.AddWeaponDamage(ScriptFormula(12), DamageType.Fire);
					attack.OnHit = (hitPayload) =>
					{
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(37))));
					};
					attack.Apply();
				};

				yield return WaitSeconds(ScriptFormula(4));
			}
		}

		IEnumerable<TickTimer> _RuneE()             //Stampede
		{
			float attackRadius = 8f;  // value is not in formulas, just a guess
			Vector3D castedPosition = new Vector3D(User.Position);
			float castAngle = MovementHelpers.GetFacingAngle(castedPosition, TargetPosition);
			float waveOffset = 0f;

			int flyerCount = (int)ScriptFormula(15);
			for (int n = 0; n < flyerCount; ++n)
			{
				waveOffset += 3.0f;
				var wavePosition = PowerMath.TranslateDirection2D(castedPosition, TargetPosition, castedPosition, waveOffset);
				var flyerPosition = RandomDirection(wavePosition, 0f, attackRadius);
				var flyer = SpawnEffect(ActorSno._demonhunter_rainofarrows_kamikaze, flyerPosition, castAngle, WaitSeconds(ScriptFormula(20)));
				flyer.OnTimeout = () =>
				{
					flyer.PlayEffectGroup(200819);
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(flyerPosition, attackRadius);
					attack.AddWeaponDamage(ScriptFormula(11), DamageType.Fire);
					attack.OnHit = (hitPayload) => { Knockback(hitPayload.Target, 90f); };
					attack.Apply();
				};

				yield return WaitSeconds(ScriptFormula(4));
			}
		}
	}
	#endregion

	//Complete
	#region HungeringArrow //Hungering Arrow
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.HungeringArrow)]
	public class HungeringArrow : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			//These are both zero, but we will keep them in the event blizzard changed their mind. This has been fixed.
			GeneratePrimaryResource(ScriptFormula(13));


			var projectile = new Projectile(
				this,
				RuneSelect(
					ActorSno._dh_bonearrow_projectile,
					ActorSno._dh_bonearrow_projectile_addsfiredamage,
					ActorSno._dh_bonearrow_projectile_splits,
					ActorSno._dh_bonearrow_projectile_addsdamage,
					ActorSno._dh_bonearrow_projectile_increasespeed,
					ActorSno._dh_bonearrow_projectile_splitsmini
				),
				User.Position
			);
			projectile.Position.Z += 5f;
			projectile.Launch(TargetPosition, ScriptFormula(7));
			projectile.OnCollision = (hit) =>
			{
				SpawnEffect(ActorSno._wizard_magicmissile_impact, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)

				AttackPayload attack = new AttackPayload(this);
				attack.SetSingleTarget(hit);
				attack.AddWeaponDamage(ScriptFormula(0) * projectile.DmgMod, Rune_A > 0 ? DamageType.Fire : (Rune_B > 0 ? DamageType.Lightning : DamageType.Physical));
				attack.OnHit = hitPayload =>
				{
					if (Rune_E > 0)
					{
						if (hitPayload.IsCriticalHit)
						{
							AttackPayload blowattack = new AttackPayload(this);
							blowattack.Targets = GetEnemiesInRadius(User.Position, 5f);
							blowattack.AddWeaponDamage(ScriptFormula(11), DamageType.Physical);
							blowattack.Apply();
						}
					}
				};
				attack.Apply();

				bool firsthit = true;
				float range = ScriptFormula(3);
				if (firsthit == false)
				{
					range = ScriptFormula(4);
				}
				var remaningtargets = GetEnemiesInRadius(hit.Position, 10f);
				int count = remaningtargets.Actors.Count();
				var targets = (GetEnemiesInRadius(hit.Position, range));
				double chance = RandomHelper.NextDouble();
				targets.Actors.Remove(hit);
				var closetarget = targets.GetClosestTo(hit.Position);
				if (Rune_B > 0)
				{
					for (int i = 0; i < 3; i++)
					{
						var projectile_split = new Projectile(this, ActorSno._dh_bonearrow_projectile_addsdamage, projectile.Position);
						projectile_split.CollidedActors.Add(hit);
						projectile_split.Position.Z += 5f;
						projectile_split.Launch(RandomDirection(projectile.Position, 7f), ScriptFormula(7));
						projectile_split.OnCollision = (split_hit) =>
						{
							WeaponDamage(split_hit, ScriptFormula(0), DamageType.Lightning);
							projectile_split.Destroy();
						};
					}
					projectile.Destroy();
				}
				else
				{
					if (closetarget != null && chance > ScriptFormula(5))
					{
						if (Rune_C > 0)
							projectile.DmgMod += ScriptFormula(10);
						projectile.Launch(closetarget.Position, ScriptFormula(7));
					}
					else
					{
						projectile.Destroy();
					}
				}

				if (Rune_A > 0)
				{
					AddBuff(hit, new HungerBuff());
				}

			};
			yield break;
		}
		[ImplementsPowerBuff(0, false)]
		public class HungerBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(3f);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(Target, 0.35f, DamageType.Fire);
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region Impale
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.Impale)]
	public class Impale : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			var proj = new Projectile(
				this,
				// FIXME: should be checked and fixed
				RuneSelect(
					ActorSno._dh_impale_projectile_base,
					ActorSno._dh_impale_projectile,
					ActorSno._dh_impale_projectile_knockback,
					ActorSno._dh_impale_projectile_dot,
					ActorSno._dh_impale_projectile, 
					ActorSno._dh_impale_projectile_damage
				),
				User.Position
			);
			proj.Position.Z += 5f;  // fix height
			proj.OnCollision = (hit) =>
			{
				hit.PlayEffectGroup(RuneSelect(221164, 222107, 222120, 222133, 221164, 222146));

				AttackPayload attack = new AttackPayload(this);
				attack.SetSingleTarget(hit);
				attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Poison : DamageType.Physical);
				attack.OnHit = (HitPayload) =>
				{
					if ((HitPayload as HitPayload).IsCriticalHit)
					{
						if (Rune_E > 0)
						{
							(HitPayload as HitPayload).TotalDamage *= (1 + ScriptFormula(13));
						}
					}
					if (Rune_A > 0)
					{
						//Nothing goes here.
					}
					else
					{
						if (Rune_B > 0)
						{
							Knockback(User.Position, hit, ScriptFormula(4));
							AddBuff(hit, new DebuffStunned(WaitSeconds(ScriptFormula(6))));
						}
						if (Rune_C > 0)
						{
							AddBuff(hit, new addsDOTDamage());
						}
						proj.Destroy();
					}
				};
				attack.Apply();
			};
			proj.Launch(TargetPosition, ScriptFormula(2));

			if (Rune_D > 0)
			{
				// FIXME: Find correct actor
				//SpawnEffect(222156, User.Position);
				//User.PlayEffectGroup(222155);
				WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(11)), ScriptFormula(12), DamageType.Physical);
			}

			yield return WaitSeconds(1f);
		}
		[ImplementsPowerBuff(1)]
		class addsDOTDamage : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(8));
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.AddWeaponDamage(ScriptFormula(9), DamageType.Fire);
					attack.Apply();
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region EvasiveFire //Evasive Fire
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.EvasiveFire)]
	public class EvasiveFire : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			GeneratePrimaryResource(ScriptFormula(4));

			// fire projectile normally, or find targets in arc if RuneB
			Vector3D[] targetDirs;
			if (Rune_B > 0)
			{
				targetDirs = new Vector3D[(int)ScriptFormula(24)];

				int takenPos = 0;
				foreach (Actor actor in GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(25), ScriptFormula(26)).Actors)
				{
					targetDirs[takenPos] = actor.Position;
					++takenPos;
					if (takenPos >= targetDirs.Length)
						break;
				}

				
				if (takenPos < targetDirs.Length)
				{
					PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 10f, targetDirs.Length - takenPos)
							 .CopyTo(targetDirs, takenPos);
				}
			}
			else
			{
				targetDirs = new Vector3D[] { TargetPosition };
			}

			foreach (Vector3D position in targetDirs)
			{
				var proj = new Projectile(this, ActorSno._xbowbolt, User.Position);
				proj.Position.Z += 5f;  // fix height
				proj.OnCollision = (hit) =>
				{
					// hit effect
					if (Rune_A > 0)
					{
						hit.PlayEffectGroup(147971);
						WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(21)), ScriptFormula(20), DamageType.Fire);
					}
					else
					{
						hit.PlayEffectGroup(RuneSelect(134836, 150801, 150807, 150803, 150804, 150805));
						WeaponDamage(hit, ScriptFormula(3), RuneSelect(DamageType.Physical, DamageType.Fire, DamageType.Physical, DamageType.Poison, DamageType.Lightning, DamageType.Cold));
					}

					proj.Destroy();
				};
				proj.Launch(position, ScriptFormula(2));
			}

			if (GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(5), 60f).Actors.Count > 0)
			{
				UseSecondaryResource(ScriptFormula(8));
				if (Rune_C > 0)
				{
					var mine = new EffectActor(this, ActorSno._dh_safetyshot_mine, User.Position);
					mine.Timeout = WaitSeconds(ScriptFormula(30));
					mine.Scale = 1f;
					mine.Spawn();
					mine.OnTimeout = () =>
					{
						mine.PlayEffectGroup(148790);
						WeaponDamage(GetEnemiesInRadius(mine.Position, ScriptFormula(29)), ScriptFormula(28), DamageType.Poison);
					};
				}
				float speed = User.Attributes[GameAttribute.Running_Rate_Total] * 3f;
				Vector3D destination = PowerMath.TranslateDirection2D(TargetPosition, User.Position, User.Position, ScriptFormula(7));//this needs to be the opposite direction of the facing direction// ScriptFormula(7);
				ActorMover _mover;
				//lets move backwards!
				User.TranslateFacing(TargetPosition, true);
				_mover = new ActorMover(User);
				_mover.Move(destination, speed, new ACDTranslateNormalMessage
				{
					AnimationTag = 69824, // dashing strike attack animation
				});
				//backflip

				if ((User as Player).SkillSet.HasPassive(218385)) //TacticalAdvantage (DH)
				{
					AddBuff(User, new MovementBuff(0.6f, WaitSeconds(2f)));
				}
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region Caltrops
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.Caltrops)]
	public class Caltrops : Skill
	{
		bool Activated = false;
		public override IEnumerable<TickTimer> Main()
		{
			UseSecondaryResource(ScriptFormula(7));

			var GroundSpot = SpawnProxy(User.Position);
			var caltropsGround = SpawnEffect(ActorSno._dh_caltrops_inactive_proxyactor, GroundSpot.Position, 0, WaitSeconds(ScriptFormula(2)));
			caltropsGround.UpdateDelay = 0.25f;
			caltropsGround.OnUpdate = () =>
			{
				var targets = GetEnemiesInRadius(GroundSpot.Position, 7f).Actors;
				if (targets.Count <= 0) return;

				caltropsGround.Destroy();
				if (Activated) return;

				var calTrops = SpawnEffect(
					RuneSelect(
						ActorSno._dh_caltrops_unruned,
						ActorSno._dh_caltrops_runea_damage,
						ActorSno._dh_caltrops_runeb_slower,
						ActorSno._dh_caltrops_runec_weakenmonsters,
						ActorSno._dh_caltrops_runed_reducediscipline,
						ActorSno._dh_caltrops_runee_empower
					),
					GroundSpot.Position,
					0,
					WaitSeconds(ScriptFormula(2))
				);
				if (Rune_E > 0)     //Bait the Trap
				{
					if (PowerMath.Distance2D(GroundSpot.Position, User.Position) < 12f)
						if (!HasBuff<CaltropsChCBuff>(User))
							AddBuff(User, new CaltropsChCBuff());
				}

				targets = GetEnemiesInRadius(GroundSpot.Position, 12f).Actors;
				foreach (var target in targets)
				{
					if (!HasBuff<ActiveCalTrops>(target))
						AddBuff(target, new ActiveCalTrops());

					if ((User as Player).SkillSet.HasPassive(218398)) //NumbingTraps (DH)
						if (!HasBuff<DamageReduceDebuff>(target))
							AddBuff(target, new DamageReduceDebuff(0.25f, WaitSeconds(3f)));
				}
				Activated = true;
			};

			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class ActiveCalTrops : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			private float SlowAmount = 0f;

			public override void Init()
			{
				base.Init();
				SlowAmount = Rune_B > 0 ? 0.6f : 0.4f;
				Timeout = WaitSeconds(ScriptFormula(2));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_C > 0 && !HasBuff<DebuffRooted>(Target))       //Torturous Ground
					AddBuff(Target, new DebuffRooted(WaitSeconds(2f)));

				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					if (!HasBuff<DebuffSlowed>(Target))
						AddBuff(Target, new DebuffSlowed(SlowAmount, WaitSeconds(1f)));
					else Target.World.BuffManager.GetFirstBuff<DebuffSlowed>(Target).Extend(60);

					if (Rune_A > 0)     //Jagged Spikes
					{
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(ScriptFormula(20), DamageType.Physical);
						attack.AutomaticHitEffects = false;
						attack.Apply();
					}
				}

				return false;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(3)]
		class CaltropsChCBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(6f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Weapon_Crit_Chance] += 0.1f;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Weapon_Crit_Chance] -= 0.1f;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region RapidFire //Rapid Fire
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.RapidFire)]
	public class RapidFire : ChanneledSkill
	{
		private Actor _target = null;
		
		int ticks = 0;
		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.1f;
			ticks = 0;
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			User.Attributes[GameAttribute.Projectile_Speed] = User.Attributes[GameAttribute.Projectile_Speed] * ScriptFormula(22);
			User.Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnChannelClose()
		{
			if (_target != null)
				_target.Destroy();
			User.Attributes[GameAttribute.Projectile_Speed] = User.Attributes[GameAttribute.Projectile_Speed] / ScriptFormula(22);
			User.Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnChannelUpdated()
		{
			User.TranslateFacing(TargetPosition);
		}

		public override IEnumerable<TickTimer> Main()
		{
			var DataOfSkill = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[DiIiS_NA.GameServer.Core.Types.SNO.SNOGroup.Power][131192].Data;
			var proj1 = new Projectile(
				this,
				RuneSelect(
					ActorSno._dh_rapidfire_projectile,
					ActorSno._dh_rapidfire_projectile_grenades, 
					ActorSno._dh_rapidfire_projectile_addspierce, 
					ActorSno._dh_rapidfire_projectile_addsmissiles,
					ActorSno._dh_rapidfire_projectile_addsdamage,
					ActorSno._dh_rapidfire_projectile_addsslow
				),
				User.Position
			);
			proj1.Position.Z += 5f;
			if (Rune_A > 0)
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f));
				proj1.LaunchArc(TargetPosition, 7f, -0.07f);
				proj1.OnArrival = () => {
					SpawnProxy(proj1.Position).PlayEffectGroup(149939);
					WeaponDamage(GetEnemiesInRadius(proj1.Position, ScriptFormula(5)), ScriptFormula(6), DamageType.Fire);
					proj1.Destroy();
				};
			}
			else
			{
				proj1.Launch(new Vector3D(TargetPosition.X + ((float)Rand.NextDouble() * 2f), TargetPosition.Y + ((float)Rand.NextDouble() * 2f), TargetPosition.Z), ScriptFormula(2));
				proj1.OnCollision = (hit) =>
				{
					SpawnEffect(ActorSno._wizard_magicmissile_impact, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
					WeaponDamage(hit, ScriptFormula(0), Rune_D > 0 ? DamageType.Fire : DamageType.Physical);

					if (Rune_B > 0 && FastRandom.Instance.NextDouble() < ScriptFormula(7))
					{ }
					else
						proj1.Destroy();

					if (Rune_E > 0)
						AddBuff(hit, new DebuffSlowed(ScriptFormula(8), WaitSeconds(ScriptFormula(9))));
				};
			}

			if (Rune_C > 0 && ticks % 10 == 0)
			{
				var targets = GetEnemiesInRadius(User.Position, 40f).Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, User.Position)).Take((int)ScriptFormula(16));
				foreach (var target in targets)
				{
					var missile = new Projectile(this, ActorSno._dh_rapidfire_projectile_addsmissiles, User.Position);
					missile.Position.Z += 5f;
					missile.Launch(target.Position, ScriptFormula(2));
					missile.OnCollision = (hit) =>
					{
						hit.PlayEffectGroup(158128);
						WeaponDamage(hit, ScriptFormula(23), DamageType.Physical);

						missile.Destroy();
					};
				}
			}

			ticks++;
			UsePrimaryResource(ScriptFormula(19));

			yield return WaitSeconds(ScriptFormula(1));
		}
	}
	#endregion

	//Complete
	#region EntanglingShot //Entangling Shot
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.EntanglingShot)]
	public class EntanglingShot : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			GeneratePrimaryResource(ScriptFormula(2));
			var proj1 = new Projectile(this, ActorSno._demonhunter_entangle_projectile, User.Position);
			proj1.Position.Z += 5f;
			proj1.OnCollision = (hit) =>
			{
				proj1.Destroy();
				AddBuff(hit, new EntangleDebuff());

				AttackPayload attack = new AttackPayload(this);
				attack.SetSingleTarget(hit);
				attack.AddWeaponDamage(ScriptFormula(5), RuneSelect(DamageType.Physical, DamageType.Cold, DamageType.Poison, DamageType.Physical, DamageType.Fire, DamageType.Physical));
				attack.OnHit = HitPayload =>
				{
					if (Rune_E > 0)
					{
						if (User is Player)
							(User as Player).AddHP(HitPayload.TotalDamage * ScriptFormula(11));
					}
				};
				attack.Apply();

			};
			proj1.Launch(TargetPosition, ScriptFormula(12));

			yield break;
		}
		[ImplementsPowerBuff(0)]
		class EntangleDebuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(4));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.PlayEffectGroup(76093);
				AddBuff(Target, new DebuffSlowed(0.7f, WaitSeconds(ScriptFormula(4))));

				var targets = GetEnemiesInRadius(Target.Position, ScriptFormula(2)).Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, Target.Position)).Take((int)ScriptFormula(3));
				foreach (var target in targets)
					AddBuff(target, new EntangleDebuff());

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					if (Rune_C > 0)
						WeaponDamage(Target, ScriptFormula(8), DamageType.Lightning);
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
	#region ElementalArrow //Elemental Arrow
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.ElementalArrow)]
	public class ElementalArrow : Skill
	{
		TickTimer ArrowTimer = null;
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			var startPosition = User.Position;

			var dmgType = DamageType.Fire;
			if (Rune_A > 0) dmgType = DamageType.Cold;
			if (Rune_B > 0 || Rune_E > 0) dmgType = DamageType.Lightning;
			if (Rune_D > 0) dmgType = DamageType.Physical;

			float rad = 2f;
			if (Rune_B > 0) rad = 4f;
			if (Rune_D > 0) rad = 3f;

			float frequency = 0.3f;
			if (Rune_B > 0 || Rune_D > 0) frequency = 0.7f;

			var proj = new Projectile(
				this, 
				RuneSelect(
					ActorSno._demonhunter_moltenarrow_projectile,
					ActorSno._dh_elementalarrow_iceprojectile, 
					ActorSno._demonhunter_elementalarrow_lightningball,
					ActorSno._demonhunter_elementalarrow_skullprojectile,
					ActorSno._demonhunter_elementalarrow_golden_projectile, 
					ActorSno._demonhunter_elementalarrow_alabaster_projectile
				),
				User.Position
			);

			if (Rune_E > 0) proj.Scale = 3f;
			if (Rune_C > 0) proj.Position.Z += 3f;
			else proj.Position.Z += 5f;

			proj.OnUpdate = () =>
			{
				if (PowerMath.Distance2D(proj.Position, startPosition + new Vector3D(0, 0, 5f)) > 65f)
					proj.Destroy();

				if (Rune_A > 0) return;     //Frost Arrow

				if (ArrowTimer == null || ArrowTimer.TimedOut)
				{
					ArrowTimer = WaitSeconds(frequency);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(proj.Position, rad);
					attack.AddWeaponDamage(Rune_D > 0 ? ScriptFormula(32) : ScriptFormula(0), dmgType);
					attack.OnHit = (hitPayload) =>
					{
						//hitPayload.Target.PlayEffectGroup(RuneSelect(154844, 155087, 154845, 135251, 156007, 154846));

						if (Rune_B > 0)         //Ball Lightning
							proj.PlayEffectGroup(153934, hitPayload.Target);

						if (Rune_C > 0)         //Screaming Skull
							if (!HasBuff<DebuffFeared>(hitPayload.Target) && Rand.NextDouble() < ScriptFormula(5))
								AddBuff(hitPayload.Target, new DebuffFeared(WaitSeconds(ScriptFormula(6))));

						if (Rune_E > 0)     //Lightning Bolts
							if (hitPayload.IsCriticalHit && !HasBuff<DebuffStunned>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(38))));

						if (Rune_D > 0)     //Nether Tentacles
							if (User is Player) (User as Player).AddPercentageHP(0.4f);
					};
					attack.Apply();
				}
			};

			proj.OnCollision = (hit) =>
			{
				if (Rune_A <= 0) return;            //Frost Arrow

				hit.PlayEffectGroup(RuneSelect(154844, 155087, 154845, 135251, 156007, 154846));
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, hit.Position, User.Position, 80f);
				var splitTargets = GetEnemiesInArcDirection(hit.Position, TargetPosition, 20f, 120f).Actors.Take(10).Where(i => i != hit);

				foreach (var target in splitTargets)
				{
					var proj2 = new Projectile(this, ActorSno._dh_elementalarrow_iceprojectile, hit.Position);
					proj2.Position.Z += 5f;
					proj2.OnCollision = hit2 =>
					{
						if (hit2 == hit) return;

						hit2.PlayEffectGroup(131673);
						WeaponDamage(hit2, ScriptFormula(12), dmgType);

						if (!HasBuff<DebuffChilled>(hit2))
							AddBuff(hit2, new DebuffChilled(ScriptFormula(18), WaitSeconds(ScriptFormula(19))));

						proj2.Destroy();
					};
					proj2.Launch(target.Position, 1.3f);
				}

				WeaponDamage(hit, ScriptFormula(12), dmgType);
				if (!HasBuff<DebuffChilled>(hit))
					AddBuff(hit, new DebuffChilled(ScriptFormula(18), WaitSeconds(ScriptFormula(19))));

				proj.Destroy();
			};

			if (Rune_B > 0 || Rune_D > 0)
				proj.Launch(TargetPosition, ScriptFormula(20));
			else
				proj.Launch(TargetPosition, ScriptFormula(1));

			yield break;
		}
	}
	#endregion

	//Complete
	#region ShadowPower //Shadow Power
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.ShadowPower)]
	public class ShadowPower : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			//Rune D Well of Darkness included here
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UseSecondaryResource(EvalTag(PowerKeys.ResourceCost));

			RemoveBuffs(User, SkillsSystem.Skills.DemonHunter.Discipline.ShadowPower);
			yield return WaitSeconds(0.1f);
			AddBuff(User, new ShadowPowerFemale());

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ShadowPowerFemale : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			private float LoH(int level)   
			{
				var loh = 5 + 0.05f * (float)Math.Pow(level, 3);
				return (Rune_E > 0 ? loh * 1.5f : loh);         //Blood Moon
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Hitpoints_On_Hit] += LoH(User.Attributes[GameAttribute.Level]);

				if (Rune_A > 0)     //Night Bane
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(3));
					attack.OnHit = hitPayload =>
					{
						if (!HasBuff<DebuffSlowed>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(10), WaitSeconds(ScriptFormula(11))));
					};
					attack.Apply();
				}
				if (Rune_B > 0)     //Shadow Glide
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(2);
				}
				if (Rune_C > 0)     //Gloom
				{
					Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(4);
				}

				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Hitpoints_On_Hit] -= LoH(User.Attributes[GameAttribute.Level]);

				if (Rune_B > 0)
				{
					Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(2);
				}
				if (Rune_C > 0)
				{
					Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(4);
				}

				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region SpikeTrap //Spike Trap
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.SpikeTrap)]
	public class SpikeTrap : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			User.Attributes[GameAttribute.Skill_Charges, 75301] -= 1;
			if (Rune_C > 0)
			{
				if (Target != null)
					AddBuff(Target, new curseDebuff());
			}
			else
			{
				List<Vector3D> trap_positions = new List<Vector3D>();
				if (Rune_D > 0)
					trap_positions = PowerMath.GenerateSpreadPositions(User.Position, PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 10f), 120f, 3).ToList();
				else
					trap_positions.Add(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 20f)));

				foreach (var targetPos in trap_positions)
				{
					var traps = User.World.GetActorsBySNO(ActorSno._demonhunter_spiketrap_proxy);
					if (traps.Count >= ScriptFormula(2))
					{
						traps.First().Destroy();
					}

					var GroundSpot = SpawnProxy(targetPos);
					GroundSpot.Unstuck();
					var spikeTrapGround = SpawnEffect(ActorSno._demonhunter_spiketrap_proxy, GroundSpot.Position, 0, WaitSeconds(ScriptFormula(4)));

					if (!(Rune_D > 0)) yield return WaitSeconds(ScriptFormula(3));

					spikeTrapGround.UpdateDelay = 2f;
					spikeTrapGround.OnUpdate = () =>
					{
						if (GetEnemiesInRadius(GroundSpot.Position, ScriptFormula(5)).Actors.Count > 0)
						{
							if (Rune_E > 0)
							{
								Vector3D lightning_pos = GroundSpot.Position;
								Actor lastTarget = null;
								for (int i = 0; i < ScriptFormula(25); i++)
								{
									SpawnEffect(ActorSno._demonhunter_spiketraprune_chainlightning_explosion, lightning_pos);
									var targets = GetEnemiesInRadius(lightning_pos, ScriptFormula(6)).Actors.Where(a => a != lastTarget).ToList();
									if (targets.Count > 0)
									{
										lastTarget = targets.First();
										AttackPayload attack = new AttackPayload(this);
										attack.SetSingleTarget(lastTarget);
										attack.AddWeaponDamage(ScriptFormula(0), DamageType.Lightning);
										attack.Apply();

										lightning_pos = lastTarget.Position;
									}
									else
										break;
								}
							}
							else
							{
								spikeTrapGround.Destroy();
								SpawnEffect(ActorSno._demonhunter_spiketrap_explosion, GroundSpot.Position);
								var targets = GetEnemiesInRadius(GroundSpot.Position, ScriptFormula(5));
								AttackPayload attack = new AttackPayload(this);
								attack.Targets = targets;
								attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : DamageType.Physical);
								attack.Apply();
							}

							if ((User as Player).SkillSet.HasPassive(218398)) //NumbingTraps (DH)
							{
								foreach (var tgt in GetEnemiesInRadius(GroundSpot.Position, ScriptFormula(5)).Actors)
									if (tgt.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.DamageReduceDebuff>(tgt) == null)
										AddBuff(tgt, new DamageReduceDebuff(0.25f, WaitSeconds(3f)));
							}
						}
					};
				}
			}

			yield break;
		}
		[ImplementsPowerBuff(1)]
		class curseDebuff : PowerBuff
		{

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(4));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override void Remove() { base.Remove(); }

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is DeathPayload)
				{
					Target.PlayEffectGroup(159027);
					WeaponDamage(GetEnemiesInRadius(Target.Position, ScriptFormula(6)), ScriptFormula(0), DamageType.Fire);
				}
			}
		}

		[ImplementsPowerBuff(2)]
		public class SpikeCountBuff : PowerBuff
		{
			public bool CoolDownStarted = false;
			public uint Max = 4;
			
			public override bool Update()
			{
				if (base.Update())
					return true;

				if ((User as Player).SkillSet.HasPassive(0x00032EE2))
					Max = 5;
				else
				{
					Max = 4;
					if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] == 5)
						User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = 4;
				}

				if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] < Max)
				{
					if(!CoolDownStarted)
					{
						StartCooldownCharges(6f); CoolDownStarted = true;

						Task.Delay(6100).ContinueWith(delegate
						{
							CoolDownStarted = false;
							User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = (int)Math.Min(User.Attributes[GameAttribute.Skill_Charges, PowerSNO] + 1, Max);
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
	#region Multishot //Multishot
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.Multishot)]
	public class Multishot : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			User.PlayEffectGroup(RuneSelect(77647, 154203, 154204, 154208, 154211, 154212));
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(21), ScriptFormula(23));
			attack.AddWeaponDamage(ScriptFormula(0),
				RuneSelect(DamageType.Physical, DamageType.Physical, DamageType.Physical, DamageType.Physical, DamageType.Lightning, DamageType.Physical));
			if (Rune_E > 0)
			{
				attack.OnHit = HitPayload =>
				{
					//Every enemy hit grants 1 Discipline. Each volley can gain up to SF(10) Discipline in this way.
					GenerateSecondaryResource(Math.Min(ScriptFormula(9), ScriptFormula(10)));
				};
			}
			attack.Apply();

			if (Rune_B > 0)
			{
				User.PlayEffectGroup(154409);
				WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(2)), ScriptFormula(1), DamageType.Poison);
			}
			yield return WaitSeconds(ScriptFormula(17));

			if (Rune_C > 0)
			{
				Vector3D[] targetDirs;
				targetDirs = new Vector3D[(int)ScriptFormula(3)];

				int takenPos = 0;
				foreach (Actor actor in GetEnemiesInRadius(User.Position, ScriptFormula(24)).Actors)
				{
					targetDirs[takenPos] = actor.Position;
					++takenPos;
					if (takenPos >= targetDirs.Length)
						break;
				}

				if (takenPos < targetDirs.Length)
				{
					PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 10f, targetDirs.Length - takenPos)
							 .CopyTo(targetDirs, takenPos);
				}

				foreach (Vector3D position in targetDirs)
				{
					// TODO: check projectile actor
					var proj = new Projectile(this, ActorSno._dh_multishotrune_bounce_missile_explode, User.Position);
					proj.Position.Z += 5f;  // fix height
					proj.OnCollision = (hit) =>
					{
						// hit effect
						hit.PlayEffectGroup(196636);

						if (Rune_B > 0)
							WeaponDamage(hit, ScriptFormula(6), DamageType.Fire);

						proj.Destroy();
					};
					proj.Launch(position, ScriptFormula(20));
				}
			}

			yield break;
		}
	}
	#endregion

	//Complete
	#region SmokeScreen //Smoke Screen
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.SmokeScreen)]
	public class SmokeScreen : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UseSecondaryResource(EvalTag(PowerKeys.ResourceCost));

			if (Rune_A > 0)
			{
				var cloudCover = SpawnEffect(ActorSno._gluttony_gascloud_proxy, User.Position, 0, WaitSeconds(ScriptFormula(4)));
				//cloudCover.PlayEffectGroup(219653);//131425
				cloudCover.UpdateDelay = 0.25f;
				cloudCover.OnUpdate = () =>
				{
					if (GetEnemiesInRadius(cloudCover.Position, ScriptFormula(3)).Actors.Count > 0)
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(cloudCover.Position, ScriptFormula(3));
						attack.AddWeaponDamage(ScriptFormula(5), DamageType.Physical);
						attack.Apply();
					}
				};
			}

			AddBuff(User, new SmokeScreenBuff());

			if ((User as Player).SkillSet.HasPassive(218385)) //TacticalAdvantage (DH)
			{
				AddBuff(User, new MovementBuff(0.6f, WaitSeconds(2f)));
			}

			yield break;
		}
		//may not be for every cast, does not show the cloud animation around you in a video..
		[ImplementsPowerBuff(2)]
		class SmokeScreenBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(0));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Has_Look_Override] = true;//0x04E733FD;
				User.Attributes[GameAttribute.Stealthed] = true;
				(User as Player).SpeedCheckDisabled = true;

				if (Rune_E > 0)
				{
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(12);
				}
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (Rune_C > 0)
				{
					if (_damageTimer == null || _damageTimer.TimedOut)
					{
						_damageTimer = WaitSeconds(_damageRate);

						GeneratePrimaryResource(ScriptFormula(10));
					}
				}

				return false;
			}

			public override void Remove()
			{
				base.Remove();
				//User.PlayEffectGroup(133698); //reappear
				User.Attributes[GameAttribute.Stealthed] = false;
				User.Attributes[GameAttribute.Has_Look_Override] = false;
				(User as Player).SpeedCheckDisabled = false;

				if (Rune_E > 0)
				{
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(12);
				}
			}
		}
	}
	#endregion

	//Complete
	#region Strafe //Strafe
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredGenerators.Strafe)]
	public class Strafe : Skill
	{

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new StrafeBuff());
			yield break;
		}


		[ImplementsPowerBuff(0)]
		class StrafeBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(1f / 4f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Projectile_Speed] = Target.Attributes[GameAttribute.Projectile_Speed] * ScriptFormula(13);
				if (Rune_D <= 0)    //Drifting Shadow
					Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= 0.25f;
				if (Rune_B > 0)
					Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += (ScriptFormula(28) - 1f);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				Vector3D targetPosition = RandomDirection(Target.Position, ScriptFormula(2));
				var enemies = GetEnemiesInRadius(Target.Position, ScriptFormula(2)).Actors;
				if (enemies.Count > 0)
					targetPosition = enemies[FastRandom.Instance.Next(0, enemies.Count)].Position;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(1f / 4f);
					if (Rune_A > 0)
					{
						Vector3D targetPos = RandomDirection(Target.Position, 6f, 20f);
						//targetPos.Z += 6f;
						var grenade = new Projectile(this, ActorSno._demonhunter_grenade_projectile, Target.Position);
						grenade.Timeout = WaitSeconds(1f);
						float targetDistance = PowerMath.Distance2D(Target.Position, targetPos);
						float bounceOffset = 1f;
						float minHeight = ScriptFormula(12);
						float height = minHeight + ScriptFormula(11);
						//float bouncePercent = 0.7f; // ScriptFormula(23);
						grenade.OnTimeout = () =>
						{
							SpawnEffect(ActorSno._grenadeproxy_norune, grenade.Position);
							//grenade.PlayEffectGroup(154020);
							WeaponDamage(GetEnemiesInRadius(grenade.Position, ScriptFormula(25)), ScriptFormula(1) * ScriptFormula(13), DamageType.Fire);
						};
						//grenade.Launch(targetPos, 0.1f, -0.1f, ScriptFormula(20));
						grenade.LaunchArc(PowerMath.TranslateDirection2D(targetPos, User.Position, targetPos, targetDistance * 0.3f * bounceOffset), height, -0.1f, ScriptFormula(20));
						UsePrimaryResource(ScriptFormula(19));
						return false;
					}

					var proj1 = new Projectile(this, (Rune_E > 0 ? ActorSno._dh_straferune_knives_knife : ActorSno._dh_strafe_projectile), User.Position);
					proj1.Position.Z += 6f;
					proj1.OnCollision = (hit) =>
					{
						SpawnEffect(ActorSno._dh_strafe_sphereexplode, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 6f)); // impact effect (fix height)
						proj1.Destroy();
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(hit);
						attack.AddWeaponDamage(ScriptFormula(0) * ScriptFormula(9), DamageType.Physical);
						attack.OnHit = (hitPayload) =>
						{
							if (Rune_E > 0 && hitPayload.IsCriticalHit)
								hitPayload.TotalDamage *= 1 + ScriptFormula(17);
						};
						attack.Apply();
					};
					proj1.Launch(targetPosition, ScriptFormula(10));
					UsePrimaryResource(ScriptFormula(19));
					if (Rune_C > 0)
					{
						var targets = GetEnemiesInRadius(Target.Position, ScriptFormula(2));
						if (targets.Actors.Count > 0)
						{
							Vector3D nearby_position = targets.Actors[FastRandom.Instance.Next(0, targets.Actors.Count)].Position;
							// TODO: check projectile actor
							var rocket = new Projectile(this, ActorSno._dh_straferune_knives_knife, User.Position);
							rocket.Position.Z += 6f;
							rocket.Launch(nearby_position, ScriptFormula(10));
							rocket.OnCollision = (hit) =>
							{
								SpawnEffect(ActorSno._dh_strafe_sphereexplode, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 6f)); // impact effect (fix height)
								rocket.Destroy();
								WeaponDamage(hit, ScriptFormula(31), DamageType.Fire);
							};
						}
					}
				}

				return false;
			}

			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Projectile_Speed] = Target.Attributes[GameAttribute.Projectile_Speed] / ScriptFormula(13);
				if (Rune_D <= 0)        //Drifting Shadow				
					Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += 0.25f;
				if (Rune_B > 0)
					Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= (ScriptFormula(28) - 1f);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region MarkedForDeath //Marked for Death
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.MarkedForDeath)]
	public class MarkedForDeath : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UseSecondaryResource(EvalTag(PowerKeys.ResourceCost));

			if (Rune_C > 0)
			{
				var GroundMark = SpawnEffect(ActorSno._dh_markedfordeath_proxyactor, TargetPosition, 0, WaitSeconds(ScriptFormula(9)));
				GroundMark.UpdateDelay = 1f;
				GroundMark.OnUpdate = () =>
				{
					foreach (Actor enemy in GetEnemiesInRadius(GroundMark.Position, ScriptFormula(7)).Actors)
					{
						AddBuff(enemy, new DeathMarkBuff());
					}
				};
			}
			else
				if (Target != null)
				AddBuff(Target, new DeathMarkBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class DeathMarkBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(Rune_C > 0 ? 1f : ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Debuff_Duration_Reduction_Percent] += ScriptFormula(1);
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					if (Rune_E > 0)
					{
						if (payload.Context.User is Player)
							(payload.Context.User as Player).AddHP((payload as HitPayload).TotalDamage * ScriptFormula(14));
					}
					if (Rune_D > 0 && payload.Context.User == User)
					{
						GeneratePrimaryResource(ScriptFormula(11));
					}
					if (Rune_A > 0)
					{
						var targets = GetEnemiesInRadius(Target.Position, ScriptFormula(3)).Actors.Where(a => !HasBuff<DeathMarkBuff>(a)).ToList();
						foreach (var tgt in targets)
							Damage(tgt, (ScriptFormula(2) * (payload as HitPayload).TotalDamage) / targets.Count, 0f, DamageType.Physical);
					}
				}
				if (payload.Target == Target && payload is DeathPayload)
				{
					if (Rune_B > 0)
					{
						var targets = GetEnemiesInRadius(Target.Position, ScriptFormula(5)).Actors.Take((int)ScriptFormula(6));
						foreach (var tgt in targets)
							AddBuff(tgt, new DeathMarkBuff());
					}
				}
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Debuff_Duration_Reduction_Percent] -= ScriptFormula(1);
			}
		}
	}
	#endregion

	//Complete
	#region Preparation //Preparation
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.Preparation)]
	public class Preparation : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(7))
			{
				User.PlayEffectGroup(158497);
			}
			else
			{
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			}

			UseSecondaryResource(EvalTag(PowerKeys.ResourceCost));

			User.PlayEffectGroup(RuneSelect(132466, 148872, 148873, 148874, 148875, 148876));
			if (Rune_A > 0)
			{
				GeneratePrimaryResource(75f);
			}
			else if (Rune_D > 0) // Battle Scars
			{
				GenerateSecondaryResource(25f);
				if (User is Player)
					(User as Player).AddHP(User.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(6));
			}
			else
			{
				GenerateSecondaryResource(30f);
				if (Rune_C > 0)
				{
					AddBuff(User, new ObsidianBuff());
				}
			}
			yield break;
		}
		[ImplementsPowerBuff(1)]
		class ObsidianBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(5));
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					GenerateSecondaryResource(ScriptFormula(4) / ScriptFormula(5));
				}

				return false;
			}
		}
	}

	[ImplementsPowerSNO(324845)]
	public class PreparationPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new PreparationPassiveBuff());
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class PreparationPassiveBuff : PowerBuff
		{
			public override bool Apply()
			{
				if (!base.Apply()) return false;

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.DemonHunter.Discipline.Preparation] > 0)
				{
					User.Attributes[GameAttribute.Resource_Max_Bonus, 6] += 15f;
					User.Attributes.BroadcastChangedIfRevealed();
				}

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.DemonHunter.Discipline.Preparation] > 0)
				{
					User.Attributes[GameAttribute.Resource_Max_Bonus, 6] -= 15f;
					User.Attributes.BroadcastChangedIfRevealed();
				}
			}
		}
	}
	#endregion

	//Complete
	#region ClusterArrow //Cluster Arrow
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.ClusterArrow)]
	public class ClusterArrow : Skill
	{
		TickTimer ClusterTimer = null;
		int ClusterCount = 0;
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			var dmgType = DamageType.Fire;
			if (Rune_B > 0 || Rune_D > 0) dmgType = DamageType.Physical;
			if (Rune_E > 0) dmgType = DamageType.Lightning;

			if (Rune_C > 0)         //Cluster Bombs "obsidian"
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 35f);
				//TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Max(PowerMath.Distance2D(User.Position, TargetPosition), 10f));

				var proj = new Projectile(this, ActorSno._demonhunter_clusterarrow_projectile_obsidian, User.Position);
				proj.Position.Z += 5f;
				proj.OnUpdate = () =>
				{
					if (PowerMath.Distance2D(proj.Position, TargetPosition + new Vector3D(0, 0, 5f)) < 3f)
					{
						proj.PlayEffectGroup(167301);
						WeaponDamage(GetEnemiesInRadius(proj.Position, 10f), 5.25f, dmgType);
						proj.Destroy();
						ClusterCount = 0;
						return;
					}

					if (ClusterCount > 5) return;

					if (ClusterTimer == null || ClusterTimer.TimedOut)
					{
						ClusterTimer = WaitSeconds(0.1f);
						ClusterCount++;

						var grenadeC = new Projectile(this, ActorSno._demonhunter_clusterarrow_babygrenade_obsidian, proj.Position);
						grenadeC.OnArrival = () =>
						{
							grenadeC.PlayEffectGroup(167359);
							WeaponDamage(GetEnemiesInRadius(grenadeC.Position, 6f), 5.25f, dmgType);
							grenadeC.Destroy();
						};
						var destination = PowerMath.TranslateDirection2D(proj.Position, TargetPosition, proj.Position, 6f);
						grenadeC.LaunchArc(destination, 7f, -0.07f, 0.3f);
					}
				};
				proj.LaunchArc(TargetPosition, 11f, -0.07f);
				yield break;
			}

			//base effect
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 60f));

			var arrow = new Projectile(
				this,
				RuneSelect(
					ActorSno._demonhunter_clusterarrow_projectile,
					ActorSno._demonhunter_clusterarrow_projectile_crimson, 
					ActorSno._demonhunter_clusterarrow_projectile_indigo, 
					ActorSno._demonhunter_clusterarrow_projectile_obsidian, 
					ActorSno._demonhunter_clusterarrow_projectile_golden,
					ActorSno._demonhunter_clusterarrow_projectile_alabaster
				),
				User.Position
			);
			arrow.Scale = 0.3f;
			arrow.Position.Z += 5f;
			arrow.OnArrival = () =>
			{
				arrow.PlayEffectGroup(RuneSelect(131729, 166558, 166563, 167301, 166912, 166622));
				//Rune A Loaded for Bear "crimson" included here
				WeaponDamage(GetEnemiesInRadius(arrow.Position, ScriptFormula(1)), ScriptFormula(0), dmgType);

				if (Rune_B > 0 || Rune_D > 0)       //Shooting Stars "indigo", Maelstrom "golden"
				{
					var additionalTargets = GetEnemiesInRadius(arrow.Position, 20f).Actors.Take(Rune_B > 0 ? 3 : 5).Where(i => (PowerMath.Distance2D(arrow.Position, i.Position) > 2f && !HasBuff<ClusterDelayedDmgBuff>(i)));
					foreach (var target in additionalTargets)
					{
						if (Rune_D > 0) (User as Player).AddPercentageHP(1);
						AddBuff(target, new ClusterDelayedDmgBuff(arrow.Position, ScriptFormula(19), dmgType));
					}

					arrow.Destroy();
					return;
				}

				//base grenades
				for (int i = 0; i < 4; i++)
				{
					var grenade = new Projectile(
						this,
						RuneSelect(
							ActorSno._demonhunter_clusterarrow_babygrenade,
							ActorSno._demonhunter_clusterarrow_babygrenade_crimson,
							ActorSno.__NONE,
							ActorSno._demonhunter_clusterarrow_babygrenade_obsidian,
							ActorSno.__NONE,
							ActorSno._demonhunter_clusterarrow_babygrenade_alabaster
						),
						arrow.Position
					);
					grenade.Position.Z += 5f;
					grenade.OnArrival = () =>
					{
						grenade.PlayEffectGroup(RuneSelect(132042, 166590, -1, 167359, -1, 166623));
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(grenade.Position, 6f);
						attack.AddWeaponDamage(ScriptFormula(5), dmgType);
						attack.OnHit = hitPayload =>
						{
							if (Rune_E > 0)     //Dazzling arrow "alabaster"
								if (!HasBuff<DebuffStunned>(hitPayload.Target) && Rand.NextDouble() < ScriptFormula(12))
									AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(13))));
						};
						attack.Apply();
						grenade.Destroy();
					};
					grenade.LaunchArc(RandomDirection(arrow.Position, 7f), 9f, -0.07f, 0.3f);
				}
				arrow.Destroy();
			};
			arrow.Launch(TargetPosition, 1.5f);

			yield break;
		}

		[ImplementsPowerBuff(1)]
		class ClusterDelayedDmgBuff : PowerBuff         //Missile strike
		{
			float Mult = 1f;
			DamageType DmgType = DamageType.Physical;
			EffectActor Emitter = null;
			Vector3D Source = null;
			float Distance = 1f;

			public ClusterDelayedDmgBuff(Vector3D source, float mult, DamageType dmgType)
			{
				Source = source;
				Mult = mult;
				DmgType = dmgType;
			}
			public override void Init()
			{
				Distance = PowerMath.Distance2D(Source, Target.Position);
				if (Distance < 1f) Distance = 1f;
				if (Distance > 10f) Distance = 10f;
				Timeout = WaitSeconds(Distance * 0.1f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Emitter = SpawnEffect(ActorSno._dh_clusterarrow_missiles_emitter, Source, 0f, WaitSeconds(Distance * 0.1f + 1f));       //Missile emitter
				Emitter.PlayEffectGroup(166596, Target);
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				WeaponDamage(Target, Mult, DmgType);
				if (Emitter != null) Emitter.Destroy();
			}
		}
	}
	#endregion

	//Not Started. Attempted and failed.
	#region Chakram
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.Chakram)]
	public class Chakram : Skill
	{

		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			//http://www.youtube.com/watch?v=9xKCTla3sQU

			//swirling motion with projectile is NoRune.

			//Rune_A has two chakrams, both same direction just flipped paths

			//Rune_B makes a boomerrang, travelling away from then back to User

			//Rune_C makes a slow curve
			//projectile speed = 0.2 while the rest is done by attack speed

			//Rune_D spirals out to target, actor calls it a straight projectile.

			//Rune_E is just a buff shield
			var runeActorSno = RuneSelect(
				ActorSno._demonhunter_chakram_projectile,
				ActorSno._demonhunter_chakram_projectile,
				ActorSno._demonhunter_chakram_indigo_boomerang_projectile,
				ActorSno._demonhunter_chakram_obsidian_slow_projectile,
				ActorSno._demonhunter_chakram_golden_straight_projectile,
				ActorSno.__NONE
			);
			var proj = new Projectile(this, runeActorSno, User.Position);
			if (Rune_E > 0)
			{
				AddBuff(User, new ChakramBuff());
			}
			else if (Rune_B > 0)
			{
				proj.Position.Z += 2f;
				proj.LaunchWA(TargetPosition, 0.8f, new Action(() => {
					proj.Launch(User.Position, 0.8f);
				}));
				proj.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(0), RuneSelect(DamageType.Physical, DamageType.Fire, DamageType.Lightning, DamageType.Poison, DamageType.Physical, DamageType.Physical));
				};

			}
			else if(Rune_C > 0)
			{
				proj.Position.Z += 2f;
				proj.Launch(TargetPosition, 0.2f);
				this.World.PlayZigAnimation(proj, User, PowerSNO, TargetPosition);
				proj.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(0), RuneSelect(DamageType.Physical, DamageType.Fire, DamageType.Lightning, DamageType.Poison, DamageType.Physical, DamageType.Physical));
				};

			}
			else
			{
				proj.Position.Z += 2f;
				proj.Launch(TargetPosition, 0.2f);
				this.World.PlaySpiralAnimation(proj, User, PowerSNO, TargetPosition);
				proj.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(0), RuneSelect(DamageType.Physical, DamageType.Fire, DamageType.Lightning, DamageType.Poison, DamageType.Physical, DamageType.Physical));
				};
				if (Rune_A > 0)
				{
					var dproj = new Projectile(this, runeActorSno, User.Position);
					dproj.Position.Z += 2f;
					dproj.Launch(TargetPosition, 0.2f);
					this.World.PlayReverSpiralAnimation(dproj, User, PowerSNO, TargetPosition);
					dproj.OnCollision = (hit) =>
					{
						WeaponDamage(hit, ScriptFormula(0), RuneSelect(DamageType.Physical, DamageType.Fire, DamageType.Lightning, DamageType.Poison, DamageType.Physical, DamageType.Physical));
					};
				}
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ChakramBuff : PowerBuff
		{
			const float _damageRate = 0.5f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(12));
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(15));
					attack.AddWeaponDamage(ScriptFormula(13), DamageType.Physical);
					attack.Apply();
				}

				return false;
			}
		}
	}
	#endregion

	//Complete
	#region Sentry //Sentry
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.Sentry)]
	public class Sentry : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			//StartCooldown(EvalTag(PowerKeys.CooldownTime));
			User.Attributes[GameAttribute.Skill_Charges, 129217] -= 1;
			var old_turret = User.World.GetActorsBySNO(RuneSelect(ActorSno._dh_sentry, ActorSno._dh_sentry_tether, ActorSno._dh_sentry_addsduration, ActorSno._dh_sentry_addsmissiles, ActorSno._dh_sentry_addsheals, ActorSno._dh_sentry_addsshield));

			//if (old_turret.Count > 0)
			int CountByHero = 0;

			foreach (var ot in old_turret)
				if (ot.Attributes[GameAttribute.Known_By_Owner] == (User as Player).PlayerIndex)
				{
					CountByHero++;
					if (CountByHero > 1)
						old_turret.First().Destroy();
				}
			
			var Turret = new EffectActor(this, RuneSelect(ActorSno._dh_sentry, ActorSno._dh_sentry_tether, ActorSno._dh_sentry_addsduration, ActorSno._dh_sentry_addsmissiles, ActorSno._dh_sentry_addsheals, ActorSno._dh_sentry_addsshield), RandomDirection(User.Position, 3f, 8f));
			Turret.Timeout = WaitSeconds(ScriptFormula(0));
			Turret.Scale = 1f;
			Turret.Spawn();
			Turret.Attributes[GameAttribute.Known_By_Owner] = (User as Player).PlayerIndex;

			if (Rune_A > 0)
				User.AddRopeEffect(154660, Turret);

			Turret.UpdateDelay = 1f / ScriptFormula(1);
			Turret.OnUpdate = () =>
			{
				var targets = GetEnemiesInRadius(Turret.Position, ScriptFormula(5)).FilterByType<Monster>();
				if (targets.Actors.Count > 0 && targets != null)
				{
					targets.SortByDistanceFrom(Turret.Position);
					var proj = new Projectile(this, ActorSno._dh_sentry_arrow, Turret.Position);
					proj.Position.Z += 5f;
					proj.OnCollision = (hit) =>
					{
						WeaponDamage(hit, ScriptFormula(2), DamageType.Physical);
						proj.Destroy();
					};
					Turret.TranslateFacing(targets.Actors[0].Position, true);
					proj.Launch(targets.Actors[0].Position, ScriptFormula(6));
				}

				if (Rune_C > 0)
					foreach (var enemy in GetEnemiesInRadius(Turret.Position, 10f).Actors)
					{
						var multi_proj = new Projectile(this, ActorSno._wardenmissile_projectile, Turret.Position);
						multi_proj.Position.Z += 5f;
						multi_proj.OnCollision = (hit) =>
						{
							WeaponDamage(hit, ScriptFormula(19), DamageType.Fire);
							multi_proj.Destroy();
						};
						multi_proj.Launch(enemy.Position, ScriptFormula(6));
					}

				if (Rune_A > 0)
					WeaponDamage(GetEnemiesInBeamDirection(Turret.Position, User.Position, PowerMath.Distance2D(User.Position, Turret.Position), 5f), ScriptFormula(6), DamageType.Fire);

				if (Rune_D > 0)
					foreach (Actor ally in Turret.GetActorsInRange(10f))
					{
						if (ally is Player)
							(ally as Player).AddHP((ally.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(11)) / ScriptFormula(1));
					}

				if (Rune_E > 0)
					foreach (Actor ally in Turret.GetActorsInRange(10f))
					{
						if (ally is Player || ally is Minion)
							AddBuff(ally, new ArmorBuff());
					}
			};
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ArmorBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(1f / ScriptFormula(1));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(10);
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(10);
			}
		}

		[ImplementsPowerBuff(1)]
		public class SentryCountBuff : PowerBuff
		{
			public bool CoolDownStarted = false;
			public uint Max = 2;

			public override bool Update()
			{
				if (base.Update())
					return true;

				if ((User as Player).SkillSet.HasPassive(0x00032EE2))
					Max = 3;
				else
				{
					Max = 2;
					if(User.Attributes[GameAttribute.Skill_Charges, PowerSNO] == 3)
						User.Attributes[GameAttribute.Skill_Charges, PowerSNO] = 2;
				}

				if (User.Attributes[GameAttribute.Skill_Charges, PowerSNO] < Max)
				{
					if (!CoolDownStarted)
					{
						StartCooldownCharges(8f); CoolDownStarted = true;

						Task.Delay(8100).ContinueWith(delegate
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
	#region Companion //Companion
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.Companion)]
	public class Companion : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (!HasBuff<CompanionPassive.CompanionPassiveBuff>(User)) yield break;

			var companion = User.World.BuffManager.GetFirstBuff<CompanionPassive.CompanionPassiveBuff>(User).companion;
			if (companion == null || companion.Dead) yield break;

			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_A > 0)     //Spider Companion
			{
				foreach (Actor enemy in GetEnemiesInRadius(companion.Position, 20f).Actors)
					if (!HasBuff<SpiderWebbedDebuff>(enemy))
						AddBuff(enemy, new SpiderWebbedDebuff());

				yield break;
			}

			if (Rune_B > 0)     //Boar Companion
			{
				foreach (Actor enemy in GetEnemiesInRadius(companion.Position, 20f).Actors)
					if (!HasBuff<DebuffTaunted>(enemy))
						AddBuff(enemy, new DebuffTaunted(WaitSeconds(5f)));

				yield break;
			}

			if (Rune_C > 0)     //Wolf Companion
			{
				var allies = GetAlliesInRadius(companion.Position, 25f).Actors;
				allies.Add(User);

				foreach (Actor ally in allies)
					if (!HasBuff<WolfDamageBuff>(ally))
						AddBuff(ally, new WolfDamageBuff());

				yield break;
			}


			if (Rune_D > 0)     //Bat Companion
			{
				GeneratePrimaryResource(ScriptFormula(8));
				yield break;
			}

			if (Rune_E > 0)     //Ferret Companion
			{
				float radius = User.Attributes[GameAttribute.Gold_PickUp_Radius];
				User.Attributes[GameAttribute.Gold_PickUp_Radius] = ScriptFormula(6);
				(User as Player).VacuumPickup();
				User.Attributes[GameAttribute.Gold_PickUp_Radius] = radius;
				yield break;
			}

			if (!HasBuff<RavenBuff>(companion))     //Raven Companion
				AddBuff(companion, new RavenBuff());

			yield break;
		}

		[ImplementsPowerBuff(4)]
		public class RavenBuff : PowerBuff
		{
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == Target)
				{
					(payload as HitPayload).TotalDamage *= 1.5f;
					Target.World.BuffManager.RemoveBuff(Target, this);
				}
			}
		}

		[ImplementsPowerBuff(2)]
		class WolfDamageBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(10f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.3f;
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.3f;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();

				Target.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= 0.3f;
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= 0.3f;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1)]
		public class SpiderWebbedDebuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(5f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.WalkSpeed *= 0.2f;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += 0.8f;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.WalkSpeed /= 0.2f;
				Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 0.8f;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(365312)]
	public class CompanionPassive : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new CompanionPassiveBuff());
			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class CompanionPassiveBuff : PowerBuff
		{
			public CompanionMinion companion = null;
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

				if (this.companion != null) return false;
				if (User.World == null) return false;
				var minionID = ActorSno._dh_companion;  //Raven
				if (User.Attributes[GameAttribute.Rune_A, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)
					minionID = ActorSno._dh_companion_spider;  //Spider, slow on hit done in HitPayload
				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)
					minionID = ActorSno._dh_companion_boar;  //Boar
				if (User.Attributes[GameAttribute.Rune_C, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)
					minionID = ActorSno._dh_companion_runec;  //Wolf
				if (User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)
					minionID = ActorSno._dh_companion_runed;  //Bat
				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)
					minionID = ActorSno._dh_companion_runee;  //Ferret

				this.companion = new CompanionMinion(this.World, this, minionID);
				this.companion.Brain.DeActivate();
				this.companion.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				this.companion.Attributes[GameAttribute.Untargetable] = true;
				this.companion.EnterWorld(this.companion.Position);
				//Logger.Debug("companion spawned");

				(this.companion as Minion).Brain.Activate();
				this.companion.Attributes[GameAttribute.Untargetable] = false;
				this.companion.Attributes.BroadcastChangedIfRevealed();

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Boar
				{
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += RegenValue;
					User.Attributes[GameAttribute.Resistance_Percent_All] += 0.2f;
				}

				if (User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Bat
				{
					User.Attributes[GameAttribute.Resource_Regen_Per_Second, 5] += 1f;
				}

				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Ferret
				{
					User.Attributes[GameAttribute.Gold_Find] += 0.1f;
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.1f;
				}

				User.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.DemonHunter.Discipline.Companion] = 1;
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				if (this.companion != null)
				{
					this.companion.Destroy();
					this.companion = null;
				}

				if (User.Attributes[GameAttribute.Rune_B, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Boar
				{
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] -= RegenValue;
					User.Attributes[GameAttribute.Resistance_Percent_All] -= 0.2f;
				}

				if (User.Attributes[GameAttribute.Rune_D, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Bat
				{
					User.Attributes[GameAttribute.Resource_Regen_Per_Second, 5] -= 1f;
				}

				if (User.Attributes[GameAttribute.Rune_E, SkillsSystem.Skills.DemonHunter.Discipline.Companion] > 0)  //Ferret
				{
					User.Attributes[GameAttribute.Gold_Find] -= 0.1f;
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.1f;
				}

				User.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.DemonHunter.Discipline.Companion] = 0;
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Vault //Vault
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.Discipline.Vault)]
	public class DemonHunterVault : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), EvalTag(PowerKeys.AttackRadius)));

			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

			UseSecondaryResource(HasBuff<VaultBuff>(User) ? (EvalTag(PowerKeys.ResourceCost) / 2f) : EvalTag(PowerKeys.ResourceCost));

			(User as Player).VaultsDone++;

			if ((User as Player).VaultsDone >= 6)
				(User as Player).GrantAchievement(74987243307059);

			(User as Player).AddTimedAction(10f, new Action<int>((q) => (User as Player).VaultsDone--));

			if (Rune_D > 0)
				if (HasBuff<VaultBuff>(User))
					RemoveBuffs(User, 111215);
				else
					AddBuff(User, new VaultBuff());
			/*
			if (EvalTag(PowerKeys.CooldownTime) == 0f)
				StartCooldown(1f);
			else
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			//*/
			ActorMover mover = new ActorMover(User);
			mover.MoveArc(TargetPosition, 1, -0.1f, new ACDTranslateArcMessage
			{
				//Field3 = 303110, // used for male barb leap, not needed?
				FlyingAnimationTagID = AnimationSetKeys.Attack2.ID,
				LandingAnimationTagID = -1,
				PowerSNO = PowerSNO
			});

			int limit = 0;
			// wait for landing
			while (!mover.Update() && limit < 180)
			{
				limit++;
				if (Rune_A > 0)
				{
					var step = SpawnEffect(ActorSno._dh_vaultrune_damage_char, User.Position, 0, WaitSeconds(ScriptFormula(13)));
					step.UpdateDelay = 0.5f;
					step.OnUpdate = () =>
					{
						WeaponDamage(GetEnemiesInRadius(step.Position, 5f), 2.5f, DamageType.Fire);
					};
				}
				if (Rune_C > 0)
				{
					var closetarget = GetEnemiesInRadius(User.Position, 10f).GetClosestTo(User.Position);
					if (closetarget != null)
					{
						var projectile = new Projectile(this, ActorSno._dh_vaultrune_projectile, User.Position);
						projectile.OnCollision = (hit) =>
						{
							WeaponDamage(hit, ScriptFormula(4), DamageType.Physical);
						};
						projectile.Launch(closetarget.Position, 0.5f);
					}
				}
				yield return WaitTicks(1);
			}

			if (Rune_E > 0)
			{
				User.PlayEffectGroup(151283);
				foreach (Actor actor in GetEnemiesInRadius(User.Position, ScriptFormula(7)).Actors)
				{
					Knockback(User.Position, actor, 5f);
					AddBuff(actor, new DebuffStunned(WaitSeconds(ScriptFormula(8))));
				}
			}

			if ((User as Player).SkillSet.HasPassive(218385)) //TacticalAdvantage (DH)
			{
				AddBuff(User, new MovementBuff(0.6f, WaitSeconds(2f)));
			}

			yield break;
		}

		[ImplementsPowerBuff(1)]
		class VaultBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(9));
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
	#region FanOfKnives //Fan of Knives
	[ImplementsPowerSNO(SkillsSystem.Skills.DemonHunter.HatredSpenders.FanOfKnives)]
	public class DemonHunterFanOfKnives : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (Rune_A > 0)
				UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			else
				StartCooldown(Rune_D > 0 ? 15f : 10f);
			int _enemiesDamaged = 0;
			if (Rune_E > 0)
			{
				AddBuff(User, new AlabasterBuff());
			}
			else
			{
				if (Rune_B > 0)
				{
					Vector3D[] targetDirs;
					targetDirs = new Vector3D[(int)ScriptFormula(13)];

					int takenPos = 0;
					foreach (Actor actor in GetEnemiesInRadius(User.Position, ScriptFormula(17)).Actors)
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
						var proj = new Projectile(this, ActorSno._demonhunter_fanofknives_knife, User.Position);
						proj.Position.Z += 5f;  // fix height
						proj.OnCollision = (hit) =>
						{
							_enemiesDamaged++;
							WeaponDamage(hit, ScriptFormula(16), DamageType.Physical);

							if ((User as Player).SkillSet.HasPassive(218398)) //NumbingTraps (DH)
							{
								if (hit.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.DamageReduceDebuff>(hit) == null)
									AddBuff(hit, new DamageReduceDebuff(0.25f, WaitSeconds(3f)));
							}

							proj.Destroy();
						};
						proj.Launch(position, ScriptFormula(7));
					}
				}

				User.PlayEffectGroup(77547);

				yield return WaitSeconds(0.5f);
				var targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = targets;
				attack.AddWeaponDamage(ScriptFormula(0), Rune_C > 0 ? DamageType.Lightning : DamageType.Physical);
				attack.OnHit = (hit) =>
				{
					_enemiesDamaged++;
					if (Rune_C > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(10))
						{
							AddBuff(hit.Target, new DebuffStunned(WaitSeconds(ScriptFormula(11))));
						}
					}
					else
						AddBuff(hit.Target, new DebuffSlowed(ScriptFormula(6), WaitSeconds(ScriptFormula(5))));
				};
				attack.Apply();

				if ((User as Player).SkillSet.HasPassive(218398)) //NumbingTraps (DH)
				{
					foreach (var tgt in targets.Actors)
						if (tgt.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.DamageReduceDebuff>(tgt) == null)
							AddBuff(tgt, new DamageReduceDebuff(0.25f, WaitSeconds(3f)));
				}
			}
			if (_enemiesDamaged >= 20)
				(User as Player).GrantAchievement(74987243307062);
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class AlabasterBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(15));
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
			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					var targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = targets;
					attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
					attack.Apply();

					if ((User as Player).SkillSet.HasPassive(218398)) //NumbingTraps (DH)
					{
						foreach (var tgt in targets.Actors)
							if (tgt.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.DamageReduceDebuff>(tgt) == null)
								AddBuff(tgt, new DamageReduceDebuff(0.25f, WaitSeconds(3f)));
					}
					base.Remove();
				}
			}
		}
	}
	#endregion

	//spirit walk: 0xF2F224EA  (used on pet proxy)
	//vault: 0x04E733FD 
	//diamondskin: 0x061F7489
	//smokescreen: 0x04E733FD
}
