//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
    [HandledSNO(ActorSno._blizzcon_kingghost_spawner)]
    class KingGhost_Spawner : Gizmo
    {
        private bool _collapsed = false;

        public KingGhost_Spawner(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
                if (this.World.SNO == WorldSno.a1trdun_level07)
                {
                    _collapsed = true;

                    var KingGhost = World.SpawnMonster(ActorSno._skeletonking_ghost, this.Position);
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
