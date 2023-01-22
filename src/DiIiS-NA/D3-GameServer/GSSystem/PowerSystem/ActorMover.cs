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
			Target = target;
		}

		public void Move(Vector3D destination, float speed, ACDTranslateNormalMessage baseMessage = null)
		{
			if (Target == null || Target.World == null) return;
			//if (destination == this.Target.Position) return;
			_SetupMove(destination, speed);
			_moveCommand = MoveCommandType.Normal;

			if (baseMessage == null)
				baseMessage = new ACDTranslateNormalMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Position = destination;
			baseMessage.Angle = (float)Math.Acos(Target.RotationW) * 2f;
			baseMessage.MovementSpeed = speed;

			
			Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = Target.DynamicID(plr); return baseMessage; }, Target);
		}

		public void MoveFixed(Vector3D targetPosition, float speed, ACDTranslateFixedMessage baseMessage = null)
		{
			if (Target == null || Target.World == null) return;
			_SetupMove(targetPosition, speed);
			_moveCommand = MoveCommandType.Fixed;

			if (baseMessage == null)
				baseMessage = new ACDTranslateFixedMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Velocity = Velocity;

			Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = (int)Target.DynamicID(plr); return baseMessage; }, Target);
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
			if (Target == null || Target.World == null) return;
			_SetupArcMove(destination, height, gravity);
			_moveCommand = MoveCommandType.Arc;

			if (baseMessage == null)
				baseMessage = new ACDTranslateArcMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.Start = Target.Position;
			baseMessage.Velocity = Velocity;
			baseMessage.Gravity = gravity;
			baseMessage.DestinationZ = destination.Z;

			Target.World.BroadcastIfRevealed(plr => { baseMessage.ActorId = (int)Target.DynamicID(plr); return baseMessage; }, Target);
		}

		public void MoveCircle(Vector3D center, float radius, float speed, float duration, ACDTranslateDetPathSpiralMessage baseMessage = null)
		{
			if (Target == null || Target.World == null) return;

			_curvatureCenter = new Vector3D(center);
			_curvatureRadius = radius;
			_angularSpeed = speed / radius;
			_SetCircleVelocity();           //projectile is placed on trajectory in LaunchCircle
			ArrivalTime = new RelativeTickTimer(Target.World.Game, (int)(duration * 60f));

			_startPosition = Target.Position;
			_endPosition = null;
			_startTick = Target.World.Game.TickCounter;
			_moveCommand = MoveCommandType.Circle;

			if (baseMessage == null)
				baseMessage = new ACDTranslateDetPathSpiralMessage();

			//baseMessage.ActorId = (int)this.Target.DynamicID;
			baseMessage.StartPosition = _startPosition;
			baseMessage.TargetPosition = Velocity;

			Target.World.BroadcastIfRevealed(plr => { baseMessage.DynamicId = Target.DynamicID(plr); return baseMessage; }, Target);
		}
		private void _SetCircleVelocity()
		{
			Vector3D angular = _curvatureCenter + new Vector3D(0, 0, _angularSpeed);
			Velocity = PowerMath.CrossProduct(angular, Target.Position);
		}

		public bool Update()
		{
			if (Target == null || Target.World == null) return true;
			_UpdatePosition();
			return Arrived;
		}

		private void _SetupMove(Vector3D destination, float speed)
		{
			Vector3D dir_normal = PowerMath.Normalize(new Vector3D(destination.X - Target.Position.X,
																   destination.Y - Target.Position.Y,
																   destination.Z - Target.Position.Z));

			Velocity = new Vector3D(dir_normal.X * speed,
										 dir_normal.Y * speed,
										 dir_normal.Z * speed);

			ArrivalTime = new RelativeTickTimer(Target.World.Game,
													 (int)(PowerMath.Distance2D(Target.Position, destination) / speed));
			_startPosition = Target.Position;
			_endPosition = destination;
			_startTick = Target.World.Game.TickCounter;
		}

		private void _SetupArcMove(Vector3D destination, float crestHeight, float gravity)
		{
			// TODO: handle when target and destination heights differ
			float absGravity = Math.Abs(gravity);
			float arcLength = (float)Math.Sqrt(2f * crestHeight / absGravity);
			int arrivalTicks = (int)(arcLength * 2f);

			float distance = PowerMath.Distance2D(Target.Position, destination);
			Vector3D normal = PowerMath.Normalize(new Vector3D(destination.X - Target.Position.X,
															   destination.Y - Target.Position.Y,
															   0f));

			Velocity = new Vector3D(normal.X * (distance / arrivalTicks),
										 normal.Y * (distance / arrivalTicks),
										 absGravity * arcLength);

			ArrivalTime = new RelativeTickTimer(Target.World.Game, arrivalTicks);
			_startPosition = Target.Position;
			_endPosition = destination;
			_startTick = Target.World.Game.TickCounter;
			_arcGravity = gravity;
		}

		private void _UpdatePosition()
		{
			if (_moveCommand == MoveCommandType.Circle && Arrived)
			{
				Target.Destroy();
				return;
			}
			if (_moveCommand != MoveCommandType.Fixed && Arrived)
			{
				Target.Position = _endPosition;
				return;
			}
			int moveTicks = 1;
			try
			{
				moveTicks = Target.World.Game.TickCounter - _startTick;
			}
			catch { }

			if (_moveCommand == MoveCommandType.Arc)
			{
				Target.Position = new Vector3D(_startPosition.X + Velocity.X * moveTicks,
					_startPosition.Y + Velocity.Y * moveTicks,
					_startPosition.Z + 0.5f * _arcGravity * (moveTicks * moveTicks) + Velocity.Z * moveTicks);
			}
			else if (_moveCommand == MoveCommandType.Circle)
			{
				Target.Position = new Vector3D(_startPosition.X + Velocity.X * moveTicks,
					_startPosition.Y + Velocity.Y * moveTicks,
					_startPosition.Z + Velocity.Z * moveTicks);
				_SetCircleVelocity();
				//this.Target.TranslateFacing(this.Target.Position + this.Velocity, true);
			}
			else
			{
				Target.Position = new Vector3D(_startPosition.X + Velocity.X * moveTicks,
					_startPosition.Y + Velocity.Y * moveTicks,
					_startPosition.Z + Velocity.Z * moveTicks);
			}
		}
	}
}
