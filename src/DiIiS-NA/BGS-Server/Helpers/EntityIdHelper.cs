namespace DiIiS_NA.LoginServer.Helpers
{
	public static class EntityIdHelper
	{
		/// <summary>
		/// Returns high-id type for given bnet.protocol.EntityId
		/// </summary>
		/// <param name="id">The bnet.protocol.EntityId</param>
		/// <returns><see cref="HighIdType"/></returns>
		public static HighIdType GetHighIdType(this bgs.protocol.EntityId id)
		{
			switch (id.High >> 48)
			{
				case 0x0100:
					return HighIdType.AccountId;
				case 0x0200:
					return HighIdType.GameAccountId;
				case 0x0300:
					return HighIdType.ItemId;
				case 0x0000:
					return HighIdType.ToonId;
				case 0x0600:
					return HighIdType.ChannelId;
			}
			return HighIdType.Unknown;
		}

		/// <summary>
		/// High id types for bnet.protocol.EntityId high-id.
		/// </summary>
		public enum HighIdType : ulong
		{
			Unknown = 0x0,
			AccountId = 0x100000000000000,
			GameAccountId = 0x200000000000000,
			ItemId = 0x300000000000000,
			ToonId = 0x000000000000000,
			GameId = 0x600000000000000,
			ChannelId = 0x600000000000000
		}
	}
}
