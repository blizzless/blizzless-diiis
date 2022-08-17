//Blizzless Project 2022 
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
    [HandledSNO(450245,450247,450248,450249,450250,
                450251,450252,450253,450254,450255,
                450256,450257,450258,450259,450260,
                450261,450262,450263,450264,450265,
                450266,450267,450268,450269 )]
    public sealed class SetDungeon : Gizmo
    {
        public bool PlrNear = false;
        public SetDungeon(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Attributes[GameAttribute.TeamID] = 2;
            this.Attributes[GameAttribute.MinimapActive] = true;
            this.Attributes.BroadcastChangedIfRevealed();
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            this.PlayAnimation(5, 447873);

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
            if (!base.Reveal(player))
                return false;
            
            this.PlayAnimation(5, 449254);
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
		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
                if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale)// * this.Scale)
                {
                    if (!PlrNear)
                    {
                        PlrNear = true;
                        this.PlayAnimation(5, 449255);
                    }
                }
                else
                {
                    if (PlrNear)
                    {
                        PlrNear = false;
                        this.PlayAnimation(5, 447868);
                    }
                }
			}
			catch { }
		}
	}
}
