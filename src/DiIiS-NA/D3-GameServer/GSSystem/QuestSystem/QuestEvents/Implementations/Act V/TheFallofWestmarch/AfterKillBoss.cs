using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class AfterKillBoss : QuestEvent
	{
		public AfterKillBoss()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			var Tyrael = world.ShowOnlyNumNPC(ActorSno._x1_tyrael_hurt, 0) as ActorSystem.InteractiveNPC;

			foreach (var Tyr in world.GetActorsBySNO(ActorSno._x1_tyrael_hurt))
			{
				(Tyr as ActorSystem.InteractiveNPC).Conversations.Clear();
				Tyr.Attributes[GameAttributes.Conversation_Icon, 0] = 2;
				Tyr.Attributes.BroadcastChangedIfRevealed();
				AddQuestConversation(Tyr, 252100);
			}

			foreach (var act in world.GetActorsBySNO(
				ActorSno._x1_death_orb_little,
				ActorSno._x1_westmarch_cath_int_debriscenter,
				ActorSno._x1_westm_cath_debrissheets_02,
				ActorSno._x1_westm_cath_debrissheets_03,
				ActorSno._x1_westm_cath_debrissheets_04,
				ActorSno._x1_westm_cath_debrissheets_05,
				ActorSno.__x1_westm_cath_debrissheets_06
				))
            {
                act.Destroy();
            }

			world.Game.QuestManager.Advance();

		}
	}
}
