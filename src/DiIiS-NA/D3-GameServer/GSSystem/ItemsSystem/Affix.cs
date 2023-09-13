using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public class Affix
	{
		public static readonly Logger Logger = LogManager.CreateLogger();
		public int AffixGbid { get; set; }

		public AffixTable Definition
		{
			get
			{
				foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
				{
					GameBalance data = asset.Data as GameBalance;
					if (data is not { Type: BalanceType.AffixList }) continue;
					foreach (AffixTable affixDefinition in data.Affixes.Where(affixDefinition => affixDefinition.Hash == AffixGbid))
					{
						return affixDefinition;
					}
				}
				return null;
			}
		}

		public int Price => (Definition == null ? 0 : Definition.Cost);

		public int ItemLevel => (Definition == null ? 0 : Definition.AffixLevel);

		public float Score = 0f;

		public int Rating
		{
			get => (int)(Price * (1 + Score));
			set { }
		}

		public Affix(int gbid)
		{
			AffixGbid = gbid;
		}

		public override String ToString()
		{
			return AffixGbid.ToString();
		}

		public static Affix Parse(String affixString)
		{
			try
			{
				int gbid = int.Parse(affixString);
				var affix = new Affix(gbid);
				return affix;
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("Affix can not be parsed: {0}", affixString), e);
			}
		}

	}
}
