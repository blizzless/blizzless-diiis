//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    
    [HandledSNO(ActorSno._p6_necro_corpse_flesh)]
    class NecromancerFlesh : Gizmo
    {
        public NecromancerFlesh(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Field2 = 16;//16;
            Field7 = 0x00000001;
            CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
            Attributes[GameAttribute.Hitpoints_Cur] = 1;
        }

    }
    //*/
}

//454066
