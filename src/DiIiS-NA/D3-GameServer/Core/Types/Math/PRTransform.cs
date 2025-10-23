using System.Text;
using CrystalMpq;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.Core.Types.Math
{
	public class PRTransform
	{
		[PersistentProperty("Quaternion")]
		public Quaternion Quaternion { get; set; }
		[PersistentProperty("Vector3D")]
		public Vector3D Vector3D { get; set; }

		public PRTransform() { }

		public PRTransform(Quaternion Q, Vector3D V)
		{
			Quaternion = Q;
			Vector3D = V;
		}
		/// <summary>
		/// Reads PRTransform from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public PRTransform(MpqFileStream stream)
		{
			Quaternion = new Quaternion(stream);
			Vector3D = new Vector3D(stream);
		}

		/// <summary>
		/// Reads PRTransform from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			Quaternion = new Quaternion();
			Quaternion.Parse(buffer);
			Vector3D = new Vector3D();
			Vector3D.Parse(buffer);
		}

		/// <summary>
		/// Encodes PRTransform to given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			Quaternion.Encode(buffer);
			Vector3D.Encode(buffer);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("PRTransform:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			Quaternion.AsText(b, pad);
			Vector3D.AsText(b, pad);
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}
