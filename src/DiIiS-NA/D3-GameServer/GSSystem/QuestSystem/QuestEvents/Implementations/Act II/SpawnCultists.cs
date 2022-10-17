//Blizzless Project 2022 
using System;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnCultists : QuestEvent
	{

		//private static readonly Logger Logger = LogManager.CreateLogger();

		public SpawnCultists()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			//Logger.Trace("SpawnCultists event started");
			StartConversation(world, 169360);
			var spawner = world.GetActorBySNO(ActorSno._spawner_triune_cultist_c_immediately);
			while (spawner != null)
			{
				world.SpawnMonster(ActorSno._triunecultist_c, spawner.Position);
				spawner.Destroy();
				spawner = world.GetActorBySNO(ActorSno._spawner_triune_cultist_c_immediately);
			}
		}

		private bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
			{
				player.Value.Conversations.StartConversation(conversationId);
			}
			return true;
		}

	}
}
