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
