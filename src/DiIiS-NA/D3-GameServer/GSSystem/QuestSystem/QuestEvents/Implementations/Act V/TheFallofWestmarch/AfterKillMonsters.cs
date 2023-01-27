using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class AfterKillMonsters : QuestEvent
	{
		public AfterKillMonsters()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			var Tyrael = world.ShowOnlyNumNPC(ActorSno._x1_tyrael_hurt, 0) as ActorSystem.InteractiveNPC;
			var Lorath = world.ShowOnlyNumNPC(ActorSno._x1_npc_lorathnahr, 0) as ActorSystem.InteractiveNPC;

			Tyrael.Conversations.Clear();
			Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
			Tyrael.Attributes.BroadcastChangedIfRevealed();

			world.Game.QuestManager.Advance();
	
		}
	}
}
