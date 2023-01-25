//Blizzless Project 2022
using System;
using System.Collections.Generic;
//Blizzless Project 2022
using System.Linq;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Banner : Gizmo
	{
		private static readonly Dictionary<int, ActorSno[]> bannerActors = new Dictionary<int, ActorSno[]>()
		{
			[0] = new ActorSno[] {
				ActorSno._banner_player_1,
				ActorSno._emotebanner_player_1,
				ActorSno._banner_player_1_act2,
				ActorSno._emotebanner_player_1_lit,
				ActorSno._banner_player_1_act5,
			},
			[1] = new ActorSno[] {
				ActorSno._banner_player_2,
				ActorSno._emotebanner_player_2,
				ActorSno._banner_player_2_act2,
				ActorSno._banner_player_2_act5,
			},
			[2] = new ActorSno[] {
				ActorSno._banner_player_3,
				ActorSno._emotebanner_player_3,
				ActorSno._banner_player_3_act2,
				ActorSno._banner_player_3_act5,
			},
			[3] = new ActorSno[] {
				ActorSno._banner_player_4,
				ActorSno._emotebanner_player_4,
				ActorSno._banner_player_4_act2,
				ActorSno._banner_player_4_act5,
			},
		};
		public Banner(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			BannerPlayerIndex = bannerActors.FirstOrDefault(x => x.Value.Contains(SNO)).Key;
		}

		public int BannerPlayerIndex = 0;

		public override bool Reveal(Player player)
		{
            return base.Reveal(player);
        }

        public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Trace("(OnTargeted) Banner has been activated ");

			if (World.Game.Players.Count == 1 || BannerPlayerIndex == 0)
			{
                player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.SimpleMessage(Opcodes.OpenBannerCustomizationMessage));
                return;
			}
			
			var banner_player = World.Game.Players.Values.Single(p => p.PlayerIndex == BannerPlayerIndex);

			if (banner_player == null || banner_player.World == null)
			{
				Logger.Warn("Banner's destination world does not exist");
				return;
			}

			//if banner has been disabled for events like active greater active swarm  /advocaite
			if(!player.Attributes[GameAttribute.Banner_Usable])
			{
				return;
			}

			player.ShowConfirmation(DynamicID(player), (() => {
				player.StartCasting(150, new Action(() => {
					if (banner_player.PlayerDirectBanner == null)
					{
						if (banner_player.World == World)
							player.Teleport(banner_player.Position);
						else
							player.ChangeWorld(banner_player.World, banner_player.Position);
					}
					else
					{
						if (banner_player.PlayerDirectBanner.World == World)
							player.Teleport(banner_player.PlayerDirectBanner.Position);
						else
							player.ChangeWorld(banner_player.PlayerDirectBanner.World, banner_player.PlayerDirectBanner.Position);
					}
				}));
			}));
		}
	}
}
