﻿//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Collision;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Scene;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Scene;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using Actor = DiIiS_NA.GameServer.GSSystem.ActorSystem.Actor;

namespace DiIiS_NA.GameServer.GSSystem.MapSystem
{
	public sealed class Scene : WorldObject
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public static readonly Dictionary<int, List<Marker>> PreCachedMarkers = new Dictionary<int, List<Marker>>();

		/// <summary>
		/// SNOHandle for the scene.
		/// </summary>
		public SNOHandle SceneSNO { get; private set; }

		/// <summary>
		/// Scene group's SNOId.
		/// Not sure on usage /raist.
		/// </summary>
		public int SceneGroupSNO { get; set; }

		public DiIiS_NA.Core.MPQ.FileFormats.Scene SceneData
		{
			get
			{
				if (!MPQStorage.Data.Assets[SNOGroup.Scene].ContainsKey(SceneSNO.Id))
					Logger.Debug("AssetsForScene not found in MPQ Storage:Scene:{0}, Asset:{1}", SNOGroup.Scene, SceneSNO.Id);
				return MPQStorage.Data.Assets[SNOGroup.Scene][SceneSNO.Id].Data as DiIiS_NA.Core.MPQ.FileFormats.Scene;
			}
		}
		/// <summary>
		/// DRLG type for scene(Exit, Entrance etc.)
		/// </summary>
		public int TileType { get; set; }

		/// <summary>
		/// Subscenes.
		/// </summary>
		public List<Scene> Subscenes { get; private set; }

		/// <summary>
		/// Parent scene if any.
		/// </summary>
		public Scene Parent;

		/// <summary>
		/// Parent scene's chunk id.
		/// </summary>
		public uint ParentChunkID
		{
			get { return (Parent != null) ? Parent.GlobalID : 0xFFFFFFFF; }
		}

		/// <summary>
		/// Visibility in MiniMap.
		/// </summary>
		public bool MiniMapVisibility { get; set; }

		/// <summary>
		/// Scene Specification.
		/// </summary>
		public SceneSpecification Specification { get; set; }

		/// <summary>
		/// Applied labels.
		/// Not sure on usage /raist.
		/// </summary>
		public int[] AppliedLabels;

		public int[] Field8;

		/// <summary>
		/// PRTransform for the scene.
		/// </summary>
		public PRTransform Transform
		{
			get { return new PRTransform { Quaternion = new Quaternion { W = RotationW, Vector3D = RotationAxis }, Vector3D = Position }; }
		}

		/// <summary>
		/// AABB bounds for the scene.
		/// </summary>
		public AABB AABBBounds
		{
			get
			{
				return SceneData.AABBBounds;
			}
		}
		/// <summary>
		/// AABB bounds for MarketSet.
		/// </summary>
		public AABB AABBMarketSetBounds
		{
			get
			{
				return SceneData.AABBMarketSetBounds;
			}
		}

		/// <summary>
		/// NavMesh for the scene.
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.Scene.NavMeshDef NavMesh
		{
			get
			{
				return SceneData.NavMesh;
			}
		}

		/// <summary>
		/// Markersets for the scene.
		/// </summary>
		public List<int> MarkerSets
		{
			get
			{
				return SceneData.MarkerSets;
			}
		}

		/// <summary>
		/// LookLink - not sure on the usage /raist.
		/// </summary>
		public string LookLink
		{
			get
			{
				return SceneData.LookLink;
			}
		}

		public bool Populated = false;

		/// <summary>
		/// NavZone for the scene.
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.Scene.NavZoneDef NavZone
		{
			get
			{
				return SceneData.NavZone;
			}
		}

		/// <summary>
		/// Possible spawning locations for randomized gizmo placement
		/// </summary>
		public List<PRTransform>[] GizmoSpawningLocations { get; private set; }

		/// <summary>
		/// Creates a new scene and adds it to given world.
		/// </summary>
		/// <param name="world">The parent world.</param>
		/// <param name="position">The position.</param>
		/// <param name="snoId">SNOId for the scene.</param>
		/// <param name="parent">The parent scene if any.</param>
		public Scene(World world, Vector3D position, int snoId, Scene parent)
			: base(world, world.NewSceneID)
		{
			SceneSNO = new SNOHandle(SNOGroup.Scene, snoId);
			Parent = parent;
			Subscenes = new List<Scene>();
			Scale = 1.0f;
			AppliedLabels = new int[0];
			Field8 = new int[0];
			Size = new Size(NavZone.V0.X * (int)NavZone.Float0, NavZone.V0.Y * (int)NavZone.Float0);
			Position = position;
			World.AddScene(this); // add scene to the world.
		}

