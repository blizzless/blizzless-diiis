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
	class ZombieDog : Minion
	{
		public new int SummonLimit = 4;

		public ZombieDog(World world, Actor master, int dogID, float mul = 1f)
			: base(world, ActorSno._wd_zombiedog, master, null)
		{
			Scale = 1.35f;
			//TODO: get a proper value for this.
			WalkSpeed *= 5;
			DamageCoefficient = mul * 2f;
			SetBrain(new MinionBrain(this));
			Attributes[GameAttribute.Summoned_By_SNO] = 102573;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];


			//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
			//Attributes[GameAttribute.Hitpoints_Max] = 20f;
			//Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			if ((master as Player).SkillSet.HasPassive(208563)) //ZombieHandler (wd)
			{
				Attributes[GameAttribute.Hitpoints_Max] *= 1.2f;
				Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];
			}
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] * mul;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] * mul;

			if ((master as Player).SkillSet.HasPassive(340909)) //MidnightFeast (wd)
			{
				Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 1.5f;
			}

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
			master.Attributes[GameAttribute.Free_Cast, SkillsSystem.Skills.WitchDoctor.Support.Sacrifice] = 1;
			master.Attributes.BroadcastChangedIfRevealed();
		}
	}
}
