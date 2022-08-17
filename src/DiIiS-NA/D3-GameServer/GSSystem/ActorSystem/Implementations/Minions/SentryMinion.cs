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
	class SentryMinion : Minion
	{
		//Changes creature with each rune,
		//RuneSelect(141402, 168815, 150024, 150025, 150026, 150027)

		public static List<int> Sentries = new List<int>() { 141402, 168815, 150024, 150025, 150026, 150027 };

		public SentryMinion(World world, PowerContext context, int SentrySNOId)
			: base(world, SentrySNOId, context.User, null)
		{
			Scale = 1.2f;
			//TODO: get a proper value for this.
			this.WalkSpeed = 0f;
			this.DamageCoefficient = context.ScriptFormula(2);
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

			if (this.Master != null)
			{
				if (this.Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (this.Master as Player).Followers)
						if (Sentries.Contains(fol.Value) && fol.Key != this.GlobalID)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(this.Master as Player).DestroyFollowerById(rm);
				}
			}

			LifeTime = TickTimer.WaitSeconds(world.Game, context.ScriptFormula(0));
		}
	}
}
