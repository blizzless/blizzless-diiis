using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
