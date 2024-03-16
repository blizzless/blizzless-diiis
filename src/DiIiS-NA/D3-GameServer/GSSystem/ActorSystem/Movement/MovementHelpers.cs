using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Misc;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement
{
	public static class MovementHelpers
	{
		/// <summary>
		/// Returns 2D angle to face the target position.
		/// </summary>
		/// <param name="lookerPosition">The looker.</param>
		/// <param name="targetPosition">The target.</param>
		/// <returns></returns>
		public static float GetFacingAngle(Vector3D lookerPosition, Vector3D targetPosition)
		{
			if ((lookerPosition == null) || (targetPosition == null))
				return 0f;

			float a = (float)Math.Atan2((targetPosition.Y - lookerPosition.Y), (targetPosition.X - lookerPosition.X));
			if (a < 0f)
				a += (float)Math.PI * 2f;

			return a;
		}

		public static float GetFacingAngle(Actor looker, Actor target)
		{
			return GetFacingAngle(looker.Position, target.Position);
		}

		public static float GetFacingAngle(Actor looker, Vector2F targetPosition)
		{
			return GetFacingAngle(looker.Position, new Vector3D(targetPosition.X, targetPosition.Y, 0));
		}

		public static float GetFacingAngle(Actor looker, Vector3D targetPosition)
		{
			if (looker == null)
				return GetFacingAngle(new Vector3D(0f, 0f, 0f), targetPosition);
			else
				return GetFacingAngle(looker.Position, targetPosition);
		}

		public static float GetDistance(Vector3D startPosition, Vector3D targetPosition)
		{
			if ((startPosition == null) || (targetPosition == null)) return 0;
			return (float)Math.Sqrt(Math.Pow(startPosition.X - targetPosition.X, 2) + Math.Pow(startPosition.Y - targetPosition.Y, 2));
		}

		public static Vector3D GetMovementPosition(Vector3D position, float speed, float facingAngle, int ticks = 6)
		{
			var xDelta = (speed * ticks) * (float)Math.Cos(facingAngle);
			var yDelta = (speed * ticks) * (float)Math.Sin(facingAngle);

			return new Vector3D(position.X + xDelta, position.Y + yDelta, position.Z);
		}

		public static List<Actor> GetUnitsOnPath(MapSystem.World world, Vector3D startPoint, Vector3D direction, float length, float thickness = 0f)
		{
			Vector3D beamEnd = PowerMath.TranslateDirection2D(startPoint, direction, startPoint, length);

			List<Actor> units = new List<Actor>();

			foreach (Actor actor in world.QuadTree.Query<Actor>(new Circle(startPoint.X, startPoint.Y, length + thickness + 25f)))
			{
				if (PowerMath.CircleInBeam(new Circle(actor.Position.X, actor.Position.Y, actor.ActorData.Cylinder.Ax2), startPoint, beamEnd, thickness) &&
					!world.PowerManager.IsDeletingActor(actor) &&
					(actor is Monster) &&
					(PowerMath.Distance2D(startPoint, actor.Position) - (actor.ActorData.Cylinder.Ax2 + thickness)) <= length + thickness)
				{
					units.Add(actor);
				}
			}

			return units;
		}

		public static Vector3D GetCorrectPosition(Vector3D startPosition, Vector3D endPosition, MapSystem.World world, float divider = 6.0f)
		{
			float distance = PowerMath.Distance2D(startPosition, endPosition);
			Vector3D destination = startPosition;
			for (int i = 1; i <= divider; i++)
			{
				var point = PowerMath.TranslateDirection2D(startPosition, endPosition, startPosition, (distance * (float)i) / divider);
				if (world.CheckLocationForFlag(point, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				{
					destination = point;
				}
				else
					return destination;
			}

			return destination;
		}
	}
}
