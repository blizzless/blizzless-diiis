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

		public bool QuestSetuped = false;

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
				return this._worlds.Values.ToList();
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
				return GetWorld(this.StartingWorldSNO);
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
				switch (this.CurrentAct)
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
				if (this.CurrentAct == 3000) return 0x0000ffff;
				int flags = 0;
				for (int i = 16; i >= 0; i--)
				{
					flags = flags << 1;
					if (this.OpenedWaypoints.Contains(i)) flags++;
				}
				return flags;
			}
		}

		public Vector3D StartPosition
		{
			get
			{
				switch (this.CurrentAct)
				{
					case 0:
						return this.StartingWorld.GetStartingPointById(24).Position;
					case 100:
						return this.StartingWorld.GetStartingPointById(59).Position;
					case 200:
						return this.StartingWorld.GetStartingPointById(172).Position;
					case 300:
						return this.StartingWorld.GetStartingPointById(172).Position;
					case 400:
						return this.StartingWorld.GetStartingPointById(172).Position;
					case 3000:
						return this.StartingWorld.GetStartingPointById(24).Position;
					default:
						return this.StartingWorld.StartingPoints.First().Position;
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
			this.GameId = gameId;
			this._lastObjectID = (uint)gameId * 100000;
			this.Empty = true;
			this.Players = new ConcurrentDictionary<GameClient, Player>();
			this._worlds = new ConcurrentDictionary<WorldSno, World>();
			this.StartingWorldSNO = WorldSno.pvp_caout_arena_01;// FIXME: track the player's save point and toss this stuff. 
			this.InitialMonsterLevel = initalLevel;
			this.MonsterLevel = initalLevel;
			this.QuestManager = new QuestManager(this);
			this.CurrentAct = -1;
			this.QuestsOrder = null;
			this.CurrentQuest = -1;
			this.CurrentStep = -1;
			this.CurrentSideQuest = -1;
			this.CurrentSideStep = -1;
			this.QuestProgress = null;
			this.SideQuestProgress = new Events(this);

			var loopThread = new Thread(Update) { Name = "GameLoopThread", IsBackground = true }; ; // create the game update thread.
			loopThread.Start();

			this.WorldGenerator = new WorldGenerator(this);
			this.GameDBSession = new GameDBSession();
			this.LockdownTimer = TickTimer.WaitSeconds(this, 60f, new Action<int>((q) =>
			{
				if (this.Empty || Players.IsEmpty)
				{
					Logger.Info("All players disconnected, closing game session.");
					this.Dispose();
					GameManager.Games.Remove(this.GameId);
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
			while (this.Working)
			{
				Stopwatch _tickWatch = new Stopwatch();
				_tickWatch.Restart();
				if (this.Players.Count == 0 && !this.Empty)
				{
					Logger.Info("Все игроки отключены, сессия игры завершена");
					this.Dispose();
					GameSystem.GameManager.Games.Remove(this.GameId);
					return;
				}


				Interlocked.Add(ref this._tickCounter, (this.TickRate + this.MissedTicks)); // +6 ticks per 100ms. Verified by setting LogoutTickTimeMessage.Ticks to 600 which eventually renders a 10 sec logout timer on client. /raist
				this.MissedTicks = 0;

				if (this.UpdateEnabled && !this.Paused)
				{
					// Lock Game instance to prevent incoming messages from modifying state while updating
					// only update worlds with active players in it - so mob brain()'s in empty worlds doesn't get called and take actions for nothing. /raist.
					lock (this.updateLock)
					{
						foreach (var pair in this._worlds.Where(pair => pair.Value.HasPlayersIn))
						{
							try
							{
								pair.Value.Update(this._tickCounter);
							}
							catch (Exception e)
							{
								Logger.WarnException(e, "update worlds exception: ");
							}
						}

						if (PvPTimer != null)
							PvPTimer.Update(this._tickCounter);

						if (GlobalPvPTimer != null)
							GlobalPvPTimer.Update(this._tickCounter);

						if (LockdownTimer != null)
							LockdownTimer.Update(this._tickCounter);

						if (QuestTimer != null)
							QuestTimer.Update(this._tickCounter);
					}
				}
				_tickWatch.Stop();

				Stopwatch _calcWatch = new Stopwatch();
				_calcWatch.Restart();
				var compensation = (int)(this.UpdateFrequency - _tickWatch.ElapsedMilliseconds); // the compensation value we need to sleep in order to get consistent 100 ms Game.Update().

				if (_tickWatch.ElapsedMilliseconds > this.UpdateFrequency)
				{
					Logger.Trace("Game.Update() took [{0}ms] more than Game.UpdateFrequency [{1}ms].", _tickWatch.ElapsedMilliseconds, this.UpdateFrequency);
					compensation = (int)(this.UpdateFrequency - (_tickWatch.ElapsedMilliseconds % this.UpdateFrequency));
					this.MissedTicks = this.TickRate * (int)(_tickWatch.ElapsedMilliseconds / this.UpdateFrequency);
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
			this.UpdateEnabled = false;
			try
			{
				switch (message.Consumer)
				{
					case Consumers.Game:
						this.Consume(client, message);
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
				this.UpdateEnabled = true;
			}
		}

		public void Consume(GameClient client, GameMessage message)
		{
			lock (this.updateLock)
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
			this.Difficulty--;
			SetDifficulty(Difficulty);
			foreach (var plr in this.Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		public void RaiseDifficulty(GameClient client, RaiseGameDifficulty message)
		{
			this.Difficulty++;
			SetDifficulty(Difficulty);
			foreach (var plr in this.Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		/// <summary>
		/// Allows a player to join the game.
		/// </summary>
		/// <param name="joinedPlayer">The new player.</param>
		public void Enter(Player joinedPlayer)
		{
			if (this.IsHardcore && !joinedPlayer.Toon.DBToon.isHardcore)
			{
				return;
			}

			Task.Run(() =>
			{
				lock (this.Players)
				{
					this.Players.TryAdd(joinedPlayer.InGameClient, joinedPlayer);

					// send all players in the game to new player that just joined (including him)
					foreach (var pair in this.Players)
					{
						if (pair.Value.PlayerGroupIndex == joinedPlayer.PlayerGroupIndex)
							this.SendNewPlayerMessage(joinedPlayer, pair.Value);
					}
					foreach (var pair in this.Players.Where(pair => pair.Value != joinedPlayer))
					{
						if (pair.Value.PlayerGroupIndex == joinedPlayer.PlayerGroupIndex)
							this.SendNewPlayerMessage(pair.Value, joinedPlayer);
					}
					joinedPlayer.LoadShownTutorials();
					joinedPlayer.LoadCrafterData();
					joinedPlayer.LoadCurrencyData();
					//joinedPlayer.LoadMailData();
					joinedPlayer.LoadStashIconsData();

					if (!this.PvP)
					{
						joinedPlayer.InGameClient.TickingEnabled = true; // it seems bnet-servers only start ticking after player is completely in-game. /raist
						joinedPlayer.InGameClient.SendMessage(new GameSyncedDataMessage
						{
							SyncedData = new GameSyncedData
							{
								GameSyncedFlags = 6,
								Act = this.CurrentAct,       //act id
								InitialMonsterLevel = this.InitialMonsterLevel, //InitialMonsterLevel
								MonsterLevel = 0x6FEA8DF5, //MonsterLevel
								RandomWeatherSeed = 0, //RandomWeatherSeed
								OpenWorldMode = this.CurrentAct == 3000 ? -1 : 0, //OpenWorldMode
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
						if ((this.CurrentStep == -1 || this.CurrentAct == 400) && (this.CurrentQuest == this.QuestsOrder[0]) && this.CurrentAct != 3000)
						{
							switch (this.CurrentAct)
							{
								case 0:
									joinedPlayer.EnterWorld(this.StartingWorld.GetStartingPointById(0).Position);
									break;
								case 100:
									joinedPlayer.EnterWorld(this.StartingWorld.GetStartingPointById(130).Position);
									break;
								case 200:
									joinedPlayer.ChangeWorld(this.GetWorld(WorldSno.a3dun_hub_adria_tower_intro), this.GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetStartingPointById(206).Position);
									break;
								case 300:
									joinedPlayer.ChangeWorld(this.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance), this.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).StartingPoints.First().Position);
									break;
								case 400:
									joinedPlayer.ChangeWorld(this.GetWorld(WorldSno.x1_westmarch_overlook_d), this.GetWorld(WorldSno.x1_westmarch_overlook_d).StartingPoints.First().Position);
									break;
								default:
									break;
							}
							joinedPlayer.PlayCutscene(0);
						}
						else
						{
							joinedPlayer.EnterWorld(this.StartPosition);
						}
					}
					else
					{
						joinedPlayer.EnterWorld(this.StartingWorld.GetStartingPointById(288 + joinedPlayer.PlayerIndex).Position);
					}
					this.Empty = false;

					foreach (var portal in this.StartingWorld.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal, ActorSno._x1_openworld_tiered_rifts_portal, ActorSno._x1_openworld_tiered_rifts_challenge_portal))
					{
						portal.Destroy();
					}

					ClientSystem.GameServer.GSBackend.PlayerJoined(this.GameId);


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
						foreach (var plr in this.Players.Values)
							if (plr.ActiveHireling != null)
							{
								plr.ActiveHireling.Dismiss();
								plr.ActiveHireling = null;
							}
					}
					/**/
					
					if (!this.PvP && !((this.CurrentStep == -1) && (this.CurrentQuest == this.QuestsOrder[0])))
					{
						joinedPlayer.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = this.CurrentQuest,
							snoLevelArea = -1,
							StepID = this.CurrentStep,
							DisplayButton = true,
							Failed = false
						});
					}

					
					if (joinedPlayer.PlayerIndex == 0)
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage)
						{
							Difficulty = (uint)this.Difficulty
						});

					

					this.UpdateLevel();
					joinedPlayer.NotifyMaintenance();

					if (this.CurrentAct == 3000 && !joinedPlayer.InGameClient.OpenWorldDefined)
					{
						joinedPlayer.InGameClient.OpenWorldDefined = true;
						joinedPlayer.InGameClient.SendMessage(new ActTransitionMessage
						{
							Act = 3000,
							OnJoin = true
						});

						foreach (var bounty in this.QuestManager.Bounties)
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
						this.CurrentQuest = 0x0004C46D;
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

						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Quest.QuestUpdateMessage
						{
							snoQuest = 0x0005727C,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Quest.QuestUpdateMessage
						{
							snoQuest = 0x00057282,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Quest.QuestUpdateMessage
						{
							snoQuest = 0x00057284,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Quest.QuestUpdateMessage
						{
							snoQuest = 0x00057287,
							snoLevelArea = -1,
							StepID = -1,
							DisplayButton = true,
							Failed = false
						});
						joinedPlayer.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Quest.QuestUpdateMessage
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
			if (this.Players.Count < 1)
				return;
			this.MonsterLevel = this.Players.Values.Select(p => p.Level).Max();
			foreach (var wld in this._worlds)
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

			if (this.PvP) return;
			if (!QuestSetuped)
			{
				this.QuestManager.SetQuests();
				this.DestinationEnterQuest = currQuest;
				this.DestinationEnterQuestStep = step;

				Logger.Trace("SetQuestProgress: quest {0}, step {1}", currQuest, step);
				this.CurrentQuest = this.QuestsOrder[0];
				this.CurrentStep = -1;

				if (this.CurrentAct == 3000)
				{
					this.QuestManager.Quests[this.CurrentQuest].Steps[-1].OnAdvance.Invoke();
					return;
				}

				if (!(currQuest == this.QuestsOrder[0] && step == -1))
					this.QuestManager.AdvanceTo(currQuest, step);

				if (this.CurrentQuest == this.QuestsOrder[0] && this.CurrentStep == -1)
					this.QuestManager.Quests[this.CurrentQuest].Steps[-1].OnAdvance.Invoke();
			}

		}
		public void SetGameMode(Mode mode)
		{
			this.GameMode = mode;

			switch (GameMode)
			{
				case Game.Mode.Portals:
					this.QuestsOrder = new int[] { -1 };
					this.StartingWorldSNO = WorldSno.weekly_challenge_hub;
					this.QuestProgress = new QuestRegistry(this);
					break;
			}
		}
		public void SetAct(int act)
		{
			if (this.PvP)
			{
				this.CurrentAct = 0;
				this.QuestsOrder = questsOrder_a1;
				this.QuestProgress = new QuestRegistry(this);
				this.StartingWorldSNO = WorldSno.pvp_caout_arena_01;
				return;
			}
			if (CurrentAct != act)
			{
				this.CurrentAct = act;

				switch (act)
				{
					case 0:
						this.QuestsOrder = questsOrder_a1;
						this.StartingWorldSNO = WorldSno.trout_town;
						this.QuestProgress = new ActI(this);
						break;
					case 100:
						this.QuestsOrder = questsOrder_a2;
						this.StartingWorldSNO = WorldSno.caout_refugeecamp;
						this.QuestProgress = new ActII(this);
						break;
					case 200:
						this.QuestsOrder = questsOrder_a3;
						this.StartingWorldSNO = WorldSno.a3dun_hub_keep;
						this.QuestProgress = new ActIII(this);
						break;
					case 300:
						this.QuestsOrder = questsOrder_a4;
						this.StartingWorldSNO = WorldSno.a4dun_heaven_hub_keep;
						this.QuestProgress = new ActIV(this);
						break;
					case 400:
						this.QuestsOrder = questsOrder_a5;
						this.StartingWorldSNO = WorldSno.x1_westmarch_hub;
						this.QuestProgress = new ActV(this);
						break;
					case 3000:
						this.QuestsOrder = questsOrder_openWorld;
						this.StartingWorldSNO = WorldSno.x1_tristram_adventure_mode_hub;
						this.QuestProgress = new OpenWorld(this);
						this.QuestManager.SetBounties();
						break;
					default:
						this.QuestsOrder = questsOrder_a1;
						this.StartingWorldSNO = WorldSno.trout_town;
						this.QuestProgress = new QuestRegistry(this);
						break;
				}
			}
		}

		public void ChangeAct(int act)
		{
			foreach(var plr in this.Players.Values)
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
			this.SetAct(act);
			this.CurrentQuest = this.QuestsOrder[0];
			this.CurrentStep = -1;
			this.QuestManager.ReloadQuests();
			this.OpenedWaypoints = new List<int>() { };
			foreach (var plr in this.Players)
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
						GameSyncedFlags = this.IsSeasoned == true ? this.IsHardcore == true ? 6 : 6 : this.IsHardcore == true ? 4 : 4,
						Act = Math.Min(this.CurrentAct, 3000),       //act id
						InitialMonsterLevel = this.InitialMonsterLevel, //InitialMonsterLevel
						MonsterLevel = 0x7044248F, //MonsterLevel
						RandomWeatherSeed = 0, //RandomWeatherSeed
						OpenWorldMode = this.CurrentAct == 3000 ? -1 : 0, //OpenWorldMode
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
						plr.Value.ChangeWorld(this.StartingWorld, this.StartingWorld.GetStartingPointById(0).Position);
						break;
					case 100:
						plr.Value.ChangeWorld(this.StartingWorld, this.StartingWorld.GetStartingPointById(130).Position);
						break;
					case 200:
						plr.Value.ChangeWorld(this.GetWorld(WorldSno.a3dun_hub_adria_tower_intro), this.GetWorld(WorldSno.a3dun_hub_adria_tower_intro).GetStartingPointById(206).Position);
						break;
					case 300:
						plr.Value.ChangeWorld(this.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance), this.GetWorld(WorldSno.a4dun_heaven_1000_monsters_fight_entrance).StartingPoints.First().Position);
						break;
					case 400:
						plr.Value.ChangeWorld(this.GetWorld(WorldSno.x1_westmarch_overlook_d), this.GetWorld(WorldSno.x1_westmarch_overlook_d).StartingPoints.First().Position);
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
				this.QuestManager.Quests[this.QuestsOrder[0]].Steps[-1].OnAdvance.Invoke();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "onAdvance():");
			}
		}

		public void UpdateLevel(int level)
		{
			this.MonsterLevel = level;
			foreach (var wld in this._worlds)
				foreach (var monster in wld.Value.Monsters)
					monster.UpdateStats();
		}

		public void SetDifficulty(int diff)
		{
			this.Difficulty = diff;
			diff++;
			if (diff > 0)
			{
				var handicapLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][256027].Data;
				this.HPModifier = handicapLevels.HandicapLevelTables[diff].HPMod;
				this.DmgModifier = handicapLevels.HandicapLevelTables[diff].DmgMod;
				this.XPModifier = (1f + handicapLevels.HandicapLevelTables[diff].XPMod);
				this.GoldModifier = (1f + handicapLevels.HandicapLevelTables[diff].GoldMod);
			}
			foreach (var wld in this._worlds)
				foreach (var monster in wld.Value.Monsters)
					monster.UpdateStats();
			foreach(var plr in this.Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.HandicapMessage(Opcodes.HandicapMessage) { Difficulty = (uint)Difficulty });
		}

		public void UnlockTeleport(int waypointId)
		{
			this.OpenedWaypoints.Add(waypointId);
		}

		public Actor GetHearthPortal()
		{
			return this.StartingWorld.Actors.Values.Where(x => x.SNO == ActorSno._hearthportal).First();
		}

		private void OnPause(GameClient client, PauseGameMessage message)
		{
			if (this.Players.Count == 1)
			{
				Logger.Trace("Game state is paused: {0}", message.Field0);
				this.Players.First().Value.Attributes[GameAttribute.Disabled] = message.Field0;
				this.Players.First().Value.Attributes[GameAttribute.Immobolize] = message.Field0;
				//this.Players.First().Value.Attributes[GameAttribute.Stunned] = message.Field0;
				this.Players.First().Value.Attributes.BroadcastChangedIfRevealed();
				//this.Players.First().Key.TickingEnabled = !message.Field0;
				this.Paused = message.Field0;
				this.Players.First().Key.SendMessage(new FreezeGameMessage
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
				AltLevel = (byte)joinedPlayer.Toon.ParagonLevel,
				HighestHeroSoloRiftLevel = 0,
				StateData = joinedPlayer.GetStateData(),
				JustJoined = false,
				Field9 = 0x77EA0000,
				ActorID = joinedPlayer.DynamicID(target),
			});

			if (this.PvP)
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
			lock (this.Players)
			{
				foreach (var plr in this.Players.Keys)
					plr.SendMessage(new BroadcastTextMessage() { Field0 = message });
			}
		}

		public void StartPvPRound()
		{
			this.CurrentPvPRound++;

			var winner = this.Players.Values.Where(p => !p.Dead).FirstOrDefault();
			if (winner != null && this.CurrentPvPRound > 1)
			{
				this.BroadcastMessage("Round is over! Winner: " + winner.Toon.Name);
				if (winner.Attributes[GameAttribute.TeamID] == 2)
					this.RedTeamWins++;
				else
					this.BlueTeamWins++;
			}

			if (this.CurrentPvPRound > 5 || Math.Abs(this.RedTeamWins - this.BlueTeamWins) > (5 - this.CurrentPvPRound))
			{
				this.BroadcastMessage("Battle is over!");
				try
				{
					var totalWinner = this.Players.Values.Where(p => p.Attributes[GameAttribute.TeamID] == (RedTeamWins > BlueTeamWins ? 2 : 3)).FirstOrDefault();
					this.BroadcastMessage("Winner: " + totalWinner.Toon.Name);
				}
				catch { Logger.Warn("Exception on FindWinner()"); }

				//foreach (var player in this.Players.Values)
				//player.World.BuffManager.AddBuff(player, player, new Mooege.Core.GS.Powers.Implementations.PVPRoundEndBuff(TickTimer.WaitSeconds(this, 1200.0f)));
				foreach (var plr in this.Players.Keys)
					plr.SendMessage(new DataIDDataMessage(Opcodes.PVPArenaWin) { Field0 = (RedTeamWins == BlueTeamWins ? 0 : (RedTeamWins < BlueTeamWins ? 2 : 3)) });
				return;
			}

			if (this.CurrentPvPRound == 1)
			{
				this.GlobalPvPTimer = TickTimer.WaitSeconds(this, 600f, new Action<int>((z) =>
				{
					this.BroadcastMessage("Time is up, battle is over!");
					if (RedTeamWins == BlueTeamWins)
					{
						this.BroadcastMessage("Draw!");

					}
					else
					{
						var TotalWinner = this.Players.Values.Where(p => p.Attributes[GameAttribute.TeamID] == (RedTeamWins > BlueTeamWins ? 2 : 3)).FirstOrDefault();
						this.BroadcastMessage("Winner: " + TotalWinner.Toon.Name);
					}

					//foreach (var player in this.Players.Values)
					//player.World.BuffManager.AddBuff(player, player, new Mooege.Core.GS.Powers.Implementations.PVPRoundEndBuff(TickTimer.WaitSeconds(this, 1200.0f)));
					foreach (var plr in this.Players.Keys)
						plr.SendMessage(new DataIDDataMessage(Opcodes.PVPArenaWin) { Field0 = (RedTeamWins == BlueTeamWins ? 0 : (RedTeamWins < BlueTeamWins ? 2 : 3)) });
				}));
			}


			this.PvPTimer = TickTimer.WaitSeconds(this, 3f, new Action<int>((x) =>
			{
				foreach (var player in this.Players.Values)
				{
					player.Revive(player.CheckPointPosition);
					player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total, player.Attributes[GameAttribute.Resource_Type_Primary]]);
					player.World.BuffManager.AddBuff(player, player, new PowerSystem.Implementations.PVPSkirmishBuff(TickTimer.WaitSeconds(this, 15.0f)));
				}
				this.BroadcastMessage("Round " + this.CurrentPvPRound + ". Battle will commence in 15 seconds!");
				this.BroadcastMessage("Score: " + this.RedTeamWins + ":" + this.BlueTeamWins);
				this.PvPTimer = TickTimer.WaitSeconds(this, 15f, new Action<int>((y) =>
				{
					this.BroadcastMessage("Fight!");
					foreach (var player in this.Players.Keys)
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
			if (this.Players.Count > 0)
				foreach (var plr in this.Players.Keys)
					plr.Connection.Disconnect();
			this._worlds.Clear();
			Thread.Sleep(1000);
			this.GameDBSession.SessionDispose();
			GameManager.Games.Remove(this.GameId);
		}

		public void TeleportToBossEncounter(int snoId)
		{
			foreach (var player in this.Players.Values)
			{
				player.ClearDoorAnimations();
			}
			this.Paused = true;

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
			World encWorld = this.GetWorld((WorldSno)encAsset.Worlds[0]);
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

			foreach (var g_player in this.Players)
			{
				if (g_player.Value.World == encWorld)
					g_player.Value.Teleport(startPoint);
				else
					g_player.Value.ChangeWorld(encWorld, startPoint);
			}
			
			
			this.Paused = false;
			
			
			//handling quest triggers
			if (this.QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
			{
				var trigger = this.QuestProgress.QuestTriggers[levelArea];
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
			if (this.GameMode == Mode.Campaign)
				switch (snoId)
				{
					case 168925: //CainIntro
								 //if (this.CurrentAct == 0)
						Task.Delay(1000).ContinueWith(delegate
						{
							foreach (var plr in this.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });

							Task.Delay(1000).ContinueWith(delegate
							{
								foreach (var plr in this.Players.Values)
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
										foreach (var plr in this.Players.Values) Cain.Unreveal(plr);
										CainQuest = Cain;
									}
									else
									{
										Cain.SetVisible(true);
										foreach (var plr in this.Players.Values) Cain.Reveal(plr);
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
										foreach (var plr in this.Players.Values)
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
													bridge.PlayAnimation(5, bridge.AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault]);
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
													Leoric.PlayActionAnimation(668);
													Task.Delay(1000).ContinueWith(delegate
													{
														foreach (var plr in this.Players.Values)
															plr.Conversations.StartConversation(17692); //Фраза Леорика
													Task.Delay(14000).ContinueWith(delegate
															{
															//Leoric.PlayActionAnimation(9854); //Леорик призывает скелетов

															Leoric.PlayActionAnimation(9848); //Себаса
															Task.Delay(1000).ContinueWith(delegate
																	{
																		foreach (var plr in this.Players.Values)
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
							foreach (var plr in this.Players.Values)
								plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });

							Task.Delay(1000).ContinueWith(delegate
							{
								if (Butcher != null)
									(Butcher as Monster).Brain.DeActivate();
								foreach (var plr in this.Players.Values)
									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)Butcher.DynamicID(plr), Duration = 1f, Snap = false });


								foreach (var plr in this.Players.Values)
									plr.Conversations.StartConversation(211980); //ФРЭШ МИТ
																				 //	StartConversation(ButcherLair, 211980);
								Task.Delay(3000).ContinueWith(delegate
									{
										foreach (var plr in this.Players.Values)
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

			foreach (var bounty in this.QuestManager.Bounties)
				bounty.CheckLevelArea(levelArea);

			this.CurrentEncounter.acceptedPlayers = 0;
			this.CurrentEncounter.activated = false;
		}

		public void AcceptBossEncounter()
		{
			this.CurrentEncounter.acceptedPlayers++;
			if (this.CurrentEncounter.acceptedPlayers >= this.Players.Count)
				TeleportToBossEncounter(this.CurrentEncounter.SnoId);
		}

		public void DeclineBossEncounter()
		{
			this.CurrentEncounter.activated = false;
			this.CurrentEncounter.acceptedPlayers = 0;
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
				if (!this.OnLoadWorldActions.ContainsKey(worldSNO))
					this.OnLoadWorldActions.Add(worldSNO, new List<Action>());

				this.OnLoadWorldActions[worldSNO].Add(action);
			}
		}

		public void AddOnLoadSceneAction(int sceneSNO, Action action)
        {
			Logger.Trace("AddOnLoadSceneAction: {0}", sceneSNO);
			if (!this.OnLoadSceneActions.ContainsKey(sceneSNO))
					this.OnLoadSceneActions.Add(sceneSNO, new List<Action>());

				this.OnLoadSceneActions[sceneSNO].Add(action);
		}

#endregion

		#region world collection

		public void AddWorld(World world)
		{
			if (world.SNO == WorldSno.__NONE || WorldExists(world.SNO))
				Logger.Error(String.Format("World has an invalid SNO or was already being tracked (ID = {0}, SNO = {1})", world.GlobalID, world.SNO));
			else
				this._worlds.TryAdd(world.SNO, world);
		}

		public void RemoveWorld(World world)
		{
			World removed;
			if (world.SNO == WorldSno.__NONE || !WorldExists(world.SNO))
				Logger.Error(String.Format("World has an invalid SNO or was not being tracked (ID = {0}, SNO = {1})", world.GlobalID, world.SNO));
			else
				this._worlds.TryRemove(world.SNO, out removed);
		}

		public World GetWorld(WorldSno worldSNO)
		{
			if (worldSNO == WorldSno.__NONE)
				return null;

			World world;

			if (this.CurrentAct != 3000 && worldSNO == WorldSno.x1_tristram_adventure_mode_hub) //fix for a1 Tristram
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
				world = this.WorldGenerator.Generate(worldSNO);
				if (world == null) Logger.Warn("Failed to generate world with sno: {0}", worldSNO);
			}
			this._worlds.TryGetValue(worldSNO, out world);
			return world;
		}

		public bool WorldExists(WorldSno worldSNO)
		{
			return this._worlds.ContainsKey(worldSNO);
		}

		public bool WorldCleared(WorldSno worldSNO)
		{
			return this._worlds[worldSNO].Actors.Values.OfType<Monster>().Where(m => m.OriginalLevelArea != -1 && !m.Dead).Count() < 5;
		}

		/// <summary>
		/// Returns World with given Waypoint id.
		/// </summary>
		/// <param name="id">The id of the WayPoint</param>
		/// <returns><see cref="World"/></returns>
		public World GetWayPointWorldById(int id)
		{
			bool isOpenWorld = this.CurrentAct == 3000;
			var actData = ((DiIiS_NA.Core.MPQ.FileFormats.Act)MPQStorage.Data.Assets[SNOGroup.Act][this.CurrentActSNOid].Data).WayPointInfo.ToList();
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
