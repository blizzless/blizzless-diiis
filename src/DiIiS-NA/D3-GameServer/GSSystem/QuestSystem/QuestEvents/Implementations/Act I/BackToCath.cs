namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class BackToCath : QuestEvent
	{
		public BackToCath()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.NotifyQuest(1, true);
			world.Game.QuestManager.Advance();
		}
	}
}
