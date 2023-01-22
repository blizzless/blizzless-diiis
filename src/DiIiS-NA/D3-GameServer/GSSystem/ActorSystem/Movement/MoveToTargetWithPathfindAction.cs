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
			Target = target;
			Heading = target.Position;
			AttackRadius = attackRadius;
			PrioritySkillSNO = prioritySkillSNO;
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
			if (Owner.WalkSpeed == 0)
			{
				Done = true;
				return;
			}

			Started = true;
			//this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(this.Owner.Position, this.Owner.CurrentDestination), 60);
			if (!Done)
				Move();
		}

		private void Move()
		{
			Stucked = true;
			Vector3D defaultPosition = Owner.Position;
			Vector3D destPoint = PowerMath.TranslateDirection2D(Owner.Position, Target.Position, Owner.Position, Math.Min(MovementHelpers.GetDistance(Owner.Position, Target.Position), Owner.WalkSpeed * 30));
			//int searchLimit = 0;
			var points = PowerMath.GenerateSpreadPositions(Owner.Position, destPoint, 30f, 6).OrderBy((pos) => MovementHelpers.GetDistance(pos, destPoint)).ToList();
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
				for (float i = 0.5f; i <= MovementHelpers.GetDistance(Owner.Position, point); i += 1f)
				{
					var point_check = PowerMath.TranslateDirection2D(Owner.Position, point, Owner.Position, i);
					if (!(Owner.World.CheckLocationForFlag(point_check, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk) && !Owner.World.CheckLocationForFlag(point_check, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoNavMeshIntersected)))
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
					Stucked = false;
					break;
				}
			}

			//if (this.Stucked && this.Owner.GetMonstersInRange(this.Owner.ActorData.Cylinder.Ax2 * 2).Count >= 2)
			//this.Stucked = false;

			//Vector3D destPoint = this._path.First();
			if (!Stucked && Owner.WalkSpeed > 0f)
			{
				Owner.Move(destPoint, MovementHelpers.GetFacingAngle(Owner, destPoint));
				_lastDelay = (int)(MovementHelpers.GetDistance(Owner.Position, destPoint) / (Owner.WalkSpeed));

				//Logger.Trace("Delay in step: {0}", _lastDelay);
			}
			else
				_lastDelay = 60;

			Timer = TickTimer.WaitTicks(
				Owner.World.Game,
				_lastDelay,
				(tick) =>
				{
					if (Canceled) return;
					if (!Stucked && Owner.WalkSpeed > 0f)
					{
						Owner.Position = MovementHelpers.GetMovementPosition(defaultPosition, Owner.WalkSpeed, MovementHelpers.GetFacingAngle(Owner.Position, destPoint), _lastDelay);
					}

					if (Owner == null ||
						Owner.Attributes[GameAttribute.Hitpoints_Cur] == 0 ||
						Owner.GetObjectsInRange<Player>(60f).Count == 0 ||
						MovementHelpers.GetDistance(Owner.Position, Target.Position) < AttackRadius ||
						(Owner is Monster && (Owner as Monster).Brain.CurrentAction == null)
					)
						Done = true;

					if (!Done)
					{
						Move();
					}
					else
					{
						if (Owner is Monster && Target != null)
						{
							var brain = (Owner as Monster).Brain as MonsterBrain;

							if (PrioritySkillSNO > 0)
							{
								brain.FastAttack(Target, PrioritySkillSNO);
							}
						}
					}
				}
			);

			//this.Done = true;
		}

		public override void Update(int tickCounter)
		{
			if (Timer != null)
				Timer.Update(tickCounter);
		}

		public override void Cancel(int tickCounter)
		{
			Done = true;
			Canceled = true;
			Owner.World.BroadcastIfRevealed(Owner.ACDWorldPositionMessage, Owner);
			//this.Owner.Position = MovementHelpers.GetMovementPosition(this.Owner.Position, this.Owner.WalkSpeed, Movement.MovementHelpers.GetFacingAngle(this.Owner.Position, this.Owner.CurrentDestination), (int)(_lastDelay / 16f));
		}
	}
}
