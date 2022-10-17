//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Monsters
{
	//89578 GlobalId: 1015703058 Position: x:338 y:320.78137 z:-11.422008 Name: a1dun_leor_firewall1
	[HandledSNO(ActorSno._a1dun_leor_firewall1, ActorSno._a1dun_leor_firewall2)]
	public class A1dun_firewall : Monster
	{
		public A1dun_firewall(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;

			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.Spawner = true;
			this.Attributes[GameAttribute.Invulnerable] = true;

			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
			(Brain as MonsterBrain).RemovePresetPower(30592);
			//(Brain as MonsterBrain).AddPresetPower(96925);
			(Brain as MonsterBrain).AddPresetPower(223284);
			this.WalkSpeed = 0.0f;
			//[224754] [EffectGroup] a4dun_spire_firewall_slideExplode
			//[223284] [Power] a4dun_spire_firewall
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

}
