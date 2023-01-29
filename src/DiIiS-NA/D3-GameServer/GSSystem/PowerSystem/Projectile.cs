using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Misc;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

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

		public Projectile(PowerContext context, ActorSno actorSNO, Vector3D position)
			: base(context.World, actorSNO)
		{
			//this.Field2 = 0x0;
			//this.Field7 = -1;  // TODO: test if this is necessary

			Field2 = 0x8;
			Field7 = -1;
			GBHandle.Type = (int)ActorType.Projectile; GBHandle.GBID = -1;

			CollFlags = 0;

			if (Scale == 0f)
				Scale = 1.00f;

			Context = context;
			Position = new Vector3D(position);
			// offset position by mpq collision data
			Position.Z += ActorData.Cylinder.Ax1 - ActorData.Cylinder.Position.Z;
			// 2 second default timeout for projectiles
			Timeout = new SecondsTickTimer(context.World.Game, 3f);

			// copy in important effect params from user
			Attributes[GameAttribute.Rune_A, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_A, context.PowerSNO];
			Attributes[GameAttribute.Rune_B, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_B, context.PowerSNO];
			Attributes[GameAttribute.Rune_C, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_C, context.PowerSNO];
			Attributes[GameAttribute.Rune_D, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_D, context.PowerSNO];
			Attributes[GameAttribute.Rune_E, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_E, context.PowerSNO];


			if (Context.User.Attributes[GameAttribute.Displays_Team_Effect] == true)
				Attributes[GameAttribute.Displays_Team_Effect] = true;

			_prevUpdatePosition = null;
			_launchPosition = null;
			_mover = new ActorMover(this);
			_spawned = false;
		}

		public void LaunchWA(Vector3D targetPosition, float speed, Action AArrival)
		{
			_onArrivalCalled = false;
			OnArrival = AArrival;

			_prevUpdatePosition = Position;
			_launchPosition = Position;

			Attributes[GameAttribute.Projectile_Speed] = speed * 0.75f;

			TranslateFacing(targetPosition, true);
			targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			targetPosition.Z += ActorData.Cylinder.Ax1 - ActorData.Cylinder.Position.Z;

			if (Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				Destroy();
				return;
			}

			Attributes[GameAttribute.DestroyWhenPathBlocked] = true;

			if (!_spawned)
			{
				EnterWorld(Position);
				_spawned = true;
			}

			_lastSpeed = Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveFixed(targetPosition, Attributes[GameAttribute.Projectile_Speed], new MessageSystem.Message.Definitions.ACD.ACDTranslateFixedMessage
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
			_prevUpdatePosition = Position;
			_launchPosition = Position;

			Attributes[GameAttribute.Projectile_Speed] = speed * 0.75f;

			TranslateFacing(targetPosition, true);
			targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			
			targetPosition.Z += ActorData.Cylinder.Ax1 - ActorData.Cylinder.Position.Z;

			if (Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				Destroy();
				return;
			}

			Attributes[GameAttribute.DestroyWhenPathBlocked] = true;

			if (!_spawned)
			{
				EnterWorld(Position);
				_spawned = true;
			}

			_lastSpeed = Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveFixed(targetPosition, Attributes[GameAttribute.Projectile_Speed], new MessageSystem.Message.Definitions.ACD.ACDTranslateFixedMessage
			{
				MoveFlags = 0x7fffffff,
				AnimationTag = AnimationSetKeys.IdleDefault.ID,
				SNOPowerPassability = -1
			});

			//Logger.Debug("Projectile launched, id: {0}", this.DynamicID);
		}

		public void LaunchCircle(Vector3D centerPosition, float radius, float speed, float duration)
		{
			Position.X += radius;
			_onArrivalCalled = false;
			_prevUpdatePosition = Position;
			_launchPosition = Position;

			Attributes[GameAttribute.Projectile_Speed] = speed;

			//targetPosition = new Vector3D(targetPosition);
			//targetPosition.Z = this.Context.User.Position.Z + 5f + this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;
			//targetPosition.Z += this.ActorData.Cylinder.Ax1 - this.ActorData.Cylinder.Position.Z;

			if (Attributes[GameAttribute.Projectile_Speed] <= 0)
			{
				Destroy();
				return;
			}

			Attributes[GameAttribute.DestroyWhenPathBlocked] = false;

			if (!_spawned)
			{
				EnterWorld(Position);
				_spawned = true;
			}

			_lastSpeed = Attributes[GameAttribute.Projectile_Speed];

			_mover.MoveCircle(centerPosition, radius, Attributes[GameAttribute.Projectile_Speed], duration, new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSpiralMessage
			{
				AnimTag = AnimationSetKeys.IdleDefault.ID
			});

			//Logger.Debug("Projectile launched, id: {0}", this.DynamicID);
		}

		public void LaunchArc(Vector3D destination, float arcHeight, float arcGravity, float visualBounce = 0f)
		{
			_onArrivalCalled = false;
			_prevUpdatePosition = Position;
			_launchPosition = Position;

			TranslateFacing(destination, true);
			if (!_spawned)
			{
				EnterWorld(Position);
				_spawned = true;
			}

			_mover.MoveArc(destination, arcHeight, arcGravity, new MessageSystem.Message.Definitions.ACD.ACDTranslateArcMessage
			{
				Field3 = 0x00800000,
				FlyingAnimationTagID = AnimationSetKeys.IdleDefault.ID,
				LandingAnimationTagID = -1,
				PowerSNO = Context.PowerSNO,
				Bounce = visualBounce
			});
		}

		public void LaunchChain(Actor Caster, Vector3D TargetPos, Action<Actor, int> OnTargetHit, float Speed = 1f, int numTargets = 0, float ChainRadius = 10f)
		{
			Position.Z += 5f;  // fix height
			ChainCurrent = Caster;
			ChainTargetsRemain = numTargets;

			OnCollision = (hit) =>
			{
				if (hit == ChainCurrent) return;
				else ChainCurrent = hit;

				OnTargetHit(ChainCurrent, ChainIteration);

				ChainTargetsRemain--;
				if (ChainTargetsRemain <= 0)
				{
					Destroy();
					return;
				}

				if (ChainCurrent == null)
				{
					Destroy();
					return;
				}

				var targets = Context.GetEnemiesInRadius(ChainCurrent.Position, ChainRadius);
				targets.Actors.Remove(ChainCurrent);
				if (!targets.Actors.Any())
				{
					Destroy();
					return;
				}

				var nextProj = new Projectile(Context, SNO, ChainCurrent.Position);
				nextProj.Position.Z += 5f;

				nextProj.ChainCurrent = ChainCurrent;
				nextProj.ChainNextPos = targets.Actors[PowerContext.Rand.Next(targets.Actors.Count)].Position;

				nextProj.ChainTargetsRemain = ChainTargetsRemain;
				nextProj.ChainIteration = ChainIteration + 1;

				nextProj.OnCollision = OnCollision;
				Destroy();
				nextProj.Launch(nextProj.ChainNextPos, Speed);
			};
			Launch(TargetPos, Speed);
		}

		private void _CheckCollisions()
		{
			if (OnCollision == null) return;

			if (World != Context.User.World)
			{
				Destroy();
				return;
			}
			if (MovementHelpers.GetDistance(_launchPosition, _prevUpdatePosition) >= 60.0f)
			{
				Destroy();
				return;
			}
			if (!(World.CheckLocationForFlag(_prevUpdatePosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk) || World.CheckLocationForFlag(_prevUpdatePosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowProjectile)))
			{
				Destroy();
				return;
			}

			// check if we collided with anything since last update

			float radius = ActorData.Cylinder.Ax2 * RadiusMod;
			Circle startCircle = new Circle(_prevUpdatePosition.X, _prevUpdatePosition.Y, radius);
			// make a velocity representing the change to the current position
			Vector2F velocity = PowerMath.VectorWithoutZ(Position - _prevUpdatePosition);

			Actor hit = null;
			TargetList targets = Context.GetEnemiesInRadius(Position, radius + 45f);
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

			if (hit != null && !CollidedActors.Contains(hit) && hit != Context.User && hit.Visible && !(hit is Door && (hit as Door).isOpened))
			{
				{
					FirstTimeCollided = true;
					CollidedActors.Add(hit);
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

			if (_lastSpeed != Attributes[GameAttribute.Projectile_Speed])
			{
				if (_mover.IsFixedMove())
				{
					Launch(_mover.GetDestination(), Attributes[GameAttribute.Projectile_Speed]);
					return;
				}
			}

			if (World != null)
				_CheckCollisions();

			// doing updates after collision tests
			if (World != null)
			{
				_prevUpdatePosition = Position;
				_mover.Update();
			}

			if (OnUpdate != null)
				OnUpdate();

			if (World != null && Arrived)
			{
				if (OnArrival != null && _onArrivalCalled == false)
				{
					_onArrivalCalled = true;
					OnArrival();
				}
				if (World != null && DestroyOnArrival &&
					Arrived) // double check arrival in case OnArrival() re-launched
					Destroy();
			}

			if (World != null)
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
