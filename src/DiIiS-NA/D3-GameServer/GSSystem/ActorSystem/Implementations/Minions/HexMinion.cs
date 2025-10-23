using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
	class HexMinion : Minion
	{
		public HexMinion(World world, PowerContext context, int HexID)
			: base(world, ActorSno._fetish_hex, context.User, null)
		{
			Scale = 1f;
			//TODO: get a proper value for this.
			WalkSpeed *= 5;
			SetBrain(new MinionBrain(this));
			DamageCoefficient = 1f * 2f;
			Attributes[GameAttributes.Summoned_By_SNO] = context.PowerSNO;
			(Brain as MinionBrain).AddPresetPower(196974); //chicken_walk.pow
			(Brain as MinionBrain).AddPresetPower(188442); //explode.pow
			(Brain as MinionBrain).AddPresetPower(107301); //Fetish.pow
			(Brain as MinionBrain).AddPresetPower(107742); //Heal

			//Attributes[GameAttribute.Hitpoints_Max] = 5f;
			//Attributes[GameAttribute.Hitpoints_Cur] = 5f;
			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 7f;

			Attributes[GameAttributes.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
			LifeTime = TickTimer.WaitSeconds(world.Game, 12f);
		}

	}
}
