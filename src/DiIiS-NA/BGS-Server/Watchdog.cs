//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Schedulers;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.LoginServer
{
    public class Watchdog
    {
		private static readonly Logger Logger = LogManager.CreateLogger();

		private uint Seconds = 0;

		private List<ScheduledTask> ScheduledTasks = new List<ScheduledTask>();
		private List<ScheduledTask> TasksToRemove = new List<ScheduledTask>();

		private List<ScheduledEvent> ScheduledEvents = new List<ScheduledEvent>();

		public void Run()
		{
			ScheduledEvents.Add(new SpawnDensityRegen());
			
			while (true)
			{
				System.Threading.Thread.Sleep(1000);
				this.Seconds++;
				try
				{
					lock (this.ScheduledTasks)
					{
						foreach (var task in this.TasksToRemove)
						{
							if (this.ScheduledTasks.Contains(task))
								this.ScheduledTasks.Remove(task);
						}
						this.TasksToRemove.Clear();

						foreach (var task in this.ScheduledTasks.Where(t => this.Seconds % t.Delay == 0))
						{
							try
							{
								task.Task.Invoke();
							}
							catch
							{
								//this.TasksToRemove.Add(task);
							}
						}
					}

					foreach (var s_event in this.ScheduledEvents)
					{
						if (s_event.TimedOut)
							s_event.ExecuteEvent();
					}
				}
				catch { }
			}
		}

		public void AddTask(uint Delay, Action Task)
		{
			this.ScheduledTasks.Add(new ScheduledTask { Delay = Delay, Task = Task });
		}

		public class ScheduledTask
		{
			public Action Task;
			public uint Delay;
		}
	}
}
