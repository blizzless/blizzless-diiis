using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public static class ActorFactory
	{
		private static readonly Dictionary<ActorSno, Type> SNOHandlers;
		private static Logger Logger = new Logger("ActorFactory");

		static ActorFactory()
		{
			SNOHandlers = Assembly.GetExecutingAssembly().GetTypes()
				.Where(x => x.IsSubclassOf(typeof(Actor)))
				.Select(x => new { Type = x, Attribute = x.GetCustomAttributes<HandledSNOAttribute>(true).FirstOrDefault() })
				.Where(x => x.Attribute != null)
				.SelectMany(x => x.Attribute.SNOIds.Select(a => new { x.Type, Sno = a }))
				.ToDictionary(x => x.Sno, x => x.Type);
        }

		public static void LazyCreate(World world, ActorSno sno, TagMap tags, Vector3D spawn, Action<Actor, Vector3D> OnCreate)
		{
			var actor = Create(world, sno, tags);
			if (actor != null)
				OnCreate.Invoke(actor, spawn);
		}

		public static Actor Create(World world, ActorSno sno, TagMap tags, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			if (!MPQStorage.Data.Assets[SNOGroup.Actor].ContainsKey((int)sno))
			{
				var path = Path.GetFileName(filePath);
				Logger.Trace($"$[underline red on white]$Actor asset not found$[/]$, Method: $[olive]${memberName}()$[/]$ - $[underline white]${memberName}() in {path}:{lineNumber}$[/]$");
				return null;
			}

			switch (sno)
			{
				case ActorSno._woodwraith_a_01:
				case ActorSno._woodwraith_a_02:
				case ActorSno._woodwraith_a_03:
				case ActorSno._woodwraith_b_01:
				case ActorSno._woodwraith_b_02:
				case ActorSno._woodwraith_b_03:
				case ActorSno._woodwraith_unique_a:
					switch (DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, 3) / 2)
					{
						case 0: sno = ActorSno._ls_woodwraith; break;
						case 1: sno = ActorSno._p4_woodwraith_a; break;
					}
					break;
			}

			if (world.Game.CurrentAct != ActEnum.OpenWorld && (
				(tags.ContainsKey(MarkerKeys.QuestRange) && tags[MarkerKeys.QuestRange].Id == 312431) ||
				(tags.ContainsKey(MarkerKeys.AdventureModeOnly) && tags[MarkerKeys.AdventureModeOnly] == 1)
				)) //non-Adventure Mode
				return null;


			if (world.Game.CurrentAct == ActEnum.OpenWorld && !world.IsPvP
				&& tags.ContainsKey(MarkerKeys.StoryModeOnly)
				&& tags[MarkerKeys.StoryModeOnly] == 1
				&& sno != ActorSno._waypoint) //only-Adventure Mode
				return null;

			if (tags.ContainsKey(MarkerKeys.RiftOnly) && tags[MarkerKeys.RiftOnly] == 1) //Rift Mode
				return null;

			var actorAsset = MPQStorage.Data.Assets[SNOGroup.Actor][(int)sno];
			var actorData = actorAsset.Data as DiIiS_NA.Core.MPQ.FileFormats.ActorData;
			if (actorData == null)
			{
				Logger.Warn("Actor data not found, Id: {0}", sno);
				return null;
			}

			if (actorData.Type == ActorType.Invalid)
			{
				Logger.Warn("Actor type is Invalid, Id: {0}", sno);
				return null;
			}

			if (actorAsset.Name.ToLower().Contains("nospawn"))
			{
				return null;
			}

			// see if we have an implementation for actor.
			if (SNOHandlers.ContainsKey(sno))
				return (Actor)Activator.CreateInstance(SNOHandlers[sno], new object[] { world, sno, tags });

			switch (actorData.Type)
			{
				case ActorType.Monster:
					if (tags.ContainsKey(MarkerKeys.ConversationList))
						return new InteractiveNPC(world, sno, tags);
					else
						if (!MPQStorage.Data.Assets[SNOGroup.Monster].ContainsKey(actorData.MonsterSNO))
						return null;

					var monsterAsset = MPQStorage.Data.Assets[SNOGroup.Monster][actorData.MonsterSNO];
					var monsterData = monsterAsset.Data as DiIiS_NA.Core.MPQ.FileFormats.Monster;
					if (monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Breakable)
						return new Gizmo(world, sno, tags);
					if (monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Ally ||
						monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Helper ||
						monsterData.Type == DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Scenery)
						return new NPC(world, sno, tags);
					else
						if (actorAsset.Name.ToLower().Contains("unique"))
						return new Unique(world, sno, tags);
					else
						return new Monster(world, sno, tags);
				case ActorType.Gizmo:
					switch (actorData.TagMap[ActorKeys.GizmoGroup])
					{
						case GizmoGroup.LootContainer:
							return new LootContainer(world, sno, tags);
						case GizmoGroup.Door:
							return new Door(world, sno, tags);
						case GizmoGroup.DestructibleLootContainer:
							return new DesctructibleLootContainer(world, sno, true, tags);
						case GizmoGroup.Destructible:
						case GizmoGroup.Passive:
						case GizmoGroup.Barricade:
							return new DesctructibleLootContainer(world, sno, false, tags);
						case GizmoGroup.Portal:
							//Prevent Development Hell portal from showing
							if (tags.ContainsKey(MarkerKeys.DestinationWorld) && tags[MarkerKeys.DestinationWorld].Id == 222591)
								return null;
							if (tags.ContainsKey(MarkerKeys.DestinationWorld) && tags[MarkerKeys.DestinationWorld].Id == 443346)
								return null;
							else
								return new Portal(world, sno, tags);
						case GizmoGroup.BossPortal:
							return new BossPortal(world, sno, tags);
						case GizmoGroup.Readable:
							return new Readable(world, sno, tags);
						case GizmoGroup.Banner:
							return new Banner(world, sno, tags);
						case GizmoGroup.CheckPoint:
							return new Checkpoint(world, sno, tags);
						case GizmoGroup.Waypoint:
							return new Waypoint(world, sno, tags);
						case GizmoGroup.Savepoint:
							return new Savepoint(world, sno, tags);
						case GizmoGroup.ProximityTriggered:
							return new ProximityTriggeredGizmo(world, sno, tags);
						case GizmoGroup.Shrine:
							return new Shrine(world, sno, tags);
						case GizmoGroup.Healthwell:
							return new Healthwell(world, sno, tags);
						case GizmoGroup.ExpPool:
							return new XPPool(world, sno, tags);
						case GizmoGroup.StartLocations:
							return new StartingPoint(world, sno, tags);
						case GizmoGroup.HearthPortal:
							return new HearthPortal(world, sno, tags);
						case GizmoGroup.DungeonStonePortal:
							return new DungeonStonePortal(world, sno, tags);
						case GizmoGroup.Headstone:
							return new Headstone(world, sno, tags);
						case GizmoGroup.Spawner:
							return new Spawner(world, sno, tags);

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
							return CreateGizmo(world, sno, tags);
						default:
#if DEBUG
							Logger.Warn("Unknown gizmo group {0}, id {1}", actorData.TagMap[ActorKeys.GizmoGroup], sno);
#else

#endif
							return CreateGizmo(world, sno, tags);
					}
				case ActorType.ServerProp:
					return new ServerProp(world, sno, tags);
				case ActorType.Environment:
					return new Environment(world, sno, tags);
				case ActorType.Item:
					return new StaticItem(world, sno, tags);
				case ActorType.Player:
					return new InteractiveNPC(world, sno, tags);
				default:
					//Logger.Warn("Unknown Actor type {0}, Id: {1}", actorData.Type, snoId);
					return null;
			}
		}

		private static Actor CreateGizmo(World world, ActorSno sno, TagMap tags)
		{
			//if (tags.ContainsKey(MarkerKeys.DestinationWorld)) //trying to spawn all portals
			//{
			//if (tags[MarkerKeys.DestinationWorld].Id != 222591)
			//return new Portal(world, snoId, tags);
			//}

			return new Gizmo(world, sno, tags);
		}
	}
}
