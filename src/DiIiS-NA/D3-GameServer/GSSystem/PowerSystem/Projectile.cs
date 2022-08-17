//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class Projectile : Actor, IUpdateable
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType { get { return ActorType.Projectile; } }

		public PowerContext Context;

		public Func<Actor, bool> CollisionFilter = null;
		public Action<Actor> OnCollision = null;
		public List<Actor> CollidedActors = new List<Actor>();
		public bool FirstTimeCollided = false;
		public Action OnUpdate = null;
		public Action OnArrival = null;
		public Action OnTimeout = null;
		public TickTimer Timeout = null;
		public bool DestroyOnArrival = false;
		public TickTimer ArrivalTime { get { return _mover.ArrivalTime; } }
		public bool Arrived { get { return _mover.Arrived; } }
		public bool Slowed = false;
		public float RadiusMod = 1f;
		public float DmgMod = 1f;
		public Actor ChainCurrent = null;
		public Vector3D ChainNextPos = null;
		public int ChainTargetsRemain = 0;
		public int ChainIteration = 0;

		private ActorMover _mover;
		private Vector3D _prevUpdatePosition;
		private Vector3D _launchPosition;
		private bool _onArrivalCalled;
		private bool _spawned;  

		public Projectile(PowerContext context, int actorSNO, Vector3D position)
			: base(context.World, actorSNO)
		{
			//this.Field2 = 0x0;
			//this.Field7 = -1;  // TODO: test if this is necessary

			this.Field2 = 0x8;
			this.Field7 = -1;
			this.GBHandle.Type = (int)ActorType.Projectile; this.GBHandle.GBID = -1;

			this.CollFlags = 0;

			if (this.Scale == 0f)
				this.Scale = 1.00f;

			this.Context = context;
			this.Position = new Vector3D(position);
			// offset position by mpq collision data
			this.Position.Z += this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			// 2 second default timeout for projectiles
			this.Timeout = new SecondsTickTimer(context.World.Game, 3f);

			// copy in important effect params from user
			this.Attributes[GameAttribute.Rune_A, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_A, context.PowerSNO];
			this.Attributes[GameAttribute.Rune_B, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_B, context.PowerSNO];
			this.Attributes[GameAttribute.Rune_C, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_C, context.PowerSNO];
			this.Attributes[GameAttribute.Rune_D, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_D, context.PowerSNO];
			this.Attributes[GameAttribute.Rune_E, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_E, context.PowerSNO];


			if (this.Context.User.Attributes[GameAttribute.Displays_Team_Effect] == true)
				this.Attributes[GameAttribute.Displays_Team_Effect] = true;

			_prevUpdatePosition = null;
			_launchPosition = null;
			_mover = new ActorMover(this);
			_spawned = false;
		}

		public void LaunchWA(Vector3D targetPosition, float speed, Action AArrival)
		{
			_onArrivalCalled = false;
			OnArrival = AArrival;

			_prevUpdatePosition = this.Position;
			_launchPosition = this.Position;

			this.Attributes[GameAttribute.Projectile_Speed] = speed * 0.75f;

			this.TranslateFacing(targetPosition, true);
			targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			targetPosition.Z += this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;

			if (this.Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				this.Destroy();
				return;
			}

			this.Attributes[GameAttribute.DestroyWhenPathBlocked] = true;

			if (!_spawned)
			{
				this.EnterWorld(this.Position);
				_spawned = true;
			}

			_lastSpeed = this.Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveFixed(targetPosition, this.Attributes[GameAttribute.Projectile_Speed], new MessageSystem.Message.Definitions.ACD.ACDTranslateFixedMessage
			{
				MoveFlags = 0x7fffffff,
				AnimationTag = AnimationSetKeys.IdleDefault.ID,
				SNOPowerPassability = -1
			});

			//Logger.Debug("Projectile launched, id: {0}", this.DynamicID);
		}

		public void Launch(Vector3D targetPosition, float speed)
		{
			_onArrivalCalled = false;
			_prevUpdatePosition = this.Position;
			_launchPosition = this.Position;

			this.Attributes[GameAttribute.Projectile_Speed] = speed * 0.75f;

			this.TranslateFacing(targetPosition, true);
			targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			
			targetPosition.Z += this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;

			if (this.Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				this.Destroy();
				return;
			}

			this.Attributes[GameAttribute.DestroyWhenPathBlocked] = true;

			if (!_spawned)
			{
				this.EnterWorld(this.Position);
				_spawned = true;
			}

			_lastSpeed = this.Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveFixed(targetPosition, this.Attributes[GameAttribute.Projectile_Speed], new MessageSystem.Message.Definitions.ACD.ACDTranslateFixedMessage
			{
				MoveFlags = 0x7fffffff,
				AnimationTag = AnimationSetKeys.IdleDefault.ID,
				SNOPowerPassability = -1
			});

			//Logger.Debug("Projectile launched, id: {0}", this.DynamicID);
		}

		public void LaunchCircle(Vector3D centerPosition, float radius, float speed, float duration)
		{
			this.Position.X += radius;
			_onArrivalCalled = false;
			_prevUpdatePosition = this.Position;
			_launchPosition = this.Position;

			this.Attributes[GameAttribute.Projectile_Speed] = speed;

			//targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			//targetPosition.Z += this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;

			if (this.Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				this.Destroy();
				return;
			}

			this.Attributes[GameAttribute.DestroyWhenPathBlocked] = false;

			if (!_spawned)
			{
				this.EnterWorld(this.Position);
				_spawned = true;
			}

			_lastSpeed = this.Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveCircle(centerPosition, radius, this.Attributes[GameAttribute.Projectile_Speed], duration, new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSpiralMessage
			{
				AnimTag = AnimationSetKeys.IdleDefault.ID
			});

			//Logger.Debug("Projectile launched, id: {0}", this.DynamicID);
		}

		public void LaunchArc(Vector3D destination, float arcHeight, float arcGravity, float visualBounce = 0f)
		{
			_onArrivalCalled = false;
			_prevUpdatePosition = this.Position;
			_launchPosition = this.Position;

			this.TranslateFacing(destination, true);
			if (!_spawned)
			{
				this.EnterWorld(this.Position);
				_spawned = true;
			}

			_mover.MoveArc(destination, arcHeight, arcGravity, new MessageSystem.Message.Definitions.ACD.ACDTranslateArcMessage
			{
				Field3 = 0x00800000,
				FlyingAnimationTagID = AnimationSetKeys.IdleDefault.ID,
				LandingAnimationTagID = -1,
				PowerSNO = this.Context.PowerSNO,
				Bounce = visualBounce
			});
		}

		public void LaunchChain(Actor Caster, Vector3D TargetPos, Action<Actor, int> OnTargetHit, float Speed = 1f, int numTargets = 0, float ChainRadius = 10f)
		{
			this.Position.Z += 5f;  // fix height
			this.ChainCurrent = Caster;
			this.ChainTargetsRemain = numTargets;

			this.OnCollision = (hit) =>
			{
				if (hit == this.ChainCurrent) return;
				else this.ChainCurrent = hit;

				OnTargetHit(this.ChainCurrent, this.ChainIteration);

				this.ChainTargetsRemain--;
				if (this.ChainTargetsRemain <= 0)
				{
					this.Destroy();
					return;
				}

				if (this.ChainCurrent == null)
				{
					this.Destroy();
					return;
				}

				var targets = this.Context.GetEnemiesInRadius(this.ChainCurrent.Position, ChainRadius);
				targets.Actors.Remove(this.ChainCurrent);
				if (targets.Actors.Count() == 0)
				{
					this.Destroy();
					return;
				}

				var nextProj = new Projectile(this.Context, this.ActorSNO.Id, this.ChainCurrent.Position);
				nextProj.Position.Z += 5f;

				nextProj.ChainCurrent = this.ChainCurrent;
				nextProj.ChainNextPos = targets.Actors[PowerContext.Rand.Next(targets.Actors.Count())].Position;

				nextProj.ChainTargetsRemain = this.ChainTargetsRemain;
				nextProj.ChainIteration = this.ChainIteration + 1;

				nextProj.OnCollision = this.OnCollision;
				this.Destroy();
				nextProj.Launch(nextProj.ChainNextPos, Speed);
			};
			this.Launch(TargetPos, Speed);
		}

		private void _CheckCollisions()
		{
			if (OnCollision == null) return;

			if (this.World != this.Context.User.World)
			{
				this.Destroy();
				return;
			}
			if (MovementHelpers.GetDistance(_launchPosition, _prevUpdatePosition) >= 60.0f)
			{
				this.Destroy();
				return;
			}
			if (!(this.World.CheckLocationForFlag(_prevUpdatePosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk) || this.World.CheckLocationForFlag(_prevUpdatePosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowProjectile)))
			{
				this.Destroy();
				return;
			}

			// check if we collided with anything since last update

			float radius = this.ActorData.Cylinder.Ax2 * RadiusMod;
			Circle startCircle = new Circle(_prevUpdatePosition.X, _prevUpdatePosition.Y, radius);
			// make a velocity representing the change to the current position
			Vector2F velocity = PowerMath.VectorWithoutZ(this.Position - _prevUpdatePosition);

			Actor hit = null;
			TargetList targets = this.Context.GetEnemiesInRadius(this.Position, radius + 45f);
			if (CollisionFilter != null)
				targets.Actors.RemoveAll(actor => !CollisionFilter(actor));
			targets.SortByDistanceFrom(_prevUpdatePosition);

			foreach (Actor target in targets.Actors)
			{
				float targetRadius = target.ActorData.Cylinder.Ax2 + 5f;//target.ActorData.Cylinder.Ax2;
				if (PowerMath.MovingCircleCollides(startCircle, velocity, new Circle(target.Position.X, target.Position.Y, targetRadius)))
				{
					hit = target;
					break;
				}
			}

			if (hit != null && !this.CollidedActors.Contains(hit) && hit != this.Context.User && hit.Visible && !(hit is Door && (hit as Door).isOpened))
			{
				{
					FirstTimeCollided = true;
					this.CollidedActors.Add(hit);
					OnCollision(hit);
				}
				//Logger.Trace("Projectile collided, actor: {0}", hit.ActorSNO.Name);
			}
		}

		private float _lastSpeed = 0;

		public void Update(int tickCounter)
		{
			if (!_spawned) return;

			// gotta make sure the actor hasn't been deleted after processing each handler

			if (_lastSpeed != this.Attributes[GameAttribute.Projectile_Speed])
			{
				if (_mover.IsFixedMove())
				{
					this.Launch(_mover.GetDestination(), this.Attributes[GameAttribute.Projectile_Speed]);
					return;
				}
			}

			if (this.World != null)
				_CheckCollisions();

			// doing updates after collision tests
			if (this.World != null)
			{
				_prevUpdatePosition = this.Position;
				_mover.Update();
			}

			if (OnUpdate != null)
				OnUpdate();

			if (this.World != null && this.Arrived)
			{
				if (OnArrival != null && _onArrivalCalled == false)
				{
					_onArrivalCalled = true;
					OnArrival();
				}
				if (this.World != null && this.DestroyOnArrival &&
					this.Arrived) // double check arrival in case OnArrival() re-launched
					Destroy();
			}

			if (this.World != null)
			{
				if (Timeout.TimedOut)
				{
					if (OnTimeout != null)
						OnTimeout();

					Destroy();
				}
			}
		}

		public override void Destroy()
		{
			_spawned = false;
			base.Destroy();
		}
	}
}
