using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		//364601, 368169 - not found, possible values below
		ActorSno._p4_bountygrounds_cursedshrine,
		ActorSno._p4_bountygrounds_cursedshrine_a5
	)]
	class CursedShrine : Gizmo
	{
		public CursedShrine(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
		}

		private bool _collapsed = false;
		private bool Randomed = (DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 5) == 0);
		private List<int> eventIds = new List<int>() { 365751, 368092, 365033 };

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
			World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.ShrineActivatedMessage) { ActorID = DynamicID(plr) }, this);
			var type = DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 4);
			switch (type)
			{
				case 0: //blessed
					foreach (var plr in GetPlayersInRange(100f))
					{
						World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineBlessedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
						plr.GrantCriteria(74987243307423);
					}
					break;
				case 1: //enlightened
					foreach (var plr in GetPlayersInRange(100f))
					{
						World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineEnlightenedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
						plr.GrantCriteria(74987243307424);
					}
					break;
				case 2: //fortune
					foreach (var plr in GetPlayersInRange(100f))
					{
						World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineFortuneBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
						plr.GrantCriteria(74987243307425);
					}
					break;
				case 3: //frenzied
					foreach (var plr in GetPlayersInRange(100f))
					{
						World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineFrenziedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
						plr.GrantCriteria(74987243307426);
					}
					break;
				default:
					foreach (var plr in GetPlayersInRange(100f))
					{
						World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineEnlightenedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
					}
					break;
			}
			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttribute.Gizmo_State] = 1;
			Attributes.BroadcastChangedIfRevealed();

			var rewardChests = GetActorsInRange<LootContainer>(20f).Where(c => c.rewardChestAvailable == false).ToList();

			foreach (var chest in rewardChests)
			{
				chest.rewardChestAvailable = true;
				foreach (var plr in chest.GetPlayersInRange(100f))
					chest.Reveal(plr);
			}
		}
	}
}
