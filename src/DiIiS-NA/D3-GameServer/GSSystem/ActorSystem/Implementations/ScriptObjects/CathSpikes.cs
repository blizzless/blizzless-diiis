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
	[HandledSNO(ActorSno._trdun_cath_bonewall_a_door)]
	public class CathSpikes : Gizmo
	{
		public CathSpikes(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}


		public override bool Reveal(Player player)
		{
			return base.Reveal(player);
			//return false;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}

	}
}
