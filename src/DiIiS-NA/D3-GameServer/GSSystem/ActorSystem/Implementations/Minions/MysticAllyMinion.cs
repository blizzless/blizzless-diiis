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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	public class MysticAllyMinion : Minion
	{
		public new int SummonLimit = 1;

		public MysticAllyMinion(World world, PowerContext context, ActorSno MysticAllyID)
			: base(world, MysticAllyID, context.User, null)
		{
			Scale = 1.35f; //they look cooler bigger :)
						   //TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = 5f;
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = 0x00058676;
			(Brain as MinionBrain).AddPresetPower(169081); //melee_instant


			//Powers are activated manually now
			/*if (context.User.Attributes[GameAttribute.Rune_A, 0x00058676] > 0)
				(Brain as MinionBrain).AddPresetPower(363878); //Fire Ally -> Explosion
			if (context.User.Attributes[GameAttribute.Rune_B, 0x00058676] > 0)
				(Brain as MinionBrain).AddPresetPower(363493); //Water Ally -> Wave
			if (context.User.Attributes[GameAttribute.Rune_C, 0x00058676] > 0)
				(Brain as MinionBrain).AddPresetPower(169715); //Earth Ally -> Boulder*/

			//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
			Attributes[GameAttribute.Hitpoints_Max] = 5f * context.User.Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];
			Attributes[GameAttribute.Attacks_Per_Second] = context.User.Attributes[GameAttribute.Attacks_Per_Second_Total];

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = 2f * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 2f * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
		}
	}
}
