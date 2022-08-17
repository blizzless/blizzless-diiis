//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
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

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class WavedInvasion : QuestEvent
	{
		public float Radius;
		public Vector3D Center;
		public List<int> Monsters;
		public int LastMob;

		public WavedInvasion(Vector3D center, float radius, List<int> mobs, int lastMob)
			: base(0)
		{
			this.Radius = radius;
			this.Center = center;
			this.Monsters = mobs;
			this.LastMob = lastMob;
		}

		public override void Execute(MapSystem.World world)
		{
			var marker = world.SpawnMonster(187359, this.Center);
			world.BuffManager.AddBuff(marker, marker, new PowerSystem.Implementations.WavedInvasionBuff(this.Monsters, this.Radius, this.LastMob));
		}
	}
}
