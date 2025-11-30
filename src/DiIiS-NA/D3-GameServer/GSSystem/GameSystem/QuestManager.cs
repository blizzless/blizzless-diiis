using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using D3.Quests;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.QuestSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using Spectre.Console;
using Monster = DiIiS_NA.GameServer.GSSystem.ActorSystem.Monster;

namespace DiIiS_NA.D3_GameServer.GSSystem.GameSystem
{
	public class QuestManager
	{
		private static readonly Logger Logger = new(nameof(QuestManager));

		public readonly Dictionary<int, QuestRegistry.Quest> Quests = new();

		public readonly Dictionary<int, QuestRegistry.Quest> SideQuests = new();

		public readonly List<Bounty> Bounties = new(); 
		
		public Game Game { get; set; }

		public int CurrentAct => Game.CurrentAct;

		public delegate void QuestProgressDelegate();
		public event QuestProgressDelegate OnQuestProgress = delegate { };

		/// <summary>
		/// Creates a new QuestManager and attaches it to a game. Quests are are share among all players in a game
		/// </summary>
		/// <param name="game">The game is used to broadcast game messages to</param>
		public QuestManager(Game game)
		{
			Game = game;
		}

		public void SetQuests()
		{
			Game.QuestProgress.SetQuests();
			Game.SideQuestProgress.SetQuests();
			Game.QuestSetup = true;
		}

		public void ClearQuests()
		{
			Quests.Clear();
			SideQuests.Clear();
		}

		public void ReloadQuests()
		{
			Quests.Clear();
			Game.QuestProgress.SetQuests();
		}

		public void SetBounties()
		{
			Bounties.Clear();
			
			var actToKillBossBounties = ItemGenerator.Bounties.Values
				.Where(bounty => bounty.BountyData0.Type == BountyData.BountyType.KillBoss)
				.Where(bounty => !Bounties.Select(b => b.LevelArea).Contains(bounty.BountyData0.LeveAreaSNO0))
				.OrderBy(_ => FastRandom.Instance.Next())
				.Select(bounty =>
				{
					var stepObjectives = bounty.QuestSteps
						.SelectMany(s => s.StepObjectiveSets)
						.SelectMany(s => s.StepObjectives)
						.ToArray();
					var killMonsterObjectiveIndex = stepObjectives
						.FindIndex(o => o.ObjectiveType == QuestStepObjectiveType.KillMonster);
					var levelAreaChecks = stepObjectives
						.Where(o => o.ObjectiveType == QuestStepObjectiveType.EnterLevelArea)
						.Select(o => o.SNOName1.Id)
						.ToList();
					
					return new Bounty
					{
						QuestManager = this,
						BountySNOid = bounty.Header.SNOId,
						Act = bounty.BountyData0.ActData,
						Type = bounty.BountyData0.Type,
						LevelArea = bounty.BountyData0.LeveAreaSNO0,
						World = WorldSno.__NONE,
						Target = stepObjectives[killMonsterObjectiveIndex].SNOName1.Id,
						TargetTaskId = killMonsterObjectiveIndex,
						AdditionalTaskId = 0,
						AdditionalTargetCounter = 0,
						AdditionalTargetNeed = 0,
						LevelAreaChecks = levelAreaChecks
					};
				})
				.ToLookup(bounty => bounty.Act);
			
			Bounties.AddRange(actToKillBossBounties[BountyData.ActT.A1].Take(1));
			Bounties.AddRange(actToKillBossBounties[BountyData.ActT.A2].Take(1));
			Bounties.AddRange(actToKillBossBounties[BountyData.ActT.A3].Take(1));
			Bounties.AddRange(actToKillBossBounties[BountyData.ActT.A4].Take(1));
			Bounties.AddRange(actToKillBossBounties[BountyData.ActT.A5].Take(1));

			var actToKillUniqueBounties = ItemGenerator.Bounties.Values
				.Where(bounty => bounty.BountyData0.Type == BountyData.BountyType.KillUnique)
				.Where(bounty => !Bounties.Select(b => b.LevelArea).Contains(bounty.BountyData0.LeveAreaSNO0))
				.OrderBy(_ => FastRandom.Instance.Next())
				.GroupBy(bounty => bounty.BountyData0.LeveAreaSNO0)//b.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Single(o => o.ObjectiveType == Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.KillAny).SNOName1.Id)
				.Select(group => group.First())
				.Select(bounty =>
				{
					var stepObjectives = bounty.QuestSteps
						.SelectMany(s => s.StepObjectiveSets)
						.SelectMany(s => s.StepObjectives)
						.ToArray();
					var killMonsterObjectiveIndex = stepObjectives.FindIndex(o => o.ObjectiveType == QuestStepObjectiveType.KillMonster);
					var killAnyObjectiveIndex = stepObjectives.FindIndex(o => o.ObjectiveType == QuestStepObjectiveType.KillAny);
					var levelAreaChecks = stepObjectives
						.Where(o => o.ObjectiveType == QuestStepObjectiveType.EnterLevelArea)
						.Select(o => o.SNOName1.Id)
						.ToList();

					return new Bounty
					{
						QuestManager = this,
						BountySNOid = bounty.Header.SNOId,
						Act = bounty.BountyData0.ActData,
						Type = bounty.BountyData0.Type,
						LevelArea = stepObjectives[killAnyObjectiveIndex].SNOName1.Id,
						World = WorldSno.__NONE,
						Target = stepObjectives[killMonsterObjectiveIndex].SNOName1.Id,
						TargetTaskId = killMonsterObjectiveIndex,
						AdditionalTaskId = killAnyObjectiveIndex,
						AdditionalTargetCounter = 0,
						AdditionalTargetNeed = stepObjectives[killAnyObjectiveIndex].CounterTarget,
						LevelAreaChecks = levelAreaChecks
					};
				})
				.ToLookup(bounty => bounty.Act);
			
			Bounties.AddRange(actToKillUniqueBounties[BountyData.ActT.A1].Take(4));
			Bounties.AddRange(actToKillUniqueBounties[BountyData.ActT.A2].Take(4));
			Bounties.AddRange(actToKillUniqueBounties[BountyData.ActT.A3].Take(4));
			Bounties.AddRange(actToKillUniqueBounties[BountyData.ActT.A4].Take(4));
			Bounties.AddRange(actToKillUniqueBounties[BountyData.ActT.A5].Take(4));
		}

