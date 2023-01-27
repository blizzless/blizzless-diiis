using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	class PotionAttributeCreator : IItemAttributeCreator
	{
		public void CreateAttributes(Item item)
		{
			item.Attributes[GameAttribute.Hitpoints_Granted] = 250f;
			item.Attributes[GameAttribute.ItemStackQuantityLo] = 1;
		}
	}
}
