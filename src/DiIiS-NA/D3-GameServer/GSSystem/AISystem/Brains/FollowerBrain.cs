using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// Placeholder brain for "follower" NPCs — scripted companions that
	/// tag along but don't fight. Currently a no-op; reserved for future
	/// scripted quest companions that need a distinct base class.
	///
	/// <para>Active hireling combat is handled by
	/// <see cref="HirelingBrain"/>, not this class.</para>
	/// </summary>
	public class FollowerBrain : Brain
	{
		/// <summary>
		/// Creates a passive follower brain. Protected so only derived
		/// follower brains can construct it.
		/// </summary>
		/// <param name="body">The actor this brain drives.</param>
		protected FollowerBrain(ActorSystem.Actor body)
			: base(body)
		{ }

		/// <summary>
		/// No-op tick — followers using this base brain do not think.
		/// Override in a subclass to add actual behaviour.
		/// </summary>
		public override void Think(int tickCounter)
		{ }
	}
}
