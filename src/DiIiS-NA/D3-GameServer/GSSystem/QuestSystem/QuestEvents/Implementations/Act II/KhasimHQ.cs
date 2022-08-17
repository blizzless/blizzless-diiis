//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class KhasimHQ : QuestEvent
	{

		public KhasimHQ()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Убираем магду
			world.GetActorBySNO(129345).Destroy();

			//Вызвать змеев
			int count = 0;

			foreach (var ActorToSpawn in world.GetActorsBySNO(81857))
			{
				world.SpawnMonster(5434, ActorToSpawn.Position);
				ActorToSpawn.Destroy();
				count++;
			}
			world.SpawnMonster(5434, world.GetActorBySNO(138428).Position);
			world.GetActorBySNO(138428).Destroy();
			count++;
			world.SpawnMonster(5434, world.GetActorBySNO(60583).Position);
			world.GetActorBySNO(60583).Destroy();
			count++;

			foreach (var actor in world.Actors.Values.Where(a => a is Monster || a is Player || a is Minion || a is Hireling))
			{
				actor.Disable = false;
			}
		}

	}
}