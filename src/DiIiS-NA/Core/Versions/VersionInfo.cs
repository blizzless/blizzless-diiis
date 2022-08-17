//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.Core.Versions
{
	public static class VersionInfo
	{
		/// <summary>
		/// Main assembly versions info.
		/// </summary>
		public static class Assembly
		{
			/// <summary>
			/// Main assemblies version.
			/// </summary>
			public const string Version = "3.22044";
		}

		/// <summary>
		/// MooNet versions info.
		/// </summary>
		public static class MooNet
		{
			/// <summary>
			/// Required client version.
			/// </summary>
			public const int RequiredClientVersion = 22044;

			public static Dictionary<string, int> ClientVersionMaps = new Dictionary<string, int>
			{
				{"Aurora 2e79d6e023_public", 22044},
				{"Aurora 1a3c949c86_public", 16603},
				{"Aurora 127cc0376a_public", 13300},
				{"Aurora ff22fd195b_public", 12811},
				{"Aurora d2b2e2dbd0_public", 11327},
				{"Aurora ab0ebd5e2c_public", 10485}, // also 10057, 10235
				{"Aurora 24e2d13e54_public", 9991},
				{"Aurora 79fef7ae8e_public", 9950}, // also 9858
				{"Aurora 8018401a9c_public", 9749},
				{"Aurora 31c8df955a_public", 9558},
				{"Aurora 8eac7d44dc_public", 9359}, // also 9327
				{"Aurora _public", 9183},
				{"Aurora bcd3e50524_public", 8896},
				{"Aurora 4a39a60e1b_public", 8815},
				{"Aurora 7f06f1aabd_public", 8610},
				{"Aurora 9e9ccb8fdf_public", 8392},
				{"Aurora f506438e8d_public", 8101},
				{"Aurora fbb3e7d1b4_public", 8059},
				{"Aurora 04768e5dce_public", 7931},
				{"Aurora 0ee3b2e0e2_public", 7841},
				{"Aurora b4367eba86_public", 7728}
			};

			/// <summary>
			/// Auth modules' hash maps for client platforms.
			/// </summary>
			//TODO: Get Hashes for Mac client.
			public static Dictionary<BattleClient.ClientPlatform, byte[]> PasswordHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"8f52906a2c85b416a595702251570f96d3522f39237603115f2f1ab24962043c".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"63BC118937E6EA2FAA7B7192676DAEB1B7CA87A9C24ED9F5ACD60E630B4DD7A4".ToByteArray() }
			};

			public static Dictionary<BattleClient.ClientPlatform, byte[]> SSOHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"8e86fbdd1ee515315e9e3e1b479b7889de1eceda0703d9876f9441ce4d934576".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"8e86fbdd1ee515315e9e3e1b479b7889de1eceda0703d9876f9441ce4d934576".ToByteArray() }
			};

			public static Dictionary<BattleClient.ClientPlatform, byte[]> ThumbprintHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"36b27cd911b33c61730a8b82c8b2495fd16e8024fc3b2dde08861c77a852941c".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"36b27cd911b33c61730a8b82c8b2495fd16e8024fc3b2dde08861c77a852941c".ToByteArray() },
			};

			public static Dictionary<BattleClient.ClientPlatform, byte[]> TokenHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"bfa574bcff509b3c92f7c4b25b2dc2d1decb962209f8c9c8582ddf4f26aac176".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"bfa574bcff509b3c92f7c4b25b2dc2d1decb962209f8c9c8582ddf4f26aac176".ToByteArray() },
			};

			public static Dictionary<BattleClient.ClientPlatform, byte[]> RiskFingerprintHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"5e298e530698af905e1247e51ef0b109b352ac310ce7802a1f63613db980ed17".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"bcfa324ab555fc66614976011d018d2be2b9dc23d0b54d94a3bd7d12472aa107".ToByteArray() },
			};

			public static Dictionary<BattleClient.ClientPlatform, byte[]> AgreementHashMap = new Dictionary<BattleClient.ClientPlatform, byte[]>()
			{
				{ BattleClient.ClientPlatform.Win,"41686a009b345b9cbe622ded9c669373950a2969411012a12f7eaac7ea9826ed".ToByteArray() },
				{ BattleClient.ClientPlatform.Mac,"41686a009b345b9cbe622ded9c669373950a2969411012a12f7eaac7ea9826ed".ToByteArray() },
			};

			public static byte[] TOS = "00736F74006167726500005553014970E37CCD158A64A2844D6D4C05FC1697988A617E049BB2E0407D71B6C6F2".ToByteArray();
			public static byte[] EULA = "00616C75656167726500005553DDD1D77970291A4E8A64BB4FE25B2EA2D69D8915D35D53679AE9FDE5EAE47ECC".ToByteArray();
			public static byte[] RMAH = "0068616D72616772650000555398A3FC047004D6D4A0A1519A874AC9B1FC5FBD62C3EAA23188E095D6793537D7".ToByteArray();

			public static Dictionary<string, uint> Regions = new Dictionary<string, uint>()
			{
				{ "US", 0x5553 },
				{ "XX", 0x5858 }, //Beta Region
				{ "EU", 0x4555 }
			};

			public static string Region = "EU";

			public static class Resources
			{
				public static string ProfanityFilterHash = "de1862793fdbabb6eb1edec6ad1c95dd99e2fd3fc6ca730ab95091d694318a24"; //9558-10485
				public static string AvailableActs = "bd9e8fc323fe1dbc1ef2e0e95e46355953040488621933d0685feba5e1163a25"; //10057
				public static string AvailableQuests = "9303df8f917e2db14ec20724c04ea5d2af4e4cb6c72606b67a262178b7e18104"; //10057
			}

			public static class Achievements
			{
				public static string AchievementFileHash = "5719bfb3321ee2b68f712cba0fe35d1e5bfb6e545d424d9547d80385d3ae2ec4"; //22044

				public static string AchievementFilename = AchievementFileHash + ".achu";

				public static string AchievementURL = "http://" + MooNet.Region + ".depot.battle.net:1119/" + AchievementFilename;
			}
		}

		public static class MPQ
		{
			public const int RequiredPatchVersion = 22044;
		}

		public static class Ingame
		{
			public const int ProtocolHash = unchecked((int)0x280CA408); //22044

			public const string MajorVersion = "2.6.9";
			public const string ServerBuild = "22044";
			public const string VersionString = MajorVersion + "." + ServerBuild;
		}
	}
}
