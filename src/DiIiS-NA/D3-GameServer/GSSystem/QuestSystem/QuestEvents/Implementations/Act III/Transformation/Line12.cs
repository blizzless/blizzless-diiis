using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Line12 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line12()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			StartConversation(world, 195769);
			var Adria = world.GetActorBySNO(ActorSno._adria_event47);
			var Portal = world.GetActorBySNO(ActorSno._townportal_red);
			//TODO: Связать с 13 и всю цепочку адрии надо сделать
			foreach (var plr in world.Players.Values) {
				Adria.Unreveal(plr); 
				Portal.Unreveal(plr);
			}
			Adria.SetVisible(false);
			Adria.Hidden = true;
			Portal.SetVisible(false);
			Portal.Hidden = true;

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
