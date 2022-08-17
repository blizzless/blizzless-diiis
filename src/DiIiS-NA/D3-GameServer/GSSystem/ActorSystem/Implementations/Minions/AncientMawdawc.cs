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
	class AncientMawdawc : Minion
	{
		public new int SummonLimit = 1;

		public AncientMawdawc(World world, PowerContext context, int AncientsID)
			: base(world, 90536, context.User, null)
		{
			Scale = 1.2f; //they look cooler bigger :)
						  //TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = context.ScriptFormula(11);
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			(Brain as MinionBrain).AddPresetPower(30592);  //Weapon_Instant
			(Brain as MinionBrain).AddPresetPower(187092); //basic melee
			(Brain as MinionBrain).AddPresetPower(168827); //Seismic Slam //Only Active with Rune_C
			(Brain as MinionBrain).AddPresetPower(168828); //Weapon Throw
														   //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
														   //Attributes[GameAttribute.Hitpoints_Max] = 20f;
														   //Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			if (this.Master != null)
			{
				if (this.Master is Player)
				{
					if ((this.Master as Player).Followers.Values.Count(a => a == 90536) > 1)
						(this.Master as Player).DestroyFollower(90536);
				}
			}

			LifeTime = TickTimer.WaitSeconds(world.Game, 30f);
		}
	}
}
