using DiIiS_NA.GameServer.Core.Types.Math;
using System.Drawing;

namespace DiIiS_NA.GameServer.Core.Types.Misc
{
	public struct Circle
	{
		/// <summary> 
		/// Center position of the circle. 
		/// </summary> 
		public Vector2F Center;

		/// <summary> 
		/// Radius of the circle. 
		/// </summary> 
		public float Radius;

		/// <summary> 
		/// Constructs a new circle. 
		/// </summary> 
		public Circle(Vector2F position, float radius)
		{
			Center = position;
			Radius = radius;
		}

		/// <summary> 
		/// Constructs a new circle. 
		/// </summary> 
		public Circle(float x, float y, float radius)
			: this(new Vector2F(x, y), radius)
		{ }

		/// <summary> 
		/// Determines if a circle intersects a rectangle. 
		/// </summary> 
		/// <returns>True if the circle and rectangle overlap. False otherwise.</returns> 
		public bool Intersects(Rectangle rectangle)
		{
			// Find the closest point to the circle within the rectangle
			float closestX = Clamp(Center.X, (float)rectangle.Left, (float)rectangle.Right);
			float closestY = Clamp(Center.Y, (float)rectangle.Top, (float)rectangle.Bottom);

			// Calculate the distance between the circle's center and this closest point
			float distanceX = Center.X - closestX;
			float distanceY = Center.Y - closestY;

			// If the distance is less than the circle's radius, an intersection occurs
			float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
			return distanceSquared < (Radius * Radius);
		}

		public bool Intersects(RectangleF rectangle)
		{
			// Find the closest point to the circle within the rectangle
			float closestX = Clamp(Center.X, (float)rectangle.Left, (float)rectangle.Right);
			float closestY = Clamp(Center.Y, (float)rectangle.Top, (float)rectangle.Bottom);

			// Calculate the distance between the circle's center and this closest point
			float distanceX = Center.X - closestX;
			float distanceY = Center.Y - closestY;

			// If the distance is less than the circle's radius, an intersection occurs
			float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
			return distanceSquared < (Radius * Radius);
		}

		public static float Clamp(float value, float min, float max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}
	}
}
