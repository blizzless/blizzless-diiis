using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class BelialStageTwo : QuestEvent
	{

		public BelialStageTwo()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Logger.Debug("BelialStageTwo event started");
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}
			world.GetActorBySNO(ActorSno._belial_trueform).Destroy();
			var Belial = world.SpawnMonster(ActorSno._belial_trueform, world.GetStartingPointById(108).Position);
			foreach (var guard in world.GetActorsBySNO(ActorSno._caldeumguard_spear_imperial))
				guard.Destroy();
		}

	}
}
