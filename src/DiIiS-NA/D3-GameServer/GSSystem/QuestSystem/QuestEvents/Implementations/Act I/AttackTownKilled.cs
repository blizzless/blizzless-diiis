using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class AttackTownKilled : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public AttackTownKilled()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
            var AttackedTown = world.Game.GetWorld(WorldSno.trout_townattack);
			var Maghda = AttackedTown.GetActorBySNO(ActorSno._maghda_a_tempprojection);
			if (Maghda == null)
				Maghda = AttackedTown.SpawnMonster(ActorSno._maghda_a_tempprojection, new Core.Types.Math.Vector3D(580f,563f,70f));
			Maghda.EnterWorld(Maghda.Position);
			Maghda.Attributes[GameAttributes.Untargetable] = true;
			Maghda.Attributes.BroadcastChangedIfRevealed();
			Maghda.PlayAnimation(5, AnimationSno.maghdaprojection_transition_in_01);

			StartConversation(AttackedTown, 194933);
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
