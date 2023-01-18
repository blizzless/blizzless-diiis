//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class FirstWaveRam : QuestEvent
	{
		public FirstWaveRam()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			var Center = new Vector3D(96.4f,147.71f,0f);
			for (int i = 0; i < 5; i++)
			{
				world.SpawnMonster(ActorSno._x1_leaperangel_a, RandomDirection(Center, 5f, 15f));
				world.SpawnMonster(ActorSno._x1_westmarchranged_b, RandomDirection(Center, 5f, 15f));
			}
			world.SpawnMonster(ActorSno._x1_leaperangel_a_fortressunique, RandomDirection(Center, 5f, 15f));
			world.GetActorBySNO(ActorSno._x1_pand_batteringram_background).PlayActionAnimation(334746);
		}
		public static Vector3D RandomDirection(Vector3D position, float minRadius, float maxRadius)
		{
			float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
			float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
			return new Vector3D(position.X + (float)Math.Cos(angle) * radius,
								position.Y + (float)Math.Sin(angle) * radius,
								position.Z);
		}
	}
}
