using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnSnakemanDefenders : QuestEvent
	{

		public SpawnSnakemanDefenders()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			//Logger.Debug("SpawnSnakemanDefenders event started");
			//StartConversation(world, 17923);
			var guard = world.GetActorBySNO(ActorSno._caldeumguard_spear_b_nowander);
			while (guard != null)
			{
				world.SpawnMonster(ActorSno._khamsin_snakeman_melee, guard.Position);
				guard.Destroy();
				guard = world.GetActorBySNO(ActorSno._caldeumguard_spear_b_nowander);
			}
		}

	}
}
