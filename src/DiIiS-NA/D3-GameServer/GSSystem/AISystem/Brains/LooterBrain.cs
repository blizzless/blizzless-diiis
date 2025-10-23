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
	public class LooterBrain : Brain
	{
		// list of power SNOs that are defined for the monster
		public Dictionary<int, Cooldown> PresetPowers { get; private set; }

		private TickTimer _powerDelay;
		private Actor _target { get; set; }
		private bool LootLegendaries { get; set; }

		public struct Cooldown
		{
			public TickTimer CooldownTimer;
			public float CooldownTime;
		}

		public LooterBrain(Actor body, bool lootsLegs)
			: base(body)
		{
			LootLegendaries = lootsLegs;
			PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
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

		public override void Update(int tickCounter)
		{
			base.Update(tickCounter);

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

				((Body as Minion).Master as Player).Inventory.PickUpGold(item);
				((Body as Minion).Master as Player).GroundItems.Remove(item.GlobalID);
				item.Destroy();
			}

			if (LootLegendaries)
			{
				List<Item> legendaries = Body.GetObjectsInRange<Item>(5f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).ToList();
				foreach (var item in legendaries)
				{
					((Body as Minion).Master as Player).Inventory.PickUp(item);
				}
			}

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

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;
			if ((Body as Minion).Master == null) return;

			if (Body.World.Game.Paused) return;

			// select and start executing a power if no active action
			if (CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					List<Actor> targets = Body.GetObjectsInRange<Item>(40f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsGold((m as Item).ItemType)).Cast<Actor>().ToList();
					if (LootLegendaries)
						targets.Concat(Body.GetObjectsInRange<Item>(40f).Where(m => ((Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).Cast<Actor>().ToList());
					if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position) < 80f)
					{
						_target = targets.First();
						//Logger.Trace("MoveToTargetWithPathfindAction to target");
						CurrentAction = new MoveToPointAction(Body, _target.Position);
					}
					else
					{
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