		private readonly struct Rewards
		{
			public int Experience { get; }
			public int Gold { get; }

			public Rewards(int experience, int gold)
			{
				Experience = experience;
				Gold = gold;
			}
			
			public Rewards(float experience, float gold) : this((int) Math.Floor(experience), (int) Math.Floor(gold)) {}
		}

		private Rewards GetCurrentQuestRewards() =>
			new Rewards(Quests[Game.CurrentQuest].RewardXp, Quests[Game.CurrentQuest].RewardGold);
		/// <summary>
		/// Advances a quest by a step
		/// </summary>
		/// <param name="snoQuest">snoID of the quest to advance</param>                               
		public void Advance()
		{
			int oldQuest = Game.CurrentQuest;
			int oldStep = Game.CurrentStep;
			Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Completed = true;
			Game.CurrentStep = Quests[Game.CurrentQuest].Steps[Game.CurrentStep].NextStep;
			Game.QuestProgress.QuestTriggers.Clear();
			ClearQuestMarker();
			try
			{
				Quests[Game.CurrentQuest].Steps[Game.CurrentStep].OnAdvance.Invoke();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Advance() exception caught:");
			}

			if (Quests[Game.CurrentQuest].Steps[Game.CurrentStep].NextStep != -1)
			{
				Logger.QuestInfo(
					$"{Emoji.Known.RightArrow} Step Advance ".StyleAnsi("deeppink4") + 
					$"Game #{Game.GameId.StyleAnsi("underline")} " +
					$"from quest {oldQuest}/" +
					$"step {oldStep.StyleAnsi("deeppink4")}" +
					$"to quest {Game.CurrentQuest}'s " +
					$"step {Game.CurrentStep.StyleAnsi("deeppink4")}");
			}
			else
			{
				Quests[Game.CurrentQuest].Completed = true;
				if (!Game.Empty)
				{
					SaveQuestProgress(true);
					Logger.QuestInfo(
						$"{Emoji.Known.NextTrackButton} Quest Advance ".StyleAnsi("white") + 
						$"Game #{Game.GameId.StyleAnsi("underline")} " +
						$"from quest {oldQuest.StyleAnsi("turquoise2")}/" +
						$"step {oldStep.StyleAnsi("deeppink4")}" +
						$"to quest {Game.CurrentQuest.StyleAnsi("turquoise2")}/" +
						$"step {Game.CurrentStep.StyleAnsi("deeppink4")}");
					Game.BroadcastPlayers((client, player) =>
					{
						if (Game.IsCurrentOpenWorld) return; // open world quest

						var rewards = GetCurrentQuestRewards();
						player.InGameClient.SendMessage(new QuestStepCompleteMessage()
						{
							QuestStepComplete = QuestStepComplete.CreateBuilder()

								.SetReward(QuestReward.CreateBuilder()
									.SetGoldGranted(rewards.Gold)
									.SetXpGranted((ulong)rewards.Experience)
									.SetSnoQuest(Game.CurrentQuest)
								)
								.SetIsQuestComplete(true)
								.Build()
						});
						player.InGameClient.SendMessage(
							new GameServer.MessageSystem.Message.Definitions.Base.
								FloatingAmountMessage()
								{
									Place = new WorldPlace()
									{
										Position = player.Position,
										WorldID = player.World.DynamicID(player),
									},

									Amount = rewards.Experience,
									Type = GameServer.MessageSystem.Message.Definitions.Base
										.FloatingAmountMessage.FloatType.Experience,
								});
						player.InGameClient.SendMessage(
							new GameServer.MessageSystem.Message.Definitions.Base.
								FloatingAmountMessage()
								{
									Place = new WorldPlace()
									{
										Position = player.Position,
										WorldID = player.World.DynamicID(player),
									},

									Amount = rewards.Gold,
									Type = GameServer.MessageSystem.Message.Definitions.Base
										.FloatingAmountMessage.FloatType.Gold,
								});
						player.UpdateExp(rewards.Experience);
						player.Inventory.AddGoldAmount(rewards.Gold);
						player.AddAchievementCounter(74987243307173, (uint)rewards.Gold);
						player.CheckQuestCriteria(Game.CurrentQuest);
					});
				}

				if (Quests[Game.CurrentQuest].NextQuest == -1) return;
				Game.CurrentQuest = Quests[Game.CurrentQuest].NextQuest;
				Game.CurrentStep = -1;
				try
				{
					Quests[Game.CurrentQuest].Steps[Game.CurrentStep].OnAdvance.Invoke();
				}
				catch (Exception e)
				{
					Logger.WarnException(e, "Advance() exception caught:");
				}

				//Пока только для одного квеста
				//	if (this.Game.CurrentQuest != 72221)
				//		if (this.Game.CurrentStep != -1)
				Advance();
			}

			if (!Game.Empty)
			{
				RevealQuestProgress();
				if ((Game.CurrentActEnum != ActEnum.OpenWorld && GameModsConfig.Instance.Quest.AutoSave) ||
				    Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Saveable)
					SaveQuestProgress(false);
			}

			OnQuestProgress();
			AutoSetQuestMarker();
			Logger.Trace(
				$"$[white]$(Advance)$[/]$ Game {Game.GameId} Advanced to quest $[underline white]${Game.CurrentQuest}$[/]$, step $[underline white]${Game.CurrentStep}$[/]$");
		}

