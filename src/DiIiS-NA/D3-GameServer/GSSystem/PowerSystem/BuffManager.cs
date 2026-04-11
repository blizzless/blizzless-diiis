using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	/// <summary>
	/// Per-world manager for all active buffs, debuffs and CC effects.
	///
	/// <para>Owns the <c>Actor → List&lt;Buff&gt;</c> map and is responsible for:</para>
	/// <list type="bullet">
	///   <item><description>Ticking every buff each game tick
	///     (<see cref="Update"/>) and pruning finished ones.</description></item>
	///   <item><description>Stacking / deduplicating buffs of the same type
	///     (<see cref="_AddBuff"/>).</description></item>
	///   <item><description>Routing attack payloads to interested buffs so
	///     they can mutate or react to incoming hits
	///     (<see cref="SendTargetPayload"/> — used by Thorns, damage
	///     reduction auras, etc.).</description></item>
	///   <item><description>Bulk removal by type / power SNO / predicate
	///     (useful for "break CC on hit", "remove all debuffs" effects).</description></item>
	/// </list>
	///
	/// <para>This class holds raw game state — no logging, no network.
	/// Buffs themselves are responsible for any visual / network work in
	/// their own <c>Apply()</c> / <c>Update()</c> / <c>Remove()</c>.</para>
	/// </summary>
	public class BuffManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>Active buffs keyed by the actor they are attached to.</summary>
		private Dictionary<Actor, List<Buff>> _buffs = new Dictionary<Actor, List<Buff>>();

		/// <summary>
		/// Ticks every active buff once. Buffs that return <c>true</c>
		/// from <c>Update()</c> are marked for removal; actors whose buff
		/// list becomes empty are evicted from the dictionary.
		/// </summary>
		public void Update()
		{
			// Make copy of keys — dictionary is mutated during the loop.
			Actor[] keys = _buffs.Keys.ToArray();

			// Update buffs and mark finished ones as removed (set to null).
			foreach (Actor target in keys)
				_RemoveBuffsIf(target, buff => buff.Update());

			// Clean up removed buffs and evict empty lists.
			foreach (Actor target in keys)
			{
				if (target == null) continue;
				_buffs[target].RemoveAll(buff => buff == null);
				if (_buffs[target].Count == 0)
					_buffs.Remove(target);
			}
		}

		/// <summary>
		/// Attaches a new buff to <paramref name="target"/>, linking the
		/// caster, resolving the buff's power SNO from the
		/// <see cref="ImplementsPowerSNO"/> attribute, then calling
		/// <c>Init()</c> and <see cref="_AddBuff"/>. No-op if the target is
		/// dead or has no world.
		/// </summary>
		/// <returns><c>true</c> if the buff was successfully applied.</returns>
		public bool AddBuff(Actor user, Actor target, Buff buff)
		{
			if (user.World == null || target.World == null) return false;
			if (target.Dead)
			{
				Logger.Trace("AddBuff rejected: target {0} is dead (buff {1})",
					target.SNO, buff?.GetType().Name ?? "<null>");
				return false;
			}

			buff.User = user;
			buff.Target = target;
			buff.World = target.World;

			// Try to load the power SNO from class attribute first,
			// then try the declaring type (for nested buff classes).
			Type buffType = buff.GetType();
			int powerSNO = ImplementsPowerSNO.GetPowerSNOForClass(buffType);
			if (powerSNO != -1)
			{
				buff.PowerSNO = powerSNO;
			}
			else if (buffType.IsNested)
			{
				powerSNO = ImplementsPowerSNO.GetPowerSNOForClass(buffType.DeclaringType);
				if (powerSNO != -1)
					buff.PowerSNO = powerSNO;
			}

			buff.Init();

			return _AddBuff(buff);
		}

		/// <summary>
		/// Removes one stack from a stacking <see cref="PowerBuff"/>. When
		/// the last stack is removed, the buff itself is removed. No-op
		/// for non-stacking buffs.
		/// </summary>
		public void RemoveStackFromBuff(Actor target, PowerBuff buff)
		{
			if (target.World == null) return;
			if (target.Dead) return;
			if (!buff.IsCountingStacks) return;

			if (buff.StackCount <= 1)
			{
				_RemoveBuffsIf(target, Buff => Buff == buff);
				return;
			}
			buff.DeStack(buff);
		}

		/// <summary>
		/// Re-applies an existing buff to a new target, optionally adding
		/// multiple stacks. Used by powers that "copy" or "spread" debuffs
		/// (e.g. Exploding Palm's spread rune).
		/// </summary>
		public void CopyBuff(Actor user, Actor target, Buff buff, int Stacks)
		{
			if (user.World == null || target.World == null) return;

			buff.User = user;
			buff.Target = target;
			buff.World = target.World;
			buff.Removed = false;

			for (int i = 0; i < Stacks; i++)
				_AddBuff(buff);
		}

		/// <summary>Removes all buffs of the given concrete type from <paramref name="target"/>.</summary>
		public void RemoveBuffs(Actor target, Type buffClass)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => buff.GetType() == buffClass);
		}

		/// <summary>Removes all buffs originating from the given power SNO.</summary>
		public void RemoveBuffs(Actor target, int powerSNO)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => buff.PowerSNO == powerSNO);
		}

		/// <summary>Removes one specific buff instance.</summary>
		public void RemoveBuff(Actor target, Buff buff)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, Buff => Buff == buff);
		}

		/// <summary>
		/// Removes every buff from <paramref name="target"/>.
		/// </summary>
		/// <param name="removeCooldowns">
		/// If <c>false</c>, cooldown buffs (power SNO 30176) are preserved.
		/// Defaults to <c>true</c>. Set to <c>false</c> for effects like
		/// "cleanse" that shouldn't reset the player's cooldowns.
		/// </param>
		public void RemoveAllBuffs(Actor target, bool removeCooldowns = true)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => (removeCooldowns ? true : buff.PowerSNO != 30176));
		}

		/// <summary>
		/// Returns the first buff of the given type attached to
		/// <paramref name="target"/>, or <c>null</c> if none.
		/// </summary>
		public T GetFirstBuff<T>(Actor target) where T : Buff
		{
			if (!_buffs.ContainsKey(target)) return null;

			Buff buff = _buffs[target].FirstOrDefault(b => b != null && b.GetType() == typeof(T));
			if (buff != null)
				return (T)buff;
			else
				return null;
		}

		/// <summary>
		/// Returns all buffs of the given type attached to
		/// <paramref name="target"/>. Empty list if none.
		/// </summary>
		public List<T> GetBuffs<T>(Actor target) where T : Buff
		{
			if (!_buffs.ContainsKey(target)) return new List<T>();

			List<Buff> buffs = _buffs[target].Where(b => b != null && b.GetType() == typeof(T)).ToList();
			if (buffs != null)
				return buffs.Cast<T>().ToList();
			else
				return new List<T>();
		}

		/// <summary>
		/// Returns every buff on <paramref name="target"/> as a
		/// <c>Dictionary&lt;Buff, stackCount&gt;</c>. Non-stacking buffs
		/// report a stack count of 1.
		/// </summary>
		public Dictionary<Buff, int> GetAllBuffs(Actor target)
		{
			var buffs = new Dictionary<Buff, int>();
			if (!_buffs.ContainsKey(target)) return buffs;
			foreach (var buff in _buffs[target].Where(b => b != null))
			{
				buffs.Add(buff, (buff is PowerBuff ? (buff as PowerBuff).StackCount : 1));
			}
			return buffs;
		}

		/// <summary>Returns <c>true</c> if the target has at least one buff of type <typeparamref name="T"/>.</summary>
		public bool HasBuff<T>(Actor target) where T : Buff
		{
			return GetFirstBuff<T>(target) != null;
		}

		/// <summary>
		/// Delivers an in-flight <see cref="Payloads.Payload"/> (normally
		/// an <see cref="Payloads.AttackPayload"/>) to every buff on the
		/// given target, giving each a chance to mutate or react to it via
		/// its <c>OnPayload</c> hook. Exceptions from individual buffs are
		/// swallowed so one broken buff can't break the attack pipeline.
		/// </summary>
		public void SendTargetPayload(Actor target, Payloads.Payload payload)
		{
			if (_buffs.ContainsKey(target))
			{
				List<Buff> buffs = _buffs[target];
				int buffCount = buffs.Count;
				for (int i = 0; i < buffCount; ++i)
				{
					if (buffs[i] != null)
						try
						{
							buffs[i].OnPayload(payload);
						}
						catch (Exception ex)
						{
							// Swallow and log — one broken buff can't take down the attack pipeline.
							Logger.WarnException(ex, "Buff {0}.OnPayload threw on target {1}",
								buffs[i].GetType().Name, target?.SNO.ToString() ?? "<null>");
						}
				}
			}
		}

		/// <summary>
		/// Inserts a buff into the dictionary, stacking onto an existing
		/// buff of the same concrete type when possible.
		///
		/// <para>The logic is subtle: we add the buff to the list
		/// <b>before</b> calling <c>Apply()</c> so that if <c>Apply()</c>
		/// recursively adds more buffs of the same type, it sees the new
		/// buff in the list. If <c>Apply()</c> returns <c>false</c>, we
		/// roll back the insertion.</para>
		/// </summary>
		private bool _AddBuff(Buff buff)
		{
			// Look up or create a buff list for the target, then add/stack
			// the buff according to its class type.

			// The logic is a bit more complex than it seems necessary because
			// we ensure the buff appears in the active buff list BEFORE
			// calling Apply(); if Apply() fails we undo adding it. This allows
			// buffs to recursively add/stack more of their own buff type
			// without worrying about overwriting existing buffs.
			if (_buffs.ContainsKey(buff.Target))
			{
				Type buffType = buff.GetType();
				Buff existingBuff = _buffs[buff.Target].FirstOrDefault(b => b != null && b.GetType() == buffType);
				if (existingBuff != null)
				{
					if (existingBuff.Stack(buff))
						return true;
					// Buff is non-stacking, just add normally.
				}

				_buffs[buff.Target].Add(buff);
				if (buff.Apply())
				{
					return true;
				}
				else
				{
					Logger.Trace("Buff {0}.Apply returned false on target {1} — rolled back",
						buff.GetType().Name, buff.Target?.SNO.ToString() ?? "<null>");
					_buffs[buff.Target].Remove(buff);
					return false;
				}
			}
			else
			{
				var keyBuffs = new List<Buff>();
				keyBuffs.Add(buff);
				_buffs[buff.Target] = keyBuffs;
				if (buff.Apply())
				{
					return true;
				}
				else
				{
					Logger.Trace("Buff {0}.Apply returned false on first-time target {1} — rolled back",
						buff.GetType().Name, buff.Target?.SNO.ToString() ?? "<null>");
					_buffs.Remove(buff.Target);
					return false;
				}
			}
		}

		/// <summary>
		/// Walks the buff list for <paramref name="target"/> and, for every
		/// buff matching <paramref name="pred"/>, calls its <c>Remove()</c>
		/// hook and nulls the slot. Actual list compaction is done later
		/// by <see cref="Update"/>.
		/// </summary>
		private void _RemoveBuffsIf(Actor target, Func<Buff, bool> pred)
		{
			if (target == null) return;
			List<Buff> buffs = _buffs[target];
			int buffCount = buffs.Count;
			for (int i = 0; i < buffCount; ++i)
			{
				if (buffs[i] != null)
				{
					if (pred(buffs[i]))
					{
						if (buffs[i] != null)
						{
							buffs[i].Remove();
							buffs[i] = null;
						}
					}
				}
			}
		}
	}
}
