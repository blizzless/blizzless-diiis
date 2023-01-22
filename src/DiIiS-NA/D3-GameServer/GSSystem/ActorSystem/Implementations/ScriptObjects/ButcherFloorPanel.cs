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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	//{[Actor] [Type: Monster] SNOId:201426 GlobalId: 1015903034 Position: x:121.353 y:121.402 z:-0.107267 Name: ButcherLair_FloorPanel_MidMiddle_Base}
	[HandledSNO(
		ActorSno._butcherlair_floorpanel_lowerright_base,
		ActorSno._butcherlair_floorpanel_upperright_base,
		ActorSno._butcherlair_floorpanel_uppermid_base,
		ActorSno._butcherlair_floorpanel_midmiddle_base,
		ActorSno._butcherlair_floorpanel_upperleft_base,
		ActorSno._butcherlair_floorpanel_lowerleft_base,
		ActorSno._butcherlair_floorpanel_lowermid_base
	)]
	public class ButcherFloorPanel : Monster
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public ButcherFloorPanel(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x8;
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttribute.Invulnerable] = true;
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(96925);
		}

	}

	[HandledSNO(ActorSno._a1dun_leor_bigfiregrate)]
	public class LeorFireGrate : Monster
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public LeorFireGrate(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x8;
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttribute.Movement_Scalar] = 0f;
			Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			Spawner = true;
			Attributes[GameAttribute.Invulnerable] = true;
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(108017);
		}

	}
}
