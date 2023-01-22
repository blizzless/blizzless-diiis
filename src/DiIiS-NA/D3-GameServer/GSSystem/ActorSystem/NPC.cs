//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class NPC : Living
	{
		public override ActorType ActorType { get { return ActorType.Monster; } }

		public NPC(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x9;
			Field7 = 1;
			Attributes[GameAttribute.TeamID] = 2;
			Attributes[GameAttribute.Is_NPC] = true;
		}
	}
}
