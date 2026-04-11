using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.AISystem
{
	/// <summary>
	/// Base class for every AI controller in the game. A <see cref="Brain"/>
	/// owns an <see cref="Actor"/> <see cref="Body"/> and is ticked once per
	/// game frame by the world update loop (see <c>Monster.Update</c>,
	/// <c>Minion.Update</c>, etc.).
	///
	/// <para>The update cycle is split into two phases:</para>
	/// <list type="number">
	///   <item><description><see cref="Think(int)"/> — subclasses inspect the
	///     world and assign a new <see cref="CurrentAction"/>.</description></item>
	///   <item><description><see cref="Perform(int)"/> — the current action
	///     is started or ticked forward.</description></item>
	/// </list>
	///
	/// <para>The base class is intentionally minimal. All interesting
	/// decision-making lives in subclasses such as
	/// <c>MonsterBrain</c>, <c>MinionBrain</c> or <c>HirelingBrain</c>.</para>
	///
	/// <para>See <c>docs/Battle.md</c> §3 for the brain system overview.</para>
	/// </summary>
	public class Brain
	{
		/// <summary>Shared logger used by subclasses for trace output.</summary>
		protected static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The actor this brain controls. The brain does not own the actor —
		/// actors own their brain through <c>Actor.Brain</c>.
		/// </summary>
		public Actor Body { get; private set; }

		/// <summary>
		/// The current high-level state of the brain. Note that
		/// <c>MonsterBrain</c> does not actively drive this — most subclasses
		/// only transition between <see cref="BrainState.Idle"/>,
		/// <see cref="BrainState.Off"/> and <see cref="BrainState.Dead"/>.
		/// </summary>
		public BrainState State { get; protected set; }

		/// <summary>
		/// The currently executing action (e.g. move-to-point, cast power).
		/// Only one action runs at a time; new actions replace the current
		/// one in subclasses.
		/// </summary>
		public ActorAction CurrentAction { get; set; }

		/// <summary>
		/// Pending queue of actions. Subclasses may enqueue via
		/// <see cref="QueueAction(ActorAction)"/>; the default
		/// <see cref="Perform(int)"/> does not drain this queue (subclasses
		/// that use it are responsible for dequeuing).
		/// </summary>
		public Queue<ActorAction> Actions { get; protected set; }

		/// <summary>
		/// Creates a new brain bound to the supplied body in the
		/// <see cref="BrainState.Idle"/> state with an empty action queue.
		/// </summary>
		/// <param name="body">Actor this brain will control.</param>
		protected Brain(Actor body)
		{
			Body = body;
			State = BrainState.Idle;
			Actions = new Queue<ActorAction>();
		}

		/// <summary>
		/// Appends an action to <see cref="Actions"/>. Not used by the base
		/// <see cref="Perform(int)"/> — only by subclasses that implement a
		/// queued-action pattern.
		/// </summary>
		protected void QueueAction(ActorAction action)
		{
			Actions.Enqueue(action);
		}

		/// <summary>
		/// Main per-tick entry point called by <c>Monster.Update</c>,
		/// <c>Minion.Update</c>, etc.
		///
		/// <para>This implements the <see cref="Think(int)"/> → <see cref="Perform(int)"/>
		/// cycle and short-circuits for dead/disabled/unparented brains.</para>
		/// </summary>
		/// <param name="tickCounter">Current game tick.</param>
		public virtual void Update(int tickCounter)
		{
			// Skip brains that are dead, disabled, or whose body has been
			// unparented from a world (can happen mid-tick during world
			// changes or destroy).
			if (State == BrainState.Dead || Body == null || Body.World == null || State == BrainState.Off)
				return;

			Think(tickCounter);   // subclass decides what to do next
			Perform(tickCounter); // drive any outstanding action forward
		}

		/// <summary>
		/// Override in subclasses to implement the actual AI decision logic.
		/// The base implementation is a no-op so brains with no behaviour
		/// (e.g. shopkeepers) can derive directly without boilerplate.
		/// </summary>
		/// <param name="tickCounter">Current game tick.</param>
		public virtual void Think(int tickCounter)
		{ }

		/// <summary>
		/// Cancels any ongoing action and transitions the brain to
		/// <see cref="BrainState.Dead"/>. Called by <c>Actor</c> when the body
		/// dies so that pending actions do not continue to fire on a corpse.
		/// </summary>
		public virtual void Kill()
		{
			if (CurrentAction != null)
			{
				CurrentAction.Cancel(0);
				CurrentAction = null;
			}
			Logger.Trace("Brain.Kill: {0} ({1}) → Dead", Body?.SNO.ToString() ?? "<null>", GetType().Name);
			State = BrainState.Dead;
		}

		/// <summary>
		/// Resumes a brain that was previously put to sleep with
		/// <see cref="DeActivate"/>. Used for off-screen or quest-gated
		/// monsters that should not think until a trigger fires.
		/// </summary>
		public void Activate()
		{
			// Only re-activate if the brain is actually off — don't thrash
			// Dead brains back into Idle.
			if (State == BrainState.Off)
			{
				Logger.Trace("Brain.Activate: {0} ({1}) Off → Idle", Body?.SNO.ToString() ?? "<null>", GetType().Name);
				State = BrainState.Idle;
			}
		}

		/// <summary>
		/// Puts the brain to sleep. No <see cref="Think(int)"/> or
		/// <see cref="Perform(int)"/> will run until <see cref="Activate"/>
		/// is called. The current action is discarded.
		/// </summary>
		public void DeActivate()
		{
			CurrentAction = null;
			Logger.Trace("Brain.DeActivate: {0} ({1}) → Off", Body?.SNO.ToString() ?? "<null>", GetType().Name);
			State = BrainState.Off;
		}

		/// <summary>
		/// Runs the current <see cref="ActorAction"/>. On the first tick it
		/// calls <c>Start</c>; on subsequent ticks it calls <c>Update</c>.
		/// When the action reports <c>Done</c>, it is cleared so the next
		/// <see cref="Think(int)"/> can assign a replacement.
		/// </summary>
		/// <param name="tickCounter">Current game tick.</param>
		private void Perform(int tickCounter)
		{
			if (CurrentAction == null)
				return;

			// First call to Start seeds the action with the current tick;
			// subsequent calls use Update (same contract as the action system
			// in ActorSystem/Actions/*).
			if (!CurrentAction.Started)
				CurrentAction.Start(tickCounter);
			else
				CurrentAction.Update(tickCounter);

			// Clear finished actions so Think() can assign a new one next
			// frame. We intentionally do NOT auto-drain the Actions queue
			// here — subclasses that use the queue must dequeue themselves.
			if (CurrentAction.Done)
				CurrentAction = null;
		}
	}
}
