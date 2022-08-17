//Blizzless Project 2022 
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
			var Tyrael = world.ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
			var Lorath = world.ShowOnlyNumNPC(284530, 0) as ActorSystem.InteractiveNPC;

			Tyrael.Conversations.Clear();
			Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
			Tyrael.Attributes.BroadcastChangedIfRevealed();

			world.Game.QuestManager.Advance();
	
		}
	}
}
