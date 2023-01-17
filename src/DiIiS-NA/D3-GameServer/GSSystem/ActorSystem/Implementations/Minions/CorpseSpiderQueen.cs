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
	class CorpseSpiderQueen : Minion
	{
		//107031, 106731, 106749, 107067, 107107, 107112

		public new int SummonLimit = 1;

		public CorpseSpiderQueen(World world, PowerContext context, int SpiderID)
			: base(world, ActorSno._witchdoctor_corpsespider_indigorune, context.User, null)
		{
			Scale = 0.7f; //they look cooler bigger :)
						  //TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = context.ScriptFormula(16) * 2f;
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			(Brain as MinionBrain).AddPresetPower(30592); //melee_instant
														  //(Brain as MinionBrain).AddPresetPower(30005); //AINearby
			if (context.Rune_C > 0)
				(Brain as MinionBrain).AddPresetPower(107103); //Spider_leap
			if (context.Rune_B > 0)
				//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
				//Attributes[GameAttribute.Hitpoints_Max] = 20f;
				//Attributes[GameAttribute.Hitpoints_Cur] = 20f;
				//Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

				Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(16) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(16) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			if (this.Master != null)
			{
				if (this.Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (this.Master as Player).Followers)
						if (fol.Value == SNO && fol.Key != this.GlobalID)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(this.Master as Player).DestroyFollowerById(rm);
				}
			}

			LifeTime = TickTimer.WaitSeconds(world.Game, 15f);
		}
	}
}
