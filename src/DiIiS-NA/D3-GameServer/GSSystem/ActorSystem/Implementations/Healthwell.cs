using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Healthwell : Gizmo
	{
		public Healthwell(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.Gizmo_State] = 0;
		}


		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			//Logger.Warn("Healthwell has no function, Powers not implemented");

			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttribute.Gizmo_State] = 1;
			Attributes.BroadcastChangedIfRevealed();
			player.AddPercentageHP(50);
			player.AddAchievementCounter(74987243307169, 1);
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
			{
				HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("x1_OpenWorld_LootRunObelisk_B"),
				Place = new MessageSystem.Message.Fields.WorldPlace { Position = Position, WorldID = World.GlobalID },
				ImageInfo = 218234,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = -1,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = true,
				PlayerUseFlags = 0
			});

			return base.Reveal(player);
		}
	}
}
