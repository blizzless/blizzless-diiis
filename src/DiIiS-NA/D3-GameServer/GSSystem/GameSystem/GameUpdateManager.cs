using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public static class GameUpdateManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger("ThreadSystem");

		private static List<GameUpdateThread> UpdateWorkers = new List<GameUpdateThread>();

		static GameUpdateManager()
		{
		}

		public static void InitWorkers()
		{
			int CPUCount = Environment.ProcessorCount;
			for (int coreId = 0; coreId < CPUCount; coreId++)
			{
				var thread = new GameUpdateThread();
				thread.CPUAffinity = (1UL << coreId);
				UpdateWorkers.Add(thread);
				var loopThread = new Thread(thread.Run) { Name = "UpdateWorkerThread", IsBackground = true }; ; // create the game update thread.
				loopThread.Start();
			}
			Logger.Info("Запущено {0} потоков", CPUCount);
		}

		public static GameUpdateThread FindWorker()
		{
			return UpdateWorkers.OrderBy(t => t.Games.Count).First();
		}
	}
}
