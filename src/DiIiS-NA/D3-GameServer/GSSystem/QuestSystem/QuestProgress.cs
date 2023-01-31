using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		}
		
		public class Quest
		{
			public bool Completed;
			public Dictionary<int, QuestStep> Steps;
			public int NextQuest;
			public int RewardXp;
			public int RewardGold;
			public bool Saveable;
		}

		public class QuestStep
		{
			public bool Completed;
			public List<Objective> Objectives = new() { Objective.Default() };
			public int NextStep;
			public Action OnAdvance;
			public bool Saveable;
		}

		public class Objective
		{
			public int Limit;
			public int Counter;
			
			public static Objective Default() => new () { Limit = 1, Counter = 0 };
		}

		// key can be ActorSno (also multiplied), DestLevelAreaSno, ConversationSno
		public Dictionary<int, QuestTrigger> QuestTriggers { get; set; }
		public Dictionary<int, QuestTrigger> GlobalQuestTriggers { get; set; }

		public int CurrentQuestRewardXp = 0;
		public int CurrentQuestRewardGold = 0;

		public QuestRegistry(Game game)
		{
			Game = game;
			QuestTriggers = new Dictionary<int, QuestTrigger>();
			GlobalQuestTriggers = new Dictionary<int, QuestTrigger>();
		}

		public virtual void SetQuests()
		{

		}


		protected void SetRiftTimer(float duration, World world, QuestEvent qevent, int idSno = 0)
		{
			Game.QuestManager.LaunchRiftQuestTimer(duration, new Action<int>((q) => { qevent.Execute(world); }), idSno);
		}

		protected void SetQuestTimer(int questId, float duration, World world, QuestEvent qevent, int Meterid = 0)
		{
			Game.QuestManager.LaunchQuestTimer(questId, duration, new Action<int>((q) => { qevent.Execute(world); }), Meterid);
		}

		protected void ListenConversation(int convId, QuestEvent qevent)
		{
			QuestTriggers.TryAdd(convId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation, count = 1, counter = 0, questEvent = qevent });
		}

		protected void GlobalListenConversation(int convId, QuestEvent qevent)
		{
			GlobalQuestTriggers.TryAdd(convId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenKill(ActorSno monsterSno, int monsterCount, QuestEvent qevent)
		{
			QuestTriggers.TryAdd((int)monsterSno,
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
			var doors = world.GetAllDoors(sno);
			if (!doors.Any()) return false;
			foreach (var door in doors)
				door.Open();
            return true;
		}
		
		//opening all doors
		protected bool OpenAll(World world)
		{
			var doors = world.GetAllDoors();
			if (!doors.Any()) return false;
			foreach (var door in doors)
				door.Open();
			return true;
		}
		
		protected void ListenKillBonus(ActorSno monsterSno, int monsterCount, QuestEvent qevent)
		{
			QuestTriggers.TryAdd((int)monsterSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup, count = monsterCount, counter = 0, questEvent = qevent });
		}

		protected void ListenTeleport(int laId, QuestEvent qevent)
		{
			QuestTriggers.TryAdd(laId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea, count = 1, counter = 0, questEvent = qevent });
		}
		protected void GlobalListenTeleport(int laId, QuestEvent qevent)
		{
			GlobalQuestTriggers.TryAdd(laId,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenProximity(ActorSno actorSno, QuestEvent qevent)
		{
			QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, count = 1, counter = 0, questEvent = qevent });
		}

		protected void ListenInteract(ActorSno actorSno, int actorCount, QuestEvent qevent)
		{
			QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = 0, questEvent = qevent });
		}
		protected void ListenInteractBonus(ActorSno actorSno, int actorCount, int counter, QuestEvent qevent)
		{
			QuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = counter, questEvent = qevent });
		}
		protected void GlobalListenInteract(ActorSno actorSno, int actorCount, QuestEvent qevent)
		{
			GlobalQuestTriggers.TryAdd((int)actorSno,
				new QuestTrigger { triggerType = DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor, count = actorCount, counter = 0, questEvent = qevent });
		}

		protected void UnlockTeleport(int waypointId)
		{
			Game.UnlockTeleport(waypointId);
		}

		public void UpdateCounter(int dataId)
		{
			var trigger = QuestTriggers[dataId];
			trigger.counter++;
			QuestTriggers[dataId] = trigger;
			if (trigger.counter <= trigger.count)
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
					Game.QuestManager.NotifyBonus(trigger.counter, (trigger.counter >= trigger.count));
				else if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor && dataId == 3628)
					Game.QuestManager.NotifyBonus(trigger.counter, (trigger.counter >= trigger.count));
				else
					Game.QuestManager.NotifyQuest(trigger.counter, (trigger.counter >= trigger.count));
		}

		public void UpdateSideCounter(int dataId)
		{
			var trigger = QuestTriggers[dataId];
			trigger.counter++;
			QuestTriggers[dataId] = trigger;
			if (trigger.counter <= trigger.count)
				Game.QuestManager.NotifySideQuest(trigger.counter, (trigger.counter >= trigger.count));
		}

		public void UpdateGlobalCounter(int dataId)
		{
			var trigger = GlobalQuestTriggers[dataId];
			trigger.counter++;
			GlobalQuestTriggers[dataId] = trigger;
		}

		//Launch Conversations.
		protected bool StartConversation(World world, Int32 conversationId)
		{
			foreach (var plr in world.Players)
				plr.Value.Conversations.StartConversation(conversationId);
			return true;
		}

		public bool HasFollower(ActorSno sno)
		{
			return Game.Players.Values.First().Followers.Any(x => x.Value == sno);
		}

		public void AddFollower(World world, ActorSno sno)
		{
			if (Game.Players.Count > 0)
				Game.Players.Values.First().AddFollower(world.GetActorBySNO(sno));
		}

		public void DestroyFollower(ActorSno sno)
		{
			if (Game.Players.Count > 0)
				Game.Players.Values.First().DestroyFollower(sno);
		}

		protected void PlayCutscene(Int32 cutsceneId)
		{
			if (!Game.Empty)
				foreach (var player in Game.Players)
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
			if (Game.Players.Count > 0)
				Game.QuestManager.Advance();
		}
	}

	public abstract class QuestEvent
	{
		Logger logger = new Logger("Conversation");

		public uint ConversationSNOId { get; set; }


		public QuestEvent(uint conversationSNOId)
		{
			ConversationSNOId = conversationSNOId;
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
