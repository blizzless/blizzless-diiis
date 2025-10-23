using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

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
			var Tyrael = world.GetActorBySNO(ActorSno._tyrael_event47, true);

			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(195719);
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(Tyrael.SNO).DynamicID(plr), Duration = 1f, Snap = false });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraZoomMessage() { Zoom = 0.5f, Duration = 1f, Snap = false });
				//foreach (var actor in world.Actors.Values)
				//	if (actor! is ActorSystem.Gizmo)
				//		actor.Reveal(plr);
			}
		}
	}
}
