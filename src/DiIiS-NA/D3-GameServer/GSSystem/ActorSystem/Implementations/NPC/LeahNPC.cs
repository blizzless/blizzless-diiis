//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
    [HandledSNO(ActorSno._leah_afterevent31_exit)]
    class LeahNPC : InteractiveNPC
    {
        public LeahNPC(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            this.Field7 = 1;
            this.Attributes[GameAttribute.TeamID] = 2;
        }

    }
}
