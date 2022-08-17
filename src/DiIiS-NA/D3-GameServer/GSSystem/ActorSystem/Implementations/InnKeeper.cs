//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		 //act 1
		 109467,
		 //act 2
		 180291,
		 //act 3
		 181473,
		 //act 4
		 182413,
		 //act 5
		 309718
		 )]
	public class Innkeeper : Vendor
	{
		public Innkeeper(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}

		protected override List<ItemsSystem.Item> GetVendorItems()
		{
			var list = new List<ItemsSystem.Item>
			{
                ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("HealthPotionBottomless")), 1, false) //HealthPotionConsole
			};

			return list;
		}
	}
}
