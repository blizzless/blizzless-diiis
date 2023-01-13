//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;

//Blizzless Project 2022 
using GameBalance = DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using System.Threading;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Text;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Skill;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;   
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.SkillsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields.BlizzLess.Net.GS.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Waypoint;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Trade;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.UI;

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem
{
	public class Player : Actor, IMessageConsumer, IUpdateable
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The ingame-client for player.
		/// </summary>
		public GameClient InGameClient { get; set; }

		/// <summary>
		/// The player index.
		/// </summary>
		public int PlayerIndex { get; private set; }

		/// <summary>
		/// The player index.
		/// </summary>
		public int PlayerGroupIndex { get; private set; }

		/// <summary>
		/// Current crafting NPC type(for learning recipes)
		/// </summary>
		public string ArtisanInteraction = "None";

		/// <summary>
		/// The player's toon.
		/// We need a better name /raist.
		/// </summary>
		public Toon Toon { get; private set; }

		public float DecreaseUseResourcePercent = 0;
		public int Level { get; private set; }
		public int ParagonLevel { get; private set; }
		public long ExperienceNext { get; private set; }
		public List<Actor> Revived = new List<Actor>() { };

		public bool LevelingBoosted { get; set; }

		public int PreSceneId = -1;

		public List<Actor> NecroSkeletons = new List<Actor> { };
		public bool ActiveSkeletons = false;
		public Actor ActiveGolem = null;
		public bool EnableGolem = false;
		public bool IsInPvPWorld
		{
			get
			{
				return (this.World != null && this.World.IsPvP);
			}
			set { }
		}

		/// <summary>
		/// Skillset for the player (or actually for player's toons class).
		/// </summary>
		public SkillSet SkillSet { get; private set; }

		/// <summary>
		/// The inventory of player's toon.
		/// </summary>
		public Inventory Inventory { get; private set; }

		public int GearScore
		{
			get
			{
				if (this.Inventory == null)
					return 0;
				else
					return this.Inventory.GetGearScore();
			}
			private set { }
		}

		public Actor PlayerDirectBanner = null;

		public uint NewDynamicID(uint globalId, int pIndex = -1)
		{
			lock (this.RevealedObjects)
			{
				if (pIndex > -1)
					return (uint)pIndex;
				for (uint i = 9; i < 4123; i++)
				{
					if (!this.RevealedObjects.ContainsValue(i))
					{
						//Logger.Trace("adding GlobalId {0} -> DynID {1} to player {2}", globalId, i, this.Toon.Name);
						return i;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// ActorType = Player.
		/// </summary>
		public override ActorType ActorType { get { return ActorType.Player; } }

		/// <summary>
		/// Revealed objects to player.
		/// </summary>
		public Dictionary<uint, uint> RevealedObjects = new Dictionary<uint, uint>();

		public ConversationManager Conversations { get; private set; }
		public int SpecialComboIndex = 0;
		// Collection of items that only the player can see. This is only used when items drop from killing an actor
		// TODO: Might want to just have a field on the item itself to indicate whether it is visible to only one player
		/// <summary>
		/// Dropped items for the player
		/// </summary>
		public Dictionary<uint, Item> GroundItems { get; private set; }

		/// <summary>
		/// Everything connected to ExpBonuses.
		/// </summary>
		public ExpBonusData ExpBonusData { get; private set; }

		public bool EventWeatherEnabled { get; set; }

		public bool BlacksmithUnlocked { get; set; }
		public bool JewelerUnlocked { get; set; }
		public bool MysticUnlocked { get; set; }
		public bool KanaiUnlocked { get; set; }

		public bool HirelingTemplarUnlocked { get; set; }
		public bool HirelingScoundrelUnlocked { get; set; }
		public bool HirelingEnchantressUnlocked { get; set; }

		public int LastMovementTick = 0;

		public int _spiritGenHit = 0;
		public int _SpiritGeneratorHit
		{
			get
			{
				return _spiritGenHit;
			}

			set
			{
				_spiritGenHit = value;
				if (this.SkillSet.HasPassive(315271) && _spiritGenHit >= 3) //Mythic Rhythm
				{
					this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.MythicRhythmBuff());
					_spiritGenHit = 0;
				}
			}
		}

		/// <summary>
		/// NPC currently interaced with
		/// </summary>
		public InteractiveNPC SelectedNPC { get; set; }
		public Dictionary<uint, int> Followers { get; private set; }
		private Hireling _activeHireling = null;
		private Hireling _questHireling = null;
		public Hireling ActiveHireling
		{
			get { return _activeHireling; }
			set
			{
				if (value == null)
				{
					this.HirelingId = null;
					lock (this.Toon.DBToon)
					{
						var dbToon = this.Toon.DBToon;
						dbToon.ActiveHireling = null;
						DBSessions.SessionUpdate(dbToon);
					}
				}
				else if (value != _activeHireling)
				{
					this.HirelingId = value.Attributes[GameAttribute.Hireling_Class];
					lock (this.Toon.DBToon)
					{
						var dbToon = this.Toon.DBToon;
						dbToon.ActiveHireling = value.Attributes[GameAttribute.Hireling_Class];
						DBSessions.SessionUpdate(dbToon);
					}
				}

				if (value == _activeHireling && value != null)
					return;

				if (_activeHireling != null)
				{
					_activeHireling.Dismiss();
				}

				_activeHireling = value;
			}
		}
		public Hireling SetQuestHireling
		{
			get { return _questHireling; }
			set
			{
				if (_questHireling != null)
				{
					_questHireling.Dismiss();
				}
				_questHireling = value;
			}
		}
		public int CurrentWingsPowerId = -1;
		private int _lastResourceUpdateTick;
		private float _CurrentHPValue = -1f;
		private float _CurrentResourceValue = -1f;
		public int GoldCollectedTempCount = 0;
		public int BloodShardsCollectedTempCount = 0;
		public int KilledMonstersTempCount = 0;
		public int KilledSeasonalTempCount = 0;
		public int KilledElitesTempCount = 0;
		public int BuffStreakKill = 0;
		private byte[] ParagonBonuses;
		public int? HirelingId = null;
		public bool IsCasting = false;
		private Action CastResult = null;
		private Action ConfirmationResult = null;
		private const float SkillChangeCooldownLength = 10f;
		/// <summary>
		/// Creates a new player.
		/// </summary>
		/// <param name="world">The initial world player joins in.</param>
		/// <param name="client">The gameclient for the player.</param>
		/// <param name="bnetToon">Toon of the player.</param>
		public Player(World world, GameClient client, Toon bnetToon)
			: base(world, bnetToon.Gender == 0 ? bnetToon.HeroTable.SNOMaleActor : bnetToon.HeroTable.SNOFemaleActor)
		{
			this.InGameClient = client;
			this.PlayerIndex = Interlocked.Increment(ref this.InGameClient.Game.PlayerIndexCounter);
			this.PlayerGroupIndex = this.InGameClient.Game.PlayerGroupIndexCounter;
			this.Toon = bnetToon;
			this.LevelingBoosted = this.Toon.LevelingBoosted;
			var dbToon = this.Toon.DBToon;
			this.HirelingId = dbToon.ActiveHireling;
			this.GBHandle.Type = (int)ActorType.Player;
			this.GBHandle.GBID = this.Toon.ClassID;
			this.Level = dbToon.Level;
			this.ParagonLevel = this.Toon.ParagonLevel;
			this.ExperienceNext = this.Toon.ExperienceNext;
			this.ParagonBonuses = dbToon.ParagonBonuses;
			this.CurrentWingsPowerId = dbToon.WingsActive;

			this.Field2 = 0x00000009;
			this.Scale = this.ModelScale;
			this.RotationW = 0.05940768f;
			this.RotationAxis = new Vector3D(0f, 0f, 0.9982339f);
			this.Field7 = -1;
			this.NameSNOId = -1;
			this.Field10 = 0x0;
			this.Dead = false;
			this.EventWeatherEnabled = false;

			var achievements = this.InGameClient.Game.GameDBSession.SessionQueryWhere<DBAchievements>(dba => dba.DBGameAccount.Id == this.Toon.GameAccount.PersistentID);

			this.BlacksmithUnlocked = achievements.Where(dba => dba.AchievementId == 74987243307766).Count() > 0;
			this.JewelerUnlocked = achievements.Where(dba => dba.AchievementId == 74987243307780).Count() > 0;
			this.MysticUnlocked = achievements.Where(dba => dba.AchievementId == 74987247205955).Count() > 0;

			this.KanaiUnlocked = false;
			foreach (var achi in achievements.Where(dba => dba.AchievementId == 74987254626662).ToList())
				foreach (var crit in AchievementSystem.AchievementManager.UnserializeBytes(achi.Criteria))
					if (crit == unchecked((uint)74987252674266))
						KanaiUnlocked = true;

			if (Level >= 70)
				this.GrantCriteria(74987254853541);

			this.HirelingTemplarUnlocked = achievements.Where(dba => dba.AchievementId == 74987243307073).Count() > 0;
			this.HirelingScoundrelUnlocked = achievements.Where(dba => dba.AchievementId == 74987243307147).Count() > 0;
			this.HirelingEnchantressUnlocked = achievements.Where(dba => dba.AchievementId == 74987243307145).Count() > 0;
			this.SkillSet = new SkillSet(this, this.Toon.Class, this.Toon);
			this.GroundItems = new Dictionary<uint, Item>();
			this.Followers = new Dictionary<uint, int>();
			this.Conversations = new ConversationManager(this);
			this.ExpBonusData = new ExpBonusData(this);
			this.SelectedNPC = null;

			this._lastResourceUpdateTick = 0;
			this.SavePointData = new SavePointData() { snoWorld = -1, SavepointId = -1 };

			// Attributes
			if (this.World.Game.PvP)
				this.Attributes[GameAttribute.TeamID] = this.PlayerIndex + 2;
			else
				this.Attributes[GameAttribute.TeamID] = 2;

			SetAllStatsInCorrectOrder();
			// Enabled stone of recall
			if (!this.World.Game.PvP & this.Toon.StoneOfPortal)
				EnableStoneOfRecall();
			else if (this.InGameClient.Game.CurrentAct == 3000)
				EnableStoneOfRecall();

			List<int> lores = UnserializeBytes(this.Toon.DBToon.Lore);
			int num = 0;
			foreach (int lore in lores)
			{
				this.LearnedLore.m_snoLoreLearned[num] = lore;
				num++;
			}
			this.LearnedLore.Count = lores.Count();

			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this.Attributes.BroadcastChangedIfRevealed();
		}

		#region Attribute Setters
		public void SetAllStatsInCorrectOrder()
		{
			SetAttributesSkills();
			SetAttributesBuffs();
			SetAttributesDamage();
			SetAttributesRessources();
			SetAttributesClassSpecific();
			SetAttributesMovement();
			SetAttributesMisc();
			SetAttributesOther();
			if (this.Inventory == null)
				this.Inventory = new Inventory(this);
			SetAttributesByItems();//needs the Inventory
			SetAttributesByItemProcs();
			SetAttributesByGems();
			SetAttributesByItemSets();
			if (this.SkillSet == null)
				this.SkillSet = new SkillSet(this, this.Toon.Class, this.Toon);
			SetAttributesByPassives();//needs the SkillSet
			SetAttributesByParagon();
			SetNewAttributes();
			UpdatePercentageHP(PercHPbeforeChange);
		}

		public void SetAttributesSkills()
		{
			//Skills
			this.Attributes[GameAttribute.SkillKit] = Toon.HeroTable.SNOSKillKit0;

			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00033C40] = 153;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00033C40] = 3753;
			Attributes[GameAttribute.Buff_Icon_Count0, 0x0006B48E] = 1;
			Attributes[GameAttribute.Buff_Icon_Count0, 0x0004601B] = 1;
			Attributes[GameAttribute.Buff_Icon_Count0, 0x00033C40] = 1;
			
			Attributes[GameAttribute.Currencies_Discovered] = 0x0011FFF8;

			this.Attributes[GameAttribute.Skill, 30592] = 1;
			this.Attributes[GameAttribute.Resource_Degeneration_Prevented] = false;
			this.Attributes[GameAttribute.Resource_Degeneration_Stop_Point] = 0;
			//scripted //this.Attributes[GameAttribute.Skill_Total, 0x7545] = 1; //Axe Operate Gizmo
			//scripted //this.Attributes[GameAttribute.Skill_Total, 0x76B7] = 1; //Punch!
			//scripted //this.Attributes[GameAttribute.Skill_Total, 0x6DF] = 1; //Use Item
			//scripted //this.Attributes[GameAttribute.Skill_Total, 0x7780] = 1; //Basic Attack
			//scripted //this.Attributes[GameAttribute.Skill_Total, 0xFFFFF] = 1;
			//this.Attributes[GameAttribute.Skill, 0xFFFFF] = 1;
		}

		public void SetAttributesBuffs()
		{
			//Buffs
			this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x33C40] = true;
			this.Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00033C40] = 0x000003FB;
			this.Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00033C40] = 0x00000077;
			this.Attributes[GameAttribute.Buff_Icon_Count0, 0x00033C40] = 1;
			this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0xCE11] = true;
			this.Attributes[GameAttribute.Buff_Icon_Count0, 0x0000CE11] = 1;
			this.Attributes[GameAttribute.Buff_Visual_Effect, 0xFFFFF] = true;
			//Wings
			if (this.CurrentWingsPowerId != -1)
			{
				this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, this.CurrentWingsPowerId] = true;
				this.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, this.CurrentWingsPowerId] = true;
				this.Attributes[GameAttribute.Buff_Icon_Start_Tick0, this.CurrentWingsPowerId] = 0;
				this.Attributes[GameAttribute.Buff_Icon_End_Tick0, this.CurrentWingsPowerId] = 100;
				this.Attributes[GameAttribute.Buff_Icon_Count0, this.CurrentWingsPowerId] = 1;
			}
		}

		public void SetAttributesDamage()
		{
			this.Attributes[GameAttribute.Primary_Damage_Attribute] = (int)Toon.HeroTable.CoreAttribute + 1;
			this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap] = 4f;
		}

		public void SetAttributesRessources()
		{
			this.Attributes[GameAttribute.Resource_Type_Primary] = (int)Toon.HeroTable.PrimaryResource + 1;
			this.Attributes[GameAttribute.Resource_Max, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = Toon.HeroTable.PrimaryResourceBase;
			this.Attributes[GameAttribute.Resource_Max_Bonus, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = 0;
			this.Attributes[GameAttribute.Resource_Factor_Level, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = Toon.HeroTable.PrimaryResourceFactorLevel;
			this.Attributes[GameAttribute.Resource_Percent, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = 0;
			this.Attributes[GameAttribute.Resource_Cur, (int)this.Attributes[GameAttribute.Resource_Type_Primary]] = GetMaxResource((int)this.Attributes[GameAttribute.Resource_Type_Primary] - 1);


			var max = this.Attributes[GameAttribute.Resource_Max, (int)this.Attributes[GameAttribute.Resource_Type_Primary] - 1];
			var cur = this.Attributes[GameAttribute.Resource_Cur, (int)this.Attributes[GameAttribute.Resource_Type_Primary] - 1];


			this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = Toon.HeroTable.PrimaryResourceRegen;
			this.Attributes[GameAttribute.Resource_Regen_Stop_Regen] = false;
			if (this.Toon.Class == ToonClass.Barbarian)
				this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] = 0;
			else
				this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] = (int)GetMaxResource((int)Toon.HeroTable.PrimaryResource + 1) * 100;

			if (Toon.HeroTable.SecondaryResource != DiIiS_NA.Core.MPQ.FileFormats.GameBalance.HeroTable.Resource.None)
			{
				this.Attributes[GameAttribute.Resource_Type_Secondary] = (int)Toon.HeroTable.SecondaryResource + 1;
				this.Attributes[GameAttribute.Resource_Max, (int)this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = Toon.HeroTable.SecondaryResourceBase;
				this.Attributes[GameAttribute.Resource_Max_Bonus, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = 0;
				this.Attributes[GameAttribute.Resource_Percent, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = 0;
				this.Attributes[GameAttribute.Resource_Factor_Level, (int)this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = Toon.HeroTable.SecondaryResourceFactorLevel;
				this.Attributes[GameAttribute.Resource_Cur, (int)this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = GetMaxResource((int)Toon.HeroTable.SecondaryResource + 1);
				this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = Toon.HeroTable.SecondaryResourceRegen;
			}

			this.Attributes[GameAttribute.Get_Hit_Recovery_Per_Level] = (int)Toon.HeroTable.GetHitRecoveryPerLevel;
			this.Attributes[GameAttribute.Get_Hit_Recovery_Base] = (int)Toon.HeroTable.GetHitRecoveryBase;

			this.Attributes[GameAttribute.Get_Hit_Max_Per_Level] = (int)Toon.HeroTable.GetHitMaxPerLevel;
			this.Attributes[GameAttribute.Get_Hit_Max_Base] = (int)Toon.HeroTable.GetHitMaxBase;
		}

		public void SetAttributesClassSpecific()
		{
			// Class specific
			switch (this.Toon.Class)
			{
				case ToonClass.Barbarian:
					//scripted //this.Attributes[GameAttribute.Skill_Total, 30078] = 1;  //Fury Trait
					this.Attributes[GameAttribute.Skill, 30078] = 1;
					this.Attributes[GameAttribute.Trait, 30078] = 1;
					this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 30078] = true;
					this.Attributes[GameAttribute.Buff_Icon_Count0, 30078] = 1;
					break;
				case ToonClass.DemonHunter:
					break;
				case ToonClass.Crusader:
					this.Attributes[GameAttribute.Skill, 0x000418F2] = 1;
					this.Attributes[GameAttribute.Skill, 0x00045CCF] = 1;
					this.Attributes[GameAttribute.Skill, 0x000564D4] = 1;

					break;
				case ToonClass.Monk:
					//scripted //this.Attributes[GameAttribute.Skill_Total, 0x0000CE11] = 1;  //Spirit Trait
					this.Attributes[GameAttribute.Skill, 0x0000CE11] = 1;
					this.Attributes[GameAttribute.Trait, 0x0000CE11] = 1;
					this.Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0xCE11] = true;
					this.Attributes[GameAttribute.Buff_Icon_Count0, 0x0000CE11] = 1;
					break;
				case ToonClass.WitchDoctor:
					break;
				case ToonClass.Wizard:
					break;
			}
		}

		public void SetAttributesMovement()
		{
			this.Attributes[GameAttribute.Movement_Scalar_Cap] = 3f;
			this.Attributes[GameAttribute.Movement_Scalar] = 1f;
			this.Attributes[GameAttribute.Walking_Rate] = Toon.HeroTable.WalkingRate;
			this.Attributes[GameAttribute.Running_Rate] = Toon.HeroTable.RunningRate;
			this.Attributes[GameAttribute.Experience_Bonus] = 0f;
			this.Attributes[GameAttribute.Sprinting_Rate] = Toon.HeroTable.SprintRate * 2;
			this.Attributes[GameAttribute.Strafing_Rate] = Toon.HeroTable.SprintRate * 2;
		}

		public void SetAttributesMisc()
		{
			//Miscellaneous
			/*
			this.Attributes[GameAttribute.Disabled] = false; // we should be making use of these ones too /raist.
			this.Attributes[GameAttribute.Loading] = true;
			this.Attributes[GameAttribute.Loading_Player_ACD] = this.PlayerIndex;
			this.Attributes[GameAttribute.Invulnerable] = true;
			this.Attributes[GameAttribute.Hidden] = false;
			this.Attributes[GameAttribute.Immobolize] = true;
			this.Attributes[GameAttribute.Untargetable] = true;
			this.Attributes[GameAttribute.CantStartDisplayedPowers] = true;
			this.Attributes[GameAttribute.IsContentRestrictedActor] = true;
			this.Attributes[GameAttribute.Cannot_Dodge] = false;
			this.Attributes[GameAttribute.Trait, 0x0000CE11] = 1;
			this.Attributes[GameAttribute.Backpack_Slots] = 60;
			this.Attributes[GameAttribute.General_Cooldown] = 0;
			//*/
			this.Attributes[GameAttribute.Disabled] = true; // we should be making use of these ones too /raist.
			this.Attributes[GameAttribute.Loading] = true;
			this.Attributes[GameAttribute.Loading_Player_ACD] = this.PlayerIndex;
			this.Attributes[GameAttribute.Invulnerable] = true;
			this.Attributes[GameAttribute.Hidden] = false;
			this.Attributes[GameAttribute.Immobolize] = true;
			this.Attributes[GameAttribute.Untargetable] = true;
			this.Attributes[GameAttribute.CantStartDisplayedPowers] = true;
			this.Attributes[GameAttribute.IsContentRestrictedActor] = true;
			this.Attributes[GameAttribute.Cannot_Dodge] = false;
			this.Attributes[GameAttribute.Trait, 0x0000CE11] = 1;
			this.Attributes[GameAttribute.TeamID] = 2;
			this.Attributes[GameAttribute.Stash_Tabs_Purchased_With_Gold] = 5;			// what do these do?
			this.Attributes[GameAttribute.Stash_Tabs_Rewarded_By_Achievements] = 5;
			this.Attributes[GameAttribute.Backpack_Slots] = 60;
			this.Attributes[GameAttribute.General_Cooldown] = 0;
		}

		public void SetAttributesByParagon()
		{
			this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, 0] = 0;
			this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, 1] = 0;
			this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, 2] = 0;
			this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, 3] = 0;
			int level = Math.Min(this.Toon.ParagonLevel, 800);
			for (int i = 0; i < level; i++)
			{
				this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, (i % 4)]++;
			}
			var assigned_bonuses = this.ParagonBonuses;
			var bonus_ids = ItemGenerator.GetParagonBonusTable(this.Toon.Class);
			foreach (var bonus in bonus_ids)
			{
				int slot_index = (bonus.Category * 4) + bonus.Index - 1;
				this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, bonus.Category] -= assigned_bonuses[slot_index];
				this.Attributes[GameAttribute.Paragon_Bonus, bonus.Hash] = assigned_bonuses[slot_index];

				float result;
				if (FormulaScript.Evaluate(bonus.AttributeSpecifiers[0].Formula.ToArray(), new ItemRandomHelper(0), out result))
				{
					if (bonus.AttributeSpecifiers[0].AttributeId == 104) //Resistance_All
					{
						foreach (var damageType in DamageType.AllTypes)
						{
							this.Attributes[GameAttribute.Resistance, damageType.AttributeKey] += (result * assigned_bonuses[slot_index]);
						}
					}
					else
					{
						if (bonus.AttributeSpecifiers[0].AttributeId == 133) result = 33f; //Hitpoints_Regen_Per_Second
						if (bonus.AttributeSpecifiers[0].AttributeId == 342) result = 16.5f; //Hitpoints_On_Hit
						if (GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] is GameAttributeF)
						{
							var attr = GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] as GameAttributeF;
							if (bonus.AttributeSpecifiers[0].SNOParam != -1)
								this.Attributes[attr, bonus.AttributeSpecifiers[0].SNOParam] += (result * assigned_bonuses[slot_index]);
							else
								this.Attributes[attr] += (result * assigned_bonuses[slot_index]);
						}
						else if (GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] is GameAttributeI)
						{
							var attr = GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] as GameAttributeI;
							if (bonus.AttributeSpecifiers[0].SNOParam != -1)
								this.Attributes[attr, bonus.AttributeSpecifiers[0].SNOParam] += (int)(result * assigned_bonuses[slot_index]);
							else
								this.Attributes[attr] += (int)(result * assigned_bonuses[slot_index]);
						}
					}
				}
				//Logger.Debug("Paragon attribute {0} - value {1}", bonus.Attribute[0].AttributeId, (result * assigned_bonuses[slot_index]));
			}
		}
		public float PercHPbeforeChange = 0;
		public void SetAttributesByItems()
		{
			const float nonPhysDefault = 0f; //was 3.051758E-05f
			var MaxHPOld = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			var PercentOfOld = this.Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
			PercHPbeforeChange = this.Attributes[GameAttribute.Hitpoints_Cur] / (this.Attributes[GameAttribute.Hitpoints_Max_Total] / 100);
			;

			var damageAttributeMinValues = new Dictionary<DamageType, float[]>
			{
				{DamageType.Physical, new[] {2f, 2f}},
				{DamageType.Arcane, new[] {nonPhysDefault, nonPhysDefault}},
				{DamageType.Cold, new[] {nonPhysDefault, nonPhysDefault}},
				{DamageType.Fire, new[] {nonPhysDefault, nonPhysDefault}},
				{DamageType.Holy, new[] {nonPhysDefault, nonPhysDefault}},
				{DamageType.Lightning, new[] {nonPhysDefault, nonPhysDefault}},
				{DamageType.Poison, new[] {nonPhysDefault, nonPhysDefault}}
			};


			foreach (var damageType in DamageType.AllTypes)
			{
				//var weaponDamageMin = damageType.AttributeKey == 0 ? this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) : (this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) + Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, damageType.AttributeKey), damageAttributeMinValues[damageType][0]));
				//var weaponDamageDelta = damageType.AttributeKey == 0 ? this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) : (this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta, 0) + Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey), damageAttributeMinValues[damageType][1]));

				this.Attributes[GameAttribute.Damage_Weapon_Min, damageType.AttributeKey] = Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min_Total, damageType.AttributeKey), damageAttributeMinValues[damageType][0]) + this.Inventory.GetItemBonus(GameAttribute.Damage_Min, damageType.AttributeKey);
				this.Attributes[GameAttribute.Damage_Weapon_Min, damageType.AttributeKey] -= this.Inventory.AdjustDualWieldMin(damageType); //Damage on weapons should not add when dual-wielding
				this.Attributes[GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey] = Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, damageType.AttributeKey), damageAttributeMinValues[damageType][1]) + this.Inventory.GetItemBonus(GameAttribute.Damage_Delta, damageType.AttributeKey);
				this.Attributes[GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey] -= this.Inventory.AdjustDualWieldDelta(damageType); //Damage on weapons should not add when dual-wielding

				this.Attributes[GameAttribute.Damage_Weapon_Bonus_Min, damageType.AttributeKey] = 0f;
				this.Attributes[GameAttribute.Damage_Weapon_Bonus_Min_X1, damageType.AttributeKey] = 0f;
				this.Attributes[GameAttribute.Damage_Weapon_Bonus_Delta, damageType.AttributeKey] = 0f;
				this.Attributes[GameAttribute.Damage_Weapon_Bonus_Delta_X1, damageType.AttributeKey] = 0f;
				this.Attributes[GameAttribute.Damage_Weapon_Bonus_Flat, damageType.AttributeKey] = 0f;

				this.Attributes[GameAttribute.Damage_Type_Percent_Bonus, damageType.AttributeKey] = this.Inventory.GetItemBonus(GameAttribute.Damage_Type_Percent_Bonus, damageType.AttributeKey);
				this.Attributes[GameAttribute.Damage_Dealt_Percent_Bonus, damageType.AttributeKey] = this.Inventory.GetItemBonus(GameAttribute.Damage_Dealt_Percent_Bonus, damageType.AttributeKey);

				this.Attributes[GameAttribute.Resistance, damageType.AttributeKey] = this.Inventory.GetItemBonus(GameAttribute.Resistance, damageType.AttributeKey);
				this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, damageType.AttributeKey] = this.Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Type, damageType.AttributeKey);
				this.Attributes[GameAttribute.Amplify_Damage_Type_Percent, damageType.AttributeKey] = this.Inventory.GetItemBonus(GameAttribute.Amplify_Damage_Type_Percent, damageType.AttributeKey);
			}

			for (int i = 0; i < 4; i++)
				this.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, i] = this.Inventory.GetItemBonus(GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, i);


			this.Attributes[GameAttribute.Resistance_All] = this.Inventory.GetItemBonus(GameAttribute.Resistance_All);
			this.Attributes[GameAttribute.Resistance_Percent_All] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Percent_All);
			this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] = this.Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Melee);
			this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] = this.Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Ranged);

			this.Attributes[GameAttribute.Thorns_Fixed, 0] = this.Inventory.GetItemBonus(GameAttribute.Thorns_Fixed, 0);

			//this.Attributes[GameAttribute.Armor_Item_Percent] = this.Inventory.GetItemBonus(GameAttribute.Armor_Item_Percent);
			float allStatsBonus = this.Inventory.GetItemBonus(GameAttribute.Stats_All_Bonus);// / 1065353216;
			/*
			this.Attributes[GameAttribute.Armor_Item] = this.Inventory.GetItemBonus(GameAttribute.Armor_Item_Total);
			this.Attributes[GameAttribute.Strength_Item] = this.Inventory.GetItemBonus(GameAttribute.Dexterity_Item);// / 1065353216;
			this.Attributes[GameAttribute.Strength_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Dexterity_Item] = this.Inventory.GetItemBonus(GameAttribute.Intelligence_Item);// / 1065353216;
			this.Attributes[GameAttribute.Dexterity_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Intelligence_Item] = this.Inventory.GetItemBonus(GameAttribute.Vitality_Item);// / 1065353216; //I know that's wild, but client can't display it properly...
			this.Attributes[GameAttribute.Intelligence_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Vitality_Item] = this.Inventory.GetItemBonus(GameAttribute.Item_Level_Requirement_Reduction);// / 1065353216;
			this.Attributes[GameAttribute.Vitality_Item] += allStatsBonus;
			//*/
			//*
			this.Attributes[GameAttribute.Strength_Item] = this.Inventory.GetItemBonus(GameAttribute.Strength_Item);// / 1065353216;
			this.Attributes[GameAttribute.Strength_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Vitality_Item] = this.Inventory.GetItemBonus(GameAttribute.Vitality_Item);// / 1065353216;
			this.Attributes[GameAttribute.Vitality_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Dexterity_Item] = this.Inventory.GetItemBonus(GameAttribute.Dexterity_Item);// / 1065353216;
			this.Attributes[GameAttribute.Dexterity_Item] += allStatsBonus;
			this.Attributes[GameAttribute.Intelligence_Item] = this.Inventory.GetItemBonus(GameAttribute.Intelligence_Item);// / 1065353216; //I know that's wild, but client can't display it properly...
			this.Attributes[GameAttribute.Intelligence_Item] += allStatsBonus;
			//*/

			//this.Attributes[GameAttribute.Cube_Enchanted_Strength_Item] = 0;
			this.Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;


			this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus);
			this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Item] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus_Item);
			this.Attributes[GameAttribute.Hitpoints_Max_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Bonus);



			this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource);

			this.Attributes[GameAttribute.Attacks_Per_Second] = this.Inventory.GetAPS();

			this.Attributes[GameAttribute.Attacks_Per_Second_Percent] = this.Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item_Percent) + this.Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Percent);
			this.Attributes[GameAttribute.Attacks_Per_Second_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item_Bonus);
			this.Attributes[GameAttribute.Attacks_Per_Second_Item] = this.Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item);
			var a = this.Attributes[GameAttribute.Attacks_Per_Second];
			var b = this.Attributes[GameAttribute.Attacks_Per_Second_Percent];
			var c = this.Attributes[GameAttribute.Attacks_Per_Second_Bonus];
			var d = this.Attributes[GameAttribute.Attacks_Per_Second_Item];
			var e = this.Attributes[GameAttribute.Attacks_Per_Second_Item_CurrentHand];
			var f = this.Attributes[GameAttribute.Attacks_Per_Second_Item_Bonus];
			var g = this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Subtotal];
			var h = this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap];
			var j = this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Uncapped];
			var k = this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Reduction];
			var o = this.Attributes[GameAttribute.Attacks_Per_Second_Total];

			if (this.Attributes[GameAttribute.Attacks_Per_Second_Total] < 1)
				this.Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			this.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] = this.Inventory.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Capped);
			this.Attributes[GameAttribute.Weapon_Crit_Chance] = 0.05f + this.Inventory.GetItemBonus(GameAttribute.Weapon_Crit_Chance);
			this.Attributes[GameAttribute.Crit_Damage_Percent] = 0.5f + this.Inventory.GetItemBonus(GameAttribute.Crit_Damage_Percent);

			this.Attributes[GameAttribute.Splash_Damage_Effect_Percent] = this.Inventory.GetItemBonus(GameAttribute.Splash_Damage_Effect_Percent);

			this.Attributes[GameAttribute.On_Hit_Fear_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Fear_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Fear_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Stun_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Stun_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Stun_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Blind_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Blind_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Blind_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Freeze_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Freeze_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Freeze_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Chill_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Chill_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Chill_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Slow_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Slow_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Slow_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Immobilize_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Immobilize_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Immobilize_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Knockback_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Knockback_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Knockback_Proc_Chance);
			this.Attributes[GameAttribute.On_Hit_Bleed_Proc_Chance] = this.Inventory.GetItemBonus(GameAttribute.On_Hit_Bleed_Proc_Chance) + this.Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Bleed_Proc_Chance);

			this.Attributes[GameAttribute.Running_Rate] = Toon.HeroTable.RunningRate + this.Inventory.GetItemBonus(GameAttribute.Running_Rate);
			this.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Movement_Scalar_Uncapped_Bonus);
			this.Attributes[GameAttribute.Movement_Scalar] = this.Inventory.GetItemBonus(GameAttribute.Movement_Scalar) + 1.0f;

			//this.Attributes[GameAttribute.Magic_Find] = this.Inventory.GetItemBonus(GameAttribute.Magic_Find);
			this.Attributes[GameAttribute.Magic_Find] = this.Inventory.GetMagicFind();
			//this.Attributes[GameAttribute.Gold_Find] = this.Inventory.GetItemBonus(GameAttribute.Gold_Find);
			this.Attributes[GameAttribute.Gold_Find] = this.Inventory.GetGoldFind();

			this.Attributes[GameAttribute.Gold_PickUp_Radius] = (5f + this.Inventory.GetItemBonus(GameAttribute.Gold_PickUp_Radius));

			this.Attributes[GameAttribute.Experience_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Experience_Bonus);
			this.Attributes[GameAttribute.Experience_Bonus_Percent] = this.Inventory.GetItemBonus(GameAttribute.Experience_Bonus_Percent);

			this.Attributes[GameAttribute.Resistance_Freeze] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Freeze);
			this.Attributes[GameAttribute.Resistance_Penetration] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Penetration);
			this.Attributes[GameAttribute.Resistance_Percent] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Percent);
			this.Attributes[GameAttribute.Resistance_Root] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Root);
			this.Attributes[GameAttribute.Resistance_Stun] = this.Inventory.GetItemBonus(GameAttribute.Resistance_Stun);
			this.Attributes[GameAttribute.Resistance_StunRootFreeze] = this.Inventory.GetItemBonus(GameAttribute.Resistance_StunRootFreeze);

			this.Attributes[GameAttribute.Dodge_Chance_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Dodge_Chance_Bonus);

			this.Attributes[GameAttribute.Block_Amount_Item_Min] = this.Inventory.GetItemBonus(GameAttribute.Block_Amount_Item_Min);
			this.Attributes[GameAttribute.Block_Amount_Item_Delta] = this.Inventory.GetItemBonus(GameAttribute.Block_Amount_Item_Delta);
			this.Attributes[GameAttribute.Block_Amount_Bonus_Percent] = this.Inventory.GetItemBonus(GameAttribute.Block_Amount_Bonus_Percent);
			this.Attributes[GameAttribute.Block_Chance] = this.Inventory.GetItemBonus(GameAttribute.Block_Chance_Item_Total);

			this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent] = 0;
			this.Attributes[GameAttribute.Health_Globe_Bonus_Health] = this.Inventory.GetItemBonus(GameAttribute.Health_Globe_Bonus_Health);

			this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second) + this.Toon.HeroTable.GetHitRecoveryBase + (this.Toon.HeroTable.GetHitRecoveryPerLevel * this.Level);

			this.Attributes[GameAttribute.Resource_Cost_Reduction_Percent_All] = this.Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent_All);
			this.Attributes[GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.PrimaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.PrimaryResource);
			this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceRegen + this.Inventory.GetItemBonus(GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource);
			this.Attributes[GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.PrimaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.PrimaryResource);

			this.Attributes[GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.SecondaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.SecondaryResource);
			this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.SecondaryResource] = Toon.HeroTable.SecondaryResourceRegen + this.Inventory.GetItemBonus(GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.SecondaryResource);
			this.Attributes[GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.SecondaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.SecondaryResource);

			this.Attributes[GameAttribute.Resource_On_Hit] = 0;
			this.Attributes[GameAttribute.Resource_On_Hit, 0] = this.Inventory.GetItemBonus(GameAttribute.Resource_On_Hit, 0);
			this.Attributes[GameAttribute.Resource_On_Crit, 1] = this.Inventory.GetItemBonus(GameAttribute.Resource_On_Crit, 1);

			this.Attributes[GameAttribute.Steal_Health_Percent] = this.Inventory.GetItemBonus(GameAttribute.Steal_Health_Percent) * 0.1f;
			this.Attributes[GameAttribute.Hitpoints_On_Hit] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_On_Hit);
			this.Attributes[GameAttribute.Hitpoints_On_Kill] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_On_Kill);
			this.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] = this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Percent_Bonus);
			this.Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Elites] = this.Inventory.GetItemBonus(GameAttribute.Damage_Percent_Bonus_Vs_Elites);
			//this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All_Capped] = 0.5f;
			//this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_Cap] = 0.5f;
			this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent] = 0.5f;
			this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction_Percent_All);
			this.Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] = this.Inventory.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Uncapped);

			//this.Attributes[GameAttribute.Projectile_Speed] = 0.3f;

			switch (this.Toon.Class)
			{
				case ToonClass.Barbarian:
					this.Attributes[GameAttribute.Power_Resource_Reduction, 80028] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 80028);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 70472] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 70472);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 79242] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 79242);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 80263] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 80263);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 78548] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 78548);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 93885] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 93885);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 86989] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 86989);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 96296] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 96296);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 109342] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 109342);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 159169] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 159169);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 93885] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 93885);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69979] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69979);
					break;
				case ToonClass.DemonHunter:
					this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.SecondaryResource] = this.Inventory.GetItemBonus(GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.SecondaryResource);
					this.Attributes[GameAttribute.Bow] = this.Inventory.GetItemBonus(GameAttribute.Bow);
					this.Attributes[GameAttribute.Crossbow] = this.Inventory.GetItemBonus(GameAttribute.Crossbow);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 129215] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 129215);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 134209] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 134209);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 77552] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 77552);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 75873] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 75873);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 86610] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 86610);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 131192] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 131192);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 131325] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 131325);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 77649] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 77649);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 134030] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 134030);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 129214] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 129214);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 75301] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 75301);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 131366] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 131366);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 129213] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 129213);
					break;
				case ToonClass.Monk:
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 95940] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 95940);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96019] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96019);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96311] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96311);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 97328] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 97328);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96090] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96090);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 97110] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 97110);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 121442] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 121442);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 111676] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 111676);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 223473] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 223473);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 96033] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 96033);
					break;
				case ToonClass.WitchDoctor:
					this.Attributes[GameAttribute.Power_Resource_Reduction, 105963] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 105963);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 103181] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 103181);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 106465] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 106465);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 83602] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 83602);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 108506] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 108506);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69866] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69866);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69867] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69867);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 74003] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 74003);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 70455] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 103181);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 67567] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 67567);
					this.Attributes[GameAttribute.Power_Cooldown_Reduction, 134837] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 134837);
					this.Attributes[GameAttribute.Power_Cooldown_Reduction, 67600] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 67600);
					this.Attributes[GameAttribute.Power_Cooldown_Reduction, 102573] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 102573);
					this.Attributes[GameAttribute.Power_Cooldown_Reduction, 30624] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 30624);
					break;
				case ToonClass.Wizard:
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 30744] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 30744);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 30783] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 30783);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 71548] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 71548);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 1765] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 1765);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 30668] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 30668);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 77113] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 77113);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 91549] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 91549);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 87525] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 87525);
					this.Attributes[GameAttribute.Power_Crit_Percent_Bonus, 93395] = this.Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 93395);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 134456] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 134456);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 30725] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 30725);
					this.Attributes[GameAttribute.Power_Duration_Increase, 30680] = this.Inventory.GetItemBonus(GameAttribute.Power_Duration_Increase, 30680);
					this.Attributes[GameAttribute.Power_Resource_Reduction, 69190] = this.Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 69190);
					this.Attributes[GameAttribute.Power_Cooldown_Reduction, 168344] = this.Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 168344);
					this.Attributes[GameAttribute.Power_Damage_Percent_Bonus, 71548] = this.Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 71548);
					break;
			}

		}

		public void UpdatePercentageHP(float percent)
		{
			var m = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this.Attributes[GameAttribute.Hitpoints_Cur] = percent * this.Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
			this.Attributes.BroadcastChangedIfRevealed();
		}
		public void UpdatePercentageHP()
		{
		}

		public void SetAttributesByGems()
		{
			this.Inventory.SetGemBonuses();
		}

		public void SetAttributesByItemProcs()
		{
			this.Attributes[GameAttribute.Item_Power_Passive, 248776] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 248776); //cluck
			this.Attributes[GameAttribute.Item_Power_Passive, 248629] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 248629); //death laugh
			this.Attributes[GameAttribute.Item_Power_Passive, 247640] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 247640); //gore1
			this.Attributes[GameAttribute.Item_Power_Passive, 249963] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 249963); //gore2
			this.Attributes[GameAttribute.Item_Power_Passive, 249954] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 249954); //gore3
			this.Attributes[GameAttribute.Item_Power_Passive, 246116] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 246116); //butcher
			this.Attributes[GameAttribute.Item_Power_Passive, 247724] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 247724); //plum!
			this.Attributes[GameAttribute.Item_Power_Passive, 245741] = this.Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 245741); //weee!
		}

		public void SetAttributesByItemSets()
		{
			this.Attributes[GameAttribute.Strength] = this.Strength;
			this.Attributes[GameAttribute.Dexterity] = this.Dexterity;
			this.Attributes[GameAttribute.Vitality] = this.Vitality;
			this.Attributes[GameAttribute.Intelligence] = this.Intelligence;
			this.Attributes.BroadcastChangedIfRevealed();

			this.Inventory.SetItemSetBonuses();
		}

		public void SetAttributesByPassives()       //also reapplies synergy buffs
		{
			// Class specific
			this.Attributes[GameAttribute.Damage_Percent_All_From_Skills] = 0;
			this.Attributes[GameAttribute.Allow_2H_And_Shield] = false;
			this.Attributes[GameAttribute.Cannot_Dodge] = false;

			foreach (int passiveId in this.SkillSet.PassiveSkills)
				switch (this.Toon.Class)
				{
					case ToonClass.Barbarian:
						switch (passiveId)
						{
							case 217819: //NervesOfSteel
								this.Attributes[GameAttribute.Armor_Item] += this.Attributes[GameAttribute.Vitality_Total] * 0.50f;
								break;
							case 205228: //Animosity
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 20;
								this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] = this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] * 1.1f;
								this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceBase + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
								break;
							case 205848: //ToughAsNails
								this.Attributes[GameAttribute.Armor_Item] *= 1.25f;
								break;
							case 205707: //Juggernaut
								this.Attributes[GameAttribute.CrowdControl_Reduction] += 0.3f;
								break;
							case 206147: //WeaponsMaster
								var weapon = this.Inventory.GetEquippedWeapon();
								if (weapon != null)
								{
									string name = weapon.ItemDefinition.Name.ToLower();
									if (name.Contains("sword") || name.Contains("dagger"))
										this.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.08f;
									else
										if (name.Contains("axe") || name.Contains("mace"))
										this.Attributes[GameAttribute.Weapon_Crit_Chance] += 0.05f;
									else
											if (name.Contains("spear") || name.Contains("polearm"))
										this.Attributes[GameAttribute.Attacks_Per_Second] *= 1.08f;
									else
												if (name.Contains("mighty"))
										this.Attributes[GameAttribute.Resource_On_Hit] += 1f;
								}
								break;
						}
						break;
					case ToonClass.DemonHunter:
						switch (passiveId)
						{
							case 155714: //Blood Vengeance
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 25;
								this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceBase + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
								break;
							case 210801: //Brooding
								this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += (this.Attributes[GameAttribute.Hitpoints_Max_Total]) / 100;
								break;
							case 155715: //Sharpshooter
								this.World.BuffManager.RemoveBuffs(this, 155715);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.SharpshooterBuff());
								break;
							case 324770: //Awareness
								this.World.BuffManager.RemoveBuffs(this, 324770);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.AwarenessBuff());
								break;
							case 209734: //Archery
								var weapon = this.Inventory.GetEquippedWeapon();
								if (weapon != null)
								{
									string name = weapon.ItemDefinition.Name.ToLower();
									if (name.Contains("xbow"))
										this.Attributes[GameAttribute.Crit_Damage_Percent] += 0.5f;
									if (name.Contains("handxbow"))
										this.Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] += 0.05f;
									else
										if (name.Contains("xbow"))
										this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] += 1f;
									else
											if (name.Contains("bow"))
									{
										this.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.08f;
										this.Attributes[GameAttribute.Damage_Percent_All_From_Skills] = 0.08f;
									}
								}
								break;
							case 155722: //Perfectionist
								this.Attributes[GameAttribute.Armor_Item] *= 1.1f;
								this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] += 0.1f;
								this.Attributes[GameAttribute.Resistance_Percent_All] += 0.1f;
								break;
						}
						break;
					case ToonClass.Monk:
						switch (passiveId)
						{
							case 209029: //FleetFooted
								this.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.1f;
								break;
							case 209027: //ExaltedSoul
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 100;
								//this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceMax + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
								this.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;
								break;
							case 209628: //SeizeTheInitiative
								this.Attributes[GameAttribute.Armor_Item] += (this.Attributes[GameAttribute.Dexterity_Total] * 0.3f);
								break;
							case 209622: //SixthSense
								this.Attributes[GameAttribute.Dodge_Chance_Bonus] += Math.Min(((this.Attributes[GameAttribute.Weapon_Crit_Chance] + this.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] + this.Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped]) * 0.425f), 0.15f);
								break;
							case 209104: //BeaconOfYtar
								this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] += 0.20f;
								break;
							case 209656: //OneWithEverything
								var maxResist = Math.Max(
									Math.Max(Math.Max(this.Attributes[GameAttribute.Resistance, DamageType.Physical.AttributeKey], this.Attributes[GameAttribute.Resistance, DamageType.Cold.AttributeKey]), this.Attributes[GameAttribute.Resistance, DamageType.Fire.AttributeKey]),
									Math.Max(Math.Max(this.Attributes[GameAttribute.Resistance, DamageType.Arcane.AttributeKey], this.Attributes[GameAttribute.Resistance, DamageType.Holy.AttributeKey]), Math.Max(this.Attributes[GameAttribute.Resistance, DamageType.Lightning.AttributeKey], this.Attributes[GameAttribute.Resistance, DamageType.Poison.AttributeKey]))
								);
								foreach (var damageType in DamageType.AllTypes)
									this.Attributes[GameAttribute.Resistance, damageType.AttributeKey] = maxResist;
								break;
							case 209812: //TheGuardiansPath
								try
								{
									var weapon = this.Inventory.GetEquippedWeapon();
									if (weapon != null && this.Inventory.GetEquippedOffHand() != null)
										this.Attributes[GameAttribute.Dodge_Chance_Bonus] += 0.15f;
									else
										if (weapon.ItemDefinition.Name.ToLower().Contains("2h"))
									{
										this.World.BuffManager.RemoveBuffs(this, 209812);
										this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.GuardiansPathBuff());
									}
								}
								catch { }
								break;
							case 341559: //Momentum
								this.World.BuffManager.RemoveBuffs(this, 341559);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.MomentumCheckBuff());
								break;
							case 209813: //Provocation
								this.Attributes[GameAttribute.CrowdControl_Reduction] += 0.25f;
								break;
						}
						break;
					case ToonClass.WitchDoctor:
						switch (passiveId)
						{
							case 208569: //SpiritualAttunement
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += this.Attributes[GameAttribute.Resource_Max, (int)Toon.HeroTable.PrimaryResource] * 0.2f;
								this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceRegen + ((Toon.HeroTable.PrimaryResourceBase + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource]) / 100);
								this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceBase + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
								break;
							case 340910: //PhysicalAttunement
								this.World.BuffManager.RemoveBuffs(this, 340910);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.PhysicalAttunementBuff());
								break;
							case 208568: //BloodRitual
								this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += (this.Attributes[GameAttribute.Hitpoints_Max_Total]) / 100;
								break;
							case 208639: //FierceLoyalty
								foreach (var minionId in this.Followers.Keys)
								{
									var minion = this.World.GetActorByGlobalId(minionId);
									if (minion != null)
										minion.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = this.Inventory.GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second);
								}
								break;
						}
						break;
					case ToonClass.Wizard:
						switch (passiveId)
						{
							case 208541: //Galvanizing Ward
								this.World.BuffManager.RemoveBuffs(this, 208541);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.GalvanizingBuff());
								break;
							case 208473: //Evocation
								this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] += 0.20f;
								break;
							case 208472: //AstralPresence
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 20;
								this.Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceRegen + 2;
								this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceBase + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
								break;
							case 208468: //Blur (Wizard)
								this.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] += 0.17f;
								break;
						}
						break;
					case ToonClass.Crusader:
						switch (passiveId)
						{
							case 286177: //HeavenlyStrength
								this.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.15f;
								this.Attributes[GameAttribute.Allow_2H_And_Shield] = true;
								break;
							case 310626: //Vigilant
								this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += (10 + 0.008f * (float)Math.Pow(this.Attributes[GameAttribute.Level], 3));
								break;
							case 356147: //Righteousness
								this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 30;
								break;
							case 310804: //HolyCause
								this.Attributes[GameAttribute.Damage_Weapon_Min, 6] *= 1.1f;
								break;
							case 356176: //DivineFortress
								this.World.BuffManager.RemoveBuffs(this, 356176);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.DivineFortressBuff());
								break;
							case 302500: //HoldYourGround
								this.Attributes[GameAttribute.Cannot_Dodge] = true;
								this.Attributes[GameAttribute.Block_Chance] += 0.15f;
								break;
							case 310783: //IronMaiden
								this.Attributes[GameAttribute.Thorns_Fixed, 0] += (87.17f * this.Attributes[GameAttribute.Level]);
								break;
							case 311629: //Finery
								this.World.BuffManager.RemoveBuffs(this, 311629);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.FineryBuff());
								break;
							case 310640: //Insurmountable
								this.World.BuffManager.RemoveBuffs(this, 310640);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.InsurmountableBuff());
								break;
							case 309830: //Indesctructible
								this.World.BuffManager.RemoveBuffs(this, 309830);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.IndestructibleBuff());
								break;
							case 356173: //Renewal
								this.World.BuffManager.RemoveBuffs(this, 356173);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.RenewalBuff());
								break;
							case 356052: //Towering Shield
								this.World.BuffManager.RemoveBuffs(this, 356052);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.ToweringShieldBuff());
								break;
						}
						break;
					case ToonClass.Necromancer:
						switch (passiveId)
						{
							case 470764: //HugeEssense
								this.Attributes[GameAttribute.Resource_Max_Bonus, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] += 40;
								break;
							case 470725:
								this.World.BuffManager.RemoveBuffs(this, 470725);
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.OnlyOne());
								break;
						}

						break;
				}

			SetAttributesSkillSets();       //reapply synergy passives (laws, mantras, summons)		
		}

		public void SetAttributesSkillSets()
		{
			// unlocking assigned skills
			for (int i = 0; i < this.SkillSet.ActiveSkills.Length; i++)
			{
				if (this.SkillSet.ActiveSkills[i].snoSkill != -1)
				{
					this.Attributes[GameAttribute.Skill, this.SkillSet.ActiveSkills[i].snoSkill] = 1;
					//scripted //this.Attributes[GameAttribute.Skill_Total, this.SkillSet.ActiveSkills[i].snoSkill] = 1;
					// update rune attributes for new skill
					this.Attributes[GameAttribute.Rune_A, this.SkillSet.ActiveSkills[i].snoSkill] = this.SkillSet.ActiveSkills[i].snoRune == 0 ? 1 : 0;
					this.Attributes[GameAttribute.Rune_B, this.SkillSet.ActiveSkills[i].snoSkill] = this.SkillSet.ActiveSkills[i].snoRune == 1 ? 1 : 0;
					this.Attributes[GameAttribute.Rune_C, this.SkillSet.ActiveSkills[i].snoSkill] = this.SkillSet.ActiveSkills[i].snoRune == 2 ? 1 : 0;
					this.Attributes[GameAttribute.Rune_D, this.SkillSet.ActiveSkills[i].snoSkill] = this.SkillSet.ActiveSkills[i].snoRune == 3 ? 1 : 0;
					this.Attributes[GameAttribute.Rune_E, this.SkillSet.ActiveSkills[i].snoSkill] = this.SkillSet.ActiveSkills[i].snoRune == 4 ? 1 : 0;

					PowerScript power = PowerLoader.CreateImplementationForPowerSNO(this.SkillSet.ActiveSkills[i].snoSkill);
					if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
					{
						this.World.PowerManager.RunPower(this, power.EvalTag(PowerKeys.SynergyPower)); //SynergyPower buff
					}
				}
			}
			for (int i = 0; i < this.SkillSet.PassiveSkills.Length; ++i)
			{
				if (this.SkillSet.PassiveSkills[i] != -1)
				{
					// switch on passive skill
					this.Attributes[GameAttribute.Trait, this.SkillSet.PassiveSkills[i]] = 1;
					this.Attributes[GameAttribute.Skill, this.SkillSet.PassiveSkills[i]] = 1;
					//scripted //this.Attributes[GameAttribute.Skill_Total, this.SkillSet.PassiveSkills[i]] = 1;
				}
			}
			if (this.Toon.Class == ToonClass.Monk)      //Setting power range override
			{
				this.Attributes[GameAttribute.PowerBonusAttackRadius, 0x000176C4] = 20f;     //Fists of Thunder
				if (this.Attributes[GameAttribute.Rune_A, 0x00017B56] > 0)      //Way of the Hundred Fists -> Fists of Fury
					this.Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017B56] = 15f;
			}
		}

		public void SetAttributesOther()
		{
			//Bonus stats
			this.Attributes[GameAttribute.Hit_Chance] = 1f;

			this.Attributes[GameAttribute.Attacks_Per_Second] = 1.2f;
			//this.Attributes[GameAttribute.Attacks_Per_Second_Item] = 1.199219f;
			this.Attributes[GameAttribute.Crit_Percent_Cap] = Toon.HeroTable.CritPercentCap;
			//scripted //this.Attributes[GameAttribute.Casting_Speed_Total] = 1f;
			this.Attributes[GameAttribute.Casting_Speed] = 1f;

			//Basic stats
			this.Attributes[GameAttribute.Level_Cap] = Program.MaxLevel;
			this.Attributes[GameAttribute.Level] = this.Level;
			this.Attributes[GameAttribute.Alt_Level] = this.ParagonLevel;
			if (this.Level == Program.MaxLevel)
			{
				this.Attributes[GameAttribute.Alt_Experience_Next_Lo] = (int)(this.ExperienceNext % UInt32.MaxValue);
				this.Attributes[GameAttribute.Alt_Experience_Next_Hi] = (int)(this.ExperienceNext / UInt32.MaxValue);
			}
			else
			{
				this.Attributes[GameAttribute.Experience_Next_Lo] = (int)(this.ExperienceNext % UInt32.MaxValue);
				this.Attributes[GameAttribute.Experience_Next_Hi] = (int)(this.ExperienceNext / UInt32.MaxValue);
				//this.Attributes[GameAttribute.Alt_Experience_Next] = 0;
			}

			this.Attributes[GameAttribute.Experience_Granted_Low] = 1000;
			this.Attributes[GameAttribute.Armor] = Toon.HeroTable.Armor;
			this.Attributes[GameAttribute.Damage_Min, 0] = Toon.HeroTable.Dmg;
			//scripted //this.Attributes[GameAttribute.Armor_Total]


			this.Attributes[GameAttribute.Strength] = (int)this.Strength;
			this.Attributes[GameAttribute.Dexterity] = (int)this.Dexterity;
			this.Attributes[GameAttribute.Vitality] = (int)this.Vitality;
			this.Attributes[GameAttribute.Intelligence] = (int)this.Intelligence;
			this.Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;

			//Hitpoints have to be calculated after Vitality
			this.Attributes[GameAttribute.Hitpoints_Factor_Level] = Toon.HeroTable.HitpointsFactorLevel;
			this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(this.Level - 35, 0);
			//this.Attributes[GameAttribute.Hitpoints_Max] = 276f;

			this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = (int)1;
			this.Attributes[GameAttribute.Hitpoints_Factor_Level] = (int)Toon.HeroTable.HitpointsFactorLevel;
			this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f;// + Math.Max(this.Level - 35, 0);
			this.Attributes[GameAttribute.Hitpoints_Max] = (int)Toon.HeroTable.HitpointsMax;

			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];

			//TestOutPutItemAttributes(); //Activate this only for finding item stats.
			this.Attributes.BroadcastChangedIfRevealed();

		}

		#endregion

		#region game-message handling & consumers

		/// <summary>
		/// Consumes the given game-message.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="message">The GameMessage.</param>
		public void Consume(GameClient client, GameMessage message)
		{
			if (message is AssignSkillMessage) OnAssignActiveSkill(client, (AssignSkillMessage)message);
			else if (message is AssignTraitsMessage) OnAssignPassiveSkills(client, (AssignTraitsMessage)message);
			else if (message is UnassignSkillMessage) OnUnassignActiveSkill(client, (UnassignSkillMessage)message);
			else if (message is TargetMessage) OnObjectTargeted(client, (TargetMessage)message);
			else if (message is ACDClientTranslateMessage) OnPlayerMovement(client, (ACDClientTranslateMessage)message);
			else if (message is TryWaypointMessage) OnTryWaypoint(client, (TryWaypointMessage)message);
			else if (message is RequestBuyItemMessage) OnRequestBuyItem(client, (RequestBuyItemMessage)message);
			else if (message is RequestSellItemMessage) OnRequestSellItem(client, (RequestSellItemMessage)message);
			else if (message is HirelingRequestLearnSkillMessage) OnHirelingRequestLearnSkill(client, (HirelingRequestLearnSkillMessage)message);
			else if (message is HirelingRetrainMessage) OnHirelingRetrainMessage();
			else if (message is HirelingSwapAgreeMessage) OnHirelingSwapAgreeMessage();
			else if (message is PetAwayMessage) OnHirelingDismiss(client, message as PetAwayMessage);
			else if (message is ChangeUsableItemMessage) OnEquipPotion(client, (ChangeUsableItemMessage)message);
			else if (message is ArtisanWindowClosedMessage) OnArtisanWindowClosed();
			else if (message is RequestTrainArtisanMessage) TrainArtisan(client, (RequestTrainArtisanMessage)message);
			else if (message is RessurectionPlayerMessage) OnResurrectOption(client, (RessurectionPlayerMessage)message);
			else if (message is PlayerTranslateFacingMessage) OnTranslateFacing(client, (PlayerTranslateFacingMessage)message);
			else if (message is LoopingAnimationPowerMessage) OnLoopingAnimationPowerMessage(client, (LoopingAnimationPowerMessage)message);
			else if (message is SecondaryAnimationPowerMessage) OnSecondaryPowerMessage(client, (SecondaryAnimationPowerMessage)message);
			else if (message is MiscPowerMessage) OnMiscPowerMessage(client, (MiscPowerMessage)message);
			else if (message is RequestBuffCancelMessage) OnRequestBuffCancel(client, (RequestBuffCancelMessage)message);
			else if (message is CancelChanneledSkillMessage) OnCancelChanneledSkill(client, (CancelChanneledSkillMessage)message);
			else if (message is TutorialShownMessage) OnTutorialShown(client, (TutorialShownMessage)message);
			else if (message is AcceptConfirmMessage) OnConfirm(client, (AcceptConfirmMessage)message);
			else if (message is SpendParagonPointsMessage) OnSpendParagonPointsMessage(client, (SpendParagonPointsMessage)message);
			else if (message is ResetParagonPointsMessage) OnResetParagonPointsMessage(client, (ResetParagonPointsMessage)message);
			else if (message is MailRetrieveMessage) OnMailRetrieve(client, (MailRetrieveMessage)message);
			else if (message is MailReadMessage) OnMailRead(client, (MailReadMessage)message);
			else if (message is StashIconStateAssignMessage) OnStashIconsAssign(client, (StashIconStateAssignMessage)message);
			else if (message is TransmuteItemsMessage) TransumteItemsPlayer(client, (TransmuteItemsMessage)message);
			else if (message is RiftStartAcceptedMessage) OpenNephalem(client, (RiftStartAcceptedMessage)message);
			else if (message is BossEncounterAcceptMessage) AcceptBossEncounter();
			else if (message is DeActivateCameraCutsceneMode) DeactivateCamera(client, (DeActivateCameraCutsceneMode)message);
			else if (message is JewelUpgradeMessage) JewelUpgrade(client, (JewelUpgradeMessage)message);
			else if (message is SwitchCosmeticMessage) SwitchCosmetic(client, (SwitchCosmeticMessage)message);

			else return;
		}
		public void SwitchCosmetic(GameClient client, SwitchCosmeticMessage message)
		{
			var Definition = ItemGenerator.GetItemDefinition(message.Field0);

			;
			if (Definition.Name.ToLower().Contains("portrait"))
			{
				client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic4 = message.Field0;
			}

			var RangeCosmetic = new[]
			{
				D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic1).Build(), // Wings
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic2).Build(), // Flag
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic3).Build(), // Pet
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic4).Build(), // Frame
            };

			//RangeCosmetic[request.CosmeticItemType - 1] = D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(unchecked((int)request.Gbid)).Build();

			client.BnetClient.Account.GameAccount.CurrentToon.StateChanged();

			var NewVisual = D3.Hero.VisualEquipment.CreateBuilder()
				.AddRangeVisualItem(client.BnetClient.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value.VisualItemList).AddRangeCosmeticItem(RangeCosmetic).Build();

			client.BnetClient.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value = NewVisual;
			client.BnetClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(client.BnetClient.Account.GameAccount.CurrentToon.HeroVisualEquipmentField);
			client.BnetClient.Account.GameAccount.NotifyUpdate();
		}
		public void ResetParagonPoints(GameClient client, ResetParagonPointsMessage message)
		{

		}
		public void SpendParagonPoints(GameClient client, SpendParagonPointsMessage message)
		{

		}
		public void JewelUpgrade(GameClient client, JewelUpgradeMessage message)
		{
			var Jewel = this.Inventory.GetItemByDynId(this, message.ActorID);
			Jewel.Attributes[GameAttribute.Jewel_Rank]++;
			Jewel.Attributes.BroadcastChangedIfRevealed();
			this.Attributes[GameAttribute.Jewel_Upgrades_Used]++;
			this.Attributes.BroadcastChangedIfRevealed();
			if (this.Attributes[GameAttribute.Jewel_Upgrades_Used] == this.Attributes[GameAttribute.Jewel_Upgrades_Max] + this.Attributes[GameAttribute.Jewel_Upgrades_Bonus])
			{
				this.Attributes[GameAttribute.Jewel_Upgrades_Max] = 0;
				this.Attributes[GameAttribute.Jewel_Upgrades_Bonus] = 0;
				this.Attributes[GameAttribute.Jewel_Upgrades_Used] = 0;
			}
			this.InGameClient.SendMessage(new JewelUpgradeResultsMessage()
			{
				ActorID = message.ActorID,
				Field1 = 1
			});
		}
		public void OnHirelingSwapAgreeMessage()
		{
			Hireling hireling = null;
			DiIiS_NA.Core.MPQ.FileFormats.Actor Data = null;
			if (this.World.Game.Players.Count > 1) return;


			switch (this.InGameClient.Game.CurrentQuest)
			{
				case 72061:
					//Templar
					Data = (DiIiS_NA.Core.MPQ.FileFormats.Actor)MPQStorage.Data.Assets[SNOGroup.Actor][52693].Data;
					hireling = new Templar(this.World, 52693, Data.TagMap);
					hireling.GBHandle.GBID = StringHashHelper.HashItemName("Templar");

					break;
				case 72738:
					//Scoundrel
					Data = (DiIiS_NA.Core.MPQ.FileFormats.Actor)MPQStorage.Data.Assets[SNOGroup.Actor][52694].Data;
					hireling = new Templar(this.World, 52694, Data.TagMap);
					hireling.GBHandle.GBID = StringHashHelper.HashItemName("Scoundrel");
					break;
				case 0:
					//Enchantress
					Data = (DiIiS_NA.Core.MPQ.FileFormats.Actor)MPQStorage.Data.Assets[SNOGroup.Actor][4482].Data;
					hireling = new Templar(this.World, 4482, Data.TagMap);
					hireling.GBHandle.GBID = StringHashHelper.HashItemName("Enchantress");
					break;

			}

			hireling.SetUpAttributes(this);
			hireling.GBHandle.Type = 4;
			hireling.Attributes[GameAttribute.Pet_Creator] = this.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Pet_Type] = 1;
			hireling.Attributes[GameAttribute.Pet_Owner] = this.PlayerIndex + 1;
			hireling.Attributes[GameAttribute.Untargetable] = false;
			hireling.Attributes[GameAttribute.NPC_Is_Escorting] = true;

			hireling.EnterWorld(RandomDirection(this.Position, 3, 10)); //Random
			hireling.Brain = new HirelingBrain(hireling, this);
			this.ActiveHireling = hireling;
			this.SelectedNPC = null;
		}

		public static Vector3D RandomDirection(Vector3D position, float minRadius, float maxRadius)
		{
			float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
			float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
			return new Vector3D(position.X + (float)Math.Cos(angle) * radius,
								position.Y + (float)Math.Sin(angle) * radius,
								position.Z);
		}

		public void AwayPet(GameClient client, PetAwayMessage message)
		{

		}
		public void DeactivateCamera(GameClient client, DeActivateCameraCutsceneMode message)
		{
			this.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
			this.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			//this.InGameClient.SendMessage(new ActivateCameraCutsceneMode() { Activate = true });
		}
		public void AcceptBossEncounter()
		{
			this.ArtisanInteraction = "QueueAccepted";
			this.InGameClient.Game.AcceptBossEncounter();
		}
		public void DeclineBossEncounter()
		{
			this.InGameClient.Game.CurrentEncounter.activated = false;
		}
		public void TransumteItemsPlayer(GameClient client, TransmuteItemsMessage message)
		{
			var recipeDefinition = ItemGenerator.GetRecipeDefinition(608170752);
			for (int i = 0; i < message.CurrenciesCount; i++)
			{
				var data = ItemGenerator.GetItemDefinition(message.GBIDCurrencies[i]).Name;
				switch(data)
				{
					case "p2_ActBountyReagent_01": break;
					case "p2_ActBountyReagent_02": break;
					case "p2_ActBountyReagent_03": break;
					case "p2_ActBountyReagent_04": break;
					case "p2_ActBountyReagent_05": break;
					case "Crafting_Looted_Reagent_01": break;
				}
			}
			foreach (var it in message.annItems)
			{
				var a = this.Inventory.GetItemByDynId(this, (uint)it);
			}

			Item ItemPortalToCows = null;
			List<Item> Items = new List<Item> { };
			for (int i = 0; i < message.ItemsCount; i++)
			{
				Items.Add(Inventory.GetItemByDynId(this, (uint)message.annItems[i]));
				if (Items[i].ActorSNO.Id == 272056)
					ItemPortalToCows = Items[i];
			}

			//Type - 0 - Новое свойство
			//Type - 1 - Преобразование
			//Type - 2 - 

			if (ItemPortalToCows != null)
			{
				this.InGameClient.SendMessage(new TransmuteResultsMessage()
				{
					annItem = -1,
					Type = -1,
					GBIDFakeItem = -1,
					GBIDPower = -1,
					FakeItemStackCount = -1
				});

				this.Inventory.DestroyInventoryItem(ItemPortalToCows);
				this.World.SpawnMonster(434659, new Vector3D(this.Position.X + 5, this.Position.Y + 5, this.Position.Z));
			}
			else
			{
				this.InGameClient.SendMessage(new TransmuteResultsMessage()
				{
					annItem = (int)Items[0].DynamicID(this),
					Type = 0,
					GBIDFakeItem = -1,
					GBIDPower = (int)Items[0].ItemDefinition.Hash,
					FakeItemStackCount = -1
				});
				this.GrantCriteria(74987245494264);
				this.GrantCriteria(74987258962046);
			}
		}
		private bool WaitToSpawn(TickTimer timer)
		{
			while (timer.TimedOut != true)
			{

			}
			return true;
		}
		public void OpenNephalem(GameClient client, RiftStartAcceptedMessage message)
		{
			//396751 - X1_OpenWorld_Tiered_Rifts_Portal - Великий портал
			//345935 - X1_OpenWorld_LootRunPortal - Обычные порталы
			//408511 - X1_OpenWorld_Tiered_Rifts_Challenge_Portal

			//1073741824 - Первый уровень Великого бафнутого
			//0 - Первый уровень Великого
			//
			bool Activated = false;
			TagMap NewTagMap = new TagMap();
			World NephalemPWorld = null;
			Actor NStone = null;
			Portal portal = null;
			int map = -1;
			int[] Maps = new int[]
					{

						331263, //x1_lr_tileset_Westmarch
                        360797, //_x1_lr_tileset_fortress_large
						288823, //x1_lr_tileset_zoltruins
						331389, //x1_lr_tileset_hexmaze
						275960, //x1_lr_tileset_icecave

						275946, //x1_lr_tileset_crypt
						275926, //x1_lr_tileset_corruptspire

						//288843, //x1_lr_tileset_sewers
			};

			switch (message.Field0)
			{
				#region Нефалемский портал
				case -1:
					Logger.Debug("Вызов нефалемского портала (Обычный)");
					Activated = false;

					foreach (var oldp in this.World.GetActorsBySNO(345935)) { oldp.Destroy(); }
					foreach (var oldp in this.World.GetActorsBySNO(396751)) { oldp.Destroy(); }

					map = Maps[RandomHelper.Next(0, Maps.Length)];
					//map = 288823;
					NewTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, map, 0)); //Мир
					NewTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 288482, 0)); //Зона
					NewTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 172, 0)); //Точка входа
					this.InGameClient.Game.WorldOfPortalNephalem = map;
					
					while (true)
					{
						map = Maps[RandomHelper.Next(0, Maps.Length)];
						if (map != this.InGameClient.Game.WorldOfPortalNephalem) break;
					}
					this.InGameClient.Game.WorldOfPortalNephalemSec = map;

					NephalemPWorld = this.InGameClient.Game.GetWorld(this.InGameClient.Game.WorldOfPortalNephalem);

					int ExitSceneSNO = -1;
					foreach (var scene in NephalemPWorld.Scenes.Values)
						if (scene.SceneSNO.Name.ToLower().Contains("exit"))
							ExitSceneSNO = scene.SceneSNO.Id;
					bool ExitSetted = false;
					foreach (var actor in NephalemPWorld.Actors.Values)
						if (actor is Portal)
						{
							if (!actor.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
							{
								if (!actor.CurrentScene.SceneSNO.Name.ToLower().Contains("exit"))
									actor.Destroy();
								else if (!ExitSetted)
								{
									(actor as Portal).Destination.DestLevelAreaSNO = 288684;
									(actor as Portal).Destination.WorldSNO = this.InGameClient.Game.WorldOfPortalNephalemSec;
									ExitSetted = true;

									var NephalemPWorldS2 = this.InGameClient.Game.GetWorld(this.InGameClient.Game.WorldOfPortalNephalemSec);
									foreach (var atr in NephalemPWorldS2.Actors.Values)
										if (atr is Portal)
										{
											if (!atr.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
												atr.Destroy();
											else
											{
												(atr as Portal).Destination.DestLevelAreaSNO = 332339;
												(atr as Portal).Destination.WorldSNO = 332336;
												(atr as Portal).Destination.StartingPointActorTag = 172;
											}
										}
										else if (atr is Waypoint)
											atr.Destroy();
								}
								else
									actor.Destroy();
							}
							else
							{
								(actor as Portal).Destination.DestLevelAreaSNO = 332339;
								(actor as Portal).Destination.WorldSNO = 332336;
								(actor as Portal).Destination.StartingPointActorTag = 24;
							}
						}
						else if (actor is Waypoint)
							actor.Destroy();

					#region Активация
					NStone = World.GetActorBySNO(364715);
					NStone.PlayAnimation(5, NStone.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
					NStone.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
					NStone.Attributes[GameAttribute.Untargetable] = !Activated;
					NStone.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
					NStone.Attributes[GameAttribute.Operatable] = Activated;
					NStone.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
					NStone.Attributes[GameAttribute.Disabled] = !Activated;
					NStone.Attributes[GameAttribute.Immunity] = !Activated;
					NStone.Attributes.BroadcastChangedIfRevealed();

					NStone.CollFlags = 0;
					World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
					{
						ActorID = NStone.DynamicID(plr),
						CollFlags = 0
					}, NStone);
					portal = new Portal(this.World, 345935, NewTagMap);

					TickTimer Timeout = new SecondsTickTimer(this.World.Game, 3.5f);
					var Boom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(Timeout));
					Boom.ContinueWith(delegate
					{
						portal.EnterWorld(NStone.Position);
						//Quest - 382695 - Великий Нефалемский
						//Quest - 337492 - Просто Нефалемский
						foreach (var plr in this.InGameClient.Game.Players.Values)
						{
							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = -1,
								DisplayButton = true,
								Failed = false
							});

							plr.InGameClient.SendMessage(new QuestCounterMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = -1,
								Checked = 1,
								Counter = 1
							});

							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = 1,
								DisplayButton = true,
								Failed = false
							});

							plr.InGameClient.Game.ActiveNephalemPortal = true;
							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.RiftStartedMessage));

							plr.InGameClient.SendMessage(new GameSyncedDataMessage
							{
								SyncedData = new GameSyncedData
								{
									GameSyncedFlags = 6,
									Act = 3000,       //act id
									InitialMonsterLevel = this.InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
									MonsterLevel = 0x64E4425E, //MonsterLevel
									RandomWeatherSeed = this.InGameClient.Game.WeatherSeed, //RandomWeatherSeed
									OpenWorldMode = -1, //OpenWorldMode
									OpenWorldModeAct = -1, //OpenWorldModeAct
									OpenWorldModeParam = -1, //OpenWorldModeParam
									OpenWorldTransitionTime = 0x00000064, //OpenWorldTransitionTime
									OpenWorldDefaultAct = 100, //OpenWorldDefaultAct
									OpenWorldBonusAct = -1, //OpenWorldBonusAct
									SNODungeonFinderLevelArea = 0x00000001, //SNODungeonFinderLevelArea
									LootRunOpen = -1, //LootRunOpen //0 - Великий Портал
									OpenLootRunLevel = 0, //OpenLootRunLevel
									LootRunBossDead = 0, //LootRunBossDead
									HunterPlayerIdx = 0, //HunterPlayerIdx
									LootRunBossActive = -1, //LootRunBossActive
									TieredLootRunFailed = 0, //TieredLootRunFailed
									LootRunChallengeCompleted = 0, //LootRunChallengeCompleted
									SetDungeonActive = 0, //SetDungeonActive
									Pregame = 0, //Pregame
									PregameEnd = 0, //PregameEnd
									RoundStart = 0, //RoundStart
									RoundEnd = 0, //RoundEnd
									PVPGameOver = 0x0, //PVPGameOver
									field_v273 = 0x0,
									TeamWins = new[] { 0x0, 0x0 }, //TeamWins
									TeamScore = new[] { 0x0, 0x0 }, //TeamScore
									PVPGameResult = new[] { 0x0, 0x0 }, //PVPGameResult
									PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
									TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID
								}
							});
						}
					});

					#endregion


					break;
				#endregion
				#region Великий портал
				default:
					this.InGameClient.Game.NephalemGreaterLevel = message.Field0;

					Logger.Debug("Вызов нефалемского портала (Уровень: {0})", message.Field0);
					Activated = false;
					foreach (var oldp in this.World.GetActorsBySNO(345935)) { oldp.Destroy(); }
					foreach (var oldp in this.World.GetActorsBySNO(396751)) { oldp.Destroy(); }

					this.InGameClient.Game.ActiveNephalemPortal = true;
					this.InGameClient.Game.NephalemGreater = true;

					map = Maps[RandomHelper.Next(0, Maps.Length)];
					NewTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, map, 0)); //Мир
					NewTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 288482, 0)); //Зона
					NewTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 172, 0)); //Точка входа
					this.InGameClient.Game.WorldOfPortalNephalem = map;

					NephalemPWorld = this.InGameClient.Game.GetWorld(map);
					foreach (var actor in NephalemPWorld.Actors.Values)
						if (actor is Portal)
						{
							if (!actor.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
								actor.Destroy();
							else
							{
								(actor as Portal).Destination.DestLevelAreaSNO = 332339;
								(actor as Portal).Destination.WorldSNO = 332336;
								(actor as Portal).Destination.StartingPointActorTag = 24;
							}
						}
						else if (actor is Waypoint)
							actor.Destroy();
					#region Активация
					NStone = World.GetActorBySNO(364715);
					NStone.PlayAnimation(5, NStone.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
					NStone.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
					NStone.Attributes[GameAttribute.Untargetable] = !Activated;
					NStone.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
					NStone.Attributes[GameAttribute.Operatable] = Activated;
					NStone.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
					NStone.Attributes[GameAttribute.Disabled] = !Activated;
					NStone.Attributes[GameAttribute.Immunity] = !Activated;
					NStone.Attributes.BroadcastChangedIfRevealed();

					NStone.CollFlags = 0;
					World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
					{
						ActorID = NStone.DynamicID(plr),
						CollFlags = 0
					}, NStone);
					portal = new Portal(this.World, 396751, NewTagMap);

					TickTimer AltTimeout = new SecondsTickTimer(this.World.Game, 3.5f);
					var AltBoom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(AltTimeout));
					AltBoom.ContinueWith(delegate
					{
						portal.EnterWorld(NStone.Position);
					//Quest - 382695 - Великий Нефалемский
					//Quest - 337492 - Просто Нефалемский

					//this.ChangeWorld(NephalemPWorld, NephalemPWorld.GetStartingPointById(172).Position);

					foreach (var plr in this.InGameClient.Game.Players.Values)
						{
							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.RiftStartedMessage));
							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 337492,
								snoLevelArea = 0x000466E2,
								StepID = -1,
								DisplayButton = true,
								Failed = false
							});
							plr.InGameClient.SendMessage(new QuestCounterMessage()
							{
								snoQuest = 337492,
								snoLevelArea = 0x000466E2,
								StepID = -1,
								Checked = 1,
								Counter = 1
							});
							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 337492,
								snoLevelArea = 0x000466E2,
								StepID = 13,
								DisplayButton = true,
								Failed = false
							});
							plr.InGameClient.SendMessage(new GameSyncedDataMessage
							{
								SyncedData = new GameSyncedData
								{
									GameSyncedFlags = 6,
									Act = 3000,       //act id
								InitialMonsterLevel = this.InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
								MonsterLevel = 0x64E4425E, //MonsterLevel
								RandomWeatherSeed = this.InGameClient.Game.WeatherSeed, //RandomWeatherSeed
								OpenWorldMode = -1, //OpenWorldMode
								OpenWorldModeAct = -1, //OpenWorldModeAct
								OpenWorldModeParam = -1, //OpenWorldModeParam
								OpenWorldTransitionTime = 0x00000064, //OpenWorldTransitionTime
								OpenWorldDefaultAct = 100, //OpenWorldDefaultAct
								OpenWorldBonusAct = -1, //OpenWorldBonusAct
								SNODungeonFinderLevelArea = 0x00000001, //SNODungeonFinderLevelArea
								LootRunOpen = 44, //LootRunOpen //0 - Великий Портал
								OpenLootRunLevel = 0, //OpenLootRunLevel
								LootRunBossDead = 0, //LootRunBossDead
								HunterPlayerIdx = 0, //HunterPlayerIdx
								LootRunBossActive = -1, //LootRunBossActive
								TieredLootRunFailed = 0, //TieredLootRunFailed
								LootRunChallengeCompleted = 0, //LootRunChallengeCompleted
								SetDungeonActive = 0, //SetDungeonActive
								Pregame = 0, //Pregame
								PregameEnd = 0, //PregameEnd
								RoundStart = 0, //RoundStart
								RoundEnd = 0, //RoundEnd
								PVPGameOver = 0x0, //PVPGameOver
								field_v273 = 0x0,
								TeamWins = new[] { 0x0, 0x0 }, //TeamWins
								TeamScore = new[] { 0x0, 0x0 }, //TeamScore
								PVPGameResult = new[] { 0x0, 0x0 }, //PVPGameResult
								PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
								TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID
							}
							});
							plr.InGameClient.SendMessage(new IntDataMessage(Opcodes.DungeonFinderSeedMessage)
							{
								Field0 = 0x3E0FC64C
							});
							plr.InGameClient.SendMessage(new IntDataMessage(Opcodes.DungeonFinderParticipatingPlayerCount)
							{
								Field0 = 1
							});
							plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
							{
								Field0 = 0
							});
							plr.InGameClient.SendMessage(new SNODataMessage(Opcodes.DungeonFinderSetTimedEvent)
							{
								Field0 = -1
							});
							plr.Attributes[GameAttribute.Tiered_Loot_Run_Death_Count] = 0;
						}
					});




					#endregion


					break;
					#endregion
			}
		}

		private void OnTutorialShown(GameClient client, TutorialShownMessage message)
		{
			
			// Server has to save what tutorials are shown, so the player
			// does not have to see them over and over...
			int index = ItemGenerator.Tutorials.IndexOf(message.SNOTutorial);
			if (index == -1) return;
			var seenTutorials = this.Toon.GameAccount.DBGameAccount.SeenTutorials;
			if(seenTutorials.Length <= 34)
				seenTutorials = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			seenTutorials[index / 8] |= (byte)(1 << (index % 8));

			lock (this.Toon.GameAccount.DBGameAccount)
			{
				var dbGAcc = this.Toon.GameAccount.DBGameAccount;
				dbGAcc.SeenTutorials = seenTutorials;
				DBSessions.SessionUpdate(dbGAcc);
			}
			//*/
		}

		private void OnConfirm(GameClient client, AcceptConfirmMessage message)
		{
			if (this.ConfirmationResult != null)
			{
				this.ConfirmationResult.Invoke();
				this.ConfirmationResult = null;
			}
		}

		private void OnSpendParagonPointsMessage(GameClient client, SpendParagonPointsMessage message)
		{
			var bonus = ItemGenerator.GetParagonBonusTable(this.Toon.Class).Where(b => b.Hash == message.BonusGBID).FirstOrDefault();

			if (bonus == null) return;
			if (message.Amount > this.Attributes[GameAttribute.Paragon_Bonus_Points_Available, bonus.Category]) return;
			//if (this.ParagonBonuses[(bonus.Category * 4) + bonus.Index - 1] + (byte)message.Amount > bonus.Limit) return;

			this.ParagonBonuses[(bonus.Category * 4) + bonus.Index - 1] += (byte)message.Amount;

			var dbToon = this.Toon.DBToon;
			dbToon.ParagonBonuses = this.ParagonBonuses;
			this.World.Game.GameDBSession.SessionUpdate(dbToon);

			this.SetAttributesByItems();
			this.SetAttributesByItemProcs();
			this.SetAttributesByGems();
			this.SetAttributesByItemSets();
			this.SetAttributesByPassives();
			this.SetAttributesByParagon();
			this.Attributes.BroadcastChangedIfRevealed();
			UpdatePercentageHP(PercHPbeforeChange);

		}
		private void OnResetParagonPointsMessage(GameClient client, ResetParagonPointsMessage message)
		{
			this.ParagonBonuses = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			var dbToon = this.Toon.DBToon;
			dbToon.ParagonBonuses = this.ParagonBonuses;
			this.World.Game.GameDBSession.SessionUpdate(dbToon);

			this.SetAttributesByItems();
			this.SetAttributesByItemProcs();
			this.SetAttributesByGems();
			this.SetAttributesByItemSets();
			this.SetAttributesByPassives();
			this.SetAttributesByParagon();
			this.Attributes.BroadcastChangedIfRevealed();
			UpdatePercentageHP(PercHPbeforeChange);

		}

		private void OnMailRead(GameClient client, MailReadMessage message)
		{
			//does it make sense?
		}

		private void OnMailRetrieve(GameClient client, MailRetrieveMessage message)
		{
			var dbMail = this.World.Game.GameDBSession.SessionGet<DBMail>((ulong)message.MailId);
			if (dbMail == null || dbMail.DBToon.Id != this.Toon.PersistentID) return;
			dbMail.Claimed = true;
			this.World.Game.GameDBSession.SessionUpdate(dbMail);

			if (dbMail.ItemGBID != -1)
				this.Inventory.PickUp(ItemGenerator.CookFromDefinition(this.World, ItemGenerator.GetItemDefinition(dbMail.ItemGBID), -1, true));

			this.LoadMailData();
		}

		private void OnStashIconsAssign(GameClient client, StashIconStateAssignMessage message)
		{
			if (message.StashIcons.Length != 4) return;
			lock (this.Toon.GameAccount.DBGameAccount)
			{
				var dbGAcc = this.Toon.GameAccount.DBGameAccount;
				dbGAcc.StashIcons = message.StashIcons;
				DBSessions.SessionUpdate(dbGAcc);
			}
			//LoadStashIconsData();
		}

		public void PlayCutscene(int cutsceneId)
		{
			this.InGameClient.SendMessage(new PlayCutsceneMessage()
			{
				Index = cutsceneId
			});
		}

		private void OnTranslateFacing(GameClient client, PlayerTranslateFacingMessage message)
		{
			this.SetFacingRotation(message.Angle);

			World.BroadcastExclusive(plr => new ACDTranslateFacingMessage
			{
				ActorId = this.DynamicID(plr),
				Angle = message.Angle,
				TurnImmediately = message.TurnImmediately
			}, this);
		}
		private void OnAssignActiveSkill(GameClient client, AssignSkillMessage message)
		{
			var old_skills = this.SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
			foreach (var skill in old_skills)
			{
				PowerScript power = PowerLoader.CreateImplementationForPowerSNO(skill);
				if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
				{
					this.World.BuffManager.RemoveBuffs(this, power.EvalTag(PowerKeys.SynergyPower));
				}
			}

			var oldSNOSkill = this.SkillSet.ActiveSkills[message.SkillIndex].snoSkill; // find replaced skills SNO.
			if (oldSNOSkill != -1)
			{
				this.Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
				this.World.BuffManager.RemoveBuffs(this, oldSNOSkill);

				var rem = new List<uint>();
				foreach (var fol in this.Followers.Where(f => this.World.GetActorByGlobalId(f.Key) == null || this.World.GetActorByGlobalId(f.Key).Attributes[GameAttribute.Summoned_By_SNO] == oldSNOSkill))
					rem.Add(fol.Key);
				foreach (var rm in rem)
					this.DestroyFollowerById(rm);
			}

			this.Attributes[GameAttribute.Skill, message.SNOSkill] = 1;
			//scripted //this.Attributes[GameAttribute.Skill_Total, message.SNOSkill] = 1;
			this.SkillSet.ActiveSkills[message.SkillIndex].snoSkill = message.SNOSkill;
			this.SkillSet.ActiveSkills[message.SkillIndex].snoRune = message.RuneIndex;
			this.SkillSet.SwitchUpdateSkills(message.SkillIndex, message.SNOSkill, message.RuneIndex, this.Toon);
			this.SetAttributesSkillSets();

			this.Attributes.BroadcastChangedIfRevealed();
			this.UpdateHeroState();

			var cooldownskill = this.SkillSet.ActiveSkills.GetValue(message.SkillIndex);

			if (this.SkillSet.HasSkill(460757))
				foreach (var skill in this.SkillSet.ActiveSkills)
					if (skill.snoSkill == 460757)
						if (skill.snoRune == 3)
							this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.P6_Necro_Devour_Aura());
						else
							this.World.BuffManager.RemoveBuffs(this, 474325);

			if (this.SkillSet.HasSkill(460870))
				foreach (var skill in this.SkillSet.ActiveSkills)
					if (skill.snoSkill == 460870)
						if (skill.snoRune == 4)
							this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.P6_Necro_Frailty_Aura());
						else
							this.World.BuffManager.RemoveBuffs(this, 473992);
			

			//_StartSkillCooldown((cooldownskill as ActiveSkillSavedData).snoSkill, SkillChangeCooldownLength);
		}
		private void OnAssignPassiveSkills(GameClient client, AssignTraitsMessage message)
		{
			for (int i = 0; i < message.SNOPowers.Length; ++i)
			{
				int oldSNOSkill = this.SkillSet.PassiveSkills[i]; // find replaced skills SNO.
				if (message.SNOPowers[i] != oldSNOSkill)
				{
					if (oldSNOSkill != -1)
					{
						this.World.BuffManager.RemoveAllBuffs(this);
						// switch off old passive skill
						this.Attributes[GameAttribute.Trait, oldSNOSkill] = 0;
						this.Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
						//scripted //this.Attributes[GameAttribute.Skill_Total, oldSNOSkill] = 0;
					}

					if (message.SNOPowers[i] != -1)
					{
						// switch on new passive skill
						this.Attributes[GameAttribute.Trait, message.SNOPowers[i]] = 1;
						this.Attributes[GameAttribute.Skill, message.SNOPowers[i]] = 1;
						//scripted //this.Attributes[GameAttribute.Skill_Total, message.SNOPowers[i]] = 1;
					}

					this.SkillSet.PassiveSkills[i] = message.SNOPowers[i];
				}
			}

			this.SkillSet.UpdatePassiveSkills(this.Toon);

			var skills = this.SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
			foreach (var skill in skills)
				_StartSkillCooldown(skill, SkillChangeCooldownLength);

			this.SetAttributesByItems();
			this.SetAttributesByGems();
			this.SetAttributesByItemSets();
			this.SetAttributesByPassives();
			this.SetAttributesByParagon();
			this.SetAttributesSkillSets();
			this.Inventory.CheckWeapons();      //Handles removal of Heavenly Strength			
			this.Attributes.BroadcastChangedIfRevealed();
			this.UpdateHeroState();
			UpdatePercentageHP(PercHPbeforeChange);

		}
		private void OnUnassignActiveSkill(GameClient client, UnassignSkillMessage message)
		{
			var oldSNOSkill = this.SkillSet.ActiveSkills[message.SkillIndex].snoSkill; // find replaced skills SNO.
			if (oldSNOSkill != -1)
			{
				this.Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
				this.World.BuffManager.RemoveBuffs(this, oldSNOSkill);

				var rem = new List<uint>();
				foreach (var fol in this.Followers.Where(f => this.World.GetActorByGlobalId(f.Key).Attributes[GameAttribute.Summoned_By_SNO] == oldSNOSkill))
					rem.Add(fol.Key);
				foreach (var rm in rem)
					this.DestroyFollowerById(rm);
			}

			this.SkillSet.ActiveSkills[message.SkillIndex].snoSkill = -1;
			this.SkillSet.ActiveSkills[message.SkillIndex].snoRune = -1;
			this.SkillSet.SwitchUpdateSkills(message.SkillIndex, -1, -1, this.Toon);
			this.SetAttributesSkillSets();

			this.Attributes.BroadcastChangedIfRevealed();
			this.UpdateHeroState();
		}
		public void SetNewAttributes()
		{
			//this.Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			//this.Attributes[GameAttribute.Attacks_Per_Second_Bonus] = 1.0f;
			//this.Attributes[GameAttribute.Gold] = 1;
			//[GameAttribute.Damage_Weapon_Min_Total, 0]
			this.Attributes[GameAttribute.Attacks_Per_Second_Percent] = 0;
			this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Uncapped] = 0;
			this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Reduction] = 0;
			this.Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap] = 0;
			//this.Attributes[GameAttribute.Gold_PickUp_Radius] = 5f;
			/*
			this.Attributes[GameAttribute.Experience_Bonus_Percent_Anniversary_Buff] = 100;
			this.Attributes[GameAttribute.Experience_Bonus_Percent_Community_Buff] = 100;
			this.Attributes[GameAttribute.Experience_Bonus_Percent_Handicap] = 100;
			this.Attributes[GameAttribute.Experience_Bonus_Percent_IGR_Buff] = 100;
			this.Attributes[GameAttribute.Experience_Bonus_Percent_Potion_Buff] = 1;
			//*/
			/*
			this.InGameClient.SendMessage(new PlayerSkillsMessage()
			{
				PlayerIndex = this.PlayerIndex,
				ActiveSkills = this.SkillSet.ActiveSkills,
				Traits = new int[4] { 0x00032E5E, -1, -1, -1 },
				LegendaryPowers = new int[4] { -1, -1, -1, -1 }
			});
			//*/

		}
		private void _StartSkillCooldown(int snoPower, float seconds)
		{
			this.World.BuffManager.AddBuff(this, this,
				new PowerSystem.Implementations.CooldownBuff(snoPower, seconds));
		}

		//private void OnPlayerChangeHotbarButtonMessage(GameClient client, PlayerChangeHotbarButtonMessage message)
		//{
		//	this.SkillSet.HotBarSkills[message.BarIndex] = message.ButtonData;
		//}

		private void OnObjectTargeted(GameClient client, TargetMessage message)
		{
			if (message.TargetID != 0xffffffff)
				message.TargetID = this.World.GetGlobalId(this, message.TargetID);

			if (this.Toon.Class == ToonClass.Crusader)
				if (this.World.BuffManager.HasBuff<CrusaderSteedCharge.PonyBuff>(this))     //Crusader -> cancel Steed Charge
					this.World.BuffManager.RemoveBuffs(this, 243853);

			bool powerHandled = this.World.PowerManager.RunPower(this, message.PowerSNO, message.TargetID, message.Place.Position, message);

			if (!powerHandled)
			{
				Actor actor = this.World.GetActorByGlobalId(message.TargetID);
				if (actor == null) return;



#if DEBUG
				Logger.Warn("OnTargetedActor ID: {0}, Name: {1}, NumInWorld: {2}", actor.ActorSNO.Id, actor.ActorSNO.Name, actor.NumberInWorld);
#else
				
#endif
				if ((actor.GBHandle.Type == 1) && (actor.Attributes[GameAttribute.TeamID] == 10))
				{
					this.ExpBonusData.MonsterAttacked(this.InGameClient.Game.TickCounter);
				}
				actor.OnTargeted(this, message);
				

			}

			this.ExpBonusData.Check(2);
		}

		private int _hackCounter = 0;

		public bool SpeedCheckDisabled = false;

		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}
		public int i = 0;

		private void OnPlayerMovement(GameClient client, ACDClientTranslateMessage message)
		{
			this.Attributes.BroadcastChangedIfRevealed();
			var a = this.GetActorsInRange(15f);
			
			#region 
			//UpdateExp(5000000);
			/*
			this.Attributes[GameAttribute.Jewel_Upgrades_Max] = 3;
			this.Attributes[GameAttribute.Jewel_Upgrades_Bonus] = 2;
			this.Attributes[GameAttribute.Jewel_Upgrades_Used] = 0;
			Attributes[GameAttribute.Currencies_Discovered] = 20;
			this.Attributes.BroadcastChangedIfRevealed();
			var Quest = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Quest][337492].Data;
			//*/
			//this.Toon.BigPortalKey++;
			//this.Toon.CraftItem4++;
			/*
			//Приглашение на великий портал
			InGameClient.SendMessage(new MessageSystem.Message.Definitions.Encounter.RiftJoinMessage()
			{
				PlayerIndex = 0,
				RiftStartServerTime = this.InGameClient.Game.TickCounter,
				RiftTier = 0
			});
			/*
			//Результаты прохождения подземелья
			InGameClient.SendMessage(new MessageSystem.Message.Definitions.Dungeon.SetDungeonResultsMessage()
			{
				SNOQuestKill = -1,
				QuestKillMonsterCounter = 0,
				SNOQuestBonus1 = -1,
				QuestBonus1Success = false,
				SNOQuestBonus2 = -1,
				QuestBonus2Success = false,
				SNOQuestMastery = -1,
				QuestMasterySuccess = false,
				QuestKillSuccess = false,
				ShowTotalTime = true,
				TimeTaken = 120,
				TargetTime = 200
			});
			/*
			//Приглашение в комплектное подземелье
			InGameClient.SendMessage(new MessageSystem.Message.Definitions.Dungeon.SetDungeonDialogMessage()
			{
				PlayerIndex = 0,
				LabelDescription = 1,
				LabelTitle = 1
			}) ;
			/*
			InGameClient.SendMessage(new BroadcastTextMessage()
			{
				Field0 = "Тест"
			});
			/*
			this.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameTextMessage)
			{
				Message = "Пампам"
			});
			//*/
			#endregion
			if (this.World == null) return;

			if (this.Dead)
			{
				this.World.BroadcastIfRevealed(this.ACDWorldPositionMessage, this);
				return;
			}

			if (this.World.Game.Paused || this.BetweenWorlds) return;

			if (message.MovementSpeed > (this.Attributes[GameAttribute.Running_Rate_Total] * 1.5f) && !SpeedCheckDisabled)
			{
				_hackCounter++;
				if (this._hackCounter > 5)
				{
					this._hackCounter = 0;
				}
				this.World.BroadcastIfRevealed(this.ACDWorldPositionMessage, this);
				return;
			}

			if (message.Position != null)
			{
				if (PowerMath.Distance2D(message.Position, this.Position) > 300f)
				{
					this.World.BroadcastIfRevealed(this.ACDWorldPositionMessage, this);
					this.InGameClient.SendMessage(new ACDTranslateSyncMessage()
					{
						ActorId = this.DynamicID(this),
						Position = this.Position
					});
					return;
				}
				this.Position = message.Position;
			}

			this.SetFacingRotation(message.Angle);

			if (this.IsCasting) StopCasting();
			this.World.BuffManager.RemoveBuffs(this, 298038);

			this.RevealScenesToPlayer();
			this.RevealPlayersToPlayer();
			this.RevealActorsToPlayer();

			this.World.BroadcastExclusive(plr => new ACDTranslateNormalMessage
			{
				ActorId = this.DynamicID(plr),
				Position = this.Position,
				Angle = message.Angle,
				SnapFacing = false,
				MovementSpeed = message.MovementSpeed,
				AnimationTag = message.AnimationTag
			}, this, true);

			foreach (var actor in GetActorsInRange())
				actor.OnPlayerApproaching(this);

			this.VacuumPickup();
			if (this.World.Game.OnLoadWorldActions.ContainsKey(this.World.WorldSNO.Id))
			{
				Logger.Trace("OnLoadWorldActions: {0}", this.World.WorldSNO.Id);
				lock (this.World.Game.OnLoadWorldActions[this.World.WorldSNO.Id])
				{
					try
					{
						foreach (var action in this.World.Game.OnLoadWorldActions[this.World.WorldSNO.Id])
						{
							action.Invoke();
						}
					}
					catch { }
					this.World.Game.OnLoadWorldActions[this.World.WorldSNO.Id].Clear();
				}
			}
			if (this.World.Game.OnLoadWorldActions.ContainsKey(this.CurrentScene.SceneSNO.Id))
			{
				Logger.Trace("OnLoadSceneActions: {0}", this.CurrentScene.SceneSNO.Id);
				lock (this.World.Game.OnLoadWorldActions[this.CurrentScene.SceneSNO.Id])
				{
					try
					{
						foreach (var action in this.World.Game.OnLoadWorldActions[this.CurrentScene.SceneSNO.Id])
						{
							action.Invoke();
						}
					}
					catch { }
					this.World.Game.OnLoadWorldActions[this.CurrentScene.SceneSNO.Id].Clear();
				}
			}

			if (this.CurrentScene.SceneSNO.Id != PreSceneId)
			{
				PreSceneId = this.CurrentScene.SceneSNO.Id;
				var levelArea = this.CurrentScene.Specification.SNOLevelAreas[0];
				if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
				{
					var trigger = this.World.Game.QuestProgress.QuestTriggers[levelArea];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}

				this.Attributes[GameAttribute.Corpse_Resurrection_Charges] = 3;		// Reset resurrection charges on zone change (TODO: do not reset charges on reentering the same zone)

#if DEBUG
				Logger.Warn("Местоположение игрока {0}, Scene: {1} SNO: {2} LevelArea: {3}", this.Toon.Name, this.CurrentScene.SceneSNO.Name, this.CurrentScene.SceneSNO.Id, this.CurrentScene.Specification.SNOLevelAreas[0]);
#else

#endif
			}
			this.LastMovementTick = this.World.Game.TickCounter;
		}

		private void OnCancelChanneledSkill(GameClient client, CancelChanneledSkillMessage message)
		{
			this.World.PowerManager.CancelChanneledSkill(this, message.PowerSNO);
		}

		private void OnRequestBuffCancel(GameClient client, RequestBuffCancelMessage message)
		{
			this.World.BuffManager.RemoveBuffs(this, message.PowerSNOId);
		}

		private void OnSecondaryPowerMessage(GameClient client, SecondaryAnimationPowerMessage message)
		{
			this.World.PowerManager.RunPower(this, message.PowerSNO, (uint)message.annTarget);
		}

		private void OnMiscPowerMessage(GameClient client, MiscPowerMessage message)
		{
			this.World.PowerManager.RunPower(this, message.PowerSNO);
		}

		private void OnLoopingAnimationPowerMessage(GameClient client, LoopingAnimationPowerMessage message)
		{
			this.StartCasting(150, new Action(() => {
				try
				{
					this.World.PowerManager.RunPower(this, message.snoPower);
				}
				catch { }
			}),message.snoPower);
		}

		private void OnTryWaypoint(GameClient client, TryWaypointMessage tryWaypointMessage)
		{
			var wpWorld = this.World.Game.GetWayPointWorldById(tryWaypointMessage.nWaypoint);
			var wayPoint = wpWorld.GetWayPointById(tryWaypointMessage.nWaypoint);
			Logger.Warn("---Waypoint Debug---");
			var proximity = new RectangleF(wayPoint.Position.X - 1f, wayPoint.Position.Y - 1f, 2f, 2f);
			var scenes = wpWorld.QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return; // cork (is it real?)

			var scene = scenes[0]; // Parent scene /fasbat

			if (scenes.Count == 2) // What if it's a subscene?
			{
				if (scenes[1].ParentChunkID != 0xFFFFFFFF)
					scene = scenes[1];
			}

			var levelArea = scene.Specification.SNOLevelAreas[0];
			Logger.Warn($"OnTryWaypoint: Id: {tryWaypointMessage.nWaypoint}, WorldId: {wpWorld.WorldSNO.Id}, levelArea: {levelArea}");
			if (wayPoint == null) return;
			Logger.Warn($"WpWorld: {wpWorld}, wayPoint: {wayPoint}");
			InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
			if (wpWorld == this.World)
				this.Teleport(wayPoint.Position);
			else
				this.ChangeWorld(wpWorld, wayPoint.Position);

			//handling quest triggers
			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[levelArea];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
				{
					try
					{
						trigger.questEvent.Execute(this.World); // launch a questEvent
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}
			foreach (var bounty in this.World.Game.QuestManager.Bounties)
				bounty.CheckLevelArea(levelArea);

			this.InGameClient.SendMessage(new PortedToWaypointMessage
			{
				PlayerIndex = this.PlayerIndex,
				LevelAreaSNO = levelArea
			});
			Logger.Warn("---Waypoint Debug End---");
		}
		public void RefreshReveal()
		{
			float Range = 200f;
			if (this.InGameClient.Game.CurrentEncounter.activated)
				Range = 360f;

			List<Actor> actors_around = this.GetActorsInRange(Range);

			foreach (var actor in actors_around)
				if (actor is not Player)
					actor.Unreveal(this);
			RevealActorsToPlayer();
		}
		private void OnRequestBuyItem(GameClient client, RequestBuyItemMessage requestBuyItemMessage)
		{
			var vendor = this.SelectedNPC as Vendor;
			if (vendor == null)
				return;
			vendor.OnRequestBuyItem(this, requestBuyItemMessage.ItemId);
		}

		private void OnRequestSellItem(GameClient client, RequestSellItemMessage requestSellItemMessage)
		{
			var vendor = this.SelectedNPC as Vendor;
			if (vendor == null)
				return;
			vendor.OnRequestSellItem(this, (int)requestSellItemMessage.ItemId);
		}

		private void OnHirelingRetrainMessage()
		{
			if (this.ActiveHireling == null) return;

			switch (this.ActiveHireling.Attributes[GameAttribute.Hireling_Class])
			{
				case 1:
					if (this.ActiveHireling is Templar)
						(this.ActiveHireling as Templar).Retrain(this);
					break;
				case 2:
					if (this.ActiveHireling is Scoundrel)
						(this.ActiveHireling as Scoundrel).Retrain(this);
					break;
				case 3:
					if (this.ActiveHireling is Enchantress)
						(this.ActiveHireling as Enchantress).Retrain(this);
					break;
				default:
					return;
			}
		}
		//*
		private void OnHirelingDismiss(GameClient client, PetAwayMessage message)
		{
			Logger.Trace("OnPetDismiss(): {0}", message.ActorID);
			var petId = this.World.GetGlobalId(this, message.ActorID);
			var pet = this.World.GetActorByGlobalId(petId);
			if (pet is Hireling)
				ActiveHireling = null;
			else
				this.DestroyFollowersBySnoId(pet.ActorSNO.Id);
		}
		private void OnHirelingRequestLearnSkill(GameClient client, MessageSystem.Message.Definitions.Hireling.HirelingRequestLearnSkillMessage message)
		{
			Logger.Debug("OnHirelingRequestLearnSkill(): {0} - {1}", message.HirelingID, message.PowerSNOId);
			var hireling = this.World.GetActorByGlobalId(this.World.GetGlobalId(this, message.HirelingID));
			if (hireling == null) return;
			switch (hireling.Attributes[GameAttribute.Hireling_Class])
			{
				case 1:
					if (!(hireling is Templar)) return;
					(hireling as Templar).SetSkill(this, message.PowerSNOId);
					break;
				case 2:
					if (!(hireling is Scoundrel)) return;
					(hireling as Scoundrel).SetSkill(this, message.PowerSNOId);
					break;
				case 3:
					if (!(hireling is Enchantress)) return;
					(hireling as Enchantress).SetSkill(this, message.PowerSNOId);
					break;
				default:
					break;
			}
		}
		//*/
		private void OnResurrectOption(GameClient client, RessurectionPlayerMessage message)
		{
			Logger.Trace("Resurrect option: {0}", message.Choice);
			switch (message.Choice)
			{
				case 0:
					this.Revive(this.Position);
					this.ChangeWorld(this.World.Game.StartingWorld, this.World.Game.StartPosition);
					break;
				case 1:
					this.Revive(this.CheckPointPosition);
					break;
				case 2:
					if (this.Attributes[GameAttribute.Corpse_Resurrection_Charges] > 0)
					{
						this.Revive(this.Position);
						this.Attributes[GameAttribute.Corpse_Resurrection_Charges]--;
					}
					break;
			}
		}
		//*/
		private void OnEquipPotion(GameClient client, ChangeUsableItemMessage message)
		{
			var activeSkills = this.Toon.DBActiveSkills;
			activeSkills.PotionGBID = message.Field1;
			this.World.Game.GameDBSession.SessionUpdate(activeSkills);
		}

		public void ToonStateChanged()
		{
			try
			{
				ClientSystem.GameServer.GSBackend.ToonStateChanged(this.Toon.PersistentID);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on ToonStateChanged(): ");
			}
		}

		private void OnArtisanWindowClosed()
		{

		}
		//*
		private void TrainArtisan(GameClient client, RequestTrainArtisanMessage message)
		{
			int AnimByLevel = 0;
			int IdleByLevel = 0;
			
			if (this.ArtisanInteraction == "Blacksmith")
			{
				if (blacksmith_data.Level > 55) return;
				var recipeDefinition = ItemGenerator.GetRecipeDefinition(string.Format("BlackSmith_Train_Level{0}", Math.Min(blacksmith_data.Level, 55)));

				//Logger.Trace(string.Format("BlackSmith_Train_Level{0}", Math.Min(blacksmith_data.Level, 45)));
				if (this.Inventory.GetGoldAmount() < recipeDefinition.Gold) return;
				bool haveEnoughIngredients = true;

				foreach (var ingr in recipeDefinition.Ingredients) //first loop (checking)
				{
					if (ingr.ItemsGBID == -1) continue;
					if (!this.Inventory.HaveEnough(ingr.ItemsGBID, ingr.Count)) { haveEnoughIngredients = false; break; } //if havent enough then exit
				}

				if (!haveEnoughIngredients) return;
				this.Inventory.RemoveGoldAmount(recipeDefinition.Gold);

				foreach (var ingr in recipeDefinition.Ingredients) //second loop (getting)
				{
					if (ingr.ItemsGBID == -1) continue;
					this.Inventory.GrabSomeItems(ingr.ItemsGBID, ingr.Count);
				}

				blacksmith_data.Level++;
				this.World.Game.GameDBSession.SessionUpdate(blacksmith_data);
				if (blacksmith_data.Level == 2)
					this.GrantAchievement(74987243307767);
				if (blacksmith_data.Level == 5)
					this.GrantAchievement(74987243307768);
				if (blacksmith_data.Level == 10)
				{
					this.GrantAchievement(74987243307769);
					this.GrantCriteria(74987249071497);
				}
				if (blacksmith_data.Level == 12)
				{
					this.GrantAchievement(74987251817289);
					//74987249993545
					if (jeweler_data.Level == 12 && mystic_data.Level == 12)
					{
						this.GrantCriteria(74987249993545);
					}
				}

				switch (blacksmith_data.Level)
				{
					case 1: AnimByLevel = 0x00011500; IdleByLevel = 0x00011210; break;
					case 2: AnimByLevel = 0x00011510; IdleByLevel = 0x00011220; break;
					case 3: AnimByLevel = 0x00011520; IdleByLevel = 0x00011230; break;
					case 4: AnimByLevel = 0x00011530; IdleByLevel = 0x00011240; break;
					case 5: AnimByLevel = 0x00011540; IdleByLevel = 0x00011250; break;
					case 6: AnimByLevel = 0x00011550; IdleByLevel = 0x00011260; break;
					case 7: AnimByLevel = 0x00011560; IdleByLevel = 0x00011270; break;
					case 8: AnimByLevel = 0x00011570; IdleByLevel = 0x00011280; break;
					case 9: AnimByLevel = 0x00011580; IdleByLevel = 0x00011290; break;
					case 10: AnimByLevel = 0x00011590; IdleByLevel = 0x00011300; break;
					case 11: AnimByLevel = 0x00011600; IdleByLevel = 0x00011310; break;
					case 12: AnimByLevel = 0x00011610; IdleByLevel = 0x00011320; break;
				}
				client.SendMessage(new CrafterLevelUpMessage
				{
					Type = 0,
					AnimTag = AnimByLevel, 
					NewIdle = IdleByLevel, 
					Level = blacksmith_data.Level
				});

			}
			if (this.ArtisanInteraction == "Jeweler")
			{
				if (jeweler_data.Level > 12) return;
				var recipeDefinition = ItemGenerator.GetRecipeDefinition(string.Format("Jeweler_Train_Level{0}", Math.Min(jeweler_data.Level, 11)));

				if (this.Inventory.GetGoldAmount() < recipeDefinition.Gold) return;
				bool haveEnoughIngredients = true;

				foreach (var ingr in recipeDefinition.Ingredients) //first loop (checking)
				{
					if (ingr.ItemsGBID == -1) continue;
					if (!this.Inventory.HaveEnough(ingr.ItemsGBID, ingr.Count)) { haveEnoughIngredients = false; break; } //if havent enough then exit
				}

				if (!haveEnoughIngredients) return;
				this.Inventory.RemoveGoldAmount(recipeDefinition.Gold);

				foreach (var ingr in recipeDefinition.Ingredients) //second loop (getting)
				{
					if (ingr.ItemsGBID == -1) continue;
					this.Inventory.GrabSomeItems(ingr.ItemsGBID, ingr.Count);
				}

				jeweler_data.Level++;
				this.World.Game.GameDBSession.SessionUpdate(jeweler_data);
				if (jeweler_data.Level == 2)
					this.GrantAchievement(74987243307781);
				if (jeweler_data.Level == 5)
					this.GrantAchievement(74987243307782);
				if (jeweler_data.Level == 10)
				{
					this.GrantAchievement(74987243307783);
					this.GrantCriteria(74987245845978);
				}
				if (jeweler_data.Level == 12)
				{ 
					this.GrantAchievement(74987257153995);
					if (blacksmith_data.Level == 12 && mystic_data.Level == 12)
					{
						this.GrantCriteria(74987249993545);
					}
				}
				switch (jeweler_data.Level)
				{
					case 1: AnimByLevel = 0x00011500; IdleByLevel = 0x00011210; break;
					case 2: AnimByLevel = 0x00011510; IdleByLevel = 0x00011220; break;
					case 3: AnimByLevel = 0x00011520; IdleByLevel = 0x00011230; break;
					case 4: AnimByLevel = 0x00011530; IdleByLevel = 0x00011240; break;
					case 5: AnimByLevel = 0x00011540; IdleByLevel = 0x00011250; break;
					case 6: AnimByLevel = 0x00011550; IdleByLevel = 0x00011260; break;
					case 7: AnimByLevel = 0x00011560; IdleByLevel = 0x00011270; break;
					case 8: AnimByLevel = 0x00011570; IdleByLevel = 0x00011280; break;
					case 9: AnimByLevel = 0x00011580; IdleByLevel = 0x00011290; break;
					case 10: AnimByLevel = 0x00011590; IdleByLevel = 0x00011300; break;
					case 11: AnimByLevel = 0x00011600; IdleByLevel = 0x00011310; break;
					case 12: AnimByLevel = 0x00011610; IdleByLevel = 0x00011320; break;
				}
				client.SendMessage(new CrafterLevelUpMessage
				{
					Type = 1,
					AnimTag = AnimByLevel,
					NewIdle = IdleByLevel,
					Level = jeweler_data.Level
				});
			}
			if (this.ArtisanInteraction == "Mystic")
			{
				if (mystic_data.Level > 12) return;
				var recipeDefinition = ItemGenerator.GetRecipeDefinition(string.Format("Mystic_Train_Level{0}", Math.Min(mystic_data.Level, 11)));

				if (this.Inventory.GetGoldAmount() < recipeDefinition.Gold) return;
				bool haveEnoughIngredients = true;

				foreach (var ingr in recipeDefinition.Ingredients) //first loop (checking)
				{
					if (ingr.ItemsGBID == -1) continue;
					if (!this.Inventory.HaveEnough(ingr.ItemsGBID, ingr.Count)) { haveEnoughIngredients = false; break; } //if havent enough then exit
				}

				if (!haveEnoughIngredients) return;
				this.Inventory.RemoveGoldAmount(recipeDefinition.Gold);

				foreach (var ingr in recipeDefinition.Ingredients) //second loop (getting)
				{
					if (ingr.ItemsGBID == -1) continue;
					this.Inventory.GrabSomeItems(ingr.ItemsGBID, ingr.Count);
				}

				mystic_data.Level++;
				this.World.Game.GameDBSession.SessionUpdate(mystic_data);
				if (mystic_data.Level == 2)
					this.GrantAchievement(74987253584575);
				if (mystic_data.Level == 5)
					this.GrantAchievement(74987256660015);
				if (mystic_data.Level == 10)
				{
					this.GrantAchievement(74987248802163);
					this.GrantCriteria(74987259424359);
				}
				if (mystic_data.Level == 12)
				{
					//this.GrantAchievement(74987256206128);
					if (jeweler_data.Level == 12 && blacksmith_data.Level == 12)
					{
						this.GrantCriteria(74987249993545);
					}
				}
				switch (mystic_data.Level)
				{
					case 1: AnimByLevel = 0x00011500; IdleByLevel = 0x00011210; break;
					case 2: AnimByLevel = 0x00011510; IdleByLevel = 0x00011220; break;
					case 3: AnimByLevel = 0x00011520; IdleByLevel = 0x00011230; break;
					case 4: AnimByLevel = 0x00011530; IdleByLevel = 0x00011240; break;
					case 5: AnimByLevel = 0x00011540; IdleByLevel = 0x00011250; break;
					case 6: AnimByLevel = 0x00011550; IdleByLevel = 0x00011260; break;
					case 7: AnimByLevel = 0x00011560; IdleByLevel = 0x00011270; break;
					case 8: AnimByLevel = 0x00011570; IdleByLevel = 0x00011280; break;
					case 9: AnimByLevel = 0x00011580; IdleByLevel = 0x00011290; break;
					case 10: AnimByLevel = 0x00011590; IdleByLevel = 0x00011300; break;
					case 11: AnimByLevel = 0x00011600; IdleByLevel = 0x00011310; break;
					case 12: AnimByLevel = 0x00011610; IdleByLevel = 0x00011320; break;
				}
				client.SendMessage(new CrafterLevelUpMessage
				{
					Type = 2,
					AnimTag = AnimByLevel,
					NewIdle = IdleByLevel,
					Level = mystic_data.Level
				});
			}
			this.LoadCrafterData();

			
			/**/
		}
		public void UnlockTransmog(int transmogGBID)
		{
			if (this.learnedTransmogs.Contains(transmogGBID)) return;
			this.InGameClient.SendMessage(new UnlockTransmogMessage() { TransmogGBID = transmogGBID });

			Logger.Trace("Learning transmog #{0}", transmogGBID);
			this.learnedTransmogs.Add(transmogGBID);
			mystic_data.LearnedRecipes = SerializeBytes(this.learnedTransmogs);
			this.World.Game.GameDBSession.SessionUpdate(mystic_data);

			this.LoadCrafterData();
		}
		#endregion

		#region update-logic

		int PreviousLevelArea = -1;

		private List<TickTimer> TimedActions = new List<TickTimer>();

		public int VaultsDone = 0;
		public int SpikeTrapsKilled = 0;

		public void AddTimedAction(float seconds, Action<int> onTimeout)
		{
			this.TimedActions.Add(TickTimer.WaitSeconds(this.World.Game, seconds, onTimeout));
		}

		public void Update(int tickCounter)
		{
			if (this.BetweenWorlds) return;

#if DEBUG
#else
			if ((this.InGameClient.Game.TickCounter - this.LastMovementTick) > 54000) //15m AFK
			{

				this.InGameClient.SendMessage(new SimpleMessage(Opcodes.CloseGameMessage));
			}
#endif

			// Check the gold
			if (this.InGameClient.Game.TickCounter % 120 == 0 && this.World != null && this.GoldCollectedTempCount > 0)
			{
				if (this.World.Game.IsHardcore)
					this.Toon.GameAccount.HardcoreGold += (ulong)this.GoldCollectedTempCount;
				else
					this.Toon.GameAccount.Gold += (ulong)this.GoldCollectedTempCount;

				this.Toon.CollectedGold += (ulong)this.GoldCollectedTempCount;

				if (this.World.Game.IsHardcore)
					this.Toon.CollectedGoldSeasonal += this.GoldCollectedTempCount;

				this.UpdateAchievementCounter(10, (uint)this.GoldCollectedTempCount);

				this.GoldCollectedTempCount = 0;
			}

			// Check the blood shards
			if (this.InGameClient.Game.TickCounter % 120 == 0 && this.World != null && this.BloodShardsCollectedTempCount > 0)
			{
				if (this.World.Game.IsHardcore)
					this.Toon.GameAccount.HardcoreBloodShards += this.BloodShardsCollectedTempCount;
				else
					this.Toon.GameAccount.BloodShards += this.BloodShardsCollectedTempCount;

				this.Toon.GameAccount.TotalBloodShards += this.BloodShardsCollectedTempCount;

				this.BloodShardsCollectedTempCount = 0;
			}

			if (this.World != null && this.SkillSet.HasPassive(298038) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new UnwaveringWillBuff());


			if (this.World != null && this.SkillSet.HasSkill(312736) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new MonkDashingStrike.DashingStrikeCountBuff());
			else if (!this.SkillSet.HasSkill(312736))
				this.Attributes[GameAttribute.Skill_Charges, 312736] = 0;

			if (this.World != null && this.SkillSet.HasSkill(129217) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new Sentry.SentryCountBuff());
			else if (!this.SkillSet.HasSkill(129217))
				this.Attributes[GameAttribute.Skill_Charges, 129217] = 0;

			if (this.World != null && this.SkillSet.HasSkill(75301) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new SpikeTrap.SpikeCountBuff());
			else if (!this.SkillSet.HasSkill(75301))
				this.Attributes[GameAttribute.Skill_Charges, 75301] = 0;

			if (this.World != null && this.SkillSet.HasSkill(464896) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new BoneSpirit.SpiritCountBuff());
			else if (!this.SkillSet.HasSkill(464896))
				this.Attributes[GameAttribute.Skill_Charges, 464896] = 0;

			if (this.World != null && this.SkillSet.HasSkill(97435) && (this.InGameClient.Game.TickCounter - this.LastMovementTick) > 90)
				this.World.BuffManager.AddBuff(this, this, new FuriousCharge.FuriousChargeCountBuff());
			else if (!this.SkillSet.HasSkill(97435))
				this.Attributes[GameAttribute.Skill_Charges, 97435] = 0;

			this.Attributes.BroadcastChangedIfRevealed();
			lock (this.TimedActions)
				foreach (var timed_action in this.TimedActions)
					timed_action.Update(tickCounter);

			foreach (var timed_out in this.TimedActions.Where(t => t.TimedOut).ToList())
				this.TimedActions.Remove(timed_out);

			// Check the Killstreaks
			this.ExpBonusData.Check(0);
			this.ExpBonusData.Check(1);

			// Check if there is an conversation to close in this tick
			Conversations.Update(this.World.Game.TickCounter);

			foreach (Actor proximityGizmo in this.GetObjectsInRange<Actor>(20f, true))
			{
				if (proximityGizmo == null || proximityGizmo.ActorSNO == null) continue;
				if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(proximityGizmo.ActorSNO.Id) && proximityGizmo.Visible) //EnterTrigger
				{
					var trigger = this.World.Game.QuestProgress.QuestTriggers[proximityGizmo.ActorSNO.Id];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
					{
						//this.World.Game.Quests.NotifyQuest(this.World.Game.CurrentQuest, Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, proximityGizmo.ActorSNO.Id);
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				else if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(proximityGizmo.ActorSNO.Id))
				{
					var trigger = this.World.Game.SideQuestProgress.QuestTriggers[proximityGizmo.ActorSNO.Id];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
					{
						this.World.Game.SideQuestProgress.UpdateSideCounter(proximityGizmo.ActorSNO.Id);
						if (trigger.count == this.World.Game.SideQuestProgress.QuestTriggers[proximityGizmo.ActorSNO.Id].counter)
							trigger.questEvent.Execute(this.World); // launch a questEvent
					}
				}
				if (this.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(proximityGizmo.ActorSNO.Id) && proximityGizmo.Visible) //EnterTrigger
				{
					var trigger = this.World.Game.SideQuestProgress.GlobalQuestTriggers[proximityGizmo.ActorSNO.Id];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
					{
						//this.World.Game.Quests.NotifyQuest(this.World.Game.CurrentQuest, Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, proximityGizmo.ActorSNO.Id);
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
							this.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(proximityGizmo.ActorSNO.Id);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
			}

			_UpdateResources();

			if (this.IsCasting) UpdateCastState();

			if (this.InGameClient.Game.TickCounter % 60 == 0 && this.World != null)
			{
				var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
				var scenes = this.World.QuadTree.Query<Scene>(proximity);
				if (scenes.Count == 0) return;
				var scene = scenes[0];
				if (this.PreviousLevelArea != scene.Specification.SNOLevelAreas[0])
				{
					this.PreviousLevelArea = scene.Specification.SNOLevelAreas[0];
					this.World.Game.WorldGenerator.CheckLevelArea(this.World, this.PreviousLevelArea);
					if (this.InGameClient.Game.TickCounter % 600 == 0)
						this.CheckLevelAreaCriteria(this.PreviousLevelArea);
				}
			}

			if (this.InGameClient.Game.TickCounter % 600 == 0 && this.World != null)
			{
				if (this.KilledMonstersTempCount != 0)
				{
					this.Toon.KilledMonsters += (ulong)this.KilledMonstersTempCount;
					this.KilledMonstersTempCount = 0;

					if (this.KilledElitesTempCount != 0)
					{
						this.Toon.KilledElites += (ulong)this.KilledElitesTempCount;
						if (this.World.Game.IsHardcore)
							this.Toon.KilledElitesSeasonal += this.KilledElitesTempCount;
						this.KilledElitesTempCount = 0;
					}

					if (this.KilledSeasonalTempCount != 0)
					{
						if (this.World.Game.IsHardcore)
							this.Toon.SeasonalKills += this.KilledSeasonalTempCount;
						this.KilledSeasonalTempCount = 0;
					}
				}

				this.CheckAchievementCounters();
			}

			#region Призывы некроманта
			bool switchertobool = false;
			bool switchertoboolTwo = false;
			ActiveSkillSavedData NowSkillGolem = null;
			foreach (var skill in this.SkillSet.ActiveSkills)
			{
				if (skill.snoSkill == 453801)
					switchertobool = true;
			}
			foreach (var skill in this.SkillSet.ActiveSkills)
				if (skill.snoSkill == 451537)
				{
					switchertoboolTwo = true;
					NowSkillGolem = skill;
				}
			ActiveSkeletons = switchertobool;
			EnableGolem = switchertoboolTwo;

			

			PowerContext Killer = new PowerContext();
			Killer.User = this;
			Killer.World = this.World;
			Killer.PowerSNO = -1;

			if (ActiveSkeletons)
			{
				while (NecroSkeletons.Count < 7)
				{
					NecromancerSkeleton_A Skeleton = new NecromancerSkeleton_A(this.World, 473147, this);
					Skeleton.Brain.DeActivate();
					Skeleton.Scale = 1.2f;

					Skeleton.EnterWorld(PowerContext.RandomDirection(this.Position, 3f, 8f));
					NecroSkeletons.Add(Skeleton);
					/*this.InGameClient.SendMessage(new PetMessage()
					{
						Owner = this.PlayerIndex,
						Index = this.CountFollowers(473147),
						PetId = Skeleton.DynamicID(this),
						Type = 70,
					});
					//*/
					Skeleton.Brain.Activate();
				}
			}
			else
			{
				foreach (var skel in NecroSkeletons)
				{
					this.InGameClient.SendMessage(new PetDetachMessage()
					{
						PetId = skel.GlobalID
					});
					this.World.Leave(skel);
				}
				NecroSkeletons.Clear();
			}
			if (EnableGolem || ActiveGolem != null)
			{
				if (ActiveGolem != null)
				{

					if (ActiveGolem.ActorSNO.Id != RuneSelect(451537, 471947, 465239, 460042, 471619, 471646, 471647) ||
						!this.SkillSet.HasSkill(451537))
					{
						if (ActiveGolem.World != null)
						{
							if (!(ActiveGolem.IsRevealedToPlayer(this)))
								this.InGameClient.SendMessage(new PetDetachMessage()
								{
									PetId = ActiveGolem.GlobalID
								});
							Killer.Target = ActiveGolem;
							(ActiveGolem as Minion).Kill(Killer);
						}
						ActiveGolem = null;
					}
				}
				else
				{
					if (Attributes[GameAttribute.Power_Cooldown, 451537] > this.InGameClient.Game.TickCounter)
					{

					}
					else
					{
						switch (RuneSelect(451537, 471947, 465239, 460042, 471619, 471646, 471647))
						{
							case 471947:
								var NGolem = new BaseGolem(this.World, this);
								NGolem.Brain.DeActivate();
								NGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								NGolem.Attributes[GameAttribute.Untargetable] = true;
								NGolem.EnterWorld(NGolem.Position);


								//(NGolem as BaseGolem).Brain.Activate();
								NGolem.Attributes[GameAttribute.Untargetable] = false;
								NGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = NGolem;
								break;
							case 471646:
								var CFGolem = new ConsumeFleshGolem(this.World, this);
								CFGolem.Brain.DeActivate();
								CFGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								CFGolem.Attributes[GameAttribute.Untargetable] = true;
								CFGolem.EnterWorld(CFGolem.Position);


								//(CFGolem as ConsumeFleshGolem).Brain.Activate();
								CFGolem.Attributes[GameAttribute.Untargetable] = false;
								CFGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = CFGolem;

								break;
							case 471647:
								var IGolem = new IceGolem(this.World, this);
								IGolem.Brain.DeActivate();
								IGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								IGolem.Attributes[GameAttribute.Untargetable] = true;
								IGolem.EnterWorld(IGolem.Position);


								//(IGolem as IceGolem).Brain.Activate();
								IGolem.Attributes[GameAttribute.Untargetable] = false;
								IGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = IGolem;
								break;
							case 465239:
								var BGolem = new BoneGolem(this.World, this);
								BGolem.Brain.DeActivate();
								BGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								BGolem.Attributes[GameAttribute.Untargetable] = true;
								BGolem.EnterWorld(BGolem.Position);


								//(BGolem as BoneGolem).Brain.Activate();
								BGolem.Attributes[GameAttribute.Untargetable] = false;
								BGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = BGolem;
								break;
							case 471619:
								var DGolem = new DecayGolem(this.World, this);
								DGolem.Brain.DeActivate();
								DGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								DGolem.Attributes[GameAttribute.Untargetable] = true;
								DGolem.EnterWorld(DGolem.Position);


								//(DGolem as DecayGolem).Brain.Activate();
								DGolem.Attributes[GameAttribute.Untargetable] = false;
								DGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = DGolem;
								break;
							case 460042:
								var BlGolem = new BloodGolem(this.World, this);
								BlGolem.Brain.DeActivate();
								BlGolem.Position = RandomDirection(this.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
								BlGolem.Attributes[GameAttribute.Untargetable] = true;
								BlGolem.EnterWorld(BlGolem.Position);


								//(BlGolem as BloodGolem).Brain.Activate();
								BlGolem.Attributes[GameAttribute.Untargetable] = false;
								BlGolem.Attributes.BroadcastChangedIfRevealed();
								ActiveGolem = BlGolem;
								break;
						}

						(ActiveGolem as Minion).Brain.Activate();
						ActiveGolem.Attributes[GameAttribute.Untargetable] = false;
						ActiveGolem.Attributes.BroadcastChangedIfRevealed();
						ActiveGolem.PlayActionAnimation(462828);
					}
				}
			}
			else
			{
				if (ActiveGolem != null)
				{
					if (ActiveGolem.World != null)
						(ActiveGolem as Minion).Kill();
					ActiveGolem = null;
				}
			}
			#endregion
		}

		#endregion
		public T RuneSelect<T>(int PowerSNO, T none, T runeA, T runeB, T runeC, T runeD, T runeE)
		{
			int Rune_A = this.Attributes[GameAttribute.Rune_A, PowerSNO];
			int Rune_B = this.Attributes[GameAttribute.Rune_B, PowerSNO];
			int Rune_C = this.Attributes[GameAttribute.Rune_C, PowerSNO];
			int Rune_D = this.Attributes[GameAttribute.Rune_D, PowerSNO];
			int Rune_E = this.Attributes[GameAttribute.Rune_E, PowerSNO];
			
			if (Rune_A > 0) return runeA;
			else if (Rune_B > 0) return runeB;
			else if (Rune_C > 0) return runeC;
			else if (Rune_D > 0) return runeD;
			else if (Rune_E > 0) return runeE;
			else return none;
		}
#region enter, leave, reveal handling

        /// <summary>
        /// Revals scenes in player's proximity.
        /// </summary>
        public void RevealScenesToPlayer()
		{
			//List<Scene> scenes_around = this.GetScenesInRegion(DefaultQueryProximityLenght * 2);
			List<Scene> scenes_around = this.World.Scenes.Values.ToList();
			if (!this.World.worldData.DynamicWorld)
				scenes_around = this.GetScenesInRegion(DefaultQueryProximityLenght * 3);

			foreach (var scene in scenes_around) // reveal scenes in player's proximity.
			{
				if (scene.IsRevealedToPlayer(this)) // if the actors is already revealed skip it.
					continue; // if the scene is already revealed, skip it.

				if (scene.Parent != null) // if it's a subscene, always make sure it's parent get reveals first and then it reveals his childs.
					scene.Parent.Reveal(this);
				else
					scene.Reveal(this);
			}

			foreach (var scene in this.World.Scenes.Values) // unreveal far scenes
			{
				if (!scene.IsRevealedToPlayer(this) || scenes_around.Contains(scene))
					continue;

				if (scene.Parent != null) // if it's a subscene, always make sure it's parent get reveals first and then it reveals his childs.
					scene.Parent.Unreveal(this);
				else
					scene.Unreveal(this);
			}
		}

		/// <summary>
		/// Reveals actors in player's proximity.
		/// </summary>
		public void RevealActorsToPlayer()
		{
			float Range = 200f;
			if (this.InGameClient.Game.CurrentEncounter.activated)
				Range = 360f;
			
			List<Actor> actors_around = this.GetActorsInRange(Range);

			if (this.World.WorldSNO.Id == 295225 ||
				this.World.WorldSNO.Id == 103209 ||
				this.World.WorldSNO.Id == 186552 ||
				this.World.WorldSNO.Id == 328484 ||
				this.World.WorldSNO.Id == 105406)
			{
				actors_around = this.World.Actors.Values.ToList();
			}

			foreach (var actor in actors_around) // reveal actors in player's proximity.
			{
				if (actor is Player) // if the actors is already revealed, skip it.
					continue;

				if (this.World.WorldSNO.Id == 332336 && actor is Portal)
					if ((actor as Portal).Destination.WorldSNO == 332336)
						continue;
				if (this.World.WorldSNO.Id == 71150 && actor is Portal)
					if ((actor as Portal).Destination.WorldSNO == 71150 && (actor as Portal).Destination.DestLevelAreaSNO == 19947)
					{
						(actor as Portal).Destination.WorldSNO = 332336;
						(actor as Portal).Destination.StartingPointActorTag = 483;
					}
					
				if (actor.ActorType != ActorType.ClientEffect && actor.ActorType != ActorType.AxeSymbol && actor.ActorType != ActorType.CustomBrain)
				{
					actor.Reveal(this);
				}
			}

			foreach (var actor in this.World.Actors.Values) // unreveal far actors
			{
				if ((actor is Player && (!this.World.IsPvP || actor == this)) || actors_around.Contains(actor)) // if the actors is already revealed, skip it.
					continue;
				
				actor.Unreveal(this);
			}
		}

		/// <summary>
		/// Reveals other players in player's proximity.
		/// </summary>
		public void RevealPlayersToPlayer()
		{
			var actors = this.GetActorsInRange<Player>(100f);

			foreach (var actor in actors) // reveal actors in player's proximity.
			{
				if (actor.IsRevealedToPlayer(this)) // if the actors is already revealed, skip it.
					continue;

				actor.Reveal(this);

				if (!this.IsRevealedToPlayer(actor))
					this.Reveal(actor);
			}
		}

		public void ReRevealPlayersToPlayer()
		{
			var actors = this.GetActorsInRange<Player>(100f);

			foreach (var actor in actors) // reveal actors in player's proximity.
			{
				if (actor.IsRevealedToPlayer(this)) // if the actors is already revealed, skip it.
				{
					actor.Unreveal(this);
				}

				actor.Reveal(this);

				if (!this.IsRevealedToPlayer(actor))
					this.Reveal(actor);
				else
				{
					this.Unreveal(actor);
					this.Reveal(actor);
				}
			}
		}

		public void ClearDoorAnimations()
		{
			var doors = this.GetActorsInRange<Door>(100f);
			foreach (var door in doors)
			{
				if (door.IsRevealedToPlayer(this))
					this.InGameClient.SendMessage(new SetIdleAnimationMessage
					{
						ActorID = door.DynamicID(this),
						AnimationSNO = AnimationSetKeys.Open.ID
					});
			}
		}

		public override void OnEnter(World world)
		{
			

			world.Reveal(this);
			this.Unreveal(this);

			if (this._CurrentHPValue == -1f)
				this.DefaultQueryProximityRadius = 60;
			
			this.InGameClient.SendMessage(new EnterWorldMessage()
			{
				EnterPosition = this.Position,
				WorldID = world.GlobalID,
				WorldSNO = world.WorldSNO.Id,
				PlayerIndex = this.PlayerIndex,
				EnterLookUsed = true,
				EnterKnownLookOverrides = new EnterKnownLookOverrides { Field0 = new int[] { -1, -1, -1, -1, -1, -1 } }
			});

			switch (world.WorldSNO.Id)
			{
				case 308705:
					this.InGameClient.SendMessage(new PlayerSetCameraObserverMessage()
					{
						Field0 = 309026,
						Field1 = new WorldPlace() { Position = new Vector3D(), WorldID = 0 }
					});
					break;
				case 306549:
					this.InGameClient.SendMessage(new PlayerSetCameraObserverMessage()
					{
						Field0 = 1541,
						Field1 = new WorldPlace() { Position = new Vector3D(), WorldID = 0 }
					});
					break;
			}

			if (this._CurrentHPValue == -1f)
				this.AddPercentageHP(100);

			this.DefaultQueryProximityRadius = 100;

			this.RevealScenesToPlayer();
			this.RevealPlayersToPlayer();

			// load all inventory items
			if (!this.Inventory.Loaded)
			{//why reload if already loaded?
				this.Inventory.LoadFromDB();
				this.Inventory.LoadStashFromDB();
			}
			else
				this.Inventory.RefreshInventoryToClient();

			// generate visual update message
			//this.Inventory.SendVisualInventory(this);
			SetAllStatsInCorrectOrder();
			SetAttributesSkillSets();
			if (this.World.IsPvP)
				DisableStoneOfRecall();
			else
				EnableStoneOfRecall();

			this.Reveal(this);

			System.Threading.Tasks.Task.Delay(3).Wait();
			this.RevealActorsToPlayer();

			//
		}

		public override void OnTeleport()
		{
			this.Unreveal(this);
			BeforeChangeWorld();
			this.RevealScenesToPlayer(); // reveal scenes in players proximity.
			this.RevealPlayersToPlayer();
			this.RevealActorsToPlayer(); // reveal actors in players proximity.
										 //TickTimer.WaitSeconds(this.World.Game, 5.0f, new Action<int>((x) => Logger.Debug("Timer")));
			this.Reveal(this);
			AfterChangeWorld();
			// load all inventory items
			if (!this.Inventory.Loaded)
			{
				//why reload if already loaded?
				this.Inventory.LoadFromDB();
				this.Inventory.LoadStashFromDB();
			}
			else
				this.Inventory.RefreshInventoryToClient();


		}

		public override void OnLeave(World world)
		{
			this.Conversations.StopAll();

			// save visual equipment
			this.Toon.HeroVisualEquipmentField.Value = this.Inventory.GetVisualEquipment();
			//this.Toon.HeroLevelField.Value = this.Attributes[GameAttribute.Level];
			this.Toon.GameAccount.ChangedFields.SetPresenceFieldValue(this.Toon.HeroVisualEquipmentField);
			this.Toon.GameAccount.ChangedFields.SetPresenceFieldValue(this.Toon.HeroLevelField);
			this.Toon.GameAccount.ChangedFields.SetPresenceFieldValue(this.Toon.HeroParagonLevelField);
			world.Unreveal(this);
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (!this.World.IsPvP || this == player)
			{
				player.InGameClient.SendMessage(new PlayerEnterKnownMessage()
				{
					PlayerIndex = this.PlayerIndex,
					ActorId = this.DynamicID(player),
				});
			}

			this.Inventory.SendVisualInventory(player);

			if (this == player) // only send this to player itself. Warning: don't remove this check or you'll make the game start crashing! /raist.
			{
				player.InGameClient.SendMessage(new PlayerActorSetInitialMessage()
				{
					ActorId = this.DynamicID(player),
					PlayerIndex = this.PlayerIndex,
				});
			}

			if (!base.Reveal(player))
				this.Inventory.Reveal(player);

			if (this == player) // only send this when player's own actor being is revealed. /raist.
			{
                player.InGameClient.SendMessage(new PlayerWarpedMessage()
                {
                    WarpReason = 9,
                    WarpFadeInSecods = 0f,
                });
            }

			if (this.SkillSet.HasSkill(460757))
				foreach (var skill in this.SkillSet.ActiveSkills)
					if (skill.snoSkill == 460757)
						if (skill.snoRune == 3)
							this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.P6_Necro_Devour_Aura());
						else
							this.World.BuffManager.RemoveBuffs(this, 474325);

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			this.Inventory.Unreveal(player);

			return true;
		}

		public Dictionary<Buff, int> AllBuffs = new Dictionary<Buff, int>();

		public bool BetweenWorlds = false;

		public override void BeforeChangeWorld()
		{
			this.ClearDoorAnimations();
			this.World.Game.QuestManager.UnsetBountyMarker(this);
			this.BetweenWorlds = true;
			this.AllBuffs = this.World.BuffManager.GetAllBuffs(this);
			this.World.BuffManager.RemoveAllBuffs(this);
			//this.Inventory.Unreveal(this);
			//this.InGameClient.TickingEnabled = false;
			/*this.InGameClient.SendMessage(new FreezeGameMessage
			{
				Field0 = true
			});*/

			this.InGameClient.SendMessage(new ACDTranslateSyncMessage()
			{
				ActorId = this.DynamicID(this),
				Position = this.Position
			});

			this._CurrentHPValue = this.Attributes[GameAttribute.Hitpoints_Cur];
			this._CurrentResourceValue = this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1];
		}

		public override void AfterChangeWorld()
		{
			//this.InGameClient.TickingEnabled = true;
			/*
			this.InGameClient.SendMessage(new FreezeGameMessage
			{
				Field0 = false
			});
			*/
			this.Inventory.Reveal(this);

			foreach (var buff in this.AllBuffs)
				this.World.BuffManager.CopyBuff(this, this, buff.Key, buff.Value);
			this.AllBuffs.Clear();
			this.BetweenWorlds = false;

			if (_CurrentHPValue != -1)
			{
				this.Attributes[GameAttribute.Hitpoints_Cur] = this._CurrentHPValue;
				this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] = this._CurrentResourceValue;
				this.Attributes.BroadcastChangedIfRevealed();
				this._CurrentHPValue = -1;
			}
			this.World.Game.QuestManager.SetBountyMarker(this);

			
			//System.Threading.Tasks.Task.Delay(1000).ContinueWith(a => {this.BetweenWorlds = false;});
		}

		#endregion

		#region hero-state

		public void WTF()
		{

			Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, 208468] = true;
			Attributes[GameAttribute.Thorns_Fixed_Total, 0] = 0;
			Attributes[GameAttribute.Damage_Delta_Total, 0] = 0;
			Attributes[GameAttribute.UnequippedTime, 4] = 0;
			Attributes[GameAttribute.Experience_Next_Lo] = 717;
			Attributes[GameAttribute.Skill_Total, 93395] = 1;
			Attributes[GameAttribute.Strength] = 0;
			Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap] = 0;
			Attributes[GameAttribute.Invulnerable] = true;
			Attributes[GameAttribute.UnequippedTime, 7] = 0;
			Attributes[GameAttribute.Damage_Min, 0] = 0;
			Attributes[GameAttribute.Damage_Weapon_Min_Total_All] = 0;
			Attributes[GameAttribute.Damage_Delta_Total, 3] = 0;
			Attributes[GameAttribute.General_Cooldown] = 0;
			Attributes[GameAttribute.Attacks_Per_Second_Total] = 0;
			Attributes[GameAttribute.Resource_Cur, 1] = 0;
			Attributes[GameAttribute.UnequippedTime, 6] = 0;
			Attributes[GameAttribute.Backpack_Slots] = 60;
			Attributes[GameAttribute.Corpse_Resurrection_Charges] = 3;
			Attributes[GameAttribute.Skill, 93395] = 1;
			Attributes[GameAttribute.Trait, 451242] = 2;
			Attributes[GameAttribute.UnequippedTime, 9] = 0;
			Attributes[GameAttribute.Attacks_Per_Second] = 0;
			Attributes[GameAttribute.TeamID] = 2;
			Attributes[GameAttribute.Resource_Degeneration_Stop_Point, 1048575] = 0;
			Attributes[GameAttribute.Resource_Max_Bonus, 1] = 0;
			Attributes[GameAttribute.Armor_Total] = 0;
			Attributes[GameAttribute.Skill_Total, 1759] = 1;
			Attributes[GameAttribute.SkillKit] = 35584;
			Attributes[GameAttribute.Armor_Item_Total] = 0;
			Attributes[GameAttribute.Resistance_Total, 5] = 0;
			Attributes[GameAttribute.Skill, 30718] = 1;
			Attributes[GameAttribute.CantStartDisplayedPowers] = true;
			Attributes[GameAttribute.Seasononlyitemsunlocked] = true;
			Attributes[GameAttribute.UnequippedTime, 10] = 0;
			Attributes[GameAttribute.Damage_Weapon_Delta_Total_All] = 0;
			Attributes[GameAttribute.Damage_Min_Total, 3] = 0;
			Attributes[GameAttribute.Resource_Cost_Reduction_Percent_All] = 0;
			Attributes[GameAttribute.Get_Hit_Recovery] = 0;
			Attributes[GameAttribute.Skill, 1759] = 1;
			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 439438] = 155;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 439438] = 3755;
			Attributes[GameAttribute.Skill, 30744] = 1;
			Attributes[GameAttribute.Get_Hit_Recovery_Per_Level] = 0;
			Attributes[GameAttribute.Requirement, 57] = 0;
			Attributes[GameAttribute.Damage_Weapon_Delta, 3] = 0;
			Attributes[GameAttribute.Attacks_Per_Second_Item_CurrentHand] = 0;
			Attributes[GameAttribute.Get_Hit_Recovery_Base] = 0;
			Attributes[GameAttribute.Resistance_From_Intelligence] = 0;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 0;
			Attributes[GameAttribute.Get_Hit_Max] = 0;
			Attributes[GameAttribute.Crit_Damage_Cap] = 0;
			Attributes[GameAttribute.Class_Damage_Reduction_Percent_PVP] = 0;
			Attributes[GameAttribute.Buff_Icon_Count0, 212032] = 1;
			Attributes[GameAttribute.Hit_Chance] = 0;
			Attributes[GameAttribute.Crit_Percent_Cap] = 0;
			Attributes[GameAttribute.Get_Hit_Max_Per_Level] = 0;
			Attributes[GameAttribute.Resource_Regen_Per_Second, 1] = 0;
			Attributes[GameAttribute.Buff_Icon_Count0, 134334] = 1;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 134334] = 2101;
			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 134334] = 301;
			Attributes[GameAttribute.Banter_Cooldown, 1048575] = 0;
			Attributes[GameAttribute.Hidden] = false;
			Attributes[GameAttribute.Buff_Icon_Count0, 439438] = 0;
			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 212032] = 0;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 212032] = 0;
			Attributes[GameAttribute.Immobolize] = false;
			Attributes[GameAttribute.Untargetable] = false;
			Attributes[GameAttribute.Loading] = false;
			Attributes[GameAttribute.Invulnerable] = false;
			Attributes[GameAttribute.Resource_Degeneration_Stop_Point, 1048575] = 0;
			Attributes[GameAttribute.CantStartDisplayedPowers] = false;
			Attributes[GameAttribute.Buff_Icon_Start_Tick0, 439438] = 0;
			Attributes[GameAttribute.Buff_Icon_End_Tick0, 439438] = 0;
			Attributes[GameAttribute.Buff_Icon_Count0, 212032] = 0;
			Attributes.BroadcastChangedIfRevealed();
		}

		/// <summary>
		/// Allows hero state message to be sent when hero's some property get's updated.
		/// </summary>
		public void UpdateHeroState()
		{
			this.InGameClient.SendMessage(new HeroStateMessage
			{
				State = this.GetStateData(),
				PlayerIndex = this.PlayerIndex
			});
		}

		public HeroStateData GetStateData()
		{
			return new HeroStateData()
			{
				LastPlayedAct = 400, //LastPlayedAct
				HighestUnlockedAct = 400, //HighestUnlockedAct
				PlayedFlags = (int)this.Toon.Flags,
				PlayerSavedData = this.GetSavedData(),
				//QuestRewardHistoryEntriesCount = QuestRewardHistory.Count,
				tQuestRewardHistory = QuestRewardHistory.ToArray()
			};
		}

