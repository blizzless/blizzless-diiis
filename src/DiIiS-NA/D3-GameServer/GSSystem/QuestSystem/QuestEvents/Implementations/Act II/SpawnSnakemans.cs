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
			var snakemanHandle = new Core.Types.SNO.SNOHandle((int)ActorSno._khamsin_snakeman_melee);
			var snakemanActor = snakemanHandle.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor;
			try
			{
				var guard_a = world.FindAt(ActorSno._caldeumguard_cleaver_a, point, 20.0f);
				Vector3D guard_a_position = guard_a.Position;
				guard_a.Destroy(); //world.Game.
				world.Game.WorldGenerator.LoadActor(snakemanHandle,
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
				snakemanActor.TagMap);
			}
			catch { }
			try
			{
				var guard_b = world.FindAt(ActorSno._caldeumguard_cleaver_a, point, 20.0f);
				Vector3D guard_b_position = guard_b.Position;
				guard_b.Destroy();
				world.Game.WorldGenerator.LoadActor(snakemanHandle,
				new PRTransform()
				{
					Quaternion = new Quaternion()
					{
						W = 0.590017f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = guard_b_position
				},
				world,
				snakemanActor.TagMap);
			}
			catch { }
			try
			{
				var guard_c = world.FindAt(ActorSno._davydimpostor, point, 20.0f);
				Vector3D guard_c_position = guard_c.Position;
				guard_c.Destroy();
				world.Game.WorldGenerator.LoadActor(snakemanHandle,
				new PRTransform()
				{
					Quaternion = new Quaternion()
					{
						W = 0.590017f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = guard_c_position
				},
				world,
				snakemanActor.TagMap);
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
