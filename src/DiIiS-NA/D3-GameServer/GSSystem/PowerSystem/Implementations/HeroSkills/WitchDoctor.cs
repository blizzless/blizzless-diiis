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
	#region PoisonDart
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.PoisonDart)]
	public class WitchDoctorPoisonDart : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			int numProjectiles = Rune_B > 0 ? (int)ScriptFormula(4) : 1;
			for (int n = 0; n < numProjectiles; ++n)
			{
				if (Rune_B > 0)
					yield return WaitSeconds(ScriptFormula(17));

				var proj = new Projectile(
					this,
					RuneSelect(
						ActorSno._witchdoctor_poisondart,
						ActorSno._witchdoctor_poisondart_runea_fire,
						ActorSno._witchdoctor_poisondart_runeb_multishot,
						ActorSno._witchdoctor_poisondart_runec_slow,
						ActorSno._witchdoctor_poisondart_runed_lowcost,
						ActorSno._witchdoctor_poisondart_snakeprojectile
					),
					User.Position
				);
				proj.Position.Z += 3f;
				proj.OnCollision = (hit) =>
				{
					// TODO: fix positioning of hit actors. possibly increase model scale? 
					SpawnEffect(
						RuneSelect(
							ActorSno._witchdoctor_poisondart_poison_impact,
							ActorSno._witchdoctor_poisondart_runea_fire_impact,
							ActorSno._witchdoctor_poisondart_poison_impact,
							ActorSno._witchdoctor_poisondart_runec_slow_impact,
							ActorSno._witchdoctor_poisondart_runed_mana_impact,
							ActorSno._witchdoctor_poisondart_snakeprojectile_impact
						),
						proj.Position
					);

					proj.Destroy();

					if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(11))
						hit.PlayEffectGroup(107163);

					if (Rune_A > 0)
						WeaponDamage(hit, ScriptFormula(2), DamageType.Fire);
					else
						WeaponDamage(hit, ScriptFormula(0), DamageType.Poison);
				};
				proj.Launch(TargetPosition, 1f);

				if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
				{
					var Fetish = new FetishMelee(World, this, 0);
					Fetish.Brain.DeActivate();
					Fetish.Position = RandomDirection(User.Position, 3f, 8f);
					Fetish.Attributes[GameAttribute.Untargetable] = true;
					Fetish.EnterWorld(Fetish.Position);
					Fetish.PlayActionAnimation(90118);
					yield return WaitSeconds(0.5f);

					(Fetish as Minion).Brain.Activate();
					Fetish.Attributes[GameAttribute.Untargetable] = false;
					Fetish.Attributes.BroadcastChangedIfRevealed();
					Fetish.LifeTime = WaitSeconds(60f);
					Fetish.PlayActionAnimation(87190);
				}
			}
		}
	}
	#endregion

	//Complete
	#region PlagueOfToads
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.PlagueOfToads)]
	public class WitchDoctorPlagueOfToads : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				Fetish.Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			if (Rune_C > 0)
			{
				// NOTE: not normal plague of toads right now but Obsidian runed "Toad of Hugeness"
				Vector3D userCastPosition = new Vector3D(User.Position);
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 7f);
				var bigtoad = SpawnEffect(ActorSno._gianttoad, inFrontOfUser, TargetPosition, WaitInfinite());

				// HACK: holy hell there is alot of hardcoded animation timings here

				bigtoad.PlayActionAnimation(110766); // spawn ani
				yield return WaitSeconds(1f);

				bigtoad.PlayActionAnimation(110520); // attack ani
				TickTimer waitAttackEnd = WaitSeconds(1.5f);
				yield return WaitSeconds(0.3f); // wait for attack ani to play a bit

				var tongueEnd = SpawnProxy(TargetPosition, WaitInfinite());
				bigtoad.AddRopeEffect(107892, tongueEnd);

				yield return WaitSeconds(0.3f); // have tongue hang there for a bit

				var tongueMover = new KnockbackBuff(-0.01f, 3f, -0.1f);
				World.BuffManager.AddBuff(bigtoad, tongueEnd, tongueMover);
				if (ValidTarget())
					World.BuffManager.AddBuff(bigtoad, Target, new KnockbackBuff(-0.01f, 3f, -0.1f));

				yield return tongueMover.ArrivalTime;
				tongueEnd.Destroy();

				if (ValidTarget())
				{
					_SetHiddenAttribute(Target, true);

					if (!waitAttackEnd.TimedOut)
						yield return waitAttackEnd;

					bigtoad.PlayActionAnimation(110636); // disgest ani, 5 seconds
					for (int n = 0; n < 5 && ValidTarget(); ++n)
					{
						WeaponDamage(Target, 0.039f, DamageType.Poison);
						yield return WaitSeconds(1f);
					}

					if (ValidTarget())
					{
						_SetHiddenAttribute(Target, false);

						bigtoad.PlayActionAnimation(110637); // regurgitate ani
						World.BuffManager.AddBuff(bigtoad, Target, new KnockbackBuff(36f));
						Target.PlayEffectGroup(18281); // actual regurgitate efg isn't working so use generic acid effect
						yield return WaitSeconds(0.9f);
					}
				}

				bigtoad.PlayActionAnimation(110764); // despawn ani
				yield return WaitSeconds(0.7f);
				bigtoad.Destroy();
			}
			else if (Rune_B > 0)
			{
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 35f));
				SpawnProxy(TargetPosition).PlayEffectGroup(106365);
				yield return WaitSeconds(0.3f);
				WeaponDamage(GetEnemiesInRadius(TargetPosition, 6f), ScriptFormula(8), DamageType.Poison);
				yield return WaitSeconds(ScriptFormula(10) - 0.3f);
			}
			else
			{
				TickTimer timeout = WaitSeconds(ScriptFormula(5) + 2f);
				Projectile[] frogs = new Projectile[3];
				for (int i = 0; i < frogs.Length; ++i)
				{
					var projectile = new Projectile(
						this,
						RuneSelect(
							ActorSno._wd_plagueoftoads_toad,
							ActorSno._wd_plagueoftoadsrune_fire_toad,
							ActorSno.__NONE,
							ActorSno.__NONE,
							ActorSno._wd_plagueoftoads_toad,
							ActorSno._wd_plagueoftoadsrune_confuse_toad
						),
						User.Position
					);
					projectile.Position.Z -= 3f;
					projectile.Timeout = timeout;
					projectile.OnCollision = (hit) =>
					{
						if (!projectile.FirstTimeCollided)
						{
							projectile.PlayEffect(Effect.GorePoison);
							WeaponDamage(hit, Rune_A > 0 ? ScriptFormula(17) : ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : DamageType.Poison);

							if (Rune_E > 0 && FastRandom.Instance.NextDouble() < ScriptFormula(12))
								AddBuff(hit, new Confusion_Debuff());
						}
					};
					frogs[i] = projectile;
				}

				while (!timeout.TimedOut)
				{
					for (int i = 0; i < frogs.Length; ++i)
					{
						if (frogs[i] == null) continue;
						var target = PowerMath.GenerateSpreadPositions(frogs[i].Position, PowerMath.TranslateDirection2D(User.Position, TargetPosition, frogs[i].Position, 10f), 30f, 3)[FastRandom.Instance.Next(0, 3)];
						//frogs[i].LaunchArc(new Vector3D(RandomDirection(frogs[i].Position, 5f, 10f)), 3f, -0.03f);
						frogs[i].LaunchArc(target, 3f, -0.03f);
					}

					yield return frogs[0].ArrivalTime;
				}
				//projectile.LaunchArc(inFrontOfminiToads, 3f, -0.03f);
				//projectile.OnCollision = (hit) =>
				//{
				//destroying it while in Launcharc causes an exception.
				//projectile.Destroy();
				//};
				//projectile.OnArrival = () => { };
				//yield return WaitSeconds(1.2f);
				//projectile.LaunchArc(new Vector3D(RandomDirection(projectile.Position, 4f, 7f)), 3f, -0.03f);
				//projectile.OnArrival = () => { };
				//yield return WaitSeconds(1.2f);
			}

			yield break;
		}

		[ImplementsPowerBuff(1)]
		class Confusion_Debuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(13));
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

		private void _SetHiddenAttribute(Actor actor, bool active)
		{
			actor.Attributes[GameAttribute.Hidden] = active;
			actor.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion

	//Complete
	#region GraspOfTheDead
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.GraspOfTheDead)]
	public class GraspOfTheDead : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			//UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			UsePrimaryResource(150);

			if (Rune_B > 0)
			{
				for (int i = 0; i < 4; ++i)
				{
					var Target = GetEnemiesInRadius(TargetPosition, ScriptFormula(14)).GetClosestTo(TargetPosition);
					if (Target != null)
					{
						SpawnEffect(ActorSno._witchdoctor_graspofthedead_indigorune_proxyactor, Target.Position);
						WeaponDamage(GetEnemiesInRadius(Target.Position, ScriptFormula(15)), ScriptFormula(10), DamageType.Holy);
						yield return WaitSeconds(ScriptFormula(13));
					}
				}
			}
			else
			{
				var Ground = SpawnEffect(
					RuneSelect(
						ActorSno._witchdoctor_graspofthedead_proxyactor,
						ActorSno._witchdoctor_graspofthedead_crimsonrune_proxyactor,
						ActorSno.__NONE,
						ActorSno._witchdoctor_graspofthedead_obsidianrune_proxyactor,
						ActorSno._witchdoctor_graspofthedead_goldenrune_proxyactor,
						ActorSno._witchdoctor_graspofthedead_alabasterrune_proxyactor
					), 
					TargetPosition,
					0,
					WaitSeconds(ScriptFormula(8))
				);
				Ground.UpdateDelay = 0.5f;
				Ground.OnUpdate = () =>
				{
					foreach (Actor enemy in GetEnemiesInRadius(TargetPosition, ScriptFormula(3)).Actors)
					{
						AddBuff(enemy, new DebuffSlowed(ScriptFormula(19), WaitSeconds(ScriptFormula(2))));
						AddBuff(enemy, new DamageGroundDebuff((User as Player)));
					}
				};
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class DamageGroundDebuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			Player plr = null;

			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(2));
			}

			public DamageGroundDebuff(Player player)
			{
				plr = player;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is DeathPayload)
				{
					if (Rune_E > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(21))
						{
							//produce a health globe or summon a dog
							if (FastRandom.Instance.NextDouble() > 0.5)
								Target.World.SpawnHealthGlobe(Target, plr, Target.Position);
							else
							{
								var dog = new ZombieDog(User.World, plr, 0);
								dog.Brain.DeActivate();
								dog.Position = Target.Position;
								dog.Attributes[GameAttribute.Untargetable] = true;
								dog.EnterWorld(dog.Position);
								dog.PlayActionAnimation(11437);
								Task.Delay(1000).ContinueWith(d =>
								{
									dog.Brain.Activate();
									dog.Attributes[GameAttribute.Untargetable] = false;
									dog.Attributes.BroadcastChangedIfRevealed();
									dog.PlayActionAnimation(11431);
								});
							}

						}
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

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
					attack.Apply();
				}

				return false;
			}
		}
	}
	#endregion

	//TODO:checking for all Haunts in a 90f radius. needs checking
	//TODO:also needs to check if monster already has haunt, to look for a new target or overwrite the existing one.
	//TODO: Something very buggy with complexEffect... very very buggy. (ex. cast spell, click-hold mouse move around)
	#region Haunt
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.Haunt)]
	public class Haunt : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			//Need to check for all Haunt Buffs in this radius.
			//Max simultaneous haunts = 3 ScriptFormula(8)
			//Max Haunt Check Radius(ScriptFormula(9)) -> 90f

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			if (Rune_B > 0)
			{
				if (Target == null)
				{

					var Lingerer = SpawnEffect(ActorSno._wd_hauntrune_indigo_spiritemitter, TargetPosition, 0, WaitSeconds(ScriptFormula(4)));
					Lingerer.OnTimeout = () =>
					{
						Lingerer.World.BuffManager.RemoveAllBuffs(Lingerer);
					};
					AddBuff(Lingerer, new HauntLinger());
				}
			}
			else
			{
				if (Target != null)
				{
					//User.AddComplexEffect(19257, Target);
					AddBuff(Target, new Haunt1());
				}
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class Haunt1 : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}
			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is DeathPayload)
				{
					//Need to check if monster already has a haunt on them, if it does -> check next monster.
					var target = GetEnemiesInRadius(payload.Context.Target.Position, ScriptFormula(10))
						.GetClosestTo(payload.Context.Target.Position);
					if (target != null)
					{
						//Target.AddComplexEffect(RuneSelect(19257, 111257, 111370, 113742, 111461, 111564), target);
						AddBuff(target, new Haunt1());
					}
					else
					{
						if (Rune_B > 0)
						{
							var Lingerer = SpawnEffect(ActorSno._wd_hauntrune_indigo_spiritemitter, Target.Position, 0, WaitSeconds(ScriptFormula(4)));
							Lingerer.OnTimeout = () =>
							{
								Lingerer.World.BuffManager.RemoveAllBuffs(Lingerer);
							};
							AddBuff(Lingerer, new HauntLinger());
						}
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

					if (Rune_D > 0)
					{
						GeneratePrimaryResource(ScriptFormula(3));
					}

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage((ScriptFormula(0) / ScriptFormula(1)), DamageType.Cold);
					attack.AutomaticHitEffects = false;
					attack.OnHit = hit =>
					{
						if (Rune_A > 0)
						{
							//45% of damage healed back to user
							float healMe = ScriptFormula(2) * hit.TotalDamage;
							if (User is Player)
								(User as Player).AddHP(healMe);
							//User.Attributes[GameAttribute.Hitpoints_Granted] = healMe;
							//User.Attributes.BroadcastChangedIfRevealed();
						}
						if (Rune_C > 0)
						{
							//DebuffSlowed Target 
							AddBuff(Target, new DebuffSlowed(ScriptFormula(5), WaitSeconds(ScriptFormula(1))));
						}
					};
					attack.Apply();
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
		//Rune_B
		[ImplementsPowerBuff(2)]
		class HauntLinger : PowerBuff
		{
			const float _damageRate = 1.25f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(4));
			}

			//Search 
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);
					//When this finds a target, it needs to destroy the spawnactor[Lingerer]
					if (GetEnemiesInRadius(Target.Position, ScriptFormula(10)).Actors.Count > 0)
					{
						var target = GetEnemiesInRadius(Target.Position, ScriptFormula(10)).GetClosestTo(Target.Position);
						//Target.AddComplexEffect(19257, target);
						AddBuff(target, new Haunt1());
					}

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
	#region ZombieCharger
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.ZombieCharger)]
	public class WitchDoctorZombieCharger : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			var runeActorSno = RuneSelect(
				ActorSno._witchdoctor_zombiecharger_projectile,
				ActorSno._witchdoctor_zombiecharger_projectile_crimsonrune,
				ActorSno._x1_wd_zombiecharger_frost_bear,
				ActorSno._witchdoctor_zombiecharger_projectile,
				ActorSno._witchdoctor_zombiecharger_projectile_goldenrune,
				ActorSno._witchdoctor_zombiecharger_projectile_alabasterrune
			);

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			if (Rune_A > 0)
			{
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 1f);
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 10f, 3);

				var BearProj1 = new Projectile(this, runeActorSno, inFrontOfUser);
				BearProj1.Position.Z -= 3f;
				BearProj1.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
				};
				BearProj1.Launch(projDestinations[1], ScriptFormula(19));

				yield return WaitSeconds(0.5f);
				var BearProj2 = new Projectile(this, runeActorSno, inFrontOfUser);
				BearProj2.Position.Z -= 3f;
				BearProj2.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
				};
				BearProj2.Launch(projDestinations[0], ScriptFormula(19));

				yield return WaitSeconds(0.5f);
				var BearProj3 = new Projectile(this, runeActorSno, inFrontOfUser);
				BearProj3.Position.Z -= 3f;
				BearProj3.OnCollision = (hit) =>
				{
					WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
				};
				BearProj3.Launch(projDestinations[2], ScriptFormula(19));
			}
			else if (Rune_B > 0)
			{
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, ScriptFormula(10), (int)ScriptFormula(3));

				for (int i = 1; i < projDestinations.Length; i++)
				{
					var multiproj = new Projectile(this, runeActorSno, inFrontOfUser);
					multiproj.Position.Z -= 3f;
					multiproj.OnCollision = (hit) =>
					{
						WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
					};
					multiproj.Launch(projDestinations[i], ScriptFormula(1));
				}
			}
			else if (Rune_D > 0)
			{
				int maxZombies = (int)ScriptFormula(24);
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
				var proj = new Projectile(this, runeActorSno, inFrontOfUser);
				proj.Position.Z -= 3f;
				proj.Launch(TargetPosition, ScriptFormula(1));
				proj.OnCollision = (hit) =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(hit);
					attack.AddWeaponDamage((maxZombies == (int)ScriptFormula(24) ? ScriptFormula(4) : ScriptFormula(31)), DamageType.Poison);
					attack.Apply();
					attack.OnDeath = DeathPayload =>
					{
						//Vector3D inFrontOfUser2 = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 22f);
						//var proj2 = new Projectile(this, RuneSelect(74056, 105501, 105543, 105463, 105969, 105812), inFrontOfUser2);
						maxZombies--;
						if (maxZombies > 0)
							proj.Launch(PowerMath.TranslateDirection2D(User.Position, TargetPosition, proj.Position, ScriptFormula(13)), ScriptFormula(14));
						//zombie new distance (SF(13))
						//zombie speed (SF(14))
						//New zombie search range (SF(15))
						//max new zombie per projectile (SF24)
						//damage scalar -> SF31
						//damage reduction per zombie -> SF30
					};
				};
			}
			else if (Rune_E > 0)
			{
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
				var proj = new Projectile(this, runeActorSno, inFrontOfUser);
				proj.Position.Z -= 3f;
				proj.OnCollision = (hit) =>
				{
					WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(11)), ScriptFormula(4), DamageType.Fire);
				};
				proj.Launch(TargetPosition, ScriptFormula(7));
			}
			else
			{
				Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 3f);
				if (Rune_C > 0)
				{
					var tower1 = SpawnEffect(ActorSno._wd_wallofzombies_tower_zombie1, inFrontOfUser, 0, WaitSeconds(1.5f));
					var tower2 = SpawnEffect(ActorSno._wd_wallofzombies_tower_zombie2, inFrontOfUser, 0, WaitSeconds(1.5f));
					var tower3 = SpawnEffect(ActorSno._wd_wallofzombies_tower_zombie3, inFrontOfUser, 0, WaitSeconds(1.5f));
					var tower4 = SpawnEffect(ActorSno._wd_wallofzombies_tower_zombie4, inFrontOfUser, 0, WaitSeconds(1.5f));
					tower1.OnTimeout = () =>
					{
						AttackPayload attack = new AttackPayload(this);
						attack.Targets = GetEnemiesInRadius(inFrontOfUser, ScriptFormula(8));
						attack.AddWeaponDamage(ScriptFormula(6), DamageType.Poison);
						attack.Apply();
						tower1.PlayEffect(Effect.GorePoison);
						tower2.PlayEffect(Effect.GorePoison);
						tower3.PlayEffect(Effect.GorePoison);
						tower4.PlayEffect(Effect.GorePoison);
					};
				}
				else
				{
					var proj = new Projectile(this, runeActorSno, inFrontOfUser);
					proj.Position.Z -= 3f;
					proj.OnCollision = (hit) =>
					{
						WeaponDamage(hit, ScriptFormula(4), DamageType.Poison);
					};
					proj.Launch(TargetPosition, ScriptFormula(1));
				}

			}

			yield break;
		}
	}
	#endregion

	//Complete
	#region Horrify
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.Horrify)]
	public class Horrify : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(218501))
				StartCooldown(Math.Max(ScriptFormula(14) - 2f, 0));
			else
				StartCooldown(ScriptFormula(14));

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			AddBuff(User, new CastEffect());
			if (Rune_A > 0)
			{
				AddBuff(User, new CrimsonBuff());
			}
			if (Rune_E > 0)
			{
				AddBuff(User, new SprintBuff());
			}
			foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(2)).Actors)
			{
				AddBuff(enemy, new DebuffFeared(WaitSeconds(ScriptFormula(3))));
				if (Rune_D > 0)
				{
					GeneratePrimaryResource(ScriptFormula(8));
				}
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class CastEffect : PowerBuff
		{
			//switch.efg
			public override void Init()
			{
				Timeout = WaitSeconds(1f);
			}
		}
		[ImplementsPowerBuff(1)]
		class SprintBuff : PowerBuff
		{
			//spring.etf alabaster
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(10));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(9);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(9);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(2)]
		class CrimsonBuff : PowerBuff
		{
			//crimson buff
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(12));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(5);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(5);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Firebats
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.Firebats)]
	public class Firebats : ChanneledSkill
	{
		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.1f;
			User.Attributes[GameAttribute.Projectile_Speed] = User.Attributes[GameAttribute.Projectile_Speed] * ScriptFormula(22);
			if (Rune_D > 0)
				User.Attributes[GameAttribute.Steal_Health_Percent] += 0.025f;
			User.Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnChannelClose()
		{
			if (Rune_D > 0)
				User.Attributes[GameAttribute.Steal_Health_Percent] -= 0.025f;
			User.Attributes[GameAttribute.Projectile_Speed] = User.Attributes[GameAttribute.Projectile_Speed] / ScriptFormula(22);
			User.Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnChannelUpdated()
		{
			User.TranslateFacing(TargetPosition);
		}

		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(ScriptFormula(0) * 0.25f); //resourceCostReduction

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			if (Rune_A > 0)
			{
				//Projectile Giant Bat Actors
				var proj = new Projectile(this, ActorSno._wd_firebatsrune_giant_batprojectile, User.Position);
				proj.Position.Z += 5f;  // unknown if this is needed
				proj.OnCollision = (hit) =>
				{
					SpawnEffect(ActorSno._wd_firebatsrune_giant_explosion, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z));
					AddBuff(hit, new BatDamage());
					proj.Destroy();
				};
				proj.Launch(TargetPosition, ScriptFormula(8));

				yield return WaitSeconds(ScriptFormula(17));
			}
			else if (Rune_B > 0)
			{
				if (GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(10), ScriptFormula(4)).Actors != null)
				{
					var proj = new Projectile(this, ActorSno._wd_firebatsrune_missiles_bat, User.Position);
					proj.Position.Z += 5f;
					proj.OnCollision = (hit) =>
					{
						hit.PlayEffectGroup(106575);
						AddBuff(hit, new BatDamage());
						proj.Destroy();
					};
					proj.Launch(new Vector3D(TargetPosition.X, TargetPosition.Y, User.Position.Z + 5f), ScriptFormula(8));
				}
				yield return WaitSeconds(ScriptFormula(6));
			}
			else if (Rune_E > 0)
			{
				AddBuff(User, new FirebatCast());
				foreach (Actor actor in GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(3), ScriptFormula(4)).Actors)
				{
					AddBuff(actor, new BatDamage());
				}

				yield return WaitSeconds(ScriptFormula(20));
			}
			else
			{
				AddBuff(User, new FirebatCast());
				foreach (Actor actor in GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(3), ScriptFormula(4)).Actors)
				{
					AddBuff(actor, new BatDamage());
				}
				yield return WaitSeconds(ScriptFormula(7));
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class FirebatCast : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(7));
			}
		}

		[ImplementsPowerBuff(2)]
		class BatDamage : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				if (Rune_C > 0)
				{
					Timeout = WaitSeconds(3f);
				}
				else
					Timeout = WaitSeconds(1f);
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);
					if (Rune_E > 0)
					{
						WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(21)), ScriptFormula(19), DamageType.Fire);
					}
					else if (Rune_A > 0)
					{
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(ScriptFormula(15), DamageType.Fire);
						attack.Apply();
					}
					else if (Rune_B > 0)
					{
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(ScriptFormula(18), DamageType.Fire);
						attack.Apply();
					}
					else if (Rune_C > 0)
					{
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(ScriptFormula(12), DamageType.Poison);
						attack.OnHit = HitPayload =>
						{
						};
						attack.Apply();
					}
					else
					{
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(Target);
						attack.AddWeaponDamage(ScriptFormula(1), DamageType.Fire);
						attack.OnHit = HitPayload =>
						{
							//Rune_D -> Healing
						};
						attack.Apply();
					}
				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region Firebomb
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.Firebomb)]
	public class Firebomb : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			Projectile[] grenades = new Projectile[1];
			for (int i = 0; i < grenades.Length; ++i)
			{
				var projectile = new Projectile(this, ActorSno._wd_fireball_head_projectile, User.Position);
				grenades[i] = projectile;
			}

			float height = ScriptFormula(3);

			for (int i = 0; i < grenades.Length; ++i)
			{
				grenades[i].LaunchArc(PowerMath.TranslateDirection2D(TargetPosition, User.Position, TargetPosition,
																	  0f), height, ScriptFormula(2));
			}
			yield return grenades[0].ArrivalTime;

			if (Rune_E > 0)
			{
				int jumps = (int)ScriptFormula(15);
				var grenadeN = grenades[0];
				WeaponDamage(GetEnemiesInRadius(grenadeN.Position, ScriptFormula(11)).Actors[0], ScriptFormula(10), DamageType.Fire);

				for (int i = 0; i < jumps; i++)
				{
					var targets = GetEnemiesInRadius(grenadeN.Position, ScriptFormula(11));
					if (targets.Actors.Count > 0)
					{
						var target = targets.Actors[FastRandom.Instance.Next(0, targets.Actors.Count)];

						grenadeN.LaunchArc(PowerMath.TranslateDirection2D(grenadeN.Position, target.Position, grenadeN.Position, PowerMath.Distance2D(grenadeN.Position, target.Position)), height, ScriptFormula(2));
						yield return grenadeN.ArrivalTime;

						WeaponDamage(target, ScriptFormula(10) * (1 - ((ScriptFormula(16) / 100f) * i)), DamageType.Fire);
					}
				}
			}
			else if (Rune_D > 0)
			{
				var heads = User.World.GetActorsBySNO(ActorSno._wd_fireball_groundmiss);
				foreach (var h in heads)
				{
					h.Destroy();
				}
				var FireColumn = new EffectActor(this, ActorSno._wd_fireball_groundmiss, new Vector3D(TargetPosition.X, TargetPosition.Y, TargetPosition.Z + 5));
				FireColumn.Timeout = WaitSeconds(ScriptFormula(14));
				FireColumn.Scale = 2f;
				FireColumn.Spawn();
				FireColumn.UpdateDelay = 0.33f; // attack every half-second
				FireColumn.OnUpdate = () =>
				{
					var targets = GetEnemiesInRadius(FireColumn.Position, ScriptFormula(26));
					if (targets.Actors.Count > 0 && targets != null)
					{
						targets.SortByDistanceFrom(FireColumn.Position);
						var proj = new Projectile(this, ActorSno._wd_fireball_head_projectile, FireColumn.Position);
						proj.Position.Z += 5f;  // unknown if this is needed
						proj.OnCollision = (hit) =>
						{
							WeaponDamage(hit, ScriptFormula(13), DamageType.Fire);

							proj.Destroy();
						};
						FireColumn.TranslateFacing(targets.Actors[0].Position, true);
						proj.LaunchArc(targets.Actors[0].Position, ScriptFormula(3), ScriptFormula(2));
					}

				};
			}
			else
			{
				foreach (var grenade in grenades)
				{
					var grenadeN = grenade;

					SpawnEffect(ActorSno._wd_fireball_groundmiss, TargetPosition);

					if (Rune_C > 0)
					{
						var pool = SpawnEffect(ActorSno._witchdoctor_firebombpool, grenade.Position, 0, WaitSeconds(ScriptFormula(12)));
						pool.UpdateDelay = 1f;
						pool.OnUpdate = () =>
						{
							WeaponDamage(GetEnemiesInRadius(grenadeN.Position, ScriptFormula(11)), ScriptFormula(10), DamageType.Fire);
						};
					}

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(grenade.Position, ScriptFormula(4));
					attack.AddWeaponDamage(ScriptFormula(0), DamageType.Fire);
					attack.Apply();
					if (Rune_A > 0)
					{
						SpawnEffect(ActorSno._wd_fireball_groundmiss_radius, grenade.Position);
						WeaponDamage(GetEnemiesInRadius(grenadeN.Position, ScriptFormula(5)), ScriptFormula(21), DamageType.Fire);
					}
					if (Rune_B > 0)
					{
						int jumps = (int)ScriptFormula(7);
						for (int i = 0; i < jumps; i++)
						{
							grenadeN.LaunchArc(PowerMath.TranslateDirection2D(User.Position, TargetPosition, grenadeN.Position, PowerMath.Distance2D(User.Position, TargetPosition)), height, ScriptFormula(2));
							yield return grenadeN.ArrivalTime;

							SpawnEffect(ActorSno._wd_fireball_groundmiss, grenadeN.Position);

							AttackPayload bonus_attack = new AttackPayload(this);
							bonus_attack.Targets = GetEnemiesInRadius(grenadeN.Position, ScriptFormula(4));
							bonus_attack.AddWeaponDamage(ScriptFormula(0), DamageType.Fire);
							bonus_attack.Apply();
						}
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region SpiritWalk
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.SpiritWalk)]
	public class SpiritWalk : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(218501))
				StartCooldown(Math.Max(EvalTag(PowerKeys.CooldownTime) - 2f, 0));
			else
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			//Newest Patch adds Run Speed Increase = SF(16) "0.50"
			Vector3D DecoySpot = new Vector3D(User.Position);
			Actor blast = SpawnProxy(DecoySpot);

			//SpawnEffect(106584, DecoySpot, 0, WaitSeconds(ScriptFormula(0))); //Male
			SpawnEffect(ActorSno._witchdoctor_spiritwalk_dummy_female, DecoySpot, 0, WaitSeconds(ScriptFormula(0))); //Female


			AddBuff(User, new SpiritWalkBuff());

			if (Rune_C > 0)
			{
				yield return WaitSeconds(ScriptFormula(0));
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(blast.Position, ScriptFormula(8));
				attack.AddWeaponDamage(ScriptFormula(6), DamageType.Fire);
				attack.Apply();
			}
			else

				yield break;
		}
		[ImplementsPowerBuff(1)]
		class SpiritWalkBuff : PowerBuff
		{
			const float _damageRate = 0.25f;
			TickTimer _damageTimer = null;

			float regenBonus = 0f;

			//Look_override ghostly appearance
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.Has_Look_Override] = true;//unchecked((int)0xF2F224EA);
				User.Attributes[GameAttribute.Walk_Passability_Power_SNO] = 106237;
				User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.2f;
				//User.Attributes[GameAttribute.Stealthed] = true;
				//User.Attributes[GameAttribute.Untargetable] = true;
				//User.Attributes[GameAttribute.UntargetableByPets] = true;
				if (Rune_D > 0)
				{
					regenBonus = ScriptFormula(9) * User.Attributes[GameAttribute.Resource_Max_Total, 0];
					User.Attributes[GameAttribute.Resource_Regen_Percent_Per_Second, 0] += regenBonus;
				}
				if (Rune_E > 0)
				{
					regenBonus = ScriptFormula(10) * User.Attributes[GameAttribute.Hitpoints_Max_Total, 0];
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += regenBonus;
				}
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.2f;
				if (Rune_D > 0)
				{
					User.Attributes[GameAttribute.Resource_Regen_Percent_Per_Second, 0] -= regenBonus;
				}
				if (Rune_E > 0)
				{
					//is this attribute by percent on its own? "Gain 16% of your maximum Life every second"
					User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= regenBonus;
				}
				User.Attributes[GameAttribute.Has_Look_Override] = false;
				User.Attributes[GameAttribute.Walk_Passability_Power_SNO] = -1;
				//User.Attributes[GameAttribute.Stealthed] = false;
				//User.Attributes[GameAttribute.Untargetable] = false;
				//User.Attributes[GameAttribute.UntargetableByPets] = false;
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (Rune_A > 0)
				{
					if (_damageTimer == null || _damageTimer.TimedOut)
					{
						_damageTimer = WaitSeconds(_damageRate);

						foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(4)).Actors)
						{
							AddBuff(Target, new SpiritWalkDamage());
						}
					}
				}
				return false;
			}
		}
		/*[ImplementsPowerBuff(1)]
		class DecoyLookBuff : PowerBuff
		{
			//Brain or something?
			//Breakable shield HP? idk..
			//Dummy Health -> Script(11) * MaxHP -> once this  
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}
		}*/
		[ImplementsPowerBuff(2)]
		class SpiritWalkDamage : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(4));
					attack.AddWeaponDamage(ScriptFormula(1), DamageType.Physical);
					attack.Apply();
				}
				return false;
			}
		}
	}
	#endregion

	//Seems alright.. just needs tweaking, check on the teleportation of the character occassionally when casting.
	#region SoulHarvest
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.SoulHarvest)]
	public class SoulHarvest : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(218501))
				StartCooldown(Math.Max(EvalTag(PowerKeys.CooldownTime) - 2f, 0));
			else
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			User.PlayEffectGroup(19275);

			foreach (Actor enemy in GetEnemiesInRadius(User.Position, ScriptFormula(3), 5).Actors)
			{
				enemy.PlayEffectGroup(1164);
				//WeaponDamage(enemy, 30f, DamageType.Physical);
				//enemy.AddComplexEffect(19277, User);
				if (Rune_E > 0)
				{
					WeaponDamage(enemy, ScriptFormula(0), DamageType.Physical);
				}
				if (Rune_C > 0)
				{
					AddBuff(enemy, new ObsidianDebuff());
					AddBuff(enemy, new DebuffSlowed(ScriptFormula(10), WaitSeconds(ScriptFormula(11))));
				}
				AddBuff(User, new soulHarvestbuff());
			}

			yield break;
		}
		[ImplementsPowerBuff(0, true)]
		class soulHarvestbuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(7));
				MaxStackCount = (int)ScriptFormula(13);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				_AddHarvest();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				{
					Target.Attributes[GameAttribute.Intelligence] -= StackCount * ScriptFormula(8);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				if (stacked)
					_AddHarvest();

				return true;
			}
			private void _AddHarvest()
			{
				if (Rune_A > 0)
				{
					if (User is Player)
						(User as Player).AddHP(ScriptFormula(9));
				}
				if (Rune_D > 0)
				{
					GeneratePrimaryResource(ScriptFormula(4));
				}

				Target.Attributes[GameAttribute.Intelligence] += ScriptFormula(8);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(1)]
		class ObsidianDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(11));
			}
		}
	}
	#endregion

	//Complete
	#region LocustSwarm
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.LocustSwarm)]
	public class LocustSwarm : Skill
	{
		//Summon a plague of locusts to assault enemies, dealing [25 * {Script Formula 18} * 100]% weapon damage per second as Poison for 3 seconds. The locusts will jump to additional nearby targets.
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			//cast, spread to those in radius, from there jump to other mobs in area within (__?__)
			//does not always focus the correct way.
			User.PlayEffectGroup(106765);

			//just a little wait for the animation
			yield return WaitSeconds(0.5f);


			foreach (Actor LocustTarget in GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(14), 30f).Actors)
			{
				AddBuff(User, LocustTarget, new LocustSwarmer(WaitSeconds(ScriptFormula(1))));
				if (Rune_D > 0 && User is Player)
					(User as Player).GeneratePrimaryResource(ScriptFormula(10));
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class LocustSwarmer : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			float _jumpRate = 3f;
			TickTimer _jumpTimer = null;

			public LocustSwarmer(TickTimer timeout)
			{
				Timeout = timeout;
			}
			public override bool Apply()
			{
				return base.Apply();
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : DamageType.Poison);
					attack.AutomaticHitEffects = false;
					attack.Apply();
				}

				if (_jumpRate > ScriptFormula(4))
				{
					if (_jumpTimer == null || _jumpTimer.TimedOut)
					{
						_jumpTimer = WaitSeconds(_jumpRate);
						//swarm jump radius
						var newTargets = GetEnemiesInRadius(Target.Position, ScriptFormula(2)).Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, Target.Position)).Take(Rune_B > 0 ? 2 : 1);
						foreach (var target in newTargets)
						{
							//Target.AddComplexEffect(106839, target);
							AddBuff(User, target, new LocustSwarmer(WaitSeconds(ScriptFormula(1))));
						}
						_jumpRate *= 0.9f; //delta = Swarm Jump Time Delta
					}
				}

				return false;
			}
			public override void OnPayload(Payload payload)
			{
				if (Rune_E > 0 && payload.Target == Target && payload is DeathPayload)
				{
					var swarm = new EffectActor(this, ActorSno._swarm_d, new Vector3D(Target.Position.X, Target.Position.Y, Target.Position.Z + 3));
					swarm.Timeout = WaitSeconds(ScriptFormula(11));
					swarm.Scale = 1f;
					swarm.Spawn();
					swarm.UpdateDelay = 1f; // attack every second
					swarm.OnUpdate = () =>
					{
						WeaponDamage(GetEnemiesInRadius(swarm.Position, 5f), ScriptFormula(12), DamageType.Poison);
					};
				}
			}
		}
		[ImplementsPowerBuff(1)]
		class DiseaseSwarm : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(11));
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(ScriptFormula(1), DamageType.Fire);
					attack.Apply();


				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region SpiritBarrage
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.SpiritBarrage)]
	public class SpiritBarrage : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			if (Rune_E > 0)
			{
				AddBuff(User, new BarrageSpirit());
			}
			else if (Rune_C > 0)
			{
				//this doesnt work.
				TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 35f));
				var AOE_Ghost = SpawnEffect(ActorSno._wd_spiritbarragerune_aoe_ghostmodel, TargetPosition, 0, WaitSeconds(ScriptFormula(11)));
				AOE_Ghost.PlayEffectGroup(186804);
				AOE_Ghost.UpdateDelay = 1f;
				AOE_Ghost.OnUpdate = () =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(AOE_Ghost.Position, ScriptFormula(13));
					attack.AddWeaponDamage(ScriptFormula(14), DamageType.Cold);
					attack.Apply();
				};
			}
			else
			{
				var proj = new Projectile(
					this,
					RuneSelect(
						ActorSno._wd_spiritbarrage_ghost,
						ActorSno._wd_spiritbarragerune_heal_ghost,
						ActorSno._wd_spiritbarragerune_multi_ghost,
						ActorSno._wd_spiritbarragerune_mana_ghost,
						ActorSno._wd_spiritbarrage_ghost,
						ActorSno._wd_spiritbarrage_ghost
					),
					User.Position
				);
				proj.Position.Z += 5f;
				proj.Timeout = WaitSeconds(1.5f);
				proj.OnCollision = (hit) =>
				{
					if (hit != null)
					{
						User.PlayEffectGroup(175350, hit);
						//yield return WaitSeconds(ScriptFormula(2));
						hit.PlayEffectGroup(175403);
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(hit);
						attack.AddWeaponDamage(ScriptFormula(0), DamageType.Cold);
						attack.OnHit = (hitPayload) =>
						{
							if (Rune_D > 0)
								GeneratePrimaryResource(ScriptFormula(5));

							if (Rune_A > 0)
								if (User is Player)
									(User as Player).AddHP(hitPayload.TotalDamage * ScriptFormula(4));
						};
						attack.Apply();
					}
				};
				proj.LaunchArc(TargetPosition, 3f, -0.03f);

				if (Rune_B > 0)
				{
					for (int i = 0; i < ScriptFormula(6); i++)
					{
						var target = GetEnemiesInRadius(User.Position, 40f).GetClosestTo(User.Position);
						if (target != null)
						{
							var add_proj = new Projectile(this, ActorSno._wd_spiritbarragerune_multi_ghost, User.Position);
							add_proj.Position.Z += 5f;
							add_proj.Timeout = WaitSeconds(1.5f);
							add_proj.OnCollision = (hit) =>
							{
								if (hit != null)
								{
									User.PlayEffectGroup(175350, hit);
									//yield return WaitSeconds(ScriptFormula(2));
									hit.PlayEffectGroup(175403);
									AttackPayload attack = new AttackPayload(this);
									attack.SetSingleTarget(hit);
									attack.AddWeaponDamage(ScriptFormula(8), DamageType.Cold);
									attack.Apply();
								}
							};
							add_proj.LaunchArc(target.Position, 3f, -0.03f);
						}
					}
				}
			}
			yield break;
		}

		[ImplementsPowerBuff(3)]
		class BarrageSpirit : PowerBuff
		{
			const float _damageRate = 0.6f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(17));
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					var Target = GetEnemiesInRadius(User.Position, ScriptFormula(19)).GetClosestTo(User.Position);
					if (Target != null)
					{
						//needs to turn to shoot at enemies.
						User.PlayEffectGroup(181866, Target);
						User.Position.Z += 10f; // this doesnt change the projectile position.
						WaitSeconds(ScriptFormula(2));
						Target.PlayEffectGroup(181944);

						WeaponDamage(Target, ScriptFormula(20), DamageType.Physical);

					}
				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region AcidCloud
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.AcidCloud)]
	public class AcidCloud : Skill
	{
		//TODO: Max Pools = 3;
		//Rune_B Splash Delay?
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				Fetish.Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			if (Rune_E > 0)
			{
				var acid = SpawnEffect(ActorSno._wd_acidcloudrune_barf_pools, TargetPosition, User.Position);
				yield return WaitSeconds(ScriptFormula(32));
				WeaponDamage(GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(31), ScriptFormula(30)), ScriptFormula(2), DamageType.Poison);
			}
			else
			{
				SpawnEffect(RuneSelect(ActorSno._wd_acidcloud, ActorSno._wd_acidcloudrune_damage, ActorSno._wd_acidcloudrune_splash, ActorSno._wd_acidcloudrune_slimes, ActorSno._wd_acidcloudrune_disease, ActorSno.__NONE), TargetPosition);
				yield return WaitSeconds(ScriptFormula(14));
				WeaponDamage(GetEnemiesInRadius(TargetPosition, ScriptFormula(1)), ScriptFormula(2), DamageType.Poison);
			}

			//slime -> 121595.ACR
			//this is a pet and theyre are a max of 3 allowed.
			//spawn slime that wanders in a certain area
			var AcidPools = SpawnEffect(Rune_C > 0 ? ActorSno._wd_acidcloudrune_slime : ActorSno._wizard_acidcloud_pools, TargetPosition, 0, WaitSeconds(ScriptFormula(5)));
			AcidPools.UpdateDelay = ScriptFormula(7);
			AcidPools.OnUpdate = () =>
			{
				foreach (Actor enemy in GetEnemiesInRadius(TargetPosition, ScriptFormula(4)).Actors)
				{
					if (AddBuff(enemy, new Disease_Debuff()))
					{
						AddBuff(enemy, new Disease_Debuff());
					}
				}
				//WeaponDamage(GetEnemiesInRadius(acid.Position, ScriptFormula(4)), ScriptFormula(8), DamageType.Poison);
			};

			yield break;
		}
		[ImplementsPowerBuff(2)]
		class Disease_Debuff : PowerBuff
		{
			float _damageRate = 0.25f; //this needs to be ScriptFormula(7) = buff tickrate
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(0.25f);
				_damageRate = ScriptFormula(7);
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(Target, ScriptFormula(8), DamageType.Poison);
				}
				return false;
			}
		}
	}
	#endregion

	//TODO: confusion ID for monsters, Runes_C,E(dogs), check equations.
	#region MassConfusion
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.SpiritRealm.MassConfusion)]
	public class MassConfusion : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(208565)) //RushOfEssence (WD)
				(User as Player).RestoreMana(100f, 10);

			//Target.PlayEffectGroup(184540);
			TargetList Half = GetEnemiesInRadius(TargetPosition, ScriptFormula(1));
			foreach (Actor enemy in GetEnemiesInRadius(TargetPosition, ScriptFormula(1), ((int)Half.Actors.Count / 2)).Actors)
			{
				AddBuff(enemy, new Confusion_Debuff());
			}
			//WeaponDamage(GetEnemiesInRadius(TargetPosition, ScriptFormula(1)), ScriptFormula(3), DamageType.Physical);

			if (Rune_B > 0)
			{
				foreach (Actor enemy in GetEnemiesInRadius(TargetPosition, ScriptFormula(1), (int)ScriptFormula(4)).Actors)
				{
					//if it doesnt have confusion, it gets stunned.
					if (!AddBuff(enemy, new Confusion_Debuff()))
					{
						AddBuff(enemy, new DebuffStunned(WaitSeconds(ScriptFormula(6))));
					}
				}
			}
			if (Rune_C > 0)
			{
				// FIXME: recheck actor sno
				//could not find the correct Projectile actor for this.
				var proj = new Projectile(this, ActorSno.__NONE, User.Position);
				proj.Position.Z += 5f;  // unknown if this is needed
				proj.OnUpdate = () =>
				{
					foreach (Actor enemy in GetEnemiesInRadius(proj.Position, ScriptFormula(10)).Actors)
					{
						AddBuff(enemy, new SpiritDoT());
					}
				};
				proj.Launch(TargetPosition, ScriptFormula(16));
			}
			yield break;
		}
		[ImplementsPowerBuff(0)]
		class Confusion_Debuff : PowerBuff
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
		[ImplementsPowerBuff(1)]
		class SpiritDoT : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(1f);

			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(Target, ScriptFormula(11), DamageType.Physical);
				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region BigBadVoodoo
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.BigBadVoodoo)]
	public class BigBadVoodoo : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(208601))
				StartCooldown(EvalTag(PowerKeys.CooldownTime) * 0.75f);
			else
				StartCooldown(EvalTag(PowerKeys.CooldownTime));

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			var Voodoo = new EffectActor(
				this,
				RuneSelect(
					ActorSno._witchdoctor_bigbadvoodoo_fetish,
					ActorSno._witchdoctor_bigbadvoodoo_fetish_red,
					ActorSno._witchdoctor_bigbadvoodoo_fetish_blue,
					ActorSno._witchdoctor_bigbadvoodoo_fetish_purple,
					ActorSno._witchdoctor_bigbadvoodoo_fetish_yellow,
					ActorSno._witchdoctor_bigbadvoodoo_fetish
				),
				TargetPosition
			);
			Voodoo.Timeout = WaitSeconds(ScriptFormula(0));
			Voodoo.Scale = 1f;
			Voodoo.Spawn();
			Voodoo.PlayEffectGroup(181291);
			Voodoo.UpdateDelay = 0.9f;
			Voodoo.OnUpdate = () =>
			{
				foreach (Actor Ally in Voodoo.GetActorsInRange(EvalTag(PowerKeys.ControllerMinRange)))
				{
					if (!(Ally is Minion || Ally is Player))
					{
						if (Rune_E > 0)
						{
							AddBuff(Ally, new DogBuff());
						}
					}
					else
					{
						if (Rune_D > 0)
						{
							if (Ally is Player)
								(Ally as Player).GeneratePrimaryResource(ScriptFormula(5));
						}
						if (Rune_C > 0)
						{
							Ally.AddHP(Ally.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(4));
						}
						AddBuff(Ally, new FetishShamanBuff());
					}

				}
			};
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class FetishShamanBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(1f);

			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				if (Rune_A > 0)
				{
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(3);
				}
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] += ScriptFormula(1);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				if (Rune_A > 0)
				{
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(3);
				}
				Target.Attributes[GameAttribute.Movement_Bonus_Run_Speed] -= ScriptFormula(1);
				Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(4)]
		class DogBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(1f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is DeathPayload && Target == payload.Target)
					if (Rand.NextDouble() < ScriptFormula(6))
					{
						var dog = new ZombieDog(User.World, User, 0);
						dog.Brain.DeActivate();
						dog.Position = Target.Position;
						dog.Attributes[GameAttribute.Untargetable] = true;
						dog.EnterWorld(dog.Position);
						dog.PlayActionAnimation(11437);
						Task.Delay(1000).ContinueWith(d =>
						{
							dog.Brain.Activate();
							dog.Attributes[GameAttribute.Untargetable] = false;
							dog.Attributes.BroadcastChangedIfRevealed();
							dog.PlayActionAnimation(11431);
						});
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
	#region WallOfZombies
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.WallOfZombies)]
	public class WallOfZombies : Skill
	{
		//TODO:Unknown how to do the width of the Wall of Zombies..
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			if (Rune_C > 0)
			{
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 52f, (int)ScriptFormula(5));

				for (int i = 0; i < projDestinations.Length; i++)
				{
					var proj = new Projectile(this, ActorSno._wd_wallofzombies_charge_projectile, User.Position);
					proj.OnCollision = (hit) =>
					{
						proj.Destroy();
						WeaponDamage(hit, ScriptFormula(4), DamageType.Physical);
						// FIXME: check and find correct actor sno
						SpawnEffect(ActorSno._x1_witchdoctor_wallofzombies_circlewall, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z));
					};
					proj.Launch(projDestinations[i], 0.2f);
				}
			}
			else if (Rune_B > 0)
			{
				//this needs to have double the width, 
				//at the moment this only shows the double length when pointing spell to the right.

				float castAngle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition);
				//Vector3D[] spawnPoints = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, (float)Math.Atan(5f / PowerMath.Distance2D(User.Position, TargetPosition)), 2);
				Vector3D[] spawnPoints = PowerMath.GenerateSpreadPositions(TargetPosition, PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 10f), 180, 2);

				for (int i = 0; i < spawnPoints.Length; ++i)
				{
					var miniwall = SpawnEffect(ActorSno._wd_wallofzombies_emitter_wide, spawnPoints[i], castAngle, WaitSeconds(ScriptFormula(0)));
					miniwall.UpdateDelay = ScriptFormula(8);
					miniwall.OnUpdate = () =>
					{
						TargetList enemies = GetEnemiesInRadius(miniwall.Position, 7f);
						WeaponDamage(enemies, ScriptFormula(14), DamageType.Physical);
					};
				}

			}
			else
			{
				float castAngle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition);
				var Wall = SpawnEffect(
					RuneSelect(
						ActorSno._wd_wallofzombies_emitter,
						ActorSno._wd_wallofzombies_emitter_wide,
						ActorSno._wd_wallofzombies_emitter_wide,
						ActorSno.__NONE,
						ActorSno._wd_wallofzombies_emitter_slow,
						ActorSno._wd_wallofzombies_emitter_tower
					),
					TargetPosition,
					castAngle,
					WaitSeconds(Rune_E > 0 ? 2 : ScriptFormula(0))
				);
				if (Rune_E > 0)
				{
					Wall.OnTimeout = () =>
					{
						TargetList enemies = GetEnemiesInArcDirection(Wall.Position, TargetPosition, 15f, 45f);
						foreach (var enemy in enemies.Actors)
							Knockback(Wall.Position, enemy, ScriptFormula(21), ScriptFormula(22));
						WeaponDamage(enemies, ScriptFormula(14), DamageType.Physical);
					};
				}
				else
				{
					Wall.UpdateDelay = ScriptFormula(8);
					Wall.OnUpdate = () =>
					{
						TargetList enemies = GetEnemiesInRadius(Wall.Position, 7f);
						WeaponDamage(enemies, ScriptFormula(14), DamageType.Physical);
						if (Rune_D > 0)
						{
							//slow movement of enemies
							foreach (var enemy in enemies.Actors)
								AddBuff(enemy, new DebuffSlowed(ScriptFormula(17), WaitSeconds(ScriptFormula(18))));
						}
					};

					if (Rune_A > 0)
					{
						int maxCreepers = (int)(ScriptFormula(0) / ScriptFormula(10));
						List<Actor> Creepers = new List<Actor>();
						for (int i = 0; i < maxCreepers; i++)
						{
							var Creeper = new WallCreeper(World, this, i);
							Creeper.Brain.DeActivate();
							Creeper.Position = RandomDirection(Wall.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
							Creeper.Attributes[GameAttribute.Untargetable] = true;
							Creeper.EnterWorld(Creeper.Position);
							Creepers.Add(Creeper);
							yield return WaitSeconds(0.2f);
						}
						foreach (Actor Creeper in Creepers)
						{
							(Creeper as Minion).Brain.Activate();
							Creeper.Attributes[GameAttribute.Untargetable] = false;
							Creeper.Attributes.BroadcastChangedIfRevealed();
						}
					}
				}
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region FetishArmy
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.FetishArmy)]
	public class FetishArmy : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(ScriptFormula(18));

			int maxFetishes = (int)(ScriptFormula(0) + ScriptFormula(9));
			List<Actor> Fetishes = new List<Actor>();
			for (int i = 0; i < maxFetishes; i++)
			{
				var Fetish = new FetishMelee(World, this, i);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				Fetishes.Add(Fetish);
				yield return WaitSeconds(0.2f);
			}
			if (Rune_C > 0)
			{
				for (int i = 0; i < ScriptFormula(10); i++)
				{
					var Fetish = new FetishShaman(World, this, i);
					Fetish.Brain.DeActivate();
					Fetish.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					Fetish.Attributes[GameAttribute.Untargetable] = true;
					Fetish.EnterWorld(Fetish.Position);
					Fetish.PlayActionAnimation(90118);
					Fetishes.Add(Fetish);
					yield return WaitSeconds(0.2f);
				}
			}
			if (Rune_E > 0)
			{
				for (int i = 0; i < ScriptFormula(13); i++)
				{
					var Fetish = new FetishHunter(World, this, i);
					Fetish.Brain.DeActivate();
					Fetish.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					Fetish.Attributes[GameAttribute.Untargetable] = true;
					Fetish.EnterWorld(Fetish.Position);
					Fetish.PlayActionAnimation(90118);
					Fetishes.Add(Fetish);
					yield return WaitSeconds(0.2f);
				}
			}
			yield return WaitSeconds(0.5f);
			foreach (Actor Fetish in Fetishes)
			{
				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.PlayActionAnimation(87190); //Not sure why this is required, but after the summon is done, it'll just be frozen otherwise.
				if (Rune_A > 0)
				{
					Fetish.PlayEffectGroup(133761);
					WeaponDamage(GetEnemiesInRadius(Fetish.Position, 5f), ScriptFormula(5), DamageType.Physical);
				}
			}

			yield break;
		}
	}
	#endregion

	//Complete
	#region Sacrifice
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.Sacrifice)]
	public class Sacrifice : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(5f);
			int dogs = 0;
			int _enemiesDamaged = 0;
			foreach (Actor dog in GetAlliesInRadius(User.Position, 100f).Actors.Where(a => a.SNO == ActorSno._wd_zombiedog && (User as Player).Followers.ContainsKey(a.GlobalID)))
			{
				if (Rune_B > 0)
					(User as Player).AddHP(ScriptFormula(6));
				if (Rune_D > 0)
					GeneratePrimaryResource(ScriptFormula(9));

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(dog.Position, ScriptFormula(2));
				attack.AddWeaponDamage(ScriptFormula(0), DamageType.Physical);
				attack.OnHit = (HitPayload) =>
				{
					if (Rune_C > 0)
					{
						AddBuff(HitPayload.Target, new DebuffSlowed(ScriptFormula(8), WaitSeconds(ScriptFormula(7))));
					}
				};
				attack.OnDeath = DeathPayload =>
				{
					_enemiesDamaged++;
				};
				attack.Apply();
				dog.PlayEffect(Effect.Gore);
				if (User is Player)
					(User as Player).Followers.Remove(dog.GlobalID);
				dog.Destroy();
				if (Rune_A > 0)
				{
					AddBuff(User, new DamageBuff());
				}
				dogs++;
			}

			User.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.WitchDoctor.Support.Sacrifice] = 0;
			User.Attributes.BroadcastChangedIfRevealed();
			RemoveBuffs(User, 102573);

			if (Rune_E > 0)
			{
				int _dogsSummoned = 0;
				for (int i = 0; i < dogs; i++)
					if (Rand.NextDouble() < ScriptFormula(10))
					{
						var dog = new ZombieDog(User.World, User, 0);
						dog.Brain.DeActivate();
						dog.Position = RandomDirection(User.Position, 3f, 8f);
						dog.Attributes[GameAttribute.Untargetable] = true;
						dog.EnterWorld(dog.Position);
						dog.PlayActionAnimation(11437);
						_dogsSummoned++;

						yield return WaitSeconds(0.5f);
						dog.Brain.Activate();
						dog.Attributes[GameAttribute.Untargetable] = false;
						dog.Attributes.BroadcastChangedIfRevealed();
						dog.PlayActionAnimation(11431);
					}

				if (_dogsSummoned >= 3)
					(User as Player).GrantAchievement(74987243307567);
			}

			if (_enemiesDamaged >= 10)
				(User as Player).GrantAchievement(74987243307569);
			yield break;
		}

		[ImplementsPowerBuff(0, true)]
		class DamageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
				MaxStackCount = 10;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && Target == payload.Context.User)
					(payload as HitPayload).TotalDamage *= 1 + (ScriptFormula(4) * StackCount);
			}
		}
	}
	#endregion

	//Pet Class
	#region Gargantuan
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.Gargantuan)]
	public class Gargantuan : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			var garg = new GargantuanMinion(World, this, 0);
			garg.Brain.DeActivate();
			garg.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
			garg.Attributes[GameAttribute.Untargetable] = true;
			garg.EnterWorld(garg.Position);
			garg.PlayActionAnimation(155988);
			yield return WaitSeconds(0.8f);

			(garg as Minion).Brain.Activate();
			garg.Attributes[GameAttribute.Untargetable] = false;
			garg.Attributes.BroadcastChangedIfRevealed();
			garg.PlayActionAnimation(144967); //Not sure why this is required, but after the summon is done, it'll just be frozen otherwise.

			if (Rune_A > 0)
				AddBuff(garg, new GargantuanPrepareBuff());

			if (Rune_C > 0)
				AddBuff(garg, new GargantuanPoisonBuff());

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class GargantuanPoisonBuff : PowerBuff
		{
			const float _tickRate = 1f;
			TickTimer _tickTimer = null;

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_tickTimer == null || _tickTimer.TimedOut)
				{
					_tickTimer = WaitSeconds(_tickRate);
					WeaponDamage(GetEnemiesInRadius(Target.Position, 8f), ScriptFormula(5), DamageType.Poison);
				}
				return false;
			}
		}

		[ImplementsPowerBuff(4)]
		class GargantuanPrepareBuff : PowerBuff
		{
			const float _tickRate = 1f;
			TickTimer _tickTimer = null;

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_tickTimer == null || _tickTimer.TimedOut)
				{
					_tickTimer = WaitSeconds(_tickRate);

					if (!HasBuff<GargantuanEnrageCDBuff>(Target))
					{
						var targets = GetEnemiesInRadius(Target.Position, 10f);
						if (targets.Actors.Count >= 5 || targets.Actors.Where(a => a is Boss || a is Champion || a is Rare || a is Unique).Count() > 1)
						{
							AddBuff(Target, new GargantuanEnrageCDBuff());
							AddBuff(Target, new GargantuanEnrageBuff());
						}
					}
				}
				return false;
			}
		}

		[ImplementsPowerBuff(1)]
		class GargantuanEnrageCDBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(120f);
			}
		}

		[ImplementsPowerBuff(2)]
		public class GargantuanEnrageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(15f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				var garg = (Target as GargantuanMinion);
				garg.WalkSpeed *= 1.2f;
				garg.CooldownReduction *= 0.65f;
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == Target && payload is HitPayload)
				{
					(payload as HitPayload).TotalDamage *= 3f;
				}
			}

			public override void Remove()
			{
				var garg = (Target as GargantuanMinion);
				garg.WalkSpeed /= 1.2f;
				garg.CooldownReduction /= 0.65f;
				base.Remove();
			}
		}
	}
	#endregion

	//Pet Class
	#region Hex
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.Hex)]
	public class Hex : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			if (Rune_B > 0)
			{
				AddBuff(User, new ChickenBuff());
			}
			else
			{
				var hex = new HexMinion(World, this, 0);
				hex.Brain.DeActivate();
				hex.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				hex.Attributes[GameAttribute.Untargetable] = true;
				hex.EnterWorld(hex.Position);
				hex.PlayActionAnimation(90118);
				yield return WaitSeconds(0.8f);

				(hex as Minion).Brain.Activate();
				hex.Attributes[GameAttribute.Untargetable] = false;
				hex.Attributes.BroadcastChangedIfRevealed();
			}
			yield break;
		}

		[ImplementsPowerBuff(2)]
		class ChickenBuff : PowerBuff
		{
			public override void Init()
			{
				base.Init();
				Timeout = WaitSeconds(ScriptFormula(5));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttribute.CantStartDisplayedPowers] = true;
				User.Attributes.BroadcastChangedIfRevealed();
				User.PlayEffectGroup(107524);

				return true;
			}

			public override void Remove()
			{
				base.Remove();

				User.PlayEffectGroup(188756);

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(9));
				attack.AddWeaponDamage(ScriptFormula(13), DamageType.Physical);
				attack.Apply();

				User.Attributes[GameAttribute.CantStartDisplayedPowers] = false;
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region CorpseSpiders
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.CorpseSpiders)]
	public class CorpseSpiders : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				TargetPosition = User.Position;
			var proj = new Projectile(
				this,
				RuneSelect(
					ActorSno._witchdoctor_corpsespiders_projectile,
					ActorSno._witchdoctor_corpsespiders_projectile_crimsonrune,
					ActorSno._witchdoctor_corpsespiders_projectile_indigorune,
					ActorSno._witchdoctor_corpsespiders_projectile_obsidianrune,
					ActorSno._witchdoctor_corpsespiders_projectile_goldenrune,
					ActorSno._witchdoctor_corpsespiders_projectile_alabasterrune
				),
				User.Position
			);
			proj.Position.Z += 5f;
			proj.LaunchArc(TargetPosition, 5f, -0.07f);
			yield return WaitSeconds(0.4f);
			proj.OnArrival = () =>
			{
				proj.Destroy();
			};
			SpawnEffect(ActorSno._witchdoctor_corpsespiders_jar_breakable, TargetPosition);

			//the rest of this is spiders, which are pets i presume?
			yield return WaitSeconds(0.05f);

			if (Rune_B > 0)
			{
				var spider = new CorpseSpiderQueen(World, this, 0);
				spider.Brain.DeActivate();
				spider.Scale = 3f;
				spider.Position = RandomDirection(TargetPosition, 3f, 8f);
				spider.Attributes[GameAttribute.Untargetable] = true;
				spider.EnterWorld(spider.Position);
				yield return WaitSeconds(0.05f);

				(spider as Minion).Brain.Activate();
				spider.Attributes[GameAttribute.Untargetable] = false;
				spider.Attributes.BroadcastChangedIfRevealed();
				AddBuff(spider, new SpiderQueenBuff());
			}
			else
			{
				for (int i = 0; i < (int)ScriptFormula(0); i++)
				{
					var spider = new CorpseSpider(
						World,
						this,
						RuneSelect(
							ActorSno._witchdoctor_corpsespider,
							ActorSno._witchdoctor_corpsespider_crimsonrune,
							ActorSno._witchdoctor_corpsespider_indigorune,
							ActorSno._witchdoctor_corpsespider_obsidianrune,
							ActorSno._witchdoctor_corpsespider_goldenrune,
							ActorSno._witchdoctor_corpsespider_alabasterrune
						),
						0
					);
					spider.Brain.DeActivate();
					spider.Position = RandomDirection(TargetPosition, 3f, 8f);
					spider.Attributes[GameAttribute.Untargetable] = true;
					spider.EnterWorld(spider.Position);
					yield return WaitSeconds(0.05f);

					(spider as Minion).Brain.Activate();
					spider.Attributes[GameAttribute.Untargetable] = false;
					spider.Attributes.BroadcastChangedIfRevealed();
					AddBuff(spider, new SpiderBuff());
				}
			}

			if ((User as Player).SkillSet.HasPassive(218588) && FastRandom.Instance.Next(100) < 5) //FetishSycophants (wd)
			{
				var Fetish = new FetishMelee(World, this, 0);
				Fetish.Brain.DeActivate();
				Fetish.Position = RandomDirection(User.Position, 3f, 8f);
				Fetish.Attributes[GameAttribute.Untargetable] = true;
				Fetish.EnterWorld(Fetish.Position);
				Fetish.PlayActionAnimation(90118);
				yield return WaitSeconds(0.5f);

				(Fetish as Minion).Brain.Activate();
				Fetish.Attributes[GameAttribute.Untargetable] = false;
				Fetish.Attributes.BroadcastChangedIfRevealed();
				Fetish.LifeTime = WaitSeconds(60f);
				Fetish.PlayActionAnimation(87190);
			}

			yield break;
		}

		[ImplementsPowerBuff(1)]
		class SpiderQueenBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					WeaponDamage(GetEnemiesInRadius(Target.Position, 6f), ScriptFormula(14), DamageType.Poison);
				}
				return false;
			}
		}

		[ImplementsPowerBuff(3)]
		class SpiderBuff : PowerBuff
		{
			public SpiderBuff()
			{
			}

			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == Target)
				{
					if (Rune_E > 0)
					{
						if (Rand.NextDouble() < ScriptFormula(11))
							AddBuff(payload.Target, new DebuffSlowed(ScriptFormula(4), WaitSeconds(5f)));
					}
					if (Rune_D > 0)
					{
						GeneratePrimaryResource(ScriptFormula(12));
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region SummonZombieDogs
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.Support.SummonZombieDogs)]
	public class SummonZombieDogs : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			PlayerHasDogsBuff buff = World.BuffManager.GetFirstBuff<PlayerHasDogsBuff>(User);
			if (buff != null)
			{
				foreach (ZombieDog dog in buff.dogs)
				{
					try
					{
						dog.Kill(this);
					}
					catch { }
				}
				World.BuffManager.RemoveBuffs(User, buff.GetType());
			}
			int maxDogs = (int)ScriptFormula(0);
			List<Actor> dogs = new List<Actor>();
			for (int i = 0; i < maxDogs; i++)
			{
				var dog = new ZombieDog(World, User, i, ScriptFormula(13));
				dog.Brain.DeActivate();
				dog.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				dog.Attributes[GameAttribute.Untargetable] = true;
				dog.EnterWorld(dog.Position);
				dog.PlayActionAnimation(11437);
				dogs.Add(dog);
				yield return WaitSeconds(0.2f);
			}
			yield return WaitSeconds(0.8f);
			foreach (Actor dog in dogs)
			{
				(dog as Minion).Brain.Activate();
				dog.Attributes[GameAttribute.Untargetable] = false;
				dog.Attributes.BroadcastChangedIfRevealed();
				dog.PlayActionAnimation(11431); //Not sure why this is required, but after the summon is done, it'll just be frozen otherwise.
				if (Rune_A > 0)
				{
					AddBuff(dog, new BurningDogBuff());
				}
				if (Rune_C > 0)
				{
					AddBuff(dog, new DogClawBuff());
				}
				if (Rune_D > 0)
				{
					AddBuff(dog, new DogHealthGlobeBuff());
				}
				if (Rune_E > 0)
				{
					AddBuff(dog, new DogLifestealBuff());
				}
			}
			AddBuff(User, new PlayerHasDogsBuff(dogs));
			if (Rune_B > 0)
			{
				AddBuff(User, new DamageAbsorbBuff());
			}

			yield break;
		}
	}

	[ImplementsPowerBuff(0)]
	class PlayerHasDogsBuff : PowerBuff
	{
		public List<Actor> dogs;

		public PlayerHasDogsBuff(List<Actor> dogs)
		{
			this.dogs = dogs;
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			//this.User.Attributes[GameAttribute.Skill_Toggled_State, SkillsSystem.Skills.WitchDoctor.Support.Sacrifice] = true;
			//User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}
		public override void Remove()
		{
			base.Remove();
		}
	}
	[ImplementsPowerBuff(1)]
	class BurningDogBuff : PowerBuff
	{
		const float _damageRate = 1f;
		TickTimer _damageTimer = null;

		public BurningDogBuff()
		{
		}
		public override bool Update()
		{
			if (base.Update())
				return true;
			if (_damageTimer == null || _damageTimer.TimedOut)
			{
				_damageTimer = WaitSeconds(_damageRate);

				WeaponDamage(GetEnemiesInRadius(Target.Position, 6f), 0.5f, DamageType.Fire);
			}
			return false;
		}
	}
	[ImplementsPowerBuff(2)]
	class DamageAbsorbBuff : PowerBuff
	{
		public DamageAbsorbBuff()
		{
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Target == Target)
			{
				Player usr = (Target as Player);
				float dmg = (payload as HitPayload).TotalDamage * ScriptFormula(9) / usr.Followers.Values.Where(a => a == ActorSno._wd_zombiedog).Count();
				(payload as HitPayload).TotalDamage *= 1 - ScriptFormula(9);
				//List<Actor> dogs = GetAlliesInRadius(Target.Position, 100f).Actors.Where(a => a.ActorSNO.Id == 51353).ToList();
				foreach (var dog in GetAlliesInRadius(Target.Position, 100f).Actors.Where(a => a.SNO == ActorSno._wd_zombiedog))
					Damage(dog, dmg, 0, DamageType.Physical);
			}
		}
	}
	[ImplementsPowerBuff(3)]
	class DogHealthGlobeBuff : PowerBuff
	{
		public DogHealthGlobeBuff()
		{
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is DeathPayload && payload.Target == Target)
			{
				if (Rand.NextDouble() < ScriptFormula(7))
					Target.World.SpawnHealthGlobe(Target, (Target as ZombieDog).Master as Player, Target.Position);
			}
		}
	}
	[ImplementsPowerBuff(4)]
	class DogLifestealBuff : PowerBuff
	{
		public DogLifestealBuff()
		{
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Context.User == Target)
			{
				Player master = (Target as ZombieDog).Master as Player;
				float heal = (payload as HitPayload).TotalDamage * ScriptFormula(8) / (master.Followers.Values.Where(a => a == ActorSno._wd_zombiedog).Count() + 1);
				(payload as HitPayload).TotalDamage *= 1 - ScriptFormula(9);
				master.AddHP(heal);
				foreach (var dog in GetAlliesInRadius(Target.Position, 100f).Actors.Where(a => a.SNO == ActorSno._wd_zombiedog))
					dog.AddHP(heal);
			}
		}
	}
	[ImplementsPowerBuff(5)]
	class DogClawBuff : PowerBuff
	{
		public DogClawBuff()
		{
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Context.User == Target)
			{
				AddBuff(payload.Target, new BleedDebuff());
			}
		}
	}
	[ImplementsPowerBuff(6)]
	class BleedDebuff : PowerBuff
	{
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
				attack.AddWeaponDamage(ScriptFormula(5), DamageType.Poison);
				attack.AutomaticHitEffects = false;
				attack.Apply();
			}
			return false;
		}
	}
	#endregion

	//RoS
	#region Piranhas
	[ImplementsPowerSNO(SkillsSystem.Skills.WitchDoctor.PhysicalRealm.Piranhas)]
	public class WitchDoctorPiranhas : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, 10f);
			//TargetPosition.Z += 5f;

			var piranha_pool = SpawnEffect(
				RuneSelect(
					ActorSno._x1_wd_piranha_proxy,
					ActorSno._x1_wd_piranha_gator_proxy,
					ActorSno._x1_wd_piranha_proxy,
					ActorSno._x1_wd_piranha_tornado_proxy,
					ActorSno._x1_wd_piranha_proxy,
					ActorSno._x1_wd_piranha_cold_proxy
				),
				TargetPosition,
				0,
				WaitSeconds(ScriptFormula(25))
			);
			piranha_pool.UpdateDelay = 1f;
			piranha_pool.OnUpdate = () =>
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(piranha_pool.Position, ScriptFormula(1));
				attack.AddWeaponDamage(ScriptFormula(29), Rune_D > 0 ? DamageType.Cold : DamageType.Poison);
				attack.OnHit = (hit) =>
				{
					if (Rune_C > 0)
					{
						if (PowerMath.Distance2D(hit.Target.Position, piranha_pool.Position) > 2f)
							Knockback(piranha_pool.Position, hit.Target, -5f);
						AddBuff(hit.Target, new DebuffStunned(WaitSeconds(1f)));
					}
					AddBuff(hit.Target, new ArmorReduceDebuff(0.15f, WaitSeconds(1f)));
				};
				attack.Apply();
			};
			yield break;
		}
	}

	#endregion
}
