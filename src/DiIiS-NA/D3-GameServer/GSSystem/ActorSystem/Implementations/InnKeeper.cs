//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
		 ActorSno._trout_barkeep,
		 //act 2
		 ActorSno._a2_uniquevendor_innkeeper,
		 //act 3
		 ActorSno._a3_uniquevendor_innkeeper,
		 //act 4
		 ActorSno._a4_uniquevendor_innkeeper,
		 //act 5
		 ActorSno._x1_a5_uniquevendor_innkeeper
	)]
	public class Innkeeper : Vendor
	{
		public Innkeeper(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
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
