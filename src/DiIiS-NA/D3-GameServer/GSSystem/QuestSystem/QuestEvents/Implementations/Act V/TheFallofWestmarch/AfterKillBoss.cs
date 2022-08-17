//Blizzless Project 2022 
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
			var Tyrael = world.ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;

			foreach (var Tyr in world.GetActorsBySNO(289293))
			{
				(Tyr as ActorSystem.InteractiveNPC).Conversations.Clear();
				Tyr.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
				Tyr.Attributes.BroadcastChangedIfRevealed();
				AddQuestConversation(Tyr, 252100);
			}

			foreach (var act in world.GetActorsBySNO(316008)) act.Destroy(); //x1_Death_Orb_Little
			foreach (var act in world.GetActorsBySNO(315665)) act.Destroy(); //x1_westmarch_cath_int_debrisCenter
			foreach (var act in world.GetActorsBySNO(315891)) act.Destroy(); //x1_westmarch_cath_debrisSheets_02
			foreach (var act in world.GetActorsBySNO(315966)) act.Destroy(); //x1_westmarch_cath_debrisSheets_03
			foreach (var act in world.GetActorsBySNO(316266)) act.Destroy(); //x1_westmarch_cath_debrisSheets_04
			foreach (var act in world.GetActorsBySNO(319475)) act.Destroy(); //x1_westmarch_cath_debrisSheets_05
			foreach (var act in world.GetActorsBySNO(324731)) act.Destroy(); //x1_westmarch_cath_debrisSheets_06

			world.Game.QuestManager.Advance();

		}
	}
}
