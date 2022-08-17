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
	class LeahInn : QuestEvent
	{
		//ActorID: 0x7A3100DD  
		//ZombieSkinny_A_LeahInn.acr (2050031837)
		//ActorSNOId: 0x00031971:ZombieSkinny_A_LeahInn.acr

		//private static readonly Logger Logger = LogManager.CreateLogger();

		public LeahInn()
			: base(151123) // 204113 // 151156
		{
		}

		List<Vector3D> ActorsVector3D = new List<Vector3D> { }; //We fill this with the vectors of the actors that transform, so we spwan zombies in the same location.
		List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.

		public override void Execute(MapSystem.World world)
		{
			List<ActorSystem.Actor> actorstotarget = new List<ActorSystem.Actor> { };
			if (world.Game.Empty) return;
			StartConversation(world, 204113);

			var spawner = world.GetActorBySNO(204605);
			actorstotarget.Add(world.SpawnMonster(203121, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(204606);
			actorstotarget.Add(world.SpawnMonster(203121, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(204607);
			actorstotarget.Add(world.SpawnMonster(203121, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(204608);
			actorstotarget.Add(world.SpawnMonster(203121, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(174023);
			actorstotarget.Add(world.SpawnMonster(203121, spawner.Position));
			spawner.Destroy();

			foreach (var actor in actorstotarget)
			{
				actor.PlayAnimation(9, 0x00029A08, 1f);
				actor.Attributes[GameAttribute.Quest_Monster] = true;
				actor.Attributes.BroadcastChangedIfRevealed();
			}
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
