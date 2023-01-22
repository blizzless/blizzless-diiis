//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(
        ActorSno._p4_setdung_totem_dh_ess,
        ActorSno._p4_setdung_totem_barb_kings,
        ActorSno._p4_setdung_totem_barb_might,
        ActorSno._p4_setdung_totem_barb_raekor,
        ActorSno._p4_setdung_totem_barb_wastes,
        ActorSno._p4_setdung_totem_cru_akkhan,
        ActorSno._p4_setdung_totem_cru_roland,
        ActorSno._p4_setdung_totem_cru_seeker,
        ActorSno._p4_setdung_totem_cru_thorns,
        ActorSno._p4_setdung_totem_dh_mar,
        ActorSno._p4_setdung_totem_dh_nat,
        ActorSno._p4_setdung_totem_dh_shadow,
        ActorSno._p4_setdung_totem_monk_innas,
        ActorSno._p4_setdung_totem_monk_storms,
        ActorSno._p4_setdung_totem_monk_sunwuko,
        ActorSno._p4_setdung_totem_monk_uliana,
        ActorSno._p4_setdung_totem_wd_haunt,
        ActorSno._p4_setdung_totem_wd_jade,
        ActorSno._p4_setdung_totem_wd_spider,
        ActorSno._p4_setdung_totem_wd_tooth,
        ActorSno._p4_setdung_totem_wiz_firebird,
        ActorSno._p4_setdung_totem_wiz_opus,
        ActorSno._p4_setdung_totem_wiz_rasha,
        ActorSno._p4_setdung_totem_wiz_vyr,
        ActorSno._p6_setdung_totem_necro_blood,
        ActorSno._p6_setdung_totem_necro_bone,
        ActorSno._p6_setdung_totem_necro_plague,
        ActorSno._p6_setdung_totem_necro_saint
    )]
    public sealed class SetDungeon : Gizmo
    {
        public bool PlrNear = false;
        public SetDungeon(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttribute.TeamID] = 2;
            Attributes[GameAttribute.MinimapActive] = true;
            Attributes.BroadcastChangedIfRevealed();
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            PlayAnimation(5, 447873);

            foreach (var plr in World.Game.Players.Values)
            {
                plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Dungeon.SetDungeonJoinMessage()
                {
                    PlayerIndex = player.PlayerIndex,
                    LabelDescription = 1,
                    LabelTitle = 1
                });
                /*
                plr.InGameClient.SendMessage(new DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter.RiftJoinMessage()
                {
                    PlayerIndex = player.PlayerIndex,
                    RiftStartServerTime = this.World.Game.TickCounter,
                    RiftTier = 1
                });
                //*/
            }
        }

        public override bool Reveal(Player player)
        {
            // TODO: check that player has set
            if (!base.Reveal(player))
                return false;
            
            PlayAnimation(5, 449254);
            return true;
        }
		/*
					+		[0]	{EmoteIdle = 449254}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - Idle
					+		[1]	{426720 = 449255}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - Near Player
					+		[2]	{71200 = 447875}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - 
					+		[3]	{411664 = 447873}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - - USE
					+		[4]	{IdleDefault = 447876}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - Idle Near Player
					+		[5]	{86016 = 447868}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - Away Player
					+		[6]	{411669 = 449674}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry - Use

				 */
		public override void OnPlayerApproaching(Player player)
		{
			try
			{
                if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale)// * this.Scale)
                {
                    if (!PlrNear)
                    {
                        PlrNear = true;
                        PlayAnimation(5, 449255);
                    }
                }
                else
                {
                    if (PlrNear)
                    {
                        PlrNear = false;
                        PlayAnimation(5, 447868);
                    }
                }
			}
			catch { }
		}
	}
}
