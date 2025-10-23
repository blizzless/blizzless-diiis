using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._pvp_targetdummy_level60)]
	public class PVPTraining : Monster
	{
		public PVPTraining(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			WalkSpeed = 0f;
		}

	}
}
