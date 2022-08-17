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

namespace DiIiS_NA.GameServer.GSSystem.AISystem
{
	public enum BrainState
	{
		/// <summary>
		/// The idle state, which basically means brain never got an update.
		/// </summary>
		Idle,

		/// <summary>
		/// The wandering state.
		/// </summary>
		Wander,

		/// <summary>
		/// Attack nearby enemies.
		/// </summary>
		Combat,

		/// <summary>
		/// Follow.
		/// </summary>
		Follow,

		/// <summary>
		/// Follow and guard.
		/// </summary>
		Guard,

		/// <summary>
		/// I see dead brains.
		/// </summary>
		Dead,
		Off,
		End
	}
}
