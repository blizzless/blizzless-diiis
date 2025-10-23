using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public static class LoreRegistry
	{
		public struct LoreLayout
		{
			public Dictionary<ActorSno, List<int>> chests_lore;
		};

		public static readonly Dictionary<WorldSno, LoreLayout> Lore = new Dictionary<WorldSno, LoreLayout>
		{
			//////////////////////////////////////////////////////////////////////////////////////////////////Act I///////////////////////////////////////////////////////////////////////////////////////////  
			{WorldSno.trout_town,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{
				{ActorSno._lore_scoundreljournal, new List<int>{158668}},
				{ActorSno._tinkerambush_swickard, new List<int>{201960}},
				{ActorSno._blacksmith_lore, new List<int>{98897}},
				{ActorSno._lore_fieldsofmiserychest, new List<int>{211562}},
				{ActorSno._lore_tinker_diary, new List<int>{154423}},
				{ActorSno._worthampriest_lore, new List<int>{156447, 156451}},
				{ActorSno._oldtristram_lore, new List<int>{91532, 91534, 91535, 131018, 91537, 91538, 91539, 158660, 91536}},
				{ActorSno._lore_darkzealot, new List<int>{156433}},
				{ActorSno._lore_cemetery, new List<int>{211558}},
				{ActorSno._trout_highlands_chiefgoatmenmummyrack_a, new List<int>{108067, 108068, 108071}},
				{ActorSno._lore_highlandschest, new List<int>{211565}},
				{ActorSno._beast_corpse_a_02, new List<int>{151112}},
				{ActorSno._tristramguard_corpse_03_descentevent, new List<int>{156659}},
				{ActorSno._adventurer_a_corpse_01_warrivevent, new List<int>{156683}},
				{ActorSno._adventurer_a_corpse_nephalemcave, new List<int>{106821}},
				{ActorSno._festeringwoods_warriorsrest_lore, new List<int>{106823}},
				{ActorSno._trout_wilderness_hangingtree_gravechest, new List<int>{145423}},                }}},																		//NewTristram
			{WorldSno.trout_tristram_inn,    new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_newtristraminn, new List<int>{211567}}, }}},											//New Tristram tavern
			{WorldSno.trout_tristram_leahsroom,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._leah_lectern, new List<int>{86639, 89489, 89519, 89520, 89521, 89522, 89523}}, }}},	//New Tristram Leah's room
			{WorldSno.trout_tristram_cainshouse,    new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._cain_journal, new List<int>{115115, 167797}}, }}},									//Cain's house
			{WorldSno.a1trdun_level01,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._cath_lecturn__lachdanansscroll, new List<int>{85757, 85759}}, }}},									//Sobor: level 1
			{WorldSno.a1trdun_level06,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._cath_lecturn__lachdanansscroll, new List<int>{85779, 85780, 85781}}, }}},								//Sobor: level 4
			{WorldSno.trdun_leoric_level01,      new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._a1dun_crypts_leoric_crown_holder, new List<int>{84556}},
																					  {ActorSno._trdun_lecturn__leorics_journal, new List<int>{156458}},
																					  {ActorSno._a1dun_crypts_leoric_crown_holder_nocrown, new List<int>{85714}}, }}},											//Hall agony: level 1
			{WorldSno.trdun_leoric_level02,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._a1dun_crypts_leoric_crown_holder, new List<int>{85724}},
																					  {ActorSno._trdun_lecturn__leorics_journal, new List<int>{156460}},
																					  {ActorSno._a1dun_crypts_leoric_crown_holder_nocrown, new List<int>{85719}}, }}},											//Hall agony: level 2
			{WorldSno.trdun_leoric_level03,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._a1dun_crypts_leoric_crown_holder, new List<int>{85729}}, }}},											//Hall agony: level 3
			{WorldSno.a1dun_spidercave_01,    new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_spidercaves, new List<int>{154405, 156462, 156453, 156455}}, }}},					//Сave Arana
			{WorldSno.trdun_jail_level01,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._a1dun_jail_ghost_queen_lore, new List<int>{107268}}, }}},											//Damn Fort
			{WorldSno.a1dun_leor_manor,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._loottype2_tristramvillager_male_c_corpse_01, new List<int>{144181}}, }}},											//Leoric Manor
			{WorldSno.a1_cave_highlands_goatcavea_level01,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_uriksjournal, new List<int>{166878}}, }}},											//Сave clan moon: level 1	
			{WorldSno.a1_cave_highlands_goatcavea_level02,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_uriksjournal, new List<int>{166896, 166898}}, }}},									//Сave clan moon: level 2	
			{WorldSno.trdun_cave_nephalem_03,     new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._drownedtemple_chest, new List<int>{119733}}, }}},											//Sunken temple
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act II/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.caout_town, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{
				{ActorSno._lorechest_guardcaptainjournal, new List<int>{170224}},
				{ActorSno._lore_lordsofhell, new List<int>{189736, 189764, 189779, 189781, 189783, 189785}},
				{ActorSno._lorechest_loveletter, new List<int>{171460, 169932}},
				{ActorSno._lorechest_onelastentry, new List<int>{170257}},
				{ActorSno._lore_belial_guardsorders, new List<int>{156468}},
				{ActorSno._lore_belialmaghdamissive2, new List<int>{156474}},
				{ActorSno._lore_belial_oasis, new List<int>{156476}},
				{ActorSno._lore_belial_boneyards, new List<int>{156465}},
				{ActorSno._caout_stingingwinds_chest_cultistcamp, new List<int>{191717}},
				{ActorSno._lore_huntersjournal3chest, new List<int>{148686}},
				{ActorSno._lore_waterpuzzle_satchel, new List<int>{183610}},
				{ActorSno._portalroulette_satchel_chest, new List<int>{178770}},
				{ActorSno._lore_desolatesands, new List<int>{189677, 189679, 189687}}, }}},																							//Caldeum
			{WorldSno.a2dun_cave_bloodvial_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_kullejournal2chest, new List<int>{148806}}, }}},											//Пещера предателя: уровень 1
			{WorldSno.a2dun_zolt_blood02, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_kullejournal1chest, new List<int>{148800}}, }}},											//Гробница наемника
			{WorldSno.a2dun_cave_bloodvial_02, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_kullejournal3chest, new List<int>{148812}}, }}},											//Пещера предателя: уровень 2
			{WorldSno.caout_refugeecamp, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_a2_leahjournal1, new List<int>{189698, 189700, 189702, 189704, 189715}}, }}},			//Тайный лагерь
			{WorldSno.caout_hub_inn, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_caldeumhistory, new List<int>{189657, 189660, 189664, 189666, 189669, 189671}}, }}},	//Таверна "Жгучие пески"
			{WorldSno.caout_interior_f, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_lordsofhell, new List<int>{189789}}, }}},												//Тайный алтарь
			{WorldSno.a2dun_swr_swr_to_oasis_level01,    new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_lordsofhell, new List<int>{189806}}, }}},											//Разрушенный резервуар
			{WorldSno.a2c2dun_zolt_treasurehunter,  new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_poltahrjournal, new List<int>{189826}}, }}},												//Зал Утерянного Идола
			{WorldSno.a2dun_cald_uprising,    new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lorechest_secretmissive, new List<int>{170007}}, }}},										//Caldeum(Uprising)
			{WorldSno.a2dun_cald, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lorechest_servantsdiary, new List<int>{170064}},
																				  {ActorSno._lore_belial_imperialguard, new List<int>{156470}},}}},											//Caldeum bazaar
			{WorldSno.caout_interior_d, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_belialmaghdamissive1, new List<int>{156472}}, }}},												//Сокрытый конклав
			{WorldSno.a2c1dun_swr_caldeum_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_belial_sewers, new List<int>{156478}}, }}},												//Калдейские стоки
			{WorldSno.a2dun_zolt_head_random01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_huntersjournal1chest, new List<int>{148672}},
																				  {ActorSno._lore_huntersjournal45chest, new List<int>{148707}},}}},												//Заброшенные руины
			{WorldSno.a2dun_aqd_special_a, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_huntersjournal2chest, new List<int>{148680}},
																				  {ActorSno._lore_aqueducts, new List<int>{189652}},}}},												//Западный канал
			{WorldSno.a2dun_zolt_level02, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_huntersjournal45chest, new List<int>{148693}}, }}},												//Залы Бурь
			{WorldSno.a2dun_zolt_level01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_huntersjournal45chest, new List<int>{148701}}, }}},												//Таинственная бездна
			{WorldSno.a2dun_aqd_special_b, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_aqueducts, new List<int>{189654}}, }}},												//Восточный канал
			



			//////////////////////////////////////////////////////////////////////////////////////////////////Act III/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.a3dun_keep_level03, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._bastionskeepguard_corpse_jonathan_l, new List<int>{204678}},
																				  {ActorSno._lore_azmodanchest3, new List<int>{204820}},
																				  {ActorSno._lore_fallofthebarbs, new List<int>{204828, 204831, 204839, 204846, 204848}},}}},				//Нижние этажи крепости: уровень 1
			{WorldSno.gluttony_boss, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_gluttonyslog_corpse, new List<int>{204850}}, }}},											//Кладовая
			{WorldSno.a3dun_hub_keep, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lorechest_hailesjournal, new List<int>{178349}}, }}},											//Главная башня бастиона
			{WorldSno.a3dun_keep_hub_inn, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_a3_leahjournal, new List<int>{204853, 204859, 204875, 204878}}, }}},					//Оружейная
			{WorldSno.a3dun_rmpt_level01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_satchel_morgan, new List<int>{190879, 191086, 191133}},
																				  {ActorSno._lore_azmodanchest1, new List<int>{204033}}, }}},												//Заоблачные стены
			{WorldSno.a3dun_rmpt_level02, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_azmodanchest2, new List<int>{204817}}, }}},												//Каменный форт
			{WorldSno.a3_battlefields_02, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_azmodanchest1, new List<int>{204822}},
																				  {ActorSno._lore_azmodanchest2, new List<int>{204824}},}}},												//Поля Кровавой Бойни
			{WorldSno.a3dun_crater_level_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lore_azmodanchest3, new List<int>{204826}}, }}},												//Арреатский кратер: уровень 1
			{WorldSno.a3dun_keep_random_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lorechest_keephistory, new List<int>{178367}}, }}},											//Укрепленный бункер: уровень 1
			
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act IV/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{WorldSno.a4dun_heaven_hub_keep, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ { ActorSno._lorechest_a4_hub_oldcouple_marta, new List<int>{166775}}, }}},											//Главная башня бастиона
			{WorldSno.a4dun_garden_of_hope_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_angiriscouncil_angel, new List<int>{193560, 193586, 193568}}, }}},							//Сады надежды: уровень 1
			{WorldSno.a4dun_garden_of_hope_random, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_angiriscouncil_angel, new List<int>{193574, 193580, 193592}}, }}},							//Сады надежды: уровень 2
			{WorldSno.a4dun_spire_level_01, new LoreLayout{ chests_lore = new Dictionary<ActorSno, List<int>>{ {ActorSno._lore_nephalem, new List<int>{211609, 211611, 211613}},
																				   {ActorSno._lore_inarius, new List<int>{211599, 211601, 211603, 211605}},}}},						//Серебрянный шпиль: уровень 1
			
		};
	}
}
