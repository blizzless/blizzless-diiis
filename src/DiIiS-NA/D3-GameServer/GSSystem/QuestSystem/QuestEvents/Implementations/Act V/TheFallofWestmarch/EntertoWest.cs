using DiIiS_NA.D3_GameServer.Core.Types.SNO;

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
			foreach (var op in world.GetActorsBySNO(ActorSno._x1_westm_door_cloister_opened)) op.Destroy();
		}
	}
}