#endregion

#region player attribute handling

		public void QueueDeath(bool state)
		{
			//this.World.BroadcastIfRevealed(this.ACDWorldPositionMessage, this);
			this.InGameClient.SendMessage(new ACDTranslateSyncMessage()
			{
				ActorId = this.DynamicID(this),
				Position = this.Position
			});
			this.Attributes[GameAttribute.QueueDeath] = state;
			this.Attributes[GameAttribute.Disabled] = state;
			this.Attributes[GameAttribute.Waiting_To_Accept_Resurrection] = false;
			this.Attributes[GameAttribute.Invulnerable] = state;
			//this.Attributes[GameAttribute.Stunned] = state;
			this.Attributes[GameAttribute.Immobolize] = state;
			this.Attributes[GameAttribute.Hidden] = state;
			this.Attributes[GameAttribute.Untargetable] = state;
			this.Attributes[GameAttribute.CantStartDisplayedPowers] = state;
			this.Attributes[GameAttribute.IsContentRestrictedActor] = state;

			this.Attributes[GameAttribute.Rest_Experience_Lo] = 0;
			this.Attributes[GameAttribute.Rest_Experience_Bonus_Percent] = 0;

			this.Attributes.BroadcastChangedIfRevealed();
			if (this.World.Game.PvP)
			{
				this.Attributes[GameAttribute.Resurrect_As_Observer] = state;
				//this.Attributes[GameAttribute.Observer] = !state;
			}
			//this.Attributes[GameAttribute.Corpse_Resurrection_Charges] = 1;	// Enable this to allow unlimited resurrection at corpse
			//this.Attributes[GameAttribute.Corpse_Resurrection_Allowed_Game_Time] = this.World.Game.TickCounter + 300; // Timer for auto-revive (seems to be broken?)
			this.Attributes.BroadcastChangedIfRevealed();
		}

		public void Resurrect()
		{
			this.Attributes[GameAttribute.Waiting_To_Accept_Resurrection] = true;
			this.Attributes.BroadcastChangedIfRevealed();
		}

		public void Revive(Vector3D spawnPosition)
		{
			if (this.World == null) return;
			/*if (this.World.Game.IsHardcore)
			{
				this.InGameClient.SendMessage(new LogoutTickTimeMessage()
				{
					Field0 = false, // true - logout with party?
					Ticks = 0, // delay 1, make this equal to 0 for instant logout
					Field2 = 0, // delay 2
				});
			} else
			{*/
			this.QueueDeath(false);
			this.Dead = false;
			this.AddPercentageHP(100);

			this.World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = AnimationSetKeys.IdleDefault.ID
			}, this);

			//removing tomb
			try
			{
				this.GetObjectsInRange<Headstone>(100.0f).Where(h => h.playerIndex == this.PlayerIndex).First().Destroy();
			}
			catch { }
			this.Teleport(spawnPosition);
			this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.ActorGhostedBuff());

			var old_skills = this.SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
			foreach (var skill in old_skills)
			{
				PowerScript power = PowerLoader.CreateImplementationForPowerSNO(skill);
				if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
				{
					this.World.BuffManager.RemoveBuffs(this, power.EvalTag(PowerKeys.SynergyPower));
				}
			}

			this.SetAttributesByItems();
			this.SetAttributesByItemProcs();
			this.SetAttributesByGems();
			this.SetAttributesByItemSets();
			this.SetAttributesByPassives();
			this.SetAttributesByParagon();
			this.SetAttributesSkillSets();

			this.Attributes[GameAttribute.Resource_Cur, this.PrimaryResourceID] = 0f;
			if (this.Toon.Class == ToonClass.DemonHunter)
				this.Attributes[GameAttribute.Resource_Cur, this.SecondaryResourceID] = 0f;
			this.Attributes.BroadcastChangedIfRevealed();

			var skills = this.SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
			var cooldowns = this.World.BuffManager.GetBuffs<CooldownBuff>(this);
			foreach (var skill in skills)
			{
				bool inCooldown = false;
				CooldownBuff skillcd = null;
				foreach (var cooldown in cooldowns)
					if (cooldown.TargetPowerSNO == skill)
					{
						skillcd = cooldown;
						inCooldown = true;
						break;
					}

				if (inCooldown && skillcd != null) skillcd.Extend((int)3 * 60);
				else _StartSkillCooldown(skill, 3f);
			}
			this.Inventory.RefreshInventoryToClient();
			UpdatePercentageHP(PercHPbeforeChange);
		}


		public float Strength
		{
			get
			{
				var baseStrength = 0.0f;

				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Strength)
					baseStrength = Toon.HeroTable.Strength + ((this.Level - 1) * 3);
				else
					baseStrength = Toon.HeroTable.Strength + (this.Level - 1);

				return baseStrength;
			}
		}

		public float TotalStrength
		{
			get
			{
				return this.Attributes[GameAttribute.Strength] + this.Inventory.GetItemBonus(GameAttribute.Strength_Item);
			}
		}

		public float Dexterity
		{
			get
			{
				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Dexterity)
					return Toon.HeroTable.Dexterity + ((this.Level - 1) * 3);
				else
					return Toon.HeroTable.Dexterity + (this.Level - 1);
			}
		}

		public float TotalDexterity
		{
			get
			{
				return this.Attributes[GameAttribute.Dexterity] + this.Inventory.GetItemBonus(GameAttribute.Dexterity_Item);
			}
		}

		public float Vitality
		{
			get
			{
				return Toon.HeroTable.Vitality + ((this.Level - 1) * 2);
			}
		}

		public float TotalVitality
		{
			get
			{
				return this.Attributes[GameAttribute.Vitality] + this.Inventory.GetItemBonus(GameAttribute.Vitality_Item);
			}
		}

		public float Intelligence
		{
			get
			{
				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Intelligence)
					return Toon.HeroTable.Intelligence + ((this.Level - 1) * 3);
				else
					return Toon.HeroTable.Intelligence + (this.Level - 1);
			}
		}

		public float TotalIntelligence
		{
			get
			{
				return this.Attributes[GameAttribute.Intelligence] + this.Inventory.GetItemBonus(GameAttribute.Intelligence_Item);
			}
		}

		public float PrimaryAttribute
		{
			get
			{
				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Strength) return this.TotalStrength;
				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Dexterity) return this.TotalDexterity;
				if (Toon.HeroTable.CoreAttribute == DiIiS_NA.Core.MPQ.FileFormats.GameBalance.PrimaryAttribute.Intelligence) return this.TotalIntelligence;
				return 0f;
			}
		}

		public float DodgeChance
		{
			get
			{
				float dex = this.TotalDexterity;
				float dodgeChance = dex / (250f * this.Attributes[GameAttribute.Level] + dex);

				if (dex > 7500f) dodgeChance += 0.04f;
				else if (dex > 6500f) dodgeChance += 0.02f;
				else if (dex > 5500f) dodgeChance += 0.01f;

				dodgeChance = 1f - (1f - dodgeChance) * (1f - this.Attributes[GameAttribute.Dodge_Chance_Bonus]);

				return Math.Min(dodgeChance, 0.75f);
			}
		}

