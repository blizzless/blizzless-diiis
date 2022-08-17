//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.Core.Schedulers
{
	public class SpawnDensityRegen : ScheduledEvent
	{
		
		public override string DBKey
		{
			get
			{
				return "SpawnDensityLastRegen";
			}
			set { }
		}

		public override bool CompensateDowntime
		{
			get
			{
				return false;
			}
			set { }
		}

		public override Action EventAction
		{
			get
			{
				return new Action(() =>
				{
					Task.Run(() =>
					{
					
					});
				});
			}
			set { }
		}


		public SpawnDensityRegen()
			: base()
		{
			this.Interval = ScheduledEvent.TimeUntilNextMonday;
		}


		public override void RecalculateInterval()
		{
			this.Interval = ScheduledEvent.TimeUntilNextMonday;
		}
	}
}
