//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
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
			this.Body = body;
			this.State = BrainState.Idle;
			this.Actions = new Queue<ActorAction>();
		}

		protected void QueueAction(ActorAction action)
		{
			this.Actions.Enqueue(action);
		}

		public virtual void Update(int tickCounter)
		{
			if (this.State == BrainState.Dead || this.Body == null || this.Body.World == null || this.State == BrainState.Off)
				return;

			this.Think(tickCounter); // let the brain think.
			this.Perform(tickCounter); // perform any outstanding actions.
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
			if (this.CurrentAction != null)
			{
				this.CurrentAction.Cancel(0);
				this.CurrentAction = null;
			}
			this.State = BrainState.Dead;
		}

		public void Activate()
		{
			if (this.State == BrainState.Off)
				this.State = BrainState.Idle;
		}

		public void DeActivate()
		{
			this.CurrentAction = null;
			this.State = BrainState.Off;
		}

		private void Perform(int tickCounter)
		{
			if (this.CurrentAction == null)
				return;

			if (!this.CurrentAction.Started)
				this.CurrentAction.Start(tickCounter);
			else
				this.CurrentAction.Update(tickCounter);

			if (this.CurrentAction.Done)
				this.CurrentAction = null;
		}
	}
}