		#region range-queries

		public List<Player> Players
		{
			get { return GetObjects<Player>(); }
		}

		public bool HasPlayers
		{
			get { return Players.Count > 0; }
		}

		public List<Actor> Actors
		{
			get { return GetObjects<Actor>(); }
		}

		public bool HasActors
		{
			get { return Actors.Count > 0; }
		}

		public List<T> GetObjects<T>() where T : WorldObject
		{
			return World.QuadTree.Query<T>(Bounds);
		}

		#endregion

		#region actor-loading

		/// <summary>
		/// Loads all markers for the scene.		
		/// </summary>
		public void LoadMarkers()
		{
			GizmoSpawningLocations = new List<PRTransform>[26]; // LocationA to LocationZ

			if (!PreCachedMarkers.ContainsKey(SceneSNO.Id)) return;

			foreach (var marker in PreCachedMarkers[SceneSNO.Id])
			{
				switch (marker.Type)
				{
					case MarkerType.Actor:
						var actor = ActorFactory.Create(World, (ActorSno)marker.SNOHandle.Id, marker.TagMap); // try to create it.
																										 //Logger.Debug("not-lazy spawned {0}", actor.GetType().Name);
						if (actor == null) continue;
						if (World.SNO == WorldSno.a3_battlefields_02 && SceneSNO.Id == 145392 && actor is StartingPoint) continue; //arreat crater hack
						if (World.SNO == WorldSno.x1_westm_intro && SceneSNO.Id == 311310 && actor is StartingPoint) continue; //A5 start location hack

						var position = marker.PRTransform.Vector3D + Position; // calculate the position for the actor.
						actor.RotationW = marker.PRTransform.Quaternion.W;
						actor.RotationAxis = marker.PRTransform.Quaternion.Vector3D;
						actor.AdjustPosition = false;
						actor.EnterWorld(position);
						//System.Threading.Thread.Sleep(3);
						break;

					case MarkerType.Encounter:
						try
						{
							//Logger.Warn("load Encounter marker {0} in {1} ({2})", marker.Name, markerSetData.FileName, marker.SNOHandle.Id);
							var encounter = marker.SNOHandle.Target as Encounter;
							var actorsno = RandomHelper.RandomItem(encounter.Spawnoptions, x => x.Probability);
							/*foreach (var option in encounter.Spawnoptions)
							{
								Logger.Trace("Encounter option {0} - {1} - {2} - {3}", option.SNOSpawn, option.Probability, option.I1, option.I2);
							}*/ //only for debugging purposes
							var actor2 = ActorFactory.Create(World, (ActorSno)actorsno.SNOSpawn, marker.TagMap); // try to create it.
							if (actor2 == null) continue;

							var position2 = marker.PRTransform.Vector3D + Position; // calculate the position for the actor.
							actor2.RotationW = marker.PRTransform.Quaternion.W;
							actor2.RotationAxis = marker.PRTransform.Quaternion.Vector3D;
							actor2.AdjustPosition = false;
							actor2.EnterWorld(position2);
						}
						catch { }

						break;

					default:
						// Save gizmo locations. They are used to spawn loots and gizmos randomly in a level area
						if ((int)marker.Type >= (int)MarkerType.GizmoLocationA && (int)marker.Type <= (int)MarkerType.GizmoLocationZ)
						{
							int index = (int)marker.Type - 50; // LocationA has id 50...

							if (GizmoSpawningLocations[index] == null)
								GizmoSpawningLocations[index] = new List<PRTransform>();

							marker.PRTransform.Vector3D += Position;
							GizmoSpawningLocations[index].Add(marker.PRTransform);
						}
						//else
						//Logger.Warn("Unhandled marker type {0} - {1} - {2} in actor loading", marker.Type, marker.Name, markerSetData.FileName);
						break;

				}
			}
		}

