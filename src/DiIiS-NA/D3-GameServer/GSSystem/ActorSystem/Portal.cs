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

		public Portal(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			if (this.World != null)
				if (this.World.GetActorsBySNO(this.ActorSNO.Id).Count > 0)
				{
					int count = this.World.GetActorsBySNO(this.ActorSNO.Id).Count;
					NumberInWorld = count;
				}
			try
			{
				//Logger.Debug("Portal {0} has destination world {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationWorld].Id);
				//Logger.Debug("Portal {0} has destination LevelArea {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationLevelArea].Id);
				//Logger.Info("Portal {0} has destination StartingPoint {1}", this.ActorSNO.Id, tags[MarkerKeys.DestinationActorTag]);
				this.Destination = new ResolvedPortalDestination
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
				Logger.Trace("Portal {0} has incomplete definition", this.ActorSNO.Id);
			}

			if (this.World.SNO == WorldSno.trdun_crypt_falsepassage_01 || this.World.SNO == WorldSno.trdun_crypt_falsepassage_02)
			{
				this.Destination = SmartExitGenerate();
			}
			

			else if (this.World.SNO == WorldSno.trout_adriascellar) //portal Adria's Hut
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trout_town,
					DestLevelAreaSNO = 101351,
					StartingPointActorTag = 8
				};

			else if (this.World.SNO == WorldSno.x1_westm_deathorb_kerwinsrow) //portal Noble's Rest Courtyard
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.x1_westm_graveyard_deathorb,
					DestLevelAreaSNO = 338946,
					StartingPointActorTag = 171
				};

			else if(this.ActorSNO.Id == 175999 && this.World.SNO == WorldSno.trdun_jail_level01) //portal Leoric Jail -> 3rd lvl Halls of Agony
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_leoric_level03,
					DestLevelAreaSNO = 19776,
					StartingPointActorTag = 172
				};

			else if(this.ActorSNO.Id == 176002 && this.World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_02) //portal advisor's tomb -> 2nd lvl Crypt
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_01,
					DestLevelAreaSNO = 60601,
					StartingPointActorTag = 171
				};

			else if(this.World.SNO == WorldSno.caout_interior_d) //portal conclave -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 201
				};

			else if(this.World.SNO == WorldSno.caout_interior_f) //portal altar -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 194
				};

			else if(this.World.SNO == WorldSno.caout_khamsin_mine) //portal Khamsin HQ -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.caout_town,
					DestLevelAreaSNO = 63666,
					StartingPointActorTag = 70
				};

			else if(this.World.SNO == WorldSno.a2dun_aqd_special_01 && this.Destination == null) //portal Khamsin HQ -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
					DestLevelAreaSNO = 62752,
					StartingPointActorTag = 86
				};

			else if(this.World.SNO == WorldSno.a2dun_zolt_shadowrealm_level01) //portal ZKShadow
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a2dun_zolt_lobby,
					DestLevelAreaSNO = 19800,
					StartingPointActorTag = 145
				};

			#region Спуск на второй уровень в подземелье на кладбище
			else if (this.World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_00 && this.ActorSNO.Id == 176002) //Crypt A1 Q3
			{
				var Portal = world.GetActorBySNO(176002);
				if (Portal == null)
				{
					this.Destination = SmartExitGenerate();
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_01,
						DestLevelAreaSNO = 60601,
						StartingPointActorTag = 15
					};
				}
			}
			else if (this.World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_01)
			{
				var Portal = world.GetActorBySNO(176002);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_00,
						DestLevelAreaSNO = 145182,
						StartingPointActorTag = 171
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_02,
						DestLevelAreaSNO = 102362,
						StartingPointActorTag = 172
					};
				}
				

			}
			#endregion

			#region 2 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level04 && this.ActorSNO.Id == 176001)
			{
				var Portal = world.GetActorBySNO(176001);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level05_templar,
						DestLevelAreaSNO = 87907,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.trdun_cain_intro,
						DestLevelAreaSNO = 60714,
						StartingPointActorTag = 15 //30
					};
				}
			}
			#endregion
			#region 3 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level05_templar && this.ActorSNO.Id == 176001)
			{
				var Portal = world.GetActorBySNO(176001);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level04,
						DestLevelAreaSNO = 19783,//19785 - 4 уровень собора
						StartingPointActorTag = 171
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a1trdun_level06,
						DestLevelAreaSNO = 19785,
						StartingPointActorTag = 172
					};
				}

			}
			#endregion
			#region 4 Этаж собора
			if (world.SNO == WorldSno.a1trdun_level06 && this.ActorSNO.Id == 176001)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a1trdun_level05_templar,
					DestLevelAreaSNO = 87907,
					StartingPointActorTag = 171
				};
			}
			if (world.SNO == WorldSno.a1trdun_level06 && this.ActorSNO.Id == 175467)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a1trdun_level07,
					DestLevelAreaSNO = 19787,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Первый этаж Агонии
			//Вход
			else if (world.SNO == WorldSno.trdun_leoric_level01 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.trdun_leoric_level01 && this.ActorSNO.Id == 175482)
			{
				this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.trdun_leoric_level02 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.trout_highlands_dunexteriora && this.ActorSNO.Id == 176001 && this.NumberInWorld == 1)
			{
				this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.trdun_jail_level01 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.trdun_leoric_level03 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
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
			if (world.SNO == WorldSno.a2dun_aqd_special_b && this.ActorSNO.Id == 176007)
			{
				var Portal = world.GetActorBySNO(176007);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 93
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 176001)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level03,
						DestLevelAreaSNO = 75436,
						StartingPointActorTag = 171
					};
				}
				else if (this.ActorSNO.Id == 175482)
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 176001)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level04,
						DestLevelAreaSNO = 93103,
						StartingPointActorTag = 171
					};
				}
				else if (this.ActorSNO.Id == 175482)
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.World.GetActorsBySNO(176001).Count > 0)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_keep_level04,
						DestLevelAreaSNO = 112580,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 204747)
					this.Destination = new ResolvedPortalDestination
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
				if (this.World.GetActorsBySNO(176001).Count > 0)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level01,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 204747)
					this.Destination = new ResolvedPortalDestination
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
				this.Destination = new ResolvedPortalDestination
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
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.trout_town,
					DestLevelAreaSNO = 91133,
					StartingPointActorTag = 148
				};
			}
			#endregion

			#region Демонический разлом на первом этаже Садов Надежды.
			else if (world.SNO == WorldSno.a4dun_garden_of_hope_01 && this.ActorSNO.Id == 224890)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a4dun_hell_portal_01,//tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = 109526,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Демонический разлом на втором этаже Садов Надежды.
			else if (world.SNO == WorldSno.a4dun_garden_of_hope_random && this.ActorSNO.Id == 224890)
			{
				this.Destination = new ResolvedPortalDestination
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
				this.Destination = new ResolvedPortalDestination
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
				this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 211300) //Выход
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_level_00,
						DestLevelAreaSNO = 198564,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
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
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_level_01,
						DestLevelAreaSNO = 109538,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
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
					this.Destination = new ResolvedPortalDestination
					{
						//TODO: 
						WorldSNO = (int)WorldSno.a4dun_spire_diabloentrance,
						DestLevelAreaSNO = 205434,
						StartingPointActorTag = 172
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a4dun_spire_exterior,
						DestLevelAreaSNO = 215396,
						StartingPointActorTag = 171
					};
			}

			#endregion

			#region Сокровищница / Зона гоблинсов =)
			else if (this.ActorSNO.Id == 393030)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.p1_tgoblin_realm,
					DestLevelAreaSNO = 378681,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Эвент - Старый тристрам

			#region 1 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_01 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_02,
						DestLevelAreaSNO = 452988,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_oldtristram,
						DestLevelAreaSNO = 455466,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 2 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_02 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_03,
						DestLevelAreaSNO = 452989,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_01,
						DestLevelAreaSNO = 452986,
						StartingPointActorTag = 171
					};
				}

			}
			#endregion
			#region 3 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_03 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_04,
						DestLevelAreaSNO = 452990,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_02,
						DestLevelAreaSNO = 452988,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 4 Этаж - собор
			else if (world.SNO == WorldSno.p43_ad_cathedral_level_04 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_05,
						DestLevelAreaSNO = 452992,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_03,
						DestLevelAreaSNO = 452989,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 5 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_05 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_06,
						DestLevelAreaSNO = 452993,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_cathedral_level_04,
						DestLevelAreaSNO = 452990,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 6 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_06 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_07,
						DestLevelAreaSNO = 452994,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_05,
						DestLevelAreaSNO = 452992,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 7 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_07 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_08,
						DestLevelAreaSNO = 452995,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_catacombs_level_06,
						DestLevelAreaSNO = 452993,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region 8 Этаж - катакомбы
			else if (world.SNO == WorldSno.p43_ad_catacombs_level_08 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.p43_ad_caves_level_09,
						DestLevelAreaSNO = 453001,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 175467 && this.World.GetActorsBySNO(175467).Count == 0)
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 175467 && this.World.GetActorsBySNO(175467).Count == 1)
				{
					this.Destination = new ResolvedPortalDestination
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
				if (this.ActorSNO.Id == 176002 && this.World.GetActorsBySNO(176002).Count == 0)
				{
					this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.x1_westm_zone_01 && this.ActorSNO.Id == 176002)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westmarch_hub,
						DestLevelAreaSNO = 270011,
						StartingPointActorTag = 466
					};
				}
			}
			#endregion
			#region Кладбище Бриартон
			else if (world.SNO == WorldSno.x1_westm_graveyard_deathorb && this.ActorSNO.Id == 176002)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
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
			else if (world.SNO == WorldSno.x1_westm_zone_03 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
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

			else if (world.SNO == WorldSno.x1_catacombs_level01 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_catacombs_level02,
						DestLevelAreaSNO = 283567,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_bog_01,
						DestLevelAreaSNO = 258142,
						StartingPointActorTag = 171
					};
				}
			}
			//Второй уровень
			else if (world.SNO == WorldSno.x1_catacombs_level02 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_catacombs_level01,
						DestLevelAreaSNO = 283553,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region Крепость пандемония. Уровень 1
			else if (world.SNO == WorldSno.x1_fortress_level_01 && this.ActorSNO.Id == 176007)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_fortress_level_02,
						DestLevelAreaSNO = 360494,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_pand_batteringram,
						DestLevelAreaSNO = 295228,
						StartingPointActorTag = 171
					};
				}
			}
			else if (world.SNO == WorldSno.x1_fortress_level_02 && this.ActorSNO.Id == 365112)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
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
			if (this.Destination == null)
			{
				//102231 - Пустыня
				Logger.Warn("Портал - {0} Не определён до конца, исполнение функции ''умного'' вычисления для выхода.", this.ActorSNO.Id);
				Smart = true;

				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.__NONE,
					DestLevelAreaSNO = -1,
					StartingPointActorTag = -1
				};
				
			}
			#endregion
			this.Field2 = 0x9;//16;
		}
		public ResolvedPortalDestination SmartExitGenerate()
		{
			Logger.Warn("Portal - {0} Smart Генерация.", this.ActorSNO.Id);
			int LevelArea = 0;
			int BackPoint = -1;
			if (this.World.SNO.IsDungeon())
			{
				if (this.World.SNO == this.World.Game.WorldOfPortalNephalem)
				{
					//Вход 1 этаж
					if(this.CurrentScene.SceneSNO.Name.ToLower().Contains("entr"))
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
							WorldSNO = (int)this.World.Game.WorldOfPortalNephalemSec,
							DestLevelAreaSNO = 288684,
							StartingPointActorTag = 172
						};
				}
				
				return new ResolvedPortalDestination
				{
					WorldSNO = (int)this.World.Game.WorldOfPortalNephalem,
					DestLevelAreaSNO = 288482,
					StartingPointActorTag = 171
				};
				
			}
			else
			{
				if (!World.Game.Players.IsEmpty)
				{
                    var player = this.World.Game.Players.First().Value;
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
			if (this.World.SNO == WorldSno.caout_town || this.World.SNO == WorldSno.a3_battlefields_02)
			{
				var portals = this.GetActorsInRange<Portal>(5f).Where(p => p.Destination != null && p.Destination.DestLevelAreaSNO != -1).ToList();
				if (portals.Count >= 2)
				{
					var random_portal = portals[FastRandom.Instance.Next(portals.Count)];
					var bounty_portals = this.World.Game.QuestManager.Bounties.Where(b => !b.PortalSpawned).SelectMany(b => b.LevelAreaChecks).Intersect(portals.Select(p => p.Destination.DestLevelAreaSNO));
					if (bounty_portals.Any())
					{
						random_portal = portals.Where(p => this.World.Game.QuestManager.Bounties.SelectMany(b => b.LevelAreaChecks).Where(w => w != -1).Contains(p.Destination.DestLevelAreaSNO)).First();
						this.World.Game.QuestManager.Bounties.Where(b => b.LevelAreaChecks.Contains(random_portal.Destination.DestLevelAreaSNO)).First().PortalSpawned = true;
					}
					foreach (var portal in portals)
						portal.randomed = false;
					random_portal.randomed = true;
				}
			}

			if (this.Destination == null || this.Destination.WorldSNO == (int)WorldSno.__NONE)
			{
				var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
				var scenes = this.World.QuadTree.Query<Scene>(proximity);
				if (scenes.Count == 0) return;

				var scene = scenes[0]; // Parent scene /fasbat

				if (scene.Specification == null) return;

				if (scene.Specification.SNONextWorld != -1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = scene.Specification.SNONextWorld,
						DestLevelAreaSNO = scene.Specification.SNONextLevelArea,
						StartingPointActorTag = scene.Specification.NextEntranceGUID
					};
				}

				if (scene.SceneSNO.Id == 129430)
					if (this.Position.Y < 100.0f)
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = (int)WorldSno.a3dun_crater_level_01,
							DestLevelAreaSNO = 86080,
							StartingPointActorTag = 171
						};

				if (scene.SceneSNO.Id == 335727) //Gideon's Row entrance
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westm_zone_01,
						DestLevelAreaSNO = 261758,
						StartingPointActorTag = 171
					};
				if (scene.SceneSNO.Id == 335742) //Gideon's Row exit
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.x1_westm_graveyard_deathorb,
						DestLevelAreaSNO = 338946,
						StartingPointActorTag = 172
					};

				if (this.World.PortalOverrides.ContainsKey(scene.SceneSNO.Id))
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)this.World.PortalOverrides[scene.SceneSNO.Id],
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
			if (this.World.IsPvP && this.Destination != null && this.Destination.DestLevelAreaSNO == 19947) //spawn safe zone
			{
				var zone_actor = new PVPSafeZone(this.World, 275752, new TagMap());
				zone_actor.AdjustPosition = false;
				zone_actor.EnterWorld(this.Position);
				this.World.BuffManager.AddBuff(zone_actor, zone_actor, new PVPSafeZoneBuff());
			}
		}

		public override bool Reveal(Player player)
		{
			
			if (this.ActorSNO.Id == 176002 && this.World.SNO == WorldSno.trdun_crypt_skeletonkingcrown_00)
			{
				//this.Destination.WorldSNO = 
			}
			if (!this.randomed && this.Destination.DestLevelAreaSNO != 19794) return false;
			//if (this.ActorSNO.Id == 209083) return false; //pony level portal
			if (this.ActorSNO.Id == 175482 && this.World.SNO == WorldSno.a4dun_heaven_hub_keep) return false; //armory a4 portal
			if (this.World.IsPvP && this.Destination != null && this.Destination.DestLevelAreaSNO != 19947) return false; //unwanted portals in PvP hub
																														  //Logger.Debug(" (Reveal) portal {0} has location {1}", this.ActorSNO, this._position);
			if (this.Destination != null)
			{
				if (this.Destination.DestLevelAreaSNO == 168200 && this.World.SNO == WorldSno.caout_town) return false; //treasure room short portal
				if (this.Destination.DestLevelAreaSNO == 154588) return false;
				if (this.Destination.DestLevelAreaSNO == 83264) return false;
				if (this.Destination.DestLevelAreaSNO == 83265) return false;
				if (this.Destination.DestLevelAreaSNO == 161964) return false;
				if (this.Destination.DestLevelAreaSNO == 81178) return false;
				if (this.Destination.DestLevelAreaSNO == 210451 && !(this.World.Game.CurrentQuest == 121792 || this.World.Game.CurrentQuest == 57339)) return false;
				if (this.Destination.DestLevelAreaSNO == 19789 && this.World.SNO == WorldSno.a1trdun_level07) return false;
				if (this.Destination.WorldSNO == (int)WorldSno.x1_tristram_adventure_mode_hub && this.Destination.StartingPointActorTag == 483 && this.World.SNO == WorldSno.trout_town)
				{
					this.Destination.WorldSNO = (int)WorldSno.trout_town;
					this.Destination.StartingPointActorTag = 338;
				}
			}

			
			if (this.World.SNO == WorldSno.a3dun_crater_st_level04) //Heart of the Damned
				if (this.Position.X < 100.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_level_02,
						DestLevelAreaSNO = 119305,
						StartingPointActorTag = 172
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level02,
						DestLevelAreaSNO = 80792,
						StartingPointActorTag = 171
					};

			if (this.World.SNO == WorldSno.a3dun_crater_st_level01b) //Tower of the Cursed lvl1
				if (this.Position.X > 300.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a3dun_crater_st_level02b,
						DestLevelAreaSNO = 139274,
						StartingPointActorTag = 172
					};

			if (this.World.SNO == WorldSno.a3dun_crater_st_level02b) //Tower of the Cursed lvl2
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = (int)WorldSno.a3dun_crater_st_level01b,
					DestLevelAreaSNO = 119653,
					StartingPointActorTag = 171
				};

			if (this.World.SNO == WorldSno.a2dun_aqd_oasis_level00) //drowned passge portals
				if (this.Position.Y > 200.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_special_01,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 95
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = (int)WorldSno.a2dun_aqd_oasis_level01,
						DestLevelAreaSNO = 192689,
						StartingPointActorTag = 96
					};
			if (this.Destination == null || this.Destination.WorldSNO == (int)WorldSno.__NONE || this.Destination.StartingPointActorTag > 500)
			{
				if (Smart == true)
					this.Destination = SmartExitGenerate();
				{
					var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
					var scenes = this.World.QuadTree.Query<Scene>(proximity);
					if (scenes.Count == 0) return false; // cork (is it real?)

					var scene = scenes[0]; // Parent scene /fasbat

					if (scenes.Count == 2) // What if it's a subscene?
					{
						if (scenes[1].ParentChunkID != 0xFFFFFFFF)
							scene = scenes[1];
					}

					if (this.World.worldData.DynamicWorld)
						if (scene.TileType == 300)
							if (this.World.NextLocation.WorldSNO != (int)WorldSno.__NONE)
								this.Destination = this.World.NextLocation;
							else if (this.World.PrevLocation.WorldSNO != (int)WorldSno.__NONE)
								this.Destination = this.World.PrevLocation;
							else
							{
								if (this.World.PrevLocation.WorldSNO != (int)WorldSno.__NONE)
									this.Destination = this.World.PrevLocation;
							}
				}
			}


			//if (this.Destination == null || this.Destination.DestLevelAreaSNO == -1) this.Destination = SmartExitGenerate(); //return false;
			if (this.Destination.WorldSNO == (int)WorldSno.a3dun_hub_adria_tower_intro && this.World.Game.CurrentQuest == 101758) return false;

			if (!base.Reveal(player))
				return false;

			player.InGameClient.SendMessage(new PortalSpecifierMessage()
			{
				ActorID = this.DynamicID(player),
				Destination = this.Destination
			});

			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName(string.Format("{0}-{1}", this.ActorSNO.Name, this.GlobalID)),
				Place = new WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
				ImageInfo = MinimapIcon,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = this.ActorSNO.Id,
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
			if (this.Destination.StartingPointActorTag != 0)
			{
				StartingPoint NeededStartingPoint = world.GetStartingPointById(this.Destination.StartingPointActorTag);
				var DestWorld = world.Game.GetWorld((WorldSno)this.Destination.WorldSNO);
				var StartingPoints = DestWorld.GetActorsBySNO(5502);

				foreach (var ST in StartingPoints) if (ST.CurrentScene.SceneSNO.Id == this.Destination.StartingPointActorTag)
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
			if (this.Destination.WorldSNO == (int)this.World.Game.WorldOfPortalNephalemSec)
			{
				this.Destination.StartingPointActorTag = 172;
			}
			var doors = this.GetActorsInRange<Door>(10f).Where(d => d.Visible);
			if (this.ActorSNO.Id == 434659 & this.Destination.WorldSNO != (int)WorldSno.p2_totallynotacowlevel)
			{
				this.Destination.WorldSNO = (int)WorldSno.p1_tgoblin_realm;
				this.Destination.StartingPointActorTag = 171;
			}
			Logger.Warn("(OnTargeted) Portal has been activated, Id: {0}, LevelArea: {1}, World: {2}", this.ActorSNO.Id, this.Destination.DestLevelAreaSNO, this.Destination.WorldSNO);
			if (this.Destination.WorldSNO != (int)WorldSno.trout_town && this.Destination.WorldSNO != (int)WorldSno.x1_tristram_adventure_mode_hub)
				foreach (var door in doors)
					if (!door.isOpened)
						return;
			//return;
			if (Destination.WorldSNO != (int)WorldSno.__NONE)
				player.InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
			if (this.World.IsPvP)
				this.Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
			var world = this.World.Game.GetWorld((WorldSno)this.Destination.WorldSNO);

			if (this.Destination.DestLevelAreaSNO == 288482 && this.World.Game.ActiveNephalemTimer == false && this.World.Game.NephalemGreater == false)
			{
				
				foreach (var plr in this.World.Game.Players.Values)
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.TimedEventStartedMessage()
					{
						Event = new ActiveEvent()
						{
							snoTimedEvent = 0x0005D6EA,
							StartTime = this.World.Game.TickCounter,
							ExpirationTime = this.World.Game.TickCounter + 54000,
							ArtificiallyElapsedTime = 0
						}
					});
				//*/
				this.World.Game.ActiveNephalemTimer = true;
				player.StartConversation(world, 330142);
			} 
			else if (this.Destination.DestLevelAreaSNO == 288482 && this.World.Game.ActiveNephalemTimer == false && this.World.Game.NephalemGreater == true)
			{
				foreach (var plr in this.World.Game.Players.Values)
				{
					plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.SimpleMessage92) { });
					
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.TimedEventStartedMessage()
					{
						Event = new ActiveEvent()
						{
							snoTimedEvent = 0x0005D6EA,
							StartTime = this.World.Game.TickCounter,
							ExpirationTime = this.World.Game.TickCounter + 54000,
							ArtificiallyElapsedTime = 0
						}
					});

					plr.InGameClient.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
					{
						Field0 = 0x0005D6EA
					});
				}
				this.World.Game.ActiveNephalemTimer = true;
				player.StartConversation(world, 0x0005099E);
				if (this.World.Game.TiredRiftTimer == null)
					this.World.Game.TiredRiftTimer = new TickerSystem.SecondsTickTimer(this.World.Game, 900.0f);

			}


			if (world == null)
			{
				world = GetSmartWorld(this.Destination.WorldSNO);
			}

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", this.Destination.WorldSNO);
				return;
			}
			Logger.Info("World - {0} - {1}", world.SNO, world.WorldSNO.Name);

			var startingPoint = world.GetStartingPointById(this.Destination.StartingPointActorTag);
			if (startingPoint == null)
				startingPoint = GetSmartStartingPoint(world);
			if (startingPoint != null)
			{
				if (this.ActorSNO.Id == 230751) //a2 timed event
				{
					if (!this.World.Game.QuestManager.SideQuests[120396].Completed)
						player.ShowConfirmation(this.DynamicID(player), (() => {
							player.ChangeWorld(world, startingPoint);
						}));
				}
				else
				{
					if (world == this.World)
						player.Teleport(startingPoint.Position);
					else
						player.ChangeWorld(world, startingPoint);
				}

				if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.QuestProgress.QuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.SideQuestProgress.QuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (this.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.SideQuestProgress.GlobalQuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
							this.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(this.Destination.DestLevelAreaSNO);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				foreach (var bounty in this.World.Game.QuestManager.Bounties)
					bounty.CheckLevelArea(this.Destination.DestLevelAreaSNO);
			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", this.Destination.StartingPointActorTag);
		}
	}
}
