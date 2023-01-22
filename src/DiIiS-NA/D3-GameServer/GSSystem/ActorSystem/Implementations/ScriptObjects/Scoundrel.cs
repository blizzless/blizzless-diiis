//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	[HandledSNO(
		ActorSno._scoundrelnpc, //scoundrel
		ActorSno._mystic_b, //mystic
		ActorSno._zoltunkulletownhead, //kulle ghost 1
		ActorSno._kullevoiceover, //kulle ghost 2
		ActorSno._intro_jeweler, //jeweler
		ActorSno._hakanprojection, //hakan projection
		ActorSno._caout_raven_perched_a, ActorSno._caout_raven_pecking_a, //parrots
		ActorSno._diablo_vo, //Diablo_VO
		ActorSno._a4_heaven_hellportal_summoner_loc //hell_portal_summoner
	)]
	public class Scoundrel : NPC
	{
		public Scoundrel(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.Untargetable] = false;
			Attributes[GameAttribute.Operatable] = true;
			Attributes[GameAttribute.Disabled] = false;
			Attributes[GameAttribute.TeamID] = 0;
			WalkSpeed = 0.5f;
		}

	}
}
