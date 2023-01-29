using DiIiS_NA.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
			QuestId = questId;
			ForceAbandon = forceAbandon;
		}

		public override void Execute(MapSystem.World world)
		{
			Logger.MethodTrace($"{QuestId}");
			world.Game.QuestManager.LaunchSideQuest(QuestId, ForceAbandon);
		}
	}
}
