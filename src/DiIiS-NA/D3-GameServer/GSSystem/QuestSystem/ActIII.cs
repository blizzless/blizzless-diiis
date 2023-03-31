using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Linq;
using System;
using System.Collections.Generic;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using System.Threading.Tasks;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class ActIII : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public ActIII(Game game) : base(game)
		{
		}

		public override void SetQuests()
		{
			#region Siege of the Bastion
			Game.QuestManager.Quests.Add(93595, new Quest { RewardXp = 6200, RewardGold = 580, Completed = false, Saveable = true, NextQuest = 93684, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[93595].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 8,
				OnAdvance = new Action(() => {
					Game.GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetActorBySNO(ActorSno._tyrael_act3, true).NotifyConversation(1);
					ListenInteract(ActorSno._tyrael_act3, 1, new LaunchConversation(204905));
					ListenConversation(204905, new Advance());
				})
			});

			Game.QuestManager.Quests[93595].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				OnAdvance = new Action(() => { //use fire torches
					var world = Game.GetWorld(WorldSno.a3dun_hub_adria_tower_intro);
					ListenInteract(ActorSno._a3dunrmpt_interactives_signal_fire_a, 5, new Advance());
					StartConversation(world, 204915);
					world.GetActorBySNO(ActorSno._tyrael_act3, true).NotifyConversation(0);
				})
			});

			Game.QuestManager.Quests[93595].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //find sergeant Dalen
					UnlockTeleport(0);
					ListenProximity(ActorSno._bastionskeepguard_melee_b_02_sgt_dalen, new LaunchConversation(196152));
					ListenConversation(196152, new Advance());
					if (Game.Empty) UnlockTeleport(1);
				})
			});

			Game.QuestManager.Quests[93595].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Raise the catapults
			Game.QuestManager.Quests.Add(93684, new Quest { RewardXp = 9000, RewardGold = 980, Completed = false, Saveable = true, NextQuest = 93697, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[93684].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[93684].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 20,
				Objectives = new List<Objective> { new Objective { Limit = 3, Counter = 0 } },
				OnAdvance = new Action(() => { //use 3 catapults
					Game.AddOnLoadWorldAction(WorldSno.a3dun_rmpt_level02, () =>
					{
						var world = Game.GetWorld(WorldSno.a3dun_rmpt_level02);
						if (Game.CurrentQuest == 93684 && Game.CurrentStep == 18)
						{
							//StartConversation(this.Game.GetWorld(81019), 106160);
						}
						world.GetActorBySNO(ActorSno._bastionskeepguard_melee_a_02_event_injured_catapult_follower).NotifyConversation(2);
						world.GetActorBySNO(ActorSno._a3_rampart_guard_captain_alt).NotifyConversation(2);
						world.GetActorBySNO(ActorSno._act3_melee_soldier_manual_captain).NotifyConversation(2);
					});
					ListenInteract(ActorSno._bastionskeepguard_melee_a_02_event_injured_catapult_follower, 1, new FirstCatapult()); //followers
					ListenInteract(ActorSno._a3_rampart_guard_captain_alt, 1, new SecondCatapult());
					ListenInteract(ActorSno._act3_melee_soldier_manual_captain, 1, new LastCatapult());
				})
			});

			Game.QuestManager.Quests[93684].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				OnAdvance = new Action(() => { //return to base
					UnlockTeleport(1);
					ListenProximity(ActorSno._tyrael_act3, new Advance());
				})
			});

			Game.QuestManager.Quests[93684].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Bastion breach
			Game.QuestManager.Quests.Add(93697, new Quest { RewardXp = 2475, RewardGold = 300, Completed = false, Saveable = true, NextQuest = 203595, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[93697].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 20,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				OnAdvance = new Action(() => { //go to 2nd level of bastion keep
					ListenTeleport(93103, new Advance());
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 22,
				OnAdvance = new Action(() => { //find breach on 2nd level
					if (Game.Empty)
                    {
						var world = Game.GetWorld(WorldSno.a3dun_hub_keep);
						while (world.GetActorBySNO(ActorSno._demontrooper_a, true) != null)
							world.GetActorBySNO(ActorSno._demontrooper_a, true).Destroy();
                    }

                    UnlockTeleport(2);
					ListenTeleport(136448, new Advance());
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				OnAdvance = new Action(() => { //find bastion's ambar (gluttony boss)
					ListenTeleport(111232, new Advance());
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 16,
				OnAdvance = new Action(() => { //kill gluttony
					UnlockTeleport(3);
					Game.AddOnLoadWorldAction(WorldSno.gluttony_boss, () =>
					{
						if (Game.CurrentQuest == 93697)
						{
							var world = Game.GetWorld(WorldSno.gluttony_boss);
							// TODO: extract this static method as extension
							ActII.DisableEveryone(world, true);
							//Старт катсцены
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSno._gluttony).DynamicID(plr), Duration = 1f, Snap = false });
							StartConversation(world, 137018);
						}
					});
					ListenConversation(137018, new EndCutScene());
					ListenKill(ActorSno._gluttony, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //return to base
					ListenProximity(ActorSno._tyrael_act3, new Advance());
				})
			});

			Game.QuestManager.Quests[93697].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Stone shake
			Game.QuestManager.Quests.Add(203595, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 101756, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[203595].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				OnAdvance = new Action(() => {
					var Tyrael = Game.GetWorld(WorldSno.a3dun_hub_keep).GetActorBySNO(ActorSno._tyrael_act3);
					(Tyrael as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(183792));
					Tyrael.Attributes[GameAttributes.Conversation_Icon, 0] = 2;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					ListenConversation(183792, new Advance());
				})
			});

			Game.QuestManager.Quests[203595].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				OnAdvance = new Action(() => { //go to Armory
					var Tyrael = Game.GetWorld(WorldSno.a3dun_hub_keep).GetActorBySNO(ActorSno._tyrael_act3);
					(Tyrael as InteractiveNPC).Conversations.Clear();     
					Tyrael.Attributes[GameAttributes.Conversation_Icon, 0] = 1;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					ListenTeleport(185228, new Advance());
				})
			});

			Game.QuestManager.Quests[203595].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				OnAdvance = new Action(() => { //kill shadows
					Game.AddOnLoadWorldAction(WorldSno.a3dun_keep_hub_inn, () =>
					{
						var world = Game.GetWorld(WorldSno.a3dun_keep_hub_inn);
						bool Activated = false;
						var NStone = world.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone);//156328
						NStone.Attributes[GameAttributes.Team_Override] = (Activated ? -1 : 2);
						NStone.Attributes[GameAttributes.Untargetable] = !Activated;
						NStone.Attributes[GameAttributes.NPC_Is_Operatable] = Activated;
						NStone.Attributes[GameAttributes.Operatable] = Activated;
						NStone.Attributes[GameAttributes.Operatable_Story_Gizmo] = Activated;
						NStone.Attributes[GameAttributes.Disabled] = !Activated;
						NStone.Attributes[GameAttributes.Immunity] = !Activated;
						NStone.Attributes.BroadcastChangedIfRevealed();
						NStone.PlayEffectGroup(205460);
						foreach (var atr in world.GetActorsBySNO(ActorSno._leah))
						{
							float facingAngle = MovementHelpers.GetFacingAngle(atr, NStone);

							atr.PlayActionAnimation(AnimationSno.leah_channel_01);
							
							//atr.PlayEffectGroup(205460); //Add Rope channel to NStone
							atr.SetFacingRotation(facingAngle);
						}
						if (Game.CurrentQuest == 203595)
						{
							ActII.DisableEveryone(world, true);
							//Старт катсцены
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone).DynamicID(plr), Duration = 1f, Snap = false });
							StartConversation(world, 134282);
						}
					});
					ListenConversation(134282, new SpawnShadows());
					ListenKill(ActorSno._shadowvermin_soulstoneevent, 8, new Advance());
				})
			});

			Game.QuestManager.Quests[203595].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 8,
				OnAdvance = new Action(() => { //talk with Leah
					ListenProximity(ActorSno._leah, new LaunchConversation(134266));
					ListenConversation(134266, new Advance());
					try
					{
						Game.GetWorld(WorldSno.a3dun_hub_keep).FindActorAt(ActorSno._a3dun_hub_drawbridge_01, new Vector3D { X = 127.121f, Y = 353.211f, Z = 0.22f }, 25f).Hidden = true;
						var world = Game.GetWorld(WorldSno.a3dun_keep_hub_inn);
						var NStone = world.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone);//156328
						foreach (var atr in world.GetActorsBySNO(ActorSno._leah))
						{
							float facingAngle = MovementHelpers.GetFacingAngle(atr, NStone);
							atr.SetFacingRotation(facingAngle);
						}
					}
					catch { }
				})
			});

			Game.QuestManager.Quests[203595].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					PlayCutscene(1);
				})
			});

			#endregion
			#region Machines of War
			Game.QuestManager.Quests.Add(101756, new Quest { RewardXp = 9075, RewardGold = 900, Completed = false, Saveable = true, NextQuest = 101750, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[101756].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 4,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				OnAdvance = new Action(() => { //go to battlefields
					ListenTeleport(154644, new Advance());
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 9,
				OnAdvance = new Action(() => { //talk with sergeant Pale
					var Serge = Game.GetWorld(WorldSno.a3_battlefields_02).GetActorBySNO(ActorSno._a3_battlefield_guard_sargeant);
					(Serge as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(170486));
					Serge.Attributes[GameAttributes.Conversation_Icon, 0] = 1;
					//this.Game.GetWorld(95804).SpawnMonster(202730, new Vector3D(4394.2188f, 396.80215f, -2.293509f));
					//ListenConversation(170486, new LaunchConversation(202735));
					ListenConversation(170486, new Advance());
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(9, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //go through Korsikk bridge
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						if (Game.CurrentQuest == 101756 && Game.CurrentStep == 9)
						{
							StartConversation(Game.GetWorld(WorldSno.a3_battlefields_02), 187146);
						}
					});
					ListenProximity(ActorSno._waypoint, new Advance());
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //kill 3 ballistas/destroy trebuchet
					if (Game.Empty) UnlockTeleport(4);
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						Open(Game.GetWorld(WorldSno.a3_battlefields_02), ActorSno._a3_battlefield_guardcatapult_door);
					});
					ListenKill(ActorSno._a3_battlefield_demonic_ballista, 2, new CompleteObjective(0));
					ListenKill(ActorSno._a3battlefield_demon_trebuchetdevice, 1, new CompleteObjective(1));
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 21,
				OnAdvance = new Action(() => { //find Rakkis bridge
					if (!Game.Empty) UnlockTeleport(4);
					//69504
					//ListenTeleport(69504, new Advance());
					ListenProximity(ActorSno._tyrael_act3, new Advance());
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						if (Game.CurrentQuest == 101756 && Game.CurrentStep == 21)
						{
							var Tyrael = Game.GetWorld(WorldSno.a3_battlefields_02).GetActorBySNO(ActorSno._tyrael_act3);
							(Tyrael as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(209125));
							//StartConversation(this.Game.GetWorld(95804), 209125);
						}
					});
					ListenConversation(209125, new Advance());
				})
			});

			Game.QuestManager.Quests[101756].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Assault Beast
			Game.QuestManager.Quests.Add(101750, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 101758, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[101750].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[101750].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				OnAdvance = new Action(() => { //find demonic gates to Siegebreaker
					if (Game.Empty) UnlockTeleport(5);
					ListenProximity(ActorSno._a3dun_crater_st_demon_chainpylon_fire_azmodan, new Advance());
					ListenTeleport(112580, new Advance());
				})
			});

			Game.QuestManager.Quests[101750].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 17,
				OnAdvance = new Action(() => { //kill Siegebreaker
					if (!Game.Empty) UnlockTeleport(5);
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_03, () =>
					{
						var world = Game.GetWorld(WorldSno.a3_battlefields_03);
						try { world.GetActorBySNO(ActorSno._siegebreakerdemon).Destroy(); } catch { }
						world.SpawnMonster(ActorSno._siegebreakerdemon, world.GetActorBySNO(ActorSno._adria, true).Position);
					});
					ListenKill(ActorSno._siegebreakerdemon, 1, new Advance());
				})
			});

			Game.QuestManager.Quests[101750].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Adria
					Game.CurrentEncounter.Activated = false;
					ListenProximity(ActorSno._adria, new LaunchConversation(196366));
					ListenConversation(196366, new Advance());
					if (Game.Empty) UnlockTeleport(6);
				})
			});

			Game.QuestManager.Quests[101750].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					PlayCutscene(3);
				})
			});

			#endregion
			#region Heart of Sin
			Game.QuestManager.Quests.Add(101758, new Quest { RewardXp = 24600, RewardGold = 1535, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.Quests[101758].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 41,
				OnAdvance = new Action(() => { //find Tower of the Doomed lv. 1
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_03, () =>
					{
						Open(Game.GetWorld(WorldSno.a3_battlefields_03), ActorSno._a3_battlefield_siegebreakergate_a);
					});
					Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						Open(Game.GetWorld(WorldSno.a3_battlefields_02), ActorSno._a3_battlefield_siegebreakergate_a);
					});
					ListenTeleport(80791, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(41, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 25,
				OnAdvance = new Action(() => { //find Heart of Sin
					if (!Game.Empty) UnlockTeleport(6);
					if (Game.Empty) UnlockTeleport(7);
					ListenTeleport(85202, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(25, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 14,
				Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
				OnAdvance = new Action(() => { //kill Daughters of Pain / Destroy Heart of Sin
					if (!Game.Empty) UnlockTeleport(7);
					ListenKill(ActorSno._succubus_daughterofpain, 3, new CompleteObjective(0));
					ListenKill(ActorSno._a3dun_crater_st_giantdemonheart_mob, 1, new CompleteObjective(1));
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 29,
				OnAdvance = new Action(() => { //find Tower of Damned lv. 1
					if (Game.Empty) UnlockTeleport(8);
					Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04, () =>
					{
						Open(Game.GetWorld(WorldSno.a3dun_crater_st_level04), ActorSno._a3dun_battlefield_demon_chainpylon_locked);
					});
					ListenTeleport(119653, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 23,
				OnAdvance = new Action(() => { //find Heart of Sin
					if (!Game.Empty) UnlockTeleport(8);
					if (Game.Empty) UnlockTeleport(9);
					ListenTeleport(119656, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(23, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 27,
				OnAdvance = new Action(() => { //kill Cydaea
					if (!Game.Empty) UnlockTeleport(9);
					ListenKill(ActorSno._mistressofpain, 1, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04b, () =>
					{
						try
						{
							var world = Game.GetWorld(WorldSno.a3dun_crater_st_level04b);
							(world.FindActorAt(ActorSno._a3dun_crater_st_demon_chainpylon_fire_mistressofpain, new Vector3D { X = 457.04f, Y = 359.03f, Z = 0.39f }, 20f) as Door).Open();
							(world.FindActorAt(ActorSno._a3dun_crater_st_demon_chainpylon_fire_mistressofpain, new Vector3D { X = 356.04f, Y = 267.03f, Z = 0.28f }, 20f) as Door).Open();
							SetActorOperable(world, ActorSno._a3dun_crater_st_giantdemonheart_mob, false);
						}
						catch { }
					});
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(27, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				OnAdvance = new Action(() => { //Destroy Heart of Sin
					Game.CurrentEncounter.Activated = false;
					ListenKill(ActorSno._a3dun_crater_st_giantdemonheart_mob, 1, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04b, () =>
					{
						try
						{
							SetActorOperable(Game.GetWorld(WorldSno.a3dun_crater_st_level04b), ActorSno._a3dun_crater_st_giantdemonheart_mob, true);
						}
						catch { }
					});
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 32,
				OnAdvance = new Action(() => { //kill Azmodan, finally
					if (Game.Empty) UnlockTeleport(10);
					ListenKill(ActorSno._azmodan, 1, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						var world = Game.GetWorld(WorldSno.a3dun_azmodan_arena);
						Open(world, ActorSno._a3dun_crater_st_demon_chainpylon_fire_azmodan);
						try { world.GetActorBySNO(ActorSno._azmodan).Destroy(); } catch { };
						world.SpawnMonster(ActorSno._azmodan, new Vector3D { X = 395.553f, Y = 394.966f, Z = 0.1f });
					});
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(32, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 5,
				OnAdvance = new Action(() => { //get Azmodan's soul
					Game.CurrentEncounter.Activated = false;
					ListenProximity(ActorSno._azmodan_bss_soulremnants, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						Game.GetWorld(WorldSno.a3dun_azmodan_arena).SpawnMonster(ActorSno._azmodan_bss_soulremnants, new Vector3D { X = 395.553f, Y = 394.966f, Z = 0.1f });
					});
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 39,
				OnAdvance = new Action(() => { //return to base
					ListenProximity(ActorSno._tyrael_act3, new Advance());
					Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						Game.GetWorld(WorldSno.a3dun_azmodan_arena).GetActorBySNO(ActorSno._azmodan_bss_soulremnants).Destroy();
					});
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(39, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 46,
				OnAdvance = new Action(() => { //talk with leutenant Lavaile
					ListenProximity(ActorSno._a3_rampart_guard_captain_alt, new LaunchConversation(160644));
					ListenConversation(160644, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(46, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 34,
				OnAdvance = new Action(() => { //go to Armory
					ListenTeleport(185228, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 36,
				OnAdvance = new Action(() =>
				{ //go to Adria tower event
					var World = Game.GetWorld(WorldSno.a3dun_hub_adria_tower);
					//Удаляем дубликаты 
					var Guardian = World.GetActorBySNO(ActorSno._bastionskeepguard_event47, true);
					var Leah = World.GetActorBySNO(ActorSno._leah_event47, true);
					var Tyrael = World.GetActorBySNO(ActorSno._tyrael_event47, true);
					var Adria = World.GetActorBySNO(ActorSno._adria_event47, true);

					foreach (var actor in World.GetActorsBySNO(ActorSno._tyrael_event47)) if (actor.GlobalID != Tyrael.GlobalID) actor.Destroy(); //Тираэль
					foreach (var actor in World.GetActorsBySNO(ActorSno._leah_event47)) if (actor.GlobalID != Leah.GlobalID) actor.Destroy(); //Лея
					
					Game.AddOnLoadWorldAction(WorldSno.a3dun_hub_adria_tower, () =>
					{
						var portal = World.GetActorBySNO(ActorSno._townportal_red);
						var Bportal = World.GetActorBySNO(ActorSno._event47_bigportal);
						
						//Прячем порталы
						foreach (var plr in World.Players.Values) { Bportal.Unreveal(plr); portal.Unreveal(plr); }
						portal.SetVisible(false); portal.Hidden = true;
						Bportal.SetVisible(false); Bportal.Hidden = true;
						
						//Черный камень душ, нельзя! ТРОГАТЬ! ФУ! БРЫСЬ!
						bool Activated = false;
						var NStone = World.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone);//156328
						NStone.Attributes[GameAttributes.Team_Override] = (Activated ? -1 : 2);
						NStone.Attributes[GameAttributes.Untargetable] = !Activated;
						NStone.Attributes[GameAttributes.NPC_Is_Operatable] = Activated;
						NStone.Attributes[GameAttributes.Operatable] = Activated;
						NStone.Attributes[GameAttributes.Operatable_Story_Gizmo] = Activated;
						NStone.Attributes[GameAttributes.Disabled] = !Activated;
						NStone.Attributes[GameAttributes.Immunity] = !Activated;
						NStone.Attributes.BroadcastChangedIfRevealed();
						//Участники сцены
						foreach (var plr in World.Players.Values)
						{
							Guardian.Reveal(plr);
							Leah.Reveal(plr);
							Tyrael.Reveal(plr);
							Adria.Reveal(plr);
						}

						script = new LeahTransformation_Line1();
						Task.Run(() => 
						{
							while (true)
							{
								if (World.Players.Values.First().Position.Y < 140)
									break;
							}
							script.Execute(World);
						});
						#region Описание скрипта
						//Понеслась
						/*
						1 - [195719] Event47_Line_1 - Тираэль - лежит
						2 - [195721] Event47_Line_2 - Адрия - Подходит к Тираэлю
						3 - [195723] Event47_Line_3 - Адрия - Хватает Тираэля, и дальше говорит
							Скилл - [199222] [Power] Adria_event47_blast
							[207601] [EffectGroup] adria_event47_channel_right_hand
						4 - [195725] Event47_Line_4 - Тираэль
						5 - [195739] Event47_Line_5 - Адрия - после диалога поднимает руку для атаки
						6 - [195741] Event47_Line_6 - Адрия - говорит и после откидывает Тираэля, Камера переходит к камню Душ
							Атака Адрии - [199220] [Actor] Adria_event47_projectile
							Скилл Адрии - [199198] [Power] Adria_event47_projectile
						7 - [195743] Event47_Line_7 - Адрия - подходит к камню
						8 - [195745] Event47_Line_8 - Адрия - Начинает кастовать, камень поднимается, а Лию начинает колбасить, после диалога зажигается круг с пентой и отдаляется камера, через секунд взрыв.
							Анимации круга
								[194705] [Anim] emitter_event47_groundRune_stage01
								[194706] [Anim] emitter_event47_groundRune_stage02
								[194707] [Anim] emitter_event47_groundRune_stage03
								[194709] [Anim] emitter_event47_groundRune_stage04
							Взрыв - [193605] [Actor] event47_transformExplosion
							Лею колбасит - [205941] [Anim] Leah_BSS_event_bound_shake
						9 - [195749] Event47_Line_9 - Лея(Диабло) - камера приближается, ЗЛО ВСТАЁТ С КОЛЕН!
							[194492] [Anim] Leah_BSS_event_kneel_to_getup
						10 -[195764] Event47_Line_10 - Лея(Диабло) - Стоит
						11 -[195767] Event47_Line_11 - Лея(Диабло) - Я голова от черноголовки - эффект злого лвлапа. и Лея получает красный ореол
							[208454] [Anim] Leah_BSS_event_lvlUp_in
							[201990] [Anim] Leah_BSS_event_lvlUp
						12 -[195769] Event47_Line_12 - Лея(Диабло) - После диалога, Адрия отходит к посиции малого портала, открывает его, и уходит.
						13 -[195776] Event47_Line_13 - Лея(Диабло)  - Лея идёт к позиции большого портала, малый портал закрывается, через 7 секунд, анимация открытия портала
							[194490] [Anim] Leah_BSS_event_open_Portal -> через 1.5 секунды, открыть большой портал, и завершить анимацию через [208444] [Anim] Leah_BSS_event_open_Portal_out
						*/
						#endregion
					});
					//Диалог тираеля и Адрии, уползающий охранник (Нахер, просто швыряем поца)
					//ListenTeleport(201250, new LaunchConversationWithCutScene(195719, Tyrael.ActorSNO.Id));
					ListenConversation(195719, new LeahTransformation_Line2());
					//Смерть охраника PlayAnimation 206664(Отлёт)->211841(СМЕРТ)
					Guardian.PlayActionAnimation(AnimationSno.omninpc_stranger_bss_event_crouching_knockback_intro);
					Guardian.PlayActionAnimation(AnimationSno.omninpc_male_hth_crawl_event47_death_01);
					ListenConversation(195721, new LeahTransformation_Line3());
					ListenConversation(195723, new LaunchConversation(195725)); // Line4
					ListenConversation(195725, new LaunchConversation(195739)); // Line5
																				//TODO: Адрия поднимает руку
					ListenConversation(195739, new LeahTransformation_Line6());
					ListenConversation(195741, new LeahTransformation_Line7());
					ListenConversation(195743, new LeahTransformation_Line8());
					//Ритуал
					ListenConversation(195745, new LeahTransformation_Ritual()); //После, старт 9 диалога
					ListenConversation(195749, new LeahTransformation_Line10());
					ListenConversation(195764, new LeahTransformation_Line11());
					ListenConversation(195767, new LeahTransformation_Line12()); //После Диалога отправить Адрию в портал (Нужно будет объединить с 13 лайном)
					ListenConversation(195769, new LeahTransformation_Line13());
					// Лея - я теряю контроль - 198760
					//ListenTeleport(201250, new Advance());
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(36, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 4,
				OnAdvance = new Action(() => { //use Heaven portal
					ListenInteract(ActorSno._event47_bigportal, 1, new ChangeAct(ActEnum.Act4));
				})
			});

			Game.QuestManager.Quests[101758].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					Game.CurrentEncounter.Activated = false;
				})
			});

			#endregion
		}
	}
}
