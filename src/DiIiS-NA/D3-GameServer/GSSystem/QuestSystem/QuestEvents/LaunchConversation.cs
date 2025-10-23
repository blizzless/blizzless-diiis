using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
