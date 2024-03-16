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
	[HandledSNO(
	ActorSno._caout_oasis_bonepile_a //caOut_Oasis_Bonepile_A
	)]
	class CowBones : DesctructibleLootContainer
	{
		public CowBones(World world, TagMap tags)
			: base(world, ActorSno._caout_oasis_bonepile_a, false, tags)
		{
			Scale *= 1.5f;
			Hidden = true;
		}
	}
}
