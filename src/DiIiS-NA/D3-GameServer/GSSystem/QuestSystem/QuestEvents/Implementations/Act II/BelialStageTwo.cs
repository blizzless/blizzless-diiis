//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
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
			world.GetActorBySNO(62975).Destroy();
			var Belial = world.SpawnMonster(62975, world.GetStartingPointById(108).Position);
			foreach (var guard in world.GetActorsBySNO(81857))
				guard.Destroy();
		}

	}
}
