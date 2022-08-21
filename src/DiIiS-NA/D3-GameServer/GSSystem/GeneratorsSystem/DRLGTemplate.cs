//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public static class DRLGTemplate
	{
		public struct DRLGLayout
		{
			public int enterPositionX;
			public int enterPositionY;
			public int exitPositionX;
			public int exitPositionY;
			public List<List<int>> map;
		};

		public static readonly Dictionary<WorldSno, List<DRLGLayout>> Templates = new Dictionary<WorldSno, List<DRLGLayout>>
		{
		#region Cathedral
			{WorldSno.a1trdun_level01, //cath, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 2,
						exitPositionX = 2,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0,  0,  0, 0},
							new List<int>{0, 0,  2,  0, 0},
							new List<int>{0, 2,  3,  0, 0},
							new List<int>{0, 7, 15, 8, 0},
							new List<int>{0, 5,  9,  0, 0},
							new List<int>{0, 0,  0,  0, 0},
						}
					},
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0,  0,  0, 0},
							new List<int>{0, 2,  2,  0, 0},
							new List<int>{0, 7, 11, 2, 0},
							new List<int>{0, 5, 13, 9, 0},
							new List<int>{0, 0,  0,  0, 0},
						}
					},
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 2,
						exitPositionX = 3,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0,  0,  0, 0, 0},
							new List<int>{0, 0,  0,  2, 0, 0},
							new List<int>{0, 0,  2,  3, 0, 0},
							new List<int>{0, 4, 13, 13, 8, 0},
							new List<int>{0, 0, 0, 0, 0, 0},
						}
					}
				}
			},
			{WorldSno.a1trdun_level04, //cath, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 4,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0},
							new List<int>{0, 4, 10, 0, 0},
							new List<int>{0, 4, 11, 2, 0},
							new List<int>{0, 0, 5, 11, 0},
							new List<int>{0, 0, 0, 1, 0},
							new List<int>{0, 0, 0, 0, 0},
						}
					},
				}
			},
			{WorldSno.a1trdun_level06, //cath, 4th level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 3,
						exitPositionX = 4,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
							new List<int>{0, 0, 0, 0, 2, 0, 0, 0},
							new List<int>{0, 0, 6, 12, 11, 0, 0, 0},
							new List<int>{0, 0, 3, 6, 15, 12, 8, 0},
							new List<int>{0, 4, 13, 13, 11, 0, 0, 0},
							new List<int>{0, 0, 0, 0, 1, 0, 0, 0},
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
						}
					},
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 3,
						exitPositionX = 1,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
							new List<int>{0, 0, 0, 6, 8, 0, 0, 0},
							new List<int>{0, 0, 6, 15, 14, 8, 0, 0},
							new List<int>{0, 4, 11, 1, 7, 12, 8, 0},
							new List<int>{0, 0, 5, 12, 9, 0, 0, 0},
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
						}
					}
				}
			},
		#endregion
		#region Araneae	
			{WorldSno.a1dun_spidercave_01, //Caverns of Araneae
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 2,
						exitPositionX = 5,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
							new List<int>{0, 0, 0, 2, 0, 0, 0, 0},
							new List<int>{0, 0, 4, 15, 8, 0, 0, 0},
							new List<int>{0, 0, 2, 3, 0, 2, 0, 0},
							new List<int>{0, 4, 15, 15, 8, 3, 0, 0},
							new List<int>{0, 4, 15, 15, 12, 15, 8, 0},
							new List<int>{0, 0, 1, 1, 0, 1, 0, 0},
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0},
						}
					},
				}
			},
		#endregion	
		#region Fields of Misery	
			{WorldSno.a1_cave_fields_minecavea_level01, //Lost Mine, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 6, 8, 0, },
							new List<int>{0, 4, 12, 15, 8, 0, },
							new List<int>{0, 0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a1_cave_fields_minecavea_level02, //Lost Mine, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 2,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 14, 14, 10, 0, },
							new List<int>{0, 0, 3, 3, 1, 0, },
							new List<int>{0, 0, 1, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.trdun_crypt_fields_flooded_memories_level01, //Decaying Crypt, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 5,
						exitPositionX = 3,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 14, 8, 0, },
							new List<int>{0, 5, 9, 0, 0, },
							new List<int>{0, 3, 0, 0, 0, },
							new List<int>{0, 3, 0, 0, 0, },
							new List<int>{0, 5, 8, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.trdun_crypt_fields_flooded_memories_level02, //Decaying Crypt, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 4, 15, 8, 0, },
							new List<int>{0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.fields_cave_swordofjustice_level01, //Khazra Den
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 4, 15, 8, 0, },
							new List<int>{0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a1_cave_fields_scavengerden_level01, //Scavenger's Den 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 3,
						exitPositionX = 2,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 6, 11, 0, 0, },
							new List<int>{0, 5, 9, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a1_cave_fields_scavengerden_level02, //Scavenger's Den 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 2,
						exitPositionX = 3,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 12, 8, 0, },
							new List<int>{0, 1, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion	
		#region Defiled crypts
			{WorldSno.trdun_crypt_skeletonkingcrown_00, //Defiled crypts, true
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 3,
						exitPositionX = 3,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 0, 0, },
							new List<int>{0, 7, 13, 10, 0, },
							new List<int>{0, 1, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.trdun_crypt_falsepassage_01, //Defiled crypts, falseOne
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 2,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 2, 7, 10, 0, },
							new List<int>{0, 5, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.trdun_crypt_falsepassage_02, //Defiled crypts, falseTwo
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, },
							new List<int>{0, 2, 2, 0, },
							new List<int>{0, 5, 11, 0, },
							new List<int>{0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion
		#region 4th Act
			{WorldSno.a4dun_spire_level_01, //The Silver Spire, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a4dun_spire_level_02, //The Silver Spire, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 2, 0, },
							new List<int>{0, 0, 6, 12, 9, 0, },
							new List<int>{0, 0, 7, 10, 0, 0, },
							new List<int>{0, 4, 13, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a4dun_spire_level_03, //The Silver Spire, 4th level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 6,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 12, 12, 12, 12, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 6,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 12, 12, 12, 12, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, 0, },
						}
					}
				}
			},
			{WorldSno.a4dun_spire_level_04, //The Silver Spire, 5th level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 6, 12, 8, 0, },
							new List<int>{0, 0, 5, 10, 0, 0, },
							new List<int>{0, 0, 6, 15, 10, 0, },
							new List<int>{0, 4, 9, 5, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion
		#region 3rd Act
			{WorldSno.a3dun_keep_level03, //The Keep Depths, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_level04, //The Keep Depths, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 5,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 10, 0, 0, 0, },
							new List<int>{0, 0, 3, 0, 2, 0, },
							new List<int>{0, 0, 7, 10, 3, 0, },
							new List<int>{0, 0, 5, 13, 11, 0, },
							new List<int>{0, 0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
}
					},
				}
			},
			{WorldSno.a3dun_keep_level05, //The Keep Depths, 3rd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 4,
						exitPositionX = 2,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 0, 3, 0, 0, },
							new List<int>{0, 0, 3, 0, 0, },
							new List<int>{0, 6, 15, 8, 0, },
							new List<int>{0, 1, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_icecaves_timed_01, //Icefall Caves, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 3,
						exitPositionX = 1,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, },
							new List<int>{0, 6, 10, 0, },
							new List<int>{0, 3, 3, 0, },
							new List<int>{0, 1, 1, 0, },
							new List<int>{0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_icecaves_timed_01_level_02, //Icefall Caves, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 4, 12, 10, 0, },
							new List<int>{0, 4, 12, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_icecaves_random_01, //Caverns of Frost, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 2, 2, 0, 0, },
							new List<int>{0, 3, 3, 0, 0, },
							new List<int>{0, 5, 13, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_icecaves_random_01_level_02, //Caverns of Frost, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 12, 8, 0, },
							new List<int>{0, 3, 0, 0, 0, },
							new List<int>{0, 5, 8, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_01, //Fortified Bunker, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_01_level_02, //Fortified Bunker, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_02, //The Barracks, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_02_level_02, //The Barracks, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_03, //Battlefield Stores, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_03_level_02, //Battlefield Stores, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 4,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 6, 10, 2, 0, },
							new List<int>{0, 5, 15, 9, 0, },
							new List<int>{0, 0, 5, 10, 0, },
							new List<int>{0, 0, 0, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_04, //The Foundry, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 4,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 10, 0, 0, 0, },
							new List<int>{0, 0, 7, 14, 8, 0, },
							new List<int>{0, 0, 5, 11, 0, 0, },
							new List<int>{0, 0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a3dun_keep_random_04_level_02, //The Foundry, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 6, 8, 0, },
							new List<int>{0, 0, 6, 13, 8, 0, },
							new List<int>{0, 4, 9, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion
		#region 2nd Act
			{WorldSno.a2c1dun_swr_caldeum_01, //Sewers of Caldeum
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 5,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 2, 0, 0, },
							new List<int>{0, 0, 0, 7, 8, 0, },
							new List<int>{0, 4, 14, 9, 0, 0, },
							new List<int>{0, 4, 15, 8, 0, 0, },
							new List<int>{0, 0, 1, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2c2dun_cave_random01, //Sirocco Caverns, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 4,
						exitPositionX = 2,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2c2dun_cave_random01_level02, //Sirocco Caverns, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, },
							new List<int>{0, 4, 9, 0, },
							new List<int>{0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2c2dun_zolt_treasurehunter, //Chamber of the Lost Idol
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 4, 10, 0, 0, },
							new List<int>{0, 0, 3, 0, 0, },
							new List<int>{0, 4, 15, 8, 0, },
							new List<int>{0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_aqd_special_a, //Western Channel
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 2, 2, 2, 0, },
							new List<int>{0, 2, 2, 5, 15, 9, 0, },
							new List<int>{0, 7, 13, 10, 7, 10, 0, },
							new List<int>{0, 1, 4, 13, 15, 9, 0, },
							new List<int>{0, 0, 0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_aqd_special_b, //Eastern Channel
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 4,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 14, 8, 0, 0, },
							new List<int>{0, 6, 13, 8, 2, 0, },
							new List<int>{0, 5, 12, 14, 15, 8, },
							new List<int>{0, 0, 0, 1, 1, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_aqd_oasis_randomfacepuzzle_small, //Tomb of Sardar
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 4,
						exitPositionX = 1,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 6, 8, 0, },
							new List<int>{0, 4, 11, 0, 0, },
							new List<int>{0, 4, 7, 8, 0, },
							new List<int>{0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large, //Tomb of Khan Dakab
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 2, 2, 0, },
							new List<int>{0, 0, 4, 7, 11, 0, },
							new List<int>{0, 0, 6, 11, 1, 0, },
							new List<int>{0, 6, 9, 5, 8, 0, },
							new List<int>{0, 5, 8, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_boneyard_worm_cave_01, //Cave of Burrowing Horror, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 4, 14, 8, 0, },
							new List<int>{0, 0, 4, 15, 8, 0, },
							new List<int>{0, 6, 12, 9, 0, 0, },
							new List<int>{0, 1, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_boneyard_worm_cave_02, //Cave of Burrowing Horror, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, },
							new List<int>{0, 0, 3, 0, },
							new List<int>{0, 4, 9, 0, },
							new List<int>{0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_cave_bloodvial_01, //Cave of the Betrayer, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_cave_bloodvial_02, //Cave of the Betrayer, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, },
							new List<int>{0, 4, 15, 8, 0, },
							new List<int>{0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_cave_mapdungeon_level01, //Mysterious Cave, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_cave_mapdungeon_level02, //Mysterious Cave, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 1,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 4, 10, 0, 0, },
							new List<int>{0, 4, 13, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_swr_swr_to_oasis_level01, //Ruined Cistern
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_shadowrealm_level01, //Realm of Shadow
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_head_random01, //The Forgotten Ruins
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_sw_random01, //The Ruins, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_sw_random01_level02, //The Ruins, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_blood02, //Vault of the Assassin
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_timed01_level01, //The Crumbling Vault
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 1,
						exitPositionX = 4,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 6, 8, 0, 0, },
							new List<int>{0, 4, 13, 10, 0, 0, },
							new List<int>{0, 0, 4, 13, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 2,
						exitPositionX = 5,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 2, 0, 0, 0, 0, },
							new List<int>{0, 4, 13, 10, 0, 0, 0, },
							new List<int>{0, 0, 4, 13, 12, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_level01, //The Unknown Depths
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 2, 0, 0, },
							new List<int>{0, 5, 12, 12, 15, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_level02, //The Storm Halls
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 2, 0, 0, },
							new List<int>{0, 5, 12, 12, 15, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_level03, //Halls of Dusk
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2dun_zolt_timed01_level02, //Vault Treasure Room
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, },
							new List<int>{0, 2, 0, },
							new List<int>{0, 1, 0, },
							new List<int>{0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_cave_oasis_random01, //Ancient Cave, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_cave_oasis_random01_level02, //Ancient Cave, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_cave_oasis_random02, //Flooded Cave, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_cave_oasis_random02_level02, //Flooded Cave, 2nd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_boneyard_spider_cave_01, //Vile Cavern, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.a2trdun_boneyard_spider_cave_02, //Vile Cavern, 2st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 1,
						exitPositionX = 2,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 6, 8, 0, 0, 0, 0, },
							new List<int>{0, 5, 12, 12, 14, 10, 0, },
							new List<int>{0, 0, 4, 12, 13, 9, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion
		#region Halls of Agony
			{WorldSno.trdun_leoric_level01, //Halls of Agony, 1st level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 6, 8, 0, },
							new List<int>{0, 0, 6, 11, 0, 0, },
							new List<int>{0, 6, 13, 9, 0, 0, },
							new List<int>{0, 1, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.trdun_leoric_level03, //Halls of Agony, 3rd level
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 1,
						exitPositionX = 4,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 4, 10, 0, 0, 0, },
							new List<int>{0, 0, 3, 0, 2, 0, },
							new List<int>{0, 0, 7, 14, 9, 0, },
							new List<int>{0, 4, 13, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
		#endregion
		#region Westmarch
			{WorldSno.x1_westm_zone_01, //Westmarch commons
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 2,
						exitPositionX = 4,
						exitPositionY = 6,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 6, 10, 0, },
							new List<int>{0, 0, 4, 15, 11, 0, },
							new List<int>{0, 4, 14, 15, 9, 0, },
							new List<int>{0, 0, 7, 11, 0, 0, },
							new List<int>{0, 0, 7, 11, 0, 0, },
							new List<int>{0, 0, 5, 13, 8, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_westm_graveyard_deathorb, //Briarthorn Cemetery
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 2,
						enterPositionY = 7,
						exitPositionX = 10,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,  0,  0,  0,  0 },
							new List<int>{0, 0,     0,  0,  0,  0,      265327, 14, 14, 14, 265297, 0 },
							new List<int>{0, 265327, 14, 14, 14, 265297, 7,     15, 15, 15, 11,     0 },
							new List<int>{0, 7,     15, 15, 15, 265340, 265362, 15, 15, 15, 11,     0 },
							new List<int>{0, 7,     15, 15, 15, 265411, 265374, 15, 15, 15, 11,     0 },
							new List<int>{0, 7,     15, 15, 15, 11,     265314, 13, 13, 13, 265223, 0 },
							new List<int>{0, 7,     15, 15, 15, 11,     0,      0,  0,  0,      0 },
							new List<int>{0, 265314, 13, 13, 13, 265223, 0},
							new List<int>{0, 0,     0, 0, 0, 0,         0},
						}
					},
				}
			},
			{WorldSno.x1_westm_zone_03, //Westmarch heights
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 3,
						exitPositionX = 5,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 2, 0, 0, 0, },
							new List<int>{0, 0, 4, 15, 12, 10, 0, },
							new List<int>{0, 4, 14, 11, 0, 7, 8, },
							new List<int>{0, 4, 13, 11, 0, 1, 0, },
							new List<int>{0, 0, 0, 1, 0, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 1,
						enterPositionY = 3,
						exitPositionX = 4,
						exitPositionY = 5,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
							new List<int>{0, 0, 0, 2, 2, 0, 0, },
							new List<int>{0, 0, 2, 3, 3, 0, 0, },
							new List<int>{0, 4, 15, 15, 11, 0, 0, },
							new List<int>{0, 0, 1, 5, 11, 0, 0, },
							new List<int>{0, 0, 0, 0, 7, 8, 0, },
							new List<int>{0, 0, 0, 0, 1, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_pand_ext_2_battlefields, //Battlefields of Eternity
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 7,
						map = new List<List<int>>{
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      0,      0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      268742, 348217, 0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  106,    -1,     7,      11,     0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  -1,     -1,     6,      10,     268788, 0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      7,      15,     15,     11,     0,      0, },
							new List<int>{0, 0,     0,  0,  0,  348399, 6,      15,     15,     10,     268788, 0, },
							new List<int>{0, 0,     0,  0,  0,  7,      15,     15,     15,     15,     11,     0, },
							new List<int>{0, 6,     14, 14, 14, 6,      346282, 15,     15,     15,     11,     0, },
							new List<int>{0, 295143,13, 13, 13, 5,      15,     15,     15,     15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  295143, 13,     13,     5,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      7,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      295143, 5,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      0,      7,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      0,      268754, 268776, 0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,      0,      0,      0,      0,      0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 7,
						map = new List<List<int>>{
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  268742, 348217, 0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  106,    -1, 7,      11,     0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  -1,     -1, 6,      10,     268788, 0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      7,  15,     15,     11,     0,      0, },
							new List<int>{0, 0,     0,  0,  0,  348399, 6,  15,     15,     10,     268788, 0, },
							new List<int>{0, 0,     0,  0,  0,  7,      15, 15,     15,     15,     11,     0, },
							new List<int>{0, 6,     14, 14, 14, 6,      15, 15,     115,    -1,     11,     0, },
							new List<int>{0, 295143,13, 13, 13, 5,      15, 15,     -1,     -1,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  295143, 13, 13,     5,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      7,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      295143, 5,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      7,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      268754, 268776, 0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 7,
						map = new List<List<int>>{
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  268742, 348217, 0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  106,    -1, 7,      11,     0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  -1,     -1, 6,      10,     268788, 0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      7,  15,     15,     11,     0,      0, },
							new List<int>{0, 0,     0,  0,  0,  348399, 6,  15,     15,     10,     268788, 0, },
							new List<int>{0, 0,     0,  0,  0,  7,      15, 15,     15,     15,     11,     0, },
							new List<int>{0, 6,     14, 14, 14, 6,      15, 15,     15,     15,     300692, -1, 0},
							new List<int>{0, 295143,13, 13, 13, 5,      15, 15,     15,     15,     -1,     -1, 0},
							new List<int>{0, 0,     0,  0,  0,  295143, 113,-1,     5,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      -1, -1,     7,      15,     11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      295143, 5,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      7,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      268754, 268776, 0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
						}
					},
					new DRLGLayout{
						enterPositionX = 5,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 7,
						map = new List<List<int>>{
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  268742, 348217, 0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  106,    -1, 7,      11,     0,      0,      0, },
							new List<int>{0, 0,     0,  0,  0,  -1,     -1, 6,      10,     268788, 0,      0, },
							new List<int>{0, 0,     0,  0,  0,  0,      7,  15,     15,     11,     0,      0, },
							new List<int>{0, 0,     0,  0,  0,  348399, 6,  15,     15,     10,     268788, 0, },
							new List<int>{0, 0,     0,  0,  0,  7,      15, 15,     15,     15,     11,     0, },
							new List<int>{0, 6,     14, 14, 14, 6,      15, 15,     15,     15,     11,     0,  0},
							new List<int>{0, 295143,13, 13, 13, 5,      15, 15,     15,     15,     300692, -1, 0},
							new List<int>{0, 0,     0,  0,  0,  295143, 13, 13,     5,      15,     -1,     -1, 0},
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      7,      15,     11,     0,  0},
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      295143, 5,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      7,      11,     0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      268754, 268776, 0, },
							new List<int>{0, 0,     0,  0,  0,  0,      0,  0,      0,      0,      0,      0, },
						}
					},
				}
			},
			{WorldSno.x1_fortress_level_01, //Pandemonius fortress lv. 1
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 4,
						exitPositionX = 4,
						exitPositionY = 2,
						map = new List<List<int>>{
							new List<int>{0, 0, 0,      0,      0, 0, },
							new List<int>{0, 6, 14,     12,     10, 0, },
							new List<int>{0, 7, 11,     0,      1, 0, },
							new List<int>{0, 3, 345345, 344012, 0, 0, },
							new List<int>{0, 1, 0,      342542, 0, 0, },
							new List<int>{0, 0, 0,      0,      0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_fortress_level_02, //Pandemonius fortress lv. 2
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 4,
						enterPositionY = 6,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0,         0,  0,  0,      0,      0, },
							new List<int>{0, 356572,    12, 10, 0,      0,      0, },
							new List<int>{0, 0,         0,  3,  0,      0,      0, },
							new List<int>{0, 0,         0,  3,  0,      0,      0, },
							new List<int>{0, 4,         12, 13, 10,     0,      0, },
							new List<int>{0, 0,         0,  0,  348859, 348855, 0, },
							new List<int>{0, 0,         0,  0,  348863, 348867, 0, },
							new List<int>{0, 0,         0,  0,  0,      0,      0, },
						}
					},
				}
			},
			{WorldSno.x1_catacombs_level02, //Ruins of Corvus
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 8,
						enterPositionY = 1,
						exitPositionX = 3,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0, 0,  0,  0, 0,   0,  0,  0, 0, },
							new List<int>{0, 0, 0,  0,  6, 14,  12, 12, 8, 0, },
							new List<int>{0, 0, 0,  0,  7, 9,   0,  0,  0, 0, },
							new List<int>{0, 0, 0,  2,  3, 0,   0,  0,  0, 0, },
							new List<int>{0, 4, 12, 13, 9, 0,   0,  0,  0, 0, },
							new List<int>{0, 0, 0,  0,  0, 0,   0,  0,  0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_catacombs_level01, //Path to Corvus
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 2, 0, 0, 0, },
							new List<int>{0, 3, 2, 2, 0, },
							new List<int>{0, 7, 15, 9, 0, },
							new List<int>{0, 5, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_catacombs_fakeentrance_02, //Path to Corvus (fake)
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 2, 0, 0, 0, },
							new List<int>{0, 3, 2, 2, 0, },
							new List<int>{0, 7, 15, 9, 0, },
							new List<int>{0, 5, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_catacombs_fakeentrance_03, //Path to Corvus (fake)
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 2, 0, 0, 0, },
							new List<int>{0, 3, 2, 2, 0, },
							new List<int>{0, 7, 15, 9, 0, },
							new List<int>{0, 5, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_catacombs_fakeentrance_04, //Path to Corvus (fake)
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 3,
						enterPositionY = 2,
						exitPositionX = 1,
						exitPositionY = 1,
						map = new List<List<int>>{
							new List<int>{0, 0, 0, 0, 0, },
							new List<int>{0, 2, 0, 0, 0, },
							new List<int>{0, 3, 2, 2, 0, },
							new List<int>{0, 7, 15, 9, 0, },
							new List<int>{0, 5, 9, 0, 0, },
							new List<int>{0, 0, 0, 0, 0, },
						}
					},
				}
			},
			{WorldSno.x1_bog_01, //Bog (the whole)
				new List<DRLGLayout>{
					new DRLGLayout{
						enterPositionX = 8,
						enterPositionY = 9,
						exitPositionX = 1,
						exitPositionY = 3,
						map = new List<List<int>>{
							new List<int>{0, 0,         0,      0,      0,      0,      0,      0,      0,      0,      0,      0,  0,      0,      0, },
							new List<int>{0, 0,         0,      0,      0,      235664, 14,     235676, 235664, 14,     265655, 14, 235676, 0,      0, },
							new List<int>{0, 235664,    14,     235676, 235664, 9,      15,     5,      9,      15,     15,     15, 5,      235676, 0, },
							new List<int>{0, 265678,    15,     5,      9,      15,     15,     15,     15,     15,     15,     15, 15,     11,     0, },
							new List<int>{0, 7,         15,     15,     15,     15,     6,      265693, 10,     15,     15,     15, 15,     11,     0, },
							new List<int>{0, 7,         251911, 15,     15,     6,      235698, 0,      235710, 10,     15,     15, 6,      235698, 0, },
							new List<int>{0, 235710,    10,     15,     15,     11,     0,      0,      0,      235710, 267877, -1, 235698, 0,      0, },
							new List<int>{0, 0,         7,      15,     15,     265624, 0,      0,      0,      235664, -1,     -1, 14,     235676, 0, },
							new List<int>{0, 0,         235710, 13,     13,     235698, 0,      0,      235664, 9,      15,     15, 15,     11,     0, },
							new List<int>{0, 0,         0,      0,      0,      0,      0,      0,      7,      15,     15,     15, 6,      235698, 0, },
							new List<int>{0, 0,         0,      0,      0,      0,      0,      0,      235710, 13,     13,     13, 235698, 0,      0, },
							new List<int>{0, 0,         0,      0,      0,      0,      0,      0,      0,      0,      0,      0,  0,      0,      0, },
						}
					},
				}
			},
		#endregion
		};
	}
}
