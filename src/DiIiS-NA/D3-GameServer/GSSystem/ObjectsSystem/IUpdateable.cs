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

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
	public interface IUpdateable
	{
		/// <summary>
		/// Tells object to update itself and call it's IUpdateable childs if any.
		/// </summary>
		/// <param name="tickCounter">The Game.TickCounter value when the function gets called.</param>
		void Update(int tickCounter);
	}
}