		/// <summary>
		/// Preloads all markers for the scene.		
		/// </summary>
		public static void PreCacheMarkers()
		{
			//Logger.Info("Pre-caching markers for scenes...");
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.Scene].Values)
			{
				DiIiS_NA.Core.MPQ.FileFormats.Scene data = asset.Data as DiIiS_NA.Core.MPQ.FileFormats.Scene;
				if (data == null) continue;

				if (!PreCachedMarkers.ContainsKey(data.Header.SNOId))
					PreCachedMarkers.Add(data.Header.SNOId, new List<Marker>());

				foreach (var markerSet in data.MarkerSets)
				{
					var markerSetData = MPQStorage.Data.Assets[SNOGroup.MarkerSet][markerSet].Data as MarkerSet;
					if (markerSetData == null) return;
					/*Logger.Info("-------------------------------------");
					Logger.Info("Marker set name {0}", markerSet);
					Logger.Info("I0 {0}", markerSetData.I0);*/

					foreach (var marker in markerSetData.Markers)
					{
						switch (marker.Type)
						{
							case MarkerType.AmbientSound:
							case MarkerType.Light:
							case MarkerType.Particle:
							case MarkerType.SubScenePosition:
							case MarkerType.AudioVolume:
							case MarkerType.MinimapMarker:
							case MarkerType.Script:
							case MarkerType.Event:
								break;
							default:
								PreCachedMarkers[data.Header.SNOId].Add(marker);
								if (marker.TagMap.ContainsKey(MarkerKeys.ConversationList))
								{
									if (!WorldGenerator.DefaultConversationLists.ContainsKey(marker.SNOHandle.Id))
										WorldGenerator.DefaultConversationLists.Add(marker.SNOHandle.Id, marker.TagMap[MarkerKeys.ConversationList].Id);
								}
								/*if (marker.Type == Mooege.Common.MPQ.FileFormats.MarkerType.Actor)
								{
									Logger.Info("-------------");
									Logger.Info("Marker name {0}", marker.Name);
									Logger.Info("SNOScene {0}", data.Header.SNOId);
									Logger.Info("Marker SNOHandle {0}, position {1} - {2} - {3}", marker.SNOHandle.Id, marker.PRTransform.Vector3D.X, marker.PRTransform.Vector3D.Y, marker.PRTransform.Vector3D.Z);
									Logger.Info("Marker TagMap:");
									foreach (var tag in marker.TagMap)
										Logger.Info("TagMap tag: {0}", tag.ToString());
								}*/
								break;
						}

					}
				}
			}
		}

		#endregion

		#region scene revealing & unrevealing

		/// <summary>
		/// Returns true if the actor is revealed to player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns><see cref="bool"/></returns>
		public bool IsRevealedToPlayer(Player player)
		{
			return player.RevealedObjects.ContainsKey(GlobalID);
		}

		/// <summary>
		/// Reveal the scene to given player.
		/// </summary>
		/// <param name="player">Player to reveal scene.</param>
		/// <returns></returns>
		public override bool Reveal(Player player)
		{
			lock (player.RevealedObjects)
			{
				if (player.RevealedObjects.ContainsKey(GlobalID)) return false;

				player.RevealedObjects.Add(GlobalID, GlobalID);

				RevealSceneMessage message = RevealMessage(player);
				if (player.EventWeatherEnabled)
					//message.SceneSpec.SNOWeather = 50549; //Halloween
					message.SceneSpec.SNOWeather = 75198; //New Year
				
				player.InGameClient.SendMessage(message);// reveal the scene itself.
				player.InGameClient.SendMessage(MapRevealMessage(player));

				foreach (var sub in Subscenes)
					sub.Reveal(player);

				return true;
			}
		}

		/// <summary>
		/// Unreveals the scene to given player.
		/// </summary>
		/// <param name="player">Player to unreveal scene.</param>
		/// <returns></returns>
		public override bool Unreveal(Player player)
		{
			lock (player.RevealedObjects)
			{
				if (!player.RevealedObjects.ContainsKey(GlobalID)) return false;

				foreach (var actor in Actors) actor.Unreveal(player);
				/*
				player.InGameClient.SendMessage(new PreloadSceneDataMessage(Opcodes.PreloadRemoveSceneMessage)
				{
					idSScene = this.GlobalID,
					SNOScene = this.SceneSNO.Id,
					SnoLevelAreas = this.Specification.SNOLevelAreas
				});
				//*/
				player.InGameClient.SendMessage(new DestroySceneMessage() { WorldID = World.GlobalID, SceneID = GlobalID });

				foreach (var subScene in Subscenes) subScene.Unreveal(player);

				player.RevealedObjects.Remove(GlobalID);
				return true;
			}
		}

		#endregion

		#region scene-related messages

		/// <summary>
		/// Returns a RevealSceneMessage.
		/// </summary>
		public RevealSceneMessage RevealMessage(Player plr)
		{
			SceneSpecification specification = Specification;
			if (World.DRLGEmuActive)
			{
				specification.SNOMusic = World.Environment.snoMusic;
				specification.SNOCombatMusic = -1;//World.Environment.snoCombatMusic;
				specification.SNOAmbient = World.Environment.snoAmbient;
				specification.SNOReverb = World.Environment.snoReverb;
				specification.SNOPresetWorld = (int)World.SNO;
			}
			else if (World.Environment != null)
			{
				specification.SNOMusic = World.Environment.snoMusic;
				specification.SNOCombatMusic = -1;//World.Environment.snoCombatMusic;
				specification.SNOAmbient = World.Environment.snoAmbient;
				specification.SNOReverb = World.Environment.snoReverb;
				specification.SNOWeather = World.Environment.snoWeather;
				specification.SNOPresetWorld = (int)World.SNO;

			}
			else
			{
				specification.SNOMusic = -1;
				specification.SNOCombatMusic = -1;
				specification.SNOAmbient = -1;
				specification.SNOReverb = -1;
				if (specification.SNOWeather == -1)
					specification.SNOWeather = 0x00013220;
				specification.SNOPresetWorld = (int)World.SNO;
			}

			if (World.Game.NephalemGreater && World.SNO.IsDungeon())
				specification.SNOLevelAreas[0] = 332339;

			switch (World.SNO)
			{
				case WorldSno.p43_ad_oldtristram: specification.SNOLevelAreas[0] = 455466; break; //p43_ad_oldtristram 
				case WorldSno.p43_ad_cathedral_level_01: specification.SNOLevelAreas[0] = 452986; break; //p43_ad_cathedral_level_01 
				case WorldSno.p43_ad_cathedral_level_02: specification.SNOLevelAreas[0] = 452988; break; //p43_ad_cathedral_level_02 
				case WorldSno.p43_ad_cathedral_level_03: specification.SNOLevelAreas[0] = 452989; break; //p43_ad_cathedral_level_03 
				case WorldSno.p43_ad_cathedral_level_04: specification.SNOLevelAreas[0] = 452990; break; //p43_ad_cathedral_level_04 
				case WorldSno.p43_ad_catacombs_level_05: specification.SNOLevelAreas[0] = 452992; break; //p43_ad_catacombs_level_05 
				case WorldSno.p43_ad_catacombs_level_06: specification.SNOLevelAreas[0] = 452993; break; //p43_ad_catacombs_level_06 
				case WorldSno.p43_ad_catacombs_level_07: specification.SNOLevelAreas[0] = 452994; break; //p43_ad_catacombs_level_07 
				case WorldSno.p43_ad_catacombs_level_08: specification.SNOLevelAreas[0] = 452995; break; //p43_ad_catacombs_level_08 
				case WorldSno.p43_ad_caves_level_09: specification.SNOLevelAreas[0] = 453001; break; //p43_ad_caves_level_09 
				case WorldSno.p43_ad_caves_level_10: specification.SNOLevelAreas[0] = 453007; break; //p43_ad_caves_level_10 
				case WorldSno.p43_ad_caves_level_11: specification.SNOLevelAreas[0] = 453006; break; //p43_ad_caves_level_11 
				case WorldSno.p43_ad_caves_level_12: specification.SNOLevelAreas[0] = 453005; break; //p43_ad_caves_level_12 
				case WorldSno.p43_ad_hell_level_13: specification.SNOLevelAreas[0] = 453009; break; //p43_ad_hell_level_13 
				case WorldSno.p43_ad_hell_level_14: specification.SNOLevelAreas[0] = 453011; break; //p43_ad_hell_level_14 
				case WorldSno.p43_ad_hell_level_15: specification.SNOLevelAreas[0] = 453012; break; //p43_ad_hell_level_15 
				case WorldSno.p43_ad_hell_level_16: specification.SNOLevelAreas[0] = 453441; break; //p43_ad_hell_level_16 
				case WorldSno.p43_ad_level02_sidedungeon_darkpassage: specification.SNOLevelAreas[0] = 453441; break; //p43_ad_level02_sidedungeon_darkpassage 
				case WorldSno.p43_ad_level03_sidedungeon_leoricstomb: specification.SNOLevelAreas[0] = 453471; break; //p43_ad_level03_sidedungeon_leoricstomb 
				case WorldSno.p43_ad_level06_sidedungeon_chamberofbone: specification.SNOLevelAreas[0] = 453583; break; //p43_ad_level06_sidedungeon_chamberofbone 
				case WorldSno.p43_ad_abandonedfarmstead: specification.SNOLevelAreas[0] = 458256; break; //p43_ad_abandonedfarmstead 
				case WorldSno.p43_ad_level15_sidedungeon_unholyaltar: specification.SNOLevelAreas[0] = 454209; break; //p43_ad_level15_sidedungeon_unholyaltar 
				default: break; // don't change anything for other worlds
			}
			return new RevealSceneMessage
			{
				WorldID = World.GlobalID,
				SceneSpec = specification,
				ChunkID = GlobalID,
				Transform = Transform,
				SceneSNO = SceneSNO.Id,
				ParentChunkID = ParentChunkID,
				SceneGroupSNO = SceneGroupSNO,
				arAppliedLabels = AppliedLabels,
				snoActors = Field8,
				Vista = 0
			};
		}

		/// <summary>
		/// Returns a MapRevealSceneMessage.
		/// </summary>
		public MapRevealSceneMessage MapRevealMessage(Player plr)
		{
			if (
			#region Город первого акта
					SceneSNO.Id == 1904 ||
				   SceneSNO.Id == 33342 ||
				   SceneSNO.Id == 33343 ||

				   SceneSNO.Id == 33347 ||
				   SceneSNO.Id == 33348 ||
				   SceneSNO.Id == 33349 ||
				   SceneSNO.Id == 414798
			#endregion
					||
			#region Город второго акта
					SceneSNO.Id == 161516 ||
				   SceneSNO.Id == 161510 ||
				   SceneSNO.Id == 185542 ||

				   SceneSNO.Id == 161507 ||
				   SceneSNO.Id == 161513 ||
				   SceneSNO.Id == 185545
			#endregion
					||
			#region Город третьего акта
					SceneSNO.Id == 172892 ||
				   SceneSNO.Id == 172880 ||
				   SceneSNO.Id == 172868 ||

				   SceneSNO.Id == 172888 ||
				   SceneSNO.Id == 172876 ||
				   SceneSNO.Id == 172863 ||

				   SceneSNO.Id == 172884 ||
				   SceneSNO.Id == 172872 ||
				   SceneSNO.Id == 172908
			#endregion
					||
			#region Город четвертого акта
					SceneSNO.Id == 183555 ||
				   SceneSNO.Id == 183556 ||
				   SceneSNO.Id == 183557 ||

				   SceneSNO.Id == 183502 ||
				   SceneSNO.Id == 183505 ||
				   SceneSNO.Id == 183557 ||

				   SceneSNO.Id == 183519 ||
				   SceneSNO.Id == 183545 ||
				   SceneSNO.Id == 183553
			#endregion
					||
			#region Город пятого акта
					SceneSNO.Id == 315706 ||
				   SceneSNO.Id == 311307 ||
				   SceneSNO.Id == 311295 ||

				   SceneSNO.Id == 313849 ||
				   SceneSNO.Id == 311316 ||
				   SceneSNO.Id == 313845 ||

				   SceneSNO.Id == 315710 ||
				   SceneSNO.Id == 311310 ||
				   SceneSNO.Id == 311298 ||

				   SceneSNO.Id == 313853 ||
				   SceneSNO.Id == 311313 ||
				   SceneSNO.Id == 311301 ||
				   SceneSNO.Id == 313857
			#endregion
					)
			{
				return new MapRevealSceneMessage
				{
					ChunkID = GlobalID,
					SceneSNO = SceneSNO.Id,
					Transform = Transform,
					WorldID = World.GlobalID,
					MiniMapVisibility = true
				};
			}
			else
			{
				return new MapRevealSceneMessage
				{
					ChunkID = GlobalID,
					SceneSNO = SceneSNO.Id,
					Transform = Transform,
					WorldID = World.GlobalID,
					MiniMapVisibility = false
				};
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("[Scene] SNOId: {0} DynamicId: {1} Name: {2}", SceneSNO.Id, GlobalID, SceneSNO.Name);
		}
	}

	/// <summary>
	/// Minimap visibility of the scene on map.
	/// </summary>
	public enum SceneMiniMapVisibility
	{
		/// <summary>
		/// Hidden.
		/// </summary>
		Hidden = 0,
		/// <summary>
		/// Revealed to player.
		/// </summary>
		Revealed = 1,
		/// <summary>
		/// Player already visited the scene.
		/// </summary>
		Visited = 2
	}
}
