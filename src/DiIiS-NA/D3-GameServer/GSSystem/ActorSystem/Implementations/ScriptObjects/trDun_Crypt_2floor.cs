//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
    [HandledSNO(116099)]
    class trDun_Crypt_2floor : Gizmo
    {
        private bool _collapsed = false;
        public trDun_Crypt_2floor(World world, int snoId, TagMap tags)
           : base(world, snoId, tags)
        {
            Field2 = 0;
        }

        public override bool Reveal(Player player)
        {
            if (!_collapsed)
                this.PlayAnimation(5, 130011); //- Тряска

            //this.PlayAnimation(5, 116098); //- Разлом
            return base.Reveal(player);
        }

        public override void OnPlayerApproaching(Player player)
        {
            if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * 3f * this.Scale && !_collapsed)
            {
                _collapsed = true;
                this.PlayAnimation(5, 116098); //- Разлом
                this.World.SpawnMonster(76953, this.Position);
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
                        Logger.Debug(monstersAlive[i] + " убит");
                        monstersAlive.RemoveAt(i);
                        monstersKilled++;
                    }
                }
            }
            return true;
        }
    }
}
