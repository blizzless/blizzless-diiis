using System;
using System.Collections.Generic;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations.MonsterSkills
{
	public abstract class SingleProjectileSkill : ActionTimedSkill
	{
		protected Projectile projectile;
		protected float speed;

		protected void SetProjectile(PowerContext context, ActorSno actorSNO, Vector3D position, float speed = 1f, Action<Actor> OnCollision = null)
		{
			if (User is Monster)
				// FIXME: Non-exist world id
				if (User.World.WorldSNO.Id == 1 ||
					User.World.WorldSNO.Id == 1)
						position.Z = (User as Monster).CorrectedPosition.Z;
			projectile = new Projectile(context, actorSNO, position);


			projectile.OnCollision = OnCollision;
			this.speed = speed;
		}

		protected IEnumerable<TickTimer> Launch()
		{

			projectile.Launch(new Vector3D(Target.Position.X, Target.Position.Y, Target.Position.Z + 5f), speed);
			yield break;
		}
	}

	[ImplementsPowerSNO(30334)] // Monster_Ranged_Projectile.pow
	public class MonsterRangedProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			var projectileId = ActorSno._d3arrow;//default
			
			switch (User.SNO)
			{
				case ActorSno._fleshpitflyer_b: projectileId = ActorSno._skeletonmage_poison_projectile; break;
				case ActorSno._demonflyer_c_bomber: projectileId = ActorSno._demonflyer_bomb_projectile; break;//demonFlyer_bomb_projectile
			}
			//*/
			SetProjectile(this, projectileId, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			return Launch();
		}
	}

	[ImplementsPowerSNO(30599)] // Weapon_Ranged_Projectile.pow
	public class WeaponRangedProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._d3arrow, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			return Launch();
		}
	}

	[ImplementsPowerSNO(466879)] // [466879] [Power] p6_Necro_Revive_skeletonMage_Projectile
	public class Necro_Revive_skeletonMage_Projectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._p6_necro_skeletonmage_f_archer_projectile, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 4.00f, DamageType.Physical);
				projectile.Destroy();
			});
			return Launch();
		}
	}

	[ImplementsPowerSNO(30474)] // Shield_Skeleton_Melee_Instant.pow
	public class ShieldSkeletonMeleeInstant : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			WeaponDamage(GetBestMeleeEnemy(), 1.50f, DamageType.Physical);
			yield break;
		}
	}

	[ImplementsPowerSNO(30258)] // graveRobber_Projectile.pow
	public class GraveRobberProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._graverobber_knife, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			return Launch();
		}
	}

	[ImplementsPowerSNO(30503)] // SkeletonSummoner_Projectile.pow
	public class SkeletonSummonerProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonsummoner_projectile, User.Position, 0.9f, (hit) =>
			{
				hit.PlayEffectGroup(19052);
				WeaponDamage(hit, 1.00f, DamageType.Arcane);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(99077)] // Goatman_Shaman_Iceball.pow
	public class ShamanIceBallProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._goatwarrior_shaman_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				hit.PlayEffectGroup(99355);
				WeaponDamage(hit, 1.5f, DamageType.Cold);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(77342)] // Goatman_Shaman_Lightningbolt.pow
	public class ShamanLightningProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonmage_lightning_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5378);
				WeaponDamage(hit, 1f, DamageType.Lightning);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30252)] // Goatman_Moonclan_Ranged_Projectile.pow
	public class GoatmanRangedProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._goatwarrior_piece_spear, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				hit.PlayEffectGroup(99355);
				WeaponDamage(hit, 1.5f, DamageType.Cold);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30252)] // FallenShaman_Projectile.pow
	public class FallenShamanProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._fallenshaman_fireball_projectile, User.Position, 0.5f, (hit) =>
			{
				//hit.PlayEffectGroup(4101);
				WeaponDamage(hit, 1f, DamageType.Fire);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(130798)] // DemonFlyer_Projectile.pow
	public class DemonFlyerProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._demonflyer_fireball_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(160401);
				WeaponDamage(hit, 1f, DamageType.Fire);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(120874)] // Succubus_bloodStar.pow
	public class SuccubusProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._succubus_bloodstar_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				WeaponDamage(hit, 1f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30449)] // SandWasp_Projectile.pow
	public class SandWaspProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._sandwasp_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5215);
				WeaponDamage(hit, 1.2f, DamageType.Poison);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(135412)] // HoodedNightmare_LightningOfUnlife.pow
	public class HoodedNightmareLightningOfUnlife : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._hoodednightmare_lighting_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(158300);
				WeaponDamage(hit, 1f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(159004)] // GoatMutant_Ranged_Projectile.pow
	public class GoatMutantRangedProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._goatmutant_ranged_spear, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 1f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(157947)] // GoatMutantShamanBlast.pow
	public class GoatMutantShamanBlast : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._goatmutant_shaman_blast_projectile, User.Position, 0.5f, (hit) =>
			{
				//hit.PlayEffectGroup(176534);
				WeaponDamage(hit, 1f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30570)] // TriuneSummoner_Projectile.pow
	public class TriuneSummonerProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._triunesummoner_fireball_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				WeaponDamage(hit, 1f, DamageType.Fire);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30500)] // skeletonMage_Lightning_pierce.pow
	public class SkeletonMageLightningpierce : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonmage_lightning_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5378);
				WeaponDamage(hit, 1f, DamageType.Lightning);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30497)] // skeletonMage_Cold_projectile.pow
	public class SkeletonMageColdprojectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonmage_cold_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5369);
				WeaponDamage(hit, 0.5f, DamageType.Cold);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30499)] // skeletonMage_Fire_projectile.pow
	public class SkeletonMageFireprojectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			var proj = ActorSno._skeletonmage_fire_projectile;
			float dmg = 1.1f;
			if (User.SNO == ActorSno._p6_necro_skeletonmage_f_archer)
			{
				proj = ActorSno._p6_necro_skeletonmage_f_archer_projectile;
				dmg = 4f;
			}
			SetProjectile(this, proj, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5373);
				WeaponDamage(hit, dmg, DamageType.Fire);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30502)] // skeletonMage_Poison_pierce.pow
	public class SkeletonMagePoisonpierce : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonmage_poison_projectile, User.Position, EvalTag(PowerKeys.ProjectileSpeed), (hit) =>
			{
				//hit.PlayEffectGroup(5384);
				WeaponDamage(hit, 1.5f, DamageType.Poison);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30509)] // SnakemanCaster_ElectricBurst.pow
	public class SnakemanCasterElectricBurst : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._skeletonmage_lightning_projectile, User.Position, 0.7f, (hit) =>
			{
				//hit.PlayEffectGroup(5384);
				WeaponDamage(hit, 0.5f, DamageType.Lightning);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(129661)] // DemonHunter_Sentry_TurretAttack.pow
	public class TurretAttackProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._dh_sentry_arrow, User.Position, 0.5f, (hit) =>
			{
				//hit.PlayEffectGroup(150040);
				WeaponDamage(hit, ScriptFormula(1), DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30001)] // AI_RunAway.pow
	public class RunAway : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			/*var _destination = PowerContext.RandomDirection(User.Position, 10f, 20f);
			var moveBuff = new MoverBuff(MovementHelpers.GetCorrectPosition(User.Position, _destination, User.World));
			AddBuff(User, moveBuff);
			yield return moveBuff.Timeout;*/

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class MoverBuff : PowerBuff
		{
			private Vector3D _destination;
			private ActorMover _mover;

			public MoverBuff(Vector3D destination)
			{
				_destination = destination;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				int aniTag;
				if (User.AnimationSet == null)
					aniTag = -1;
				else if (User.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk))
					aniTag = User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk);
				else if (User.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run))
					aniTag = User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run);
				else
					aniTag = -1;

				User.TranslateFacing(_destination, true);
				_mover = new ActorMover(User);
				_mover.Move(_destination, User.WalkSpeed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = aniTag,
				});

				Timeout = _mover.ArrivalTime;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				if (base.Update())
					return true;
				return false;
			}
		}
	}

	[ImplementsPowerSNO(105371)] // TreasureGoblin_Escape.pow
	public class TreasureGoblinEscape : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if (User.Attributes[GameAttribute.Hitpoints_Cur] < User.Attributes[GameAttribute.Hitpoints_Max_Total])
			{
				for (int i = 0; i < 4; i++)
				{
					var _destination = RandomDirection(User.Position, 20f, 30f);
					var moveBuff = new MoverBuff(MovementHelpers.GetCorrectPosition(User.Position, _destination, User.World));
					AddBuff(User, moveBuff);
					yield return moveBuff.Timeout;
				}
			}
			else
				yield return WaitSeconds(1f);

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class MoverBuff : PowerBuff
		{
			private Vector3D _destination;
			private ActorMover _mover;

			public MoverBuff(Vector3D destination)
			{
				_destination = destination;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				int aniTag;
				if (User.AnimationSet == null)
					aniTag = -1;
				else if (User.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run))
					aniTag = User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run);
				else
					aniTag = -1;

				User.TranslateFacing(_destination, true);
				_mover = new ActorMover(User);
				_mover.Move(_destination, 0.7f, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = aniTag,
				});

				Timeout = _mover.ArrivalTime;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				if (base.Update())
					return true;
				return false;
			}
		}
	}

	[ImplementsPowerSNO(1729)] // AI_Wander.pow
	public class Wander : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{

			var _destination = RandomDirection(User.Position, 10f, 20f);
			var moveBuff = new MoverBuff(MovementHelpers.GetCorrectPosition(User.Position, _destination, User.World));
			AddBuff(User, moveBuff);
			yield return moveBuff.Timeout;

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class MoverBuff : PowerBuff
		{
			private Vector3D _destination;
			private ActorMover _mover;

			public MoverBuff(Vector3D destination)
			{
				_destination = destination;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				int aniTag;
				if (User.AnimationSet == null)
					aniTag = -1;
				else if (User.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk))
					aniTag = User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk);
				else if (User.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run))
					aniTag = User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run);
				else
					aniTag = -1;

				User.TranslateFacing(_destination, true);
				_mover = new ActorMover(User);
				_mover.Move(_destination, User.WalkSpeed, new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = aniTag,
				});

				Timeout = _mover.ArrivalTime;

				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				_mover.Update();

				if (base.Update())
					return true;
				return false;
			}
		}
	}

	[ImplementsPowerSNO(152540)] // Unique_Monster_Generic_Projectile.pow
	public class UniqueMonsterGenericProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._x1_unique_monster_generic_projectile_physical, User.Position, 0.5f, (hit) =>
			{
				hit.PlayEffectGroup(159158);
				WeaponDamage(hit, 2.00f, DamageType.Arcane);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f; //adjust height
			return Launch();
		}
	}

	[ImplementsPowerSNO(107729)] // QuillDemon_Projectile.pow
	public class QuillDemonProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._quilldemonhorn_projectile, User.Position, 1f, (hit) =>
			{
				// Looking at the tagmaps for 107729, the damage should probably be more accurately calculated, but this will have to do for now.
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 2f + (float)Rand.NextDouble() * 4;
			return Launch();
		}
	}

	[ImplementsPowerSNO(110518)] // ZombieFemale_Projectile.pow
	public class WretchedMotherProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._zombie_female_barfball_projectile, User.Position, 1.00f, (hit) =>
			{
				hit.PlayEffectGroup(142812);
				WeaponDamage(hit, 1.00f, DamageType.Poison);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f;  // fix height
			return Launch();
		}
	}

	[ImplementsPowerSNO(99902)] // Scoundrel_Ranged_Projectile.pow
	public class ScoundrelRangedProjectile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._dh_bonearrow_projectile, User.Position, 1.00f, (hit) =>
			{
				//hit.PlayEffectGroup(142812);
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f;  // fix height
			return Launch();
		}
	}

	[ImplementsPowerSNO(30273)] // HirelingMage_MagicMissile.pow
	public class EnchantressMagicMissile : SingleProjectileSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._g_magicprojectile, User.Position, 1.00f, (hit) =>
			{
				//hit.PlayEffectGroup(142812);
				WeaponDamage(hit, 1.00f, DamageType.Arcane);
				projectile.Destroy();
			});
			projectile.Position.Z += 5f;  // fix height
			return Launch();
		}
	}
}
