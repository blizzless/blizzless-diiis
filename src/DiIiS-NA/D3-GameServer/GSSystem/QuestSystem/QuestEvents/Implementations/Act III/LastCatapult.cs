//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LastCatapult : QuestEvent
	{
		public bool raised = false;

		public LastCatapult()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (!raised)
			{
				var catapult = (world.GetActorBySNO(149529) as actIIICatapult);
				catapult.Raise();
				world.Game.QuestManager.CompleteObjective(0);
				raised = true;
				StartConversation(world, 191896);
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
