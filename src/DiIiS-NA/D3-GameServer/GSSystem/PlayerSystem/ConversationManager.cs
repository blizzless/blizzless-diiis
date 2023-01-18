//Blizzless Project 2022
using System;
//Blizzless Project 2022
using System.Collections.Generic;
//Blizzless Project 2022
using System.Linq;
//Blizzless Project 2022
using System.Threading.Tasks;
//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.Core;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Conversation;
//Blizzless Project 2022
using System.Collections.Concurrent;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem
{
	/// <summary>
	/// Wraps a conversation asset and manages the whole conversation
	/// </summary>
	public class Conversation
	{
		Logger Logger = new Logger("Conversation");
		public event EventHandler ConversationEnded;

		public int ConvPiggyBack { get { return asset.snoConvPiggyback; } }
		public int SNOId = -1;
		public ConversationTypes ConversationType { get { return asset.ConversationType; } }

		private DiIiS_NA.Core.MPQ.FileFormats.Conversation asset
		{
			get
			{
				return (DiIiS_NA.Core.MPQ.FileFormats.Conversation)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Conversation][this.SNOId].Data;
			}
		}

		private int LineIndex = 0;            // index within the RootTreeNodes, conversation progress
		private Player player;
		private ConversationManager manager;
		private int currentUniqueLineID;        // id used to identify the current line clientside
		private int startTick = 0;            // start tick of the current line. used to determine, when to start the next line
		private ConversationTreeNode currentLineNode = null;
		private int endTick = 0;

		// Find a childnode with a matching class id, that one holds information about how long the speaker talks
		// If there is no matching childnode, there must be one with -1 which only combines all class specific into one
		private int duration
		{
			get
			{
				var node = from a in currentLineNode.ChildNodes where a.ClassFilter == player.Toon.VoiceClassID select a;
				if (node.Count() == 0)
					node = from a in currentLineNode.ChildNodes where a.ClassFilter == -1 select a;
				if (node.Count() == 0) return 1;

				return node.First().CompressedDisplayTimes.ElementAt((int)manager.ClientLanguage).Languages[player.Toon.VoiceClassID * 2 + (player.Toon.Gender == 0 ? 0 : 1)];
			}
		}

		// This returns the dynamicID of other conversation partners. The client uses its position to identify where you can hear the conversation.
		// This implementation relies on there beeing exactly one actor with a given sno in the world!!
		// TODO add actor identification for Followers
		private ActorSystem.Actor GetSpeaker(Speaker speaker)
		{
			switch (speaker)
			{
				case Speaker.AltNPC1:
					return GetActorBySNO((ActorSno)asset.SNOAltNpc1);
				case Speaker.AltNPC2:
					return GetActorBySNO((ActorSno)asset.SNOAltNpc2);
				case Speaker.AltNPC3:
					return GetActorBySNO((ActorSno)asset.SNOAltNpc3);
				case Speaker.AltNPC4:
					return GetActorBySNO((ActorSno)asset.SNOAltNpc4);
				case Speaker.Player:
					return player;
				case Speaker.PrimaryNPC:
					return GetActorBySNO((ActorSno)asset.SNOPrimaryNpc);
				case Speaker.EnchantressFollower:
				case Speaker.ScoundrelFollower:
				case Speaker.TemplarFollower:
				case Speaker.None:
					return null;
			}
			return null;
		}

		private ActorSystem.Actor GetActorBySNO(ActorSno sno)
		{
            ActorSystem.Actor SearchFunc(ActorSno a) => player.World.Actors.Values.Where(actor => actor.SNO == a && actor.IsRevealedToPlayer(player)).OrderBy((actor) => PowerMath.Distance2D(actor.Position, player.Position)).FirstOrDefault();
            //if (sno == 121208)
            //	sno = 4580; //hack
            var result = SearchFunc(sno);

            if (result != null)
            {
                //result.Reveal(player);
                return result;
            }

            if (sno == ActorSno._templarnpc)
            {
                return SearchFunc(ActorSno._templarnpc_imprisoned);
            }
            else
            {
                result = SearchFunc(sno);
                if (result == null)
                    //return player;
                    return player.World.SpawnMonster(sno, new Vector3D(player.Position.X, player.Position.Y, player.Position.Z + 150));
                else
                {
                    result.Reveal(player);
                    return result;
                }
            }
        }

		/// <summary>
		/// Creates a new conversation wrapper for an asset with a given sno.
		/// </summary>
		/// <param name="snoConversation">sno of the asset to wrap</param>
		/// <param name="player">player that receives messages</param>
		/// <param name="manager">the quest manager that provides ids</param>
		public Conversation(int snoConversation, Player player, ConversationManager manager)
		{
			this.SNOId = snoConversation;
			this.player = player;
			this.manager = manager;
		}

		/// <summary>
		/// Starts the conversation
		/// </summary>
		public void Start()
		{
			try
			{
				PlayLine(LineIndex);
			}
			catch
			{
				Logger.Warn("Conversation start error!");
			}
			//if (this.SNOId == 181330) fullHeal(); //TODO this
		}

		/// <summary>
		///  Immediatly ends the conversation
		/// </summary>
		public void Stop()
		{
			StopLine(true);
			EndConversation();
		}

		/// <summary>
		/// Sets a new end tick for line playback
		/// </summary>
		/// <param name="endTick"></param>
		public void UpdateAdvance(int endTick)
		{
			this.endTick = endTick;
		}

		/// <summary>
		/// Skips to the next line of the conversation
		/// </summary>
		public void Interrupt()
		{
			PlayNextLine(true);
		}

		/// <summary>
		/// Periodically call this method to make sure conversation progresses
		/// </summary>
		public void Update(int tickCounter)
		{
			if (endTick > 0 && currentLineNode == null)
				PlayNextLine(false);
			else
			{
				try
				{

					// rotate the primary speaker to face the secondary speaker
					if (currentLineNode != null)
						if (currentLineNode.LineSpeaker != Speaker.Player && currentLineNode.SpeakerTarget != Speaker.None)
						{
							var speaker1 = GetSpeaker(currentLineNode.LineSpeaker);
							var speaker2 = GetSpeaker(currentLineNode.SpeakerTarget);

							if (!(speaker1 is PlayerSystem.Player) && speaker2.Position != speaker1.Position) //prevent spinning bug
							{
								Vector3D translation = speaker2.Position - speaker1.Position;
								Vector2F flatTranslation = new Vector2F(translation.X, translation.Y);

								float facingAngle = flatTranslation.Rotation();
								speaker1.SetFacingRotation(facingAngle);

								player.World.BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
								{
									ActorId = speaker1.DynamicID(plr),
									Angle = facingAngle,
									TurnImmediately = false
								}, speaker1);
							}
						}

					// start the next line if the playback has finished
					if (tickCounter > endTick)
						PlayNextLine(false);
				 }
				catch
				{
					Logger.Trace("Conv error");
					EndConversation();
				}
			}
		}

		/// <summary>
		/// Stops current line and starts the next if there is one, or ends the conversation
		/// </summary>
		/// <param name="interrupt">sets, whether the speaker is interrupted</param>
		public void PlayNextLine(bool interrupt)
		{
			StopLine(interrupt);

			if (asset.RootTreeNodes.Count > LineIndex + 1)
				PlayLine(++LineIndex);
			else
				EndConversation();
		}

		/// <summary>
		/// Ends the conversation, though i dont know, what it actually does. This is only through observation
		/// </summary>
		private void EndConversation()
		{
			player.InGameClient.SendMessage(new EndConversationMessage()
			{
				SNOConversation = asset.Header.SNOId,
				ActorId = player.DynamicID(player),
				Field2 = -1
			});

			if (asset.ConversationType != ConversationTypes.AmbientFloat && asset.ConversationType != ConversationTypes.GlobalFloat)
				player.CheckConversationCriteria(asset.Header.SNOId);

			Logger.Trace("Handling conversation for Conversation: {0}", this.SNOId);
			if (this.player.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.SNOId))
			{
				var trigger = this.player.World.Game.QuestProgress.QuestTriggers[this.SNOId];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation)
				{
					try
					{
						trigger.questEvent.Execute(this.player.World); // launch a questEvent
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}
			if (this.player.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(this.SNOId)) //EnterLevelArea
			{
				var trigger = this.player.World.Game.SideQuestProgress.QuestTriggers[this.SNOId];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation)
				{
					try
					{
						trigger.questEvent.Execute(this.player.World); // launch a questEvent
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}
			if (this.player.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(this.SNOId))
			{
				var trigger = this.player.World.Game.SideQuestProgress.GlobalQuestTriggers[this.SNOId];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.HadConversation)
				{
					try
					{
						trigger.questEvent.Execute(this.player.World); // launch a questEvent
						this.player.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(this.SNOId);
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}

			if (ConversationEnded != null)
				ConversationEnded(this, null);

			if (this.SNOId == 72817) //Tristram parom man
			{
				var world = this.player.World.Game.GetWorld(WorldSno.trout_townattack);
				this.player.ChangeWorld(world, world.GetStartingPointById(116).Position);
			}

			else if (this.SNOId == 208400) //Cow king
			{
				var portal = this.player.World.Game.GetWorld(WorldSno.trout_town).GetActorBySNO(ActorSno._g_portal_tentacle_trist);
				(portal as WhimsyshirePortal).Open();
			}

			else if (this.SNOId == 275450) //PvP hub gatekeeper
			{
				this.player.ShowConfirmation(this.player.DynamicID(this.player), (() =>
				{
					var world = this.player.World.Game.GetWorld(WorldSno.pvp_duel_small_multi);
					this.player.ChangeWorld(world, world.GetStartingPointById(288).Position);
				}));
			}
			else if (this.SNOId == 340878)
			{
				foreach (var plr in this.player.InGameClient.Game.Players.Values)
				{
					if (this.player.InGameClient.Game.NephalemGreater)
					{
						plr.Attributes[GameAttribute.Jewel_Upgrades_Max] = 0;
						plr.Attributes[GameAttribute.Jewel_Upgrades_Bonus] = 0;
						plr.Attributes[GameAttribute.Jewel_Upgrades_Used] = 0;

						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 46,
							TaskIndex = 0,
							Checked = 1,
							Counter = 1
						});
						plr.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 5,
							DisplayButton = true,
							Failed = false
						});

						plr.InGameClient.SendMessage(new DungeonFinderClosingMessage()
						{
							Field0 = 26396,
							Field1 = 0
						});
						//RiftEndScreenInfoBlobMessage - 524


						plr.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.RiftEndScreenInfoBlobMessage)
						{
							Data = D3.GameMessage.RiftEndScreenInfo.CreateBuilder()
								.SetNewPersonalBest(true)
								.SetSuccess(true)
								.SetIsFromCheat(false)
								.SetRiftTier(plr.InGameClient.Game.NephalemGreaterLevel + 1)
								.AddParticipantGameAccounts(D3.OnlineService.GameAccountHandle.CreateBuilder().SetId((uint)plr.Toon.GameAccount.BnetEntityId.Low).SetProgram(17459).SetRegion(1))
								.SetGoldReward(5000 * plr.Level)
								.SetXpReward(50000 * (long)plr.Level)
								.SetCompletionTimeMs(900 * 1000 - plr.InGameClient.Game.LastTieredRiftTimeout * 1000)
								.SetBannerConfiguration(plr.Toon.GameAccount.BannerConfigurationField.Value)
								.Build().ToByteArray()

					});

						player.InGameClient.SendMessage(new PlatinumAwardedMessage
						{
							CurrentPlatinum = player.InGameClient.BnetClient.Account.GameAccount.Platinum,
							PlatinumIncrement = 5
						});

						player.InGameClient.BnetClient.Account.GameAccount.Platinum += 5;
						plr.UpdateExp(50000 * plr.Level);
						plr.Inventory.AddGoldAmount(5000 * plr.Level);

					}
					else
					{
						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 10,
							TaskIndex = 0,
							Checked = 1,
							Counter = 1
						});
						plr.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 5,
							DisplayButton = true,
							Failed = false
						});
						plr.InGameClient.SendMessage(new QuestStepCompleteMessage()
						{
							QuestStepComplete = D3.Quests.QuestStepComplete.CreateBuilder()
							.SetIsQuestComplete(true)
							.SetWasRewardAutoequipped(false)
							.SetReward(D3.Quests.QuestReward.CreateBuilder().SetPlatinumGranted(1).SetGoldGranted(1000 * plr.Level).SetBonusXpGranted(10000 * (ulong)plr.Level))
							.Build()
						});

						player.InGameClient.SendMessage(new PlatinumAwardedMessage
						{
							CurrentPlatinum = player.InGameClient.BnetClient.Account.GameAccount.Platinum,
							PlatinumIncrement = 1
						});
						player.InGameClient.BnetClient.Account.GameAccount.Platinum += 1;

						plr.Inventory.AddGoldAmount(1000 * plr.Level);

						player.GrantCriteria(74987243379080);
						if (player.InGameClient.Game.Difficulty >= 2)
						{
							player.GrantCriteria(74987250579270);
							if(player.InGameClient.Game.Difficulty >= 3)
								player.GrantCriteria(74987247265988);
						}
					}
					//Таймер до закрытия
					/*
					plr.InGameClient.SendMessage(new DungeonFinderClosingMessage()
					{
						Field0 = 51605,
						Field1 = -1
					});
					//*/
					//Обнуляем прогресс
					plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
					{
						Field0 = 0
					});

					plr.InGameClient.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
					{
						Field0 = 0
					});
				}


				this.player.InGameClient.Game.ActiveNephalemKilledBoss = false;
				this.player.InGameClient.Game.ActiveNephalemKilledMobs = false;
				this.player.InGameClient.Game.ActiveNephalemPortal = false;
				this.player.InGameClient.Game.ActiveNephalemTimer = false;
				this.player.InGameClient.Game.ActiveNephalemProgress = 0;
				//Enabled banner /advocaite
				this.player.Attributes[GameAttributeB.Banner_Usable] = true;
				var HubWorld = this.player.InGameClient.Game.GetWorld(WorldSno.x1_tristram_adventure_mode_hub);
				var NStone = HubWorld.GetActorBySNO(ActorSno._x1_openworld_lootrunobelisk_b);
				bool Activated = true;
				NStone.SetIdleAnimation(NStone.AnimationSet.TagMapAnimDefault[Core.Types.TagMap.AnimationSetKeys.IdleDefault]);
				NStone.PlayActionAnimation(NStone.AnimationSet.TagMapAnimDefault[Core.Types.TagMap.AnimationSetKeys.Closing]);
				NStone.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
				NStone.Attributes[GameAttribute.Untargetable] = !Activated;
				NStone.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
				NStone.Attributes[GameAttribute.Operatable] = Activated;
				NStone.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
				NStone.Attributes[GameAttribute.Disabled] = !Activated;
				NStone.Attributes[GameAttribute.Immunity] = !Activated;
				NStone.Attributes.BroadcastChangedIfRevealed();

				foreach (var p in HubWorld.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal, ActorSno._x1_openworld_tiered_rifts_portal))
					p.Destroy();
			}

		}

		/// <summary>
		/// Stops readout and display of current conversation line
		/// </summary>
		/// <param name="interrupted">sets whether the speaker is interrupted or not</param>
		private void StopLine(bool interrupted)
		{
			player.InGameClient.SendMessage(new StopConvLineMessage()
			{
				PlayLineParamsId = currentUniqueLineID,
				Interrupt = interrupted,
			});
		}

		/// <summary>
		/// Starts readout and display of a certain conversation line
		/// </summary>
		/// <param name="LineIndex">index of the line withing the rootnodes</param>
		private void PlayLine(int LineIndex)
		{
			if (asset.RootTreeNodes[LineIndex].ConvNodeType == 6)
			{
				currentLineNode = null;
				return;
			}

			if (asset.RootTreeNodes[LineIndex].ConvNodeType == 4)
				currentLineNode = asset.RootTreeNodes[LineIndex].ChildNodes[RandomHelper.Next(asset.RootTreeNodes[LineIndex].ChildNodes.Count)];
			else
				currentLineNode = asset.RootTreeNodes[LineIndex];

			currentUniqueLineID = manager.GetNextUniqueLineID();

			if (!GetSpeaker(currentLineNode.LineSpeaker).IsRevealedToPlayer(player))
				GetSpeaker(currentLineNode.LineSpeaker).Reveal(player);

			startTick = player.World.Game.TickCounter;
			endTick = startTick + duration;

			// TODO Actor id should be CurrentSpeaker.DynamicID not PrimaryNPC.ActorID. This is a workaround because no audio for the player is playing otherwise
			player.InGameClient.SendMessage(new PlayConvLineMessage()
			{
				ActorID = GetSpeaker(currentLineNode.LineSpeaker).DynamicID(player), // GetActorBySNO(asset.SNOPrimaryNpc).DynamicID,
				Field1 = new uint[9]
						{
							player.DynamicID(player), asset.SNOPrimaryNpc != -1 ? GetActorBySNO((ActorSno)asset.SNOPrimaryNpc).DynamicID(player) : 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF
						},

				PlayLineParams = new PlayLineParams()
				{
					SNOConversation = asset.Header.SNOId,
					SpeakingPlayerIndex = 0x00000000,
					Zoom = false,
					FirstLine = true,
					Hostiile = false,
					PlayerInitiated = true,
					LineID = currentLineNode.LineID,
					Speaker = currentLineNode.LineSpeaker,
					LineGender = -1,
					AudioClass = (GameBalance.Class)player.Toon.VoiceClassID,
					Gender = (player.Toon.Gender == 0) ? VoiceGender.Male : VoiceGender.Female,
					TextClass = currentLineNode.LineSpeaker == Speaker.Player ? (GameBalance.Class)player.Toon.VoiceClassID : GameBalance.Class.None,
					SNOSpeakerActor = (int)GetSpeaker(currentLineNode.LineSpeaker).SNO,
					LineFlags = 0x00000000,
					AnimationTag = currentLineNode.AnimationTag,
					Duration = duration,
					Id = currentUniqueLineID,
					Priority = 0x00000000
				},
				Duration = duration,
			});//, true);
		}
	}

	public class ConversationManager
	{
		Logger Logger = new Logger("ConversationManager");
		internal enum Language { Invalid, Global, enUS, enGB, enSG, esES, esMX, frFR, itIT, deDE, koKR, ptBR, ruRU, zhCN, zTW, trTR, plPL, ptPT }

		private Player player;
		private ConcurrentDictionary<int, Conversation> openConversations = new ConcurrentDictionary<int, Conversation>();
		private int linesPlayedTotal = 0;

		internal Language ClientLanguage { get { return Language.enUS; } }

		internal int GetNextUniqueLineID()
		{
			return linesPlayedTotal++;
		}

		public ConversationManager(Player player)
		{
			this.player = player;
		}

		/// <summary>
		/// Stops all conversations
		/// </summary>
		public void StopAll()
		{
			foreach (var pair in this.openConversations)
				pair.Value.Stop();
		}

		/// <summary>
		/// Starts and plays a conversation
		/// </summary>
		/// <param name="snoConversation">SnoID of the conversation</param>
		public void StartConversation(int snoConversation)
		{
			if (!DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Conversation].ContainsKey(snoConversation))
			{
				Logger.Warn("Conversation not found: {0}", snoConversation);
				return;
			}
			if (snoConversation != 131349)
				if (!openConversations.ContainsKey(snoConversation))
				{
					#if DEBUG
						Logger.Warn("Conversation started: {0}", snoConversation);
					#endif
					Conversation newConversation = new Conversation(snoConversation, player, this);
					newConversation.Start();
					newConversation.ConversationEnded += new EventHandler(ConversationEnded);

					openConversations.TryAdd(snoConversation, newConversation);
                    #region События по началу диалогов
                    switch (snoConversation)
					{
						case 198199:
							//Task.Delay(1000).Wait();

							break;
					}
                    #endregion
                }
        }

		/// <summary>
		/// Remove conversation from the list of open conversations and start its piggyback conversation
		/// </summary>
		void ConversationEnded(object sender, EventArgs e)
		{
			Conversation conversation = sender as Conversation;
			Logger.Trace(" (ConversationEnded) Sending a notify with type {0} and value {1}", conversation.ConversationType, conversation.SNOId);

			//quests.Notify(QuestStepObjectiveType.HadConversation, conversation.SNOId); //deprecated

			//����������
			if (player.PlayerIndex == 0)
				switch (conversation.SNOId)
				{
					#region Events after Dialogs
						#region A1-Q2
					case 17667:
						//var BlacksmithQuest = player.InGameClient.Game.GetWorld(71150).GetActorBySNO(65036,true);
						var world = player.InGameClient.Game.GetWorld(WorldSno.trdun_cain_intro);
						var CainBrains = world.GetActorBySNO(ActorSno._cain_intro, true);
						Vector3D CainPath = new Vector3D(76.99389f, 155.145f, 0.0997252f);
						var facingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(CainBrains, CainPath);
						CainBrains.Move(CainPath, facingAngle);
						var A1Q2Wait1 = Task.Delay(7000).ContinueWith(delegate
						{
							var actor = world.GetActorsBySNO(ActorSno._trdun_cath_bookcaseshelf_door_reverse).Where(d => d.Visible).FirstOrDefault();
							(actor as Door).Open();

							var A1Q2Wait2 = Task.Delay(2000).ContinueWith(delegate
							{
								CainBrains.Hidden = true;
								CainBrains.Unreveal(player);
							});
						});

						break;
						#endregion
						#region A1-Q3
						case 198292:
							var BlacksmithQuest = player.InGameClient.Game.GetWorld(WorldSno.trout_town).GetActorBySNO(ActorSno._pt_blacksmith_nonvendor, true);
							BlacksmithQuest.WalkSpeed = 0.33f;
							Vector3D FirstPoint = new Vector3D(2905.856f, 2584.807f, 0.5997877f);
							Vector3D SecondPoint = new Vector3D(2790.396f, 2611.313f, 0.5997864f);

							var FirstfacingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(BlacksmithQuest, FirstPoint);

							var SecondfacingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(BlacksmithQuest, SecondPoint);

							BlacksmithQuest.Move(FirstPoint, FirstfacingAngle);

							var ListenerKingSkeletons = Task.Delay(3000).ContinueWith(delegate
							{
								BlacksmithQuest.Move(SecondPoint, SecondfacingAngle);
							});
							break;

					#endregion
					//168282
					#region A1-Q4
					case 168282:
						var wrld = player.InGameClient.Game.GetWorld(WorldSno.a1trdun_level05_templar);
						foreach (var Wall in wrld.GetActorsBySNO(ActorSno._trdun_cath_bonewall_a_door))
							if (Wall.Position.Z > -23f)
							{
								Wall.PlayAnimation(11, 108568);
								Wall.Attributes[GameAttribute.Deleted_On_Server] = true;
								Wall.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
								Wall.Attributes.BroadcastChangedIfRevealed();
								Wall.Destroy();
							}
						break;
					case 17921:
						var cryptwrld = player.InGameClient.Game.GetWorld(WorldSno.a1trdun_level07);
						foreach (var ghost in cryptwrld.GetActorsBySNO(ActorSno._skeletonking_ghost))
							ghost.Destroy();
						break;
					#endregion
					#region A1-Q4 Event_DoK
					case 139823: //Event_DoK_Kill.cnv
									 //���� ���������� �����
							var LeoricGhost = player.World.GetActorBySNO(ActorSno._skeletonking_leoricghost);
							var LachdananGhost = player.World.GetActorBySNO(ActorSno._ghostknight3);
							var SwordPlace = player.World.GetActorBySNO(ActorSno._trdun_crypt_deathoftheking_sword_clickable);

							LachdananGhost.Move(SwordPlace.Position, ActorSystem.Movement.MovementHelpers.GetFacingAngle(LachdananGhost, SwordPlace.Position));

							var ListenerA1Q4Event1 = Task.Delay(4000).ContinueWith(delegate
							{
								StartConversation(139825);
							});
							break;
						case 139825: //Event_DoK_Death.cnv
							var LeoricGhost1 = player.World.GetActorBySNO(ActorSno._skeletonking_leoricghost);
							var GhostKnights1 = player.World.GetActorsBySNO(ActorSno._ghostknight2);
							var LachdananGhost1 = player.World.GetActorBySNO(ActorSno._ghostknight3);

							var ListenerA1Q4Event2 = Task.Delay(10000).ContinueWith(delegate
							{
								player.World.Leave(LeoricGhost1);
								player.World.Leave(LachdananGhost1);
								foreach (var GK in GhostKnights1)
								{
									player.World.Leave(GK);
								}
							});
							break;
                    #endregion
						#region A1-SQ-Farmer
                    case 60179:
							//player.InGameClient.Game.QuestManager.ClearQuests();
							//player.InGameClient.Game.QuestManager.SetQuests();
							player.InGameClient.Game.QuestManager.LaunchSideQuest(81925);
							var NearActors = player.CurrentScene.Actors;
							int Founded = 0;
							List<uint> monstersAlive = new List<uint> { };
							foreach (var Actor in NearActors)
								if (Actor.SNO == ActorSno._fleshpitflyerspawner_b_event_farmambush)
								{
									Founded++;
									monstersAlive.Add(Actor.GlobalID);
								}
							if (Founded == 4)
								Logger.Debug("������� ��� �������, ���������� �������� ������.");
							else
							{
								Logger.Debug("������� �� ��� �������, �������.");
								monstersAlive.Clear();
								var OldPit = player.World.GetActorsBySNO(ActorSno._fleshpitflyerspawner_b_event_farmambush);
								foreach (var actor in OldPit)
									player.World.Leave(actor);
								var SpawnerOfPits = player.World.GetActorsBySNO(ActorSno._spawner_fleshpitflyer_b_immediate);
								foreach (var actor in SpawnerOfPits)
									player.World.SpawnMonster(ActorSno._fleshpitflyerspawner_b_event_farmambush, actor.Position);

								var NewPits = player.World.GetActorsBySNO(ActorSno._fleshpitflyerspawner_b_event_farmambush);
								foreach (var Actor in NewPits)
									monstersAlive.Add(Actor.GlobalID);
								Logger.Debug("������� ��������. ������ �������� ������.");
							}
							break;
						#endregion
						#region A5
						case 308393:
                        var npc = player.World.GetActorBySNO(ActorSno._x1_npc_westmarch_introguy) as ActorSystem.InteractiveNPC;
                        npc.Conversations.Clear();
                        npc.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
                        npc.Attributes.BroadcastChangedIfRevealed();
						break;
						#endregion
					#endregion
				}

            openConversations.TryRemove(conversation.SNOId, out _);

			if (conversation.ConvPiggyBack != -1)
				StartConversation(conversation.ConvPiggyBack);

			_conversationTrigger = true;
		}

		/// <summary>
		/// Returns true when the conversation playing finishes.
		/// </summary>
		private bool _conversationTrigger = false;
		public bool ConversationRunning()
		{
			var status = _conversationTrigger;
			_conversationTrigger = false;
			return status;
		}
		/// <summary>
		/// Update all open conversations
		/// </summary>
		/// <param name="gameTick"></param>
		public void Update(int tickCounter)
		{
			foreach (var pair in this.openConversations)
				pair.Value.Update(tickCounter);
		}

		/// <summary>
		/// Consumes conversations related messages
		/// </summary>
		/// <param name="client"></param>
		/// <param name="message"></param>
		public void Consume(GameClient client, GameMessage message)
		{
			if (message is RequestCloseConversationWindowMessage)
			{
				foreach (var pair in this.openConversations)
					pair.Value.Interrupt();
			}

			if (message is UpdateConvAutoAdvanceMessage)
			{
				UpdateConvAutoAdvanceMessage tmpMessage = (UpdateConvAutoAdvanceMessage)message;
				if (openConversations.ContainsKey(tmpMessage.SNOConversation))
					openConversations[tmpMessage.SNOConversation].UpdateAdvance(tmpMessage.EndTick);
			}

			if (message is AdvanceConvMessage)
			{
				if (openConversations.ContainsKey(((AdvanceConvMessage)message).SNOConversation))
				{
					openConversations[((AdvanceConvMessage)message).SNOConversation].PlayNextLine(true);
				}
			}
		}
	}
}
