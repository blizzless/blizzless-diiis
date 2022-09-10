//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
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
	public abstract class SummoningSkill : ActionTimedSkill
	{
		public Vector3D SpawnPosition { get; set; }

		protected void RandomPostion() // spawn actor at random postion
		{
			this.SpawnPosition = RandomDirection(User.Position, 5, 10);
		}

		protected void UserPostion() // spawn actor at user postion
		{
			this.SpawnPosition = User.Position;
		}

		protected void InFrontPostion() // spawn actor in front of user
		{
			float userFacing = (float)Math.Acos(this.User.RotationW) * 2f;
			this.SpawnPosition = new Vector3D(User.Position.X + 8 * (float)Math.Cos(userFacing),
											 User.Position.Y + 8 * (float)Math.Sin(userFacing),
											 User.Position.Z);
		}

		public void SummonMonster(ActorSno actorSNO)
		{
			if (User.GetActorsInRange(80f).Count > 100) return;
			var monster = ActorFactory.Create(User.World, actorSNO, new TagMap());
			monster.Scale = 1f;  // TODO: look this up properly
			monster.EnterWorld(this.SpawnPosition);
			this.World.BuffManager.AddBuff(User, monster, new Implementations.SummonedBuff());
		}
	}

	[ImplementsPowerSNO(94734)] // Summon_Zombie_Vomit.pow
	public class WretchedMotherVomit : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			InFrontPostion();
			SummonMonster((this.User as Monster).SNOSummons[0]);
			yield break;
		}
	}

	[ImplementsPowerSNO(30550)] // Summon_Zombie_Crawler.pow
	public class SpawnZombieCrawler : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			this.World.BuffManager.AddBuff(User, User, new DeathTriggerBuff());
			if ((User is Monster) && ((User as Monster).Brain is MonsterBrain))
				((User as Monster).Brain as MonsterBrain).PresetPowers.Remove(30550);
			yield break;
		}

		class DeathTriggerBuff : Buff
		{
			public DeathTriggerBuff() { }

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == User && payload is DeathPayload)
				{
					if (User.GetActorsInRange(80f).Count > 100) return;
					var monster = ActorFactory.Create(User.World, (this.User as Monster).SNOSummons[0], new TagMap());
					if (monster != null)
					{
						monster.Scale = 1.35f;
						monster.EnterWorld(User.Position);
						this.World.BuffManager.AddBuff(User, monster, new Implementations.SummonedBuff());
					}
				}
			}
		}
	}

	[ImplementsPowerSNO(30178)] // CorpulentExplode.pow
	public class CorpulentExplode : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			this.World.BuffManager.AddBuff(User, User, new DeathTriggerBuff());
			if ((User is Monster) && ((User as Monster).Brain is MonsterBrain))
				((User as Monster).Brain as MonsterBrain).PresetPowers.Remove(30178);
			yield break;
		}

		class DeathTriggerBuff : Buff
		{
			public DeathTriggerBuff() { }

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == User && payload is DeathPayload)
				{
					if (User.GetActorsInRange(80f).Count > 100) return;
					User.PlayAnimation(11, User.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Explode]);
					for (int i = 0; i < 3; i++)
					{
						var monster = ActorFactory.Create(User.World, (this.User as Monster).SNOSummons[0], new TagMap());
						monster.Scale = 1.35f;
						monster.EnterWorld(RandomDirection(User.Position, 1, 3));
						this.World.BuffManager.AddBuff(User, monster, new Implementations.SummonedBuff());
					}
				}
			}
		}
	}

	[ImplementsPowerSNO(66547)] // Fallen_Lunatic_Suicide.pow
	public class FallenLunaticSuicide : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			this.World.BuffManager.AddBuff(User, User, new SuicideBuff());
			if ((User is Monster) && ((User as Monster).Brain is MonsterBrain))
				((User as Monster).Brain as MonsterBrain).PresetPowers.Remove(66547);
			yield break;
		}

		class SuicideBuff : Buff
		{
			public TickTimer SuicideTimer = null;

			public SuicideBuff() { }

			public override bool Update()
			{
				if (base.Update())
					return true;

				var targets = User.GetPlayersInRange(15f);
				if (targets.Count > 0)
				{
					if (SuicideTimer == null) SuicideTimer = new SecondsTickTimer(User.World.Game, 2f);
				}
				else
				{
					SuicideTimer = null;
					return false;
				}

				if (SuicideTimer != null && SuicideTimer.TimedOut)
				{
					SuicideTimer = null;
					var dmgTargets = GetEnemiesInRadius(User.Position, 6f);
					WeaponDamage(dmgTargets, 5.0f, DamageType.Physical);
					User.PlayAnimation(11, User.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Attack]);
					WeaponDamage(User, 1000.0f, DamageType.Physical);
					//(User as Living).Kill();
					//foreach (var anim in Target.AnimationSet.TagMapAnimDefault)
					//Logger.Debug("animation: {0}", anim.ToString());
				}
				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is DeathPayload)
				{
					SuicideTimer = null;
				}
			}
		}
	}

	[ImplementsPowerSNO(30543)] // Summon Skeleton
	public class SummonSkeleton : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RandomPostion();
			if (User is Monster)
				SummonMonster((this.User as Monster).SNOSummons[0]);
			yield break;
		}
	}

	[ImplementsPowerSNO(30800)] // Summon Spores
	public class SummonSpores : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RandomPostion();
			SummonMonster(ActorSno._spore);  // HACK: we don't have this in mpq
			yield break;
		}
	}
	[ImplementsPowerSNO(117580)] // Summon FleshPitFlyers
	public class SummonFleshPitFlyers : SummoningSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UserPostion();
			SummonMonster((this.User as Monster).SNOSummons[0]);
			yield break;
		}
	}
}
