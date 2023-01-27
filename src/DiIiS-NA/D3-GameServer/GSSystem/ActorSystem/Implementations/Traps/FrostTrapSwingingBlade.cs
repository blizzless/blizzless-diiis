using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			Field2 = 0x8;
			CollFlags = 0;
			WalkSpeed = 0;
			
			Attributes[GameAttribute.Movement_Scalar] = 0f;
			Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			Spawner = true;
			Attributes[GameAttribute.Invulnerable] = true;
			//(Brain as MonsterBrain).RemovePresetPower(30592);
			//(Brain as MonsterBrain).AddPresetPower(96925);
		}

	}
}
