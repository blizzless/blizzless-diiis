using System.Linq;
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
	class SentryMinion : Minion
	{
		//Changes creature with each rune,
		//RuneSelect(141402, 168815, 150024, 150025, 150026, 150027)

		public static readonly List<ActorSno> Sentries = new List<ActorSno>()
		{
			ActorSno._dh_sentry,
			ActorSno._dh_sentry_tether,
			ActorSno._dh_sentry_addsduration,
			ActorSno._dh_sentry_addsmissiles,
			ActorSno._dh_sentry_addsheals,
			ActorSno._dh_sentry_addsshield
		};

		public SentryMinion(World world, PowerContext context, ActorSno SentrySNO)
			: base(world, SentrySNO, context.User, null)
		{
			Scale = 1.2f;
			//TODO: get a proper value for this.
			WalkSpeed = 0f;
			DamageCoefficient = context.ScriptFormula(2);
			SetBrain(new MinionBrain(this));
			(Brain as MinionBrain).AddPresetPower(129661); //DemonHunter_Sentry_TurretAttack.pow
														   //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
														   //Attributes[GameAttribute.Hitpoints_Max] = 20f;
														   //Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(2) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(2) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			if (Master != null)
			{
				if (Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (Master as Player).Followers)
						if (Sentries.Contains(fol.Value) && fol.Key != GlobalID)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(Master as Player).DestroyFollowerById(rm);
				}
			}

			LifeTime = TickTimer.WaitSeconds(world.Game, context.ScriptFormula(0));
		}
	}
}
