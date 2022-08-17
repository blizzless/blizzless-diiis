//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class FetishShaman : Minion
	{
		//Melee - 87189, 89933 - ranged, 90320 - shaman, skeleton? - 89934
		public FetishShaman(World world, PowerContext context, int FetishID)
			: base(world, 90320, context.User, null)
		{
			Scale = 1.2f; //they look cooler bigger :)
						  //TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = context.ScriptFormula(11) * 2f;
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			(Brain as MinionBrain).AddPresetPower(118442); //fetisharmy_shaman.pow
														   //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
														   //Attributes[GameAttribute.Hitpoints_Max] = 20f;
														   //Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			LifeTime = TickTimer.WaitSeconds(world.Game, 20f);
		}
	}
}
