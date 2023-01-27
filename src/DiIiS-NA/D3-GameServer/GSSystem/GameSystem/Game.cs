//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using GameBalance = DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Threading;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Team;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Text;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using System.Diagnostics;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public class Game : IMessageConsumer
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The game id.
		/// </summary>
		public int GameId { get; private set; }

		/// <summary>
		/// Dictionary that maps gameclient's to players.
		/// </summary>
		public ConcurrentDictionary<GameClient, Player> Players { get; private set; }

		public List<Player> ConnectedPlayers = new List<Player>();

		public bool QuestSetup = false;

		public int LoadedPlayers = 0;

		public int CurrentPvPRound = 0;

		public TickTimer PvPTimer;
		public TickTimer GlobalPvPTimer;
		public TickTimer QuestTimer;

		public WorldSno WorldOfPortalNephalem = WorldSno.__NONE;
		public WorldSno WorldOfPortalNephalemSec = WorldSno.__NONE;
		public int NephalemGreaterLevel = -1;
		public bool NephalemGreater = false;
		public bool NephalemBuff = false;
		public bool ActiveNephalemPortal = false;
		public bool ActiveNephalemTimer = false;
		public float ActiveNephalemProgress = 0f;
		public bool ActiveNephalemKilledMobs = false;
		public bool ActiveNephalemKilledBoss = false;
		public SecondsTickTimer TiredRiftTimer;
		public int LastTieredRiftTimeout = 0;

		public TickTimer LockdownTimer;
		public Actor SideQuestGizmo = null;

		public int RedTeamWins = 0;
		public int BlueTeamWins = 0;
		/// <summary>
		/// DynamicId counter for objects.
		/// </summary>
		private uint _lastObjectID = 10001;

		/// <summary>
		/// Returns a new dynamicId for objects.
		/// </summary>
		private object obj = new object();
		public uint NewActorGameID
		{
			get
			{
				lock (obj)
				{
					_lastObjectID++;
					return _lastObjectID;
				}
			}
		}

		/// <summary>
		/// Dictionary that tracks world.
		/// NOTE: This tracks by WorldSNO rather than by DynamicID; this.Objects _does_ still contain the world since it is a DynamicObject
		/// </summary>
		private readonly ConcurrentDictionary<WorldSno, World> _worlds;

		public List<World> Worlds
		{
			get
			{
				return _worlds.Values.ToList();
			}
		}

		public Mode GameMode = Mode.Campaign;
		public enum Mode
		{
			Campaign = 0,
			Bounties = 1,
			Portals = 6 //6?
		}
		public struct BossEncounter
		{
			public int SnoId;
			public bool activated;
			public int acceptedPlayers;
		};

		public Dictionary<WorldSno, List<Action>> OnLoadWorldActions = new Dictionary<WorldSno, List<Action>>();
		public Dictionary<int, List<Action>> OnLoadSceneActions = new Dictionary<int, List<Action>>();

		public BossEncounter CurrentEncounter = new BossEncounter { SnoId = -1, activated = false, acceptedPlayers = 0 };

		/// <summary>
		/// Starting world's sno.
		/// </summary>
		public WorldSno StartingWorldSNO { get; private set; }

		/// <summary>
		/// Starting world's monster level
		/// </summary>
		public int InitialMonsterLevel { get; set; }
		public int MonsterLevel { get; private set; }

		/// <summary>
		/// Is it world without players?
		/// </summary>
		public bool Empty { get; private set; }

		/// <summary>
		/// Paused game state (for single-player only)
		/// </summary>
		public bool Paused { get; private set; }

		private bool UpdateEnabled = true;

		/// <summary>
		/// Starting world for the game.
		/// </summary>
		public World StartingWorld
		{
			get
			{
				return GetWorld(StartingWorldSNO);
			}
		}

		/// <summary>
		/// Player index counter.
		/// </summary>
		public int PlayerIndexCounter = -1;
		public int PlayerGroupIndexCounter = 0;

		/// <summary>
		/// Current quest SNOid.
		/// </summary>
		public int CurrentQuest = -1;
		public int CurrentSideQuest = -1;

		/// <summary>
		/// Current quest step SNOid.
		/// </summary>
		public int DestinationEnterQuest = -1;
		public int DestinationEnterQuestStep = -1;

		/// <summary>
		/// Current act system id.
		/// </summary>
		public int CurrentAct = -1;
		
		/// <summary>
		/// Current difficulty system id.
		/// </summary>
		public int Difficulty = 0;
		public float HPModifier = 1f;
		public float DmgModifier = 1f;
		public float XPModifier = 1f;
		public float GoldModifier = 1f;
		
		/// <summary>
		/// Hardcore mode flag.
		/// </summary>
		public bool IsHardcore = false;
		public bool IsSeasoned = false;

		public List<int> OpenedWaypoints = new List<int>();

		public Dictionary<DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT, int> BountiesCompleted = new Dictionary<DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT, int>()
		{
			{DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A1, 0},
			{DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A2, 0},
			{DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A3, 0},
			{DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A4, 0},
			{DiIiS_NA.Core.MPQ.FileFormats.BountyData.ActT.A5, 0}
		};

		/// <summary>
		/// Current act SNO id.
		/// </summary>
		public int CurrentActSNOid
		{
			get
			{
				switch (CurrentAct)
				{
					case 0:
						return 70015;
					case 100:
						return 70016;
					case 200:
						return 70017;
					case 300:
						return 70018;
					case 400:
						return 236915;
					default:
						return 70015;
				}
			}
		}

		/// <summary>
		/// Last completed quest SNOid.
		/// </summary>
		public int LastCompletedQuest = -1;

		/// <summary>
		/// Current quest step SNOid.
		/// </summary>
		public int CurrentStep = -1;
		public int CurrentSideStep = -1;

		/// <summary>
		/// Current quest order (for auto-advance).
		/// </summary>
		public int[] QuestsOrder;

		/// <summary>
		/// Current quest progress handler.
		/// </summary>
		public QuestRegistry QuestProgress;
		public QuestRegistry SideQuestProgress;

		/// <summary>
		/// World generator for this game
		/// </summary>
		public WorldGenerator WorldGenerator;

		/// <summary>
		/// Database connection for this game
		/// </summary>
		public GameDBSession GameDBSession;

		/// <summary>
		/// Update frequency for the game - 100 ms.
		/// </summary>
		public readonly long UpdateFrequency = 100;

		/// <summary>
		/// Incremented tick value on each Game.Update().
		/// </summary>
		public readonly int TickRate = 6;

		/// <summary>
		/// Tick counter.
		/// </summary>
		private int _tickCounter;

		/// <summary>
		/// Returns the latest tick count.
		/// </summary>
		public int TickCounter
		{
			get { return _tickCounter; }
		}

		/// <summary>
		/// Stopwatch that measures time takent to get a full Game.Update(). 
		/// </summary>
		//private readonly Stopwatch _tickWatch;

		/// <summary>
		/// DynamicId counter for scene.
		/// </summary>
		private uint _lastSceneID = 0x04000000;

		/// <summary>
		/// Returns a new dynamicId for scenes.
		/// </summary>
		public uint NewSceneID
		{
			get
			{
				lock (obj)
				{
					_lastSceneID++;
					return _lastSceneID;
				}
			}
		}

		public int WaypointFlags
		{
			get
			{
				if (CurrentAct == 3000) return 0x0000ffff;
				int flags = 0;
				for (int i = 16; i >= 0; i--)
				{
					flags = flags << 1;
					if (OpenedWaypoints.Contains(i)) flags++;
				}
				return flags;
			}
		}

		public Vector3D StartPosition
		{
			get
			{
				switch (CurrentAct)
				{
					case 0:
						return StartingWorld.GetStartingPointById(24).Position;
					case 100:
						return StartingWorld.GetStartingPointById(59).Position;
					case 200:
						return StartingWorld.GetStartingPointById(172).Position;
					case 300:
						return StartingWorld.GetStartingPointById(172).Position;
					case 400:
						return StartingWorld.GetStartingPointById(172).Position;
					case 3000:
						return StartingWorld.GetStartingPointById(24).Position;
					default:
						return StartingWorld.StartingPoints.First().Position;
				}
			}
		}

		/// <summary>
		/// DynamicId counter for worlds.
		/// </summary>
		private uint _lastWorldID = 0x07000000;

		public int WeatherSeed = DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next();

		/// <summary>
		/// Returns a new dynamicId for worlds.
		/// </summary>
		public uint NewWorldID { get { return _lastWorldID++; } }

		public QuestManager QuestManager { get; private set; }
		//public AI.Pather Pathfinder { get; private set; }

		public bool Working = true;

		public bool PvP = false;

		/// <summary>
		/// Creates a new game with given gameId.
		/// </summary>
		/// <param name="gameId"></param>
		public Game(int gameId, int initalLevel, bool endless = false)
		{
			GameId = gameId;
			_lastObjectID = (uint)gameId * 100000;
			Empty = true;
			Players = new ConcurrentDictionary<GameClient, Player>();
			_worlds = new ConcurrentDictionary<WorldSno, World>();
			StartingWorldSNO = WorldSno.pvp_caout_arena_01;// FIXME: track the player's save point and toss this stuff. 
			InitialMonsterLevel = initalLevel;
			MonsterLevel = initalLevel;
			QuestManager = new QuestManager(this);
			CurrentAct = -1;
			QuestsOrder = null;
			CurrentQuest = -1;
			CurrentStep = -1;
			CurrentSideQuest = -1;
			CurrentSideStep = -1;
			QuestProgress = null;
			SideQuestProgress = new Events(this);

			var loopThread = new Thread(Update) { Name = "GameLoopThread", IsBackground = true }; ; // create the game update thread.
			loopThread.Start();

			WorldGenerator = new WorldGenerator(this);
			GameDBSession = new GameDBSession();
			LockdownTimer = TickTimer.WaitSeconds(this, 60f, new Action<int>((q) =>
			{
				if (Empty || Players.IsEmpty)
				{
					Logger.Info("All players disconnected, closing game session.");
					Dispose();
					GameManager.Games.Remove(GameId);
				}
			}));


		}

		#region update & tick managment

		private object updateLock = new object();

		public int MissedTicks = 0;
		public bool UpdateInProgress = false;
		/// <summary>
		/// The main game loop.
		/// </summary>
		public void Update()
		{
			while (Working)
			{
				Stopwatch _tickWatch = new Stopwatch();
				_tickWatch.Restart();
				if (Players.Count == 0 && !Empty)
				{
					Logger.Info("Все игроки отключены, сессия игры завершена");
					Dispose();
					GameManager.Games.Remove(GameId);
					return;
				}


				Interlocked.Add(ref _tickCounter, (TickRate + MissedTicks)); // +6 ticks per 100ms. Verified by setting LogoutTickTimeMessage.Ticks to 600 which eventually renders a 10 sec logout timer on client. /raist
				MissedTicks = 0;

				if (UpdateEnabled && !Paused)
				{
					// Lock Game instance to prevent incoming messages from modifying state while updating
					// only update worlds with active players in it - so mob brain()'s in empty worlds doesn't get called and take actions for nothing. /raist.
					lock (updateLock)
					{
						foreach (var pair in _worlds.Where(pair => pair.Value.HasPlayersIn))
						{
							try
							{
								pair.Value.Update(_tickCounter);
							}
							catch (Exception e)
							{
								Logger.WarnException(e, "update worlds exception: ");
							}
						}

						if (PvPTimer != null)
							PvPTimer.Update(_tickCounter);

						if (GlobalPvPTimer != null)
							GlobalPvPTimer.Update(_tickCounter);

						if (LockdownTimer != null)
							LockdownTimer.Update(_tickCounter);

						if (QuestTimer != null)
							QuestTimer.Update(_tickCounter);
					}
				}
				_tickWatch.Stop();

				Stopwatch _calcWatch = new Stopwatch();
				_calcWatch.Restart();
				var compensation = (int)(UpdateFrequency - _tickWatch.ElapsedMilliseconds); // the compensation value we need to sleep in order to get consistent 100 ms Game.Update().

				if (_tickWatch.ElapsedMilliseconds > UpdateFrequency)
				{
					Logger.Trace("Game.Update() took [{0}ms] more than Game.UpdateFrequency [{1}ms].", _tickWatch.ElapsedMilliseconds, UpdateFrequency);
					compensation = (int)(UpdateFrequency - (_tickWatch.ElapsedMilliseconds % UpdateFrequency));
					MissedTicks = TickRate * (int)(_tickWatch.ElapsedMilliseconds / UpdateFrequency);
				}
				_calcWatch.Stop();
				Thread.Sleep(Math.Max(0, compensation - (int)_calcWatch.ElapsedMilliseconds)); // sleep until next Update().
			}
		}
		#endregion

		#region game-message handling & routing
		/// <summary>
		/// Routers incoming GameMessage to it's proper consumer.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="message"></param>
		public void Route(GameClient client, GameMessage message)
		{
			UpdateEnabled = false;
			try
			{
				switch (message.Consumer)
				{
					case Consumers.Game:
						Consume(client, message);
						break;
					case Consumers.Inventory:
						client.Player.Inventory.Consume(client, message);
						break;
					case Consumers.Player:
						client.Player.Consume(client, message);
						break;

					case Consumers.Conversations:
						client.Player.Conversations.Consume(client, message);
						break;

					case Consumers.SelectedNPC:
						if (client.Player.SelectedNPC != null)
							client.Player.SelectedNPC.Consume(client, message);
						break;
				}
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled exception caught:");
			}
			finally
			{
				UpdateEnabled = true;
			}
		}

		public void Consume(GameClient client, GameMessage message)
		{
			lock (updateLock)
			{
				if (message is PauseGameMessage) OnPause(client, (PauseGameMessage)message);
				else if (message is RaiseGameDifficulty) RaiseDifficulty(client, (RaiseGameDifficulty)message);
				else if (message is LowGameDifficulty) LowDifficulty(client, (LowGameDifficulty)message);
			}
		}

		#endregion

		#region player-handling

		public void LowDifficulty(GameClient client, LowGameDifficulty message)
		{
			Difficulty--;
			SetDifficulty(Difficulty);
			foreach (var plr in Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		public void RaiseDifficulty(GameClient client, RaiseGameDifficulty message)
		{
			Difficulty++;
			SetDifficulty(Difficulty);
			foreach (var plr in Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		/// <summary>
		/// Allows a player to join the game.
		/// </summary>
		/// <param name="joinedPlayer">The new player.</param>
		public void Enter(Player joinedPlayer)
		{
			if (IsHardcore && !joinedPlayer.Toon.DBToon.isHardcore)
			{
				return;
			}

			Task.Run(() =>
			{
				lock (Players)
				{
					Players.TryAdd(joinedPlayer.InGameClient, joinedPlayer);

					// send all players in the game to new player that just joined (including him)
					foreach (var pair in Players)
					{
						if (pair.Value.PlayerGroupIndex == joinedPlayer.PlayerGroupIndex)
							SendNewPlayerMessage(joinedPlayer, pair.Value);
					}
					foreach (var pair in Players.Where(pair => pair.Value != joinedPlayer))
					{
						if (pair.Value.PlayerGroupIndex == joinedPlayer.PlayerGroupIndex)
							SendNewPlayerMessage(pair.Value, joinedPlayer);
					}
					joinedPlayer.LoadShownTutorials();
					joinedPlayer.LoadCrafterData();
					joinedPlayer.LoadCurrencyData();
					//joinedPlayer.LoadMailData();
					joinedPlayer.LoadStashIconsData();

					if (!PvP)
					{
						joinedPlayer.InGameClient.TickingEnabled = true; // it seems bnet-servers only start ticking after player is completely in-game. /raist
						joinedPlayer.InGameClient.SendMessage(new GameSyncedDataMessage
						{
							SyncedData = new GameSyncedData
							{
								GameSyncedFlags = 6,
								Act = CurrentAct,       //act id
								InitialMonsterLevel = InitialMonsterLevel, //InitialMonsterLevel
								MonsterLevel = 0x6FEA8DF5, //MonsterLevel
								RandomWeatherSeed = 0, //RandomWeatherSeed
								OpenWorldMode = CurrentAct == 3000 ? -1 : 0, //OpenWorldMode
								OpenWorldModeAct = -1, //OpenWorldModeAct
								OpenWorldModeParam = -1, //OpenWorldModeParam
								OpenWorldTransitionTime = 0, //OpenWorldTransitionTime
								OpenWorldDefaultAct = -1, //OpenWorldDefaultAct
								OpenWorldBonusAct = -1, //OpenWorldBonusAct
								SNODungeonFinderLevelArea = 0, //SNODungeonFinderLevelArea
								LootRunOpen = GameMode == Mode.Portals ? 0 : -1, //LootRunOpen //0 - Великий Портал
								OpenLootRunLevel = 0, //OpenLootRunLevel
								LootRunBossDead = 0, //LootRunBossDead
								HunterPlayerIdx = -1, //HunterPlayerIdx
								LootRunBossActive = 0, //LootRunBossActive
								TieredLootRunFailed = 0, //TieredLootRunFailed
								LootRunChallengeCompleted = 0, //LootRunChallengeCompleted
								SetDungeonActive = -1, //SetDungeonActive
								Pregame = 0, //Pregame
								PregameEnd = 0, //PregameEnd
								RoundStart = 0, //RoundStart
								RoundEnd = 0, //RoundEnd
								PVPGameOver = 0x0, //PVPGameOver
								field_v273 = 0x0,
								TeamWins = new[] { 0x0, 0x0 }, //TeamWins
								TeamScore = new[] { 0x0, 0x0 }, //TeamScore
								PVPGameResult = new[] { -1, -1 }, //PVPGameResult
								PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
								TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID
							}
						});
						if ((CurrentStep == -1 || CurrentAct == 400) && (CurrentQuest == QuestsOrder[0]) && CurrentAct != 3000)
						{
							switch (CurrentAct)
							{
								case 0:
									joinedPlayer.EnterWorld(StartingWorld.GetStartingPointById(0).Position);
									break;
								case 100:
									joinedPlayer.EnterWorld(StartingWorld.GetStartingPointById(130).Position);
									break;
								case 200:
									joinedPlayer.ChangeWorld(GetWorld(WorldSno.a3dun_hub_adria_tower_intro), GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetStartingPointById(206).Position);
									break;
								case 300:
									joinedPlayer.ChangeWorld(GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance), GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).StartingPoints.First().Position);
									break;
								case 400:
									joinedPlayer.ChangeWorld(GetWorld(WorldSno.x1_westmarch_overlook_d), GetWorld(WorldSno.x1_westmarch_overlook_d).StartingPoints.First().Position);
									break;
								default:
									break;
							}
							joinedPlayer.PlayCutscene(0);
						}
						else
						{
							joinedPlayer.EnterWorld(StartPosition);
						}
					}
					else
					{
						joinedPlayer.EnterWorld(StartingWorld.GetStartingPointById(288 + joinedPlayer.PlayerIndex).Position);
					}
					Empty = false;

					foreach (var portal in StartingWorld.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal, ActorSno._x1_openworld_tiered_rifts_portal, ActorSno._x1_openworld_tiered_rifts_challenge_portal))
					{
						portal.Destroy();
					}

					ClientSystem.GameServer.GSBackend.PlayerJoined(GameId);


					//joinedPlayer.InGameClient.SendTick();
					/*
					if (this.Players.Count < 2)
					{
						int? hirelingId = joinedPlayer.Toon.DBToon.ActiveHireling;
						if (hirelingId != null)
						{
							Hireling hireling = null;
							switch (hirelingId)
							{
								case 1:
									hireling = new Templar(joinedPlayer.World, 52693, new TagMap());
									hireling.GBHandle.GBID = StringHashHelper.HashItemName("Templar");
									break;
								case 2:
									hireling = new Scoundrel(joinedPlayer.World, 52694, new TagMap());
									hireling.GBHandle.GBID = StringHashHelper.HashItemName("Scoundrel");
									break;
								case 3:
									hireling = new Enchantress(joinedPlayer.World, 4482, new TagMap());
									hireling.GBHandle.GBID = StringHashHelper.HashItemName("Enchantress");
									break;
								default:
									hireling = new Templar(joinedPlayer.World, 52693, new TagMap());
									hireling.GBHandle.GBID = StringHashHelper.HashItemName("Templar");
									break;
							}
							hireling.SetUpAttributes(joinedPlayer);
							hireling.GBHandle.Type = 4;

							hireling.Attributes[GameAttribute.Pet_Creator] = joinedPlayer.PlayerIndex;
							hireling.Attributes[GameAttribute.Pet_Type] = 0;
							hireling.Attributes[GameAttribute.Pet_Owner] = joinedPlayer.PlayerIndex;
							hireling.Attributes[GameAttribute.Untargetable] = false;
							hireling.Attributes[GameAttribute.NPC_Is_Escorting] = true;

							hireling.RotationW = joinedPlayer.RotationW;
							hireling.RotationAxis = joinedPlayer.RotationAxis;
							if (hireling.Brain == null)
								hireling.Brain = new AISystem.Brains.HirelingBrain(hireling, joinedPlayer);
							hireling.Brain.DeActivate();
							hireling.EnterWorld(joinedPlayer.Position);
							hireling.Brain = new HirelingBrain(hireling, joinedPlayer);
							(hireling.Brain as HirelingBrain).Activate();
							joinedPlayer.ActiveHireling = hireling;
						}
					}
					else
					/**/
					{
						foreach (var plr in Players.Values)
							if (plr.ActiveHireling != null)
							{
								plr.ActiveHireling.Dismiss();
								plr.ActiveHireling = null;
							}
					}
					/**/
					
					if (!PvP && !((CurrentStep == -1) && (CurrentQuest == QuestsOrder[0])))
					{
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = CurrentQuest,
							snoLevelArea = -1,
							StepID = CurrentStep,
							DisplayButton = true,
							Failed = false
						});
					}

					
					if (joinedPlayer.PlayerIndex == 0)
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage)
						{
							Difficulty = (uint)Difficulty
						});

					

					UpdateLevel();
					joinedPlayer.NotifyMaintenance();

					if (CurrentAct == 3000 && !joinedPlayer.InGameClient.OpenWorldDefined)
					{
						joinedPlayer.InGameClient.OpenWorldDefined = true;
						joinedPlayer.InGameClient.SendMessage(new ActTransitionMessage
						{
							Act = 3000,
							OnJoin = true
						});

						foreach (var bounty in QuestManager.Bounties)
						{
							joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = bounty.BountySNOid,
								snoLevelArea = bounty.LevelArea,
								StepID = -1,
								DisplayButton = true,
								Failed = false
							});
						}
						CurrentQuest = 0x0004C46D;
						QuestManager.Advance();

						joinedPlayer.InGameClient.SendMessage(new IntDataMessage(Opcodes.DungeonFinderSeedMessage)
						{
							Field0 = 0x3E0FC64C
						});

						joinedPlayer.InGameClient.SendMessage(new IntDataMessage(Opcodes.DungeonFinderParticipatingPlayerCount)
						{
							Field0 = 0
						});

						joinedPlayer.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
						{
							Field0 = 0
						});

						joinedPlayer.InGameClient.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
						{
							Field0 = 0
						});

						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.SavePointInfoMessage()
						{
							snoLevelArea = joinedPlayer.CurrentScene.Specification.SNOLevelAreas[0],//102362,
						});

						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage
						{
							snoQuest = 0x0005727C,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage
						{
							snoQuest = 0x00057282,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage
						{
							snoQuest = 0x00057284,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage
						{
							snoQuest = 0x00057287,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage
						{
							snoQuest = 0x00057289,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
					}
				}
			});
		}

		public void UpdateLevel()
		{
			if (Players.Count < 1)
				return;
			MonsterLevel = Players.Values.Select(p => p.Level).Max();
			foreach (var wld in _worlds)
				foreach (var monster in wld.Value.Monsters)
					monster.UpdateStats();
		}


		public void EnablePerfTest(int charId)
		{
			
		}

		private int[] questsOrder_a1 = new[] { 87700, 72095, 72221, 72061, 117779, 72738, 73236, 72546, 72801, 136656 };

		private int[] questsOrder_a2 = new[] { 80322, 93396, 74128, 57331, 78264, 78266, 57335, 57337, 121792, 57339 };

		private int[] questsOrder_a3 = new[] { 93595, 93684, 93697, 203595, 101756, 101750, 101758 };

		private int[] questsOrder_a4 = new[] { 112498, 113910, 114795, 114901 };

		private int[] questsOrder_a5 = new[] { 251355, 284683, 285098, 257120, 263851, 273790, 269552, 273408 };

		private int[] questsOrder_openWorld = new[] { 312429 };

		
		public void SetQuestProgress(int currQuest, int step)
		{

			if (PvP) return;
			if (!QuestSetup)
			{
				QuestManager.SetQuests();
				DestinationEnterQuest = currQuest;
				DestinationEnterQuestStep = step;

				Logger.Trace("SetQuestProgress: quest {0}, step {1}", currQuest, step);
				CurrentQuest = QuestsOrder[0];
				CurrentStep = -1;

				if (CurrentAct is 3000)
				{
					QuestManager.Quests[CurrentQuest].Steps[-1].OnAdvance.Invoke();
					return;
				}

				if (!(currQuest == QuestsOrder[0] && step == -1))
				{
					if (currQuest == -1 && step == -1)
					{
						currQuest = CurrentQuest;
						QuestManager.Quests[currQuest].Steps[-1].OnAdvance.Invoke();
						return;
					}

					QuestManager.AdvanceTo(currQuest, step);
					return;
				}

				if (CurrentQuest == QuestsOrder[0] && CurrentStep == -1)
					QuestManager.Quests[CurrentQuest].Steps[-1].OnAdvance.Invoke();
			}

		}
		public void SetGameMode(Mode mode)
		{
			GameMode = mode;

			switch (GameMode)
			{
				case Mode.Portals:
					QuestsOrder = new int[] { -1 };
					StartingWorldSNO = WorldSno.weekly_challenge_hub;
					QuestProgress = new QuestRegistry(this);
					break;
			}
		}
		public void SetAct(int act)
		{
			if (PvP)
			{
				CurrentAct = 0;
				QuestsOrder = questsOrder_a1;
				QuestProgress = new QuestRegistry(this);
				StartingWorldSNO = WorldSno.pvp_caout_arena_01;
				return;
			}
			if (CurrentAct != act)
			{
				CurrentAct = act;

				switch (act)
				{
					case 0:
						QuestsOrder = questsOrder_a1;
						StartingWorldSNO = WorldSno.trout_town;
						QuestProgress = new ActI(this);
						break;
					case 100:
						QuestsOrder = questsOrder_a2;
						StartingWorldSNO = WorldSno.caout_refugeecamp;
						QuestProgress = new ActII(this);
						break;
					case 200:
						QuestsOrder = questsOrder_a3;
						StartingWorldSNO = WorldSno.a3dun_hub_keep;
						QuestProgress = new ActIII(this);
						break;
					case 300:
						QuestsOrder = questsOrder_a4;
						StartingWorldSNO = WorldSno.a4dun_heaven_hub_keep;
						QuestProgress = new ActIV(this);
						break;
					case 400:
						QuestsOrder = questsOrder_a5;
						StartingWorldSNO = WorldSno.x1_westmarch_hub;
						QuestProgress = new ActV(this);
						break;
					case 3000:
						QuestsOrder = questsOrder_openWorld;
						StartingWorldSNO = WorldSno.x1_tristram_adventure_mode_hub;
						QuestProgress = new OpenWorld(this);
						QuestManager.SetBounties();
						break;
					default:
						QuestsOrder = questsOrder_a1;
						StartingWorldSNO = WorldSno.trout_town;
						QuestProgress = new QuestRegistry(this);
						break;
				}
			}
		}

		public void ChangeAct(int act)
		{
			foreach(var plr in Players.Values)
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
			SetAct(act);
			CurrentQuest = QuestsOrder[0];
			CurrentStep = -1;
			QuestManager.ReloadQuests();
			OpenedWaypoints = new List<int>() { };
			foreach (var plr in Players)
			{
				plr.Key.SendMessage(new ActTransitionMessage
				{
					Act = act,
					OnJoin = true, //with cutscenes
				});

				plr.Value.UpdateHeroState();
				if (act == 3000)
				{
					plr.Key.SendMessage(new IntDataMessage(Opcodes.DungeonFinderSeedMessage)
					{
						Field0 = 0x3E0FC64C
					});

					plr.Key.SendMessage(new IntDataMessage(Opcodes.DungeonFinderParticipatingPlayerCount)
					{
						Field0 = 0
					});

					plr.Key.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
					{
						Field0 = 0
					});

					plr.Key.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
					{
						Field0 = 0
					});
				}
				plr.Key.SendMessage(new GameSyncedDataMessage
				{
					SyncedData = new GameSyncedData
					{
						GameSyncedFlags = IsSeasoned ? IsHardcore ? 6 : 4 : IsHardcore == true ? 4 : 6,
						Act = Math.Min(CurrentAct, 3000),       //act id
						InitialMonsterLevel = InitialMonsterLevel, //InitialMonsterLevel
						MonsterLevel = 0x7044248F, //MonsterLevel
						RandomWeatherSeed = 0, //RandomWeatherSeed
						OpenWorldMode = CurrentAct == 3000 ? -1 : 0, //OpenWorldMode
						OpenWorldModeAct = -1, //OpenWorldModeAct
						OpenWorldModeParam = -1, //OpenWorldModeParam
						OpenWorldTransitionTime = 200, //OpenWorldTransitionTime
						OpenWorldDefaultAct = -1, //OpenWorldDefaultAct
						OpenWorldBonusAct = -1, //OpenWorldBonusAct
						SNODungeonFinderLevelArea = 0, //SNODungeonFinderLevelArea
						LootRunOpen = GameMode == Mode.Portals ? 0 : -1, //LootRunOpen //0 - Великий Портал
						OpenLootRunLevel = 0, //OpenLootRunLevel
						LootRunBossDead = 0, //LootRunBossDead
						HunterPlayerIdx = -1, //HunterPlayerIdx
						LootRunBossActive = 0, //LootRunBossActive
						TieredLootRunFailed = 0, //TieredLootRunFailed
						LootRunChallengeCompleted = 0, //LootRunChallengeCompleted
						SetDungeonActive = -1, //SetDungeonActive
						Pregame = 0, //Pregame
						PregameEnd = 0, //PregameEnd
						RoundStart = 0, //RoundStart
						RoundEnd = 0, //RoundEnd
						PVPGameOver = 0x0, //PVPGameOver
						field_v273 = 0x0,
						TeamWins = new[] { 0x0, 0x0 }, //TeamWins
						TeamScore = new[] { 0x0, 0x0 }, //TeamScore
						PVPGameResult = new[] { -1, -1 }, //PVPGameResult
						PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
						TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID
					}
				});
				switch (act)
				{
					case 0:
						plr.Value.ChangeWorld(StartingWorld, StartingWorld.GetStartingPointById(0).Position);
						break;
					case 100:
						plr.Value.ChangeWorld(StartingWorld, StartingWorld.GetStartingPointById(130).Position);
						break;
					case 200:
						plr.Value.ChangeWorld(GetWorld(WorldSno.a3dun_hub_adria_tower_intro), GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetStartingPointById(206).Position);
						break;
					case 300:
						plr.Value.ChangeWorld(GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance), GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).StartingPoints.First().Position);
						break;
					case 400:
						plr.Value.ChangeWorld(GetWorld(WorldSno.x1_westmarch_overlook_d), GetWorld(WorldSno.x1_westmarch_overlook_d).StartingPoints.First().Position);
						break;
					default:
						break;
				}
				for (int i = 0; i < 10; i++)
				{
					plr.Key.SendMessage(new PlayerLoadoutTabIconMessage(Opcodes.PlayerLoadoutTabIconMessage)
					{
						Field0 = i,
						TabIcon = i
					});
				}
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 0, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 1, TeamFlags = 0, TeamColoring = 2 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 2, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 3, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 4, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 5, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 6, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 7, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 8, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 9, TeamFlags = 0, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 10, TeamFlags = 2, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 11, TeamFlags = 2, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 12, TeamFlags = 2, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 13, TeamFlags = 2, TeamColoring = -1 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 14, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 15, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 16, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 17, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 18, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 19, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 20, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 21, TeamFlags = 0, TeamColoring = 0 });
				plr.Key.SendMessage(new RevealTeamMessage() { Team = 22, TeamFlags = 0, TeamColoring = 0 });

				plr.Value.PlayCutscene(0);
			}
			try
			{
				QuestManager.Quests[QuestsOrder[0]].Steps[-1].OnAdvance.Invoke();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "onAdvance():");
			}
		}

		public void UpdateLevel(int level)
		{
			MonsterLevel = level;
			foreach (var wld in _worlds)
				foreach (var monster in wld.Value.Monsters)
					monster.UpdateStats();
		}

		public void SetDifficulty(int diff)
		{
			Difficulty = diff;
			diff++;
			if (diff > 0)
			{
				var handicapLevels = (GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][256027].Data;
				HPModifier = handicapLevels.HandicapLevelTables[diff].HPMod;
				DmgModifier = handicapLevels.HandicapLevelTables[diff].DmgMod;
				XPModifier = (1f + handicapLevels.HandicapLevelTables[diff].XPMod);
				GoldModifier = (1f + handicapLevels.HandicapLevelTables[diff].GoldMod);
			}
			foreach (var wld in _worlds)
				foreach (var monster in wld.Value.Monsters)
					monster.UpdateStats();
			foreach(var plr in Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		public void UnlockTeleport(int waypointId)
		{
			OpenedWaypoints.Add(waypointId);
		}

		public Actor GetHearthPortal()
		{
			return StartingWorld.Actors.Values.Where(x => x.SNO == ActorSno._hearthportal).First();
		}

		private void OnPause(GameClient client, PauseGameMessage message)
		{
			if (Players.Count == 1)
			{
				Logger.Trace("Game state is paused: {0}", message.Field0);
				Players.First().Value.Attributes[GameAttribute.Disabled] = message.Field0;
				Players.First().Value.Attributes[GameAttribute.Immobolize] = message.Field0;
				//this.Players.First().Value.Attributes[GameAttribute.Stunned] = message.Field0;
				Players.First().Value.Attributes.BroadcastChangedIfRevealed();
				//this.Players.First().Key.TickingEnabled = !message.Field0;
				Paused = message.Field0;
				Players.First().Key.SendMessage(new FreezeGameMessage
				{
					Field0 = message.Field0
				});
			}
		}

		/// <summary>
		/// Sends NewPlayerMessage to players when a new player joins the game. 
		/// </summary>
		/// <param name="target">Target player to send the message.</param>
		/// <param name="joinedPlayer">The new joined player.</param>
		private void SendNewPlayerMessage(Player target, Player joinedPlayer)
		{
			target.InGameClient.SendMessage(new NewPlayerMessage
			{
				PlayerIndex = joinedPlayer.PlayerIndex,
				NewToonId = (long)joinedPlayer.Toon.D3EntityID.IdLow,
				GameAccountId = new GameAccountHandle() { ID = (uint)joinedPlayer.Toon.GameAccount.BnetEntityId.Low, Program = 0x00004433, Region = 1 },
				ToonName = joinedPlayer.Toon.Name,
				Team = 0x00000002,
				Class = joinedPlayer.ClassSNO,
				snoActorPortrait = joinedPlayer.Toon.DBToon.Cosmetic4,
				Level = joinedPlayer.Toon.Level,
				AltLevel = (ushort)joinedPlayer.Toon.ParagonLevel,
				HighestHeroSoloRiftLevel = 0,
				StateData = joinedPlayer.GetStateData(),
				JustJoined = false,
				Field9 = 0x77EA0000,
				ActorID = joinedPlayer.DynamicID(target),
			});

			if (PvP)
				target.InGameClient.SendMessage(new RevealTeamMessage
				{
					Team = joinedPlayer.PlayerIndex + 2,
					TeamFlags = 0,
					TeamColoring = 0
				});

			target.InGameClient.SendMessage(joinedPlayer.GetPlayerBanner()); // send player banner proto - D3.GameMessage.PlayerBanner
		}

		public void BroadcastMessage(string message)
		{
			lock (Players)
			{
				foreach (var plr in Players.Keys)
					plr.SendMessage(new BroadcastTextMessage() { Field0 = message });
			}
		}

		public void StartPvPRound()
		{
			CurrentPvPRound++;

			var winner = Players.Values.Where(p => !p.Dead).FirstOrDefault();
			if (winner != null && CurrentPvPRound > 1)
			{
				BroadcastMessage("Round is over! Winner: " + winner.Toon.Name);
				if (winner.Attributes[GameAttribute.TeamID] == 2)
					RedTeamWins++;
				else
					BlueTeamWins++;
			}

			if (CurrentPvPRound > 5 || Math.Abs(RedTeamWins - BlueTeamWins) > (5 - CurrentPvPRound))
			{
				BroadcastMessage("Battle is over!");
				try
				{
					var totalWinner = Players.Values.Where(p => p.Attributes[GameAttribute.TeamID] == (RedTeamWins > BlueTeamWins ? 2 : 3)).FirstOrDefault();
					BroadcastMessage("Winner: " + totalWinner.Toon.Name);
				}
				catch { Logger.Warn("Exception on FindWinner()"); }

				//foreach (var player in this.Players.Values)
				//player.World.BuffManager.AddBuff(player, player, new Mooege.Core.GS.Powers.Implementations.PVPRoundEndBuff(TickTimer.WaitSeconds(this, 1200.0f)));
				foreach (var plr in Players.Keys)
					plr.SendMessage(new DataIDDataMessage(Opcodes.PVPArenaWin) { Field0 = (RedTeamWins == BlueTeamWins ? 0 : (RedTeamWins < BlueTeamWins ? 2 : 3)) });
				return;
			}

			if (CurrentPvPRound == 1)
			{
				GlobalPvPTimer = TickTimer.WaitSeconds(this, 600f, new Action<int>((z) =>
				{
					BroadcastMessage("Time is up, battle is over!");
					if (RedTeamWins == BlueTeamWins)
					{
						BroadcastMessage("Draw!");

					}
					else
					{
						var TotalWinner = Players.Values.Where(p => p.Attributes[GameAttribute.TeamID] == (RedTeamWins > BlueTeamWins ? 2 : 3)).FirstOrDefault();
						BroadcastMessage("Winner: " + TotalWinner.Toon.Name);
					}

					//foreach (var player in this.Players.Values)
					//player.World.BuffManager.AddBuff(player, player, new Mooege.Core.GS.Powers.Implementations.PVPRoundEndBuff(TickTimer.WaitSeconds(this, 1200.0f)));
					foreach (var plr in Players.Keys)
						plr.SendMessage(new DataIDDataMessage(Opcodes.PVPArenaWin) { Field0 = (RedTeamWins == BlueTeamWins ? 0 : (RedTeamWins < BlueTeamWins ? 2 : 3)) });
				}));
			}


			PvPTimer = TickTimer.WaitSeconds(this, 3f, new Action<int>((x) =>
			{
				foreach (var player in Players.Values)
				{
					player.Revive(player.CheckPointPosition);
					player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total, player.Attributes[GameAttribute.Resource_Type_Primary]]);
					player.World.BuffManager.AddBuff(player, player, new PowerSystem.Implementations.PVPSkirmishBuff(TickTimer.WaitSeconds(this, 15.0f)));
				}
				BroadcastMessage("Round " + CurrentPvPRound + ". Battle will commence in 15 seconds!");
				BroadcastMessage("Score: " + RedTeamWins + ":" + BlueTeamWins);
				PvPTimer = TickTimer.WaitSeconds(this, 15f, new Action<int>((y) =>
				{
					BroadcastMessage("Fight!");
					foreach (var player in Players.Keys)
					{
						//player.SendMessage(new FightAnnounceMessage());
					}
				}));
			}));
		}

		/// <summary>
		/// Disposes all memory defore destroying game. 
		/// </summary>
		public void Dispose()
		{
			Working = false;
			if (Players.Count > 0)
				foreach (var plr in Players.Keys)
					plr.Connection.Disconnect();
			_worlds.Clear();
			Thread.Sleep(1000);
			GameDBSession.SessionDispose();
			GameManager.Games.Remove(GameId);
		}

		public void TeleportToBossEncounter(int snoId)
		{
			foreach (var player in Players.Values)
			{
				player.ClearDoorAnimations();
			}
			Paused = true;

			/*foreach (var player in this.Players.Values)
			{
				player.InGameClient.SendMessage(new ACDTranslateSyncMessage()
				{
					ActorId = player.DynamicID(player),
					Position = player.Position
				});
				/*player.InGameClient.SendMessage(new FreezeGameMessage
				{
					Field0 = true
				});
			}*/

			var encAsset = (DiIiS_NA.Core.MPQ.FileFormats.BossEncounter)MPQStorage.Data.Assets[SNOGroup.BossEncounter][snoId].Data;
			World encWorld = GetWorld((WorldSno)encAsset.Worlds[0]);
			Logger.Debug("TeleportToBossEncounter, worldId: {0}", encAsset.Worlds[0]);
			Vector3D startPoint = null;
			switch (snoId)
			{
				case 168925: //CainIntro
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				case 159592: //Leoric
					startPoint = encWorld.GetStartingPointById(23).Position;
					break;
				case 181436: //SpiderQueen
					startPoint = encWorld.GetStartingPointById(30).Position;
					break;
				case 159591: //Cain Death
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				case 158915: //Butcher
					startPoint = encWorld.GetStartingPointById(191).Position;
					break;
				case 195234: //Maghda
					startPoint = encWorld.StartingPoints.First().Position;
					break;
				case 226716: //SiegeBreaker
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				case 188021: //Cydaea
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				case 182960: //Iskatu
					startPoint = encWorld.GetStartingPointById(287).Position;
					break;
				case 220541: //Imperius_Spire
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				case 161280: //Diablo
					startPoint = encWorld.GetStartingPointById(172).Position;
					break;
				default:
					startPoint = encWorld.StartingPoints.First().Position;
					break;
			}
			var proximity = new RectangleF(startPoint.X - 1f, startPoint.Y - 1f, 2f, 2f);
			var scenes = encWorld.QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return; // cork (is it real?)

			var scene = scenes[0]; // Parent scene /fasbat

			if (scenes.Count == 2) // What if it's a subscene?
			{
				if (scenes[1].ParentChunkID != 0xFFFFFFFF)
					scene = scenes[1];
			}

			var levelArea = scene.Specification.SNOLevelAreas[0];

			foreach (var g_player in Players)
			{
				if (g_player.Value.World == encWorld)
					g_player.Value.Teleport(startPoint);
				else
					g_player.Value.ChangeWorld(encWorld, startPoint);
			}
			
			
			Paused = false;
			
			
			//handling quest triggers
			if (QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
			{
				var trigger = QuestProgress.QuestTriggers[levelArea];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
				{
					try
					{
						trigger.questEvent.Execute(encWorld); // launch a questEvent
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}
			//Исполнение скриптов катсцены
			if (GameMode == Mode.Campaign)
				switch (snoId)
				{
					case 168925: //CainIntro
								 //if (this.CurrentAct == 0)
						Task.Delay(1000).ContinueWith(delegate
						{
							foreach (var plr in Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });

							Task.Delay(1000).ContinueWith(delegate
							{
								foreach (var plr in Players.Values)
									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage()
									{
										ActorID = (int)encWorld.GetActorBySNO(ActorSno._test_cainintro_greybox_bridge_trout_tempworking).DynamicID(plr), Duration = 1f, Snap = false
									});

								Actor CainRun = null;
								Actor CainQuest = null;
								//Убираем лишнего каина.
								foreach (var Cain in encWorld.GetActorsBySNO(ActorSno._cain_intro))
									if (Cain.Position.Y > 140)
									{
										Cain.SetVisible(false);
										foreach (var plr in Players.Values) Cain.Unreveal(plr);
										CainQuest = Cain;
									}
									else
									{
										Cain.SetVisible(true);
										foreach (var plr in Players.Values) Cain.Reveal(plr);
										CainRun = Cain;
									}


								//Скелеты
								var Skeletons = encWorld.GetActorsBySNO(ActorSno._skeleton_cain);
								//Камни
								//var Rocks = encWorld.GetActorsBySNO(176);
								//Берем позицию для леорика, а самого на мороз
								Vector3D FakeLeoricPosition = new Vector3D(0f, 0f, 0f);
								foreach (var fake in encWorld.GetActorsBySNO(ActorSno._skeletonking_ghost))
								{
									FakeLeoricPosition = fake.Position;
									fake.Destroy();
								}
								//Берем каина
								var FirstPoint = new Vector3D(120.92718f, 121.26151f, 0.099973306f);
								var SecondPoint = new Vector3D(120.73298f, 160.61829f, 0.31863004f);
								var SceletonPoint = new Vector3D(120.11514f, 140.77332f, 0.31863004f);

								var FirstfacingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(CainRun, FirstPoint);
								var SecondfacingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(FirstPoint, SecondPoint);
								var ThirdfacingAngle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(SecondPoint, FakeLeoricPosition);
								//Подготовления завершены - НАЧИНАЕМ ТЕАТР=)
								Task.Delay(3000).ContinueWith(delegate
									{
										CainRun.Move(FirstPoint, FirstfacingAngle);
										foreach (var plr in Players.Values)
											plr.Conversations.StartConversation(80920);//Запуск диалога - 80920 //Фраза Каина, бежит первым до начала мостика, оглядывается. //"Cain_Run_CainIntro", 81080 - Анимация 
									Task.Delay(5000).ContinueWith(delegate
											{
												foreach (var skeleton in Skeletons)
												{
													skeleton.Move(SceletonPoint, ActorSystem.Movement.MovementHelpers.GetFacingAngle(skeleton, SceletonPoint));
												}
												CainRun.Move(SecondPoint, SecondfacingAngle);

												Task.Delay(7000).ContinueWith(delegate
												{
													//foreach (var rock in Rocks)
													//{
													//{[1013103213, {[Actor] [Type: Gizmo] SNOId:78439 GlobalId: 1013103213 Position: x:119.54008 y:140.65799 z:-4.535186 Name: Test_CainIntro_greybox_bridge_trOut_TempWorking}]}
													//Обрушиваем мостик //EffectGroup "CainIntro_shake", 81546
													var bridge = encWorld.GetActorBySNO(ActorSno._test_cainintro_greybox_bridge_trout_tempworking);
													bridge.PlayAnimation(5, (AnimationSno)bridge.AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault]);
													//}
													foreach (var skeleton in Skeletons)
													{
													//Убиваем скелетов
													skeleton.Destroy();
													}
												});
												Task.Delay(5000).ContinueWith(delegate
												{
													CainRun.Move(SecondPoint, ThirdfacingAngle);

												//(Должен быть диалог Король скилет.)
												var Leoric = encWorld.SpawnMonster(ActorSno._skeletonking_ghost, FakeLeoricPosition);
													Leoric.PlayActionAnimation(AnimationSno.skeletonking_ghost_spawn);
													Task.Delay(1000).ContinueWith(delegate
													{
														foreach (var plr in Players.Values)
															plr.Conversations.StartConversation(17692); //Фраза Леорика
															Task.Delay(14000).ContinueWith(delegate
															{
															//Leoric.PlayActionAnimation(9854); //Леорик призывает скелетов

															Leoric.PlayActionAnimation(AnimationSno.skeletonking_ghost_despawn); //Себаса
															Task.Delay(1000).ContinueWith(delegate
																	{
																		foreach (var plr in Players.Values)
																		{
																			plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
																			plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
																		}
																		CainQuest.SetVisible(true);
																		CainRun.SetVisible(false);

																		foreach (var fake in encWorld.GetActorsBySNO(ActorSno._skeletonking_ghost))
																		{
																			FakeLeoricPosition = fake.Position;
																			fake.Destroy();
																		}
																	});
															});
													});
												});
											});
									});
							});
						});

						break;
					case 159592: //Leoric

						break;
					case 158915: //ButcherLair
								 //if (this.CurrentAct == 0)

						var Butcher = encWorld.GetActorBySNO(ActorSno._butcher);
						if (Butcher != null)
							(Butcher as Monster).Brain.DeActivate();
						else
						{
							Butcher = encWorld.SpawnMonster(ActorSno._butcher, new Vector3D { X = 93.022f, Y = 89.86f, Z = 0.1f });
							(Butcher as Monster).Brain.DeActivate();
						}
						Task.Delay(1000).ContinueWith(delegate
						{
							//Butcher - 3526
							foreach (var plr in Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });

							Task.Delay(1000).ContinueWith(delegate
							{
								if (Butcher != null)
									(Butcher as Monster).Brain.DeActivate();
								foreach (var plr in Players.Values)
									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)Butcher.DynamicID(plr), Duration = 1f, Snap = false });


								foreach (var plr in Players.Values)
									plr.Conversations.StartConversation(211980); //ФРЭШ МИТ
																				 //	StartConversation(ButcherLair, 211980);
								Task.Delay(3000).ContinueWith(delegate
									{
										foreach (var plr in Players.Values)
										{
											plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
											plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });

										}
										Task.Delay(1500).ContinueWith(delegate
										{
											(Butcher as Monster).Brain.Activate();
										});
									});
							});
						});

						break;
				}

			foreach (var bounty in QuestManager.Bounties)
				bounty.CheckLevelArea(levelArea);

			CurrentEncounter.acceptedPlayers = 0;
			CurrentEncounter.activated = false;
		}

		public void AcceptBossEncounter()
		{
			CurrentEncounter.acceptedPlayers++;
			if (CurrentEncounter.acceptedPlayers >= Players.Count)
				TeleportToBossEncounter(CurrentEncounter.SnoId);
		}

		public void DeclineBossEncounter()
		{
			CurrentEncounter.activated = false;
			CurrentEncounter.acceptedPlayers = 0;
		}

		public void AddOnLoadWorldAction(WorldSno worldSNO, Action action)
		{
			Logger.Trace("AddOnLoadWorldAction: {0}", worldSNO);
			if (Players.Values.Any(p => p.World != null && p.World.SNO == worldSNO))
			{
				action.Invoke();
			}
			else
			{
				if (!OnLoadWorldActions.ContainsKey(worldSNO))
					OnLoadWorldActions.Add(worldSNO, new List<Action>());

				OnLoadWorldActions[worldSNO].Add(action);
			}
		}

		public void AddOnLoadSceneAction(int sceneSNO, Action action)
        {
			Logger.Trace("AddOnLoadSceneAction: {0}", sceneSNO);
			if (!OnLoadSceneActions.ContainsKey(sceneSNO))
					OnLoadSceneActions.Add(sceneSNO, new List<Action>());

				OnLoadSceneActions[sceneSNO].Add(action);
		}

