//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Environment : Actor
	{
		public override ActorType ActorType { get { return ActorType.Environment; } }

		public Environment(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Field2 = 0x10;//16;
			this.Field7 = 0x00000000;
			this.CollFlags = 1;
		}
	}
}
