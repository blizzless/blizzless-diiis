//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
using System;
using System.Numerics;
using DiIiS_NA.Core.Helpers.Math;

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
		public float DistanceSquared(ref Vector3D point)
		{
			float x = point.X - X, 
				y = point.Y - Y,
				z = point.Z - Z;

			return ((x * x) + (y * y)) + (z * z);
		}

		public static bool operator ==(Vector3D a, Vector3D b)
		{
			if (ReferenceEquals(null, a))
				return ReferenceEquals(null, b);
			return a.Equals(b);
		}

		public static bool operator !=(Vector3D a, Vector3D b)
		{
			return !(a == b);
		}

		public static bool operator >(Vector3D a, Vector3D b)
		{
			if (ReferenceEquals(null, a))
				return !ReferenceEquals(null, b);
			return a.X > b.X
				&& a.Y > b.Y
				&& a.Z > b.Z;
		}

		public static Vector3D operator +(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Vector3D operator -(Vector3D a, Vector3D b)
		{
			return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static bool operator <(Vector3D a, Vector3D b)
		{
			return !(a > b);
		}

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
				return System.Math.Abs(X - v.X) < 0.0001
					&& System.Math.Abs(Y - v.Y) < 0.0001
					&& System.Math.Abs(Z - v.Z) < 0.0001;
			}
			return false;
		}

		public override string ToString() => $"X:{X:F4}, Y:{Y:F4} Z:{Z:F4}";

		public bool IsNear(Vector3D other, float distance) => DistanceSquared(ref other) < distance * distance;

		public bool IsNearSquared(Vector3D other, float distanceSquared) => DistanceSquared(ref other) < distanceSquared;
	}
}
