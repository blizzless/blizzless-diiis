using DiIiS_NA.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class Dummy : QuestEvent
	{
		Logger logger = new Logger("Advancing");

		public Dummy()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//do nothing
		}
	}
}
