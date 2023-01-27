using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
    class LeahTransformation_Line11 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line11()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			StartConversation(world, 195767);
			var Leah = world.GetActorBySNO(ActorSno._leah_event47);
			Leah.PlayActionAnimation(AnimationSno.leah_bss_event_lvlup);
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
