//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Collision;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using Gibbed.IO;

namespace DiIiS_NA.GameServer.Core.Types.Scene
{
	public class SceneCachedValues
	{
		[PersistentProperty("Unknown1")]
		public int CachedValuesValid { get; set; }
		[PersistentProperty("Unknown2")]
		public int NavMeshSizeX { get; set; }
		[PersistentProperty("Unknown3")]
		public int NavMeshSizeY { get; set; }
		[PersistentProperty("AABB1")]
		public AABB AABB1 { get; set; }
		[PersistentProperty("AABB2")]
		public AABB AABB2 { get; set; }
		[PersistentProperty("Unknown4", 4)]
		public int[] Unknown4 { get; set; } // MaxLength = 4
		[PersistentProperty("Unknown5")]
		public int Unknown5 { get; set; }

		public SceneCachedValues() { }

		public SceneCachedValues(int U1, int U2, int U3, AABB AB1, AABB AB2, int[] U4, int U5)
		{
			CachedValuesValid = U1;
			NavMeshSizeX = U2;
			NavMeshSizeY = U3;
			AABB1 = AB1;
			AABB2 = AB2;
			Unknown4 = U4;
			Unknown5 = U5;
		}
		/// <summary>
		/// Reads SceneCachedValues from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public SceneCachedValues(MpqFileStream stream)
		{
			CachedValuesValid = stream.ReadValueS32();
			NavMeshSizeX = stream.ReadValueS32();
			NavMeshSizeY = stream.ReadValueS32();
			AABB1 = new AABB(stream);
			AABB2 = new AABB(stream);
			Unknown4 = new int[4];
			for (int i = 0; i < Unknown4.Length; i++)
			{
				Unknown4[i] = stream.ReadValueS32();
			}
			Unknown5 = stream.ReadValueS32();
		}

		/// <summary>
		/// Parses SceneCachedValues from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			CachedValuesValid = buffer.ReadInt(32);
			NavMeshSizeX = buffer.ReadInt(32);
			NavMeshSizeY = buffer.ReadInt(32);
			AABB1 = new AABB();
			AABB1.Parse(buffer);
			AABB2 = new AABB();
			AABB2.Parse(buffer);
			Unknown4 = new int[4];
			for (int i = 0; i < Unknown4.Length; i++) Unknown4[i] = buffer.ReadInt(32);
			Unknown5 = buffer.ReadInt(32);
		}

		/// <summary>
		/// Encodes SceneCachedValues to given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, CachedValuesValid);
			buffer.WriteInt(32, NavMeshSizeX);
			buffer.WriteInt(32, NavMeshSizeY);
			AABB1.Encode(buffer);
			AABB2.Encode(buffer);
			for (int i = 0; i < Unknown4.Length; i++) buffer.WriteInt(32, Unknown4[i]);
			buffer.WriteInt(32, Unknown5);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("SceneCachedValues:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad);
			b.AppendLine("CachedValuesValid: 0x" + CachedValuesValid.ToString("X8") + " (" + CachedValuesValid + ")");
			b.Append(' ', pad);
			b.AppendLine("NavMeshSizeX: 0x" + NavMeshSizeX.ToString("X8") + " (" + NavMeshSizeX + ")");
			b.Append(' ', pad);
			b.AppendLine("NavMeshSizeY: 0x" + NavMeshSizeY.ToString("X8") + " (" + NavMeshSizeY + ")");
			AABB1.AsText(b, pad);
			AABB2.AsText(b, pad);
			b.Append(' ', pad);
			b.AppendLine("Unknown4:");
			b.Append(' ', pad);
			b.AppendLine("{");
			for (int i = 0; i < Unknown4.Length;)
			{
				b.Append(' ', pad + 1);
				for (int j = 0; j < 8 && i < Unknown4.Length; j++, i++)
				{
					b.Append("0x" + Unknown4[i].ToString("X8") + ", ");
				}
				b.AppendLine();
			}
			b.Append(' ', pad);
			b.AppendLine("}");
			b.AppendLine();
			b.Append(' ', pad);
			b.AppendLine("Unknown5: 0x" + Unknown5.ToString("X8") + " (" + Unknown5 + ")");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}


	}
}
