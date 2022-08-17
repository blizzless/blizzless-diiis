//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;

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
				var monster = world.SpawnMonster(6632, new Vector3D(actor.Position.X, actor.Position.Y, actor.Position.Z));
				monster.Attributes[GameAttribute.God] = true;
				monster.Attributes.BroadcastChangedIfRevealed();
				(monster as Monster).Brain.DeActivate();
				monsters.Add(monster);
			}
			System.Threading.Tasks.Task.Delay(600).ContinueWith(delegate
			{
				foreach (var monster in monsters)
				{
					monster.Attributes[GameAttribute.Quest_Monster] = true;
					monster.Attributes[GameAttribute.God] = false;
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
