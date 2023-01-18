//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	class FirstCatapult : QuestEvent
	{
		public bool raised = false;

		public FirstCatapult()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Logger.Debug("FirstCatapult event started");
			if (!raised)
			{
				var catapult = (world.GetActorBySNO(ActorSno._a3dun_wall_lift_gategizmolong) as ActIIICatapult);
				catapult.Raise();
				world.Game.QuestManager.CompleteObjective(0);
				raised = true;
				StartConversation(world, 191877);
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
