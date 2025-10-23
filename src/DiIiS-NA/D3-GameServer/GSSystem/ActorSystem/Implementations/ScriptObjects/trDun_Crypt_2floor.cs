using System.Collections.Generic;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
    [HandledSNO(ActorSno._trdun_skeletonking_sealed_door_1000_pounder)]
    class TrDun_Crypt_2floor : Gizmo
    {
        private bool _collapsed = false;
        public TrDun_Crypt_2floor(World world, ActorSno sno, TagMap tags)
           : base(world, sno, tags)
        {
            Field2 = 0;
        }

        public override bool Reveal(Player player)
        {
            if (!_collapsed)
                this.PlayAnimation(5, AnimationSno.trdun_skeletonking_sealed_door_1000_pounder_idle); //- Тряска

            //this.PlayAnimation(5, 116098); //- Fault
            return base.Reveal(player);
        }

        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 3f * Scale && !_collapsed)
            {
                _collapsed = true;
                this.PlayAnimation(5, AnimationSno.trdun_skeletonking_sealed_door_1000_pounder_death); //- Разлом
                this.World.SpawnMonster(ActorSno._unburied_a_unique, this.Position);
            }
        }

        private bool OnKillListener(List<uint> monstersAlive, World world)
        {
            int monstersKilled = 0;
            var monsterCount = monstersAlive.Count;
            while (monstersKilled != monsterCount)
            {
                for (int i = monstersAlive.Count - 1; i >= 0; i--)
                {
                    if (world.HasMonster(monstersAlive[i]))
                    {
                    }
                    else
                    {
                        Logger.Debug(monstersAlive[i] + " killed");
                        monstersAlive.RemoveAt(i);
                        monstersKilled++;
                    }
                }
            }
            return true;
        }
    }
}
