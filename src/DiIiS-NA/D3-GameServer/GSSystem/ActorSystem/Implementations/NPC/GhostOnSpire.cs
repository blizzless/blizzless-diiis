//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(
        ActorSno._a4dun_aspect_ghost_01,
        ActorSno._a4dun_aspect_ghost_02,
        ActorSno._a4dun_aspect_ghost_03,
        ActorSno._a4dun_aspect_ghost_04,
        ActorSno._a4dun_aspect_ghost_05,
        ActorSno._a4dun_aspect_ghost_06,
        ActorSno._a4dun_aspect_ghost_07
    )] //Ghosts
    public class GhostOnSpire : InteractiveNPC
    {
        public GhostOnSpire(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Field7 = 1;
            Attributes[GameAttribute.TeamID] = 2;
            Attributes.BroadcastChangedIfRevealed();
        }

    }
}
