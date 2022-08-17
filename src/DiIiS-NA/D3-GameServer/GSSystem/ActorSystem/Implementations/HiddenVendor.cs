//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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
	[HandledSNO(81610, 105372, 81609, 107419, 106354, 115928, 144328, 176826)]
	public class HiddenVendor : Vendor
	{
		private bool Enabled = false;

		public HiddenVendor(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Enabled = (FastRandom.Instance.Next(100) < 40);
		}

		protected override List<Item> GetVendorItems()
		{
			var list = new List<Item>();

			for (int i = 0; i < 9; i++)
			{
				var itm = ItemGenerator.GenerateRandomEquip(this, this.level, 6, 7);
				itm.Attributes[GameAttribute.Item_Cost_Percent_Bonus] = 3f;
				list.Add(itm);
			}

			return list;
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (!this.Enabled) return false;
			return base.Reveal(player);
		}

		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			switch (this.ActorSNO.Id)
			{
				case 81610:
					player.GrantCriteria(74987243309911);
					break;
				case 105372:
					player.GrantCriteria(74987243309912);
					break;
				case 81609:
					player.GrantCriteria(74987243309913);
					break;
				case 107419:
					player.GrantCriteria(74987243309914);
					break;
				case 106354:
					player.GrantCriteria(74987243309915);
					break;
				case 115928:
					player.GrantCriteria(74987243309918);
					break;
				case 144328:
					player.GrantCriteria(74987243309920);
					break;
				case 176826:
					player.GrantCriteria(74987243309922);
					break;
			}
		}
	}
}
