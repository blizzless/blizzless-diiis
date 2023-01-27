using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		ActorSno._skeletonking,       //Leoric King
		ActorSno._spiderqueen,      //Aranea
		ActorSno._butcher,       //Butcher
		ActorSno._maghda,       //Maghda
		ActorSno._zoltunkulle,      //Zoltun Kulle
		ActorSno._belial_trueform,      //Belial (small)
		ActorSno._belial,       //Belial (big)
		ActorSno._gluttony,      //Gluttony
		ActorSno._siegebreakerdemon,      //Siegebreaker
		ActorSno._mistressofpain,      //Cydaea
		ActorSno._azmodan,      //Azmodan
		ActorSno._bigred_izual,     //Izual
		ActorSno._terrordemon_a_unique_1000monster,     //Iskatu
		ActorSno._despair,       //Despair (Rakanoth)
		ActorSno._diablo,     //Diablo
		ActorSno._terrordiablo,     //Diablo's shadow
		ActorSno._x1_urzael_boss,     //Urzael
		ActorSno._x1_adria_boss,     //Adria
		ActorSno._x1_malthael_boss,      //Malthael
		//Nephalem Bosses
		ActorSno._x1_lr_boss_mistressofpain, //X1_LR_Boss_MistressofPain 
		ActorSno._x1_lr_boss_angel_corrupt_a, //X1_LR_Boss_Angel_Corrupt_A 
		ActorSno._x1_lr_boss_creepmob_a, //X1_LR_Boss_creepMob_A 
		ActorSno._x1_lr_boss_skeletonsummoner_c, //X1_LR_Boss_SkeletonSummoner_C 
		ActorSno._x1_lr_boss_succubus_a, //X1_LR_Boss_Succubus_A 
		ActorSno._x1_lr_boss_snakeman_melee_belial, //X1_LR_Boss_Snakeman_Melee_Belial 
		ActorSno._x1_lr_boss_terrordemon_a, //X1_LR_Boss_TerrorDemon_A 
		ActorSno._p4_lr_boss_sandmonster_turret, //P4_LR_Boss_Sandmonster_Turret 
		ActorSno._x1_lr_boss_skeletonking, //x1_LR_Boss_SkeletonKing 
		ActorSno._x1_lr_boss_gluttony, //x1_LR_Boss_Gluttony 
		ActorSno._x1_lr_boss_despair, //x1_LR_Boss_Despair 
		ActorSno._x1_lr_boss_malletdemon, //x1_LR_Boss_MalletDemon 
		ActorSno._x1_lr_boss_morluspellcaster_ice, //X1_LR_Boss_morluSpellcaster_Ice 
		ActorSno._x1_lr_boss_sandmonster, //X1_LR_Boss_SandMonster 
		ActorSno._x1_lr_boss_morluspellcaster_fire, //X1_LR_Boss_morluSpellcaster_Fire 
		ActorSno._x1_lr_boss_deathmaiden, //X1_LR_Boss_DeathMaiden 
		ActorSno._x1_lr_boss_secret_cow, //X1_LR_Boss_Secret_Cow 
		ActorSno._x1_lr_boss_squigglet, //X1_LR_Boss_Squigglet 
		ActorSno._x1_lr_boss_sniperangel, //X1_LR_Boss_sniperAngel 
		ActorSno._x1_lr_boss_westmarchbrute, //X1_LR_Boss_westmarchBrute 
		ActorSno._x1_lr_boss_dark_angel, //X1_LR_Boss_Dark_Angel 
		ActorSno._x1_lr_boss_bigred_izual, //X1_LR_Boss_BigRed_Izual 
		ActorSno._x1_lr_boss_demonflyermega, //X1_LR_Boss_demonFlyerMega 
		ActorSno._x1_lr_boss_ratking_a, //X1_LR_Boss_RatKing_A 
		ActorSno._x1_lr_boss_ratking_a_ui, //X1_LR_Boss_RatKing_A_UI 
		ActorSno._x1_lr_boss_terrordemon_a_breathminion, //X1_LR_Boss_TerrorDemon_A_BreathMinion 
		ActorSno._x1_lr_boss_butcher, //x1_LR_Boss_Butcher 
		ActorSno._x1_lr_boss_zoltunkulle, //X1_LR_Boss_ZoltunKulle 
		ActorSno._x1_lr_boss_minion_shadowvermin_a, //X1_LR_Boss_Minion_shadowVermin_A 
		ActorSno._x1_lr_boss_minion_terrordemon_clone_c, //X1_LR_Boss_Minion_TerrorDemon_Clone_C 
		ActorSno._x1_lr_boss_minion_swarm_a, //X1_LR_Boss_Minion_Swarm_A 
		ActorSno._x1_lr_boss_minion_electriceel_b //X1_LR_Boss_Minion_electricEel_B 
	)/*Act Bosses*/]
	public sealed class Boss : Monster
	{
		public Boss(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			if (sno == ActorSno._zoltunkulle && world.SNO == WorldSno.a2dun_zolt_lobby) SetVisible(false);
			Attributes[GameAttribute.MinimapActive] = true;
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			Attributes[GameAttribute.using_Bossbar] = true;
			Attributes[GameAttribute.InBossEncounter] = true;
			Attributes[GameAttribute.Hitpoints_Max] *= 10.0f;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 7.8f;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] *= 7.8f;
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.TeamID] = 10;
			
			WalkSpeed *= 0.5f;
            MonsterBrain monsterBrain = (Brain as MonsterBrain);
            switch (sno)
			{
				case ActorSno._diablo: //Diablo
                             //(Brain as MonsterBrain).RemovePresetPower(30592);
                             //(Brain as MonsterBrain).AddPresetPower(136189); //[136189] Diablo_ClawRip
                    monsterBrain.AddPresetPower(136223); //Diablo_RingOfFire
                    monsterBrain.AddPresetPower(136226); //Diablo_HellSpikes
					;

					/*
						[199476] Diablo_StompAndStun
						[219598] Diablo_Teleport
						[167560] Diablo_LightningBreath_v2
						[185997] Diablo_ExpandingFireRing
						[169212] Diablo_Smash_Puny_Destructible
						[136828] Diablo_CurseOfAnguish
						[136829] Diablo_CurseOfPain
						[136830] Diablo_CurseOfHate
						[136831] Diablo_CurseOfDestruction
						
						[439719] Diablo_LightningBreath_LR_TerrorDemon_Clone
						[214831] Diablo_FireMeteor
						[161174] Diablo_CorruptionShield
						[136219] Diablo_LightningBreath
						[136223] Diablo_RingOfFire
						[136226] Diablo_HellSpikes
						
						[214668] Diablo_GetHit
						
						[136237] Diablo_ShadowVanish
						[136281] Diablo_ShadowClones
						[142582] Diablo_ShadowVanish_Charge
						[136849] Diablo_ShadowVanish_Grab
						
						[141865] Diablo_Phase1Buff
						[136850] Diablo_Phase2Buff
						[136852] Diablo_Phase3Buff
						[478072] Diablo_StompAndStunMB313
						
						[478410] Diablo_LightningBreath_Turret_MB313
						[195816] Diablo_Charge
						[428985] Diablo_LightningBreath_LR_TerrorDemon
						[376396] Uber_Gluttony_Gas_Cloud_Diablo
						[375473] Uber_SkeletonKing_Summon_Skeleton_Diablo
						[375493] Uber_Maghda_Summon_Beserker_Diablo
						[365978] Uber_Diablo_StompAndStun
						[375537] Uber_Despair_SummonMinion_Diablo
						[375929] UberDiablo_MirrorImage
						[376039] Uber_Despair_TeleportEnrage_Diablo
						[376043] Uber_ZoltunKulle_SlowTime_Diablo
						[376056] Uber_Despair_Volley_Diablo
						[375439] x1_Uber_Diablo_HellSpikes
						[375904] Diablo_LightningBreath_Uber
						[375905] Diablo_ClawRip_Uber
						[375907] Diablo_RingOfFire_Uber
						[375908] Diablo_ExpandingFireRing_Uber

						[453765] p43_d1_Diablo_ClawRip
												
						[328715] x1_Malthael_Diablo_AIState
						[334760] x1_Malthael_Diablo_TeleportFireNovaLightning
						
						
					*/
					break;
				case ActorSno._skeletonking://Leoric King
                    monsterBrain.RemovePresetPower(30592);
                    monsterBrain.AddPresetPower(30496);
                    monsterBrain.AddPresetPower(30504);
                    monsterBrain.AddPresetPower(73824);
                    monsterBrain.AddPresetPower(79334);
					break;
				case ActorSno._butcher://Butcher
                    monsterBrain.AddPresetPower(83008);
					break;
				case ActorSno._belial_trueform://Belial (small)
					HasLoot = false;
					break;
				case ActorSno._belial://Belial (big)
                    monsterBrain.AddPresetPower(152540);
					break;
				case ActorSno._maghda://Maghda
                    monsterBrain.AddPresetPower(131744); //summon berserker
                                                         //(Brain as MonsterBrain).AddPresetPower(131745); //mothDust
                    monsterBrain.AddPresetPower(131749); //teleport
					break;
				case ActorSno._gluttony://Gluttony
                    monsterBrain.AddPresetPower(93676); //gas cloud
                    monsterBrain.AddPresetPower(211292); //slime spawn
					break;
				default:
					break;
			}
		}

		public int AntiCCTriggerCount = 0;

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Boss;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}


		public override bool Reveal(PlayerSystem.Player player)
		{
			if (SNO == ActorSno._terrordemon_a_unique_1000monster)
			{
				Destroy();
				return false;
			}

			return base.Reveal(player);
		}
	}
}
