namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class Advance : QuestEvent
	{
		public Advance()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.Advance();
		}
	}
	class AdvanceWithNotify : QuestEvent
	{
		public AdvanceWithNotify()
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
