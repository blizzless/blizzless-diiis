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
			List<Game> InactiveGames = new List<Game>();
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
				Stopwatch _tickWatch = new Stopwatch();
				_tickWatch.Restart();

				lock (_lock)
				{
					foreach (var game in Games)
					{
						if (!game.Working)
							InactiveGames.Add(game);
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
									catch { }
									game.MissedTicks = 0;
									game.UpdateInProgress = false;
								});
							}
							else
							{
								game.MissedTicks += 6;
							}
						}
					}

					foreach (var game in InactiveGames)
						Games.Remove(game);

					InactiveGames.Clear();
				}

				_tickWatch.Stop();

				var compensation = (int)(100 - _tickWatch.ElapsedMilliseconds); // the compensation value we need to sleep in order to get consistent 100 ms Game.Update().

				if (_tickWatch.ElapsedMilliseconds > 100)
				{
					Logger.Trace("Game.Update() took [{0}ms] more than Game.UpdateFrequency [{1}ms].", _tickWatch.ElapsedMilliseconds, 100);
					compensation = (int)(100 - _tickWatch.ElapsedMilliseconds % 100);
					missedTicks = 6 * (int)(_tickWatch.ElapsedMilliseconds / 100);
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
