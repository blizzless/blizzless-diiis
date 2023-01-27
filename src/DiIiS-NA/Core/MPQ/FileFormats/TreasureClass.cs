//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.SNO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.TreasureClass)]
    public class TreasureClass : FileFormat
    {
		Logger Logger = new Logger("TreasureClass");
		public static TreasureClass GenericTreasure
		{
			get
			{
				return new StandardTreasureClass();
			}
		}
		public class StandardTreasureClass : TreasureClass
		{
			//public override Item CreateDrop(Player player)
			//{
			//	return ItemGenerator.CreateGold(player, Mooege.Common.Helpers.Math.FastRandom.Instance.Next(100, 500));
			//}
		}
		[PersistentProperty("Percentage")]
		public float Percentage { get; private set; }

		[PersistentProperty("I0")]
		public int I0 { get; private set; }

		[PersistentProperty("LootDropModifiersCount")]
		public int LootDropModifiersCount { get; private set; }

		[PersistentProperty("LootDropModifiers")]
		public List<LootDropModifier> LootDropModifiers { get; private set; }

		public TreasureClass() { }

		
	}

	public class LootDropModifier
	{
		[PersistentProperty("I0")]
		public int I0 { get; private set; }

		[PersistentProperty("SNOSubTreasureClass")]
		public int SNOSubTreasureClass { get; private set; }

		[PersistentProperty("Percentage")]
		public float Percentage { get; private set; }

		[PersistentProperty("I1")]
		public int I1 { get; private set; }

		[PersistentProperty("GBIdQualityClass")]
		public int GBIdQualityClass { get; private set; }

		[PersistentProperty("I2")]
		public int I2 { get; private set; }

		[PersistentProperty("I3")]
		public int I3 { get; private set; }

		[PersistentProperty("SNOCondition")]
		public int SNOCondition { get; private set; }

		[PersistentProperty("ItemSpecifier")]
		public ItemSpecifierData ItemSpecifier { get; private set; }

		[PersistentProperty("I5")]
		public int I5 { get; private set; }

		[PersistentProperty("I6")]
		public int I6 { get; private set; }

		public LootDropModifier() { }
	}
}
