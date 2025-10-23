using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class EndSceneinHome : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public EndSceneinHome()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}
			world.Game.QuestManager.Advance();
		}
	}
}
