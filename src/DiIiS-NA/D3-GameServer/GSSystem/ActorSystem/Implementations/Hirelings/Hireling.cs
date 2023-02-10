using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
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
			Attributes[GameAttributes.TeamID] = 2;
			Interactions.Add(new HireInteraction());
			Interactions.Add(new InventoryInteraction());
			if (skillKit != -1)
				Attributes[GameAttributes.SkillKit] = skillKit;
		}

		public void SetUpAttributes(Player player)
		{
			owner = player;

			var info = player.HirelingInfo[Attributes[GameAttributes.Hireling_Class]];
			//*
			// TODO: fix this hardcoded crap
			if (!IsProxy)
				Attributes[GameAttributes.Buff_Visual_Effect, 0x000FFFFF] = true;

			Attributes[GameAttributes.Level] = player.Level;
			Attributes[GameAttributes.Experience_Next_Lo] = 0;

			if (!IsHireling && !IsProxy) // original doesn't need more attribs
				return;

			if (info.Skill1SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill1SNOId] = 1;
				Attributes[GameAttributes.Skill, info.Skill1SNOId] = 1;
			}

			if (info.Skill2SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill2SNOId] = 1;
				Attributes[GameAttributes.Skill, info.Skill2SNOId] = 1;
			}

			if (info.Skill3SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill3SNOId] = 1;
				Attributes[GameAttributes.Skill, info.Skill3SNOId] = 1;
			}

			if (info.Skill4SNOId != -1)
			{
				//scripted //this.Attributes[GameAttribute.Skill_Total, info.Skill4SNOId] = 1;
				Attributes[GameAttributes.Skill, info.Skill4SNOId] = 1;
			}

			/**/
			_lastResourceUpdateTick = 0;
			Attributes[GameAttributes.SkillKit] = skillKit;
			WalkSpeed = 0.45f;
			
			#region hardcoded attribs :/
			//*
			Attributes[GameAttributes.Attacks_Per_Second] = 1f;
			Attributes[GameAttributes.Attacks_Per_Second_Item] = 1.199219f;
			Attributes[GameAttributes.Casting_Speed] = 1;
			Attributes[GameAttributes.Damage_Delta, 0] = 1f;
			Attributes[GameAttributes.Damage_Min, 0] = 1f;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 2f;
			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 6f;
			Attributes[GameAttributes.General_Cooldown] = 0;
			Attributes[GameAttributes.Hit_Chance] = 1;
			Attributes[GameAttributes.Hitpoints_Factor_Vitality] = 10f + Math.Max(Attributes[GameAttributes.Level] - 35, 0);
			Attributes[GameAttributes.Hitpoints_Max] = 276f;
			Attributes[GameAttributes.Hitpoints_Cur] = 1f;
			Attributes[GameAttributes.Level_Cap] = 70;
			Attributes[GameAttributes.Movement_Scalar] = 1;
			Attributes[GameAttributes.Resource_Max, 0] = 1.0f;
			Attributes[GameAttributes.Resource_Cur, 0] = 1.0f;
			Attributes[GameAttributes.Resource_Type_Primary] = 0;
			Attributes[GameAttributes.Running_Rate] = 0.3598633f;
			Attributes[GameAttributes.Sprinting_Rate] = 0.3598633f;
			Attributes[GameAttributes.Strafing_Rate] = 0.1799316f;
			Attributes[GameAttributes.Walking_Rate] = 0.3598633f;

			if (IsProxy)
				return;

			Attributes[GameAttributes.Callout_Cooldown, 0x000FFFFF] = 0x00000797;
			Attributes[GameAttributes.Buff_Visual_Effect, 0x000FFFFF] = true;
			Attributes[GameAttributes.Buff_Icon_Count0, 0x000075C1] = 1;
			Attributes[GameAttributes.Buff_Exclusive_Type_Active, 0x000075C1] = true;
			Attributes[GameAttributes.Conversation_Icon, 0] = 1;
			Attributes[GameAttributes.Buff_Exclusive_Type_Active, 0x20c51] = true;
			Attributes[GameAttributes.Buff_Icon_End_Tick0, 0x00020C51] = 0x00000A75;
			Attributes[GameAttributes.Buff_Icon_Start_Tick0, 0x00020C51] = 0x00000375;
			Attributes[GameAttributes.Buff_Icon_Count0, 0x00020C51] = 3;
			Attributes[GameAttributes.Callout_Cooldown, 0x1618a] = 743;
			Attributes[GameAttributes.Callout_Cooldown, 0x01CAB6] = 743;
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
				Attributes[GameAttributes.Vitality] = 5f + (Attributes[GameAttributes.Level] * 2) + (GetItemBonus(GameAttributes.Vitality_Item));// * 2.5f);
				Attributes[GameAttributes.Strength] = 5f + (Attributes[GameAttributes.Level] * (this is Templar ? 3 : 1)) + (GetItemBonus(GameAttributes.Strength_Item));// * 2.5f);
				Attributes[GameAttributes.Dexterity] = 5f + (Attributes[GameAttributes.Level] * (this is Scoundrel ? 3 : 1)) + (GetItemBonus(GameAttributes.Dexterity_Item));// * 2.5f);
				Attributes[GameAttributes.Intelligence] = 5f + (Attributes[GameAttributes.Level] * (this is Enchantress ? 3 : 1)) + (GetItemBonus(GameAttributes.Intelligence_Item));// * 2.5f);

				Attributes[GameAttributes.Attacks_Per_Second_Item] = GetItemBonus(GameAttributes.Attacks_Per_Second_Item);
				//*
				Attributes[GameAttributes.Crit_Percent_Bonus_Capped] = GetItemBonus(GameAttributes.Crit_Percent_Bonus_Capped);
				Attributes[GameAttributes.Weapon_Crit_Chance] = GetItemBonus(GameAttributes.Weapon_Crit_Chance);
				Attributes[GameAttributes.Crit_Damage_Percent] = 0.5f + GetItemBonus(GameAttributes.Crit_Damage_Percent);
				Attributes[GameAttributes.Crit_Percent_Bonus_Uncapped] = GetItemBonus(GameAttributes.Crit_Percent_Bonus_Uncapped);

				Attributes[GameAttributes.Armor_Item] = GetItemBonus(GameAttributes.Armor_Item);
				//*
				for (int i = 0; i < 7; i++)
				{
					Attributes[GameAttributes.Damage_Weapon_Min, i] = Math.Max(GetItemBonus(GameAttributes.Damage_Weapon_Min, i), 2f) + GetItemBonus(GameAttributes.Damage_Min, i);
					Attributes[GameAttributes.Damage_Weapon_Delta, i] = Math.Max(GetItemBonus(GameAttributes.Damage_Weapon_Delta_Total, i), 2f) + GetItemBonus(GameAttributes.Damage_Delta, i);
					Attributes[GameAttributes.Damage_Weapon_Bonus_Min, i] = GetItemBonus(GameAttributes.Damage_Weapon_Bonus_Min, i);
					Attributes[GameAttributes.Damage_Weapon_Bonus_Delta, i] = GetItemBonus(GameAttributes.Damage_Weapon_Bonus_Delta, i);
					Attributes[GameAttributes.Resistance, i] = GetItemBonus(GameAttributes.Resistance, i);
				}
				//*/
				Attributes[GameAttributes.Resistance_All] = GetItemBonus(GameAttributes.Resistance_All);
				Attributes[GameAttributes.Resistance_Percent_All] = GetItemBonus(GameAttributes.Resistance_Percent_All);
				Attributes[GameAttributes.Damage_Percent_Reduction_From_Melee] = GetItemBonus(GameAttributes.Damage_Percent_Reduction_From_Melee);
				Attributes[GameAttributes.Damage_Percent_Reduction_From_Ranged] = GetItemBonus(GameAttributes.Damage_Percent_Reduction_From_Ranged);

				Attributes[GameAttributes.Thorns_Fixed] = GetItemBonus(GameAttributes.Thorns_Fixed, 0);

				Attributes[GameAttributes.Steal_Health_Percent] = GetItemBonus(GameAttributes.Steal_Health_Percent);
				Attributes[GameAttributes.Hitpoints_On_Hit] = GetItemBonus(GameAttributes.Hitpoints_On_Hit);
				Attributes[GameAttributes.Hitpoints_On_Kill] = GetItemBonus(GameAttributes.Hitpoints_On_Kill);

				Attributes[GameAttributes.Magic_Find] = GetItemBonus(GameAttributes.Magic_Find);
				Attributes[GameAttributes.Gold_Find] = GetItemBonus(GameAttributes.Gold_Find);

				Attributes[GameAttributes.Dodge_Chance_Bonus] = GetItemBonus(GameAttributes.Dodge_Chance_Bonus);
				
				Attributes[GameAttributes.Block_Amount_Item_Min] = GetItemBonus(GameAttributes.Block_Amount_Item_Min);
				Attributes[GameAttributes.Block_Amount_Item_Delta] = GetItemBonus(GameAttributes.Block_Amount_Item_Delta);
				Attributes[GameAttributes.Block_Amount_Bonus_Percent] = GetItemBonus(GameAttributes.Block_Amount_Bonus_Percent);
				Attributes[GameAttributes.Block_Chance] = GetItemBonus(GameAttributes.Block_Chance_Item_Total);
				//*/
				Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Item] = GetItemBonus(GameAttributes.Hitpoints_Max_Percent_Bonus_Item);
				Attributes[GameAttributes.Hitpoints_Max_Bonus] = GetItemBonus(GameAttributes.Hitpoints_Max_Bonus);
				Attributes[GameAttributes.Hitpoints_Factor_Vitality] = 10f + Math.Max(Attributes[GameAttributes.Level] - 35, 0);
				Attributes[GameAttributes.Hitpoints_Regen_Per_Second] = GetItemBonus(GameAttributes.Hitpoints_Regen_Per_Second) + 10f + (10f * Attributes[GameAttributes.Level]);

				Attributes[GameAttributes.Core_Attributes_From_Item_Bonus_Multiplier] = 1;
				Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
				Attributes[GameAttributes.Hitpoints_Max] = 276f; //+ (this.Attributes[GameAttribute.Vitality] * (10f + Math.Max(this.Attributes[GameAttribute.Level] - 35, 0)));
				Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
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

			hireling.Attributes[GameAttributes.Pet_Creator] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttributes.Pet_Type] = 1;
			hireling.Attributes[GameAttributes.Pet_Owner] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttributes.Untargetable] = false;
			hireling.Attributes[GameAttributes.NPC_Is_Escorting] = true;

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

			if (player.ActiveHireling.Attributes[GameAttributes.Hireling_Class] == Attributes[GameAttributes.Hireling_Class])
				return;

			var hireling = CreateHireling(World, proxySNO, Tags);
			hireling.SetUpAttributes(player);
			hireling.GBHandle.Type = 4;
			hireling.GBHandle.GBID = hirelingGBID;
			hireling.Attributes[GameAttributes.Is_NPC] = false;
			hireling.Attributes[GameAttributes.NPC_Is_Operatable] = false;
			hireling.Attributes[GameAttributes.NPC_Has_Interact_Options, 0] = false;
			hireling.Attributes[GameAttributes.Buff_Visual_Effect, 0x00FFFFF] = false;

			hireling.Attributes[GameAttributes.Pet_Creator] = player.PlayerIndex + 1;
			hireling.Attributes[GameAttributes.Pet_Type] = 1;
			hireling.Attributes[GameAttributes.Pet_Owner] = player.PlayerIndex + 1;

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
				float quantity = tickSeconds * Attributes[GameAttributes.Hitpoints_Regen_Per_Second];

				AddHP(quantity);
			}
		}

		public override void AddHP(float quantity, bool guidingLight = false)
		{
			if (Dead) return;
			if (quantity == 0) return;
			if (quantity > 0)
			{
				if (Attributes[GameAttributes.Hitpoints_Cur] < Attributes[GameAttributes.Hitpoints_Max_Total])
				{
					Attributes[GameAttributes.Hitpoints_Cur] = Math.Min(
						Attributes[GameAttributes.Hitpoints_Cur] + quantity,
						Attributes[GameAttributes.Hitpoints_Max_Total]);

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

			if (!IsHireling && ((player.ActiveHireling != null && Attributes[GameAttributes.Hireling_Class] == player.ActiveHireling.Attributes[GameAttributes.Hireling_Class])))// || (player.HirelingId != null && this.Attributes[GameAttribute.Hireling_Class] == player.HirelingId)))
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
			var inventory_list = World.Game.GameDbSession.SessionQueryWhere<DBInventory>(dbi => dbi.DBToon.Id == player.Toon.PersistentID && dbi.HirelingId != 0 && dbi.HirelingId == Attributes[GameAttributes.Hireling_Class]);
			foreach (var inv_item in inventory_list)
			{
				Item item = ItemGenerator.LoadFromDB(player, inv_item);
				item.Owner = this;
				item.Attributes[GameAttributes.Item_Equipped] = true;
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
			item.DBInventory.HirelingId = Attributes[GameAttributes.Hireling_Class];
			item.DBInventory.EquipmentSlot = slot;
			item.DBInventory.LocationX = 0;
			item.DBInventory.LocationY = 0;
			World.Game.GameDbSession.SessionUpdate(item.DBInventory);
			item.Attributes[GameAttributes.Item_Equipped] = true;
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
			World.Game.GameDbSession.SessionDelete(item.DBInventory);
			owner.Inventory.PickUp(item);
			item.Unreveal(owner);
			item.Attributes[GameAttributes.Item_Equipped] = false;
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

		public IEnumerable<Item> GetEquippedItems(Player player)
		{
			return _equipment[player].Values;
		}

		public float GetItemBonus(GameAttributeF attributeF)
		{
			var stats = GetEquippedItems(owner)
				.Where(item => item.Attributes[GameAttributes.Durability_Cur] > 0 ||
				               item.Attributes[GameAttributes.Durability_Max] == 0);

			if (attributeF == GameAttributes.Attacks_Per_Second_Item)
			{
				return stats.Any()
					? stats.Select(item => item.Attributes[attributeF]).Where(a => a > 0f).Aggregate(1f, (x, y) => x * y)
					: 0f;
			}
			return stats.Sum(item => item.Attributes[attributeF]);
		}

		public int GetItemBonus(GameAttributeI attributeI)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttributes.Durability_Cur] > 0 || item.Attributes[GameAttributes.Durability_Max] == 0).Sum(item => item.Attributes[attributeI]);
		}

		public bool GetItemBonus(GameAttributeB attributeB)
		{
			return GetEquippedItems(owner).Any(item => item.Attributes[attributeB]);
		}

		public float GetItemBonus(GameAttributeF attributeF, int attributeKey)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttributes.Durability_Cur] > 0 || item.Attributes[GameAttributes.Durability_Max] == 0).Sum(item => item.Attributes[attributeF, attributeKey]);
		}

		public int GetItemBonus(GameAttributeI attributeI, int attributeKey)
		{
			return GetEquippedItems(owner).Where(item => item.Attributes[GameAttributes.Durability_Cur] > 0 || item.Attributes[GameAttributes.Durability_Max] == 0).Sum(item => item.Attributes[attributeI, attributeKey]);
		}
		#endregion
	}
}
