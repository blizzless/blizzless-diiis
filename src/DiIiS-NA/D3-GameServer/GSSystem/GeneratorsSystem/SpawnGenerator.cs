using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Helpers.Math;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public static class SpawnGenerator
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public class MonsterLayout
		{
			public bool lazy_load;
			public int additional_density;
			public bool can_spawn_goblin;
			public List<int> melee;
			public List<int> range;
			public List<int> dangerous;
		};

		public static List<int> TotalMonsters(int la)
		{
			List<int> total = new List<int> { };
			total.AddRange(Spawns[la].melee);
			total.AddRange(Spawns[la].range);
			total.AddRange(Spawns[la].dangerous);
			return total;
		}

		public static bool IsMelee(int la, int monsterId)
		{
			return Spawns[la].melee.Contains(monsterId);
		}

		public static bool IsRange(int la, int monsterId)
		{
			return Spawns[la].range.Contains(monsterId);
		}

		public static bool IsDangerous(int la, int monsterId)
		{
			return Spawns[la].dangerous.Contains(monsterId);
		}

		public static void RegenerateDensity()
		{
			//Logger.Info("Regenerating spawn density map...");
			foreach (var spawn in Spawns)
			{
				Spawns[spawn.Key].additional_density = FastRandom.Instance.Next(0, 6);
			}
		}

		// key is WorldSno, LevelArea
		public static Dictionary<int, MonsterLayout> Spawns = new Dictionary<int, MonsterLayout>
		{
			#region Act I
			{91324,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6652,  203121, 6644 }, range = new List<int>{ 219725 }, dangerous = new List<int>{ 218339 }}}, //REMOVED 4982 QUILL FIEND (OP damage)							//road to Old Tristram ruins
			{101351,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6652,  203121, 6644 }, range = new List<int>{ 219725 }, dangerous = new List<int>{ 218345, 218431 }}}, //REMOVED 4982 QUILL FIEND (OP damage)						//Old Tristram ruins
			{19933,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6652, 5393 }, range = new List<int>{ 5346 }, dangerous = new List<int>{ 218666, 218321 }}},										//Cath, outside
			{19954,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6652,  203121, 6632,  5393, 6653 }, range = new List<int>{ 219725, 3847, 6639 }, dangerous = new List<int>{ 218332, 85900 }}}, //REMOVED 4982 QUILL FIEND (OP damage) + 5235 SCAVENGER (jump mechanic nukes)		//The Weeping Hollow
			{72712,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393, 6653 }, range = new List<int>{ 5387 }, dangerous = new List<int>{ 220683 }}},													//Tristram Graveyard
			{83264,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5393, 3848,  5395, 5411, 6653, 4156 }, range = new List<int>{ 5346, 3847, 6639 }, dangerous = new List<int>{ 225502 }}}, // REMOVED ID 5235 SCAVENGER (OP jump mechanic)	//Cursed crypt 1
			{83265,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5393, 3848,  5395, 5411, 6653, 4156 }, range = new List<int>{ 5346, 3847, 6639 }, dangerous = new List<int>{ 225502 }}},	// REMOVED ID 5235 SCAVENGER (OP jump mechanic)	//Cursed crypt 2
			{154588,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393, 3848,  5395, 5411, 6653 }, range = new List<int>{ 5346, 3847, 6639 }, dangerous = new List<int>{ 218348, 218351 }}}, // REMOVED ID 5235 SCAVENGER (OP jump mechanic)	//Cursed crypt 3 (true)
			{102362,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393 }, range = new List<int>{ 5387 }, dangerous = new List<int>{}}},												//Chancellor's Tomb
			{19952,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4282, 4285, 3337, 5236, 5235 }, range = new List<int>{ 4290, 4286 }, dangerous = new List<int>{ 218422, 218424, 218428 }}},				//Fields of Misery
			{19953,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5395, 4153, 4201, 5275 }, range = new List<int>{ 5347 }, dangerous = new List<int>{ 209553, 218444, 218441 }}},											//The Festering Woods
			{60397,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5395 }, range = new List<int>{ 5388 }, dangerous = new List<int>{}}},											//Crypt of the Ancients
			{60396,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5395 }, range = new List<int>{ 179343, 5347 }, dangerous = new List<int>{}}},									//Warrior's Rest
			{60398,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5276, 6025 }, range = new List<int>{ 5347, 5388 }, dangerous = new List<int>{}}},										//Drowned Temple
			{19780,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5275, 6652,4156, 6646, 6644 }, range = new List<int>{ 5346 }, dangerous = new List<int>{ 218307, 218308 }}},					//Cathedral, lvl. 1
			{19783,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5393, 5275, 6356, 4156 }, range = new List<int>{ 5346, 5387 }, dangerous = new List<int>{ 218656, 218356, 218362, 218364 }}},					//Cathedral, lvl. 2
			{87907,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393, 6024, 5275 }, range = new List<int>{ 5387 }, dangerous = new List<int>{}}},					//Cathedral, lvl. 3 (Kormac)
			{19785,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5393, 3848, 5395, 5275, 4156, 3893 }, range = new List<int>{ 5346, 5387, 3847 }, dangerous = new List<int>{ 218396, 218405,  }}},				//Cathedral, lvl. 4
			{19787,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393, 5275 }, range = new List<int>{ 5346 }, dangerous = new List<int>{}}},											//The Royal Crypts
			//{129795,		new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393 }, range = new List<int>{ 5346 }, dangerous = new List<int>{}}},												//Wortham Rocks
			{78572,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5467, 5474, 4157, 4153 }, range = new List<int>{ 166726, 122367 }, dangerous = new List<int>{ 129439, 218462, 218458, 218456, 218448 }}},											//Caves of Aranea
			{93632,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4283, 5236 }, range = new List<int>{ 6500, 4287, 4290 }, dangerous = new List<int>{ 218469 }}},								//Highlands Crossing
			{19940,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4282, 4283, 4285, 3337, 3338, 5236 }, range = new List<int>{ 4287, 6500, 375, 4290, 4286, 6024 }, dangerous = new List<int>{ 218508, 218536, 218473, 111580, 76676 }}},		//Southern highlands
			{19941,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4282, 4283, 3338, 4154, 195747 }, range = new List<int>{ 4287, 375, 4290, 6500 }, dangerous = new List<int>{}}},							//Northern highlands
			{19774,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6042, 6052, 6024, 6359, 6654, 90453, 6647, 104424, 178300 }, range = new List<int>{ 6046 }, dangerous = new List<int>{ 218674, 218672, 218676, 105959 }}},			//Halls of Agony, lv. 1
			{19775,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6042, 6052, 6024, 6359, 6654, 90453, 6647, 104424, 178300 }, range = new List<int>{ 6046 }, dangerous = new List<int>{}}},			//Halls of Agony, lv. 2
			{94672,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4203, 3849, 434 }, range = new List<int>{ 5389 }, dangerous = new List<int>{}}},										//The Cursed Hold
			{19776,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6042, 6052, 6024, 6654, 90453, 6647, 104424, 178300 }, range = new List<int>{ 6046 }, dangerous = new List<int>{ 105620, 220034, 218678 }}},	//Halls of Agony, lv. 3			
			{80116,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6052, 6024, 5395, 5275, 178300 }, range = new List<int>{ 5346, 5387 }, dangerous = new List<int>{}}},							//The Lyceum
			{82326,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6024, 4201, 5395, 434, 5276, 5393 }, range = new List<int>{ 5346 }, dangerous = new List<int>{}}},					//Watch Tower, lv. 1
			{82327,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6024, 4201, 5395, 434, 5276, 5393 }, range = new List<int>{ 5346 }, dangerous = new List<int>{}}},					//Watch Tower, lv. 2
			
			{135952,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6042,6052, 6024, 6359, 178300 }, range = new List<int>{ 6046 }, dangerous = new List<int>{}}},							//Highlands Cave
			{100854,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6042, 6024 }, range = new List<int>{ 6046 }, dangerous = new List<int>{ 218656, 218662, 218664 }}},										//Leoric's Manor
			{19935,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6052, 6025, 178300 }, range = new List<int>{ }, dangerous = new List<int>{}}},												//Wortham
			{87832,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6052, 6024, 6027, 4283, 178300 }, range = new List<int>{ 4287, 4290 }, dangerous = new List<int>{}}},							//Highlands Passage
			{1199,          new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6024, 4283, 3338 }, range = new List<int>{ 4287, 4290, 4286 }, dangerous = new List<int>{}}},						//Leoric's Hunting Grounds
			{119893,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6024, 178213, 4285 }, range = new List<int>{ 4286 }, dangerous = new List<int>{}}},								//Khazra Den
			{148551,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 51281 }, range = new List<int>{ }, dangerous = new List<int>{}}},													//Cells of the Condemned
			{19553,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4157, 5393, 136943, 370, 5395 }, range = new List<int>{ 4153, 5347 }, dangerous = new List<int>{ 209553, 218444, 218441 }}},												//The Festering Woods			
			{82566,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4282, 4283, 5467 }, range = new List<int>{ 4287, 4290 }, dangerous = new List<int>{}}},											//Cave of the Moon Clan, Lv.1
			{82567,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4282, 4283, 5467 }, range = new List<int>{ 4287, 4290 }, dangerous = new List<int>{}}},								//Cave of the Moon Clan, Lv.2					
			{179212,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4285, 5235 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//Sheltered Cottage
			{82372,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5236, 6360, 4157, 4983 }, range = new List<int>{ 4153 }, dangerous = new List<int>{}}},												//Lost Mine, Lv.1
			{82373,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5236, 6360, 4157, 4983 }, range = new List<int>{ 4153 }, dangerous = new List<int>{}}},												//Lost Mine, Lv.2
			{81165,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5236, 4157, 4983 }, range = new List<int>{ 4153 }, dangerous = new List<int>{}}},														//Scavenger's Den, Lv.1
			{81175,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5236, 4157, 4983 }, range = new List<int>{ 4153 }, dangerous = new List<int>{}}},														//Scavenger's Den, Lv.2
			{135237,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5235, 4156, 6646, 4156, 4152, 4157 }, range = new List<int>{  }, dangerous = new List<int>{}}},													//Den of the Fallen, Lv.1
			{194232,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5235, 4156, 6646, 4156, 4152, 4157 }, range = new List<int>{  }, dangerous = new List<int>{}}},													//Den of the Fallen, Lv.2
			{102300,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6359, 6652, 4157, 5393 }, range = new List<int>{ 6638, 6359, 6356 }, dangerous = new List<int>{}}},										//Decaying Crypt, Lv.1
			{165798,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6652 }, range = new List<int>{ 6638 }, dangerous = new List<int>{}}},												//Decaying Crypt, Lv.2
			{161964,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//The Cave Under the Well
			{106756,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//Dank Cellar
			{106757,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//Damp Cellar
			{103202,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//Dark Cellar
			{62968,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652 }, range = new List<int>{  }, dangerous = new List<int>{}}},												//The Hidden Cellar
			{107051,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6644, 6652, 128781 }, range = new List<int>{  }, dangerous = new List<int>{}}},											//Musty Cellar
			{211479,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 209087, 210502, 207378,207559, 207560, 192965 }, range = new List<int>{  }, dangerous = new List<int>{ 218804, 218807, 212667, 218802, 209506, 218806, 201679, 214948, 218808, 212664 }}},                                            //Whimsyshire

			#endregion
			#region Act II
			{19839,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4080, 6053, 6028, 5199, 5209, 5396, 4541, 4550, 5208, 4551, 4542 }, range = new List<int>{ 4070 }, dangerous = new List<int>{ 59970, 5203 }}},										//Stinging Winds Desert
			{19835,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6060, 6027, 6061, 6044, 6053, 5209, 5208 }, range = new List<int>{ 6038 }, dangerous = new List<int>{ 218810, 221442, 221810 }}},																						//Road to Alcarnus
			{19825,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6060, 6027, 6061, 6044, 6054 }, range = new List<int>{ 6038 }, dangerous = new List<int>{ 222003, 222001, 221981, 221999 }}},																						//Alcarnus
			{144117,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6043, 6028, 5208, 5512  }, range = new List<int>{ 6047 }, dangerous = new List<int>{ 147155, 144400 }}},																							//Deserted Cellar						
			{141067,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6053, 6025 }, range = new List<int>{ }, dangerous = new List<int>{ 140947 }}},																										//Hadi's Claim Mine
			{148905,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6053, 6054 }, range = new List<int>{ }, dangerous = new List<int>{}}},																										//Town Cellar
			{102936,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6053, 6054 }, range = new List<int>{ }, dangerous = new List<int>{}}},																										//Secret Altar
			{148904,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6054, 6028, 6027 }, range = new List<int>{ }, dangerous = new List<int>{}}},																									//Sandy Cellar
			{102932,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6054, 6028, 6027 }, range = new List<int>{ }, dangerous = new List<int>{}}},																									//Hidden Conclave
			{195998,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6028 }, range = new List<int>{ }, dangerous = new List<int>{ 195639 }}},																												//Abandoned Cellar
			{148903,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6027, 4089 }, range = new List<int>{ }, dangerous = new List<int>{}}},																										//Alcarnus Cellar
			{19836,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 6060, 5396, 4541, 4550, 5208, 4551, 4542, 4089 }, range = new List<int>{ 6027 }, dangerous = new List<int>{}}},																						//Sundered Canyon
			{170118,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5199, 5208, 4541, 4551, 4542, 4550 }, range = new List<int>{ }, dangerous = new List<int>{}}},																										//Black Canyon Bridge
			{19838,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5199, 4080, 5208, 5396 }, range = new List<int>{ 4070 }, dangerous = new List<int>{ 221406, 221402 }}},																						//Black Canyon Mines
			{19837,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5199,4080, 5208, 4089, 5396, 4541, 4550, 4551 }, range = new List<int>{ 6028, 4070 }, dangerous = new List<int>{ 221377, 221379, 221367, 221372 }}},																//Howling Plateau
			{19794,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4080, 5208, 5512, 4550, 4541, 4551, 4542, 5432 }, range = new List<int>{ 4098, 5428 }, dangerous = new List<int>{}}},																				//The Crumbling Vault
			{57425,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4080, 4083, 4093, 5434, 4094, 4083, 4542, 5432, 3384 }, range = new List<int>{ 4099, 4098, 3981, 4071, 5429, 5428 }, dangerous = new List<int>{ 222180, 215445, 208543, 140424, 113994, 115132, 140424 }}},					//Dahlgur Oasis
			{166127,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4080 }, range = new List<int>{ 4098 }, dangerous = new List<int>{ 166133 }}},																											//The lost Caravan
			{175367,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4080, 5432 }, range = new List<int>{ 4098, 5428 }, dangerous = new List<int>{}}},																											//Path to the Oasis
			{159897,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4080 }, range = new List<int>{ }, dangerous = new List<int>{}}},																												//Storage Cellar
			{159899,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4080 }, range = new List<int>{ }, dangerous = new List<int>{}}},																												//Storm Cellar
			{19798,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 231349, 4083, 231356, 5278, 5397, 4090, 137995, 137996, 208962, 204944, 208963, 5188, 5187, 5192, 368, 4196, 5468, 231355 }, range = new List<int>{ 231351, 5349, 5376, 5368, 5372, 5382 }, dangerous = new List<int>{ 222502, 222510, 222511, 222512 }}},		//The Storm Halls
			{19797,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 231349, 4083, 231356, 5278, 5397, 4090, 137994, 137992, 208832, 204944, 208963, 5188, 5187, 5192, 4196, 368, 5191, 231355 }, range = new List<int>{ 231351, 5349, 5368, 5372, 5382, 5376 }, dangerous = new List<int>{ 222427, 222413, 165602 }}},					//The Unknown Depths
			{175330,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4093, 4090, 5512, 4071 }, range = new List<int>{ }, dangerous = new List<int>{}}},																										//Ancient Path
			{53834,     new MonsterLayout{ lazy_load = true, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6655, 218795, 5210, 4105, 4104, 5396, 5188, 3385, 4542, 5191, 5513, 5512, 4093, 204256 }, range = new List<int>{ 5376 }, dangerous = new List<int>{ 222352, 222385, 219832, 217744, 111868, 222400, 222339, 218947 }}},					//Desolate Sands
			{62780,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4202, 5210, 4105, 56784, 4080, 5513, 4071, 4090, 4204 }, range = new List<int>{ 4155, 5381, 5429, 5428, 5430, 5277 }, dangerous = new List<int>{ 222236, 222238 }}},											//Eastern Channel
			{159588,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4204, 217308, 437, 5090 }, range = new List<int>{ }, dangerous = new List<int>{}}},																									//Sirocco Caverns, Lv.1
			{220805,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4204, 217308, 437 }, range = new List<int>{ }, dangerous = new List<int>{}}},																									//Sirocco Caverns, Lv.2
			{111667,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6238, 4202, 4158, 5088, 5238 }, range = new List<int>{ }, dangerous = new List<int>{}}},																							//Cave of Burrowing Horror, Lv.1
			{218969,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6238, 4202, 4158, 5238 }, range = new List<int>{ }, dangerous = new List<int>{}}},																							//Cave of Burrowing Horror, Lv.2
			{19791,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 5277, 4104, 56784, 5513, 5432, 5512, 5381 }, range = new List<int>{ 5428 }, dangerous = new List<int>{ 219583, 222008, 222005, 156738, 156763, }}},																		//Sewers of Caldeum
			{19795,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5396, 5412 }, range = new List<int>{ 5348, 5381, 5367 }, dangerous = new List<int>{}}},																									//Chamber of the Lost Idol
			{62778,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4104, 4105, 56784, 4080, 5381, 5513, 5432 }, range = new List<int>{ 5430, 5428, 5277 }, dangerous = new List<int>{ 222186, 222189 }}},											//Western Channel
			{169494,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 4197 }, range = new List<int>{ 5375 }, dangerous = new List<int>{}}},																								//Mysterious Cave, Lv.1
			{194239,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 4197 }, range = new List<int>{ 5375 }, dangerous = new List<int>{}}},																								//Mysterious Cave, Lv.2
			{146838,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5396, 5277, 4104, 56784, 5512, 5432 }, range = new List<int>{ 5381, 5428 }, dangerous = new List<int>{}}},																//Ruined Cistern
			{196225,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5209, 4080 }, range = new List<int>{ 4070 }, dangerous = new List<int>{}}},																									//Tunnels of the Rockworm
			{204629,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 4158, 55135 }, range = new List<int>{ }, dangerous = new List<int>{}}},																			//Cave of the Betrayer, Lv.1
			{204675,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 4158 }, range = new List<int>{ }, dangerous = new List<int>{}}},																			//Cave of the Betrayer, Lv.2
			{19801,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 5432, 208962, 208963, 204944, 5512 }, range = new List<int>{ 5428 }, dangerous = new List<int>{}}},																			//The Ruins, Lv.1
			{222577,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 5432, 208962, 208963, 204944, 5512 }, range = new List<int>{}, dangerous = new List<int>{}}},																		//The Ruins, Lv.2
			{111671,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5468, 4158, 4202, 5513, 5475, 5088 }, range = new List<int>{ }, dangerous = new List<int>{}}},																					//Vile Cavern, Lv.1
			{218968,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5468, 4158, 4202 }, range = new List<int>{ }, dangerous = new List<int>{}}},																					//Vile Cavern, Lv.2
			{123182,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 208962, 208832, 5375, 204944, 208963 }, range = new List<int>{ 3982 }, dangerous = new List<int>{}}},											//Vault of the Assassin, part 1
			{153714,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 208962, 208832, 5375, 204944, 208963 }, range = new List<int>{ 982 }, dangerous = new List<int>{}}},											//Vault of the Assassin, part 2
			{61632,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5396, 5188, 208962, 208832, 204944, 208963, 5193 }, range = new List<int>{ 5371, 5375 }, dangerous = new List<int>{}}},														//The Forgotten Ruins
			{19799,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5278, 5397, 4198 }, range = new List<int>{}, dangerous = new List<int>{}}},																										//Halls of Dusk
			{80592,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 208962, 208832, 204944, 208963, 199478, 4198 }, range = new List<int>{ 5372, 5382 }, dangerous = new List<int>{ 222523 }}},																				//Halls of Dusk
			{192689,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4105, 5433, 56784, 4080, 5513, 5432 }, range = new List<int>{ 5429, 5428 }, dangerous = new List<int>{}}},																			//Hidden Aqueducts
			{158594,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4104, 56784, 5513, 5432 }, range = new List<int>{ 5428 }, dangerous = new List<int>{}}},																						//Tomb of Khan Dakab
			{158384,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5513, 5432 }, range = new List<int>{ 5428 }, dangerous = new List<int>{}}},																									//Tomb of Sardar
			{62575,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 56784, 5513, 368 }, range = new List<int>{ 5368 }, dangerous = new List<int>{}}},																										//Ancient Cave, Lv.1
			{194242,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 56784, 5513, 368 }, range = new List<int>{ 5368 }, dangerous = new List<int>{}}},																									//Ancient Cave, Lv.2
			{62574,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5468, 5475, 5088 }, range = new List<int>{  }, dangerous = new List<int>{}}},																											//Flooded Cave, Lv.1
			{161105,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5468, 5475, 5088 }, range = new List<int>{  }, dangerous = new List<int>{}}},                                                                                                     //Flooded Cave, Lv.2
			#endregion
			#region Act III
			{75436,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6055, 169456, 170763, 5397, 182285, 204232, 77796, 4985, 62736 }, range = new List<int>{ 170781 }, dangerous = new List<int>{ 220468, 220710, 220444, 220455 }}},																//The Keep Depths, Lv.1
			{93103,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6055,4095, 169456, 170763, 5397, 182285, 204232, 77796, 4985, 62736, 130794 }, range = new List<int>{ 170781 }, dangerous = new List<int>{ 220481, 220479, 220476 }}},															//The Keep Depths, Lv.2
			{136448,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 6055,4095, 169456, 170763, 5397, 182285, 204232, 77796, 62736 }, range = new List<int>{ 170781 }, dangerous = new List<int>{ 220499, 220491, 220485 }}},														//The Keep Depths, Lv.3
			{182258,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4084,4095, 4091, 4746, 130794 }, range = new List<int>{ 4100, 4072 }, dangerous = new List<int>{}}},																						//The Barracks, lv. 1
			{221753,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4084,4095, 4091, 4746, 130794 }, range = new List<int>{ 4100, 4072 }, dangerous = new List<int>{}}},																						//The Barracks, lv. 2
			{92960,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4084,4085, 4091, 4092, 77796, 62736, 221770, 130794 }, range = new List<int>{ 5581, 365, 4073 }, dangerous = new List<int>{ 220232 }}},											//Skycrown Battlements
			{69504,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4085, 4295, 141194, 4085, 221770, 130794 }, range = new List<int>{ 365, 4303, 4300, 5581, 4073 }, dangerous = new List<int>{ 220377, 220775, 220773, 220727 }}},													//Rakkis Crossing
			{119653,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4296, 4746 }, range = new List<int>{ 4300, 4304, 5508 }, dangerous = new List<int>{ 220857, 220853 }}},																				//Tower of the Cursed, Lv.1
			{139274,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4296, 4746 }, range = new List<int>{ 4300, 4304, 5508 }, dangerous = new List<int>{ 220868 }}},																				//Tower of the Cursed, Lv.2
			//{119656,		new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 144315, 169615, 5467 }, range = new List<int>{ 5508 }, dangerous = new List<int>{}}},																							//Heart of the Damned
			{191078,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 191592, 3342, 5239, 3850 }, range = new List<int>{  }, dangerous = new List<int>{}}},																								//Icefall Caves, Lv.1
			{221703,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 3342, 5239, 3850 }, range = new List<int>{  }, dangerous = new List<int>{}}},																										//Icefall Caves, Lv.2
			{189345,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5239, 4548, 4552, 4985 }, range = new List<int>{  }, dangerous = new List<int>{}}},																							//Caverns of Frost, Lv.1
			{221702,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5239, 4548, 4552, 4985 }, range = new List<int>{  }, dangerous = new List<int>{ 212750 }}},																							//Caverns of Frost, Lv.2
			{182274,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 170763, 182281, 182285, 62736, 221770, 130794 }, range = new List<int>{ 170781, 182279 }, dangerous = new List<int>{}}},																//Battlefield Stores, Lv.1
			{221754,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 170763, 182281, 182285, 62736, 221770, 130794 }, range = new List<int>{ 170781, 182279 }, dangerous = new List<int>{}}},																//Battlefield Stores, Lv.2
			{93173,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4085, 4091, 77796, 62736 }, range = new List<int>{ 5581 }, dangerous = new List<int>{ 220395, 220397 }}},																	//Stonefort
			{112565,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4295, 4085, 221770, 4738, 130794 }, range = new List<int>{ 4303, 4300, 5581, 60722 }, dangerous = new List<int>{ 218566, 220705, 220708 }}},															//Fields of Slaughter
			{155048,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 4295, 4738 }, range = new List<int>{ 60722 }, dangerous = new List<int>{ 220699 }}},																									//The Bridge of Korsikk
			{112548,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4295, 60722, 141194, 4738, 4106, 221770 }, range = new List<int>{ 4300, 5581 }, dangerous = new List<int>{ 220509, 220688, 220691 }}},																//The Battlefields
			{86080,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4296, 4085, 4738, 4747, 4746,  144315 }, range = new List<int>{ 189852, 4300, 5581, 365 }, dangerous = new List<int>{ 220777, 220381, 220795 }}}, // REMOVED 203048 (Horrible burrow animations)	 																			//Arreat Crater, Lv.1
			{119305,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 4296, 4085, 4738, 169615, 4747, 4746,  144315 }, range = new List<int>{ 4300, 5581, 365, 205767, 5508 }, dangerous = new List<int>{ 220853, 220850, 220851 }}}, // REMOVED 203048 (Horrible burrow animations)															//Arreat Crater, Lv.2
			{80791,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 144315, 4085, 169615, 4747, 4746 }, range = new List<int>{ 365, 4073, 5508 }, dangerous = new List<int>{ 220810, 220783, 220435, 220806 }}},																			//Tower of the Damned, Lv.1
			{80792,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 144315, 4085, 169615, 4747, 4746 }, range = new List<int>{ 365, 4073, 5508 }, dangerous = new List<int>{ 220817, 220814 }}},																			//Tower of the Damned, Lv.2
			{119306,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 121353, 4747, 4746 }, range = new List<int>{ 5581, 5508 }, dangerous = new List<int>{ 220884, 220881, 220889 }}},																										//The Core of Arreat
			{174666,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 141194, 62736, 5514, 221770, 4985, 4984, 4746, 130794 }, range = new List<int>{ 60722 }, dangerous = new List<int>{}}},																					//Fortified Bunker, Lv.1
			{221752,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 141194, 62736, 221770, 4985, 4984, 4746, 130794 }, range = new List<int>{ 60722 }, dangerous = new List<int>{}}},																					//Fortified Bunker, Lv.2
			{182502,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 141194, 60049, 5436, 62736, 130794, 106707 }, range = new List<int>{ 60722, 5508 }, dangerous = new List<int>{}}},																					//The Foundry, Lv.1
			{221761,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 141194, 60049, 5436, 62736, 130794 }, range = new List<int>{ 60722, 5508 }, dangerous = new List<int>{}}},                                                                                    //The Foundry, Lv.2
			#endregion
			#region Act IV
			{109526,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 3361, 82764, 106712, 121353 }, range = new List<int>{ 106713 }, dangerous = new List<int>{}}},																						//Hell Rift, lv.1
			{109531,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 82764, 106708, 3362, 106712, 121353 }, range = new List<int>{ 205767, 152679 }, dangerous = new List<int>{}}},																		//Hell Rift, lv.2
			{109514,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 3361, 82764, 106708, 106712, 106711 }, range = new List<int>{ 106713, 136864, 152679 }, dangerous = new List<int>{ 219651, 219727, 219668 }}},														//Garden of Hope, lv.1
			{109516,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 106711, 134416, 82764, 4755, 106708, 60049, 3362, 106712, 106714 }, range = new List<int>{ 106713, 152679 }, dangerous = new List<int>{ 219925 }}},									//Garden of Hope, lv.2
			{201668,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 141196, 60049 }, range = new List<int>{  }, dangerous = new List<int>{}}},																									//Blessed Chancel
			{201671,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 141196 }, range = new List<int>{  }, dangerous = new List<int>{}}},																											//Sacellum of Virtue
			{201670,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 106711, 106708 }, range = new List<int>{  }, dangerous = new List<int>{ 223691 }}},																									//Radiant Chapel
			{175738,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 136864 }, range = new List<int>{  }, dangerous = new List<int>{}}},																											//Holy Sanctum
			{109538,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 106713, 136864, 106711, 82764, 4760, 4757, 137856, 106708, 199478, 106712, 106714 }, range = new List<int>{ 219673, 106709 }, dangerous = new List<int>{ 219847, 219893, 219916 }}},					//The Silver Spire, Lv.1
			{109540,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 136864, 3361, 106711, 106710, 82764, 4760, 4757, 106708, 199478, 106714 }, range = new List<int>{ 106709 }, dangerous = new List<int>{ 219949, 219960, 219985, 218873 }}},									//The Silver Spire, Lv.2
			{109542,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 106711, 82764, 199478, 106710, 82764, 199478, 134416 }, range = new List<int>{ 152679, 219673, 5508 }, dangerous = new List<int>{}}},											//The Silver Spire, Lv.4
			{210728,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 106713, 136864, 106711, 82764, 4760, 4757, 137856, 106708, 199478, 106712, 106714, 3361, 106710, 175614 }, range = new List<int>{ 219673, 106709 }, dangerous = new List<int>{}}},    //The Silver Spire, Lv.5
			#endregion

			#region Act V
			{261758,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 273417, 272330, 282027, 276309, 309114, 277203, 276495, 310894, 310893 }, range = new List<int>{ 282789, 310888 }, dangerous = new List<int>{ 360853, 360861, 369424, 309462 }}},										//Westmarch Commons
			{338946,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 341711, 297708, 262442, 342218, 342237, 309114, 051356, 342244, 334796, 334792, 334798 }, range = new List<int>{ }, dangerous = new List<int>{ 351183, 373821, 373819, 361417 }}},										//Briarthorn Cemetery
			{263493,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 273417, 272330, 262442, 330603, 282027, 276309, 277203, 288691, 310893, 298827, 299231 }, range = new List<int>{ 282789, 310888 }, dangerous = new List<int>{ 373830, 363986, 355667, 355672, 355680, 353443 }}},										//Westmarch Heights
			{282487,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 237333, 361665 }, range = new List<int>{ }, dangerous = new List<int>{}}},										//Overgrown Ruins
			{245964,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 239014, 237333, 239516, 361665 }, range = new List<int>{ }, dangerous = new List<int>{ 361755, 361771 }}},										//Blood Marsh
			{258142,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 239014, 237333, 363639, 289387, 254175, 246343, 005276, 239516, 237876 }, range = new List<int>{ 289388 }, dangerous = new List<int>{ 290453, 361991, 351179 }}},										//Paths of the Drowned
			{283553,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 276492, 343183, 347255, 345949, 283269, 351023 }, range = new List<int>{ }, dangerous = new List<int>{ 373881, 362303, 362299 }}},										//Passage to Corvus
			{283567,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 276492, 343183, 347255, 345949, 283269, 351023 }, range = new List<int>{ }, dangerous = new List<int>{ 342355, 362309, 362305, 373842 }}},										//Ruins of Corvus
			{338602,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 296283, 271579, 271806 }, range = new List<int>{ }, dangerous = new List<int>{ 363108, 363060, 373883, 362895, 362891, 341760, 363051, 363073 }}}, //REMOVED 275108 (horrible tunnel animation)										//Battlefields of Eternity
			{271234,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 241288, 305579 }, range = new List<int>{ }, dangerous = new List<int>{ 363374, 363378, 363228, 363232, 363367 }}},										//Pandemonium Fortress lv. 1
			{360494,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 241288, 305579, 340920, 299231 }, range = new List<int>{ }, dangerous = new List<int>{ 360243, 360244, 360241, 363230 }}},                                        //Pandemonium Fortress lv. 2
			#endregion
			//x1_lr_tileset_Westmarch
			{331263,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 341711, 297708, 262442, 342218, 342237, 309114, 051356, 342244, 334796, 334792, 334798 }, range = new List<int>{ }, dangerous = new List<int>{ 351183, 373821, 373819, 361417 }}},
			//_x1_lr_tileset_fortress_large
			{360797,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 241288, 305579, 340920, 299231 }, range = new List<int>{ }, dangerous = new List<int>{ 363374, 363378, 363228, 363232, 363367, 360243, 360244, 360241, 363230 }}},
			//x1_lr_tileset_zoltruins
			{288823,     new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 5396, 5432, 208962, 208963, 204944, 5512 }, range = new List<int>{ 5428 }, dangerous = new List<int>{}}},
			//x1_lr_tileset_hexmaze

			//x1_lr_tileset_icecave
			{275960, new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 191592, 3342, 5239, 3850 }, range = new List<int>{  }, dangerous = new List<int>{}}},																								
			//x1_lr_tileset_crypt
			{275946,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = false, melee = new List<int>{ 5393, 3848,  5395, 5411, 6653 }, range = new List<int>{ 5346, 3847, 6639 }, dangerous = new List<int>{ 218348, 218351 }}},
			//x1_lr_tileset_corruptspire
			{275926,        new MonsterLayout{ lazy_load = false, additional_density = 0, can_spawn_goblin = true, melee = new List<int>{ 106711, 82764, 199478, 106710, 82764, 199478, 134416 }, range = new List<int>{ 152679, 219673, 5508 }, dangerous = new List<int>{}}},
			//

		};
	}
}
