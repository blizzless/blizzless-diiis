//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    
    [HandledSNO(454066)]
    class NecromancerFlesh : Gizmo
    {
        public NecromancerFlesh(MapSystem.World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field2 = 16;//16;
            this.Field7 = 0x00000001;
            this.CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
            this.Attributes[GameAttribute.Hitpoints_Cur] = 1;
        }

    }
    //*/
}

//454066
