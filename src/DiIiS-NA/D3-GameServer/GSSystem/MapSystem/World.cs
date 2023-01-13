//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.QuadTrees;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.MapSystem
{
	public sealed class World : DynamicObject, IRevealable, IUpdateable
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		public readonly Dictionary<World, List<Item>> DbItems = new Dictionary<World, List<Item>>(); //we need this list to delete item_instances from items which have no owner anymore.
		public readonly Dictionary<ulong, Item> CachedItems = new Dictionary<ulong, Item>();

		public int LastCEId = 3000;


		/// <summary>
		/// Game that the world belongs to.
		/// </summary>
		public Game Game { get; private set; }

		public bool DRLGEmuActive = false;
		/// <summary>
		/// SNOHandle for the world.
		/// </summary>
		public SNOHandle WorldSNO { get; private set; }

		/// <summary>
		/// QuadTree that contains scenes & actors.
		/// </summary>
		private QuadTree _quadTree;
		public static QuadTree _PvPQuadTree = new QuadTree(new Size(60, 60), 0);

		public QuadTree QuadTree
		{
			get
			{
				return (this.IsPvP ? _PvPQuadTree : _quadTree);
			}
			set { }
		}

		/// <summary>
		/// WorldData loaded from MPQs/DB
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.World worldData = new DiIiS_NA.Core.MPQ.FileFormats.World();

		/// <summary>
		/// Destination for portals(on Exit and DungeonStone)
		/// </summary>
		public ResolvedPortalDestination NextLocation;

		/// <summary>
		/// Destination for portals(on Entrance)
		/// </summary>
		public ResolvedPortalDestination PrevLocation;

		/// <summary>
		/// List of scenes contained in the world.
		/// </summary>
		private readonly ConcurrentDictionary<uint, Scene> _scenes;

		private static readonly ConcurrentDictionary<uint, Scene> _PvPscenes = new ConcurrentDictionary<uint, Scene>();

		public ConcurrentDictionary<uint, Scene> Scenes
		{
			get
			{
				return (this.IsPvP ? _PvPscenes : _scenes);
			}
			set { }
		}

		/// <summary>
		/// List of actors contained in the world.
		/// </summary>
		private readonly ConcurrentDictionary<uint, Actor> _actors;

		public static readonly ConcurrentDictionary<uint, Actor> _PvPActors = new ConcurrentDictionary<uint, Actor>();

		public ConcurrentDictionary<uint, Actor> Actors
		{
			get
			{
				return (this.IsPvP ? _PvPActors : _actors);
			}
			set { }
		}

		public Dictionary<int, int> PortalOverrides = new Dictionary<int, int>();

		/// <summary>
		/// List of players contained in the world.
		/// </summary>
		private readonly ConcurrentDictionary<uint, Player> _players;

		public static readonly ConcurrentDictionary<uint, Player> _PvPPlayers = new ConcurrentDictionary<uint, Player>();

		public ConcurrentDictionary<uint, Player> Players
		{
			get
			{
				return (this.IsPvP ? _PvPPlayers : _players);
			}
			set { }
		}

		/// <summary>
		/// Returns true if the world has players in.
		/// </summary>
		public bool HasPlayersIn { get { return this.Players.Count > 0; } }

		/// <summary>
		/// Returns a new dynamicId for scenes.
		/// </summary>
		public uint NewSceneID { get { return this.IsPvP ? World.NewPvPSceneID : this.Game.NewSceneID; } }

		public bool IsPvP { get { return this.WorldSNO.Id == 279626; } } //PvP_Duel_Small

		public static bool PvPMapLoaded = false;

		public Scene GetSceneBySnoId(int SnoID)
		{
			Scene scene = null;
			foreach (var _scene in _scenes.Values)
			{
				if (_scene.SceneSNO.Id == SnoID)
					scene = _scene;
			}

			return scene;
		}

		// Environment
		public DiIiS_NA.Core.MPQ.FileFormats.Environment Environment
		{
			get
			{
				return ((DiIiS_NA.Core.MPQ.FileFormats.World)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Worlds][this.WorldSNO.Id].Data).Environment;
			}
		}

		private static uint _lastPvPObjectID = 10001;
		private static object obj = new object();
		public static uint NewActorPvPID
		{
			get
			{
				lock (obj)
				{
					_lastPvPObjectID++;
					return _lastPvPObjectID;
				}
			}
		}

		private static uint _lastPvPSceneID = 0x06000000;
		public static uint NewPvPSceneID
		{
			get
			{
				lock (obj)
				{
					_lastPvPSceneID++;
					return _lastPvPSceneID;
				}
			}
		}

		/// <summary>
		/// Returns list of available starting points.
		/// </summary>
		public List<StartingPoint> StartingPoints
		{
			get { return this.Actors.Values.OfType<StartingPoint>().Select(actor => actor).ToList(); }
		}

		public List<Portal> Portals
		{
			get { return this.Actors.Values.OfType<Portal>().Select(actor => actor).ToList(); }
		}

		public List<Monster> Monsters
		{
			get { return this.Actors.Values.OfType<Monster>().Select(actor => actor).ToList(); }
		}

		private PowerManager _powerManager;
		public static PowerManager _PvPPowerManager = new PowerManager();

		public PowerManager PowerManager { get { return this.IsPvP ? World._PvPPowerManager : this._powerManager; } }

		private BuffManager _buffManager;
		public static BuffManager _PvPBuffManager = new BuffManager();

		public BuffManager BuffManager { get { return this.IsPvP ? World._PvPBuffManager : this._buffManager; } }

		/// <summary>
		/// Creates a new world for the given game with given snoId.
		/// </summary>
		/// <param name="game">The parent game.</param>
		/// <param name="snoId">The snoId for the world.</param>
		public World(Game game, int snoId)
			: base(snoId == 279626 ? 99999 : game.NewWorldID)
		{
			this.WorldSNO = new SNOHandle(SNOGroup.Worlds, snoId);

			this.Game = game;

			this._scenes = new ConcurrentDictionary<uint, Scene>();
			this._actors = new ConcurrentDictionary<uint, Actor>();
			this._players = new ConcurrentDictionary<uint, Player>();
			this._quadTree = new QuadTree(new Size(60, 60), 0);
			this.NextLocation = this.PrevLocation = new ResolvedPortalDestination
			{
				WorldSNO = -1,
				DestLevelAreaSNO = -1,
				StartingPointActorTag = -1
			};
			this._powerManager = new PowerManager();
			this._buffManager = new BuffManager();

			this.Game.AddWorld(this);
			//this.Game.StartTracking(this); // start tracking the dynamicId for the world.

			if (this.WorldSNO.Id == 267412) //Blood Marsh
			{
				var worlds = new List<int>() { 283552, 341037, 341038, 341040 };
				var scenes = new List<int>() { 265624, 265655, 265678, 265693 };
				foreach (var scene in scenes)
				{
					var wld = worlds[FastRandom.Instance.Next(worlds.Count())];
					this.PortalOverrides.Add(scene, wld);
					worlds.Remove(wld);
				}
			}
		}

		#region update & tick logic

		public void Update(int tickCounter)
		{
			foreach (var player in this.Players.Values)
			{
				player.InGameClient.SendTick(); // if there's available messages to send, will handle ticking and flush the outgoing buffer.
			}

			var actorsToUpdate = new List<IUpdateable>(); // list of actor to update.

			foreach (var player in this.Players.Values) // get players in the world.
			{
				foreach (var actor in player.GetActorsInRange().OfType<IUpdateable>()) // get IUpdateable actors in range.
				{
					if (actorsToUpdate.Contains(actor as IUpdateable)) // don't let a single actor in range of more than players to get updated more thance per tick /raist.
						continue;

					actorsToUpdate.Add(actor as IUpdateable);
				}
			}
			foreach (var minion in this.Actors.Values.OfType<Minion>())
			{
				if (actorsToUpdate.Contains(minion as IUpdateable))
					continue;
				actorsToUpdate.Add(minion as IUpdateable);
			}
			foreach (var actor in actorsToUpdate) // trigger the updates.
			{
				actor.Update(tickCounter);
			}

			this.BuffManager.Update();
			this.PowerManager.Update();

			if (tickCounter % 6 == 0 && this._flippyTimers.Count() > 0)
			{
				UpdateFlippy(tickCounter);
			}
		}

		#endregion

		#region message broadcasting

		// NOTE: Scenes are actually laid out in cells with Subscenes filling in certain areas under a Scene.
		// We can use this design feature to track Actors' current scene and send updates to it and neighboring
		// scenes instead of distance checking for broadcasting messages. / komiga
		// I'll be soon adding that feature /raist.

		/// <summary>
		/// Broadcasts a message for a given actor to only players that actor has been revealed.
		/// </summary>
		/// <param name="message">The message to broadcast.</param>
		/// <param name="actor">The actor.</param>
		public void BroadcastIfRevealed(Func<Player, GameMessage> message, Actor actor)
		{
			foreach (var player in this.Players.Values)
			{
				if (player.RevealedObjects.ContainsKey(actor.GlobalID))
					player.InGameClient.SendMessage(message(player));
			}
		}

		/// <summary>
		/// Broadcasts a message to all players in the world.
		/// </summary>
		/// <param name="message"></param>
		public void BroadcastGlobal(Func<Player, GameMessage> message)
		{
			foreach (var player in this.Players.Values)
			{
				player.InGameClient.SendMessage(message(player));
			}
		}

		/// <summary>
		/// Broadcasts a message to all players in the range of given actor.
		/// </summary>
		/// <param name="message">The message to broadcast.</param>
		/// <param name="actor">The actor.</param>
		public void BroadcastInclusive(Func<Player, GameMessage> message, Actor actor)
		{
			var players = actor.GetPlayersInRange();
			foreach (var player in players)
			{
				player.InGameClient.SendMessage(message(player));
			}
		}

		/// <summary>
		/// Broadcasts a message to all players in the range of given actor, but not the player itself if actor is the player.
		/// </summary>
		/// <param name="message">The message to broadcast.</param>
		/// <param name="actor">The actor.</param>
		public void BroadcastExclusive(Func<Player, GameMessage> message, Actor actor, bool global = false)
		{
			var players = actor.GetPlayersInRange();
			if (global) players = this.Players.Values.ToList();
			foreach (var player in players.Where(player => player != actor))
			{
				if (player.RevealedObjects.ContainsKey(actor.GlobalID)) //revealed only!
					player.InGameClient.SendMessage(message(player));
			}
		}

		#endregion

		#region reveal logic

		/// <summary>
		/// Reveals the world to given player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public bool Reveal(Player player)
		{
			lock (player.RevealedObjects)
			{
				if (player.RevealedObjects.ContainsKey(this.GlobalID))
					return false;

				int sceneGridSize = this.WorldSNO.Name.ToLower().Contains("zolt") ? 100 : 60;
				
				player.InGameClient.SendMessage(new RevealWorldMessage() // Reveal world to player
				{
					WorldID = this.GlobalID,
					WorldSNO = this.WorldSNO.Id,
					OriginX = 540,
					OriginY = -600,
					StitchSizeInFeetX = sceneGridSize,
					StitchSizeInFeetY = sceneGridSize,
					WorldSizeInFeetX = 5040,
					WorldSizeInFeetY = 5040,
					snoDungeonFinderSourceWorld = -1
				});
				player.InGameClient.SendMessage(new WorldStatusMessage() { WorldID = this.GlobalID, Field1 = false });
				//*
				player.InGameClient.SendMessage(new WorldSyncedDataMessage()
				{
					WorldID = this.GlobalID,
					SyncedData = new WorldSyncedData()
					{
						SnoWeatherOverride = -1,
						WeatherIntensityOverride = 0,
						WeatherIntensityOverrideEnd = 0
					}
				});

				//*/
				player.RevealedObjects.Add(this.GlobalID, this.GlobalID);

				return true;
			}
		}

		/// <summary>
		/// Unreveals the world to player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public bool Unreveal(Player player)
		{
			if (!player.RevealedObjects.ContainsKey(this.GlobalID)) return false;

			foreach (var scene in this.Scenes.Values) scene.Unreveal(player);
			player.RevealedObjects.Remove(this.GlobalID);

			player.InGameClient.SendMessage(new WorldStatusMessage() { WorldID = this.GlobalID, Field1 = true });
			player.InGameClient.SendMessage(new PrefetchDataMessage(Opcodes.PrefetchWorldMessage) { SNO = this.WorldSNO.Id });
			//player.InGameClient.SendMessage(new WorldDeletedMessage() { WorldID = this.GlobalID });

			return true;
		}

		#endregion

		#region actor enter & leave functionality

		/// <summary>
		/// Allows an actor to enter the world.
		/// </summary>
		/// <param name="actor">The actor entering the world.</param>
		public void Enter(Actor actor)
		{
			this.AddActor(actor);
			actor.OnEnter(this);

			// reveal actor to player's in-range.
			foreach (var player in actor.GetPlayersInRange())
			{
				actor.Reveal(player);
			}

			//Убираем балки с проходов
			if (this.WorldSNO.Id == 71150)
			{
				foreach (var boarded in this.GetActorsBySNO(111888))
					boarded.Destroy();
				foreach (var boarded in this.GetActorsBySNO(111856))
					boarded.Destroy();
			}
		}

		/// <summary>
		/// Allows an actor to leave the world.
		/// </summary>
		/// <param name="actor">The actor leaving the world.</param>
		public void Leave(Actor actor)
		{
			actor.OnLeave(this);

			foreach (var player in this.Players.Values)
			{
				actor.Unreveal(player);
			}

			if (this.HasActor(actor.GlobalID))
				this.RemoveActor(actor);

			if (!(actor is Player)) return; // if the leaving actors is a player, unreveal the actors revealed to him contained in the world.

			var revealedObjects = (actor as Player).RevealedObjects.Keys.ToList(); // list of revealed actors.
			foreach (var obj_id in revealedObjects)
			{
				var obj = this.GetActorByGlobalId(obj_id);
				//if (obj != actor) // do not unreveal the player itself.
				try
				{
					if (obj != null)
						obj.Unreveal(actor as Player);
					//System.Threading.Tasks.Task.Delay(5).Wait();
				}
				catch { }
			}
		}

		#endregion

		#region Отображение только конкретной итерации NPC
		public Actor ShowOnlyNumNPC(int SNO, int Num)
		{
			Actor Setted = null;
			var list = this.GetActorsBySNO(SNO);
			foreach (var actor in this.GetActorsBySNO(SNO))
			{
				if (actor.NumberInWorld == Num)
				{
					Setted = actor;
					actor.Hidden = false;
					actor.SetVisible(true);
					foreach (var plr in this.Players.Values)
						actor.Reveal(plr);
				}
				else
				{
					actor.Hidden = true;
					actor.SetVisible(false);
					foreach (var plr in this.Players.Values)
						actor.Unreveal(plr);
				}
			}
			return Setted;
		}

        #endregion

        #region monster spawning & item drops

        /// <summary>
        /// Spawns a monster with given SNOId in given position.
        /// </summary>
        /// <param name="monsterSNOId">The SNOId of the monster.</param>
        /// <param name="position">The position to spawn it.</param>
		
        public Actor SpawnMonster(int monsterSNOId, Vector3D position)
		{
			if (monsterSNOId != 1)
			{
				var monster = ActorFactory.Create(this, monsterSNOId, new TagMap());
				if (monster != null)
				{
					monster.EnterWorld(position);
					if (monster.AnimationSet != null)
					{
						if (monster.AnimationSet.TagMapAnimDefault.ContainsKey(70097))
							monster.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.Animation.PlayAnimationMessage
							{
								ActorID = monster.DynamicID(plr),
								AnimReason = 5,
								UnitAniimStartTime = 0,
								tAnim = new PlayAnimationMessageSpec[]
								{
								new PlayAnimationMessageSpec()
								{
									Duration = 150,
									AnimationSNO = monster.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Spawn],
									PermutationIndex = 0,
									Speed = 1
								}
								}

							}, monster);
						else if (monster.AnimationSet.TagMapAnimDefault.ContainsKey(291072))
							monster.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.Animation.PlayAnimationMessage
							{
								ActorID = monster.DynamicID(plr),
								AnimReason = 5,
								UnitAniimStartTime = 0,
								tAnim = new PlayAnimationMessageSpec[]
								{
								new PlayAnimationMessageSpec()
								{
									Duration = 150,
									AnimationSNO = monster.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Spawn2],
									PermutationIndex = 0,
									Speed = 1
								}
								}

							}, monster);
					}	
					return monster;
				}
			}
			return null;
		}

		private Queue<Queue<Action>> _flippyTimers = new Queue<Queue<Action>>();

		private const int FlippyDurationInTicks = 10;
		private const int FlippyMaxDistanceManhattan = 10;  // length of one side of the square around the player where the item will appear
		private const int FlippyDefaultFlippy = 0x6d82;  // g_flippy.prt

		public void SpawnItem(Actor source, Player player, int GBid)
		{
			var item = ItemGenerator.CookFromDefinition(player.World, ItemGenerator.GetItemDefinition(GBid));
			if (item == null) return;
			player.GroundItems[item.GlobalID] = item; // FIXME: Hacky. /komiga
			DropItem(source, null, item);
		}
		public void PlayPieAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathPieWedgeMessage
			{
				ann = (int)actor.DynamicID(plr),
				StartPos = User.Position,
				FirstTagetPos = User.Position,
				MoveFlags = 9,
				AnimTag = 1,
				PieData = new DPathPieData()
				{
					Field0 = TargetPosition,
					Field1 = 1,
					Field2 = 1,
					Field3 = 1
				},
				Field6 = 1f,

			}, actor);
		}
		public void PlayCircleAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSinMessage
			{
				ActorID = actor.DynamicID(plr),
				DPath = 6,
				// 0 - crashes client
				// 1 - random scuttle (charged bolt effect)
				// 2 - random movement, random movement pauses (toads hopping)
				// 3 - clockwise spiral
				// 4 - counter-clockwise spiral
				Seed = 1,
				Carry = 1,
				TargetPostition = TargetPosition,
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData()
				{
					annOwner = (int)actor.DynamicID(plr),
					SinIncAccel = 0f,
					LateralMaxDistance = 9f,
					LateralStartDistance = 1f,
					OOLateralDistanceToScale = 5f,
					SinIncPerTick = 1f,
					Speed = 0.5f
				},
				SpeedMulti = 0.1f,

			}, actor);
		}
		public void PlayZigAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSinMessage
			{
				ActorID = actor.DynamicID(plr),
				DPath = 5,
				Seed = 1,
				Carry = 1,
				TargetPostition = TargetPosition,
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData()
				{
					annOwner = (int)actor.DynamicID(plr),
					SinIncAccel = 0f,
					LateralMaxDistance = 9f,
					LateralStartDistance = 1f,
					OOLateralDistanceToScale = 5f,
					SinIncPerTick = 0.2f,
					Speed = 0.5f
				},
				SpeedMulti = 1f,

			}, actor);
		}
		public void PlayReverSpiralAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSinMessage
			{
				ActorID = actor.DynamicID(plr),
				DPath = 4,
				// 0 - crashes client
				// 1 - random scuttle (charged bolt effect)
				// 2 - random movement, random movement pauses (toads hopping)
				// 3 - clockwise spiral
				// 4 - counter-clockwise spiral
				Seed = 1,
				Carry = 1,
				TargetPostition = TargetPosition,
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData()
				{
					annOwner = (int)actor.DynamicID(plr),
					SinIncAccel = 0.2f,
					LateralMaxDistance = 0.1f,
					LateralStartDistance = 0.1f,
					OOLateralDistanceToScale = 0.1f,
					SinIncPerTick = 1f,
					Speed = 1f
				},
				SpeedMulti = 1f,

			}, actor);
		}
		public void PlaySpiralAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{
			
			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateDetPathSinMessage 
			{
				ActorID = actor.DynamicID(plr),
				DPath = 3,
				// 0 - crashes client
				// 1 - random scuttle (charged bolt effect)
				// 2 - random movement, random movement pauses (toads hopping)
				// 3 - clockwise spiral
				// 4 - counter-clockwise spiral
				Seed = 1,
				Carry = 1,
				TargetPostition = TargetPosition,
				Angle = ActorSystem.Movement.MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData()
				{
					annOwner = (int)actor.DynamicID(plr),
					SinIncAccel = 0.2f,
					LateralMaxDistance = 0.1f,
					LateralStartDistance = 0.1f,
					OOLateralDistanceToScale = 0.1f,
					SinIncPerTick = 1f,
					Speed = 1f
				},
				SpeedMulti = 1f,
				
			}, actor);
		}
		public void SpawnRandomEquip(Actor source, Player player, int forceQuality = -1, int forceLevel = -1)
		{
			//Logger.Debug("SpawnRandomEquip(): quality {0}", forceQuality);
			if (player != null)
			{
				int level = (forceLevel > 0 ? forceLevel : source.Attributes[GameAttribute.Level]);
				var item = ItemGenerator.GenerateRandomEquip(player, level, forceQuality);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;

				DropItem(source, null, item);
			}
		}
		public void SpawnRandomLegOrSetEquip(Actor source, Player player)
		{
			//Logger.Debug("SpawnRandomEquip(): quality {0}", forceQuality);
			if (player != null)
			{
				var item = ItemGenerator.GenerateLegOrSetRandom(player);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;

				DropItem(source, null, item);
			}
		}
		public void SpawnRandomCraftItem(Actor source, Player player)
		{
			if (player != null)
			{
				var item = ItemGenerator.GenerateRandomCraftItem(player, source.Attributes[GameAttribute.Level], true);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);

				if (source.Attributes[GameAttribute.Level] >= Program.MaxLevel)
				{
					item = ItemGenerator.GenerateRandomCraftItem(player, 35);
					if (item == null) return;
					player.GroundItems[item.GlobalID] = item;
					DropItem(source, null, item);

					item = ItemGenerator.GenerateRandomCraftItem(player, 55);
					if (item == null) return;
					player.GroundItems[item.GlobalID] = item;
					DropItem(source, null, item);
				}
			}
		}
		public void SpawnRandomUniqueGem(Actor source, Player player)
		{
			if (player != null)
			{
				var item = ItemGenerator.GenerateRandomUniqueGem(player);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		public void SpawnRandomGem(Actor source, Player player)
		{
			if (player != null && //player.JewelerUnlocked && 
				player.Attributes[GameAttribute.Level] >= 15)
			{
				var item = ItemGenerator.GenerateRandomGem(player, source.Attributes[GameAttribute.Level], source is Goblin);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		public void SpawnRandomPotion(Actor source, Player player)
		{
			if (player != null && !player.Inventory.HaveEnough(DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("HealthPotionBottomless"), 1))
			{
				var item = ItemGenerator.GenerateRandomPotion(player);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		public void SpawnEssence(Actor source, Player player)
		{
			int essence = (source.Attributes[GameAttribute.Level] > 60 ? 2087837753 : -152489231);
			if (player != null)
			{
				var item = ItemGenerator.CookFromDefinition(player.World, ItemGenerator.GetItemDefinition(essence)); //Crafting_Demonic_Reagent
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		/// <summary>
		/// Spanws gold for given player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="position">The position for drop.</param>
		public void SpawnGold(Actor source, Player player, int Min = -1)
		{
			int amount = (int)(LootManager.GetGoldAmount(player.Attributes[GameAttribute.Level]) * this.Game.GoldModifier * DiIiS_NA.GameServer.Config.Instance.RateMoney);
			if (Min != -1)
				amount += Min;
			var item = ItemGenerator.CreateGold(player, amount); // somehow the actual ammount is not shown on ground /raist.
			player.GroundItems[item.GlobalID] = item;
			DropItem(source, player, item);
		}
		/// <summary>
		/// Spanws blood shards for given player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="position">The position for drop.</param>
		public void SpawnBloodShards(Actor source, Player player, int forceAmount = 0)
		{
			int amount = LootManager.GetBloodShardsAmount(this.Game.Difficulty + 3);
			if (forceAmount == 0 && amount == 0) return; //don't drop shards on Normal
			var item = ItemGenerator.CreateBloodShards(player, forceAmount > 0 ? forceAmount : amount); // somehow the actual ammount is not shown on ground /raist.
			player.GroundItems[item.GlobalID] = item;
			DropItem(source, player, item);
		}
		/// <summary>
		/// Returns the first actor found with a given sno id
		/// </summary>
		/// <param name="sno"></param>
		/// <returns></returns>
		public Actor GetActorBySNO(int sno, bool onlyVisible = false)
		{
			foreach (var actor in this.Actors.Values)
			{
				if (actor.ActorSNO.Id == sno && (!onlyVisible || (onlyVisible && actor.Visible && !actor.Hidden)))
					return actor;
			}
			return null;
		}
		public List<Portal> GetPortalsByLevelArea(int la)
		{
			List<Portal> portals = new List<Portal>();
			foreach (var actor in this.Actors.Values)
			{
				if (actor is Portal)
					if ((actor as Portal).Destination != null)
						if ((actor as Portal).Destination.DestLevelAreaSNO == la)
						{
							bool alreadyAdded = false;
							foreach (var pt in portals)
								if (pt.Position == actor.Position) alreadyAdded = true;
							if (!alreadyAdded)
								portals.Add(actor as Portal);
						}
			}
			return portals;
		}
		/// <summary>
		/// Returns all actors matching a SNO id
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public List<Actor> GetActorsBySNO(int sno)
		{
			List<Actor> matchingActors = new List<Actor>();
			foreach (var actor in this.Actors.Values)
			{
				if (actor.ActorSNO.Id == sno)
					matchingActors.Add(actor);
			}
			return matchingActors;
		}
		/// <summary>
		/// Returns true if any actors exist under a well defined group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public bool HasActorsInGroup(string group)
		{
			var groupHash = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName(group);
			foreach (var actor in this.Actors.Values)
			{
				if (actor.Tags != null)
					if (actor.Tags.ContainsKey(MarkerKeys.Group1Hash))
						if (actor.Tags[MarkerKeys.Group1Hash] == groupHash) return true;
			}
			return false;
		}
		/// <summary>
		/// Returns all actors matching a group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public List<Actor> GetActorsInGroup(string group)
		{
			List<Actor> matchingActors = new List<Actor>();
			var groupHash = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName(group);
			foreach (var actor in this.Actors.Values)
			{
				if (actor.Tags != null)
					if (actor.Tags.ContainsKey(MarkerKeys.Group1Hash))
						if (actor.Tags[MarkerKeys.Group1Hash] == groupHash) matchingActors.Add(actor);
			}
			return matchingActors;
		}
		public List<Actor> GetActorsInGroup(int hash)
		{
			List<Actor> matchingActors = new List<Actor>();
			foreach (var actor in this.Actors.Values)
			{
				if (actor.Tags != null)
					if (actor.Tags.ContainsKey(MarkerKeys.Group1Hash))
						if (actor.Tags[MarkerKeys.Group1Hash] == hash) matchingActors.Add(actor);
			}
			return matchingActors;
		}
		/// <summary>
		/// Spanws a health-globe for given player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="position">The position for drop.</param>
		public void SpawnHealthGlobe(Actor source, Player player, Vector3D position)
		{
			// TODO: Health-globe should be spawned for all players in range. /raist.
			var item = ItemGenerator.CreateHealthGlobe(player, RandomHelper.Next(10, 15));
			DropItem(source, player, item);
		}
		public void SpawnArcaneGlobe(Actor source, Player player, Vector3D position)
		{
			var item = ItemGenerator.CreateArcaneGlobe(player);
			DropItem(source, player, item);
		}
		public void SpawnPowerGlobe(Actor source, Player player, Vector3D position)
		{
			var item = ItemGenerator.CreatePowerGlobe(player);
			DropItem(source, player, item);
		}
		/// <summary>
		/// Update the flippy animations and remove them once they have timed out
		/// </summary>
		/// <param name="tickCounter"></param>
		private void UpdateFlippy(int tickCounter)
		{
			if (_flippyTimers.Peek() == null)
				_flippyTimers.Dequeue();

			if (_flippyTimers.Peek().Count() == 2)
				_flippyTimers.Peek().Dequeue().Invoke();
			else
			{
				_flippyTimers.Dequeue().Dequeue().Invoke();
				if (_flippyTimers.Count() > 0)
					_flippyTimers.Peek().Dequeue().Invoke();
			}
		}
		/// <summary>
		/// Drops a given item to a random position close to the player
		/// </summary>
		/// <param name="player">Player to which to reveal the item</param>
		/// <param name="item">Item to reveal</param>
		public void DropItem(Player player, Item item)
		{
			DropItem(player, player, item);
		}
		/// <summary>
		/// Drops a given item to a random position close to a source actor
		/// </summary>
		/// <param name="source">Source actor of the flippy animation</param>
		/// <param name="player">Player to which to reveal the item</param>
		/// <param name="item">Item to reveal</param>
		public void DropItem(Actor source, Player player, Item item)
		{
			// Get a random location close to the source
			float x = (float)(RandomHelper.NextDouble() - 0.5) * FlippyMaxDistanceManhattan;
			float y = (float)(RandomHelper.NextDouble() - 0.5) * FlippyMaxDistanceManhattan;
			item.Position = source.Position + new Vector3D(x, y, 0);
			if (this.worldData.DynamicWorld)
				item.Position.Z = GetZForLocation(item.Position, source.Position.Z);
			item.Unstuck();

			// Items send either only a particle effect or default particle and either FlippyTag.Actor or their own actorsno
			int particleSNO = -1;
			int actorSNO = -1;

			if (item.SnoFlippyParticle != null)
			{
				particleSNO = item.SnoFlippyParticle.Id;
			}
			else
			{
				actorSNO = item.SnoFlippyActory == null ? -1 : item.SnoFlippyActory.Id;
				particleSNO = FlippyDefaultFlippy;
			}

			var queue = new Queue<Action>();
			queue.Enqueue(() => {
				BroadcastIfRevealed(plr => new FlippyMessage
				{
					ActorID = (int)source.GlobalID,
					Destination = item.Position,
					SNOFlippyActor = actorSNO,
					SNOParticleEffect = particleSNO
				}, source);
			});
			queue.Enqueue(() => item.Drop(null, item.Position));

			_flippyTimers.Enqueue(queue);
		}

		#endregion

		#region collections managemnet

		/// <summary>
		/// Adds given scene to world.
		/// </summary>
		/// <param name="scene">The scene to add.</param>
		public void AddScene(Scene scene)
		{
			if (scene.GlobalID == 0 || HasScene(scene.GlobalID))
				throw new Exception(String.Format("Scene has an invalid ID or was already present (ID = {0})", scene.GlobalID));

			this.Scenes.TryAdd(scene.GlobalID, scene); // add to scenes collection.
			this.QuadTree.Insert(scene); // add it to quad-tree too.
		}

		/// <summary>
		/// Removes given scene from world.
		/// </summary>
		/// <param name="scene">The scene to remove.</param>
		public void RemoveScene(Scene scene)
		{
			if (scene.GlobalID == 0 || !HasScene(scene.GlobalID))
				throw new Exception(String.Format("Scene has an invalid ID or was not present (ID = {0})", scene.GlobalID));

			Scene remotedScene;
			this.Scenes.TryRemove(scene.GlobalID, out remotedScene); // remove it from scenes collection.
			this.QuadTree.Remove(scene); // remove from quad-tree too.
		}

		/// <summary>
		/// Returns the scene with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the scene.</param>
		/// <returns></returns>
		public Scene GetScene(uint dynamicID)
		{
			Scene scene;
			this.Scenes.TryGetValue(dynamicID, out scene);
			return scene;
		}

        internal object GetActorByDynamicId(uint actor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if world contains a scene with given dynamicId.
        /// </summary>
        /// <param name="dynamicID">The dynamicId of the scene.</param>
        /// <returns><see cref="bool"/></returns>
        public bool HasScene(uint dynamicID)
		{
			return this.Scenes.ContainsKey(dynamicID);
		}

		/// <summary>
		/// Adds given actor to world.
		/// </summary>
		/// <param name="actor">The actor to add.</param>
		private void AddActor(Actor actor)
		{
			if (actor.GlobalID == 0 || HasActor(actor.GlobalID))
			{
				Logger.Warn("Actor has an invalid ID or was already present (ID = {0})", actor.GlobalID);
				//actor.DynamicID = this.NewActorID;
				return;
			}

			this.Actors.TryAdd(actor.GlobalID, actor); // add to actors collection.
			this.QuadTree.Insert(actor); // add it to quad-tree too.

			if (actor.ActorType == ActorType.Player) // if actor is a player, add it to players collection too.
				this.AddPlayer((Player)actor);
		}

		/// <summary>
		/// Removes given actor from world.
		/// </summary>
		/// <param name="actor">The actor to remove.</param>
		private void RemoveActor(Actor actor)
		{
			if (actor.GlobalID == 0 || !this.Actors.ContainsKey(actor.GlobalID))
				throw new Exception(String.Format("Actor has an invalid ID or was not present (ID = {0})", actor.GlobalID));

			Actor removedActor;
			this.Actors.TryRemove(actor.GlobalID, out removedActor); // remove it from actors collection.
			this.QuadTree.Remove(actor); // remove from quad-tree too.

			if (actor.ActorType == ActorType.Player) // if actors is a player, remove it from players collection too.
				this.RemovePlayer((Player)actor);
		}

		public Actor GetActorByGlobalId(uint globalID)
		{
			Actor actor;
			this.Actors.TryGetValue(globalID, out actor);
			return actor;
		}

		public uint GetGlobalId(Player plr, uint dynamicID)
		{
			try
			{
				return plr.RevealedObjects.Single(a => a.Value == dynamicID).Key;
			}
			catch
			{
				//Logger.Warn("Ошибка с актором - ID {0}", dynamicID);
				return dynamicID;
			}
		}

		/// <summary>
		/// Returns the actor with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the actor.</param>
		/// <param name="matchType">The actor-type.</param>
		/// <returns></returns>
		public Actor GetActorByGlobalId(uint dynamicID, ActorType matchType)
		{
			var actor = this.GetActorByGlobalId(dynamicID);
			if (actor != null)
			{
				if (actor.ActorType == matchType)
					return actor;
				else
					Logger.Warn("Attempted to get actor ID {0} as a {1}, whereas the actor is type {2}",
						dynamicID, Enum.GetName(typeof(ActorType), matchType), Enum.GetName(typeof(ActorType), actor.ActorType));
			}
			return null;
		}

		/// <summary>
		/// Returns true if the world has an actor with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the actor.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasActor(uint dynamicID)
		{
			return this.Actors.ContainsKey(dynamicID);
		}

		/// <summary>
		/// Returns true if the world has an actor with given dynamicId and type.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the actor.</param>
		/// <param name="matchType">The type of the actor.</param>
		/// <returns></returns>
		public bool HasActor(uint dynamicID, ActorType matchType)
		{
			var actor = this.GetActorByGlobalId(dynamicID, matchType);
			return actor != null;
		}

		/// <summary>
		/// Returns actor instance with given type.
		/// </summary>
		/// <typeparam name="T">Type of the actor.</typeparam>
		/// <returns>Actor</returns>
		public T GetActorInstance<T>() where T : Actor
		{
			return this.Actors.Values.OfType<T>().FirstOrDefault();
		}

		/// <summary>
		/// Adds given player to world.
		/// </summary>
		/// <param name="player">The player to add.</param>
		private void AddPlayer(Player player)
		{
			if (player.GlobalID == 0 || HasPlayer(player.GlobalID))
				throw new Exception(String.Format("Player has an invalid ID or was already present (ID = {0})", player.GlobalID));

			this.Players.TryAdd(player.GlobalID, player); // add it to players collection.
		}

		/// <summary>
		/// Removes given player from world.
		/// </summary>
		/// <param name="player"></param>
		private void RemovePlayer(Player player)
		{
			if (player.GlobalID == 0 || !this.Players.ContainsKey(player.GlobalID))
				throw new Exception(String.Format("Player has an invalid ID or was not present (ID = {0})", player.GlobalID));

			Player removedPlayer;
			this.Players.TryRemove(player.GlobalID, out removedPlayer); // remove it from players collection.
		}

		/// <summary>
		/// Returns player with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the player.</param>
		/// <returns></returns>
		public Player GetPlayer(uint dynamicID)
		{
			Player player;
			this.Players.TryGetValue(dynamicID, out player);
			return player;
		}

		/// <summary>
		/// Returns true if world contains a player with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the player.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasPlayer(uint dynamicID)
		{
			return this.Players.ContainsKey(dynamicID);
		}

		/// <summary>
		/// Returns item with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the item.</param>
		/// <returns></returns>
		public Item GetItem(uint dynamicID)
		{
			return (Item)GetActorByGlobalId(dynamicID, ActorType.Item);
		}

		/// <summary>
		/// Returns true if world contains a monster with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the monster.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasMonster(uint dynamicID)
		{
			return HasActor(dynamicID, ActorType.Monster);
		}

		/// <summary>
		/// Returns true if world contains an item with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the item.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasItem(uint dynamicID)
		{
			return HasActor(dynamicID, ActorType.Item);
		}

		#endregion

		#region misc-queries

		/// <summary>
		/// Returns StartingPoint with given id.
		/// </summary>
		/// <param name="id">The id of the StartingPoint.</param>
		/// <returns><see cref="StartingPoint"/></returns>

		public StartingPoint GetStartingPointById(int id)
		{
			return Actors.Values.OfType<StartingPoint>().Where(sp => sp.TargetId == id).ToList().FirstOrDefault();
		}

		public Actor FindAt(int actorId, Vector3D position, float radius = 3.0f)
		{
			var proximityCircle = new Circle(position.X, position.Y, radius);
			var actors = this.QuadTree.Query<Actor>(proximityCircle);
			foreach (var actr in actors)
				if (actr.Attributes[GameAttribute.Disabled] == false && actr.Attributes[GameAttribute.Gizmo_Has_Been_Operated] == false && actr.ActorSNO.Id == actorId) return actr;
			return null;
		}

		/// <summary>
		/// Returns WayPoint with given id.
		/// </summary>
		/// <param name="id">The id of the WayPoint</param>
		/// <returns><see cref="Waypoint"/></returns>
		public Waypoint GetWayPointById(int id)
		{
			return Actors.Values.OfType<Waypoint>().FirstOrDefault(waypoint => waypoint.WaypointId == id);
		}

		#endregion

		#region destroy, ctor, finalizer

		public override void Destroy()
		{
			// TODO: Destroy all objects /raist

			// TODO: Destroy pre-generated tile set

			this.worldData = null;
			//Game game = this.Game;
			this.Game = null;
			//game.EndTracking(this);
		}

		#endregion

		public bool CheckLocationForFlag(Vector3D location, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags flags)
		{
			// We loop Scenes as its far quicker than looking thru the QuadTree - DarkLotus

			foreach (Scene s in this.Scenes.Values)
			{
				if (s.Bounds.Contains(location.X, location.Y))
				{
					Scene scene = s;
					if (s.Parent != null) { scene = s.Parent; }
					if (s.Subscenes.Count > 0)
					{
						foreach (var subscene in s.Subscenes)
						{
							if (subscene.Bounds.Contains(location.X, location.Y))
							{
								scene = subscene;
							}
						}
					}

					int x = (int)((location.X - scene.Bounds.Left) / 2.5f);
					int y = (int)((location.Y - scene.Bounds.Top) / 2.5f);
					int total = (int)((y * scene.NavMesh.SquaresCountX) + x);
					if (total < 0 || total > scene.NavMesh.NavMeshSquareCount)
					{
						Logger.Error("Navmesh overflow!");
						return false;
					}
					try
					{
						return (scene.NavMesh.Squares[total].Flags & flags) == flags;
					}
					catch { }
				}
			}
			return false;
		}

		public float GetZForLocation(Vector3D location, float defaultZ)
		{
			foreach (Scene s in this.Scenes.Values)
			{
				if (s.Bounds.Contains(location.X, location.Y))
				{
					Scene scene = s;
					if (s.Parent != null) { scene = s.Parent; }
					if (s.Subscenes.Count > 0)
					{
						foreach (var subscene in s.Subscenes)
						{
							if (subscene.Bounds.Contains(location.X, location.Y))
							{
								scene = subscene;
							}
						}
					}

					int x = (int)((location.X - scene.Bounds.Left) / 2.5f);
					int y = (int)((location.Y - scene.Bounds.Top) / 2.5f);
					int total = (int)((y * scene.NavMesh.SquaresCountX) + x);
					if (total < 0 || total > scene.NavMesh.NavMeshSquareCount)
					{
						Logger.Error("Navmesh overflow!");
						return defaultZ;
					}
					try
					{
						return scene.NavMesh.Squares[total].Z;
					}
					catch
					{
						return defaultZ;
					}
				}
			}
			return defaultZ;
		}

		public bool CheckRayPath(Vector3D start, Vector3D destination)
		{

			var proximity = new RectangleF(start.X - 1f, start.Y - 1f, 2f, 2f);
			var scenes = this.QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return false;

			var scene = scenes[0]; // Parent scene /fasbat

			if (scenes.Count == 2) // What if it's a subscene? /fasbat
			{
				if (scenes[1].ParentChunkID != 0xFFFFFFFF)
					scene = scenes[1];
			}

			return true;
		}

		public override string ToString()
		{
			return string.Format("[World] SNOId: {0} GlobalId: {1} Name: {2}", this.WorldSNO.Id, this.GlobalID, this.WorldSNO.Name);
		}
	}
}
