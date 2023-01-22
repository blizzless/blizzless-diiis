//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	[HandledSNO(
		ActorSno._a1_genericvendor_tinker,
		ActorSno._a1_uniquevendor_alchemist,
		ActorSno._a1_uniquevendor_armorer,
		ActorSno._a1_uniquevendor_curios,
		ActorSno._a1_uniquevendor_weaponsmith,
		ActorSno._a2_uniquevendor_event_mapvendor,
		ActorSno._a2_uniquevendor_tinker,
		ActorSno._a3_uniquevendor_alchemist
	)]
	public class HiddenVendor : Vendor
	{
		// TODO: extract
		private static readonly Dictionary<ActorSno, ulong> criteria = new Dictionary<ActorSno, ulong>
		{
			[ActorSno._a1_genericvendor_tinker] = 74987243309911,
			[ActorSno._a1_uniquevendor_alchemist] = 74987243309912,
			[ActorSno._a1_uniquevendor_armorer] = 74987243309913,
			[ActorSno._a1_uniquevendor_curios] = 74987243309914,
			[ActorSno._a1_uniquevendor_weaponsmith] = 74987243309915,
			[ActorSno._a2_uniquevendor_event_mapvendor] = 74987243309918,
			[ActorSno._a2_uniquevendor_tinker] = 74987243309920,
			[ActorSno._a3_uniquevendor_alchemist] = 74987243309922
		};
		private bool Enabled = false;

		public HiddenVendor(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Enabled = (FastRandom.Instance.Next(100) < 40);
		}

		protected override List<Item> GetVendorItems()
		{
			var list = new List<Item>();

			for (int i = 0; i < 9; i++)
			{
				var itm = ItemGenerator.GenerateRandomEquip(this, level, 6, 7);
				itm.Attributes[GameAttribute.Item_Cost_Percent_Bonus] = 3f;
				list.Add(itm);
			}

			return list;
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (!Enabled) return false;
			return base.Reveal(player);
		}

		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			if (criteria.ContainsKey(SNO))
				player.GrantCriteria(criteria[SNO]);
		}
	}
}
