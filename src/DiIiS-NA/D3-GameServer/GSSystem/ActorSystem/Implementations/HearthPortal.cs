//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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
	class HearthPortal : Gizmo
	{
		public WorldSno ReturnWorld = WorldSno.__NONE;

		public Vector3D ReturnPosition = null;

		public Player Owner = null;

		public HearthPortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
			SetVisible(false);
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Trace("(OnTargeted) HearthPortal has been activated ");

			var world = World.Game.GetWorld(ReturnWorld);

			if (world == null)
			{
				Logger.Warn("HearthPortal's world does not exist (WorldSNO = {0})", ReturnWorld);
				return;
			}

			if (World.Game.QuestManager.SideQuests.ContainsKey(120396) && World.Game.QuestManager.SideQuests[120396].Completed && ReturnWorld == WorldSno.a2dun_zolt_timed01_level01) return;

			Vector3D exCheckpoint = player.CheckPointPosition;

			if (world == World)
				player.Teleport(ReturnPosition);
			else
				player.ChangeWorld(world, ReturnPosition);

			player.CheckPointPosition = exCheckpoint;
			SetVisible(false);
		}

		public override bool Reveal(Player player)
		{
			if (player != Owner) return false;
			return base.Reveal(player);
		}
	}
}
