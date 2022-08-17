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
	class LeahTransformation_Line1 : QuestEvent
	{
		public LeahTransformation_Line1()
			: base(0)
		{
			
		}

		public override void Execute(MapSystem.World world)
		{
			var Tyrael = world.GetActorBySNO(195377, true);

			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(195719);
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(Tyrael.ActorSNO.Id).DynamicID(plr), Duration = 1f, Snap = false });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraZoomMessage() { Zoom = 0.5f, Duration = 1f, Snap = false });
				//foreach (var actor in world.Actors.Values)
				//	if (actor! is ActorSystem.Gizmo)
				//		actor.Reveal(plr);
			}
		}
	}
}
