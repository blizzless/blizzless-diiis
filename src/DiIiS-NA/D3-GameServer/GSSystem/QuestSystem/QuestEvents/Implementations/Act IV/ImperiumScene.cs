//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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
	class ImperiumScene : QuestEvent
	{

		int ConversationId = -1;
		int ActorSNO = -1;

		public ImperiumScene()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
			List<Vector3D> Plants = new List<Vector3D> { };
			List<Actor> Demons = new List<Actor> { };
			var Hope = world.GetActorBySNO(114074);
			var Fate = world.GetActorBySNO(112768);
			//Vector3D PlantToImperius = world.GetActorBySNO(205570).Position;
			var Imperius = world.GetActorBySNO(195606);
			
			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(this.ConversationId);
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)Imperius.DynamicID(plr), Duration = 1f, Snap = false });
				
			}

			#region Удаляем всех лишних.
			foreach (var Mob in world.GetActorsBySNO(106708))
			{
				Plants.Add(Mob.Position);
				Mob.Destroy();
			}

			foreach (var Mob in world.GetActorsBySNO(114074)) //Hope
				if (Mob != Hope)
					Mob.Destroy();
			foreach (var Mob in world.GetActorsBySNO(112768)) //Fate
				if (Mob != Fate)
					Mob.Destroy();
			foreach (var Mob in world.GetActorsBySNO(205570)) //Angels
				Mob.Destroy();

			#endregion

			foreach (var plr in world.Players.Values)
			{
				Imperius.SetVisible(false); Imperius.Hidden = true; Imperius.Unreveal(plr);
				Hope.SetVisible(false); Hope.Hidden = true; Hope.Unreveal(plr);
				Fate.SetVisible(false); Fate.Hidden = true; Fate.Unreveal(plr);
			}
			//Начинаем спектакль
			//1 шаг - спауним первого черта
			var FirstMob = world.SpawnMonster(106708, Plants[0]);
			(FirstMob as Monster).Brain.DeActivate();

			//2 шаг - убиваем монстра
			Task.Delay(2000).ContinueWith(delegate
			{
				(FirstMob as Monster).Kill();

				//3 шаг добавляем остальных чертов
				Task.Delay(3000).ContinueWith(delegate
				{
					foreach (var plant in Plants)
					{
						var Demon = world.SpawnMonster(106708, plant);
						Demon.PlayAnimation(11, 159227, 1, 6);
						Demons.Add(Demon);
					}
					Task.Delay(3000).ContinueWith(delegate
					{
						foreach (var plr in world.Players.Values)
						{
							Imperius.SetVisible(true); Imperius.Hidden = false; Imperius.Reveal(plr);
							Hope.SetVisible(true); Hope.Hidden = false; Hope.Reveal(plr);
							Fate.SetVisible(true); Fate.Hidden = false; Fate.Reveal(plr);
						}
						Imperius.PlayActionAnimation(205702);
						Fate.PlayActionAnimation(204712);
						Hope.PlayActionAnimation(204712);
						//Fate.PlayAnimation(11, 204712, 1);
						Task.Delay(3000).ContinueWith(delegate
						{
							foreach (var Demon in Demons) (Demon as Monster).Kill();
							world.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
							{
								ActorID = Imperius.DynamicID(plr),
								AnimationSNO = 205703,
							}, Imperius);

							StartConversation(world, 196579);
							world.Game.QuestManager.Advance();
							
						});
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
