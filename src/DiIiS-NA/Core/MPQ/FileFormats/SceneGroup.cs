using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using System.Collections.Generic;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
	[FileFormat(SNOGroup.SceneGroup)]
	public class SceneGroup : FileFormat
	{
		public Header Header { get; private set; }
		public int I0 { get; private set; }
		public List<SceneGroupItem> Items { get; private set; }
		public int I1 { get; private set; }

		public SceneGroup(MpqFile file)
		{
			var stream = file.Open();
			Header = new Header(stream);
			I0 = stream.ReadValueS32();
			Items = stream.ReadSerializedData<SceneGroupItem>();
			stream.Position += 8;
			I1 = stream.ReadValueS32();
			stream.Close();
		}
	}

	public class SceneGroupItem : ISerializableData
	{
		public int SNOScene { get; private set; }
		public int I0 { get; private set; }
		public int LabelGBId { get; private set; }

		public void Read(MpqFileStream stream)
		{
			SNOScene = stream.ReadValueS32();
			I0 = stream.ReadValueS32();
			LabelGBId = stream.ReadValueS32();
		}
	}
}
