using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.SNO;
using System;
using System.Globalization;
using System.Threading;

namespace DiIiS_NA.Core.MPQ
{
	public class DBAsset : Asset
	{

		protected override bool SourceAvailable
		{
			get { return true; }
		}

		public DBAsset(SNOGroup group, Int32 snoId, string name)
			: base(group, snoId, name)
		{
		}

		public override void RunParser()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // Use invariant culture so that we don't hit pitfalls in non en/US systems with different number formats.
			_data = (FileFormat)PersistenceManager.Load(Parser, SNOId.ToString());
		}
	}
}
