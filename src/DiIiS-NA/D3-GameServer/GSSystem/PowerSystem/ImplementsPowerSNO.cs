using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