#endregion

#region saved-data

		private PlayerSavedData GetSavedData()
		{
			var item = StringHashHelper.HashItemName("HealthPotionBottomless");
			
			return new PlayerSavedData()
			{
				HotBarButtons = this.SkillSet.HotBarSkills,
				HotBarButton = new HotbarButtonData { SNOSkill = -1, RuneType = -1, ItemGBId = StringHashHelper.HashItemName("HealthPotionBottomless")//2142362846//this.Toon.DBActiveSkills.PotionGBID
				, ItemAnn = -1 },
				SkillSlotEverAssigned = 0x0F, //0xB4,
				PlaytimeTotal = this.Toon.TimePlayed,
#if DEBUG
				WaypointFlags = 0x0000ffff,
#else
					WaypointFlags = this.World.Game.WaypointFlags,
#endif

				HirelingData = new HirelingSavedData()
				{
					HirelingInfos = this.HirelingInfo,
					ActiveHireling = 0x00000000,
					AvailableHirelings = 0x00000004,
				},

				TimeLastLevel = 0,
				LearnedLore = this.LearnedLore,

				ActiveSkills = this.SkillSet.ActiveSkills,
				snoTraits = this.SkillSet.PassiveSkills,
				GBIDLegendaryPowers = new int[4] { -1, -1, -1, -1 },
				
				SavePointData = new SavePointData { snoWorld = -1, SavepointId = -1, },
				EventFlags = 0
			};
		}

		public SavePointData SavePointData { get; set; }

		public LearnedLore LearnedLore = new LearnedLore()
		{
			Count = 0x00000000,
			m_snoLoreLearned = new int[512]
			 {
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,
				0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000,0x00000000
			 },
		};

		public void SaveStats() //Save 6 primary stats into DB for showing on hero screen
		{
			//Logger.Debug("SaveStats(): Strength {0}", this.Inventory.GetItemBonus(GameAttribute.Strength_Item).ToString("F0"));
			float damageFromWeapon = (this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min_Total, 0) + this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, 0)) * (1f + (this.PrimaryAttribute / 100f));

			float totalDamage =
				(damageFromWeapon
				+ (damageFromWeapon * this.Inventory.GetItemBonus(GameAttribute.Weapon_Crit_Chance) * (1.5f + this.Inventory.GetItemBonus(GameAttribute.Crit_Damage_Percent))))
				* this.Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Total);

			string serialized = "";
			serialized += this.Inventory.GetItemBonus(GameAttribute.Strength_Item).ToString("F0");
			serialized += ";";
			serialized += this.Inventory.GetItemBonus(GameAttribute.Dexterity_Item).ToString("F0");
			serialized += ";";
			serialized += this.Inventory.GetItemBonus(GameAttribute.Intelligence_Item).ToString("F0");
			serialized += ";";
			serialized += this.Inventory.GetItemBonus(GameAttribute.Vitality_Item).ToString("F0");
			serialized += ";";
			serialized += this.Inventory.GetItemBonus(GameAttribute.Armor_Item).ToString("F0");
			serialized += ";";
			serialized += (totalDamage).ToString("F0");
			var dbStats = this.Toon.DBToon;
			dbStats.Stats = serialized;
			this.World.Game.GameDBSession.SessionUpdate(dbStats);
		}

		public List<PlayerQuestRewardHistoryEntry> QuestRewardHistory
		{
			get
			{
				var result = new List<PlayerQuestRewardHistoryEntry>();
				var quests = this.InGameClient.Game.QuestManager.Quests.Where(q => q.Value.Completed == true).ToList();
				foreach (var quest in quests)
				{
					this.InGameClient.SendMessage(new QuestUpdateMessage()
					{
						snoQuest = quest.Key,
						snoLevelArea = -1,
						StepID = quest.Value.Steps.Last().Key,
						DisplayButton = false,
						Failed = false
					});

					result.Add(new PlayerQuestRewardHistoryEntry()
					{
						snoQuest = quest.Key,
						Field1 = 0,
						Field2 = (PlayerQuestRewardHistoryEntry.Difficulty)this.InGameClient.Game.Difficulty
					});
				}
				return result;
			}
		}

		private HirelingInfo[] _hirelingInfo = null;

		public HirelingInfo[] HirelingInfo
		{
			get
			{
				if (_hirelingInfo == null)
				{
					_hirelingInfo = new HirelingInfo[4];
					for (int i = 0; i < 4; i++)
						_hirelingInfo[i] = GetHirelingInfo(i);
				}
				return _hirelingInfo;
			}
			set
			{
				_hirelingInfo = value;
			}
		}

