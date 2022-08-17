namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class ChangeAct : QuestEvent
	{
		int ActId = 0;

		public ChangeAct(int actId)
			: base(0)
		{
			this.ActId = actId;
		}

		public override void Execute(MapSystem.World world)
		{ 
			foreach (var plr in world.Players.Values)
				plr.ShowConfirmation(world.GetActorBySNO(this.ActId == 300 ? 188441 : (this.ActId == 400 ? 114074 : 144797)).DynamicID(plr), (() => {
					world.Game.QuestManager.Advance();
					world.Game.ChangeAct(this.ActId);
				}));
		}
	}
}
