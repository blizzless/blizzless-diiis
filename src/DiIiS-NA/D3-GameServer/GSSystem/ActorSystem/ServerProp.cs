//Blizzless Project 2022 
using System.Collections.Generic;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class ServerProp : Actor
	{
		private static readonly HashSet<ActorSno> hidden = new HashSet<ActorSno>
		{
			ActorSno._mouthofazmodan,
			ActorSno._a1_sk_throne_gate,
			ActorSno._gluttony_fading_block_collision,
			ActorSno._caout_militarywallb,
			ActorSno._caout_militarywallb_invisible_teleportblocker,
			ActorSno._a3_battlefield_barricade_solid,
			ActorSno._a3dun_crater_st_giantdemonheart_shield,
			ActorSno._temp_zknavblocker,
			ActorSno._adriacover,
			ActorSno._caout_militarywallb_invisible_cemeterygate,
			ActorSno._townattack_chapelloc,
			ActorSno._caoutstingingwinds_stingingwinds_mine_blocker,
			ActorSno._invisboxcollision_flippy,
			ActorSno.__x1_westm_urzael_fire_event_flash,
		};
		public override ActorType ActorType
		{
			get { return ActorType.ServerProp; }
		}
		//a2dun_Zolt_Hall_NS_480_02 - 1784
		//a2dun_Zolt_Portalroom_A - 31076

		public ServerProp(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x9;
			Field7 = 0x00000001;
			CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
								//this.Attributes[GameAttribute.MinimapActive] = true;

		}

		private bool triggered = false;

		public override bool Reveal(Player player)
		{
			if (hidden.Contains(SNO) ||
				(SNO == ActorSno._x1_westm_door_cloister_locked && World.Game.CurrentQuest != 251355) ||   //A5_closedDoor
				((SNO == ActorSno._trout_newtristram_blocking_cart || SNO == ActorSno._cain_intro_bridge_invisi_wall) && World.Game.CurrentQuest != 87700))          //Tristram invis wall
				return false;

			if (!triggered)
			{
				triggered = true;
				if (SNO == ActorSno._invisboxcollision_leorlogs && FastRandom.Instance.Next(100) < 30) //invisBoxCollision_LeorLogs
				{
					World.SpawnMonster(ActorSno._trout_highlands_manor_firewood, Position);
				}
			}

			return base.Reveal(player);
		}
	}
}
