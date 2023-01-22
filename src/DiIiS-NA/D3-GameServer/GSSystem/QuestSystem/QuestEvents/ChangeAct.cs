using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class ChangeAct : QuestEvent
	{
		int ActId = 0;

		public ChangeAct(int actId)
			: base(0)
		{
			ActId = actId;
		}

		public override void Execute(MapSystem.World world)
        {
            foreach (var plr in world.Players.Values)
                plr.ShowConfirmation(world.GetActorBySNO(GetChangeActor()).DynamicID(plr), (() =>
                {
                    world.Game.QuestManager.Advance();
                    world.Game.ChangeAct(ActId);
                }));
        }

        private ActorSno GetChangeActor() => ActId switch
        {
            300 => ActorSno._event47_bigportal,
            400 => ActorSno._hope,
            _ => ActorSno._actchangetempobject,
        };
    }
}
