//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.NPC;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;

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
		public InteractiveNPC(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 0] = true; //second param - playerIndex
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 1] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 2] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 3] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 4] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 5] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 6] = true;
			this.Attributes[GameAttribute.NPC_Has_Interact_Options, 7] = true;
			this.Attributes[GameAttribute.NPC_Is_Operatable] = true;
			this.Attributes[GameAttribute.MinimapActive] = true;
			Interactions = new List<IInteraction>();
			Conversations = new List<ConversationInteraction>();

			
			World.Game.QuestManager.OnQuestProgress += new QuestManager.QuestProgressDelegate(quest_OnQuestProgress);
			UpdateConversationList(); // show conversations with no quest dependency
		}

		protected override void quest_OnQuestProgress() // shadows Actors'Mooege.Core.GS.Actors.InteractiveNPC.quest_OnQuestProgress(Mooege.Core.GS.Games.Quest)'
		{
			if (this is Hireling && (this as Hireling).IsHireling) return;
			// call base classe update range stuff			
			try
			{
				UpdateQuestRangeVisbility();
				UpdateConversationList();
			}
			catch { }
			// Logger.Debug(" (quesy_OnQuestProgress) has been called -> updatin conversaton list ");
		}

		

		public override bool Reveal(Player player)
		{
			if (this.ActorSNO.Id == 81609) return false;
			if (this.ActorSNO.Id == 114622 && this.World.WorldSNO.Id == 71150 && this.World.Game.CurrentAct != 3000) return false;
			return base.Reveal(player);
		}

		private void UpdateConversationList()
		{
			this.ImportantConversationSNO = -1;
			this.OverridedConv = false;
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
					if (entry.SpecialEventFlag != this.World.Game.CurrentAct && entry.SpecialEventFlag != -1) continue;

					if (entry.SNOQuestActive == -1)
						ConversationsNew.Add(entry.SNOConversation);

					if (entry.Type != ConversationTypes.GlobalFloat)
						withames = true;

					if (World.Game.QuestManager.HasCurrentQuest(entry.SNOQuestActive, entry.SNOQuestRange, (entry.ConditionReqs == 1)) || World.Game.QuestProgress.QuestTriggers.ContainsKey(entry.SNOConversation))
					{
						ConversationsNew.Add(entry.SNOConversation);
						if (entry.ConditionReqs == 1)
							this.ImportantConversationSNO = entry.SNOConversation;
					}
					else if (World.Game.CurrentSideStep == -1 & World.Game.CurrentSideQuest == -1)
						if (entry.SNOQuestRange == -3)
						{
							ConversationsNew.Add(entry.SNOConversation);
							this.ImportantConversationSNO = entry.SNOConversation;
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
					if (conversation.ConversationSNO == this.ImportantConversationSNO)
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
				
				this.Attributes[GameAttribute.Conversation_Icon, 0] = questConversation ? 2 : (sideConversation ? 4 : 0);
				
				if (ForceConversationSNO != -1)
				{
					questConversation = true;
					this.Attributes[GameAttribute.Conversation_Icon, 0] = questConversation ? 2 : (sideConversation ? 4 : 0);
					Conversations.Add(new ConversationInteraction(ForceConversationSNO));
				}

				Attributes.BroadcastChangedIfRevealed();
			}
			if (this.ActorSNO.Id == 114622 && this.Tags.ContainsKey(MarkerKeys.QuestRange) && this.Tags[MarkerKeys.QuestRange].Id == 312431) //TyraelBountyTurnin
			{
				bool active =
					this.World.Game.CurrentSideQuest == 356988 ||
					this.World.Game.CurrentSideQuest == 356994 ||
					this.World.Game.CurrentSideQuest == 356996 ||
					this.World.Game.CurrentSideQuest == 356999 ||
					this.World.Game.CurrentSideQuest == 357001;
				this.Attributes[GameAttribute.Conversation_Icon, 0] = active ? 1 : 3;
				Attributes.BroadcastChangedIfRevealed();
			}

			bool HasReaded = false;
			foreach (var conv in this.Conversations)
				if (conv.Read == false)
					HasReaded = true;
			if (!HasReaded && this.Attributes[GameAttribute.Conversation_Icon, 0] != 2)
			{
				this.Attributes[GameAttribute.Conversation_Icon, 0] = 1;

				//if (entry.Type == ConversationTypes.GlobalFloat)
				
			}
			
			Attributes.BroadcastChangedIfRevealed();
		}

		private bool _ambientPlayed = false;

		public override void OnPlayerApproaching(Player player)
		{
			if (this is Hireling && (this as Hireling).IsHireling) return;
			base.OnPlayerApproaching(player);

			if (!this.IsRevealedToPlayer(player)) return;
			
			try
			{
				if (player.Position.DistanceSquared(ref _position) < 121f && !_ambientPlayed && this.Attributes[GameAttribute.Conversation_Icon, 0] != 2)
				{
					_ambientPlayed = true;
					if (FastRandom.Instance.Next(100) < 50)
					{
						if (ConversationList != null)
						{
							var suitable_entries = ConversationList.AmbientConversationListEntries.Where(entry => entry.SpecialEventFlag == this.World.Game.CurrentAct).ToList();
							if (suitable_entries.Count() > 0)
							{
								var random_conv = suitable_entries[FastRandom.Instance.Next(suitable_entries.Count())];
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
				Logger.Trace(" (OnTargeted) the npc has dynID {0} and Actor snoId {1}, ConversationList - {2} ", DynamicID(player), ActorSNO.Id, Tags[MarkerKeys.ConversationList].Id);
			else
				Logger.Trace(" (OnTargeted) the npc has dynID {0} and Actor snoId {1}, ", DynamicID(player), ActorSNO.Id);

			player.SelectedNPC = this;
			if (!OverridedConv)
				UpdateConversationList();

			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.QuestProgress.UpdateCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id].counter)
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
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
			if (Interactions.Count == 0 && this.ImportantConversationSNO > 0)
			{
				player.Conversations.StartConversation(this.ImportantConversationSNO);
				if (ForceConversationSNO == Conversations[0].ConversationSNO) ForceConversationSNO = -1;
				this.ImportantConversationSNO = -1;
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
				ActorID = this.DynamicID(player),
				tNPCInteraction = npcInters,
				Type = NPCInteractOptionsType.Normal
			});

			// TODO: this has no effect, why is it sent?
			player.InGameClient.SendMessage(new PlayEffectMessage()
			{
				ActorId = this.DynamicID(player),
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
