using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using System.Collections.Generic;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class GargantuanMinion : Minion
	{
		public GargantuanMinion(World world, PowerContext context, int GargID)
			: base(world, ActorSno._wd_gargantuan, context.User, null)
		{
			Scale = 1f;
			//TODO: get a proper value for this.
			WalkSpeed *= 5;
			DamageCoefficient = context.ScriptFormula(24) * 2f;
			SetBrain(new MinionBrain(this));
			Attributes[GameAttributes.Summoned_By_SNO] = context.PowerSNO;
			//(Brain as MinionBrain).AddPresetPower(30005);
			//(Brain as MinionBrain).AddPresetPower(30001);
			(Brain as MinionBrain).AddPresetPower(30592);
			//(Brain as MinionBrain).AddPresetPower(30550);
			if (context.Rune_B > 0)
				(Brain as MinionBrain).AddPresetPower(121942); // Witchdoctor_Gargantuan_Cleave
			if ((context.User as Player).SkillSet.HasPassive(208563)) //ZombieHandler (wd)
			{
				Attributes[GameAttributes.Hitpoints_Max] *= 1.2f;
				Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max];
			}
			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttributes.Damage_Weapon_Min, 0] = context.ScriptFormula(24) * context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = context.ScriptFormula(24) * context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0];

			if ((context.User as Player).SkillSet.HasPassive(340909)) //MidnightFeast (wd)
			{
				Attributes[GameAttributes.Damage_Weapon_Min, 0] *= 1.5f;
			}

			Attributes[GameAttributes.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			if (Master != null)
			{
				if (Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (Master as Player).Followers)
						if (fol.Value == SNO && fol.Key != GlobalID)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(Master as Player).DestroyFollowerById(rm);
				}
			}
		}
	}
}
