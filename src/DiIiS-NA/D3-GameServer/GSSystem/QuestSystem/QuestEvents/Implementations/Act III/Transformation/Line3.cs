using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Line3 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line3()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			StartConversation(world, 195723);
			
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
