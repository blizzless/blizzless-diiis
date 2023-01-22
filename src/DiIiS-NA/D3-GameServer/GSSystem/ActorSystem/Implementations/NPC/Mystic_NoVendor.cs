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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._pt_mystic_novendor)] //PT_Mystic_NoVendor
	public class MysticNoVendor : InteractiveNPC
	{
		public MysticNoVendor(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.Invulnerable] = true;
		}

		protected override void ReadTags()
		{

			base.ReadTags();
		}
	}
	[HandledSNO(ActorSno._templarnpc)]
	public class TemplarNPC : InteractiveNPC
	{
		public TemplarNPC(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.Invulnerable] = true;
		}

		protected override void ReadTags()
		{
			base.ReadTags();
		}
	}
}
