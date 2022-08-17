//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions
{
	public abstract class ActorAction
	{
		protected static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The action owner actor.
		/// </summary>
		public Actor Owner { get; private set; }

		/// <summary>
		/// Returns true if the action is completed.
		/// </summary>
		public bool Done { get; protected set; }

		/// <summary>
		/// Returns true if the action is already started.
		/// </summary>
		public bool Started { get; protected set; }

		protected ActorAction(Actor owner)
		{
			this.Owner = owner;
			this.Started = false;
			this.Done = false;
		}

		public abstract void Start(int tickCounter);

		public abstract void Update(int tickCounter);

		public abstract void Cancel(int tickCounter);
	}
}
