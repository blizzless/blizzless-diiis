using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	class WeaponAttributeCreator : IItemAttributeCreator
	{
		public void CreateAttributes(Item item)
		{
			item.Attributes[GameAttribute.Skill, 0x7780] = 1;
			item.Attributes[GameAttribute.IdentifyCost] = 1;

			const float heroAttackspeed = 1.2f; // musst be calculated by Skills + passives + affixes  + ...
			const float heroMaxDmg = 50f; // musst be calculated by Skills + passives + affixes  + ...
			const float heroMinDmg = 10f; // musst be calculated by Skills + passives + affixes  + ...
			const float offhandMultiplier = 0.8f;

			var weaponDmg = (float)(RandomHelper.NextDouble() * 100) + 10;
			var attackspeed = (float)(RandomHelper.NextDouble() + 0.5);
			var minWeaponDmg = weaponDmg - ((float)(RandomHelper.NextDouble() * 20) + 10);

			item.Attributes[GameAttribute.Attacks_Per_Second_Total] = attackspeed * heroAttackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item] = attackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_Subtotal] = attackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_Total] = attackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_MainHand] = attackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_OffHand] = attackspeed + 1;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_Total_MainHand] = attackspeed;
			item.Attributes[GameAttribute.Attacks_Per_Second_Item_Total_OffHand] = attackspeed + 1;

			item.Attributes[GameAttribute.Damage_Weapon_Min, 0] = minWeaponDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0] = minWeaponDmg + heroMinDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0xFFFFF] = minWeaponDmg + heroMinDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total_All] = minWeaponDmg + heroMinDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total_MainHand, 0] = minWeaponDmg + heroMinDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total_MainHand, 0xFFFFF] = minWeaponDmg + heroMinDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total_OffHand, 0] = (minWeaponDmg + heroMinDmg) * offhandMultiplier;
			item.Attributes[GameAttribute.Damage_Weapon_Min_Total_OffHand, 0xFFFFF] = (minWeaponDmg + heroMinDmg) * offhandMultiplier;

			item.Attributes[GameAttribute.Damage_Weapon_Max, 0] = weaponDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Max, 0xFFFFF] = weaponDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Max_Total, 0] = weaponDmg + heroMaxDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Max_Total, 0xFFFFF] = weaponDmg + heroMaxDmg;

			item.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = weaponDmg - minWeaponDmg;
			item.Attributes[GameAttribute.Damage_Weapon_Delta_SubTotal, 0] = (weaponDmg + heroMaxDmg) - (minWeaponDmg + heroMinDmg);
			item.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0] = (weaponDmg + heroMaxDmg) - (minWeaponDmg + heroMinDmg);
			item.Attributes[GameAttribute.Damage_Weapon_Delta_Total_All] = (weaponDmg + heroMaxDmg) - (minWeaponDmg + heroMinDmg);
			item.Attributes[GameAttribute.Damage_Weapon_Delta_Total_MainHand, 0] = (weaponDmg + heroMaxDmg) - (minWeaponDmg + heroMinDmg);
			item.Attributes[GameAttribute.Damage_Weapon_Delta_Total_OffHand, 0] = 3.051758E-05f;

			bool equipped = false; // (item.InvLoc.x + item.InvLoc.y == 0) ? 0 : 1;

			item.Attributes[GameAttribute.Item_Equipped] = equipped;

			item.Attributes[GameAttribute.Durability_Max] = 400;
			item.Attributes[GameAttribute.Durability_Cur] = 400;
		}
	}
}
