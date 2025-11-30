using System.Text;
using CrystalMpq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.MessageSystem;
using Gibbed.IO;
using DiIiS_NA.Core.Storage;
using System;
using System.Numerics;
using DiIiS_NA.Core.Helpers.Math;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;

namespace DiIiS_NA.GameServer.Core.Types.Math
{
	public class Vector3D : ISerializableData
	{
		[PersistentProperty("X")]
		public float X { get; set; }
		[PersistentProperty("Y")]
		public float Y { get; set; }
		[PersistentProperty("Z")]
		public float Z { get; set; }

		public Vector3D()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}

		public Vector3D(Vector3D vector)
		{
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
		}

		public Vector3D(float x, float y, float z)
		{
			Set(x, y, z);
		}

		/// <summary>
		/// Reads Vector3D from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public Vector3D(MpqFileStream stream)
			: this(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32())
		{
		}

		public void Read(MpqFileStream stream)
		{
			X = stream.ReadValueF32();
			Y = stream.ReadValueF32();
			Z = stream.ReadValueF32();
		}

		/// <summary>
		/// Parses Vector3D from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			X = buffer.ReadFloat32();
			Y = buffer.ReadFloat32();
			Z = buffer.ReadFloat32();
		}

		/// <summary>
		/// Encodes Vector3D to given GameBitBuffer.
		/// </summary>		
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			buffer.WriteFloat32(X);
			buffer.WriteFloat32(Y);
			buffer.WriteFloat32(Z);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("Vector3D:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad);
			b.AppendLine("X: " + X.ToString("G"));
			b.Append(' ', pad);
			b.AppendLine("Y: " + Y.ToString("G"));
			b.Append(' ', pad);
			b.AppendLine("Z: " + Z.ToString("G"));
			b.Append(' ', --pad);
			b.AppendLine("}");
		}

		public void Set(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Calculates the distance squared from this vector to another.
		/// </summary>
		/// <param name="point">the second <see cref="Vector3" /></param>
		/// <returns>the distance squared between the vectors</returns>
		public float DistanceSquared(ref Vector3D point) // todo: remove ref
		{
			float x = point.X - X, 
				y = point.Y - Y,
				z = point.Z - Z;

			return ((x * x) + (y * y)) + (z * z);
		}

		public static double Distance(Vector3D vector1, Vector3D vector2)
		{
			return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z));
		}

		public static bool IsInDistanceSquared(Vector3D position, Vector3D relative, double distanceMax, double distanceMin = -1f)
		{
			var dist = Distance(position, relative);
			return dist < distanceMax && dist > distanceMin;
		}

		private static Random rand = new Random();

		public Vector3D Around(float radius)
		{
			return Around(radius, radius, radius);
		}
		public Vector3D Around(float x, float y, float z)
		{
			float newX = X + ((float)rand.NextDouble() * 2 * x) - x;
			float newY = Y + ((float)rand.NextDouble() * 2 * y) - y;
			float newZ = Z + ((float)rand.NextDouble() * 2 * z) - z;
			return new Vector3D(newX, newY, newZ);
		}

		public Vector3D Around(Vector3D vector)
		{
			return Around(vector.X, vector.Y, vector.Z);
		}

		public static bool operator ==(Vector3D a, Vector3D b) => a?.Equals(b) ?? ReferenceEquals(null, b);

		public static bool operator !=(Vector3D a, Vector3D b) => !(a == b);

		public static bool operator >(Vector3D a, Vector3D b)
		{
			return ReferenceEquals(null, a)
				? !ReferenceEquals(null, b)
				: a.X > b.X
				  && a.Y > b.Y
				  && a.Z > b.Z;
		}

		public static Vector3D operator +(Vector3D a, Vector3D b) => new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static Vector3D operator -(Vector3D a, Vector3D b) => new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static bool operator <(Vector3D a, Vector3D b) => !(a > b);

		public static bool operator >=(Vector3D a, Vector3D b)
		{
			if (ReferenceEquals(null, a))
				return ReferenceEquals(null, b);
			return a.X >= b.X
				&& a.Y >= b.Y
				&& a.Z >= b.Z;
		}

		public static bool operator <=(Vector3D a, Vector3D b)
		{
			if (ReferenceEquals(null, a))
				return ReferenceEquals(null, b);
			return a.X <= b.X
				&& a.Y <= b.Y
				&& a.Z <= b.Z;
		}

		public override bool Equals(object o)
		{
			if (ReferenceEquals(this, o))
				return true;
			var v = o as Vector3D;
			if (v != null)
			{
				return System.Math.Abs(X - v.X) < Globals.FLOAT_TOLERANCE
				       && System.Math.Abs(Y - v.Y) < Globals.FLOAT_TOLERANCE
				       && System.Math.Abs(Z - v.Z) < Globals.FLOAT_TOLERANCE;
			}
			return false;
		}

		public override string ToString() => $"X:{X:F4}, Y:{Y:F4} Z:{Z:F4}";

		public bool IsNear(Vector3D other, float distance) => DistanceSquared(ref other) < distance;

        public override int GetHashCode() => HashCode.Combine(X.ToDouble(decimals: 6), Y.ToDouble(decimals: 6), Z.ToDouble(decimals: 6));
    }

	public static class VectorExtensions
	{
        /// <summary>
        /// Takes all actors from the given collection that are within the specified distance of the reference position.
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actors"></param>
        /// <param name="referencePosition"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static TSearch[] WhereNearbyOf<TActor, TSearch>(
			this IEnumerable<TActor> actors,
			TSearch[] referenceActors,
            Func<TSearch, bool> query,
            double maxDistance = 50f,
            double minDistance = 1f)
		where TActor : Actor
        where TSearch : Actor
        {
            return actors.OfType<TSearch>()
                    .Where(actor => query(actor))
                    .Where((actor, dist) =>
                        referenceActors.Any(refActor => refActor.GlobalID != actor.GlobalID && Vector3D.IsInDistanceSquared(refActor.Position, actor.Position, maxDistance, minDistance)))
                    .ToArray();
        }
    }
}
