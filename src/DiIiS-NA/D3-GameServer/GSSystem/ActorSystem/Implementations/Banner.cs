//Blizzless Project 2022 
using System;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Banner : Gizmo
	{
		public Banner(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			if (this.ActorSNO.Name.Contains("Player_2"))
				this.BannerPlayerIndex = 1;
			else if (this.ActorSNO.Name.Contains("Player_3"))
				this.BannerPlayerIndex = 2;
			else if (this.ActorSNO.Name.Contains("Player_4"))
				this.BannerPlayerIndex = 3;
		}

		public int BannerPlayerIndex = 0;

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Trace("(OnTargeted) Banner has been activated ");

			var banner_player = this.World.Game.Players.Values.Single(p => p.PlayerIndex == this.BannerPlayerIndex);

			if (banner_player == null || banner_player.World == null)
			{
				Logger.Warn("Banner's destination world does not exist");
				return;
			}

			player.ShowConfirmation(this.DynamicID(player), (() => {
				player.StartCasting(150, new Action(() => {
					if (banner_player.PlayerDirectBanner == null)
					{
						if (banner_player.World == this.World)
							player.Teleport(banner_player.Position);
						else
							player.ChangeWorld(banner_player.World, banner_player.Position);
					}
					else
					{
						if (banner_player.PlayerDirectBanner.World == this.World)
							player.Teleport(banner_player.PlayerDirectBanner.Position);
						else
							player.ChangeWorld(banner_player.PlayerDirectBanner.World, banner_player.PlayerDirectBanner.Position);
					}
				}));
			}));
		}
	}
}
