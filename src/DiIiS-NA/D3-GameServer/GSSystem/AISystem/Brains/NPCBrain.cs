using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// Passive (do-nothing) brain used by generic, non-combat NPCs such as
	/// town vendors, quest givers and ambient pedestrians.
	///
	/// <para><see cref="Think"/> is intentionally empty: these actors don't
	/// pathfind, pick targets, or cast powers. The constructor is
	/// <c>protected</c> because this class is meant to be subclassed by any
	/// specialised passive NPC brain, not instantiated directly.</para>
	///
	/// <para>Compare with <see cref="AggressiveNPCBrain"/> for friendly NPCs
	/// that actually fight monsters, or <see cref="HirelingBrain"/> for
	/// hireling companions.</para>
	/// </summary>
	public class NPCBrain : Brain
	{
		/// <summary>
		/// Creates a passive NPC brain. Protected so only derived brains
		/// (specialised passive NPCs) can construct it.
		/// </summary>
		/// <param name="body">The actor this brain drives.</param>
		protected NPCBrain(ActorSystem.Actor body)
			: base(body)
		{ }

		/// <summary>
		/// No-op tick — passive NPCs don't think. Kept as an override so the
		/// base <see cref="Brain.Update"/> loop can call it without branching.
		/// </summary>
		public override void Think(int tickCounter)
		{ }
	}
}
