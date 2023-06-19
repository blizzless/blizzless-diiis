using System;
using System.Collections.Generic;
using DiIiS_NA.D3_GameServer;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
	public static class LootManager
	{

		static LootManager()
		{
		}

		public static int Common
		{
			get { return 1; }
			set { }
		}

		public static int Uncommon
		{
			get { return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(3, 5); }
			set { }
		}

		public static int Rare
		{
			get { return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(5, 8); }
			set { }
		}

		public static int Epic
		{
			get { return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(8, 11); }
			set { }
		}

		public static int GetLootQuality(int MonsterQuality, int difficulty)
		{
			float roll = (float)DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble();
			switch (MonsterQuality)
			{
				case 0: //Normal
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9984f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9982f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9979f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9976f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9972f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9968f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9963f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 1: //Champion
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9984f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9982f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9979f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9976f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9972f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9968f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9963f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 2: //Rare (Elite)
				case 4: //Unique
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9984f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9982f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9979f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9976f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9972f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9968f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9963f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 7: //Boss
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.995f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9942f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9934f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9924f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9913f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.985f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.98f)
								return Rare;
							return Epic;
						default: return Common;
					}
				default:
					return Common;
			}
		}

		public static int GetSeasonalLootQuality(int MonsterQuality, int difficulty)
		{
			float roll = (float)DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble();
			switch (MonsterQuality)
			{
				case 0: //Normal
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9952f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9945f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9938f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9927f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9916f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9904f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.2f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9889f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 1: //Champion
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9952f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9945f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9938f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9927f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9916f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9904f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.08f)
								return Common;
							if (roll < 0.5f)
								return Uncommon;
							if (roll < 0.9889f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 2: //Rare (Elite)
				case 4: //Unique
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9952f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9945f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9938f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9927f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9916f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9904f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.05f)
								return Common;
							if (roll < 0.35f)
								return Uncommon;
							if (roll < 0.9889f)
								return Rare;
							return Epic;
						default: return Common;
					}
				case 7: //Boss
					switch (difficulty)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.985f)
								return Rare;
							return Epic;
						case 4: //T1
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9827f)
								return Rare;
							return Epic;
						case 5: //T2
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9802f)
								return Rare;
							return Epic;
						case 6: //T3
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9772f)
								return Rare;
							return Epic;
						case 7: //T4
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9737f)
								return Rare;
							return Epic;
						case 8: //T5
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9698f)
								return Rare;
							return Epic;
						case 9: //T6
							if (roll < 0.03f)
								return Common;
							if (roll < 0.29f)
								return Uncommon;
							if (roll < 0.9653f)
								return Rare;
							return Epic;
						default: return Common;
					}
				default:
					return Common;
			}
		}

		public static List<float> GetDropRates(int MonsterQuality, int level = 60)
		{
			if (level < 6)
				switch (MonsterQuality)
				{
					case 1: //Champion
						return new List<float> { 1f, 1f, 0.5f };
					case 2: //Rare (Elite)
					case 4: //Unique
						return new List<float> { 1f, 1f, 0.7f, 0.5f };
					default: return new List<float> { 0.04f };
				}

			switch (MonsterQuality)
			{
				case 0: //Normal
					return new List<float> { 0.06f };
				case 1: //Champion
					return new List<float> { 1f, 1f, 0.7f, 0.5f };
				case 2: //Rare (Elite)
				case 4: //Unique
					return new List<float> { 1f, 1f, 0.9f, 0.7f };
				case 7: //Boss
					return new List<float> { 1f, 1f, 1f, 0.7f, 0.5f, 0.3f };
				default:
					return new List<float> { 0.04f };
			}
		}

		public static List<float> GetSeasonalDropRates(int MonsterQuality, int level)
		{
			if (level < 10)
				switch (MonsterQuality)
				{
					case 1: //Champion
						return new List<float> { 1f, 1f, 1f };
					case 2: //Rare (Elite)
					case 4: //Unique
						return new List<float> { 1f, 1f, 1f, 1f };
					default: return new List<float> { 0.08f };
				}

			switch (MonsterQuality)
			{
				case 0: //Normal
					return new List<float> { 0.18f * GameModsConfig.Instance.Rate.ChangeDrop };
				case 1: //Champion
					return new List<float> { 1f, 1f, 1f, 1f, 0.75f * GameModsConfig.Instance.Rate.ChangeDrop };
				case 2: //Rare (Elite)
				case 4: //Unique
					return new List<float> { 1f, 1f, 1f, 1f, 1f };
				case 7: //Boss
					return new List<float> { 1f, 1f, 1f, 1f, 1f, 0.75f * GameModsConfig.Instance.Rate.ChangeDrop, 0.4f * GameModsConfig.Instance.Rate.ChangeDrop };
				default:
					return new List<float> { 0.12f * GameModsConfig.Instance.Rate.ChangeDrop };
			}
		}

		public static int GetGoldAmount(int level)
		{
			return Math.Max(1, (int)(DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(level, level * 2) * (DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble(7, 10) / 10f)));
		}

		public static int GetBloodShardsAmount(int difficulty)
		{
			switch (difficulty)
			{
				case 0:
					return 0;
				case 1:
					return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble() < 0.25 ? 1 : 0;
				case 2:
					return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble() < 0.5 ? 1 : 0;
				case 3:
					return DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble() < 0.75 ? 1 : 0;
				case 4: //T1
				case 5: //T2
					return 1;
				case 6: //T3
				case 7: //T4
					return 2;
				case 8: //T5
				case 9: //T6
					return 3;
				default: return 0;
			}
		}

		public static float GetEssenceDropChance(int difficulty)
		{
			switch (difficulty)
			{
				case 0:
					return 0.15f;
				case 1:
					return 0.18f;
				case 2:
					return 0.21f;
				case 3:
					return 0.25f;
				case 4: //T1
					return 0.31f;
				case 5: //T2
					return 0.37f;
				case 6: //T3
					return 0.44f;
				case 7: //T4
					return 0.53f;
				case 8: //T5
					return 0.64f;
				case 9: //T6
					return 0.77f;
				default: return 0f;
			}
		}
	}
}
