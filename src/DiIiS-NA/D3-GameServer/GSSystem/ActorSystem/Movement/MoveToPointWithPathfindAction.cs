//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement
{
	public class MoveToPointWithPathfindAction : ActorAction
	{
		public Vector3D Heading { get; private set; }
		public float AttackRadius { get; private set; }

		private int _lastDelay = 0;
		//private int _totalDelay = 0;

		private TickTimer Timer;
		private bool Stucked = true;

		public MoveToPointWithPathfindAction(Actor owner, Vector3D heading, float attackRadius = 6f)
			: base(owner)
		{
			this.Heading = heading;
			this.AttackRadius = attackRadius;
		}

		public override void Start(int tickCounter)
		{
			if (this.Heading == this.Owner.Position)
			{
				this.Started = true;
				this.Done = true;
				return;
			}

			this.Started = true;
			
			if (!this.Done)
				this.Move();
		}

		private void Move()
		{
			this.Stucked = true;
			Vector3D defaultPosition = this.Owner.Position;
			Vector3D destPoint = PowerMath.TranslateDirection2D(this.Owner.Position, this.Heading, this.Owner.Position, Math.Min(MovementHelpers.GetDistance(this.Owner.Position, this.Heading), this.Owner.WalkSpeed * 60));
			if (this.Owner.Spawner)
				destPoint = this.Owner.Position;

			var points = PowerMath.GenerateSpreadPositions(this.Owner.Position, destPoint, 30f, 6).OrderBy((pos) => MovementHelpers.GetDistance(pos, destPoint)).ToList();
			foreach (var point in points)
			{
				bool point_accessible = true;
				for (float i = 0.5f; i <= MovementHelpers.GetDistance(this.Owner.Position, point); i += 1f)
				{
					var point_check = PowerMath.TranslateDirection2D(this.Owner.Position, point, this.Owner.Position, i);
					if (!(this.Owner.World.CheckLocationForFlag(point_check, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk) && !this.Owner.World.CheckLocationForFlag(point_check, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoNavMeshIntersected)))
					{
						point_accessible = false;
						break;
					}
				}

				if (point_accessible)
				{
					destPoint = point;
					if (this.Owner.Spawner)
						destPoint = this.Owner.Position;
					this.Stucked = false;
					break;
				}
			}

			if (!this.Stucked && this.Owner.WalkSpeed > 0f)
			{
				this.Owner.Move(destPoint, MovementHelpers.GetFacingAngle(this.Owner, destPoint));
				var delay = (int)(MovementHelpers.GetDistance(this.Owner.Position, destPoint) / this.Owner.WalkSpeed);
				_lastDelay = delay + (6 - (delay % 6));

			}
			else
				_lastDelay = 60;

			this.Timer = TickTimer.WaitTicks(
				this.Owner.World.Game,
				_lastDelay,
				(tick) =>
				{
					if (!this.Stucked && this.Owner.WalkSpeed > 0f)
					{
						this.Owner.Position = MovementHelpers.GetMovementPosition(defaultPosition, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(defaultPosition, destPoint), _lastDelay);
					}

					if (this.Owner == null ||
						this.Owner.Attributes[GameAttribute.Hitpoints_Cur] == 0 ||
						this.Owner.GetObjectsInRange<Player>(50f).Count == 0 ||
						MovementHelpers.GetDistance(this.Owner.Position, this.Heading) < this.AttackRadius ||
						(this.Owner is Monster && (this.Owner as Monster).Brain.CurrentAction == null)
					)
					{
						this.Done = true;
						//ТЕСТ
						this.Owner.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateSyncMessage()
						{ ActorId = this.Owner.DynamicID(plr), Position = this.Owner.Position, Snap = false }, this.Owner);

					}
					if (!this.Done)
					{
						this.Move();
					}
				}
			);
		}

		public override void Update(int tickCounter)
		{
			if (this.Timer != null)
				this.Timer.Update(tickCounter);
		}

		public override void Cancel(int tickCounter)
		{
			this.Done = true;
		}
	}
}
