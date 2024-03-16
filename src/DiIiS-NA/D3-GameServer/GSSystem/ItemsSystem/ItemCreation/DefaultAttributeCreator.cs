using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	class DefaultAttributeCreator : IItemAttributeCreator
	{
		public void CreateAttributes(Item item)
		{
			item.Attributes[GameAttributes.Item_Quality_Level] = 1;
			item.Attributes[GameAttributes.Seed] = RandomHelper.Next(); //unchecked((int)2286800181);
		}
	}
}
