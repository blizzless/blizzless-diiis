//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
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
	public class MoveToTargetWithPathfindAction : ActorAction
	{
		public Actor Target { get; private set; }
		public Vector3D Heading { get; private set; }
		public float AttackRadius { get; private set; }

		private int _lastDelay = 0;
		private int PrioritySkillSNO = -1;

		private TickTimer Timer;
		private bool Stucked = true;
		private bool Canceled = false;

		//private List<Vector3D> _path = new List<Vector3D>();
		//private AI.Pather.PathRequestTask _pathRequestTask;
		public MoveToTargetWithPathfindAction(Actor owner, Actor target, float attackRadius = 6f, int prioritySkillSNO = -1)
			: base(owner)
		{
			// Sending a request for a Path to the Pathing thread.
			//_pathRequestTask = owner.World.Game.Pathfinder.GetPath(owner, owner.Position, heading);
			this.Target = target;
			this.Heading = target.Position;
			this.AttackRadius = attackRadius;
			this.PrioritySkillSNO = prioritySkillSNO;
		}

		public override void Start(int tickCounter)
		{
			// Just wait, path request hasnt been processed yet, idealy this would be null or something instead - Darklotus
			//if (!_pathRequestTask.PathFound)
			//return;

			/*if (this.Heading == this.Owner.Position)
			{
				this.Started = true;
				this.Done = true;
				return;
			}*/

			// No path found, so end Action.
			/*if (_pathRequestTask.Path.Count < 1)
			{
				this.Started = true;
				this.Done = true;
				return;
			}*/
			//_path = _pathRequestTask.Path;
			// Each path step will be 2.5f apart roughly, not sure on the math to get correct walk speed for the timer.
			// mobs sometimes skip a bit, pretty sure this is because timing isnt correct.  :( - DarkLotus
			//Logger.Trace("Start() {0}", tickCounter);
			if (this.Owner.WalkSpeed == 0)
			{
				this.Done = true;
				return;
			}

			this.Started = true;
			//this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(this.Owner.Position, this.Owner.CurrentDestination), 60);
			if (!this.Done)
				this.Move();
		}

		private void Move()
		{
			this.Stucked = true;
			Vector3D defaultPosition = this.Owner.Position;
			Vector3D destPoint = PowerMath.TranslateDirection2D(this.Owner.Position, this.Target.Position, this.Owner.Position, Math.Min(MovementHelpers.GetDistance(this.Owner.Position, this.Target.Position), this.Owner.WalkSpeed * 30));
			//int searchLimit = 0;
			var points = PowerMath.GenerateSpreadPositions(this.Owner.Position, destPoint, 30f, 6).OrderBy((pos) => MovementHelpers.GetDistance(pos, destPoint)).ToList();
			foreach (var point in points)
			{
				/*if (MovementHelpers.GetUnitsOnPath(
					this.Owner.World, 
					PowerMath.TranslateDirection2D(this.Owner.Position, destPoint, this.Owner.Position, this.Owner.ActorData.Cylinder.Ax2 * 2), 
					destPoint,
					MovementHelpers.GetDistance(this.Owner.Position, destPoint), 
					this.Owner.ActorData.Cylinder.Ax2 / 2f
				).Count > 1 || !this.Owner.World.CheckLocationForFlag(destPoint, Mooege.Common.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))*/
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
				//var half_point = PowerMath.TranslateDirection2D(this.Owner.Position, point, this.Owner.Position, MovementHelpers.GetDistance(this.Owner.Position, point) / 2f);
				//var pre_half_point = PowerMath.TranslateDirection2D(this.Owner.Position, half_point, this.Owner.Position, MovementHelpers.GetDistance(this.Owner.Position, half_point) / 2f);
				//var post_half_point = PowerMath.TranslateDirection2D(half_point, point, half_point, MovementHelpers.GetDistance(half_point, point) / 2f);
				if (point_accessible)
				{
					destPoint = point;
					this.Stucked = false;
					break;
				}
			}

			//if (this.Stucked && this.Owner.GetMonstersInRange(this.Owner.ActorData.Cylinder.Ax2 * 2).Count >= 2)
			//this.Stucked = false;

			//Vector3D destPoint = this._path.First();
			if (!this.Stucked && this.Owner.WalkSpeed > 0f)
			{
				this.Owner.Move(destPoint, MovementHelpers.GetFacingAngle(this.Owner, destPoint));
				_lastDelay = (int)(MovementHelpers.GetDistance(this.Owner.Position, destPoint) / (this.Owner.WalkSpeed));

				//Logger.Trace("Delay in step: {0}", _lastDelay);
			}
			else
				_lastDelay = 60;

			this.Timer = TickTimer.WaitTicks(
				this.Owner.World.Game,
				_lastDelay,
				(tick) =>
				{
					if (this.Canceled) return;
					if (!this.Stucked && this.Owner.WalkSpeed > 0f)
					{
						this.Owner.Position = MovementHelpers.GetMovementPosition(defaultPosition, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(this.Owner.Position, destPoint), _lastDelay);
					}

					if (this.Owner == null ||
						this.Owner.Attributes[GameAttribute.Hitpoints_Cur] == 0 ||
						this.Owner.GetObjectsInRange<Player>(60f).Count == 0 ||
						MovementHelpers.GetDistance(this.Owner.Position, this.Target.Position) < this.AttackRadius ||
						(this.Owner is Monster && (this.Owner as Monster).Brain.CurrentAction == null)
					)
						this.Done = true;

					if (!this.Done)
					{
						this.Move();
					}
					else
					{
						if (this.Owner is Monster && this.Target != null)
						{
							var brain = (this.Owner as Monster).Brain as MonsterBrain;

							if (this.PrioritySkillSNO > 0)
							{
								brain.FastAttack(this.Target, this.PrioritySkillSNO);
							}
						}
					}
				}
			);

			//this.Done = true;
		}

		public override void Update(int tickCounter)
		{
			if (this.Timer != null)
				this.Timer.Update(tickCounter);
		}

		public override void Cancel(int tickCounter)
		{
			this.Done = true;
			this.Canceled = true;
			this.Owner.World.BroadcastIfRevealed(this.Owner.ACDWorldPositionMessage, this.Owner);
			//this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(this.Owner.Position, this.Owner.CurrentDestination), (int)(_lastDelay / 16f));
		}
	}
}
