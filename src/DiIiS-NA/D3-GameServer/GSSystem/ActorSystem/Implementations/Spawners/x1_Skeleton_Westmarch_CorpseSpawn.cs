using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * Scale * Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                PlayEffectGroup(295060);
                World.SpawnMonster(ActorSno._x1_skeleton_westmarch_ghost_a, Position);
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
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 1.5 * Scale * Scale && !_collapsed)
            {
                _collapsed = true;
                //int duration = 500; // ticks

                PlayEffectGroup(295060);
                World.SpawnMonster(ActorSno._x1_skeletonarcher_westmarch_a, Position);
            }
        }

    }
    //292834
}
