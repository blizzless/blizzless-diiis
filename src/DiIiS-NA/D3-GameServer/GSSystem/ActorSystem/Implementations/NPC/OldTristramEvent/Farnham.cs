//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._p43_ad_farnham)]
    class Farnham : InteractiveNPC
    {
        public Farnham(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Field7 = 1;
            Attributes[GameAttribute.TeamID] = 2;
        }
    }
}
