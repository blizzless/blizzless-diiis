//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	//{[Actor] [Type: Monster] SNOId:201426 GlobalId: 1015903034 Position: x:121.353 y:121.402 z:-0.107267 Name: ButcherLair_FloorPanel_MidMiddle_Base}
	[HandledSNO(201464, 201454, 201438, 201426, 201423, 201242, 200969)]
	public class ButcherFloorPanel : Monster
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public ButcherFloorPanel(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(96925);
		}

	}

	[HandledSNO(108012)]
	public class LeorFireGrate : Monster
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public LeorFireGrate(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.Spawner = true;
			this.Attributes[GameAttribute.Invulnerable] = true;
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(108017);
		}

	}
}