#endregion

#region cooked messages

		public void StopMoving()
		{
			this.World.BroadcastIfRevealed(plr => new ACDTranslateNormalMessage
			{
				ActorId = this.DynamicID(plr),
				Position = this.Position,
				SnapFacing = false,
				MovementSpeed = 0,
				AnimationTag = -1
			}, this);
		}

		public void CheckBonusSets()
		{
			List<DBBonusSets> sets = this.World.Game.GameDBSession.SessionQueryWhere<DBBonusSets>(dbi => dbi.DBAccount.Id == this.Toon.GameAccount.AccountId).ToList();
			foreach (var bonusSet in sets)
			{
				if (this.World.Game.IsHardcore)
				{
					if (bonusSet.ClaimedHardcore) continue;
				}
				else
				{
					if (bonusSet.Claimed) continue;
				}

				//if (!BonusSetsList.CollectionEditions.ContainsKey(bonusSet.SetId)) continue;

				if (bonusSet.SetId == 6 && this.World.Game.IsHardcore) continue;

				//if (!(bonusSet.Claimed || bonusSet.ClaimedHardcore))
				//	BonusSetsList.CollectionEditions[bonusSet.SetId].ClaimOnce(this);

				if (this.World.Game.IsHardcore)
					bonusSet.ClaimedHardcore = true;
				else
				{
					bonusSet.Claimed = true;
					bonusSet.ClaimedToon = this.Toon.DBToon;
				}

				//BonusSetsList.CollectionEditions[bonusSet.SetId].Claim(this);
				this.World.Game.GameDBSession.SessionUpdate(bonusSet);
				//this.InGameClient.SendMessage(new BroadcastTextMessage() { Field0 = "You have been granted with gifts from bonus pack!" });
			}
		}

		public HirelingInfo GetHirelingInfo(int type)
		{
			var query = this.World.Game.GameDBSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == this.Toon.PersistentID && dbh.Class == type).ToList();
			if (query.Count == 0)
			{ //returns empty data
				var hireling_empty = new HirelingInfo { HirelingIndex = type, GbidName = 0x0000, Dead = false, Skill1SNOId = -1, Skill2SNOId = -1, Skill3SNOId = -1, Skill4SNOId = -1, annItems = -1 };
				return hireling_empty;
			}
			var hireling_full = new HirelingInfo
			{
				HirelingIndex = type,
				GbidName = 0x0000,
				Dead = false,
				Skill1SNOId = query.First().Skill1SNOId,
				Skill2SNOId = query.First().Skill2SNOId,
				Skill3SNOId = query.First().Skill3SNOId,
				Skill4SNOId = query.First().Skill4SNOId,
				annItems = -1
			};
			return hireling_full;
		}

		private List<int> Unserialize(string data)
		{
			string[] recparts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			List<int> ret = new List<int>();
			foreach (string recid in recparts)
			{
				ret.Add(Convert.ToInt32(recid, 10));
			}
			return ret;
		}

		private string Serialize(List<int> data)
		{
			string serialized = "";
			foreach (int id in data)
			{
				serialized += id;
				serialized += ";";
			}
			return serialized;
		}

		private List<int> UnserializeBytes(byte[] data)
		{
			return Enumerable.Range(0, data.Length / 4).Select(i => BitConverter.ToInt32(data, i * 4)).ToList();
		}

		private byte[] SerializeBytes(List<int> data)
		{
			return data.SelectMany(BitConverter.GetBytes).ToArray();
		}

		public void LearnRecipe(string artisan, int recipe)
		{
			Logger.Trace("Learning recipe #{0}, Artisan type: {1}", recipe, artisan);
			/*var query = this.World.Game.GameDBSession.SessionQuerySingle<DBCraft>(
					dbi =>
					dbi.DBGameAccount.Id == this.Toon.GameAccount.PersistentID && 
					dbi.Artisan == artisan &&
					dbi.isHardcore == this.World.Game.IsHardcore);*/
			if (artisan == "Blacksmith")
			{
				this.learnedBlacksmithRecipes.Add(recipe);
				this.blacksmith_data.LearnedRecipes = SerializeBytes(this.learnedBlacksmithRecipes);
				this.World.Game.GameDBSession.SessionUpdate(blacksmith_data);
				this.UpdateAchievementCounter(404, 1, 0);
			}
			if (artisan == "Jeweler")
			{
				this.learnedJewelerRecipes.Add(recipe);
				jeweler_data.LearnedRecipes = SerializeBytes(this.learnedJewelerRecipes);
				this.World.Game.GameDBSession.SessionUpdate(jeweler_data);
				this.UpdateAchievementCounter(404, 1, 1);
			}

			this.LoadCrafterData();
		}

		public bool RecipeAvailable(DiIiS_NA.Core.MPQ.FileFormats.GameBalance.RecipeTable recipe_definition)
		{
			if (recipe_definition.Flags == 0) return true;
			return (this.learnedBlacksmithRecipes.Contains(recipe_definition.Hash) || this.learnedJewelerRecipes.Contains(recipe_definition.Hash));
		}

		public PlayerBannerMessage GetPlayerBanner()
		{
			var playerBanner = D3.GameMessage.PlayerBanner.CreateBuilder()
				.SetPlayerIndex((uint)this.PlayerIndex)
				.SetBanner(this.Toon.GameAccount.BannerConfigurationField.Value)
				.Build();

			return new PlayerBannerMessage() { PlayerBanner = playerBanner };
		}

		private List<int> learnedBlacksmithRecipes = new List<int>();
		private List<int> learnedJewelerRecipes = new List<int>();
		private List<int> learnedTransmogs = new List<int>();

		private DBCraft blacksmith_data = null;
		private DBCraft jeweler_data = null;
		private DBCraft mystic_data = null;

		public void LoadCrafterData()
		{
			if (blacksmith_data == null)
			{
				List<DBCraft> craft_data = this.World.Game.GameDBSession.SessionQueryWhere<DBCraft>(dbc => dbc.DBGameAccount.Id == this.Toon.GameAccount.PersistentID);

				blacksmith_data = craft_data.Single(dbc => dbc.Artisan == "Blacksmith" && dbc.isHardcore == this.World.Game.IsHardcore && dbc.isSeasoned == this.World.Game.IsSeasoned);
				jeweler_data = craft_data.Single(dbc => dbc.Artisan == "Jeweler" && dbc.isHardcore == this.World.Game.IsHardcore && dbc.isSeasoned == this.World.Game.IsSeasoned);
				mystic_data = craft_data.Single(dbc => dbc.Artisan == "Mystic" && dbc.isHardcore == this.World.Game.IsHardcore && dbc.isSeasoned == this.World.Game.IsSeasoned);
			}



			D3.ItemCrafting.CrafterData blacksmith = D3.ItemCrafting.CrafterData.CreateBuilder()
				.SetLevel(this.InGameClient.Game.CurrentAct == 3000 ? this.BlacksmithUnlocked == false && blacksmith_data.Level < 1 ? 1 : blacksmith_data.Level : blacksmith_data.Level)
				.SetCooldownEnd(0)
				.AddRangeRecipes(this.UnserializeBytes(blacksmith_data.LearnedRecipes))
				.Build();
			this.learnedBlacksmithRecipes = this.UnserializeBytes(blacksmith_data.LearnedRecipes);
			D3.ItemCrafting.CrafterData jeweler = D3.ItemCrafting.CrafterData.CreateBuilder()
				.SetLevel(this.InGameClient.Game.CurrentAct == 3000 ? this.JewelerUnlocked == false && jeweler_data.Level < 1 ? 1 : jeweler_data.Level : jeweler_data.Level)
				.SetCooldownEnd(0)
				.AddRangeRecipes(this.UnserializeBytes(jeweler_data.LearnedRecipes))
				.Build();
			this.learnedJewelerRecipes = this.UnserializeBytes(jeweler_data.LearnedRecipes);
			D3.ItemCrafting.CrafterData mystic = D3.ItemCrafting.CrafterData.CreateBuilder()
				.SetLevel(this.InGameClient.Game.CurrentAct == 3000 ? this.MysticUnlocked == false && mystic_data.Level < 1 ? 1 : mystic_data.Level : mystic_data.Level)
				.SetCooldownEnd(0)
				.Build();
			
			D3.ItemCrafting.CrafterSavedData transmog = D3.ItemCrafting.CrafterSavedData.CreateBuilder()
				.SetTransmogData(D3.GameBalance.BitPackedGbidArray.CreateBuilder().SetBitfield(ByteString.CopyFrom(mystic_data.LearnedRecipes)))
				//.AddRangeUnlockedTransmogs(this.UnserializeBytes(mystic_data.LearnedRecipes))
				.Build();
			this.learnedTransmogs = this.UnserializeBytes(mystic_data.LearnedRecipes);

			if (this.BlacksmithUnlocked || this.InGameClient.Game.CurrentAct == 3000)
				this.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataBlacksmithInitialMessage) { Data = blacksmith.ToByteArray() });
			
			if (this.JewelerUnlocked || this.InGameClient.Game.CurrentAct == 3000)
				this.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataJewelerInitialMessage) { Data = jeweler.ToByteArray() });
			
			if (this.MysticUnlocked || this.InGameClient.Game.CurrentAct == 3000)
			{
				this.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataMysticInitialMessage) { Data = mystic.ToByteArray() });
				this.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataTransmogInitialMessage) { Data = transmog.ToByteArray() });
			}
		}

		public void LoadCurrencyData()
		{
			int bloodShards = 0;
			if (this.World.Game.IsHardcore)
				bloodShards = this.Toon.GameAccount.HardcoreBloodShards;
			else
				bloodShards = this.Toon.GameAccount.BloodShards;

			this.Inventory.UpdateCurrencies();

		}
		
		public void LoadMailData()
		{
			List<DBMail> mail_data = this.World.Game.GameDBSession.SessionQueryWhere<DBMail>(dbm => dbm.DBToon.Id == this.Toon.PersistentID && dbm.Claimed == false);
			var mails = D3.Items.Mails.CreateBuilder();
			foreach (var mail in mail_data)
			{
				var mail_row = D3.Items.Mail.CreateBuilder()
					.SetAccountTo(this.Toon.D3EntityID)
					.SetAccountFrom(this.Toon.D3EntityID)
					.SetMailId(mail.Id)
					.SetTitle(mail.Title)
					.SetBody(mail.Body);
				if (mail.ItemGBID != -1)
					mail_row.SetAttachments(D3.Items.MailAttachments.CreateBuilder()
						.SetItems(D3.Items.ItemList.CreateBuilder()
							.AddItems(D3.Items.SavedItem.CreateBuilder()
								.SetId(D3.OnlineService.ItemId.CreateBuilder().SetIdLow(0).SetIdHigh(0x3C000002517A294))
								.SetHirelingClass(0)
								.SetItemSlot(0)
								.SetSquareIndex(0)
								.SetUsedSocketCount(0)
								.SetGenerator(D3.Items.Generator.CreateBuilder()
									.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(mail.ItemGBID).SetGameBalanceType(2))
									.SetFlags(2147483647)
									.SetSeed(0)
									.SetDurability(0)
									.SetStackSize(0)
								)
							)
						)
					);
				mails.AddMailsProp(mail_row);
			}
			var mail_contents = D3.GameMessage.MailContents.CreateBuilder()
				.SetAppendMessages(false)
				.SetMails(mails)
				.Build();

			this.InGameClient.SendMessage(new MailDigestMessage() { MailContents = mail_contents }) ;
		}
		//*/
		public void LoadStashIconsData()
		{
			var dbGAcc = this.Toon.GameAccount.DBGameAccount;
			if (dbGAcc.StashIcons == null) return;

			//this.InGameClient.SendMessage(new StashIconStateMessage() { StashIcons = dbGAcc.StashIcons });
		}

		public void NotifyMaintenance()
		{
			if (GameServer.ClientSystem.GameServer.MaintenanceTime > 0 && GameServer.ClientSystem.GameServer.MaintenanceTime > (int)DateTime.Now.ToUnixTime())
				this.InGameClient.SendMessage(new LogoutTickTimeMessage()
				{
					Field0 = false, // true - logout with party?
					Ticks = 0, // delay 1, make this equal to 0 for instant logout
					Field2 = 10000, // delay 2
					Field3 = (GameServer.ClientSystem.GameServer.MaintenanceTime - (int)DateTime.Now.ToUnixTime()) * 60 //maintenance counter
				});
		}

		public void LoadShownTutorials()
		{
			List<byte> tutorials = new List<byte>();
			tutorials.Add(64);
			for (int i = 0; i < 15; i++)
				tutorials.Add(0);
			var seenTutorials = this.Toon.GameAccount.DBGameAccount.SeenTutorials;

			D3.GameMessage.TutorialState state = D3.GameMessage.TutorialState.CreateBuilder()
				.SetSeenTutorials(ByteString.CopyFrom(seenTutorials))
				.Build();
			this.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.TutorialStateMessage) { Data = state.ToByteArray() });
		}

		private List<ulong> _unlockedAchievements = new List<ulong>();
		private List<ulong> _unlockedCriterias = new List<ulong>();

		private Dictionary<ulong, uint> AchievementCounters = new Dictionary<ulong, uint>();

		public int DodgesInARow = 0;
		public int BlocksInARow = 0;

		public void GrantAchievement(ulong id)
		{
			if (_unlockedAchievements.Contains(id)) return;
			if (this.InGameClient.BnetClient.Account.GameAccount.Achievements.Where(a => a.AchievementId == id && a.Completion != -1).Count() > 0) return;
			if (_unlockedAchievements.Contains(id)) return;
			_unlockedAchievements.Add(id);
			try
			{
				var Achievement = AchievementSystem.AchievementManager.GetAchievementById(id);
				long Platinum = -1;
				foreach (var attr in Achievement.AttributesList)
					if (attr.Key == "Reward Currency Quantity")
						Platinum = Int64.Parse(attr.Value);
				this.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Platinum.PlatinumAchievementAwardedMessage
				{
					CurrentPlatinum = this.InGameClient.BnetClient.Account.GameAccount.Platinum,
					idAchievement = id,
					PlatinumIncrement = Platinum
				});
				if (Platinum > 0)
				{
					this.InGameClient.BnetClient.Account.GameAccount.Platinum += (int)Platinum;
					this.Inventory.UpdateCurrencies();
				}
				ClientSystem.GameServer.GSBackend.GrantAchievement(this.Toon.GameAccount.PersistentID, id);
				
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on GrantAchievement(): ");
			}
		}

		public void AddAchievementCounter(ulong id, uint count)
		{
			lock (this.AchievementCounters)
			{
				if (!this.AchievementCounters.ContainsKey(id))
					this.AchievementCounters.Add(id, count);
				else
					this.AchievementCounters[id] += count;
			}
		}

		public void CheckAchievementCounters()
		{
			lock (this.AchievementCounters)
			{
				foreach (var counter in this.AchievementCounters)
				{
					if (counter.Value == 0) continue;
					this.UpdateSingleAchievementCounter(counter.Key, counter.Value);
				}
				this.AchievementCounters.Clear();
			}
		}

		public void GrantCriteria(ulong id)
		{
			if (_unlockedCriterias.Contains(id)) return;
			_unlockedCriterias.Add(id);
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.GrantCriteria(this.Toon.GameAccount.PersistentID, id);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on GrantCriteria(): ");
			}
		}

		public void UpdateQuantity(ulong id, uint counter)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.UpdateQuantity(this.Toon.GameAccount.PersistentID, id, counter);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on UpdateQuantity(): ");
			}
		}

		public void UpdateAchievementCounter(int type, uint addCounter, int comparand = -1, ulong achiId = 0)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.UpdateAchievementCounter(this.Toon.GameAccount.PersistentID, type, addCounter, comparand, achiId);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on UpdateAchievementCounter(): ");
			}
		}

		public void UpdateSingleAchievementCounter(ulong achievementId, uint addCounter)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.UpdateSingleAchievementCounter(this.Toon.GameAccount.PersistentID, achievementId, addCounter);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on UpdateSingleAchievementCounter(): ");
			}
		}

		public void CheckQuestCriteria(int questId)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckQuestCriteria(this.Toon.GameAccount.PersistentID, questId, this.World.Game.Players.Count > 1);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckQuestCriteria(): ");
			}
		}

		public void CheckKillMonsterCriteria(int actorId, int type)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckKillMonsterCriteria(this.Toon.GameAccount.PersistentID, actorId, type, this.World.Game.IsHardcore);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckKillMonsterCriteria(): ");
			}
		}

		public void CheckLevelCap()
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckLevelCap(this.Toon.GameAccount.PersistentID);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckLevelCap(): ");
			}
		}

		public void CheckSalvageItemCriteria(int itemId)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckSalvageItemCriteria(this.Toon.GameAccount.PersistentID, itemId);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckSalvageItemCriteria(): ");
			}
		}

		public void CheckConversationCriteria(int convId)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckConversationCriteria(this.Toon.GameAccount.PersistentID, convId);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckConversationCriteria(): ");
			}
		}

		public void CheckLevelAreaCriteria(int laId)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.CheckLevelAreaCriteria(this.Toon.GameAccount.PersistentID, laId);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on CheckLevelAreaCriteria(): ");
			}
		}

		public void ParagonLevelUp()
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.ParagonLevelUp(this.Toon.GameAccount.PersistentID);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on ParagonLevelUp(): ");
			}
		}

		public void UniqueItemIdentified(ulong itemId)
		{
			try
			{
				GameServer.ClientSystem.GameServer.GSBackend.UniqueItemIdentified(this.Toon.GameAccount.PersistentID, itemId);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Exception on UniqueItemIdentified(): ");
			}
		}

		public void SetProgress(int act, int difficulty)
		{
			if (act > 400) return;
			var dbGAcc = this.World.Game.GameDBSession.SessionGet<DBGameAccount>(this.Toon.GameAccount.PersistentID);
			var progress = dbGAcc.BossProgress;
			if (progress[(act / 100)] == 0xff || progress[(act / 100)] < (byte)difficulty)
			{
				progress[(act / 100)] = (byte)difficulty;

				dbGAcc.BossProgress = progress;
				this.World.Game.GameDBSession.SessionUpdate(dbGAcc);
			}
		}

		public int castingsnopower = -1;

		public void StartCasting(int durationTicks, Action result, int skillsno = -1)
		{
			this.IsCasting = true;
			this.CastResult = result;
			this.Attributes[GameAttribute.Looping_Animation_Start_Time] = this.World.Game.TickCounter;
			this.Attributes[GameAttribute.Looping_Animation_End_Time] = this.World.Game.TickCounter + durationTicks;
			castingsnopower = skillsno;
			if (castingsnopower != -1)
			{
				this.Attributes[GameAttribute.Buff_Icon_Start_Tick0, castingsnopower] = this.World.Game.TickCounter;
				this.Attributes[GameAttribute.Buff_Icon_End_Tick0, castingsnopower] = this.World.Game.TickCounter + durationTicks;
				this.Attributes[GameAttribute.Buff_Icon_Count0, castingsnopower] = 1;
				this.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, castingsnopower] = true;

			}
			this.Attributes.BroadcastChangedIfRevealed();
		}

		public void StopCasting()
		{
			this.IsCasting = false;
			this.Attributes[GameAttribute.Looping_Animation_Start_Time] = -1;
			this.Attributes[GameAttribute.Looping_Animation_End_Time] = -1;
			if (castingsnopower != -1)
			{
				this.Attributes[GameAttribute.Buff_Icon_Start_Tick0, castingsnopower] = -1;
				this.Attributes[GameAttribute.Buff_Icon_End_Tick0, castingsnopower] = -1;
				this.Attributes[GameAttribute.Buff_Icon_Count0, castingsnopower] = 0;
				this.Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, castingsnopower] = false;
			}
			this.Attributes.BroadcastChangedIfRevealed();
		}

		private void UpdateCastState()
		{
			if (this.Attributes[GameAttribute.Looping_Animation_End_Time] <= this.World.Game.TickCounter)
			{
				StopCasting();
				this.CastResult.Invoke();
				this.CastResult = null;
			}
		}

		public void ShowConfirmation(uint actorId, Action result)
		{
			this.ConfirmationResult = result;

			this.InGameClient.SendMessage(new ConfirmMessage()
			{
				ActorID = actorId
			});
		}

