//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
			this.Target.Attributes[GameAttribute.Invulnerable] = true;
			this.Target.Attributes[GameAttribute.Has_Look_Override] = true;//0x0782CAC5;
			return true;
		}

		public override void Remove()
		{
			this.Target.Attributes[GameAttribute.Invulnerable] = false;
			this.Target.Attributes[GameAttribute.Has_Look_Override] = false;
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
			this.Target.Attributes[GameAttribute.Invulnerable] = true;
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
			this.Target.Attributes[GameAttribute.Invulnerable] = false;
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
			this.Radius = radius;
			this.Monsters = mobs;
			this.LastMob = lastMob;
			this.LastSolo = lastSolo;
		}

		public override bool Apply()
		{
			base.Apply();
			this.Target.Attributes[GameAttribute.Invulnerable] = true;
			return true;
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_tickTimer == null || _tickTimer.TimedOut)
			{
				var monster = ActorFactory.Create(Target.World, this.Monsters[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, this.Monsters.Count())], new TagMap());
				monster.EnterWorld(RandomDirection(Target.Position, 5f, this.Radius));
				monster.HasLoot = (Target.World.Game.CurrentAct == 3000);
				monster.Unstuck();
				monster.Teleport(new Vector3D(monster.Position.X, monster.Position.Y, monster.World.GetZForLocation(monster.Position, this.Target.Position.Z)));
				_tickTimer = WaitSeconds(0.5f);
			}
			return false;
		}

		public override void Remove()
		{
			if (this.LastMob != ActorSno.__NONE)
			{
				Monster leaderMob = null;
				List<Affix> packAffixes = new List<Affix>();
				for (int n = 0; n < (this.LastSolo ? 1 : 4); n++)
				{
					if (n == 0)
					{
						if (this.LastSolo)
							leaderMob = new Unique(Target.World, this.LastMob, new TagMap());
						else
							leaderMob = new Rare(Target.World, this.LastMob, new TagMap());
						leaderMob.EnterWorld(Target.Position);
						leaderMob.Unstuck();
						leaderMob.Teleport(new Vector3D(leaderMob.Position.X, leaderMob.Position.Y, leaderMob.World.GetZForLocation(leaderMob.Position, this.Target.Position.Z)));
						packAffixes = MonsterAffixGenerator.Generate(leaderMob, Math.Min(Target.World.Game.Difficulty + 1, 5));
					}
					else
					{
						var minion = new RareMinion(Target.World, this.LastMob, new TagMap());
						minion.EnterWorld(RandomDirection(leaderMob.Position, 5f, 10f));
						minion.Unstuck();
						minion.Teleport(new Vector3D(minion.Position.X, minion.Position.Y, minion.World.GetZForLocation(minion.Position, this.Target.Position.Z)));
						MonsterAffixGenerator.CopyAffixes(minion, packAffixes);
					}
				}
			}
			this.Target.Attributes[GameAttribute.Invulnerable] = false;
			base.Remove();
			this.Target.Destroy();
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
			this.Radius = radius;
			this.Monsters = mobs;
			this.LastMob = lastMob;
		}

		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(15f);
		}

		public override bool Apply()
		{
			base.Apply();
			this.Target.Attributes[GameAttribute.Invulnerable] = true;
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
					var monster = ActorFactory.Create(Target.World, this.Monsters[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, this.Monsters.Count())], new TagMap());
					monster.EnterWorld(RandomDirection(Target.Position, 5f, this.Radius));
					monster.HasLoot = (Target.World.Game.CurrentAct == 3000);
					monster.Unstuck();
					monster.Teleport(new Vector3D(monster.Position.X, monster.Position.Y, monster.World.GetZForLocation(monster.Position, this.Target.Position.Z)));
				}
				_tickTimer = WaitSeconds(4f);
			}
			return false;
		}

		public override void Remove()
		{
			if (this.LastMob != ActorSno.__NONE)
			{
				Monster leaderMob = null;
				List<Affix> packAffixes = new List<Affix>();
				for (int n = 0; n < 4; n++)
				{
					if (n == 0)
					{
						leaderMob = new Rare(Target.World, this.LastMob, new TagMap());
						leaderMob.EnterWorld(Target.Position);
						leaderMob.Unstuck();
						leaderMob.Teleport(new Vector3D(leaderMob.Position.X, leaderMob.Position.Y, leaderMob.World.GetZForLocation(leaderMob.Position, this.Target.Position.Z)));
						packAffixes = MonsterAffixGenerator.Generate(leaderMob, Math.Min(Target.World.Game.Difficulty + 1, 5));
					}
					else
					{
						var minion = new RareMinion(Target.World, this.LastMob, new TagMap());
						minion.EnterWorld(RandomDirection(leaderMob.Position, 5f, 10f));
						minion.Unstuck();
						minion.Teleport(new Vector3D(minion.Position.X, minion.Position.Y, minion.World.GetZForLocation(minion.Position, this.Target.Position.Z)));
						MonsterAffixGenerator.CopyAffixes(minion, packAffixes);
					}
				}
			}
			this.Target.Attributes[GameAttribute.Invulnerable] = false;
			base.Remove();
			this.Target.Destroy();
		}
	}
}
