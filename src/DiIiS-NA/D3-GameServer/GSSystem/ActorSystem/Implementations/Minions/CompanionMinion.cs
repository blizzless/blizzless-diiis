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
	public class CompanionMinion : Minion
	{
		//Changes creature with each rune,
		//RuneSelect(133741, 173827, 181748, 159098, 159102, 159144)

		public new int SummonLimit = 1;

		private static readonly List<ActorSno> Companions = new List<ActorSno>() {
			ActorSno._dh_companion,
			ActorSno._dh_companion_spider, 
			ActorSno._dh_companion_boar, 
			ActorSno._dh_companion_runec, 
			ActorSno._dh_companion_runed,
			ActorSno._dh_companion_ferret
		};

		public CompanionMinion(World world, PowerContext context, ActorSno CompanionSNO)
			: base(world, CompanionSNO, context.User, null)
		{
			Scale = 1.2f;
			if (context.User.Attributes[GameAttribute.Rune_B, 0x000592ff] > 0) Scale = 2f;  //Boar
			if (context.User.Attributes[GameAttribute.Rune_C, 0x000592ff] > 0) Scale = 2f;  //Wolf
																							//TODO: get a proper value for this.
			WalkSpeed *= 5;
			DamageCoefficient = context.ScriptFormula(0);
			Attributes[GameAttribute.Invulnerable] = true;
			Attributes[GameAttribute.Is_Helper] = true;
			Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			if (CompanionSNO == ActorSno._dh_companion_ferret)
				SetBrain(new LooterBrain(this, false));
			else
			{
				SetBrain(new MinionBrain(this));
				(Brain as MinionBrain).AddPresetPower(169081); //melee_instant

				if (context.User.Attributes[GameAttribute.Rune_A, 0x000592ff] > 0)  //Spider
					(Brain as MinionBrain).AddPresetPower(133887); //cleave				

				if (context.User.Attributes[GameAttribute.Rune_B, 0x000592ff] > 0)  //Boar
					(Brain as MinionBrain).AddPresetPower(133887); //cleave

				if (context.User.Attributes[GameAttribute.Rune_C, 0x000592ff] > 0)  //Wolf
					(Brain as MinionBrain).AddPresetPower(133887); //cleave					
																   //(Brain as MinionBrain).AddPresetPower(133887); //ChargeAttack
			}

			Attributes[GameAttribute.Attacks_Per_Second] = context.User.Attributes[GameAttribute.Attacks_Per_Second_Total];

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(0) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(0) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
		}
	}
}
