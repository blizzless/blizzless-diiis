using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Line10 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line10()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			StartConversation(world, 195764);

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
