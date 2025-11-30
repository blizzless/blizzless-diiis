using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public class GameUpdateThread
	{
		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThreadId();

		[DllImport("libc.so.6")]
		private static extern int getpid();

		[DllImport("libc.so.6")]
		private static extern int sched_setaffinity(int pid, IntPtr cpusetsize, ref ulong cpuset);

		private int CurrentTId
		{
			get
			{
				return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetCurrentThreadId() : getpid();
			}
		}

		private static readonly Logger Logger = LogManager.CreateLogger();
		public List<Game> Games = new List<Game>();

		private object _lock = new object();

		public ulong CPUAffinity = 0;

		public void Run()
		{
			List<Game> inactiveGames = new List<Game>();
			int missedTicks = 0;

			Thread.BeginThreadAffinity();
			if (CPUAffinity != 0)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					CurrentThread.ProcessorAffinity = new IntPtr((int)CPUAffinity);
				else
					sched_setaffinity(0, new IntPtr(sizeof(ulong)), ref CPUAffinity);
			}

			while (true)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Restart();

				lock (_lock)
				{
					foreach (var game in Games)
					{
						if (!game.Working)
							inactiveGames.Add(game);
						else
						{
							if (!game.UpdateInProgress)
							{
								game.UpdateInProgress = true;
								Task.Run(() =>
								{
                                    try
                                    {
                                        game.Update();
                                    }
                                    catch (Exception ex)
                                    {
										Logger.ErrorException(ex, "Error in Game.Update()");
                                    }

									game.MissedTicks = 0;
									game.UpdateInProgress = false;
								});
							}
							else
							{
								game.MissedTicks += 6;
                                if (game.MissedTicks > 60)
                                {
                                    Logger.Warn("Game.Update() is running too slow. GameId: {0}", game.GameId);
                                    game.MissedTicks = 0;
                                }
							}
						}
					}

                    foreach (var game in inactiveGames)
                    {
                        game.Working = false;
                        Games.Remove(game);
                    }

                    inactiveGames.Clear();
				}

				stopwatch.Stop();

				var compensation = (int)(100 - stopwatch.ElapsedMilliseconds); // the compensation value we need to sleep in order to get consistent 100 ms Game.Update().

				if (stopwatch.ElapsedMilliseconds > 100)
				{
					Logger.Trace("Game.Update() took [{0}ms] more than Game.UpdateFrequency [{1}ms].", stopwatch.ElapsedMilliseconds, 100);
					compensation = (int)(100 - (stopwatch.ElapsedMilliseconds % 100));
					missedTicks = 6 * (int)(stopwatch.ElapsedMilliseconds / 100);
					Thread.Sleep(Math.Max(0, compensation)); // sleep until next Update().
				}
				else
				{
					missedTicks = 0;
					Thread.Sleep(Math.Max(0, compensation)); // sleep until next Update().
				}
			}

			//Thread.EndThreadAffinity();
		}

		public void AddGame(Game game)
		{
			lock (_lock)
			{
				Games.Add(game);
			}
		}

		private ProcessThread CurrentThread
		{
			get
			{
				int id = CurrentTId;
				return
					(from ProcessThread th in Process.GetCurrentProcess().Threads
					 where th.Id == id
					 select th).Single();
			}
		}
	}
}
