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
	class MirrorImageMinion : Minion
	{
		public MirrorImageMinion(World world, PowerContext context, int ImageID, float lifetime)
			: base(world, 98010, context.User, null) //male Mirror images
		{
			Scale = 1.2f; //they look cooler bigger :)
						  //TODO: get a proper value for this.
			this.WalkSpeed *= 5;
			this.DamageCoefficient = context.ScriptFormula(11);
			SetBrain(new MinionBrain(this));
			this.Attributes[GameAttribute.Summoned_By_SNO] = context.PowerSNO;
			//TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
			//Attributes[GameAttribute.Hitpoints_Max] = 20f;
			//Attributes[GameAttribute.Hitpoints_Cur] = 20f;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;

			Attributes[GameAttribute.Damage_Weapon_Min, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0

			LifeTime = TickTimer.WaitSeconds(world.Game, lifetime);

			if (this.Master != null && context.ScriptFormula(1) < (this.Master as Player).Followers.Values.Where(f => f == 98010).Count())
			{
				if (this.Master is Player)
				{
					var rem = new List<uint>();
					foreach (var fol in (this.Master as Player).Followers.Where(f => f.Key != this.GlobalID).Take((this.Master as Player).Followers.Values.Where(f => f == 98010).Count() - (int)context.ScriptFormula(1)))
						if (fol.Value == 98010)
							rem.Add(fol.Key);
					foreach (var rm in rem)
						(this.Master as Player).DestroyFollowerById(rm);
				}
			}
		}
	}
}
