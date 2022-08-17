//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class ActorMover
	{
		public Actor Target;

		public Vector3D Velocity { get; private set; }
		public TickTimer ArrivalTime { get; private set; }
		public bool Arrived { get { return (ArrivalTime != null ? ArrivalTime.TimedOut : true); } }

		private Vector3D _startPosition;
		private Vector3D _endPosition;
		private int _startTick;
		private float _arcGravity;
		private float _angularSpeed;
		private float _curvatureRadius;
		private Vector3D _curvatureCenter;

		private enum MoveCommandType
		{
			Normal,
			Fixed,
			Arc,
			Circle
		}
		private MoveCommandType _moveCommand;

		public ActorMover(Actor target)
		{
			this.Target = target;
		}

		public void Move(Vector3D destination, float speed, ACDTranslateNormalMessage baseMessage = null)
		{
			if (this.Target == null || this.Target.World == null) return;
			//if (destination == this.Target.Position) return;
			_SetupMove(destination, speed);
			_moveCommand = MoveCommandType.Normal;

			if (baseMessage == null)
				baseMessage = new ACDTranslateNormalMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Position = destination;
			baseMessage.Angle = (float)Math.Acos(this.Target.RotationW) * 2f;
			baseMessage.MovementSpeed = speed;

			
			this.Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = this.Target.DynamicID(plr); return baseMessage; }, this.Target);
		}

		public void MoveFixed(Vector3D targetPosition, float speed, ACDTranslateFixedMessage baseMessage = null)
		{
			if (this.Target == null || this.Target.World == null) return;
			_SetupMove(targetPosition, speed);
			_moveCommand = MoveCommandType.Fixed;

			if (baseMessage == null)
				baseMessage = new ACDTranslateFixedMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Velocity = this.Velocity;

			this.Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = (int)this.Target.DynamicID(plr); return baseMessage; }, this.Target);
		}

		public bool IsFixedMove()
		{
			return (_moveCommand == MoveCommandType.Fixed);
		}

		public Vector3D GetDestination()
		{
			return _endPosition;
		}

		public void MoveArc(Vector3D destination, float height, float gravity, ACDTranslateArcMessage baseMessage = null)
		{
			if (this.Target == null || this.Target.World == null) return;
			_SetupArcMove(destination, height, gravity);
			_moveCommand = MoveCommandType.Arc;

			if (baseMessage == null)
				baseMessage = new ACDTranslateArcMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Start = this.Target.Position;
			baseMessage.Velocity = this.Velocity;
			baseMessage.Gravity = gravity;
			baseMessage.DestinationZ = destination.Z;

			this.Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = (int)this.Target.DynamicID(plr); return baseMessage; }, this.Target);
		}

		public void MoveCircle(Vector3D center, float radius, float speed, float duration, ACDTranslateDetPathSpiralMessage baseMessage = null)
		{
			if (this.Target == null || this.Target.World == null) return;

			_curvatureCenter = new Vector3D(center);
			_curvatureRadius = radius;
			_angularSpeed = speed / radius;
			_SetCircleVelocity();           //projectile is placed on trajectory in LaunchCircle
			this.ArrivalTime = new RelativeTickTimer(this.Target.World.Game, (int)(duration * 60f));

			_startPosition = this.Target.Position;
			_endPosition = null;
			_startTick = this.Target.World.Game.TickCounter;
			_moveCommand = MoveCommandType.Circle;

			if (baseMessage == null)
				baseMessage = new ACDTranslateDetPathSpiralMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.StartPosition = _startPosition;
			baseMessage.TargetPosition = this.Velocity;

			this.Target.World.BroadcastIfRevealed(plr => { baseMessage.DynamicId = this.Target.DynamicID(plr); return baseMessage; }, this.Target);
		}
		private void _SetCircleVelocity()
		{
			Vector3D angular = _curvatureCenter + new Vector3D(0, 0, _angularSpeed);
			this.Velocity = PowerMath.CrossProduct(angular, this.Target.Position);
		}

		public bool Update()
		{
			if (this.Target == null || this.Target.World == null) return true;
			_UpdatePosition();
			return this.Arrived;
		}

		private void _SetupMove(Vector3D destination, float speed)
		{
			Vector3D dir_normal = PowerMath.Normalize(new Vector3D(destination.X - this.Target.Position.X,
																   destination.Y - this.Target.Position.Y,
																   destination.Z - this.Target.Position.Z));

			this.Velocity = new Vector3D(dir_normal.X * speed,
										 dir_normal.Y * speed,
										 dir_normal.Z * speed);

			this.ArrivalTime = new RelativeTickTimer(this.Target.World.Game,
													 (int)(PowerMath.Distance2D(this.Target.Position, destination) / speed));
			_startPosition = this.Target.Position;
			_endPosition = destination;
			_startTick = this.Target.World.Game.TickCounter;
		}

		private void _SetupArcMove(Vector3D destination, float crestHeight, float gravity)
		{
			// TODO: handle when target and destination heights differ
			float absGravity = Math.Abs(gravity);
			float arcLength = (float)Math.Sqrt(2f * crestHeight / absGravity);
			int arrivalTicks = (int)(arcLength * 2f);

			float distance = PowerMath.Distance2D(this.Target.Position, destination);
			Vector3D normal = PowerMath.Normalize(new Vector3D(destination.X - this.Target.Position.X,
															   destination.Y - this.Target.Position.Y,
															   0f));

			this.Velocity = new Vector3D(normal.X * (distance / arrivalTicks),
										 normal.Y * (distance / arrivalTicks),
										 absGravity * arcLength);

			this.ArrivalTime = new RelativeTickTimer(this.Target.World.Game, arrivalTicks);
			_startPosition = this.Target.Position;
			_endPosition = destination;
			_startTick = this.Target.World.Game.TickCounter;
			_arcGravity = gravity;
		}

		private void _UpdatePosition()
		{
			if (_moveCommand == MoveCommandType.Circle && this.Arrived)
			{
				this.Target.Destroy();
				return;
			}
			if (_moveCommand != MoveCommandType.Fixed && this.Arrived)
			{
				this.Target.Position = _endPosition;
				return;
			}
			int moveTicks = 1;
			try
			{
				moveTicks = this.Target.World.Game.TickCounter - _startTick;
			}
			catch { }

			if (_moveCommand == MoveCommandType.Arc)
			{
				this.Target.Position = new Vector3D(_startPosition.X + this.Velocity.X * moveTicks,
					_startPosition.Y + this.Velocity.Y * moveTicks,
					_startPosition.Z + 0.5f * _arcGravity * (moveTicks * moveTicks) + this.Velocity.Z * moveTicks);
			}
			else if (_moveCommand == MoveCommandType.Circle)
			{
				this.Target.Position = new Vector3D(_startPosition.X + this.Velocity.X * moveTicks,
					_startPosition.Y + this.Velocity.Y * moveTicks,
					_startPosition.Z + this.Velocity.Z * moveTicks);
				_SetCircleVelocity();
				//this.Target.TranslateFacing(this.Target.Position + this.Velocity, true);
			}
			else
			{
				this.Target.Position = new Vector3D(_startPosition.X + this.Velocity.X * moveTicks,
					_startPosition.Y + this.Velocity.Y * moveTicks,
					_startPosition.Z + this.Velocity.Z * moveTicks);
			}
		}
	}
}
