//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
	[HandledSNO(364559, 365097)]
	class CursedChest : Gizmo
	{
		public CursedChest(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
		}

		private bool _collapsed = false;
		private bool Randomed = (DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 5) == 0);
		private List<int> eventIds = new List<int>() { 365305, 368306, 369332 };

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
				if (player.Position.DistanceSquared(ref _position) < 225f && !_collapsed && this.Randomed)
				{
					_collapsed = true;

					this.World.Game.SideQuestGizmo = this;
					this.World.Game.QuestManager.LaunchSideQuest(eventIds[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, eventIds.Count())], true);
				}
			}
			catch { }
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (!Randomed) return false;

			if (!base.Reveal(player))
				return false;

			return true;
		}

		public void Activate()
		{
			var rewardChests = this.GetActorsInRange<LootContainer>(20f).Where(c => c.rewardChestAvailable == false).ToList();

			foreach (var chest in rewardChests)
			{
				chest.rewardChestAvailable = true;
				foreach (var plr in chest.GetPlayersInRange(100f))
					chest.Reveal(plr);
			}

			this.Destroy();
		}
	}
}
