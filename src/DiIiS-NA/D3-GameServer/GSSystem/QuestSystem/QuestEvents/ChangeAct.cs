using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.GameSystem;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class ChangeAct : QuestEvent
	{
		ActEnum ActId = ActEnum.Act1;

		public ChangeAct(ActEnum actId)
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
            ActEnum.Act4 => ActorSno._event47_bigportal,
            ActEnum.Act5 => ActorSno._hope,
            _ => ActorSno._actchangetempobject,
        };
    }
}
