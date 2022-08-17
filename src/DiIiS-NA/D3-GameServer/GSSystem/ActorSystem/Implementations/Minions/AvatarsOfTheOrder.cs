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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class AvatarMelee : Minion
	{
		public AvatarMelee(World world, PowerContext context, int AvatarID, float damageMult, TickTimer lifeTime)
			: base(world, 345682, context.User, null)
		{
			Scale = 1.2f;
			WalkSpeed *= 5;
			DamageCoefficient = 4f;     //Useless otherwise
			SetBrain(new MinionBrain(this));

			Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;

			Attributes[GameAttribute.Hitpoints_Max] = context.User.Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total] * damageMult;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total] * damageMult;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Pet_Type] = 0x8;

			LifeTime = lifeTime;
		}
	}

	class AvatarRanged : Minion
	{
		public AvatarRanged(World world, PowerContext context, int AvatarID, float damageMult, TickTimer lifeTime)
			: base(world, 369795, context.User, null)
		{
			Scale = 1f;
			WalkSpeed *= 5;
			DamageCoefficient = 4f;         //Useless otherwise
			SetBrain(new MinionBrain(this));
			(Brain as MinionBrain).AddPresetPower(369807);

			Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;

			Attributes[GameAttribute.Hitpoints_Max] = context.User.Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total] * damageMult;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total] * damageMult;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Pet_Type] = 0x8;

			LifeTime = lifeTime;
		}
	}
}
