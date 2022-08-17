//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public static class ActorFactory
	{
		private static readonly Dictionary<int, Type> SNOHandlers = new Dictionary<int, Type>();
		private static Logger Logger = new Logger("ActorFactory");

		static ActorFactory()
		{
			LoadSNOHandlers();
		}

		public static void LazyCreate(World world, int snoId, TagMap tags, Vector3D spawn, Action<Actor, Vector3D> OnCreate)
		{
			var actor = Create(world, snoId, tags);
			if (actor != null)
				OnCreate.Invoke(actor, spawn);
		}

		public static Actor Create(World world, int snoId, TagMap tags)
		{
			if (!MPQStorage.Data.Assets[SNOGroup.Actor].ContainsKey(snoId))
			{
				//Logger.Warn("Actor asset not found, Id: {0}", snoId);
				return null;
			}

			switch (snoId)
			{
				case 6572:
				case 139454:
				case 139456:
				case 170324:
				case 170325:
				case 495:
				case 496:
					int variable = DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, 3);
					switch (variable)
					{
						case 0: snoId = 470241; break;
						case 1: snoId = 470241; break;
						case 2: snoId = 430928; break;
						case 3: snoId = 430928; break;
					}
					break;
			}

			if (world.Game.CurrentAct != 3000 && (
				(tags.ContainsKey(MarkerKeys.QuestRange) && tags[MarkerKeys.QuestRange].Id == 312431) ||
				(tags.ContainsKey(MarkerKeys.AdventureModeOnly) && tags[MarkerKeys.AdventureModeOnly] == 1)
				)) //non-Adventure Mode
				return null;


			if (world.Game.CurrentAct == 3000 && !world.IsPvP
				&& tags.ContainsKey(MarkerKeys.StoryModeOnly)
				&& tags[MarkerKeys.StoryModeOnly] == 1
				&& snoId != 6442) //only-Adventure Mode
				return null;

			if (tags.ContainsKey(MarkerKeys.RiftOnly) && tags[MarkerKeys.RiftOnly] == 1) //Rift Mode
				return null;

			var actorAsset = MPQStorage.Data.Assets[SNOGroup.Actor][snoId];
			var actorData = actorAsset.Data as DiIiS_NA.Core.MPQ.FileFormats.Actor;
			if (actorData == null)
			{
				Logger.Warn("Actor data not found, Id: {0}", snoId);
				return null;
			}

			if (actorData.Type == ActorType.Invalid)
			{
				Logger.Warn("Actor type is Invalid, Id: {0}", snoId);
				return null;
			}

			if (actorAsset.Name.ToLower().Contains("nospawn"))
			{
				return null;
			}

			// see if we have an implementation for actor.
			if (SNOHandlers.ContainsKey(snoId))
				return (Actor)Activator.CreateInstance(SNOHandlers[snoId], new object[] { world, snoId, tags });

			switch (actorData.Type)
			{
				case ActorType.Monster:
					if (tags.ContainsKey(MarkerKeys.ConversationList))
						return new InteractiveNPC(world, snoId, tags);
					else
						if (!MPQStorage.Data.Assets[SNOGroup.Monster].ContainsKey(actorData.MonsterSNO))
						return null;

					var monsterAsset = MPQStorage.Data.Assets[SNOGroup.Monster][actorData.MonsterSNO];
					var monsterData = monsterAsset.Data as DiIiS_NA.Core.MPQ.FileFormats.Monster;
					if (monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Breakable)
						return new Gizmo(world, snoId, tags);
					if (monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Ally ||
						monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Helper ||
						monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Scenery)
						return new NPC(world, snoId, tags);
					else
						if (actorAsset.Name.ToLower().Contains("unique"))
						return new Unique(world, snoId, tags);
					else
						return new Monster(world, snoId, tags);
				case ActorType.Gizmo:
					switch (actorData.TagMap[ActorKeys.GizmoGroup])
					{
						case GizmoGroup.LootContainer:
							return new LootContainer(world, snoId, tags);
						case GizmoGroup.Door:
							return new Door(world, snoId, tags);
						case GizmoGroup.DestructibleLootContainer:
							return new DesctructibleLootContainer(world, snoId, true, tags);
						case GizmoGroup.Destructible:
						case GizmoGroup.Passive:
						case GizmoGroup.Barricade:
							return new DesctructibleLootContainer(world, snoId, false, tags);
						case GizmoGroup.Portal:
							//Prevent Development Hell portal from showing
							if (tags.ContainsKey(MarkerKeys.DestinationWorld) && tags[MarkerKeys.DestinationWorld].Id == 222591)
								return null;
							if (tags.ContainsKey(MarkerKeys.DestinationWorld) && tags[MarkerKeys.DestinationWorld].Id == 443346)
								return null;
							else
								return new Portal(world, snoId, tags);
						case GizmoGroup.BossPortal:
							return new BossPortal(world, snoId, tags);
						case GizmoGroup.Readable:
							return new Readable(world, snoId, tags);
						case GizmoGroup.Banner:
							return new Banner(world, snoId, tags);
						case GizmoGroup.CheckPoint:
							return new Checkpoint(world, snoId, tags);
						case GizmoGroup.Waypoint:
							return new Waypoint(world, snoId, tags);
						case GizmoGroup.Savepoint:
							return new Savepoint(world, snoId, tags);
						case GizmoGroup.ProximityTriggered:
							return new ProximityTriggeredGizmo(world, snoId, tags);
						case GizmoGroup.Shrine:
							return new Shrine(world, snoId, tags);
						case GizmoGroup.Healthwell:
							return new Healthwell(world, snoId, tags);
						case GizmoGroup.ExpPool:
							return new XPPool(world, snoId, tags);
						case GizmoGroup.StartLocations:
							return new StartingPoint(world, snoId, tags);
						case GizmoGroup.HearthPortal:
							return new HearthPortal(world, snoId, tags);
						case GizmoGroup.DungeonStonePortal:
							return new DungeonStonePortal(world, snoId, tags);
						case GizmoGroup.Headstone:
							return new Headstone(world, snoId, tags);
						case GizmoGroup.Spawner:
							return new Spawner(world, snoId, tags);

						case GizmoGroup.GateGizmo:
						case GizmoGroup.ActChangeTempObject:
						case GizmoGroup.CathedralIdol:
						case GizmoGroup.NephalemAltar:
						case GizmoGroup.PlayerSharedStash:
						case GizmoGroup.QuestLoot:
						case GizmoGroup.ServerProp:
						case GizmoGroup.Sign:
						case GizmoGroup.TownPortal:
						case GizmoGroup.Trigger:
						case GizmoGroup.ScriptObject:
						case GizmoGroup.LootRunObelisk:
						case GizmoGroup.Unknown:
							return CreateGizmo(world, snoId, tags);
						default:
#if DEBUG
							Logger.Warn("Unknown gizmo group {0}, id {1}", actorData.TagMap[ActorKeys.GizmoGroup], snoId);
#else

#endif
							return CreateGizmo(world, snoId, tags);
					}
				case ActorType.ServerProp:
					return new ServerProp(world, snoId, tags);
				case ActorType.Environment:
					return new Environment(world, snoId, tags);
				case ActorType.Item:
					return new StaticItem(world, snoId, tags);
				case ActorType.Player:
					return new InteractiveNPC(world, snoId, tags);
				default:
					//Logger.Warn("Unknown Actor type {0}, Id: {1}", actorData.Type, snoId);
					return null;
			}
		}

		private static Actor CreateGizmo(World world, int snoId, TagMap tags)
		{
			//if (tags.ContainsKey(MarkerKeys.DestinationWorld)) //trying to spawn all portals
			//{
			//if (tags[MarkerKeys.DestinationWorld].Id != 222591)
			//return new Portal(world, snoId, tags);
			//}

			return new Gizmo(world, snoId, tags);
		}

		public static void LoadSNOHandlers()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(Actor))) continue;

				var attributes = (HandledSNOAttribute[])type.GetCustomAttributes(typeof(HandledSNOAttribute), true);
				if (attributes.Length == 0) continue;

				foreach (var sno in attributes.First().SNOIds)
				{
					if (!SNOHandlers.ContainsKey(sno))
						SNOHandlers.Add(sno, type);
				}
			}
		}
	}
}
