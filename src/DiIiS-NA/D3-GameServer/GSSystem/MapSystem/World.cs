using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.QuadTrees;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.LoginServer.Toons;
using Actor = DiIiS_NA.GameServer.GSSystem.ActorSystem.Actor;
using Circle = DiIiS_NA.GameServer.Core.Types.Misc.Circle;
using Environment = DiIiS_NA.Core.MPQ.FileFormats.Environment;
using Monster = DiIiS_NA.GameServer.GSSystem.ActorSystem.Monster;
using ResolvedPortalDestination = DiIiS_NA.GameServer.MessageSystem.Message.Fields.ResolvedPortalDestination;

namespace DiIiS_NA.GameServer.GSSystem.MapSystem
{
	public sealed class World : DynamicObject, IRevealable, IUpdateable
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		public readonly Dictionary<World, List<Item>> DbItems = new(); //we need this list to delete item_instances from items which have no owner anymore.
		public readonly Dictionary<ulong, Item> CachedItems = new();

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
        public WorldSno SNO => (WorldSno)WorldSNO.Id;

        /// <summary>
		/// QuadTree that contains scenes & actors.
		/// </summary>
		private QuadTree _quadTree;
		public static QuadTree _PvPQuadTree = new(new Size(60, 60), 0);

		public QuadTree QuadTree
		{
			get => (IsPvP ? _PvPQuadTree : _quadTree);
			set { }
		}

		/// <summary>
		/// WorldData loaded from MPQs/DB
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.World worldData = new();

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

		private static readonly ConcurrentDictionary<uint, Scene> _PvPscenes = new();

		public ConcurrentDictionary<uint, Scene> Scenes
		{
			get => (IsPvP ? _PvPscenes : _scenes);
			set { }
		}

		/// <summary>
		/// List of actors contained in the world.
		/// </summary>
		private readonly ConcurrentDictionary<uint, Actor> _actors;

		public static readonly ConcurrentDictionary<uint, Actor> _PvPActors = new();

		public ConcurrentDictionary<uint, Actor> Actors
		{
			get => (IsPvP ? _PvPActors : _actors);
			set { }
		}

		public Dictionary<int, WorldSno> PortalOverrides = new();

		/// <summary>
		/// List of players contained in the world.
		/// </summary>
		private readonly ConcurrentDictionary<uint, Player> _players;

		public static readonly ConcurrentDictionary<uint, Player> _PvPPlayers = new();

		public ConcurrentDictionary<uint, Player> Players => (IsPvP ? _PvPPlayers : _players);

		/// <summary>
		/// Returns true if the world has players in.
		/// </summary>
		public bool HasPlayersIn => Players.Count > 0;

		/// <summary>
		/// Returns a new dynamicId for scenes.
		/// </summary>
		public uint NewSceneID => IsPvP ? NewPvPSceneID : Game.NewSceneId;

		public bool IsPvP => SNO == WorldSno.pvp_duel_small_multi; //PvP_Duel_Small

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
		public Environment Environment => ((DiIiS_NA.Core.MPQ.FileFormats.World)MPQStorage.Data.Assets[SNOGroup.Worlds][WorldSNO.Id].Data).Environment;

