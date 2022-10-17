//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
	public class QuestRegistry
	{
		public Game Game { get; private set; }

		protected QuestEvent script = null;

		public struct QuestTrigger
		{
			public DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType triggerType;
			public int count;
			public int counter;
			public QuestEvent questEvent;
		};

		public class Quest
		{
			public bool Completed;
			public Dictionary<int, QuestStep> Steps;
			public int NextQuest;
			public int RewardXp;
			public int RewardGold;
			public bool Saveable;
		};

		public class QuestStep
		{
			public bool Completed;
			public List<Objective> Objectives;
			public int NextStep;
			public Action OnAdvance;
			public bool Saveable;
		};

		public class Objective
		{
			public int Limit;
			public int Counter;
		};

		// key can be ActorSno (also multiplied), DestLevelAreaSno, ConversationSno
		public Dictionary<int, QuestTrigger> QuestTriggers { get; set; }
		public Dictionary<int, QuestTrigger> GlobalQuestTriggers { get; set; }

		public int CurrentQuestRewardXp = 0;
		public int CurrentQuestRewardGold = 0;

		public QuestRegistry(Game game)
		{
			this.Game = game;
			this.QuestTriggers = new Dictionary<int, QuestTrigger>();
			this.GlobalQuestTriggers = new Dictionary<int, QuestTrigger>();
		}

		public virtual void SetQuests()
		{

		}


		protected void SetRiftTimer(float duration, MapSystem.World world, QuestEvent qevent, int idSNO = 0)
		{
			this.Game.QuestManager.LaunchRiftQuestTimer(duration, new Action<int>((q) => { qevent.Execute(world); }), idSNO);
		}

		protected void SetQuestTimer(int questId, float duration, MapSystem.World world, QuestEvent qevent, int Meterid = 0)
		{
			this.Game.QuestManager.LaunchQuestTimer(questId, duration, new Action<int>((q) => { qevent.Execute(world); }), Meterid);
		}

		protected void ListenConversation(int convId, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd(convId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation, count = 1, counter = 0, questEvent = qevent });
		}

		protected void GlobalListenConversation(int convId, QuestEvent qevent)
		{
			this.GlobalQuestTriggers.TryAdd(convId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenKill(ActorSno monsterSno, int monsterCount, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd((int)monsterSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster, count = monsterCount, counter = 0, questEvent = qevent });
		}

		public void ActiveArrow(World world, ActorSno sno, WorldSno destworld = WorldSno.__NONE)
		{
			Actor target = null;
			if (destworld != WorldSno.__NONE)
			{
				foreach (Portal tar in world.GetActorsBySNO(sno))
					if (tar.Destination.WorldSNO == (int)destworld)
						target = tar;
			}
			else
				target = world.GetActorBySNO(sno, true);

			world.BroadcastGlobal(plr => new MapMarkerInfoMessage()
			{
				HashedName = StringHashHelper.HashItemName("QuestMarker"),
				Place = new WorldPlace { Position = target.Position, WorldID = target.World.GlobalID },
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
		public void DisableArrow(World world, Actor target)
		{
			world.BroadcastGlobal(plr => new MapMarkerInfoMessage()
			{
				HashedName = StringHashHelper.HashItemName("QuestMarker"),
				Place = new WorldPlace { Position = target.Position, WorldID = target.World.GlobalID },
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
		}

		public void ActivateQuestMonsters(World world, ActorSno sno)
		{
            foreach (var actr in world.Actors.Values.Where(x => x.SNO == sno))
			{
				actr.Attributes[GameAttribute.Quest_Monster] = true;
				actr.Attributes.BroadcastChangedIfRevealed();
			}
		}
		public void DeactivateQuestMonsters(World world, ActorSno sno)
		{
			foreach (var actr in world.Actors.Values.Where(x => x.SNO == sno))
            {
				actr.Attributes[GameAttribute.Quest_Monster] = false;
				actr.Attributes.BroadcastChangedIfRevealed();
            }
        }

		//opening gates or door(for getting pass)
		protected bool Open(World world, ActorSno sno)
		{
			var actor = world.GetActorsBySNO(sno).Where(d => d.Visible).FirstOrDefault();
            if (actor == null)
                return false;
            
            (actor as Door).Open();
            return true;
		}

		protected bool OpenAll(World world, ActorSno sno)
		{
			foreach (var actor in world.GetActorsBySNO(sno).Where(d => d.Visible).ToList())
				(actor as Door).Open();
			return true;
		}

		protected void ListenKillBonus(ActorSno monsterSno, int monsterCount, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd((int)monsterSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup, count = monsterCount, counter = 0, questEvent = qevent });
		}

		protected void ListenTeleport(int laId, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd(laId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea, count = 1, counter = 0, questEvent = qevent });
		}
		protected void GlobalListenTeleport(int laId, QuestEvent qevent)
		{
			this.GlobalQuestTriggers.TryAdd(laId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenProximity(ActorSno actorSno, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenInteract(ActorSno actorSno, int actorCount, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = 0, questEvent = qevent });
		}
		protected void ListenInteractBonus(ActorSno actorSno, int actorCount, int counter, QuestEvent qevent)
		{
			this.QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = counter, questEvent = qevent });
		}
		protected void GlobalListenInteract(ActorSno actorSno, int actorCount, QuestEvent qevent)
		{
			this.GlobalQuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = 0, questEvent = qevent });
		}

		protected void UnlockTeleport(int waypointId)
		{
			this.Game.UnlockTeleport(waypointId);
		}

		public void UpdateCounter(int dataId)
		{
			var trigger = this.QuestTriggers[dataId];
			trigger.counter++;
			this.QuestTriggers[dataId] = trigger;
			if (trigger.counter <= trigger.count)
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
					this.Game.QuestManager.NotifyBonus(trigger.counter, (trigger.counter >= trigger.count));
				else if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor && dataId == 3628)
					this.Game.QuestManager.NotifyBonus(trigger.counter, (trigger.counter >= trigger.count));
				else
					this.Game.QuestManager.NotifyQuest(trigger.counter, (trigger.counter >= trigger.count));
		}

		public void UpdateSideCounter(int dataId)
		{
			var trigger = this.QuestTriggers[dataId];
			trigger.counter++;
			this.QuestTriggers[dataId] = trigger;
			if (trigger.counter <= trigger.count)
				this.Game.QuestManager.NotifySideQuest(trigger.counter, (trigger.counter >= trigger.count));
		}

		public void UpdateGlobalCounter(int dataId)
		{
			var trigger = this.GlobalQuestTriggers[dataId];
			trigger.counter++;
			this.GlobalQuestTriggers[dataId] = trigger;
		}

		//Launch Conversations.
		protected bool StartConversation(World world, Int32 conversationId)
		{
			foreach (var plr in world.Players)
				plr.Value.Conversations.StartConversation(conversationId);
			return true;
		}

		public void AddFollower(World world, ActorSno sno)
		{
			if (this.Game.Players.Count > 0)
				this.Game.Players.Values.First().AddFollower(world.GetActorBySNO(sno));
		}

		public void DestroyFollower(ActorSno sno)
		{
			if (this.Game.Players.Count > 0)
				this.Game.Players.Values.First().DestroyFollower(sno);
		}

		protected void PlayCutscene(Int32 cutsceneId)
		{
			if (!this.Game.Empty)
				foreach (var player in this.Game.Players)
				{
					player.Value.PlayCutscene(cutsceneId);
				}
		}

		//Not Operable Rumford (To disable giving u the same quest while ur in the event)
		public static bool SetActorOperable(World world, ActorSno sno, bool status)
		{
			var actor = world.GetActorBySNO(sno);

			if (actor == null)
				return false;

			actor.Attributes[GameAttribute.Team_Override] = (status ? -1 : 2);
			actor.Attributes[GameAttribute.Untargetable] = !status;
			actor.Attributes[GameAttribute.NPC_Is_Operatable] = status;
			actor.Attributes[GameAttribute.Operatable] = status;
			actor.Attributes[GameAttribute.Operatable_Story_Gizmo] = status;
			actor.Attributes[GameAttribute.Disabled] = !status;
			actor.Attributes[GameAttribute.Immunity] = !status;
			actor.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public static bool SetActorVisible(World world, ActorSno sno, bool status)
		{
			var actor = world.GetActorBySNO(sno, true);

			if (actor == null)
				return false;

			actor.Attributes[GameAttribute.NPC_Is_Operatable] = status;
			actor.Attributes[GameAttribute.Operatable] = status;
			actor.Attributes[GameAttribute.Operatable_Story_Gizmo] = status;
			actor.Attributes[GameAttribute.Untargetable] = !status;
			actor.Attributes[GameAttribute.Disabled] = !status;
			actor.Attributes[GameAttribute.Immunity] = !status;
			actor.Attributes[GameAttribute.Hidden] = !status;
			actor.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public void Advance(int questId)
		{
			if (this.Game.Players.Count > 0)
				this.Game.QuestManager.Advance();
		}
	}

	public abstract class QuestEvent
	{
		Logger logger = new Logger("Conversation");

		public uint ConversationSNOId { get; set; }


		public QuestEvent(uint conversationSNOId)
		{
			this.ConversationSNOId = conversationSNOId;
		}

		public abstract void Execute(World world);

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
				foreach (var N in actor.World.GetActorsBySNO(actor.SNO))
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
	}
}
