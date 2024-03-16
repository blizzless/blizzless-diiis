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
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System.Threading.Tasks;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnMiraImon : QuestEvent
	{
		//ActorID: 0x7A3100DD  
		//ZombieSkinny_A_LeahInn.acr (2050031837)
		//ActorSNOId: 0x00031971:ZombieSkinny_A_LeahInn.acr

		//private static readonly Logger Logger = LogManager.CreateLogger();

		public SpawnMiraImon()
			: base(151123)
		{
		}

		List<Vector3D> ActorsVector3D = new List<Vector3D> { }; //We fill this with the vectors of the actors that transform, so we spwan zombies in the same location.
		List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			var spawner = world.GetActorBySNO(ActorSno._blacksmithwife);
			if (spawner != null)
			{
				world.SpawnMonster(ActorSno._zombiefemale_a_blacksmitha, spawner.Position);
				spawner.Destroy();
			}
		}

		private bool HoudiniVsZombies(MapSystem.World world, ActorSno sno)
		{
			var actorSourcePosition = world.GetActorBySNO(sno);

			ActorsVector3D.Add(new Vector3D(actorSourcePosition.Position.X, actorSourcePosition.Position.Y, actorSourcePosition.Position.Z));
			actorSourcePosition.Destroy();
			return true;
		}

		private bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
			{
				player.Value.Conversations.StartConversation(conversationId);
			}
			return true;
		}

		private bool LaunchWave(List<Vector3D> Coordinates, MapSystem.World world, Int32 SnoId)
		{
			var counter = 0;
			var monsterSNOHandle = new Core.Types.SNO.SNOHandle(SnoId);
			var monsterActor = monsterSNOHandle.Target as DiIiS_NA.Core.MPQ.FileFormats.ActorData;

			foreach (Vector3D coords in Coordinates)
			{
				Parallel.ForEach(world.Players, player => //Threading because many spawns at once with out Parallel freezes D3.
				{
					var PRTransform = new PRTransform()
					{
						Quaternion = new Quaternion()
						{
							W = 0.590017f,
							Vector3D = new Vector3D(0, 0, 0)
						},
						Vector3D = Coordinates[counter]
					};

					//Load the actor here.
					uint actor = 0;
					actor = world.Game.WorldGenerator.LoadActor(monsterSNOHandle, PRTransform, world, monsterActor.TagMap);

					monstersAlive.Add(actor);
					counter++;
				});
			}
			return true;
		}
	}
}
