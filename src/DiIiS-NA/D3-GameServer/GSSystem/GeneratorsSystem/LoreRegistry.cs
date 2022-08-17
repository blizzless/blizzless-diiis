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

namespace DiIiS_NA.GameServer.GSSystem.GeneratorsSystem
{
	public static class LoreRegistry
	{
		public struct LoreLayout
		{
			public Dictionary<int, List<int>> chests_lore;
		};

		public static readonly Dictionary<int, LoreLayout> Lore = new Dictionary<int, LoreLayout>
		{
			//////////////////////////////////////////////////////////////////////////////////////////////////Act I///////////////////////////////////////////////////////////////////////////////////////////  
			{71150,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{
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
			{109362,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {230232, new List<int>{211567}}, }}},											//New Tristram tavern
			{86856,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {86817, new List<int>{86639, 89489, 89519, 89520, 89521, 89522, 89523}}, }}},	//New Tristram Leah's room
			{130161,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {115124, new List<int>{115115, 167797}}, }}},									//Cain's house
			{50579,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {85790, new List<int>{85757, 85759}}, }}},									//Sobor: level 1
			{50584,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {85790, new List<int>{85779, 85780, 85781}}, }}},								//Sobor: level 4
			{2826,      new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{84556}},
																					  {5891, new List<int>{156458}},
																					  {170633, new List<int>{85714}}, }}},											//Hall agony: level 1
			{58982,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{85724}},
																					  {5891, new List<int>{156460}},
																					  {170633, new List<int>{85719}}, }}},											//Hall agony: level 2
			{58983,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {159446, new List<int>{85729}}, }}},											//Hall agony: level 3
			{180550,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {167350, new List<int>{154405, 156462, 156453, 156455}}, }}},					//Сave Arana
			{94676,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {105758, new List<int>{107268}}, }}},											//Damn Fort
			{75049,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {187436, new List<int>{144181}}, }}},											//Leoric Manor
			{82502,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {166661, new List<int>{166878}}, }}},											//Сave clan moon: level 1	
			{82511,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {166661, new List<int>{166896, 166898}}, }}},									//Сave clan moon: level 2	
			{60395,     new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {137189, new List<int>{119733}}, }}},											//Sunken temple
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act II/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{70885, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{
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
			{204628, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216316, new List<int>{148806}}, }}},											//Пещера предателя: уровень 1
			{123183, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216311, new List<int>{148800}}, }}},											//Гробница наемника
			{204674, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216768, new List<int>{148812}}, }}},											//Пещера предателя: уровень 2
			{161472, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192154, new List<int>{189698, 189700, 189702, 189704, 189715}}, }}},			//Тайный лагерь
			{174530, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {190014, new List<int>{189657, 189660, 189664, 189666, 189669, 189671}}, }}},	//Таверна "Жгучие пески"
			{51270, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {194145, new List<int>{189789}}, }}},												//Тайный алтарь
			{146619,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {194145, new List<int>{189806}}, }}},											//Разрушенный резервуар
			{2812,  new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192325, new List<int>{189826}}, }}},												//Зал Утерянного Идола
			{109894,    new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {169999, new List<int>{170007}}, }}},										//Caldeum(Uprising)
			{86594, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {170063, new List<int>{170064}},
																				  {145601, new List<int>{156470}},}}},											//Caldeum bazaar
			{50657, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {167090, new List<int>{156472}}, }}},												//Сокрытый конклав
			{50588, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {145609, new List<int>{156478}}, }}},												//Калдейские стоки
			{61631, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216022, new List<int>{148672}},
																				  {216805, new List<int>{148707}},}}},												//Заброшенные руины
			{62776, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216308, new List<int>{148680}},
																				  {189984, new List<int>{189652}},}}},												//Западный канал
			{50611, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216805, new List<int>{148693}}, }}},												//Залы Бурь
			{50610, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216805, new List<int>{148701}}, }}},												//Таинственная бездна
			{62779, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {189984, new List<int>{189654}}, }}},												//Восточный канал
			



			//////////////////////////////////////////////////////////////////////////////////////////////////Act III/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{93104, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {204724, new List<int>{204678}},
																				  {213447, new List<int>{204820}},
																				  {213470, new List<int>{204828, 204831, 204839, 204846, 204848}},}}},				//Нижние этажи крепости: уровень 1
			{103209, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {212704, new List<int>{204850}}, }}},											//Кладовая
			{172909, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {178357, new List<int>{178349}}, }}},											//Главная башня бастиона
			{182875, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {212222, new List<int>{204853, 204859, 204875, 204878}}, }}},					//Оружейная
			{81019, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {192778, new List<int>{190879, 191086, 191133}},
																				  {213445, new List<int>{204033}}, }}},												//Заоблачные стены
			{93099, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213446, new List<int>{204817}}, }}},												//Каменный форт
			{95804, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213445, new List<int>{204822}},
																				  {213446, new List<int>{204824}},}}},												//Поля Кровавой Бойни
			{81049, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {213447, new List<int>{204826}}, }}},												//Арреатский кратер: уровень 1
			{174516, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {178366, new List<int>{178367}}, }}},											//Укрепленный бункер: уровень 1
			
			
			
			//////////////////////////////////////////////////////////////////////////////////////////////////Act IV/////////////////////////////////////////////////////////////////////////////////////////// 
			
			
			{178152, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {177462, new List<int>{166775}}, }}},											//Главная башня бастиона
			{109513, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216482, new List<int>{193560, 193586, 193568}}, }}},							//Сады надежды: уровень 1
			{219659, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216482, new List<int>{193574, 193580, 193592}}, }}},							//Сады надежды: уровень 2
			{121579, new LoreLayout{ chests_lore = new Dictionary<int, List<int>>{ {216551, new List<int>{211609, 211611, 211613}},
																				   {216537, new List<int>{211599, 211601, 211603, 211605}},}}},						//Серебрянный шпиль: уровень 1
			
		};
	}
}
