//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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
	public interface IRevealable
	{
		/// <summary>
		/// Reveals the object to a player.
		/// </summary>
		/// <returns>true if the object was revealed or false if the object was already revealed.</returns>
		bool Reveal(Player player);

		/// <summary>
		/// Unreveals the object from a player.
		/// </summary>
		/// <returns>true if the object was unrevealed or false if the object wasn't already revealed.</returns>
		bool Unreveal(Player player);
	}
}
