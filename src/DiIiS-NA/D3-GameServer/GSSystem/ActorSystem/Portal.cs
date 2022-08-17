//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
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

			if (this.World.WorldSNO.Id == 72636 || this.World.WorldSNO.Id == 72637)
			{
				this.Destination = SmartExitGenerate();
			}
			
			else if (this.World.WorldSNO.Id == 62751) //portal Adria's Hut
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 71150,
					DestLevelAreaSNO = 101351,
					StartingPointActorTag = 8
				};

			else if (this.World.WorldSNO.Id == 339151) //portal Noble's Rest Courtyard
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 338944,
					DestLevelAreaSNO = 338946,
					StartingPointActorTag = 171
				};

			else if(this.ActorSNO.Id == 175999 && this.World.WorldSNO.Id == 94676) //portal Leoric Jail -> 3rd lvl Halls of Agony
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 58983,
					DestLevelAreaSNO = 19776,
					StartingPointActorTag = 172
				};

			else if(this.ActorSNO.Id == 176002 && this.World.WorldSNO.Id == 92126) //portal advisor's tomb -> 2nd lvl Crypt
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 60600,
					DestLevelAreaSNO = 60601,
					StartingPointActorTag = 171
				};

			else if(this.World.WorldSNO.Id == 50657) //portal conclave -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 70885,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 201
				};

			else if(this.World.WorldSNO.Id == 51270) //portal altar -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 70885,
					DestLevelAreaSNO = 19839,
					StartingPointActorTag = 194
				};

			else if(this.World.WorldSNO.Id == 60432) //portal Khamsin HQ -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 70885,
					DestLevelAreaSNO = 63666,
					StartingPointActorTag = 70
				};

			else if(this.World.WorldSNO.Id == 59486 && this.Destination == null) //portal Khamsin HQ -> Stinging winds
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 59486,
					DestLevelAreaSNO = 62752,
					StartingPointActorTag = 86
				};

			else if(this.World.WorldSNO.Id == 80589) //portal ZKShadow
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 50613,
					DestLevelAreaSNO = 19800,
					StartingPointActorTag = 145
				};

			#region Спуск на второй уровень в подземелье на кладбище
			else if (this.World.WorldSNO.Id == 154587 && this.ActorSNO.Id == 176002) //Crypt A1 Q3
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
						WorldSNO = 60600,
						DestLevelAreaSNO = 60601,
						StartingPointActorTag = 15
					};
				}
			}
			else if (this.World.WorldSNO.Id == 60600)
			{
				var Portal = world.GetActorBySNO(176002);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 154587,
						DestLevelAreaSNO = 145182,
						StartingPointActorTag = 171
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 92126,
						DestLevelAreaSNO = 102362,
						StartingPointActorTag = 172
					};
				}
				

			}
			#endregion

			#region 2 Этаж собора
			if (world.WorldSNO.Id == 50582 && this.ActorSNO.Id == 176001)
			{
				var Portal = world.GetActorBySNO(176001);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 105406,
						DestLevelAreaSNO = 87907,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 60713,
						DestLevelAreaSNO = 60714,
						StartingPointActorTag = 15 //30
					};
				}
			}
			#endregion
			#region 3 Этаж собора
			if (world.WorldSNO.Id == 105406 && this.ActorSNO.Id == 176001)
			{
				var Portal = world.GetActorBySNO(176001);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 50582,
						DestLevelAreaSNO = 19783,//19785 - 4 уровень собора
						StartingPointActorTag = 171
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 50584,
						DestLevelAreaSNO = 19785,
						StartingPointActorTag = 172
					};
				}

			}
			#endregion
			#region 4 Этаж собора
			if (world.WorldSNO.Id == 50584 && this.ActorSNO.Id == 176001)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 105406,
					DestLevelAreaSNO = 87907,
					StartingPointActorTag = 171
				};
			}
			if (world.WorldSNO.Id == 50584 && this.ActorSNO.Id == 175467)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 50585,
					DestLevelAreaSNO = 19787,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Первый этаж Агонии
			//Вход
			else if (world.WorldSNO.Id == 2826 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 75049,
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
			else if (world.WorldSNO.Id == 2826 && this.ActorSNO.Id == 175482)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 58982,
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
			else if (world.WorldSNO.Id == 58982 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 2826,
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
			else if (world.WorldSNO.Id == 87707 && this.ActorSNO.Id == 176001 && this.NumberInWorld == 1)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 58982,
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
			else if (world.WorldSNO.Id == 94676 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 58983,
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
			else if (world.WorldSNO.Id == 58983 && this.ActorSNO.Id == 175999)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 94676,
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
			if (world.WorldSNO.Id == 62779 && this.ActorSNO.Id == 176007)
			{
				var Portal = world.GetActorBySNO(176007);
				if (Portal == null)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 59486,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 93
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 59486,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 87
					};
				}

			}
			#endregion

			#region Нижние этажи крепости: Уровень 2
			if (world.WorldSNO.Id == 75434)
			{
				if (this.ActorSNO.Id == 176001)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 93104,
						DestLevelAreaSNO = 75436,
						StartingPointActorTag = 171
					};
				}
				else if (this.ActorSNO.Id == 175482)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 136415,
						DestLevelAreaSNO = 136448,
						StartingPointActorTag = 172
					};
				}
			}
			#endregion
			#region Нижние этажи крепости: Уровень 3
			else if (world.WorldSNO.Id == 136415)
			{
				if (this.ActorSNO.Id == 176001)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 75434,
						DestLevelAreaSNO = 93103,
						StartingPointActorTag = 171
					};
				}
				else if (this.ActorSNO.Id == 175482)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 103209,
						DestLevelAreaSNO = 111232,
						StartingPointActorTag = 172
					};
				}
			}
			//103209
			#endregion

			#region Ареатский кратер: Уровень 1
			else if (world.WorldSNO.Id == 81049)
			{
				if (this.World.GetActorsBySNO(176001).Count > 0)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 75434,
						DestLevelAreaSNO = 112580,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 79401,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 172
					};
			}
			#endregion
			#region Ареатский кратер: Уровень 1
			else if (world.WorldSNO.Id == 81049)
			{
				if (this.World.GetActorsBySNO(176001).Count > 0)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 75434,
						DestLevelAreaSNO = 112580,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 79401,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 172
					};
			}
			#endregion

			#region Башня обреченных: Уровень 1
			else if (world.WorldSNO.Id == 79401)
			{
				if (this.ActorSNO.Id == 204747)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 81049,
						DestLevelAreaSNO = 86080,
						StartingPointActorTag = 171
					};
			}
			#endregion

			#region Башня обреченных: Уровень 2
			else if (world.WorldSNO.Id == 80763)
			{
				if (this.World.GetActorsBySNO(176001).Count > 0)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 79401,
						DestLevelAreaSNO = 80791,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 85201,
						DestLevelAreaSNO = 85202,
						StartingPointActorTag = 172
					};
			}
			#endregion

			#region Ареатский кратер: Уровень 2

			#endregion
			#region Башня проклятых: Уровень 1
			else if (world.WorldSNO.Id == 119641)
			{
				if (this.ActorSNO.Id == 204747)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 81934,
						DestLevelAreaSNO = 119305,
						StartingPointActorTag = 171
					};
			}
			#endregion

			#region Вход на первый этаж Садов
			//109143
			else if (world.WorldSNO.Id == 109143)
			{
				//if (this.ActorSNO.Id == 204747)
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 109513,
					DestLevelAreaSNO = 109514,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Пещера под колодцем
			else if (world.WorldSNO.Id == 230288)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 71150,
					DestLevelAreaSNO = 91133,
					StartingPointActorTag = 148
				};
			}
			#endregion

			#region Демонический разлом на первом этаже Садов Надежды.
			else if (world.WorldSNO.Id == 109513 && this.ActorSNO.Id == 224890)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 109525,//tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = 109526,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Демонический разлом на втором этаже Садов Надежды.
			else if (world.WorldSNO.Id == 219659 && this.ActorSNO.Id == 224890)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 109530,//tags[MarkerKeys.DestinationWorld].Id,
					DestLevelAreaSNO = 109526,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Выход на первый этаж садов
			//109143
			else if (world.WorldSNO.Id == 109525)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 109513,
					DestLevelAreaSNO = 109514,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Выход на второй этаж садов
			//109143
			else if (world.WorldSNO.Id == 109530)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 219659,
					DestLevelAreaSNO = 109516,
					StartingPointActorTag = 172
				};
			}
			#endregion

			#region Шпиль
			//1 Этаж
			else if (world.WorldSNO.Id == 121579)
			{
				if (this.ActorSNO.Id == 211300) //Выход
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 198281,
						DestLevelAreaSNO = 198564,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 214956,
						DestLevelAreaSNO = 215396,
						StartingPointActorTag = 172
					};
			}
			//Пролёт 
			else if (world.WorldSNO.Id == 214956)
			{
				if (world.Portals.Count == 0) //Выход
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 121579,
						DestLevelAreaSNO = 109538,
						StartingPointActorTag = 171
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 129305,
						DestLevelAreaSNO = 109540,
						StartingPointActorTag = 172
					};
			}
			//2 Этаж
			else if (world.WorldSNO.Id == 129305)
			{
				if (world.Portals.Count == 0)
					this.Destination = new ResolvedPortalDestination
					{
						//TODO: 
						WorldSNO = 205399,
						DestLevelAreaSNO = 205434,
						StartingPointActorTag = 172
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 214956,
						DestLevelAreaSNO = 215396,
						StartingPointActorTag = 171
					};
			}

			#endregion

			#region Зона гоблинсов =)
			else if (this.ActorSNO.Id == 393030)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 379962,
					DestLevelAreaSNO = 378681,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Эвент - Старый тристрам

			#region 1 Этаж - собор
			else if (world.WorldSNO.Id == 452721 && this.ActorSNO.Id == 176001)
			{
				if (this.ActorSNO.Id == 176001)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452922,
							DestLevelAreaSNO = 452988,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 455282,
							DestLevelAreaSNO = 455466,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 2 Этаж - собор
			else if (world.WorldSNO.Id == 452922 && this.ActorSNO.Id == 176001)
			{
				if (this.ActorSNO.Id == 176001)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452984,
							DestLevelAreaSNO = 452989,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452721,
							DestLevelAreaSNO = 452986,
							StartingPointActorTag = 171
						};
					}

			}
			#endregion
			#region 3 Этаж - собор
			else if (world.WorldSNO.Id == 452984 && this.ActorSNO.Id == 176001)
			{
				if (this.ActorSNO.Id == 176001)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452985,
							DestLevelAreaSNO = 452990,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452922,
							DestLevelAreaSNO = 452988,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 4 Этаж - собор
			else if (world.WorldSNO.Id == 452985 && this.ActorSNO.Id == 176001)
			{
				if (this.ActorSNO.Id == 176001)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452991,
							DestLevelAreaSNO = 452992,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452984,
							DestLevelAreaSNO = 452989,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 5 Этаж - катакомбы
			else if (world.WorldSNO.Id == 452991 && this.ActorSNO.Id == 341572)
			{
				if (this.ActorSNO.Id == 341572)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452996,
							DestLevelAreaSNO = 452993,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452985,
							DestLevelAreaSNO = 452990,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 6 Этаж - катакомбы
			else if (world.WorldSNO.Id == 452996 && this.ActorSNO.Id == 341572)
			{
				if (this.ActorSNO.Id == 341572)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452997,
							DestLevelAreaSNO = 452994,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452991,
							DestLevelAreaSNO = 452992,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 7 Этаж - катакомбы
			else if (world.WorldSNO.Id == 452997 && this.ActorSNO.Id == 341572)
			{
				if (this.ActorSNO.Id == 341572)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452998,
							DestLevelAreaSNO = 452995,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452996,
							DestLevelAreaSNO = 452993,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#region 8 Этаж - катакомбы
			else if (world.WorldSNO.Id == 452998 && this.ActorSNO.Id == 341572)
			{
				if (this.ActorSNO.Id == 341572)
					if (this.NumberInWorld == 1)
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452999,
							DestLevelAreaSNO = 453001,
							StartingPointActorTag = 172
						};
					}
					else
					{
						this.Destination = new ResolvedPortalDestination
						{
							WorldSNO = 452997,
							DestLevelAreaSNO = 452994,
							StartingPointActorTag = 171
						};
					}
			}
			#endregion
			#endregion


			#region Старый монастырь
			else if (world.WorldSNO.Id == 430335)
			{
				if (this.ActorSNO.Id == 175467 && this.World.GetActorsBySNO(175467).Count == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 428493,
						DestLevelAreaSNO = 428494,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region Руины Сечерона
			else if (world.WorldSNO.Id == 428493)
			{
				if (this.ActorSNO.Id == 175467 && this.World.GetActorsBySNO(175467).Count == 1)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 430335,
						DestLevelAreaSNO = 430336,
						StartingPointActorTag = 172
					};
				}
			}
			#endregion
			#region Вечный лес
			else if (world.WorldSNO.Id == 444305)
			{
				if (this.ActorSNO.Id == 176002 && this.World.GetActorsBySNO(176002).Count == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 428493,
						DestLevelAreaSNO = 428494,
						StartingPointActorTag = 615
					};
				}
			}
			#endregion

			#region 5 Акт
			#region Торговый квартал Вестмарша
			else if (world.WorldSNO.Id == 261712 && this.ActorSNO.Id == 176002)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 304235,
						DestLevelAreaSNO = 270011,
						StartingPointActorTag = 466
					};
				}
			}
			#endregion
			#region Кладбище Бриартон
			else if (world.WorldSNO.Id == 338944 && this.ActorSNO.Id == 176002)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 338891,
						DestLevelAreaSNO = 338956,
						StartingPointActorTag = 171
					};
				}
			}
			//338946
			#endregion
			#region Верхний Вестамарш
			else if (world.WorldSNO.Id == 263494 && this.ActorSNO.Id == 176001)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 304235,
						DestLevelAreaSNO = 270011,
						StartingPointActorTag = 442
					};
				}
			}
			//338946
			#endregion
			#region Проход к Корвусу
			//Выход - W:267412 A: 258142 P: 171

			else if (world.WorldSNO.Id == 283552 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 283566,
						DestLevelAreaSNO = 283567,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 267412,
						DestLevelAreaSNO = 258142,
						StartingPointActorTag = 171
					};
				}
			}
			//Второй уровень
			else if (world.WorldSNO.Id == 283566 && this.ActorSNO.Id == 341572)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 283552,
						DestLevelAreaSNO = 283553,
						StartingPointActorTag = 171
					};
				}
			}
			#endregion
			#region Крепость пандемония. Уровень 1
			else if (world.WorldSNO.Id == 271233 && this.ActorSNO.Id == 176007)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 271235,
						DestLevelAreaSNO = 360494,
						StartingPointActorTag = 172
					};
				}
				else
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 295225,
						DestLevelAreaSNO = 295228,
						StartingPointActorTag = 171
					};
				}
			}
			else if (world.WorldSNO.Id == 271235 && this.ActorSNO.Id == 365112)
			{
				if (this.NumberInWorld == 0)
				{
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 271233,
						DestLevelAreaSNO = 271234,
						StartingPointActorTag = 171
					};
				}

			}
			#endregion
			#endregion
			#region Сокровищница
			else if (this.ActorSNO.Id == 393030)
			{
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 379962,
					DestLevelAreaSNO = 378681,
					StartingPointActorTag = 172
				};
			}
			#endregion
			#region Умное вычисление выхода
			if (this.Destination == null)
			{
				//102231 - Пустыня
				Logger.Warn("Портал - {0} Не определён до конца, исполнение функции ''умного'' вычисления для выхода.", this.ActorSNO.Id);
				Smart = true;

				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = -1,
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
			if (this.World.WorldSNO.Name.ToLower().Contains("x1_lr_tileset"))
			{
				if (this.World.WorldSNO.Id == this.World.Game.WorldOfPortalNephalem)
				{
					//Вход 1 этаж
					if(this.CurrentScene.SceneSNO.Name.ToLower().Contains("entr"))
						return new ResolvedPortalDestination
						{
							WorldSNO = 332336,
							DestLevelAreaSNO = 332339,
							StartingPointActorTag = 24
						};
					//Выход на второй этаж
					else
						return new ResolvedPortalDestination
						{
							WorldSNO = this.World.Game.WorldOfPortalNephalemSec,
							DestLevelAreaSNO = 288684,
							StartingPointActorTag = 172
						};
				}
				
				return new ResolvedPortalDestination
				{
					WorldSNO = this.World.Game.WorldOfPortalNephalem,
					DestLevelAreaSNO = 288482,
					StartingPointActorTag = 171
				};
				
			}
			else
			{
				if (this.World.Game.Players.Count > 0)
				{
					if (this.World.Game.Players.First().Value.CurrentScene.Specification.SNOLevelAreas[1] != -1)
						LevelArea = this.World.Game.Players.First().Value.CurrentScene.Specification.SNOLevelAreas[1];
					else
						LevelArea = this.World.Game.Players.First().Value.CurrentScene.Specification.SNOLevelAreas[0];

					if (this.World.Game.Players.First().Value.GetActorsInRange<StartingPoint>(20f).Count > 0)
						BackPoint = (this.World.Game.Players.First().Value.GetActorsInRange<StartingPoint>(20f).First() as StartingPoint).TargetId;

					return new ResolvedPortalDestination
					{
						WorldSNO = this.World.Game.Players.First().Value.World.WorldSNO.Id,
						DestLevelAreaSNO = LevelArea,
						StartingPointActorTag = BackPoint
					};
				}
				else
				{
					return new ResolvedPortalDestination
					{
						WorldSNO = -1,
						DestLevelAreaSNO = LevelArea,
						StartingPointActorTag = -1
					};
				}
			}
		}
		public override void EnterWorld(Vector3D position)
		{
			base.EnterWorld(position);
			if (this.World.WorldSNO.Id == 70885 || this.World.WorldSNO.Id == 95804)
			{
				var portals = this.GetActorsInRange<Portal>(5f).Where(p => p.Destination != null && p.Destination.DestLevelAreaSNO != -1).ToList();
				if (portals.Count >= 2)
				{
					var random_portal = portals[FastRandom.Instance.Next(portals.Count)];
					var bounty_portals = this.World.Game.QuestManager.Bounties.Where(b => !b.PortalSpawned).SelectMany(b => b.LevelAreaChecks).Intersect(portals.Select(p => p.Destination.DestLevelAreaSNO));
					if (bounty_portals.Count() > 0)
					{
						random_portal = portals.Where(p => this.World.Game.QuestManager.Bounties.SelectMany(b => b.LevelAreaChecks).Where(w => w != -1).Contains(p.Destination.DestLevelAreaSNO)).First();
						this.World.Game.QuestManager.Bounties.Where(b => b.LevelAreaChecks.Contains(random_portal.Destination.DestLevelAreaSNO)).First().PortalSpawned = true;
					}
					foreach (var portal in portals)
						portal.randomed = false;
					random_portal.randomed = true;
				}
			}

			if (this.Destination == null || this.Destination.WorldSNO == -1)
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
							WorldSNO = 81049,
							DestLevelAreaSNO = 86080,
							StartingPointActorTag = 171
						};

				if (scene.SceneSNO.Id == 335727) //Gideon's Row entrance
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 261712,
						DestLevelAreaSNO = 261758,
						StartingPointActorTag = 171
					};
				if (scene.SceneSNO.Id == 335742) //Gideon's Row exit
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 338944,
						DestLevelAreaSNO = 338946,
						StartingPointActorTag = 172
					};

				if (this.World.PortalOverrides.ContainsKey(scene.SceneSNO.Id))
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = this.World.PortalOverrides[scene.SceneSNO.Id],
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
			
			if (this.ActorSNO.Id == 176002 && this.World.WorldSNO.Id == 154587)
			{
				//this.Destination.WorldSNO = 
			}
			if (!this.randomed && this.Destination.DestLevelAreaSNO != 19794) return false;
			//if (this.ActorSNO.Id == 209083) return false; //pony level portal
			if (this.ActorSNO.Id == 175482 && this.World.WorldSNO.Id == 178152) return false; //armory a4 portal
			if (this.World.IsPvP && this.Destination != null && this.Destination.DestLevelAreaSNO != 19947) return false; //unwanted portals in PvP hub
																														  //Logger.Debug(" (Reveal) portal {0} has location {1}", this.ActorSNO, this._position);
			if (this.Destination != null)
			{
				if (this.Destination.DestLevelAreaSNO == 168200 && this.World.WorldSNO.Id == 70885) return false; //treasure room short portal
				if (this.Destination.DestLevelAreaSNO == 154588) return false;
				if (this.Destination.DestLevelAreaSNO == 83264) return false;
				if (this.Destination.DestLevelAreaSNO == 83265) return false;
				if (this.Destination.DestLevelAreaSNO == 161964) return false;
				if (this.Destination.DestLevelAreaSNO == 81178) return false;
				if (this.Destination.DestLevelAreaSNO == 210451 && !(this.World.Game.CurrentQuest == 121792 || this.World.Game.CurrentQuest == 57339)) return false;
				if (this.Destination.DestLevelAreaSNO == 19789 && this.World.WorldSNO.Id == 50585) return false;
			}

			if (this.World.WorldSNO.Id == 80763 && this.Destination.WorldSNO == 85201) //Tower of the Damned lvl2
				this.Destination.StartingPointActorTag = 172;

			
			if (this.World.WorldSNO.Id == 85201) //Heart of the Damned
				if (this.Position.X < 100.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 81934,
						DestLevelAreaSNO = 119305,
						StartingPointActorTag = 172
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 80763,
						DestLevelAreaSNO = 80792,
						StartingPointActorTag = 171
					};

			if (this.World.WorldSNO.Id == 119641) //Tower of the Cursed lvl1
				if (this.Position.X > 300.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 139272,
						DestLevelAreaSNO = 139274,
						StartingPointActorTag = 172
					};

			if (this.World.WorldSNO.Id == 139272) //Tower of the Cursed lvl2
				this.Destination = new ResolvedPortalDestination
				{
					WorldSNO = 119641,
					DestLevelAreaSNO = 119653,
					StartingPointActorTag = 171
				};

			if (this.World.WorldSNO.Id == 192687) //drowned passge portals
				if (this.Position.Y > 200.0f)
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 59486,
						DestLevelAreaSNO = 62752,
						StartingPointActorTag = 95
					};
				else
					this.Destination = new ResolvedPortalDestination
					{
						WorldSNO = 192640,
						DestLevelAreaSNO = 192689,
						StartingPointActorTag = 96
					};
			if (this.Destination == null || this.Destination.WorldSNO == -1 || this.Destination.StartingPointActorTag > 500)
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
							if (this.World.NextLocation.WorldSNO != -1)
								this.Destination = this.World.NextLocation;
							else if (this.World.PrevLocation.WorldSNO != -1)
								this.Destination = this.World.PrevLocation;
							else
							{
								if (this.World.PrevLocation.WorldSNO != -1)
									this.Destination = this.World.PrevLocation;
							}
				}
			}


			//if (this.Destination == null || this.Destination.DestLevelAreaSNO == -1) this.Destination = SmartExitGenerate(); //return false;
			if (this.Destination.WorldSNO == 204707 && this.World.Game.CurrentQuest == 101758) return false;

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
				var DestWorld = world.Game.GetWorld(this.Destination.WorldSNO);
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
			if (this.Destination.WorldSNO == this.World.Game.WorldOfPortalNephalemSec)
			{
				this.Destination.StartingPointActorTag = 172;
			}
			var doors = this.GetActorsInRange<Door>(10f).Where(d => d.Visible);
			if (this.ActorSNO.Id == 434659 & this.Destination.WorldSNO != 434649)
			{
				this.Destination.WorldSNO = 379962;
				this.Destination.StartingPointActorTag = 171;
			}
			Logger.Warn("(OnTargeted) Portal has been activated, Id: {0}, LevelArea: {1}, World: {2}", this.ActorSNO.Id, this.Destination.DestLevelAreaSNO, this.Destination.WorldSNO);
			if (this.Destination.WorldSNO != 71150 && this.Destination.WorldSNO != 332336)
				foreach (var door in doors)
					if (!door.isOpened)
						return;
			//return;
			if (Destination.WorldSNO != -1)
				player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.SimpleMessage(Opcodes.LoadingWarping));
			if (this.World.IsPvP)
				this.Destination.WorldSNO = 332336;
			var world = this.World.Game.GetWorld(this.Destination.WorldSNO);

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
			Logger.Info("World - {0} - {1}", world.WorldSNO.Id, world.WorldSNO.Name);

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
