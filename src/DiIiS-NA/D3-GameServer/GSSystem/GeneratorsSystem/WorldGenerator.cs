using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.Collision;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
using Scene = DiIiS_NA.GameServer.GSSystem.MapSystem.Scene;
using Affix = DiIiS_NA.GameServer.GSSystem.ItemsSystem.Affix;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.WorldSceneBase.Entities;
using DiIiS_NA.GameServer.Core.Types.Scene;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public class DRLGEmuScene
	{
		public int SnoID;
		public Asset Asset;
		public int Weather;
		public int Music;
		public int LevelArea;

		public DRLGEmuScene(int _SnoID, int _Weather, int _Music, int _LevelArea)
		{
			SnoID = _SnoID;
			Weather = _Weather;
			Music = _Music;
			LevelArea = _LevelArea;
		}
	}

	public class WorldGenerator
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		private static readonly ActorSno[] d1ModeHiddenActors = new ActorSno[]
		{
			ActorSno._x1_mysticintro_npc,
			ActorSno._tristramfemale,
			ActorSno._a1_uniquevendor_armorer,
			ActorSno._x1_lore_mysticnotes,
			ActorSno._templarnpc_imprisoned,
			ActorSno._adventurer_d_templarintrounique,
			ActorSno._x1_catacombs_jeweler,
			ActorSno._waypoint,
		};

		public Game Game { get; set; }

		public WorldGenerator(Game game)
		{
			Game = game;
		}

		public static Dictionary<int, int> DefaultConversationLists = new();

		private readonly List<int> LoadedLevelAreas = new();

		public void CheckLevelArea(World world, int levelAreaSNO)
		{
			if (SpawnGenerator.Spawns.ContainsKey(levelAreaSNO) && SpawnGenerator.Spawns[levelAreaSNO].LazyLoad)
				if (!LoadedLevelAreas.Contains(levelAreaSNO))
				{
					LoadedLevelAreas.Add(levelAreaSNO);

					// Load monsters for level area
					foreach (var scene in _lazyLevelAreas[levelAreaSNO])
					{
						LoadMonstersLayout(world, levelAreaSNO, scene);
					}
				}
		}

		public World Generate(WorldSno worldSNO)
		{
			if (!MPQStorage.Data.Assets[SNOGroup.Worlds].ContainsKey((int)worldSNO))
			{
				Logger.Error("Can't find a valid world definition for sno: {0}", worldSNO);
				return null;
			}

			var worldAsset = MPQStorage.Data.Assets[SNOGroup.Worlds][(int)worldSNO];
			Dictionary<int, List<Scene>> levelAreas = new Dictionary<int, List<Scene>>();
			World world = new World(Game, worldSNO);
			bool DRLGEmuActive = false;
			world.worldData = (DiIiS_NA.Core.MPQ.FileFormats.World)worldAsset.Data;
			if (worldSNO == WorldSno.a2dun_swr_swr_to_oasis_level01)
				world.worldData.DynamicWorld = true;

			//445736 - p4_forest_snow_icecave_01
			if (world.worldData.DynamicWorld && !worldSNO.IsNotDynamicWorld()) //Gardens of Hope - 2 lvl is NOT random
			{
				if (!GameServerConfig.Instance.DRLGemu)
					Logger.Warn("DRLG-Emu is Disabled.");
				string DRLGVersion = "1.8";
				var WorldContainer = DBSessions.WorldSession.Query<DRLG_Container>().Where(dbt => dbt.WorldSNO == (int)worldSNO).ToList();
				if (WorldContainer.Count > 0 && worldSNO != WorldSno.a1trdun_level05_templar && GameServerConfig.Instance.DRLGemu)
				{
					DRLGEmuActive = true;
					Logger.Warn("World - {0} [{1}] is dynamic! Found container, DRLG-Emu v{2} Activated!", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
				}
				else if (!GenerateRandomDungeon(worldSNO, world.worldData))
				{
					Logger.Error("DRLG-Emu v{2} - World - {0} [{1}] is dynamic! DRLG Engine can't find container! Template system is not configured for this world.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
					return null;
				}
				else
				{
					Logger.Warn("DRLG-Emu v{2} - World - {0} [{1}] is dynamic! DRLG Engine can't find container! Template system is started.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
					if (world.worldData.DRLGParams != null)
					{
						world.NextLocation = new MessageSystem.Message.Fields.ResolvedPortalDestination
						{
							WorldSNO = world.worldData.DRLGParams[0].NextWorld,
							DestLevelAreaSNO = world.worldData.DRLGParams[0].NextLevelArea,
							StartingPointActorTag = world.worldData.DRLGParams[0].NextStartingPoint
						};
						world.PrevLocation = new MessageSystem.Message.Fields.ResolvedPortalDestination
						{
							WorldSNO = world.worldData.DRLGParams[0].PrevWorld,
							DestLevelAreaSNO = world.worldData.DRLGParams[0].PrevLevelArea,
							StartingPointActorTag = world.worldData.DRLGParams[0].PrevStartingPoint
						};
					}
					else
					{
						Logger.Warn("DRLG-Emu v{2} - World - {0} [{1}] is dynamic! DRLG Engine can't find container! Template system is not configured for this world.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
						return null;
					}
				}


			}
			else
			{
				world.PrevLocation = new MessageSystem.Message.Fields.ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.__NONE,
					DestLevelAreaSNO = -1,
					StartingPointActorTag = -1
				};
			}

			if (DRLGEmuActive)
			{
				List<List<DRLGEmuScene>> containers = new List<List<DRLGEmuScene>> { };
				List<DRLGEmuScene> enterChunks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> exitChunks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> wayChunks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> endChunks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> fillerChunks = new List<DRLGEmuScene> { };

				var WorldContainer = DBSessions.WorldSession.Query<DRLG_Container>().Where(dbt => dbt.WorldSNO == (int)world.SNO).First();
				var tiles = DBSessions.WorldSession.Query<DRLG_Tile>().Where(dbt => dbt.Head_Container == (int)WorldContainer.Id).ToList();

			REP:
				tiles = DBSessions.WorldSession.Query<DRLG_Tile>().Where(dbt => dbt.Head_Container == (int)WorldContainer.Id).ToList();
				//All Scenes
				foreach (var Tile in tiles)
				{
					switch (Tile.Type)
					{
						case 0: //enter
							enterChunks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 1: //exits
							exitChunks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 2: //way
							wayChunks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 3: //dead ends
							endChunks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 4: //fillers
							fillerChunks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
					}
				}
				containers.Add(enterChunks);
				containers.Add(exitChunks);
				containers.Add(wayChunks);
				containers.Add(endChunks);
				containers.Add(fillerChunks);

				if (world.SNO.IsGenerated())
					while (true)
					{

						DRLGGenerateProcess(world, containers, fillerChunks, WorldContainer.RangeofScenes);
						if (world.worldData.SceneParams.ChunkCount > 15)
							break;

					}
				else
				{
					try
					{
						DRLGGenerateProcess(world, containers, fillerChunks, WorldContainer.RangeofScenes);
					}
					catch
					{
						Logger.Info("DRLG generator found an error in the calculation, repeat.");
						goto REP;
					}
				}
				Logger.Info("DRLG work - Completed");
			}

			var clusters = new Dictionary<int, SceneCluster>();
			if (world.worldData.SceneClusterSet != null)
			{
				foreach (var cluster in world.worldData.SceneClusterSet.SceneClusters)
					clusters[cluster.ClusterId] = cluster;
			}

			float minX = 0.0f;
			float minY = 0.0f;

			minX = world.worldData.SceneParams.SceneChunks.Min(x => x.PRTransform.Vector3D.X);
			minY = world.worldData.SceneParams.SceneChunks.Min(x => x.PRTransform.Vector3D.Y);
			
			var clusterCount = new Dictionary<int, int>();

			foreach (var sceneChunk in world.worldData.SceneParams.SceneChunks)
			{
				var cID = sceneChunk.SceneSpecification.ClusterID;
				if (cID != -1 && clusters.ContainsKey(cID)) 
				{
					if (!clusterCount.ContainsKey(cID))
						clusterCount[cID] = 0; 
					clusterCount[cID]++;
				}
			}

			// For each cluster generate a list of randomly selected subcenes /fasbat
			var clusterSelected = new Dictionary<int, List<SubSceneEntry>>();
			foreach (var cID in clusterCount.Keys)
			{
				var selected = new List<SubSceneEntry>();
				clusterSelected[cID] = selected;
				var count = clusterCount[cID];
				foreach (var group in clusters[cID].SubSceneGroups) // First select from each subscene group /fasbat
				{
					for (int i = 0; i < group.I0 && count > 0; i++, count--) //TODO Rename I0 to requiredCount? /fasbat
					{
						var subSceneEntry = RandomHelper.RandomItem(group.Entries, entry => entry.Probability);
						selected.Add(subSceneEntry);
					}

					if (count == 0)
						break;
				}

				while (count > 0) // Fill the rest with defaults /fasbat
				{
					//Default subscenes are not currently stored in db, use first if available
					//var subSceneEntry = RandomHelper.RandomItem(clusters[cID].Default.Entries, entry => entry.Probability);
					if (clusters[cID].SubSceneGroups.Count > 0)
					{
						var subSceneEntry = RandomHelper.RandomItem(clusters[cID].SubSceneGroups.First().Entries, entry => entry.Probability);
						selected.Add(subSceneEntry);
					}
					count--;
				}
			}

			if (!(world.IsPvP && World.PvPMapLoaded))
				foreach (var sceneChunk in world.worldData.SceneParams.SceneChunks)
				{
					var position = sceneChunk.PRTransform.Vector3D - new Vector3D(minX, minY, 0);
					var scene = new Scene(world, position, sceneChunk.SNOHandle.Id, null)
					{
						MiniMapVisibility = (world.Game.PvP || world.IsPvP),
						RotationW = sceneChunk.PRTransform.Quaternion.W,
						RotationAxis = sceneChunk.PRTransform.Quaternion.Vector3D,
						SceneGroupSNO = -1,
						Specification = sceneChunk.SceneSpecification,
						TileType = sceneChunk.SceneSpecification.OnPathBits
					};

					if (sceneChunk.SceneSpecification.ClusterID != -1)
					{
						if (!clusters.ContainsKey(sceneChunk.SceneSpecification.ClusterID))
						{
							Logger.Trace("Referenced clusterID {0} not found for chunk {1} in world {2}", sceneChunk.SceneSpecification.ClusterID, sceneChunk.SNOHandle.Id, worldSNO);
						}
						else
						{
							var entries = clusterSelected[sceneChunk.SceneSpecification.ClusterID];
							if (entries.TryPickRandom(out var subSceneEntry))
							{
								entries.Remove(subSceneEntry);
							}
							else
								Logger.Error("No SubScenes defined for cluster {0} in world {1}", sceneChunk.SceneSpecification.ClusterID, world.GlobalID);

							Vector3D pos = FindSubScenePosition(sceneChunk);

							if (pos == null)
							{
								Logger.Error("No scene position marker for SubScenes of Scene {0} found", sceneChunk.SNOHandle.Id);
							}
							else
							{
								if (subSceneEntry != null)
								{
									if (MPQStorage.Data.Assets[SNOGroup.Scene].ContainsKey(subSceneEntry.SNOScene))
									{
										var subScenePosition = scene.Position + pos;
										var subscene = new Scene(world, subScenePosition, subSceneEntry.SNOScene, scene)
										{
											MiniMapVisibility = false,
											RotationW = sceneChunk.PRTransform.Quaternion.W,
											RotationAxis = sceneChunk.PRTransform.Quaternion.Vector3D,
											Specification = sceneChunk.SceneSpecification
										};
										scene.Subscenes.Add(subscene);
										subscene.LoadMarkers();
									}
									else
									{
										Logger.Error("Scene not found in mpq storage: {0}", subSceneEntry.SNOScene);
									}
								}
							}
						}
					}
					scene.LoadMarkers();

					// add scene to level area dictionary
					foreach (var levelArea in scene.Specification.SNOLevelAreas)
					{
						if (levelArea != -1)
						{
							if (!levelAreas.ContainsKey(levelArea))
								levelAreas.Add(levelArea, new List<Scene>());

							levelAreas[levelArea].Add(scene);
						}
					}
				}

			if (world.IsPvP)
				World.PvPMapLoaded = true;

			//world.LevelAreasData = levelAreas;

			if (worldSNO == WorldSno.a1trdun_level05_templar)
				world.SpawnMonster(ActorSno._waypoint, new Vector3D { X = 700.67f, Y = 580.128f, Z = 0.1f });

			try
			{
				if (!world.IsPvP)
					LoadLevelAreas(levelAreas, world);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "loadLevelAreas exception: ");
			}

			#region patches
			switch (worldSNO)
			{
				case WorldSno.x1_pand_ext_2_battlefields: //x1_pand_ext_2_battlefields
					RandomSpawnInWorldWithLevelArea(world, ActorSno._x1_pandext_siegerune);
					RandomSpawnInWorldWithLevelArea(world, ActorSno._x1_pandext_siegerune);
					break;
				case WorldSno.x1_westm_zone_03:
					RandomSpawnInWorldWithLevelArea(world, ActorSno._x1_deathmaiden_unique_fire_a);
					RandomSpawnInWorldWithLevelArea(world, ActorSno._x1_deathmaiden_unique_fire_a);
					RandomSpawnInWorldWithLevelArea(world, ActorSno._x1_deathmaiden_unique_fire_a);
					break;
				case WorldSno.trdun_leoric_level03: //Setting portal to the third floor of the Agony's Halls near the entrance to the Butcher.
					Vector3D sceneOfPos = world.GetSceneBySnoId(78824).Position;
					world.SpawnMonster(ActorSno._waypoint, new Vector3D(sceneOfPos.X + 149.0907f, sceneOfPos.Y + 106.7075f, sceneOfPos.Z));
					break;
				case WorldSno.x1_westm_graveyard_deathorb:
					FilterWaypoints(world);
					break;
				case WorldSno.x1_lr_tileset_hexmaze:
					foreach (var actor in world.GetActorsBySNO(
						ActorSno._x1_pand_hexmaze_en_lore_sister1_chest,
						ActorSno._x1_pand_hexmaze_en_lore_sister2_chest,
						ActorSno._x1_pand_hexmaze_en_lore_sister3_chest,
						ActorSno._x1_pand_hexmaze_en_enchantress
						)) actor.Destroy();
					break;
				case WorldSno.trout_town: //mercenary
					var templar = world.GetActorBySNO(ActorSno._templar);
					var hasMalthaelNpc = world.GetActorBySNO(ActorSno._x1_malthael_npc);

					if (hasMalthaelNpc == null)
					{
						ActorSystem.Implementations.Hirelings.MalthaelHireling malthaelHire = new ActorSystem.Implementations.Hirelings.MalthaelHireling(world, ActorSno._x1_malthael_npc_nocollision, templar.Tags)
							{
								RotationAxis = new Vector3D(0f, 0f, 0.4313562f),
								RotationW = 0.9021817f,
								Attributes =
								{
									[GameAttributes.Team_Override] = 2
								}
							};
						malthaelHire.EnterWorld(new Vector3D(3017.266f, 2851.986f, 24.04533f));
					}
					foreach (var door in world.GetActorsBySNO(ActorSno._house_door_trout_newtristram))
						door.Destroy();
					if (Game.CurrentAct == ActEnum.OpenWorld)
					{
						var townDoor = world.GetActorBySNO(ActorSno._trout_newtristram_gate_town);
						townDoor.Attributes[GameAttributes.Team_Override] = 2;
						townDoor.Attributes[GameAttributes.Untargetable] = true;
						townDoor.Attributes[GameAttributes.NPC_Is_Operatable] = false;
						townDoor.Attributes[GameAttributes.Operatable] = false;
						townDoor.Attributes[GameAttributes.Operatable_Story_Gizmo] = false;
						townDoor.Attributes[GameAttributes.Disabled] = true;
						townDoor.Attributes[GameAttributes.Immunity] = true;
						townDoor.Attributes.BroadcastChangedIfRevealed();
					}
					break;
				case WorldSno.a1trdun_level04: //Cathedral Level 2
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
				case WorldSno.a1trdun_level06: //Cathedral Level 4
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
				case WorldSno.a1trdun_level05_templar: //Cathedral Level 3
					foreach (var actor in world.GetActorsBySNO(
						ActorSno._x1_mysticintro_npc,
						ActorSno._tristramfemale,
						ActorSno._omninpc_tristram_male_e,
						ActorSno._x1_lore_mysticnotes,
						ActorSno._a1_uniquevendor_armorer
						)) actor.Destroy();
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;

				case WorldSno.a2dun_swr_swr_to_oasis_level01: //kill useless portal in location if game not in adventure mode
					if (Game.CurrentAct != ActEnum.OpenWorld)
						foreach (var waypoint in world.GetActorsBySNO(ActorSno._waypoint)) waypoint.Destroy();
					break;
				case WorldSno.a2dun_zolt_head_random01: //remove blood pool
					foreach (var act in world.GetActorsBySNO(ActorSno._a2dun_zolt_blood_container_02)) act.Destroy();
					break;
				case WorldSno.a2dun_aqd_special_01: //Main Drain. Remove useless portals.
					foreach (var port in world.Actors.Values)
						if (port is Portal portal)
							if (portal.Destination.WorldSNO == (int)WorldSno.a2dun_aqd_special_b_level01)
								portal.Destroy();
					break;
				case WorldSno.a3dun_keep_level04: //kill useless portal in location if game not in adventure mode
					if (Game.CurrentAct != ActEnum.OpenWorld)
						foreach (var waypoint in world.GetActorsBySNO(ActorSno._waypoint)) waypoint.Destroy();
					break;
				#region kill all portals in demonic rifts on the first floor of the gardens (now and on the second floor), because there are a lot of them), the script will create a script to destroy the demon. Add the voice of Diablo to several areas;
				case WorldSno.a4dun_garden_of_hope_01: //1st floor of the gardens
					foreach (var hellPortal in world.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal))
						hellPortal.Destroy();
					break;
				case WorldSno.a4dun_garden_of_hope_random: //2nd floor of the gardens
					foreach (var hellPortal in world.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal))
						hellPortal.Destroy();
					break;
                #endregion
				case WorldSno.a4dun_spire_level_00:
					var leahGhost = world.SpawnMonster(ActorSno._a4dun_aspect_ghost_07, new Vector3D(570f, 570f, 0.1f)) as InteractiveNPC;
					leahGhost.Conversations.Clear();
					leahGhost.Conversations.Add(new ConversationInteraction(198600));
					leahGhost.Attributes[GameAttributes.Conversation_Icon, 0] = 6;
					leahGhost.Attributes.BroadcastChangedIfRevealed();
					break;
					//428f, 836f, -20.3f
				case WorldSno.a4dun_spire_level_01:
					var zoltunGhost = world.SpawnMonster(ActorSno._a4dun_aspect_ghost_02, new Vector3D(428f, 836f, -2f)) as InteractiveNPC;
					zoltunGhost.Conversations.Clear();
					zoltunGhost.Conversations.Add(new ConversationInteraction(198402));
					zoltunGhost.Attributes[GameAttributes.Conversation_Icon, 0] = 6;
					zoltunGhost.Attributes.BroadcastChangedIfRevealed();
					break;
				case WorldSno.a3dun_ruins_frost_city_a_02:
					foreach (var waypoint in world.GetActorsBySNO(ActorSno._waypoint)) 
						waypoint.Destroy();
					break;
				case WorldSno.p43_ad_oldtristram:
					foreach (var waypoint in world.GetActorsBySNO(ActorSno._trout_oldtristram_exit_gate)) 
						waypoint.Destroy();
					break;
				case WorldSno.x1_tristram_adventure_mode_hub:
					
					//Display only one seller
					world.ShowOnlyNumNPC(ActorSno._a1_uniquevendor_miner_intown_01, 0);
					//Display only one mystic
					world.ShowOnlyNumNPC(ActorSno._pt_mystic, 1);
					var Door = world.GetActorBySNO(ActorSno._trout_newtristram_gate_town);
					Door.Attributes[GameAttributes.Team_Override] = 2;
					Door.Attributes[GameAttributes.Untargetable] = true;
					Door.Attributes[GameAttributes.NPC_Is_Operatable] = false;
					Door.Attributes[GameAttributes.Operatable] = false;
					Door.Attributes[GameAttributes.Operatable_Story_Gizmo] = false;
					Door.Attributes[GameAttributes.Disabled] = true;
					Door.Attributes[GameAttributes.Immunity] = true;
					Door.Attributes.BroadcastChangedIfRevealed();
					break;
				case WorldSno.p43_ad_cathedral_level_01: //1st floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_cathedral_level_02: //2nd floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_cathedral_level_03: //3rd floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_cathedral_level_04: //4th floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_catacombs_level_05: //5th floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_catacombs_level_06: //6th floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_catacombs_level_07: //7th floor of the cathedral (D1 mode)
				case WorldSno.p43_ad_catacombs_level_08: //8th floor of the cathedral (D1 mode)
					foreach (var actor in world.GetActorsBySNO(d1ModeHiddenActors)) 
						actor.Destroy();
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var startingPoint in actor.GetActorsInRange<StartingPoint>(20f)) startingPoint.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
			}
			#endregion
			#region Global patch when generating
			foreach (var oldPoint in world.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal, ActorSno._x1_openworld_tiered_rifts_portal,
															   ActorSno._x1_openworld_tiered_rifts_challenge_portal, ActorSno._x1_westm_bridge_scoundrel)) 
			{ 
				oldPoint.Destroy();
			}
			foreach (var oldPoint in world.GetActorsBySNO(ActorSno._placedgold)) { foreach(var plr in world.Game.Players.Values) world.SpawnGold(oldPoint, plr); oldPoint.Destroy(); }
																				  
			if(world.SNO != WorldSno.a1trdun_level05_templar) 
				foreach (var oldPoint in world.GetActorsBySNO(ActorSno._x1_openworld_tiered_rifts_challenge_portal)) { oldPoint.Destroy(); }//109209 - Bone Walls from the Cathedral
			#endregion

			return world;
		}

		public void RandomSpawnInWorldWithLevelArea(World world, ActorSno monsterSno, int levelArea = -1)
		{
			List<Scene> scenes = world.Scenes.Values.ToList();
			scenes = levelArea != -1 
				? scenes.Where(sc => sc.Specification.SNOLevelAreas[0] == levelArea && !sc.SceneSNO.Name.ToLower().Contains("filler")).ToList() 
				: scenes.Where(sc => !sc.SceneSNO.Name.ToLower().Contains("filler")).ToList();
			Vector3D randomScene = scenes.PickRandom().Position;
			Vector3D startingPoint = null;

			while (true)
			{
				startingPoint = new Vector3D(randomScene.X + RandomHelper.Next(0, 240), randomScene.Y + RandomHelper.Next(0, 240), randomScene.Z);
				if (world.CheckLocationForFlag(startingPoint, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					break;
			}
			world.SpawnMonster(monsterSno, startingPoint);
		}

		public void FilterWaypoints(World world, int sceneSno = -1)
		{
			var waypoints = world.GetActorsBySNO(ActorSno._waypoint);
			if (sceneSno != -1) waypoints = waypoints.Where(wp => wp.CurrentScene.SceneSNO.Id == sceneSno).ToList();

			if (waypoints.Count > 1)
			{
				int randomPoint = RandomHelper.Next(0, waypoints.Count);
				for (int i = 0; i < waypoints.Count; i++)
				{
					if (i != randomPoint)
						waypoints[i].Destroy();
				}
			}
		}

		/// <summary>
		/// Status of an added exit to world
		/// Used when a new tile is needed in a specific place
		/// </summary>
		public enum ExitStatus
		{
			Free, //no tile in that direction
			Blocked, //"wall" in that direction
			Open //"path" in that direction
		}
		public static void DRLGGenerateProcess(World world, List<List<DRLGEmuScene>> container, List<DRLGEmuScene> fillers, long range)
		{
			if (world.worldData.SceneParams == null)
				world.worldData.CreateNewSceneParams();

			world.worldData.SceneParams.SceneChunks.Clear();
			List<Vector3D> busyChunks = new List<Vector3D> { };
			List<Vector3D> reservedChunks = new List<Vector3D> { };
			Dictionary<char[], Vector3D> waitChunks = new Dictionary<char[], Vector3D> { };
			Dictionary<char[], Vector3D> nextWaitChunks = new Dictionary<char[], Vector3D> { };

			bool hasExit = false;

			List<List<DRLGEmuScene>> DRLGContainers = container;
			List<DRLGEmuScene> FillerChuncks = fillers;

			char currentNav = '.';
			
			DRLGEmuScene currentScene = null;

			foreach (var Container in DRLGContainers)
				foreach (var Scene in Container)
					try
					{
						Scene.Asset = MPQStorage.Data.Assets[SNOGroup.Scene][Scene.SnoID];
					}
					catch
					{
						Logger.Error("Scene {0}, not added on DRLG", Scene.SnoID);
					}
			bool rift = world.SNO.IsGenerated();
			//Getting Enter
			var loadedScene = new SceneChunk();
			currentScene = DRLGContainers[0].PickRandom();
			loadedScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
			loadedScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), new Vector3D(0, 0, 0));
			loadedScene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));

			world.worldData.SceneParams.SceneChunks.Add(loadedScene); //Add Chunk
			busyChunks.Add(loadedScene.PRTransform.Vector3D); //Cords Busy

			SceneChunk prestScene = loadedScene;
			Vector3D placeToNewScene = new Vector3D();

			var nextScene = new SceneChunk();
			char[] toWaitChunks;
			var splits = prestScene.SNOHandle.Name.Split('_');
			int positionOfNav = 2;
			if (splits[0].ToLower().StartsWith("p43") ||
				splits[2].ToLower().Contains("random") ||
				splits[2].ToLower().Contains("corrupt"))
				positionOfNav = 3;
			else if (prestScene.SNOHandle.Name.StartsWith("x1_Pand"))
				positionOfNav = 4;

			int RangetoNext = (int)range;

			//First Switch
			switch (prestScene.SNOHandle.Name.Split('_')[positionOfNav])
			{

				case "S":
					currentNav = 'N';
					while (true)
					{
						if (prestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							positionOfNav = 3;
						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X + RangetoNext, prestScene.PRTransform.Vector3D.Y, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var Point in toWaitChunks)
						if (Point != currentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								case 'N': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								case 'E': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								case 'W': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextScene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextScene); //Add scene
					busyChunks.Add(nextScene.PRTransform.Vector3D); //Occupy cell
					prestScene = nextScene;
					break;
				case "N":
					currentNav = 'S';
					while (true)
					{
						if (prestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							positionOfNav = 3;

						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X - RangetoNext, prestScene.PRTransform.Vector3D.Y, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var point in toWaitChunks)
						if (point != currentNav)
							switch (point)
							{
								//S - build to the right +240:0
								case 'S': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//N - build to the left -240:0
								case 'N': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//E - build up 0:+240
								case 'E': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								//W - build down 0:-240
								case 'W': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextScene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextScene); //Добавить сцену
					busyChunks.Add(nextScene.PRTransform.Vector3D); //Занять клетку
					prestScene = nextScene;
					break;
				case "E":
					currentNav = 'W';
					while (true)
					{
						if (prestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							positionOfNav = 3;

						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X, prestScene.PRTransform.Vector3D.Y + RangetoNext, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var Point in toWaitChunks)
						if (Point != currentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								case 'N': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								case 'E': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								case 'W': waitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextScene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextScene); //Добавить сцену
					busyChunks.Add(nextScene.PRTransform.Vector3D); //Занять клетку
					prestScene = nextScene;
					break;
				case "W":

					currentNav = 'E';
					while (true)
					{
						if (prestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							positionOfNav = 3;

						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X, prestScene.PRTransform.Vector3D.Y - RangetoNext, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var point in toWaitChunks)
						if (point != currentNav)
							switch (point)
							{
								//S - Build to the right +240:0
								case 'S': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//N - Build to the left -240:0
								case 'N': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//E - Build up 0:+240
								case 'E': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								//W - Build down 0:-240
								case 'W': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextScene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextScene); // add scene
					busyChunks.Add(nextScene.PRTransform.Vector3D); //occupy cell
					prestScene = nextScene;
					break;

				case "EW":

					var nextscene1 = new SceneChunk();

					currentNav = 'E';
					while (true)
					{
						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X, prestScene.PRTransform.Vector3D.Y - RangetoNext, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var point in toWaitChunks)
						if (point != currentNav)
							switch (point)
							{
								//S - Build to the right +240:0
								case 'S': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//N - Build to the left -240:0
								case 'N': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//E - Build up 0:+240
								case 'E': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								//W - Build down 0:-240
								case 'W': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextscene1.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextscene1.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextscene1.SceneSpecification = new SceneSpecification(
								0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
								-1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene1); //Добавить сцену
					busyChunks.Add(nextscene1.PRTransform.Vector3D); //Занять клетку

					currentNav = 'W';
					while (true)
					{
						currentScene = DRLGContainers[2].PickRandom();
						if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					placeToNewScene = new Vector3D(prestScene.PRTransform.Vector3D.X, prestScene.PRTransform.Vector3D.Y + RangetoNext, prestScene.PRTransform.Vector3D.Z);
					toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
					foreach (var point in toWaitChunks)
						if (point != currentNav)
							switch (point)
							{
								//S - Build to the right +240:0
								case 'S': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//N - Build to the left -240:0
								case 'N': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
								//E - Build up 0:+240
								case 'E': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
								//W - Build down 0:-240
								case 'W': waitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
							}
					nextScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
					nextScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
					nextScene.SceneSpecification = new SceneSpecification(
								0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
								-1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextScene); //Добавить сцену
					busyChunks.Add(nextScene.PRTransform.Vector3D); //Занять клетку
					prestScene = nextScene;

					break;
			}
			int DRLGDeep = 5;
			if (rift)
				DRLGDeep = 20;


				//Deep and exits
			for (int i = 0; i <= DRLGDeep; i++)
			{
				foreach (var waitedScene in waitChunks)
				{
					var newScene = new SceneChunk();
					switch (waitedScene.Key[0])
					{
						case 'S':
							currentNav = 'N';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (hasExit)
									{
										currentScene = DRLGContainers[3].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //Way
										{
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
									else
									{
										currentScene = DRLGContainers[1].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav)) //Exit
										{
											hasExit = true;
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
								}
								else
								{
									currentScene = DRLGContainers[2].PickRandom();
									#region проверка на будущее
									toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var point in toWaitChunks)
										if (point != waitedScene.Key[0])
											switch (point)
											{
												//S - Build to the right
												case 'S':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												
												//N - Build to the left
												case 'N':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												
												//E - Build up
												case 'E':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												
												//W - Build down
												case 'W':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)) ||
														 (reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
									if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break; 
									}
								}
							}
							placeToNewScene = waitedScene.Value;
							toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();

							foreach (var point in toWaitChunks)
								if (point != currentNav)
									switch (point)
									{
										//S - Build right +240:0
										case 'S': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										//N - Build left  -240:0
										case 'N': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										//E - Build up    0:+240
										case 'E': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
										//W - Build down  0:-240
										case 'W': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
									}
							newScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
							newScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
							newScene.SceneSpecification = new SceneSpecification(
								   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
								   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								   new int[4] { 0, 0, 0, 0 }, 0));

							world.worldData.SceneParams.SceneChunks.Add(newScene); //Добавить сцену
							busyChunks.Add(newScene.PRTransform.Vector3D); //Занять клетку
							prestScene = newScene;
							break;
						case 'N':
							currentNav = 'S';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (hasExit)
									{
										currentScene = DRLGContainers[3].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //Way
										{
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
									else
									{
										currentScene = DRLGContainers[1].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav)) //Exit
										{
											hasExit = true;
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
								}
								else
								{
									currentScene = DRLGContainers[2].PickRandom();
									#region проверка на будущее
									toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
									bool forceStop = false;
									foreach (var point in toWaitChunks)
										if (point != waitedScene.Key[0])
											switch (point)
											{
												//S - Build to the right +240:0
												case 'S':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																forceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												//N - Build to the left  -240:0
												case 'N':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																forceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												//E - Build up         0:+240
												case 'E':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																forceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												//W - Build down       0:-240
												case 'W':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)) ||
														 (reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;// else PosOfNav = 3;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																forceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
									if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
									else if (forceStop)
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
								}
							}
							placeToNewScene = waitedScene.Value;
							toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();

							foreach (var Point in toWaitChunks)
								if (Point != currentNav)
									switch (Point)
									{
										//S - Build to the right +240:0
										//N - Build to the left  -240:0
										//E - Build up         0:+240
										//W - Build down       0:-240

										case 'S': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'N': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'E': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
										case 'W': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
									}
							newScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
							newScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
							newScene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newScene); //Добавить сцену
							busyChunks.Add(newScene.PRTransform.Vector3D); //Занять клетку
							prestScene = newScene;
							break;
						case 'E':
							currentNav = 'W';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (hasExit)
									{
										currentScene = DRLGContainers[3].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //Way
										{
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
									else
									{
										currentScene = DRLGContainers[1].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav)) //Exit
										{
											hasExit = true;
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
								}
								else
								{
									currentScene = DRLGContainers[2].PickRandom();
									#region проверка на будущее
									toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in toWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//S - Build to the right +240:0
												//N - Build to the left  -240:0
												//E - Build up         0:+240
												//W - Build down       0:-240

												case 'S':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom();//CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom();//CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)) ||
														 (reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
									if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
								}
							}
							placeToNewScene = waitedScene.Value;
							toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();

							foreach (var Point in toWaitChunks)
								if (Point != currentNav)
									switch (Point)
									{
										//S - Build to the right +240:0
										//N - Build to the left  -240:0
										//E - Build up         0:+240
										//W - Build down       0:-240

										case 'S': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'N': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'E': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
										case 'W': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
									}
							newScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
							newScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
							newScene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newScene); //Add scene
							busyChunks.Add(newScene.PRTransform.Vector3D); //occupy the cell
							prestScene = newScene;
							break;
						case 'W':
							currentNav = 'E';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (hasExit)
									{
										currentScene = DRLGContainers[3].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //Way
										{
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
									else
									{
										currentScene = DRLGContainers[1].PickRandom();
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
										if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav)) //Exit
										{
											hasExit = true;
											if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
											break;
										}
									}
								}
								else
								{
									currentScene = DRLGContainers[2].PickRandom();
									#region проверка на будущее
									toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in toWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//S - Build to the right +240:0
												//N - Build to the left  -240:0
												//E - Build up         0:+240
												//W - Build down       0:-240

												case 'S':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)) ||
														(reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)) ||
														 (reservedChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))))
														while (true)
														{
															currentScene = DRLGContainers[3].PickRandom(); // CurrentScene Switch
															if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
															if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 4;
									if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(currentNav) & currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray().Length > 1) //Way
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (currentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || currentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) positionOfNav = 3;
										break;
									}
								}
							}
							placeToNewScene = waitedScene.Value;
							toWaitChunks = currentScene.Asset.Name.Split('_')[positionOfNav].ToCharArray();

							foreach (var Point in toWaitChunks)
								if (Point != currentNav)
									switch (Point)
									{
										//S - Build to the right +240:0
										//N - Build to the left  -240:0
										//E - Build up         0:+240
										//W - Build down       0:-240

										case 'S': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X + RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'N': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X - RangetoNext, placeToNewScene.Y, placeToNewScene.Z)); break;
										case 'E': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y + RangetoNext, placeToNewScene.Z)); break;
										case 'W': if (!busyChunks.Contains(new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z))) nextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(placeToNewScene.X, placeToNewScene.Y - RangetoNext, placeToNewScene.Z)); break;
									}
							newScene.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
							newScene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), placeToNewScene);
							newScene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newScene); //Add scene
							busyChunks.Add(newScene.PRTransform.Vector3D); //occupy the cell
							prestScene = newScene;
							break;
					}
				}
				waitChunks.Clear();


				foreach (var nextChunk in nextWaitChunks)
				{
					bool unique = true;
					//we don't put scene on scene!
					foreach (var busyChunk in busyChunks) //check already created
						if (nextChunk.Value == busyChunk)
							unique = false;
					foreach (var reservedChunk in reservedChunks)
						if (nextChunk.Value == reservedChunk) //check reserve
							unique = false;
					if (unique)
					{
						reservedChunks.Add(nextChunk.Value);
						waitChunks.Add(nextChunk.Key, nextChunk.Value);
					}
				}
				nextWaitChunks.Clear();
				reservedChunks.Clear();
			}

			//Force Check Exit
			if (!hasExit)
			{
				bool first = false;
				bool finish = false;
				var newSceneChunk = new SceneChunk();
				foreach (var chunk in world.worldData.SceneParams.SceneChunks)
				{
					if (!hasExit)
					{
						//skip first chunk
						if (!first) { first = true; continue; }
						//we start to find dead ends
						//if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
						if (chunk.SNOHandle.Name.Split('_')[positionOfNav].ToCharArray().Length == 1)
						{
							char Nav = chunk.SNOHandle.Name.Split('_')[positionOfNav].ToCharArray()[0];
							while (true)
							{
								currentScene = DRLGContainers[1].PickRandom();

								if (currentScene.Asset.Name.Split('_')[positionOfNav].Contains(Nav)) //Exit
								{
									hasExit = true;
									break;
								}
							}

							newSceneChunk.SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID);
							newSceneChunk.PRTransform = chunk.PRTransform;
							newSceneChunk.SceneSpecification = new SceneSpecification(
								   0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
								   -1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								   new int[4] { 0, 0, 0, 0 }, 0));
							finish = true;
						}
					}
					if (finish)
					{
						//premature completion and adding an exit
						world.worldData.SceneParams.SceneChunks.Add(newSceneChunk); //Add scene
						world.worldData.SceneParams.SceneChunks.Remove(chunk);
						break;
					}
				}
			}

			//Forming Range
			float chunkMinX = -480;
			float chunkMaxX = 0;
			float chunkMinY = -480;
			float chunkMaxY = 0;
			foreach (var chunk in world.worldData.SceneParams.SceneChunks)
			{
				if (chunk.PRTransform.Vector3D.X > chunkMaxX) chunkMaxX = chunk.PRTransform.Vector3D.X;
				if (chunk.PRTransform.Vector3D.Y > chunkMaxY) chunkMaxY = chunk.PRTransform.Vector3D.Y;
			}
			chunkMaxX += RangetoNext;
			chunkMaxY += RangetoNext;

			//Fillers
			List<SceneChunk> FillerChunks = new List<SceneChunk>();
			if (FillerChuncks.Count > 0)
			{
				float x = chunkMinX;
				float y = chunkMinY;
				while (x < chunkMaxX)
				{
					float returnToMinY = chunkMinY;
					while (y < chunkMaxY)
					{
						bool busy = false;
						foreach (var chunk in world.worldData.SceneParams.SceneChunks)
							if (Math.Abs(chunk.PRTransform.Vector3D.X - x) < Globals.FLOAT_TOLERANCE & Math.Abs(chunk.PRTransform.Vector3D.Y - y) < Globals.FLOAT_TOLERANCE)
							{
								busy = true; 
								break;
							}

						if (!busy)
						{
							currentScene = DRLGContainers[4].PickRandom();

							var newscene = new SceneChunk
							{
								SNOHandle = new SNOHandle(SNOGroup.Scene, currentScene.SnoID),
								PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), new Vector3D(x, y, 0)),
								SceneSpecification = new SceneSpecification(0, new Vector2D(0, 0), new int[4] { rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : currentScene.LevelArea, rift ? currentScene.LevelArea : -1, -1, -1 },
									-1, -1, -1, -1, -1, -1, currentScene.Music, -1, -1, -1, currentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1, new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new int[4] { 0, 0, 0, 0 }, 0))
							};
							FillerChunks.Add(newscene);
						}
						busy = false;
						y += RangetoNext;
					}
					y = returnToMinY;
					x += RangetoNext;
				}

			}
			world.worldData.SceneParams.ChunkCount = world.worldData.SceneParams.SceneChunks.Count;
			foreach (var fil in FillerChunks)
			{
				world.worldData.SceneParams.SceneChunks.Add(fil); //Add scene
			}

			world.DRLGEmuActive = true;
		}

		private bool GenerateRandomDungeon(WorldSno worldSNO, DiIiS_NA.Core.MPQ.FileFormats.World worldData)
		{
			//if ((worldData.DRLGParams == null)||(worldData.DRLGParams.Count == 0))
			//return false;
			if (worldData.SceneParams == null)
			{
				worldData.SceneParams = new SceneParams() 
				{
					ChunkCount = 0 ,
					SceneChunks = new List<SceneChunk>() 
					{ 
						
					}
				};
			}
			worldData.SceneParams.ChunkCount = 0;
			worldData.SceneParams.SceneChunks.Clear();
			Dictionary<int, TileInfo> tiles = new Dictionary<int, TileInfo>();

			Logger.Debug("Generating random world: {0}", worldSNO);
			//Each DRLGParam is a level
			if ((worldData.DRLGParams == null) || (worldData.DRLGParams.Count <= 0)) return true;
			foreach (var drlgParam in worldData.DRLGParams)
			{
				//Logger.Debug("DRLGParams: LevelArea: {0}", drlgparam.LevelArea);
				foreach (var tile in drlgParam.Tiles)
				{
					Logger.Trace("RandomGeneration: TileType: {0}", (TileTypes)tile.TileType);
					tiles.Add(tile.SNOScene, tile);
				}

				TileInfo entrance = new TileInfo();
				//HACK for Defiled Crypt as there is no tile yet with type 200. Maybe changing in DB would make more sense than putting this hack in
				//	[11]: {[161961, Mooege.Common.MPQ.MPQAsset]}Worlds\\a1trDun_Cave_Old_Ruins_Random01.wrl
				if (worldSNO == WorldSno.a1trdun_cave_old_ruins_random01)
				{
					entrance = tiles[131902];
					tiles.Remove(131902);
				}
				else
					entrance = GetTileInfo(tiles, TileTypes.Entrance);

				Dictionary<Vector3D, TileInfo> worldTiles = new Dictionary<Vector3D, TileInfo>();

				if (DRLGTemplate.Templates.ContainsKey(worldSNO))
				{
					DRLGTemplate.DRLGLayout world_layout = DRLGTemplate.Templates[worldSNO].PickRandom();
					int coordY = 0;

					foreach (List<int> row in world_layout.map)
					{
						int coordX = 0;
						foreach (int cell in row)
						{
							if (cell != -1)
							{
								Vector3D tilePosition = new Vector3D(drlgParam.ChunkSize * (coordY + 1),
									drlgParam.ChunkSize * (coordX + 1), 0);

								if (coordX == world_layout.enterPositionX && coordY == world_layout.enterPositionY)
								{
									worldTiles.Add(tilePosition,
										cell <= 115
											? GetTileInfo(tiles, TileTypes.Entrance, cell)
											: GetTile(tiles, cell));
								}
								else if (coordX == world_layout.exitPositionX &&
								         coordY == world_layout.exitPositionY)
								{
									worldTiles.Add(tilePosition,
										cell <= 115
											? GetTileInfo(tiles, TileTypes.Exit, cell)
											: GetTile(tiles, cell));
								}
								else
								{
									worldTiles.Add(tilePosition,
										cell <= 115
											? GetTileInfo(tiles, TileTypes.Normal, cell)
											: GetTile(tiles, cell));
								}
							}

							coordX++;
						}

						coordY++;
					}
				}
				else
				{
					Vector3D initialStartTilePosition = new Vector3D(480, 480, 0);
					worldTiles.Add(initialStartTilePosition, entrance);
					AddAdjacentTiles(worldTiles, entrance, drlgParam.ChunkSize, tiles, 0, initialStartTilePosition);
					AddFillers(worldTiles, tiles, drlgParam.ChunkSize);
				}

				foreach (var tile in worldTiles)
				{
					AddTile(worldData, tile.Value, tile.Key);
				}

				//AddFiller
				Logger.Debug("RandomGeneration: LevelArea: {0}", drlgParam.LevelArea);
				foreach (var chunk in worldData.SceneParams.SceneChunks)
				{
					if (drlgParam.LevelArea != -1)
					{
						chunk.SceneSpecification.SNOLevelAreas[0] = drlgParam.LevelArea;
						chunk.SceneSpecification.SNOWeather = drlgParam.Weather;
					}
					if (worldSNO == WorldSno.x1_bog_01) //A5 marsh
					{
						if (chunk.PRTransform.Vector3D.Y < 960 || chunk.PRTransform.Vector3D.X < 720)
							chunk.SceneSpecification.SNOLevelAreas[0] = 258142;
					}
				}
			}
			//Coordinates are added after selection of tiles and map
			//Leave it for Defiler Crypt debugging
			//AddTile(world, tiles[132218], new Vector3D(720, 480, 0));
			//AddTile(world, tiles[132203], new Vector3D(480, 240, 0));
			//AddTile(world, tiles[132263], new Vector3D(240, 480, 0));
			//return world;
			return true;
		}

		/// <summary>
		/// Adds filler tiles around the world
		/// </summary>
		/// <param name="worldTiles"></param>
		/// <param name="tiles"></param>
		private void AddFillers(Dictionary<Vector3D, TileInfo> worldTiles, Dictionary<int, TileInfo> tiles, int chunkSize)
		{
			Dictionary<Vector3D, TileInfo> fillersToAdd = new Dictionary<Vector3D, TileInfo>();
			foreach (var tile in worldTiles)
			{
				Dictionary<TileExits, Vector3D> adjacentPositions = GetAdjacentPositions(tile.Key, chunkSize);
				foreach (var position in adjacentPositions)
				{
					//Add filler to all free tiles (all exits should have been filled and the blocked ones don't need anything else)
					if (GetExitStatus(worldTiles, position.Value, position.Key) == ExitStatus.Free && !worldTiles.ContainsKey(position.Value))
					{
						//random filler
						if (!fillersToAdd.ContainsKey(position.Value))
							fillersToAdd.Add(position.Value, GetTileInfo(tiles, 0));
					}
				}
			}

			foreach (var tile in fillersToAdd)
			{
				worldTiles.Add(tile.Key, tile.Value);
			}
		}

		/// <summary>
		/// Adds tiles to all exits of a tile
		/// </summary>
		/// <param name="worldTiles">Contains a list of already added tiles.</param>
		/// <param name="tileInfo">Originating tile</param>
		/// <param name="tiles">List of tiles to choose from</param>
		/// <param name="counter">Contains how many tiles were added. When counter reached it will look for an exit.
		/// If exit was not found look for deadend(filler?). </param>
		/// <param name="position">Position of originating tile.</param>
		/// <param name="x">Originating tile world x position</param>
		private int AddAdjacentTiles(Dictionary<Vector3D, TileInfo> worldTiles, TileInfo tileInfo, int chunkSize, Dictionary<int, TileInfo> tiles, int counter, Vector3D position)
		{
			Logger.Trace("Counter: {0}, ExitDirectionbitsOfGivenTile: {1}", counter, tileInfo.ExitDirectionBits);
			var lookUpExits = GetLookUpExitBits(tileInfo.ExitDirectionBits);

			Dictionary<TileExits, Vector3D> randomizedExitTypes = GetAdjacentPositions(position, chunkSize, true).Where(exit => (lookUpExits & (int)exit.Key) > 0 && !worldTiles.ContainsKey(exit.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);

			//add adjacent tiles for each randomized direction
			//var lastExit = randomizedExitTypes.Last();
			foreach (var exit in randomizedExitTypes)
			{
				if (worldTiles.ContainsKey(exit.Value)) continue;
				worldTiles.Add(exit.Value, null);
				if (exit.Key == randomizedExitTypes.Last().Key) //continuing passage
					counter = AdjacentTileAtExit(worldTiles, tiles, chunkSize, counter, exit.Value, false);
				else
					counter = AdjacentTileAtExit(worldTiles, tiles, chunkSize, counter, exit.Value, true);
			}

			return counter;
		}

		private bool CheckAdjacentTiles(Dictionary<Vector3D, TileInfo> worldTiles, TileInfo tileInfo, int chunkSize, Dictionary<int, TileInfo> tiles, Vector3D position)
		{
			var lookUpExits = GetLookUpExitBits(tileInfo.ExitDirectionBits);

			Dictionary<TileExits, Vector3D> randomizedExitTypes = GetAdjacentPositions(position, chunkSize, true).Where(exit => (lookUpExits & (int)exit.Key) > 0 && !worldTiles.ContainsKey(exit.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);

			//add adjacent tiles for each randomized direction
			foreach (var exit in randomizedExitTypes)
			{
				if (GetTileInfo(tiles, (int)TileTypes.Normal, GetAdjacentExitStatus(worldTiles, exit.Value, chunkSize), true) == null) return false;
			}
			return true;
		}

		/// <summary>
		/// Adds an adjacent tile in the given exit position
		/// </summary>
		/// <param name="worldTiles"></param>
		/// <param name="tiles"></param>
		/// <param name="counter"></param>
		/// <returns></returns>
		private int AdjacentTileAtExit(Dictionary<Vector3D, TileInfo> worldTiles, Dictionary<int, TileInfo> tiles, int chunkSize, int counter, Vector3D position, bool lookingForCork)
		{
			TileTypes tileTypeToFind = TileTypes.Normal;
			//Find if other exits are in the area of the new tile to add
			bool incCounter = true;
			if (counter > 30)
			{
				worldTiles.Remove(position);
				return counter;
			}
			if (lookingForCork) incCounter = false;

			Dictionary<TileExits, ExitStatus> exitStatus = GetAdjacentExitStatus(worldTiles, position, chunkSize);
			if (counter > 5) //TODO: this value must be set according to difficulty
			{
				if (!ContainsTileType(worldTiles, TileTypes.Exit))
					tileTypeToFind = TileTypes.Exit;
				lookingForCork = true;
			}
			if (tiles.ContainsKey(199783) && counter > 0)
			{
				lookingForCork = true; //hack for aqueducs deep
			}

			TileInfo newTile = GetTileInfo(tiles, (int)tileTypeToFind, exitStatus, lookingForCork);

			if (tiles.ContainsKey(67021) && tiles.ContainsKey(91612) && !ContainsTileType(worldTiles, TileTypes.EventTile1) && incCounter)
			{
				newTile = GetTile(tiles, 67021); //hack for Kormac's spawn scene
			}
			if (tiles.ContainsKey(91612) && counter == 1 && incCounter)
			{
				newTile = GetTile(tiles, 91612); //hack for Kormac's stash scene
			}
			if (tiles.ContainsKey(72876) && incCounter)
			{
				if (!ContainsTileType(worldTiles, TileTypes.EventTile1))
					newTile = GetTile(tiles, 72876); //hack for Stonefort: cat 1
				if (counter == 1)
					newTile = GetTile(tiles, 111118); //hack for Stonefort: corner
				if (counter == 2)
					newTile = GetTile(tiles, 71341); //hack for Stonefort: cat 2
			}

			if (tiles.ContainsKey(69230) && counter == 1 && incCounter)
			{
				newTile = GetTile(tiles, 69230); //hack for ZK archives
			}
			if (newTile == null)
			{
				if (tiles.ContainsKey(109296) || tiles.ContainsKey(96987))
					newTile = GetTile(tiles, 74907);
				else
					newTile = GetTileInfo(tiles, (int)TileTypes.Exit, exitStatus, lookingForCork); //trying to find from exits
				if (newTile == null)
				{
					worldTiles.Remove(position);
					return counter;
				}
			}
			worldTiles[position] = newTile;
			int threshold = 0;
			if (!lookingForCork && !tiles.ContainsKey(72876)) //stonefort safe
				while (!CheckAdjacentTiles(worldTiles, newTile, chunkSize, tiles, position))
				{
					newTile = GetTileInfo(tiles, (int)tileTypeToFind, exitStatus, lookingForCork);
					worldTiles[position] = newTile;
					threshold++;
					if (threshold > 10) break;
				}
			Logger.Trace("Added tile: Type: {0}, SNOScene: {1}, ExitTypes: {2}", newTile.TileType, newTile.SNOScene, newTile.ExitDirectionBits);
			counter = AddAdjacentTiles(worldTiles, newTile, chunkSize, tiles, (incCounter ? counter + 1 : counter), position);
			return counter;
		}

		/// <summary>
		/// Returns the status of all exits for a specified position
		/// </summary>
		/// <param name="worldTiles">Tiles already added to world</param>
		/// <param name="position">Position</param>
		private Dictionary<TileExits, ExitStatus> GetAdjacentExitStatus(Dictionary<Vector3D, TileInfo> worldTiles, Vector3D position, int chunkSize)
		{
			Dictionary<TileExits, ExitStatus> exitStatusDict = new Dictionary<TileExits, ExitStatus>();
			//Compute East adjacent Location
			Vector3D positionEast = new Vector3D(position.X + chunkSize, position.Y, position.Z);
			ExitStatus exitStatusEast = GetExitStatus(worldTiles, positionEast, TileExits.West);
			exitStatusDict.Add(TileExits.East, exitStatusEast);

			Vector3D positionWest = new Vector3D(position.X - chunkSize, position.Y, position.Z);
			ExitStatus exitStatusWest = GetExitStatus(worldTiles, positionWest, TileExits.East);
			exitStatusDict.Add(TileExits.West, exitStatusWest);

			Vector3D positionNorth = new Vector3D(position.X, position.Y + chunkSize, position.Z);
			ExitStatus exitStatusNorth = GetExitStatus(worldTiles, positionNorth, TileExits.South);
			exitStatusDict.Add(TileExits.North, exitStatusNorth);

			Vector3D positionSouth = new Vector3D(position.X, position.Y - chunkSize, position.Z);
			ExitStatus exitStatusSouth = GetExitStatus(worldTiles, positionSouth, TileExits.North);
			exitStatusDict.Add(TileExits.South, exitStatusSouth);

			return exitStatusDict;
		}

		/// <summary>
		/// Returns a dictionary of all positions adjacent to a tile
		/// </summary>
		/// <param name="position"></param>
		/// <param name="isRandom"></param>
		private Dictionary<TileExits, Vector3D> GetAdjacentPositions(Vector3D position, int chunkSize, bool isRandom = false)
		{
			Vector3D positionEast = new Vector3D(position.X - chunkSize, position.Y, 0);
			Vector3D positionWest = new Vector3D(position.X + chunkSize, position.Y, 0);
			Vector3D positionNorth = new Vector3D(position.X, position.Y - chunkSize, 0);
			Vector3D positionSouth = new Vector3D(position.X, position.Y + chunkSize, 0);

			//get a random direction
			Dictionary<TileExits, Vector3D> exitTypes = new Dictionary<TileExits, Vector3D>
			{
				{ TileExits.East, positionEast },
				{ TileExits.West, positionWest },
				{ TileExits.North, positionNorth },
				{ TileExits.South, positionSouth }
			};

			if (!isRandom)
				return exitTypes;

			//randomize
			Dictionary<TileExits, Vector3D> randomExitTypes = new Dictionary<TileExits, Vector3D>();
			var count = exitTypes.Count;

			//Randomise exit directions
			for (int i = 0; i < count; i++)
			{
				//Chose a random exit to test
				Vector3D chosenExitPosition = exitTypes.PickRandom().Value;
				var chosenExitDirection = (from pair in exitTypes
										   where pair.Value == chosenExitPosition
										   select pair.Key).FirstOrDefault();
				randomExitTypes.Add(chosenExitDirection, chosenExitPosition);
				exitTypes.Remove(chosenExitDirection);
			}

			return randomExitTypes;
		}

		private bool ContainsTileType(Dictionary<Vector3D, TileInfo> worldTiles, TileTypes tileType)
		{
			foreach (var tileInfo in worldTiles)
			{
				if (tileInfo.Value == null) continue;
				if (tileInfo.Value.TileType == (int)tileType) return true;
			}
			return false;
		}

		/// <summary>
		/// Provides the exit status given position and exit (NSEW)
		/// </summary>
		/// <param name="worldTiles"></param>
		/// <param name="position"></param>
		/// <param name="exit"></param>
		/// <returns></returns>
		private ExitStatus GetExitStatus(Dictionary<Vector3D, TileInfo> worldTiles, Vector3D position, TileExits exit)
		{
			if (!worldTiles.ContainsKey(position) || worldTiles[position] == null) return ExitStatus.Free;
			else
			{
				//if (worldTiles[position] == null) return ExitStatus.Blocked;
				if ((worldTiles[position].ExitDirectionBits & (int)exit) > 0) return ExitStatus.Open;
				else return ExitStatus.Blocked;
			}
		}

		/// <summary>
		/// Provides what entrances to look-up based on an entrance set of bits
		/// N means look for S
		/// S means look for N
		/// W means look for E
		/// E means look for W
		/// basically switch first two bits and last two bits
		/// </summary>
		/// <param name="exitDirectionBits"></param>
		/// <returns></returns>
		private int GetLookUpExitBits(int exitDirectionBits)
		{
			return (((exitDirectionBits & ~3) & (int)0x4U) << 1 | ((exitDirectionBits & ~3) & (int)0x8U) >> 1)
				+ (((exitDirectionBits & ~12) & (int)0x1U) << 1 | ((exitDirectionBits & ~12) & (int)0x2U) >> 1);
		}

		/// <summary>
		/// Get tileInfo with specific requirements
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="exitDirectionBits"></param>
		/// <param name="tileType"></param>
		/// <param name="exitStatus"></param>
		/// <returns></returns>
		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, int tileType, Dictionary<TileExits, ExitStatus> exitStatus, bool isCork)
		{
			//get all exits that need to be in the new tile
			int mustHaveExits = 0;
			Dictionary<int, TileInfo> acceptedTiles = tiles;
			foreach (TileExits exit in Enum.GetValues(typeof(TileExits)))
			{
				if (exitStatus[exit] == ExitStatus.Open) mustHaveExits += (int)exit;

				//delete from the pool of tiles those that do have exits that are blocked
				if (exitStatus[exit] == ExitStatus.Blocked || (isCork && exitStatus[exit] == ExitStatus.Free))
				{
					acceptedTiles = acceptedTiles.Where(pair => (pair.Value.ExitDirectionBits & (int)exit) == 0).ToDictionary(pair => pair.Key, pair => pair.Value);
				}
			}
			Logger.Trace("Looking for tile with Exits: {0}", mustHaveExits);
			if (isCork)
				return GetTileInfo(acceptedTiles
					.Where(pair => pair.Value.TileType == tileType)
					.ToDictionary(pair => pair.Key, pair => pair.Value), mustHaveExits);
			return GetTileInfo(acceptedTiles
				.Where(pair => pair.Value.TileType == tileType && !Enum.IsDefined(typeof(TileExits), pair.Value.ExitDirectionBits))
				.ToDictionary(pair => pair.Key, pair => pair.Value), mustHaveExits);
		}

		/// <summary>
		/// Returns a tileinfo from a list of tiles that has specific exit directions
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="exitDirectionBits"></param>
		/// <returns></returns>
		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, int exitDirectionBits)
		{
			//if no exit direction bits return filler
			if (exitDirectionBits == 0)
			{
				//return filler
				return GetTileInfo(tiles, TileTypes.Filler);
			}
			List<TileInfo> tilesWithRightDirection = (from pair in tiles where ((pair.Value.ExitDirectionBits & exitDirectionBits) > 0) select pair.Value).ToList();

			if (tilesWithRightDirection.Count == 0)
			{
				Logger.Trace("Did not find matching tile");
				//TODO: Never return null. Try to find other tiles that match entry pattern and rotate
				//There should be a field that defines if tile can be rotated
				return null;
			}

			return tilesWithRightDirection.PickRandom();
		}

		private TileInfo GetTile(Dictionary<int, TileInfo> tiles, int snoId)
		{
			if (!tiles.ContainsKey(snoId)) return null;
			return tiles.First(x => x.Key == snoId).Value;
		}

		/// <summary>
		/// Returns a tileinfo from a list of tiles that has a specific type
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="tileType"></param>
		/// <returns></returns>
		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, TileTypes tileType)
		{
			var tilesWithRightType = tiles.Values.Where(tile => tile.TileType == (int)tileType);
			return tilesWithRightType.PickRandom();
		}

		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, TileTypes tileType, int exitDirectionBits)
		{
			if (exitDirectionBits == 0)
			{
				//return filler
				return GetTileInfo(tiles, TileTypes.Filler);
			}
			List<TileInfo> tilesWithRightTypeAndDirection;

			if (tileType == TileTypes.Normal)
				tilesWithRightTypeAndDirection = (from pair in tiles where (pair.Value.TileType is 100 or 101 or 102 && pair.Value.ExitDirectionBits == exitDirectionBits) select pair.Value).ToList<TileInfo>();
			else
				tilesWithRightTypeAndDirection = (from pair in tiles where (pair.Value.TileType == (int)tileType && pair.Value.ExitDirectionBits == exitDirectionBits) select pair.Value).ToList<TileInfo>();
			if (tilesWithRightTypeAndDirection.Any())
				return RandomHelper.RandomItem(tilesWithRightTypeAndDirection, entry => entry.Probability);
			
			Logger.Error("Did not find matching tile for template! Type: {0}, Direction: {1}", tileType, exitDirectionBits);
			return null;

		}

		private void AddTile(DiIiS_NA.Core.MPQ.FileFormats.World worldData, TileInfo tileInfo, Vector3D location)
		{
			var sceneChunk = new SceneChunk
			{
				SNOHandle = new SNOHandle(tileInfo.SNOScene),
				PRTransform = new PRTransform
				{
					Quaternion = new Quaternion
					{
						W = 1.0f,
						Vector3D = new Vector3D(0, 0, 0)
					},
					Vector3D = new Vector3D()
				}
			};
			sceneChunk.PRTransform.Vector3D = location;

			var spec = new SceneSpecification
			{
				//scene.Specification = spec;
				Cell = new Vector2D() { X = 0, Y = 0 },
				CellZ = 0,
				SNOLevelAreas = new[] { -1, -1, -1, -1 },
				SNOMusic = -1,
				SNONextLevelArea = -1,
				SNONextWorld = -1,
				SNOPresetWorld = -1,
				SNOPrevLevelArea = -1,
				SNOPrevWorld = -1,
				SNOReverb = -1,
				SNOWeather = 50542,
				SNOCombatMusic = -1,
				SNOAmbient = -1,
				ClusterID = -1,
				PrevEntranceGUID = 14,
				DRLGIndex = 5,
				SceneChunk = -1,
				OnPathBits = tileInfo.TileType, //we can make it TileType value
				SceneCachedValues = new SceneCachedValues
				{
					CachedValuesValid = 63,
					NavMeshSizeX = 96,
					NavMeshSizeY = 96
				}
			};
			//Logger.Trace("Adding Tile: SNOscene {0}", tileInfo.SNOScene);
			var sceneFile = MPQStorage.Data.Assets[SNOGroup.Scene][tileInfo.SNOScene];
			var sceneData = (DiIiS_NA.Core.MPQ.FileFormats.Scene)sceneFile.Data;
			spec.SceneCachedValues.AABB1 = sceneData.AABBBounds;//new AABB(){Min = new Vector3D(0, 0, 0), Max = new Vector3D(240, 240, 240)};//
			spec.SceneCachedValues.AABB2 = sceneData.AABBMarketSetBounds;//new AABB(){Min = new Vector3D(0, 0, 0), Max = new Vector3D(240, 240, 240)};//
			spec.SceneCachedValues.Unknown4 = new int[4] { 0, 0, 0, 0 };

			sceneChunk.SceneSpecification = spec;

			worldData.SceneParams.SceneChunks.Add(sceneChunk);
			worldData.SceneParams.ChunkCount++;
			//System.Threading.Thread.Sleep(3);
		}

		private static readonly Dictionary<int, float> GizmosToSpawn = new()
		{
			{51300, 0.3f},   //chest common
			{138989, 0.3f}, //healthwell_global
			{176074, 0.06f}, //blessed
			{176075, 0.06f}, //enlightened
			{176076, 0.06f}, //fortune
			{176077, 0.06f},  //frenzied
			{79319, 0.06f},   //bloody chest
			{62860, 0.05f},  //Chest_rare
			{373463, 0.05f}, //PoolOfReflection
			//135384, //shrine_global
		};

		private static readonly List<int> Goblins = new()
		{
			{5984},
			{5985},
			{5987},
			{5988}
		};

		private readonly Dictionary<int, List<Scene>> _lazyLevelAreas = new();

		/// <summary>
		/// Loads content for level areas. Call this after scenes have been generated and after scenes have their GizmoLocations
		/// set (this is done in Scene.LoadActors right now)
		/// 
		/// Each Scene has one to four level areas assigned to it. I dont know if that means
		/// the scene belongs to both level areas or if the scene is split
		/// Scenes marker tags have generic GizmoLocationA to Z that are used 
		/// to provide random spawning possibilities.
		/// For each of these 26 LocationGroups, the LevelArea has a entry in its SpawnType array that defines
		/// what type of actor/encounter/adventure could spawn there
		/// 
		/// It could for example define, that for a level area X, out of the four spawning options
		/// two are randomly picked and have barrels placed there
		/// </summary>
		/// <param name="levelAreas">Dictionary that for every level area has the scenes it consists of</param>
		/// <param name="world">The world to which to add loaded actors</param>
		private void LoadLevelAreas(Dictionary<int, List<Scene>> levelAreas, World world)
		{
			Dictionary<PRTransform, DiIiS_NA.Core.MPQ.FileFormats.ActorData> dict = new Dictionary<PRTransform, DiIiS_NA.Core.MPQ.FileFormats.ActorData>();
			foreach (int la in levelAreas.Keys)
			{
				SNOHandle levelAreaHandle = new SNOHandle(SNOGroup.LevelArea, la);
				if (!levelAreaHandle.IsValid)
				{
					Logger.Warn("Level area {0} does not exist", la);
					continue;
				}
				var levelArea = levelAreaHandle.Target as LevelArea;

				List<PRTransform> gizmoLocations = new List<PRTransform>();

				for (int i = 0; i < 26; i++)
				{
					// Merge the gizmo starting locations from all scenes and
					// their subscenes into a single list for the whole level area
					foreach (var scene in levelAreas[la])
					{
						if (scene.GizmoSpawningLocations[i] != null)
							gizmoLocations.AddRange(scene.GizmoSpawningLocations[i]);
						foreach (Scene subScene in scene.Subscenes)
						{
							if (subScene.GizmoSpawningLocations[i] != null)
								gizmoLocations.AddRange(subScene.GizmoSpawningLocations[i]);
						}
					}
				}

				List<PRTransform> finalLocationsList = new List<PRTransform>();
				foreach (PRTransform currValue in gizmoLocations)
				{
					if (!finalLocationsList.Contains(currValue) && world.CheckLocationForFlag(currValue.Vector3D, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						finalLocationsList.Add(currValue);
					}
				}

				gizmoLocations = finalLocationsList; //sorting with no duplications

				if (world.SNO == WorldSno.a2dun_cave_mapdungeon_level01) //Mysterious Cave lv 1
				{
					var handle = new SNOHandle(207706);
					if (handle == null || gizmoLocations.Count == 0) continue;
					LazyLoadActor(handle, gizmoLocations.PickRandom(), world, ((DiIiS_NA.Core.MPQ.FileFormats.ActorData)handle.Target).TagMap);
				}
				else
					foreach (var location in gizmoLocations)
					{
						if (FastRandom.Instance.Next(100) < 1)
						{
							SNOHandle gizmoHandle = null;
							float seed = (float)FastRandom.Instance.NextDouble();
							foreach (var pair in GizmosToSpawn)
							{
								if (seed < pair.Value)
								{
									gizmoHandle = new SNOHandle(pair.Key);
									break;
								}
								else
									seed -= pair.Value;
							}
							if (gizmoHandle == null) continue;
							LazyLoadActor(gizmoHandle, location, world, ((DiIiS_NA.Core.MPQ.FileFormats.ActorData)gizmoHandle.Target).TagMap);
						}
					}

				if (gizmoLocations.Count > 0 && world.Game.MonsterLevel >= Program.MaxLevel && FastRandom.Instance.Next(100) < 30)
				{
					var handleChest = new SNOHandle(96993); //leg chest
					if (handleChest == null) continue;
					var goldenChest = LoadActor(handleChest, gizmoLocations.PickRandom(), world, ((DiIiS_NA.Core.MPQ.FileFormats.ActorData)handleChest.Target).TagMap);
					if (goldenChest > 0)
						(world.GetActorByGlobalId(goldenChest) as LegendaryChest).ChestActive = true;
				}

				if (world.DRLGEmuActive)
				{
					int wid = (int)world.SNO;
					// Load monsters for level area
					foreach (var scene in levelAreas.First().Value)
					{
						if (!SpawnGenerator.Spawns.ContainsKey(wid)) break;
						if (SpawnGenerator.Spawns[wid].LazyLoad)
						{
							_lazyLevelAreas.Add(wid, levelAreas.First().Value);
							break;
						}
						else
							LoadMonstersLayout(world, wid, scene);
					}
					#region unique spawn
					//unique spawn
					if (SpawnGenerator.Spawns.ContainsKey(wid) && SpawnGenerator.Spawns[wid].Dangerous.Count > 0 && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomUnique = new SNOHandle(SpawnGenerator.Spawns[wid].Dangerous.PickRandom());
						var scene = levelAreas.First().Value.PickRandom();
						int x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
						int y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						int threshold = 0;
						while ((scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Flags & DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoSpawn) != 0)
						{
							threshold++;
							if (threshold >= 20)
							{
								break;
								//continue;
							}
							x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
							y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						}

						var uniq = LoadActor(
							randomUnique,
							new PRTransform
							{
								Vector3D = new Vector3D
								{
									X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
								},
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
							},
							world,
							new TagMap()
						);

						if (uniq > 0)
							if (world.GetActorByGlobalId(uniq) as Unique != null)
								(world.GetActorByGlobalId(uniq) as Unique).CanDropKey = true;
					}
					#endregion
					#region goblin spawn
					//goblin spawn
					if (SpawnGenerator.Spawns.ContainsKey(wid) && SpawnGenerator.Spawns[wid].CanSpawnGoblin && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomGoblin = new SNOHandle(Goblins.PickRandom());
						if (world.Game.IsHardcore) randomGoblin = new SNOHandle(3852);
						var scene = levelAreas.First().Value.PickRandom();
						int x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
						int y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						int threshold = 0;
						while ((scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Flags & DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoSpawn) != 0)
						{
							threshold++;
							if (threshold >= 20)
							{
								break;
								//continue;
							}
							x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
							y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						}

						LazyLoadActor(
							randomGoblin,
							new PRTransform
							{
								Vector3D = new Vector3D
								{
									X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
								},
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
							},
							world,
							new TagMap()
						);
					}
					#endregion
				}
				else
				{
					//
					// Load monsters for level area
					foreach (var scene in levelAreas[la])
					{
						if (!SpawnGenerator.Spawns.ContainsKey(la)) break;
						if (SpawnGenerator.Spawns[la].LazyLoad)
						{
							_lazyLevelAreas.Add(la, levelAreas[la]);
							break;
						}
						else
							LoadMonstersLayout(world, la, scene);
					}
					#region unique spawn
					//unique spawn
					if (SpawnGenerator.Spawns.ContainsKey(la) && SpawnGenerator.Spawns[la].Dangerous.Count > 0 && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomUnique = new SNOHandle(SpawnGenerator.Spawns[la].Dangerous.PickRandom());
						var scene = levelAreas[la].PickRandom();
						int x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
						int y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						int threshold = 0;
						while ((scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Flags & DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoSpawn) != 0)
						{
							threshold++;
							if (threshold >= 20)
							{
								break;
								//continue;
							}
							x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
							y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						}

						var uniq = LoadActor(
							randomUnique,
							new PRTransform
							{
								Vector3D = new Vector3D
								{
									X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
								},
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
							},
							world,
							new TagMap()
						);

						if (uniq > 0)
							if (world.GetActorByGlobalId(uniq) as Unique != null)
								(world.GetActorByGlobalId(uniq) as Unique).CanDropKey = true;
					}
					#endregion
					#region goblin spawn
					//goblin spawn
					if (SpawnGenerator.Spawns.ContainsKey(la) && SpawnGenerator.Spawns[la].CanSpawnGoblin && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomGoblin = new SNOHandle(Goblins.PickRandom());
						if (world.Game.IsHardcore) randomGoblin = new SNOHandle(3852);
						var scene = levelAreas[la].PickRandom();
						int x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
						int y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						int threshold = 0;
						while ((scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Flags & DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoSpawn) != 0)
						{
							threshold++;
							if (threshold >= 20)
							{
								break;
								//continue;
							}
							x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
							y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
						}

						LazyLoadActor(
							randomGoblin,
							new PRTransform
							{
								Vector3D = new Vector3D
								{
									X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
									Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
								},
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
							},
							world,
							new TagMap()
						);
					}
					#endregion
				}
			}
		}

		public void LoadMonstersLayout(World world, int la, Scene scene)
		{
			if (scene.Populated) return;
			scene.Populated = true;
			if (!SpawnGenerator.Spawns.ContainsKey(la)) return;
			if (scene.SceneData.NoSpawn) return;

			List<Affix> packAffixes = new List<Affix>();
			int packs_count = world.worldData.DynamicWorld ? 5 : 4;
			packs_count += (Game.Difficulty / 3);

			if (world.worldData.DRLGParams != null && world.worldData.DRLGParams.Count > 0)
			{
				if (world.worldData.DRLGParams.First().ChunkSize == 120)
					packs_count -= 2;
				if (world.worldData.DRLGParams.First().ChunkSize > 240)
					packs_count += 2;
			}

			if (Game.Difficulty > 4)
				packs_count += SpawnGenerator.Spawns[la].AdditionalDensity;

			var groupId = 0;


			for (int i = 0; i < packs_count; i++)
			{
				int x = FastRandom.Instance.Next(scene.NavMesh.SquaresCountX);
				int y = FastRandom.Instance.Next(scene.NavMesh.SquaresCountY);
				groupId = FastRandom.Instance.Next();

				if ((scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Flags & DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.NoSpawn) == 0)
				{
					bool isElite = (FastRandom.Instance.NextDouble() < 0.03);
					if (isElite)
					{
						#region elite spawn
						int randomMeleeMonsterId = -1;
						int randomRangedMonsterId = -1;
						if (SpawnGenerator.Spawns[la].Melee.Count > 0) randomMeleeMonsterId = SpawnGenerator.Spawns[la].Melee.PickRandom();
						if (SpawnGenerator.Spawns[la].Range.Count > 0) randomRangedMonsterId = SpawnGenerator.Spawns[la].Range.PickRandom();
						SNOHandle meleeMonsterHandle = (randomMeleeMonsterId == -1 ? null : new SNOHandle(randomMeleeMonsterId));
						SNOHandle rangedMonsterHandle = (randomRangedMonsterId == -1 ? null : new SNOHandle(randomRangedMonsterId));
						if (rangedMonsterHandle == null) rangedMonsterHandle = meleeMonsterHandle;
						else
							if (meleeMonsterHandle == null) meleeMonsterHandle = rangedMonsterHandle;
						for (int n = 0; n < 5; n++)
						{
							if (n == 0 || FastRandom.Instance.NextDouble() < 0.85)
							{
								uint actor = LoadActor(
								(n == 0 ? (FastRandom.Instance.NextDouble() < 0.5 ? meleeMonsterHandle : rangedMonsterHandle) : meleeMonsterHandle),
									new PRTransform
									{
										Vector3D = new Vector3D
										{
											X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
											Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
											Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
										},
										Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
									},
									world,
									new TagMap(),
									(n == 0 ? MonsterType.Elite : MonsterType.EliteMinion),
									groupId
								);
								if (actor > 0)
									if (n == 0)
										packAffixes = MonsterAffixGenerator.Generate(world.GetActorByGlobalId(actor), Math.Min(world.Game.Difficulty + 1, 5));
									else
										MonsterAffixGenerator.CopyAffixes(world.GetActorByGlobalId(actor), packAffixes);
							}
						}
					}
					#endregion
					else
					{
						bool isChampion = (FastRandom.Instance.NextDouble() < 0.07);
						if (!isChampion)
						#region default spawn
						{
							int randomMeleeMonsterId = -1;
							int randomRangedMonsterId = -1;
							if (SpawnGenerator.Spawns[la].Melee.Count > 0) randomMeleeMonsterId = SpawnGenerator.Spawns[la].Melee.PickRandom();
							if (SpawnGenerator.Spawns[la].Range.Count > 0) randomRangedMonsterId = SpawnGenerator.Spawns[la].Range.PickRandom();
							SNOHandle meleeMonsterHandle = (randomMeleeMonsterId == -1 ? null : new SNOHandle(randomMeleeMonsterId));
							SNOHandle rangedMonsterHandle = (randomRangedMonsterId == -1 ? null : new SNOHandle(randomRangedMonsterId));
							//int maxMobsInStack = (SpawnGenerator.IsMelee(la, randomMonsterId) ? 6 : (SpawnGenerator.IsDangerous(la, randomMonsterId) ? 1 : 3));
							for (int n = 0; n < 6; n++)
							{
								if (n == 0 || FastRandom.Instance.NextDouble() < 0.6)
								{
									LazyLoadActor(
										(meleeMonsterHandle == null ? rangedMonsterHandle : (rangedMonsterHandle == null ? meleeMonsterHandle : (FastRandom.Instance.NextDouble() < 0.65 ? meleeMonsterHandle : rangedMonsterHandle))),
										new PRTransform
										{
											Vector3D = new Vector3D
											{
												X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
												Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
												Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
											},
											Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
										},
										world,
										new TagMap(),
										MonsterType.Default
									);
								}
							}
						}
						#endregion
						else //spawn champions
						#region champion spawn
						{
							SNOHandle championHandle = new SNOHandle(SpawnGenerator.Spawns[la].Melee.PickRandom());
							groupId = FastRandom.Instance.Next();
							for (int n = 0; n < 4; n++)
							{
								if (n == 0 || FastRandom.Instance.NextDouble() < 0.85)
								{
									uint actor = LoadActor(
										championHandle,
										new PRTransform
										{
											Vector3D = new Vector3D
											{
												X = (float)(x * 2.4 + scene.Position.X) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
												Y = (float)(y * 2.4 + scene.Position.Y) + (float)(FastRandom.Instance.NextDouble() * 20 - 10),
												Z = scene.NavMesh.Squares[y * scene.NavMesh.SquaresCountX + x].Z + scene.Position.Z
											},
											Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * Math.PI * 2))
										},
										world,
										new TagMap(),
										MonsterType.Champion,
										groupId
									);
									if (actor > 0)
										if (n == 0)
											packAffixes = MonsterAffixGenerator.Generate(world.GetActorByGlobalId(actor), Math.Min(world.Game.Difficulty + 1, 5));
										else
											MonsterAffixGenerator.CopyAffixes(world.GetActorByGlobalId(actor), packAffixes);
								}
							}
						}
						#endregion
					}
				}
				else i--;
			}
		}

		//TODO: Move this out as loading actors can happen even after world was generated
		public uint LoadActor(SNOHandle actorHandle, PRTransform location, World world, TagMap tagMap, MonsterType monsterType = MonsterType.Default, int groupId = 0)
		{
			try
			{
				var actorSno = (ActorSno)actorHandle.Id; // TODO: maybe we can replace SNOHandle
				if (world.QuadTree
					    .Query<Waypoint>(new Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 60f))
					    .Count > 0 ||
				    world.QuadTree
					    .Query<Portal>(new Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 5f)).Count >
				    0)
				{
					Logger.Debug("Load actor {0} ignored - waypoint nearby.", actorSno);
					return 0;
				}

				var actor = ActorFactory.Create(world, actorSno, tagMap);

				switch (monsterType)
				{
					case MonsterType.Champion:
						actor = new Champion(world, actorSno, tagMap);
						actor.GroupId = groupId;
						break;
					case MonsterType.Elite:
						actor = new Rare(world, actorSno, tagMap);
						actor.GroupId = groupId;
						break;
					case MonsterType.EliteMinion:
						actor = new RareMinion(world, actorSno, tagMap);
						actor.GroupId = groupId;
						break;
				}

				if (actor == null)
				{
					if (actorSno != ActorSno.__NONE)
						Logger.Warn("ActorFactory did not load actor {0}", actorHandle);
					return 0;
				}

				actor.RotationW = location.Quaternion.W;
				actor.RotationAxis = location.Quaternion.Vector3D;
				actor.EnterWorld(location.Vector3D);
				return actor.GlobalID;
			}
			catch (Exception ex)
			{
				Logger.Error("Error loading actor {0} at {1}", actorHandle.Id, location);
				return 0;
			}
		}

		public void LazyLoadActor(SNOHandle actorHandle, PRTransform location, World world, TagMap tagMap, MonsterType monsterType = MonsterType.Default)
		{
			var actorSno = (ActorSno)actorHandle.Id; // TODO: maybe we can replace SNOHandle
			if (world.QuadTree.Query<Waypoint>(new DiIiS_NA.GameServer.Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 60f)).Count > 0 ||
				world.QuadTree.Query<Portal>(new DiIiS_NA.GameServer.Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 40f)).Count > 0)
			{
				Logger.Debug("Load actor {0} ignored - waypoint nearby.", actorSno);
				return;
			}

			ActorFactory.LazyCreate(world, actorSno, tagMap, location.Vector3D, ((actor, spawn_pos) =>
			{
                switch (monsterType)
                {
                    case MonsterType.Champion:
                        actor = new Champion(world, actorSno, tagMap);
                        break;
                    case MonsterType.Elite:
                        actor = new Rare(world, actorSno, tagMap);
                        break;
                    case MonsterType.EliteMinion:
                        actor = new RareMinion(world, actorSno, tagMap);
                        break;
                }

                if (actor == null)
				{
					if (actorSno != ActorSno.__NONE)
						Logger.Warn("ActorFactory did not load actor {0}", actorSno);
				}
				else
				{
					actor.RotationW = location.Quaternion.W;
					actor.RotationAxis = location.Quaternion.Vector3D;
					actor.EnterWorld(spawn_pos);
				}
			})); // try to create it.;
		}

		public enum MonsterType
		{
			Default,
			Champion,
			Elite,
			EliteMinion
		}

		/// <summary>
		/// Loads all markersets of a scene and looks for the one with the subscene position
		/// </summary>
		private Vector3D FindSubScenePosition(SceneChunk sceneChunk)
		{
			var mpqScene = MPQStorage.Data.Assets[SNOGroup.Scene][sceneChunk.SNOHandle.Id].Data as DiIiS_NA.Core.MPQ.FileFormats.Scene;

			foreach (var markerSet in mpqScene.MarkerSets)
			{
				var mpqMarkerSet = MPQStorage.Data.Assets[SNOGroup.MarkerSet][markerSet].Data as MarkerSet;
				foreach (var marker in mpqMarkerSet.Markers)
					if (marker.Type == MarkerType.SubScenePosition)
						return marker.PRTransform.Vector3D;
			}
			return null;
		}
	}
}
