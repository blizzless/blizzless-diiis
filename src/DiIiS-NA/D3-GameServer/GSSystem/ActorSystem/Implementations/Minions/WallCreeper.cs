using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class WallCreeper : Minion
	{
		public new int SummonLimit = 10;

		public WallCreeper(World world, PowerContext context, int creeperID)
			: base(world, ActorSno._wd_wallofzombiesrune_spawn, context.User, null)
		{
			Scale = 1.2f; //they look cooler bigger :)
						  //TODO: get a proper value for this.
			WalkSpeed *= 2;
			DamageCoefficient = 2f * 2f;
			SetBrain(new MinionBrain(this));
			Attributes[GameAttributes.Summoned_By_SNO] = context.PowerSNO;
			//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
			//Attributes[GameAttribute.Hitpoints_Max] = 20f;
			//Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 2f * context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 2f * context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttributes.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			LifeTime = TickTimer.WaitSeconds(world.Game, 5f);
		}
	}
}
