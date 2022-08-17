//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Spawners
{
    [HandledSNO(327)]
    class KingGhost_Spawner : Gizmo
    {
        private bool _collapsed = false;

        public KingGhost_Spawner(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
                if (this.World.WorldSNO.Id == 50585)
                {
                    _collapsed = true;

                    var KingGhost = World.SpawnMonster(5360, this.Position);
                    KingGhost.Attributes[MessageSystem.GameAttribute.Untargetable] = true;
                    KingGhost.Attributes.BroadcastChangedIfRevealed();
                    StartConversation(World, 17921);
                }
        }
        protected bool StartConversation(World world, int conversationId)
        {
            foreach (var plr in world.Players)
                plr.Value.Conversations.StartConversation(conversationId);
            return true;
        }

    }
}
