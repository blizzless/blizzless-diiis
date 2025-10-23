using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.SNO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Recipe)]
    public class Recipe : FileFormat//RecipeDefinition_fields
    {
        public Header Header { get; private set; }
        public ItemSpecifierData ItemSpecifierData { get; private set; }

        public Recipe(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            ItemSpecifierData = new ItemSpecifierData(stream);
            stream.Close();
        }
    }
}
