using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