#endregion

#region generic properties

		public int ClassSNO
		{
			get
			{

				if (this.Toon.Gender == 0)
				{
					return Toon.HeroTable.SNOMaleActor;
				}
				else
				{
					return Toon.HeroTable.SNOFemaleActor;
				}
			}
		}

		public int AdditionalLootItems
		{
			get
			{
				if (this.World.BuffManager.HasBuff<PowerSystem.Implementations.NephalemValorBuff>(this))
				{
					return Math.Max(this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.NephalemValorBuff>(this).StackCount - 3, 0);
				}
				else return 0;
			}
		}

		public float ModelScale
		{
			get
			{
				switch (this.Toon.Class)
				{
					case ToonClass.Barbarian:
						return 1.2f;
					case ToonClass.Crusader:
						return 1.2f;
					case ToonClass.DemonHunter:
						return 1.35f;
					case ToonClass.Monk:
						return 1.43f;
					case ToonClass.WitchDoctor:
						return 1.1f;
					case ToonClass.Wizard:
						return 1.3f;
				}
				return 1.43f;
			}
		}

		public int PrimaryResourceID
		{
			get
			{
				return (int)Toon.HeroTable.PrimaryResource;
			}
		}

		public int SecondaryResourceID
		{
			get
			{
				return (int)Toon.HeroTable.SecondaryResource;
			}
		}

		public bool IsInTown
		{
			get
			{
				var town_areas = new List<int> { 19947, 168314, 92945, 197101 };
				var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
				var scenes = this.World.QuadTree.Query<Scene>(proximity);
				if (scenes.Count == 0) return false;

				var scene = scenes[0];

				if (scenes.Count == 2) // What if it's a subscene?
				{
					if (scenes[1].ParentChunkID != 0xFFFFFFFF)
						scene = scenes[1];
				}

				return town_areas.Contains(scene.Specification.SNOLevelAreas[0]);
			}
		}

