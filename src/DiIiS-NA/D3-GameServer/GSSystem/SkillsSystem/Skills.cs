using DiIiS_NA.LoginServer.Toons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiIiS_NA.GameServer.GSSystem.SkillsSystem
{
	public class Skills
	{
		public static int None = -1;
		public static int BasicAttack = 0x00007780;

		public static List<int> GetAllActiveSkillsByClass(ToonClass @class)
		{
			switch (@class)
			{
				case ToonClass.Barbarian:
					return Barbarian.AllActiveSkillsList;
				case ToonClass.Crusader:
					return Crusader.AllActiveSkillsList;
				case ToonClass.DemonHunter:
					return DemonHunter.AllActiveSkillsList;
				case ToonClass.Monk:
					return Monk.AllActiveSkillsList;
				case ToonClass.WitchDoctor:
					return WitchDoctor.AllActiveSkillsList;
				case ToonClass.Wizard:
					return Wizard.AllActiveSkillsList;
				case ToonClass.Necromancer:
					return Necromancer.AllActiveSkillsList;
				default:
					return null;
			}
		}

		public static List<int> GetPrimarySkillsByClass(ToonClass @class)
		{
			switch (@class)
			{
				case ToonClass.Barbarian:
					return Barbarian.FuryGenerators.List;
				case ToonClass.Crusader:
					return Crusader.FaithGenerators.List;
				case ToonClass.DemonHunter:
					return DemonHunter.HatredGenerators.List;
				case ToonClass.Monk:
					return Monk.SpiritGenerator.List;
				case ToonClass.WitchDoctor:
					return WitchDoctor.PhysicalRealm.List;
				case ToonClass.Wizard:
					return Wizard.Signature.List;
				case ToonClass.Necromancer:
					return Necromancer.MainSkills.List;
				default:
					return null;
			}
		}

		public static List<int> GetSecondarySkillsByClass(ToonClass @class)
		{
			switch (@class)
			{
				case ToonClass.Barbarian:
					return Barbarian.FurySpenders.List;
				case ToonClass.Crusader:
					return Crusader.FaithSpenders.List;
				case ToonClass.DemonHunter:
					return DemonHunter.HatredSpenders.List;
				case ToonClass.Monk:
					return Monk.SpiritSpenders.List;
				case ToonClass.WitchDoctor:
					return WitchDoctor.SpiritRealm.List;
				case ToonClass.Wizard:
					return Wizard.Offensive.List;
				case ToonClass.Necromancer:
					return Necromancer.SecondrySkills.List;
				default:
					return null;
			}
		}

		public static List<int> GetExtraSkillsByClass(ToonClass @class)
		{
			switch (@class)
			{
				case ToonClass.Barbarian:
					return Barbarian.Situational.List;
				case ToonClass.Crusader:
					return Crusader.Situational.List;
				case ToonClass.DemonHunter:
					return DemonHunter.Discipline.List;
				case ToonClass.Monk:
					return Monk.Mantras.List;
				case ToonClass.WitchDoctor:
					return WitchDoctor.Support.List;
				case ToonClass.Wizard:
					return Wizard.Utility.List;
				case ToonClass.Necromancer:
					return Necromancer.ExtraSkills.List;
				default:
					return null;
			}
		}

		public static List<int> GetPassiveSkills(ToonClass @class)
		{
			switch (@class)
			{
				case ToonClass.Barbarian:
					return Barbarian.Passives.List;
				case ToonClass.Crusader:
					return Crusader.Passives.List;
				case ToonClass.DemonHunter:
					return DemonHunter.Passives.List;
				case ToonClass.Monk:
					return Monk.Passives.List;
				case ToonClass.WitchDoctor:
					return WitchDoctor.Passives.List;
				case ToonClass.Wizard:
					return Wizard.Passives.List;
				case ToonClass.Necromancer:
					return Necromancer.Passives.List;
				default:
					return null;
			}
		}

		#region barbarian

		public class Barbarian
		{
			public static readonly List<int> AllActiveSkillsList =
				FuryGenerators.List.Concat(FurySpenders.List).Concat(Situational.List).ToList();

			public class FuryGenerators
			{
				public const int Bash = 0x0001358A;
				public const int Cleave = 0x00013987;
				public const int LeapAttack = 0x00016CE1;
				public const int GroundStomp = 0x00013656;
				public const int Frenzy = 0x000132D4;
				public const int WarCry = 375483;
				public const int FuriousCharge = 0x00017C9B;
				public const int WeaponThrow = 377452;

				public static readonly List<int> List = new List<int>
				{
					Bash,
					Cleave,
					LeapAttack,
					GroundStomp,
					Frenzy,
					WarCry,
					FuriousCharge,
					WeaponThrow
				};
			}

			public class FurySpenders
			{
				public const int HammerOfTheAncients = 0x0001389C;
				public const int ThreateningShout = 0x000134E5;
				public const int BattleRage = 0x000134E4;
				public const int Rend = 0x00011348;
				public const int SiesmicSlam = 0x000153CD;
				public const int Sprint = 0x000132D7;
				public const int Whirlwind = 0x00017828;
				public const int Avalanche = 353447;
				public const int AncientSpear = 377453;

				public static readonly List<int> List = new List<int>
				{
					HammerOfTheAncients,
					ThreateningShout,
					BattleRage,
					Rend,
					SiesmicSlam,
					Sprint,
					Whirlwind,
					Avalanche,
					AncientSpear
				};
			}

			public class Situational
			{
				public const int IgnorePain = 0x000136A8;
				public const int Revenge = 0x0001AB1E;
				public const int Overpower = 0x00026DC1;
				public const int Earthquake = 0x0001823E;
				public const int CallOfTheAncients = 0x000138B1;
				public const int WrathOfTheBerserker = 0x000136F7;

				public static readonly List<int> List = new List<int>
				{
					IgnorePain,
					Revenge,
					Overpower,
					Earthquake,
					CallOfTheAncients,
					WrathOfTheBerserker
				};
			}

			public class Passives
			{
				public const int BloodThirst = 0x000321A1;
				public const int PoundOfFlesh = 0x00032195;
				public const int Ruthless = 0x00032177;
				public const int WeaponsMaster = 0x00032543;
				public const int InspiringPresence = 0x000322EA;
				public const int BerserkerRage = 0x00032183;
				public const int Animosity = 0x000321AC;
				public const int Superstition = 0x000322B3;
				public const int ToughAsNails = 0x00032418;
				public const int NoEscape = 0x00031FB5;
				public const int Relentless = 0x00032256;
				public const int Brawler = 0x0003214D;
				public const int Juggernaut = 0x0003238B;
				public const int BoonOfBulKathos = 0x00031F3B;
				public const int Unforgiving = 0x000321F4;
				public const int EarthenMight = 361661;
				public const int SwordAndBoard = 340877;
				public const int Rampage = 296572;

				public static readonly List<int> List = new List<int>
				{
					BloodThirst,
					PoundOfFlesh,
					Ruthless,
					WeaponsMaster,
					InspiringPresence,
					BerserkerRage,
					Animosity,
					Superstition,
					ToughAsNails,
					NoEscape,
					Relentless,
					Brawler,
					Juggernaut,
					BoonOfBulKathos,
					Unforgiving,
					EarthenMight,
					SwordAndBoard,
					Rampage
				};
			}
		}

		#endregion

		#region Crusader

		public class Crusader
		{
			public static readonly List<int> AllActiveSkillsList =
				FaithGenerators.List.Concat(FaithSpenders.List).Concat(Situational.List).ToList();

			public class FaithGenerators
			{
				public const int Punish = 285903;
				public const int Slash = 289243;
				public const int Smite = 286510;
				public const int Justice = 325216;
				public const int Provoke = 290545;
				public const int AkaratChampion = 269032;

				public static readonly List<int> List = new List<int>
				{
					Punish,
					Slash,
					Smite,
					Justice,
					Provoke,
					AkaratChampion
				};
			}

			public class FaithSpenders
			{
				public const int ShieldBash = 353492;
				public const int SweepAttack = 239042;
				public const int BlessedHammer = 266766;
				public const int BlessedShield = 266951;
				public const int FistOfTheHeavens = 239218;
				public const int Phalanx = 330729;
				public const int FallingSword = 239137;

				public static readonly List<int> List = new List<int>
				{
					ShieldBash,
					SweepAttack,
					BlessedHammer,
					BlessedShield,
					FistOfTheHeavens,
					Phalanx,
					FallingSword
				};
			}

			public class Situational
			{
				public const int ShieldGlare = 268530;
				public const int IronSkin = 291804;
				public const int Consecration = 273941;
				public const int Judgment = 267600;
				public const int Condemn = 266627;
				public const int SteedCharge = 243853;
				public const int LawsOfValor = 342281;
				public const int LawsOfJustice = 342280;
				public const int LawsOfHope = 342279;
				public const int HeavenFury = 316014;
				public const int Bombardment = 284876;

				public static readonly List<int> List = new List<int>
				{
					ShieldGlare,
					IronSkin,
					Consecration,
					Judgment,
					SteedCharge,
					LawsOfValor,
					LawsOfJustice,
					LawsOfHope,
					HeavenFury,
					Bombardment
				};
			}

			public class Passives
			{
				public const int HeavenlyStrength = 286177;
				public const int Fervor = 357218;
				public const int Vigilant = 310626;
				public const int Righteousness = 356147;
				public const int Insurmountable = 310640;
				public const int Indestructible = 309830;
				public const int HolyCause = 310804;
				public const int Wrathful = 310775;
				public const int DivineFortress = 356176;
				public const int LordCommander = 348741;
				public const int HoldYourGround = 302500;
				public const int LongArmOfTheLaw = 310678;
				public const int IronMaiden = 310783;
				public const int Renewal = 356173;
				public const int Finery = 311629;
				public const int Blunt = 348773;
				public const int ToweringShield = 356052;

				public static readonly List<int> List = new List<int>
				{
					HeavenlyStrength,
					Fervor,
					Vigilant,
					Righteousness,
					Insurmountable,
					Indestructible,
					HolyCause,
					Wrathful,
					DivineFortress,
					LordCommander,
					HoldYourGround,
					LongArmOfTheLaw,
					IronMaiden,
					Renewal,
					Finery,
					Blunt,
					ToweringShield
				};
			}
		}

		#endregion

		#region demon-hunter

		public class DemonHunter
		{
			public static readonly List<int> AllActiveSkillsList =
				HatredGenerators.List.Concat(HatredSpenders.List).Concat(Discipline.List).ToList();


			public class HatredGenerators
			{
				public const int HungeringArrow = 0x0001F8BF;
				public const int EvasiveFire = 377450;
				public const int BolaShot = 0x00012EF0; //Renamed Bolas
				public const int EntanglingShot = 361936;
				public const int Grenades = 0x00015252;
				public const int SpikeTrap = 0x00012625;
				public const int Strafe = 0x00020B8E;
				public const int Vengeance = 302846;

				public static readonly List<int> List = new List<int>
				{
					HungeringArrow,
					EvasiveFire,
					BolaShot,
					EntanglingShot,
					Grenades,
					SpikeTrap,
					Strafe,
					Vengeance
				};
			}

			public class HatredSpenders
			{
				public const int Impale = 0x00020126;
				public const int RapidFire = 0x00020078;
				public const int Chakram = 0x0001F8BD;
				public const int ElementalArrow = 0x000200FD;
				public const int FanOfKnives = 0x00012EEA;
				public const int Multishot = 0x00012F51;
				public const int ClusterArrow = 0x0001F8BE;
				public const int RainOfVengeance = 0x001FF0F;

				public static readonly List<int> List = new List<int>
				{
					Impale,
					RapidFire,
					Chakram,
					ElementalArrow,
					FanOfKnives,
					Multishot,
					ClusterArrow,
					RainOfVengeance
				};
			}

			public class Discipline
			{
				public const int Caltrops = 0x0001F8C0;
				public const int Vault = 0x0001B26F;
				public const int ShadowPower = 0x0001FF0E;
				public const int Companion = 365311;
				public const int SmokeScreen = 0x0001FE87;
				public const int Sentry = 0x0001F8C1;
				public const int MarkedForDeath = 0x0001FEB2;
				public const int Preparation = 0x0001F8BC;

				public static readonly List<int> List = new List<int>
				{
					Caltrops,
					Vault,
					ShadowPower,
					Companion,
					SmokeScreen,
					Sentry,
					MarkedForDeath,
					Preparation
				};
			}

			public class Passives
			{
				public const int Brooding = 0x00033771;
				public const int ThrillOfTheHunt = 0x00033919;
				public const int Vengeance = 0x00026042;
				public const int SteadyAim = 0x0002820B;
				public const int CullTheWeak = 0x00026049;
				public const int Fundamentals = 0x00026047;
				public const int HotPursuit = 0x0002604D;
				public const int Archery = 0x00033346;
				public const int Perfectionist = 0x0002604A;
				public const int CustomEngineering = 0x00032EE2;
				public const int Grenadier = 0x00032F8B;
				public const int Sharpshooter = 0x00026043;
				public const int Ballistics = 0x0002604B; // ID: 155723
				public const int Ambush = 352920;
				public const int Awareness = 324770;
				public const int SingleOut = 338859;

				public static readonly List<int> List = new List<int>
				{
					Brooding,
					ThrillOfTheHunt,
					Vengeance,
					SteadyAim,
					CullTheWeak,
					Fundamentals,
					HotPursuit,
					Archery,
					Perfectionist,
					CustomEngineering,
					Grenadier,
					Sharpshooter,
					Ballistics,
					Ambush,
					Awareness,
					SingleOut
				};
			}
		}

		#endregion

		#region monk

		public class Monk
		{
			public static readonly List<int> AllActiveSkillsList =
				SpiritGenerator.List.Concat(SpiritSpenders.List).Concat(Mantras.List).ToList();

			public class SpiritGenerator
			{
				public const int FistsOfThunder = 0x000176C4;
				public const int DeadlyReach = 0x00017713;
				public const int CripplingWave = 0x00017837;
				public const int ExplodingPalm = 0x00017C30;
				public const int SweepingWind = 0x0001775A;
				public const int WayOfTheHundredFists = 0x00017B56;
				public const int Epiphany = 312307;

				public static readonly List<int> List = new List<int>
				{
					FistsOfThunder,
					DeadlyReach,
					CripplingWave,
					ExplodingPalm,
					SweepingWind,
					WayOfTheHundredFists,
					Epiphany
				};
			}

			public class SpiritSpenders
			{
				public const int BlindingFlash = 0x000216FA;
				public const int BreathOfHeaven = 0x00010E0A;
				public const int LashingTailKick = 0x0001B43C;
				public const int DashingStrike = 312736;
				public const int LethalDecoy = 0x00011044;
				public const int InnerSanctuary = 317076;
				public const int TempestRush = 0x0001DA62;
				public const int Serenity = 0x000177D7;
				public const int SevenSidedStrike = 0x000179B6;
				public const int MysticAlly = 0x00058676;
				public const int WaveOfLight = 0x00017721;
				public const int CycloneStrike = 223473;

				public static readonly List<int> List = new List<int>
				{
					BlindingFlash,
					BreathOfHeaven,
					LashingTailKick,
					DashingStrike,
					LethalDecoy,
					InnerSanctuary,
					TempestRush,
					Serenity,
					SevenSidedStrike,
					MysticAlly,
					WaveOfLight,
					CycloneStrike
				};
			}

			public class Mantras
			{
				public const int MantraOfEvasion = 375049;
				public const int MantraOfRetribution = 375082;
				public const int MantraOfHealing = 373143;
				public const int MantraOfConviction = 375088;

				public static readonly List<int> List = new List<int>
				{
					MantraOfEvasion,
					MantraOfRetribution,
					MantraOfHealing,
					MantraOfConviction
				};
			}

			public class Passives
			{
				public const int Resolve = 0x00033A7D;
				public const int TheGuardiansPath = 0x00033394;
				public const int SixthSense = 0x000332D6;
				public const int OneWithEverything = 0x000332F8;
				public const int SeizeTheInitiative = 0x000332DC;
				public const int Transcendence = 0x00033162;
				public const int ChantOfResonance = 0x00026333;
				public const int BeaconOfYtar = 0x000330D0;
				public const int FleetFooted = 0x00033085;
				public const int ExaltedSoul = 0x00033083;
				public const int Pacifism = 0x00033395;
				public const int GuidingLight = 0x0002634C;
				public const int NearDeathExperience = 0x00026344;
				public const int Unity = 368899;
				public const int Momentum = 341559;
				public const int MythicRhythm = 315271;

				public static readonly List<int> List = new List<int>
				{
					Resolve,
					TheGuardiansPath,
					SixthSense,
					OneWithEverything,
					SeizeTheInitiative,
					Transcendence,
					ChantOfResonance,
					BeaconOfYtar,
					FleetFooted,
					ExaltedSoul,
					Pacifism,
					GuidingLight,
					NearDeathExperience,
					Unity,
					Momentum,
					MythicRhythm
				};
			}
		}

		#endregion

		#region witch-doctor

		public class WitchDoctor
		{
			public static readonly List<int> AllActiveSkillsList =
				PhysicalRealm.List.Concat(SpiritRealm.List).Concat(Support.List).ToList();

			public class PhysicalRealm
			{
				public const int PoisonDart = 0x0001930D;
				public const int PlagueOfToads = 0x00019FE1;
				public const int ZombieCharger = 0x00012113;
				public const int CorpseSpiders = 0x000110EA;
				public const int Firebats = 0x00019DEB;
				public const int Firebomb = 0x000107EF;
				public const int LocustSwarm = 0x000110EB;
				public const int AcidCloud = 0x00011337;
				public const int WallOfZombies = 0x00020EB5;
				public const int Piranhas = 347265;

				public static readonly List<int> List = new List<int>
				{
					PoisonDart,
					PlagueOfToads,
					ZombieCharger,
					CorpseSpiders,
					Firebats,
					Firebomb,
					LocustSwarm,
					AcidCloud,
					WallOfZombies,
					Piranhas
				};
			}

			public class SpiritRealm
			{
				public const int Haunt = 0x00014692;
				public const int Horrify = 0x00010854;
				public const int SpiritWalk = 0x00019EFD;
				public const int SoulHarvest = 0x00010820;
				public const int SpiritBarrage = 0x0001A7DA;
				public const int MassConfusion = 0x00010810;

				public static readonly List<int> List = new List<int>
				{
					Haunt,
					Horrify,
					SpiritWalk,
					SoulHarvest,
					SpiritBarrage,
					MassConfusion
				};
			}

			public class Support
			{
				public const int SummonZombieDogs = 0x000190AD;
				public const int GraspOfTheDead = 0x00010E3E;
				public const int Hex = 0x000077A7;
				public const int Sacrifice = 0x000190AC;
				public const int Gargantuan = 0x000077A0;
				public const int BigBadVoodoo = 0x0001CA9A;
				public const int FetishArmy = 0x000011C51;

				public static readonly List<int> List = new List<int>
				{
					SummonZombieDogs,
					GraspOfTheDead,
					Hex,
					Sacrifice,
					Gargantuan,
					BigBadVoodoo,
					FetishArmy
				};
			}

			public class Passives
			{
				public const int CircleOfLife = 0x00032EBB;
				public const int Vermin = 0x00032EB7;
				public const int SpiritualAttunement = 0x00032EB9;
				public const int GruesomeFeast = 0x00032ED2;
				public const int BloodRitual = 0x00032EB8;
				public const int ZombieHandler = 0x00032EB3;
				public const int PierceTheVeil = 0x00032EF4;
				public const int RushOfEssence = 0x00032EB5;
				public const int VisionQuest = 0x00033091;
				public const int FierceLoyalty = 0x00032EFF;
				public const int DeathTrance = 0x00032EB4;
				public const int TribalRites = 0x00032ED9;
				public const int CreepingDeath = 340908;
				public const int PhysicalAttunement = 340910;
				public const int MidnightFeast = 340909;

				public static readonly List<int> List = new List<int>
				{
					CircleOfLife,
					Vermin,
					SpiritualAttunement,
					GruesomeFeast,
					BloodRitual,
					ZombieHandler,
					PierceTheVeil,
					RushOfEssence,
					VisionQuest,
					FierceLoyalty,
					DeathTrance,
					TribalRites,
					CreepingDeath,
					PhysicalAttunement,
					MidnightFeast
				};
			}
		}

		#endregion

		#region wizard

		public class Wizard
		{
			public static readonly List<int> AllActiveSkillsList =
				Signature.List.Concat(Offensive.List).Concat(Utility.List).ToList();

			public class Signature
			{
				public const int MagicMissile = 0x00007818;
				public const int ShockPulse = 0x0000783F;
				public const int SpectralBlade = 0x0001177C;
				public const int Electrocute = 0x000006E5;

				public static readonly List<int> List = new List<int>
				{
					MagicMissile,
					ShockPulse,
					SpectralBlade,
					Electrocute
				};
			}

			public class Offensive
			{
				public const int WaveOfForce = 0x0000784C;
				public const int ArcaneOrb = 0x000077CC;
				public const int EnergyTwister = 0x00012D39;
				public const int Disintegrate = 0x0001659D;
				public const int ExplosiveBlast = 0x000155E5;
				public const int Hydra = 0x00007805;
				public const int RayOfFrost = 0x00016CD3;
				public const int ArcaneTorrent = 0x00020D38;
				public const int Meteor = 0x00010E46;
				public const int Blizzard = 0x000077D8;

				public static readonly List<int> List = new List<int>
				{
					WaveOfForce,
					ArcaneOrb,
					EnergyTwister,
					Disintegrate,
					ExplosiveBlast,
					Hydra,
					RayOfFrost,
					ArcaneTorrent,
					Meteor,
					Blizzard
				};
			}

			public class Utility
			{
				public const int FrostNova = 0x000077FE;
				public const int IceArmor = 0x00011E07;
				public const int MagicWeapon = 0x0001294C;
				public const int DiamondSkin = 0x0001274F;
				public const int StormArmor = 0x00012303;
				public const int MirrorImage = 0x00017EEB;
				public const int SlowTime = 0x000006E9;
				public const int Teleport = 0x00029198;
				public const int EnergyArmor = 0x000153CF;
				public const int Familiar = 0x00018330;
				public const int Archon = 0x00020ED8;
				public const int BlackHole = 243141;

				public static readonly List<int> List = new List<int>
				{
					FrostNova,
					IceArmor,
					MagicWeapon,
					DiamondSkin,
					StormArmor,
					MirrorImage,
					SlowTime,
					Teleport,
					EnergyArmor,
					Familiar,
					Archon,
					BlackHole
				};
			}

			public class Passives
			{
				public const int PowerHungry = 0x00032E5E;
				public const int TemporalFlux = 0x00032E5D;
				public const int GlassCannon = 0x00032E57;
				public const int Prodigy = 0x00032E6D;
				public const int Virtuoso = 0x00032E65;
				public const int AstralPresence = 0x00032E58;
				public const int Illusionist = 0x00032EA3;
				public const int GalvanizingWard = 0x00032E9D;
				public const int Blur = 0x00032E54;
				public const int Evocation = 0x00032E59;
				public const int ArcaneDynamo = 0x00032FB7;
				public const int UnstableAnomaly = 0x00032E5A;
				public const int UnwaveringWill = 298038;
				public const int Audacity = 341540;
				public const int ElementalExposure = 342326;

				public static readonly List<int> List = new List<int>
				{
					PowerHungry,
					TemporalFlux,
					GlassCannon,
					Prodigy,
					Virtuoso,
					AstralPresence,
					Illusionist,
					GalvanizingWard,
					Blur,
					Evocation,
					ArcaneDynamo,
					UnstableAnomaly,
					UnwaveringWill,
					Audacity,
					ElementalExposure
				};
			}
		}

		#endregion

		#region Necromancer

		public class Necromancer
		{
			public static readonly List<int> AllActiveSkillsList =
				MainSkills.List.Concat(SecondrySkills.List).Concat(SecondrySkills.List).ToList();

			public class MainSkills
			{
				public const int BoneSpikes = 462147; //1 Уровень
				public const int GrimScythe = 462198; //3 Уровень
				public const int SiphonBlood = 453563;//11 Уровень
				public static readonly List<int> List = new List<int>
				{
					BoneSpikes,
					GrimScythe,
					SiphonBlood
				};
			}

			public class SecondrySkills
			{
				public const int BoneSpear = 451490;//2 Уровень
				public const int SkeletalMage = 462089;//5 Уровень
				public const int DeathNova = 462243;//12 Уровень

				public static readonly List<int> List = new List<int>
				{
					BoneSpear,
					SkeletalMage,
					DeathNova
				};
			}

			public class ExtraSkills
			{
				//Corpses
				public const int CorpseExlosion = 454174;//4 Уровень
				public const int CorpseLance = 461650;//8 Уровень
				public const int Devour = 460757;//16 Уровень
				public const int Revive = 462239;//22 Уровень

				//Reanimation
				public const int CommandSkeleton = 453801;//9уровень
				public const int CommandGolem = 451537;//13 уровень
				public const int ArmyofDead = 460358;//22 уровень
				public const int LandOfDead = 465839;//38 уровень

				//Curses
				public const int Decrepify = 451491;//14 уровень
				public const int Leech = 462255;//17 уровень
				public const int Frailty = 460870;//22 уровень

				//Blood&Bone
				public const int BoneArmor = 466857;//19 уровень
				public const int BoneSpirit = 464896;//25 уровень
				public const int BloodRush = 454090; //30 уровень
				public const int Simulacrum = 465350;//61 уровень
				public static readonly List<int> List = new List<int>
				{
					CorpseExlosion,
					CorpseLance,
					Devour,
					Revive,
					CommandSkeleton,
					CommandGolem,
					ArmyofDead,
					LandOfDead,
					Decrepify,
					Leech,
					Frailty,
					BoneArmor,
					BoneSpirit,
					BloodRush,
					Simulacrum
				};
			}

			public class Passives
			{
				public const int Decrepify = 471738;
				public const int Frailty = 471845;
				public const int Leech = 471869;
				public const int PuppetMaster = 464994;
				public const int BloodIsPower = 465037;
				public const int LifeFromDeath = 465703;
				public const int HealthRegen = 465264;
				public const int BloodForBlood = 465821;
				public const int CheatDeath = 465952;
				public const int FueledByDeath = 465917;
				public const int StandAlone = 470725;
				public const int MaxEssence = 470764;
				public const int SwiftHarvesting = 470805;
				public const int DarkReaping = 470812;
				public const int EternalTorment = 472795;
				public const int Serration = 472905;
				public const int RathmasShield = 472910;
				public const int AberrantAnimator = 472949;
				public const int CommanderOTRisen = 472962;
				public const int BonePrison = 472965;
				public const int GrislyTribute = 473019;
				public const int RigorMortis = 466415;
				public const int SpreadingMalediction = 472220;

				public static readonly List<int> List = new List<int>
				{
					Decrepify,
					Frailty,
					Leech,
					PuppetMaster,
					BloodIsPower,
					LifeFromDeath,
					HealthRegen,
					BloodForBlood,
					CheatDeath,
					FueledByDeath,
					StandAlone,
					MaxEssence,
					SwiftHarvesting,
					DarkReaping,
					EternalTorment,
					Serration,
					RathmasShield,
					AberrantAnimator,
					CommanderOTRisen,
					BonePrison,
					RigorMortis,
					SpreadingMalediction
				};
			}
		}

		#endregion
	}
}
