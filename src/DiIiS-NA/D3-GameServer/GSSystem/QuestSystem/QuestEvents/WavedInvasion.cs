using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class WavedInvasion : QuestEvent
	{
		public float Radius;
		public Vector3D Center;
		public List<ActorSno> Monsters;
		public ActorSno LastMob;

		public WavedInvasion(Vector3D center, float radius, List<ActorSno> mobs, ActorSno lastMob)
			: base(0)
		{
			Radius = radius;
			Center = center;
			Monsters = mobs;
			LastMob = lastMob;
		}

		public override void Execute(MapSystem.World world)
		{
			var marker = world.SpawnMonster(ActorSno._generic_proxy_normal, Center);
			world.BuffManager.AddBuff(marker, marker, new PowerSystem.Implementations.WavedInvasionBuff(Monsters, Radius, LastMob));
		}
	}
}
