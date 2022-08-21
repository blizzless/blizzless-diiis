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
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class ActV : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public ActV(Game game) : base(game)
		{
			//*
			var Quest1Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][251355].Data;
			var Quest2Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][284683].Data;
			var Quest3Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][285098].Data;
			var Quest4Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][257120].Data;
			var Quest5Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][263851].Data;
			var Quest6Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][273790].Data;
			var Quest7Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][269552].Data;
			var Quest8Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][273408].Data;

		}
		public static void AddQuestConversation(Actor actor, int conversation)
		{
			var NPC = actor as InteractiveNPC;
			if (NPC != null)
			{
				NPC.Conversations.Clear();
				NPC.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(conversation));
				NPC.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
				NPC.Attributes.BroadcastChangedIfRevealed();
				NPC.ForceConversationSNO = conversation;
			}
			else if (actor != null)
			{
				foreach (var N in actor.World.GetActorsBySNO(actor.ActorSNO.Id))
					if (N is InteractiveNPC)
					{
						NPC = N as InteractiveNPC;
						NPC.Conversations.Clear();
						NPC.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(conversation));
						NPC.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
						NPC.Attributes.BroadcastChangedIfRevealed();
						NPC.ForceConversationSNO = conversation;
					}
			}
			else
				Logger.Warn("Не удалось присвоить диалог для NPC.");
		}
		public static void RemoveConversations(Actor actor)
		{
			var NPC = actor as InteractiveNPC;
			if (NPC != null)
			{
				NPC.Conversations.Clear();
				NPC.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
				NPC.Attributes.BroadcastChangedIfRevealed();
			}
		}

		public override void SetQuests()
		{
			#region The Fall of Westmarch.
			//x1_WestM_IntroQuest
			this.Game.QuestManager.Quests.Add(251355, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 284683, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[251355].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk with Lorath Nahr
					ListenKill(319442, 1, new LaunchConversation(320130));
					ListenConversation(320130, new Advance());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(2, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 59,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//enter Westmarch
					var world = this.Game.GetWorld(WorldSno.x1_westm_intro);
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westmarch_overlook_d, () =>
					{
				
						if (this.Game.CurrentQuest == 251355 && this.Game.CurrentStep == 2)
						{
                            StartConversation(this.Game.GetWorld(WorldSno.x1_westmarch_overlook_d), 317212);
						}
					});
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_intro, () =>
					{
                        StartConversation(world, 311433);

						//Delete fake giant door
						if (world.GetActorBySNO(328008) != null)
							world.Leave(world.GetActorBySNO(328008));
						//Delete Cadala from this location TODO: нужно найти анимации Кадалы с убийственной волной на монстров)
						foreach (var cadal in world.GetActorsBySNO(311858))
							world.Leave(cadal);
					});
                    var npc = world.GetActorBySNO(308377);
                    if (npc != null)
					{
						var introGuy = npc as InteractiveNPC;
						introGuy.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(308393));
						introGuy.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
						introGuy.Attributes.BroadcastChangedIfRevealed();
					}
                    ListenConversation(308393, new EnterToWest());
					//ListenInteract(309222, 1, new Advance());

					//Locked Door - 316495 - Wait
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(59, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 14,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find a5 hub
					var npc = this.Game.GetWorld(WorldSno.x1_westm_intro).GetActorBySNO(308377);
					if (npc != null)
					{
						var introGuy = npc as InteractiveNPC;
						introGuy.Conversations.Clear();
						introGuy.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
						introGuy.Attributes.BroadcastChangedIfRevealed();

					}
					ListenInteract(309812, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 7,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill mobs at hub
					var world = this.Game.GetWorld(WorldSno.x1_westm_intro);
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_intro, () =>
					{
						if (this.Game.CurrentQuest == 251355 && this.Game.CurrentStep == 14)
						{
							try { world.FindAt(316495, new Vector3D { X = 555.9f, Y = 403.47f, Z = 10.2f }, 5.0f).Destroy(); } catch { }
						}
					});
					ListenKill(276309, 10, new Advance());
					var Tyrael = world.ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
					foreach (var general in world.GetActorsBySNO(303828)) { general.SetVisible(false); general.Hidden = true; } //x1_WestmHub_General
					foreach (var general in world.GetActorsBySNO(364173)) { general.SetVisible(false); general.Hidden = true; } //x1_WestmHub_BSS_postChange
					foreach (var general in world.GetActorsBySNO(175310)) { general.SetVisible(false); general.Hidden = true; } //PT_Mystic_NoVendor_NonGlobalFollower
					foreach (var general in world.GetActorsBySNO(259252)) { general.SetVisible(false); general.Hidden = true; } // X1_WestmHub_angryman_Temp
					foreach (var general in world.GetActorsBySNO(259256)) { general.SetVisible(false); general.Hidden = true; } //X1_WestmHub_grieving_Temp
					var Lorath = world.ShowOnlyNumNPC(284530, 0) as ActorSystem.InteractiveNPC;


				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 57,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					var world = this.Game.GetWorld(WorldSno.x1_westm_intro);
					//Delete Monsters
					foreach (var skeleton in world.GetActorsBySNO(276309)) skeleton.Destroy(); //x1_Skeleton_Westmarch_A
					foreach (var skeleton in world.GetActorsBySNO(309114)) skeleton.Destroy(); //x1_Ghost_Dark_A
					foreach (var skeleton in world.GetActorsBySNO(282027)) skeleton.Destroy(); //x1_Shield_Skeleton_Westmarch_A

					//Talk to Tyrael
					var Tyrael = world.ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
					var Lorath = world.ShowOnlyNumNPC(284530, 0) as ActorSystem.InteractiveNPC;

					AddQuestConversation(Tyrael, 252089);

					//ListenInteract(289293, 1, new LaunchConversation(252089));
					ListenConversation(252089, new AfterKillMonsters());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(57, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//enter the church
					var world = this.Game.GetWorld(WorldSno.x1_westm_intro);
					var Tyrael = world.ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
					Tyrael.Conversations.Clear();
					Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
					Tyrael.Attributes.BroadcastChangedIfRevealed();

					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_intro, () =>
					{
						Open(world, 316548);
					});
					if (world.GetActorBySNO(316548) != null)
						world.GetActorBySNO(316548).Destroy();
					ListenTeleport(309413, new Advance());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Kill unique 273419
					ListenKill(273419, 1, new AfterKillBoss());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 67,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Tyrael
					UnlockTeleport(0);
					var Tyrael = this.Game.GetWorld(WorldSno.x1_westmarch_hub).ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
					AddQuestConversation(Tyrael, 252100);
					//ListenInteract(289293, 1, new LaunchConversation(252100));
					ListenConversation(252100, new Advance());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(67, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 5,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//leave the church
					var Tyrael = this.Game.GetWorld(WorldSno.x1_westmarch_hub).ShowOnlyNumNPC(289293, 0) as ActorSystem.InteractiveNPC;
					if (Tyrael != null)
					{
						Tyrael.Conversations.Clear();
						Tyrael.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
						Tyrael.Attributes.BroadcastChangedIfRevealed();
					}
					ListenTeleport(270011, new Advance());
				})
			});

			this.Game.QuestManager.Quests[251355].Steps.Add(5, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
				})
			});

			#endregion
			#region Souls of the Dead
			//X1_WestmHub_Survivor_Rescue
			this.Game.QuestManager.Quests.Add(284683, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 285098, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[284683].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 47,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(47, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 62,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//OnTargetedActor ID: 315793, Name: x1_westm_Door_Cloister, NumInWorld: 0
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westmarch_hub, () =>
					{
						if (Game.CurrentQuest == 284683 && Game.CurrentStep == -1 || Game.CurrentQuest == 284683 && Game.CurrentStep == 47)
                            ActiveArrow(this.Game.GetWorld(WorldSno.x1_westmarch_hub), 315793);
					});
                    //Enter Westmarch Commons
                    ListenTeleport(261758, new BackToCath());
                    var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt);
					// FIXME: incorrect snoId or possible code duplicate
                    foreach (var Myst in world.GetActorsBySNO(339463)) //Mystic
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}
					foreach (var Myst in world.GetActorsBySNO(339463)) //Mystic_EnchanceEvent
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(62, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 57,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
                    //find orbs
                    var world = this.Game.GetWorld(WorldSno.x1_westmarch_hub);
                    var target = world.GetActorBySNO(315793, true);
                    DisableArrow(world, target);
                    ListenTeleport(338956, new BackToCath());

				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(57, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 60,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//destroy bodies
					var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt);
					UnlockTeleport(1);
					foreach (var Myst in world.GetActorsBySNO(175310)) //Mystic_NonGlobalFollower
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}
					foreach (var Myst in world.GetActorsBySNO(339463)) //Mystic_EnchanceEvent
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}
					ListenKill(316839, 4, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_gideonscourt, () =>
					{
						
						if (this.Game.CurrentQuest == 284683 && this.Game.CurrentStep == 57)
						{
							setActorOperable(world, 319396, false);
							setActorOperable(world, 375106, false);
						}
					});
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(60, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//slay Drygha
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_gideonscourt, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 319396, true);
					});
					ListenKill(319396, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 68,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//destroy orb
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_gideonscourt, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 375106, true);
					});
					ListenKill(375106, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(68, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 30,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt);
					//destroy effects
					foreach (var act in world.GetActorsBySNO(316810)) act.Destroy();
					foreach (var act in world.GetActorsBySNO(324508)) act.Destroy();
					foreach (var Myst in world.GetActorsBySNO(175310)) //Mystic_NonGlobalFollower
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}
					foreach (var Myst in world.GetActorsBySNO(339463)) //Mystic_EnchanceEvent
					{
						Myst.Hidden = true;
						Myst.SetVisible(false);
					}


					//check out pile
					ListenInteract(360303, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(30, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 32,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Mystic
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_gideonscourt, () =>
					{
						if (this.Game.CurrentQuest == 284683 && this.Game.CurrentStep == 30)
						{
							var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt);
							var Mysts = world.GetActorsBySNO(61524);
							if (Mysts.Count < 1)
								Mysts.Add(world.SpawnMonster(61524, new Vector3D(385.6301f,289.3048f,-18.602905f)));
							//foreach (var Myst in World.GetActorsBySNO(175310))
								;//175310
								//StartConversation(this.Game.GetWorld(338891), 305750);
							foreach (var Myst in Mysts) //PT_Mystic_NoVendor
							{
								world.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateFacingMessage
                                {
									ActorId = Myst.DynamicID(plr),
									Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(Myst, plr),
									TurnImmediately = true
								}, Myst);

								Myst.PlayActionAnimation(324119);
								AddQuestConversation(Myst, 305750);
								(Myst as InteractiveNPC).Conversations.Clear();
								(Myst as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(305750));
								(Myst as InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 2;
								(Myst as InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
								(Myst as InteractiveNPC).ForceConversationSNO = 305750;
							}
						}
					});
					ListenConversation(305750, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(32, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 55,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//exit alley
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_gideonscourt, () =>
					{
						var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt);
						foreach (var Myst in world.GetActorsBySNO(175310)) //Mystic_NonGlobalFollower
						{
							Myst.Hidden = true;
							Myst.SetVisible(false);
						} 
						foreach (var Myst in world.GetActorsBySNO(339463)) //Mystic_EnchanceEvent
						{
							Myst.Hidden = true;
							Myst.SetVisible(false);
						}
						foreach (var Myst in world.GetActorsBySNO(61524)) //PT_Mystic_NoVendor
						{
							Myst.Hidden = true;
							Myst.SetVisible(false);
						}
						foreach (var Malt in world.GetActorsBySNO(373456))
						{
							bool Activated = false;

							Malt.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
							Malt.Attributes[GameAttribute.Untargetable] = !Activated;
							Malt.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
							Malt.Attributes[GameAttribute.Operatable] = Activated;
							Malt.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
							Malt.Attributes[GameAttribute.Disabled] = !Activated;
							Malt.Attributes[GameAttribute.Immunity] = !Activated;
						}
						Open(world, 319830);
						AddFollower(world, 175310);
					});
					ListenTeleport(338946, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(55, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 49,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find death orb
					ListenTeleport(339158, new AdvanceWithNotify());
					if (!this.Game.Empty)
					{
						DestroyFollower(175310);
						AddFollower(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 175310);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(49, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 53,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//destroy bodies
					UnlockTeleport(2);
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_kerwinsrow, () =>
					{
						if (this.Game.CurrentQuest == 284683 && this.Game.CurrentStep == 49)
						{
							var world = this.Game.GetWorld(WorldSno.x1_westm_deathorb_kerwinsrow);
							setActorOperable(world, 336383, false);
							setActorOperable(world, 375111, false);
						}
					});
					ListenKill(316839, 6, new Advance());
					if (!this.Game.Empty)
					{
						DestroyFollower(175310);
						AddFollower(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 175310);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(53, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 34,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//slay guardian
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_kerwinsrow, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_westm_deathorb_kerwinsrow), 336383, true);
					});
					ListenKill(336383, 1, new Advance());
					if (!this.Game.Empty)
					{
						DestroyFollower(175310);
						AddFollower(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 175310);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(34, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 40,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//destroy final orb
					this.Game.AddOnLoadWorldAction(WorldSno.x1_westm_deathorb_kerwinsrow, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_westm_deathorb_kerwinsrow), 375111, true);
					});
					ListenKill(375111, 1, new Advance());
					if (!this.Game.Empty)
					{
						DestroyFollower(175310);
						AddFollower(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 175310);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(40, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 42,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Mystic
					ListenProximity(175310, new LaunchConversation(305871));
					ListenConversation(305871, new AdvanceWithNotify());
					if (!this.Game.Empty)
					{
						DestroyFollower(175310);
						AddFollower(this.Game.GetWorld(WorldSno.x1_westm_deathorb_gideonscourt), 175310);
					}
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(42, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 29,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//return to hub
					ListenTeleport(270011, new Advance());
					if (!this.Game.Empty)
						foreach (var plr in this.Game.Players.Values)
						{
							if (!plr.MysticUnlocked)
							{
								plr.MysticUnlocked = true;
								plr.GrantAchievement(74987247205955);
								plr.LoadCrafterData();
							}
						}
					DestroyFollower(175310);
				})
			});

			this.Game.QuestManager.Quests[284683].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
				})
			});

			#endregion
			#region The Harbinger
			//X1_Westm_KillUrzael
			this.Game.QuestManager.Quests.Add(285098, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 257120, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[285098].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//enter Westmarch Heights
					ListenTeleport(263493, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 12,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find Tower
					ListenTeleport(308487, new AdvanceWithNotify());
					var Quest3Data = (DiIiS_NA.Core.MPQ.FileFormats.Quest)MPQStorage.Data.Assets[SNOGroup.Quest][285098].Data;

					ListenKillBonus(355667, 3, new SideTarget());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 14,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill Urzael
					UnlockTeleport(3);
					ListenKill(291368, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 16,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Malthael spirit
					this.Game.AddOnLoadWorldAction(WorldSno.x1_urzael_arena, () =>
					{
						var malthael = this.Game.GetWorld(WorldSno.x1_urzael_arena).SpawnMonster(256248, new Vector3D { X = 97.65f, Y = 350.23f, Z = 0.1f });
						malthael.NotifyConversation(1);
					});
					this.Game.CurrentEncounter.activated = false;
					ListenInteract(256248, 1, new LaunchConversation(274423));
					ListenConversation(274423, new Advance());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//return to the Hub
					ListenTeleport(270011, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Tyrael
					ListenInteract(289293, 1, new LaunchConversation(283403));
					ListenConversation(283403, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[285098].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
					PlayCutscene(1);
				})
			});

			#endregion
			#region The Witch
			//x1_Adria
			this.Game.QuestManager.Quests.Add(257120, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 263851, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[257120].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 67,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(67, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 65,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
                    //find entrance
                    //DisableArrow(this.Game.GetWorld(304235), target);
                    var westmarchWorld = this.Game.GetWorld(WorldSno.x1_westmarch_hub);
                    this.Game.AddOnLoadWorldAction(WorldSno.x1_westmarch_hub, () =>
					{
                        westmarchWorld.BroadcastGlobal(plr => new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
						{
							HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("QuestMarker"),
							Place = new MessageSystem.Message.Fields.WorldPlace { Position = new Vector3D(435.1377f, 439.43f, -0.96f), WorldID = westmarchWorld.GlobalID },
							ImageInfo = 81058,
							Label = -1,
							snoStringList = -1,
							snoKnownActorOverride = -1,
							snoQuestSource = -1,
							Image = -1,
							Active = true,
							CanBecomeArrow = true,
							RespectsFoW = false,
							IsPing = false,
							PlayerUseFlags = 0
						});
					});

                    ListenProximity(341337, new LaunchConversation(345820));
                    ListenConversation(345820, new AdvanceWithNotify());
                    AddFollower(westmarchWorld, 284530);
                    StartConversation(westmarchWorld, 305750);
					this.Game.AddOnLoadWorldAction(WorldSno.x1_bog_adriaritual, () =>
					{
						var world = this.Game.GetWorld(WorldSno.x1_bog_adriaritual);
						world.BroadcastGlobal(plr => new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
						{
							HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("QuestMarker"),
							Place = new MessageSystem.Message.Fields.WorldPlace { Position = new Vector3D(435.1377f, 439.43f, -0.96f), WorldID = westmarchWorld.GlobalID },
							ImageInfo = 81058,
							Label = -1,
							snoStringList = -1,
							snoKnownActorOverride = -1,
							snoQuestSource = -1,
							Image = -1,
							Active = false,
							CanBecomeArrow = false,
							RespectsFoW = false,
							IsPing = false,
							PlayerUseFlags = 0
						});
                        setActorOperable(world, 340914, false);
					});
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(65, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 92,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill mobs
					this.Game.AddOnLoadWorldAction(WorldSno.x1_bog_adriaritual, () =>
					{
						script = new WavedInvasion(new Vector3D { X = 101.62f, Y = 105.97f, Z = 0.1f }, 30f, new List<int> { 356157, 356160 }, 356380);
						script.Execute(this.Game.GetWorld(WorldSno.x1_bog_adriaritual));
					});
					ListenKill(356380, 1, new Advance());
					DestroyFollower(284530);
					AddFollower(this.Game.GetWorld(WorldSno.x1_westmarch_hub), 284530);
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(92, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 106,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find Nephalem Guidestone
					this.Game.AddOnLoadWorldAction(WorldSno.x1_bog_adriaritual, () =>
					{
						Open(this.Game.GetWorld(WorldSno.x1_bog_adriaritual), 340914);
					});
					ListenProximity(346878, new AdvanceWithNotify());
					DestroyFollower(284530);
					AddFollower(this.Game.GetWorld(WorldSno.x1_westmarch_hub), 284530);
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(106, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 73,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//use waystone
					UnlockTeleport(4);
					ListenInteract(346878, 1, new Advance());
					DestroyFollower(284530);
					AddFollower(this.Game.GetWorld(WorldSno.x1_westmarch_hub), 284530);
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(73, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 10,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find catacombs
					var world = this.Game.GetWorld(WorldSno.x1_bog_adriaritual);

					Portal Dest = null;
					foreach (Portal prtl in world.GetActorsBySNO(176007))
						if (prtl.Destination.WorldSNO == (int)WorldSno.x1_catacombs_level01) Dest = prtl;

					if (Dest != null)
						world.BroadcastGlobal(plr => new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
						{
							HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("QuestMarker"),
							Place = new MessageSystem.Message.Fields.WorldPlace { Position = Dest.Position, WorldID = world.GlobalID },
							ImageInfo = 81058,
							Label = -1,
							snoStringList = -1,
							snoKnownActorOverride = -1,
							snoQuestSource = -1,
							Image = -1,
							Active = true,
							CanBecomeArrow = true,
							RespectsFoW = false,
							IsPing = false,
							PlayerUseFlags = 0
						});
					//*/
					ListenTeleport(283553, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(10, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 110,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//search tomb
					var world = this.Game.GetWorld(WorldSno.x1_bog_adriaritual);

					Portal Dest = null;
					foreach (Portal prtl in world.GetActorsBySNO(176007))
						if (prtl.Destination.WorldSNO == (int)WorldSno.x1_catacombs_level01) Dest = prtl;

					if (Dest != null)
						world.BroadcastGlobal(plr => new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
						{
							HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("QuestMarker"),
							Place = new MessageSystem.Message.Fields.WorldPlace { Position = Dest.Position, WorldID = world.GlobalID },
							ImageInfo = 81058,
							Label = -1,
							snoStringList = -1,
							snoKnownActorOverride = -1,
							snoQuestSource = -1,
							Image = -1,
							Active = false,
							CanBecomeArrow = false,
							RespectsFoW = false,
							IsPing = false,
							PlayerUseFlags = 0
						});
					//*/
					UnlockTeleport(5);
					//[World] SNOId: 283566 GlobalId: 117440518 Name: x1_catacombs_level02
					ListenTeleport(283567, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(110, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 14,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//go Adria
					UnlockTeleport(6);
					ListenTeleport(287220, new AdvanceWithNotify());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 78,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill Adria
					//UnlockTeleport(7); //hacky
					ListenKill(279394, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(78, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 115,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Lorath
					this.Game.CurrentEncounter.activated = false;
					var world = this.Game.GetWorld(WorldSno.x1_adria_boss_arena_02);

					foreach (var Myst in world.GetActorsBySNO(284530)) //284530
					{
						AddQuestConversation(Myst, 260191);
						(Myst as InteractiveNPC).Conversations.Clear();
						(Myst as InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(260191));
						(Myst as InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 2;
						(Myst as InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
						(Myst as InteractiveNPC).ForceConversationSNO = 260191;
					}

					this.Game.AddOnLoadWorldAction(WorldSno.x1_adria_boss_arena_02, () =>
					{
						world.GetActorBySNO(284530).NotifyConversation(1);
					});
					this.Game.CurrentEncounter.activated = false;

					//ListenInteract(284530, 1, new LaunchConversation(260191));
					ListenConversation(260191, new Advance());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(115, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Tyrael
					foreach (var Myst in this.Game.GetWorld(WorldSno.x1_adria_boss_arena_02).GetActorsBySNO(284530)) //284530
					{
						(Myst as InteractiveNPC).Conversations.Clear();
						(Myst as InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 1;
						(Myst as InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
					}
					ListenInteract(289293, 1, new LaunchConversation(274440));
					ListenConversation(274440, new Advance());
				})
			});

			this.Game.QuestManager.Quests[257120].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
					PlayCutscene(2);
				})
			});

			#endregion
			#region The Pandemonium Gate
			//x1_ToHeaven
			this.Game.QuestManager.Quests.Add(263851, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 273790, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[263851].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 15,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[263851].Steps.Add(15, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 17,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//go to Pandemonium Gate
					ListenTeleport(339468, new Advance());
				})
			});

			this.Game.QuestManager.Quests[263851].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 19,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill reapers and Lamiel
					ListenKill(348771, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[263851].Steps.Add(19, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Imperius
					this.Game.AddOnLoadWorldAction(WorldSno.x1_heaven_pandemonium_portal, () =>
					{
						this.Game.GetWorld(WorldSno.x1_heaven_pandemonium_portal).GetActorBySNO(368315).NotifyConversation(1);
					});
					ListenInteract(368315, 1, new LaunchConversation(361192));
					ListenConversation(361192, new Advance());
				})
			});

			this.Game.QuestManager.Quests[263851].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//use portal
					ListenTeleport(299453, new Advance());
				})
			});

			this.Game.QuestManager.Quests[263851].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
				})
			});

			#endregion
			#region The Battlefields of Eternity
			//X1_PandExt_ExteriorFull
			this.Game.QuestManager.Quests.Add(273790, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 269552, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[273790].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 35,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(35, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 41,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//cork for Imperius
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_ext_gateoverlook, () =>
					{
						if (this.Game.CurrentQuest == 273790 && this.Game.CurrentStep == 35)
						{
							script = new Advance();
							script.Execute(this.Game.GetWorld(WorldSno.x1_pand_ext_gateoverlook));
						}
					});
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(41, new QuestStep
            {
				Completed = false,
				Saveable = true,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
                    //reach Imperius
                    ListenInteract(275409, 1, new LaunchConversation(361245));
                    ListenConversation(361245, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_ext_gateoverlook, () =>
					{
                        var world = this.Game.GetWorld(WorldSno.x1_pand_ext_gateoverlook);
                        if (world.GetActorBySNO(275409, true) != null)
                            world.GetActorBySNO(275409, true).NotifyConversation(1);
					});
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 51,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//get to Siege Camp
					ListenProximity(364231, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(51, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//gather siege rune
					ListenInteract(361364, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(11, new QuestStep
            {
				Completed = false,
				Saveable = true,
				NextStep = 43,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
                    //talk to Imperius
                    ListenInteract(275409, 1, new LaunchConversation(361252));
                    ListenConversation(361252, new LaunchConversation(361275));
                    ListenConversation(361275, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_ext_gateoverlook, () =>
					{
                        var world = this.Game.GetWorld(WorldSno.x1_pand_ext_gateoverlook);
                        if (world.GetActorBySNO(275409, true) != null)
                            world.GetActorBySNO(275409, true).NotifyConversation(1);
					});
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(43, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 45,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//hunt for Siege Runes
					UnlockTeleport(8);
					ListenInteract(361364, 2, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(45, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 30,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//enter Siege outpost
					ListenTeleport(339397, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(30, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 33,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Kill Ram Defense Captain (Thilor)
					ListenKill(338681, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(33, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 15,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//talk to Tyrael
					ListenInteract(290323, 1, new LaunchConversation(346540));
					ListenConversation(346540, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_ext_batteringram_entrance_a, () =>
					{
						this.Game.GetWorld(WorldSno.x1_pand_ext_batteringram_entrance_a).GetActorBySNO(290323).NotifyConversation(1);
					});
				})
			});

			this.Game.QuestManager.Quests[273790].Steps.Add(15, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
				})
			});

			#endregion
			#region Breaching the Fortress
			//x1_BatteringRamFight
			this.Game.QuestManager.Quests.Add(269552, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = 273408, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[269552].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 32,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(32, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 25,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Board Ram
					UnlockTeleport(9);
					ListenTeleport(295228, new Advance());
					var RamWorld = this.Game.GetWorld(WorldSno.x1_pand_batteringram);
					RamWorld.GetActorBySNO(376686).Hidden = true;
					RamWorld.GetActorBySNO(376686).SetVisible(false);
					RamWorld.GetActorBySNO(345259).Hidden = true;
					RamWorld.GetActorBySNO(345259).SetVisible(false);
					RamWorld.GetActorBySNO(323353).Hidden = true;
					RamWorld.GetActorBySNO(323353).SetVisible(false);
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_batteringram, () =>
					{
						RamWorld.GetActorBySNO(176002).SetVisible(false);
						foreach (var plr in this.Game.Players.Values)
						{
							RamWorld.GetActorBySNO(323353).Unreveal(plr);
							RamWorld.GetActorBySNO(345259).Unreveal(plr);
							RamWorld.GetActorBySNO(376686).Unreveal(plr);
							RamWorld.GetActorBySNO(176002).Unreveal(plr);
						}
						if (this.Game.CurrentQuest == 269552)
							RamWorld.GetActorBySNO(295438).PlayActionAnimation(299978);
					});
					
					

				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(25, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 27,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//breach phase
					ListenKill(340920, 2, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_batteringram, () =>
					{
						setActorVisible(this.Game.GetWorld(WorldSno.x1_pand_batteringram), 358946, false);
					});
				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(27, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 29,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Fight Ram Boss
					ListenKill(358946, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_batteringram, () =>
					{
						if (this.Game.CurrentQuest == 269552)
							setActorVisible(this.Game.GetWorld(WorldSno.x1_pand_batteringram), 358946, true);
					});
				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(29, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 22,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Finish the Gate
					
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_batteringram, () =>
					{
						if (this.Game.CurrentQuest == 269552)
						{
							//Должен быть Удар 1!
							//{ 70176 = 334746} //Удар 1
							script = new FirstWaveRam();
							script.Execute(this.Game.GetWorld(WorldSno.x1_pand_batteringram));
						}
					});
					//После волны - Удар 2!
					//{ 70192 = 334747} //Удар 2 (есть обломки)
					ListenKill(360242, 1, new SecondWaveRam());
					//После волны - Удар 2!
					//{ 70192 = 334747} //Удар 2 (есть обломки)
					ListenKill(360243, 1, new ThirdWaveRam());
					//Последняя война - Удар 3, пробиваем дыру.
					//{ 70208 = 334748} //Удар 3 (с пробитием)
					ListenKill(360245, 1, new Babah());
				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 8,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//enter Breach
					var RamWorld = this.Game.GetWorld(WorldSno.x1_pand_batteringram);

					this.Game.CurrentEncounter.activated = false;
					ListenTeleport(271234, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_pand_batteringram, () =>
					{
						RamWorld.GetActorBySNO(176002).SetVisible(true);
						foreach (var plr in this.Game.Players.Values)
						{
							RamWorld.GetActorBySNO(176002).Reveal(plr);
						}
						if (this.Game.CurrentQuest != 269552)
							RamWorld.GetActorBySNO(295438).SetIdleAnimation(360069);
						//RamWorld.GetActorBySNO(295438).PlayActionAnimation(299978); 
						//Open(this.Game.GetWorld(295225), 345259);
						Open(RamWorld, 295438);
					});
				})
			});

			this.Game.QuestManager.Quests[269552].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
					PlayCutscene(3);
				})
			});

			#endregion
			#region Angel of Death
			//x1_Fortress_KillMalthael
			this.Game.QuestManager.Quests.Add(273408, new Quest { RewardXp = 7000, RewardGold = 620, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.Quests[273408].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 30,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(30, new QuestStep
            {
				Completed = false,
				Saveable = true,
				NextStep = 12,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//FortressIntroTyrael
					var world = this.Game.GetWorld(WorldSno.x1_fortress_level_01);

					AddQuestConversation(world.GetActorBySNO(6353), 302646);
                    //ListenProximity(6353, new LaunchConversation(302646));
                    ListenConversation(302646, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_fortress_level_01, () =>
					{
						if (this.Game.CurrentQuest == 273408 && this.Game.CurrentStep == 30)
                        {
                            world.GetActorBySNO(6353).NotifyConversation(1);
                        }
                    });
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 36,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Spirit Well 1
					RemoveConversations(this.Game.GetWorld(WorldSno.x1_fortress_level_01).GetActorBySNO(6353));
					UnlockTeleport(10);
					ListenInteract(308737, 1, new LaunchConversation(335174));
					ListenInteract(314802, 1, new LaunchConversation(336672));
					ListenInteract(319402, 1, new LaunchConversation(336674));
					ListenInteract(314804, 1, new LaunchConversation(336676));
					ListenInteract(314806, 1, new LaunchConversation(336678));
					ListenInteract(314817, 1, new LaunchConversation(336680));
					ListenInteract(314792, 1, new LaunchConversation(336682));
					ListenInteract(473887, 1, new LaunchConversation(469534)); //x1_fortress_SpiritLevel1_Necromancer

					ListenConversation(335174, new LaunchConversation(308752));
					ListenConversation(336672, new LaunchConversation(314906));
					ListenConversation(336674, new LaunchConversation(319523));
					ListenConversation(336676, new LaunchConversation(314911));
					ListenConversation(336678, new LaunchConversation(314915));
					ListenConversation(336680, new LaunchConversation(314919));
					ListenConversation(336682, new LaunchConversation(314924));
					ListenConversation(469534, new LaunchConversation(469542));

					ListenConversation(308752, new Advance());
					ListenConversation(314906, new Advance());
					ListenConversation(319523, new Advance());
					ListenConversation(314911, new Advance());
					ListenConversation(314915, new Advance());
					ListenConversation(314919, new Advance());
					ListenConversation(314924, new Advance());
					ListenConversation(469542, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(36, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 65,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Spirit Well 2
					ListenTeleport(360494, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(65, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 61,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Kill Death Maiden
					ListenKill(360241, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_fortress_level_02, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_fortress_level_02), 347276, false);
					});
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(61, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Destroy Soul Prison
					UnlockTeleport(11);
					ListenKill(347276, 1, new Advance());
					this.Game.AddOnLoadWorldAction(WorldSno.x1_fortress_level_02, () =>
					{
						setActorOperable(this.Game.GetWorld(WorldSno.x1_fortress_level_02), 347276, true);
					});
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 8,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//find Malthael
					ListenTeleport(330576, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(8, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 21,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//kill Malthael
					ListenKill(297730, 1, new Advance());
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(21, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = 7,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//Success
					this.Game.CurrentEncounter.activated = false;
					ListenProximity(6353, new LaunchConversation(351334));
					ListenConversation(351334, new Advance());
					if (this.Game.IsHardcore)
					{
						foreach (var plr in this.Game.Players.Values)
							if (!plr.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.HardcoreAdventureModeUnlocked))
							{
								plr.Toon.GameAccount.Flags = plr.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.HardcoreAdventureModeUnlocked;
								//plr.InGameClient.SendMessage(new AdventureModeUnlockedMessage());
							}
					}
					else
					{
						foreach (var plr in this.Game.Players.Values)
							if (!plr.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.AdventureModeUnlocked))
							{
								plr.Toon.GameAccount.Flags = plr.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.AdventureModeUnlocked;
								//plr.InGameClient.SendMessage(new AdventureModeUnlockedMessage());
							}
					}
					this.Game.AddOnLoadWorldAction(WorldSno.x1_malthael_boss_arena, () =>
					{
						this.Game.GetWorld(WorldSno.x1_malthael_boss_arena).GetActorBySNO(6353).NotifyConversation(1);
					});
				})
			});

			this.Game.QuestManager.Quests[273408].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = true,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					//complete
				})
			});

			#endregion
		}
	}
}
