using System;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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
