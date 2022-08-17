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
			var guard = world.GetActorBySNO(57470);
			while (guard != null)
			{
				world.SpawnMonster(60816, guard.Position);
				guard.Destroy();
				guard = world.GetActorBySNO(57470);
			}
		}

	}
}
