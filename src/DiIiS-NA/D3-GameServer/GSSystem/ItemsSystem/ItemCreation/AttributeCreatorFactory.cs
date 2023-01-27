using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	internal class AttributeCreatorFactory
	{
		public List<IItemAttributeCreator> Create(ItemTypeTable itemType)
		{
			var creatorList = new List<IItemAttributeCreator> { new DefaultAttributeCreator() };

			//if (Item.IsWeapon(itemType)) creatorList.Add(new WeaponAttributeCreator());
			//else if (Item.IsPotion(itemType))  creatorList.Add(new PotionAttributeCreator());

			return creatorList;
		}
	}
}
