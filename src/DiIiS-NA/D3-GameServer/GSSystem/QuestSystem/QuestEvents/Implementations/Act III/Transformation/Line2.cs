using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahTransformation_Line2 : QuestEvent
	{
		public bool raised = false;

		public LeahTransformation_Line2()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			StartConversation(world, 195721);
			var Tyrael = world.GetActorBySNO(ActorSno._tyrael_event47);
			var Adria = world.GetActorBySNO(ActorSno._adria_event47);
			float facingAngle = MovementHelpers.GetFacingAngle(Adria, Tyrael);
			Adria.Move(new Core.Types.Math.Vector3D(Tyrael.Position.X, Tyrael.Position.Y - 5f, Tyrael.Position.Z), facingAngle);
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
