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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	[HandledSNO(ActorSno._unique_captaindaltyn)]
	public class Unique_CaptainDaltyn : Monster
	{
		public Unique_CaptainDaltyn(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.Immune_To_Charm] = true;
			Attributes[GameAttribute.//Blizzless Project 2022 
using_Bossbar] = true;
			Attributes[GameAttribute.InBossEncounter] = true;

			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.TeamID] = 10;


			WalkSpeed = 0.2f;

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
