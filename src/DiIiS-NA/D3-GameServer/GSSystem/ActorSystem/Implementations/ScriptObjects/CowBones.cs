//Blizzless Project 2022 
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
	172208 //caOut_Oasis_Bonepile_A
	)]
	class CowBones : DesctructibleLootContainer
	{
		public CowBones(World world, TagMap tags)
			: base(world, 172208, false, tags)
		{
			this.Scale *= 1.5f;
			this.Hidden = true;
		}
	}
}
