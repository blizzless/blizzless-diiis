//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
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
			var Leah = world.GetActorBySNO(195376);
			var BPortal = world.GetActorBySNO(188441);//event47_BigPortal - 188441
			StartConversation(world, 195776);
			float facingAngle = MovementHelpers.GetFacingAngle(Leah, BPortal);
			Leah.Move(BPortal.Position, facingAngle);

			Task.Delay(7000).ContinueWith(delegate 
			{
				Leah.PlayActionAnimation(201990);
				BPortal.Hidden = false;
				BPortal.SetVisible(true);
				foreach (var plr in world.Players.Values)
					BPortal.Reveal(plr);

				Task.Delay(2000).ContinueWith(delegate
				{
					Leah.PlayActionAnimation(208444);
					Task.Delay(3000).ContinueWith(delegate
					{
						
						world.Game.QuestManager.Advance();
						foreach (var actor in world.GetActorsBySNO(195376)) actor.Destroy(); //Лея
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
