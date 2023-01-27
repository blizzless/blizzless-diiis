using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Linq;
using System;
using System.Collections.Generic;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SurviveTheWaves : QuestEvent
	{

		private static readonly Logger Logger = LogManager.CreateLogger();

		public SurviveTheWaves()
			: base(151087)  // 198199 // 80088  // 151102
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			SetActorOperable(world, ActorSno._captainrumfoord, false);
			StartConversation(world, 198199);

			//System.Threading.Tasks.Task.Delay(1000).Wait();
			var wave1Actors = world.GetActorsInGroup("GizmoGroup1");
			List<Actor> monsters = new List<Actor>() { };
			System.Threading.Tasks.Task.Delay(1000).ContinueWith(delegate
			{
				foreach (var actor in wave1Actors)
				{
					var monster = world.SpawnMonster(ActorSno._zombieskinny_a, new Vector3D(actor.Position.X, actor.Position.Y, actor.Position.Z));
					monster.Attributes[GameAttribute.God] = true;
					monster.Attributes.BroadcastChangedIfRevealed();
					(monster as Monster).Brain.DeActivate();
					monsters.Add(monster);
				}
				System.Threading.Tasks.Task.Delay(2500).ContinueWith(delegate
				{
					foreach (var monster in monsters)
					{
						monster.Attributes[GameAttribute.Quest_Monster] = true;
						monster.Attributes[GameAttribute.God] = false;
						(monster as Monster).Brain.Activate();
						monster.Attributes.BroadcastChangedIfRevealed();
					}
					monsters.Clear();
					System.Threading.Tasks.Task.Delay(5000).ContinueWith(delegate
					{
						foreach (var actor in wave1Actors)
						{
							var monster = world.SpawnMonster(ActorSno._zombieskinny_a, new Vector3D(actor.Position.X, actor.Position.Y, actor.Position.Z));
							monster.Attributes[GameAttribute.God] = true;
							monster.Attributes.BroadcastChangedIfRevealed();
							(monster as Monster).Brain.DeActivate();
							monsters.Add(monster);
							System.Threading.Tasks.Task.Delay(2500).ContinueWith(delegate
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
					});
				});
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

		//Not Operable Rumford (To disable giving u the same quest while ur in the event)
		public static bool SetActorOperable(MapSystem.World world, ActorSno sno, bool status)
		{
			var actor = world.GetActorBySNO(sno);
			foreach (var player in world.Players)
			{
				actor.Attributes[GameAttribute.NPC_Is_Operatable] = status;
			}
			return true;
		}

	}
}
