//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._px_ruins_frost_camp_barbnpc)] //px_Ruins_Frost_Camp_BarbNPC
	public class BarbarianNPC : InteractiveNPC
	{
		public BarbarianNPC(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{ }

		protected override void ReadTags()
		{
			
		}
	}
}