#endregion

#region experience handling

		//Max((Resource_Max + ((Level#NONE - 1) * Resource_Factor_Level) + Resource_Max_Bonus) * (Resource_Max_Percent_Bonus + 1), 0)
		private float GetMaxResource(int resourceId)
		{
			if (resourceId == 2) return 0;
			return (Math.Max((this.Attributes[GameAttribute.Resource_Max, resourceId] + ((this.Attributes[GameAttribute.Level] - 1) * this.Attributes[GameAttribute.Resource_Factor_Level, resourceId]) + this.Attributes[GameAttribute.Resource_Max_Bonus, resourceId]) * (this.Attributes[GameAttribute.Resource_Max_Percent_Bonus, resourceId] + 1), 0));
		}

		public static List<long> LevelBorders = new List<long>{
			0, 280, 2700, 4500, 6600, 9000, 11700, 14000, 16500, 19200, 22100, /* Level 0-10 */
            25200, 28500, 32000, 35700, 39600, 43700, 48000, 52500, 57200, 62100, /* Level 11-20 */
            67200, 72500, 78000, 83700, 89600, 95700, 102000, 108500, 115200, 122100, /* Level 21-30 */
            150000, 157500, 180000, 203500, 228000, 273000, 320000, 369000, 420000, 473000, /* Level 31-40 */
            528000, 585000, 644000, 705000, 768000, 833000, 900000, 1453500, 2080000, 3180000, /* Level 41-50 */
            4050000, 5005000, 6048000, 7980000, 10092000, 12390000, 14880000, 17019000, 20150000, 24586000, /* Level 51-60 */
            27000000, 29400000, 31900000, 39100000, 46800000, 55000000, 63700000, 72900000, 82600000, 100000000, /* Level 61-70 */
        };
		public static List<long> ParagonLevelBorders = new List<long>
		{
			//7200000,8640000,10800000,11520000,12960000,14400000,15840000,17280000,18720000,20160000,21600000,23040000,24480000,25920000,27360000,
			//28800000,30240000,31680000,33120000,34560000,36000000,37440000,38880000,40320,41760000,43200000,41760000,43200000,44640000,46080000,
			//47520000,48960000,50400000,51840000,53280000,54720000,56160000,57600000,59040000,60480000,61920000,63360000,64800000,66240000,67680000,
			//69120000,70560000,72000000,73440000,76320000,77760000,79200000,80640000,82080000,83520000,84960000,86400000,87840000
		};
		public static void GeneratePLB()
		{
			long PreviosExp = 7200000;
			ParagonLevelBorders.Add(7200000);
			for (int i = 0; i < 59; i++)
			{
				PreviosExp += 1440000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 10; i++)
			{
				PreviosExp += 2880000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 3; i++)
			{
				PreviosExp += 5040000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			PreviosExp += 3660000;
			ParagonLevelBorders.Add(PreviosExp);
			for (int i = 0; i < 75; i++)
			{
				PreviosExp += 1020000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 101; i++)
			{
				PreviosExp += 2040000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 100; i++)
			{
				PreviosExp += 4080000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 99; i++)
			{
				PreviosExp += 6120000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 51; i++)
			{
				PreviosExp += 8160000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 50; i++)
			{
				PreviosExp += 20400000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 50; i++)
			{
				PreviosExp += 40800000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 50; i++)
			{
				PreviosExp += 61200000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 50; i++)
			{
				PreviosExp += 81600000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 50; i++)
			{
				PreviosExp += 102000000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			for (int i = 0; i < 1500; i++)
			{
				PreviosExp += 122400000;
				ParagonLevelBorders.Add(PreviosExp);
			}
			long boosterofup = 229500000;
			for (int i = 0; i < 17750; i++)
			{
				boosterofup += 102000;
				PreviosExp += boosterofup;
				ParagonLevelBorders.Add(PreviosExp);
			}
		}
		//public static List<long> ParagonLevelBorders = ((GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][252616].Data).Experience.Select(row => row.DeltaXP).ToList();

		public static int[] LevelUpEffects =
		{
			85186, 85186, 85186, 85186, 85186, 85190, 85190, 85190, 85190, 85190, /* Level 1-10 */
			85187, 85187, 85187, 85187, 85187, 85187, 85187, 85187, 85187, 85187, /* Level 11-20 */
			85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, /* Level 21-30 */
			85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, /* Level 31-40 */
			85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, 85192, /* Level 41-50 */
			85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194, /* Level 51-60 */
			85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194, 85194 /* Level 61-70 */
		};

		public void AddRestExperience()
		{
			long exp_needed = 0;
			if (this.Attributes[GameAttribute.Level] == this.Attributes[GameAttribute.Level_Cap])
				exp_needed = ParagonLevelBorders[this.Attributes[GameAttribute.Alt_Level]];
			else
				exp_needed = LevelBorders[this.Attributes[GameAttribute.Level]];

			this.Attributes[GameAttribute.Rest_Experience_Lo] = Math.Min(this.Attributes[GameAttribute.Rest_Experience_Lo] + (int)(exp_needed / 10), (int)exp_needed);
			this.Attributes[GameAttribute.Rest_Experience_Bonus_Percent] = 0.25f;
			this.Attributes.BroadcastChangedIfRevealed();
		}

		private object _XPlock = new object();

		public void UpdateExp(int addedExp)
		{
			lock (this._XPlock)
			{
				if (this.Dead) return;
				if (this.World.Game.IsHardcore && this.Attributes[GameAttribute.Level] >= 70)
					addedExp *= 5;

				if (this.Attributes[GameAttribute.Alt_Level] >= 515)
				{
					var XPcap = (91.262575239831f * Math.Pow(this.Attributes[GameAttribute.Alt_Level], 3)) - (44301.083380565047f * Math.Pow(this.Attributes[GameAttribute.Alt_Level], 2)) + (3829010.395566940308f * this.Attributes[GameAttribute.Alt_Level]) + 322795582.543823242188f;
					addedExp = (int)((float)(ParagonLevelBorders[this.Attributes[GameAttribute.Alt_Level]] / XPcap) * addedExp);
				}

				if (this.Attributes[GameAttribute.Rest_Experience_Lo] > 0)
				{
					var multipliedExp = (int)Math.Min(addedExp * this.Attributes[GameAttribute.Rest_Experience_Bonus_Percent], this.Attributes[GameAttribute.Rest_Experience_Lo]);
					addedExp += multipliedExp;
					this.Attributes[GameAttribute.Rest_Experience_Lo] -= multipliedExp;
				}

				if (this.Attributes[GameAttribute.Level] == this.Attributes[GameAttribute.Level_Cap])
					this.Attributes[GameAttribute.Alt_Experience_Next_Lo] -= addedExp;
				else
					this.Attributes[GameAttribute.Experience_Next_Lo] -= addedExp;

				// Levelup
				while ((this.Attributes[GameAttribute.Level] >= this.Attributes[GameAttribute.Level_Cap]) ? (this.Attributes[GameAttribute.Alt_Experience_Next_Lo] <= 0) : (this.Attributes[GameAttribute.Experience_Next_Lo] <= 0))
				{

					// No more levelup at Level_Cap
					if (this.Attributes[GameAttribute.Level] >= this.Attributes[GameAttribute.Level_Cap])
					{
						this.ParagonLevel++;
						this.Toon.ParagonLevelUp();
						this.ParagonLevelUp();
						this.Attributes[GameAttribute.Alt_Level]++;
						this.InGameClient.SendMessage(new ParagonLevel()
						{
							PlayerIndex = this.PlayerIndex,
							Level = this.ParagonLevel
						});
						this.Conversations.StartConversation(0x0002A777); //LevelUp Conversation

						this.Attributes[GameAttribute.Alt_Experience_Next_Lo] = this.Attributes[GameAttribute.Alt_Experience_Next_Lo] + (int)ParagonLevelBorders[this.Attributes[GameAttribute.Alt_Level]];
						// On level up, health is set to max
						this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
						// set resources to max as well
						this.Attributes[GameAttribute.Resource_Cur, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = this.Attributes[GameAttribute.Resource_Max_Total, this.Attributes[GameAttribute.Resource_Type_Primary] - 1];
						this.Attributes[GameAttribute.Resource_Cur, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = this.Attributes[GameAttribute.Resource_Max_Total, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1];

						this.ExperienceNext = this.Attributes[GameAttribute.Alt_Experience_Next_Lo];
						this.Attributes.BroadcastChangedIfRevealed();

						this.PlayEffect(Effect.ParagonLevelUp, null, false);
						this.World.PowerManager.RunPower(this, 252038); //g_LevelUp_AA.pow 252038
						return;
					}

					this.Level++;
					this.Attributes[GameAttribute.Level]++;
					this.Toon.LevelUp();
					if ((this.World.Game.MonsterLevel + 1) == this.Attributes[GameAttribute.Level]) //if this is suitable level to update
						this.World.Game.UpdateLevel(this.Attributes[GameAttribute.Level]);

					this.InGameClient.SendMessage(new PlayerLevel()
					{
						PlayerIndex = this.PlayerIndex,
						Level = this.Level
					});
					

					//Test Update Monster Level
					if (this.PlayerIndex == 0)
					{
						this.InGameClient.Game.InitialMonsterLevel = this.Level;
						this.InGameClient.SendMessage(new GameSyncedDataMessage
						{
							SyncedData = new GameSyncedData
							{
								GameSyncedFlags = this.InGameClient.Game.IsSeasoned == true ? this.InGameClient.Game.IsHardcore == true ? 3 : 2 : this.InGameClient.Game.IsHardcore == true ? 1 : 0,
								Act = Math.Min(this.InGameClient.Game.CurrentAct, 3000),       //act id
								InitialMonsterLevel = this.InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
								MonsterLevel = 0x64E4425E, //MonsterLevel
								RandomWeatherSeed = this.InGameClient.Game.WeatherSeed, //RandomWeatherSeed
								OpenWorldMode = this.InGameClient.Game.CurrentAct == 3000 ? 1 : 0, //OpenWorldMode
								OpenWorldModeAct = -1, //OpenWorldModeAct
								OpenWorldModeParam = -1, //OpenWorldModeParam
								OpenWorldTransitionTime = 0x00000064, //OpenWorldTransitionTime
								OpenWorldDefaultAct = 100, //OpenWorldDefaultAct
								OpenWorldBonusAct = -1, //OpenWorldBonusAct
								SNODungeonFinderLevelArea = 0x00000001, //SNODungeonFinderLevelArea
								LootRunOpen = this.InGameClient.Game.GameMode == Game.Mode.Portals ? 0 : -1, //LootRunOpen //0 - Великий Портал
								OpenLootRunLevel = 1, //OpenLootRunLevel
								LootRunBossDead = 0, //LootRunBossDead
								HunterPlayerIdx = 0, //HunterPlayerIdx
								LootRunBossActive = -1, //LootRunBossActive
								TieredLootRunFailed = -1, //TieredLootRunFailed
								LootRunChallengeCompleted = -1, //LootRunChallengeCompleted
								SetDungeonActive = -1, //SetDungeonActive
								Pregame = 0, //Pregame
								PregameEnd = 0, //PregameEnd
								RoundStart = 0, //RoundStart
								RoundEnd = 0, //RoundEnd
								PVPGameOver = 0x0, //PVPGameOver
								field_v273 = 0x0,
								TeamWins = new[] { 0x0, 0x0 }, //TeamWins
								TeamScore = new[] { 0x0, 0x0 }, //TeamScore
								PVPGameResult = new[] { 0x0, 0x0 }, //PVPGameResult
								PartyGuideHeroId = 0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
								TiredRiftPaticipatingHeroID = new long[] { 0x0, 0x0, 0x0, 0x0 }, //TiredRiftPaticipatingHeroID
							}
						});
					}
					this.Conversations.StartConversation(0x0002A777); //LevelUp Conversation

					if (this.Attributes[GameAttribute.Level] >= this.Attributes[GameAttribute.Level_Cap])
					{
						this.Attributes[GameAttribute.Alt_Experience_Next_Lo] = (int)ParagonLevelBorders[this.Toon.ParagonLevel];
						this.Toon.ExperienceNext = (int)ParagonLevelBorders[this.Toon.ParagonLevel];
					}
					else
					{
						this.Attributes[GameAttribute.Experience_Next_Lo] = this.Attributes[GameAttribute.Experience_Next_Lo] + (int)LevelBorders[this.Attributes[GameAttribute.Level]];
						this.Toon.ExperienceNext = this.Attributes[GameAttribute.Experience_Next_Lo];
					}

					// 4 main attributes are incremented according to class
					this.Attributes[GameAttribute.Strength] = this.Strength;
					this.Attributes[GameAttribute.Intelligence] = this.Intelligence;
					this.Attributes[GameAttribute.Vitality] = this.Vitality;
					this.Attributes[GameAttribute.Dexterity] = this.Dexterity;

					// On level up, health is set to max
					this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];

					// force GameAttributeMap to re-calc resources for the active resource types
					this.Attributes[GameAttribute.Resource_Max, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = this.Attributes[GameAttribute.Resource_Max, this.Attributes[GameAttribute.Resource_Type_Primary] - 1];
					this.Attributes[GameAttribute.Resource_Max, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = this.Attributes[GameAttribute.Resource_Max, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1];

					// set resources to max as well
					this.Attributes[GameAttribute.Resource_Cur, this.Attributes[GameAttribute.Resource_Type_Primary] - 1] = this.Attributes[GameAttribute.Resource_Max_Total, this.Attributes[GameAttribute.Resource_Type_Primary] - 1];
					this.Attributes[GameAttribute.Resource_Cur, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1] = this.Attributes[GameAttribute.Resource_Max_Total, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1];

					this.Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(this.Level - 35, 0);

					this.Attributes.BroadcastChangedIfRevealed();

					this.PlayEffect(Effect.LevelUp, null, false);
					this.World.PowerManager.RunPower(this, 85954); //g_LevelUp.pow 85954


					switch (this.Level)
					{
						case 10:
							if (this.World.Game.IsHardcore)
								this.GrantAchievement(74987243307034);
							else
								this.GrantAchievement(74987243307105);
							break;
						case 20:
							if (this.World.Game.IsHardcore)
								this.GrantAchievement(74987243307035);
							else
								this.GrantAchievement(74987243307104);
							break;
						case 30:
							if (this.World.Game.IsHardcore)
								this.GrantAchievement(74987243307036);
							else
								this.GrantAchievement(74987243307103);
							break;
						case 40:
							if (this.World.Game.IsHardcore)
								this.GrantAchievement(74987243307037);
							else
								this.GrantAchievement(74987243307102);
							break;
						case 50:
							if (this.World.Game.IsHardcore)
								this.GrantAchievement(74987243307038);
							else
								this.GrantAchievement(74987243307101);
							if (this.World.Game.IsSeasoned)
								this.GrantCriteria(74987250038929);
							break;
						case 60:
							if (this.World.Game.IsHardcore)
							{
								this.GrantAchievement(74987243307039);
								if (!this.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.HardcoreTormentUnlocked))
									this.Toon.GameAccount.Flags = this.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.HardcoreTormentUnlocked;
							}
							else
							{
								this.GrantAchievement(74987243307100);
								if (!this.Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.TormentUnlocked))
									this.Toon.GameAccount.Flags = this.Toon.GameAccount.Flags | GameAccount.GameAccountFlags.TormentUnlocked;
							}
							this.CheckLevelCap();
							break;
						case 70:
							this.GrantCriteria(74987254853541);
							break;
						default:
							break;
					}
				}

				this.ExperienceNext = (this.Attributes[GameAttribute.Level] == 70 ? this.Attributes[GameAttribute.Alt_Experience_Next_Lo] : this.Attributes[GameAttribute.Experience_Next_Lo]);
				this.Attributes.BroadcastChangedIfRevealed();
				this.Toon.GameAccount.NotifyUpdate();

				//this.Attributes.SendMessage(this.InGameClient, this.DynamicID); kills the player atm
			}
		}

		#endregion

		#region gold, heath-glob collection
		public void VacuumPickupHealthOrb(float radius = -1)
		{
			if (radius == -1)
				radius = this.Attributes[GameAttribute.Gold_PickUp_Radius];
			var itemList = this.GetItemsInRange(radius);
			foreach (Item item in itemList)
			{
				if (Item.IsHealthGlobe(item.ItemType))
				{
					var playersAffected = this.GetPlayersInRange(26f);
					foreach (Player player in playersAffected)
					{
						foreach (Player targetAffected in playersAffected)
						{
							player.InGameClient.SendMessage(new PlayEffectMessage()
							{
								ActorId = targetAffected.DynamicID(player),
								Effect = Effect.HealthOrbPickup
							});
						}

						//every summon and mercenary owned by you must broadcast their green text to you /H_DANILO
						player.AddPercentageHP((int)item.Attributes[GameAttribute.Health_Globe_Bonus_Health]);
						//passive abilities
						if (player.SkillSet.HasPassive(208478)) //wizard PowerHungry
							player.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.HungryBuff());
						if (player.SkillSet.HasPassive(208594)) //wd GruesomeFeast
						{
							player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total, (int)player.Toon.HeroTable.PrimaryResource + 1] * 0.1f);
							player.World.BuffManager.AddBuff(player, player, new PowerSystem.Implementations.GruesomeFeastIntBuff());
						}
						if (player.SkillSet.HasPassive(205205)) //barbarian PoundOfFlesh
							player.AddPercentageHP((int)(item.Attributes[GameAttribute.Health_Globe_Bonus_Health] * 0.5f));
						if (player.SkillSet.HasPassive(155714)) //dh Vengeance
						{
							player.GeneratePrimaryResource(20f);
							player.GenerateSecondaryResource(2f);
						}
					}
					item.Destroy();
				}
			}
		}

		public void VacuumPickup()
		{

			var itemList = this.GetItemsInRange(this.Attributes[GameAttribute.Gold_PickUp_Radius]);
			foreach (Item item in itemList)
			{
				if (Item.IsGold(item.ItemType))
				{
					if (!this.GroundItems.ContainsKey(item.GlobalID)) continue;

					this.InGameClient.SendMessage(new FloatingAmountMessage()
					{
						Place = new WorldPlace()
						{
							Position = this.Position,
							WorldID = this.World.DynamicID(this),
						},

						Amount = item.Attributes[GameAttribute.Gold],
						Type = FloatingAmountMessage.FloatType.Gold,
					});
					this.InGameClient.SendMessage(new PlayEffectMessage()
					{
						ActorId = this.DynamicID(this),
						Effect = Effect.GoldPickup,
						PlayerId = 0
					});
					PlayEffect(Effect.Sound, 36726);
					this.Inventory.PickUpGold(item);
					this.GroundItems.Remove(item.GlobalID);
					item.Destroy();
				}

				else if (Item.IsBloodShard(item.ItemType) || item.ItemDefinition.Name == "HoradricRelic")
				{
					if (!this.GroundItems.ContainsKey(item.GlobalID)) continue;

					this.InGameClient.SendMessage(new FloatingAmountMessage()
					{
						Place = new WorldPlace()
						{
							Position = this.Position,
							WorldID = this.World.DynamicID(this),
						},

						Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
						Type = FloatingAmountMessage.FloatType.BloodStone,
					});

					this.Inventory.PickUpBloodShard(item);
					this.GroundItems.Remove(item.GlobalID);
					item.Destroy();
				}

				else if (item.ItemDefinition.Name == "Platinum")
				{
					
					this.InGameClient.SendMessage(new FloatingAmountMessage()
					{
						Place = new WorldPlace()
						{
							Position = this.Position,
							WorldID = this.World.DynamicID(this),
						},

						Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
						Type = FloatingAmountMessage.FloatType.Platinum,
					});
					PlayEffect(Effect.Sound, 433266);

					this.Inventory.PickUpPlatinum(item);
					this.GroundItems.Remove(item.GlobalID);
					item.Destroy();
				}

				else if (item.ItemDefinition.Name == "Crafting_Looted_Reagent_01")
				{
					/*
					this.InGameClient.SendMessage(new FloatingAmountMessage()
					{
						Place = new WorldPlace()
						{
							Position = this.Position,
							WorldID = this.World.DynamicID(this),
						},

						Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
						Type = (FloatingAmountMessage.FloatType)22,
					});
					//*/
					this.Toon.CraftItem4++;
					this.Inventory.UpdateCurrencies();
					item.Destroy();
				}

				else if (Item.IsHealthGlobe(item.ItemType))
				{
					var playersAffected = this.GetPlayersInRange(26f);
					foreach (Player player in playersAffected)
					{
						foreach (Player targetAffected in playersAffected)
						{
							player.InGameClient.SendMessage(new PlayEffectMessage()
							{
								ActorId = targetAffected.DynamicID(player),
								Effect = Effect.HealthOrbPickup
							});
						}

						//every summon and mercenary owned by you must broadcast their green text to you /H_DANILO
						player.AddPercentageHP((int)item.Attributes[GameAttribute.Health_Globe_Bonus_Health]);
						//passive abilities
						if (player.SkillSet.HasPassive(208478)) //wizard PowerHungry
							player.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.HungryBuff());
						if (player.SkillSet.HasPassive(208594)) //wd GruesomeFeast
						{
							player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total, (int)player.Toon.HeroTable.PrimaryResource + 1] * 0.1f);
							player.World.BuffManager.AddBuff(player, player, new PowerSystem.Implementations.GruesomeFeastIntBuff());
						}
						if (player.SkillSet.HasPassive(205205)) //barbarian PoundOfFlesh
							player.AddPercentageHP((int)(item.Attributes[GameAttribute.Health_Globe_Bonus_Health] * 0.5f));
						if (player.SkillSet.HasPassive(155714)) //dh Vengeance
						{
							player.GeneratePrimaryResource(20f);
							player.GenerateSecondaryResource(2f);
						}
					}
					item.Destroy();
				}

				else if (item.ItemDefinition.Name == "ArcaneGlobe" && this.Toon.Class == ToonClass.Wizard)
				{
					this.GeneratePrimaryResource(50f);
					item.Destroy();
				}

				else if (item.ItemDefinition.Name == "p1_normal_rifts_Orb" || item.ItemDefinition.Name == "p1_tiered_rifts_Orb")
				{
					if (this.InGameClient.Game.ActiveNephalemTimer == true && this.InGameClient.Game.ActiveNephalemKilledMobs == false)
					{
						this.InGameClient.Game.ActiveNephalemProgress += 15f;
						foreach (var plr in this.InGameClient.Game.Players.Values)
						{
							plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DunggeonFinderProgressGlyphPickUp)
							{
								Field0 = this.InGameClient.Game.ActiveNephalemProgress
							});

							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.KillCounterRefresh)
							{

							});
							plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
							{
								Field0 = this.InGameClient.Game.ActiveNephalemProgress
							});
						}
						if (this.InGameClient.Game.ActiveNephalemProgress > 650)
						{
							this.InGameClient.Game.ActiveNephalemKilledMobs = true;
							foreach (var plr in this.InGameClient.Game.Players.Values)
							{
								if (this.InGameClient.Game.NephalemGreater)
								{
									plr.InGameClient.SendMessage(new QuestCounterMessage()
									{
										snoQuest = 0x00052654,
										snoLevelArea = 0x000466E2,
										StepID = 13,
										TaskIndex = 0,
										Checked = 1,
										Counter = 1
									});
									plr.InGameClient.SendMessage(new QuestUpdateMessage()
									{
										snoQuest = 0x00052654,
										snoLevelArea = 0x000466E2,
										StepID = 16,
										DisplayButton = true,
										Failed = false
									});


								}
								else
								{
									plr.InGameClient.SendMessage(new QuestCounterMessage()
									{
										snoQuest = 0x00052654,
										snoLevelArea = 0x000466E2,
										StepID = 1,
										TaskIndex = 0,
										Checked = 1,
										Counter = 1
									});
									plr.InGameClient.SendMessage(new QuestUpdateMessage()
									{
										snoQuest = 0x00052654,
										snoLevelArea = 0x000466E2,
										StepID = 3,
										DisplayButton = true,
										Failed = false
									});
								}

								
								plr.InGameClient.SendMessage(new PlayMusicMessage(Opcodes.PlayMusicMessage)
								{
									SNO = 0x0005BBD8
								});

								plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameChatTextMessage) { Message = "Messages:LR_BossSpawned" });
								plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameTextMessage) { Message = "Messages:LR_BossSpawned" });

							}
							this.StartConversation(this.World, 366542);
							SpawnNephalemBoss(this.World);
							//358489
						}
					}
					item.Destroy();

				}

				else if (item.ItemDefinition.Name == "PowerGlobe_v2_x1_NoFlippy")
				{
					this.World.BuffManager.AddBuff(this, this, new NephalemValorBuff());
					item.Destroy();
				}

				else if (Item.IsPotion(item.ItemType))
				{
					if ((!this.GroundItems.ContainsKey(item.GlobalID) && this.World.Game.Players.Count > 1) || !this.Inventory.HasInventorySpace(item)) continue;
					this.Inventory.PickUp(item);
				}
			}

			//
			foreach (var skill in this.SkillSet.ActiveSkills)
			{
				if (skill.snoSkill == 460757 && skill.snoRune == 3)
				{
					//Play Aura - 472217
					//this.PlayEffectGroup(472217);
					var Fleshes = this.GetActorsInRange<ActorSystem.Implementations.NecromancerFlesh>(15f + (this.Attributes[GameAttribute.Gold_PickUp_Radius] * 0.5f));//454066
					foreach (var flesh in Fleshes)
					{
						this.InGameClient.SendMessage(new EffectGroupACDToACDMessage()
						{
							EffectSNOId = 470480,
							TargetID = this.DynamicID(this),
							ActorID = flesh.DynamicID(this)
						});
						flesh.PlayEffectGroup(470482);
						this.Attributes[GameAttribute.Resource_Cur, (int)this.Toon.HeroTable.PrimaryResource] += 11f;
						this.Attributes.BroadcastChangedIfRevealed();
						flesh.Destroy();
					}
				}
			}
		}

		public Actor SpawnNephalemBoss(World world)
		{
			#region Боссы
			int[] BossSNOs = new int[]
			{
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
				360329, //X1_LR_Boss_Minion_electricEel_B 
			};
			#endregion

			Actor Boss = world.SpawnMonster(BossSNOs[RandomHelper.Next(0, BossSNOs.Length - 1)], this.Position);
			Boss.Attributes[GameAttribute.Bounty_Objective] = true;
			Boss.Attributes[GameAttribute.Is_Loot_Run_Boss] = true;
			Boss.Attributes.BroadcastChangedIfRevealed();


			foreach (var plr in world.Players.Values)
				plr.InGameClient.SendMessage(new WorldSyncedDataMessage()
				{
					WorldID = world.GlobalID,
					SyncedData = new WorldSyncedData()
					{
						SnoWeatherOverride = 362462,
						WeatherIntensityOverride = 0,
						WeatherIntensityOverrideEnd = 0
					}
				});


			return Boss;
		}

		public bool StartConversation(World world, int conversationId)
		{
			foreach (var plr in world.Players)
				plr.Value.Conversations.StartConversation(conversationId);
			return true;
		}

		public void AddPercentageHP(int percentage, bool GuidingLight = false)
		{
			float quantity = (percentage * this.Attributes[GameAttribute.Hitpoints_Max_Total]) / 100;
			this.AddHP(quantity, GuidingLight);
		}

		public void AddPercentageHP(float percentage, bool GuidingLight = false)
		{
			float quantity = (percentage * this.Attributes[GameAttribute.Hitpoints_Max_Total]) / 100;
			this.AddHP(quantity, GuidingLight);
		}

		public override void AddHP(float quantity, bool GuidingLight = false)
		{
			if (this.Dead) return;
			if (quantity == 0) return;
			if (quantity > 0)
			{
				if (this.Attributes[GameAttribute.Hitpoints_Cur] < this.Attributes[GameAttribute.Hitpoints_Max_Total])
				{
					if (this.Toon.Class == ToonClass.Barbarian)
						if (this.SkillSet.HasPassive(205217))
							quantity += 0.01f * this.Attributes[GameAttribute.Health_Globe_Bonus_Health];

					if (GuidingLight)       //Monk -> Guiding Light
					{
						float missingHP = (this.Attributes[GameAttribute.Hitpoints_Max_Total] - this.Attributes[GameAttribute.Hitpoints_Cur]) / this.Attributes[GameAttribute.Hitpoints_Max_Total];
						if (missingHP > 0.05f)
							if (!this.World.BuffManager.HasBuff<PowerSystem.Implementations.GuidingLightBuff>(this))
								this.World.BuffManager.AddBuff(this, this, new PowerSystem.Implementations.GuidingLightBuff(Math.Min(missingHP, 0.3f), TickTimer.WaitSeconds(this.World.Game, 10.0f)));
					}

					this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
						this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
						this.Attributes[GameAttribute.Hitpoints_Max_Total]);

					this.Attributes.BroadcastChangedIfRevealed();
					this.InGameClient.SendMessage(new FloatingNumberMessage
					{
						ActorID = this.DynamicID(this),
						Number = quantity,
						Type = FloatingNumberMessage.FloatType.Green
					});
				}
			}
			else
			{
				this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(
					this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
					0);

				this.Attributes.BroadcastChangedIfRevealed();
			}
		}

		//only for WD passive
		public void RestoreMana(float quantity, int secs)
		{
			this.Attributes[GameAttribute.Resource_Regen_Per_Second, 0] += quantity / secs;
			System.Threading.Tasks.Task.Delay(1000 * secs).ContinueWith(t =>
			{
				this.Attributes[GameAttribute.Resource_Regen_Per_Second, 0] -= quantity / secs;
			});
		}

