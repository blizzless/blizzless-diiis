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
	class StartSideQuest : QuestEvent
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public int QuestId;
		public bool ForceAbandon;

		public StartSideQuest(int questId, bool forceAbandon)
			: base(0)
		{
			this.QuestId = questId;
			this.ForceAbandon = forceAbandon;
		}

		public override void Execute(MapSystem.World world)
		{
			Logger.Trace("StartSideQuest(): {0}", this.QuestId);
			world.Game.QuestManager.LaunchSideQuest(this.QuestId, this.ForceAbandon);
		}
	}
}
