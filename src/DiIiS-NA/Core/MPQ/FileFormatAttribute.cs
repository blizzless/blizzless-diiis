using DiIiS_NA.GameServer.Core.Types.SNO;
using System;

namespace DiIiS_NA.Core.MPQ
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FileFormatAttribute : Attribute
	{
		public SNOGroup Group { get; private set; }

		public FileFormatAttribute(SNOGroup group)
		{
			this.Group = group;
		}
	}
}
