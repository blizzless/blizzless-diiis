//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
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
