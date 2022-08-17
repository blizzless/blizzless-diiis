namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class EnterToWest : QuestEvent
	{
		public EnterToWest()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.NotifyQuest(1, true);
			world.Game.QuestManager.Advance();
			foreach (var op in world.GetActorsBySNO(316498)) op.Destroy();
		}
	}
}
