using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class Invasion : QuestEvent
	{
		public float Radius;
		public float Duration;
		public Vector3D Center;
		public List<ActorSno> Monsters;
		public ActorSno LastMob;
		public bool LastSolo;

		public Invasion(Vector3D center, float radius, List<ActorSno> mobs, float duration, ActorSno lastMob, bool lastSolo)
			: base(0)
		{
			Radius = radius;
			Center = center;
			Monsters = mobs;
			Duration = duration;
			LastMob = lastMob;
			LastSolo = lastSolo;
		}

		public override void Execute(MapSystem.World world)
		{
			var marker = world.SpawnMonster(ActorSno._generic_proxy_normal, Center);
			world.BuffManager.AddBuff(marker, marker, new PowerSystem.Implementations.InvasionBuff(TickTimer.WaitSeconds(world.Game, Duration), Monsters, Radius, LastMob, LastSolo));
		}
	}
}
