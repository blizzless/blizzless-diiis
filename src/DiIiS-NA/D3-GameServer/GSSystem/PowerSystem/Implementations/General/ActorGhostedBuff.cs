using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerBuff(0)]
	[ImplementsPowerSNO(224639)]
	public class ActorGhostedBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			base.Apply();
			Target.Attributes[GameAttribute.Invulnerable] = true;
			Target.Attributes[GameAttribute.Has_Look_Override] = true;//0x0782CAC5;
			return true;
		}

		public override void Remove()
		{
			Target.Attributes[GameAttribute.Invulnerable] = false;
			Target.Attributes[GameAttribute.Has_Look_Override] = false;
			base.Remove();
		}
	}

	[ImplementsPowerBuff(0)]
	[ImplementsPowerSNO(212032)]
	public class PerfTestBuff : PowerBuff
	{
		TickTimer _damageTimer = null;
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3600f);
		}

		public override bool Apply()
		{
			base.Apply();
			Target.Attributes[GameAttribute.Invulnerable] = true;
			return true;
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_damageTimer == null || _damageTimer.TimedOut)
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, 20f);
				attack.AddWeaponDamage(1000f, DamageType.Physical);
				//we divide by four because this is by second, and tick-intervals = 0.25
				attack.AutomaticHitEffects = false;
				attack.Apply();

				_damageTimer = WaitSeconds(1f);

				for (int i = 0; i < 2; i++)
				{
					var monster = ActorFactory.Create(User.World, ActorSno._zombie_a, new TagMap());
					monster.Scale = 1.35f;  // TODO: look this up properly
					monster.EnterWorld(RandomDirection(User.Position, 5f, 18f));
				}
			}
			return false;
		}

		public override void Remove()
		{
			Target.Attributes[GameAttribute.Invulnerable] = false;
			base.Remove();
		}
	}

	[ImplementsPowerBuff(1)]
	[ImplementsPowerSNO(185997)]
	public class InvasionBuff : PowerBuff
	{
		TickTimer _tickTimer = null;
		public float Radius;
		public List<ActorSno> Monsters;
		public ActorSno LastMob;
		public bool LastSolo;

		public InvasionBuff(TickTimer timeout, List<ActorSno> mobs, float radius, ActorSno lastMob, bool lastSolo)
			: base()
		{
			Timeout = timeout;
			Radius = radius;
			Monsters = mobs;
			LastMob = lastMob;
			LastSolo = lastSolo;
		}

		public override bool Apply()
		{
			base.Apply();
			Target.Attributes[GameAttribute.Invulnerable] = true;
			return true;
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_tickTimer == null || _tickTimer.TimedOut)
			{
				var monster = ActorFactory.Create(Target.World, Monsters[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, Monsters.Count())], new TagMap());
				monster.EnterWorld(RandomDirection(Target.Position, 5f, Radius));
				monster.HasLoot = (Target.World.Game.CurrentAct == 3000);
				monster.Unstuck();
				monster.Teleport(new Vector3D(monster.Position.X, monster.Position.Y, monster.World.GetZForLocation(monster.Position, Target.Position.Z)));
				_tickTimer = WaitSeconds(0.5f);
			}
			return false;
		}

		public override void Remove()
		{
			if (LastMob != ActorSno.__NONE)
			{
				Monster leaderMob = null;
				List<Affix> packAffixes = new List<Affix>();
				for (int n = 0; n < (LastSolo ? 1 : 4); n++)
				{
					if (n == 0)
					{
						if (LastSolo)
							leaderMob = new Unique(Target.World, LastMob, new TagMap());
						else
							leaderMob = new Rare(Target.World, LastMob, new TagMap());
						leaderMob.EnterWorld(Target.Position);
						leaderMob.Unstuck();
						leaderMob.Teleport(new Vector3D(leaderMob.Position.X, leaderMob.Position.Y, leaderMob.World.GetZForLocation(leaderMob.Position, Target.Position.Z)));
						packAffixes = MonsterAffixGenerator.Generate(leaderMob, Math.Min(Target.World.Game.Difficulty + 1, 5));
					}
					else
					{
						var minion = new RareMinion(Target.World, LastMob, new TagMap());
						minion.EnterWorld(RandomDirection(leaderMob.Position, 5f, 10f));
						minion.Unstuck();
						minion.Teleport(new Vector3D(minion.Position.X, minion.Position.Y, minion.World.GetZForLocation(minion.Position, Target.Position.Z)));
						MonsterAffixGenerator.CopyAffixes(minion, packAffixes);
					}
				}
			}
			Target.Attributes[GameAttribute.Invulnerable] = false;
			base.Remove();
			Target.Destroy();
		}
	}
	[ImplementsPowerBuff(2)]
	[ImplementsPowerSNO(185997)]
	public class WavedInvasionBuff : PowerBuff
	{
		TickTimer _tickTimer = null;
		public float Radius;
		public List<ActorSno> Monsters;
		public ActorSno LastMob;

		public WavedInvasionBuff(List<ActorSno> mobs, float radius, ActorSno lastMob)
			: base()
		{
			Radius = radius;
			Monsters = mobs;
			LastMob = lastMob;
		}

		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(15f);
		}

		public override bool Apply()
		{
			base.Apply();
			Target.Attributes[GameAttribute.Invulnerable] = true;
			return true;
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_tickTimer == null || _tickTimer.TimedOut)
			{
				for (int i = 0; i < 10; i++)
				{
					var monster = ActorFactory.Create(Target.World, Monsters[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, Monsters.Count())], new TagMap());
					monster.EnterWorld(RandomDirection(Target.Position, 5f, Radius));
					monster.HasLoot = (Target.World.Game.CurrentAct == 3000);
					monster.Unstuck();
					monster.Teleport(new Vector3D(monster.Position.X, monster.Position.Y, monster.World.GetZForLocation(monster.Position, Target.Position.Z)));
				}
				_tickTimer = WaitSeconds(4f);
			}
			return false;
		}

		public override void Remove()
		{
			if (LastMob != ActorSno.__NONE)
			{
				Monster leaderMob = null;
				List<Affix> packAffixes = new List<Affix>();
				for (int n = 0; n < 4; n++)
				{
					if (n == 0)
					{
						leaderMob = new Rare(Target.World, LastMob, new TagMap());
						leaderMob.EnterWorld(Target.Position);
						leaderMob.Unstuck();
						leaderMob.Teleport(new Vector3D(leaderMob.Position.X, leaderMob.Position.Y, leaderMob.World.GetZForLocation(leaderMob.Position, Target.Position.Z)));
						packAffixes = MonsterAffixGenerator.Generate(leaderMob, Math.Min(Target.World.Game.Difficulty + 1, 5));
					}
					else
					{
						var minion = new RareMinion(Target.World, LastMob, new TagMap());
						minion.EnterWorld(RandomDirection(leaderMob.Position, 5f, 10f));
						minion.Unstuck();
						minion.Teleport(new Vector3D(minion.Position.X, minion.Position.Y, minion.World.GetZForLocation(minion.Position, Target.Position.Z)));
						MonsterAffixGenerator.CopyAffixes(minion, packAffixes);
					}
				}
			}
			Target.Attributes[GameAttribute.Invulnerable] = false;
			base.Remove();
			Target.Destroy();
		}
	}
}
