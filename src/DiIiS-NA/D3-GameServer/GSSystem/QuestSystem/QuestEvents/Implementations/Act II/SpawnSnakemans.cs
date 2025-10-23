using System;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnSnakemans : QuestEvent
	{

		public SpawnSnakemans()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//if (world.Game.Empty) return;
			//Logger.Trace("SpawnSnakemans event started");
			var point = new Vector3D { X = 835.331f, Y = 410.121f, Z = 161.842f };
			var snakeManHandle = new Core.Types.SNO.SNOHandle((int)ActorSno._khamsin_snakeman_melee);
			var snakeManActor = snakeManHandle.Target as DiIiS_NA.Core.MPQ.FileFormats.ActorData;
			try
			{
				var caldeumGuard = world.FindActorAt(ActorSno._caldeumguard_cleaver_a, point, 20.0f);
				Vector3D guard_a_position = caldeumGuard.Position;
				caldeumGuard.Destroy(); //world.Game.
				world.Game.WorldGenerator.LoadActor(snakeManHandle,
				new PRTransform()
				{
					Quaternion = new Quaternion()
					{
						W = 0.590017f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = guard_a_position
				},
				world,
				snakeManActor.TagMap);
			}
			catch { }
			try
			{
				var caldeumGuard = world.FindActorAt(ActorSno._caldeumguard_cleaver_a, point, 20.0f);
				Vector3D caldeumGuardPosition = caldeumGuard.Position;
				caldeumGuard.Destroy();
				world.Game.WorldGenerator.LoadActor(snakeManHandle,
				new PRTransform()
				{
					Quaternion = new Quaternion()
					{
						W = 0.590017f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = caldeumGuardPosition
				},
				world,
				snakeManActor.TagMap);
			}
			catch { }
			try
			{
				var davydImpostor = world.FindActorAt(ActorSno._davydimpostor, point, 20.0f);
				Vector3D davydPosition = davydImpostor.Position;
				davydImpostor.Destroy();
				world.Game.WorldGenerator.LoadActor(snakeManHandle,
				new PRTransform()
				{
					Quaternion = new Quaternion()
					{
						W = 0.590017f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = davydPosition
				},
				world,
				snakeManActor.TagMap);
			}
			catch { }
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
