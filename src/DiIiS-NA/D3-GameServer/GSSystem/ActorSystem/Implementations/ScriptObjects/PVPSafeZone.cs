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
	[HandledSNO(ActorSno._pvp_murderball_highscoringzone)] //HighScoringZone
	public class PVPSafeZone : Monster
	{
		public PVPSafeZone(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Scale = 1.5f;
			Field2 = 0x8;
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttribute.Invulnerable] = true;
			Attributes[GameAttribute.Disabled] = true;
			WalkSpeed = 0f;
		}

	}
}
