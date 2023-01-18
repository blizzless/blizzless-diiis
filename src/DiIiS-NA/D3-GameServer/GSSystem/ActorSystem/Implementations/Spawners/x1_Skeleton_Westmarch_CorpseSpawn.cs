//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
    [HandledSNO(
        ActorSno._x1_skeleton_westmarch_corpsespawn_06_ground,
        ActorSno._x1_skeleton_westmarch_corpsespawn_10_ground,
        ActorSno._x1_skeleton_westmarch_corpsespawn_01_ground,
        ActorSno._x1_skeleton_westmarch_corpsespawn_09_ground,
        ActorSno._x1_skeleton_westmarch_corpsespawn_02_sitagainstwall,
        ActorSno._x1_skeleton_westmarch_corpsespawn_04_neckstabbed,
        ActorSno._x1_skeleton_westmarch_corpsespawn_03_ground
    )]
    class x1_Skeleton_Westmarch_CorpseSpawn : Gizmo
    {
        private bool _collapsed = false;

        public x1_Skeleton_Westmarch_CorpseSpawn(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                this.PlayEffectGroup(295060);
                World.SpawnMonster(ActorSno._x1_skeleton_westmarch_ghost_a, this.Position);
            }
        }

    }
    [HandledSNO(ActorSno._x1_skeletonarcher_westmarch_corpsespawn)]
    class X1_SkeletonArcher_Westmarch_CorpseSpawn : Gizmo
    {
        private bool _collapsed = false;

        public X1_SkeletonArcher_Westmarch_CorpseSpawn(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {

        }
        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * this.Scale * this.Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                this.PlayEffectGroup(295060);
                World.SpawnMonster(ActorSno._x1_skeletonarcher_westmarch_a, this.Position);
            }
        }

    }
    //292834
}
