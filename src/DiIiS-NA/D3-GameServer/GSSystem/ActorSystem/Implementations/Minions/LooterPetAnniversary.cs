using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
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
			Attributes[GameAttributes.Invulnerable] = true;
			Attributes[GameAttributes.Is_Helper] = true;
			SetBrain(new LooterBrain(this, true));

			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttributes.Pet_Type] = 0x8;
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
