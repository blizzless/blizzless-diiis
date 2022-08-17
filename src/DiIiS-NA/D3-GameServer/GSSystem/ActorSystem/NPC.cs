//Blizzless Project 2022 
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

		public NPC(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Field2 = 0x9;
			this.Field7 = 1;
			this.Attributes[GameAttribute.Is_NPC] = true;
		}
	}
}
