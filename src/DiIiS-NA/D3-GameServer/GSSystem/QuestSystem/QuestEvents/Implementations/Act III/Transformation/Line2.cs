//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
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
			var Tyrael = world.GetActorBySNO(195377);
			var Adria = world.GetActorBySNO(195378);
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
