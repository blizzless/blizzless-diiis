//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Collision;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
//Blizzless Project 2022 
using Scene = DiIiS_NA.GameServer.GSSystem.MapSystem.Scene;
//Blizzless Project 2022 
using Affix = DiIiS_NA.GameServer.GSSystem.ItemsSystem.Affix;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.WorldSceneBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Scene;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
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
			this.SnoID = _SnoID;
			this.Weather = _Weather;
			this.Music = _Music;
			this.LevelArea = _LevelArea;
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
			this.Game = game;
		}

		public static Dictionary<int, int> DefaultConversationLists = new Dictionary<int, int>();

		private List<int> LoadedLevelAreas = new List<int>();

		public void CheckLevelArea(World world, int levelAreaSNO)
		{
			if (SpawnGenerator.Spawns.ContainsKey(levelAreaSNO) && SpawnGenerator.Spawns[levelAreaSNO].lazy_load == true)
				if (!this.LoadedLevelAreas.Contains(levelAreaSNO))
				{
					this.LoadedLevelAreas.Add(levelAreaSNO);

					// Load monsters for level area
					foreach (var scene in this.LazyLevelAreas[levelAreaSNO])
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
			World world = new World(this.Game, worldSNO);
			bool DRLGEmuActive = false;
			world.worldData = (DiIiS_NA.Core.MPQ.FileFormats.World)worldAsset.Data;
			if (worldSNO == WorldSno.a2dun_swr_swr_to_oasis_level01)
				world.worldData.DynamicWorld = true;

			//445736 - p4_forest_snow_icecave_01
			if (world.worldData.DynamicWorld && !worldSNO.IsNotDynamicWorld()) //Gardens of Hope - 2 lvl is NOT random
			{
				if (!GameServer.Config.Instance.DRLGemu)
					Logger.Warn("DRLG-Emu деактивирован.");
				string DRLGVersion = "1.8";
				var WorldContainer = DBSessions.WorldSession.Query<DRLG_Container>().Where(dbt => dbt.WorldSNO == (int)worldSNO).ToList();
				if (WorldContainer.Count > 0 && worldSNO != WorldSno.a1trdun_level05_templar && GameServer.Config.Instance.DRLGemu)
				{
					DRLGEmuActive = true;
					Logger.Warn("Мир - {0} [{1}] динамический! Найден контейнер, DRLG-Emu v{2} Активирован!", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
				}
				else if (!GenerateRandomDungeon(worldSNO, world.worldData))
				{
					Logger.Error("DRLG-Emu v{2} - World - {0} [{1}] динамический! DRLG Engine не нашёл контейнер! Шаблонная система не настроена для этого мира.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
					return null;
				}
				else
				{
					Logger.Warn("DRLG-Emu v{2} - Мир - {0} [{1}] динамический! DRLG Engine не нашёл контейнер! Запуск шаблонной системы генерации.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
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
						Logger.Warn("DRLG-Emu v{2} - Мир - {0} [{1}] динамический! DRLG Engine не нашёл контейнер! Сбой шаблона генерации, не установлена обратная связь.", worldAsset.Name, worldAsset.SNOId, DRLGVersion);
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

			if (DRLGEmuActive == true)
			{
				List<List<DRLGEmuScene>> DRLGContainers = new List<List<DRLGEmuScene>> { };
				List<DRLGEmuScene> EnterChuncks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> ExitChuncks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> WayChuncks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> EndChuncks = new List<DRLGEmuScene> { };
				List<DRLGEmuScene> FillerChuncks = new List<DRLGEmuScene> { };

				var WorldContainer = DBSessions.WorldSession.Query<DRLG_Container>().Where(dbt => dbt.WorldSNO == (int)world.SNO).First();
				var DRLGTiles = DBSessions.WorldSession.Query<DRLG_Tile>().Where(dbt => dbt.Head_Container == (int)WorldContainer.Id).ToList();

			REP:
				DRLGTiles = DBSessions.WorldSession.Query<DRLG_Tile>().Where(dbt => dbt.Head_Container == (int)WorldContainer.Id).ToList();
				//All Scenes
				foreach (var Tile in DRLGTiles)
				{
					switch (Tile.Type)
					{
						case 0: //Входы
							EnterChuncks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 1: //Выходы
							ExitChuncks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 2: //Пути
							WayChuncks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 3: //Тупики
							EndChuncks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
						case 4: //Филлеры
							FillerChuncks.Add(new DRLGEmuScene(Tile.SNOHandle_Id, Tile.SNOWeather, Tile.SNOMusic, Tile.SNOLevelArea));
							break;
					}
				}
				DRLGContainers.Add(EnterChuncks);
				DRLGContainers.Add(ExitChuncks);
				DRLGContainers.Add(WayChuncks);
				DRLGContainers.Add(EndChuncks);
				DRLGContainers.Add(FillerChuncks);

				if (world.SNO.IsGenerated())
					while (true)
					{

						DRLGGenerateProcess(world, DRLGContainers, FillerChuncks, WorldContainer.RangeofScenes);
						if (world.worldData.SceneParams.ChunkCount > 15)
							break;

					}
				else
				{
					try
					{
						DRLGGenerateProcess(world, DRLGContainers, FillerChuncks, WorldContainer.RangeofScenes);
					}
					catch
					{
						Logger.Info("DRLG генератор нашёл ошибку в расчёте, повтор.");
						goto REP;
					}
				}
				Logger.Info("DRLG work - Completed");
			}

			var clusters = new Dictionary<int, DiIiS_NA.Core.MPQ.FileFormats.SceneCluster>();
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
			var clusterSelected = new Dictionary<int, List<DiIiS_NA.Core.MPQ.FileFormats.SubSceneEntry>>();
			foreach (var cID in clusterCount.Keys)
			{
				var selected = new List<DiIiS_NA.Core.MPQ.FileFormats.SubSceneEntry>();
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
							DiIiS_NA.Core.MPQ.FileFormats.SubSceneEntry subSceneEntry = null;

							if (entries.Count > 0)
							{
								subSceneEntry = RandomHelper.RandomItem<DiIiS_NA.Core.MPQ.FileFormats.SubSceneEntry>(entries, entry => 1);
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
					loadLevelAreas(levelAreas, world);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "loadLevelAreas exception: ");
			}

			#region Патчи 
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
				case WorldSno.trdun_leoric_level03: //Установка портала на третий этаж Залов Агонии рядом с входом к Мяснику.
					Vector3D Scene0Pos = world.GetSceneBySnoId(78824).Position;
					world.SpawnMonster(ActorSno._waypoint, new Vector3D(Scene0Pos.X + 149.0907f, Scene0Pos.Y + 106.7075f, Scene0Pos.Z));
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
				case WorldSno.trout_town: //Упоротый наёмник =)
					var Templar = world.GetActorBySNO(ActorSno._templar);
					var hasmalth = world.GetActorBySNO(ActorSno._x1_malthael_npc);

					if (hasmalth == null)
					{
						ActorSystem.Implementations.Hirelings.MalthaelHireling malthaelHire = new ActorSystem.Implementations.Hirelings.MalthaelHireling(world, ActorSno._x1_malthael_npc_nocollision, Templar.Tags);
						malthaelHire.RotationAxis = new Vector3D(0f, 0f, 0.4313562f);
						malthaelHire.RotationW = 0.9021817f;
						malthaelHire.Attributes[GameAttribute.Team_Override] = 2;
						malthaelHire.EnterWorld(new Vector3D(3017.266f, 2851.986f, 24.04533f));
					}
					foreach (var door in world.GetActorsBySNO(ActorSno._house_door_trout_newtristram))
						door.Destroy();
					if (this.Game.CurrentAct == 3000)
					{
						var TownDoor = world.GetActorBySNO(ActorSno._trout_newtristram_gate_town);
						TownDoor.Attributes[GameAttribute.Team_Override] = 2;
						TownDoor.Attributes[GameAttribute.Untargetable] = true;
						TownDoor.Attributes[GameAttribute.NPC_Is_Operatable] = false;
						TownDoor.Attributes[GameAttribute.Operatable] = false;
						TownDoor.Attributes[GameAttribute.Operatable_Story_Gizmo] = false;
						TownDoor.Attributes[GameAttribute.Disabled] = true;
						TownDoor.Attributes[GameAttribute.Immunity] = true;
						TownDoor.Attributes.BroadcastChangedIfRevealed();
					}
					break;
				case WorldSno.a1trdun_level04: //2 уровень собора
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
				case WorldSno.a1trdun_level06: //4 уровень собора
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
				case WorldSno.a1trdun_level05_templar: //Лишние NPC в соборе (3 уровень)
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

				case WorldSno.a2dun_swr_swr_to_oasis_level01: //Убиваем ненужный портал в локации если игра не в режиме приключений
					if (this.Game.CurrentAct != 3000)
						foreach (var wayp in world.GetActorsBySNO(ActorSno._waypoint)) wayp.Destroy();
					break;
				case WorldSno.a2dun_zolt_head_random01: //Убираем кровь кула
					foreach (var act in world.GetActorsBySNO(ActorSno._a2dun_zolt_blood_container_02)) act.Destroy();
					break;
				case WorldSno.a2dun_aqd_special_01: //Главный водосток. Убираем лишние порталы.
					foreach (var port in world.Actors.Values)
						if (port is Portal)
							if ((port as Portal).Destination.WorldSNO == (int)WorldSno.a2dun_aqd_special_b_level01)
								port.Destroy();
					break;
				case WorldSno.a3dun_keep_level04: //Убиваем ненужный портал в локации если игра не в режиме приключений
					if (this.Game.CurrentAct != 3000)
						foreach (var wayp in world.GetActorsBySNO(ActorSno._waypoint)) wayp.Destroy();
					break;
				#region Убиваем все порталы в демонические разломы на первом этаже садов(теперь и на втором этаже), а то чет дохера их), создавать будет скрипт уничтожения скверны. Добалвяем голос Дьябло на несколько участков
				case WorldSno.a4dun_garden_of_hope_01: //1 Этаж садов
					foreach (var HellPortal in world.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal))
						HellPortal.Destroy();
					break;
				case WorldSno.a4dun_garden_of_hope_random: //2 Этаж садов
					foreach (var HellPortal in world.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal))
						HellPortal.Destroy();
					break;
                #endregion
				case WorldSno.a4dun_spire_level_00:
					var LeahGhost = world.SpawnMonster(ActorSno._a4dun_aspect_ghost_07, new Vector3D(570f, 570f, 0.1f)) as InteractiveNPC;
					LeahGhost.Conversations.Clear();
					LeahGhost.Conversations.Add(new ConversationInteraction(198600));
					LeahGhost.Attributes[GameAttribute.Conversation_Icon, 0] = 6;
					LeahGhost.Attributes.BroadcastChangedIfRevealed();
					break;
					//428f, 836f, -20.3f
				case WorldSno.a4dun_spire_level_01:
					var ZoltunGhost = world.SpawnMonster(ActorSno._a4dun_aspect_ghost_02, new Vector3D(428f, 836f, -2f)) as InteractiveNPC;
					ZoltunGhost.Conversations.Clear();
					ZoltunGhost.Conversations.Add(new ConversationInteraction(198402));
					ZoltunGhost.Attributes[GameAttribute.Conversation_Icon, 0] = 6;
					ZoltunGhost.Attributes.BroadcastChangedIfRevealed();
					break;
				case WorldSno.a3dun_ruins_frost_city_a_02:
					foreach (var wayp in world.GetActorsBySNO(ActorSno._waypoint)) wayp.Destroy();
					break;
				case WorldSno.p43_ad_oldtristram:
					foreach (var wayp in world.GetActorsBySNO(ActorSno._trout_oldtristram_exit_gate)) wayp.Destroy();
					break;
				case WorldSno.x1_tristram_adventure_mode_hub:
					
					//Отображаем только одного продавца
					world.ShowOnlyNumNPC(ActorSno._a1_uniquevendor_miner_intown_01, 0);
					//Отображаем только одного мистика
					world.ShowOnlyNumNPC(ActorSno._pt_mystic, 1);
					var Door = world.GetActorBySNO(ActorSno._trout_newtristram_gate_town);
					Door.Attributes[GameAttribute.Team_Override] = 2;
					Door.Attributes[GameAttribute.Untargetable] = true;
					Door.Attributes[GameAttribute.NPC_Is_Operatable] = false;
					Door.Attributes[GameAttribute.Operatable] = false;
					Door.Attributes[GameAttribute.Operatable_Story_Gizmo] = false;
					Door.Attributes[GameAttribute.Disabled] = true;
					Door.Attributes[GameAttribute.Immunity] = true;
					Door.Attributes.BroadcastChangedIfRevealed();
					break;
				case WorldSno.p43_ad_cathedral_level_01: //1 этаж собора (Режим D1)
				case WorldSno.p43_ad_cathedral_level_02: //2 этаж собора (Режим D1)
				case WorldSno.p43_ad_cathedral_level_03: //3 этаж собора (Режим D1)
				case WorldSno.p43_ad_cathedral_level_04: //4 этаж собора (Режим D1)
				case WorldSno.p43_ad_catacombs_level_05: //5 этаж собора (Режим D1)
				case WorldSno.p43_ad_catacombs_level_06: //6 этаж собора (Режим D1)
				case WorldSno.p43_ad_catacombs_level_07: //7 этаж собора (Режим D1)
				case WorldSno.p43_ad_catacombs_level_08: //8 этаж собора (Режим D1)
					foreach (var actor in world.GetActorsBySNO(d1ModeHiddenActors)) actor.Destroy();
					foreach (var actor in world.GetActorsBySNO(ActorSno._g_portal_townportal_red))
					{
						foreach (var sp in actor.GetActorsInRange<StartingPoint>(20f)) sp.Destroy();
						actor.Destroy(); //g_Portal_TownPortal_Red
					}
					break;
			}
			#endregion
			#region Глобал патч при генерации
			foreach (var oldp in world.GetActorsBySNO(
				ActorSno._x1_openworld_lootrunportal,
				ActorSno._x1_openworld_tiered_rifts_portal,
				ActorSno._x1_openworld_tiered_rifts_challenge_portal,
				ActorSno._x1_westm_bridge_scoundrel
				)) 
			{ 
				oldp.Destroy();
			}
			foreach (var oldp in world.GetActorsBySNO(ActorSno._placedgold)) { foreach(var plr in world.Game.Players.Values) world.SpawnGold(oldp, plr); oldp.Destroy(); }
																				  
			if(world.SNO != WorldSno.a1trdun_level05_templar) 
				foreach (var oldp in world.GetActorsBySNO(ActorSno._x1_openworld_tiered_rifts_challenge_portal)) { oldp.Destroy(); }//109209 - Костяные стены из собора
			#endregion

			return world;
		}

		public void RandomSpawnInWorldWithLevelArea(World world, ActorSno monsterSno, int LevelArea = -1)
		{
			List<Scene> Scenes = world.Scenes.Values.ToList();
			if (LevelArea != -1) Scenes = Scenes.Where(sc => sc.Specification.SNOLevelAreas[0] == LevelArea && !sc.SceneSNO.Name.ToLower().Contains("filler")).ToList();
			else Scenes = Scenes.Where(sc => !sc.SceneSNO.Name.ToLower().Contains("filler")).ToList();
			int RandomScene = RandomHelper.Next(0, Scenes.Count - 1);
			Vector3D SSV = Scenes[RandomScene].Position;
			Vector3D SP = null;

			while (true)
			{
				SP = new Vector3D(SSV.X + RandomHelper.Next(0, 240), SSV.Y + RandomHelper.Next(0, 240), SSV.Z);
				if (world.CheckLocationForFlag(SP, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					break;
			}
			world.SpawnMonster(monsterSno, SP);
		}

		public void FilterWaypoints(World world, int SceneSNO = -1)
		{
			var waypoints = world.GetActorsBySNO(ActorSno._waypoint);
			if (SceneSNO != -1) waypoints = waypoints.Where(wp => wp.CurrentScene.SceneSNO.Id == SceneSNO).ToList();

			if (waypoints.Count > 1)
			{
				int RandomPoint = RandomHelper.Next(0, waypoints.Count - 1);
				for (int i = 0; i < waypoints.Count; i++)
				{
					if (i != RandomPoint)
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
		public static void DRLGGenerateProcess(World world, List<List<DRLGEmuScene>> Cont, List<DRLGEmuScene> Fillers, long RangofSc)
		{
			if (world.worldData.SceneParams == null)
				world.worldData.CreateNewSceneParams();

			world.worldData.SceneParams.SceneChunks.Clear();
			List<Vector3D> BusyChunks = new List<Vector3D> { };
			List<Vector3D> ReservedChunks = new List<Vector3D> { };
			Dictionary<char[], Vector3D> WaitChunks = new Dictionary<char[], Vector3D> { };
			Dictionary<char[], Vector3D> NextWaitChunks = new Dictionary<char[], Vector3D> { };

			bool HasExit = false;

			List<List<DRLGEmuScene>> DRLGContainers = Cont;
			List<DRLGEmuScene> FillerChuncks = Fillers;

			char CurrentNav = '.';
			
			DRLGEmuScene CurrentScene = null;

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
			bool Rift = world.SNO.IsGenerated();
			//Getting Enter
			var loadedscene = new SceneChunk();
			CurrentScene = DRLGContainers[0][RandomHelper.Next(0, DRLGContainers[0].Count)];
			loadedscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
			loadedscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), new Vector3D(0, 0, 0));
			loadedscene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));

			world.worldData.SceneParams.SceneChunks.Add(loadedscene); //Add Chunk
			BusyChunks.Add(loadedscene.PRTransform.Vector3D); //Cords Busy

			SceneChunk PrestScene = loadedscene;
			Vector3D PlaceToNewScene = new Vector3D();

			var nextscene = new SceneChunk();
			char[] ToWaitChunks;
			var splits = PrestScene.SNOHandle.Name.Split('_');
			int PosOfNav = 2;
			if (splits[0].ToLower().StartsWith("p43") ||
				splits[2].ToLower().Contains("random") ||
				splits[2].ToLower().Contains("corrupt"))
				PosOfNav = 3;
			else if (PrestScene.SNOHandle.Name.StartsWith("x1_Pand"))
				PosOfNav = 4;

			int RangetoNext = (int)RangofSc;

			//First Switch
			switch (PrestScene.SNOHandle.Name.Split('_')[PosOfNav])
			{

				case "S":
					CurrentNav = 'N';
					while (true)
					{
						if (PrestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							PosOfNav = 3;
						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X + RangetoNext, PrestScene.PRTransform.Vector3D.Y, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene); //Добавить сцену
					BusyChunks.Add(nextscene.PRTransform.Vector3D); //Занять клетку
					PrestScene = nextscene;
					break;
				case "N":
					CurrentNav = 'S';
					while (true)
					{
						if (PrestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							PosOfNav = 3;

						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X - RangetoNext, PrestScene.PRTransform.Vector3D.Y, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene); //Добавить сцену
					BusyChunks.Add(nextscene.PRTransform.Vector3D); //Занять клетку
					PrestScene = nextscene;
					break;
				case "E":
					CurrentNav = 'W';
					while (true)
					{
						if (PrestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							PosOfNav = 3;

						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X, PrestScene.PRTransform.Vector3D.Y + RangetoNext, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene); //Добавить сцену
					BusyChunks.Add(nextscene.PRTransform.Vector3D); //Занять клетку
					PrestScene = nextscene;
					break;
				case "W":

					CurrentNav = 'E';
					while (true)
					{
						if (PrestScene.SNOHandle.Name.StartsWith("x1_Pand"))
							PosOfNav = 3;

						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X, PrestScene.PRTransform.Vector3D.Y - RangetoNext, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene.SceneSpecification = new SceneSpecification(
							   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
							   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
							   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
							   new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene); //Добавить сцену
					BusyChunks.Add(nextscene.PRTransform.Vector3D); //Занять клетку
					PrestScene = nextscene;
					break;

				case "EW":

					var nextscene1 = new SceneChunk();

					CurrentNav = 'E';
					while (true)
					{
						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X, PrestScene.PRTransform.Vector3D.Y - RangetoNext, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene1.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene1.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene1.SceneSpecification = new SceneSpecification(
								0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
								-1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene1); //Добавить сцену
					BusyChunks.Add(nextscene1.PRTransform.Vector3D); //Занять клетку

					CurrentNav = 'W';
					while (true)
					{
						CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
						if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
							break;
					}
					PlaceToNewScene = new Vector3D(PrestScene.PRTransform.Vector3D.X, PrestScene.PRTransform.Vector3D.Y + RangetoNext, PrestScene.PRTransform.Vector3D.Z);
					ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
					foreach (var Point in ToWaitChunks)
						if (Point != CurrentNav)
							switch (Point)
							{
								//Куда застраивать в названии
								//S - Строить направо +240:0
								//N - Строить налево  -240:0
								//E - Строить вверх   0:+240
								//W - Строить вниз    0:-240
								case 'S': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'N': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
								case 'E': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
								case 'W': WaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
							}
					nextscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
					nextscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
					nextscene.SceneSpecification = new SceneSpecification(
								0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
								-1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								new int[4] { 0, 0, 0, 0 }, 0));

					world.worldData.SceneParams.SceneChunks.Add(nextscene); //Добавить сцену
					BusyChunks.Add(nextscene.PRTransform.Vector3D); //Занять клетку
					PrestScene = nextscene;

					break;
			}
			int DRLGDeep = 5;
			if (Rift)
				DRLGDeep = 20;


				//Deep and exits
			for (int i = 0; i <= DRLGDeep; i++)
			{
				foreach (var waitedScene in WaitChunks)
				{
					var newscene = new SceneChunk();
					switch (waitedScene.Key[0])
					{
						case 'S':
							CurrentNav = 'N';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (HasExit)
									{
										CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
										{
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
									else
									{
										CurrentScene = DRLGContainers[1][RandomHelper.Next(0, DRLGContainers[1].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav)) //Exit
										{
											HasExit = true;
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
								}
								else
								{
									CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
									#region проверка на будущее
									ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in ToWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//Куда застраивать в названии
												//S - Строить направо +240:0
												//N - Строить налево  -240:0
												//E - Строить вверх   0:+240
												//W - Строить вниз    0:-240

												case 'S':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)) ||
														 (ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
									if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break; 
									}
								}
							}
							PlaceToNewScene = waitedScene.Value;
							ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();

							foreach (var Point in ToWaitChunks)
								if (Point != CurrentNav)
									switch (Point)
									{
										//Куда застраивать в названии
										//S - Строить направо +240:0
										//N - Строить налево  -240:0
										//E - Строить вверх   0:+240
										//W - Строить вниз    0:-240

										case 'S': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'N': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'E': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
										case 'W': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
									}
							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
							newscene.SceneSpecification = new SceneSpecification(
								   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
								   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								   new int[4] { 0, 0, 0, 0 }, 0));

							world.worldData.SceneParams.SceneChunks.Add(newscene); //Добавить сцену
							BusyChunks.Add(newscene.PRTransform.Vector3D); //Занять клетку
							PrestScene = newscene;
							break;
						case 'N':
							CurrentNav = 'S';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (HasExit)
									{
										CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
										{
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
									else
									{
										CurrentScene = DRLGContainers[1][RandomHelper.Next(0, DRLGContainers[1].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav)) //Exit
										{
											HasExit = true;
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
								}
								else
								{
									CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
									#region проверка на будущее
									ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in ToWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//Куда застраивать в названии
												//S - Строить направо +240:0
												//N - Строить налево  -240:0
												//E - Строить вверх   0:+240
												//W - Строить вниз    0:-240

												case 'S':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)) ||
														 (ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;// else PosOfNav = 3;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
									if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
								}
							}
							PlaceToNewScene = waitedScene.Value;
							ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();

							foreach (var Point in ToWaitChunks)
								if (Point != CurrentNav)
									switch (Point)
									{
										//Куда застраивать в названии
										//S - Строить направо +240:0
										//N - Строить налево  -240:0
										//E - Строить вверх   0:+240
										//W - Строить вниз    0:-240

										case 'S': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'N': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'E': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
										case 'W': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
									}
							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
							newscene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newscene); //Добавить сцену
							BusyChunks.Add(newscene.PRTransform.Vector3D); //Занять клетку
							PrestScene = newscene;
							break;
						case 'E':
							CurrentNav = 'W';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (HasExit)
									{
										CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
										{
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
									else
									{
										CurrentScene = DRLGContainers[1][RandomHelper.Next(0, DRLGContainers[1].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav)) //Exit
										{
											HasExit = true;
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
								}
								else
								{
									CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
									#region проверка на будущее
									ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in ToWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//Куда застраивать в названии
												//S - Строить направо +240:0
												//N - Строить налево  -240:0
												//E - Строить вверх   0:+240
												//W - Строить вниз    0:-240

												case 'S':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)) ||
														 (ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
									if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
								}
							}
							PlaceToNewScene = waitedScene.Value;
							ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();

							foreach (var Point in ToWaitChunks)
								if (Point != CurrentNav)
									switch (Point)
									{
										//Куда застраивать в названии
										//S - Строить направо +240:0
										//N - Строить налево  -240:0
										//E - Строить вверх   0:+240
										//W - Строить вниз    0:-240

										case 'S': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'N': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'E': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
										case 'W': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
									}
							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
							newscene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newscene); //Добавить сцену
							BusyChunks.Add(newscene.PRTransform.Vector3D); //Занять клетку
							PrestScene = newscene;
							break;
						case 'W':
							CurrentNav = 'E';
							while (true)
							{
								if (i > DRLGDeep - 1)
								{
									if (HasExit)
									{
										CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
										{
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
									else
									{
										CurrentScene = DRLGContainers[1][RandomHelper.Next(0, DRLGContainers[1].Count)];
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
										if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav)) //Exit
										{
											HasExit = true;
											if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
											break;
										}
									}
								}
								else
								{
									CurrentScene = DRLGContainers[2][RandomHelper.Next(0, DRLGContainers[2].Count)];
									#region проверка на будущее
									ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();
									bool ForceStop = false;
									foreach (var Point in ToWaitChunks)
										if (Point != waitedScene.Key[0])
											switch (Point)
											{
												//Куда застраивать в названии
												//S - Строить направо +240:0
												//N - Строить налево  -240:0
												//E - Строить вверх   0:+240
												//W - Строить вниз    0:-240

												case 'S':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'N':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'E':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)) ||
														(ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
												case 'W':
													if (BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)) ||
														 (ReservedChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))))
														while (true)
														{
															CurrentScene = DRLGContainers[3][RandomHelper.Next(0, DRLGContainers[3].Count)];//CurrentScene Switch
															if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
															if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //End
															{
																ForceStop = true;
																if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
																break;
															}
														}
													break;
											}
									#endregion
									if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 4;
									if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length > 1) //Way
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
									else if (ForceStop)
									{
										if (CurrentScene.Asset.Name.ToLower().Contains("hexmaze_edge") || CurrentScene.Asset.Name.ToLower().Contains("hexmaze_exit")) PosOfNav = 3;
										break;
									}
								}
							}
							PlaceToNewScene = waitedScene.Value;
							ToWaitChunks = CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray();

							foreach (var Point in ToWaitChunks)
								if (Point != CurrentNav)
									switch (Point)
									{
										//Куда застраивать в названии
										//S - Строить направо +240:0
										//N - Строить налево  -240:0
										//E - Строить вверх   0:+240
										//W - Строить вниз    0:-240

										case 'S': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X + RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'N': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X - RangetoNext, PlaceToNewScene.Y, PlaceToNewScene.Z)); break;
										case 'E': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y + RangetoNext, PlaceToNewScene.Z)); break;
										case 'W': if (!BusyChunks.Contains(new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z))) NextWaitChunks.Add(Point.ToString().ToArray(), new Vector3D(PlaceToNewScene.X, PlaceToNewScene.Y - RangetoNext, PlaceToNewScene.Z)); break;
									}
							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), PlaceToNewScene);
							newscene.SceneSpecification = new SceneSpecification(
				   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
				   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
				   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
				   new int[4] { 0, 0, 0, 0 }, 0));
							world.worldData.SceneParams.SceneChunks.Add(newscene); //Добавить сцену
							BusyChunks.Add(newscene.PRTransform.Vector3D); //Занять клетку
							PrestScene = newscene;
							break;
					}
				}
				WaitChunks.Clear();


				foreach (var NC in NextWaitChunks)
				{
					bool Unique = true;
					//Не накладываем сцену на сцену!
					foreach (var EC in BusyChunks) //Проверяем уже созданные.
						if (NC.Value == EC)
							Unique = false;
					foreach (var RC in ReservedChunks)
						if (NC.Value == RC) //Проверяем резерв!
							Unique = false;
					if (Unique)
					{
						ReservedChunks.Add(NC.Value);
						WaitChunks.Add(NC.Key, NC.Value);
					}

				}
				NextWaitChunks.Clear();
				ReservedChunks.Clear();
			}

			//Force Check Exit
			if (!HasExit)
			{
				bool First = false;
				bool Finish = false;
				var newscene = new SceneChunk();
				foreach (var Chunck in world.worldData.SceneParams.SceneChunks)
				{
					if (!HasExit)
					{
						//Пропускаем первый чанк
						if (!First) { First = true; continue; }
						//Начинаем искать тупики
						//if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(CurrentNav) & CurrentScene.Asset.Name.Split('_')[PosOfNav].ToCharArray().Length == 1) //Way
						if (Chunck.SNOHandle.Name.Split('_')[PosOfNav].ToCharArray().Length == 1)
						{
							char Nav = Chunck.SNOHandle.Name.Split('_')[PosOfNav].ToCharArray()[0];
							while (true)
							{
								CurrentScene = DRLGContainers[1][RandomHelper.Next(0, DRLGContainers[1].Count)];

								if (CurrentScene.Asset.Name.Split('_')[PosOfNav].Contains(Nav)) //Exit
								{
									HasExit = true;
									break;
								}
							}

							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = Chunck.PRTransform;
							newscene.SceneSpecification = new SceneSpecification(
								   0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
								   -1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1,
								   new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)),
								   new int[4] { 0, 0, 0, 0 }, 0));
							Finish = true;
						}
					}
					if (Finish)
					{
						//Преждевременное завершение и добавление выхода
						world.worldData.SceneParams.SceneChunks.Add(newscene); //Добавить сцену
						world.worldData.SceneParams.SceneChunks.Remove(Chunck);
						break;
					}
				}
			}

			//Forming Range
			float ChunkminX = -480;
			float ChunkmaxX = 0;
			float ChunkminY = -480;
			float ChunkmaxY = 0;
			foreach (var chunk in world.worldData.SceneParams.SceneChunks)
			{
				if (chunk.PRTransform.Vector3D.X > ChunkmaxX) ChunkmaxX = chunk.PRTransform.Vector3D.X;
				if (chunk.PRTransform.Vector3D.Y > ChunkmaxY) ChunkmaxY = chunk.PRTransform.Vector3D.Y;
			}
			ChunkmaxX += RangetoNext;
			ChunkmaxY += RangetoNext;

			//Fillers
			List<SceneChunk> FillerChunks = new List<SceneChunk>();
			if (FillerChuncks.Count > 0)
			{
				float X = ChunkminX;
				float Y = ChunkminY;
				while (X < ChunkmaxX)
				{
					float ReturnToMinY = ChunkminY;
					while (Y < ChunkmaxY)
					{
						bool Busy = false;
						foreach (var chunk in world.worldData.SceneParams.SceneChunks)
							if (chunk.PRTransform.Vector3D.X == X & chunk.PRTransform.Vector3D.Y == Y)
							{ Busy = true; break; }

						if (!Busy)
						{
							CurrentScene = DRLGContainers[4][RandomHelper.Next(0, DRLGContainers[4].Count)];

							var newscene = new SceneChunk();
							newscene.SNOHandle = new SNOHandle(SNOGroup.Scene, CurrentScene.SnoID);
							newscene.PRTransform = new PRTransform(new Quaternion(new Vector3D(0f, 0f, 0f), 1), new Vector3D(X, Y, 0));
							newscene.SceneSpecification = new SceneSpecification(0, new Vector2D(0, 0), new int[4] { Rift ? world.SNO != world.Game.WorldOfPortalNephalem ? 288684 : 288482 : CurrentScene.LevelArea, Rift ? CurrentScene.LevelArea : -1, -1, -1 },
								-1, -1, -1, -1, -1, -1, CurrentScene.Music, -1, -1, -1, CurrentScene.Weather, -1, -1, -1, -1, -1, new SceneCachedValues(-1, -1, -1, new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new AABB(new Vector3D(-1, -1, -1), new Vector3D(-1, -1, -1)), new int[4] { 0, 0, 0, 0 }, 0));
							FillerChunks.Add(newscene);
						}
						Busy = false;
						Y += RangetoNext;
					}
					Y = ReturnToMinY;
					X += RangetoNext;
				}

			}
			world.worldData.SceneParams.ChunkCount = world.worldData.SceneParams.SceneChunks.Count;
			foreach (var fil in FillerChunks)
			{
				world.worldData.SceneParams.SceneChunks.Add(fil); //Добавить сцену
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
			if ((worldData.DRLGParams != null) && (worldData.DRLGParams.Count > 0))
				for (int paramIndex = 0; paramIndex < worldData.DRLGParams.Count; paramIndex++)
				{
					var drlgparam = worldData.DRLGParams[paramIndex];
					//Logger.Debug("DRLGParams: LevelArea: {0}", drlgparam.LevelArea);
					foreach (var tile in drlgparam.Tiles)
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
						DRLGTemplate.DRLGLayout world_layout = DRLGTemplate.Templates[worldSNO][FastRandom.Instance.Next(DRLGTemplate.Templates[worldSNO].Count)];
						int coordY = 0;

						foreach (List<int> row in world_layout.map)
						{
							int coordX = 0;
							foreach (int cell in row)
							{
								if (cell != -1)
								{
									Vector3D TilePosition = new Vector3D(drlgparam.ChunkSize * (coordY + 1), drlgparam.ChunkSize * (coordX + 1), 0);

									if (coordX == world_layout.enterPositionX && coordY == world_layout.enterPositionY)
									{
										if (cell <= 115)
											worldTiles.Add(TilePosition, GetTileInfo(tiles, TileTypes.Entrance, cell));
										else
											worldTiles.Add(TilePosition, GetTile(tiles, cell));
									}
									else
										if (coordX == world_layout.exitPositionX && coordY == world_layout.exitPositionY)
									{
										if (cell <= 115)
											worldTiles.Add(TilePosition, GetTileInfo(tiles, TileTypes.Exit, cell));
										else
											worldTiles.Add(TilePosition, GetTile(tiles, cell));
									}
									else
									{
										if (cell <= 115)
											worldTiles.Add(TilePosition, GetTileInfo(tiles, TileTypes.Normal, cell));
										else
											worldTiles.Add(TilePosition, GetTile(tiles, cell));
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
						AddAdjacentTiles(worldTiles, entrance, drlgparam.ChunkSize, tiles, 0, initialStartTilePosition);
						AddFillers(worldTiles, tiles, drlgparam.ChunkSize);
					}

					foreach (var tile in worldTiles)
					{
						AddTile(worldData, tile.Value, tile.Key);
					}

					//AddFiller
					Logger.Debug("RandomGeneration: LevelArea: {0}", drlgparam.LevelArea);
					foreach (var chunk in worldData.SceneParams.SceneChunks)
					{
						if (drlgparam.LevelArea != -1)
						{
							chunk.SceneSpecification.SNOLevelAreas[0] = drlgparam.LevelArea;
							chunk.SceneSpecification.SNOWeather = drlgparam.Weather;
						}
						if (worldSNO == WorldSno.x1_bog_01) //A5 marsh
						{
							if (chunk.PRTransform.Vector3D.Y < 960 || chunk.PRTransform.Vector3D.X < 720)
								chunk.SceneSpecification.SNOLevelAreas[0] = 258142;
						}
					}
					//ProcessCommands(drlgparam, worldData, paramIndex);
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
					counter = AddadjacentTileAtExit(worldTiles, tiles, chunkSize, counter, exit.Value, false);
				else
					counter = AddadjacentTileAtExit(worldTiles, tiles, chunkSize, counter, exit.Value, true);
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
				if (GetTileInfo(tiles, (int)TileTypes.Normal, GetadjacentExitStatus(worldTiles, exit.Value, chunkSize), true) == null) return false;
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
		private int AddadjacentTileAtExit(Dictionary<Vector3D, TileInfo> worldTiles, Dictionary<int, TileInfo> tiles, int chunkSize, int counter, Vector3D position, bool lookingForCork)
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

			Dictionary<TileExits, ExitStatus> exitStatus = GetadjacentExitStatus(worldTiles, position, chunkSize);
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
		private Dictionary<TileExits, ExitStatus> GetadjacentExitStatus(Dictionary<Vector3D, TileInfo> worldTiles, Vector3D position, int chunkSize)
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
			Dictionary<TileExits, Vector3D> exitTypes = new Dictionary<TileExits, Vector3D>();
			exitTypes.Add(TileExits.East, positionEast);
			exitTypes.Add(TileExits.West, positionWest);
			exitTypes.Add(TileExits.North, positionNorth);
			exitTypes.Add(TileExits.South, positionSouth);

			if (!isRandom)
				return exitTypes;

			//randomize
			Dictionary<TileExits, Vector3D> randomExitTypes = new Dictionary<TileExits, Vector3D>();
			var count = exitTypes.Count;

			//Randomise exit directions
			for (int i = 0; i < count; i++)
			{
				//Chose a random exit to test
				Vector3D chosenExitPosition = RandomHelper.RandomValue(exitTypes);
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
			Dictionary<int, TileInfo> acceptedTiles = new Dictionary<int, TileInfo>();
			//By default use all tiles
			acceptedTiles = tiles;
			foreach (TileExits exit in System.Enum.GetValues(typeof(TileExits)))
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
				return GetTileInfo(acceptedTiles.Where(
					pair => pair.Value.TileType == tileType //&& 
															//System.Enum.IsDefined(typeof(TileExits), pair.Value.ExitDirectionBits)
				).ToDictionary(pair => pair.Key, pair => pair.Value), mustHaveExits);
			else
				return GetTileInfo(acceptedTiles.Where(
					pair => pair.Value.TileType == tileType &&
					!System.Enum.IsDefined(typeof(TileExits), pair.Value.ExitDirectionBits)
				).ToDictionary(pair => pair.Key, pair => pair.Value), mustHaveExits);
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
			List<TileInfo> tilesWithRightDirection = (from pair in tiles where ((pair.Value.ExitDirectionBits & exitDirectionBits) > 0) select pair.Value).ToList<TileInfo>();

			if (tilesWithRightDirection.Count == 0)
			{
				Logger.Trace("Did not find matching tile");
				//TODO: Never return null. Try to find other tiles that match entry pattern and rotate
				//There should be a field that defines if tile can be rotated
				return null;
			}

			return RandomHelper.RandomItem(tilesWithRightDirection, x => 1.0f);
		}

		private TileInfo GetTile(Dictionary<int, TileInfo> tiles, int snoId)
		{
			if (!tiles.ContainsKey(snoId)) return null;
			return tiles.Where(x => x.Key == snoId).First().Value;
		}

		/// <summary>
		/// Returns a tileinfo from a list of tiles that has a specific type
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="exitDirectionBits"></param>
		/// <returns></returns>
		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, TileTypes tileType)
		{
			var tilesWithRightType = (from pair in tiles where (pair.Value.TileType == (int)tileType) select pair.Value);
			return RandomHelper.RandomItem(tilesWithRightType, x => 1);
		}

		private TileInfo GetTileInfo(Dictionary<int, TileInfo> tiles, TileTypes tileType, int exitDirectionBits)
		{
			if (exitDirectionBits == 0)
			{
				//return filler
				return GetTileInfo(tiles, TileTypes.Filler);
			}
			List<TileInfo> tilesWithRightTypeAndDirection = new List<TileInfo>();

			if (tileType == TileTypes.Normal)
				tilesWithRightTypeAndDirection = (from pair in tiles where ((pair.Value.TileType == 100 || pair.Value.TileType == 101 || pair.Value.TileType == 102) && pair.Value.ExitDirectionBits == exitDirectionBits) select pair.Value).ToList<TileInfo>();
			else
				tilesWithRightTypeAndDirection = (from pair in tiles where (pair.Value.TileType == (int)tileType && pair.Value.ExitDirectionBits == exitDirectionBits) select pair.Value).ToList<TileInfo>();
			if (tilesWithRightTypeAndDirection.Count == 0)
			{
				Logger.Error("Did not find matching tile for template! Type: {0}, Direction: {1}", tileType, exitDirectionBits);
				return null;
			}

			return RandomHelper.RandomItem(tilesWithRightTypeAndDirection, entry => entry.Probability);
		}

		private void AddTile(DiIiS_NA.Core.MPQ.FileFormats.World worldData, TileInfo tileInfo, Vector3D location)
		{
			var sceneChunk = new SceneChunk();
			sceneChunk.SNOHandle = new SNOHandle(tileInfo.SNOScene);
			sceneChunk.PRTransform = new PRTransform();
			sceneChunk.PRTransform.Quaternion = new Quaternion();
			sceneChunk.PRTransform.Quaternion.W = 1.0f;
			sceneChunk.PRTransform.Quaternion.Vector3D = new Vector3D(0, 0, 0);
			sceneChunk.PRTransform.Vector3D = new Vector3D();
			sceneChunk.PRTransform.Vector3D = location;

			var spec = new SceneSpecification();
			//scene.Specification = spec;
			spec.Cell = new Vector2D() { X = 0, Y = 0 };
			spec.CellZ = 0;
			spec.SNOLevelAreas = new int[] { -1, -1, -1, -1 };
			spec.SNOMusic = -1;
			spec.SNONextLevelArea = -1;
			spec.SNONextWorld = -1;
			spec.SNOPresetWorld = -1;
			spec.SNOPrevLevelArea = -1;
			spec.SNOPrevWorld = -1;
			spec.SNOReverb = -1;
			spec.SNOWeather = 50542;
			spec.SNOCombatMusic = -1;
			spec.SNOAmbient = -1;
			spec.ClusterID = -1;
			spec.PrevEntranceGUID = 14;
			spec.DRLGIndex = 5;
			spec.SceneChunk = -1;
			spec.OnPathBits = tileInfo.TileType; //we can make it TileType value
			spec.SceneCachedValues = new SceneCachedValues();
			spec.SceneCachedValues.CachedValuesValid = 63;
			spec.SceneCachedValues.NavMeshSizeX = 96;
			spec.SceneCachedValues.NavMeshSizeY = 96;
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

		private static readonly Dictionary<int, float> GizmosToSpawn = new Dictionary<int, float>{
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

		private static readonly List<int> Goblins = new List<int>{
			{5984},
			{5985},
			{5987},
			{5988}
		};

		private Dictionary<int, List<Scene>> LazyLevelAreas = new Dictionary<int, List<Scene>>();

		/// <summary>
		/// Loads content for level areas. Call this after scenes have been generated and after scenes have their GizmoLocations
		/// set (this is done in Scene.LoadActors right now)
		/// </summary>
		/// <param name="levelAreas">Dictionary that for every level area has the scenes it consists of</param>
		/// <param name="world">The world to which to add loaded actors</param>
		private void loadLevelAreas(Dictionary<int, List<Scene>> levelAreas, World world)
		{
			/// Each Scene has one to four level areas assigned to it. I dont know if that means
			/// the scene belongs to both level areas or if the scene is split
			/// Scenes marker tags have generic GizmoLocationA to Z that are used 
			/// to provide random spawning possibilities.
			/// For each of these 26 LocationGroups, the LevelArea has a entry in its SpawnType array that defines
			/// what type of actor/encounter/adventure could spawn there
			/// 
			/// It could for example define, that for a level area X, out of the four spawning options
			/// two are randomly picked and have barrels placed there
			

			Dictionary<PRTransform, DiIiS_NA.Core.MPQ.FileFormats.Actor> dict = new Dictionary<PRTransform, DiIiS_NA.Core.MPQ.FileFormats.Actor>();
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
					LazyLoadActor(handle, gizmoLocations[FastRandom.Instance.Next(gizmoLocations.Count)], world, ((DiIiS_NA.Core.MPQ.FileFormats.Actor)handle.Target).TagMap);
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
							LazyLoadActor(gizmoHandle, location, world, ((DiIiS_NA.Core.MPQ.FileFormats.Actor)gizmoHandle.Target).TagMap);
						}
					}

				if (gizmoLocations.Count > 0 && world.Game.MonsterLevel >= Program.MaxLevel && FastRandom.Instance.Next(100) < 30)
				{
					var handle_chest = new SNOHandle(96993); //leg chest
					if (handle_chest == null) continue;
					var golden_chest = LoadActor(handle_chest, gizmoLocations[FastRandom.Instance.Next(0, gizmoLocations.Count - 1)], world, ((DiIiS_NA.Core.MPQ.FileFormats.Actor)handle_chest.Target).TagMap);
					if (golden_chest > 0)
						(world.GetActorByGlobalId(golden_chest) as LegendaryChest).ChestActive = true;
				}

				if (world.DRLGEmuActive)
				{
					int wid = (int)world.SNO;
					// Load monsters for level area
					foreach (var scene in levelAreas.First().Value)
					{
						if (!SpawnGenerator.Spawns.ContainsKey(wid)) break;
						if (SpawnGenerator.Spawns[wid].lazy_load == true)
						{
							this.LazyLevelAreas.Add(wid, levelAreas.First().Value);
							break;
						}
						else
							LoadMonstersLayout(world, wid, scene);
					}
					#region unique spawn
					//unique spawn
					if (SpawnGenerator.Spawns.ContainsKey(wid) && SpawnGenerator.Spawns[wid].dangerous.Count > 0 && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomUnique = new SNOHandle(SpawnGenerator.Spawns[wid].dangerous[FastRandom.Instance.Next(SpawnGenerator.Spawns[wid].dangerous.Count)]);
						var scene = levelAreas.First().Value[FastRandom.Instance.Next(levelAreas.First().Value.Count)];
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
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
					if (SpawnGenerator.Spawns.ContainsKey(wid) && SpawnGenerator.Spawns[wid].can_spawn_goblin && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomGoblin = new SNOHandle(Goblins[FastRandom.Instance.Next(Goblins.Count)]);
						if (world.Game.IsHardcore) randomGoblin = new SNOHandle(3852);
						var scene = levelAreas.First().Value[FastRandom.Instance.Next(levelAreas.First().Value.Count)];
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
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
						if (SpawnGenerator.Spawns[la].lazy_load == true)
						{
							this.LazyLevelAreas.Add(la, levelAreas[la]);
							break;
						}
						else
							LoadMonstersLayout(world, la, scene);
					}
					#region unique spawn
					//unique spawn
					if (SpawnGenerator.Spawns.ContainsKey(la) && SpawnGenerator.Spawns[la].dangerous.Count > 0 && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomUnique = new SNOHandle(SpawnGenerator.Spawns[la].dangerous[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].dangerous.Count)]);
						var scene = levelAreas[la][FastRandom.Instance.Next(levelAreas[la].Count)];
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
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
					if (SpawnGenerator.Spawns.ContainsKey(la) && SpawnGenerator.Spawns[la].can_spawn_goblin && FastRandom.Instance.NextDouble() < 0.5)
					{
						var randomGoblin = new SNOHandle(Goblins[FastRandom.Instance.Next(Goblins.Count)]);
						if (world.Game.IsHardcore) randomGoblin = new SNOHandle(3852);
						var scene = levelAreas[la][FastRandom.Instance.Next(levelAreas[la].Count)];
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
								Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
			packs_count += (this.Game.Difficulty / 3);

			if (world.worldData.DRLGParams != null && world.worldData.DRLGParams.Count > 0)
			{
				if (world.worldData.DRLGParams.First().ChunkSize == 120)
					packs_count -= 2;
				if (world.worldData.DRLGParams.First().ChunkSize > 240)
					packs_count += 2;
			}

			if (this.Game.Difficulty > 4)
				packs_count += SpawnGenerator.Spawns[la].additional_density;

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
						if (SpawnGenerator.Spawns[la].melee.Count > 0) randomMeleeMonsterId = SpawnGenerator.Spawns[la].melee[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].melee.Count)];
						if (SpawnGenerator.Spawns[la].range.Count > 0) randomRangedMonsterId = SpawnGenerator.Spawns[la].range[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].range.Count)];
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
										Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
							if (SpawnGenerator.Spawns[la].melee.Count > 0) randomMeleeMonsterId = SpawnGenerator.Spawns[la].melee[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].melee.Count)];
							if (SpawnGenerator.Spawns[la].range.Count > 0) randomRangedMonsterId = SpawnGenerator.Spawns[la].range[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].range.Count)];
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
											Quaternion = Quaternion.FacingRotation((float)(FastRandom.Instance.NextDouble() * System.Math.PI * 2))
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
							SNOHandle championHandle = new SNOHandle(SpawnGenerator.Spawns[la].melee[FastRandom.Instance.Next(SpawnGenerator.Spawns[la].melee.Count)]);
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
			var actorSno = (ActorSno)actorHandle.Id; // TODO: maybe we can replace SNOHandle
			if (world.QuadTree.Query<Waypoint>(new Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 60f)).Count > 0 ||
				world.QuadTree.Query<Portal>(new Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 5f)).Count > 0)
			{
				Logger.Trace("Load actor {0} ignored - waypoint nearby.", actorSno);
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

		public void LazyLoadActor(SNOHandle actorHandle, PRTransform location, World world, TagMap tagMap, MonsterType monsterType = MonsterType.Default)
		{
			var actorSno = (ActorSno)actorHandle.Id; // TODO: maybe we can replace SNOHandle
			if (world.QuadTree.Query<Waypoint>(new DiIiS_NA.GameServer.Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 60f)).Count > 0 ||
				world.QuadTree.Query<Portal>(new DiIiS_NA.GameServer.Core.Types.Misc.Circle(location.Vector3D.X, location.Vector3D.Y, 40f)).Count > 0)
			{
				Logger.Trace("Load actor {0} ignored - waypoint nearby.", actorSno);
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
		private Vector3D FindSubScenePosition(DiIiS_NA.Core.MPQ.FileFormats.SceneChunk sceneChunk)
		{
			var mpqScene = MPQStorage.Data.Assets[SNOGroup.Scene][sceneChunk.SNOHandle.Id].Data as DiIiS_NA.Core.MPQ.FileFormats.Scene;

			foreach (var markerSet in mpqScene.MarkerSets)
			{
				var mpqMarkerSet = MPQStorage.Data.Assets[SNOGroup.MarkerSet][markerSet].Data as DiIiS_NA.Core.MPQ.FileFormats.MarkerSet;
				foreach (var marker in mpqMarkerSet.Markers)
					if (marker.Type == DiIiS_NA.Core.MPQ.FileFormats.MarkerType.SubScenePosition)
						return marker.PRTransform.Vector3D;
			}
			return null;
		}
	}
}
