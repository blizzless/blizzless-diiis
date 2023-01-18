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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Monsters
{
	#region Spore
	[HandledSNO(ActorSno._spore)]
	public class Spore : Monster
	{
		public Spore(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30525);//SporeCloud.pow
		}
	}
	#endregion
	#region QuillDemon
	[HandledSNO(
		ActorSno._quilldemon_a,
		ActorSno._quilldemon_b,
		ActorSno._quilldemon_d,
		ActorSno._quilldemon_c,
		ActorSno._quilldemon_a_unique_loothoarderleader,
		ActorSno._quilldemon_a_loothoarder,
		ActorSno._quilldemon_c_unique_01,
		ActorSno._quilldemon_a_baby_event
	)]
	public class QuillDemon : Monster
	{
		public QuillDemon(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(107729);
		}
	}
	#endregion
	#region DarkCultists
	[HandledSNO(
		ActorSno._triunecultist_a,
		ActorSno._triunecultist_b,
		ActorSno._triunecultist_c,
		ActorSno._triunecultist_d
	)]
	public class DarkCultists : Monster
	{
		public DarkCultists(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(ActorSno._townattackcultistmelee)]
	public class DarkCultistsTownAttackMelee : Monster
	{
		public DarkCultistsTownAttackMelee(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(
		ActorSno._triunesummoner_a,
		ActorSno._triunesummoner_b,
		ActorSno._triunesummoner_c,
		ActorSno._triunesummoner_d
	)]
	public class DarkSummoner : Monster
	{
		public DarkSummoner(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30570);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 10f;
			this.WalkSpeed = 0.15f;
		}
	}
	[HandledSNO(ActorSno._townattack_summoner)]
	public class DarkCultistSummnoer : Monster
	{
		public DarkCultistSummnoer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{

			(Brain as MonsterBrain).AddPresetPower(30547);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.15f;
		}
	}
	[HandledSNO(ActorSno._townattack_cultist)]
	public class DarkCultistSummnoerTownAttack : Monster
	{
		public DarkCultistSummnoerTownAttack(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30570);
			(Brain as MonsterBrain).AddPresetPower(30547);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.15f;
		}
	}
    #endregion
    #region SandsShark
    [HandledSNO(ActorSno._sandshark_a)] //SandShark_A
	public class Shark : Monster
	{
		public Shark(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Lacuni
	[HandledSNO(
		ActorSno._lacunimale_a,
		ActorSno._lacunimale_b,
		ActorSno._lacunimale_c,
		ActorSno._lacunifemale_a,
		ActorSno._lacunifemale_b
	)]
	public class Lacuni : Monster
	{
		public Lacuni(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Fallens
	[HandledSNO(
		ActorSno._fallenchampion_a,
		ActorSno._fallenchampion_b,
		ActorSno._fallenchampion_c,
		ActorSno._fallenchampion_d
	)]
	public class Fallens : Monster
	{
		public Fallens(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region ACT V
	#region BodyPile
	[HandledSNO(ActorSno._x1_westm_alley_bodypile_a_sp)]
	public class BodyPile : Monster
	{
		public BodyPile(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//(Brain as MonsterBrain).PresetPowers.Clear();
			//(Brain as MonsterBrain).AddPresetPower(117580);
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;

			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.Spawner = true;
		}

	}
	#endregion
	[HandledSNO(
		ActorSno._x1_malthael_spirit,
		ActorSno._shadowvermin_b,
		ActorSno._x1_shadowvermin_a,
		ActorSno._shadowvermin_c,
		ActorSno._p6_shadowvermin,
		ActorSno._shadowvermin_a_1000monsterfight,
		ActorSno._x1_fortress_judgeevent_shadowvermin,
		ActorSno._shadowvermin_c_spire,
		ActorSno._shadowvermin_a
	)]
	public class Malthael_Spirit : Monster
	{
		public Malthael_Spirit(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.2f;
		}
	}
	//273417
	[HandledSNO(
		ActorSno._x1_deathmaiden_a,
		ActorSno._x1_deathmaiden_unique_a,
		ActorSno._x1_deathmaiden_unique_b,
		ActorSno._x1_deathmaiden_unique_c,
		ActorSno._x1_deathmaiden_unique_heaven
	)]
	public class DeathMaiden : Monster
	{
		public DeathMaiden(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	//282789
	[HandledSNO(
		ActorSno._x1_skeletonarcher_westmarch_a,
		ActorSno._x1_skeletonarcher_westmarch_unique_a,
		ActorSno._x1_skeletonarcher_westmarch_ghost_a
	)]
	public class SkeletonArcher_Westmarch : Monster
	{
		public SkeletonArcher_Westmarch(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.24f;
			//this.WalkSpeed /= 2f;
		}
		
	}
	//276309
	[HandledSNO(
		ActorSno._x1_skeleton_westmarch_a,
		ActorSno._x1_ghostguard_02_a
		//, 282027
	)]
	public class Skeleton_Westmarch : Monster
	{
		public Skeleton_Westmarch(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.25f;
		}
	}
	#endregion
	#region Ghost
	[HandledSNO(
		ActorSno._ghost_a,
		ActorSno._ghost_a_norun,
		ActorSno._ghost_b,
		ActorSno._ghost_c,
		ActorSno._ghost_d
	)]
	public class EnragedPhantom : Monster
	{
		public EnragedPhantom(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			
		}
	}
	[HandledSNO(ActorSno._x1_ghost_dark_introoverlook, ActorSno._x1_ghost_dark_a)]
	public class DarkGhost : Monster
	{
		public DarkGhost(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.25f;
			
		}
	}
	#endregion
	#region Unburieds
	[HandledSNO(
		ActorSno._unburied_a,
		ActorSno._unburied_b,
		ActorSno._unburied_c,
		ActorSno._unburied_d
	)]
	public class Unburied : Monster
	{
		public Unburied(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region WoodWraiths
	// Work
	//[435848] P4_WoodWraith_A_Forest_Event_02 (Monster )
	//[470241] LS_WoodWraith (Monster )
	//[430928] P4_WoodWraith_A (Monster )
	//
	[HandledSNO(
		ActorSno._woodwraith_a_01,
		ActorSno._woodwraith_a_02,
		ActorSno._woodwraith_a_03
	)]
	public class WoodWraith : Monster
	{
		public WoodWraith(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.SetVisible(false);
		}
	}
	[HandledSNO(
		ActorSno._woodwraith_b_01,
		ActorSno._woodwraith_b_02,
		ActorSno._woodwraith_b_03
	)]
	public class HighLandWalker : Monster
	{
		public HighLandWalker(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.SetVisible(false);
		}
	}
	[HandledSNO(ActorSno._woodwraith_unique_a)]
	public class TheOldMan : Monster
	{
		public TheOldMan(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			// Summon Spores
		}
	}
	#endregion
	#region Zombies
	[HandledSNO(ActorSno._zombie_a)]
	public class WalkingCorpse : Monster
	{
		public WalkingCorpse(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombie_b)]
	public class HungryCorpse : Monster
	{
		public HungryCorpse(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombie_c)]
	public class BloatedCorpse : Monster
	{
		public BloatedCorpse(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombie_e)]
	public class RancidStumbler : Monster
	{
		public RancidStumbler(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombieskinny_a)] //ZombieSkinny
	public class Risen : Monster
	{
		public Risen(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombieskinny_b)] //ZombieSkinny
	public class RavenousDead : Monster
	{
		public RavenousDead(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombieskinny_c)] //ZombieSkinny
	public class VoraciousZombie : Monster
	{
		public VoraciousZombie(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombieskinny_d)] //ZombieSkinny
	public class Decayer : Monster
	{
		public Decayer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	//Risen
	[HandledSNO(ActorSno._zombieskinny_custom_a)] //ZombieSkinny_Custom_A.acr
	public class ZombieSkinny : Monster
	{
		public ZombieSkinny(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombiecrawler_barricade_a)] //ZombieCrawler_Barricade_A.acr
	public class CrowlingTorso : Monster
	{
		public CrowlingTorso(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._zombieskinny_a_leahinn)] //ZombieSkinny_A_LeahInn.acr
	public class LeahInnZombie : Monster
	{
		public LeahInnZombie(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
    #endregion
    #region Skeleton
    [HandledSNO(ActorSno._skeleton_a)]
    public class Skeleton : Monster
    {
        public Skeleton(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
        }
    }
    [HandledSNO(ActorSno._skeleton_a_cain)]
	public class RoyalHanchman : Monster
	{
		public RoyalHanchman(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._skeleton_b)]
	public class Returned : Monster
	{
		public Returned(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._skeleton_d)]
	public class SkeletalWarrior : Monster
	{
		public SkeletalWarrior(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._skeleton_cain)]
	public class SkeletonKnee : Monster
	{
		public SkeletonKnee(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//30474
		}
	}
	[HandledSNO(ActorSno._skeleton_twohander_a)]
	public class SkeletalExecutioner : Monster
	{
		public SkeletalExecutioner(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._skeleton_twohander_b)]
	public class ReturnedExecutioner : Monster
	{
		public ReturnedExecutioner(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region Skeleton_Necromantic_Minion
	//Necromantic Minion
	[HandledSNO(ActorSno._skeleton_a_templarintro_nowander)]
	public class NecromanticMinion : Monster
	{
		public NecromanticMinion(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region TriuneCultists
	[HandledSNO(ActorSno._triunecultist_c_event)]
	public class TriuneCultist : Monster
	{
		public TriuneCultist(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Skeleton_Summoner
	//No Uniques Added
	// Tomb Guardian -> All
	[HandledSNO(
		ActorSno._skeletonsummoner_a,
		ActorSno._skeletonsummoner_b,
		ActorSno._skeletonsummoner_c,
		ActorSno._skeletonsummoner_d
	)]
	public class TombGuardian : Monster
	{
		public TombGuardian(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			(Brain as MonsterBrain).AddPresetPower(30503);
			(Brain as MonsterBrain).AddPresetPower(30543); //Summon Skeletons
		}
	}
	#endregion
	#region Skeleton_Archer
	[HandledSNO(ActorSno._skeletonarcher_a)]
	public class SkeletalArcher : Monster
	{
		public SkeletalArcher(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region Shield_Skeleton
	[HandledSNO(ActorSno._shield_skeleton_a)]
	public class SkeletalShieldBearer : Monster
	{
		public SkeletalShieldBearer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._shield_skeleton_b)]
	public class ReturnedShieldMan : Monster
	{
		public ReturnedShieldMan(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._shield_skeleton_c)]
	public class SkeletalSentry : Monster
	{
		public SkeletalSentry(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region Grotesque
	[HandledSNO(
		ActorSno._corpulent_a,
		ActorSno._corpulent_b,
		ActorSno._corpulent_c,
		ActorSno._corpulent_d
	)]
	public class Corpulent : Monster
	{
		//3851 suicide blood, 220536 suicide imps = these happen on different SNOs and happen as they are dying.

		public Corpulent(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30178); //Explode
		}
	}
	[HandledSNO(ActorSno._lamprey_a)]
	public class CorpseWorm : Monster
	{

		public CorpseWorm(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}

	#endregion
	#region FleshPitFlyers
	[HandledSNO(ActorSno._fleshpitflyer_a)]
	public class CarrionBat : Monster
	{
		public CarrionBat(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._fleshpitflyer_b_event_ambusher)]
	public class PlagueCarrier : Monster
	{
		public PlagueCarrier(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._fleshpitflyer_a_unique_01)]
	public class Glidewing : Monster
	{
		public Glidewing(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	[HandledSNO(ActorSno._fleshpitflyer_e)]
	public class VileHellbat : Monster
	{
		public VileHellbat(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}
	}
	#endregion
	#region CarrionNest-FleshPitFlyerSpawner
	[HandledSNO(
		ActorSno._fleshpitflyerspawner_a,
		ActorSno._fleshpitflyerspawner_b,
		ActorSno._fleshpitflyerspawner_c,
		ActorSno._fleshpitflyerspawner_d,
		ActorSno._fleshpitflyer_b,
		ActorSno._fleshpitflyerspawner_b_event_farmambush,
		ActorSno._fleshpitflyerspawner_e_gardens,
		ActorSno._x1_spawner_fleshpitflyerspawner_b,
		ActorSno._x1_spawner_fleshpitflyerspawner_gardens
	)]
	public class CarrionNest : Monster
	{
		public CarrionNest(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).PresetPowers.Clear();
			(Brain as MonsterBrain).AddPresetPower(117580); 
			this.Field2 = 0x8;
			this.CollFlags = 0;
			this.WalkSpeed = 0;

			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.Spawner = true;
		}

	}

	#endregion
	#region Columns
	[HandledSNO(ActorSno._trdun_crypt_pillar_spawner)]
	public class CryptColumn : Monster
	{
		public CryptColumn(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).PresetPowers.Clear();
			(Brain as MonsterBrain).AddPresetPower(117580);

			this.Attributes[GameAttribute.Movement_Scalar] = 0f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = 0f;
			this.WalkSpeed = 0f;
			this.Spawner = true;
		}

	}
	#endregion
	#region Wretched Mothers
	[HandledSNO(ActorSno._zombiefemale_a_tristramquest_unique, ActorSno._zombiefemale_a_tristramquest)] // ZombieFemale_A_TristramQuest_Unique.acr
	public class WretchedMother : Monster
	{
		public WretchedMother(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592); // Only distance attack
			(Brain as MonsterBrain).AddPresetPower(94734);
			(Brain as MonsterBrain).AddPresetPower(110518);
			//this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 4f;
			//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 4f;
		}
	}
	#endregion
	#region GoatmanShaman
	[HandledSNO(
		ActorSno._goatman_melee_a,
		ActorSno._goatman_melee_b,
		ActorSno._goatman_melee_c,
		ActorSno._goatman_melee_d
	)] // Goatman_Melee_A, Goatman_Melee_B, Goatman_Melee_C, Goatman_Melee_D
	public class Goatman_Moonclan_Melee : Monster
	{
		public Goatman_Moonclan_Melee(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Brain = new MonsterBrain(this);

			(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			this.WalkSpeed = 0.2f;
		}
	}
	[HandledSNO(ActorSno._goatman_melee_a_unique_01)] // [218428] Goatman_Melee_A_Unique_01
	public class Goatman_Moonclan_Melee_Unique1 : Monster
	{
		public Goatman_Moonclan_Melee_Unique1(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Brain = new MonsterBrain(this);

			(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			this.WalkSpeed = 0.2f;
		}
	}
	//218428
	[HandledSNO(ActorSno._goatman_ranged_a, ActorSno._goatman_ranged_b)] // Goatman_Ranged_A, Goatman_Ranged_B
	public class Goatman_Moonclan_Ranged : Monster
	{
		public Goatman_Moonclan_Ranged(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Brain = new MonsterBrain(this);

			//(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			(Brain as MonsterBrain).AddPresetPower(30252); //Range_instant
			this.WalkSpeed = 0.2f;
		}
	}
	[HandledSNO(
		ActorSno._goatman_shaman_b,
		ActorSno._goatman_shaman_a,
		ActorSno._goatman_shaman_a_event_gharbad_the_weak
	)]
	public class GoatmanShaman : Monster
	{
		public GoatmanShaman(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592); // Only distance attack
			(Brain as MonsterBrain).AddPresetPower(77342);
			(Brain as MonsterBrain).AddPresetPower(99077);
			this.WalkSpeed = 0.2f;
		}
	}

	#endregion
	#region GoatmanMelee
	[HandledSNO(ActorSno._goatman_melee_b_event_gharbad_the_weak)]
	public class GoatmanMelee : Monster
	{
		public GoatmanMelee(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.2f;
		}
	}
	#endregion
	#region GoatmanRanged
	[HandledSNO(ActorSno._goatman_ranged_b_event_gharbad_the_weak)]
	public class GoatmanRanged : Monster
	{
		public GoatmanRanged(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592); // Only distance attack
			(Brain as MonsterBrain).AddPresetPower(30252);
		}
	}
	#endregion
	#region WitherMoth
	[HandledSNO(ActorSno._withermoth_a)]
	public class WitherMoth : Monster
	{
		public WitherMoth(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(99077);
		}
	}
	#endregion
	#region TriuneWizard
	[HandledSNO(ActorSno._triunewizard)]
	public class TriuneWizard : Monster
	{
		public TriuneWizard(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.35f;
			(Brain as MonsterBrain).AddPresetPower(99077);
		}
	}
	#endregion
	#region TriuneBerserker
	[HandledSNO(
		ActorSno._triune_berserker_a,
		ActorSno._triune_berserker_b,
		ActorSno._triune_berserker_c,
		ActorSno._triune_berserker_d,
		ActorSno._triune_berserker_maghdapet
	)]
	public class TriuneBerserker : Monster
	{
		public TriuneBerserker(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.35f;
			(Brain as MonsterBrain).AddPresetPower(99077);
		}
	}
	#endregion
	#region Overseer
	[HandledSNO(ActorSno._gravedigger_warden)]
	public class Overseer : Unique
	{
		public Overseer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.1f;
		}
	}
	#endregion
	#region AzmodanProxy
	[HandledSNO(ActorSno._azmodan_mouth, ActorSno._keep_spy)]
	public class AzmodanProxy : Unique
	{
		public AzmodanProxy(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.1f;
			this.SetVisible(false);
		}
	}
	#endregion
	#region SkeletonMages
	[HandledSNO(ActorSno._skeletonmage_fire_a, ActorSno._skeletonmage_fire_b)]
	public class SkeletonMageFire : Monster
	{
		public SkeletonMageFire(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30499);
		}
	}
	[HandledSNO(ActorSno._skeletonmage_cold_a, ActorSno._skeletonmage_cold_b)]
	public class SkeletonMageCold : Monster
	{
		public SkeletonMageCold(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30497);
		}
	}
	[HandledSNO(ActorSno._skeletonmage_lightning_a, ActorSno._skeletonmage_lightning_b)]
	public class SkeletonMageLightning : Monster
	{
		public SkeletonMageLightning(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30500);
		}
	}
	[HandledSNO(ActorSno._skeletonmage_poison_a, ActorSno._skeletonmage_poison_b)]
	public class SkeletonMagePoison : Monster
	{
		public SkeletonMagePoison(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30502);
			this.WalkSpeed = 0.8f;
		}
	}
	#endregion
	#region A2Snakeman
	[HandledSNO(
		ActorSno._snakeman_melee_a,
		ActorSno._snakeman_melee_b,
		ActorSno._snakeman_melee_c,
		ActorSno._snakeman_melee_a_adriarescue,
		ActorSno._snakeman_melee_a_escapefromcaldeum,
		ActorSno._snakeman_melee_a_unique_01,
		ActorSno._snakeman_melee_belial,
		ActorSno._snakeman_melee_b_unique_01
	)]
	public class SnakemanMelee : Monster
	{
		public SnakemanMelee(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(
		ActorSno._snakeman_caster_a,
		ActorSno._snakeman_caster_b,
		ActorSno._snakeman_caster_c,
		ActorSno._snakeman_caster_a_spawner_escapefromcaldeum,
		ActorSno._snakeman_caster_a_unique_01,
		ActorSno._snakeman_caster_belial,
		ActorSno._snakeman_caster_b_unique_01,
		ActorSno._snakeman_caster_b_unique_02
	)]
	public class SnakemanCaster : Monster
	{
		public SnakemanCaster(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(
		ActorSno._fastmummy_a,
		ActorSno._fastmummy_b,
		ActorSno._fastmummy_c,
		ActorSno._fastmummy_b_facepuzzleunique,
		ActorSno._fastmummy_b_fastmummyambush,
		ActorSno._fastmummy_b_unique_01,
		ActorSno._fastmummy_b_unique_02,
		ActorSno._fastmummy_c_unique,
		ActorSno._fastmummy_c_unique_01
	)]
	public class FastMummy : Monster
	{
		public FastMummy(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}

	#endregion
	#region A2FallenShaman
	[HandledSNO(
		ActorSno._fallenshaman_a,
		ActorSno._fallenshaman_b,
		ActorSno._fallenshaman_c,
		ActorSno._fallenshaman_a_zoltlev
	)]
	public class FallenShaman : Monster
	{
		public FallenShaman(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30252);
			this.WalkSpeed = 0.4f;
		}
	}
	#endregion
	#region A3DemonFlyer
	[HandledSNO(
		ActorSno._demonflyer_a,
		ActorSno._demonflyer_b_noflee,
		ActorSno._demonflyer_c,
		ActorSno._demonflyer_a_bomber,
		ActorSno._demonflyer_a_swoop,
		ActorSno._demonflyer_b,
		ActorSno._demonflyermega_a
	)]
	public class DemonFlyer : Monster
	{
		public DemonFlyer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30334);
			(Brain as MonsterBrain).AddPresetPower(130798);
		}
	}
	#endregion
	#region A3Succubus
	[HandledSNO(
		ActorSno._succubus_daughterofpain,
		ActorSno._succubus_b,
		ActorSno._succubus_a
	)]
	public class Succubus : Monster
	{
		public Succubus(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(120874);
		}
	}
	#endregion
	#region A2SandWasp
	[HandledSNO(
		ActorSno._sandwasp_a,
		ActorSno._sandwasp_b,
		ActorSno._sandwasp_c,
		ActorSno._sandwasp_d
	)]
	public class SandWasp : Monster
	{
		public SandWasp(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30334);
			(Brain as MonsterBrain).AddPresetPower(30449);
		}
	}
	#endregion
	#region A4HoodedNightmare
	[HandledSNO(ActorSno._hoodednightmare_a)]
	public class HoodedNightmare : Monster
	{
		public HoodedNightmare(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(135412);
		}
	}
	#endregion
	#region GoatMutants
	[HandledSNO(ActorSno._goatmutant_shaman_a, ActorSno._goatmutant_shaman_b)]
	public class GoatMutantShaman : Monster
	{
		public GoatMutantShaman(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30952);
			(Brain as MonsterBrain).AddPresetPower(157947);
		}
	}
	[HandledSNO(ActorSno._goatmutant_ranged_a, ActorSno._goatmutant_ranged_b)]
	public class GoatMutantRanged : Monster
	{
		public GoatMutantRanged(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30952);
			(Brain as MonsterBrain).AddPresetPower(159004);
		}
	}
	#endregion
	#region FallenLunatic
	[HandledSNO(
		ActorSno._fallenlunatic_a,
		ActorSno._fallenlunatic_b,
		ActorSno._fallenlunatic_c,
		ActorSno._fallenlunatic_d
	)]
	public class FallenLunatic : Monster
	{
		public FallenLunatic(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(66547);
			this.WalkSpeed = 0.4f;
		}
	}
	#endregion
	#region Wraith
	[HandledSNO(
		ActorSno._x1_wraith_a,
		ActorSno._x1_wraith_a_dark,
		ActorSno._x1_wraith_unique_a,
		ActorSno._x1_wraith_unique_b,
		ActorSno._p6_x1_wraith_unique_a_unique_rof_v3_01,
		ActorSno._x1_wraith_a_fortressunique
	)]
	public class Wrath : Monster
	{
		public Wrath(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Spiders
	[HandledSNO(ActorSno._spider_elemental_cold_tesla_a)] //Spider_Elemental_Cold_tesla_A
	public class Spider_Elemental_Cold_tesla_A : Monster
	{
		public Spider_Elemental_Cold_tesla_A(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
}
