using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class NecromancerSkeleton_A : Minion
	{
		public NecromancerSkeleton_A(MapSystem.World world, ActorSno sno, Actor master)
			: base(world, sno, master, null)
		{
			Scale = 1.35f;
			
			PowerContext context = new PowerContext();
			context.User = master as Player;
			context.World = master.World;
			context.PowerSNO = 453801;

			WalkSpeed *= 3;
			DamageCoefficient = context.ScriptFormula(14) * 2f;
			SetBrain(new MinionBrain(this));
			
			Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Item] = 1;
			Attributes[GameAttribute.Hitpoints_Max_Bonus] = 1;
			Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 1f;
			Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = 0;

			Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;
			Attributes[GameAttribute.Hitpoints_Max] = 20f * (Master as Player).Toon.Level;
			Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];

			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Summoned_By_SNO] = 453801;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = 0.5f * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 0.5f * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			LifeTime = null;// TickTimer.WaitSeconds(world.Game, 6f);
		}
	}
}
