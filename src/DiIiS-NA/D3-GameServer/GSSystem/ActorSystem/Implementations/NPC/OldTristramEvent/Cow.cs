//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
    [HandledSNO(453600)]
    class Cow : NPC
    {
        public Cow(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field7 = 1;
            this.Attributes[GameAttribute.TeamID] = 2;
        }
    }
}
