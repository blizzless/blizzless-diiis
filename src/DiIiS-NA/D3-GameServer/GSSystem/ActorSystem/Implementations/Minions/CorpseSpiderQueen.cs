using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
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
			WalkSpeed *= 5;
			DamageCoefficient = context.ScriptFormula(16) * 2f;
			SetBrain(new MinionBrain(this));
			Attributes[GameAttributes.Summoned_By_SNO] = context.PowerSNO;
			(Brain as MinionBrain).AddPresetPower(30592); //melee_instant
														  //(Brain as MinionBrain).AddPresetPower(30005); //AINearby
			if (context.Rune_C > 0)
				(Brain as MinionBrain).AddPresetPower(107103); //Spider_leap
			if (context.Rune_B > 0)
				//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
				//Attributes[GameAttribute.Hitpoints_Max] = 20f;
				//Attributes[GameAttribute.Hitpoints_Cur] = 20f;
				//Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

				Attributes[GameAttributes.Damage_Weapon_Min, 0] = context.ScriptFormula(16) * context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = context.ScriptFormula(16) * context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0];

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

			LifeTime = TickTimer.WaitSeconds(world.Game, 15f);
		}
	}
}
