//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._x1_westm_barricade_solid_debries)]
	public class ActVBarricade : Gizmo
	{
		public ActVBarricade(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}

		public override bool Reveal(Player player)
		{
			if (World.SNO == WorldSno.x1_westmarch_hub) return false;
			return base.Reveal(player);
		}
	}
}
