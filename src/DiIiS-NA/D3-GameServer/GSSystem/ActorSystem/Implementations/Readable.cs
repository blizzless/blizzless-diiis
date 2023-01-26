//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Readable : Gizmo
	{
		private bool used = false;

		public Readable(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.TeamID] = 1;
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (ActorData.TagMap.ContainsKey(ActorKeys.Lore))
				Logger.Debug("Lore detected: {0}", ActorData.TagMap[ActorKeys.Lore].Id);

			if (LoreRegistry.Lore.ContainsKey(World.SNO) && LoreRegistry.Lore[World.SNO].chests_lore.ContainsKey(SNO))
				foreach (var p in GetPlayersInRange(30))
					foreach (int loreId in LoreRegistry.Lore[World.SNO].chests_lore[SNO])
						if (!p.HasLore(loreId))
						{
							World.DropItem(this, null, ItemGenerator.CreateLore(p, loreId));
							break;
						}

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec()
					{
						Duration = 50,
						AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
						PermutationIndex = 0,
						AnimationTag = 0, 
						Speed = 1
					}
				}

			}, this);

			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			used = true;
			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			Attributes[GameAttribute.TeamID] = 2;
			Attributes[GameAttribute.Untargetable] = true;
			Attributes[GameAttribute.Operatable] = false;
			Attributes[GameAttribute.Operatable_Story_Gizmo] = false;
			Attributes[GameAttribute.Disabled] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttribute.Chest_Open, 0xFFFFFF] = true;
			Attributes.BroadcastChangedIfRevealed();

			base.OnTargeted(player, message);
		}

		public override bool Reveal(Player player)
		{
			if (used)
				return false;
			else
				return base.Reveal(player);
		}
	}
}
