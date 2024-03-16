using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
