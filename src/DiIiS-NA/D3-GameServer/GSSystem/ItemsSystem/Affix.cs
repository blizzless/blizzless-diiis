//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
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
					if (data != null && data.Type == BalanceType.AffixList)
					{
						foreach (var affixDefinition in data.Affixes)
						{
							if (affixDefinition.Hash == this.AffixGbid) return affixDefinition;
						}
					}
				}
				return null;
			}
		}

		public int Price
		{
			get
			{
				return (this.Definition == null ? 0 : this.Definition.Cost);
			}
		}

		public int ItemLevel
		{
			get
			{
				return (this.Definition == null ? 0 : this.Definition.AffixLevel);
			}
		}

		public float Score = 0f;

		public int Rating
		{
			get
			{
				return (int)(this.Price * (1 + this.Score));
			}
			set { }
		}

		public Affix(int gbid)
		{
			AffixGbid = gbid;
		}

		public override String ToString()
		{
			return String.Format("{0}", AffixGbid);
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
