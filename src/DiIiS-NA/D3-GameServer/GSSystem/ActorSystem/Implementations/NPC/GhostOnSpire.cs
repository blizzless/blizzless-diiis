//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(196900, 196901, 196902, 196903, 196904, 196905)] //Ghosts
    public class GhostOnSpire : InteractiveNPC
    {
        public GhostOnSpire(MapSystem.World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field7 = 1;
            this.Attributes[GameAttribute.TeamID] = 2;
            this.Attributes.BroadcastChangedIfRevealed();
        }

    }
}
