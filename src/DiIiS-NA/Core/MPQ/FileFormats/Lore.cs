//Blizzless Project 2022
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Lore)]
    public class Lore : FileFormat
    {
        public Header Header { get; private set; }
        public int I0 { get; private set; }
        public LoreCategory Category { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int SNOConversation { get; private set; }
        public int I3 { get; private set; }

        public Lore(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.I0 = stream.ReadValueS32();
            this.Category = (LoreCategory)stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.SNOConversation = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
            stream.Close();
        }
    }
    public enum LoreCategory
    {
        Quest = 0,
        World,
        People,
        Bestiary,
    };
}
