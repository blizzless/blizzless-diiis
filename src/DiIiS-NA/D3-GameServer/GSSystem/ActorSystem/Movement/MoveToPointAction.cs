using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
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
			Heading = heading;
		}

		public override void Start(int tickCounter)
		{
			var distance = MovementHelpers.GetDistance(Owner.Position, Heading);
			var facingAngle = MovementHelpers.GetFacingAngle(Owner, Heading);
			Owner.Move(Heading, facingAngle);

			//Logger.Trace("Heading: " + this.Heading);
			//Logger.Trace("Start point: " + this.Owner.Position);

			Timer = new SteppedRelativeTickTimer(Owner.World.Game, 6, (int)(distance / Owner.WalkSpeed),
			(tick) =>
			{
				Owner.Position = MovementHelpers.GetMovementPosition(Owner.Position, Owner.WalkSpeed, facingAngle, 12);
				//Logger.Trace("Step: " + this.Owner.Position);
			},
			(tick) =>
			{
				Owner.Position = Heading;
				//Logger.Trace("Completed: " + this.Owner.Position);
				Done = true;
			});

			Started = true;
		}

		public override void Update(int tickCounter)
		{
			Timer.Update(tickCounter);
		}

		public override void Cancel(int tickCounter)
		{
			Done = true;
		}
	}
}
