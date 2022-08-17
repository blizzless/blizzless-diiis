//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Ritual : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Ritual()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Адрия - Начинает кастовать, камень поднимается, а Лию начинает колбасить, после диалога зажигается круг с пентой и отдаляется камера, через секунд взрыв.
			
			var RitualCircle = world.GetActorBySNO(193601);
			var Leah = world.GetActorBySNO(195376);
			var NStone = world.GetActorBySNO(156328);
			RitualCircle.PlayActionAnimation(194705); // stage1
			Task.Delay(1500).ContinueWith(delegate
			{
				RitualCircle.PlayActionAnimation(194706); // stage2
				Leah.PlayActionAnimation(205941);
				Task.Delay(1500).ContinueWith(delegate
				{
					RitualCircle.PlayActionAnimation(194707); // stage3

					Task.Delay(1500).ContinueWith(delegate
					{
						RitualCircle.PlayActionAnimation(194709); // stage4

						Task.Delay(1500).ContinueWith(delegate
						{
							RitualCircle.PlayEffectGroup(199076);
							NStone.Destroy();
							StartConversation(world, 195749);
							Leah.PlayActionAnimation(194492);
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
