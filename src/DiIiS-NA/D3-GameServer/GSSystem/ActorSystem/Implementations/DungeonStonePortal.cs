//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public class DungeonStonePortal : Actor
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		private ResolvedPortalDestination Destination { get; set; }

		public DungeonStonePortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{

			//this.Field2 = 0x9;//16;
			Attributes[GameAttribute.MinimapActive] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 218394;
			if (World.SNO.IsDungeon())
			{
				Destination = new ResolvedPortalDestination()
				{
					DestLevelAreaSNO = 332339,
					WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub,
					StartingPointActorTag = 24
				};
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_02)
			{
				Portal Exit = null;
				foreach (Actor actor in World.Game.GetWorld(WorldSno.trdun_crypt_skeletonkingcrown_00).GetStartingPointById(172).GetActorsInRange(120f))
					if (actor is Portal)
						Exit = actor as Portal;
				if (Exit != null)
					Destination = Exit.Destination;
			}
			else if (Destination == null)
				Destination = World.PrevLocation;

			player.InGameClient.SendMessage(new PortalSpecifierMessage()
			{
				ActorID = DynamicID(player),
				Destination = Destination
			});

			return true;
		}
		public StartingPoint GetSmartStartingPoint(World world)
		{
			if (Destination.StartingPointActorTag != 0)
			{
				StartingPoint NeededStartingPoint = world.GetStartingPointById(Destination.StartingPointActorTag);
				var DestWorld = world.Game.GetWorld((WorldSno)Destination.WorldSNO);
				var StartingPoints = DestWorld.GetActorsBySNO(ActorSno._start_location_0);
				foreach (var ST in StartingPoints)
				{
					if (ST.CurrentScene.SceneSNO.Id == Destination.StartingPointActorTag)
						NeededStartingPoint = (ST as StartingPoint);
				}
				if (NeededStartingPoint != null)
					return NeededStartingPoint;
				else
					return null;
			}
			else
				return null;
		}
		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Debug("(OnTargeted) Portal has been activated ");

			if (World.SNO.IsDungeon())
			{
				Destination.DestLevelAreaSNO = 332339;
				Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
				Destination.StartingPointActorTag = 24;
			}

			var world = World.Game.GetWorld((WorldSno)Destination.WorldSNO);

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", Destination.WorldSNO);
				return;
			}

			var startingPoint = world.GetStartingPointById(Destination.StartingPointActorTag);

			if (startingPoint == null)
				startingPoint = GetSmartStartingPoint(world);

			if (startingPoint != null)
			{

				player.ShowConfirmation(DynamicID(player), (() => {
					player.StartCasting(150, new Action(() => {
						if (world == World)
							player.Teleport(startingPoint.Position);
						else
							player.ChangeWorld(world, startingPoint);
					}));
				}));

			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", Destination.StartingPointActorTag);
		}
	}
}
