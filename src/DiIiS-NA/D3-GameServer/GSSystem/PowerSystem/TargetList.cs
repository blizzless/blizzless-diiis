//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class TargetList
	{
		// list of actors that are the primary targets
		public List<Actor> Actors { get; set; }

		// list of extra actors that are near the targets, i.e. destructables like barrels, tombstones etc.
		public List<Actor> ExtraActors { get; private set; }

		public TargetList()
		{
			this.Actors = new List<Actor>();
			this.ExtraActors = new List<Actor>();
		}

		public void SortByDistanceFrom(Vector3D position)
		{
			this.Actors = this.Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, position)).ToList();
		}

		public TargetList FilterByType<T>()
		{
			this.Actors = this.Actors.Where(actor => actor is T).ToList();
			return this;
		}

		public TargetList Distinct()
		{
			this.Actors = this.Actors.Distinct().ToList();
			return this;
		}

		public Actor GetClosestTo(Vector3D position)
		{
			Actor closest = null;
			float closestDistance = float.MaxValue;
			foreach (Actor actor in this.Actors)
			{
				float distance = PowerMath.Distance2D(actor.Position, position);
				if (distance < closestDistance)
				{
					closest = actor;
					closestDistance = distance;
				}
			}

			return closest;
		}
	}
}
