//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
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
			this.LootLegendaries = lootsLegs;
			this.PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						this.PresetPowers.Add(monsterSkill.SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = 1f });
					}
				}
			}
		}

		public override void Update(int tickCounter)
		{
			base.Update(tickCounter);

			List<Item> gold = this.Body.GetObjectsInRange<Item>(5f).Where(m => ((this.Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsGold((m as Item).ItemType)).ToList();
			foreach (var item in gold)
			{
				((this.Body as Minion).Master as Player).InGameClient.SendMessage(new FloatingAmountMessage()
				{
					Place = new WorldPlace()
					{
						Position = this.Body.Position,
						WorldID = this.Body.World.GlobalID,
					},

					Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
					Type = FloatingAmountMessage.FloatType.Gold,
				});

				((this.Body as Minion).Master as Player).Inventory.PickUpGold(item);
				((this.Body as Minion).Master as Player).GroundItems.Remove(item.GlobalID);
				item.Destroy();
			}

			if (this.LootLegendaries)
			{
				List<Item> legendaries = this.Body.GetObjectsInRange<Item>(5f).Where(m => ((this.Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).ToList();
				foreach (var item in legendaries)
				{
					((this.Body as Minion).Master as Player).Inventory.PickUp(item);
				}
			}

			List<Item> shards = this.Body.GetObjectsInRange<Item>(5f).Where(m => ((this.Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsBloodShard((m as Item).ItemType)).ToList();
			foreach (var item in shards)
			{
				((this.Body as Minion).Master as Player).InGameClient.SendMessage(new FloatingAmountMessage()
				{
					Place = new WorldPlace()
					{
						Position = this.Body.Position,
						WorldID = this.Body.World.GlobalID,
					},

					Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
					Type = FloatingAmountMessage.FloatType.BloodStone,
				});

				((this.Body as Minion).Master as Player).Inventory.PickUpBloodShard(item);
				((this.Body as Minion).Master as Player).GroundItems.Remove(item.GlobalID);
				item.Destroy();
			}
		}

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;
			if ((this.Body as Minion).Master == null) return;

			if (this.Body.World.Game.Paused) return;

			// select and start executing a power if no active action
			if (this.CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(this.Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					List<Actor> targets = this.Body.GetObjectsInRange<Item>(40f).Where(m => ((this.Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && Item.IsGold((m as Item).ItemType)).Cast<Actor>().ToList();
					if (this.LootLegendaries)
						targets.Concat(this.Body.GetObjectsInRange<Item>(40f).Where(m => ((this.Body as Minion).Master as Player).GroundItems.ContainsKey(m.GlobalID) && (m as Item).ItemDefinition.Name.Contains("Unique_")).Cast<Actor>().ToList());
					if (targets.Count != 0 && PowerMath.Distance2D(this.Body.Position, (this.Body as Minion).Master.Position) < 80f)
					{
						_target = targets.First();
						//Logger.Trace("MoveToTargetWithPathfindAction to target");
						this.CurrentAction = new MoveToPointAction(this.Body, _target.Position);
					}
					else
					{
						var distToMaster = PowerMath.Distance2D(this.Body.Position, (this.Body as Minion).Master.Position);
						if ((distToMaster > 8f) || (distToMaster < 3f))
						{
							var Rand = FastRandom.Instance;
							var position = (this.Body as Minion).Master.Position;
							float angle = (float)(Rand.NextDouble() * Math.PI * 2);
							float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
							var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
							this.CurrentAction = new MoveToPointAction(this.Body, near);
						}
					}
				}
			}
		}
	}
}
