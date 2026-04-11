

using DiIiS_NA.GameServer.GSSystem.ActorSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	/// <summary>
	/// Abstract base of the damage / effect pipeline.
	///
	/// <para>A <c>Payload</c> is the in-flight object that carries the
	/// context of a power from the caster to the target. Concrete
	/// subclasses are, in the order they appear in the damage pipeline:</para>
	/// <list type="number">
	///   <item><description><see cref="AttackPayload"/> — one per cast,
	///     holds the damage entries and crit roll. Spawns HitPayloads.</description></item>
	///   <item><description><see cref="HitPayload"/> — one per target
	///     hit; computes mitigated damage and applies it.</description></item>
	///   <item><description><see cref="DeathPayload"/> — spawned if the
	///     hit reduces HP to zero; handles drops, XP, on-kill effects.</description></item>
	/// </list>
	///
	/// <para>All three share the same <see cref="Context"/> (the
	/// <see cref="PowerContext"/> for the originating cast) and a
	/// <see cref="Target"/> reference, which is why they live under a
	/// common base.</para>
	/// </summary>
	public abstract class Payload
	{
		/// <summary>
		/// The originating power's context — caster, rune, power SNO,
		/// user-facing tagmap values, etc.
		/// </summary>
		public PowerContext Context;

		/// <summary>
		/// The specific actor this payload is being applied to. For an
		/// <see cref="AttackPayload"/> this is the primary target; for a
		/// <see cref="HitPayload"/> / <see cref="DeathPayload"/> it is
		/// the actor that just took damage / died.
		/// </summary>
		public Actor Target;

		/// <summary>
		/// Creates a payload for the given context and target. Subclasses
		/// fill in the rest of their state.
		/// </summary>
		public Payload(PowerContext context, Actor target)
		{
			Context = context;
			Target = target;
		}
	}
}
