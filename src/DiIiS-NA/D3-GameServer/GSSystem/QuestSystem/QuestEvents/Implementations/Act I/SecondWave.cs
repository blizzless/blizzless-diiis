using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SecondWave : QuestEvent
	{

		public SecondWave()
			: base(4)   // 198199 // 80088  // 151102
		{
		}

		public override void Execute(MapSystem.World world)
		{
			List<Actor> monsters = new List<Actor>() { };
			if (world.Game.Players.Count == 0) return;
			//Wave three: Skinnies + RumFord conversation #2 "They Keep Coming!".
			StartConversation(world, 80088);
			var wave2Actors = world.GetActorsInGroup("GizmoGroup2");
			foreach (var actor in wave2Actors)
			{
				var monster = world.SpawnMonster(ActorSno._zombiecrawler_a, new Vector3D(actor.Position.X, actor.Position.Y, actor.Position.Z));
				monster.Attributes[GameAttributes.God] = true;
				monster.Attributes.BroadcastChangedIfRevealed();
				(monster as Monster).Brain.DeActivate();
				monsters.Add(monster);
			}
			System.Threading.Tasks.Task.Delay(600).ContinueWith(delegate
			{
				foreach (var monster in monsters)
				{
					monster.Attributes[GameAttributes.Quest_Monster] = true;
					monster.Attributes[GameAttributes.God] = false;
					(monster as Monster).Brain.Activate();
					monster.Attributes.BroadcastChangedIfRevealed();
				}
			});
		}

		//Launch Conversations.
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
