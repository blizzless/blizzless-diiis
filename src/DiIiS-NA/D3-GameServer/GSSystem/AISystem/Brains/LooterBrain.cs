using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// Pet-companion brain for loot-picker minions (e.g. the Treasure Goblin
	/// pet from Boon of the Hoarder / Puzzle Ring, follower loot pets, etc.).
	///
	/// <para>Behaviour:</para>
	/// <list type="bullet">
	///   <item><description>Every <see cref="Update"/> tick, auto-picks
	///     up gold and blood shards within 5 tiles of the body, crediting
	///     the master player. Legendaries are picked up too if
	///     <see cref="LootLegendaries"/> is set.</description></item>
	///   <item><description><see cref="Think"/> walks the body toward the
	///     nearest gold/legendary drop within 40 tiles of itself.</description></item>
	///   <item><description>Like minions, it leashes to its master within
	///     an 80-tile radius and a 3–8 tile idle ring.</description></item>
	/// </list>
	///
	/// <para>The <c>PresetPowers</c> field is populated but never consulted
	/// — this brain has no combat code path. The field is retained for
	/// forward-compat with shared minion tooling.</para>
	/// </summary>
	public class LooterBrain : Brain
	{
		/// <summary>
		/// Powers inherited from the MPQ monster data. Populated but
		/// currently unused; kept for parity with other minion brains.
		/// </summary>
		public Dictionary<int, Cooldown> PresetPowers { get; private set; }

		/// <summary>Random startup stagger so loot pets don't move in lockstep.</summary>
		private TickTimer _powerDelay;

		/// <summary>Current loot-pickup target (a gold / item drop).</summary>
		private Actor _target { get; set; }

		/// <summary>If true, this pet also picks up legendary ("Unique_") items.</summary>
		private bool LootLegendaries { get; set; }

		/// <summary>Per-power cooldown tracker. Unused by LooterBrain.</summary>
		public struct Cooldown
		{
			/// <summary>Active timer; <c>null</c> when ready to cast.</summary>
			public TickTimer CooldownTimer;

			/// <summary>Base cooldown duration in seconds.</summary>
			public float CooldownTime;
		}

		/// <summary>
		/// Creates a loot-picker brain.
		/// </summary>
		/// <param name="body">The pet actor.</param>
		/// <param name="lootsLegs">
		/// Whether to also auto-pick legendary drops. Gold and blood shards
		/// are always picked up regardless of this flag.
		/// </param>
		public LooterBrain(Actor body, bool lootsLegs)
			: base(body)
		{
			LootLegendaries = lootsLegs;
			PresetPowers = new Dictionary<int, Cooldown>();
			Logger.Trace("LooterBrain spawned: {0} (lootsLegs: {1})",
				body?.SNO.ToString() ?? "<null>", lootsLegs);

			// Build list of powers defined in monster mpq data. Retained
			// for parity with the other minion brains; not consulted.
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						PresetPowers.Add(monsterSkill.SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = 1f });
					}
				}
			}
		}

		/// <summary>
		/// Per-tick update. In addition to running the base
		/// <see cref="Brain.Update"/> (which calls <see cref="Think"/> and
		/// advances <c>CurrentAction</c>), scans a 5-tile radius and picks
		/// up gold, blood shards and (optionally) legendary drops for the
		/// master player. Each pickup emits a floating gold/blood-shard
		/// amount message.
		/// </summary>
		public override void Update(int tickCounter)
		{
			base.Update(tickCounter);

			// Gold pickups within 5 tiles → credit to master, spawn float.
			List<Item> gold = Body.GetObjectsInRange<Item>(5f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsGold((m as Item).ItemType)).ToList();
			foreach (var item in gold)
			{
				((Body as Minion).Master as Player).InGameClient.SendMessage(new FloatingAmountMessage()
				{
					Place = new WorldPlace()
					{
						Position = Body.Position,
						WorldID = Body.World.GlobalID,
					},

					Amount = item.Attributes[GameAttributes.ItemStackQuantityLo],
					Type = FloatingAmountMessage.FloatType.Gold,
				});

				Logger.Trace("LooterBrain picked up {0} gold for {1}",
					item.Attributes[GameAttributes.ItemStackQuantityLo],
					((Body as Minion).Master as Player)?.Toon?.Name ?? "<unknown>");
				((Body as Minion).Master as Player).Inventory.PickUpGold(item);
				((Body as Minion).Master as Player).GroundItems.Remove(item.GlobalID);
				item.Destroy();
			}

			// Legendary pickups within 5 tiles — only if flag is set.
			if (LootLegendaries)
			{
				List<Item> legendaries = Body.GetObjectsInRange<Item>(5f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).ToList();
				foreach (var item in legendaries)
				{
					Logger.Debug("LooterBrain auto-picked legendary {0} for {1}",
						item.ItemDefinition.Name,
						((Body as Minion).Master as Player)?.Toon?.Name ?? "<unknown>");
					((Body as Minion).Master as Player).Inventory.PickUp(item);
				}
			}

			// Blood-shard pickups within 5 tiles → credit to master.
			List<Item> shards = Body.GetObjectsInRange<Item>(5f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsBloodShard((m as Item).ItemType)).ToList();
			foreach (var item in shards)
			{
				((Body as Minion).Master as Player).InGameClient.SendMessage(new FloatingAmountMessage()
				{
					Place = new WorldPlace()
					{
						Position = Body.Position,
						WorldID = Body.World.GlobalID,
					},

					Amount = item.Attributes[GameAttributes.ItemStackQuantityLo],
					Type = FloatingAmountMessage.FloatType.BloodStone,
				});

				((Body as Minion).Master as Player).Inventory.PickUpBloodShard(item);
				((Body as Minion).Master as Player).GroundItems.Remove(item.GlobalID);
				item.Destroy();
			}
		}

		/// <summary>
		/// Main AI tick. Walks toward the nearest gold/legendary drop, or
		/// idles in a 3–8 tile ring around the master if nothing is worth
		/// picking up. Pure navigation — no combat.
		/// </summary>
		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;
			if ((Body as Minion).Master == null) return;

			if (Body.World.Game.Paused) return;

			// Select and start executing a move if no active action.
			if (CurrentAction == null)
			{
				// Small random stagger on first think.
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					// Scan 40 tiles for lootable gold (and legendaries if enabled).
					List<Actor> targets = Body.GetObjectsInRange<Item>(40f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsGold((m as Item).ItemType)).Cast<Actor>().ToList();
					if (LootLegendaries)
						targets.Concat(Body.GetObjectsInRange<Item>(40f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).Cast<Actor>().ToList());

					// 80-tile master leash; any drop inside the search
					// radius becomes the new walk target.
					if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position) < 80f)
					{
						_target = targets.First();
						//Logger.Trace("MoveToTargetWithPathfindAction to target");
						CurrentAction = new MoveToPointAction(Body, _target.Position);
					}
					else
					{
						// No drops → wander in a 3–8 tile ring around the master.
						var distToMaster = PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position);
						if ((distToMaster > 8f) || (distToMaster < 3f))
						{
							var Rand = FastRandom.Instance;
							var position = (Body as Minion).Master.Position;
							float angle = (float)(Rand.NextDouble() * Math.PI * 2);
							float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
							var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
							CurrentAction = new MoveToPointAction(Body, near);
						}
					}
				}
			}
		}
	}
}
