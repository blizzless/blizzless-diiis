//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public static class LoreRegistry
	{
		public struct LoreLayout
		{
			public Dictionary<int, List<int>> chests_lore;
		};

		public static readonly Dictionary<WorldSno, LoreLayout> Lore = new Dictionary<WorldSno, LoreLayout>
		{
			//////////////////////////////////////////////////////////////////////////////////////////////////Act I///////////////////////////////////////////////////////////////////////////////////////////  
			{WorldSno.trout_town,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{
				{224686, new List<int>{158668}},
				{199346, new List<int>{201960}},
				{158681, new List<int>{98897}},
				{230235, new List<int>{211562}},
				{154435, new List<int>{154423}},
				{166611, new List<int>{156447, 156451}},
				{85791, new List<int>{91532, 91534, 91535, 131018, 91537, 91538, 91539, 158660, 91536}},
				{146701, new List<int>{156433}},
				{230231, new List<int>{211558}},
				{96594, new List<int>{108067, 108068, 108071}},
				{230240, new List<int>{211565}},
				{3341, new List<int>{151112}},
				{156653, new List<int>{156659}},
				{156682, new List<int>{156683}},
				{119801, new List<int>{106821}},
				{137125, new List<int>{106823}},
				{108792, new List<int>{145423}},                }}},																		//NewTristram
			{WorldSno.trout_tristram_inn,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {230232, new List<int>{211567}}, }}},											//New Tristram tavern
			{WorldSno.trout_tristram_leahsroom,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {86817, new List<int>{86639, 89489, 89519, 89520, 89521, 89522, 89523}}, }}},	//New Tristram Leah's room
			{WorldSno.trout_tristram_cainshouse,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {115124, new List<int>{115115, 167797}}, }}},									//Cain's house
			{WorldSno.a1trdun_level01,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {85790, new List<int>{85757, 85759}}, }}},									//Sobor: level 1
			{WorldSno.a1trdun_level06,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {85790, new List<int>{85779, 85780, 85781}}, }}},								//Sobor: level 4
			{WorldSno.trdun_leoric_level01,      new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{84556}},
																					  {5891, new List<int>{156458}},
																					  {170633, new List<int>{85714}}, }}},											//Hall agony: level 1
			{WorldSno.trdun_leoric_level02,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{85724}},
																					  {5891, new List<int>{156460}},
																					  {170633, new List<int>{85719}}, }}},											//Hall agony: level 2
			{WorldSno.trdun_leoric_level03,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{85729}}, }}},											//Hall agony: level 3
			{WorldSno.a1dun_spidercave_01,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {167350, new List<int>{154405, 156462, 156453, 156455}}, }}},					//Сave Arana
			{WorldSno.trdun_jail_level01,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {105758, new List<int>{107268}}, }}},											//Damn Fort
			{WorldSno.a1dun_leor_manor,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {187436, new List<int>{144181}}, }}},											//Leoric Manor
			{WorldSno.a1_cave_highlands_goatcavea_level01,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {166661, new List<int>{166878}}, }}},											//Сave clan moon: level 1	
			{WorldSno.a1_cave_highlands_goatcavea_level02,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {166661, new List<int>{166896, 166898}}, }}},									//Сave clan moon: level 2	
			{WorldSno.trdun_cave_nephalem_03,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {137189, new List<int>{119733}}, }}},											//Sunken temple
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act II/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.caout_town, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{
				{170233, new List<int>{170224}},
				{194145, new List<int>{189736, 189764, 189779, 189781, 189783, 189785}},
				{169836, new List<int>{171460, 169932}},
				{170238, new List<int>{170257}},
				{145599, new List<int>{156468}},
				{167210, new List<int>{156474}},
				{145607, new List<int>{156476}},
				{167339, new List<int>{156465}},
				{191734, new List<int>{191717}},
				{216775, new List<int>{148686}},
				{230712, new List<int>{183610}},
				{218649, new List<int>{178770}},
				{192437, new List<int>{189677, 189679, 189687}}, }}},																							//Caldeum
			{WorldSno.a2dun_cave_bloodvial_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216316, new List<int>{148806}}, }}},											//Пещера предателя: уровень 1
			{WorldSno.a2dun_zolt_blood02, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216311, new List<int>{148800}}, }}},											//Гробница наемника
			{WorldSno.a2dun_cave_bloodvial_02, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216768, new List<int>{148812}}, }}},											//Пещера предателя: уровень 2
			{WorldSno.caout_refugeecamp, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192154, new List<int>{189698, 189700, 189702, 189704, 189715}}, }}},			//Тайный лагерь
			{WorldSno.caout_hub_inn, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {190014, new List<int>{189657, 189660, 189664, 189666, 189669, 189671}}, }}},	//Таверна "Жгучие пески"
			{WorldSno.caout_interior_f, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {194145, new List<int>{189789}}, }}},												//Тайный алтарь
			{WorldSno.a2dun_swr_swr_to_oasis_level01,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {194145, new List<int>{189806}}, }}},											//Разрушенный резервуар
			{WorldSno.a2c2dun_zolt_treasurehunter,  new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192325, new List<int>{189826}}, }}},												//Зал Утерянного Идола
			{WorldSno.a2dun_cald_uprising,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {169999, new List<int>{170007}}, }}},										//Caldeum(Uprising)
			{WorldSno.a2dun_cald, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {170063, new List<int>{170064}},
																				  {145601, new List<int>{156470}},}}},											//Caldeum bazaar
			{WorldSno.caout_interior_d, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {167090, new List<int>{156472}}, }}},												//Сокрытый конклав
			{WorldSno.a2c1dun_swr_caldeum_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {145609, new List<int>{156478}}, }}},												//Калдейские стоки
			{WorldSno.a2dun_zolt_head_random01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216022, new List<int>{148672}},
																				  {216805, new List<int>{148707}},}}},												//Заброшенные руины
			{WorldSno.a2dun_aqd_special_a, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216308, new List<int>{148680}},
																				  {189984, new List<int>{189652}},}}},												//Западный канал
			{WorldSno.a2dun_zolt_level02, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216805, new List<int>{148693}}, }}},												//Залы Бурь
			{WorldSno.a2dun_zolt_level01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216805, new List<int>{148701}}, }}},												//Таинственная бездна
			{WorldSno.a2dun_aqd_special_b, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {189984, new List<int>{189654}}, }}},												//Восточный канал
			



			//////////////////////////////////////////////////////////////////////////////////////////////////Act III/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.a3dun_keep_level03, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {204724, new List<int>{204678}},
																				  {213447, new List<int>{204820}},
																				  {213470, new List<int>{204828, 204831, 204839, 204846, 204848}},}}},				//Нижние этажи крепости: уровень 1
			{WorldSno.gluttony_boss, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {212704, new List<int>{204850}}, }}},											//Кладовая
			{WorldSno.a3dun_hub_keep, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {178357, new List<int>{178349}}, }}},											//Главная башня бастиона
			{WorldSno.a3dun_keep_hub_inn, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {212222, new List<int>{204853, 204859, 204875, 204878}}, }}},					//Оружейная
			{WorldSno.a3dun_rmpt_level01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192778, new List<int>{190879, 191086, 191133}},
																				  {213445, new List<int>{204033}}, }}},												//Заоблачные стены
			{WorldSno.a3dun_rmpt_level02, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213446, new List<int>{204817}}, }}},												//Каменный форт
			{WorldSno.a3_battlefields_02, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213445, new List<int>{204822}},
																				  {213446, new List<int>{204824}},}}},												//Поля Кровавой Бойни
			{WorldSno.a3dun_crater_level_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213447, new List<int>{204826}}, }}},												//Арреатский кратер: уровень 1
			{WorldSno.a3dun_keep_random_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {178366, new List<int>{178367}}, }}},											//Укрепленный бункер: уровень 1
			
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act IV/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.a4dun_heaven_hub_keep, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {177462, new List<int>{166775}}, }}},											//Главная башня бастиона
			{WorldSno.a4dun_garden_of_hope_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216482, new List<int>{193560, 193586, 193568}}, }}},							//Сады надежды: уровень 1
			{WorldSno.a4dun_garden_of_hope_random, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216482, new List<int>{193574, 193580, 193592}}, }}},							//Сады надежды: уровень 2
			{WorldSno.a4dun_spire_level_01, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216551, new List<int>{211609, 211611, 211613}},
																				   {216537, new List<int>{211599, 211601, 211603, 211605}},}}},						//Серебрянный шпиль: уровень 1
			
		};
	}
}
