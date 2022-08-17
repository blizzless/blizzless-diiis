namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnBelialDefenders : QuestEvent
	{

		public SpawnBelialDefenders()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Logger.Debug("SpawnBelialDefenders event started");
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
