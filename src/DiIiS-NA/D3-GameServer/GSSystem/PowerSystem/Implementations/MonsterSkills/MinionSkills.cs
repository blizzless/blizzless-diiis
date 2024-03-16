using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations.MonsterSkills
{
	[ImplementsPowerSNO(118442)] // fetisharmy_shaman.pow
	public class FetishShamanFire : Skill
	{
		public float CooldownTime = 3f;

		public override IEnumerable<TickTimer> Main()
		{
			User.PlayEffectGroup(213553);
			foreach (Actor fireTarget in GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(2), ScriptFormula(3)).Actors)
			{
				WeaponDamage(fireTarget, 1f, DamageType.Fire);
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(119166)] // fetisharmy_hunter.pow
	public class FetishHunterShoot : SingleProjectileSkill
	{
		public float CooldownTime = 1f;

		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._witchdoctor_fetisharmy_hunter, User.Position, 1f, (hit) =>
			{
				WeaponDamage(hit, 1.00f, DamageType.Poison);
				projectile.Destroy();
			});
			return Launch();
		}
	}

	[ImplementsPowerSNO(107103)] // CorpseSpiderLeap
	public class CorpseSpiderLeap : Skill
	{
		public float CooldownTime = 5f;

		public override IEnumerable<TickTimer> Main()
		{
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 25f));

			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

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

			AttackPayload attack = new AttackPayload(this);
			attack.Target = Target;
			//ScriptFormula(1) states "% of willpower Damage", perhaps the damage should be calculated that way instead.
			attack.AddWeaponDamage(1f, DamageType.Physical);
			attack.Apply();

			yield break;
		}
	}

	[ImplementsPowerSNO(169155)] // mystically_pet_runea_kick.pow
	public class MysticAllyFireKick : Skill
	{
		public float CooldownTime = 5f;

		public override IEnumerable<TickTimer> Main()
		{
			foreach (Actor fireTarget in GetEnemiesInArcDirection(User.Position, TargetPosition, 10f, 30f).Actors)
			{
				WeaponDamage(fireTarget, 2f, DamageType.Fire);
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(363878)] //Mystic Ally -> Fire Ally Explode
	public class MysticAllyFireRuneExplode : Skill
	{
		public float CooldownTime = 5f;

		public override IEnumerable<TickTimer> Main()
		{
			User.PlayEffectGroup(190831);
			AttackPayload explosion = new AttackPayload(this);
			explosion.Targets = GetEnemiesInRadius(User.Position, 15f);
			explosion.AddWeaponDamage(3f, DamageType.Fire);
			explosion.Apply();

			yield break;
		}
	}

	[ImplementsPowerSNO(133887)] // Witchdoctor_Gargantuan_Cleave.pow
	public class DHCompanionCleave : Skill
	{
		public float CooldownTime = 2f;

		public override IEnumerable<TickTimer> Main()
		{
			WeaponDamage(GetEnemiesInArcDirection(User.Position, TargetPosition, 10f, 120f), 1.5f, DamageType.Physical);

			yield break;
		}
	}

	[ImplementsPowerSNO(121942)] // Witchdoctor_Gargantuan_Cleave.pow
	public class GargantuanCleave : Skill
	{
		public float CooldownTime = 3f;

		public override IEnumerable<TickTimer> Main()
		{
			foreach (Actor target in GetEnemiesInArcDirection(User.Position, TargetPosition, 10f, 90f).Actors)
			{
				WeaponDamage(target, 1.3f, DamageType.Physical);
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(369807)]        //Crusader Avatar of the Order shot
	public class AvatarArcherShoot : SingleProjectileSkill
	{
		public float CooldownTime = 0f;

		public override IEnumerable<TickTimer> Main()
		{
			SetProjectile(this, ActorSno._d3arrow, User.Position, 1.5f, (hit) =>
			{
				WeaponDamage(hit, 1.00f, DamageType.Physical);
				projectile.Destroy();
			});
			return Launch();
		}
	}
}
