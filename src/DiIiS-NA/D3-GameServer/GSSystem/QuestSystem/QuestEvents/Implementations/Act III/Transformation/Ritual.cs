//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
			
			var RitualCircle = world.GetActorBySNO(ActorSno._event47_groundrune);
			var Leah = world.GetActorBySNO(ActorSno._leah_event47);
			var NStone = world.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone);
			RitualCircle.PlayActionAnimation(AnimationSno.emitter_event47_groundrune_stage01); // stage1
			Task.Delay(1500).ContinueWith(delegate
			{
				RitualCircle.PlayActionAnimation(AnimationSno.emitter_event47_groundrune_stage02); // stage2
				Leah.PlayActionAnimation(AnimationSno.leah_bss_event_bound_shake);
				Task.Delay(1500).ContinueWith(delegate
				{
					RitualCircle.PlayActionAnimation(AnimationSno.emitter_event47_groundrune_stage03); // stage3

					Task.Delay(1500).ContinueWith(delegate
					{
						RitualCircle.PlayActionAnimation(AnimationSno.emitter_event47_groundrune_stage04); // stage4

						Task.Delay(1500).ContinueWith(delegate
						{
							RitualCircle.PlayEffectGroup(199076);
							NStone.Destroy();
							StartConversation(world, 195749);
							Leah.PlayActionAnimation(AnimationSno.leah_bss_event_kneel_to_getup);
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
