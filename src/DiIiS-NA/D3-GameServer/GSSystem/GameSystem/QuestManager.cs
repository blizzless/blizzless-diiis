//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public class QuestManager
	{
		private static readonly Logger Logger = new Logger("QuestManager");

		/// <summary>
		/// Accessor for quests
		/// </summary>
		/// <param name="snoQuest">snoId of the quest to retrieve</param>
		/// <returns></returns>
		public Dictionary<int, QuestRegistry.Quest> Quests = new Dictionary<int, QuestRegistry.Quest>();

		public Dictionary<int, QuestRegistry.Quest> SideQuests = new Dictionary<int, QuestRegistry.Quest>();

		public List<Bounty> Bounties = new List<Bounty>(); 
		
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
			
			var kill_boss_bounties = ItemGenerator.Bounties.Values
				.Where(b => b.BountyData0.Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillBoss)
				.Where(b => !Bounties.Select(b => b.LevelArea).Contains(b.BountyData0.LeveAreaSNO0))
				.OrderBy(b => FastRandom.Instance.Next())
				.Select(b => new Bounty()
				{
					QuestManager = this,
					BountySNOid = b.Header.SNOId,
					Act = b.BountyData0.ActData,
					Type = b.BountyData0.Type,
					LevelArea = b.BountyData0.LeveAreaSNO0,
					World = WorldSno.__NONE,
					Target = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Single(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster).SNOName1.Id,
					TargetTaskId = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.ToList()
								.FindIndex(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster),
					AdditionalTaskId = 0,
					AdditionalTargetCounter = 0,
					AdditionalTargetNeed = 0,
					LevelAreaChecks = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Where(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
								.Select(o => o.SNOName1.Id)
								.ToList()
				});
			Bounties.AddRange(kill_boss_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A1).Take(1)); //A1
			Bounties.AddRange(kill_boss_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A2).Take(1)); //A2
			Bounties.AddRange(kill_boss_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A3).Take(1)); //A3
			Bounties.AddRange(kill_boss_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A4).Take(1)); //A4
			Bounties.AddRange(kill_boss_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A5).Take(1)); //A5

			var kill_unique_bounties = ItemGenerator.Bounties.Values
				.Where(b => b.BountyData0.Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillUnique)
				.Where(b => !Bounties.Select(b => b.LevelArea).Contains(b.BountyData0.LeveAreaSNO0))
				.OrderBy(b => FastRandom.Instance.Next())
				.GroupBy(b => b.BountyData0.LeveAreaSNO0)//b.QuestSteps.SelectMany(s => s.StepObjectiveSets).SelectMany(s => s.StepObjectives).Single(o => o.ObjectiveType == Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.KillAny).SNOName1.Id)
				.Select(b => b.First())
				.Select(b => new Bounty()
				{
					QuestManager = this,
					BountySNOid = b.Header.SNOId,
					Act = b.BountyData0.ActData,
					Type = b.BountyData0.Type,
					LevelArea = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Single(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillAny).SNOName1.Id,
					World = WorldSno.__NONE,
					Target = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Single(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster).SNOName1.Id,
					TargetTaskId = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.ToList()
								.FindIndex(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster),
					AdditionalTaskId = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.ToList()
								.FindIndex(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillAny),
					AdditionalTargetCounter = 0,
					AdditionalTargetNeed = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Single(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillAny).CounterTarget,
					LevelAreaChecks = b.QuestSteps
								.SelectMany(s => s.StepObjectiveSets)
								.SelectMany(s => s.StepObjectives)
								.Where(o => o.ObjectiveType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
								.Select(o => o.SNOName1.Id)
								.ToList()
				});
			Bounties.AddRange(kill_unique_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A1).Take(4)); //A1
			Bounties.AddRange(kill_unique_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A2).Take(4)); //A2
			Bounties.AddRange(kill_unique_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A3).Take(4)); //A3
			Bounties.AddRange(kill_unique_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A4).Take(4)); //A4
			Bounties.AddRange(kill_unique_bounties.Where(b => b.Act == DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A5).Take(4)); //A5
		}

		/// <summary>
		/// Advances a quest by a step
		/// </summary>
		/// <param name="snoQuest">snoID of the quest to advance</param>                               
		public void Advance()
		{
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
			}
			else
			{
				Quests[Game.CurrentQuest].Completed = true;
				if (!Game.Empty)
				{
					SaveQuestProgress(true);
					Logger.Trace(" (Advance) quest {0} completed: {1}", Game.CurrentQuest, Quests[Game.CurrentQuest].Completed);
					foreach (var player in Game.Players.Values)
					{
						int xpReward = (int)(Quests[Game.CurrentQuest].RewardXp * Game.XPModifier);
						int goldReward = (int)(Quests[Game.CurrentQuest].RewardGold * Game.GoldModifier);
						if (Game.CurrentQuest != 312429)
						{
							player.InGameClient.SendMessage(new QuestStepCompleteMessage()
							{
								QuestStepComplete = D3.Quests.QuestStepComplete.CreateBuilder()

								.SetReward(D3.Quests.QuestReward.CreateBuilder()
									.SetGoldGranted(goldReward)
									.SetXpGranted((ulong)xpReward)
									.SetSnoQuest(Game.CurrentQuest)
									)
								.SetIsQuestComplete(true)
								.Build()
								//snoQuest = this.Game.CurrentQuest,
								//isQuestComplete = true,
								//rewardXp = xpReward,
								//rewardGold = goldReward
							});
							player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.FloatingAmountMessage()
							{
								Place = new WorldPlace()
								{
									Position = player.Position,
									WorldID = player.World.DynamicID(player),
								},

								Amount = xpReward,
								Type = MessageSystem.Message.Definitions.Base.FloatingAmountMessage.FloatType.Experience,
							});
							player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.FloatingAmountMessage()
							{
								Place = new WorldPlace()
								{
									Position = player.Position,
									WorldID = player.World.DynamicID(player),
								},

								Amount = goldReward,
								Type = MessageSystem.Message.Definitions.Base.FloatingAmountMessage.FloatType.Gold,
							});
							player.UpdateExp(xpReward);
							player.Inventory.AddGoldAmount(goldReward);
							player.AddAchievementCounter(74987243307173, (uint)goldReward);
							player.CheckQuestCriteria(Game.CurrentQuest);
						}
					};
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
				if (Quests[Game.CurrentQuest].Steps[Game.CurrentStep].Saveable)
					SaveQuestProgress(false);
			}
			OnQuestProgress();
			AutoSetQuestMarker();
			Logger.Trace(" (Advance) Advanced to quest {0}, step {1}", Game.CurrentQuest, Game.CurrentStep);
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
				Logger.Trace(" (SideAdvance) quest {0} completed: {1}", Game.CurrentSideQuest, SideQuests[Game.CurrentSideQuest].Completed);

				foreach (var player in Game.Players.Values)
				{
					int xpReward = (int)(SideQuests[Game.CurrentSideQuest].RewardXp * Game.XPModifier);
					int goldReward = (int)(SideQuests[Game.CurrentSideQuest].RewardGold * Game.GoldModifier);

					player.InGameClient.SendMessage(new QuestStepCompleteMessage()
					{
						QuestStepComplete = D3.Quests.QuestStepComplete.CreateBuilder()
							
							.SetReward(D3.Quests.QuestReward.CreateBuilder()
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
						player.World.SpawnRandomEquip(player, player, LootManager.Epic, player.Attributes[GameAttribute.Level]);
					}
					var toon = player.Toon.DBToon;
					toon.EventsCompleted++;
					Game.GameDBSession.SessionUpdate(toon);
					player.CheckQuestCriteria(Game.CurrentSideQuest);
				};

				Game.CurrentSideQuest = -1;
				Game.CurrentSideStep = -1;
			}

			OnQuestProgress();
			Logger.Trace(" (SideAdvance) Advanced to side quest {0}, step {1}", Game.CurrentSideQuest, Game.CurrentSideStep);
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

		public float QuestTimerEstimate = 0f;

		public void LaunchRiftQuestTimer(float duration, Action<int> onDone, int idSNO = 0)
		{
			foreach (var player in Game.Players.Values)
			{
				
			};

			QuestTimerEstimate = duration;

			Game.QuestTimer = SteppedTickTimer.WaitSecondsStepped(Game, 1f, duration, new Action<int>((q) =>
			{
				QuestTimerEstimate -= 1f;
				
			}),
			onDone);
		}

		public void LaunchQuestTimer(int questId, float duration, Action<int> onDone, int Meterid = 0)
		{
			foreach (var player in Game.Players.Values)
			{
				player.InGameClient.SendMessage(new QuestMeterMessage()
				{
					snoQuest = questId,
					annMeter = Meterid,
					flMeter = 1f
				});
			};

			QuestTimerEstimate = duration;

			Game.QuestTimer = SteppedTickTimer.WaitSecondsStepped(Game, 1f, duration, new Action<int>((q) =>
			{
				QuestTimerEstimate -= 1f;
				foreach (var player in Game.Players.Values)
				{
					player.InGameClient.SendMessage(new QuestMeterMessage()
					{
						snoQuest = questId,
						annMeter = Meterid,
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
			foreach (var player in Game.Players.Values)
				player.InGameClient.SendMessage(new QuestCounterMessage()
				{
					snoQuest = Game.CurrentQuest,
					snoLevelArea = -1,
					StepID = Game.CurrentStep,
					TaskIndex = objId,
					Counter = objective.Counter,
					Checked = objective.Counter < objective.Limit ? 0 : 1,
				});

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
			if (Game.QuestProgress.QuestTriggers.Count == 1)
			{
				Logger.Trace("AutoSetQuestMarker()");
				var trigger = Game.QuestProgress.QuestTriggers.First();
				if (trigger.Value.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
					foreach (var world in Game.Worlds)
					{
						var actors = world.GetActorsBySNO((ActorSno)trigger.Key).Where(d => d.Visible);
						Actor actor = null;
						if (actors.Count() == 1) actor = actors.First();
						if (actor != null)
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MapMarkerInfoMessage
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
								});
					}

				if (trigger.Value.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation)
					foreach (var world in Game.Worlds)
					{
						var actors = world.Actors.Values.Where(d => d.Visible && (d is InteractiveNPC) && (d as InteractiveNPC).Conversations.Any(c => c.ConversationSNO == trigger.Key));
						Actor actor = null;
						if (actors.Count() == 1) actor = actors.First();
						if (actor != null)
							foreach (var plr in world.Players.Values)
								plr.InGameClient.SendMessage(new MapMarkerInfoMessage
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
								});
					}
			}
		}

		public void SetBountyMarker(Player player)
		{
			foreach (var bounty in Bounties.Where(b => !b.Finished && b.Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillUnique))
			{
				var unique = player.World.GetActorsBySNO((ActorSno)bounty.Target).Where(u => !u.Dead).FirstOrDefault();
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
			if (Quests.ContainsKey(snoQuest) || SideQuests.ContainsKey(snoQuest))
			{
				if (strictFilter)
				{
					if ((Game.CurrentQuest == snoQuest) && (Game.CurrentStep == Step)
						||
					(Game.CurrentSideQuest == snoQuest) && (Game.CurrentSideStep == Step))
						return true;
				}
				else
				{
					if ((Game.CurrentQuest == snoQuest || snoQuest == -1) && (Game.CurrentStep == Step || Step == -1 || Step == 0)
						||
					(Game.CurrentSideQuest == snoQuest || snoQuest == -1) && (Game.CurrentSideStep == Step || Step == -1 || Step == 0))
						return true;
				}
			}
			return false;
		}

		public bool HasQuest(int snoQuest)
		{
			return Quests.ContainsKey(snoQuest);
		}

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

		public bool IsDone(int snoQuest)
		{
			return Quests.ContainsKey(snoQuest) && Quests[snoQuest].Completed;
		}

		public bool IsInQuestRange(DiIiS_NA.Core.MPQ.FileFormats.QuestRange range)
		{
			
			if (range.Header.SNOId == 312431) return (Game.CurrentAct == 3000);
			if (range.Header.SNOId == 214766) return true; 

			bool started = false;
			bool ended = false;


			foreach (var range_entry in range.Enitys)
			{
				if (range_entry != null)
				{
					if (range_entry.Start.SNOQuest == -1 || range_entry.Start.StepID == -1)
						started = true;
					else
					{
						if (Quests.ContainsKey(range_entry.Start.SNOQuest) && Quests[range_entry.Start.SNOQuest].Steps.ContainsKey(range_entry.Start.StepID))
						{
							if (Quests[range_entry.Start.SNOQuest].Completed ||
							Quests[range_entry.Start.SNOQuest].Steps[range_entry.Start.StepID].Completed ||
							(Game.CurrentQuest == range_entry.Start.SNOQuest && Game.CurrentStep == range_entry.Start.StepID)) // rumford conversation needs current step
								started = true;
						}
						//else logger.Warn("QuestRange {0} references unknown quest {1}", range.Header.SNOId, range.Start.SNOQuest);
					}

					//Logger.Debug("IsInQuestRange {0} and started? {1} ", range.Header.SNOId, started);

					if (range_entry.End.SNOQuest == -1 || range_entry.End.StepID < 0)
						ended = false;
					else
					{
						if (Quests.ContainsKey(range_entry.End.SNOQuest) && Quests[range_entry.End.SNOQuest].Steps.ContainsKey(range_entry.End.StepID))
						{
							if (Quests[range_entry.End.SNOQuest].Completed ||
								Quests[range_entry.End.SNOQuest].Steps[range_entry.End.StepID].Completed)
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
			foreach (var player in Game.Players.Values)
			{
				player.Toon.CurrentAct = CurrentAct;
				player.Toon.CurrentQuestId = Game.CurrentQuest;
				player.Toon.CurrentQuestStepId = Game.CurrentStep;

				List<DBQuestHistory> query = Game.GameDBSession.SessionQueryWhere<DBQuestHistory>(
					dbi => dbi.DBToon.Id == player.Toon.PersistentID && dbi.QuestId == Game.CurrentQuest);
				if (query.Count == 0)
				{
					var questHistory = new DBQuestHistory();
					questHistory.DBToon = player.Toon.DBToon;
					questHistory.QuestId = Game.CurrentQuest;
					questHistory.QuestStep = Game.CurrentStep;
					Game.GameDBSession.SessionSave(questHistory);
				}
				else
				{
					var questHistory = query.First();
					if (Quests[Game.CurrentQuest].Steps[questHistory.QuestStep].Completed)
					{
						questHistory.QuestStep = Game.CurrentStep;
						if (questCompleted) questHistory.isCompleted = true;
						Game.GameDBSession.SessionUpdate(questHistory);
					}
				}
			}
		}
	}

	public class Bounty
	{
		public QuestManager QuestManager { get; set; }
		public int BountySNOid { get; set; }
		public DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT Act { get; set; }
		public DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType Type { get; set; }
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

		public static Dictionary<int, int> LevelAreaOverrides = new Dictionary<int, int>() //first is in-game, second is in-data
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
			if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillUnique && LevelArea == levelArea && AdditionalTargetNeed != AdditionalTargetCounter)
			{
				var Quest = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Quest][BountySNOid];
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
			if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillBoss && Target == snoId)
			{
				Complete();
			}
			if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillUnique && (LevelArea == levelArea || (LevelAreaOverrides.ContainsKey(levelArea) && LevelAreaOverrides[levelArea] == LevelArea)))
			{
				AdditionalTargetCounter++;
				foreach (var player in QuestManager.Game.Players.Values)
				{
					List<MapSystem.Scene> Scenes = new List<MapSystem.Scene>();
					int MonsterCount = 0;
					foreach (var scene in QuestManager.Game.GetWorld(world).Scenes.Values)
						if (!scene.SceneSNO.Name.ToLower().Contains("filler"))
							if (scene.Specification.SNOLevelAreas[0] == LevelArea)
							{
								Scenes.Add(scene);
								foreach (var act in scene.Actors)
									if (act is Monster)
										MonsterCount++;
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
					if (MonsterCount < AdditionalTargetCounter + 20)
					{
						while (MonsterCount < AdditionalTargetCounter + 20)
						{
							Core.Types.Math.Vector3D SSV = Scenes[RandomHelper.Next(0, Scenes.Count)].Position;
							Core.Types.Math.Vector3D SP = null;
							while (true)
							{
								SP = new Core.Types.Math.Vector3D(SSV.X + RandomHelper.Next(0, 240), SSV.Y + RandomHelper.Next(0, 240), SSV.Z);
								if (QuestManager.Game.GetWorld(world).CheckLocationForFlag(SP, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
									break;
							}
							QuestManager.Game.GetWorld(world).SpawnMonster((ActorSno)GeneratorsSystem.SpawnGenerator.Spawns[LevelArea].melee[FastRandom.Instance.Next(GeneratorsSystem.SpawnGenerator.Spawns[LevelArea].melee.Count())], SP);
							MonsterCount++;
						}
					} //Нужен дополнительный спаун монстров, их мало
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
						List<MapSystem.Scene> Scenes = new List<MapSystem.Scene>();
						foreach (var scene in QuestManager.Game.GetWorld(world).Scenes.Values)
						{
							if (!scene.SceneSNO.Name.ToLower().Contains("filler"))
								if (scene.Specification.SNOLevelAreas[0] == LevelArea)
									Scenes.Add(scene);
						}

						Core.Types.Math.Vector3D SSV = Scenes[RandomHelper.Next(0, Scenes.Count - 1)].Position;
						Core.Types.Math.Vector3D SP = null;
						while (true)
						{
							SP = new Core.Types.Math.Vector3D(SSV.X + RandomHelper.Next(0, 240), SSV.Y + RandomHelper.Next(0, 240), SSV.Z);
							if (QuestManager.Game.GetWorld(world).CheckLocationForFlag(SP, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
								break;
						}
						QuestManager.Game.GetWorld(world).SpawnMonster((ActorSno)Target, SP);
						TargetSpawned = true;
					}

				if (AdditionalTargetNeed <= AdditionalTargetCounter && TargetKilled)
					Complete();
			}
			if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.ClearDungeon && World == world)
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
				var xpReward = 1000 * player.Level * (1 + (player.Level / 7)) * QuestManager.Game.XPModifier;
				if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.KillUnique)
					xpReward *= 1.8f;
				if (Type == DiIiS_NA.Core.MPQ.FileFormats.BountyData.BountyType.ClearDungeon)
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
					QuestStepComplete = D3.Quests.QuestStepComplete.CreateBuilder()
						.SetIsQuestComplete(true)
						.SetReward(D3.Quests.QuestReward.CreateBuilder()
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
			QuestManager.Game.BountiesCompleted[Act]++;
			if (QuestManager.Game.BountiesCompleted[Act] == 5)
			{
				switch (Act)
				{
					case DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A1:
						QuestManager.LaunchSideQuest(356988, true); //x1_AdventureMode_BountyTurnin_A1
						break;
					case DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A2:
						QuestManager.LaunchSideQuest(356994, true); //x1_AdventureMode_BountyTurnin_A2
						break;
					case DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A3:
						QuestManager.LaunchSideQuest(356996, true); //x1_AdventureMode_BountyTurnin_A3
						break;
					case DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A4:
						QuestManager.LaunchSideQuest(356999, true); //x1_AdventureMode_BountyTurnin_A4
						break;
					case DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A5:
						QuestManager.LaunchSideQuest(357001, true); //x1_AdventureMode_BountyTurnin_A5
						break;
				}
			}
		}
	}
}
