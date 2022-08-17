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
using DiIiS_NA.Core.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	class DefaultAttributeCreator : IItemAttributeCreator
	{
		public void CreateAttributes(Item item)
		{
			item.Attributes[GameAttribute.Item_Quality_Level] = 1;
			item.Attributes[GameAttribute.Seed] = RandomHelper.Next(); //unchecked((int)2286800181);
		}
	}
}
