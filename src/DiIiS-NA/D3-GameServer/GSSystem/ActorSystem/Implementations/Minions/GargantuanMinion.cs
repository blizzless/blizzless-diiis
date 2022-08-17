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
	class GargantuanMinion : Minion
	{
		public GargantuanMinion(World world, PowerContext context, int GargID)
			: base(world, 122305, context.User, null)
		{
			Scale = 1f;
			//TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = context.ScriptFormula(24) * 2f;
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			//(Brain as MinionBrain).AddPresetPower(30005);
			//(Brain as MinionBrain).AddPresetPower(30001);
			(Brain as MinionBrain).AddPresetPower(30592);
			//(Brain as MinionBrain).AddPresetPower(30550);
			if (context.Rune_B > 0)
				(Brain as MinionBrain).AddPresetPower(121942); // Witchdoctor_Gargantuan_Cleave
			if ((context.User as Player).SkillSet.HasPassive(208563)) //ZombieHandler (wd)
			{
				Attributes[GameAttribute.Hitpoints_Max] *= 1.2f;
				Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];
			}
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(24) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(24) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			if ((context.User as Player).SkillSet.HasPassive(340909)) //MidnightFeast (wd)
			{
				Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 1.5f;
			}

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			if (this.Master != null)
			{
				if (this.Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (this.Master as Player).Followers)
						if (fol.Value == 122305 && fol.Key != this.GlobalID)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(this.Master as Player).DestroyFollowerById(rm);
				}
			}
		}
	}
}