#endregion

#region Resource Generate/Use

		int _DisciplineSpent = 0;
		int _HatredSpent = 0;
		int _WrathSpent = 0;

		public void GeneratePrimaryResource(float amount)
		{
			if (this.Toon.Class == ToonClass.Barbarian)
				if (this.World.BuffManager.HasBuff<PowerSystem.Implementations.WrathOfTheBerserker.BerserkerBuff>(this))
					this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.WrathOfTheBerserker.BerserkerBuff>(this).GainedFury += amount;

			if (this.Toon.Class == ToonClass.Monk)
				if (this.World.BuffManager.HasBuff<PowerSystem.Implementations.GuardiansPathBuff>(this))  //Monk -> The Guardian's Path 2H
					amount *= 1.35f;

			_ModifyResourceAttribute(this.PrimaryResourceID, amount);
		}

		public void UsePrimaryResource(float amount, bool tick = false)
		{
			amount = Math.Max((amount - this.Attributes[GameAttribute.Resource_Cost_Reduction_Amount]) * (1f - this.Attributes[GameAttribute.Resource_Cost_Reduction_Percent_Total, (int)Toon.HeroTable.PrimaryResource + 1]), 0);
			amount = amount * (1f - DecreaseUseResourcePercent);
			if (this.Toon.Class == ToonClass.Crusader)
			{
				_WrathSpent += (int)amount;
				if (!tick && this.SkillSet.HasPassive(310775))  //Wrathful passive
					this.AddHP(_WrathSpent * 15f * this.Attributes[GameAttribute.Level]);

				//Laws of Hope -> Faith's reward
				if (!tick && this.World.BuffManager.HasBuff<PowerSystem.Implementations.CrusaderLawsOfHope.LawsShieldBuff>(this))
					if (this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.CrusaderLawsOfHope.LawsShieldBuff>(this).HealPerWrath)
						this.AddHP(_WrathSpent * 15f * this.Attributes[GameAttribute.Level]);

				if (_WrathSpent >= 20)              //Akarat Champion -> Fire Starter
					if (!tick && this.World.BuffManager.HasBuff<PowerSystem.Implementations.CrusaderAkaratChampion.AkaratBuff>(this))
						this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.CrusaderAkaratChampion.AkaratBuff>(this).wrathBlast = true;
			}

			if (this.Toon.Class == ToonClass.DemonHunter)
			{
				_HatredSpent += (int)amount;

				if (_HatredSpent >= 150 && _DisciplineSpent >= 50)
					this.GrantAchievement(74987243307068);

				this.AddTimedAction(6f, new Action<int>((q) => _HatredSpent -= (int)amount));
			}

			if (this.Toon.Class == ToonClass.Barbarian)
			{
				if (this.SkillSet.HasPassive(105217) && !tick) //Bloodthirst (Burb)
					this.AddHP(amount * 1.93f * this.Attributes[GameAttribute.Level]);

				if (!tick && this.World.BuffManager.HasBuff<PowerSystem.Implementations.IgnorePain.IgnorePainBuff>(this))
					if (this.Attributes[GameAttribute.Rune_E, 79528] > 0) //IgnorePain
						this.AddHP(amount * 13.76f * this.Attributes[GameAttribute.Level]);
			}

			if (this.Toon.Class == ToonClass.Wizard)
			{
				if (this.World.BuffManager.HasBuff<HungryBuff>(this))   //Power Hungry
				{
					amount = 0f;
					this.World.BuffManager.RemoveStackFromBuff(this, this.World.BuffManager.GetFirstBuff<HungryBuff>(this));
				}
			}

			if (this.Toon.Class == ToonClass.Monk)
			{
				if (this.SkillSet.HasPassive(209250)) //Transcendence (Monk)
					this.AddHP(amount * (50f + (this.Attributes[GameAttribute.Health_Globe_Bonus_Health] * 0.004f)));
			}

			if (this.SkillSet.HasPassive(208628)) //PierceTheVeil (WD)
				amount *= 1.3f;
			if (this.SkillSet.HasPassive(208568)) //BloodRitual (WD)
			{
				amount *= 0.9f;
				this.AddHP(amount * -0.1f);
			}

			if (this.SkillSet.HasPassive(205398) && this.Attributes[GameAttribute.Hitpoints_Cur] < (this.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.35f)) //Relentless (Barbarian)
				amount *= 0.25f;
			_ModifyResourceAttribute(this.PrimaryResourceID, -amount);
		}

		public void GenerateSecondaryResource(float amount)
		{
			_ModifyResourceAttribute(this.SecondaryResourceID, amount);
		}

		public void UseSecondaryResource(float amount)
		{
			amount = Math.Max((amount - this.Attributes[GameAttribute.Resource_Cost_Reduction_Amount]) * (1f - this.Attributes[GameAttribute.Resource_Cost_Reduction_Percent_Total, (int)Toon.HeroTable.SecondaryResource]), 0);

			if (this.SkillSet.HasPassive(155722)) //dh - Perfectionist
				amount *= 0.9f;

			if (this.Toon.Class == ToonClass.DemonHunter)
			{
				_DisciplineSpent += (int)amount;

				if (_HatredSpent >= 150 && _DisciplineSpent >= 50)
					this.GrantAchievement(74987243307068);

				this.AddTimedAction(6f, new Action<int>((q) => _DisciplineSpent -= (int)amount));
			}

			_ModifyResourceAttribute(this.SecondaryResourceID, -amount);
		}

		private void _ModifyResourceAttribute(int resourceID, float amount)
		{
			if (resourceID == -1 || amount == 0) return;
			float current = this.Attributes[GameAttribute.Resource_Cur, resourceID];
			if (amount > 0f)
			{
				this.Attributes[GameAttribute.Resource_Cur, resourceID] = Math.Min(
					this.Attributes[GameAttribute.Resource_Cur, resourceID] + amount,
					this.Attributes[GameAttribute.Resource_Max_Total, resourceID]);
			}
			else
			{
				this.Attributes[GameAttribute.Resource_Cur, resourceID] = Math.Max(
					this.Attributes[GameAttribute.Resource_Cur, resourceID] + amount,
					0f);
			}

			if (current == this.Attributes[GameAttribute.Resource_Cur, resourceID]) return;

			this.Attributes.BroadcastChangedIfRevealed();
		}


		int _fullFuryFirstTick = 0;
		int _ArmorFirstTick = 0;

		private void _UpdateResources()
		{
			// will crash client when loading if you try to update resources too early
			if (this.World == null) return;

			if (this.InGameClient.Game.Paused) return;

			if (!(this.InGameClient.Game.TickCounter % 30 == 0)) return;

			if (this.InGameClient.Game.TickCounter % 60 == 0)
				this.LastSecondCasts = 0;

			if (this.Toon.Class == ToonClass.Barbarian)
			{
				if (this.Attributes[GameAttribute.Resource_Cur, 2] < this.Attributes[GameAttribute.Resource_Max_Total, 2])
					_fullFuryFirstTick = this.InGameClient.Game.TickCounter;

				if ((this.InGameClient.Game.TickCounter - _fullFuryFirstTick) >= 18000)
					this.GrantAchievement(74987243307047);
			}

			if (this.Toon.Class == ToonClass.Wizard)
			{
				if (!this.World.BuffManager.HasBuff<PowerSystem.Implementations.IceArmor.IceArmorBuff>(this) &&
					!this.World.BuffManager.HasBuff<PowerSystem.Implementations.StormArmor.StormArmorBuff>(this) &&
					!this.World.BuffManager.HasBuff<PowerSystem.Implementations.EnergyArmor.EnergyArmorBuff>(this))
					_ArmorFirstTick = this.InGameClient.Game.TickCounter;

				if ((this.InGameClient.Game.TickCounter - _ArmorFirstTick) >= 72000)
					this.GrantAchievement(74987243307588);
			}

			// 1 tick = 1/60s, so multiply ticks in seconds against resource regen per-second to get the amount to update
			float tickSeconds = 1f / 60f * (this.InGameClient.Game.TickCounter - _lastResourceUpdateTick);
			_lastResourceUpdateTick = this.InGameClient.Game.TickCounter;

			GeneratePrimaryResource(Math.Max(tickSeconds * this.Attributes[GameAttribute.Resource_Regen_Total, this.Attributes[GameAttribute.Resource_Type_Primary] - 1], 0));
			GenerateSecondaryResource(Math.Max(tickSeconds * this.Attributes[GameAttribute.Resource_Regen_Total, this.Attributes[GameAttribute.Resource_Type_Secondary] - 1], 0));

			float totalHPregen = //(this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] + 
				this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus]//)
				* (1 + this.Attributes[GameAttribute.Hitpoints_Regen_Bonus_Percent]);
			if (!this.Dead && !this.World.Game.PvP) AddHP(Math.Max(tickSeconds * totalHPregen, 0));

			if (this.Toon.Class == ToonClass.Barbarian)
				if (this.SkillSet.HasPassive(205300)) //barbarian fury
					GeneratePrimaryResource(tickSeconds * 1.80f);
				else
					UsePrimaryResource(tickSeconds * 0.90f, true);
		}

