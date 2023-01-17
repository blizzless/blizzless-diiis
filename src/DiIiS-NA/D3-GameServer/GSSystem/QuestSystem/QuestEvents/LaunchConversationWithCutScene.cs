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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class LaunchConversationWithCutScene : QuestEvent
	{

		int ConversationId = -1;
		ActorSno ActorSNO = ActorSno.__NONE;

		public LaunchConversationWithCutScene(int convSNOid, ActorSno ActorSno = ActorSno.__NONE)
			: base(0)
		{
			this.ConversationId = convSNOid;
			ActorSNO = ActorSno;
		}

		public override void Execute(MapSystem.World world)
		{
			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(this.ConversationId);
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				if (ActorSNO != ActorSno.__NONE)
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSNO).DynamicID(plr), Duration = 1f, Snap = false });
				foreach (var actor in world.Actors.Values)
					if (actor is not ActorSystem.Gizmo)
						actor.Reveal(plr);
			}
		}
	}
}
