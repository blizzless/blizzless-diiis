using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ImplementsPowerBuff : Attribute
	{
		public int BuffSlot;
		public bool CountStacks;

		public ImplementsPowerBuff(int buffSlot, bool countStacks = false)
		{
			BuffSlot = buffSlot;
			CountStacks = countStacks;
		}
	}
}
