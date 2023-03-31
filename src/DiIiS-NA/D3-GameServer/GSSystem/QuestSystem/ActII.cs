using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using System;
using System.Collections.Generic;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations;
using System.Linq;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class ActII : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public int refugees = 0; //temp

		public ActII(Game game) : base(game)
		{
		}

		public override void SetQuests()
		{
			#region Shadows in the Desert
			Game.QuestManager.Quests.Add(80322, new Quest { RewardXp = 4400, RewardGold = 490, Completed = false, Saveable = true, NextQuest = 93396, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[80322].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 82,
				OnAdvance = new Action(() => {
					var world = Game.GetWorld(WorldSno.caout_town);
					//ListenProximity(151989, new Advance());
					ListenConversation(57401, new Advance());
					//Base world State
					world.GetActorBySNO(ActorSno._caout_stingingwinds_khamsin_gate).SetUsable(false); //Khamsim_Gate
					world.GetActorBySNO(ActorSno._bezir).Hidden = true; //Bezir
					world.ShowOnlyNumNPC(ActorSno._kadin, 1); //Kadin
					world.ShowOnlyNumNPC(ActorSno._aleser, 1); //Aleser
					world.ShowOnlyNumNPC(ActorSno._caliem, 1); //Caliem
					world.ShowOnlyNumNPC(ActorSno._davyd, -1); //Davyd
					foreach (var Door in world.GetActorsBySNO(ActorSno._caout_stingingwinds_khamsin_gate))
						Door.SetUsable(false);//Khamsim_Gate
					
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(82, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 85,
				OnAdvance = new Action(() => { //go to Caldeum
					ListenTeleport(55313, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(85, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 50,
				OnAdvance = new Action(() => { //talk with Asheera (201085)
					var world = Game.GetWorld(WorldSno.caout_town);
					world.GetActorBySNO(ActorSno._a2duncald_deco_sewer_lid).SetUsable(false);
					world.ShowOnlyNumNPC(ActorSno._asheara, 2);
					UnlockTeleport(0);
					//ListenProximity(3205, new LaunchConversation(201085));
					ListenConversation(201085, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(50, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 61,
				OnAdvance = new Action(() => { //go through canyon
					try 
					{
						Door TDoor = (Game.GetWorld(WorldSno.caout_town).FindActorAt(ActorSno._a2dun_cald_exit_gate, new Vector3D { X = 2905.62f, Y = 1568.82f, Z = 250.75f }, 6.0f) as Door);
						//ListenProximity(TDoor, )
						TDoor.Open();
					} catch { }
					//ListenProximity(85843, new LaunchConversation(169197));
					ListenConversation(169197, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(61, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 52,
				OnAdvance = new Action(() => { //kill cultists
					var world = Game.GetWorld(WorldSno.caout_town);
					AddFollower(world, ActorSno._enchantressnpc);
					//ListenProximity(85843, new SpawnCultists());
					script = new SpawnCultists();
					script.Execute(world);
					//ListenKill(ActorSno._triunecultist_c, 7, new Advance());
					ListenProximity(ActorSno._enchantressnpc, new Advance());		// HACK: Skip ambush
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(52, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 102,
				OnAdvance = new Action(() => { //talk with enchantress
					DestroyFollower(ActorSno._enchantressnpc);
					//ListenProximity(85843, new LaunchConversation(85832));
					var EnchNPC = (Game.GetWorld(WorldSno.caout_town).GetActorBySNO(ActorSno._enchantressnpc) as InteractiveNPC);
					EnchNPC.Conversations.Clear();
					EnchNPC.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(85832));
					ListenConversation(85832, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(102, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 106,
				OnAdvance = new Action(() => { //kill Lakuni's
					var world = Game.GetWorld(WorldSno.caout_town);
					world.GetActorBySNO(ActorSno._enchantressnpc).Hidden = true;
					AddFollower(world, ActorSno._enchantressnpc);
					Open(world, ActorSno._caoutstingingwinds_illusion_rocks);
					ListenKill(ActorSno._lacunifemale_a, 5, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(106, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 91,
				OnAdvance = new Action(() => { //talk with Steel Wolf's leader
					var world = Game.GetWorld(WorldSno.caout_town);
					DestroyFollower(ActorSno._enchantressnpc);
					AddFollower(world, ActorSno._enchantressnpc);
					//ListenProximity(164195, new LaunchConversation(164197));
					var Leader = (world.GetActorBySNO(ActorSno._caldeumguard_cleaver_a_jarulf) as InteractiveNPC);
					Leader.Conversations.Clear();
					Leader.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(164197));
					ListenConversation(164197, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(91, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 58,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //break rituals (2 counters)
					var world = Game.GetWorld(WorldSno.caout_town);
					DestroyFollower(ActorSno._enchantressnpc);
					AddFollower(world, ActorSno._enchantressnpc);
					var Leader = (world.GetActorBySNO(ActorSno._caldeumguard_cleaver_a_jarulf) as InteractiveNPC);
					Leader.Conversations.Clear();
					ListenProximity(ActorSno._caldeumtortured_poor_male_a_ritualvictim, new CompleteObjective(0));
					ListenProximity(ActorSno._caldeumtortured_poor_male_a, new CompleteObjective(1));
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(58, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 117,
				OnAdvance = new Action(() => { //go to Canyon Bridge
					DestroyFollower(ActorSno._enchantressnpc);
					AddFollower(Game.GetWorld(WorldSno.caout_town), ActorSno._enchantressnpc);
					ListenProximity(ActorSno._caout_mine_rope_short, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(117, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 10,
				OnAdvance = new Action(() => { //talk with enchantress
					DestroyFollower(ActorSno._enchantressnpc);
					AddFollower(Game.GetWorld(WorldSno.caout_town), ActorSno._enchantressnpc);
					ListenProximity(ActorSno._enchantressnpc, new LaunchConversation(86196));
					ListenConversation(86196, new Advance());
				})
			});

			Game.QuestManager.Quests[80322].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					DestroyFollower(ActorSno._enchantressnpc);
					Open(Game.GetWorld(WorldSno.caout_town), ActorSno._caoutstingingwinds_illusion_bridge);
					if (!Game.Empty)
						foreach (var plr in Game.Players.Values)
						{
							if (!plr.HirelingEnchantressUnlocked)
							{
								plr.HirelingEnchantressUnlocked = true;
								plr.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 3 });
								plr.GrantAchievement(74987243307145);
							}
							if (Game.Players.Count > 1)
								plr.InGameClient.SendMessage(new HirelingNoSwapMessage() { NewClass = 3 }); //Призвать нельзя!
							else
								plr.InGameClient.SendMessage(new HirelingSwapMessage() { NewClass = 3 }); //Возможность призвать Храмовника
						}
				})
			});

			#endregion
			#region Road to Alcarnus
			Game.QuestManager.Quests.Add(93396, new Quest { RewardXp = 4600, RewardGold = 500, Completed = false, Saveable = true, NextQuest = 74128, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[93396].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 76,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(76, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 58,
				OnAdvance = new Action(() => { //go through Canyon bridge
					ListenProximity(ActorSno._waypoint, new Advance());
					
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(58, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 46,
				OnAdvance = new Action(() => { //find Khasim gate
					var world = Game.GetWorld(WorldSno.caout_town);
					ListenProximity(ActorSno._lore_belial_guardsorders, new Advance());
					world.GetActorBySNO(ActorSno._bezir).Hidden = true; //Bezir
					world.ShowOnlyNumNPC(ActorSno._kadin, 0); //Kadin
					world.ShowOnlyNumNPC(ActorSno._aleser, 0); //Aleser
					world.ShowOnlyNumNPC(ActorSno._caliem, 0); //Caliem
					world.ShowOnlyNumNPC(ActorSno._davyd, -1); //Davyd

				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(46, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 74,
				OnAdvance = new Action(() => { //talk with leutenant Vahem
					UnlockTeleport(2);
					UnlockTeleport(3);
					//ListenProximity(90959, new LaunchConversation(201369));
					ListenConversation(201369, new Advance());
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(74, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 30,
				OnAdvance = new Action(() => { //enter HQ
					ListenTeleport(61066, new Advance());
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(30, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //kill demons and open cell
					Game.AddOnLoadWorldAction(WorldSno.caout_khamsin_mine, () =>
					{
						if ((Game.CurrentQuest == 93396 & Game.CurrentStep == 30) || (Game.CurrentQuest == 93396 & Game.CurrentStep == 74))
						{
							var world = Game.GetWorld(WorldSno.caout_khamsin_mine);
							DisableEveryone(world, true);

							StartConversation(world, 195060);
							ListenConversation(195060, new LaunchConversation(195062));
							ListenConversation(195062, new KhasimHQ());

							//ListenKill(60583, 1, new CompleteObjective(0));
							ListenKill(ActorSno._snakeman_melee_c, 6, new CompleteObjective(0));
							//5434
							ListenInteract(ActorSno._caout_cage_noscript, 1, new CompleteObjective(1));
						}
					});
					

				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 48,
				OnAdvance = new Action(() => { //kill stealthed demons
					script = new SpawnSnakemans();
					script.Execute(Game.GetWorld(WorldSno.caout_town));
					ListenKill(ActorSno._khamsin_snakeman_melee, 3, new Advance());
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(48, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				OnAdvance = new Action(() => { //talk with captain David
					Game.GetWorld(WorldSno.caout_town).ShowOnlyNumNPC(ActorSno._davyd, 1);
					//ListenProximity(80980, new LaunchConversation(60608));
					ListenConversation(81351, new Advance());
				})
			});

			Game.QuestManager.Quests[93396].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					Open(Game.GetWorld(WorldSno.caout_town), ActorSno._caout_stingingwinds_khamsin_gate);
				})
			});

			#endregion
			#region City on Blood
			Game.QuestManager.Quests.Add(74128, new Quest { RewardXp = 6600, RewardGold = 765, Completed = false, Saveable = true, NextQuest = 57331, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[74128].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 5,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[74128].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 54,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //exit through Khasim east gates and find Alcarnus
					ListenProximity(ActorSno._davyd, new CompleteObjective(0));
					ListenProximity(ActorSno._body_hangedc_caout_gore, new CompleteObjective(1));
				})
			});

			Game.QuestManager.Quests[74128].Steps.Add(54, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				OnAdvance = new Action(() => { //find Maghda's lair and optionally, free 8 cells
					var encW = Game.GetWorld(WorldSno.caout_town);
					encW.SpawnMonster(ActorSno._caout_cage, new Vector3D(528.7084f,	 1469.1945f, 197.2559f));
					encW.SpawnMonster(ActorSno._caout_cage, new Vector3D(475.812f,	 1554.7146f, 197.25589f));
					encW.SpawnMonster(ActorSno._caout_cage, new Vector3D(463.88342f, 1542.4092f, 197.25587f));
					encW.SpawnMonster(ActorSno._caout_cage, new Vector3D(399.93198f, 1485.7723f, 197.38196f));
					encW.SpawnMonster(ActorSno._caout_cage, new Vector3D(509.43765f, 1254.6984f, 197.31921f));
					ListenInteract(ActorSno._caout_cage, 8, new Dummy());
					ListenTeleport(195268, new Advance());
				})
			});

			Game.QuestManager.Quests[74128].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 9,
				OnAdvance = new Action(() => { //kill Maghda
					UnlockTeleport(4);
					ListenKill(ActorSno._maghda, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[74128].Steps.Add(9, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				OnAdvance = new Action(() => { //return to camp
					Game.CurrentEncounter.Activated = false;
					Game.AddOnLoadWorldAction(WorldSno.caout_cellar_alcarnus_main, () =>
					{
						Open(Game.GetWorld(WorldSno.caout_cellar_alcarnus_main), ActorSno._caout_stingingwinds_arena_bridge);
					});
					ListenProximity(ActorSno._hearthportal, new Advance());
				})
			});

			Game.QuestManager.Quests[74128].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					PlayCutscene(1);
				})
			});

			#endregion
			#region Audience with Emperor
			Game.QuestManager.Quests.Add(57331, new Quest { RewardXp = 1875, RewardGold = 260, Completed = false, Saveable = true, NextQuest = 78264, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[57331].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 38,
				OnAdvance = new Action(() => { //talk with Asheera
											   //ListenProximity(3205, new LaunchConversation(201285));
					Game.GetWorld(WorldSno.caout_town).ShowOnlyNumNPC(ActorSno._asheara, 0);
					ListenConversation(201285, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(38, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 18,
				OnAdvance = new Action(() => { //talk with Asheera for reach emperor's palace
											   //ListenConversation(165807, new Advance());
					ListenProximity(ActorSno._caldeumguard_captain_b_ravd, new AskBossEncounter(162231));
					ListenTeleport(81178, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 21,
				OnAdvance = new Action(() => { //talk with Emperor
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_intro, () =>
					{
						//ID: 59447 Name: BelialBoyEmperor
						var world = Game.GetWorld(WorldSno.a2_belial_room_intro);
						foreach (var plr in Game.Players.Values)
						{
							plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSno._belialboyemperor).DynamicID(plr), Duration = 1f, Snap = false });
						}
						foreach (var leah in world.GetActorsBySNO(ActorSno._leah))
							if (leah is TownLeah)
								(leah as TownLeah).Brain.DeActivate();
						SetActorOperable(world, ActorSno._a2dun_cald_belial_room_gate_a, false);
						DisableEveryone(world, true);
						StartConversation(world, 160894);
					});
					//ListenTeleport(81178, new LaunchConversation(160894));
					ListenConversation(160894, new LaunchConversation(160896));
					ListenConversation(160896, new LaunchConversation(116036));
					ListenConversation(116036, new EndCutSceneWithAdvance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //kill demons
					int snakes = 0;
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_intro, () =>
					{
						if (Game.CurrentQuest == 57331 && Game.CurrentStep == 21)
						{
							var world = Game.GetWorld(WorldSno.a2_belial_room_intro);
							DisableEveryone(world, false);
							AddFollower(world, ActorSno._leah);
							foreach (var leah in world.GetActorsBySNO(ActorSno._leah))
								if (leah is TownLeah)
									(leah as TownLeah).Brain.Activate(); 
							script = new SpawnSnakemanGuards();
							script.Execute(world);

							foreach (var snake in world.GetActorsBySNO(ActorSno._khamsin_snakeman_melee))
								snakes++;
						}
					});
					
					ListenKill(ActorSno._khamsin_snakeman_melee, snakes, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 31,
				OnAdvance = new Action(() => { //escape the emperor's palace
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_intro, () =>
					{
						if (Game.CurrentQuest == 57331 && Game.CurrentStep == 2)
						{
							var world = Game.GetWorld(WorldSno.a2_belial_room_intro);
							DestroyFollower(ActorSno._leah);
							AddFollower(world, ActorSno._leah);
							Open(world, ActorSno._a2dun_cald_belial_room_gate_a);
						}
					});
					ListenTeleport(102964, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(31, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 34,
				OnAdvance = new Action(() => { //destroy 4 demon summoners
					Game.AddOnLoadWorldAction(WorldSno.a2dun_cald, () =>
					{
						if (Game.CurrentQuest == 57331 && Game.CurrentStep == 31)
						{
							var world = Game.GetWorld(WorldSno.a2dun_cald);
							world.ShowOnlyNumNPC(ActorSno._asheara, -1); //Leave all Asheara
							world.ShowOnlyNumNPC(ActorSno._leah, -1); //Leave all Leah
							world.ShowOnlyNumNPC(ActorSno._adria, -1); //Leave all Adria
							DestroyFollower(ActorSno._leah);
							AddFollower(Game.GetWorld(WorldSno.a2_belial_room_intro), ActorSno._leah);
							script = new SpawnSnakemanDefenders();
							script.Execute(world);

						}
					});
					ListenKill(ActorSno._a2dun_cald_belial_summoningmachine_node_monster, 4, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 7,
				OnAdvance = new Action(() => { //hide into Caldeum drains
					Game.AddOnLoadWorldAction(WorldSno.a2dun_cald, () =>
					{
						if (Game.CurrentQuest == 57331 && Game.CurrentStep == 34)
						{
							DestroyFollower(ActorSno._leah);
							AddFollower(Game.GetWorld(WorldSno.a2_belial_room_intro), ActorSno._leah);

							foreach (var act in Game.GetWorld(WorldSno.a2dun_cald).GetActorsBySNO(ActorSno._temp_snakeportal_center))
								act.Destroy();//TEMP_SnakePortal_Center
						}
					});
					ListenTeleport(19791, new Advance());
				})
			});

			Game.QuestManager.Quests[57331].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					Game.AddOnLoadWorldAction(WorldSno.a2dun_cald, () =>
					{
						if (Game.CurrentQuest == 57331 && Game.CurrentStep == 7)
						{
							var world = Game.GetWorld(WorldSno.a2c1dun_swr_caldeum_01);
							DestroyFollower(ActorSno._leah);
							world.ShowOnlyNumNPC(ActorSno._leah, -1); //Leave all Leah
							world.ShowOnlyNumNPC(ActorSno._leahsewer, -1); //Leave all LeahSewer
						}
					});
					Game.CurrentEncounter.Activated = false;
				})
			});

			#endregion
			#region Unexpected Help (Rescue Adria)
			Game.QuestManager.Quests.Add(78264, new Quest { RewardXp = 5200, RewardGold = 530, Completed = false, Saveable = true, NextQuest = 78266, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[78264].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 9,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[78264].Steps.Add(9, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //find Cursed Pit
					if (Game.Empty) UnlockTeleport(1);
					ListenTeleport(58494, new Advance());
				})
			});

			Game.QuestManager.Quests[78264].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 15,
				OnAdvance = new Action(() => { //kill guardians
					Game.AddOnLoadWorldAction(WorldSno.a2dun_swr_adria_level01, () =>
					{
						if (Game.CurrentQuest == 78264 && Game.CurrentStep == 2)
						{
							var world = Game.GetWorld(WorldSno.a2dun_swr_adria_level01);
							StartConversation(world, 81197);
							foreach (var plr in Game.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							foreach (var plr in Game.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSno._adria).DynamicID(plr), Duration = 1f, Snap = false });
							ListenConversation(81197, new EndCutScene());
						}
					});
					ListenKill(ActorSno._snakeman_caster_a_adriatorturer, 1, new Advance());
					UnlockTeleport(1);
				})
			});

			Game.QuestManager.Quests[78264].Steps.Add(15, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 21,
				OnAdvance = new Action(() => { //talk with Adria in pit
					Game.AddOnLoadWorldAction(WorldSno.a2dun_swr_adria_level01, () =>
					{
						AddQuestConversation(Game.GetWorld(WorldSno.a2dun_swr_adria_level01).GetActorBySNO(ActorSno._adria), 81674);
						ListenConversation(81674, new Advance());
					});
				})
			});

			Game.QuestManager.Quests[78264].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 8,
				OnAdvance = new Action(() => { //talk with Adria in camp
					var world = Game.GetWorld(WorldSno.a2dun_swr_adria_level01);
					Game.CurrentEncounter.Activated = false;
					if (world.GetActorBySNO(ActorSno._adria) != null)
						RemoveConversations(world.GetActorBySNO(ActorSno._adria));

					var Adria = world.ShowOnlyNumNPC(ActorSno._adria, 0);
					var Portal = world.GetActorBySNO(ActorSno._adria_town_portal);
					var AltPortal = world.SpawnMonster(ActorSno._adria_town_portal, Portal.Position);


					Adria.Move(Portal.Position, ActorSystem.Movement.MovementHelpers.GetFacingAngle(Adria, Portal)); //Only Talk Adria
					Portal.SetVisible(true);
					Portal.Hidden = false;

                    System.Threading.Tasks.Task.Delay(3000).ContinueWith(delegate
					{
						world.ShowOnlyNumNPC(ActorSno._adria, -1); //Only Talk Adria
						Portal.Destroy();
						AltPortal.Destroy();
					});

						AddQuestConversation(Game.GetWorld(WorldSno.caout_refugeecamp).GetActorBySNO(ActorSno._adria), 58139);
					//ListenProximity(6353, new LaunchConversation(58139));
					
					ListenConversation(58139, new Advance());
				})
			});

			Game.QuestManager.Quests[78264].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					PlayCutscene(2);
					Game.GetWorld(WorldSno.caout_town).GetActorBySNO(ActorSno._a2duncald_deco_sewer_lid).SetUsable(true);

				})
			});

			#endregion
			#region Horadric traitor
			Game.QuestManager.Quests.Add(78266, new Quest { RewardXp = 7425, RewardGold = 810, Completed = false, Saveable = true, NextQuest = 57335, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[78266].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				OnAdvance = new Action(() => {
				})
			});
			Game.QuestManager.Quests[78266].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 34,
				OnAdvance = new Action(() => { //find passage to Oasis
					if (Game.DestinationEnterQuest == 78266)
						if (Game.DestinationEnterQuestStep == -1 || Game.DestinationEnterQuestStep == 2)
						{
							Game.AddOnLoadWorldAction(WorldSno.caout_refugeecamp, () =>
							{
								ActiveArrow(Game.GetWorld(WorldSno.caout_refugeecamp), ActorSno._g_portal_archtall_orange_icondoor, WorldSno.caout_town);
							});
							Game.AddOnLoadWorldAction(WorldSno.caout_town, () =>
							{
								ActiveArrow(Game.GetWorld(WorldSno.caout_town), ActorSno._g_portal_circle_blue, WorldSno.a2_swr_fcauseway_01);
							});
						}
					
					ListenProximity(ActorSno._a2dunswr_gates_causeway_gates, new Advance());
					
					//ListenInteract(177881, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 31,
				OnAdvance = new Action(() => { //talk with Emperor
					ListenProximity(ActorSno._hakan, new Advance());
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(31, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				OnAdvance = new Action(() => { //find Oasis
					ListenConversation(180063, new LaunchConversation(187093));
					ListenTeleport(175367, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a2_swr_fcauseway_01, () =>
					{
						if (Game.CurrentQuest == 78266 && Game.CurrentStep == 31)
						{
							StartConversation(Game.GetWorld(WorldSno.a2_swr_fcauseway_01), 180063);
						}
					});
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 22,
				OnAdvance = new Action(() => { //enter ruins in Oasis
					ListenTeleport(61632, new Advance());
					if (Game.Empty) UnlockTeleport(5);
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 24,
				OnAdvance = new Action(() => { //find Kulle's head
					UnlockTeleport(5);
					UnlockTeleport(6);
					ListenProximity(ActorSno._a2dun_zolt_head_container, new Advance());
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(24, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				OnAdvance = new Action(() => { //get Kulle's head
					ListenInteract(ActorSno._a2dun_zolt_head_container, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 11,
				OnAdvance = new Action(() => { //talk with Adria in camp
					ListenProximity(ActorSno._tyrael, new LaunchConversation(123146));
					ListenConversation(123146, new Advance());
				})
			});

			Game.QuestManager.Quests[78266].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Blood and Sand
			Game.QuestManager.Quests.Add(57335, new Quest { RewardXp = 9650, RewardGold = 1090, Completed = false, Saveable = true, NextQuest = 57337, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[57335].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 34,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 40,
				OnAdvance = new Action(() => { //enter to drain in Oasis
					ListenTeleport(62752, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(40, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 52,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //turn east lever and turn west lever and open gates to drowned passage
											   //try {(this.Game.GetWorld(59486).FindAt(83629, new Vector3D{X = 175.1f, Y = 62.275f, Z = 50.17f}, 20.0f) as Door).Open();} catch {}
					var world = Game.GetWorld(WorldSno.a2dun_aqd_special_01);
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_special_01, () =>
					{
						(world.FindActorAt(ActorSno._a2dun_aqd_act_stone_slab_a_01, new Vector3D { X = 175.1f, Y = 62.275f, Z = 50.17f }, 20.0f) as Door).Open();
					});
					ListenInteract(ActorSno._a2dun_aqd_act_waterwheel_lever_a_01, 1, new CompleteObjective(0));
					ListenInteract(ActorSno._a2dun_aqd_act_waterwheel_lever_b_01, 1, new Advance());
					StartConversation(world, 186905);
					//ListenInteract(83295, 1, new CompleteObjective(2));
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(52, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 54,
				OnAdvance = new Action(() => { //enter to drowned passage
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_special_01, () =>
					{
						var world = Game.GetWorld(WorldSno.a2dun_aqd_special_01);
						Open(world, ActorSno._a2dun_aqd_act_stone_slab_a_01);
						Open(world, ActorSno._a2dun_aqd_special_01_waterfall);
						Open(world, ActorSno._a2dun_aqd_mainpuzzle_door);
						(world.FindActorAt(ActorSno._a2dun_aqd_act_stone_slab_a_01, new Vector3D { X = 80.5f, Y = 155.631f, Z = 50.33f }, 20.0f) as Door).Open();
					});
					//try {(this.Game.GetWorld(59486).FindAt(83629, new Vector3D{X = 80.5f, Y = 155.631f, Z = 50.33f}, 20.0f) as Door).Open();} catch {}
					ListenTeleport(192694, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(54, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 56,
				OnAdvance = new Action(() => { //kill Deceiveds
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00);
						SetActorOperable(world, ActorSno._spawner_leor_iron_maiden_jewelerquest, false);
						world.SpawnMonster(ActorSno._fastmummy_a, new Vector3D { X = 75.209f, Y = 191.342f, Z = -1.5f });
						world.SpawnMonster(ActorSno._fastmummy_a, new Vector3D { X = 44.703f, Y = 179.753f, Z = -1.56f });
						world.SpawnMonster(ActorSno._fastmummy_a, new Vector3D { X = 43.304f, Y = 205.28f, Z = -0.34f });
					});
					ListenKill(ActorSno._fastmummy_a, 3, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(56, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 58,
				OnAdvance = new Action(() => { //break talking barrel
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						SetActorOperable(Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00), ActorSno._spawner_leor_iron_maiden_jewelerquest, true);
					});
					ListenInteract(ActorSno._spawner_leor_iron_maiden_jewelerquest, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(58, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 60,
				OnAdvance = new Action(() => { //talk with jeweler
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00);
						world.SpawnMonster(ActorSno._intro_jeweler, world.GetActorBySNO(ActorSno._spawner_leor_iron_maiden_jewelerquest, true).Position);
					});
					ListenProximity(ActorSno._intro_jeweler, new LaunchConversation(168948));
					ListenConversation(168948, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(60, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 62,
				OnAdvance = new Action(() => { //find crucible
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						if (Game.CurrentQuest == 57335 && Game.CurrentStep == 60)
						{
							AddFollower(Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00), ActorSno._intro_jeweler);
						}
					});
					ListenProximity(ActorSno._zombie_unique_jewelerquest, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(62, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 64,
				OnAdvance = new Action(() => { //kill Gevin
					var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00);
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						if (Game.CurrentQuest == 57335 && Game.CurrentStep == 62)
						{
							DestroyFollower(ActorSno._intro_jeweler);
							AddFollower(world, ActorSno._intro_jeweler);
						}
					});
					bool Killed = true;
					foreach (var act in world.Actors.Values)
						if (act.SNO == ActorSno._zombie_unique_jewelerquest)
							Killed = false;
					if (!Killed)
						ListenKill(ActorSno._zombie_unique_jewelerquest, 1, new Advance());
					else
					{
						script = new Advance();
						script.Execute(world);
					}

				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(64, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 44,
				OnAdvance = new Action(() => { //get crucible
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						if (Game.CurrentQuest == 57335 && Game.CurrentStep == 64)
						{
							DestroyFollower(ActorSno._intro_jeweler);
							AddFollower(Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00), ActorSno._intro_jeweler);
						}
					});
					ListenInteract(ActorSno._a2dun_aqd_jeweler_altar, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(44, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 24,
				OnAdvance = new Action(() => { //enter the ancient passage
					Game.AddOnLoadWorldAction(WorldSno.a2dun_aqd_oasis_level00, () =>
					{
						if (Game.CurrentQuest == 57335 && Game.CurrentStep == 44)
						{
							DestroyFollower(ActorSno._intro_jeweler);
							AddFollower(Game.GetWorld(WorldSno.a2dun_aqd_oasis_level00), ActorSno._intro_jeweler);
						}
					});
					ListenTeleport(175330, new Advance());
					if (!Game.Empty)
						foreach (var plr in Game.Players.Values)
						{
							if (!plr.JewelerUnlocked)
							{
								plr.JewelerUnlocked = true;
								plr.GrantAchievement(74987243307780);
								//plr.UpdateAchievementCounter(403, 1, 1);
								plr.LoadCrafterData();
							}
						}
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(24, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 8,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //find blood in 2 caves
					if (Game.Empty) UnlockTeleport(7);
					DestroyFollower(ActorSno._intro_jeweler);
					ListenInteract(ActorSno._a2dun_zolt_blood_container, 1, new CompleteObjective(0));
					ListenInteract(ActorSno._a2dun_zolt_blood_container_02, 1, new CompleteObjective(1));
				})
			});

			Game.QuestManager.Quests[57335].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					PlayCutscene(3);
				})
			});

			#endregion
			#region Black Soulstone
			Game.QuestManager.Quests.Add(57337, new Quest { RewardXp = 14515, RewardGold = 1675, Completed = false, Saveable = true, NextQuest = 121792, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[57337].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 25,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(25, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 39,
				OnAdvance = new Action(() => { //enter the Kulle's archives				
					UnlockTeleport(7);
					UnlockTeleport(8);
					ListenTeleport(19800, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(39, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 35,
				OnAdvance = new Action(() => { //enter the Limit
					ListenProximity(ActorSno._hakanprojection, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(35, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 41,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //open Abyss lock and open Stormhalls lock
					if (Game.Empty) UnlockTeleport(9);
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_lobby, () =>
					{
						if (Game.CurrentQuest == 57337 && Game.CurrentStep == 35)
						{
							StartConversation(Game.GetWorld(WorldSno.a2dun_zolt_lobby), 187015);
						}
					});
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_level01, () =>
					{
						(Game.GetWorld(WorldSno.a2dun_zolt_level01).GetActorBySNO(ActorSno._spawner_zolt_centerpiece) as Spawner).Spawn();
					});
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_level02, () =>
					{
						(Game.GetWorld(WorldSno.a2dun_zolt_level02).GetActorBySNO(ActorSno._spawner_zolt_centerpiece) as Spawner).Spawn();
					});
					ListenInteract(ActorSno._a2dun_zolt_centerpiece_a, 2, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(41, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 0,
				OnAdvance = new Action(() => { //enter the shadows world
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_lobby, () =>
					{
						Open(Game.GetWorld(WorldSno.a2dun_zolt_lobby), ActorSno._a2dun_zolt_shadow_realm_portal_terminus);
					});
					UnlockTeleport(9);
					ListenTeleport(80592, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(0, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //find Kulle's body
					ListenInteract(ActorSno._a2dun_zolt_body_container, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				OnAdvance = new Action(() => { //talk with Leah
					ListenProximity(ActorSno._leah, new LaunchConversation(62505));
					foreach (var act in Game.GetWorld(WorldSno.a2dun_zolt_lobby).GetActorsBySNO(ActorSno._temp_zknavblocker))
						act.Destroy();
					ListenConversation(62505, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 27,
				OnAdvance = new Action(() => { //enter the soulstone storage
					ListenTeleport(60194, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(27, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 4,
				OnAdvance = new Action(() => { //talk with Kulle
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_bossfight_level04, () =>
					{
						if (Game.CurrentQuest == 57337 && Game.CurrentStep == 27)
						{
							StartConversation(Game.GetWorld(WorldSno.a2dun_zolt_bossfight_level04), 202697);
						}
					});
					ListenConversation(202697, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 31,
				OnAdvance = new Action(() => { //kill Kulle
					ListenKill(ActorSno._zoltunkulle, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(31, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 33,
				OnAdvance = new Action(() => { //get Soulstone
					Game.AddOnLoadWorldAction(WorldSno.a2dun_zolt_bossfight_level04, () =>
					{
						Open(Game.GetWorld(WorldSno.a2dun_zolt_bossfight_level04), ActorSno._a2dun_zolt_sandbridgebase_bossfight);
					});
					ListenInteract(ActorSno._a2dun_zolt_black_soulstone, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(33, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 6,
				OnAdvance = new Action(() => { //talk with Adria in camp
					ListenProximity(ActorSno._adria, new LaunchConversation(80513));
					ListenConversation(80513, new Advance());
				})
			});

			Game.QuestManager.Quests[57337].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Rush in Caldeum
			Game.QuestManager.Quests.Add(121792, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 57339, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[121792].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 34,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[121792].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 23,
				OnAdvance = new Action(() => { //find Asheara
					Game.AddOnLoadWorldAction(WorldSno.caout_town, () =>
					{
						var world = Game.GetWorld(WorldSno.caout_town);
						world.ShowOnlyNumNPC(ActorSno._asheara, 1);
						script = new Advance();
						script.Execute(world);
					});
					//ListenProximity(3205, new Advance());
				})
			});

			Game.QuestManager.Quests[121792].Steps.Add(23, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 21,
				OnAdvance = new Action(() => { //talk with Asheara
					foreach (var Ashe in Game.GetWorld(WorldSno.caout_town).GetActorsBySNO(ActorSno._asheara))
						AddQuestConversation(Ashe, 121359);
					ListenConversation(121359, new Advance());
				})
			});

			Game.QuestManager.Quests[121792].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //todo: timed event 115494
					var world = Game.GetWorld(WorldSno.caout_town);
					try { (world.FindActorAt(ActorSno._a2dun_cald_exit_gate, new Vector3D { X = 3135.3f, Y = 1546.1f, Z = 250.545f }, 15.0f) as Door).Open(); } catch { }
					foreach (var Ashe in world.GetActorsBySNO(ActorSno._asheara))
						RemoveConversations(Ashe);
					StartConversation(world, 178852);
					ListenConversation(178852, new RefugeesRescue());
					//ListenProximity(162378, new RefugeesRescue());
				})
			});

			Game.QuestManager.Quests[121792].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Lord of Lies
			Game.QuestManager.Quests.Add(57339, new Quest { RewardXp = 10850, RewardGold = 1160, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[57339].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 12,
				OnAdvance = new Action(() => { //enter the Caldeum palace
					AddFollower(Game.GetWorld(WorldSno.a2_belial_room_intro), ActorSno._leah);
					AddFollower(Game.GetWorld(WorldSno.caout_refugeecamp), ActorSno._adria);
					ListenTeleport(210451, new Advance());
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 29,
				OnAdvance = new Action(() => { //go to emperor's palace
					var world = Game.GetWorld(WorldSno.a2dun_cald_uprising);
					foreach (var door in world.GetActorsBySNO(ActorSno._a2dun_cald_gate_belial_destroyable))
						door.Destroy();
					foreach (var guard in world.GetActorsBySNO(ActorSno._caldeumguard_spear_b_nowander))
						guard.Destroy();
					DestroyFollower(ActorSno._leah);
					DestroyFollower(ActorSno._adria);
					AddFollower(Game.GetWorld(WorldSno.a2_belial_room_intro), ActorSno._leah);
					AddFollower(Game.GetWorld(WorldSno.caout_refugeecamp), ActorSno._adria);
					Game.AddOnLoadWorldAction(WorldSno.a2dun_cald_uprising, () =>
					{
						script = new SpawnBelialDefenders();
						script.Execute(world);
						Open(world, ActorSno._a2dun_cald_belial_magic_blocker);
					});
					ListenTeleport(60757, new Advance());
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				OnAdvance = new Action(() => { //kill Belial
					DestroyFollower(ActorSno._leah);
					DestroyFollower(ActorSno._adria);
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_01, () =>
					{
						var world = Game.GetWorld(WorldSno.a2_belial_room_01);
						SetActorOperable(world, ActorSno._a2dun_cald_belial_room_a_breakable_main, false);
						SetActorOperable(world, ActorSno._a2dun_cald_belial_room_gate_a, false);
						SetActorVisible(world, ActorSno._a2dun_zolt_black_soulstone_nofx, false);
						//stage 1
						if (Game.CurrentQuest == 57339 && Game.CurrentStep == 29)
						{
							DisableEveryone(world, true);
							foreach (var Adr in world.GetActorsBySNO(ActorSno._adria))
								(Adr as Minion).Brain.DeActivate();
							foreach (var Adr in world.GetActorsBySNO(ActorSno._leah))
								(Adr as TownLeah).Brain.DeActivate();
							//Старт катсцены
							System.Threading.Tasks.Task.Run(() => 
							{
								while (true)
									if (Game.Players.First().Value.World.SNO == WorldSno.a2_belial_room_01)
										break;

								while (true)
									if (world.GetActorBySNO(ActorSno._belialboyemperor).IsRevealedToPlayer(Game.Players.First().Value))
										break;

								foreach (var plr in Game.Players.Values)
									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });

								foreach (var plr in Game.Players.Values)
									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSno._belialboyemperor).DynamicID(plr), Duration = 1f, Snap = false });
								StartConversation(world, 61130);
							});
							
						}
					});
					ListenConversation(61130, new BelialStageOne());

					//stage 2
					ListenKill(ActorSno._khamsin_snakeman_melee, 4, new LaunchConversationWithCutScene(68408, ActorSno._belial_trueform));
					ListenConversation(68408, new BelialStageTwo());

					//stage 3
					ListenKill(ActorSno._belial_trueform, 1, new LaunchConversationWithCutScene(62229, ActorSno._belial_trueform));
					ListenConversation(62229, new BelialStageThree());
					ListenKill(ActorSno._belial, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 27,
				OnAdvance = new Action(() => { //get Belial's soul
					Game.CurrentEncounter.Activated = false;
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_01, () =>
					{
						(Game.GetWorld(WorldSno.a2_belial_room_01).GetActorBySNO(ActorSno._a2dun_cald_belial_room_a_breakable_main) as BelialRoom).Rebuild();
					});
					//this.Game.GetWorld(60756).SpawnMonster(206391, this.Game.GetWorld(60756).GetStartingPointById(108).Position);
					ListenInteract(ActorSno._belial_bss_soul, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(27, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 14,
				OnAdvance = new Action(() => { //talk with Tyrael in camp
					Game.AddOnLoadWorldAction(WorldSno.a2_belial_room_01, () =>
					{
						var world = Game.GetWorld(WorldSno.a2_belial_room_01);
						SetActorOperable(world, ActorSno._a2dun_cald_belial_room_gate_a, true);
						world.GetActorBySNO(ActorSno._belial_bss_soul, true).Destroy();
					});
					ListenProximity(ActorSno._zoltunkulletownhead, new LaunchConversation(80329));
					ListenConversation(80329, new Advance());
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with caravan leader
					ListenInteract(ActorSno._hub_caravanleader, 1, new LaunchConversation(177669));
					ListenConversation(177669, new ChangeAct(ActEnum.Act3));
					Game.GetWorld(WorldSno.caout_refugeecamp).GetActorBySNO(ActorSno._hub_caravanleader, true).NotifyConversation(1);
				})
			});

			Game.QuestManager.Quests[57339].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
		}

		public static void AddQuestConversation(Actor actor, int conversation)
		{
			var NPC = actor as InteractiveNPC;
			if (NPC != null)
			{ 
				NPC.Conversations.Clear();
				NPC.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(conversation));
				NPC.Attributes[GameAttributes.Conversation_Icon, 0] = 2;
				NPC.Attributes.BroadcastChangedIfRevealed(); 

			}
			else if (actor != null)
			{
				foreach (var N in actor.World.GetActorsBySNO(actor.SNO))
					if (N is InteractiveNPC)
					{
						NPC = N as InteractiveNPC;
						NPC.Conversations.Clear();
						NPC.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(conversation));
						NPC.Attributes[GameAttributes.Conversation_Icon, 0] = 2;
						NPC.Attributes.BroadcastChangedIfRevealed();
					} 
			}
			else
				Logger.Warn("Failed to assign a dialog for NPC.");
		}

		public static void RemoveConversations(Actor actor)
		{
			if (actor is InteractiveNPC npc)
			{
				npc.Conversations.Clear();
				npc.Attributes[GameAttributes.Conversation_Icon, 0] = 1;
				npc.Attributes.BroadcastChangedIfRevealed();
			}
		}

		public static void DisableEveryone(MapSystem.World world, bool disabled)
		{
			foreach (var actor in world.Actors.Values.Where(a => a is Monster || a is Player || a is Minion || a is Hireling))
			{
				actor.Disable = disabled;
			}
		}
	}
}