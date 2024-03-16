using System;
using System.Globalization;
using System.Threading;
using CrystalMpq;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.SNO;

namespace DiIiS_NA.Core.MPQ
{
    public class MPQAsset : Asset
    {
        public MpqFile MpqFile { get; set; }

        protected override bool SourceAvailable
        {
            get { return MpqFile != null && MpqFile.Size != 0; }
        }

        public MPQAsset(SNOGroup group, Int32 snoId, string name)
            : base(group, snoId, name)
        {

        }

        public override void RunParser()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // Use invariant culture so that we don't hit pitfalls in non en/US systems with different number formats.
            _data = (FileFormat)Activator.CreateInstance(Parser, new object[] { MpqFile });
            PersistenceManager.LoadPartial(_data, SNOId.ToString());
        }
    }
}
