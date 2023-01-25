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
	class LooterPetAnniversary : Minion
	{
		public new int SummonLimit = 1;

		public LooterPetAnniversary(World world, Actor master)
			: base(world, ActorSno._fallenlunatic_a, master, null)
		{
			Scale = 0.75f;
			WalkSpeed *= 5;
			CollFlags = 0;
			DamageCoefficient = 0;
			Attributes[GameAttribute.Invulnerable] = true;
			Attributes[GameAttribute.Is_Helper] = true;
			SetBrain(new LooterBrain(this, true));

			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Pet_Type] = 0x8;
			//Pet_Owner and Pet_Creator seems to be 0
		}

		public override bool Reveal(Player player)
		{
			if (World.IsPvP)
				return false;
			return base.Reveal(player);
		}
	}
}
