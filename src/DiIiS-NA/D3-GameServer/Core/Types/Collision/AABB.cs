//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;

namespace DiIiS_NA.GameServer.Core.Types.Collision
{
	public class AABB
	{
		[PersistentProperty("Min")]
		public Vector3D Min { get; set; }
		[PersistentProperty("Max")]
		public Vector3D Max { get; set; }

		public AABB()
		{
		}

		public AABB(Vector3D Mi, Vector3D Ma)
		{
			Min = Mi;
			Max = Ma;
		}

		/// <summary>
		/// Reads AABB from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public AABB(MpqFileStream stream)
		{
			Min = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
			Max = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
		}

		/// <summary>
		/// Parses AABB from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			Min = new Vector3D();
			Min.Parse(buffer);
			Max = new Vector3D();
			Max.Parse(buffer);
		}

		/// <summary>
		/// Encodes AABB to given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			Min.Encode(buffer);
			Max.Encode(buffer);
		}

		public bool IsWithin(Vector3D v)
		{
			if (v >= Min &&
				v <= Max)
			{
				return true;
			}
			return false;
		}

		public bool Intersects(AABB other)
		{
			if (// Max < o.Min
				Max.X < other.Min.X ||
				Max.Y < other.Min.Y ||
				Max.Z < other.Min.Z ||
				// Min > o.Max
				Min.X > other.Max.X ||
				Min.Y > other.Max.Y ||
				Min.Z > other.Max.Z)
			{
				return false;
			}
			return true; // Intersects if above fails
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("AABB:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			Min.AsText(b, pad);
			Max.AsText(b, pad);
			b.Append(' ', --pad);
			b.AppendLine("}");
		}

		public override string ToString()
		{
			return string.Format("AABB: min:{0} max:{1}", Min, Max);
		}
	}
}
