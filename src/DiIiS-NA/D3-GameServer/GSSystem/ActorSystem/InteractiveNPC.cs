using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.NPC;
using DiIiS_NA.GameServer.ClientSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.D3_GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class InteractiveNPC : NPC, IMessageConsumer
	{

		public static Logger Logger = new Logger("InteractiveNPC");


		public List<IInteraction> Interactions { get; private set; }
		public List<ConversationInteraction> Conversations { get; private set; }
		public bool OverridedConv = false;
		public int ImportantConversationSNO = -1;
		public int SideQuestSNOConv = -1;
		public int ForceConversationSNO = -1;
		public InteractiveNPC(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.NPC_Has_Interact_Options, 0] = true; //second param - playerIndex
			Attributes[GameAttributes.NPC_Has_Interact_Options, 1] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 2] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 3] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 4] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 5] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 6] = true;
			Attributes[GameAttributes.NPC_Has_Interact_Options, 7] = true;
			Attributes[GameAttributes.NPC_Is_Operatable] = true;
			Attributes[GameAttributes.MinimapActive] = true;
			Interactions = new List<IInteraction>();
			Conversations = new List<ConversationInteraction>();

			
			World.Game.QuestManager.OnQuestProgress += new QuestManager.QuestProgressDelegate(QuestProgress);
			UpdateConversationList(); // show conversations with no quest dependency
		}

		protected override void QuestProgress() // shadows Actors'Mooege.Core.GS.Actors.InteractiveNPC.quest_OnQuestProgress(Mooege.Core.GS.Games.Quest)'
		{
			if (this is Hireling && (this as Hireling).IsHireling) return;
			// call base classe update range stuff			
			try
			{
				UpdateQuestRangeVisibility();
				UpdateConversationList();
			}
			catch { }
			// Logger.Debug(" (quesy_OnQuestProgress) has been called -> updatin conversaton list ");
		}

		

		public override bool Reveal(Player player)
		{
			if (SNO == ActorSno._a1_uniquevendor_armorer) return false;
			if (SNO == ActorSno._tyrael_heaven && World.SNO == WorldSno.trout_town && World.Game.CurrentAct != ActEnum.OpenWorld) return false;
			return base.Reveal(player);
		}

		private void UpdateConversationList()
		{
			ImportantConversationSNO = -1;
			OverridedConv = false;
			bool withames = false;

			if (ConversationList != null) // this is from Actor
			{
				var ConversationsNew = new List<int>();
				foreach (var entry in ConversationList.ConversationListEntries) // again on actor
				{
					//190178
					if (!DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Conversation].ContainsKey(entry.SNOConversation)) continue; //save from incorrent SNO Ids
					if (entry.SNOConversation == 181330)
						ConversationsNew.Add(entry.SNOConversation);
					if (World == null) return;
					if (World.Game.CurrentAct != null && entry.SpecialEventFlag != (int)World.Game.CurrentAct) continue;

					if (entry.SNOQuestActive == -1)
						ConversationsNew.Add(entry.SNOConversation);

					if (entry.Type != ConversationTypes.GlobalFloat)
						withames = true;

					if (World.Game.QuestManager.HasCurrentQuest(entry.SNOQuestActive, entry.SNOQuestRange, (entry.ConditionReqs == 1)) || World.Game.QuestProgress.QuestTriggers.ContainsKey(entry.SNOConversation))
					{
						ConversationsNew.Add(entry.SNOConversation);
						if (entry.ConditionReqs == 1)
							ImportantConversationSNO = entry.SNOConversation;
					}
					else if (World.Game.CurrentSideStep == -1 & World.Game.CurrentSideQuest == -1)
						if (entry.SNOQuestRange == -3)
						{
							ConversationsNew.Add(entry.SNOConversation);
							ImportantConversationSNO = entry.SNOConversation;
							SideQuestSNOConv = entry.SNOConversation;
						}
				}

				// remove outdates conversation options and add new ones
				Conversations = Conversations.Where(x => ConversationsNew.Contains(x.ConversationSNO)).ToList(); // this is in the InteractiveNPC
				foreach (var sno in ConversationsNew)
					if (!Conversations.Select(x => x.ConversationSNO).Contains(sno))
						Conversations.Add(new ConversationInteraction(sno, withames));

				

				// search for an unread questconversation
				bool questConversation = false;
				bool sideConversation = false;

				foreach (var conversation in Conversations) // this is in the InteractiveNPC
					if (conversation.ConversationSNO == ImportantConversationSNO)
					{
						if (conversation.Read == false) questConversation = true;
					}
					else
						sideConversation = true;

				ConversationInteraction ToDeleteSide = null;
				if (SideQuestSNOConv > -1)
					foreach (var conv in Conversations)
						if (conv.Read == true & conv.ConversationSNO == SideQuestSNOConv)
							ToDeleteSide = conv;
				if (ToDeleteSide != null)
					Conversations.Remove(ToDeleteSide);

				if (this is Healer) return;
				
				Attributes[GameAttributes.Conversation_Icon, 0] = questConversation ? 2 : (sideConversation ? 4 : 0);
				
				if (ForceConversationSNO != -1)
				{
					questConversation = true;
					Attributes[GameAttributes.Conversation_Icon, 0] = questConversation ? 2 : (sideConversation ? 4 : 0);
					Conversations.Add(new ConversationInteraction(ForceConversationSNO));
				}

				Attributes.BroadcastChangedIfRevealed();
			}
			if (SNO == ActorSno._tyrael_heaven && Tags.ContainsKey(MarkerKeys.QuestRange) && Tags[MarkerKeys.QuestRange].Id == 312431) //TyraelBountyTurnin
			{
				bool active =
					World.Game.CurrentSideQuest == 356988 ||
					World.Game.CurrentSideQuest == 356994 ||
					World.Game.CurrentSideQuest == 356996 ||
					World.Game.CurrentSideQuest == 356999 ||
					World.Game.CurrentSideQuest == 357001;
				Attributes[GameAttributes.Conversation_Icon, 0] = active ? 1 : 3;
				Attributes.BroadcastChangedIfRevealed();
			}

			bool HasReaded = false;
			foreach (var conv in Conversations)
				if (conv.Read == false)
					HasReaded = true;
			if (!HasReaded && Attributes[GameAttributes.Conversation_Icon, 0] != 2)
			{
				Attributes[GameAttributes.Conversation_Icon, 0] = 1;

				//if (entry.Type == ConversationTypes.GlobalFloat)
				
			}
			
			Attributes.BroadcastChangedIfRevealed();
		}

		private bool _ambientPlayed = false;

		public override void OnPlayerApproaching(Player player)
		{
			if (this is Hireling && (this as Hireling).IsHireling) return;
			base.OnPlayerApproaching(player);

			if (!IsRevealedToPlayer(player)) return;
			
			try
			{
				if (player.Position.DistanceSquared(ref _position) < 121f && !_ambientPlayed && Attributes[GameAttributes.Conversation_Icon, 0] != 2)
				{
					_ambientPlayed = true;
					if (FastRandom.Instance.Next(100) < 50)
					{
						if (ConversationList != null)
						{
							var suitableEntries = ConversationList.AmbientConversationListEntries.Where(entry => entry.SpecialEventFlag == (int)World.Game.CurrentAct).ToList();
							if (suitableEntries.Count > 0)
							{
								var random_conv = suitableEntries.PickRandom();
								player.Conversations.StartConversation(random_conv.SNOConversation);
								if (ForceConversationSNO == Conversations[0].ConversationSNO) ForceConversationSNO = -1;
							}
						}
					}
				}
			}
			//*/
			catch { }
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (ConversationList != null)
				Logger.Trace(" (OnTargeted) the npc has dynID {0} and Actor snoId {1}, ConversationList - {2} ", DynamicID(player), SNO, Tags[MarkerKeys.ConversationList].Id);
			else
				Logger.Trace(" (OnTargeted) the npc has dynID {0} and Actor snoId {1}, ", DynamicID(player), SNO);

			player.SelectedNPC = this;
			if (!OverridedConv)
				UpdateConversationList();

			if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.QuestProgress.QuestTriggers[(int)SNO];
				if (trigger.TriggerType == QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
					if (trigger.Count == World.Game.QuestProgress.QuestTriggers[(int)SNO].Counter)
						try
						{
							trigger.QuestEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
				}
			}

			var count = Interactions.Count + Conversations.Count;
			if (count == 0)
				return;

			// If there is only one conversation option, immediatly select it without showing menu
			if (Interactions.Count == 0 && Conversations.Count == 1)
			{
				player.Conversations.StartConversation(Conversations[0].ConversationSNO);
				if (ForceConversationSNO == Conversations[0].ConversationSNO) ForceConversationSNO = -1;
				Conversations[0].MarkAsRead();
				UpdateConversationList();
				return;
			}
			if (Interactions.Count == 0 && ImportantConversationSNO > 0)
			{
				player.Conversations.StartConversation(ImportantConversationSNO);
				if (ForceConversationSNO == Conversations[0].ConversationSNO) ForceConversationSNO = -1;
				ImportantConversationSNO = -1;
				UpdateConversationList();
				return;
			}


			NPCInteraction[] npcInters = new NPCInteraction[count];

			//bool HaveWithTitles = false;
			var it = 0;
			foreach (var conv in Conversations)
			{
				//if (entry.Type == ConversationTypes.GlobalFloat)
				//var Type = ConversationList.ConversationListEntries.Where(x => x.SNOConversation == conv.ConversationSNO);
				var Data = (DiIiS_NA.Core.MPQ.FileFormats.Conversation)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Conversation][conv.ConversationSNO].Data;

				if (Data.ConversationType != ConversationTypes.GlobalFloat)
				{
					npcInters[it] = conv.AsNPCInteraction(this, player);
					it++;
					//HaveWithTitles = true;
				}
			}
			

			foreach (var inter in Interactions)
			{
				npcInters[it] = inter.AsNPCInteraction(this, player);
				it++;
			}


			player.InGameClient.SendMessage(new NPCInteractOptionsMessage()
			{
				ActorID = DynamicID(player),
				tNPCInteraction = npcInters,
				Type = NPCInteractOptionsType.Normal
			});

			// TODO: this has no effect, why is it sent?
			player.InGameClient.SendMessage(new PlayEffectMessage()
			{
				ActorId = DynamicID(player),
				Effect = Effect.Unknown36
			});

			if (OverridedConv)
				UpdateConversationList();
		}
		public void Consume(GameClient client, GameMessage message)
		{
			if (message is NPCSelectConversationMessage) OnSelectConversation(client.Player, message as NPCSelectConversationMessage);
			if (message is HirelingHireMessage) OnHire(client.Player);
			if (message is HirelingInventoryMessage) OnInventory(client.Player, message as HirelingInventoryMessage);
			if (message is CraftInteractionMessage) OnCraft(client.Player);
			else return;
		}
		public virtual void OnCraft(Player player)
		{
			throw new NotImplementedException();
		}
		public virtual void OnInventory(Player player)
		{
			throw new NotImplementedException();
		}
		public virtual void OnInventory(Player player, HirelingInventoryMessage message)
		{
			throw new NotImplementedException();
		}
		public virtual void OnHire(Player player)
		{
			throw new NotImplementedException();
		}
		private void OnSelectConversation(Player player, NPCSelectConversationMessage message)
		{
			var conversation = Conversations.FirstOrDefault(conv => conv.ConversationSNO == message.ConversationSNO);
			if (conversation == null)
				return;

			player.Conversations.StartConversation(conversation.ConversationSNO);
			if (ForceConversationSNO == Conversations[0].ConversationSNO) ForceConversationSNO = -1;
			conversation.MarkAsRead();

			UpdateConversationList(); // erekose now the dialogs shit are updated properly :D yay !

		}
	}
}
