//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class EndCutScene : QuestEvent
	{
		int ActId = 0;

		public EndCutScene()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{ //177544
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}

		}
		
	}
	class EndCutSceneWithAdvance : QuestEvent
	{
		int ActId = 0;

		public EndCutSceneWithAdvance()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{ //177544
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}
			world.Game.QuestManager.Advance();
		}

	}
}