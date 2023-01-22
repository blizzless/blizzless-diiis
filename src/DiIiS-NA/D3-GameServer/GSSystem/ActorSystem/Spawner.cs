//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Spawner : Actor
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// What actor this gizmo will spawn
		/// </summary>
		private SNOHandle ActorToSpawnSNO { get; set; }

		private bool triggered = false;

		public override ActorType ActorType
		{
			get { return ActorType.Gizmo; }
		}

		public Spawner(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags, false)
		{
			Field2 = 8;
			Field7 = 0x00000000;

			//Actor.Data.TagMap contains: {66368 = 291072}
			//public static TagKeyInt Spawn2 = new TagKeyInt(291072);
			//TODO: Find why Tags is not the same as Actor.Data.TagMap
			if (Tags.ContainsKey(MarkerKeys.SpawnActor))
				ActorToSpawnSNO = Tags[MarkerKeys.SpawnActor];

			if (SNO == ActorSno._spawner_zolt_centerpiece) ActorToSpawnSNO = new SNOHandle(SNOGroup.Actor, (int)ActorSno._a2dun_zolt_centerpiece_a);
		}

		/// <summary>
		/// Rewrite the quest handling event
		/// </summary>
		/// <param name="quest"></param>
		protected override void quest_OnQuestProgress()
		{
			if (SNO == ActorSno._spawner_zolt_centerpiece) return;
			//Spawn if this is spawner
			try
			{
				if (World.Game.QuestManager.IsInQuestRange(_questRange) && !triggered)
				{
					Spawn();
				}
			}
			catch { }
		}

		/// <summary>
		/// Override for AfterChangeWorld
		/// </summary>
		public override void AfterChangeWorld()
		{
			base.AfterChangeWorld();
		}

		/// <summary>
		/// Main spawn method
		/// </summary>
		public void Spawn()
		{
			
			triggered = true;
			if (ActorToSpawnSNO == null)
			{
				Logger.Trace("Triggered spawner with no ActorToSpawnSNO found.");
				//Try revealing this
				/*foreach (var player in this.World.Players.Values)
				{
					base.Reveal(player);
				}*/
				return;
			}
			//Logger.Trace("Triggered spawner with actorSNO = {0}.", ActorToSpawnSNO.Id);
			var location = new PRTransform()
			{
				Quaternion = new Quaternion
				{
					W = RotationW,
					Vector3D = RotationAxis
				},
				Vector3D = Position
			};

			//this.World.Game.WorldGenerator.Actions.Enqueue(() =>
			World.Game.WorldGenerator.LoadActor(ActorToSpawnSNO, location, World, ((DiIiS_NA.Core.MPQ.FileFormats.Actor)ActorToSpawnSNO.Target).TagMap);
			//Mooege.Core.GS.Generators.WorldGenerator.loadActor(ActorToSpawnSNO, location, this.World, ((Mooege.Common.MPQ.FileFormats.Actor)ActorToSpawnSNO.Target).TagMap);
		}

		/// <summary>
		/// Reveal Override. For Spawner Gizmos there is no reveal necessary.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Reveal(Player player)
		{
			/*if (this.ActorSNO.Id == 74187 && this.World.GetActorBySNO(7295) == null)
			{
				this.Spawn();
			}*/
			return false;
		}
	}
}
