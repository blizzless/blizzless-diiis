//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class SkeletalMage : Minion
	{
		public bool Rune_Flesh = false;
		//Melee - 87189, 89933 - ranged, 90320 - shaman, skeleton? - 89934
		public SkeletalMage(World world, PowerContext context, int FetishID, int SNO)
			: base(world, SNO, context.User, null)
		{
			Scale = 1.2f; 
			this.WalkSpeed *= 5;
			float UsedEssense = 0f;
			if (context.Rune_A > 0)
				Rune_Flesh = true;
			
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			//(Brain as MinionBrain).AddPresetPower(119166);
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			this.DamageCoefficient = context.ScriptFormula(14) * 2f;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = (context.ScriptFormula(14) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0]);
			//(UsedEssense * 3f)
			if (context.Rune_B > 0)
			{
				UsedEssense = (context as PowerSystem.Implementations.SummonSkeletalMage).Count;
				Attributes[GameAttribute.Damage_Weapon_Min, 0] += (Attributes[GameAttribute.Damage_Weapon_Min, 0] / 100 * 3) * UsedEssense;
			}
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(14) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
			if (context.Rune_C > 0)
				LifeTime = TickTimer.WaitSeconds(world.Game, 8f);
			else
				LifeTime = TickTimer.WaitSeconds(world.Game, 6f);
		}
	}
}
