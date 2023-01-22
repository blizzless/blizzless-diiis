//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
    [HandledSNO(ActorSno._x1_westm_door_giant_opening_event)]
    class X1_Westm_Door_Giant_Event : Gizmo
    {
        private bool _collapsed = false;
        public List<Core.Types.Math.Vector3D> positions = new List<Core.Types.Math.Vector3D>() { };
        public List<Core.Types.Math.Vector3D> shieldpositions = new List<Core.Types.Math.Vector3D>() { };
        public X1_Westm_Door_Giant_Event(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Field2 = 0;

            //hide skeletons.
            foreach (var skeleton in GetActorsInRegion<Monster>(120))
            {
                switch (skeleton.SNO)
                {
                    case ActorSno._x1_shield_skeleton_westmarch_ghost_a: //x1_Shield_Skeleton_Westmarch_Ghost_A
                        shieldpositions.Add(skeleton.Position);
                        skeleton.Destroy();
                        break;
                    case ActorSno._x1_skeleton_westmarch_ghost_a: //x1_Skeleton_Westmarch_Ghost_A
                        positions.Add(skeleton.Position);
                        skeleton.Destroy();
                        break;
                }
            }
        }

        public override bool Reveal(Player player)
        {
            if (World.SNO == WorldSno.x1_westm_intro)
            {
                World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
                {
                    ActorID = DynamicID(plr),
                    AnimationSNO = AnimationSetKeys.Open.ID
                }, this);
            }
            //
            if (positions.Count == 0)
            {
                foreach (var skeleton in GetActorsInRegion<Monster>(120))
                {
                    switch (skeleton.SNO)
                    {
                        case ActorSno._x1_shield_skeleton_westmarch_ghost_a: //x1_Shield_Skeleton_Westmarch_Ghost_A
                            shieldpositions.Add(skeleton.Position);
                            skeleton.Destroy();
                            break;
                        case ActorSno._x1_skeleton_westmarch_ghost_a: //x1_Skeleton_Westmarch_Ghost_A
                            positions.Add(skeleton.Position);
                            skeleton.Destroy();
                            break;
                    }
                }
            }
            return base.Reveal(player);
        }

        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && !_collapsed)
            {
                _collapsed = true;
                #region Animation of big gates
                PlayAnimation(11, 312534, 1);
                World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
                {
                    ActorID = DynamicID(plr),
                    AnimationSNO = 286920
                }, this);
                #endregion

                #region return skeletons
                foreach (var skeleton in positions) World.SpawnMonster(ActorSno._x1_skeleton_westmarch_ghost_a, skeleton);
                foreach (var skeleton in shieldpositions) World.SpawnMonster(ActorSno._x1_shield_skeleton_westmarch_ghost_a, skeleton);
                #endregion
            }
        }
    }
}
