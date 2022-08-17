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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Monsters
{
	#region Unique_CaptainDaltyn
	[HandledSNO(156801)]
	public class Unique_CaptainDaltyn : Monster
	{
		public Unique_CaptainDaltyn(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.Immune_To_Charm] = true;
			this.Attributes[GameAttribute.//Blizzless Project 2022 
using_Bossbar] = true;
			this.Attributes[GameAttribute.InBossEncounter] = true;

			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this.Attributes[GameAttribute.TeamID] = 10;


			this.WalkSpeed = 0.2f;

		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Boss;
			}
			set
			{
		
			}
		}


	}
	#endregion

}
