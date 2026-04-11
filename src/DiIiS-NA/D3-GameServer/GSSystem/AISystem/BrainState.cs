using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.AISystem
{
	/// <summary>
	/// High-level state of a <see cref="Brain"/>.
	///
	/// <para>Only a subset of these values are actively driven by the
	/// concrete brain implementations. The most important ones are:</para>
	/// <list type="bullet">
	///   <item><description><see cref="Idle"/> — default / alive and
	///     thinking.</description></item>
	///   <item><description><see cref="Dead"/> — set by
	///     <see cref="Brain.Kill"/>, the brain's <c>Update()</c> becomes a
	///     no-op.</description></item>
	///   <item><description><see cref="Off"/> — set by
	///     <see cref="Brain.DeActivate"/>, used for off-screen or
	///     quest-gated monsters.</description></item>
	/// </list>
	///
	/// <para>The remaining states (<see cref="Wander"/>, <see cref="Combat"/>,
	/// <see cref="Follow"/>, <see cref="Guard"/>, <see cref="End"/>) are
	/// retained for NPC/scripted uses; <c>MonsterBrain</c> does NOT transition
	/// between them — it encodes state implicitly via <c>CurrentAction</c>.
	/// </para>
	/// </summary>
	public enum BrainState
	{
		/// <summary>
		/// The idle state, which basically means brain never got an update.
		/// </summary>
		Idle,

		/// <summary>
		/// The wandering state. Reserved for future / scripted use.
		/// </summary>
		Wander,

		/// <summary>
		/// Attack nearby enemies. Reserved for future / scripted use —
		/// <c>MonsterBrain</c> enters combat implicitly when a valid target
		/// is found.
		/// </summary>
		Combat,

		/// <summary>
		/// Follow a leader actor. Reserved for future / scripted use.
		/// </summary>
		Follow,

		/// <summary>
		/// Follow and guard. Reserved for future / scripted use.
		/// </summary>
		Guard,

		/// <summary>
		/// The brain is dead and will skip all updates.
		/// Set by <see cref="Brain.Kill"/>.
		/// </summary>
		Dead,

		/// <summary>
		/// The brain is sleeping and will skip all updates until
		/// <see cref="Brain.Activate"/> is called. Used by off-screen,
		/// unspawned or quest-gated monsters.
		/// </summary>
		Off,

		/// <summary>End-of-enum marker. Not used as a real state.</summary>
		End
	}
}