		private static uint _lastPvPObjectID = 10001;
		private static readonly object _obj = new();
		public static uint NewActorPvPID
		{
			get
			{
				lock (_obj)
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
				lock (_obj)
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
			get { return Actors.Values.OfType<StartingPoint>().Select(actor => actor).ToList(); }
		}

		public List<Portal> Portals => Actors.Values.OfType<Portal>().Select(actor => actor).ToList();

		public List<Monster> Monsters
		{
			get { return Actors.Values.OfType<Monster>().Select(actor => actor).ToList(); }
		}

		private PowerManager _powerManager;
		public static PowerManager _PvPPowerManager = new();

		public PowerManager PowerManager => IsPvP ? _PvPPowerManager : _powerManager;

		private BuffManager _buffManager;
		public static BuffManager _PvPBuffManager = new();

		public BuffManager BuffManager => IsPvP ? _PvPBuffManager : _buffManager;

		/// <summary>
		/// Creates a new world for the given game with given snoId.
		/// </summary>
		/// <param name="game">The parent game.</param>
		/// <param name="sno">The sno for the world.</param>
		public World(Game game, WorldSno sno)
			: base(sno == WorldSno.pvp_duel_small_multi ? 99999 : game.NewWorldId)
		{
			WorldSNO = new SNOHandle(SNOGroup.Worlds, (int)sno);

			Game = game;

			_scenes = new ConcurrentDictionary<uint, Scene>();
			_actors = new ConcurrentDictionary<uint, Actor>();
			_players = new ConcurrentDictionary<uint, Player>();
			_quadTree = new QuadTree(new Size(60, 60), 0);
			NextLocation = PrevLocation = new ResolvedPortalDestination
			{
				WorldSNO = (int)WorldSno.__NONE,
				DestLevelAreaSNO = -1,
				StartingPointActorTag = -1
			};
			_powerManager = new PowerManager();
			_buffManager = new BuffManager();

			Game.AddWorld(this);
			//this.Game.StartTracking(this); // start tracking the dynamicId for the world.

			if (SNO == WorldSno.x1_bog_01) //Blood Marsh
			{
				var worlds = new List<WorldSno> { WorldSno.x1_catacombs_level01, WorldSno.x1_catacombs_fakeentrance_02, WorldSno.x1_catacombs_fakeentrance_03, WorldSno.x1_catacombs_fakeentrance_04 };
				var scenes = new List<int> { 265624, 265655, 265678, 265693 };
				foreach (var scene in scenes)
				{
					var wld = worlds.PickRandom();
					PortalOverrides.Add(scene, wld);
					worlds.Remove(wld);
				}
			}
		}

		#region update & tick logic

		/// <summary>
		/// Retrieve all portals located within a specified <param name="radius"/> of the given <param name="actor"/>.
		/// </summary>
		/// <param name="actor">The actor located near the portals</param>
		/// <param name="radius">The radius of the portals to be returned is to be specified.</param>
		/// <returns>Order all existing portals in the world by ascending distance from a specified <param name="actor"></param>.</returns>
		/// <exception cref="ArgumentNullException">If <param name="actor"></param> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <param name="radius"></param> is not null but lesser than 0.</exception>
		public ImmutableArray<Portal> GetPortals(Actor actor, float? radius = null)
		{
			if (actor == null)
				throw new ArgumentNullException(nameof(actor));
			if (radius <= 0)
				throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be greater than zero.");

			if (radius is { } r)
				Logger.MethodTrace(
					$"All portals near $[underline]${actor.SNO} ({actor.GetType().Name})$[/]$ within $[underline]${r}$[/]$ radius");
			else
				Logger.MethodTrace($"All portals near $[underline]${actor.SNO} ({actor.GetType().Name})$[/]$");

			return Portals
				.Where(portal =>
				{
					if (radius is not { } r) return true;
					var position = actor.Position;
					var distance = portal.Position.DistanceSquared(ref position);
					return distance <= radius.Value;
				})
				.OrderBy(s =>
				{
					var position = actor.Position;
					return s.Position.DistanceSquared(ref position);
				})
				.ToImmutableArray();
		}

		public void Update(int tickCounter)
		{
			foreach (var player in Players.Values)
			{
				player.InGameClient.SendTick(); // if there's available messages to send, will handle ticking and flush the outgoing buffer.
			}

			var actorsToUpdate = new List<IUpdateable>(); // list of actor to update.

			foreach (var player in Players.Values) // get players in the world.
			{
				foreach (var actor in player.GetActorsInRange().OfType<IUpdateable>()) // get IUpdateable actors in range.
				{
					if (actorsToUpdate.Contains(actor)) // don't let a single actor in range of more than players to get updated more thance per tick /raist.
						continue;

					actorsToUpdate.Add(actor);
				}
			}
			foreach (var minion in Actors.Values.OfType<Minion>())
			{
				if (actorsToUpdate.Contains(minion))
					continue;
				actorsToUpdate.Add(minion);
			}
			foreach (var actor in actorsToUpdate) // trigger the updates.
			{
				actor.Update(tickCounter);
			}

			BuffManager.Update();
			PowerManager.Update();

			if (tickCounter % 6 == 0 && _flippyTimers.Any())
			{
				UpdateFlippy(tickCounter);
			}
		}

		#endregion

		#region message broadcasting

		/// <summary>
		/// Broadcasts a message to all players in the world.
		/// </summary>
		/// <param name="action">The action that will be invoked to all players</param>
		/// <exception cref="Exception">If there was an error to broadcast to player.</exception>
		public void BroadcastOperation(Action<Player> action)
		{
			foreach (var player in Players.Values)
			{
				if (player == null) continue;
				try
				{
					action(player);
				}
				catch (Exception ex)
				{
					throw new Exception("Error while broadcasting to player " + player.Name, ex);
				}
			}
		}
		
		/// <summary>
		/// Broadcasts a message to all players in the world where the <param name="predicate"></param> is true.
		/// </summary>
		/// <param name="predicate">Players matching criteria</param>
		/// <param name="action">The action that will be invoked to all players</param>
		/// <exception cref="Exception">If there was an error to broadcast to player</exception>
		public void BroadcastOperation(Func<Player, bool> predicate, Action<Player> action)
		{
			foreach (var player in Players.Values.Where(predicate))
			{
				if (player == null) continue;
				try
				{
					action(player);
				}
				catch (Exception ex)
				{
					throw new Exception("Error while broadcasting to player " + player.Name, ex);
				}
			}
		}

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
			BroadcastOperation(player => player.RevealedObjects.ContainsKey(actor.GlobalID), player => player.InGameClient.SendMessage(message(player)));
		}

