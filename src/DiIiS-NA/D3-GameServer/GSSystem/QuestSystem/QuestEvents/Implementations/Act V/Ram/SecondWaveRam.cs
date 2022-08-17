//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class SecondWaveRam : QuestEvent
	{
		public SecondWaveRam()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			var Center = new Vector3D(96.4f, 147.71f, 0f);
			for (int i = 0; i < 5; i++)
			{ world.SpawnMonster(304307, RandomDirection(Center, 5f, 15f)); world.SpawnMonster(340920, RandomDirection(Center, 5f, 15f)); }
			world.SpawnMonster(360243, RandomDirection(Center, 5f, 15f));

			world.GetActorBySNO(295438).PlayActionAnimation(334747);
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
