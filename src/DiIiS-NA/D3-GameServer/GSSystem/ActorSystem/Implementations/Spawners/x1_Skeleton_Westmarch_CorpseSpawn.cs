//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
    [HandledSNO(311938, 311944, 311932, 311943, 311933, 311936, 311934)]
    class x1_Skeleton_Westmarch_CorpseSpawn : Gizmo
    {
        private bool _collapsed = false;

        public x1_Skeleton_Westmarch_CorpseSpawn(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                this.PlayEffectGroup(295060);
                World.SpawnMonster(310893, this.Position);
            }
        }

    }
    [HandledSNO(292834)]
    class x1_SkeletonArcher_Westmarch_CorpseSpawn : Gizmo
    {
        private bool _collapsed = false;

        public x1_SkeletonArcher_Westmarch_CorpseSpawn(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                this.PlayEffectGroup(295060);
                World.SpawnMonster(282789, this.Position);
            }
        }

    }
    //292834
}