#endregion

		#region world collection

		public void AddWorld(World world)
		{
			if (world.SNO == WorldSno.__NONE || WorldExists(world.SNO))
				Logger.Error(String.Format("World has an invalid SNO or was already being tracked (ID = {0}, SNO = {1})", world.GlobalID, world.SNO));
			else
				_worlds.TryAdd(world.SNO, world);
		}

		public void RemoveWorld(World world)
		{
			World removed;
			if (world.SNO == WorldSno.__NONE || !WorldExists(world.SNO))
				Logger.Error(String.Format("World has an invalid SNO or was not being tracked (ID = {0}, SNO = {1})", world.GlobalID, world.SNO));
			else
				_worlds.TryRemove(world.SNO, out removed);
		}

		public World GetWorld(WorldSno worldSNO)
		{
			if (worldSNO == WorldSno.__NONE)
				return null;

			World world;

			if (CurrentAct != 3000 && worldSNO == WorldSno.x1_tristram_adventure_mode_hub) //fix for a1 Tristram
				worldSNO = WorldSno.trout_town;

			if (!WorldExists(worldSNO)) // If it doesn't exist, try to load it
			{
				//Task loading = Task.Run(() => {world = this.WorldGenerator.Generate(worldSNO);});
				//if (!loading.Wait(TimeSpan.FromSeconds(30)))
				//Logger.Warn("Failed to generate world with sno: {0}", worldSNO);
				//bool loaded = false;

				//Action action = (() => {
				//	world = this.WorldGenerator.Generate(worldSNO);
				//});
				//this.WorldGenerator.Actions.Enqueue(action);
				//while (!loaded && this.Working)
				//{
				/*var timer = new System.Timers.Timer(1);
				timer.Elapsed += (src, args) => { if (!this.WorldGenerator.Actions.Contains(action)) loaded = true; };
				timer.AutoReset = false;
				timer.Start();*/
				//Task.Delay(1000).ContinueWith(t => { if (!this.WorldGenerator.Actions.Contains(action)) loaded = true; }).Wait();
				//}
				world = WorldGenerator.Generate(worldSNO);
				if (world == null) Logger.Warn("Failed to generate world with sno: {0}", worldSNO);
			}
			_worlds.TryGetValue(worldSNO, out world);
			return world;
		}

		public bool WorldExists(WorldSno worldSNO)
		{
			return _worlds.ContainsKey(worldSNO);
		}

		public bool WorldCleared(WorldSno worldSNO)
		{
			return _worlds[worldSNO].Actors.Values.OfType<Monster>().Where(m => m.OriginalLevelArea != -1 && !m.Dead).Count() < 5;
		}

		/// <summary>
		/// Returns World with given Waypoint id.
		/// </summary>
		/// <param name="id">The id of the WayPoint</param>
		/// <returns><see cref="World"/></returns>
		public World GetWayPointWorldById(int id)
		{
			bool isOpenWorld = CurrentAct == 3000;
			var actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][CurrentActSNOid].Data).WayPointInfo.ToList();
			if (isOpenWorld)
				actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70015].Data).WayPointInfo
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70016].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70017].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][70018].Data).WayPointInfo)
							.Union(((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][236915].Data).WayPointInfo)
							.Where(w => w.SNOWorld != -1).ToList();
			var wayPointInfo = actData.Where(w => w.Flags == 3 || (isOpenWorld ? (w.Flags == 2) : (w.Flags == 1))).ToList();
			//Logger.Debug("GetWayPointWorldById: world id {0}", wayPointInfo[id].SNOWorld);
			return GetWorld((WorldSno)wayPointInfo[id].SNOWorld);
		}

#endregion

	}
}
