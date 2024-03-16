using CrystalMpq;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.MessageSystem;
using Gibbed.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.Core.Types.Scene
{
	public class SceneSpecification
	{
		[PersistentProperty("CellZ")]
		public int CellZ { get; set; } // Position.Z rounded down
		[PersistentProperty("Cell")]
		public Vector2D Cell { get; set; }
		[PersistentProperty("SNOLevelAreas", 4)]
		public int[] SNOLevelAreas { get; set; } // Area names - MaxLength = 4
		[PersistentProperty("SNOPrevWorld")]
		public int SNOPrevWorld { get; set; }
		[PersistentProperty("Unknown1")] //PrevStartPointTag
		public int PrevEntranceGUID { get; set; }
		[PersistentProperty("SNOPrevLevelArea")]
		public int SNOPrevLevelArea { get; set; }
		[PersistentProperty("SNONextWorld")]
		public int SNONextWorld { get; set; }
		[PersistentProperty("Unknown2")] //PrevNextPointTag
		public int NextEntranceGUID { get; set; }
		[PersistentProperty("SNONextLevelArea")]
		public int SNONextLevelArea { get; set; }
		[PersistentProperty("SNOMusic")]
		public int SNOMusic { get; set; }
		[PersistentProperty("SNOCombatMusic")]
		public int SNOCombatMusic { get; set; }
		[PersistentProperty("SNOAmbient")]
		public int SNOAmbient { get; set; }
		[PersistentProperty("SNOReverb")]
		public int SNOReverb { get; set; }
		[PersistentProperty("SNOWeather")]
		public int SNOWeather { get; set; }
		[PersistentProperty("SNOPresetWorld")]
		public int SNOPresetWorld { get; set; }
		[PersistentProperty("Unknown3")]
		public int DRLGIndex { get; set; }
		[PersistentProperty("Unknown4")]
		public int SceneChunk { get; set; }
		[PersistentProperty("Unknown5")]
		public int OnPathBits { get; set; }
		[PersistentProperty("ClusterID")]
		public int ClusterID { get; set; }
		[PersistentProperty("SceneCachedValues")]
		public SceneCachedValues SceneCachedValues { get; set; }

		public SceneSpecification() { }

		public SceneSpecification(int CZ, Vector2D C, int[] SNOLA, int SNOPWorld, int U1, int SNOPLA, int SNONWorld, int U2,
								 int SNONLA, int SNOMus, int SNOCombatMus, int Ambi, int Rev, int Weath, int SNOPresWorld, int U3, int U4, int U5, int ClustID, SceneCachedValues SCV)
		{
			CellZ = CZ;
			Cell = C;
			SNOLevelAreas = SNOLA;
			SNOPrevWorld = SNOPWorld;
			PrevEntranceGUID = U1;
			NextEntranceGUID = U2;
			SNOPrevLevelArea = SNOPLA;
			SNONextWorld = SNONWorld;
			SNONextLevelArea = SNONLA;
			SNOMusic = SNOMus;
			SNOAmbient = Ambi;
			SNOReverb = Rev;
			SNOWeather = Weath;
			SNOPresetWorld = SNOPresWorld;
			DRLGIndex = U3;
			SceneChunk = U4;
			OnPathBits = U5;
			ClusterID = ClustID;
			SceneCachedValues = SCV;
		}

		/// <summary>
		/// Reads SceneSpecification from given MPQFileStream.
		/// </summary>
		/// <param name="stream">The MPQFileStream to read from.</param>
		public SceneSpecification(MpqFileStream stream)
		{
			CellZ = stream.ReadValueS32();
			Cell = new Vector2D(stream);
			SNOLevelAreas = new int[4];

			for (int i = 0; i < SNOLevelAreas.Length; i++)
			{
				SNOLevelAreas[i] = stream.ReadValueS32();
			}

			SNOPrevWorld = stream.ReadValueS32();
			PrevEntranceGUID = stream.ReadValueS32();
			SNOPrevLevelArea = stream.ReadValueS32();
			SNONextWorld = stream.ReadValueS32();
			NextEntranceGUID = stream.ReadValueS32();
			SNONextLevelArea = stream.ReadValueS32();
			SNOMusic = stream.ReadValueS32();
			SNOCombatMusic = stream.ReadValueS32();
			SNOAmbient = stream.ReadValueS32();
			SNOReverb = stream.ReadValueS32();
			SNOWeather = stream.ReadValueS32();
			SNOPresetWorld = stream.ReadValueS32();
			DRLGIndex = stream.ReadValueS32();
			SceneChunk = stream.ReadValueS32();
			OnPathBits = stream.ReadValueS32();
			stream.Position += (9 * 4);
			ClusterID = stream.ReadValueS32();
			SceneCachedValues = new SceneCachedValues(stream);
		}

		/// <summary>
		/// Parses SceneSpecification from given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to parse from.</param>
		public void Parse(GameBitBuffer buffer)
		{
			CellZ = buffer.ReadInt(32);
			Cell = new Vector2D();
			Cell.Parse(buffer);
			SNOLevelAreas = new int /* sno */[4];
			for (int i = 0; i < SNOLevelAreas.Length; i++) SNOLevelAreas[i] = buffer.ReadInt(32);
			SNOPrevWorld = buffer.ReadInt(32);
			PrevEntranceGUID = buffer.ReadInt(32);
			SNOPrevLevelArea = buffer.ReadInt(32);
			SNONextWorld = buffer.ReadInt(32);
			NextEntranceGUID = buffer.ReadInt(32);
			SNONextLevelArea = buffer.ReadInt(32);
			SNOMusic = buffer.ReadInt(32);
			SNOAmbient = buffer.ReadInt(32);
			SNOReverb = buffer.ReadInt(32);
			SNOWeather = buffer.ReadInt(32);
			SNOPresetWorld = buffer.ReadInt(32);
			DRLGIndex = buffer.ReadInt(32);
			SceneChunk = buffer.ReadInt(32);
			OnPathBits = buffer.ReadInt(32);
			ClusterID = buffer.ReadInt(32);
			SceneCachedValues = new SceneCachedValues();
			SceneCachedValues.Parse(buffer);
		}

		/// <summary>
		/// Encodes SceneSpecification to given GameBitBuffer.
		/// </summary>
		/// <param name="buffer">The GameBitBuffer to write.</param>
		public void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, CellZ);
			Cell.Encode(buffer);
			for (int i = 0; i < SNOLevelAreas.Length; i++) buffer.WriteInt(32, SNOLevelAreas[i]);
			buffer.WriteInt(32, SNOPrevWorld);
			buffer.WriteInt(32, PrevEntranceGUID);
			buffer.WriteInt(32, SNOPrevLevelArea);
			buffer.WriteInt(32, SNONextWorld);
			buffer.WriteInt(32, NextEntranceGUID);
			buffer.WriteInt(32, SNONextLevelArea);
			buffer.WriteInt(32, SNOMusic);
			buffer.WriteInt(32, SNOAmbient);
			buffer.WriteInt(32, SNOReverb);
			buffer.WriteInt(32, SNOWeather);
			buffer.WriteInt(32, SNOPresetWorld);
			buffer.WriteInt(32, DRLGIndex);
			buffer.WriteInt(32, SceneChunk);
			buffer.WriteInt(32, OnPathBits);
			buffer.WriteInt(32, ClusterID);
			SceneCachedValues.Encode(buffer);
		}

		public void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("SceneSpecification:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad);
			b.AppendLine("CellZ: 0x" + CellZ.ToString("X8") + " (" + CellZ + ")");
			Cell.AsText(b, pad);
			b.Append(' ', pad);
			b.AppendLine("arSnoLevelAreas:");
			b.Append(' ', pad);
			b.AppendLine("{");
			for (int i = 0; i < SNOLevelAreas.Length;)
			{
				b.Append(' ', pad + 1);
				for (int j = 0; j < 8 && i < SNOLevelAreas.Length; j++, i++)
				{
					b.Append("0x" + SNOLevelAreas[i].ToString("X8") + ", ");
				}
				b.AppendLine();
			}
			b.Append(' ', pad);
			b.AppendLine("}");
			b.AppendLine();
			b.Append(' ', pad);
			b.AppendLine("snoPrevWorld: 0x" + SNOPrevWorld.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("PrevEntranceGUID: 0x" + PrevEntranceGUID.ToString("X8") + " (" + PrevEntranceGUID + ")");
			b.Append(' ', pad);
			b.AppendLine("snoPrevLevelArea: 0x" + SNOPrevLevelArea.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoNextWorld: 0x" + SNONextWorld.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("NextEntranceGUID: 0x" + NextEntranceGUID.ToString("X8") + " (" + NextEntranceGUID + ")");
			b.Append(' ', pad);
			b.AppendLine("snoNextLevelArea: 0x" + SNONextLevelArea.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoMusic: 0x" + SNOMusic.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoAmbient: 0x" + SNOAmbient.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoReverb: 0x" + SNOReverb.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoWeather: 0x" + SNOWeather.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("snoPresetWorld: 0x" + SNOPresetWorld.ToString("X8"));
			b.Append(' ', pad);
			b.AppendLine("DRLGIndex: 0x" + DRLGIndex.ToString("X8") + " (" + DRLGIndex + ")");
			b.Append(' ', pad);
			b.AppendLine("SceneChunk: 0x" + SceneChunk.ToString("X8") + " (" + SceneChunk + ")");
			b.Append(' ', pad);
			b.AppendLine("OnPathBits: 0x" + OnPathBits.ToString("X8") + " (" + OnPathBits + ")");
			b.Append(' ', pad);
			b.AppendLine("ClusterId: 0x" + ClusterID.ToString("X8") + " (" + ClusterID + ")");
			SceneCachedValues.AsText(b, pad);
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}
