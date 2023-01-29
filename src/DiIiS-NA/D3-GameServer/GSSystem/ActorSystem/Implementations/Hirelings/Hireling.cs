using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
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
		public bool HasHireling { get { return hirelingSNO != ActorSno.__NONE; } }
		public bool HasProxy { get { return proxySNO != ActorSno.__NONE; } }
		public int PetType { get { return IsProxy ? 22 : 0; } }
		private Dictionary<Player, Dictionary<int, Item>> _equipment = new Dictionary<Player, Dictionary<int, Item>>();

		public Hireling(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.TeamID] = 2;
			Interactions.Add(new HireInteraction());
			Interactions.Add(new InventoryInteraction());
			if (skillKit != -1)
				Attributes[GameAttribute.SkillKit] = skillKit;
		}

		public void SetUpAttributes(Player player)
		{
			owner = player;

			var info = player.HirelingInfo[Attributes[GameAttribute.Hireling_Class]];
			//*
			// TODO: fix this hardcoded crap
			if (!IsProxy)
				Attributes[GameAttribute.Buff_Visual_Effect, 0x000FFFFF] = true;

			Attributes[GameAttribute.Level] = player.Level;
			Attributes[GameAttribute.Experience_Next_Lo] = 0;

			if (!IsHireling && !IsProxy) // original doesn't need more attribs
				return;

			if (info.Skill1SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill1SNOId] = 1;
				Attributes[GameAttribute.Skill, info.Skill1SNOId] = 1;
			}

			if (info.Skill2SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill2SNOId] = 1;
				Attributes[GameAttribute.Skill, info.Skill2SNOId] = 1;
			}

			if (info.Skill3SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill3SNOId] = 1;
				Attributes[GameAttribute.Skill, info.Skill3SNOId] = 1;
			}

			if (info.Skill4SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill4SNOId] = 1;
				Attributes[GameAttribute.Skill, info.Skill4SNOId] = 1;
			}

			/**/
			_lastResourceUpdateTick = 0;
			Attributes[GameAttribute.SkillKit] = skillKit;
			WalkSpeed = 0.45f;
			
			#region hardcoded attribs :/
			//*
			Attributes[GameAttribute.Attacks_Per_Second] = 1f;
			Attributes[GameAttribute.Attacks_Per_Second_Item] = 1.199219f;
			Attributes[GameAttribute.Casting_Speed] = 1;
			Attributes[GameAttribute.Damage_Delta, 0] = 1f;
			Attributes[GameAttribute.Damage_Min, 0] = 1f;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 2f;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = 6f;
			Attributes[GameAttribute.General_Cooldown] = 0;
			Attributes[GameAttribute.Hit_Chance] = 1;
			Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(Attributes[GameAttribute.Level] - 35, 0);
			Attributes[GameAttribute.Hitpoints_Max] = 276f;
			Attributes[GameAttribute.Hitpoints_Cur] = 1f;
			Attributes[GameAttribute.Level_Cap] = 70;
			Attributes[GameAttribute.Movement_Scalar] = 1;
			Attributes[GameAttribute.Resource_Max, 0] = 1.0f;
			Attributes[GameAttribute.Resource_Cur, 0] = 1.0f;
			Attributes[GameAttribute.Resource_Type_Primary] = 0;
			Attributes[GameAttribute.Running_Rate] = 0.3598633f;
			Attributes[GameAttribute.Sprinting_Rate] = 0.3598633f;
			Attributes[GameAttribute.Strafing_Rate] = 0.1799316f;
			Attributes[GameAttribute.Walking_Rate] = 0.3598633f;

			if (IsProxy)
				return;

			Attributes[GameAttribute.Callout_Cooldown, 0x000FFFFF] = 0x00000797;
			Attributes[GameAttribute.Buff_Visual_Effect, 0x000FFFFF] = true;
			Attributes[GameAttribute.Buff_Icon_Count0, 0x000075C1] = 1;
			Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x000075C1] = true;
			Attributes[GameAttribute.Conversation_Icon, 0] = 1;
			Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x20c51] = true;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00020C51] = 0x00000A75;
			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00020C51] = 0x00000375;
			Attributes[GameAttribute.Buff_Icon_Count0, 0x00020C51] = 3;
			Attributes[GameAttribute.Callout_Cooldown, 0x1618a] = 743;
			Attributes[GameAttribute.Callout_Cooldown, 0x01CAB6] = 743;
			//*/
			#endregion

		}

		public virtual Hireling CreateHireling(MapSystem.World world, ActorSno sno, TagMap tags)
		{
			throw new NotImplementedException();
		}

		public void UpdateAttributes()
		{
			if (!IsHireling || owner == null)
				return;
			//*
			try
			{
				Attributes[GameAttribute.Vitality] = 5f + (Attributes[GameAttribute.Level] * 2) + (GetItemBonus(GameAttribute.Vitality_Item));// * 2.5f);
				Attributes[GameAttribute.Strength] = 5f + (Attributes[GameAttribute.Level] * (this is Templar ? 3 : 1)) + (GetItemBonus(GameAttribute.Strength_Item));// * 2.5f);
				Attributes[GameAttribute.Dexterity] = 5f + (Attributes[GameAttribute.Level] * (this is Scoundrel ? 3 : 1)) + (GetItemBonus(GameAttribute.Dexterity_Item));// * 2.5f);
				Attributes[GameAttribute.Intelligence] = 5f + (Attributes[GameAttribute.Level] * (this is Enchantress ? 3 : 1)) + (GetItemBonus(GameAttribute.Intelligence_Item));// * 2.5f);

				Attributes[GameAttribute.Attacks_Per_Second_Item] = GetItemBonus(GameAttribute.Attacks_Per_Second_Item);
				//*
				Attributes[GameAttribute.Crit_Percent_Bonus_Capped] = GetItemBonus(GameAttribute.Crit_Percent_Bonus_Capped);
				Attributes[GameAttribute.Weapon_Crit_Chance] = GetItemBonus(GameAttribute.Weapon_Crit_Chance);
				Attributes[GameAttribute.Crit_Damage_Percent] = 0.5f + GetItemBonus(GameAttribute.Crit_Damage_Percent);
				Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] = GetItemBonus(GameAttribute.Crit_Percent_Bonus_Uncapped);

				Attributes[GameAttribute.Armor_Item] = GetItemBonus(GameAttribute.Armor_Item);
				//*
				for (int i = 0; i < 7; i++)
				{
					Attributes[GameAttribute.Damage_Weapon_Min, i] = Math.Max(GetItemBonus(GameAttribute.Damage_Weapon_Min, i), 2f) + GetItemBonus(GameAttribute.Damage_Min, i);
					Attributes[GameAttribute.Damage_Weapon_Delta, i] = Math.Max(GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, i), 2f) + GetItemBonus(GameAttribute.Damage_Delta, i);
					Attributes[GameAttribute.Damage_Weapon_Bonus_Min, i] = GetItemBonus(GameAttribute.Damage_Weapon_Bonus_Min, i);
					Attributes[GameAttribute.Damage_Weapon_Bonus_Delta, i] = GetItemBonus(GameAttribute.Damage_Weapon_Bonus_Delta, i);
					Attributes[GameAttribute.Resistance, i] = GetItemBonus(GameAttribute.Resistance, i);
				}
				//*/
				Attributes[GameAttribute.Resistance_All] = GetItemBonus(GameAttribute.Resistance_All);
				Attributes[GameAttribute.Resistance_Percent_All] = GetItemBonus(GameAttribute.Resistance_Percent_All);
				Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] = GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Melee);
				Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] = GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Ranged);

				Attributes[GameAttribute.Thorns_Fixed] = GetItemBonus(GameAttribute.Thorns_Fixed, 0);

				Attributes[GameAttribute.Steal_Health_Percent] = GetItemBonus(GameAttribute.Steal_Health_Percent);
				Attributes[GameAttribute.Hitpoints_On_Hit] = GetItemBonus(GameAttribute.Hitpoints_On_Hit);
				Attributes[GameAttribute.Hitpoints_On_Kill] = GetItemBonus(GameAttribute.Hitpoints_On_Kill);

				Attributes[GameAttribute.Magic_Find] = GetItemBonus(GameAttribute.Magic_Find);
				Attributes[GameAttribute.Gold_Find] = GetItemBonus(GameAttribute.Gold_Find);

				Attributes[GameAttribute.Dodge_Chance_Bonus] = GetItemBonus(GameAttribute.Dodge_Chance_Bonus);
				
				Attributes[GameAttribute.Block_Amount_Item_Min] = GetItemBonus(GameAttribute.Block_Amount_Item_Min);
				Attributes[GameAttribute.Block_Amount_Item_Delta] = GetItemBonus(GameAttribute.Block_Amount_Item_Delta);
				Attributes[GameAttribute.Block_Amount_Bonus_Percent] = GetItemBonus(GameAttribute.Block_Amount_Bonus_Percent);
				Attributes[GameAttribute.Block_Chance] = GetItemBonus(GameAttribute.Block_Chance_Item_Total);
				//*/
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Item] = GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus_Item);
				Attributes[GameAttribute.Hitpoints_Max_Bonus] = GetItemBonus(GameAttribute.Hitpoints_Max_Bonus);
				Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(Attributes[GameAttribute.Level] - 35, 0);
				Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second) + 10f + (10f * Attributes[GameAttribute.Level]);

				Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
				Attributes[GameAttribute.Hitpoints_Max] = 276f; //+ (this.Attributes[GameAttribute.Vitality] * (10f + Math.Max(this.Attributes[GameAttribute.Level] - 35, 0)));
				Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
				/**/
			}
			catch { }

			Attributes.BroadcastChangedIfRevealed();
			/**/
		}

		public override void OnHire(Player player)
		{
			if (hirelingSNO == ActorSno.__NONE)
				return;

			if (World.Game.Players.Count > 1) return;

			if (IsHireling || IsProxy)
				return; // This really shouldn't happen.. /fasbat

			Unreveal(player);
			var hireling = CreateHireling(World, hirelingSNO, Tags);
			hireling.SetUpAttributes(player);
			hireling.GBHandle.Type = 4;
			hireling.GBHandle.GBID = hirelingGBID;

			hireling.Attributes[GameAttribute.Pet_Creator] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Pet_Type] = 1;
			hireling.Attributes[GameAttribute.Pet_Owner] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Untargetable] = false;
			hireling.Attributes[GameAttribute.NPC_Is_Escorting] = true;

			hireling.RotationW = RotationW;
			hireling.RotationAxis = RotationAxis;

			//hireling.Brain.DeActivate();
			hireling.EnterWorld(Position);
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

			if (player.ActiveHireling.Attributes[GameAttribute.Hireling_Class] == Attributes[GameAttribute.Hireling_Class])
				return;

			var hireling = CreateHireling(World, proxySNO, Tags);
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

			hireling.RotationW = RotationW;
			hireling.RotationAxis = RotationAxis;

			hireling.EnterWorld(Position);
		}

		public void Dismiss()
		{
			Destroy();
		}

		public void Update(int tickCounter)
		{
			if (Brain == null)
				return;

			if (!Dead)
				Brain.Update(tickCounter);

			if (World.Game.TickCounter % 30 == 0 && !Dead)
			{
				float tickSeconds = 1f / 60f * (World.Game.TickCounter - _lastResourceUpdateTick);
				_lastResourceUpdateTick = World.Game.TickCounter;
				float quantity = tickSeconds * Attributes[GameAttribute.Hitpoints_Regen_Per_Second];

				AddHP(quantity);
			}
		}

		public override void AddHP(float quantity, bool guidingLight = false)
		{
			if (Dead) return;
			if (quantity == 0) return;
			if (quantity > 0)
			{
				if (Attributes[GameAttribute.Hitpoints_Cur] < Attributes[GameAttribute.Hitpoints_Max_Total])
				{
					Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
						Attributes[GameAttribute.Hitpoints_Cur] + quantity,
						Attributes[GameAttribute.Hitpoints_Max_Total]);

					Attributes.BroadcastChangedIfRevealed();
				}
			}
		}

		public VisualInventoryMessage GetVisualEquipment(Player player)
		{
			return new VisualInventoryMessage()
			{
				ActorID = DynamicID(player),
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
			if (World.Game.PvP)
				return false;

			if (World.Game.Players.Count > 1)
				return false;

			if (!IsHireling && ((player.ActiveHireling != null && Attributes[GameAttribute.Hireling_Class] == player.ActiveHireling.Attributes[GameAttribute.Hireling_Class])))// || (player.HirelingId != null && this.Attributes[GameAttribute.Hireling_Class] == player.HirelingId)))
				return false;

			if (owner == null)
				SetUpAttributes(player);
			else if (IsProxy && owner != player)
				return false;

			if (!base.Reveal(player))
				return false;

			if (!_equipment.ContainsKey(player))
			{
				LoadInventory(player);
			}

			foreach (var item in _equipment[player].Values)
				item.Reveal(player);

			player.InGameClient.SendMessage(GetVisualEquipment(player));

			if (IsHireling && owner != null && owner == player)
				player.InGameClient.SendMessage(new PetMessage() //70-77
				{
					Owner = player.PlayerIndex,
					Index = 10,
					PetId = DynamicID(player),
					Type = SNO == ActorSno._x1_malthael_npc ? 29 : 0,
				});

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			foreach (var item in _equipment[player].Values)
				item.Unreveal(player);
			return true;
		}

		public void LoadInventory(Player player)
		{
			_equipment.Add(player, new Dictionary<int, Item>());
			var inventory_list = World.Game.GameDBSession.SessionQueryWhere<DBInventory>(dbi => dbi.DBToon.Id == player.Toon.PersistentID && dbi.HirelingId != 0 && dbi.HirelingId == Attributes[GameAttribute.Hireling_Class]);
			foreach (var inv_item in inventory_list)
			{
				Item item = ItemGenerator.LoadFromDB(player, inv_item);
				item.Owner = this;
				item.Attributes[GameAttribute.Item_Equipped] = true;
				item.SetInventoryLocation(inv_item.EquipmentSlot, 0, 0);
				if (!_equipment[player].ContainsKey(inv_item.EquipmentSlot))
					_equipment[player].Add(inv_item.EquipmentSlot, item);
				//Logger.Info("Item {0} added to hireling equipment", inv_item.GbId);
			}

			UpdateAttributes();
		}

		public void EquipItem(Player owner, int slot, Item item)
		{
			if (item.DBInventory == null) return;

			if (_equipment[owner].ContainsKey(slot))
				UnequipItem(owner, slot, item);

			item.Owner = this;
			item.SetInventoryLocation(slot, 0, 0);
			item.DBInventory.HirelingId = Attributes[GameAttribute.Hireling_Class];
			item.DBInventory.EquipmentSlot = slot;
			item.DBInventory.LocationX = 0;
			item.DBInventory.LocationY = 0;
			World.Game.GameDBSession.SessionUpdate(item.DBInventory);
			item.Attributes[GameAttribute.Item_Equipped] = true;
			_equipment[owner].Add(slot, item);
			RefreshEquipment(owner);
			UpdateAttributes();

			if (_equipment[owner].Count >= 6)
				owner.GrantAchievement(74987243307149);
		}

		public void UnequipItem(Player owner, int slot, Item item)
		{
			if (item.DBInventory == null) return;

			if (!_equipment[owner].ContainsKey(slot)) return;

			item.Owner = owner;
			_equipment[owner].Remove(slot);
			World.Game.GameDBSession.SessionDelete(item.DBInventory);
			owner.Inventory.PickUp(item);
			item.Unreveal(owner);
			item.Attributes[GameAttribute.Item_Equipped] = false;
			item.Reveal(owner);
			RefreshEquipment(owner);
			UpdateAttributes();
		}

		public void UnequipItemFromSlot(Player owner, int slot)
		{
			if (!_equipment[owner].ContainsKey(slot)) return;
			var item = _equipment[owner][slot];
			UnequipItem(owner, slot, item);
		}

		public Item GetItemByDynId(Player player, uint dynamicId)
		{
			if (_equipment[player].Values.Any(it => it.IsRevealedToPlayer(player) && it.DynamicID(player) == dynamicId))
				return _equipment[player].Values.Single(it => it.IsRevealedToPlayer(player) && it.DynamicID(player) == dynamicId);

			return null;
		}

		public void RefreshEquipment(Player player)
		{
			foreach (var item in _equipment[player].Values)
				item.Unreveal(player);
			foreach (var item in _equipment[player].Values)
				item.Reveal(player);

			player.InGameClient.SendMessage(GetVisualEquipment(player));
		}

		#region EqupimentStats

		public List<Item> GetEquippedItems(Player player)
		{
			return _equipment[player].Values.ToList();
		}

		public float GetItemBonus(GameAttributeF attributeF)
		{
			var stats = GetEquippedItems(owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0);

			if (attributeF == GameAttribute.Attacks_Per_Second_Item)
				return stats.Count() > 0 ? stats.Select(item => item.Attributes[attributeF]).Where(a => a > 0f).Aggregate(1f, (x, y) => x * y) : 0f;

			return stats.Sum(item => item.Attributes[attributeF]);
		}

		public int GetItemBonus(GameAttributeI attributeI)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI]);
		}

		public bool GetItemBonus(GameAttributeB attributeB)
		{
			return GetEquippedItems(owner).Any(item => item.Attributes[attributeB]);
		}

		public float GetItemBonus(GameAttributeF attributeF, int attributeKey)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeF, attributeKey]);
		}

		public int GetItemBonus(GameAttributeI attributeI, int attributeKey)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI, attributeKey]);
		}
		#endregion
	}
}
