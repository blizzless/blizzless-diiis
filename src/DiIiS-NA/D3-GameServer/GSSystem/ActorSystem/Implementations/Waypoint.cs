using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Drawing;
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public sealed class Waypoint : Gizmo
	{
		public int WaypointId { get; private set; }

		private bool _activated = false;

		public int SNOLevelArea = -1;

		public Waypoint(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.MinimapIconOverride] = 129569;
			Attributes[GameAttributes.MinimapActive] = true;
		}

		public override void OnEnter(World world)
		{
			ReadWaypointId();
		}

		private void ReadWaypointId()
		{
			bool isOpenWorld = World.Game.CurrentAct == ActEnum.OpenWorld;
			var actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][World.Game.CurrentActSnoId].Data).WayPointInfo.ToList();
			if (isOpenWorld)
				actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70015].Data).WayPointInfo
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70016].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70017].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70018].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][236915].Data).WayPointInfo)
							.Where(w => w.SNOWorld != -1).ToList();
			var wayPointInfo = actData.Where(w => w.Flags == 3 || (isOpenWorld ? (w.Flags == 2) : (w.Flags == 1))).ToList();

			var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
			var scenes = World.QuadTree.Query<Scene>(proximity);
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
					if (wayPointInfo[i].SNOWorld != (int)World.SNO || wayPointInfo[i].SNOLevelArea != area)
						continue;

					SNOLevelArea = wayPointInfo[i].SNOLevelArea;
					WaypointId = i;
					break;
				}
			}
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			var world = player.World;

			world.BroadcastIfRevealed(plr => new PlayAnimationMessage()
			{
				ActorID = DynamicID(plr),
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
				ActorID = DynamicID(player)
			});

			//this.Attributes[Net.GS.Message.GameAttribute.Gizmo_Has_Been_Operated] = true;

			//handling quest triggers (special for Waypoints)
			if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.QuestProgress.QuestTriggers[(int)SNO];
				if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
					if (trigger.Count == World.Game.QuestProgress.QuestTriggers[(int)SNO * (-1)].Counter)
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

			if (World.Game.CurrentAct == ActEnum.OpenWorld && !player.InGameClient.OpenWorldDefined)
			{
				player.InGameClient.OpenWorldDefined = true;
				player.InGameClient.SendMessage(new ActTransitionMessage
				{
					Act = (int)ActEnum.OpenWorld,
					OnJoin = false
				});

				player.InGameClient.SendMessage(new GameSyncedDataMessage
				{
					SyncedData = new GameSyncedData
					{
						GameSyncedFlags = World.Game.IsSeasoned ? World.Game.IsHardcore ? 3 : 2 : World.Game.IsHardcore ? 1 : 0,
						Act = (int)ActEnum.OpenWorld,       //act id
						InitialMonsterLevel = player.InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
						MonsterLevel = 0x0000000, //MonsterLevel
						RandomWeatherSeed = player.InGameClient.Game.WeatherSeed, //RandomWeatherSeed
						OpenWorldMode = 1, //OpenWorldMode
						OpenWorldModeAct = (int)ActEnum.OpenWorld, //OpenWorldModeAct
						OpenWorldModeParam = 0, //OpenWorldModeParam
						OpenWorldTransitionTime = 0, //OpenWorldTransitionTime
						OpenWorldDefaultAct = 1, //OpenWorldDefaultAct
						OpenWorldBonusAct = 0, //OpenWorldBonusAct
						SNODungeonFinderLevelArea = 0, //SNODungeonFinderLevelArea
						LootRunOpen = -1, //LootRunOpen //0 - Great Portal
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

				foreach (var bounty in World.Game.QuestManager.Bounties)
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
					ActorID = DynamicID(player)
				});
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName($"{Name}-{GlobalID}"),
				Place = new WorldPlace { Position = Position, WorldID = World.GlobalID },
				ImageInfo = 129569,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = (int)SNO,
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
			if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && !_activated)
			{
				_activated = true;

				if (World.Game.OpenedWaypoints.Contains(WaypointId) || World.Game.CurrentAct == ActEnum.OpenWorld) return;

				Logger.MethodTrace($"Waypoint has been activated: {WaypointId}, levelArea: {SNOLevelArea}");

				World.BroadcastIfRevealed(plr => new WaypointActivatedMessage
				{
					WaypointDyID = DynamicID(plr),
					PlayerDyID = player.DynamicID(plr),
					SNOLevelArea = SNOLevelArea,
					Announce = true
				}, this);

				World.Game.UnlockTeleport(WaypointId);

				foreach (var game_player in World.Game.Players)
					game_player.Value.UpdateHeroState();

			}
		}
	}
}