		/// <summary>
		/// Broadcasts a message to all players in the world.
		/// </summary>
		/// <param name="message"></param>
		public void BroadcastGlobal(Func<Player, GameMessage> message)
		{
			BroadcastOperation(player => player.InGameClient.SendMessage(message(player)));
		}

		/// <summary>
		/// Broadcasts a message to all players in the range of given actor.
		/// </summary>
		/// <param name="message">The message to broadcast.</param>
		/// <param name="actor">The actor.</param>
		public void BroadcastInclusive(Func<Player, GameMessage> message, Actor actor, float? radius = null)
		{
			var players = actor.GetPlayersInRange(radius);
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
			if (global) players = Players.Values.ToList();
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
				if (player.RevealedObjects.ContainsKey(GlobalID))
					return false;

				int sceneGridSize = SNO.IsUsingZoltCustomGrid() ? 100 : 60;
				
				player.InGameClient.SendMessage(new RevealWorldMessage() // Reveal world to player
				{
					WorldID = GlobalID,
					WorldSNO = WorldSNO.Id,
					OriginX = 540,
					OriginY = -600,
					StitchSizeInFeetX = sceneGridSize,
					StitchSizeInFeetY = sceneGridSize,
					WorldSizeInFeetX = 5040,
					WorldSizeInFeetY = 5040,
					snoDungeonFinderSourceWorld = -1
				});
				player.InGameClient.SendMessage(new WorldStatusMessage { WorldID = GlobalID, Field1 = false });
				//*
				player.InGameClient.SendMessage(new WorldSyncedDataMessage
				{
					WorldID = GlobalID,
					SyncedData = new WorldSyncedData
					{
						SnoWeatherOverride = -1,
						WeatherIntensityOverride = 0,
						WeatherIntensityOverrideEnd = 0
					}
				});

				//*/
				player.RevealedObjects.Add(GlobalID, GlobalID);

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
			if (!player.RevealedObjects.ContainsKey(GlobalID)) return false;

			foreach (var scene in Scenes.Values) scene.Unreveal(player);
			player.RevealedObjects.Remove(GlobalID);

			player.InGameClient.SendMessage(new WorldStatusMessage { WorldID = GlobalID, Field1 = true });
			player.InGameClient.SendMessage(new PrefetchDataMessage(Opcodes.PrefetchWorldMessage) { SNO = WorldSNO.Id });
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
			AddActor(actor);
			actor.OnEnter(this);

			// reveal actor to player's in-range.
			foreach (var player in actor.GetPlayersInRange())
			{
				actor.Reveal(player);
			}

			//Убираем балки с проходов
			if (SNO == WorldSno.trout_town)
			{
				foreach (var boarded in GetActorsBySNO(ActorSno._trout_oldtristram_cellardoor_boarded))
					boarded.Destroy();
				foreach (var boarded in GetActorsBySNO(ActorSno._trout_oldtristram_cellardoor_rubble))
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

			foreach (var player in Players.Values)
			{
				actor.Unreveal(player);
			}

			if (HasActor(actor.GlobalID))
				RemoveActor(actor);

			if (!(actor is Player)) return; // if the leaving actors is a player, unreveal the actors revealed to him contained in the world.

			var revealedObjects = (actor as Player).RevealedObjects.Keys.ToList(); // list of revealed actors.
			foreach (var obj_id in revealedObjects)
			{
				var obj = GetActorByGlobalId(obj_id);
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
		public Actor ShowOnlyNumNPC(ActorSno SNO, int Num)
		{
			Actor Setted = null;
			foreach (var actor in GetActorsBySNO(SNO))
			{
				var isVisible = actor.NumberInWorld == Num;
				if (isVisible)
					Setted = actor;

				actor.Hidden = !isVisible;
				actor.SetVisible(isVisible);
				foreach (var plr in Players.Values)
                {
                    if (isVisible) actor.Reveal(plr); else actor.Unreveal(plr);
				}
            }
			return Setted;
		}

        #endregion

        #region monster spawning & item drops

        /// <summary>
        /// Spawns a monster with given SNOId in given position.
        /// </summary>
        /// <param name="monsterSno">The SNOId of the monster.</param>
        /// <param name="position">The position to spawn it.</param>
		
        public Actor SpawnMonster(ActorSno monsterSno, Vector3D position)
        {
            if (monsterSno == ActorSno.__NONE)  return null;
            Logger.MethodTrace($"Spawning monster {monsterSno} at {position}");
            var monster = ActorFactory.Create(this, monsterSno, new TagMap());
            if (monster == null) return null;
            
            monster.EnterWorld(position);
            if (monster.AnimationSet == null) return monster;
            var animationTag = new[] { AnimationSetKeys.Spawn, AnimationSetKeys.Spawn2 }.FirstOrDefault(x => monster.AnimationSet.TagMapAnimDefault.ContainsKey(x));

            if (animationTag != null)
            {
	            monster.World.BroadcastIfRevealed(plr => new PlayAnimationMessage
	            {
		            ActorID = monster.DynamicID(plr),
		            AnimReason = 5,
		            UnitAniimStartTime = 0,
		            tAnim = new PlayAnimationMessageSpec[]
		            {
			            new()
			            {
				            Duration = 150,
				            AnimationSNO = monster.AnimationSet.TagMapAnimDefault[animationTag],
				            PermutationIndex = 0,
				            Speed = 1
			            }
		            }

	            }, monster);
            }
            return monster;
        }

        private Queue<Queue<Action>> _flippyTimers = new();

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
		
		[Obsolete("Isn't used anymore. Is it useful?")]
		public void PlayPieAnimation(Actor actor, Actor user, int powerSNO, Vector3D targetPosition)
		{

			BroadcastIfRevealed(plr => new ACDTranslateDetPathPieWedgeMessage
			{
				ann = (int)actor.DynamicID(plr),
				StartPos = user.Position,
				FirstTagetPos = user.Position,
				MoveFlags = 9,
				AnimTag = 1,
				PieData = new DPathPieData
				{
					Field0 = targetPosition,
					Field1 = 1,
					Field2 = 1,
					Field3 = 1
				},
				Field6 = 1f,

			}, actor);
		}
		public void PlayCircleAnimation(Actor actor, Actor User, int PowerSNO, Vector3D TargetPosition)
		{

			BroadcastIfRevealed(plr => new ACDTranslateDetPathSinMessage
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
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData
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

			BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new ACDTranslateDetPathSinMessage
			{
				ActorID = actor.DynamicID(plr),
				DPath = 5,
				Seed = 1,
				Carry = 1,
				TargetPostition = TargetPosition,
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData
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

			BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new ACDTranslateDetPathSinMessage
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
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData
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
			
			BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
			{
				ActorId = actor.DynamicID(plr),
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				TurnImmediately = true
			}, actor);

			BroadcastIfRevealed(plr => new ACDTranslateDetPathSinMessage 
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
				Angle = MovementHelpers.GetFacingAngle(User.Position, TargetPosition),
				StartPosition = User.Position,
				MoveFlags = 1,
				AnimTag = 1,
				PowerSNO = PowerSNO,
				Var0Int = 1,
				Var0Fl = 1f,
				SinData = new DPathSinData
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
		public Item SpawnRandomEquip(Actor source, Player player, int forceQuality = -1, int forceLevel = -1,
			GameBalance.ItemTypeTable type = null, bool canBeUnidentified = true, ToonClass toonClass = ToonClass.Unknown)
		{
			Logger.MethodTrace($"quality {forceQuality}");
			if (player != null)
			{
				int level = (forceLevel > 0 ? forceLevel : source.Attributes[GameAttributes.Level]);
				if (toonClass == ToonClass.Unknown && type == null)
				{
					var item = ItemGenerator.GenerateRandomEquip(player, level, forceQuality, forceQuality, canBeUnidentified: canBeUnidentified);
					if (item == null) return null;
					player.GroundItems[item.GlobalID] = item;

					DropItem(source, null, item);
					return item;
				}
				else
				{
					var item = ItemGenerator.GenerateRandomEquip(player, level, forceQuality, forceQuality, type: type,ownerClass: toonClass, canBeUnidentified: canBeUnidentified);
					if (item == null) return null;
					player.GroundItems[item.GlobalID] = item;

					DropItem(source, null, item);
					return item;
				}
			}

			return null;
		}
		public void SpawnRandomLegOrSetEquip(Actor source, Player player)
		{
			//Logger.MethodTrace("quality {0}", forceQuality);
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
				var item = ItemGenerator.GenerateRandomCraftItem(player, source.Attributes[GameAttributes.Level], true);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);

				if (source.Attributes[GameAttributes.Level] >= Program.MAX_LEVEL)
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
				player.Attributes[GameAttributes.Level] >= 15)
			{
				var item = ItemGenerator.GenerateRandomGem(player, source.Attributes[GameAttributes.Level], source is Goblin);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		public void SpawnRandomPotion(Actor source, Player player)
		{
			if (player != null && !player.Inventory.HaveEnough(StringHashHelper.HashItemName("HealthPotionBottomless"), 1))
			{
				var item = ItemGenerator.GenerateRandomPotion(player);
				if (item == null) return;
				player.GroundItems[item.GlobalID] = item;
				DropItem(source, null, item);
			}
		}
		public void SpawnEssence(Actor source, Player player)
		{
			int essence = (source.Attributes[GameAttributes.Level] > 60 ? 2087837753 : -152489231);
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
			int amount = (int)(LootManager.GetGoldAmount(player.Attributes[GameAttributes.Level]) * Game.GoldModifier * GameModsConfig.Instance.Rate.Gold);
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
			int amount = LootManager.GetBloodShardsAmount(Game.Difficulty + 3);
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
		public Actor GetActorBySNO(ActorSno sno, bool onlyVisible = false)
		{
			return Actors.Values.FirstOrDefault(x => x.SNO == sno && (!onlyVisible || (onlyVisible && x.Visible && !x.Hidden)));
		}
		public List<Portal> GetPortalsByLevelArea(int la)
		{
			List<Portal> portals = new List<Portal>();
			foreach (var actor in Actors.Values)
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
		/// Returns all actors matching one of SNOs
		/// </summary>
		/// <param name="sno"></param>
		/// <returns></returns>
		public List<Actor> GetActorsBySNO(params ActorSno[] sno)
        {
			return Actors.Values.Where(x => sno.Contains(x.SNO)).ToList();
        }
		/// <summary>
		/// Returns true if any actors exist under a well defined group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public bool HasActorsInGroup(string group)
		{
			var groupHash = StringHashHelper.HashItemName(group);
			foreach (var actor in Actors.Values)
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
			var groupHash = StringHashHelper.HashItemName(group);
			foreach (var actor in Actors.Values)
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
			foreach (var actor in Actors.Values)
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
				if (_flippyTimers.Any())
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
			if (worldData.DynamicWorld)
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
				throw new Exception($"Scene has an invalid ID or was already present (ID = {scene.GlobalID})");

			Scenes.TryAdd(scene.GlobalID, scene); // add to scenes collection.
			QuadTree.Insert(scene); // add it to quad-tree too.
		}

		/// <summary>
		/// Removes given scene from world.
		/// </summary>
		/// <param name="scene">The scene to remove.</param>
		public void RemoveScene(Scene scene)
		{
			if (scene.GlobalID == 0 || !HasScene(scene.GlobalID))
				throw new Exception($"Scene has an invalid ID or was not present (ID = {scene.GlobalID})");

			Scenes.TryRemove(scene.GlobalID, out _); // remove it from scenes collection.
			QuadTree.Remove(scene); // remove from quad-tree too.
		}

		/// <summary>
		/// Returns the scene with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the scene.</param>
		/// <returns></returns>
		public Scene GetScene(uint dynamicID)
		{
			Scenes.TryGetValue(dynamicID, out var scene);
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
			return Scenes.ContainsKey(dynamicID);
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

			Actors.TryAdd(actor.GlobalID, actor); // add to actors collection.
			QuadTree.Insert(actor); // add it to quad-tree too.

			if (actor.ActorType == ActorType.Player) // if actor is a player, add it to players collection too.
				AddPlayer((Player)actor);
		}

		/// <summary>
		/// Removes given actor from world.
		/// </summary>
		/// <param name="actor">The actor to remove.</param>
		private void RemoveActor(Actor actor)
		{
			if (actor.GlobalID == 0 || !Actors.ContainsKey(actor.GlobalID))
				throw new Exception($"Actor has an invalid ID or was not present (ID = {actor.GlobalID})");

			Actors.TryRemove(actor.GlobalID, out _); // remove it from actors collection.
			QuadTree.Remove(actor); // remove from quad-tree too.

			if (actor.ActorType == ActorType.Player) // if actors is a player, remove it from players collection too.
				RemovePlayer((Player)actor);
		}

		public Actor GetActorByGlobalId(uint globalID)
		{
			Actors.TryGetValue(globalID, out var actor);
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
			var actor = GetActorByGlobalId(dynamicID);
			if (actor != null)
			{
				if (actor.ActorType == matchType)
					return actor;
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
			return Actors.ContainsKey(dynamicID);
		}

		/// <summary>
		/// Returns true if the world has an actor with given dynamicId and type.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the actor.</param>
		/// <param name="matchType">The type of the actor.</param>
		/// <returns></returns>
		public bool HasActor(uint dynamicID, ActorType matchType)
		{
			var actor = GetActorByGlobalId(dynamicID, matchType);
			return actor != null;
		}

		/// <summary>
		/// Returns actor instance with given type.
		/// </summary>
		/// <typeparam name="T">Type of the actor.</typeparam>
		/// <returns>Actor</returns>
		public T GetActorInstance<T>() where T : Actor
		{
			return Actors.Values.OfType<T>().FirstOrDefault();
		}

		/// <summary>
		/// Adds given player to world.
		/// </summary>
		/// <param name="player">The player to add.</param>
		private bool AddPlayer(Player player)
		{
			if (player == null)
				throw new Exception($"Player in world {SNO} is null and cannot be removed.");
			
			if (player.GlobalID == 0 || HasPlayer(player.GlobalID))
				throw new Exception($"Player has an invalid ID or was already present (ID = {player.GlobalID})");

			return Players.TryAdd(player.GlobalID, player); // add it to players collection.
		}

		/// <summary>
		/// Removes given player from world.
		/// </summary>
		/// <param name="player"></param>
		private bool RemovePlayer(Player player)
		{
			if (player == null)
				throw new Exception($"Player in world {SNO} is null and cannot be removed.");
			
			if (player.GlobalID == 0 || !Players.ContainsKey(player.GlobalID))
				throw new Exception($"Player has an invalid ID or was not present (ID = {player.GlobalID})");

			return Players.TryRemove(player.GlobalID, out _); // remove it from players collection.
		}

		/// <summary>
		/// Returns player with a given predicate
		/// </summary>
		/// <param name="predicate">Predicate to find player</param>
		/// <param name="player">Player result</param>
		/// <returns>Whether the player was found.</returns>
		public bool TryGetPlayer(Func<Player, bool> predicate, out Player player)
		{
			player = Players.Values.FirstOrDefault(predicate);
			return player != null;
		}

		/// <summary>
		/// Returns true if world contains a player with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the player.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasPlayer(uint dynamicID) => Players.ContainsKey(dynamicID);

		/// <summary>
		/// Returns item with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the item.</param>
		/// <returns></returns>
		public Item GetItem(uint dynamicID) => (Item)GetActorByGlobalId(dynamicID, ActorType.Item);

		/// <summary>
		/// Returns true if world contains a monster with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the monster.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasMonster(uint dynamicID) => HasActor(dynamicID, ActorType.Monster);

		/// <summary>
		/// Returns true if world contains an item with given dynamicId.
		/// </summary>
		/// <param name="dynamicID">The dynamicId of the item.</param>
		/// <returns><see cref="bool"/></returns>
		public bool HasItem(uint dynamicID) => HasActor(dynamicID, ActorType.Item);

		#endregion

		#region misc-queries

		/// <summary>
		/// Returns StartingPoint with given id.
		/// </summary>
		/// <param name="id">The id of the StartingPoint.</param>
		/// <returns><see cref="StartingPoint"/></returns>

		public StartingPoint GetStartingPointById(int id) => Actors.Values.OfType<StartingPoint>().Where(sp => sp.TargetId == id).ToList().FirstOrDefault();

		public Actor FindActorAt(ActorSno actorSno, Vector3D position, float radius = 3.0f)
		{
			var proximityCircle = new Circle(position.X, position.Y, radius);
			var actors = QuadTree.Query<Actor>(proximityCircle);
			foreach (var actor in actors)
				if (actor.Attributes[GameAttributes.Disabled] == false && actor.Attributes[GameAttributes.Gizmo_Has_Been_Operated] == false && actor.SNO == actorSno) return actor;
			return null;
		}

		/// <summary>
		/// Returns WayPoint with given id.
		/// </summary>
		/// <param name="id">The id of the WayPoint</param>
		/// <returns><see cref="Waypoint"/></returns>
		public Waypoint GetWayPointById(int id) => Actors.Values.OfType<Waypoint>().FirstOrDefault(waypoint => waypoint.WaypointId == id);

		public Waypoint[] GetAllWaypoints() => Actors.Values.OfType<Waypoint>().ToArray();

		public Waypoint[] GetAllWaypointsInWorld(WorldSno worldSno) => Actors.Values.OfType<Waypoint>().Where(waypoint => waypoint.World.SNO == worldSno).ToArray();

		public Waypoint[] GetAllWaypointsInWorld(World world) => Actors.Values.OfType<Waypoint>().Where(waypoint => waypoint.World == world).ToArray();

		#endregion

		#region destroy, ctor, finalizer

		public override void Destroy()
		{
			Logger.Trace($"$[red]$Destroying$[/]$ World #{GlobalID} $[underline red]${SNO}$[/]$");
			// TODO: Destroy all objects @iamdroppy - solution below added for testing on 21/01/2023
			// foreach (var actor in Actors.Values)
			// 	try
			// 	{
			// 		actor.Destroy();
			// 	}
			// 	catch {}
			//
			// foreach (var player in Players.Values)
			// 	try
			// 	{
			// 		player.Destroy();
			// 	}
			// 	catch{}
			// foreach (var portal in Portals)
			// 	try
			// 	{
			// 		portal.Destroy();
			// 	}
			// 	catch{}
			// TODO: Destroy pre-generated tile set

			worldData = null;
			//Game game = this.Game;
			Game = null;
			//game.EndTracking(this);
		}

		#endregion

		public bool CheckLocationForFlag(Vector3D location, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags flags)
		{
			// We loop Scenes as its far quicker than looking thru the QuadTree - DarkLotus

			foreach (Scene s in Scenes.Values)
			{
				if (s.Bounds.Contains(location.X, location.Y))
				{
					Scene scene = s;
					if (s.Parent != null) { scene = s.Parent; }
					if (s.Subscenes.Count > 0)
					{
						foreach (var subScene in s.Subscenes.Where(subScene => subScene.Bounds.Contains(location.X, location.Y)))
						{
							scene = subScene;
						}
					}

					int x = (int)((location.X - scene.Bounds.Left) / 2.5f);
					int y = (int)((location.Y - scene.Bounds.Top) / 2.5f);
					int total = (y * scene.NavMesh.SquaresCountX) + x;
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
			foreach (Scene s in Scenes.Values.Where(s => s.Bounds.Contains(location.X, location.Y)))
			{
				Scene scene = s;
				if (s.Parent != null)
				{
					scene = s.Parent;
				}

				if (s.Subscenes.Count > 0)
				{
					foreach (var subScene in s.Subscenes)
					{
						if (subScene.Bounds.Contains(location.X, location.Y))
						{
							scene = subScene;
						}
					}
				}

				int x = (int)((location.X - scene.Bounds.Left) / 2.5f);
				int y = (int)((location.Y - scene.Bounds.Top) / 2.5f);
				int total = (y * scene.NavMesh.SquaresCountX) + x;
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

			return defaultZ;
		}

		[Obsolete("Isn't used anymore")] // made obsolete by @iamdroppy on 28/01/2023
		public bool CheckRayPath(Vector3D start, Vector3D destination)
		{
			var proximity = new RectangleF(start.X - 1f, start.Y - 1f, 2f, 2f);
			var scenes = QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return false;

			if (scenes.Count == 2) // What if it's a subscene? /fasbat
			{
				if (scenes[1].ParentChunkID != 0xFFFFFFFF)
				{
				}
			}

			return true;
		}

		public override string ToString()
		{
			return $"[World] SNOId: {WorldSNO.Id} GlobalId: {GlobalID} Name: {WorldSNO.Name}";
		}

		public ImmutableArray<Door> GetAllDoors() =>
			Actors.Select(a => a.Value).Where(a => a is Door).Cast<Door>().ToImmutableArray();
		public ImmutableArray<Door> GetAllDoors(ActorSno sno) =>
			Actors.Select(a => a.Value).Where(a => a is Door && a.SNO == sno).Cast<Door>().ToImmutableArray();
		public ImmutableArray<Door> OpenAllDoors()
		{
			List<Door> openedDoors = new();
			var doors = GetAllDoors();
			
			foreach (var door in doors)
			{
				openedDoors.Add(door);
				door.Open();
			}

			return openedDoors.ToImmutableArray();
		}
	}
}
