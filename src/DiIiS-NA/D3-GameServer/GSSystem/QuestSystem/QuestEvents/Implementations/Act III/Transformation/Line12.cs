//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
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
			var Adria = world.GetActorBySNO(195378);
			var Portal = world.GetActorBySNO(5660);
			//TODO: Связать с 13 и всю цепочку адрии надо сделать
			foreach (var plr in world.Players.Values) { Adria.Unreveal(plr); Portal.Unreveal(plr); }
			Adria.SetVisible(false); Adria.Hidden = true; Portal.SetVisible(false); Portal.Hidden = true;

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
