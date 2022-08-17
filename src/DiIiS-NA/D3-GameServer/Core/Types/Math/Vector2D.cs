//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using System.Text;

namespace DiIiS_NA.GameServer.Core.Types.Math
{
	public class Vector2D
	{
		[PersistentProperty("X")]
		public int X { get; set; }

		[PersistentProperty("Y")]
		public int Y { get; set; }

		public Vector2D() { }

		/// <summary>
		/// Reads Vector2D from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public Vector2D(MpqFileStream stream)
		{
			X = stream.ReadValueS32();
			Y = stream.ReadValueS32();
		}

		public Vector2D(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Parses Vector2D from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			X = buffer.ReadInt(32);
			Y = buffer.ReadInt(32);
		}

		/// <summary>
		/// Encodes Vector2D to given GameBitBuffer.
		/// </summary>		
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, X);
			buffer.WriteInt(32, Y);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("Vector2D:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad);
			b.AppendLine("X: 0x" + X.ToString("X8") + " (" + X + ")");
			b.Append(' ', pad);
			b.AppendLine("Y: 0x" + Y.ToString("X8") + " (" + Y + ")");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}

		public override string ToString()
		{
			return string.Format("x:{0} y:{1}", X, Y);
		}
	}
}
