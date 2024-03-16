using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._gharbad_the_weak_ghost)]
	public class Gharbad : InteractiveNPC
	{
		public Gharbad(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttributes.Invulnerable] = true;
		}

		public override bool Reveal(Player player)
		{
			NotifyConversation(2);

			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);

			if (World.Game.CurrentSideQuest != 225253)
				player.Conversations.StartConversation(81069);
			else
				player.Conversations.StartConversation(81099);
		}

		public void Resurrect()
		{
			World.SpawnMonster(ActorSno._goatmutant_melee_a_unique_gharbad, Position);
			Destroy();
		}
	}
}
