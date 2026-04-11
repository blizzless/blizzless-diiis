using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	/// <summary>
	/// Per-world orchestrator for power execution.
	///
	/// <para>Every world has its own <c>PowerManager</c> that:</para>
	/// <list type="bullet">
	///   <item><description>Runs queued power scripts each tick,
	///     advancing their coroutine-style state machines.</description></item>
	///   <item><description>Tracks currently-channeled skills so a
	///     re-cast of the same channel re-uses the existing instance
	///     instead of spawning a duplicate.</description></item>
	///   <item><description>Delays actor deletion by ~10s after death so
	///     ongoing visual / buff effects don't leak.</description></item>
	///   <item><description>Applies break-CC-on-cast, item-proc checks,
	///     and the Attacks-Per-Second cheat detection.</description></item>
	/// </list>
	///
	/// <para>The main entry point for firing a power is
	/// <see cref="RunPower(Actor, PowerScript, Actor, Vector3D, TargetMessage)"/>
	/// (or its SNO overload). Powers are written as C# iterator methods
	/// that <c>yield return</c> <see cref="TickTimer"/>s between
	/// sub-actions; the manager advances them here in
	/// <see cref="_UpdateExecutingScripts"/>.</para>
	/// </summary>
	public class PowerManager
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>All actively-channeled skills in this world.</summary>
		private List<ChanneledSkill> _channeledSkills = new List<ChanneledSkill>();

		/// <summary>
		/// One entry in the coroutine execution queue. <c>PowerEnumerator</c>
		/// is the iterator returned by <c>PowerScript.Run()</c>; each
		/// yielded <see cref="TickTimer"/> gates the next step.
		/// </summary>
		private class ExecutingScript
		{
			/// <summary>The in-flight power coroutine.</summary>
			public IEnumerator<TickTimer> PowerEnumerator;

			/// <summary>The originating power script (for cancellation).</summary>
			public PowerScript Script;
		}

		/// <summary>All in-flight power scripts being ticked.</summary>
		private List<ExecutingScript> _executingScripts = new List<ExecutingScript>();

		/// <summary>
		/// Actors that have been killed and are pending deletion. Rather
		/// ugly hack needed because deleting actors immediately when they
		/// still have visual buff effects applied causes the effects to
		/// stay around forever on the client.
		/// </summary>
		private Dictionary<Actor, TickTimer> _deletingActors = new Dictionary<Actor, TickTimer>();

		/// <summary>Creates an empty power manager for a world.</summary>
		public PowerManager()
		{
		}

		/// <summary>
		/// Per-tick update. Advances pending actor deletions and every
		/// executing power script's coroutine.
		/// </summary>
		public void Update()
		{
			_UpdateDeletingActors();
			_UpdateExecutingScripts();
		}

		/// <summary>
		/// Fires visual-only on-cast item procs for legendary / set effects
		/// that want to show a flourish when the player casts anything.
		/// Currently just handles two hard-coded item power SNOs.
		/// </summary>
		private void CheckItemProcs(Player user)
		{
			if (user.SkillSet.HasItemPassiveProc(248776))
			{
				user.PlayEffectGroup(249956);
			}
			if (user.Attributes[GameAttributes.Item_Power_Passive, 246116] == 1 && FastRandom.Instance.NextDouble() < 0.2)
			{
				user.PlayEffectGroup(246117);
			}
		}

		/// <summary>Rolling counter used by the APS cheat detector.</summary>
		private int cheatCounter = 0;

		/// <summary>
		/// Starts executing a power script with the given target context.
		/// Handles:
		/// <list type="bullet">
		///   <item><description>Teleport walkability check (powers 168344 / 167648).</description></item>
		///   <item><description>Disabled-actor gate.</description></item>
		///   <item><description>Item-proc fires for players.</description></item>
		///   <item><description>Break-CC attempts (Stun / Fear / Root) if
		///     the power's tagmap opts in.</description></item>
		///   <item><description>Channeled-skill deduplication
		///     (re-uses existing open channels).</description></item>
		///   <item><description>Facing translation for non-channeled casts.</description></item>
		///   <item><description>Per-second cast-count vs. APS cheat
		///     detection for players.</description></item>
		/// </list>
		/// </summary>
		/// <returns><c>true</c> if the power started executing.</returns>
		public bool RunPower(Actor user, PowerScript power, Actor target = null, Vector3D targetPosition = null, TargetMessage targetMessage = null)
		{
			// Teleport walkability check — powers 168344/167648 are the
			// two player teleport implementations.
			if (power.PowerSNO == 168344 || power.PowerSNO == 167648) //teleport
			{
				if (!user.World.CheckLocationForFlag(PowerMath.TranslateDirection2D(user.Position, targetPosition, user.Position, Math.Min(PowerMath.Distance2D(user.Position, targetPosition), 35f)), DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					Logger.Trace("Teleport power {0} rejected: target position {1} is not walkable", power.PowerSNO, targetPosition);
					return false;
				}
			}

			if (user.Attributes[GameAttributes.Disabled] == true)
			{
				Logger.Trace("RunPower rejected: user {0} is Disabled (power {1})", user.SNO, power.PowerSNO);
				return false;
			}

			if (user is Player && targetPosition != null)
				CheckItemProcs(user as Player);

			// Break stun if possible — powers opting in via PowerKeys.BreaksStun
			// roll their break-chance formula and remove Stun (power SNO 101000) on success.
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksStun) != null)
				if (user.Attributes[GameAttributes.Stunned] == true || user.Attributes[GameAttributes.Frozen] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksStun, user.Attributes, PowerContext.Rand, out result) && result > 0)
					{
						user.World.BuffManager.RemoveBuffs(user, 101000);
						user.Attributes[GameAttributes.Frozen] = false;
						user.Attributes.BroadcastChangedIfRevealed();
					}
				}
			// Break fear if possible (power SNO 101002 is the fear buff).
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksFear) != null)
				if (user.Attributes[GameAttributes.Feared] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksFear, user.Attributes, PowerContext.Rand, out result) && result > 0)
						user.World.BuffManager.RemoveBuffs(user, 101002);
				}
			// Break root if possible (power SNO 101003 is the root buff).
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksRoot) != null)
				if (user.Attributes[GameAttributes.IsRooted] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksRoot, user.Attributes, PowerContext.Rand, out result) && result > 0)
						user.World.BuffManager.RemoveBuffs(user, 101003);
				}
			// Replace the power with the existing channel instance if one
			// already exists — this is what makes held channels continue
			// instead of restarting from scratch each input tick.
			if (power is ChanneledSkill)
			{
				var existingChannel = _FindChannelingSkill(user, power.PowerSNO);
				if (existingChannel != null)
				{
					power = existingChannel;
				}
				else  // New channeled skill — add it to the tracking list.
				{
					_channeledSkills.Add((ChanneledSkill)power);
				}
			}
			else
			{
				// Instant cast: face the target immediately.
				user.TranslateFacing(targetPosition, true);
			}

			// Copy in context params for the power's coroutine to read.
			power.User = user;
			power.Target = target;
			power.World = user.World;
			power.TargetPosition = targetPosition;
			power.TargetMessage = targetMessage;

			user.LastSecondCasts++;

			// APS cheat detector: if a player casts more than APS+1 times
			// in the current second window, subtract a hair of APS. Not a
			// ban, just a tiny slow-down to make cheaters' speedhacks
			// gradually regress.
			if (user is Player && !(power is ChanneledSkill) && power.PowerSNO != 109344 && user.LastSecondCasts > user.Attributes[GameAttributes.Attacks_Per_Second_Total] + 1f)
			{
				//fix for ApS cheating
				user.Attributes[GameAttributes.Attacks_Per_Second] -= 0.00000001f;
				user.Attributes.BroadcastChangedIfRevealed();
				cheatCounter++;
				if (cheatCounter > 5)
				{
					Logger.Warn("Possible attack-speed cheat: player {0}, skill {1}, casts/sec {2:F2} (APS limit {3:F2})",
						(power.User as Player)?.Toon?.Name ?? "<unknown>",
						power.PowerSNO,
						user.LastSecondCasts,
						user.Attributes[GameAttributes.Attacks_Per_Second_Total]);
					cheatCounter = 0;
				}

			}

			_StartScript(power);
			return true;
		}

		/// <summary>
		/// SNO-based overload. Resolves a target actor from
		/// <paramref name="targetId"/> (falling back to the player's
		/// revealed-objects map if necessary) and runs the matching
		/// power implementation.
		/// </summary>
		public bool RunPower(Actor user, int powerSNO, uint targetId = uint.MaxValue, Vector3D targetPosition = null, TargetMessage targetMessage = null)
		{
			Actor target;
			if (powerSNO == -1) return false;

			if (targetId == uint.MaxValue)
			{
				target = null;
			}
			else
			{
				target = user.World.GetActorByGlobalId(targetId);
				// Players may target objects via their client-side
				// revealed-objects map — re-resolve through that if the
				// direct lookup fails.
				if (user is Player)
					foreach (var obj in (user as Player).RevealedObjects)
						if (obj.Value == targetId)
							target = user.World.GetActorByGlobalId(obj.Key);



				if (target == null)
					return false;

				targetPosition = target.Position;
			}

			// Find and run a power implementation matching the SNO.
			var implementation = PowerLoader.CreateImplementationForPowerSNO(powerSNO);
			if (implementation != null)
			{
				return RunPower(user, implementation, target, targetPosition, targetMessage);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Advances every running power coroutine. Scripts whose current
		/// <see cref="TickTimer"/> has expired call <c>MoveNext()</c>; if
		/// the coroutine finishes or yields <c>StopExecution</c>, it's
		/// removed from the execution list. All exceptions are swallowed
		/// so a single broken power can't crash the world tick.
		/// </summary>
		private void _UpdateExecutingScripts()
		{
			// Process all powers, removing from the list the ones that expire.
			try
			{
				_executingScripts.RemoveAll(script =>
				{
					try
					{
						if (script.PowerEnumerator.Current.TimedOut)
						{
							if (script.PowerEnumerator.MoveNext())
								return script.PowerEnumerator.Current == PowerScript.StopExecution;
							else
								return true;
						}
						else
						{
							return false;
						}
					}
					catch (Exception ex)
					{
						Logger.WarnException(ex, "Power script threw — removing from queue (power {0}, user {1})",
							script.Script?.PowerSNO ?? -1,
							script.Script?.User?.SNO.ToString() ?? "<null>");
						return true;
					}

				});
			}
			catch (Exception ex)
			{
				Logger.ErrorException(ex, "PowerManager._UpdateExecutingScripts: unexpected exception, tick swallowed");
			}
		}

		/// <summary>
		/// Cancels an in-progress channeled skill. Called when the player
		/// releases the skill button.
		/// </summary>
		public void CancelChanneledSkill(Actor user, int powerSNO)
		{
			var channeledSkill = _FindChannelingSkill(user, powerSNO);
			if (channeledSkill != null)
			{
				channeledSkill.CloseChannel();
				_channeledSkills.Remove(channeledSkill);
			}
			else
			{
				Logger.Debug("cancel channel for power {0}, but it doesn't have an open channel to cancel", powerSNO);
			}
		}

		/// <summary>
		/// Finds an open channel for <paramref name="user"/> on the given
		/// power SNO, or <c>null</c> if none.
		/// </summary>
		private ChanneledSkill _FindChannelingSkill(Actor user, int powerSNO)
		{
			return _channeledSkills.FirstOrDefault(impl => impl.User == user &&
														   impl.PowerSNO == powerSNO &&
														   impl.IsChannelOpen);
		}

		/// <summary>
		/// Kicks off a script's coroutine. If it yields
		/// <see cref="PowerScript.StopExecution"/> on the first step, the
		/// script is considered instant and is not added to the execution
		/// list.
		/// </summary>
		private void _StartScript(PowerScript script)
		{
			var powerEnum = script.Run().GetEnumerator();
			if (powerEnum.MoveNext() && powerEnum.Current != PowerScript.StopExecution)
			{
				_executingScripts.Add(new ExecutingScript
				{
					PowerEnumerator = powerEnum,
					Script = script
				});
			}
		}

		/// <summary>
		/// Processes the deferred-deletion queue. When the 10-second grace
		/// timer on a dying actor expires, the actor is finally destroyed.
		/// </summary>
		private void _UpdateDeletingActors()
		{
			foreach (var key in _deletingActors.Keys.ToArray())
			{
				if (_deletingActors[key].TimedOut)
				{
					key.Destroy();
					_deletingActors.Remove(key);
				}
			}
		}

		/// <summary>
		/// Marks an actor for deferred deletion ~10s from now. Used by the
		/// death pipeline to keep corpses around long enough for death
		/// visuals / buffs to run out.
		/// </summary>
		public void AddDeletingActor(Actor actor)
		{
			try
			{
				_deletingActors.Add(actor, new SecondsTickTimer(actor.World.Game, 10f));
			}
			catch (ArgumentException)
			{
				// Actor already queued for deletion — harmless race.
				Logger.Trace("AddDeletingActor: {0} already queued for deletion", actor?.SNO.ToString() ?? "<null>");
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the actor is queued for deferred
		/// deletion. Used by <see cref="Payloads.AttackPayload.Apply"/> to
		/// skip targets that are already "dead" from the engine's point of
		/// view.
		/// </summary>
		public bool IsDeletingActor(Actor actor)
		{
			return _deletingActors.ContainsKey(actor);
		}

		/// <summary>
		/// Forcibly cancels every channel and running script belonging to
		/// <paramref name="user"/>. Used on death, disconnect, teleport
		/// between areas, etc.
		/// </summary>
		public void CancelAllPowers(Actor user)
		{
			try
			{
				int channelsCancelled = _channeledSkills.RemoveAll(impl =>
				{
					if (impl.User == user && impl.IsChannelOpen)
					{
						impl.CloseChannel();
						return true;
					}
					return false;
				});

				int scriptsCancelled = _executingScripts.RemoveAll((script) => script.Script.User == user);

				if (channelsCancelled > 0 || scriptsCancelled > 0)
					Logger.Debug("CancelAllPowers for {0}: {1} channels, {2} scripts",
						user?.SNO.ToString() ?? "<null>", channelsCancelled, scriptsCancelled);
			}
			catch (Exception ex)
			{
				Logger.WarnException(ex, "CancelAllPowers threw for user {0}", user?.SNO.ToString() ?? "<null>");
			}
		}
	}
}
