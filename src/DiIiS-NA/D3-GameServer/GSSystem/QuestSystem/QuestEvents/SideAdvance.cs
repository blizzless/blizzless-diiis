using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class SideAdvance : QuestEvent
	{
		public SideAdvance()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.SideAdvance();
		}
	}
}
