//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(81068)]
	public class Gharbad : InteractiveNPC
	{
		public Gharbad(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
		}

		public override bool Reveal(Player player)
		{
			this.NotifyConversation(2);

			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);

			if (this.World.Game.CurrentSideQuest != 225253)
				player.Conversations.StartConversation(81069);
			else
				player.Conversations.StartConversation(81099);
		}

		public void Resurrect()
		{
			this.World.SpawnMonster(81342, this.Position);
			this.Destroy();
		}
	}
}
