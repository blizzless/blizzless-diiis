//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement
{
	public class MoveToPointAction : ActorAction
	{
		public Vector3D Heading { get; private set; }

		public SteppedRelativeTickTimer Timer;

		public MoveToPointAction(Actor owner, Vector3D heading)
			: base(owner)
		{
			this.Heading = heading;
		}

		public override void Start(int tickCounter)
		{
			var distance = MovementHelpers.GetDistance(this.Owner.Position, this.Heading);
			var facingAngle = MovementHelpers.GetFacingAngle(this.Owner, this.Heading);
			this.Owner.Move(this.Heading, facingAngle);

			//Logger.Trace("Heading: " + this.Heading);
			//Logger.Trace("Start point: " + this.Owner.Position);

			this.Timer = new SteppedRelativeTickTimer(this.Owner.World.Game, 6, (int)(distance / this.Owner.WalkSpeed),
			(tick) =>
			{
				this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, facingAngle, 12);
				//Logger.Trace("Step: " + this.Owner.Position);
			},
			(tick) =>
			{
				this.Owner.Position = Heading;
				//Logger.Trace("Completed: " + this.Owner.Position);
				this.Done = true;
			});

			this.Started = true;
		}

		public override void Update(int tickCounter)
		{
			this.Timer.Update(tickCounter);
		}

		public override void Cancel(int tickCounter)
		{
			this.Done = true;
		}
	}
}
