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

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class LaunchConversation : QuestEvent
	{

		int ConversationId = -1;

		public LaunchConversation(int convSNOid)
			: base(0)
		{
			ConversationId = convSNOid;
		}

		public override void Execute(MapSystem.World world)
		{
			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(ConversationId);
			}
		}
	}
}
