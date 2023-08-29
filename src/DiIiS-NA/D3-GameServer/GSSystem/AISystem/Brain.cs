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
	public class Brain
	{
		protected static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The body chained to brain.
		/// </summary>
		public Actor Body { get; private set; }

		/// <summary>
		/// The current brain state.
		/// </summary>
		public BrainState State { get; protected set; }

		/// <summary>
		/// The current action for the brain.
		/// </summary>
		public ActorAction CurrentAction { get; set; }

		/// <summary>
		/// Actions to be taken.
		/// </summary>
		public Queue<ActorAction> Actions { get; protected set; }

		protected Brain(Actor body)
		{
			Body = body;
			State = BrainState.Idle;
			Actions = new Queue<ActorAction>();
		}

		protected void QueueAction(ActorAction action)
		{
			Actions.Enqueue(action);
		}

		public virtual void Update(int tickCounter)
		{
			if (State == BrainState.Dead || Body?.World == null || State == BrainState.Off)
				return;

			Think(tickCounter); // let the brain think.
			Perform(tickCounter); // perform any outstanding actions.
		}

		/// <summary>
		/// Lets the brain think and decide the next action to take.
		/// </summary>
		public virtual void Think(int tickCounter)
		{ }

		/// <summary>
		/// Stop all brain activities.
		/// </summary>
		public virtual void Kill()
		{
			if (CurrentAction != null)
			{
				CurrentAction.Cancel(0);
				CurrentAction = null;
			}
			State = BrainState.Dead;
		}

		public void Activate()
		{
			if (State == BrainState.Off)
				State = BrainState.Idle;
		}

		public void DeActivate()
		{
			CurrentAction = null;
			State = BrainState.Off;
		}

		private void Perform(int tickCounter)
		{
			if (CurrentAction == null)
				return;

			if (!CurrentAction.Started)
				CurrentAction.Start(tickCounter);
			else
				CurrentAction.Update(tickCounter);

			if (CurrentAction.Done)
				CurrentAction = null;
		}
	}
}
