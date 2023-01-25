//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
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

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class OpenWorld : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public OpenWorld(Game game) : base(game)
		{
		}

		public override void SetQuests()
		{
			#region x1_OpenWorld_quest
			Game.QuestManager.Quests.Add(312429, new Quest { RewardXp = 1125, RewardGold = 370, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[312429].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					script = new CryptPortals();
					script.Execute(Game.GetWorld(WorldSno.trout_town));
                    Game.AddOnLoadWorldAction(WorldSno.a1dun_spidercave_02, () =>
                    {
						Game.GetWorld(WorldSno.a1dun_spidercave_02).SpawnMonster(ActorSno._spiderqueen, new Vector3D { X = 149.439f, Y = 121.452f, Z = 13.794f }); 
					});//spawn spider queen
					Game.AddOnLoadWorldAction(WorldSno.trdun_butcherslair_02, () => 
					{ 
						Game.GetWorld(WorldSno.trdun_butcherslair_02).SpawnMonster(ActorSno._butcher, new Vector3D { X = 93.022f, Y = 89.86f, Z = 0.1f }); 
					});//spawn Butcher
					Game.AddOnLoadWorldAction(WorldSno.a4dun_spire_exterior, () =>
					{
						Game.GetWorld(WorldSno.a4dun_spire_exterior).SpawnMonster(ActorSno._bigred_izual, new Vector3D { X = 585.439f, Y = 560.823f, Z = 0.1f }); 
					});//spawn Izual
					//this.Game.AddOnLoadAction(109984, () => { foreach (var giz in this.Game.GetWorld(109894).GetActorsBySNO(180254)) giz.Destroy();  });//destroy walls for Belial
					Game.GetWorld(WorldSno.a4dun_garden_of_hope_01).SpawnMonster(ActorSno._waypoint, new Vector3D { X = 931.48f, Y = 1172.24f, Z = -14.7f }); //waypoint
					Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						var world = Game.GetWorld(WorldSno.a3dun_azmodan_arena);
						try { world.GetActorBySNO(ActorSno._azmodan).Destroy(); } catch { };
						world.SpawnMonster(ActorSno._azmodan, new Vector3D { X = 395.553f, Y = 394.966f, Z = 0.1f });
					}); //spawn Azmodan
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_03, () =>
					{
						var world = Game.GetWorld(WorldSno.a3_battlefields_03);
						try { world.GetActorBySNO(ActorSno._siegebreakerdemon).Destroy(); } catch { }
						world.SpawnMonster(ActorSno._siegebreakerdemon, new Vector3D { X = 396.565f, Y = 366.167f, Z = 0.1f });
					}); //spawn Siegebreaker

				})
			});

			Game.QuestManager.Quests[312429].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			Game.QuestManager.Quests[312429].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion

			#region Nephalem Portal
			
			Game.QuestManager.SideQuests.Add(382695, new Quest { RewardXp = 10000, RewardGold = 1000, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[382695].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					
				})
			});

			Game.QuestManager.SideQuests[382695].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {

					
					
				})
			});

			Game.QuestManager.SideQuests[382695].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					var NephalemWorld = Game.GetWorld(Game.WorldOfPortalNephalem);
					ActorSystem.Actor BossOfPortal = null;
					
					switch (Game.WorldOfPortalNephalem)
					{
						default:
							List<MapSystem.Scene> Scenes = new List<MapSystem.Scene>();
							foreach (var scene in NephalemWorld.Scenes.Values)
							{
								if (!scene.SceneSNO.Name.ToLower().Contains("filler"))
									Scenes.Add(scene);
							}
							int SceneNum = Scenes.Count - DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, 3);
							Vector3D SSV = Scenes[SceneNum - 1].Position;
							Vector3D SP = null;
							while (true)
							{
								SP = new Vector3D(SSV.X + DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, 240), SSV.Y + DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, 240), SSV.Z);
								if (NephalemWorld.CheckLocationForFlag(SP, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
									break;
							}
							BossOfPortal = NephalemWorld.SpawnMonster(ActorSno._x1_lr_boss_mistressofpain, SP);
							break;
					}
					ActiveArrow(NephalemWorld, BossOfPortal.SNO);
					ListenKill(BossOfPortal.SNO, 1, new QuestEvents.SideAdvance());
                })
			});

			Game.QuestManager.SideQuests[382695].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			Game.QuestManager.SideQuests[382695].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						
					}
				})
			});

			//*/
			#endregion

			#region Nephalem Portal

			Game.QuestManager.SideQuests.Add(337492, new Quest { RewardXp = 10000, RewardGold = 1000, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[337492].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					
				})
			});

			Game.QuestManager.SideQuests[337492].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {

				})
			});

			Game.QuestManager.SideQuests[337492].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					

				})
			});

			Game.QuestManager.SideQuests[337492].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete

				})
			});

			Game.QuestManager.SideQuests[337492].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{

					}
				})
			});

			//*/
			#endregion
		}
	}
}
