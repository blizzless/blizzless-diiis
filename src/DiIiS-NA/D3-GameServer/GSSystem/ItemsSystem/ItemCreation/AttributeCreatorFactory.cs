//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
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