#endregion

#region lore

		/// <summary>
		/// Checks if player has lore
		/// </summary>
		/// <param name="loreSNOId"></param>
		/// <returns></returns>
		public bool HasLore(int loreSNOId)
		{
			return LearnedLore.m_snoLoreLearned.Contains(loreSNOId);
		}

		/// <summary>
		/// Plays lore to player
		/// </summary>
		/// <param name="loreSNOId"></param>
		/// <param name="immediately">if false, lore will have new lore button</param>
		public void PlayLore(int loreSNOId, bool immediately)
		{
			// play lore to player
			InGameClient.SendMessage(new LoreMessage
			{
				LoreSNOId = loreSNOId
			});
			if (!HasLore(loreSNOId))
			{
				AddLore(loreSNOId);
				if (MPQStorage.Data.Assets[SNOGroup.Lore].ContainsKey(loreSNOId))
				{
					this.CheckConversationCriteria((MPQStorage.Data.Assets[SNOGroup.Lore][loreSNOId].Data as DiIiS_NA.Core.MPQ.FileFormats.Lore).SNOConversation);
				}
			}
		}

		/// <summary>
		/// Adds lore to player's state
		/// </summary>
		/// <param name="loreSNOId"></param>
		public void AddLore(int loreSNOId)
		{
			if (this.LearnedLore.Count < this.LearnedLore.m_snoLoreLearned.Length)
			{
				LearnedLore.m_snoLoreLearned[LearnedLore.Count] = loreSNOId;
				LearnedLore.Count++; // Count
				UpdateHeroState();
				Logger.Trace("Learning lore #{0}", loreSNOId);
				var dbToon = this.Toon.DBToon;
				dbToon.Lore = SerializeBytes(LearnedLore.m_snoLoreLearned.Take(LearnedLore.Count).ToList());
				this.World.Game.GameDBSession.SessionUpdate(dbToon);
			}
		}

#endregion

#region StoneOfRecall

		public void EnableStoneOfRecall()
		{
			Attributes[GameAttribute.Skill, 0x0002EC66] = 1;
			Attributes.SendChangedMessage(this.InGameClient);
		}

		public void DisableStoneOfRecall()
		{
			Attributes[GameAttribute.Skill, 0x0002EC66] = 0;
			Attributes.SendChangedMessage(this.InGameClient);
		}

#endregion

#region Minions and Hirelings handling

		public void AddFollower(Actor source)
		{
			//Rangged Power - 30599
			if (source == null) return;

			var minion = new Minion(this.World, source.ActorSNO.Id, this, source.Tags, true);
			minion.SetBrain(new MinionBrain(minion));
			minion.Brain.DeActivate();
			minion.WalkSpeed *= 4;
			minion.Position = this.Position;
			minion.Attributes[GameAttribute.TeamID] = Attributes[GameAttribute.TeamID];
			minion.Attributes[GameAttribute.Untargetable] = true;
			minion.Attributes[GameAttribute.No_Damage] = true;
			minion.Attributes[GameAttribute.Invulnerable] = true;
			minion.Attributes[GameAttribute.TeamID] = 2;
			minion.Attributes[GameAttribute.NPC_Is_Escorting] = true;
			minion.Attributes[GameAttribute.Pet_Creator] = 1;
			minion.Attributes[GameAttribute.Pet_Owner] = 1;
			minion.Attributes[GameAttribute.Pet_Type] = 25;
			
			//*/
			minion.Attributes[GameAttribute.Effect_Owner_ANN] = (int)this.DynamicID(this);
			minion.EnterWorld(minion.Position);
			(minion as Minion).Brain.Activate();

			source.Hidden = true;
			source.SetVisible(false);

			minion.SetVisible(true);
			minion.Hidden = false;

			if (minion.ActorSNO.Id == 4580)
			{
				(minion.Brain as MinionBrain).PresetPowers.Clear();
 				(minion.Brain as MinionBrain).AddPresetPower(30599);
			}

			if ((this.Followers.Count >= 6 && this.ActiveHireling != null) || (this.Followers.Count >= 7))
			{
				if (this.Toon.Class == ToonClass.WitchDoctor)
				{
					this.GrantAchievement(74987243307563);
				}
			}
		}

		public bool HaveFollower(int id)
		{
			return this.Followers.ContainsValue(id);
		}

		public void DestroyFollower(int id)
		{
			if (this.Followers.ContainsValue(id))
			{
				var dynId = this.Followers.Where(x => x.Value == id).First().Key;
				var actor = this.World.GetActorByGlobalId(dynId);
				if (actor != null)
					actor.Destroy();
				this.Followers.Remove(dynId);
			}
		}

		public void SetFollowerIndex(int snoId)
		{
			if (!this.HaveFollower(snoId))
			{
				for (int i = 1; i < 8; i++)
					if (!_followerIndexes.ContainsKey(i))
					{
						_followerIndexes.Add(i, snoId);
						return;
					}
			}
		}

		public void FreeFollowerIndex(int snoId)
		{
			if (!this.HaveFollower(snoId))
			{
				_followerIndexes.Remove(this.FindFollowerIndex(snoId));
			}
		}

		private Dictionary<int, int> _followerIndexes = new Dictionary<int, int>();

		public int FindFollowerIndex(int snoId)
		{
			if (this.HaveFollower(snoId))
			{
				return _followerIndexes.Where(i => i.Value == snoId).FirstOrDefault().Key;
			}
			else return 0;
		}

		public int CountFollowers(int snoId)
		{
			return this.Followers.Values.Where(f => f == snoId).Count();
		}

		public int CountAllFollowers()
		{
			return this.Followers.Count();
		}

		public void DestroyFollowerById(uint dynId)
		{
			if (this.Followers.ContainsKey(dynId))
			{
				var actor = this.World.GetActorByGlobalId(dynId);
				this.Followers.Remove(dynId);
				if (actor != null)
				{
					this.FreeFollowerIndex(actor.ActorSNO.Id);
					actor.Destroy();
				}
			}
		}

		public void DestroyFollowersBySnoId(int snoId)
		{
			var fol_list = this.Followers.Where(f => f.Value == snoId).Select(f => f.Key).ToList();
			foreach (var fol in fol_list)
				this.DestroyFollowerById(fol);
		}

#endregion
	}
}
