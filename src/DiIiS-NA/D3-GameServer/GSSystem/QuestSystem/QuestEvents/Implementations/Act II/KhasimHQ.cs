using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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
			world.GetActorBySNO(ActorSno._maghda_a_tempprojection).Destroy();

			//Вызвать змеев
			int count = 0;

			foreach (var ActorToSpawn in world.GetActorsBySNO(ActorSno._caldeumguard_spear_imperial, ActorSno._caldeumguard_captain_b_khamsin, ActorSno._khamsin_mine_unique))
			{
				world.SpawnMonster(ActorSno._snakeman_melee_c, ActorToSpawn.Position);
				ActorToSpawn.Destroy();
				count++;
			}

			foreach (var actor in world.Actors.Values.Where(a => a is Monster || a is Player || a is Minion || a is Hireling))
			{
				actor.Disable = false;
			}
		}

	}
}