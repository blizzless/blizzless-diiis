//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Threading;
//Blizzless Project 2022 
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
			return UpdateWorkers.OrderBy(t => t.Games.Count()).First();
		}
	}
}
