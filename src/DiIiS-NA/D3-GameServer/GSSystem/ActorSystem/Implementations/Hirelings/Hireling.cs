//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings
{
	public class Hireling : InteractiveNPC, IUpdateable
	{
		protected ActorSno mainSNO = ActorSno.__NONE;
		protected ActorSno hirelingSNO = ActorSno.__NONE;
		protected ActorSno proxySNO = ActorSno.__NONE;
		protected int skillKit = -1;
		protected int hirelingGBID = -1;

		protected Player owner = null;
		// Resource generation timing
		private int _lastResourceUpdateTick;

		public bool IsProxy { get { return SNO == proxySNO; } }
		public bool IsHireling { get { return SNO == hirelingSNO; } }
		public bool HasHireling { get { return this.hirelingSNO != ActorSno.__NONE; } }
		public bool HasProxy { get { return this.proxySNO != ActorSno.__NONE; } }
		public int PetType { get { return IsProxy ? 22 : 0; } }
		private Dictionary<Player, Dictionary<int, Item>> _equipment = new Dictionary<Player, Dictionary<int, Item>>();

		public Hireling(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.TeamID] = 2;
			Interactions.Add(new HireInteraction());
			Interactions.Add(new InventoryInteraction());
			if (skillKit != -1)
				this.Attributes[GameAttribute.SkillKit] = skillKit;
		}

		public void SetUpAttributes(Player player)
		{
			this.owner = player;

			var info = player.HirelingInfo[this.Attributes[GameAttribute.Hireling_Class]];
			//*
			// TODO: fix this hardcoded crap
			if (!IsProxy)
				this.Attributes[GameAttribute.Buff_Visual_Effect, 0x000FFFFF] = true;

			this.Attributes[GameAttribute.Level] = player.Level;
			this.Attributes[GameAttribute.Experience_Next_Lo] = 0;

			if (!IsHireling && !IsProxy) // original doesn't need more attribs
				return;

			if (info.Skill1SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill1SNOId] = 1;
				this.Attributes[GameAttribute.Skill, info.Skill1SNOId] = 1;
			}

			if (info.Skill2SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill2SNOId] = 1;
				this.Attributes[GameAttribute.Skill, info.Skill2SNOId] = 1;
			}

			if (info.Skill3SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill3SNOId] = 1;
				this.Attributes[GameAttribute.Skill, info.Skill3SNOId] = 1;
			}

			if (info.Skill4SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill4SNOId] = 1;
				this.Attributes[GameAttribute.Skill, info.Skill4SNOId] = 1;
			}

			/**/
			this._lastResourceUpdateTick = 0;
			this.Attributes[GameAttribute.SkillKit] = skillKit;
			this.WalkSpeed = 0.45f;
			
			#region hardcoded attribs :/
			//*
			this.Attributes[GameAttribute.Attacks_Per_Second] = 1f;
			this.Attributes[GameAttribute.Attacks_Per_Second_Item] = 1.199219f;
			this.Attributes[GameAttribute.Casting_Speed] = 1;
			this.Attributes[GameAttribute.Damage_Delta, 0] = 1f;
			this.Attributes[GameAttribute.Damage_Min, 0] = 1f;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 2f;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 6f;
			this.Attributes[GameAttribute.General_Cooldown] = 0;
			this.Attributes[GameAttribute.Hit_Chance] = 1;
			this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(this.Attributes[GameAttribute.Level] - 35, 0);
			this.Attributes[GameAttribute.Hitpoints_Max] = 276f;
			this.Attributes[GameAttribute.Hitpoints_Cur] = 1f;
			this.Attributes[GameAttribute.Level_Cap] = 70;
			this.Attributes[GameAttribute.Movement_Scalar] = 1;
			this.Attributes[GameAttribute.Resource_Max, 0] = 1.0f;
			this.Attributes[GameAttribute.Resource_Cur, 0] = 1.0f;
			this.Attributes[GameAttribute.Resource_Type_Primary] = 0;
			this.Attributes[GameAttribute.Running_Rate] = 0.3598633f;
			this.Attributes[GameAttribute.Sprinting_Rate] = 0.3598633f;
			this.Attributes[GameAttribute.Strafing_Rate] = 0.1799316f;
			this.Attributes[GameAttribute.Walking_Rate] = 0.3598633f;

			if (IsProxy)
				return;

			this.Attributes[GameAttribute.Callout_Cooldown, 0x000FFFFF] = 0x00000797;
			this.Attributes[GameAttribute.Buff_Visual_Effect, 0x000FFFFF] = true;
			this.Attributes[GameAttribute.Buff_Icon_Count0, 0x000075C1] = 1;
			this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x000075C1] = true;
			this.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
			this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x20c51] = true;
			this.Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00020C51] = 0x00000A75;
			this.Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00020C51] = 0x00000375;
			this.Attributes[GameAttribute.Buff_Icon_Count0, 0x00020C51] = 3;
			this.Attributes[GameAttribute.Callout_Cooldown, 0x1618a] = 743;
			this.Attributes[GameAttribute.Callout_Cooldown, 0x01CAB6] = 743;
			//*/
			#endregion

		}

		public virtual Hireling CreateHireling(MapSystem.World world, ActorSno sno, TagMap tags)
		{
			throw new NotImplementedException();
		}

		public void UpdateAttributes()
		{
			if (!this.IsHireling || this.owner == null)
				return;
			//*
			try
			{
				this.Attributes[GameAttribute.Vitality] = 5f + (this.Attributes[GameAttribute.Level] * 2) + (this.GetItemBonus(GameAttribute.Vitality_Item));// * 2.5f);
				this.Attributes[GameAttribute.Strength] = 5f + (this.Attributes[GameAttribute.Level] * (this is Templar ? 3 : 1)) + (this.GetItemBonus(GameAttribute.Strength_Item));// * 2.5f);
				this.Attributes[GameAttribute.Dexterity] = 5f + (this.Attributes[GameAttribute.Level] * (this is Scoundrel ? 3 : 1)) + (this.GetItemBonus(GameAttribute.Dexterity_Item));// * 2.5f);
				this.Attributes[GameAttribute.Intelligence] = 5f + (this.Attributes[GameAttribute.Level] * (this is Enchantress ? 3 : 1)) + (this.GetItemBonus(GameAttribute.Intelligence_Item));// * 2.5f);

				this.Attributes[GameAttribute.Attacks_Per_Second_Item] = this.GetItemBonus(GameAttribute.Attacks_Per_Second_Item);
				//*
				this.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] = this.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Capped);
				this.Attributes[GameAttribute.Weapon_Crit_Chance] = this.GetItemBonus(GameAttribute.Weapon_Crit_Chance);
				this.Attributes[GameAttribute.Crit_Damage_Percent] = 0.5f + this.GetItemBonus(GameAttribute.Crit_Damage_Percent);
				this.Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] = this.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Uncapped);

				this.Attributes[GameAttribute.Armor_Item] = this.GetItemBonus(GameAttribute.Armor_Item);
				//*
				for (int i = 0; i < 7; i++)
				{
					this.Attributes[GameAttribute.Damage_Weapon_Min, i] = Math.Max(this.GetItemBonus(GameAttribute.Damage_Weapon_Min, i), 2f) + this.GetItemBonus(GameAttribute.Damage_Min, i);
					this.Attributes[GameAttribute.Damage_Weapon_Delta, i] = Math.Max(this.GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, i), 2f) + this.GetItemBonus(GameAttribute.Damage_Delta, i);
					this.Attributes[GameAttribute.Damage_Weapon_Bonus_Min, i] = this.GetItemBonus(GameAttribute.Damage_Weapon_Bonus_Min, i);
					this.Attributes[GameAttribute.Damage_Weapon_Bonus_Delta, i] = this.GetItemBonus(GameAttribute.Damage_Weapon_Bonus_Delta, i);
					this.Attributes[GameAttribute.Resistance, i] = this.GetItemBonus(GameAttribute.Resistance, i);
				}
				//*/
				this.Attributes[GameAttribute.Resistance_All] = this.GetItemBonus(GameAttribute.Resistance_All);
				this.Attributes[GameAttribute.Resistance_Percent_All] = this.GetItemBonus(GameAttribute.Resistance_Percent_All);
				this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] = this.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Melee);
				this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] = this.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Ranged);

				this.Attributes[GameAttribute.Thorns_Fixed] = this.GetItemBonus(GameAttribute.Thorns_Fixed, 0);

				this.Attributes[GameAttribute.Steal_Health_Percent] = this.GetItemBonus(GameAttribute.Steal_Health_Percent);
				this.Attributes[GameAttribute.Hitpoints_On_Hit] = this.GetItemBonus(GameAttribute.Hitpoints_On_Hit);
				this.Attributes[GameAttribute.Hitpoints_On_Kill] = this.GetItemBonus(GameAttribute.Hitpoints_On_Kill);

				this.Attributes[GameAttribute.Magic_Find] = this.GetItemBonus(GameAttribute.Magic_Find);
				this.Attributes[GameAttribute.Gold_Find] = this.GetItemBonus(GameAttribute.Gold_Find);

				this.Attributes[GameAttribute.Dodge_Chance_Bonus] = this.GetItemBonus(GameAttribute.Dodge_Chance_Bonus);
				
				this.Attributes[GameAttribute.Block_Amount_Item_Min] = this.GetItemBonus(GameAttribute.Block_Amount_Item_Min);
				this.Attributes[GameAttribute.Block_Amount_Item_Delta] = this.GetItemBonus(GameAttribute.Block_Amount_Item_Delta);
				this.Attributes[GameAttribute.Block_Amount_Bonus_Percent] = this.GetItemBonus(GameAttribute.Block_Amount_Bonus_Percent);
				this.Attributes[GameAttribute.Block_Chance] = this.GetItemBonus(GameAttribute.Block_Chance_Item_Total);
				//*/
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Item] = this.GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus_Item);
				this.Attributes[GameAttribute.Hitpoints_Max_Bonus] = this.GetItemBonus(GameAttribute.Hitpoints_Max_Bonus);
				this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(this.Attributes[GameAttribute.Level] - 35, 0);
				this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = this.GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second) + 10f + (10f * this.Attributes[GameAttribute.Level]);

				this.Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
				this.Attributes[GameAttribute.Hitpoints_Max] = 276f; //+ (this.Attributes[GameAttribute.Vitality] * (10f + Math.Max(this.Attributes[GameAttribute.Level] - 35, 0)));
				this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
				/**/
			}
			catch { }

			this.Attributes.BroadcastChangedIfRevealed();
			/**/
		}

		public override void OnHire(Player player)
		{
			if (hirelingSNO == ActorSno.__NONE)
				return;

			if (this.World.Game.Players.Count > 1) return;

			if (IsHireling || IsProxy)
				return; // This really shouldn't happen.. /fasbat

			this.Unreveal(player);
			var hireling = CreateHireling(this.World, hirelingSNO, this.Tags);
			hireling.SetUpAttributes(player);
			hireling.GBHandle.Type = 4;
			hireling.GBHandle.GBID = hirelingGBID;

			hireling.Attributes[GameAttribute.Pet_Creator] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Pet_Type] = 1;
			hireling.Attributes[GameAttribute.Pet_Owner] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Untargetable] = false;
			hireling.Attributes[GameAttribute.NPC_Is_Escorting] = true;

			hireling.RotationW = this.RotationW;
			hireling.RotationAxis = this.RotationAxis;

			//hireling.Brain.DeActivate();
			hireling.EnterWorld(this.Position);
			hireling.Brain = new HirelingBrain(hireling, player);
			//(hireling.Brain as HirelingBrain).Activate();
			player.ActiveHireling = hireling;
			//this.Destroy();
			player.SelectedNPC = null;
		}

		public override void OnInventory(Player player, HirelingInventoryMessage message)
		{

			switch (message.Id)
			{
				case 401:
					
					break;
			}
			if (player.ActiveHireling == null)
				return;
			if (proxySNO == ActorSno.__NONE)
				return;

			if (IsHireling || IsProxy)
				return;

			if (player.ActiveHireling.Attributes[GameAttribute.Hireling_Class] == this.Attributes[GameAttribute.Hireling_Class])
				return;

			var hireling = CreateHireling(this.World, proxySNO, this.Tags);
			hireling.SetUpAttributes(player);
			hireling.GBHandle.Type = 4;
			hireling.GBHandle.GBID = hirelingGBID;
			hireling.Attributes[GameAttribute.Is_NPC] = false;
			hireling.Attributes[GameAttribute.NPC_Is_Operatable] = false;
			hireling.Attributes[GameAttribute.NPC_Has_Interact_Options, 0] = false;
			hireling.Attributes[GameAttribute.Buff_Visual_Effect, 0x00FFFFF] = false;

			hireling.Attributes[GameAttribute.Pet_Creator] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Pet_Type] = 1;
			hireling.Attributes[GameAttribute.Pet_Owner] = player.PlayerIndex + 1;

			hireling.RotationW = this.RotationW;
			hireling.RotationAxis = this.RotationAxis;

			hireling.EnterWorld(this.Position);
		}

		public void Dismiss()
		{
			this.Destroy();
		}

		public void Update(int tickCounter)
		{
			if (this.Brain == null)
				return;

			if (!this.Dead)
				this.Brain.Update(tickCounter);

			if (this.World.Game.TickCounter % 30 == 0 && !this.Dead)
			{
				float tickSeconds = 1f / 60f * (this.World.Game.TickCounter - _lastResourceUpdateTick);
				_lastResourceUpdateTick = this.World.Game.TickCounter;
				float quantity = tickSeconds * this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second];

				this.AddHP(quantity);
			}
		}

		public override void AddHP(float quantity, bool GuidingLight = false)
		{
			if (this.Dead) return;
			if (quantity == 0) return;
			if (quantity > 0)
			{
				if (this.Attributes[GameAttribute.Hitpoints_Cur] < this.Attributes[GameAttribute.Hitpoints_Max_Total])
				{
					this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
						this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
						this.Attributes[GameAttribute.Hitpoints_Max_Total]);

					this.Attributes.BroadcastChangedIfRevealed();
				}
			}
		}

		public VisualInventoryMessage GetVisualEquipment(Player player)
		{
			return new VisualInventoryMessage()
			{
				ActorID = this.DynamicID(player),
				EquipmentList = new VisualEquipment()
				{
					Equipment = new VisualItem[]
					{
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
						new VisualItem() //weapon
						{
							GbId = (_equipment[player].ContainsKey(21) ? _equipment[player][21].GBHandle.GBID : -1),
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = -1,
						},
						new VisualItem() //offhand
						{
							GbId = (_equipment[player].ContainsKey(22) ? _equipment[player][22].GBHandle.GBID : -1),
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = -1,
						},
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
						new VisualItem()
						{
							GbId = -1,
							DyeType = 0,
							ItemEffectType = 0,
							EffectLevel = 0,
						},
					}
				}
			};
		}

		public override bool Reveal(Player player)
		{
			if (this.World.Game.PvP)
				return false;

			if (this.World.Game.Players.Count > 1)
				return false;

			if (!IsHireling && ((player.ActiveHireling != null && this.Attributes[GameAttribute.Hireling_Class] == player.ActiveHireling.Attributes[GameAttribute.Hireling_Class])))// || (player.HirelingId != null && this.Attributes[GameAttribute.Hireling_Class] == player.HirelingId)))
				return false;

			if (owner == null)
				SetUpAttributes(player);
			else if (IsProxy && owner != player)
				return false;

			if (!base.Reveal(player))
				return false;

			if (!_equipment.ContainsKey(player))
			{
				this.LoadInventory(player);
			}

			foreach (var item in this._equipment[player].Values)
				item.Reveal(player);

			player.InGameClient.SendMessage(GetVisualEquipment(player));

			if (this.IsHireling && owner != null && owner == player)
				player.InGameClient.SendMessage(new PetMessage() //70-77
				{
					Owner = player.PlayerIndex,
					Index = 10,
					PetId = this.DynamicID(player),
					Type = this.SNO == ActorSno._x1_malthael_npc ? 29 : 0,
				});

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			foreach (var item in this._equipment[player].Values)
				item.Unreveal(player);
			return true;
		}

		public void LoadInventory(Player player)
		{
			this._equipment.Add(player, new Dictionary<int, Item>());
			var inventory_list = this.World.Game.GameDBSession.SessionQueryWhere<DBInventory>(dbi => dbi.DBToon.Id == player.Toon.PersistentID && dbi.HirelingId != 0 && dbi.HirelingId == this.Attributes[GameAttribute.Hireling_Class]);
			foreach (var inv_item in inventory_list)
			{
				Item item = ItemGenerator.LoadFromDB(player, inv_item);
				item.Owner = this;
				item.Attributes[GameAttribute.Item_Equipped] = true;
				item.SetInventoryLocation(inv_item.EquipmentSlot, 0, 0);
				if (!this._equipment[player].ContainsKey(inv_item.EquipmentSlot))
					this._equipment[player].Add(inv_item.EquipmentSlot, item);
				//Logger.Info("Item {0} added to hireling equipment", inv_item.GbId);
			}

			this.UpdateAttributes();
		}

		public void EquipItem(Player owner, int slot, Item item)
		{
			if (item.DBInventory == null) return;

			if (this._equipment[owner].ContainsKey(slot))
				this.UnequipItem(owner, slot, item);

			item.Owner = this;
			item.SetInventoryLocation(slot, 0, 0);
			item.DBInventory.HirelingId = this.Attributes[GameAttribute.Hireling_Class];
			item.DBInventory.EquipmentSlot = slot;
			item.DBInventory.LocationX = 0;
			item.DBInventory.LocationY = 0;
			this.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
			item.Attributes[GameAttribute.Item_Equipped] = true;
			this._equipment[owner].Add(slot, item);
			this.RefreshEquipment(owner);
			this.UpdateAttributes();

			if (this._equipment[owner].Count >= 6)
				owner.GrantAchievement(74987243307149);
		}

		public void UnequipItem(Player owner, int slot, Item item)
		{
			if (item.DBInventory == null) return;

			if (!this._equipment[owner].ContainsKey(slot)) return;

			item.Owner = owner;
			this._equipment[owner].Remove(slot);
			this.World.Game.GameDBSession.SessionDelete(item.DBInventory);
			owner.Inventory.PickUp(item);
			item.Unreveal(owner);
			item.Attributes[GameAttribute.Item_Equipped] = false;
			item.Reveal(owner);
			this.RefreshEquipment(owner);
			this.UpdateAttributes();
		}

		public void UnequipItemFromSlot(Player owner, int slot)
		{
			if (!this._equipment[owner].ContainsKey(slot)) return;
			var item = this._equipment[owner][slot];
			this.UnequipItem(owner, slot, item);
		}

		public Item GetItemByDynId(Player player, uint DynamicId)
		{
			if (this._equipment[player].Values.Where(it => it.IsRevealedToPlayer(player) && it.DynamicID(player) == DynamicId).Count() > 0)
				return this._equipment[player].Values.Single(it => it.IsRevealedToPlayer(player) && it.DynamicID(player) == DynamicId);
			else
				return null;
		}

		public void RefreshEquipment(Player player)
		{
			foreach (var item in this._equipment[player].Values)
				item.Unreveal(player);
			foreach (var item in this._equipment[player].Values)
				item.Reveal(player);

			player.InGameClient.SendMessage(GetVisualEquipment(player));
		}

		#region EqupimentStats

		public List<Item> GetEquippedItems(Player player)
		{
			return this._equipment[player].Values.ToList();
		}

		public float GetItemBonus(GameAttributeF attributeF)
		{
			var stats = this.GetEquippedItems(this.owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0);

			if (attributeF == GameAttribute.Attacks_Per_Second_Item)
				return stats.Count() > 0 ? stats.Select(item => item.Attributes[attributeF]).Where(a => a > 0f).Aggregate(1f, (x, y) => x * y) : 0f;

			return stats.Sum(item => item.Attributes[attributeF]);
		}

		public int GetItemBonus(GameAttributeI attributeI)
		{
			return this.GetEquippedItems(this.owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI]);
		}

		public bool GetItemBonus(GameAttributeB attributeB)
		{
			return this.GetEquippedItems(this.owner).Where(item => item.Attributes[attributeB] == true).Count() > 0;
		}

		public float GetItemBonus(GameAttributeF attributeF, int attributeKey)
		{
			return this.GetEquippedItems(this.owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeF, attributeKey]);
		}

		public int GetItemBonus(GameAttributeI attributeI, int attributeKey)
		{
			return this.GetEquippedItems(this.owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI, attributeKey]);
		}
		#endregion
	}
}
