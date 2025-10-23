using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	//I Act
    #region Skeleton King
    [ImplementsPowerSNO(30496)] // SkeletonKing_Summon_Skeleton.pow
	public class SkeletonKingSummonSkeleton : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (User.GetActorsInRange(80f).Count < 100)
				for (int i = 0; i < 3; i++)
				{
					var monster = ActorFactory.Create(User.World, (ActorSno)(User as Monster).SnoSummons[0], new TagMap());
					monster.Scale = 1.35f;
					monster.EnterWorld(RandomDirection(Target.Position, 3, 10));
					World.BuffManager.AddBuff(User, monster, new SummonedBuff());
				}
			yield break;
		}
	}

	[ImplementsPowerSNO(30504)] // SkeletonKing_Cleave.pow
	public class SkeletonKingCleave : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			WeaponDamage(GetBestMeleeEnemy(11f), 1.0f, DamageType.Physical);
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, 10f, 120f);
			attack.AddWeaponDamage(1.0f, DamageType.Physical);
			attack.Apply();
			yield break;
		}
	}

	[ImplementsPowerSNO(73824)] // SkeletonKing_Whirlwind.pow
	public class SkeletonKingWhirlwind : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			
			AddBuff(User, new WhirlwindEffect());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class WhirlwindEffect : PowerBuff
		{
			private TickTimer _damageTimer;
			private TickTimer _AnimTimer;

			public override void Init()
			{
				Timeout = WaitSeconds(7f);
				User.PlayAnimation(5, AnimationSno.skeletonking_whirlwind_start);
			}

			//This needs to be added into whirlwind, because your walking speed does become slower once whirlwind is active.
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.Attributes[GameAttributes.Running_Rate] = User.Attributes[GameAttributes.Running_Rate] * EvalTag(PowerKeys.WalkingSpeedMultiplier);
				User.Attributes.BroadcastChangedIfRevealed();
				/*
				[009863] [Anim] SkeletonKing_Whirlwind_end
				[009864] [Anim] SkeletonKing_Whirlwind_loop
				[009865] [Anim] SkeletonKing_Whirlwind_start
				[009902] [Anim] Skeleton_assemble_skeletonKing
				[081880] [Anim] SkeletonKing_Whirlwind_loop_FX
				 */
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.PlayActionAnimation(AnimationSno.skeletonking_whirlwind_end);
				User.Attributes[GameAttributes.Running_Rate] = User.Attributes[GameAttributes.Running_Rate] / EvalTag(PowerKeys.WalkingSpeedMultiplier);
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_AnimTimer == null || _AnimTimer.TimedOut)
				{
					_AnimTimer = WaitSeconds(4f);
					User.PlayActionAnimation(AnimationSno.skeletonking_whirlwind_loop_fx);
				}

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(0.25f);
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, 10f);
					attack.AddWeaponDamage(1.75f, DamageType.Physical);
					attack.OnHit = (hitPayload) =>
					{

					};
					attack.Apply();
				}

				return false;
			}
		}
	}

	[ImplementsPowerSNO(79334)] // SkeletonKing_Teleport.pow 
	public class SkeletonKingTeleport : MonsterAffixSkill
	{
		public new float CooldownTime = 8f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				User.PlayEffectGroup(170232);
				User.Teleport(RandomDirection(Target.Position, 8f, 15f));
				User.Unstuck();
				User.PlayEffectGroup(170232);
			}
			yield break;
		}
	}
    #endregion
    #region Butcher
    [ImplementsPowerSNO(83008)] // Butcher_GrapplingHook.pow
	public class ButcherHook : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			var projectile = new Projectile(this, ActorSno._butcher_hook, User.Position);
			projectile.Scale = 3f;
			projectile.Timeout = WaitSeconds(0.5f);
			projectile.OnCollision = (hit) =>
			{
				_setupReturnProjectile(hit.Position);

				AttackPayload attack = new AttackPayload(this);
				attack.SetSingleTarget(hit);
				attack.AddWeaponDamage(0.5f, DamageType.Physical);
				attack.OnHit = (hitPayload) =>
				{
					// GET OVER HERE
					//unknown on magnitude/knockback offset?
					//hitPayload.Target.PlayEffectGroup(79420);
					Knockback(hitPayload.Target, -25f, ScriptFormula(3), ScriptFormula(4));
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
			yield break;
		}

		private void _setupReturnProjectile(Vector3D spawnPosition)
		{
			Vector3D inFrontOfUser = PowerMath.TranslateDirection2D(User.Position, spawnPosition, User.Position, 5f);

			var return_proj = new Projectile(this, ActorSno._butcher_hook, new Vector3D(spawnPosition.X, spawnPosition.Y, User.Position.Z));
			return_proj.Scale = 3f;
			return_proj.DestroyOnArrival = true;
			return_proj.LaunchArc(inFrontOfUser, 1f, -0.03f);
			User.AddRopeEffect(79402, return_proj);
		}
	}
    #endregion
	//II Act
    #region Maghda
    [ImplementsPowerSNO(131749)]
	public class MaghdaTeleport : Skill
	{
		public float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				User.PlayEffectGroup(170232);
				User.Teleport(RandomDirection(Target.Position, 10f, 30f));
				User.Unstuck();
				User.PlayEffectGroup(170232);
			}
			yield break;
		}
	}
	[ImplementsPowerSNO(131744)]
	public class MaghdaSummonBerserker : SummoningSkill
	{
		public float CooldownTime = 5f;
		public override IEnumerable<TickTimer> Main()
		{
			if (User.GetActorsInRange(80f).Count < 20)
				for (int i = 0; i < 2; i++)
				{
					var monster = ActorFactory.Create(User.World, ActorSno._triune_berserker_maghdapet, new TagMap());
					monster.Scale = 1.35f;
					monster.EnterWorld(RandomDirection(User.Position, 3, 10));
					monster.Unstuck();
					World.BuffManager.AddBuff(User, monster, new SummonedBuff());
				}
			yield break;
		}
	}
	#endregion
	#region Belial
	/*
	[095811] Belial_Phase3Buff
	[095856] Belial_LightningBreath
	[096212] Belial_LightningStrike_v2
	[096712] Belial_Melee
	[259123] BelialArmProxy
	[241757] Belial_LightningStrike_Enrage
	[098565] Belial_Sprint
	[063079] Belial_Ranged_Attack
	[153000] A2_Evacuation_BelialBomb
	[156429] Belial_Melee_Reach
	[067753] Belial_Ground_Pound
	[105312] Belial_Sprint_Away
	//*/
	[ImplementsPowerSNO(96212)]
	public class Belial_LightningStrike_v2 : Skill
	{
		public float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{

			yield break;
		}
	}
	[ImplementsPowerSNO(95856)]
	public class Belial_LightningBreath : Skill
	{
		public float CooldownTime = 20f;

		public override IEnumerable<TickTimer> Main()
		{

			yield break;
		}
	}
	[ImplementsPowerSNO(63079)]
	public class Belial_Ranged_Attack : Skill
	{
		public float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			
			yield break;
		}
	}
	#endregion
	//III Act
	#region Gluttony
	[ImplementsPowerSNO(93676)]
	public class GluttonyGasCloud : SummoningSkill
	{
		public float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			var GroundSpot = SpawnProxy(User.Position);
			var cloud = SpawnEffect(ActorSno._gluttony_gascloud_proxy, User.Position, 0, WaitSeconds(30f));
			cloud.UpdateDelay = 1f;
			cloud.OnUpdate = () =>
			{
				WeaponDamage(GetEnemiesInRadius(GroundSpot.Position, 10f), 0.5f, DamageType.Poison);
			};
			yield break;
		}
	}
	[ImplementsPowerSNO(211292)]
	public class GluttonySlimeSpawn : SummoningSkill
	{
		public float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			if (User.GetActorsInRange(80f).Count < 20)
				for (int i = 0; i < 2; i++)
				{
					var monster = ActorFactory.Create(User.World, ActorSno._gluttony_slime, new TagMap());
					monster.Scale = 1.35f;
					monster.EnterWorld(RandomDirection(Target.Position, 3, 10));
					monster.Unstuck();
					World.BuffManager.AddBuff(User, monster, new SummonedBuff());
				}
			yield break;
		}
	}
    #endregion

	//IV Act
    #region Diablo
    [ImplementsPowerSNO(136223)] // Diablo_RingOfFire.pow
	public class Diablo_RingOfFire : Skill
	{
		public float CooldownTime = 5f;
		public override IEnumerable<TickTimer> Main()
		{
			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Power][136223].Data;
			User.PlayActionAnimation(AnimationSno.diablo_ring_of_fire);
			yield return WaitSeconds(0.5f);
			//User.PlayEffectGroup(196518);
			var Point = SpawnEffect(ActorSno._diablo_ringoffire_damagearea, TargetPosition, 0, WaitSeconds(1.5f));
			Point.PlayEffectGroup(226351);
			yield return WaitSeconds(0.5f);
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, 25f);
			attack.AddWeaponDamage(3.5f, DamageType.Fire);
			attack.OnHit = hit =>
			{
				hit.Target.PlayEffectGroup(184544);
			};

			attack.Apply();
			yield break;
		}
	}
	[ImplementsPowerSNO(136226)] // Diablo_HellSpikes.pow
	public class Diablo_HellSpikes : Skill
	{
		public float CooldownTime = 15f;

		public override IEnumerable<TickTimer> Main()
		{

			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Power][136226].Data;
			User.PlayActionAnimation(AnimationSno.diablo_ring_of_fire);
			//RandomDirection(User.Position, 5, 45)

			if (Target != null)
			{
				AddBuff(Target, new RootDebuff());
				//var Point = SpawnEffect(220210, TargetPosition, 0, WaitSeconds(3f));
				//AddBuff(Target, new RootCDDebuff());
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class RootDebuff : PowerBuff
		{
			EffectActor eff = null;
			public override void Init()
			{
				eff = SpawnEffect(ActorSno._a4dun_diablo_bone_prison_untargetable, Target.Position, 0, WaitSeconds(4.5f));
				Timeout = WaitSeconds(3f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Target.Attributes[GameAttributes.Root_Immune] == false)
				{
					eff.PlayActionAnimation(AnimationSno.a4dun_diablo_bone_prison_closing);
					Target.Attributes[GameAttributes.IsRooted] = true;
					Target.Attributes.BroadcastChangedIfRevealed();
				}

				return true;
			}
			public override void Remove()
			{
				eff.PlayActionAnimation(AnimationSno.a4dun_diablo_bone_prison_opening);
				base.Remove();
				Target.Attributes[GameAttributes.IsRooted] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
    #endregion

	//V Act
}
