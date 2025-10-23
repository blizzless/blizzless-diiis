using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class NecromancerSkeleton_A : Minion
	{
		public NecromancerSkeleton_A(MapSystem.World world, ActorSno sno, Actor master)
			: base(world, sno, master, null)
		{
			Scale = 1.35f;
			
			PowerContext context = new()
			{
				User = master as Player,
				World = master.World,
				PowerSNO = 453801
			};

			WalkSpeed *= 3;
			DamageCoefficient = context.ScriptFormula(14) * 2f;
			SetBrain(new MinionBrain(this));
			
			Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Item] = 1;
			Attributes[GameAttributes.Hitpoints_Max_Bonus] = 1;
			Attributes[GameAttributes.Hitpoints_Factor_Vitality] = 1f;
			Attributes[GameAttributes.Hitpoints_Regen_Per_Second] = 0;

			Attributes[GameAttributes.Core_Attributes_From_Item_Bonus_Multiplier] = 1;
			Attributes[GameAttributes.Hitpoints_Max] = 20f * ((Player) Master).Toon.Level;
			Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];

			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttributes.Summoned_By_SNO] = 453801;
			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 0.5f * context!.User!.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 0.5f * context!.User!.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttributes.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			LifeTime = null;// TickTimer.WaitSeconds(world.Game, 6f);
		}
	}
}
