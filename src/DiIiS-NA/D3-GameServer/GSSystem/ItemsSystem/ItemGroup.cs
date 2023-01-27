using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public static class ItemGroup
	{
		private static Dictionary<int, ItemTypeTable> ItemTypes = new Dictionary<int, ItemTypeTable>();

		static ItemGroup()
		{
			foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
			{
				GameBalance data = asset.Data as GameBalance;
				if (data != null && data.Type == BalanceType.ItemTypes)
				{
					foreach (var itemTypeDef in data.ItemType)
					{

						ItemTypes.TryAdd(itemTypeDef.Hash, itemTypeDef);
					}
				}
			}
		}

		public static List<ItemTypeTable> HierarchyToList(ItemTypeTable itemType)
		{
			List<ItemTypeTable> result = new List<ItemTypeTable>();
			var curType = itemType;
			if (curType != null)
			{
				result.Add(curType);
				while (curType.I0 != -1)// & curType.ParentType != 0)
				{
					curType = ItemTypes[curType.I0];
					result.Add(curType);
				}
			}
			return result;
		}

		public static List<int> HierarchyToHashList(ItemTypeTable itemType)
		{
			List<int> result = new List<int>();
			var types = HierarchyToList(itemType);
			foreach (var type in types)
			{
				result.Add(type.Hash);
			}
			return result;
		}

		public static List<int> SubTypesToHashList(string name)
		{
			List<int> result = new List<int>();
			ItemTypeTable rootType = FromString(name);
			if (rootType != null)
			{
				result.Add(rootType.Hash);
				for (int i = 0; i < result.Count; ++i)
				{
					foreach (var type in ItemTypes.Values)
						if (type.I0 == result[i])
							result.Add(type.Hash);
				}
			}
			return result;
		}

		public static ItemTypeTable FromString(string name)
		{
			int hash = StringHashHelper.HashItemName(name);
			return FromHash(hash);
		}

		public static ItemTypeTable FromHash(int hash)
		{
			ItemTypeTable result = null;
			if (ItemTypes.TryGetValue(hash, out result))
			{
				return result;
			}
			return null;
		}

		public static bool IsSubType(ItemTypeTable type, string rootTypeName)
		{
			return IsSubType(type, StringHashHelper.HashItemName(rootTypeName));
		}

		public static bool IsSubType(ItemTypeTable type, int rootTypeHash)
		{
			if (type == null)
				return false;

			if (type.Hash == rootTypeHash)
				return true;
			var curType = type;
			while (curType.I0 != -1)
			{
				curType = ItemTypes[curType.I0];
				if (curType.Hash == rootTypeHash)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetRootType(ItemTypeTable itemType)
		{
			var curType = itemType;
			while (curType.ParentType != -1)
			{
				curType = ItemTypes[curType.ParentType];
			}
			return curType.Hash;
		}

		public static bool Is2H(ItemTypeTable type)
		{
			return (type.Labels[0] & 0x400) != 0;
		}
	}
}
