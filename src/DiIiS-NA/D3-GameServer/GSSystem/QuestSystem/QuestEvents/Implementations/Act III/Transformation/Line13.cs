using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using System;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Line13 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line13()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			var Leah = world.GetActorBySNO(ActorSno._leah_event47);
			var BPortal = world.GetActorBySNO(ActorSno._event47_bigportal);//event47_BigPortal - 188441
			StartConversation(world, 195776);
			float facingAngle = MovementHelpers.GetFacingAngle(Leah, BPortal);
			Leah.Move(BPortal.Position, facingAngle);

			Task.Delay(7000).ContinueWith(delegate 
			{
				Leah.PlayActionAnimation(AnimationSno.leah_bss_event_lvlup);
				BPortal.Hidden = false;
				BPortal.SetVisible(true);
				foreach (var plr in world.Players.Values)
					BPortal.Reveal(plr);

				Task.Delay(2000).ContinueWith(delegate
				{
					Leah.PlayActionAnimation(AnimationSno.leah_bss_event_open_portal_out);
					Task.Delay(3000).ContinueWith(delegate
					{
						
						world.Game.QuestManager.Advance();
						foreach (var actor in world.GetActorsBySNO(ActorSno._leah_event47)) actor.Destroy(); //Лея
						foreach (var plr in world.Players.Values)
						{
							plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
						}
					});
				});
			});

			
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
