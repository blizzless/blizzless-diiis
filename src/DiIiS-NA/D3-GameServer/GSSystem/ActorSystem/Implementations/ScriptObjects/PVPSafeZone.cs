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
	[HandledSNO(275752)] //HighScoringZone
	public class PVPSafeZone : Monster
	{
		public PVPSafeZone(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Scale = 1.5f;
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
			this.Attributes[GameAttribute.Disabled] = true;
			this.WalkSpeed = 0f;
		}

	}
}
