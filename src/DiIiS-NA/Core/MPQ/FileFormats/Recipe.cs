//Blizzless Project 2022
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;
//Blizzless Project 2022 
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
            this.Header = new Header(stream);
            ItemSpecifierData = new ItemSpecifierData(stream);
            stream.Close();
        }
    }
}
