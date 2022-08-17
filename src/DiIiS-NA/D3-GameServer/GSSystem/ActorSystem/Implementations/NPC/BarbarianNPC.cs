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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(435707)] //px_Ruins_Frost_Camp_BarbNPC
	public class BarbarianNPC : InteractiveNPC
	{
		public BarbarianNPC(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{ }

		protected override void ReadTags()
		{
			
		}
	}
}