		public void SideAdvance()
		{
			if (Game.CurrentSideQuest == -1) return;

			Game.QuestTimer = null;
			SideQuests[Game.CurrentSideQuest].Steps[Game.CurrentSideStep].Completed = true;
			Game.CurrentSideStep = SideQuests[Game.CurrentSideQuest].Steps[Game.CurrentSideStep].NextStep;
			Game.SideQuestProgress.QuestTriggers.Clear();
			try
			{
				 SideQuests[Game.CurrentSideQuest].Steps[Game.CurrentSideStep].OnAdvance.Invoke();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "SideAdvance() exception caught:");
			}

			RevealSideQuestProgress();

			if (SideQuests[Game.CurrentSideQuest].Steps[Game.CurrentSideStep].NextStep == -1 &&
				SideQuests[Game.CurrentSideQuest].Steps[Game.CurrentSideStep] == SideQuests[Game.CurrentSideQuest].Steps.Last().Value)
			{
				SideQuests[Game.CurrentSideQuest].Completed = true;
				Logger.Trace($"$[white]$(Side-Advance)$[/]$ Game {Game.GameId} Side-Advanced to quest {Game.CurrentSideQuest} completed: {SideQuests[Game.CurrentSideQuest].Completed}");

				foreach (var player in Game.Players.Values)
				{
					int xpReward = (int)(SideQuests[Game.CurrentSideQuest].RewardXp * Game.XpModifier);
					int goldReward = (int)(SideQuests[Game.CurrentSideQuest].RewardGold * Game.GoldModifier);

					player.InGameClient.SendMessage(new QuestStepCompleteMessage()
					{
						QuestStepComplete = QuestStepComplete.CreateBuilder()
							
							.SetReward(QuestReward.CreateBuilder()
								.SetGoldGranted(goldReward)
								.SetXpGranted((ulong)xpReward)
								.SetSnoQuest(Game.CurrentSideQuest)
								)
							.SetIsQuestComplete(true)
							.Build()
						//snoQuest = this.Game.CurrentSideQuest,
						//isQuestComplete = true,
						//rewardXp = xpReward,
						//rewardGold = goldReward
					}) ;
					player.UpdateExp(xpReward);
					player.Inventory.AddGoldAmount(goldReward);

					player.AddAchievementCounter(74987243307173, (uint)goldReward);

					int chance = Game.IsHardcore ? 6 : 2;
					if (FastRandom.Instance.Next(100) < chance && Game.MonsterLevel >= 70)
					{
						player.World.SpawnRandomEquip(player, player, LootManager.Epic, player.Attributes[GameAttributes.Level]);
					}
					var toon = player.Toon.DbToon;
					toon.EventsCompleted++;
					Game.GameDbSession.SessionUpdate(toon);
					player.CheckQuestCriteria(Game.CurrentSideQuest);
				};

				Game.CurrentSideQuest = -1;
				Game.CurrentSideStep = -1;
			}

			OnQuestProgress();
			Logger.Trace($"$[white]$(Side-Advance)$[/]$ Game {Game.GameId} Side-Advanced to side-quest {Game.CurrentSideQuest} completed: {Game.CurrentSideStep}");
		}

