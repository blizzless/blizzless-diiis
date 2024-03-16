using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using System.Collections.Generic;
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
			Actors = new List<Actor>();
			ExtraActors = new List<Actor>();
		}

		public void SortByDistanceFrom(Vector3D position)
		{
			Actors = Actors.OrderBy(actor => PowerMath.Distance2D(actor.Position, position)).ToList();
		}

		public TargetList FilterByType<T>()
		{
			Actors = Actors.Where(actor => actor is T).ToList();
			return this;
		}

		public TargetList Distinct()
		{
			Actors = Actors.Distinct().ToList();
			return this;
		}

		public Actor GetClosestTo(Vector3D position)
		{
			Actor closest = null;
			float closestDistance = float.MaxValue;
			foreach (Actor actor in Actors)
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
