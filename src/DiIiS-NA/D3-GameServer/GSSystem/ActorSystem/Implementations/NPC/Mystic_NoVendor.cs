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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(61524)] //PT_Mystic_NoVendor
	public class MysticNoVendor : InteractiveNPC
	{
		public MysticNoVendor(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Invulnerable] = true;
		}

		protected override void ReadTags()
		{

			base.ReadTags();
		}
	}
	[HandledSNO(87037)] //PT_Mystic_NoVendor
	public class TemplarNPC : InteractiveNPC
	{
		public TemplarNPC(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Invulnerable] = true;
		}

		protected override void ReadTags()
		{
			base.ReadTags();
		}
	}
}
