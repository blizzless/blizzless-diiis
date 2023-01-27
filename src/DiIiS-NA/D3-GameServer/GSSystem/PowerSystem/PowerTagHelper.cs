using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class PowerTagHelper
	{
		public static TagKeyScript GenerateTagForScriptFormula(int SF_N)
		{
			return new TagKeyScript(266496 + 256 * (SF_N / 10) + 16 * (SF_N % 10));
		}

		public static TagMap FindTagMapWithKey(int powerSNO, TagKey key)
		{
			if (powerSNO <= 0) return null;
			Power power = (Power)MPQStorage.Data.Assets[SNOGroup.Power][powerSNO].Data;

			// TODO: figure out which tagmaps to search and in what order, eventually will probably
			// have to reorder them based on whether PvPing or not.
			TagMap[] tagMaps = new TagMap[]
			{
				power.Powerdef.GeneralTagMap,
				power.Powerdef.TagMap,
				power.Powerdef.ContactTagMap0,
				power.Powerdef.ContactTagMap1,
				power.Powerdef.ContactTagMap2,
				power.Powerdef.ContactTagMap3,
				power.Powerdef.PVPGeneralTagMap,
				power.Powerdef.PVPContactTagMap0,
				power.Powerdef.PVPContactTagMap1,
				power.Powerdef.PVPContactTagMap2,
				power.Powerdef.PVPContactTagMap3,
			};

			foreach (TagMap tagmap in tagMaps)
			{
				if (tagmap.ContainsKey(key))
					return tagmap;
			}

			return null;
		}
	}
}
