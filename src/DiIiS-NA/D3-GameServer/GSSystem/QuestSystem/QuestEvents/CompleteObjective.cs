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
	class CompleteObjective : QuestEvent
	{
		private int objectiveId = 0;
		public bool outplayed = false;

		public CompleteObjective(int objId)
			: base(0)
		{
			objectiveId = objId;
		}

		public override void Execute(MapSystem.World world)
		{
			if (!outplayed)
			{
				outplayed = true;
				world.Game.QuestManager.CompleteObjective(objectiveId);
			}
		}
	}
}
