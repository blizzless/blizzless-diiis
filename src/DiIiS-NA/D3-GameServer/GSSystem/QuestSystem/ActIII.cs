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
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
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
			this.Game.QuestManager.Quests.Add(93595, new Quest { RewardXp = 6200, RewardGold = 580, Completed = false, Saveable = true, NextQuest = 93684, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[93595].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 8,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					this.Game.GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetActorBySNO(170038, true).NotifyConversation(1);
					ListenInteract(170038, 1, new LaunchConversation(204905));
					ListenConversation(204905, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93595].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 26,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //use fire torches
					var world = this.Game.GetWorld(WorldSno.a3dun_hub_adria_tower_intro);
					ListenInteract(196211, 5, new Advance());
					StartConversation(world, 204915);
					world.GetActorBySNO(170038, true).NotifyConversation(0);
				})
			});

			this.Game.QuestManager.Quests[93595].Steps.Add(26, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find sergeant Dalen
					UnlockTeleport(0);
					ListenProximity(196150, new LaunchConversation(196152));
					ListenConversation(196152, new Advance());
					if (this.Game.Empty) UnlockTeleport(1);
				})
			});

			this.Game.QuestManager.Quests[93595].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Raise the catapults
			this.Game.QuestManager.Quests.Add(93684, new Quest { RewardXp = 9000, RewardGold = 980, Completed = false, Saveable = true, NextQuest = 93697, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[93684].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[93684].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 20,
				Objectives = new List<Objective> { new Objective { Limit = 3, Counter = 0 } },
				OnAdvance = new Action(() => { //use 3 catapults
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_rmpt_level02, () =>
					{
						var world = this.Game.GetWorld(WorldSno.a3dun_rmpt_level02);
						if (this.Game.CurrentQuest == 93684 && this.Game.CurrentStep == 18)
						{
							//StartConversation(this.Game.GetWorld(81019), 106160);
						}
						world.GetActorBySNO(154137).NotifyConversation(2);
						world.GetActorBySNO(162406).NotifyConversation(2);
						world.GetActorBySNO(149810).NotifyConversation(2);
					});
					ListenInteract(154137, 1, new FirstCatapult()); //followers
					ListenInteract(162406, 1, new SecondCatapult());
					ListenInteract(149810, 1, new LastCatapult());
				})
			});

			this.Game.QuestManager.Quests[93684].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //return to base
					UnlockTeleport(1);
					ListenProximity(170038, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93684].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Bastion breach
			this.Game.QuestManager.Quests.Add(93697, new Quest { RewardXp = 2475, RewardGold = 300, Completed = false, Saveable = true, NextQuest = 203595, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[93697].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 20,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //go to 2nd level of bastion keep
					ListenTeleport(93103, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 22,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find breach on 2nd level
					if (this.Game.Empty)
                    {
						var world = this.Game.GetWorld(WorldSno.a3dun_hub_keep);
						while (world.GetActorBySNO(77796, true) != null)
							world.GetActorBySNO(77796, true).Destroy();
                    }

                    UnlockTeleport(2);
					ListenTeleport(136448, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find bastion's ambar (gluttony boss)
					ListenTeleport(111232, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 16,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill gluttony
					UnlockTeleport(3);
					this.Game.AddOnLoadWorldAction(WorldSno.gluttony_boss, () =>
					{
						if (this.Game.CurrentQuest == 93697)
						{
							var world = this.Game.GetWorld(WorldSno.gluttony_boss);
							// TODO: extract this static method as extension
							ActII.DisableEveryone(world, true);
							//Старт катсцены
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(87642).DynamicID(plr), Duration = 1f, Snap = false });
							StartConversation(world, 137018);
						}
					});
					ListenConversation(137018, new EndCutScene());
					ListenKill(87642, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //return to base
					ListenProximity(170038, new Advance());
				})
			});

			this.Game.QuestManager.Quests[93697].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Stone shake
			this.Game.QuestManager.Quests.Add(203595, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 101756, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[203595].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					var Tyrael = this.Game.GetWorld(WorldSno.a3dun_hub_keep).GetActorBySNO(170038);
					(Tyrael as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(183792));
					Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					ListenConversation(183792, new Advance());
				})
			});

			this.Game.QuestManager.Quests[203595].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //go to Armory
					var Tyrael = this.Game.GetWorld(WorldSno.a3dun_hub_keep).GetActorBySNO(170038);
					(Tyrael as InteractiveNPC).Conversations.Clear();     
					Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					Tyrael.Attributes.BroadcastChangedIfRevealed();
					ListenTeleport(185228, new Advance());
				})
			});

			this.Game.QuestManager.Quests[203595].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill shadows
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_keep_hub_inn, () =>
					{
						var world = this.Game.GetWorld(WorldSno.a3dun_keep_hub_inn);
						bool Activated = false;
						var NStone = world.GetActorBySNO(156328);//156328
						NStone.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
						NStone.Attributes[GameAttribute.Untargetable] = !Activated;
						NStone.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
						NStone.Attributes[GameAttribute.Operatable] = Activated;
						NStone.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
						NStone.Attributes[GameAttribute.Disabled] = !Activated;
						NStone.Attributes[GameAttribute.Immunity] = !Activated;
						NStone.Attributes.BroadcastChangedIfRevealed();
						NStone.PlayEffectGroup(205460);
						foreach (var atr in world.GetActorsBySNO(4580))
						{
							float facingAngle = MovementHelpers.GetFacingAngle(atr, NStone);

							atr.PlayActionAnimation(139775);
							
							//atr.PlayEffectGroup(205460); //Add Rope channel to NStone
							atr.SetFacingRotation(facingAngle);
						}
						if (this.Game.CurrentQuest == 203595)
						{
							ActII.DisableEveryone(world, true);
							//Старт катсцены
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)world.GetActorBySNO(156328).DynamicID(plr), Duration = 1f, Snap = false });
							StartConversation(world, 134282);
						}
					});
					ListenConversation(134282, new SpawnShadows());
					ListenKill(201921, 8, new Advance());
				})
			});

			this.Game.QuestManager.Quests[203595].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 8,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Leah
					ListenProximity(4580, new LaunchConversation(134266));
					ListenConversation(134266, new Advance());
					try
					{
						this.Game.GetWorld(WorldSno.a3dun_hub_keep).FindAt(206188, new Vector3D { X = 127.121f, Y = 353.211f, Z = 0.22f }, 25f).Hidden = true;
						var world = this.Game.GetWorld(WorldSno.a3dun_keep_hub_inn);
						var NStone = world.GetActorBySNO(156328);//156328
						foreach (var atr in world.GetActorsBySNO(4580))
						{
							float facingAngle = MovementHelpers.GetFacingAngle(atr, NStone);
							atr.SetFacingRotation(facingAngle);
						}
					}
					catch { }
				})
			});

			this.Game.QuestManager.Quests[203595].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					PlayCutscene(1);
				})
			});

			#endregion
			#region Machines of War
			this.Game.QuestManager.Quests.Add(101756, new Quest { RewardXp = 9075, RewardGold = 900, Completed = false, Saveable = true, NextQuest = 101750, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[101756].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //go to battlefields
					ListenTeleport(154644, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 9,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with sergeant Pale
					var Serge = this.Game.GetWorld(WorldSno.a3_battlefields_02).GetActorBySNO(170482);
					(Serge as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(170486));
					Serge.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					//this.Game.GetWorld(95804).SpawnMonster(202730, new Vector3D(4394.2188f, 396.80215f, -2.293509f));
					//ListenConversation(170486, new LaunchConversation(202735));
					ListenConversation(170486, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(9, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //go through Korsikk bridge
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						if (this.Game.CurrentQuest == 101756 && this.Game.CurrentStep == 9)
						{
							StartConversation(this.Game.GetWorld(WorldSno.a3_battlefields_02), 187146);
						}
					});
					ListenProximity(6442, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 }, new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill 3 ballistas/destroy trebuchet
					if (this.Game.Empty) UnlockTeleport(4);
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						Open(this.Game.GetWorld(WorldSno.a3_battlefields_02), 182443);
					});
					ListenKill(176988, 2, new CompleteObjective(0));
					ListenKill(177041, 1, new CompleteObjective(1));
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 21,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Rakkis bridge
					if (!this.Game.Empty) UnlockTeleport(4);
					//69504
					//ListenTeleport(69504, new Advance());
					ListenProximity(170038, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						if (this.Game.CurrentQuest == 101756 && this.Game.CurrentStep == 21)
						{
							var Tyrael = this.Game.GetWorld(WorldSno.a3_battlefields_02).GetActorBySNO(170038);
							(Tyrael as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(209125));
							//StartConversation(this.Game.GetWorld(95804), 209125);
						}
					});
					ListenConversation(209125, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101756].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});

			#endregion
			#region Assault Beast
			this.Game.QuestManager.Quests.Add(101750, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = 101758, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[101750].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[101750].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find demonic gates to Siegebreaker
					if (this.Game.Empty) UnlockTeleport(5);
					ListenProximity(198977, new Advance());
					ListenTeleport(112580, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101750].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 17,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill Siegebreaker
					if (!this.Game.Empty) UnlockTeleport(5);
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_03, () =>
					{
						var world = this.Game.GetWorld(WorldSno.a3_battlefields_03);
						try { world.GetActorBySNO(96192).Destroy(); } catch { }
						world.SpawnMonster(96192, world.GetActorBySNO(3095, true).Position);
					});
					ListenKill(96192, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101750].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Adria
					this.Game.CurrentEncounter.activated = false;
					ListenProximity(3095, new LaunchConversation(196366));
					ListenConversation(196366, new Advance());
					if (this.Game.Empty) UnlockTeleport(6);
				})
			});

			this.Game.QuestManager.Quests[101750].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					PlayCutscene(3);
				})
			});

			#endregion
			#region Heart of Sin
			this.Game.QuestManager.Quests.Add(101758, new Quest { RewardXp = 24600, RewardGold = 1535, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[101758].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 41,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Tower of the Doomed lv. 1
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_03, () =>
					{
						Open(this.Game.GetWorld(WorldSno.a3_battlefields_03), 155128);
					});
					this.Game.AddOnLoadWorldAction(WorldSno.a3_battlefields_02, () =>
					{
						Open(this.Game.GetWorld(WorldSno.a3_battlefields_02), 155128);
					});
					ListenTeleport(80791, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(41, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 25,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Heart of Sin
					if (!this.Game.Empty) UnlockTeleport(6);
					if (this.Game.Empty) UnlockTeleport(7);
					ListenTeleport(85202, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(25, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 14,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 }, new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill Daughters of Pain / Destroy Heart of Sin
					if (!this.Game.Empty) UnlockTeleport(7);
					ListenKill(152535, 3, new CompleteObjective(0));
					ListenKill(193077, 1, new CompleteObjective(1));
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 29,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Tower of Damned lv. 1
					if (this.Game.Empty) UnlockTeleport(8);
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04, () =>
					{
						Open(this.Game.GetWorld(WorldSno.a3dun_crater_st_level04), 177040);
					});
					ListenTeleport(119653, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 23,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Heart of Sin
					if (!this.Game.Empty) UnlockTeleport(8);
					if (this.Game.Empty) UnlockTeleport(9);
					ListenTeleport(119656, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(23, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 27,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill Cydaea
					if (!this.Game.Empty) UnlockTeleport(9);
					ListenKill(95250, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04b, () =>
					{
						try
						{
							var world = this.Game.GetWorld(WorldSno.a3dun_crater_st_level04b);
							(world.FindAt(201680, new Vector3D { X = 457.04f, Y = 359.03f, Z = 0.39f }, 20f) as Door).Open();
							(world.FindAt(201680, new Vector3D { X = 356.04f, Y = 267.03f, Z = 0.28f }, 20f) as Door).Open();
							setActorOperable(world, 193077, false);
						}
						catch { }
					});
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(27, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //Destroy Heart of Sin
					this.Game.CurrentEncounter.activated = false;
					ListenKill(193077, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_crater_st_level04b, () =>
					{
						try
						{
							setActorOperable(this.Game.GetWorld(WorldSno.a3dun_crater_st_level04b), 193077, true);
						}
						catch { }
					});
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 32,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill Azmodan, finally
					if (this.Game.Empty) UnlockTeleport(10);
					ListenKill(89690, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						var world = this.Game.GetWorld(WorldSno.a3dun_azmodan_arena);
						OpenAll(world, 198977);
						try { world.GetActorBySNO(89690).Destroy(); } catch { };
						world.SpawnMonster(89690, new Vector3D { X = 395.553f, Y = 394.966f, Z = 0.1f });
					});
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(32, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 5,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //get Azmodan's soul
					this.Game.CurrentEncounter.activated = false;
					ListenProximity(204992, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						this.Game.GetWorld(WorldSno.a3dun_azmodan_arena).SpawnMonster(204992, new Vector3D { X = 395.553f, Y = 394.966f, Z = 0.1f });
					});
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 39,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //return to base
					ListenProximity(170038, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_azmodan_arena, () =>
					{
						this.Game.GetWorld(WorldSno.a3dun_azmodan_arena).GetActorBySNO(204992).Destroy();
					});
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(39, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 46,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with leutenant Lavaile
					ListenProximity(162406, new LaunchConversation(160644));
					ListenConversation(160644, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(46, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 34,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //go to Armory
					ListenTeleport(185228, new Advance());
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 36,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() =>
				{ //go to Adria tower event
					var World = this.Game.GetWorld(WorldSno.a3dun_hub_adria_tower);
					//Удаляем дубликаты 
					var Guardian = World.GetActorBySNO(196244, true);
					var Leah = World.GetActorBySNO(195376, true);
					var Tyrael = World.GetActorBySNO(195377, true);
					var Adria = World.GetActorBySNO(195378, true);

					foreach (var actor in World.GetActorsBySNO(195377)) if (actor.GlobalID != Tyrael.GlobalID) actor.Destroy(); //Тираэль
					foreach (var actor in World.GetActorsBySNO(195376)) if (actor.GlobalID != Leah.GlobalID) actor.Destroy(); //Лея
					
					this.Game.AddOnLoadWorldAction(WorldSno.a3dun_hub_adria_tower, () =>
					{
						var portal = World.GetActorBySNO(5660);
						var Bportal = World.GetActorBySNO(188441);
						
						//Прячем порталы
						foreach (var plr in World.Players.Values) { Bportal.Unreveal(plr); portal.Unreveal(plr); }
						portal.SetVisible(false); portal.Hidden = true;
						Bportal.SetVisible(false); Bportal.Hidden = true;
						
						//Черный камень душ, нельзя! ТРОГАТЬ! ФУ! БРЫСЬ!
						bool Activated = false;
						var NStone = World.GetActorBySNO(156328);//156328
						NStone.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
						NStone.Attributes[GameAttribute.Untargetable] = !Activated;
						NStone.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
						NStone.Attributes[GameAttribute.Operatable] = Activated;
						NStone.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
						NStone.Attributes[GameAttribute.Disabled] = !Activated;
						NStone.Attributes[GameAttribute.Immunity] = !Activated;
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
					Guardian.PlayActionAnimation(206664);
					Guardian.PlayActionAnimation(211841);
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

			this.Game.QuestManager.Quests[101758].Steps.Add(36, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //use Heaven portal
					ListenInteract(188441, 1, new ChangeAct(300));
				})
			});

			this.Game.QuestManager.Quests[101758].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					this.Game.CurrentEncounter.activated = false;
				})
			});

			#endregion
		}
	}
}
