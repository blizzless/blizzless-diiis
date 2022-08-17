//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
	[HandledSNO(80812, //scoundrel
	194263, //mystic
	146980, //kulle ghost 1
	105681, //kulle ghost 2
	61544, //jeweler
	220114, //hakan projection
	205746, 205756, //parrots
	215103, //Diablo_VO
	143502 //hell_portal_summoner
	)]
	public class Scoundrel : NPC
	{
		public Scoundrel(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.Untargetable] = false;
			this.Attributes[GameAttribute.Operatable] = true;
			this.Attributes[GameAttribute.Disabled] = false;
			this.Attributes[GameAttribute.TeamID] = 0;
			this.WalkSpeed = 0.5f;
		}

	}
}
