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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public class DungeonStonePortal : Actor
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		private ResolvedPortalDestination Destination { get; set; }

		public DungeonStonePortal(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{

			//this.Field2 = 0x9;//16;
			this.Attributes[GameAttribute.MinimapActive] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 218394;
			if (this.World.WorldSNO.Name.ToLower().Contains("x1_lr_tileset"))
			{
				this.Destination = new ResolvedPortalDestination()
				{
					DestLevelAreaSNO = 332339,
					WorldSNO = 332336,
					StartingPointActorTag = 24
				};
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (World.WorldSNO.Id == 92126)
			{
				Portal Exit = null;
				foreach (Actor actor in World.Game.GetWorld(154587).GetStartingPointById(172).GetActorsInRange(120f))
					if (actor is Portal)
						Exit = actor as Portal;
				if (Exit != null)
					this.Destination = Exit.Destination;
			}
			else if (Destination == null)
				this.Destination = this.World.PrevLocation;

			player.InGameClient.SendMessage(new PortalSpecifierMessage()
			{
				ActorID = this.DynamicID(player),
				Destination = this.Destination
			});

			return true;
		}
		public StartingPoint GetSmartStartingPoint(World world)
		{
			if (this.Destination.StartingPointActorTag != 0)
			{
				StartingPoint NeededStartingPoint = world.GetStartingPointById(this.Destination.StartingPointActorTag);
				var DestWorld = world.Game.GetWorld(this.Destination.WorldSNO);
				var StartingPoints = DestWorld.GetActorsBySNO(5502);
				foreach (var ST in StartingPoints)
				{
					if (ST.CurrentScene.SceneSNO.Id == this.Destination.StartingPointActorTag)
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

			if (this.World.WorldSNO.Name.ToLower().Contains("x1_lr_tileset"))
			{
				this.Destination.DestLevelAreaSNO = 332339;
				this.Destination.WorldSNO = 332336;
				this.Destination.StartingPointActorTag = 24;
			}

			var world = this.World.Game.GetWorld(this.Destination.WorldSNO);

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", this.Destination.WorldSNO);
				return;
			}

			var startingPoint = world.GetStartingPointById(this.Destination.StartingPointActorTag);

			if (startingPoint == null)
				startingPoint = GetSmartStartingPoint(world);

			if (startingPoint != null)
			{

				player.ShowConfirmation(this.DynamicID(player), (() => {
					player.StartCasting(150, new Action(() => {
						if (world == this.World)
							player.Teleport(startingPoint.Position);
						else
							player.ChangeWorld(world, startingPoint);
					}));
				}));

			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", this.Destination.StartingPointActorTag);
		}
	}
}
