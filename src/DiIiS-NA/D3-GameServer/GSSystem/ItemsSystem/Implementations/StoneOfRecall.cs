namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.Implementations
{
	[HandledItem("StoneOfRecall")]
	class StoneOfRecall : Item
	{
		public StoneOfRecall(MapSystem.World world, DiIiS_NA.Core.MPQ.FileFormats.GameBalance.ItemTable definition, int cork = -1, bool cork2 = false, int cork3 = -1)
			: base(world, definition)
		{
		}

		public override void OnTargeted(PlayerSystem.Player player, MessageSystem.Message.Definitions.World.TargetMessage message)
		{
			player.EnableStoneOfRecall();
			this.Destroy();
		}
	}
}
