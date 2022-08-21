//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

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
			var Maghda = AttackedTown.GetActorBySNO(129345);
			if (Maghda == null)
				Maghda = AttackedTown.SpawnMonster(129345, new Core.Types.Math.Vector3D(580f,563f,70f));
			Maghda.EnterWorld(Maghda.Position);
			Maghda.Attributes[GameAttribute.Untargetable] = true;
			Maghda.Attributes.BroadcastChangedIfRevealed();
			Maghda.PlayAnimation(5, 193535);

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
