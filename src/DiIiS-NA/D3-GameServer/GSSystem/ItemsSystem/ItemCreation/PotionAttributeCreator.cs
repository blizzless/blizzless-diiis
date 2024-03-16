using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.ItemCreation
{
	class PotionAttributeCreator : IItemAttributeCreator
	{
		public void CreateAttributes(Item item)
		{
			item.Attributes[GameAttributes.Hitpoints_Granted] = 250f;
			item.Attributes[GameAttributes.ItemStackQuantityLo] = 1;
		}
	}
}