		public void LaunchSideQuest(int questId, bool forceAbandon = false)
		{
			if (!SideQuests.ContainsKey(questId)) return;
			if (SideQuests[questId].Completed) return;

			if (Game.CurrentSideQuest != -1 && !forceAbandon) return;

			Game.CurrentSideQuest = questId;
			Game.CurrentSideStep = -1;
			SideAdvance();
		}

		public void AbandonSideQuest()
		{
			if (!SideQuests.ContainsKey(Game.CurrentSideQuest)) return;
			if (SideQuests[Game.CurrentSideQuest].Completed) return;

			SideQuests[Game.CurrentSideQuest].Completed = true;

			foreach (var player in Game.Players.Values)
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentSideQuest,
					snoLevelArea = -1,
					StepID = -1,
					TaskIndex = 0,
					Counter = 0,
					Checked = 0,
				});

			if (Game.CurrentSideQuest == 120396)
			{
				foreach (var plr in Game.Players.Values)
				{
					if (plr.World.SNO == WorldSno.a2dun_zolt_timed01_level01)
					{
						var world = plr.World.Game.GetWorld(WorldSno.caout_town);
						plr.ChangeWorld(world, world.GetStartingPointById(63));
					}
				}
			}

			Game.CurrentSideQuest = -1;
			Game.CurrentSideStep = -1;
		}

		public void AdvanceTo(int snoQuest, int stepId)
		{
			if (stepId == -1)
				stepId = Quests[snoQuest].Steps[-1].NextStep;
			int cycle = 0;
			while (!(Game.CurrentQuest == snoQuest && Game.CurrentStep == stepId) && Game.Working) //adjusting completed quests
			{
				Advance();
				cycle++;
				if (cycle > 200) break;
			}
		}
		public void AdvanceToFirstStep(int snoQuest)
		{
			var quest = Quests[snoQuest].Steps.OrderBy(s => s.Key).FirstOrDefault();
			if (quest.Value != null)
			{
				AdvanceTo(snoQuest, quest.Key == -1 ? quest.Value.NextStep : quest.Key);
			}
			else
			{
				Logger.Error("AdvanceToNext: quest {0} not found", snoQuest);
			}
		}

		public float QuestTimerEstimate = 0f;

		public void LaunchRiftQuestTimer(float duration, Action<int> onDone, int idSno = 0)
		{
			QuestTimerEstimate = duration;

			Game.QuestTimer = SteppedTickTimer.WaitSecondsStepped(Game, 1f, duration, new Action<int>((q) =>
			{
				QuestTimerEstimate -= 1f;
				
			}),
			onDone);
		}

		public void LaunchQuestTimer(int questId, float duration, Action<int> onDone, int masterId = 0)
		{
			Game.BroadcastPlayers((client, player) =>
			{
				player.InGameClient.SendMessage(new QuestMeterMessage()
				{
					snoQuest = questId,
					annMeter = masterId,
					flMeter = 1f
				});
			});

			QuestTimerEstimate = duration;

			Game.QuestTimer = SteppedTickTimer.WaitSecondsStepped(Game, 1f, duration, new Action<int>((q) =>
			{
				QuestTimerEstimate -= 1f;
				foreach (var player in Game.Players.Values)
				{
					player.InGameClient.SendMessage(new QuestMeterMessage()
					{
						snoQuest = questId,
						annMeter = masterId,
						flMeter = (QuestTimerEstimate / duration)
					});
				};
			}),
			onDone);
		}

		public void CompleteObjective(int objId)
		{
			if (Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Objectives[objId].Counter >= Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Objectives[objId].Limit)
				return;

			Logger.Trace("(CompleteObjective) Completing objective through quest manager");
			Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Objectives[objId].Counter++;

			var objective = Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Objectives[objId];
			Game.BroadcastPlayers((client, player) =>
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentStep,
					TaskIndex = objId,
					Counter = objective.Counter,
					Checked = objective.Counter < objective.Limit ? 0 : 1,
				}));

			if (!Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Objectives.Any(obj => obj.Counter < obj.Limit))
				Advance();
		}


		/// <summary>
		/// Call this, to trigger quest progress if a certain event has occured
		/// </summary>
		/// 
		public void NotifyQuest(int value, bool complete)
		{
			foreach (var player in Game.Players.Values)
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentStep,
					TaskIndex = 0,
					Counter = value,
					Checked = complete ? 1 : 0,
				});
		}

		public void NotifySideQuest(int value, bool complete)
		{
			foreach (var player in Game.Players.Values)
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentSideQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentSideStep,
					TaskIndex = 0,
					Counter = value,
					Checked = complete ? 1 : 0,
				});
		}

		/// <summary>
		/// Call this, to trigger quest progress in bonus objectives for certain event occured
		/// </summary>
		public void NotifyBonus(int value, bool complete)
		{
			foreach (var player in Game.Players.Values)
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentStep,
					TaskIndex = 1,
					Counter = value,
					Checked = complete ? 1 : 0,
				});
		}

		public void AutoSetQuestMarker()
		{
			Logger.MethodTrace(
				$"{Game.QuestProgress.QuestTriggers.Count} triggers found on {Game.CurrentActEnum.ToString()} - quest {Game.CurrentQuest} step {Game.CurrentStep}");

			// TODO: more triggers?
			#if DEBUG
			if (Game.QuestProgress.QuestTriggers.Count > 1)
				Logger.Warn($"Found {Game.QuestProgress.QuestTriggers.Count} triggers on {Game.CurrentActEnum.ToString()} - quest {Game.CurrentQuest} step {Game.CurrentStep} but only one is supported");
			#endif
			if (Game.QuestProgress.QuestTriggers.Count == 1)
			{
				var trigger = Game.QuestProgress.QuestTriggers.First();
				switch (trigger.Value.TriggerType)
				{
					case QuestStepObjectiveType.InteractWithActor:
					{
						foreach (var world in Game.Worlds)
						{
							var actor = world.GetActorsBySNO((ActorSno)trigger.Key).FirstOrDefault(d => d.Visible);
							if (actor != null)
								world.BroadcastOperation(player =>
									player.InGameClient.SendMessage(new MapMarkerInfoMessage
									{
										HashedName = StringHashHelper.HashItemName("QuestMarker"),
										Place = new WorldPlace { Position = actor.Position, WorldID = world.GlobalID },
										ImageInfo = 81058,
										Label = -1,
										snoStringList = -1,
										snoKnownActorOverride = -1,
										snoQuestSource = -1,
										Image = -1,
										Active = true,
										CanBecomeArrow = true,
										RespectsFoW = false,
										IsPing = true,
										PlayerUseFlags = 0
									}));
						}

						break;
					}
					case QuestStepObjectiveType.HadConversation:
					{
						foreach (var world in Game.Worlds)
						{
							var actor = world.Actors.Values.FirstOrDefault(d => d.Visible && (d is InteractiveNPC npc) 
								&& npc.Conversations.Any(c => c.ConversationSNO == trigger.Key));
							if (actor != null)
								world.BroadcastOperation(player =>
									player.InGameClient.SendMessage(new MapMarkerInfoMessage
									{
										HashedName = StringHashHelper.HashItemName("QuestMarker"),
										Place = new WorldPlace { Position = actor.Position, WorldID = world.GlobalID },
										ImageInfo = 81058,
										Label = -1,
										snoStringList = -1,
										snoKnownActorOverride = -1,
										snoQuestSource = -1,
										Image = -1,
										Active = true,
										CanBecomeArrow = true,
										RespectsFoW = false,
										IsPing = true,
										PlayerUseFlags = 0
									}));
						}

						break;
					}
				}
			}
		}

		public void SetBountyMarker(Player player)
		{
			foreach (var bounty in Bounties.Where(b => !b.Finished && b.Type == BountyData.BountyType.KillUnique))
			{
				var unique = player.World.GetActorsBySNO((ActorSno)bounty.Target).FirstOrDefault(u => !u.Dead);
				if (unique == null) continue;
				player.InGameClient.SendMessage(new MapMarkerInfoMessage
				{
					HashedName = StringHashHelper.HashItemName("BountyMarker"),
					Place = new WorldPlace { Position = unique.Position, WorldID = player.World.GlobalID },
					ImageInfo = 81058,
					Label = -1,
					snoStringList = -1,
					snoKnownActorOverride = -1,
					snoQuestSource = -1,
					Image = -1,
					Active = true,
					CanBecomeArrow = true,
					RespectsFoW = false,
					IsPing = true,
					MaxDislpayRangeSq = 1000000f,
					MinDislpayRangeSq = 0f,
					DiscoveryRangeSq = 1000000f,
					PlayerUseFlags = 0
				});
			}
		}

		public void UnsetBountyMarker(Player player)
		{
			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName("BountyMarker"),
				Place = new WorldPlace { Position = player.Position, WorldID = player.World.GlobalID },
				ImageInfo = -1,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = -1,
				snoQuestSource = -1,
				Image = -1,
				Active = false,
				CanBecomeArrow = true,
				RespectsFoW = false,
				IsPing = true,
				MaxDislpayRangeSq = 100000f,
				MinDislpayRangeSq = 0f,
				DiscoveryRangeSq = 100000f,
				PlayerUseFlags = 0
			});
		}

		public void ClearQuestMarker()
		{
			foreach (var plr in Game.Players.Values)
				plr.InGameClient.SendMessage(new MapMarkerInfoMessage
				{
					HashedName = StringHashHelper.HashItemName("QuestMarker"),
					Place = new WorldPlace { Position = plr.Position, WorldID = plr.World.GlobalID },
					ImageInfo = -1,
					Label = -1,
					snoStringList = -1,
					snoKnownActorOverride = -1,
					snoQuestSource = -1,
					Image = -1,
					Active = false,
					CanBecomeArrow = true,
					RespectsFoW = false,
					IsPing = true,
					PlayerUseFlags = 0
				});
		}

		public bool HasCurrentQuest(int snoQuest, int Step, bool strictFilter)
		{
			if (!Quests.ContainsKey(snoQuest) && !SideQuests.ContainsKey(snoQuest)) return false;
			if (strictFilter)
			{
				if ((Game.CurrentQuest == snoQuest) && (Game.CurrentStep == Step) ||
				    (Game.CurrentSideQuest == snoQuest) && (Game.CurrentSideStep == Step))
					return true;
			}
			else
			{
				if ((Game.CurrentQuest == snoQuest || snoQuest == -1) &&
				    (Game.CurrentStep == Step || Step == -1 || Step == 0) ||
				    (Game.CurrentSideQuest == snoQuest || snoQuest == -1) &&
				    (Game.CurrentSideStep == Step || Step == -1 || Step == 0))
					return true;
			}

			return false;
		}

		public bool HasQuest(int snoQuest) => Quests.ContainsKey(snoQuest);

		public void SetQuestsForJoined(Player joinedPlayer)
		{
			//this.Game.CurrentQuest;
			//this.Game.CurrentStep;

			joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage()
			{
				snoQuest = Game.CurrentQuest,
				snoLevelArea = -1,
				StepID = Game.CurrentStep,
				DisplayButton = true,
				Failed = false
			});
		}

		public bool IsDone(int snoQuest) => Quests.ContainsKey(snoQuest) && Quests[snoQuest].Completed;

		public bool IsInQuestRange(QuestRange range)
		{
			if (range.Header.SNOId == 312431) return (Game.CurrentAct == 3000);
			if (range.Header.SNOId == 214766) return true; 

			bool started = false;
			bool ended = false;


			foreach (var rangeEntry in range.Enitys)
			{
				if (rangeEntry != null)
				{
					if (rangeEntry.Start.SNOQuest == -1 || rangeEntry.Start.StepID == -1)
						started = true;
					else
					{
						if (Quests.ContainsKey(rangeEntry.Start.SNOQuest) && Quests[rangeEntry.Start.SNOQuest].Steps.ContainsKey(rangeEntry.Start.StepID))
						{
							if (Quests[rangeEntry.Start.SNOQuest].Completed ||
							Quests[rangeEntry.Start.SNOQuest].Steps[rangeEntry.Start.StepID].Completed ||
							(Game.CurrentQuest == rangeEntry.Start.SNOQuest && Game.CurrentStep == rangeEntry.Start.StepID)) // rumford conversation needs current step
								started = true;
						}
						//else logger.Warn("QuestRange {0} references unknown quest {1}", range.Header.SNOId, range.Start.SNOQuest);
					}

					//Logger.Debug("IsInQuestRange {0} and started? {1} ", range.Header.SNOId, started);

					if (rangeEntry.End.SNOQuest == -1 || rangeEntry.End.StepID < 0)
						ended = false;
					else
					{
						if (Quests.ContainsKey(rangeEntry.End.SNOQuest) && Quests[rangeEntry.End.SNOQuest].Steps.ContainsKey(rangeEntry.End.StepID))
						{
							if (Quests[rangeEntry.End.SNOQuest].Completed ||
								Quests[rangeEntry.End.SNOQuest].Steps[rangeEntry.End.StepID].Completed)
								ended = true;
						}
						//else logger.Warn("QuestRange {0} references unknown quest {1}", range.Header.SNOId, range.End.SNOQuest);
					}

					if (started && !ended)
						return true;
					else
					{
						started = false;
						ended = false;
					}
				}
			}

			//Logger.Debug("IsInQuestRange {0} and ended? {1} ", range.Header.SNOId, ended);
			//Logger.Debug("IsInQuestRange [{0}-{1}] -> [{2}-{3}]: {4}", range.Start.SNOQuest, range.Start.StepID, range.End.SNOQuest, range.End.StepID, (bool)(started && !ended));
			return false;
		}

		public void RevealQuestProgress()
		{
			foreach (var player in Game.Players.Values)
			{
				player.InGameClient.SendMessage(new QuestUpdateMessage()
				{
					snoQuest = Game.CurrentQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentStep,
					DisplayButton = true,
					Failed = false
				});
			}
		}

		public void RevealSideQuestProgress()
		{
			foreach (var player in Game.Players.Values)
			{
				player.InGameClient.SendMessage(new QuestUpdateMessage()
				{
					snoQuest = Game.CurrentSideQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentSideStep,
					DisplayButton = true,
					Failed = false
				});
			}
		}

		public void SaveQuestProgress(bool questCompleted)
		{
			Game.BroadcastPlayers((client, player) =>
			{
				player.Toon.CurrentAct = CurrentAct;
				player.Toon.CurrentQuestId = Game.CurrentQuest;
				player.Toon.CurrentQuestStepId = Game.CurrentStep;

				List<DBQuestHistory> query = Game.GameDbSession.SessionQueryWhere<DBQuestHistory>(
					dbi => dbi.DBToon.Id == player.Toon.PersistentID && dbi.QuestId == Game.CurrentQuest);
				if (query.Count == 0)
				{
					var questHistory = new DBQuestHistory
					{
						DBToon = player.Toon.DbToon,
						QuestId = Game.CurrentQuest,
						QuestStep = Game.CurrentStep
					};
					Game.GameDbSession.SessionSave(questHistory);
				}
				else
				{
					var questHistory = query.First();
					if (Quests[Game.CurrentQuest].Steps[questHistory.QuestStep].Completed)
					{
						questHistory.QuestStep = Game.CurrentStep;
						if (questCompleted) questHistory.isCompleted = true;
						Game.GameDbSession.SessionUpdate(questHistory);
					}
				}
			});
		}
	}

	public class Bounty
	{
		public QuestManager QuestManager { get; set; }
		public int BountySNOid { get; set; }
		public BountyData.ActT Act { get; set; }
		public BountyData.BountyType Type { get; set; }
		public int LevelArea { get; set; }
		public WorldSno World { get; set; }
		public bool PortalSpawned = false;
		public bool SubsceneSpawned = false;
		public int Target { get; set; }
		public bool TargetKilled = false;
		public bool TargetSpawned = false;
		public int TargetTaskId { get; set; }
		public int AdditionalTaskId { get; set; }
		public int AdditionalTargetCounter { get; set; }
		public int AdditionalTargetNeed { get; set; }
		public List<int> LevelAreaChecks { get; set; }
		public bool Finished = false;

		public static Dictionary<int, int> LevelAreaOverrides = new() //first is in-game, second is in-data
		{
			{338602, 377700}, //battlefields of eterntity
			{271234, 370512}, //x1 fortress lv1
			{360494, 366169}, //x1 fortress lv2
			{261758, 368269}, //westm commons
			{93632, 344169}, //southern highlands (bridge)
			{19940, 344169}, //southern highlands
			{19941, 344170}, //northern highlands
			{1199, 344170}, //leoric's hunting grounds
			{19943, 344170}, //leoric's manor courtyard
			{83264, 154588}, //defiled crypt
			{19835, 368276}, //road to Alcarnus
			{19825, 368276}, //Alcarnus
			{175330, 53834}, //ancient path
		};
		public void CheckKillMonster(int levelArea)
		{
			if (Finished) return;
			if (levelArea == 19943) levelArea = 19780;
			if (Type == BountyData.BountyType.KillUnique && LevelArea == levelArea && AdditionalTargetNeed != AdditionalTargetCounter)
			{
				var Quest = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[GameServer.Core.Types.SNO.SNOGroup.Quest][BountySNOid];
				AdditionalTargetCounter++;

				foreach (var player in QuestManager.Game.Players.Values)
					player.InGameClient.SendMessage(new QuestCounterMessage()
					{
						snoQuest = BountySNOid,
						snoLevelArea = LevelArea,
						StepID = 4,
						TaskIndex = 0,
						Counter = AdditionalTargetCounter,
						Checked = AdditionalTargetNeed == AdditionalTargetCounter ? 1 : 0,
					});
			}
		}
		public void CheckKill(int snoId, int levelArea, WorldSno world)
		{
			//435868
			//220789

			if (Finished) return;
			if (Type == BountyData.BountyType.KillBoss && Target == snoId)
			{
				Complete();
			}
			if (Type == BountyData.BountyType.KillUnique && (LevelArea == levelArea || (LevelAreaOverrides.ContainsKey(levelArea) && LevelAreaOverrides[levelArea] == LevelArea)))
			{
				AdditionalTargetCounter++;
				foreach (var player in QuestManager.Game.Players.Values)
				{
					List<GameServer.GSSystem.MapSystem.Scene> scenes = new List<GameServer.GSSystem.MapSystem.Scene>();
					int monsterCount = 0;
					foreach (var scene in QuestManager.Game.GetWorld(world).Scenes.Values)
						if (!scene.SceneSNO.Name.ToLower().Contains("filler"))
							if (scene.Specification.SNOLevelAreas[0] == LevelArea)
							{
								scenes.Add(scene);
								foreach (var act in scene.Actors)
									if (act is Monster)
										monsterCount++;
							}


					player.InGameClient.SendMessage(new QuestCounterMessage()
					{
						snoQuest = BountySNOid,
						snoLevelArea = LevelArea,
						StepID = 4,
						TaskIndex = AdditionalTaskId,
						Counter = AdditionalTargetCounter,
						Checked = (AdditionalTargetNeed <= AdditionalTargetCounter) ? 1 : 0
					});
					if (monsterCount < AdditionalTargetCounter + 20)
					{
						while (monsterCount < AdditionalTargetCounter + 20)
						{
							GameServer.Core.Types.Math.Vector3D scenePoint = scenes.PickRandom().Position;
							GameServer.Core.Types.Math.Vector3D point = null;
							while (true)
							{
								point = new GameServer.Core.Types.Math.Vector3D(scenePoint.X + RandomHelper.Next(0, 240), scenePoint.Y + RandomHelper.Next(0, 240), scenePoint.Z);
								if (QuestManager.Game.GetWorld(world).CheckLocationForFlag(point, Scene.NavCellFlags.AllowWalk))
									break;
							}
							QuestManager.Game.GetWorld(world).SpawnMonster((ActorSno)GameServer.GSSystem.GeneratorsSystem.SpawnGenerator.Spawns[LevelArea].Melee.PickRandom(), point);
							monsterCount++;
						}
					} // Need additional monster spawn, there are few of them
				}
				if (Target == snoId)
				{
					TargetKilled = true;
					foreach (var player in QuestManager.Game.Players.Values)
						player.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = BountySNOid,
							snoLevelArea = LevelArea,
							StepID = 4,
							TaskIndex = TargetTaskId,
							Counter = 1,
							Checked = 1
						});
				}
				if (!TargetSpawned)
					if (QuestManager.Game.GetWorld(world).GetActorBySNO((ActorSno)Target) == null)
					{
						List<GameServer.GSSystem.MapSystem.Scene> scenes = new List<GameServer.GSSystem.MapSystem.Scene>();
						foreach (var scene in QuestManager.Game.GetWorld(world).Scenes.Values)
						{
							if (!scene.SceneSNO.Name.ToLower().Contains("filler"))
								if (scene.Specification.SNOLevelAreas[0] == LevelArea)
									scenes.Add(scene);
						}

						GameServer.Core.Types.Math.Vector3D scenePoint = scenes.PickRandom().Position;
						GameServer.Core.Types.Math.Vector3D point = null;
						while (true)
						{
							point = new GameServer.Core.Types.Math.Vector3D(scenePoint.X + RandomHelper.Next(0, 240), scenePoint.Y + RandomHelper.Next(0, 240), scenePoint.Z);
							if (QuestManager.Game.GetWorld(world).CheckLocationForFlag(point, Scene.NavCellFlags.AllowWalk))
								break;
						}
						QuestManager.Game.GetWorld(world).SpawnMonster((ActorSno)Target, point);
						TargetSpawned = true;
					}

				if (AdditionalTargetNeed <= AdditionalTargetCounter && TargetKilled)
					Complete();
			}
			if (Type == BountyData.BountyType.ClearDungeon && World == world)
			{
				if (QuestManager.Game.WorldCleared(world))
					Complete();
			}
		}

		public void CheckLevelArea(int snoId)
		{
			if (Finished) return;
			if (LevelAreaChecks.Contains(snoId))
			{
				foreach (var player in QuestManager.Game.Players.Values)
					player.InGameClient.SendMessage(new QuestCounterMessage()
					{
						snoQuest = BountySNOid,
						snoLevelArea = LevelArea,
						StepID = 4,
						TaskIndex = LevelAreaChecks.IndexOf(snoId),
						Counter = 1,
						Checked = 1
					});
			}
		}

		public void Complete()
		{
			foreach (var player in QuestManager.Game.Players.Values)
			{
				var xpReward = 1000 * player.Level * (1 + (player.Level / 7)) * QuestManager.Game.XpModifier;
				if (Type == BountyData.BountyType.KillUnique)
					xpReward *= 1.8f;
				if (Type == BountyData.BountyType.ClearDungeon)
					xpReward *= 5f;
				var goldReward = 10000 * QuestManager.Game.GoldModifier;
				player.InGameClient.SendMessage(new QuestUpdateMessage()
				{
					snoQuest = BountySNOid,
					snoLevelArea = LevelArea,
					StepID = 3,
					DisplayButton = true,
					Failed = false
				});
				player.InGameClient.SendMessage(new QuestStepCompleteMessage()
				{
					QuestStepComplete = QuestStepComplete.CreateBuilder()
						.SetIsQuestComplete(true)
						.SetReward(QuestReward.CreateBuilder()
							.SetSnoQuest(BountySNOid)
							.SetXpGranted((ulong)xpReward)
							.SetGoldGranted((int)goldReward)
							.Build()
							).Build()
				});
				// Adding the criterion!
				player.GrantCriteria(3367569);
				// Increase for the completion of the assignment.
				player.UpdateExp((int)xpReward);
				player.Inventory.AddGoldAmount((int)goldReward);
				player.Toon.TotalBounties++;
				player.UpdateAchievementCounter(412, 1);
			}
			Finished = true;
			if (++QuestManager.Game.BountiesCompleted[Act] == 5)
			{
				switch (Act)
				{
					case BountyData.ActT.A1:
						QuestManager.LaunchSideQuest(356988, true); //x1_AdventureMode_BountyTurnin_A1
						break;
					case BountyData.ActT.A2:
						QuestManager.LaunchSideQuest(356994, true); //x1_AdventureMode_BountyTurnin_A2
						break;
					case BountyData.ActT.A3:
						QuestManager.LaunchSideQuest(356996, true); //x1_AdventureMode_BountyTurnin_A3
						break;
					case BountyData.ActT.A4:
						QuestManager.LaunchSideQuest(356999, true); //x1_AdventureMode_BountyTurnin_A4
						break;
					case BountyData.ActT.A5:
						QuestManager.LaunchSideQuest(357001, true); //x1_AdventureMode_BountyTurnin_A5
						break;
				}
			}
		}
	}
}
