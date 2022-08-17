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
	[HandledSNO(5482)]
	public class Spore : Monster
	{
		public Spore(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30525);//SporeCloud.pow
		}
	}
	#endregion
	#region QuillDemon
	[HandledSNO(4982, 4983, 4984, 4985, 201878, 187664, 220455, 128781)]
	public class QuillDemon : Monster
	{
		public QuillDemon(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(107729);
		}
	}
	#endregion
	#region DarkCultists
	[HandledSNO(6024,6028)]
	public class DarkCultists : Monster
	{
		public DarkCultists(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(6052)]
	public class BerserkMini : Monster
	{
		public BerserkMini(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(90008)]
	public class DarkCultistsTownAttackMelee : Monster
	{
		public DarkCultistsTownAttackMelee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(6027)]
	public class CrazyDarkCultistsMelee : Monster
	{
		public CrazyDarkCultistsMelee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(6035, 6036, 6038, 6039)]
	public class DarkSummoner : Monster
	{
		public DarkSummoner(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30570);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 10f;
			this.WalkSpeed = 0.15f;
		}
	}
	[HandledSNO(178297)]
	public class DarkCultistSummnoer : Monster
	{
		public DarkCultistSummnoer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{

			(Brain as MonsterBrain).AddPresetPower(30547);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.15f;
		}
	}
	[HandledSNO(90367)]
	public class DarkCultistSummnoerTownAttack : Monster
	{
		public DarkCultistSummnoerTownAttack(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
    [HandledSNO(5199 //SandShark_A
				)]
	public class Shark : Monster
	{
		public Shark(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Lacuni
	[HandledSNO(4550, //LacuniMale_A
				4542 //LacuniFemale_B
				)]
	public class Lacuni : Monster
	{
		public Lacuni(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Fallens
	[HandledSNO(4070, //FallenChampion_A
				0
				)]
	public class Fallens : Monster
	{
		public Fallens(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(335727, 249013)]
	public class BodyPile : Monster
	{
		public BodyPile(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(277203, 82764, 261556, 135611, 466620, 360766, 188462, 334290, 199478, 60049)]
	public class Malthael_Spirit : Monster
	{
		public Malthael_Spirit(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.2f;
		}
	}
	[HandledSNO(199478)]
	public class shadowVermin : Monster
	{
		public shadowVermin(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.2f;
		}
	}
	//273417
	[HandledSNO(273417, 273418, 273419, 274324, 346563, 348771)]
	public class DeathMaiden : Monster
	{
		public DeathMaiden(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	//282789
	[HandledSNO(282789, 360861, 310888)]
	public class SkeletonArcher_Westmarch : Monster
	{
		public SkeletonArcher_Westmarch(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.24f;
			//this.WalkSpeed /= 2f;
		}
		
	}
	//276309
	[HandledSNO(276309, 276495//, 282027
		)]
	public class Skeleton_Westmarch : Monster
	{
		public Skeleton_Westmarch(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.25f;
		}
	}
	#endregion
	#region Ghost
	[HandledSNO(370, 136943, 4196, 4197, 4198)]
	public class EnragedPhantom : Monster
	{
		public EnragedPhantom(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			
		}
	}
	[HandledSNO(319442, 309114)]
	public class DarkGhost : Monster
	{
		public DarkGhost(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.25f;
			
		}
	}
	#endregion
	#region Unburieds
	[HandledSNO(6356)]
	public class Unburied : Monster
	{
		public Unburied(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}

	[HandledSNO(6359)]
	public class DisentombHulk : Monster
	{
		public DisentombHulk(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(6572, 139454, 139456)]
	public class WoodWraith : Monster
	{
		public WoodWraith(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.SetVisible(false);
		}
	}
	[HandledSNO(170324, 170325, 495)]
	public class HighLandWalker : Monster
	{
		public HighLandWalker(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.SetVisible(false);
		}
	}
	[HandledSNO(496)]
	public class TheOldMan : Monster
	{
		public TheOldMan(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			// Summon Spores
		}
	}
	#endregion
	#region Zombies
	[HandledSNO(6652)]
	public class WalkingCorpse : Monster
	{
		public WalkingCorpse(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6653)]
	public class HungryCorpse : Monster
	{
		public HungryCorpse(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6654)]
	public class BloatedCorpse : Monster
	{
		public BloatedCorpse(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(204256)]
	public class RancidStumbler : Monster
	{
		public RancidStumbler(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6644)] //ZombieSkinny
	public class Risen : Monster
	{
		public Risen(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6646)] //ZombieSkinny
	public class RavenousDead : Monster
	{
		public RavenousDead(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6647)] //ZombieSkinny
	public class VoraciousZombie : Monster
	{
		public VoraciousZombie(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(6651)] //ZombieSkinny
	public class Decayer : Monster
	{
		public Decayer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	//Risen
	[HandledSNO(218339)] //ZombieSkinny_Custom_A.acr
	public class ZombieSkinny : Monster
	{
		public ZombieSkinny(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(218367)] //ZombieCrawler_Barricade_A.acr
	public class CrowlingTorso : Monster
	{
		public CrowlingTorso(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(203121)] //ZombieSkinny_A_LeahInn.acr
	public class LeahInnZombie : Monster
	{
		public LeahInnZombie(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region Skeleton
	[HandledSNO(539)]
	public class Skeleton : Monster
	{
		public Skeleton(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(87012)]
	public class RoyalHanchman : Monster
	{
		public RoyalHanchman(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(5395)]
	public class Returned : Monster
	{
		public Returned(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(5397)]
	public class SkeletalWarrior : Monster
	{
		public SkeletalWarrior(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(80652)]
	public class SkeletonKnee : Monster
	{
		public SkeletonKnee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//30474
		}
	}
	[HandledSNO(5411)]
	public class SkeletalExecutioner : Monster
	{
		public SkeletalExecutioner(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(434)]
	public class ReturnedExecutioner : Monster
	{
		public ReturnedExecutioner(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region Skeleton_Necromantic_Minion
	//Necromantic Minion
	[HandledSNO(105863)]
	public class NecromanticMinion : Monster
	{
		public NecromanticMinion(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region TriuneCultists
	[HandledSNO(90960)]
	public class TriuneCultist : Monster
	{
		public TriuneCultist(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(5387, 5389)]
	public class TombGuardian : Monster
	{
		public TombGuardian(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(5346)]
	public class SkeletalArcher : Monster
	{
		public SkeletalArcher(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}

	[HandledSNO(5347)]
	public class ReturnedArcher : Monster
	{
		public ReturnedArcher(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region Shield_Skeleton
	[HandledSNO(5275)]
	public class SkeletalShieldBearer : Monster
	{
		public SkeletalShieldBearer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(5276)]
	public class ReturnedShieldMan : Monster
	{
		public ReturnedShieldMan(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(5277)]
	public class SkeletalSentry : Monster
	{
		public SkeletalSentry(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region Grotesque
	[HandledSNO(3847, 3848, 3849, 3850)]
	public class Corpulent : Monster
	{
		//3851 suicide blood, 220536 suicide imps = these happen on different SNOs and happen as they are dying.

		public Corpulent(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(30178); //Explode
		}
	}
	[HandledSNO(4564)]
	public class CorpseWorm : Monster
	{

		public CorpseWorm(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}

	#endregion
	#region FleshPitFlyers
	[HandledSNO(4156)]
	public class CarrionBat : Monster
	{
		public CarrionBat(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(4157, 81954)]
	public class PlagueCarrier : Monster
	{
		public PlagueCarrier(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(218314)]
	public class Glidewing : Monster
	{
		public Glidewing(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(218362)]
	public class Firestarter : Monster
	{
		public Firestarter(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	[HandledSNO(195747)]
	public class VileHellbat : Monster
	{
		public VileHellbat(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}
	}
	#endregion
	#region CarrionNest-FleshPitFlyerSpawner
	[HandledSNO(4152, 4153, 4154, 4155, 4157,
		81982, 207433, 308159, 410428)]
	public class CarrionNest : Monster
	{
		public CarrionNest(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(5840)]
	public class CryptColumn : Monster
	{
		public CryptColumn(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(219725, 108444)] // ZombieFemale_A_TristramQuest_Unique.acr
	public class WretchedMother : Monster
	{
		public WretchedMother(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(4282, 4283, 4284)] // Goatman_Melee_A, Goatman_Melee_B, Goatman_Melee_C, Goatman_Melee_D
	public class Goatman_Moonclan_Melee : Monster
	{
		public Goatman_Moonclan_Melee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Brain = new MonsterBrain(this);

			(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			this.WalkSpeed = 0.2f;
		}
	}
	[HandledSNO(218428)] // [218428] Goatman_Melee_A_Unique_01
	public class Goatman_Moonclan_Melee_Unique1 : Monster
	{
		public Goatman_Moonclan_Melee_Unique1(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Brain = new MonsterBrain(this);

			(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			this.WalkSpeed = 0.2f;
		}
	}
	//218428
	[HandledSNO(4286, 4287)] // Goatman_Ranged_A, Goatman_Ranged_B
	public class Goatman_Moonclan_Ranged : Monster
	{
		public Goatman_Moonclan_Ranged(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Brain = new MonsterBrain(this);

			//(Brain as MonsterBrain).AddPresetPower(30592); //melee_instant
			(Brain as MonsterBrain).AddPresetPower(30252); //Range_instant
			this.WalkSpeed = 0.2f;
		}
	}
	[HandledSNO(375, 4290, 81093)]
	public class GoatmanShaman : Monster
	{
		public GoatmanShaman(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592); // Only distance attack
			(Brain as MonsterBrain).AddPresetPower(77342);
			(Brain as MonsterBrain).AddPresetPower(99077);
			this.WalkSpeed = 0.2f;
		}
	}

	#endregion
	#region GoatmanMelee
	[HandledSNO(4282, 4283, 4284, 81090)]
	public class GoatmanMelee : Monster
	{
		public GoatmanMelee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.2f;
		}
	}
	#endregion
	#region GoatmanRanged
	[HandledSNO(4286, 4287, 81618)]
	public class GoatmanRanged : Monster
	{
		public GoatmanRanged(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592); // Only distance attack
			(Brain as MonsterBrain).AddPresetPower(30252);
		}
	}
	#endregion
	#region WitherMoth
	[HandledSNO(6500)]
	public class WitherMoth : Monster
	{
		public WitherMoth(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(99077);
		}
	}
	#endregion
	#region TriuneWizard
	[HandledSNO(6050)]
	public class TriuneWizard : Monster
	{
		public TriuneWizard(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
	[HandledSNO(6052, 6053, 6054, 178512)]
	public class TriuneBerserker : Monster
	{
		public TriuneBerserker(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.35f;
			(Brain as MonsterBrain).AddPresetPower(99077);
		}
	}
	#endregion
	#region TownAttack Cultist
	[HandledSNO(90367)]
	public class TownAttackCultist : Monster
	{
		public TownAttackCultist(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.1f;
		}
	}
	#endregion
	#region Overseer
	[HandledSNO(98879)]
	public class Overseer : Unique
	{
		public Overseer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.1f;
		}
	}
	#endregion
	#region AzmodanProxy
	[HandledSNO(134722, 111712)]
	public class AzmodanProxy : Unique
	{
		public AzmodanProxy(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.1f;
			this.SetVisible(false);
		}
	}
	#endregion
	#region SkeletonMages
	[HandledSNO(5371, 5372)]
	public class SkeletonMageFire : Monster
	{
		public SkeletonMageFire(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30499);
		}
	}
	[HandledSNO(5367, 5368)]
	public class SkeletonMageCold : Monster
	{
		public SkeletonMageCold(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30497);
		}
	}
	[HandledSNO(5375, 5376)]
	public class SkeletonMageLightning : Monster
	{
		public SkeletonMageLightning(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30500);
		}
	}
	[HandledSNO(5381, 5382)]
	public class SkeletonMagePoison : Monster
	{
		public SkeletonMagePoison(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30502);
			this.WalkSpeed = 0.8f;
		}
	}
	#endregion
	#region A2Snakeman
	[HandledSNO(5428, 5429, 5430, 104015)]
	public class SnakemanRanged : Monster
	{
		public SnakemanRanged(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).RemovePresetPower(30503);
			(Brain as MonsterBrain).AddPresetPower(30509);
		}
	}
	[HandledSNO(5432, 213842, 160525, 222005, 22248, 5433, 5434, 104014)]
	public class SnakemanMelee : Monster
	{
		public SnakemanMelee(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(5428, 5429, 188400, 160443, 222008, 104015, 5430, 367073, 367095)]
	public class SnakemanCaster : Monster
	{
		public SnakemanCaster(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}
	[HandledSNO(4104, 4105, 4106, 219583, 203795, 110613, 222186, 222400, 217744, 220691)]
	public class FastMummy : Monster
	{
		public FastMummy(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.WalkSpeed = 0.3f;
		}
	}

	#endregion
	#region A2FallenShaman
	[HandledSNO(4100, 231351, 4098, 4099)]
	public class FallenShaman : Monster
	{
		public FallenShaman(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30592);
			(Brain as MonsterBrain).AddPresetPower(30252);
			this.WalkSpeed = 0.4f;
		}
	}
	#endregion
	#region A3DemonFlyer
	[HandledSNO(62736, 221770, 134416, 132951, 121327, 130794, 141209)]
	public class DemonFlyer : Monster
	{
		public DemonFlyer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30334);
			(Brain as MonsterBrain).AddPresetPower(130798);
		}
	}
	#endregion
	#region A3Succubus
	[HandledSNO(152535, 152679, 5508)]
	public class Succubus : Monster
	{
		public Succubus(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(120874);
		}
	}
	#endregion
	#region A2SandWasp
	[HandledSNO(5208, 5209, 5210)]
	public class SandWasp : Monster
	{
		public SandWasp(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30334);
			(Brain as MonsterBrain).AddPresetPower(30449);
		}
	}
	#endregion
	#region A4HoodedNightmare
	[HandledSNO(106710)]
	public class HoodedNightmare : Monster
	{
		public HoodedNightmare(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(135412);
		}
	}
	#endregion
	#region GoatMutants
	[HandledSNO(4303, 4304)]
	public class GoatMutantShaman : Monster
	{
		public GoatMutantShaman(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30952);
			(Brain as MonsterBrain).AddPresetPower(157947);
		}
	}
	[HandledSNO(4299, 4300)]
	public class GoatMutantRanged : Monster
	{
		public GoatMutantRanged(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).RemovePresetPower(30952);
			(Brain as MonsterBrain).AddPresetPower(159004);
		}
	}
	#endregion
	#region FallenLunatic
	[HandledSNO(4093, 4094)]
	public class FallenLunatic : Monster
	{
		public FallenLunatic(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			(Brain as MonsterBrain).AddPresetPower(66547);
			this.WalkSpeed = 0.4f;
		}
	}
	#endregion
	#region Wraith
	[HandledSNO(241288, 304460, 363232, 363361, 470719, 360244)]
	public class Wrath : Monster
	{
		public Wrath(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
	#region Spiders
	[HandledSNO(208832 //Spider_Elemental_Cold_tesla_A
				)]
	public class Spider_Elemental_Cold_tesla_A : Monster
	{
		public Spider_Elemental_Cold_tesla_A(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.Movement_Scalar] = this.Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			this.Attributes[GameAttribute.Run_Speed_Granted] = this.Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			this.WalkSpeed = 0.3f;
		}
	}
	#endregion
}
