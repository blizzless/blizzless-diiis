//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Portal : Actor
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		public ResolvedPortalDestination Destination { get; set; }
		private int MinimapIcon;

		public bool randomed = true;
		bool Smart = false;

		public Portal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			if (World != null)
            {
				int count = World.GetActorsBySNO(SNO).Count;
				if (count > 0)
					NumberInWorld = count;
            }

            try
			{
				//Logger.Debug("Portal {0} has destination world {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationWorld].Id);
				//Logger.Debug("Portal {0} has destination LevelArea {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationLevelArea].Id);
				//Logger.Info("Portal {0} has destination StartingPoint {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationActorTag]);
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = tags[MarkerKeys.DestinationLevelArea].Id,
					StartingPointActorTag = tags[MarkerKeys.DestinationActorTag]
				};

				// Override minimap icon in markerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}

			}
			catch (KeyNotFoundException)
			{
				Logger.Trace("Portal {0} has incomplete definition", SNO);
			}

			if (World.SNO == WorldSno.trdun_crypt_falsepassage_01 || World.SNO == WorldSno.trdun_crypt_falsepassage_02)
			{
				Destination = SmartExitGenerate();
			}
			

			else if (World.SNO == WorldSno.trout_adriascellar) //portal Adria's Hut
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trout_town,
					DestLevelAreaSNO = 101351,
					StartingPointActorTag = 8
				};

			else if (World.SNO == WorldSno.x1_westm_deathorb_kerwinsrow) //portal Noble's Rest Courtyard
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.x1_westm_graveyard_deathorb,
					DestLevelAreaSNO = 338946,
					StartingPointActorTag = 171
				};

			else if(SNO == ActorSno._g_portal_arch_orange && World.SNO == WorldSno.trdun_jail_level01) //portal Leoric Jail -> 3rd lvl Halls of Agony
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level03,
					DestLevelAreaSNO = 19776,
					StartingPointActorTag = 172
				};

			else if(SNO == ActorSno._g_portal_archtall_blue && World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_02) //portal advisor's tomb -> 2nd lvl Crypt
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_01,
					DestLevelAreaSNO = 60601,
					StartingPointActorTag = 171
				};

			else if(World.SNO == WorldSno.caout_interior_d) //portal conclave -> Stinging winds
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 201
				};

			else if(World.SNO == WorldSno.caout_interior_f) //portal altar -> Stinging winds
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 194
				};

			else if(World.SNO == WorldSno.caout_khamsin_mine) //portal Khamsin HQ -> Stinging winds
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 63666,
					StartingPointActorTag = 70
				};

			else if(World.SNO == WorldSno.a2dun_aqd_special_01 && Destination == null) //portal Khamsin HQ -> Stinging winds
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
					DestLevelAreaSNO = 62752,
					StartingPointActorTag = 86
				};

			else if(World.SNO == WorldSno.a2dun_zolt_shadowrealm_level01) //portal ZKShadow
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a2dun_zolt_lobby,
					DestLevelAreaSNO = 19800,
					StartingPointActorTag = 145
				};

			#region Спуск на второй уровень в подземелье на кладбище
			else if (World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_00 && SNO == ActorSno._g_portal_archtall_blue) //Crypt A1 Q3
			{
				var Portal = world.GetActorBySNO(ActorSno._g_portal_archtall_blue);
				if (Portal == null)
				{
					Destination = SmartExitGenerate();
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_01,
						DestLevelAreaSNO = 60601,
						StartingPointActorTag = 15
					};
				}
			}
			else if (World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_01)
			{
				var Portal = world.GetActorBySNO(ActorSno._g_portal_archtall_blue);
				if (Portal == null)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_00,
						DestLevelAreaSNO = 145182,
						StartingPointActorTag = 171
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_02,
						DestLevelAreaSNO = 102362,
						StartingPointActorTag = 172
					};
				}
				

			}
			#endregion

			#region 2 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level04 && SNO == ActorSno._g_portal_archtall_orange)
			{
				var Portal = world.GetActorBySNO(ActorSno._g_portal_archtall_orange);
				if (Portal == null)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level05_templar,
						DestLevelAreaSNO = 87907,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_cain_intro,
						DestLevelAreaSNO = 60714,
						StartingPointActorTag = 15 //30
					};
				}
			}
			#endregion
			#region 3 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level05_templar && SNO == ActorSno._g_portal_archtall_orange)
			{
				var Portal = world.GetActorBySNO(ActorSno._g_portal_archtall_orange);
				if (Portal == null)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level04,
						DestLevelAreaSNO = 19783,//19785 - 4 уровень собора
						StartingPointActorTag = 171
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level06,
						DestLevelAreaSNO = 19785,
						StartingPointActorTag = 172
					};
				}

			}
			#endregion
			#region 4 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level06 && SNO == ActorSno._g_portal_archtall_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a1trdun_level05_templar,
					DestLevelAreaSNO = 87907,
					StartingPointActorTag = 171
				};
			}
			if (world.SNO == WorldSno.a1trdun_level06 && SNO == ActorSno._g_portal_rectangle_blue)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a1trdun_level07,
					DestLevelAreaSNO = 19787,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Первый этаж Агонии
			//Вход
			else if (world.SNO == WorldSno.trdun_leoric_level01 && SNO == ActorSno._g_portal_arch_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a1dun_leor_manor,
					DestLevelAreaSNO = 100854,
					StartingPointActorTag = 171
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}
			//Выход на 2 этаж
			else if (world.SNO == WorldSno.trdun_leoric_level01 && SNO == ActorSno._g_portal_rectangle_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level02,
					DestLevelAreaSNO = 19775,
					StartingPointActorTag = 172,//tags[MarkerKeys.DestinationActorTag]
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}
			#endregion
			#region Второй этаж Агонии
			//Вход
			else if (world.SNO == WorldSno.trdun_leoric_level02 && SNO == ActorSno._g_portal_arch_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level01,
					DestLevelAreaSNO = 19774,
					StartingPointActorTag = 171
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}
			#endregion
			#region Переправа в высокогорье
			//Вход
			else if (world.SNO == WorldSno.trout_highlands_dunexteriora && SNO == ActorSno._g_portal_archtall_orange && NumberInWorld == 1)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level02,
					DestLevelAreaSNO = 19775,
					StartingPointActorTag = 171
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}
			#endregion
			#region Проклятая крепость
			//Выход на 3 этаж Агонии
			else if (world.SNO == WorldSno.trdun_jail_level01 && SNO == ActorSno._g_portal_arch_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level03,
					DestLevelAreaSNO = 19776,
					StartingPointActorTag = 172
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}
			#endregion
			#region Третий этаж Агонии
			//Вход
			else if (world.SNO == WorldSno.trdun_leoric_level03 && SNO == ActorSno._g_portal_arch_orange)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_jail_level01,
					DestLevelAreaSNO = 94672,
					StartingPointActorTag = 171
				};

				// Override minimap icon in merkerset tags
				if (tags.ContainsKey(MarkerKeys.MinimapTexture))
				{
					MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
				}
				else
				{
					MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
				}
			}

			#endregion

			#region Восточный водосток
			if (world.SNO == WorldSno.a2dun_aqd_special_b && SNO == ActorSno._g_portal_square_blue)
			{
				var Portal = world.GetActorBySNO(ActorSno._g_portal_square_blue);
				if (Portal == null)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 93
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 87
					};
				}

			}
			#endregion

			#region Нижние этажи крепости: Уровень 2
			if (world.SNO == WorldSno.a3dun_keep_level04)
			{
				if (SNO == ActorSno._g_portal_archtall_orange)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level03,
						DestLevelAreaSNO = 75436,
						StartingPointActorTag = 171
					};
				}
				else if (SNO == ActorSno._g_portal_rectangle_orange)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level05,
						DestLevelAreaSNO = 136448,
						StartingPointActorTag = 172
					};
				}
			}
			#endregion
			#region Нижние этажи крепости: Уровень 3
			else if (world.SNO == WorldSno.a3dun_keep_level05)
			{
				if (SNO == ActorSno._g_portal_archtall_orange)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level04,
						DestLevelAreaSNO = 93103,
						StartingPointActorTag = 171
					};
				}
				else if (SNO == ActorSno._g_portal_rectangle_orange)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.gluttony_boss,
						DestLevelAreaSNO = 111232,
						StartingPointActorTag = 172
					};
				}
			}
			//103209
			#endregion

			#region Ареатский кратер: Уровень 1
			else if (world.SNO == WorldSno.a3dun_crater_level_01)
			{
				if (World.GetActorsBySNO(ActorSno._g_portal_archtall_orange).Count > 0)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level04,
						DestLevelAreaSNO = 112580,
						StartingPointActorTag = 171
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level01,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 172
					};
			}
			#endregion

			#region Башня обреченных: Уровень 1
			else if (world.SNO == WorldSno.a3dun_crater_st_level01)
			{
				if (SNO == ActorSno._g_portal_archtall_orange_largeradius)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_level_01,
						DestLevelAreaSNO = 86080,
						StartingPointActorTag = 171
					};
			}
			#endregion

			#region Башня обреченных: Уровень 2
			else if (world.SNO == WorldSno.a3dun_crater_st_level02)
			{
				if (World.GetActorsBySNO(ActorSno._g_portal_archtall_orange).Count > 0)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level01,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 171
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level04,
						DestLevelAreaSNO = 85202,
						StartingPointActorTag = 172
					};
			}
			#endregion

			#region Ареатский кратер: Уровень 2

			#endregion
			#region Башня проклятых: Уровень 1
			else if (world.SNO == WorldSno.a3dun_crater_st_level01b)
			{
				if (SNO == ActorSno._g_portal_archtall_orange_largeradius)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_level_02,
						DestLevelAreaSNO = 119305,
						StartingPointActorTag = 171
					};
			}
			#endregion

			#region Вход на первый этаж Садов
			//109143
			else if (world.SNO == WorldSno.a4dun_heaven_1000_monsters_fight)
			{
				//if (this.ActorSNO.Id == 204747)
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_garden_of_hope_01,
					DestLevelAreaSNO = 109514,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Пещера под колодцем
			else if (world.SNO == WorldSno.a1trdun_cave_qa_well)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trout_town,
					DestLevelAreaSNO = 91133,
					StartingPointActorTag = 148
				};
			}
			#endregion

			#region Демонический разлом на первом этаже Садов Надежды.
			else if (world.SNO == WorldSno.a4dun_garden_of_hope_01 && SNO == ActorSno._a4_heaven_gardens_hellportal)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_hell_portal_01,//tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = 109526,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Демонический разлом на втором этаже Садов Надежды.
			else if (world.SNO == WorldSno.a4dun_garden_of_hope_random && SNO == ActorSno._a4_heaven_gardens_hellportal)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_hell_portal_02,//tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = 109526,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Выход на первый этаж садов
			//109143
			else if (world.SNO == WorldSno.a4dun_hell_portal_01)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_garden_of_hope_01,
					DestLevelAreaSNO = 109514,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Выход на второй этаж садов
			//109143
			else if (world.SNO == WorldSno.a4dun_hell_portal_02)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_garden_of_hope_random,
					DestLevelAreaSNO = 109516,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Шпиль
			//1 Этаж
			else if (world.SNO == WorldSno.a4dun_spire_level_01)
			{
				if (SNO == ActorSno._a4dun_spire_elevator_portal_down) //Выход
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_level_00,
						DestLevelAreaSNO = 198564,
						StartingPointActorTag = 171
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_exterior,
						DestLevelAreaSNO = 215396,
						StartingPointActorTag = 172
					};
			}
			//Пролёт 
			else if (world.SNO == WorldSno.a4dun_spire_exterior)
			{
				if (world.Portals.Count == 0) //Выход
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_level_01,
						DestLevelAreaSNO = 109538,
						StartingPointActorTag = 171
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_level_02,
						DestLevelAreaSNO = 109540,
						StartingPointActorTag = 172
					};
			}
			//2 Этаж
			else if (world.SNO == WorldSno.a4dun_spire_level_02)
			{
				if (world.Portals.Count == 0)
					Destination = new ResolvedPortalDestination
					{
						//TODO: 
						WorldSNO = (int)WorldSno.a4dun_spire_diabloentrance,
						DestLevelAreaSNO = 205434,
						StartingPointActorTag = 172
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_exterior,
						DestLevelAreaSNO = 215396,
						StartingPointActorTag = 171
					};
			}

			#endregion

			#region Сокровищница / Зона гоблинсов =)
			else if (SNO == ActorSno._p1_greed_portal)
			{
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.p1_tgoblin_realm,
					DestLevelAreaSNO = 378681,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Эвент - Старый тристрам

			#region 1 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_01 && SNO == ActorSno._g_portal_archtall_orange)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_02,
						DestLevelAreaSNO = 452988,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_oldtristram,
						DestLevelAreaSNO = 455466,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 2 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_02 && SNO == ActorSno._g_portal_archtall_orange)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_03,
						DestLevelAreaSNO = 452989,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_01,
						DestLevelAreaSNO = 452986,
						StartingPointActorTag = 171
					};
				}

			}
			#endregion
			#region 3 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_03 && SNO == ActorSno._g_portal_archtall_orange)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_04,
						DestLevelAreaSNO = 452990,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_02,
						DestLevelAreaSNO = 452988,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 4 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_04 && SNO == ActorSno._g_portal_archtall_orange)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_05,
						DestLevelAreaSNO = 452992,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_03,
						DestLevelAreaSNO = 452989,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 5 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_05 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_06,
						DestLevelAreaSNO = 452993,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_04,
						DestLevelAreaSNO = 452990,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 6 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_06 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_07,
						DestLevelAreaSNO = 452994,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_05,
						DestLevelAreaSNO = 452992,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 7 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_07 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_08,
						DestLevelAreaSNO = 452995,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_06,
						DestLevelAreaSNO = 452993,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 8 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_08 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_caves_level_09,
						DestLevelAreaSNO = 453001,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_07,
						DestLevelAreaSNO = 452994,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#endregion


			#region Старый монастырь
			else if (world.SNO == WorldSno.a3dun_ruins_frost_city_a_02)
			{
				if (SNO == ActorSno._g_portal_rectangle_blue && World.GetActorsBySNO(ActorSno._g_portal_rectangle_blue).Count == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_ruins_frost_city_a_01,
						DestLevelAreaSNO = 428494,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region Руины Сечерона
			else if (world.SNO == WorldSno.a3dun_ruins_frost_city_a_01)
			{
				if (SNO == ActorSno._g_portal_rectangle_blue && World.GetActorsBySNO(ActorSno._g_portal_rectangle_blue).Count == 1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_ruins_frost_city_a_02,
						DestLevelAreaSNO = 430336,
						StartingPointActorTag = 172
					};
				}
			}
			#endregion
			#region Вечный лес
			else if (world.SNO == WorldSno.p4_forest_snow_01)
			{
				if (SNO == ActorSno._g_portal_archtall_blue && World.GetActorsBySNO(ActorSno._g_portal_archtall_blue).Count == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_ruins_frost_city_a_01,
						DestLevelAreaSNO = 428494,
						StartingPointActorTag = 615
					};
				}
			}
			#endregion

			#region 5 Акт
			#region Торговый квартал Вестмарша
			else if (world.SNO == WorldSno.x1_westm_zone_01 && SNO == ActorSno._g_portal_archtall_blue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westmarch_hub,
						DestLevelAreaSNO = 270011,
						StartingPointActorTag = 466
					};
				}
			}
			#endregion
			#region Кладбище Бриартон
			else if (world.SNO == WorldSno.x1_westm_graveyard_deathorb && SNO == ActorSno._g_portal_archtall_blue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westm_deathorb_gideonscourt,
						DestLevelAreaSNO = 338956,
						StartingPointActorTag = 171
					};
				}
			}
			//338946
			#endregion
			#region Верхний Вестамарш
			else if (world.SNO == WorldSno.x1_westm_zone_03 && SNO == ActorSno._g_portal_archtall_orange)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westmarch_hub,
						DestLevelAreaSNO = 270011,
						StartingPointActorTag = 442
					};
				}
			}
			//338946
			#endregion
			#region Проход к Корвусу
			//Выход - W:267412 A: 258142 P: 171

			else if (world.SNO == WorldSno.x1_catacombs_level01 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_catacombs_level02,
						DestLevelAreaSNO = 283567,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_bog_01,
						DestLevelAreaSNO = 258142,
						StartingPointActorTag = 171
					};
				}
			}
			//Второй уровень
			else if (world.SNO == WorldSno.x1_catacombs_level02 && SNO == ActorSno._g_portal_ladder_veryshort_blue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_catacombs_level01,
						DestLevelAreaSNO = 283553,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region Крепость пандемония. Уровень 1
			else if (world.SNO == WorldSno.x1_fortress_level_01 && SNO == ActorSno._g_portal_square_blue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_fortress_level_02,
						DestLevelAreaSNO = 360494,
						StartingPointActorTag = 172
					};
				}
				else
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_pand_batteringram,
						DestLevelAreaSNO = 295228,
						StartingPointActorTag = 171
					};
				}
			}
			else if (world.SNO == WorldSno.x1_fortress_level_02 && SNO == ActorSno._g_portal_archtall_blue_iconblue)
			{
				if (NumberInWorld == 0)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_fortress_level_01,
						DestLevelAreaSNO = 271234,
						StartingPointActorTag = 171
					};
				}

			}
			#endregion
			#endregion
			#region Умное вычисление выхода
			if (Destination == null)
			{
				//102231 - Пустыня
				Logger.Warn("Портал - {0} Не определён до конца, исполнение функции ''умного'' вычисления для выхода.", SNO);
				Smart = true;

				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.__NONE,
					DestLevelAreaSNO = -1,
					StartingPointActorTag = -1
				};
				
			}
			#endregion
			Field2 = 0x9;//16;
		}
		public ResolvedPortalDestination SmartExitGenerate()
		{
			Logger.Warn("Portal - {0} Smart Generation.", SNO);
			int LevelArea = 0;
			int BackPoint = -1;
			if (World.SNO.IsDungeon())
			{
				if (World.SNO == World.Game.WorldOfPortalNephalem)
				{
					//Вход 1 этаж
					if(CurrentScene.SceneSNO.Name.ToLower().Contains("entr"))
						return new ResolvedPortalDestination
						{
							WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub,
							DestLevelAreaSNO = 332339,
							StartingPointActorTag = 24
						};
					//Выход на второй этаж
					else
						return new ResolvedPortalDestination
						{
							WorldSNO = (int)World.Game.WorldOfPortalNephalemSec,
							DestLevelAreaSNO = 288684,
							StartingPointActorTag = 172
						};
				}
				
				return new ResolvedPortalDestination
				{
					WorldSNO = (int)World.Game.WorldOfPortalNephalem,
					DestLevelAreaSNO = 288482,
					StartingPointActorTag = 171
				};
				
			}
			else
			{
				if (!World.Game.Players.IsEmpty)
				{
                    var player = World.Game.Players.First().Value;
                    LevelArea = player.CurrentScene.Specification.SNOLevelAreas.LastOrDefault(x => x != -1);

					if (player.GetActorsInRange<StartingPoint>(20f).Count > 0)
						BackPoint = (player.GetActorsInRange<StartingPoint>(20f).First() as StartingPoint).TargetId;

					return new ResolvedPortalDestination
                    {
						WorldSNO = (int)player.World.SNO,
						DestLevelAreaSNO = LevelArea,
						StartingPointActorTag = BackPoint
					};
				}
				else
				{
					return new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.__NONE,
						DestLevelAreaSNO = LevelArea,
						StartingPointActorTag = -1
					};
				}
			}
		}
		public override void EnterWorld(Vector3D position)
		{
			base.EnterWorld(position);
			if (World.SNO == WorldSno.caout_town || World.SNO == WorldSno.a3_battlefields_02)
			{
				var portals = GetActorsInRange<Portal>(5f).Where(p => p.Destination != null && p.Destination.DestLevelAreaSNO != -1).ToList();
				if (portals.Count >= 2)
				{
					var random_portal = portals[FastRandom.Instance.Next(portals.Count)];
					var bounty_portals = World.Game.QuestManager.Bounties.Where(b => !b.PortalSpawned).SelectMany(b => b.LevelAreaChecks).Intersect(portals.Select(p => p.Destination.DestLevelAreaSNO));
					if (bounty_portals.Any())
					{
						random_portal = portals.First(p => World.Game.QuestManager.Bounties.SelectMany(b => b.LevelAreaChecks).Where(w => w != -1).Contains(p.Destination.DestLevelAreaSNO));
						World.Game.QuestManager.Bounties.First(b => b.LevelAreaChecks.Contains(random_portal.Destination.DestLevelAreaSNO)).PortalSpawned = true;
					}
					foreach (var portal in portals)
						portal.randomed = false;
					random_portal.randomed = true;
				}
			}

			if (Destination == null || Destination.WorldSNO == (int)WorldSno.__NONE)
			{
				var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
				var scenes = World.QuadTree.Query<Scene>(proximity);
				if (scenes.Count == 0) return;

				var scene = scenes[0]; // Parent scene /fasbat

				if (scene.Specification == null) return;

				if (scene.Specification.SNONextWorld != -1)
				{
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = scene.Specification.SNONextWorld,
						DestLevelAreaSNO = scene.Specification.SNONextLevelArea,
						StartingPointActorTag = scene.Specification.NextEntranceGUID
					};
				}

				if (scene.SceneSNO.Id == 129430)
					if (Position.Y < 100.0f)
						Destination = new ResolvedPortalDestination
						{
							WorldSNO = (int)WorldSno.a3dun_crater_level_01,
							DestLevelAreaSNO = 86080,
							StartingPointActorTag = 171
						};

				if (scene.SceneSNO.Id == 335727) //Gideon's Row entrance
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westm_zone_01,
						DestLevelAreaSNO = 261758,
						StartingPointActorTag = 171
					};
				if (scene.SceneSNO.Id == 335742) //Gideon's Row exit
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westm_graveyard_deathorb,
						DestLevelAreaSNO = 338946,
						StartingPointActorTag = 172
					};

				if (World.PortalOverrides.ContainsKey(scene.SceneSNO.Id))
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)World.PortalOverrides[scene.SceneSNO.Id],
						DestLevelAreaSNO = 283553,
						StartingPointActorTag = 172
					};
			}
			/*
			if (this.Destination != null && this.Destination.WorldSNO == 129305 && this.World.WorldSNO.Id == 214956) //Izual exit
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 129306,
					DestLevelAreaSNO = 109542,
					StartingPointActorTag = 172
				};
			//*/
			if (World.IsPvP && Destination != null && Destination.DestLevelAreaSNO == 19947) //spawn safe zone
			{
				var zone_actor = new PVPSafeZone(World, ActorSno._pvp_murderball_highscoringzone, new TagMap());
				zone_actor.AdjustPosition = false;
				zone_actor.EnterWorld(Position);
				World.BuffManager.AddBuff(zone_actor, zone_actor, new PVPSafeZoneBuff());
			}
		}

		public override bool Reveal(Player player)
		{
			
			if (SNO == ActorSno._g_portal_archtall_blue && World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_00)
			{
				//this.Destination.WorldSNO = 
			}
			if (!randomed && Destination.DestLevelAreaSNO != 19794) return false;
			//if (this.ActorSNO.Id == 209083) return false; //pony level portal
			if (SNO == ActorSno._g_portal_rectangle_orange && World.SNO == WorldSno.a4dun_heaven_hub_keep) return false; //armory a4 portal
			if (World.IsPvP && Destination != null && Destination.DestLevelAreaSNO != 19947) return false; //unwanted portals in PvP hub
																														  //Logger.Debug(" (Reveal) portal {0} has location {1}", this.ActorSNO, this._position);
			if (Destination != null)
			{
				if (Destination.DestLevelAreaSNO == 168200 && World.SNO == WorldSno.caout_town) return false; //treasure room short portal
				if (Destination.DestLevelAreaSNO == 154588) return false;
				if (Destination.DestLevelAreaSNO == 83264) return false;
				if (Destination.DestLevelAreaSNO == 83265) return false;
				if (Destination.DestLevelAreaSNO == 161964) return false;
				if (Destination.DestLevelAreaSNO == 81178) return false;
				if (Destination.DestLevelAreaSNO == 210451 && !(World.Game.CurrentQuest == 121792 || World.Game.CurrentQuest == 57339)) return false;
				if (Destination.DestLevelAreaSNO == 19789 && World.SNO == WorldSno.a1trdun_level07) return false;
				if (Destination.WorldSNO == (int)WorldSno.x1_tristram_adventure_mode_hub && Destination.StartingPointActorTag == 483 && World.SNO == WorldSno.trout_town)
				{
					Destination.WorldSNO = (int)WorldSno.trout_town;
					Destination.StartingPointActorTag = 338;
				}
			}

			
			if (World.SNO == WorldSno.a3dun_crater_st_level04) //Heart of the Damned
				if (Position.X < 100.0f)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_level_02,
						DestLevelAreaSNO = 119305,
						StartingPointActorTag = 172
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level02,
						DestLevelAreaSNO = 80792,
						StartingPointActorTag = 171
					};

			if (World.SNO == WorldSno.a3dun_crater_st_level01b) //Tower of the Cursed lvl1
				if (Position.X > 300.0f)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level02b,
						DestLevelAreaSNO = 139274,
						StartingPointActorTag = 172
					};

			if (World.SNO == WorldSno.a3dun_crater_st_level02b) //Tower of the Cursed lvl2
				Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a3dun_crater_st_level01b,
					DestLevelAreaSNO = 119653,
					StartingPointActorTag = 171
				};

			if (World.SNO == WorldSno.a2dun_aqd_oasis_level00) //drowned passge portals
				if (Position.Y > 200.0f)
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 95
					};
				else
					Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_oasis_level01,
						DestLevelAreaSNO = 192689,
						StartingPointActorTag = 96
					};
			if (Destination == null || Destination.WorldSNO == (int)WorldSno.__NONE || Destination.StartingPointActorTag > 500)
			{
				if (Smart == true)
					Destination = SmartExitGenerate();
				{
					var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
					var scenes = World.QuadTree.Query<Scene>(proximity);
					if (scenes.Count == 0) return false; // cork (is it real?)

					var scene = scenes[0]; // Parent scene /fasbat

					if (scenes.Count == 2) // What if it's a subscene?
					{
						if (scenes[1].ParentChunkID != 0xFFFFFFFF)
							scene = scenes[1];
					}

					if (World.worldData.DynamicWorld)
						if (scene.TileType == 300)
							if (World.NextLocation.WorldSNO != (int)WorldSno.__NONE)
								Destination = World.NextLocation;
							else if (World.PrevLocation.WorldSNO != (int)WorldSno.__NONE)
								Destination = World.PrevLocation;
							else
							{
								if (World.PrevLocation.WorldSNO != (int)WorldSno.__NONE)
									Destination = World.PrevLocation;
							}
				}
			}


			//if (this.Destination == null || this.Destination.DestLevelAreaSNO == -1) this.Destination = SmartExitGenerate(); //return false;
			if (Destination.WorldSNO == (int)WorldSno.a3dun_hub_adria_tower_intro && World.Game.CurrentQuest == 101758) return false;

			if (!base.Reveal(player))
				return false;

			player.InGameClient.SendMessage(new PortalSpecifierMessage()
			{
				ActorID = DynamicID(player),
				Destination = Destination
			});

			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName(string.Format("{0}-{1}", Name, GlobalID)),
				Place = new WorldPlace { Position = Position, WorldID = World.GlobalID },
				ImageInfo = MinimapIcon,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = (int)SNO,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = false,
				PlayerUseFlags = 0
			});

			return true;
		}
		public World GetSmartWorld(int world)
		{
			return null;
		}
		public StartingPoint GetSmartStartingPoint(World world)
		{
			if (Destination.StartingPointActorTag != 0)
			{
				StartingPoint NeededStartingPoint = world.GetStartingPointById(Destination.StartingPointActorTag);
				var DestWorld = world.Game.GetWorld((WorldSno)Destination.WorldSNO);
				var StartingPoints = DestWorld.GetActorsBySNO(ActorSno._start_location_0);

				foreach (var ST in StartingPoints) if (ST.CurrentScene.SceneSNO.Id == Destination.StartingPointActorTag)
						NeededStartingPoint = (ST as StartingPoint);

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
			if (Destination.WorldSNO == (int)World.Game.WorldOfPortalNephalemSec)
			{
				Destination.StartingPointActorTag = 172;
			}
			var doors = GetActorsInRange<Door>(10f).Where(d => d.Visible);
			if (SNO == ActorSno._p2_totallynotacowlevel_portal && Destination.WorldSNO != (int)WorldSno.p2_totallynotacowlevel)
			{
				Destination.WorldSNO = (int)WorldSno.p1_tgoblin_realm;
				Destination.StartingPointActorTag = 171;
			}
			Logger.Warn("(OnTargeted) Portal has been activated, Id: {0}, LevelArea: {1}, World: {2}", SNO, Destination.DestLevelAreaSNO, Destination.WorldSNO);
			if (Destination.WorldSNO != (int)WorldSno.trout_town && Destination.WorldSNO != (int)WorldSno.x1_tristram_adventure_mode_hub)
				foreach (var door in doors)
					if (!door.isOpened)
						return;
			//return;
			if (Destination.WorldSNO != (int)WorldSno.__NONE)
				player.InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
			if (World.IsPvP)
				Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
			var world = World.Game.GetWorld((WorldSno)Destination.WorldSNO);

			if (Destination.DestLevelAreaSNO == 288482 && World.Game.ActiveNephalemTimer == false && World.Game.NephalemGreater == false)
			{
				
				foreach (var plr in World.Game.Players.Values)
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.TimedEventStartedMessage()
					{
						Event = new ActiveEvent()
						{
							snoTimedEvent = 0x0005D6EA,
							StartTime = World.Game.TickCounter,
							ExpirationTime = World.Game.TickCounter + 54000,
							ArtificiallyElapsedTime = 0
						}
					});
				//*/
				World.Game.ActiveNephalemTimer = true;
				player.StartConversation(world, 330142);
			} 
			else if (Destination.DestLevelAreaSNO == 288482 && World.Game.ActiveNephalemTimer == false && World.Game.NephalemGreater == true)
			{
				foreach (var plr in World.Game.Players.Values)
				{
					plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.SimpleMessage92) { });
					
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.TimedEventStartedMessage()
					{
						Event = new ActiveEvent()
						{
							snoTimedEvent = 0x0005D6EA,
							StartTime = World.Game.TickCounter,
							ExpirationTime = World.Game.TickCounter + 54000,
							ArtificiallyElapsedTime = 0
						}
					});

					plr.InGameClient.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
					{
						Field0 = 0x0005D6EA
					});
				}
				World.Game.ActiveNephalemTimer = true;
				player.StartConversation(world, 0x0005099E);
				if (World.Game.TiredRiftTimer == null)
					World.Game.TiredRiftTimer = new TickerSystem.SecondsTickTimer(World.Game, 900.0f);

			}


			if (world == null)
			{
				world = GetSmartWorld(Destination.WorldSNO);
			}

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", Destination.WorldSNO);
				return;
			}
			Logger.Info("World - {0} - {1}", world.SNO, world.WorldSNO.Name);

			var startingPoint = world.GetStartingPointById(Destination.StartingPointActorTag);
			if (startingPoint == null)
				startingPoint = GetSmartStartingPoint(world);
			if (startingPoint != null)
			{
				if (SNO == ActorSno._a2dun_zolt_portal_timedevent) //a2 timed event
				{
					if (!World.Game.QuestManager.SideQuests[120396].Completed)
						player.ShowConfirmation(DynamicID(player), (() => {
							player.ChangeWorld(world, startingPoint);
						}));
				}
				else
				{
					if (world == World)
						player.Teleport(startingPoint.Position);
					else
						player.ChangeWorld(world, startingPoint);
				}

				if (World.Game.QuestProgress.QuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.QuestProgress.QuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (World.Game.SideQuestProgress.QuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.SideQuestProgress.QuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.SideQuestProgress.GlobalQuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(World); // launch a questEvent
							World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(Destination.DestLevelAreaSNO);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				foreach (var bounty in World.Game.QuestManager.Bounties)
					bounty.CheckLevelArea(Destination.DestLevelAreaSNO);
			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", Destination.StartingPointActorTag);
		}
	}
}
