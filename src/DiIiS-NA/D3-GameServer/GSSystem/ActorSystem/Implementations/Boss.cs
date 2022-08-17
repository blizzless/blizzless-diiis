//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
	5350,       //Leoric King
	51341,      //Aranea
	3526,       //Butcher
	6031,       //Maghda
	80509,      //Zoltun Kulle
	62975,      //Belial (small)
	3349,       //Belial (big)
	87642,      //Gluttony
	96192,      //Siegebreaker
	95250,      //Cydaea
	89690,      //Azmodan
	148449,     //Izual
	196102,     //Iskatu
	4630,       //Despair (Rakanoth)
	114917,     //Diablo
	133562,     //Diablo's shadow
	291368,     //Urzael
	279394,     //Adria
	297730,      //Malthael
	//Nephalem Bosses
	358429, //X1_LR_Boss_MistressofPain 
	358489, //X1_LR_Boss_Angel_Corrupt_A 
	358614, //X1_LR_Boss_creepMob_A 
	359094, //X1_LR_Boss_SkeletonSummoner_C 
	359688, //X1_LR_Boss_Succubus_A 
	360281, //X1_LR_Boss_Snakeman_Melee_Belial 
	360636, //X1_LR_Boss_TerrorDemon_A 
	434201, //P4_LR_Boss_Sandmonster_Turret 
	343743, //x1_LR_Boss_SkeletonKing 
	343751, //x1_LR_Boss_Gluttony 
	343759, //x1_LR_Boss_Despair 
	343767, //x1_LR_Boss_MalletDemon 
	344119, //X1_LR_Boss_morluSpellcaster_Ice 
	344389, //X1_LR_Boss_SandMonster 
	345004, //X1_LR_Boss_morluSpellcaster_Fire 
	346563, //X1_LR_Boss_DeathMaiden 
	353517, //X1_LR_Boss_Secret_Cow 
	353535, //X1_LR_Boss_Squigglet 
	353823, //X1_LR_Boss_sniperAngel 
	353874, //X1_LR_Boss_westmarchBrute 
	354050, //X1_LR_Boss_Dark_Angel 
	354144, //X1_LR_Boss_BigRed_Izual 
	354652, //X1_LR_Boss_demonFlyerMega 
	426943, //X1_LR_Boss_RatKing_A 
	428323, //X1_LR_Boss_RatKing_A_UI 
	429010, //X1_LR_Boss_TerrorDemon_A_BreathMinion 
	357917, //x1_LR_Boss_Butcher 
	358208, //X1_LR_Boss_ZoltunKulle 
	360766, //X1_LR_Boss_Minion_shadowVermin_A 
	360794, //X1_LR_Boss_Minion_TerrorDemon_Clone_C 
	360327, //X1_LR_Boss_Minion_Swarm_A 
	360329 //X1_LR_Boss_Minion_electricEel_B 
	)/*Act Bosses*/]
	public sealed class Boss : Monster
	{
		public Boss(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			if (snoId == 80509 && world.WorldSNO.Id == 50613) this.SetVisible(false);
			this.Attributes[GameAttribute.MinimapActive] = true;
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			this.Attributes[GameAttribute.//Blizzless Project 2022 
using_Bossbar] = true;
			this.Attributes[GameAttribute.InBossEncounter] = true;
			this.Attributes[GameAttribute.Hitpoints_Max] *= 10.0f;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 7.8f;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] *= 7.8f;
			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this.Attributes[GameAttribute.TeamID] = 10;
			
			this.WalkSpeed *= 0.5f;
			switch (snoId)
			{
				case 114917: //Diablo
					//(Brain as MonsterBrain).RemovePresetPower(30592);
					//(Brain as MonsterBrain).AddPresetPower(136189); //[136189] Diablo_ClawRip
					(Brain as MonsterBrain).AddPresetPower(136223); //Diablo_RingOfFire
					(Brain as MonsterBrain).AddPresetPower(136226); //Diablo_HellSpikes
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
				case 5350://Leoric King
					(Brain as MonsterBrain).RemovePresetPower(30592);
					(Brain as MonsterBrain).AddPresetPower(30496);
					(Brain as MonsterBrain).AddPresetPower(30504);
					(Brain as MonsterBrain).AddPresetPower(73824);
					(Brain as MonsterBrain).AddPresetPower(79334);
					break;
				case 3526://Butcher
					(Brain as MonsterBrain).AddPresetPower(83008);
					break;
				case 62975://Belial (small)
					this.HasLoot = false;
					break;
				case 3349://Belial (big)
					(Brain as MonsterBrain).AddPresetPower(152540);
					break;
				case 6031://Maghda
					(Brain as MonsterBrain).AddPresetPower(131744); //summon berserker
																	//(Brain as MonsterBrain).AddPresetPower(131745); //mothDust
					(Brain as MonsterBrain).AddPresetPower(131749); //teleport
					break;
				case 87642://Gluttony
					(Brain as MonsterBrain).AddPresetPower(93676); //gas cloud
					(Brain as MonsterBrain).AddPresetPower(211292); //slime spawn
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
			if (this.ActorSNO.Id == 196102)
			{
				this.Destroy();
				return false;
			}

			return base.Reveal(player);
		}
	}
}
