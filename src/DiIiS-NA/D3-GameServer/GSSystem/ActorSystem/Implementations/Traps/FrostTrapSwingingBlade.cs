//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    //SNOId:404663 GlobalId: 1018503124 Position: x:302.6683 y:457.36 z:0 Name: P4_Ruins_Frost_Trap_Swinging_Blade
    [HandledSNO(ActorSno._p4_ruins_frost_trap_swinging_blade)]
    public class FrostTrapSwingingBlade : Monster
    {
		public FrostTrapSwingingBlade(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			
			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.Spawner = true;
			this.Attributes[GameAttribute.Invulnerable] = true;
			//(Brain as MonsterBrain).RemovePresetPower(30592);
			//(Brain as MonsterBrain).AddPresetPower(96925);
		}

	}
}
