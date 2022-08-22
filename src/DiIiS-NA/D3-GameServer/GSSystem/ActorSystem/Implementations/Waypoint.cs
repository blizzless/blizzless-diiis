//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public sealed class Waypoint : Gizmo
	{
		public int WaypointId { get; private set; }

		private bool _activated = false;

		public int SNOLevelArea = -1;

		public Waypoint(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.Attributes[GameAttribute.MinimapIconOverride] = 129569;
			this.Attributes[GameAttribute.MinimapActive] = true;
		}

		public override void OnEnter(World world)
		{
			this.ReadWaypointId();
		}

		private void ReadWaypointId()
		{
			bool isOpenWorld = this.World.Game.CurrentAct == 3000;
			var actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][this.World.Game.CurrentActSNOid].Data).WayPointInfo.ToList();
			if (isOpenWorld)
				actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70015].Data).WayPointInfo
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70016].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70017].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70018].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][236915].Data).WayPointInfo)
							.Where(w => w.SNOWorld != -1).ToList();
			var wayPointInfo = actData.Where(w => w.Flags == 3 || (isOpenWorld ? (w.Flags == 2) : (w.Flags == 1))).ToList();

			var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
			var scenes = this.World.QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return; // TODO: fixme! /raist

			var scene = scenes[0]; // Parent scene /fasbat

			if (scenes.Count == 2) // What if it's a subscene? /fasbat
			{
				if (scenes[1].ParentChunkID != 0xFFFFFFFF)
					scene = scenes[1];
			}

			for (int i = 0; i < wayPointInfo.Count; i++)
			{
				if (wayPointInfo[i].SNOLevelArea == -1)
					continue;

				if (scene.Specification == null) continue;
				foreach (var area in scene.Specification.SNOLevelAreas)
				{
					if (wayPointInfo[i].SNOWorld != (int)this.World.SNO || wayPointInfo[i].SNOLevelArea != area)
						continue;

					this.SNOLevelArea = wayPointInfo[i].SNOLevelArea;
					this.WaypointId = i;
					break;
				}
			}
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			var world = player.World;

			world.BroadcastIfRevealed(plr => new PlayAnimationMessage()
			{
				ActorID = this.DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0f,
				tAnim = new[]
					{
						new PlayAnimationMessageSpec()
						{
							Duration = 4,
							AnimationSNO = 0x2f761,
							PermutationIndex = 0,
							AnimationTag = 0,
							Speed = 1f,
						}
					}
			}, this);

			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenWaypointSelectionWindowMessage)
			{
				ActorID = this.DynamicID(player)
			});

			//this.Attributes[Net.GS.Message.GameAttribute.Gizmo_Has_Been_Operated] = true;

			//handling quest triggers (special for Waypoints)
			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.QuestProgress.UpdateCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id * (-1)].counter)
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

			if (this.World.Game.CurrentAct == 3000 && !player.InGameClient.OpenWorldDefined)
			{
				player.InGameClient.OpenWorldDefined = true;
				player.InGameClient.SendMessage(new ActTransitionMessage
				{
					Act = 3000,
					OnJoin = false
				});

				player.InGameClient.SendMessage(new GameSyncedDataMessage
				{
					SyncedData = new GameSyncedData
					{
						GameSyncedFlags = this.World.Game.IsSeasoned == true ? this.World.Game.IsHardcore == true ? 3 : 2 : this.World.Game.IsHardcore == true ? 1 : 0,
						Act = 3000,       //act id
						InitialMonsterLevel = player.InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
						MonsterLevel = 0x0000000, //MonsterLevel
						RandomWeatherSeed = player.InGameClient.Game.WeatherSeed, //RandomWeatherSeed
						OpenWorldMode = 1, //OpenWorldMode
						OpenWorldModeAct = 3000, //OpenWorldModeAct
						OpenWorldModeParam = 0, //OpenWorldModeParam
						OpenWorldTransitionTime = 0, //OpenWorldTransitionTime
						OpenWorldDefaultAct = 1, //OpenWorldDefaultAct
						OpenWorldBonusAct = 0, //OpenWorldBonusAct
						SNODungeonFinderLevelArea = 0, //SNODungeonFinderLevelArea
						LootRunOpen = -1, //LootRunOpen //0 - Великий Портал
						OpenLootRunLevel = -1, //OpenLootRunLevel
						LootRunBossDead = 0, //LootRunBossDead
						HunterPlayerIdx = 0, //HunterPlayerIdx
						LootRunBossActive = -1, //LootRunBossActive
						TieredLootRunFailed = -1, //TieredLootRunFailed
						LootRunChallengeCompleted = -1, //LootRunChallengeCompleted
						SetDungeonActive = -1, //SetDungeonActive
						Pregame = 0, //Pregame
						PregameEnd = 0, //PregameEnd
						RoundStart = 0, //RoundStart
						RoundEnd = 0, //RoundEnd
						PVPGameOver = 0x0, //PVPGameOver
						field_v273 = 0x0,
						TeamWins = new[] { 0x0, 0x0 }, //TeamWins
						TeamScore = new[] { 0x0, 0x0 }, //TeamScore
						PVPGameResult = new[] { 0x0, 0x0 }, //PVPGameResult
						PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)player.InGameClient.Game.Players.Values.First().Toon.PersistentID }
						TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID

					}
				});

				foreach (var bounty in this.World.Game.QuestManager.Bounties)
				{
					player.InGameClient.SendMessage(new QuestUpdateMessage()
					{
						snoQuest = bounty.BountySNOid,
						snoLevelArea = bounty.LevelArea,
						StepID = 4,
						DisplayButton = true,
						Failed = false
					});

					
				}

				player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenWaypointSelectionWindowMessage)
				{
					ActorID = this.DynamicID(player)
				});
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName(string.Format("{0}-{1}", this.ActorSNO.Name, this.GlobalID)),
				Place = new WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
				ImageInfo = 129569,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = this.ActorSNO.Id,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = false,
				PlayerUseFlags = 0
			});
			return true;
		}

		public override void OnPlayerApproaching(Player player)
		{
			if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_activated)
			{
				_activated = true;

				if (this.World.Game.OpenedWaypoints.Contains(this.WaypointId) || this.World.Game.CurrentAct == 3000) return;

				Logger.Debug("(OnTargeted) Waypoint has been activated: {0}, levelArea: {1}", WaypointId, SNOLevelArea);

				this.World.BroadcastIfRevealed(plr => new WaypointActivatedMessage
				{
					WaypointDyID = this.DynamicID(plr),
					PlayerDyID = player.DynamicID(plr),
					SNOLevelArea = SNOLevelArea,
					Announce = true
				}, this);

				this.World.Game.UnlockTeleport(this.WaypointId);

				foreach (var game_player in this.World.Game.Players)
					game_player.Value.UpdateHeroState();

			}
		}
	}
}
