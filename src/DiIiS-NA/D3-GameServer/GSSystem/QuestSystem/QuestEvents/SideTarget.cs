//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
    class SideTarget : Dummy
    {

        public override void Execute(MapSystem.World world)
        {
            foreach (var actr in world.Actors.Values)
                if (actr.SNO == ActorSno._zombiefemale_a_tristramquest_unique)
                {
                    actr.Attributes[GameAttribute.Quest_Monster] = false;
                    actr.Attributes.BroadcastChangedIfRevealed();
                }
        }
    }
}
