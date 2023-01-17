//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		ActorSno._x1_armorscavenger_asteroidrain, //Sartor
		ActorSno._x1_monstrosity_scorpionbug_a_gardenevent1, //Balata	
		ActorSno._x1_bog_hillbilly_evil, //Lurk	
		ActorSno._x1_shield_skeleton_westmarch_fireambush_captain, //Trejiak
		ActorSno._sandshark_b_sewersharkevent, //Moontooth Dreadshark	
		ActorSno._ghost_d_ghosthuntersevent, //Raziel	
		ActorSno._fastmummy_a_shadeofradament, //Flesh of Nar Gulle	
		ActorSno._rockworm_stationary_queenworm, //Shaitan the Broodmother	
		ActorSno._sandmonster_a_portalroulette, //Ernutet	
		ActorSno._fallenshaman_b_water_money, //Mundunogo	
		ActorSno._dunedervish_a_dyingmanmine, //Dervish Lord
		ActorSno._rockworm_stationary_kingworm, //Graveljaw the Devourer	
		ActorSno._triunevesselactivated_b_corpseeaterevent, //Hurax
		ActorSno._triunecultist_c_tortureleader, //Cultist Grand Inquisitor
		ActorSno._triunesummoner_b_rabbitholeevent, //Cadhul the Deathcaller
		ActorSno._fleshpitflyer_a_unique_02, //Firestarter
		ActorSno._gravedigger_b, //Dataminer
		ActorSno._skeleton_a_cain_unique, //Cain Intro Skeleton
		ActorSno._zombiefemale_a_blacksmitha,  //Mira Imon
		ActorSno._ghost_a_unique_chancellor, //Imon advisor
		ActorSno._triunesummoner_a_unique_swordofjustice, //prophet Urik
		ActorSno._graverobber_c_nigel, //graverobber Nigel
		ActorSno._townattack_summoner_unique, //Wortham cultist leader
		ActorSno._snakeman_caster_a_adriatorturer, //a2_swr_Adria snakeman
		ActorSno._x1_bigred_chronodemon_burned_ramguard, //Thilor
		ActorSno._x1_westmarchbrute_batteringramboss //Mordrath
	)]
	public class Unique : Monster
	{
		public bool CanDropKey = false;

		public Unique(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.Hitpoints_Max] *= 6.0f;
			//this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max];
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			//this.Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 3.5f;
			//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] *= 3.5f;

			MonsterAffixGenerator.Generate(this, Math.Min(this.World.Game.Difficulty + 1, 5));
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Unique;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}
	}
}
