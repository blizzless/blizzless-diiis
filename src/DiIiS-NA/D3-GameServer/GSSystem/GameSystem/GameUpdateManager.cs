using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	[Obsolete("This class is obsolete and will be removed in the future.")]
	public class GameUpdateManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger<GameUpdateManager>();

		private static readonly List<GameUpdateThread> _updateWorkers = new();

		static GameUpdateManager()
		{
		}

		public static void InitWorkers()
		{
			int CPUCount = Environment.ProcessorCount;
			for (int coreId = 0; coreId < CPUCount; coreId++)
			{
				var thread = new GameUpdateThread();
				//thread.CPUAffinity = (1UL << coreId);
				_updateWorkers.Add(thread);
				var loopThread = new Thread(thread.Run) { Name = "UpdateWorkerThread", IsBackground = true };
				loopThread.Start();
			}
			Logger.Info("Started {0} threads", CPUCount);
		}

		public static GameUpdateThread FindWorker()
		{
			return _updateWorkers.OrderBy(t => t.Games.Count).First();
		}
	}
}
