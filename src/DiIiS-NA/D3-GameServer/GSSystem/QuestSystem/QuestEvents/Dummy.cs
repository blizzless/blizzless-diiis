//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
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
