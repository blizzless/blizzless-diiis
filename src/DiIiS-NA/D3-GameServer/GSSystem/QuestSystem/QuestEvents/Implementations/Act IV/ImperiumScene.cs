using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var Hope = world.GetActorBySNO(ActorSno._hope);
			var Fate = world.GetActorBySNO(ActorSno._fate);
			//Vector3D PlantToImperius = world.GetActorBySNO(205570).Position;
			var Imperius = world.GetActorBySNO(ActorSno._imperius);
			
			foreach (var plr in world.Players.Values)
			{
				plr.Conversations.StartConversation(ConversationId);
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)Imperius.DynamicID(plr), Duration = 1f, Snap = false });
				
			}

			#region Удаляем всех лишних.
			foreach (var Mob in world.GetActorsBySNO(ActorSno._bigred_a))
			{
				Plants.Add(Mob.Position);
				Mob.Destroy();
			}

			foreach (var Mob in world.GetActorsBySNO(ActorSno._hope)) //Hope
				if (Mob != Hope)
					Mob.Destroy();
			foreach (var Mob in world.GetActorsBySNO(ActorSno._fate)) //Fate
				if (Mob != Fate)
					Mob.Destroy();
			foreach (var Mob in world.GetActorsBySNO(ActorSno._angel_trooper_a_tyraelpurpose)) //Angels
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
			var FirstMob = world.SpawnMonster(ActorSno._bigred_a, Plants[0]);
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
						var Demon = world.SpawnMonster(ActorSno._bigred_a, plant);
						Demon.PlayAnimation(11, AnimationSno.bigred_hole_spawn_02, 1, 6);
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
						Imperius.PlayActionAnimation(AnimationSno.omninpc_male_imperius_tyreal_purpose_fall_to_knee);
						Fate.PlayActionAnimation(AnimationSno.omninpc_male_fate_spawn_01);
						Hope.PlayActionAnimation(AnimationSno.omninpc_male_fate_spawn_01);
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
