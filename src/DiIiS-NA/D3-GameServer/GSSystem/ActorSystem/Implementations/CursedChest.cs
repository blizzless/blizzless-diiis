using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._x1_global_chest_cursedchest, ActorSno._x1_global_chest_cursedchest_b)]
	class CursedChest : Gizmo
	{
		public CursedChest(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
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
				if (player.Position.DistanceSquared(ref _position) < 225f && !_collapsed && Randomed)
				{
					_collapsed = true;

					World.Game.SideQuestGizmo = this;
					World.Game.QuestManager.LaunchSideQuest(eventIds[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, eventIds.Count())], true);
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
			var rewardChests = GetActorsInRange<LootContainer>(20f).Where(c => c.rewardChestAvailable == false).ToList();

			foreach (var chest in rewardChests)
			{
				chest.rewardChestAvailable = true;
				foreach (var plr in chest.GetPlayersInRange(100f))
					chest.Reveal(plr);
			}

			Destroy();
		}
	}
}
