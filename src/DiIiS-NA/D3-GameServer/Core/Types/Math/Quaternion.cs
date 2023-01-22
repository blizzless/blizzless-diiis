//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;

namespace DiIiS_NA.GameServer.Core.Types.Math
{
	public class Quaternion
	{
		[PersistentProperty("W")]
		public float W { get; set; }
		[PersistentProperty("Vector3D")]
		public Vector3D Vector3D { get; set; }

		public Quaternion() { }

		public Quaternion(Vector3D V, float inW)
		{
			Vector3D = V;
			W = inW;
		}
		/// <summary>
		/// Creates an quaternion that rotates along the Z-axis by the specified "facing" angle. 
		/// </summary>
		/// <param name="facingAngle">The angle in radians.</param>
		/// <returns></returns>
		public static Quaternion FacingRotation(float facingAngle)
		{
			return new Quaternion
			{
				W = (float)System.Math.Cos(facingAngle / 2f),
				Vector3D = new Vector3D(0, 0, (float)System.Math.Sin(facingAngle / 2f))
			};
		}

		/// <summary>
		/// Reads Quaternion from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public Quaternion(MpqFileStream stream)
		{
			Vector3D = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
			W = stream.ReadValueF32();
		}

		/// <summary>
		/// Parses Quaternion from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			W = buffer.ReadFloat32();
			Vector3D = new Vector3D();
			Vector3D.Parse(buffer);
		}

		/// <summary>
		/// Encodes Quaternion to given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			buffer.WriteFloat32(W);
			Vector3D.Encode(buffer);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("Quaternion:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad);
			b.AppendLine("W: " + W.ToString("G"));
			Vector3D.AsText(b, pad);
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}
