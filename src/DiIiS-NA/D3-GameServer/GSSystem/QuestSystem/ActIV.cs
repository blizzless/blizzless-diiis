//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class ActIV : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public ActIV(Game game) : base(game)
		{
		}

		public override void SetQuests()
		{
			#region Fall of Heavens
			Game.QuestManager.Quests.Add(112498, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 113910, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[112498].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => {
					UnlockTeleport(0); //{[World] SNOId: 182944 GlobalId: 117440513 Name: a4dun_heaven_1000_monsters_fight_entrance}
					var Tyrael = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).GetActorBySNO(ActorSno._tyrael) as InteractiveNPC;
					Tyrael.Conversations.Clear();
					Tyrael.OverridedConv = true;
					Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					Tyrael.SetUsable(false);
					ListenProximity(ActorSno._tyrael, new LaunchConversation(195607));
					ListenConversation(195607, new Advance());
				})
			});

			Game.QuestManager.Quests[112498].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 17,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Tyrael
					UnlockTeleport(1);
					Game.AddOnLoadWorldAction(WorldSno.a4dun_heaven_1000_monsters_fight_entrance, () =>
					{
						if (Game.CurrentQuest == 112498 && Game.CurrentStep == 2)
						{
							var Tyrael = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).GetActorBySNO(ActorSno._tyrael) as InteractiveNPC;
							Tyrael.Conversations.Clear();
							Tyrael.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(112449));
							Tyrael.OverridedConv = true;
							Tyrael.SetUsable(true);
							Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
							Tyrael.Attributes.BroadcastChangedIfRevealed();
							//StartConversation(this.Game.GetWorld(182944), 112449);
						}
					});
					ListenConversation(112449, new LaunchConversation(195641));
					ListenConversation(195641, new Advance());
				})
			});

			Game.QuestManager.Quests[112498].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 15,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to Hall of Light
					//ListenProximity(182963, new AskBossEncounter(182960));
					ListenTeleport(109149, new Advance());
				})
			});

			Game.QuestManager.Quests[112498].Steps.Add(15, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 12,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Iskatu
					if (!Game.Empty)
					{
						var world = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight);
						script = new Invasion(world.Players.First().Value.Position, 50f, new List<ActorSno> { ActorSno._shadowvermin_a }, 30f, ActorSno._terrordemon_a_unique_1000monster, true);
						script.Execute(world);
						ListenKill(ActorSno._terrordemon_a_unique_1000monster, 1, new Advance());
					}
				})
			});

			Game.QuestManager.Quests[112498].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //complete
					Game.CurrentEncounter.activated = false;
				})
			});

			#endregion
			#region Restoration of Hope
			Game.QuestManager.Quests.Add(113910, new Quest { RewardXp = 12600, RewardGold = 1260, Completed = false, Saveable = true, NextQuest = 114795, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[113910].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 66,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(66, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 58,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find Iterael
					var world = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight);
					if (Game.Empty) UnlockTeleport(2);
					try
					{
						if (world.GetActorBySNO(ActorSno._a4dungarden_corruption_gate, true) != null)
							Open(world, ActorSno._a4dungarden_corruption_gate);
					}
					catch { }
					try
					{
						if (world.GetActorBySNO(ActorSno._spawner_itherael, true) != null)
							world.SpawnMonster(ActorSno._fate, world.GetActorBySNO(ActorSno._spawner_itherael, true).Position);
					}
					catch { }
					ListenProximity(ActorSno._fate, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(58, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 40,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Iterael

					var Ityrael = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight).GetActorBySNO(ActorSno._fate) as InteractiveNPC;
					Ityrael.Conversations.Clear();
					Ityrael.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(112763));
					Ityrael.OverridedConv = true;
					Ityrael.SetUsable(true);
					Ityrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					Ityrael.Attributes.BroadcastChangedIfRevealed();

					ListenConversation(112763, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(40, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 17,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find Library of Fate
					if (!Game.Empty) UnlockTeleport(2); 
					if (Game.Empty) UnlockTeleport(3);
					var Ityrael = Game.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight).GetActorBySNO(ActorSno._fate) as InteractiveNPC;
					Ityrael.Conversations.Clear();
					Ityrael.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					Ityrael.Attributes.BroadcastChangedIfRevealed();

					ListenTeleport(109514, new Advance());
					//ListenProximity(161276, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //enter the Library
					ListenTeleport(143648, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Rakanoth
					if (!Game.Empty) UnlockTeleport(3);
					var Library = Game.GetWorld(WorldSno.a4dun_libraryoffate);

					ListenKill(ActorSno._despair, 1, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a4dun_libraryoffate, () =>
					{
						var Fate = Library.GetActorBySNO(ActorSno._fate); Vector3D Fate_Dist = Fate.Position; Library.Leave(Fate);
						var Hope = Library.GetActorBySNO(ActorSno._hope); Vector3D Hope_Dist = Hope.Position; Library.Leave(Hope);
						var Hope_Bound = Library.GetActorBySNO(ActorSno._a4dunspire_interactives_hope_bound);
						var ExitPortal = Library.GetActorBySNO(ActorSno._g_portal_heaventeal);
						ExitPortal.Hidden = true;
						ExitPortal.SetVisible(false);

						Hope_Bound.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
						Hope_Bound.Attributes[GameAttribute.Gizmo_State] = 1;
						Hope_Bound.Attributes[GameAttribute.Untargetable] = true;
						Hope_Bound.Attributes.BroadcastChangedIfRevealed();

						foreach (var plr in Library.Players.Values)
							plr.Teleport(new Vector3D(272.52695f, 650.290004f, 15.191491f));
						StartConversation(Library, 217221); // Голос дъябло до битвы
					});
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 33,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //destroy Auriel's jail
					Game.CurrentEncounter.activated = false;
					
					var Library = Game.GetWorld(WorldSno.a4dun_libraryoffate);
					StartConversation(Library, 217223); // Голос дъябло после битвы

					var Hope_Bound = Library.GetActorBySNO(ActorSno._a4dunspire_interactives_hope_bound);
					Hope_Bound.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = false;
					Hope_Bound.Attributes[GameAttribute.Gizmo_State] = 0;
					Hope_Bound.Attributes[GameAttribute.Untargetable] = false;
					Hope_Bound.Attributes.BroadcastChangedIfRevealed();

					ListenInteract(ActorSno._a4dunspire_interactives_hope_bound, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(33, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 38,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Auriel

					var Library = Game.GetWorld(WorldSno.a4dun_libraryoffate);
					var Hope_Bound = Library.GetActorBySNO(ActorSno._a4dunspire_interactives_hope_bound);
					var Hope = Library.SpawnMonster(ActorSno._hope, new Vector3D(Hope_Bound.Position.X - 0.3854f, Hope_Bound.Position.Y + 0.44201f, Hope_Bound.Position.Z));
                    var Fate = Library.SpawnMonster(ActorSno._fate, new Vector3D(Hope_Bound.Position.X - 18.6041f, Hope_Bound.Position.Y + 2.35458f, Hope_Bound.Position.Z));
                    
                    Hope.PlayAnimation(11, AnimationSno.omninpc_female_hope_spawn_01, 1);
                    Fate.PlayAnimation(11, AnimationSno.omninpc_male_fate_spawn_01, 1);

					Hope.Attributes[GameAttribute.MinimapActive] = true;
                    (Hope as InteractiveNPC).Conversations.Clear();
                    (Hope as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(114124));
                    Hope.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					(Hope as InteractiveNPC).OverridedConv = true;
					Hope.Attributes.BroadcastChangedIfRevealed();

					//ListenProximity(114074, new LaunchConversation(114124));
					ListenConversation(114124, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(38, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 42,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //return to Gardens of Hope lv. 1
					PlayCutscene(1);
					var Library = Game.GetWorld(WorldSno.a4dun_libraryoffate);
					var Hope = Library.GetActorBySNO(ActorSno._hope, true);
					(Hope as InteractiveNPC).Conversations.Clear();
					Hope.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					(Hope as InteractiveNPC).OverridedConv = true;
					Hope.Attributes.BroadcastChangedIfRevealed();
					var ExitPortal = Library.GetActorBySNO(ActorSno._g_portal_heaventeal);
					ExitPortal.Hidden = false;
					ExitPortal.SetVisible(true);
					foreach (var plr in Library.Players.Values)
						ExitPortal.Reveal(plr);

					ListenTeleport(109514, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(42, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 44,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find demonic rift
					ListenProximity(ActorSno._a4_heaven_gardens_hellportal, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(44, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 62,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //destroy Eye of Hell
					ListenKill(ActorSno._a4dun_garden_hellportal_pillar, 1, new Advance());
					
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(62, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 50,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //leave demonic rift
					var World = Game.GetWorld(WorldSno.a4dun_hell_portal_01);
					World.SpawnMonster(ActorSno._diablo_vo, World.Players.Values.First().Position);
					StartConversation(World, 217230);
					ListenTeleport(109514, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(50, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 52,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to 2nd lv. of Gardens of Hope
					ListenTeleport(109516, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(52, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 48,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find another demonic rift
					if (Game.Empty) UnlockTeleport(4);
					ListenProximity(ActorSno._a4_heaven_gardens_hellportal, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(48, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 60,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //destroy Eye of Hell
					ListenKill(ActorSno._a4dun_garden_hellportal_pillar, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(60, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 56,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //leave demonic rift
					var World = Game.GetWorld(WorldSno.a4dun_hell_portal_02);
					World.SpawnMonster(ActorSno._diablo_vo, World.Players.Values.First().Position);
					StartConversation(World, 217232);
					ListenTeleport(109516, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(56, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 54,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find portal to Crystal Collonade
					ListenProximity(ActorSno._coreelitedemon_a_nopod_unique, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(54, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 23,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Tyrael
					if (!Game.Empty) UnlockTeleport(4);
					
					var Garden = Game.GetWorld(WorldSno.a4dun_garden_of_hope_random);
					var Tyrael = Garden.GetActorBySNO(ActorSno._tyrael_heaven_spire);
					Tyrael.Attributes[GameAttribute.MinimapActive] = true;
					(Tyrael as InteractiveNPC).Conversations.Clear();
					(Tyrael as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(114131));
					Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					(Tyrael as InteractiveNPC).OverridedConv = true;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					ListenConversation(114131, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(23, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 29,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to Crystal Collonade
					ListenTeleport(119882, new Advance());
				})
			});

			Game.QuestManager.Quests[113910].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //complete
					Game.CurrentEncounter.activated = false;
					PlayCutscene(2);
				})
			});

			#endregion
			#region To the Spire
			Game.QuestManager.Quests.Add(114795, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 114901, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[114795].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 14,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[114795].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find Imperius
					var CrystalWorld = Game.GetWorld(WorldSno.a4dun_garden3_spireentrance);
					
					Game.AddOnLoadWorldAction(WorldSno.a4dun_garden3_spireentrance, () =>
					{
						foreach (var mob in CrystalWorld.GetActorsBySNO(ActorSno._bigred_a))
						{
							(mob as Monster).Brain.DeActivate();
							mob.Attributes[GameAttribute.Untargetable] = true;
						}
						script = new ImperiumScene();
						script.Execute(CrystalWorld);
					});

				})
			});

			Game.QuestManager.Quests[114795].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 16,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Imperius
					//ListenProximity(195606, new LaunchConversation(196579));
					ListenConversation(196579, new Advance());
				})
			});

			Game.QuestManager.Quests[114795].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 12,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to road to Spire
					Game.AddOnLoadWorldAction(WorldSno.a4dun_garden3_spireentrance, () =>
					{
						var CrystalWorld = Game.GetWorld(WorldSno.a4dun_garden3_spireentrance);
						foreach (var plr in CrystalWorld.Players.Values)
						{
							plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
						}

						if (Game.CurrentQuest == 114795 && Game.CurrentStep == 16)
						{
							StartConversation(CrystalWorld, 196566);
						}
					});
					ListenTeleport(198564, new Advance());
				})
			});

			Game.QuestManager.Quests[114795].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region United Evil
			Game.QuestManager.Quests.Add(114901, new Quest { RewardXp = 21000, RewardGold = 1600, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[114901].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 24,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(24, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to Spire exterior
					if (Game.Empty) UnlockTeleport(5);
					ListenTeleport(215396, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 7,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Izual
					if (!Game.Empty) UnlockTeleport(5);
					Game.AddOnLoadWorldAction(WorldSno.a4dun_spire_exterior, () =>
					{
						var world = Game.GetWorld(WorldSno.a4dun_spire_exterior);
						world.SpawnMonster(ActorSno._bigred_izual, new Vector3D { X = 585.439f, Y = 560.823f, Z = 0.1f });
						var iceBarrier = world.GetActorBySNO(ActorSno._a4dunspire_interactives_izual_ice_barrier_a);
						while (iceBarrier != null)
						{
							iceBarrier.Destroy();
							iceBarrier = world.GetActorBySNO(ActorSno._a4dunspire_interactives_izual_ice_barrier_a);
						}
					});
					ListenKill(ActorSno._bigred_izual, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 20,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to Spire entrance (heavens peak)
					if (Game.Empty) UnlockTeleport(6);
					ListenTeleport(205434, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 22,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Tyrael
					if (!Game.Empty) UnlockTeleport(6);
					if (Game.Empty) UnlockTeleport(7);
					ListenProximity(ActorSno._tyrael_heaven_spire, new LaunchConversation(199698));
					ListenConversation(199698, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //go to Crystal Arch
					ListenTeleport(109563, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //find Diablo
					if (!Game.Empty) UnlockTeleport(7);
					Game.AddOnLoadWorldAction(WorldSno.a4dun_diablo_arena, () =>
					{
						Open(Game.GetWorld(WorldSno.a4dun_diablo_arena), ActorSno._a4dun_diablo_bone_gate);
					});
					ListenProximity(ActorSno._a4dun_diablo_bone_gate, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 6,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Diablo (1st phase, to 50% hp)		
					Game.AddOnLoadWorldAction(WorldSno.a4dun_diablo_arena, () =>
					{
						if (Game.CurrentQuest == 114901 && Game.CurrentStep == 1)
						{
							StartConversation(Game.GetWorld(WorldSno.a4dun_diablo_arena), 132632);
						}
					});
					//seems hacky but works
					//ListenTeleport(153669, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 12,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Diablo Shadow (2nd phase)
					ListenKill(ActorSno._terrordiablo, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //kill Diablo (3rd phase)
					var targetWorld = Game.GetWorld(WorldSno.a4dun_diablo_arena_phase3);
					TeleportToWorld(Game.GetWorld(WorldSno.a4dun_diablo_shadowrealm_01), targetWorld, 172);
					StartConversation(targetWorld, 132640);
					ListenKill(ActorSno._diablo, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 17,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //destroy Diablo
					StartConversation(Game.GetWorld(WorldSno.a4dun_diablo_arena_phase3), 205783);
					ListenConversation(205783, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 5,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //talk with Auriel
					Game.GetWorld(WorldSno.a4dun_diablo_arena_phase3).GetActorBySNO(ActorSno._hope).NotifyConversation(1);
					if (Game.IsHardcore)
					{
						foreach (var plr in Game.Players.Values)
							if (!plr.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.HardcoreMasterUnlocked))
								plr.Toon.GameAccount.Flags = plr.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.HardcoreMasterUnlocked;
					}
					else
					{
						foreach (var plr in Game.Players.Values)
							if (!plr.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.MasterUnlocked))
								plr.Toon.GameAccount.Flags = plr.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.MasterUnlocked;
					}
					ListenInteract(ActorSno._hope, 1, new LaunchConversation(199726));
					ListenConversation(199726, new Advance());
				})
			});

			Game.QuestManager.Quests[114901].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { Objective.Default() },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
		}

		protected void TeleportToWorld(MapSystem.World source, MapSystem.World dest, int point)
		{
			foreach (var plr in source.Players.Values)
				plr.ChangeWorld(dest, dest.GetStartingPointById(point).Position);
		}
	}
}
