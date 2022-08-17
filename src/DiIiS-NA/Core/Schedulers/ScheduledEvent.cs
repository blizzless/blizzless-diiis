//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.Core.Schedulers
{
	public class ScheduledEvent
	{
		public virtual string DBKey
		{
			get
			{
				return "BaseEventLastCheck";
			}
			set { }
		}

		public TimeSpan Interval = new TimeSpan(0, 0, 1);

		private uint LastCheckTime = 0;

		public virtual bool CompensateDowntime
		{
			get
			{
				return true;
			}
			set { }
		}

		public virtual Action EventAction
		{
			get
			{
				return null;
			}
			set { }
		}

		public ScheduledEvent()
		{
			var param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == this.DBKey);
			if (param.Count < 1)
			{
				var new_param = new DBGlobalParams
				{
					Name = this.DBKey,
					Value = DateTime.Now.ToUnixTime()
				};

				DBSessions.SessionSave(new_param);
				this.LastCheckTime = DateTime.Now.ToUnixTime();
				if (this.CompensateDowntime && this.EventAction != null)
					this.EventAction.Invoke();
				return;
			}
			else
			{
				var db_param = param.First();
				this.LastCheckTime = (uint)db_param.Value;
				if (this.LastCheckTime < (DateTime.Now.ToUnixTime() - this.Interval.TotalSeconds))
				{
					this.LastCheckTime = DateTime.Now.ToUnixTime();
					db_param.Value = (ulong)this.LastCheckTime;
					DBSessions.SessionUpdate(db_param);
					if (this.CompensateDowntime && this.EventAction != null)
						this.EventAction.Invoke();
				}
			}
		}

		public bool TimedOut
		{
			get
			{
				return (this.LastCheckTime < (DateTime.Now.ToUnixTime() - this.Interval.TotalSeconds));
			}
			set { }
		}

		public void ExecuteEvent()
		{
			this.RecalculateInterval();
			this.LastCheckTime = DateTime.Now.ToUnixTime();
			var db_param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == this.DBKey).First();
			db_param.Value = (ulong)this.LastCheckTime;
			DBSessions.SessionUpdate(db_param);
			if (this.EventAction != null)
				this.EventAction.Invoke();
		}

		public virtual void RecalculateInterval()
		{
		}

		public static TimeSpan TimeUntilNextDay
		{
			get
			{
				return (DateTime.Now.AddDays(1).Date - DateTime.Now).Add(new TimeSpan(0, 0, 1));
			}
			set { }
		}

		public static TimeSpan TimeUntilNextDayTime(DayOfWeek day, int hour, int minute)
		{
			DateTime today = DateTime.Today.Add(new TimeSpan(hour, minute, 0));

			while (today.DayOfWeek != day || (today - DateTime.Now).TotalSeconds < 0)
			{
				today = today.AddDays(1);
			}
			return (today - DateTime.Now).Add(new TimeSpan(0, 0, 1));
		}

		public static TimeSpan TimeUntilNextMinute
		{
			get
			{
				return new TimeSpan(0, 0, 61 - DateTime.Now.Second);
			}
			set { }
		}

		public static TimeSpan TimeUntilNextSaturday
		{
			get
			{
				DateTime today = DateTime.Now.AddDays(1);
				while (today.DayOfWeek != DayOfWeek.Saturday)
				{
					today = today.AddDays(1);
				}
				return (today.Date - DateTime.Now).Add(new TimeSpan(0, 0, 1));
			}
			set { }
		}

		public static TimeSpan TimeUntilNextSunday
		{
			get
			{
				DateTime today = DateTime.Now.AddDays(1);
				while (today.DayOfWeek != DayOfWeek.Sunday)
				{
					today = today.AddDays(1);
				}
				return (today.Date - DateTime.Now).Add(new TimeSpan(0, 0, 1));
			}
			set { }
		}

		public static TimeSpan TimeUntilNextMonday
		{
			get
			{
				DateTime today = DateTime.Now.AddDays(1);
				while (today.DayOfWeek != DayOfWeek.Monday)
				{
					today = today.AddDays(1);
					            
				}
				return (today.Date - DateTime.Now).Add(new TimeSpan(0, 0, 1));
			}
			set { }
		}
	}
}
