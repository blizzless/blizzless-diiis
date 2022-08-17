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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ImplementsPowerSNO : Attribute
	{
		public int PowerSNO;

		public ImplementsPowerSNO(int powerSNO)
		{
			PowerSNO = powerSNO;
		}

		public static int GetPowerSNOForClass(Type klass)
		{
			var attributes = (ImplementsPowerSNO[])klass.GetCustomAttributes(typeof(ImplementsPowerSNO), true);
			int powerSNO = -1;
			foreach (var snoAttribute in attributes)
			{
				powerSNO = snoAttribute.PowerSNO;
			}

			return powerSNO;
		}
	}
}
