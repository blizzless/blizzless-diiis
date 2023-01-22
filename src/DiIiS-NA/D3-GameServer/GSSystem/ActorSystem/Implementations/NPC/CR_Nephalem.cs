//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._p6_challengerift_nephalem)] //SNO - 470782, Name - P6_ChallengeRift_Nephalem
    class CR_Nephalem : InteractiveNPC
    {
        public CR_Nephalem(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Conversations.Clear();
            Conversations.Add(new Interactions.ConversationInteraction(471065));
            Conversations.Add(new Interactions.ConversationInteraction(471076));

            Field7 = 1;
            Attributes[GameAttribute.MinimapActive] = true;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            base.OnTargeted(player, message);
        }
    }
}
