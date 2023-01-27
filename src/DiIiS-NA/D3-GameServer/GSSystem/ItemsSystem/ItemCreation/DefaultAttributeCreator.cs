using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.Core.Helpers;
using DiIiS_NA.Core.Helpers.Math;
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
