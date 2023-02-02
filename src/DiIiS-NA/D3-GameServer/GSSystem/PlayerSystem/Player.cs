//Blizzless Project 2022

using System;
//Blizzless Project 2022
using System.Collections.Generic;
using System.Collections.Immutable;
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
using System.Reflection;
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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans;
using DiIiS_NA.D3_GameServer.GSSystem.PlayerSystem;
using NHibernate.Util;

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem;

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
    public ArtisanType? CurrentArtisan { get; set; }

    /// <summary>
    /// The player's toon.
    /// We need a better name /raist.
    /// </summary>
    public Toon Toon { get; private set; }

    public float DecreaseUseResourcePercent = 0;
    public int Level { get; private set; }
    public int ParagonLevel { get; private set; }
    public long ExperienceNext { get; private set; }
    public List<Actor> Revived = new() { };

    public bool LevelingBoosted { get; set; }

    public int PreSceneId = -1;

    public List<Actor> NecroSkeletons = new() { };
    public bool ActiveSkeletons = false;
    
    public Actor ActiveGolem = null;
    public bool EnableGolem = false;

    public bool IsInPvPWorld
    {
        get => World != null && World.IsPvP;
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
        get => Inventory?.GetGearScore() ?? 0;
        private set { }
    }

    public Actor PlayerDirectBanner = null;

    public uint NewDynamicID(uint globalId, int pIndex = -1)
    {
        lock (RevealedObjects)
        {
            if (pIndex > -1)
                return (uint)pIndex;
            for (uint i = 9; i < 4123; i++)
                if (!RevealedObjects.ContainsValue(i))
                    //Logger.Trace("adding GlobalId {0} -> DynID {1} to player {2}", globalId, i, this.Toon.Name);
                    return i;
            return 0;
        }
    }

    /// <summary>
    /// ActorType = Player.
    /// </summary>
    public override ActorType ActorType => ActorType.Player;

    /// <summary>
    /// Revealed objects to player.
    /// </summary>
    public Dictionary<uint, uint> RevealedObjects = new();

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
        get => _spiritGenHit;

        set
        {
            _spiritGenHit = value;
            if (SkillSet.HasPassive(315271) && _spiritGenHit >= 3) //Mythic Rhythm
            {
                World.BuffManager.AddBuff(this, this, new MythicRhythmBuff());
                _spiritGenHit = 0;
            }
        }
    }

    /// <summary>
    /// NPC currently interacted with
    /// </summary>
    public InteractiveNPC SelectedNPC { get; set; }

    public Dictionary<uint, ActorSno> Followers { get; private set; }
    private Hireling _activeHireling = null;
    private Hireling _questHireling = null;

    public Hireling ActiveHireling
    {
        get => _activeHireling;
        set
        {
            if (value == null)
            {
                HirelingId = null;
                lock (Toon.DBToon)
                {
                    var dbToon = Toon.DBToon;
                    dbToon.ActiveHireling = null;
                    DBSessions.SessionUpdate(dbToon);
                }
            }
            else if (value != _activeHireling)
            {
                HirelingId = value.Attributes[GameAttribute.Hireling_Class];
                lock (Toon.DBToon)
                {
                    var dbToon = Toon.DBToon;
                    dbToon.ActiveHireling = value.Attributes[GameAttribute.Hireling_Class];
                    DBSessions.SessionUpdate(dbToon);
                }
            }

            if (value == _activeHireling && value != null)
                return;

            if (_activeHireling != null) _activeHireling.Dismiss();

            _activeHireling = value;
        }
    }

    public Hireling QuestHireling
    {
        get => _questHireling;
        set
        {
            if (_questHireling != null) _questHireling.Dismiss();
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
    private ushort[] ParagonBonuses;
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
        : base(world,
            bnetToon.Gender == 0
                ? (ActorSno)bnetToon.HeroTable.SNOMaleActor
                : (ActorSno)bnetToon.HeroTable.SNOFemaleActor)
    {
        InGameClient = client;
        PlayerIndex = Interlocked.Increment(ref InGameClient.Game.PlayerIndexCounter);
        PlayerGroupIndex = InGameClient.Game.PlayerGroupIndexCounter;
        Toon = bnetToon;
        LevelingBoosted = Toon.LevelingBoosted;
        var dbToon = Toon.DBToon;
        HirelingId = dbToon.ActiveHireling;
        GBHandle.Type = (int)ActorType.Player;
        GBHandle.GBID = Toon.ClassID;
        Level = dbToon.Level;
        ParagonLevel = Toon.ParagonLevel;
        ExperienceNext = Toon.ExperienceNext;
        ParagonBonuses = dbToon.ParagonBonuses;
        CurrentWingsPowerId = dbToon.WingsActive;

        Field2 = 0x00000009;
        Scale = ModelScale;
        RotationW = 0.05940768f;
        RotationAxis = new Vector3D(0f, 0f, 0.9982339f);
        Field7 = -1;
        NameSNO = ActorSno.__NONE;
        Field10 = 0x0;
        Dead = false;
        EventWeatherEnabled = false;

        var achievements =
            InGameClient.Game.GameDbSession.SessionQueryWhere<DBAchievements>(dba =>
                dba.DBGameAccount.Id == Toon.GameAccount.PersistentID);

        BlacksmithUnlocked = achievements.Any(dba => dba.AchievementId == 74987243307766);
        JewelerUnlocked = achievements.Any(dba => dba.AchievementId == 74987243307780);
        MysticUnlocked = achievements.Any(dba => dba.AchievementId == 74987247205955);

        KanaiUnlocked = achievements.Where(dba => dba.AchievementId == 74987254626662)
            .SelectMany(x => AchievementSystem.AchievementManager.UnserializeBytes(x.Criteria))
            .Any(x => x == unchecked((uint)74987252674266));

        if (Level >= 70)
            GrantCriteria(74987254853541);

        HirelingTemplarUnlocked = achievements.Any(dba => dba.AchievementId == 74987243307073);
        HirelingScoundrelUnlocked = achievements.Any(dba => dba.AchievementId == 74987243307147);
        HirelingEnchantressUnlocked = achievements.Any(dba => dba.AchievementId == 74987243307145);
        SkillSet = new SkillSet(this, Toon.Class, Toon);
        GroundItems = new Dictionary<uint, Item>();
        Followers = new Dictionary<uint, ActorSno>();
        Conversations = new ConversationManager(this);
        ExpBonusData = new ExpBonusData(this);
        SelectedNPC = null;

        _lastResourceUpdateTick = 0;
        SavePointData = new SavePointData() { snoWorld = -1, SavepointId = -1 };

        // Attributes
        if (World.Game.PvP)
            Attributes[GameAttribute.TeamID] = PlayerIndex + 2;
        else
            Attributes[GameAttribute.TeamID] = 2;

        //make sure if greater is not active enable banner.
        if (!World.Game.NephalemGreater) Attributes[GameAttribute.Banner_Usable] = true;
        SetAllStatsInCorrectOrder();
        // Enabled stone of recall
        if (!World.Game.PvP & Toon.StoneOfPortal)
            EnableStoneOfRecall();
        else if (InGameClient.Game.CurrentAct == 3000)
            EnableStoneOfRecall();

        var lores = UnserializeBytes(Toon.DBToon.Lore);
        var num = 0;
        foreach (var lore in lores)
        {
            LearnedLore.m_snoLoreLearned[num] = lore;
            num++;
        }

        LearnedLore.Count = lores.Count();

        Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
        Attributes.BroadcastChangedIfRevealed();
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
        Inventory ??= new Inventory(this);
        SetAttributesByItems(); //needs the Inventory
        SetAttributesByItemProcs();
        SetAttributesByGems();
        SetAttributesByItemSets();
        SkillSet ??= new SkillSet(this, Toon.Class, Toon);
        SetAttributesByPassives(); //needs the SkillSet
        SetAttributesByParagon();
        SetNewAttributes();
        UpdatePercentageHP(PercHPbeforeChange);
    }

    public void SetAttributesSkills()
    {
        //Skills
        Attributes[GameAttribute.SkillKit] = Toon.HeroTable.SNOSKillKit0;

        Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00033C40] = 153;
        Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00033C40] = 3753;
        Attributes[GameAttribute.Buff_Icon_Count0, 0x0006B48E] = 1;
        Attributes[GameAttribute.Buff_Icon_Count0, 0x0004601B] = 1;
        Attributes[GameAttribute.Buff_Icon_Count0, 0x00033C40] = 1;

        Attributes[GameAttribute.Currencies_Discovered] = 0x0011FFF8;

        Attributes[GameAttribute.Skill, 30592] = 1;
        Attributes[GameAttribute.Resource_Degeneration_Prevented] = false;
        Attributes[GameAttribute.Resource_Degeneration_Stop_Point] = 0;
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
        Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0x33C40] = true;
        Attributes[GameAttribute.Buff_Icon_End_Tick0, 0x00033C40] = 0x000003FB;
        Attributes[GameAttribute.Buff_Icon_Start_Tick0, 0x00033C40] = 0x00000077;
        Attributes[GameAttribute.Buff_Icon_Count0, 0x00033C40] = 1;
        Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0xCE11] = true;
        Attributes[GameAttribute.Buff_Icon_Count0, 0x0000CE11] = 1;
        Attributes[GameAttribute.Buff_Visual_Effect, 0xFFFFF] = true;
        //Wings
        if (CurrentWingsPowerId != -1)
        {
            Attributes[GameAttribute.Buff_Exclusive_Type_Active, CurrentWingsPowerId] = true;
            Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, CurrentWingsPowerId] = true;
            Attributes[GameAttribute.Buff_Icon_Start_Tick0, CurrentWingsPowerId] = 0;
            Attributes[GameAttribute.Buff_Icon_End_Tick0, CurrentWingsPowerId] = 100;
            Attributes[GameAttribute.Buff_Icon_Count0, CurrentWingsPowerId] = 1;
        }
    }

    public void SetAttributesDamage()
    {
        Attributes[GameAttribute.Primary_Damage_Attribute] = (int)Toon.HeroTable.CoreAttribute + 1;
        Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap] = 4f;
    }

    public void SetAttributesRessources()
    {
        Attributes[GameAttribute.Resource_Type_Primary] = (int)Toon.HeroTable.PrimaryResource + 1;
        Attributes[GameAttribute.Resource_Max, Attributes[GameAttribute.Resource_Type_Primary] - 1] =
            Toon.HeroTable.PrimaryResourceBase;
        Attributes[GameAttribute.Resource_Max_Bonus, Attributes[GameAttribute.Resource_Type_Primary] - 1] = 0;
        Attributes[GameAttribute.Resource_Factor_Level, Attributes[GameAttribute.Resource_Type_Primary] - 1] =
            Toon.HeroTable.PrimaryResourceFactorLevel;
        Attributes[GameAttribute.Resource_Percent, Attributes[GameAttribute.Resource_Type_Primary] - 1] = 0;
        Attributes[GameAttribute.Resource_Cur, (int)Attributes[GameAttribute.Resource_Type_Primary]] =
            GetMaxResource((int)Attributes[GameAttribute.Resource_Type_Primary] - 1);


        var max = Attributes[GameAttribute.Resource_Max, (int)Attributes[GameAttribute.Resource_Type_Primary] - 1];
        var cur = Attributes[GameAttribute.Resource_Cur, (int)Attributes[GameAttribute.Resource_Type_Primary] - 1];


        Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Attributes[GameAttribute.Resource_Type_Primary] - 1] =
            Toon.HeroTable.PrimaryResourceRegen;
        Attributes[GameAttribute.Resource_Regen_Stop_Regen] = false;
        if (Toon.Class == ToonClass.Barbarian)
            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] = 0;
        else
            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] =
                (int)GetMaxResource((int)Toon.HeroTable.PrimaryResource + 1) * 100;

        if (Toon.HeroTable.SecondaryResource != GameBalance.HeroTable.Resource.None)
        {
            Attributes[GameAttribute.Resource_Type_Secondary] = (int)Toon.HeroTable.SecondaryResource + 1;
            Attributes[GameAttribute.Resource_Max, (int)Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                Toon.HeroTable.SecondaryResourceBase;
            Attributes[GameAttribute.Resource_Max_Bonus, Attributes[GameAttribute.Resource_Type_Secondary] - 1] = 0;
            Attributes[GameAttribute.Resource_Percent, Attributes[GameAttribute.Resource_Type_Secondary] - 1] = 0;
            Attributes[GameAttribute.Resource_Factor_Level,
                    (int)Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                Toon.HeroTable.SecondaryResourceFactorLevel;
            Attributes[GameAttribute.Resource_Cur, (int)Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                GetMaxResource((int)Toon.HeroTable.SecondaryResource + 1);
            Attributes[GameAttribute.Resource_Regen_Per_Second,
                (int)Attributes[GameAttribute.Resource_Type_Secondary] - 1] = Toon.HeroTable.SecondaryResourceRegen;
        }

        Attributes[GameAttribute.Get_Hit_Recovery_Per_Level] = (int)Toon.HeroTable.GetHitRecoveryPerLevel;
        Attributes[GameAttribute.Get_Hit_Recovery_Base] = (int)Toon.HeroTable.GetHitRecoveryBase;

        Attributes[GameAttribute.Get_Hit_Max_Per_Level] = (int)Toon.HeroTable.GetHitMaxPerLevel;
        Attributes[GameAttribute.Get_Hit_Max_Base] = (int)Toon.HeroTable.GetHitMaxBase;
    }

    public void SetAttributesClassSpecific()
    {
        // Class specific
        switch (Toon.Class)
        {
            case ToonClass.Barbarian:
                //scripted //this.Attributes[GameAttribute.Skill_Total, 30078] = 1;  //Fury Trait
                Attributes[GameAttribute.Skill, 30078] = 1;
                Attributes[GameAttribute.Trait, 30078] = 1;
                Attributes[GameAttribute.Buff_Exclusive_Type_Active, 30078] = true;
                Attributes[GameAttribute.Buff_Icon_Count0, 30078] = 1;
                break;
            case ToonClass.DemonHunter:
                break;
            case ToonClass.Crusader:
                Attributes[GameAttribute.Skill, 0x000418F2] = 1;
                Attributes[GameAttribute.Skill, 0x00045CCF] = 1;
                Attributes[GameAttribute.Skill, 0x000564D4] = 1;

                break;
            case ToonClass.Monk:
                //scripted //this.Attributes[GameAttribute.Skill_Total, 0x0000CE11] = 1;  //Spirit Trait
                Attributes[GameAttribute.Skill, 0x0000CE11] = 1;
                Attributes[GameAttribute.Trait, 0x0000CE11] = 1;
                Attributes[GameAttribute.Buff_Exclusive_Type_Active, 0xCE11] = true;
                Attributes[GameAttribute.Buff_Icon_Count0, 0x0000CE11] = 1;
                break;
            case ToonClass.WitchDoctor:
                break;
            case ToonClass.Wizard:
                break;
        }
    }

    public void SetAttributesMovement()
    {
        Attributes[GameAttribute.Movement_Scalar_Cap] = 3f;
        Attributes[GameAttribute.Movement_Scalar] = 1f;
        Attributes[GameAttribute.Walking_Rate] = Toon.HeroTable.WalkingRate;
        Attributes[GameAttribute.Running_Rate] = Toon.HeroTable.RunningRate;
        Attributes[GameAttribute.Experience_Bonus] = 0f;
        Attributes[GameAttribute.Sprinting_Rate] = Toon.HeroTable.SprintRate * 2;
        Attributes[GameAttribute.Strafing_Rate] = Toon.HeroTable.SprintRate * 2;
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
        Attributes[GameAttribute.Disabled] = true; // we should be making use of these ones too /raist.
        Attributes[GameAttribute.Loading] = true;
        Attributes[GameAttribute.Loading_Player_ACD] = PlayerIndex;
        Attributes[GameAttribute.Invulnerable] = true;
        Attributes[GameAttribute.Hidden] = false;
        Attributes[GameAttribute.Immobolize] = true;
        Attributes[GameAttribute.Untargetable] = true;
        Attributes[GameAttribute.CantStartDisplayedPowers] = true;
        Attributes[GameAttribute.IsContentRestrictedActor] = true;
        Attributes[GameAttribute.Cannot_Dodge] = false;
        Attributes[GameAttribute.Trait, 0x0000CE11] = 1;
        Attributes[GameAttribute.TeamID] = 2;
        Attributes[GameAttribute.Stash_Tabs_Purchased_With_Gold] = 5; // what do these do?
        Attributes[GameAttribute.Stash_Tabs_Rewarded_By_Achievements] = 5;
        Attributes[GameAttribute.Backpack_Slots] = 60;
        Attributes[GameAttribute.General_Cooldown] = 0;
    }

    public void SetAttributesByParagon()
    {
        // Until the Paragon 800 should be distributed on the 4 tabs,
        // after that only in the first Core tab.
        var baseParagonPoints = Math.Min(Toon.ParagonLevel, 800);
        var extraParagonPoints = Math.Max(0, Toon.ParagonLevel - 800);
        for (var i = 0; i < 4; i++)
        {
            Attributes[GameAttribute.Paragon_Bonus_Points_Available, i] = baseParagonPoints / 4;
            // Process remainder only for base points.
            if (i < baseParagonPoints % 4) Attributes[GameAttribute.Paragon_Bonus_Points_Available, i]++;
        }

        // First tab of Paragon (Core) - pos 0.
        Attributes[GameAttribute.Paragon_Bonus_Points_Available, 0] += extraParagonPoints;

        var assigned_bonuses = ParagonBonuses;

        var bonus_ids = ItemGenerator.GetParagonBonusTable(Toon.Class);

        foreach (var bonus in bonus_ids)
        {
            var slot_index = bonus.Category * 4 + bonus.Index - 1;

            Attributes[GameAttribute.Paragon_Bonus_Points_Available, bonus.Category] -= assigned_bonuses[slot_index];

            Attributes[GameAttribute.Paragon_Bonus, bonus.Hash] = assigned_bonuses[slot_index];

            float result;
            if (FormulaScript.Evaluate(bonus.AttributeSpecifiers[0].Formula.ToArray(), new ItemRandomHelper(0),
                    out result))
            {
                if (bonus.AttributeSpecifiers[0].AttributeId == 104) //Resistance_All
                {
                    foreach (var damageType in DamageType.AllTypes)
                        Attributes[GameAttribute.Resistance, damageType.AttributeKey] +=
                            result * assigned_bonuses[slot_index];
                }
                else
                {
                    if (bonus.AttributeSpecifiers[0].AttributeId == 133) result = 33f; //Hitpoints_Regen_Per_Second

                    if (bonus.AttributeSpecifiers[0].AttributeId == 342) result = 16.5f; //Hitpoints_On_Hit

                    if (GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] is GameAttributeF)
                    {
                        var attr = GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] as GameAttributeF;
                        if (bonus.AttributeSpecifiers[0].SNOParam != -1)
                            Attributes[attr, bonus.AttributeSpecifiers[0].SNOParam] +=
                                result * assigned_bonuses[slot_index];
                        else
                            Attributes[attr] += result * assigned_bonuses[slot_index];
                    }
                    else if (GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] is GameAttributeI)
                    {
                        var attr = GameAttribute.Attributes[bonus.AttributeSpecifiers[0].AttributeId] as GameAttributeI;
                        if (bonus.AttributeSpecifiers[0].SNOParam != -1)
                            Attributes[attr, bonus.AttributeSpecifiers[0].SNOParam] +=
                                (int)(result * assigned_bonuses[slot_index]);
                        else
                            Attributes[attr] += (int)(result * assigned_bonuses[slot_index]);
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
        var MaxHPOld = Attributes[GameAttribute.Hitpoints_Max_Total];
        var PercentOfOld = Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
        PercHPbeforeChange = Attributes[GameAttribute.Hitpoints_Cur] /
                             (Attributes[GameAttribute.Hitpoints_Max_Total] / 100);
        ;

        var damageAttributeMinValues = new Dictionary<DamageType, float[]>
        {
            { DamageType.Physical, new[] { 2f, 2f } },
            { DamageType.Arcane, new[] { nonPhysDefault, nonPhysDefault } },
            { DamageType.Cold, new[] { nonPhysDefault, nonPhysDefault } },
            { DamageType.Fire, new[] { nonPhysDefault, nonPhysDefault } },
            { DamageType.Holy, new[] { nonPhysDefault, nonPhysDefault } },
            { DamageType.Lightning, new[] { nonPhysDefault, nonPhysDefault } },
            { DamageType.Poison, new[] { nonPhysDefault, nonPhysDefault } }
        };


        foreach (var damageType in DamageType.AllTypes)
        {
            //var weaponDamageMin = damageType.AttributeKey == 0 ? this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) : (this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) + Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, damageType.AttributeKey), damageAttributeMinValues[damageType][0]));
            //var weaponDamageDelta = damageType.AttributeKey == 0 ? this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min, 0) : (this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta, 0) + Math.Max(this.Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey), damageAttributeMinValues[damageType][1]));

            Attributes[GameAttribute.Damage_Weapon_Min, damageType.AttributeKey] =
                Math.Max(Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min_Total, damageType.AttributeKey),
                    damageAttributeMinValues[damageType][0]) +
                Inventory.GetItemBonus(GameAttribute.Damage_Min, damageType.AttributeKey);
            Attributes[GameAttribute.Damage_Weapon_Min, damageType.AttributeKey] -=
                Inventory.AdjustDualWieldMin(damageType); //Damage on weapons should not add when dual-wielding
            Attributes[GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey] =
                Math.Max(Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, damageType.AttributeKey),
                    damageAttributeMinValues[damageType][1]) +
                Inventory.GetItemBonus(GameAttribute.Damage_Delta, damageType.AttributeKey);
            Attributes[GameAttribute.Damage_Weapon_Delta, damageType.AttributeKey] -=
                Inventory.AdjustDualWieldDelta(damageType); //Damage on weapons should not add when dual-wielding

            Attributes[GameAttribute.Damage_Weapon_Bonus_Min, damageType.AttributeKey] = 0f;
            Attributes[GameAttribute.Damage_Weapon_Bonus_Min_X1, damageType.AttributeKey] = 0f;
            Attributes[GameAttribute.Damage_Weapon_Bonus_Delta, damageType.AttributeKey] = 0f;
            Attributes[GameAttribute.Damage_Weapon_Bonus_Delta_X1, damageType.AttributeKey] = 0f;
            Attributes[GameAttribute.Damage_Weapon_Bonus_Flat, damageType.AttributeKey] = 0f;

            Attributes[GameAttribute.Damage_Type_Percent_Bonus, damageType.AttributeKey] =
                Inventory.GetItemBonus(GameAttribute.Damage_Type_Percent_Bonus, damageType.AttributeKey);
            Attributes[GameAttribute.Damage_Dealt_Percent_Bonus, damageType.AttributeKey] =
                Inventory.GetItemBonus(GameAttribute.Damage_Dealt_Percent_Bonus, damageType.AttributeKey);

            Attributes[GameAttribute.Resistance, damageType.AttributeKey] =
                Inventory.GetItemBonus(GameAttribute.Resistance, damageType.AttributeKey);
            Attributes[GameAttribute.Damage_Percent_Reduction_From_Type, damageType.AttributeKey] =
                Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Type, damageType.AttributeKey);
            Attributes[GameAttribute.Amplify_Damage_Type_Percent, damageType.AttributeKey] =
                Inventory.GetItemBonus(GameAttribute.Amplify_Damage_Type_Percent, damageType.AttributeKey);
        }

        for (var i = 0; i < 4; i++)
            Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, i] =
                Inventory.GetItemBonus(GameAttribute.Damage_Percent_Bonus_Vs_Monster_Type, i);


        Attributes[GameAttribute.Resistance_All] = Inventory.GetItemBonus(GameAttribute.Resistance_All);
        Attributes[GameAttribute.Resistance_Percent_All] = Inventory.GetItemBonus(GameAttribute.Resistance_Percent_All);
        Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] =
            Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Melee);
        Attributes[GameAttribute.Damage_Percent_Reduction_From_Ranged] =
            Inventory.GetItemBonus(GameAttribute.Damage_Percent_Reduction_From_Ranged);

        Attributes[GameAttribute.Thorns_Fixed, 0] = Inventory.GetItemBonus(GameAttribute.Thorns_Fixed, 0);

        //this.Attributes[GameAttribute.Armor_Item_Percent] = this.Inventory.GetItemBonus(GameAttribute.Armor_Item_Percent);
        var allStatsBonus = Inventory.GetItemBonus(GameAttribute.Stats_All_Bonus); // / 1065353216;
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
        Attributes[GameAttribute.Strength_Item] = Inventory.GetItemBonus(GameAttribute.Strength_Item); // / 1065353216;
        Attributes[GameAttribute.Strength_Item] += allStatsBonus;
        Attributes[GameAttribute.Vitality_Item] = Inventory.GetItemBonus(GameAttribute.Vitality_Item); // / 1065353216;
        Attributes[GameAttribute.Vitality_Item] += allStatsBonus;
        Attributes[GameAttribute.Dexterity_Item] =
            Inventory.GetItemBonus(GameAttribute.Dexterity_Item); // / 1065353216;
        Attributes[GameAttribute.Dexterity_Item] += allStatsBonus;
        Attributes[GameAttribute.Intelligence_Item] =
            Inventory.GetItemBonus(GameAttribute
                .Intelligence_Item); // / 1065353216; //I know that's wild, but client can't display it properly...
        Attributes[GameAttribute.Intelligence_Item] += allStatsBonus;
        //*/

        //this.Attributes[GameAttribute.Cube_Enchanted_Strength_Item] = 0;
        Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;


        Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] =
            Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus);
        Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Item] =
            Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Percent_Bonus_Item);
        Attributes[GameAttribute.Hitpoints_Max_Bonus] = Inventory.GetItemBonus(GameAttribute.Hitpoints_Max_Bonus);


        Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] =
            Inventory.GetItemBonus(GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource);

        Attributes[GameAttribute.Attacks_Per_Second] = Inventory.GetAPS();

        Attributes[GameAttribute.Attacks_Per_Second_Percent] =
            Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item_Percent) +
            Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Percent);
        Attributes[GameAttribute.Attacks_Per_Second_Bonus] =
            Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item_Bonus);
        Attributes[GameAttribute.Attacks_Per_Second_Item] =
            Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Item);
        var a = Attributes[GameAttribute.Attacks_Per_Second];
        var b = Attributes[GameAttribute.Attacks_Per_Second_Percent];
        var c = Attributes[GameAttribute.Attacks_Per_Second_Bonus];
        var d = Attributes[GameAttribute.Attacks_Per_Second_Item];
        var e = Attributes[GameAttribute.Attacks_Per_Second_Item_CurrentHand];
        var f = Attributes[GameAttribute.Attacks_Per_Second_Item_Bonus];
        var g = Attributes[GameAttribute.Attacks_Per_Second_Percent_Subtotal];
        var h = Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap];
        var j = Attributes[GameAttribute.Attacks_Per_Second_Percent_Uncapped];
        var k = Attributes[GameAttribute.Attacks_Per_Second_Percent_Reduction];
        var o = Attributes[GameAttribute.Attacks_Per_Second_Total];

        if (Attributes[GameAttribute.Attacks_Per_Second_Total] < 1)
            Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
        Attributes[GameAttribute.Crit_Percent_Bonus_Capped] =
            Inventory.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Capped);
        Attributes[GameAttribute.Weapon_Crit_Chance] = 0.05f + Inventory.GetItemBonus(GameAttribute.Weapon_Crit_Chance);
        Attributes[GameAttribute.Crit_Damage_Percent] =
            0.5f + Inventory.GetItemBonus(GameAttribute.Crit_Damage_Percent);

        Attributes[GameAttribute.Splash_Damage_Effect_Percent] =
            Inventory.GetItemBonus(GameAttribute.Splash_Damage_Effect_Percent);

        Attributes[GameAttribute.On_Hit_Fear_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Fear_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Fear_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Stun_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Stun_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Stun_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Blind_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Blind_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Blind_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Freeze_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Freeze_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Freeze_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Chill_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Chill_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Chill_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Slow_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Slow_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Slow_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Immobilize_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Immobilize_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Immobilize_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Knockback_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Knockback_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Knockback_Proc_Chance);
        Attributes[GameAttribute.On_Hit_Bleed_Proc_Chance] =
            Inventory.GetItemBonus(GameAttribute.On_Hit_Bleed_Proc_Chance) +
            Inventory.GetItemBonus(GameAttribute.Weapon_On_Hit_Bleed_Proc_Chance);

        Attributes[GameAttribute.Running_Rate] =
            Toon.HeroTable.RunningRate + Inventory.GetItemBonus(GameAttribute.Running_Rate);
        Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] =
            Inventory.GetItemBonus(GameAttribute.Movement_Scalar_Uncapped_Bonus);
        Attributes[GameAttribute.Movement_Scalar] = Inventory.GetItemBonus(GameAttribute.Movement_Scalar) + 1.0f;

        //this.Attributes[GameAttribute.Magic_Find] = this.Inventory.GetItemBonus(GameAttribute.Magic_Find);
        Attributes[GameAttribute.Magic_Find] = Inventory.GetMagicFind();
        //this.Attributes[GameAttribute.Gold_Find] = this.Inventory.GetItemBonus(GameAttribute.Gold_Find);
        Attributes[GameAttribute.Gold_Find] = Inventory.GetGoldFind();

        Attributes[GameAttribute.Gold_PickUp_Radius] = 5f + Inventory.GetItemBonus(GameAttribute.Gold_PickUp_Radius);

        Attributes[GameAttribute.Experience_Bonus] = Inventory.GetItemBonus(GameAttribute.Experience_Bonus);
        Attributes[GameAttribute.Experience_Bonus_Percent] =
            Inventory.GetItemBonus(GameAttribute.Experience_Bonus_Percent);

        Attributes[GameAttribute.Resistance_Freeze] = Inventory.GetItemBonus(GameAttribute.Resistance_Freeze);
        Attributes[GameAttribute.Resistance_Penetration] = Inventory.GetItemBonus(GameAttribute.Resistance_Penetration);
        Attributes[GameAttribute.Resistance_Percent] = Inventory.GetItemBonus(GameAttribute.Resistance_Percent);
        Attributes[GameAttribute.Resistance_Root] = Inventory.GetItemBonus(GameAttribute.Resistance_Root);
        Attributes[GameAttribute.Resistance_Stun] = Inventory.GetItemBonus(GameAttribute.Resistance_Stun);
        Attributes[GameAttribute.Resistance_StunRootFreeze] =
            Inventory.GetItemBonus(GameAttribute.Resistance_StunRootFreeze);

        Attributes[GameAttribute.Dodge_Chance_Bonus] = Inventory.GetItemBonus(GameAttribute.Dodge_Chance_Bonus);

        Attributes[GameAttribute.Block_Amount_Item_Min] = Inventory.GetItemBonus(GameAttribute.Block_Amount_Item_Min);
        Attributes[GameAttribute.Block_Amount_Item_Delta] =
            Inventory.GetItemBonus(GameAttribute.Block_Amount_Item_Delta);
        Attributes[GameAttribute.Block_Amount_Bonus_Percent] =
            Inventory.GetItemBonus(GameAttribute.Block_Amount_Bonus_Percent);
        Attributes[GameAttribute.Block_Chance] = Inventory.GetItemBonus(GameAttribute.Block_Chance_Item_Total);

        Attributes[GameAttribute.Power_Cooldown_Reduction_Percent] = 0;
        Attributes[GameAttribute.Health_Globe_Bonus_Health] =
            Inventory.GetItemBonus(GameAttribute.Health_Globe_Bonus_Health);

        Attributes[GameAttribute.Hitpoints_Regen_Per_Second] =
            Inventory.GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second) + Toon.HeroTable.GetHitRecoveryBase +
            Toon.HeroTable.GetHitRecoveryPerLevel * Level;

        Attributes[GameAttribute.Resource_Cost_Reduction_Percent_All] =
            Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent_All);
        Attributes[GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.PrimaryResource] =
            Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.PrimaryResource);
        Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] =
            Toon.HeroTable.PrimaryResourceRegen + Inventory.GetItemBonus(GameAttribute.Resource_Regen_Per_Second,
                (int)Toon.HeroTable.PrimaryResource);
        Attributes[GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.PrimaryResource] =
            Inventory.GetItemBonus(GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.PrimaryResource);

        Attributes[GameAttribute.Resource_Cost_Reduction_Percent, (int)Toon.HeroTable.SecondaryResource] =
            Inventory.GetItemBonus(GameAttribute.Resource_Cost_Reduction_Percent,
                (int)Toon.HeroTable.SecondaryResource);
        Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.SecondaryResource] =
            Toon.HeroTable.SecondaryResourceRegen + Inventory.GetItemBonus(GameAttribute.Resource_Regen_Per_Second,
                (int)Toon.HeroTable.SecondaryResource);
        Attributes[GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.SecondaryResource] =
            Inventory.GetItemBonus(GameAttribute.Resource_Regen_Bonus_Percent, (int)Toon.HeroTable.SecondaryResource);

        Attributes[GameAttribute.Resource_On_Hit] = 0;
        Attributes[GameAttribute.Resource_On_Hit, 0] = Inventory.GetItemBonus(GameAttribute.Resource_On_Hit, 0);
        Attributes[GameAttribute.Resource_On_Crit, 1] = Inventory.GetItemBonus(GameAttribute.Resource_On_Crit, 1);

        Attributes[GameAttribute.Steal_Health_Percent] =
            Inventory.GetItemBonus(GameAttribute.Steal_Health_Percent) * 0.1f;
        Attributes[GameAttribute.Hitpoints_On_Hit] = Inventory.GetItemBonus(GameAttribute.Hitpoints_On_Hit);
        Attributes[GameAttribute.Hitpoints_On_Kill] = Inventory.GetItemBonus(GameAttribute.Hitpoints_On_Kill);
        Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] =
            Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Percent_Bonus);
        Attributes[GameAttribute.Damage_Percent_Bonus_Vs_Elites] =
            Inventory.GetItemBonus(GameAttribute.Damage_Percent_Bonus_Vs_Elites);
        //this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All_Capped] = 0.5f;
        //this.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_Cap] = 0.5f;
        Attributes[GameAttribute.Power_Cooldown_Reduction_Percent] = 0.5f;
        Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] =
            Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction_Percent_All);
        Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] =
            Inventory.GetItemBonus(GameAttribute.Crit_Percent_Bonus_Uncapped);

        //this.Attributes[GameAttribute.Projectile_Speed] = 0.3f;

        switch (Toon.Class)
        {
            case ToonClass.Barbarian:
                Attributes[GameAttribute.Power_Resource_Reduction, 80028] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 80028);
                Attributes[GameAttribute.Power_Resource_Reduction, 70472] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 70472);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 79242] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 79242);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 80263] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 80263);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 78548] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 78548);
                Attributes[GameAttribute.Power_Resource_Reduction, 93885] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 93885);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 86989] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 86989);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 96296] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 96296);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 109342] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 109342);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 159169] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 159169);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 93885] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 93885);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69979] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69979);
                break;
            case ToonClass.DemonHunter:
                Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.SecondaryResource] =
                    Inventory.GetItemBonus(GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.SecondaryResource);
                Attributes[GameAttribute.Bow] = Inventory.GetItemBonus(GameAttribute.Bow);
                Attributes[GameAttribute.Crossbow] = Inventory.GetItemBonus(GameAttribute.Crossbow);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 129215] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 129215);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 134209] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 134209);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 77552] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 77552);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 75873] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 75873);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 86610] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 86610);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 131192] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 131192);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 131325] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 131325);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 77649] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 77649);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 134030] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 134030);
                Attributes[GameAttribute.Power_Resource_Reduction, 129214] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 129214);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 75301] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 75301);
                Attributes[GameAttribute.Power_Resource_Reduction, 131366] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 131366);
                Attributes[GameAttribute.Power_Resource_Reduction, 129213] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 129213);
                break;
            case ToonClass.Monk:
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 95940] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 95940);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96019] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96019);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96311] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96311);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 97328] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 97328);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 96090] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 96090);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 97110] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 97110);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 121442] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 121442);
                Attributes[GameAttribute.Power_Resource_Reduction, 111676] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 111676);
                Attributes[GameAttribute.Power_Resource_Reduction, 223473] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 223473);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 96033] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 96033);
                break;
            case ToonClass.WitchDoctor:
                Attributes[GameAttribute.Power_Resource_Reduction, 105963] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 105963);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 103181] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 103181);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 106465] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 106465);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 83602] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 83602);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 108506] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 108506);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69866] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69866);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 69867] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 69867);
                Attributes[GameAttribute.Power_Resource_Reduction, 74003] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 74003);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 70455] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 103181);
                Attributes[GameAttribute.Power_Resource_Reduction, 67567] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 67567);
                Attributes[GameAttribute.Power_Cooldown_Reduction, 134837] =
                    Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 134837);
                Attributes[GameAttribute.Power_Cooldown_Reduction, 67600] =
                    Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 67600);
                Attributes[GameAttribute.Power_Cooldown_Reduction, 102573] =
                    Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 102573);
                Attributes[GameAttribute.Power_Cooldown_Reduction, 30624] =
                    Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 30624);
                break;
            case ToonClass.Wizard:
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 30744] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 30744);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 30783] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 30783);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 71548] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 71548);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 1765] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 1765);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 30668] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 30668);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 77113] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 77113);
                Attributes[GameAttribute.Power_Resource_Reduction, 91549] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 91549);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 87525] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 87525);
                Attributes[GameAttribute.Power_Crit_Percent_Bonus, 93395] =
                    Inventory.GetItemBonus(GameAttribute.Power_Crit_Percent_Bonus, 93395);
                Attributes[GameAttribute.Power_Resource_Reduction, 134456] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 134456);
                Attributes[GameAttribute.Power_Resource_Reduction, 30725] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 30725);
                Attributes[GameAttribute.Power_Duration_Increase, 30680] =
                    Inventory.GetItemBonus(GameAttribute.Power_Duration_Increase, 30680);
                Attributes[GameAttribute.Power_Resource_Reduction, 69190] =
                    Inventory.GetItemBonus(GameAttribute.Power_Resource_Reduction, 69190);
                Attributes[GameAttribute.Power_Cooldown_Reduction, 168344] =
                    Inventory.GetItemBonus(GameAttribute.Power_Cooldown_Reduction, 168344);
                Attributes[GameAttribute.Power_Damage_Percent_Bonus, 71548] =
                    Inventory.GetItemBonus(GameAttribute.Power_Damage_Percent_Bonus, 71548);
                break;
        }
    }

    public void UpdatePercentageHP(float percent)
    {
        var m = Attributes[GameAttribute.Hitpoints_Max_Total];
        Attributes[GameAttribute.Hitpoints_Cur] = percent * Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
        Attributes.BroadcastChangedIfRevealed();
    }

    public void UpdatePercentageHP()
    {
    }

    public void SetAttributesByGems()
    {
        Inventory.SetGemBonuses();
    }

    public void SetAttributesByItemProcs()
    {
        Attributes[GameAttribute.Item_Power_Passive, 248776] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 248776); //cluck
        Attributes[GameAttribute.Item_Power_Passive, 248629] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 248629); //death laugh
        Attributes[GameAttribute.Item_Power_Passive, 247640] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 247640); //gore1
        Attributes[GameAttribute.Item_Power_Passive, 249963] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 249963); //gore2
        Attributes[GameAttribute.Item_Power_Passive, 249954] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 249954); //gore3
        Attributes[GameAttribute.Item_Power_Passive, 246116] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 246116); //butcher
        Attributes[GameAttribute.Item_Power_Passive, 247724] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 247724); //plum!
        Attributes[GameAttribute.Item_Power_Passive, 245741] =
            Inventory.GetItemBonus(GameAttribute.Item_Power_Passive, 245741); //weee!
    }

    public void SetAttributesByItemSets()
    {
        Attributes[GameAttribute.Strength] = Strength;
        Attributes[GameAttribute.Dexterity] = Dexterity;
        Attributes[GameAttribute.Vitality] = Vitality;
        Attributes[GameAttribute.Intelligence] = Intelligence;
        Attributes.BroadcastChangedIfRevealed();

        Inventory.SetItemSetBonuses();
    }

    public void SetAttributesByPassives() //also reapplies synergy buffs
    {
        // Class specific
        Attributes[GameAttribute.Damage_Percent_All_From_Skills] = 0;
        Attributes[GameAttribute.Allow_2H_And_Shield] = false;
        Attributes[GameAttribute.Cannot_Dodge] = false;

        foreach (var passiveId in SkillSet.PassiveSkills)
            switch (Toon.Class)
            {
                case ToonClass.Barbarian:
                    switch (passiveId)
                    {
                        case 217819: //NervesOfSteel
                            Attributes[GameAttribute.Armor_Item] += Attributes[GameAttribute.Vitality_Total] * 0.50f;
                            break;
                        case 205228: //Animosity
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 20;
                            Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] =
                                Attributes[GameAttribute.Resource_Regen_Per_Second,
                                    (int)Toon.HeroTable.PrimaryResource] * 1.1f;
                            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceBase + Attributes[GameAttribute.Resource_Max_Bonus,
                                    (int)Toon.HeroTable.PrimaryResource];
                            break;
                        case 205848: //ToughAsNails
                            Attributes[GameAttribute.Armor_Item] *= 1.25f;
                            break;
                        case 205707: //Juggernaut
                            Attributes[GameAttribute.CrowdControl_Reduction] += 0.3f;
                            break;
                        case 206147: //WeaponsMaster
                            var weapon = Inventory.GetEquippedWeapon();
                            if (weapon != null)
                            {
                                var name = weapon.ItemDefinition.Name.ToLower();
                                if (name.Contains("sword") || name.Contains("dagger"))
                                    Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.08f;
                                else if (name.Contains("axe") || name.Contains("mace"))
                                    Attributes[GameAttribute.Weapon_Crit_Chance] += 0.05f;
                                else if (name.Contains("spear") || name.Contains("polearm"))
                                    Attributes[GameAttribute.Attacks_Per_Second] *= 1.08f;
                                else if (name.Contains("mighty"))
                                    Attributes[GameAttribute.Resource_On_Hit] += 1f;
                            }

                            break;
                    }

                    break;
                case ToonClass.DemonHunter:
                    switch (passiveId)
                    {
                        case 155714: //Blood Vengeance
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 25;
                            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceBase + Attributes[GameAttribute.Resource_Max_Bonus,
                                    (int)Toon.HeroTable.PrimaryResource];
                            break;
                        case 210801: //Brooding
                            Attributes[GameAttribute.Hitpoints_Regen_Per_Second] +=
                                Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
                            break;
                        case 155715: //Sharpshooter
                            World.BuffManager.RemoveBuffs(this, 155715);
                            World.BuffManager.AddBuff(this, this, new SharpshooterBuff());
                            break;
                        case 324770: //Awareness
                            World.BuffManager.RemoveBuffs(this, 324770);
                            World.BuffManager.AddBuff(this, this, new AwarenessBuff());
                            break;
                        case 209734: //Archery
                            var weapon = Inventory.GetEquippedWeapon();
                            if (weapon != null)
                            {
                                var name = weapon.ItemDefinition.Name.ToLower();
                                if (name.Contains("xbow"))
                                    Attributes[GameAttribute.Crit_Damage_Percent] += 0.5f;
                                if (name.Contains("handxbow"))
                                {
                                    Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] += 0.05f;
                                }
                                else if (name.Contains("xbow"))
                                {
                                    Attributes[GameAttribute.Resource_Regen_Per_Second,
                                        (int)Toon.HeroTable.PrimaryResource] += 1f;
                                }
                                else if (name.Contains("bow"))
                                {
                                    Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += 0.08f;
                                    Attributes[GameAttribute.Damage_Percent_All_From_Skills] = 0.08f;
                                }
                            }

                            break;
                        case 155722: //Perfectionist
                            Attributes[GameAttribute.Armor_Item] *= 1.1f;
                            Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus] += 0.1f;
                            Attributes[GameAttribute.Resistance_Percent_All] += 0.1f;
                            break;
                    }

                    break;
                case ToonClass.Monk:
                    switch (passiveId)
                    {
                        case 209029: //FleetFooted
                            Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += 0.1f;
                            break;
                        case 209027: //ExaltedSoul
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 100;
                            //this.Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] = Toon.HeroTable.PrimaryResourceMax + this.Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource];
                            Attributes[GameAttribute.Resource_Regen_Per_Second, 3] += 2f;
                            break;
                        case 209628: //SeizeTheInitiative
                            Attributes[GameAttribute.Armor_Item] += Attributes[GameAttribute.Dexterity_Total] * 0.3f;
                            break;
                        case 209622: //SixthSense
                            Attributes[GameAttribute.Dodge_Chance_Bonus] += Math.Min(
                                (Attributes[GameAttribute.Weapon_Crit_Chance] +
                                 Attributes[GameAttribute.Crit_Percent_Bonus_Capped] +
                                 Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped]) * 0.425f, 0.15f);
                            break;
                        case 209104: //BeaconOfYtar
                            Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] += 0.20f;
                            break;
                        case 209656: //OneWithEverything
                            var maxResist = Math.Max(
                                Math.Max(
                                    Math.Max(Attributes[GameAttribute.Resistance, DamageType.Physical.AttributeKey],
                                        Attributes[GameAttribute.Resistance, DamageType.Cold.AttributeKey]),
                                    Attributes[GameAttribute.Resistance, DamageType.Fire.AttributeKey]),
                                Math.Max(
                                    Math.Max(Attributes[GameAttribute.Resistance, DamageType.Arcane.AttributeKey],
                                        Attributes[GameAttribute.Resistance, DamageType.Holy.AttributeKey]),
                                    Math.Max(Attributes[GameAttribute.Resistance, DamageType.Lightning.AttributeKey],
                                        Attributes[GameAttribute.Resistance, DamageType.Poison.AttributeKey]))
                            );
                            foreach (var damageType in DamageType.AllTypes)
                                Attributes[GameAttribute.Resistance, damageType.AttributeKey] = maxResist;
                            break;
                        case 209812: //TheGuardiansPath
                            try
                            {
                                var weapon = Inventory.GetEquippedWeapon();
                                if (weapon != null && Inventory.GetEquippedOffHand() != null)
                                {
                                    Attributes[GameAttribute.Dodge_Chance_Bonus] += 0.15f;
                                }
                                else if (weapon.ItemDefinition.Name.ToLower().Contains("2h"))
                                {
                                    World.BuffManager.RemoveBuffs(this, 209812);
                                    World.BuffManager.AddBuff(this, this, new GuardiansPathBuff());
                                }
                            }
                            catch
                            {
                            }

                            break;
                        case 341559: //Momentum
                            World.BuffManager.RemoveBuffs(this, 341559);
                            World.BuffManager.AddBuff(this, this, new MomentumCheckBuff());
                            break;
                        case 209813: //Provocation
                            Attributes[GameAttribute.CrowdControl_Reduction] += 0.25f;
                            break;
                    }

                    break;
                case ToonClass.WitchDoctor:
                    switch (passiveId)
                    {
                        case 208569: //SpiritualAttunement
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] +=
                                Attributes[GameAttribute.Resource_Max, (int)Toon.HeroTable.PrimaryResource] * 0.2f;
                            Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceRegen + (Toon.HeroTable.PrimaryResourceBase +
                                                                       Attributes[GameAttribute.Resource_Max_Bonus,
                                                                           (int)Toon.HeroTable.PrimaryResource]) / 100;
                            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceBase + Attributes[GameAttribute.Resource_Max_Bonus,
                                    (int)Toon.HeroTable.PrimaryResource];
                            break;
                        case 340910: //PhysicalAttunement
                            World.BuffManager.RemoveBuffs(this, 340910);
                            World.BuffManager.AddBuff(this, this, new PhysicalAttunementBuff());
                            break;
                        case 208568: //BloodRitual
                            Attributes[GameAttribute.Hitpoints_Regen_Per_Second] +=
                                Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
                            break;
                        case 208639: //FierceLoyalty
                            foreach (var minionId in Followers.Keys)
                            {
                                var minion = World.GetActorByGlobalId(minionId);
                                if (minion != null)
                                    minion.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] =
                                        Inventory.GetItemBonus(GameAttribute.Hitpoints_Regen_Per_Second);
                            }

                            break;
                    }

                    break;
                case ToonClass.Wizard:
                    switch (passiveId)
                    {
                        case 208541: //Galvanizing Ward
                            World.BuffManager.RemoveBuffs(this, 208541);
                            World.BuffManager.AddBuff(this, this, new GalvanizingBuff());
                            break;
                        case 208473: //Evocation
                            Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All] += 0.20f;
                            break;
                        case 208472: //AstralPresence
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 20;
                            Attributes[GameAttribute.Resource_Regen_Per_Second, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceRegen + 2;
                            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] =
                                Toon.HeroTable.PrimaryResourceBase + Attributes[GameAttribute.Resource_Max_Bonus,
                                    (int)Toon.HeroTable.PrimaryResource];
                            break;
                        case 208468: //Blur (Wizard)
                            Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] += 0.17f;
                            break;
                    }

                    break;
                case ToonClass.Crusader:
                    switch (passiveId)
                    {
                        case 286177: //HeavenlyStrength
                            Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= 0.15f;
                            Attributes[GameAttribute.Allow_2H_And_Shield] = true;
                            break;
                        case 310626: //Vigilant
                            Attributes[GameAttribute.Hitpoints_Regen_Per_Second] +=
                                10 + 0.008f * (float)Math.Pow(Attributes[GameAttribute.Level], 3);
                            break;
                        case 356147: //Righteousness
                            Attributes[GameAttribute.Resource_Max_Bonus, (int)Toon.HeroTable.PrimaryResource] += 30;
                            break;
                        case 310804: //HolyCause
                            Attributes[GameAttribute.Damage_Weapon_Min, 6] *= 1.1f;
                            break;
                        case 356176: //DivineFortress
                            World.BuffManager.RemoveBuffs(this, 356176);
                            World.BuffManager.AddBuff(this, this, new DivineFortressBuff());
                            break;
                        case 302500: //HoldYourGround
                            Attributes[GameAttribute.Cannot_Dodge] = true;
                            Attributes[GameAttribute.Block_Chance] += 0.15f;
                            break;
                        case 310783: //IronMaiden
                            Attributes[GameAttribute.Thorns_Fixed, 0] += 87.17f * Attributes[GameAttribute.Level];
                            break;
                        case 311629: //Finery
                            World.BuffManager.RemoveBuffs(this, 311629);
                            World.BuffManager.AddBuff(this, this, new FineryBuff());
                            break;
                        case 310640: //Insurmountable
                            World.BuffManager.RemoveBuffs(this, 310640);
                            World.BuffManager.AddBuff(this, this, new InsurmountableBuff());
                            break;
                        case 309830: //Indesctructible
                            World.BuffManager.RemoveBuffs(this, 309830);
                            World.BuffManager.AddBuff(this, this, new IndestructibleBuff());
                            break;
                        case 356173: //Renewal
                            World.BuffManager.RemoveBuffs(this, 356173);
                            World.BuffManager.AddBuff(this, this, new RenewalBuff());
                            break;
                        case 356052: //Towering Shield
                            World.BuffManager.RemoveBuffs(this, 356052);
                            World.BuffManager.AddBuff(this, this, new ToweringShieldBuff());
                            break;
                    }

                    break;
                case ToonClass.Necromancer:
                    switch (passiveId)
                    {
                        case 470764: //HugeEssense
                            Attributes[GameAttribute.Resource_Max_Bonus,
                                Attributes[GameAttribute.Resource_Type_Primary] - 1] += 40;
                            break;
                        case 470725:
                            World.BuffManager.RemoveBuffs(this, 470725);
                            World.BuffManager.AddBuff(this, this, new OnlyOne());
                            break;
                    }

                    break;
            }

        SetAttributesSkillSets(); //reapply synergy passives (laws, mantras, summons)
    }

    public void SetAttributesSkillSets()
    {
        // unlocking assigned skills
        for (var i = 0; i < SkillSet.ActiveSkills.Length; i++)
            if (SkillSet.ActiveSkills[i].snoSkill != -1)
            {
                Attributes[GameAttribute.Skill, SkillSet.ActiveSkills[i].snoSkill] = 1;
                //scripted //this.Attributes[GameAttribute.Skill_Total, this.SkillSet.ActiveSkills[i].snoSkill] = 1;
                // update rune attributes for new skill
                Attributes[GameAttribute.Rune_A, SkillSet.ActiveSkills[i].snoSkill] =
                    SkillSet.ActiveSkills[i].snoRune == 0 ? 1 : 0;
                Attributes[GameAttribute.Rune_B, SkillSet.ActiveSkills[i].snoSkill] =
                    SkillSet.ActiveSkills[i].snoRune == 1 ? 1 : 0;
                Attributes[GameAttribute.Rune_C, SkillSet.ActiveSkills[i].snoSkill] =
                    SkillSet.ActiveSkills[i].snoRune == 2 ? 1 : 0;
                Attributes[GameAttribute.Rune_D, SkillSet.ActiveSkills[i].snoSkill] =
                    SkillSet.ActiveSkills[i].snoRune == 3 ? 1 : 0;
                Attributes[GameAttribute.Rune_E, SkillSet.ActiveSkills[i].snoSkill] =
                    SkillSet.ActiveSkills[i].snoRune == 4 ? 1 : 0;

                var power = PowerLoader.CreateImplementationForPowerSNO(SkillSet.ActiveSkills[i].snoSkill);
                if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
                    World.PowerManager.RunPower(this, power.EvalTag(PowerKeys.SynergyPower)); //SynergyPower buff
            }

        for (var i = 0; i < SkillSet.PassiveSkills.Length; ++i)
            if (SkillSet.PassiveSkills[i] != -1)
            {
                // switch on passive skill
                Attributes[GameAttribute.Trait, SkillSet.PassiveSkills[i]] = 1;
                Attributes[GameAttribute.Skill, SkillSet.PassiveSkills[i]] = 1;
                //scripted //this.Attributes[GameAttribute.Skill_Total, this.SkillSet.PassiveSkills[i]] = 1;
            }

        if (Toon.Class == ToonClass.Monk) //Setting power range override
        {
            Attributes[GameAttribute.PowerBonusAttackRadius, 0x000176C4] = 20f; //Fists of Thunder
            if (Attributes[GameAttribute.Rune_A, 0x00017B56] > 0) //Way of the Hundred Fists -> Fists of Fury
                Attributes[GameAttribute.PowerBonusAttackRadius, 0x00017B56] = 15f;
        }
    }

    public void SetAttributesOther()
    {
        //Bonus stats
        Attributes[GameAttribute.Hit_Chance] = 1f;

        Attributes[GameAttribute.Attacks_Per_Second] = 1.2f;
        //this.Attributes[GameAttribute.Attacks_Per_Second_Item] = 1.199219f;
        Attributes[GameAttribute.Crit_Percent_Cap] = Toon.HeroTable.CritPercentCap;
        //scripted //this.Attributes[GameAttribute.Casting_Speed_Total] = 1f;
        Attributes[GameAttribute.Casting_Speed] = 1f;

        //Basic stats
        Attributes[GameAttribute.Level_Cap] = Program.MaxLevel;
        Attributes[GameAttribute.Level] = Level;
        Attributes[GameAttribute.Alt_Level] = ParagonLevel;
        if (Level == Program.MaxLevel)
        {
            Attributes[GameAttribute.Alt_Experience_Next_Lo] = (int)(ExperienceNext % uint.MaxValue);
            Attributes[GameAttribute.Alt_Experience_Next_Hi] = (int)(ExperienceNext / uint.MaxValue);
        }
        else
        {
            Attributes[GameAttribute.Experience_Next_Lo] = (int)(ExperienceNext % uint.MaxValue);
            Attributes[GameAttribute.Experience_Next_Hi] = (int)(ExperienceNext / uint.MaxValue);
            //this.Attributes[GameAttribute.Alt_Experience_Next] = 0;
        }

        Attributes[GameAttribute.Experience_Granted_Low] = 1000;
        Attributes[GameAttribute.Armor] = Toon.HeroTable.Armor;
        Attributes[GameAttribute.Damage_Min, 0] = Toon.HeroTable.Dmg;
        //scripted //this.Attributes[GameAttribute.Armor_Total]


        Attributes[GameAttribute.Strength] = (int)Strength;
        Attributes[GameAttribute.Dexterity] = (int)Dexterity;
        Attributes[GameAttribute.Vitality] = (int)Vitality;
        Attributes[GameAttribute.Intelligence] = (int)Intelligence;
        Attributes[GameAttribute.Core_Attributes_From_Item_Bonus_Multiplier] = 1;

        //Hitpoints have to be calculated after Vitality
        Attributes[GameAttribute.Hitpoints_Factor_Level] = Toon.HeroTable.HitpointsFactorLevel;
        Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(Level - 35, 0);
        //this.Attributes[GameAttribute.Hitpoints_Max] = 276f;

        Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = (int)1;
        Attributes[GameAttribute.Hitpoints_Factor_Level] = (int)Toon.HeroTable.HitpointsFactorLevel;
        Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f; // + Math.Max(this.Level - 35, 0);
        Attributes[GameAttribute.Hitpoints_Max] = (int)Toon.HeroTable.HitpointsMax;

        Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];

        Attributes[GameAttribute.Corpse_Resurrection_Charges] = 3;
        //TestOutPutItemAttributes(); //Activate this only for finding item stats.
        Attributes.BroadcastChangedIfRevealed();
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
        if (message is AssignSkillMessage skillMessage) OnAssignActiveSkill(client, skillMessage);
        else if (message is AssignTraitsMessage traitsMessage) OnAssignPassiveSkills(client, traitsMessage);
        else if (message is UnassignSkillMessage unassignSkillMessage) OnUnassignActiveSkill(client, unassignSkillMessage);
        else if (message is TargetMessage targetMessage) OnObjectTargeted(client, targetMessage);
        else if (message is ACDClientTranslateMessage translateMessage) OnPlayerMovement(client, translateMessage);
        else if (message is TryWaypointMessage waypointMessage) OnTryWaypoint(client, waypointMessage);
        else if (message is RequestBuyItemMessage itemMessage) OnRequestBuyItem(client, itemMessage);
        else if (message is RequestSellItemMessage sellItemMessage) OnRequestSellItem(client, sellItemMessage);
        else if (message is HirelingRequestLearnSkillMessage learnSkillMessage)
            OnHirelingRequestLearnSkill(client, learnSkillMessage);
        else if (message is HirelingRetrainMessage) OnHirelingRetrainMessage();
        else if (message is HirelingSwapAgreeMessage) OnHirelingSwapAgreeMessage();
        else if (message is PetAwayMessage awayMessage) OnHirelingDismiss(client, awayMessage);
        else if (message is ChangeUsableItemMessage usableItemMessage) OnEquipPotion(client, usableItemMessage);
        else if (message is ArtisanWindowClosedMessage) OnArtisanWindowClosed();
        else if (message is RequestTrainArtisanMessage artisanMessage) TrainArtisan(client, artisanMessage);
        else if (message is RessurectionPlayerMessage playerMessage) OnResurrectOption(client, playerMessage);
        else if (message is PlayerTranslateFacingMessage facingMessage)
            OnTranslateFacing(client, facingMessage);
        else if (message is LoopingAnimationPowerMessage powerMessage)
            OnLoopingAnimationPowerMessage(client, powerMessage);
        else if (message is SecondaryAnimationPowerMessage animationPowerMessage)
            OnSecondaryPowerMessage(client, animationPowerMessage);
        else if (message is MiscPowerMessage miscPowerMessage) OnMiscPowerMessage(client, miscPowerMessage);
        else if (message is RequestBuffCancelMessage cancelMessage) OnRequestBuffCancel(client, cancelMessage);
        else if (message is CancelChanneledSkillMessage channeledSkillMessage)
            OnCancelChanneledSkill(client, channeledSkillMessage);
        else if (message is TutorialShownMessage shownMessage) OnTutorialShown(client, shownMessage);
        else if (message is AcceptConfirmMessage confirmMessage) OnConfirm(client, confirmMessage);
        else if (message is SpendParagonPointsMessage pointsMessage)
            OnSpendParagonPointsMessage(client, pointsMessage);
        else if (message is ResetParagonPointsMessage paragonPointsMessage)
            OnResetParagonPointsMessage(client, paragonPointsMessage);
        else if (message is MailRetrieveMessage retrieveMessage) OnMailRetrieve(client, retrieveMessage);
        else if (message is MailReadMessage readMessage) OnMailRead(client, readMessage);
        else if (message is StashIconStateAssignMessage assignMessage)
            OnStashIconsAssign(client, assignMessage);
        else if (message is TransmuteItemsMessage itemsMessage) TransmuteItemsPlayer(client, itemsMessage);
        else if (message is RiftStartAcceptedMessage acceptedMessage) OpenNephalem(client, acceptedMessage);
        else if (message is BossEncounterAcceptMessage) AcceptBossEncounter();
        else if (message is DeActivateCameraCutsceneMode mode)
            DeactivateCamera(client, mode);
        else if (message is JewelUpgradeMessage upgradeMessage) JewelUpgrade(client, upgradeMessage);
        else if (message is SwitchCosmeticMessage cosmeticMessage) SwitchCosmetic(client, cosmeticMessage);

        else return;
    }

    public void SwitchCosmetic(GameClient client, SwitchCosmeticMessage message)
    {
        var Definition = ItemGenerator.GetItemDefinition(message.Field0);

        ;
        if (Definition.Name.ToLower().Contains("portrait"))
            client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic4 = message.Field0;

        var RangeCosmetic = new[]
        {
            D3.Hero.VisualCosmeticItem.CreateBuilder()
                .SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic1).Build(), // Wings
            D3.Hero.VisualCosmeticItem.CreateBuilder()
                .SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic2).Build(), // Flag
            D3.Hero.VisualCosmeticItem.CreateBuilder()
                .SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic3).Build(), // Pet
            D3.Hero.VisualCosmeticItem.CreateBuilder()
                .SetGbid(client.BnetClient.Account.GameAccount.CurrentToon.Cosmetic4).Build() // Frame
        };

        //RangeCosmetic[request.CosmeticItemType - 1] = D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(unchecked((int)request.Gbid)).Build();

        client.BnetClient.Account.GameAccount.CurrentToon.StateChanged();

        var NewVisual = D3.Hero.VisualEquipment.CreateBuilder()
            .AddRangeVisualItem(client.BnetClient.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value
                .VisualItemList).AddRangeCosmeticItem(RangeCosmetic).Build();

        client.BnetClient.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value = NewVisual;
        client.BnetClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(client.BnetClient.Account.GameAccount
            .CurrentToon.HeroVisualEquipmentField);
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
        var Jewel = Inventory.GetItemByDynId(this, message.ActorID);
        Jewel.Attributes[GameAttribute.Jewel_Rank]++;
        Jewel.Attributes.BroadcastChangedIfRevealed();
        Attributes[GameAttribute.Jewel_Upgrades_Used]++;
        Attributes.BroadcastChangedIfRevealed();
        if (Attributes[GameAttribute.Jewel_Upgrades_Used] == Attributes[GameAttribute.Jewel_Upgrades_Max] +
            Attributes[GameAttribute.Jewel_Upgrades_Bonus])
        {
            Attributes[GameAttribute.Jewel_Upgrades_Max] = 0;
            Attributes[GameAttribute.Jewel_Upgrades_Bonus] = 0;
            Attributes[GameAttribute.Jewel_Upgrades_Used] = 0;
        }

        InGameClient.SendMessage(new JewelUpgradeResultsMessage()
        {
            ActorID = message.ActorID,
            Field1 = 1
        });
    }

    public void OnHirelingSwapAgreeMessage()
    {
        Hireling hireling = null;
        DiIiS_NA.Core.MPQ.FileFormats.ActorData Data = null;
        if (World.Game.Players.Count > 1) return;


        switch (InGameClient.Game.CurrentQuest)
        {
            case 72061:
                //Templar
                Data = (DiIiS_NA.Core.MPQ.FileFormats.ActorData)MPQStorage.Data.Assets[SNOGroup.Actor][52693].Data;
                hireling = new Templar(World, ActorSno._hireling_templar, Data.TagMap);
                hireling.GBHandle.GBID = StringHashHelper.HashItemName("Templar");

                break;
            case 72738:
                //Scoundrel
                Data = (DiIiS_NA.Core.MPQ.FileFormats.ActorData)MPQStorage.Data.Assets[SNOGroup.Actor][52694].Data;
                hireling = new Templar(World, ActorSno._hireling_scoundrel, Data.TagMap);
                hireling.GBHandle.GBID = StringHashHelper.HashItemName("Scoundrel");
                break;
            case 0:
                //Enchantress
                Data = (DiIiS_NA.Core.MPQ.FileFormats.ActorData)MPQStorage.Data.Assets[SNOGroup.Actor][4482].Data;
                hireling = new Templar(World, ActorSno._hireling_enchantress, Data.TagMap);
                hireling.GBHandle.GBID = StringHashHelper.HashItemName("Enchantress");
                break;
        }

        hireling.SetUpAttributes(this);
        hireling.GBHandle.Type = 4;
        hireling.Attributes[GameAttribute.Pet_Creator] = PlayerIndex + 1;
        hireling.Attributes[GameAttribute.Pet_Type] = 1;
        hireling.Attributes[GameAttribute.Pet_Owner] = PlayerIndex + 1;
        hireling.Attributes[GameAttribute.Untargetable] = false;
        hireling.Attributes[GameAttribute.NPC_Is_Escorting] = true;

        hireling.EnterWorld(RandomDirection(Position, 3, 10)); //Random
        hireling.Brain = new HirelingBrain(hireling, this);
        ActiveHireling = hireling;
        SelectedNPC = null;
    }

    public static Vector3D RandomDirection(Vector3D position, float minRadius, float maxRadius)
    {
        var angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
        var radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
        return new Vector3D(position.X + (float)Math.Cos(angle) * radius,
            position.Y + (float)Math.Sin(angle) * radius,
            position.Z);
    }

    public void AwayPet(GameClient client, PetAwayMessage message)
    {
    }

    public void DeactivateCamera(GameClient client, DeActivateCameraCutsceneMode message)
    {
        InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
        InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
        //this.InGameClient.SendMessage(new ActivateCameraCutsceneMode() { Activate = true });
    }

    public void AcceptBossEncounter()
    {
        InGameClient.Game.AcceptBossEncounter();
    }

    public void DeclineBossEncounter()
    {
        InGameClient.Game.CurrentEncounter.Activated = false;
    }

    public void TransmuteItemsPlayer(GameClient client, TransmuteItemsMessage message)
    {
        var recipeDefinition = ItemGenerator.GetRecipeDefinition(608170752);
        for (var i = 0; i < message.CurrenciesCount; i++)
        {
            var data = ItemGenerator.GetItemDefinition(message.GBIDCurrencies[i]).Name;
            switch (data)
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
            var a = Inventory.GetItemByDynId(this, (uint)it);
        }

        Item ItemPortalToCows = null;
        var Items = new List<Item> { };
        for (var i = 0; i < message.ItemsCount; i++)
        {
            Items.Add(Inventory.GetItemByDynId(this, (uint)message.annItems[i]));
            if (Items[i].SNO == ActorSno._x1_polearm_norm_unique_05)
                ItemPortalToCows = Items[i];
        }

        //Type - 0 - New property
        //Type - 1 - Transmutation
        //Type - 2 -

        if (ItemPortalToCows != null)
        {
            InGameClient.SendMessage(new TransmuteResultsMessage()
            {
                annItem = -1,
                Type = -1,
                GBIDFakeItem = -1,
                GBIDPower = -1,
                FakeItemStackCount = -1
            });

            Inventory.DestroyInventoryItem(ItemPortalToCows);
            World.SpawnMonster(ActorSno._p2_totallynotacowlevel_portal,
                new Vector3D(Position.X + 5, Position.Y + 5, Position.Z));
        }
        else
        {
            InGameClient.SendMessage(new TransmuteResultsMessage()
            {
                annItem = (int)Items[0].DynamicID(this),
                Type = 0,
                GBIDFakeItem = -1,
                GBIDPower = (int)Items[0].ItemDefinition.Hash,
                FakeItemStackCount = -1
            });
            GrantCriteria(74987245494264);
            GrantCriteria(74987258962046);
        }
    }

    private bool WaitToSpawn(TickTimer timer)
    {
        while (!timer.TimedOut) ;

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
        var activated = false;
        var newTagMap = new TagMap();
        World nephalemPWorld = null;
        Actor lootRunObelisk = null;
        Portal portal = null;
        var map = WorldSno.__NONE;
        var maps = new WorldSno[]
        {
            WorldSno.x1_lr_tileset_westmarch, //x1_lr_tileset_Westmarch
            WorldSno.x1_lr_tileset_fortress_large, //_x1_lr_tileset_fortress_large
            WorldSno.x1_lr_tileset_zoltruins, //x1_lr_tileset_zoltruins
            WorldSno.x1_lr_tileset_hexmaze, //x1_lr_tileset_hexmaze
            WorldSno.x1_lr_tileset_icecave, //x1_lr_tileset_icecave

            WorldSno.x1_lr_tileset_crypt, //x1_lr_tileset_crypt
            WorldSno.x1_lr_tileset_corruptspire //x1_lr_tileset_corruptspire

            //288843, //x1_lr_tileset_sewers
        };

        switch (message.Field0)
        {
            #region Нефалемский портал

            case -1:
                Logger.Debug("Calling Nephalem Portal (Normal)");
                activated = false;

                foreach (var oldp in World.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal,
                             ActorSno._x1_openworld_tiered_rifts_portal)) oldp.Destroy();

                map = maps.PickRandom();
                //map = 288823;
                newTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, (int)map, 0)); //World
                newTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 288482, 0)); //Zone
                newTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 172, 0)); //Entry-Pointа
                InGameClient.Game.WorldOfPortalNephalem = map;

                while (true)
                {
                    map = maps.PickRandom();
                    if (map != InGameClient.Game.WorldOfPortalNephalem) break;
                }

                InGameClient.Game.WorldOfPortalNephalemSec = map;

                nephalemPWorld = InGameClient.Game.GetWorld(InGameClient.Game.WorldOfPortalNephalem);

                var exitSceneSno = -1;
                foreach (var scene in nephalemPWorld.Scenes.Values)
                    if (scene.SceneSNO.Name.ToLower().Contains("exit"))
                        exitSceneSno = scene.SceneSNO.Id;
                var exitSetted = false;
                foreach (var actor in nephalemPWorld.Actors.Values)
                    if (actor is Portal actor1)
                    {
                        if (!actor1.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
                        {
                            if (!actor1.CurrentScene.SceneSNO.Name.ToLower().Contains("exit"))
                            {
                                actor1.Destroy();
                            }
                            else if (!exitSetted)
                            {
                                actor1.Destination.DestLevelAreaSNO = 288684;
                                actor1.Destination.WorldSNO = (int)InGameClient.Game.WorldOfPortalNephalemSec;
                                exitSetted = true;

                                var nephalemPWorldS2 =
                                    InGameClient.Game.GetWorld(InGameClient.Game.WorldOfPortalNephalemSec);
                                foreach (var atr in nephalemPWorldS2.Actors.Values)
                                    if (atr is Portal portal1)
                                    {
                                        if (!portal1.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
                                        {
                                            portal1.Destroy();
                                        }
                                        else
                                        {
                                            portal1.Destination.DestLevelAreaSNO = 332339;
                                            portal1.Destination.WorldSNO =
                                                (int)WorldSno.x1_tristram_adventure_mode_hub;
                                            portal1.Destination.StartingPointActorTag = 172;
                                        }
                                    }
                                    else if (atr is Waypoint)
                                    {
                                        atr.Destroy();
                                    }
                            }
                            else
                            {
                                actor1.Destroy();
                            }
                        }
                        else
                        {
                            actor1.Destination.DestLevelAreaSNO = 332339;
                            actor1.Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
                            actor1.Destination.StartingPointActorTag = 24;
                        }
                    }
                    else if (actor is Waypoint)
                    {
                        actor.Destroy();
                    }

                #region Activation

                lootRunObelisk = World.GetActorBySNO(ActorSno._x1_openworld_lootrunobelisk_b);
                lootRunObelisk.PlayAnimation(5, (AnimationSno)lootRunObelisk.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                lootRunObelisk.Attributes[GameAttribute.Team_Override] = activated ? -1 : 2;
                lootRunObelisk.Attributes[GameAttribute.Untargetable] = !activated;
                lootRunObelisk.Attributes[GameAttribute.NPC_Is_Operatable] = activated;
                lootRunObelisk.Attributes[GameAttribute.Operatable] = activated;
                lootRunObelisk.Attributes[GameAttribute.Operatable_Story_Gizmo] = activated;
                lootRunObelisk.Attributes[GameAttribute.Disabled] = !activated;
                lootRunObelisk.Attributes[GameAttribute.Immunity] = !activated;
                lootRunObelisk.Attributes.BroadcastChangedIfRevealed();

                lootRunObelisk.CollFlags = 0;
                World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
                {
                    ActorID = lootRunObelisk.DynamicID(plr),
                    CollFlags = 0
                }, lootRunObelisk);
                portal = new Portal(World, ActorSno._x1_openworld_lootrunportal, newTagMap);

                TickTimer timeout = new SecondsTickTimer(World.Game, 3.5f);
                var boom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(timeout));
                boom.ContinueWith(delegate
                {
                    portal.EnterWorld(lootRunObelisk.Position);
                    //Quest - 382695 - The Great Nephalem
                    //Quest - 337492 - Just Nephalem
                    foreach (var plr in InGameClient.Game.Players.Values)
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
                                Act = 3000, //act id
                                InitialMonsterLevel = InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
                                MonsterLevel = 0x64E4425E, //MonsterLevel
                                RandomWeatherSeed = InGameClient.Game.WeatherSeed, //RandomWeatherSeed
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
                                PartyGuideHeroId =
                                    0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
                                TiredRiftPaticipatingHeroID =
                                    new long[] { 0x0, 0x0, 0x0, 0x0 } //TiredRiftPaticipatingHeroID
                            }
                        });
                    }
                });

                #endregion


                break;

            #endregion

            #region Великий портал

            default:
                InGameClient.Game.NephalemGreaterLevel = message.Field0;

                Logger.Debug("Calling Nephalem Portal (Level: {0})", message.Field0);
                activated = false;
                foreach (var oldp in World.GetActorsBySNO(ActorSno._x1_openworld_lootrunportal,
                             ActorSno._x1_openworld_tiered_rifts_portal)) oldp.Destroy();

                InGameClient.Game.ActiveNephalemPortal = true;
                InGameClient.Game.NephalemGreater = true;
                //disable banner while greater is active enable once boss is killed or portal is closed /advocaite
                Attributes[GameAttribute.Banner_Usable] = false;
                map = maps.PickRandom();
                newTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, (int)map, 0)); //World
                newTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 288482, 0)); //Zone
                newTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 172, 0)); //Entry-Pointа
                InGameClient.Game.WorldOfPortalNephalem = map;

                nephalemPWorld = InGameClient.Game.GetWorld(map);
                foreach (var actor in nephalemPWorld.Actors.Values)
                    if (actor is Portal portal1)
                    {
                        if (!portal1.CurrentScene.SceneSNO.Name.ToLower().Contains("entrance"))
                        {
                            portal1.Destroy();
                        }
                        else
                        {
                            portal1.Destination.DestLevelAreaSNO = 332339;
                            portal1.Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
                            portal1.Destination.StartingPointActorTag = 24;
                        }
                    }
                    else if (actor is Waypoint)
                    {
                        actor.Destroy();
                    }

                #region Активация

                lootRunObelisk = World.GetActorBySNO(ActorSno._x1_openworld_lootrunobelisk_b);
                lootRunObelisk.PlayAnimation(5, (AnimationSno)lootRunObelisk.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                lootRunObelisk.Attributes[GameAttribute.Team_Override] = activated ? -1 : 2;
                lootRunObelisk.Attributes[GameAttribute.Untargetable] = !activated;
                lootRunObelisk.Attributes[GameAttribute.NPC_Is_Operatable] = activated;
                lootRunObelisk.Attributes[GameAttribute.Operatable] = activated;
                lootRunObelisk.Attributes[GameAttribute.Operatable_Story_Gizmo] = activated;
                lootRunObelisk.Attributes[GameAttribute.Disabled] = !activated;
                lootRunObelisk.Attributes[GameAttribute.Immunity] = !activated;
                lootRunObelisk.Attributes.BroadcastChangedIfRevealed();

                lootRunObelisk.CollFlags = 0;
                World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
                {
                    ActorID = lootRunObelisk.DynamicID(plr),
                    CollFlags = 0
                }, lootRunObelisk);
                portal = new Portal(World, ActorSno._x1_openworld_tiered_rifts_portal, newTagMap);

                TickTimer altTimeout = new SecondsTickTimer(World.Game, 3.5f);
                var altBoom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(altTimeout));
                altBoom.ContinueWith(delegate
                {
                    portal.EnterWorld(lootRunObelisk.Position);
                    //Quest - 382695 - Великий Нефалемский
                    //Quest - 337492 - Просто Нефалемский

                    //this.ChangeWorld(NephalemPWorld, NephalemPWorld.GetStartingPointById(172).Position);

                    foreach (var plr in InGameClient.Game.Players.Values)
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
                                Act = 3000, //act id
                                InitialMonsterLevel = InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
                                MonsterLevel = 0x64E4425E, //MonsterLevel
                                RandomWeatherSeed = InGameClient.Game.WeatherSeed, //RandomWeatherSeed
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
                                PartyGuideHeroId =
                                    0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
                                TiredRiftPaticipatingHeroID =
                                    new long[] { 0x0, 0x0, 0x0, 0x0 } //TiredRiftPaticipatingHeroID
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
        var index = ItemGenerator.Tutorials.IndexOf(message.SNOTutorial);
        if (index == -1) return;
        var seenTutorials = Toon.GameAccount.DBGameAccount.SeenTutorials;
        if (seenTutorials.Length <= 34)
            seenTutorials = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
        seenTutorials[index / 8] |= (byte)(1 << (index % 8));

        lock (Toon.GameAccount.DBGameAccount)
        {
            var gameAccount = Toon.GameAccount.DBGameAccount;
            gameAccount.SeenTutorials = seenTutorials;
            DBSessions.SessionUpdate(gameAccount);
        }
        //*/
    }

    private void OnConfirm(GameClient client, AcceptConfirmMessage message)
    {
        if (ConfirmationResult != null)
        {
            ConfirmationResult.Invoke();
            ConfirmationResult = null;
        }
    }

    private void OnSpendParagonPointsMessage(GameClient client, SpendParagonPointsMessage message)
    {
        var bonus = ItemGenerator.GetParagonBonusTable(Toon.Class).FirstOrDefault(b => b.Hash == message.BonusGBID);

        if (bonus == null) return;
        if (message.Amount > Attributes[GameAttribute.Paragon_Bonus_Points_Available, bonus.Category]) return;
        //if (this.ParagonBonuses[(bonus.Category * 4) + bonus.Index - 1] + (byte)message.Amount > bonus.Limit) return;

        // message.Amount have the value send to add on attr of Paragon tabs.
        ParagonBonuses[bonus.Category * 4 + bonus.Index - 1] += (ushort)message.Amount;

        var dbToon = Toon.DBToon;
        dbToon.ParagonBonuses = ParagonBonuses;
        World.Game.GameDbSession.SessionUpdate(dbToon);

        SetAttributesByItems();
        SetAttributesByItemProcs();
        SetAttributesByGems();
        SetAttributesByItemSets();
        SetAttributesByPassives();
        SetAttributesByParagon();
        Attributes.BroadcastChangedIfRevealed();
        UpdatePercentageHP(PercHPbeforeChange);
    }

    private void OnResetParagonPointsMessage(GameClient client, ResetParagonPointsMessage message)
    {
        ParagonBonuses = new ushort[]
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var dbToon = Toon.DBToon;
        dbToon.ParagonBonuses = ParagonBonuses;
        World.Game.GameDbSession.SessionUpdate(dbToon);

        SetAttributesByItems();
        SetAttributesByItemProcs();
        SetAttributesByGems();
        SetAttributesByItemSets();
        SetAttributesByPassives();
        SetAttributesByParagon();
        Attributes.BroadcastChangedIfRevealed();
        UpdatePercentageHP(PercHPbeforeChange);
    }

    private void OnMailRead(GameClient client, MailReadMessage message)
    {
        //does it make sense?
    }

    private void OnMailRetrieve(GameClient client, MailRetrieveMessage message)
    {
        var dbMail = World.Game.GameDbSession.SessionGet<DBMail>((ulong)message.MailId);
        if (dbMail == null || dbMail.DBToon.Id != Toon.PersistentID) return;
        dbMail.Claimed = true;
        World.Game.GameDbSession.SessionUpdate(dbMail);

        if (dbMail.ItemGBID != -1)
            Inventory.PickUp(ItemGenerator.CookFromDefinition(World, ItemGenerator.GetItemDefinition(dbMail.ItemGBID),
                -1, true));

        LoadMailData();
    }

    private void OnStashIconsAssign(GameClient client, StashIconStateAssignMessage message)
    {
        if (message.StashIcons.Length != 4) return;
        lock (Toon.GameAccount.DBGameAccount)
        {
            var dbGAcc = Toon.GameAccount.DBGameAccount;
            dbGAcc.StashIcons = message.StashIcons;
            DBSessions.SessionUpdate(dbGAcc);
        }
        //LoadStashIconsData();
    }

    public void PlayCutscene(int cutsceneId)
    {
        InGameClient.SendMessage(new PlayCutsceneMessage()
        {
            Index = cutsceneId
        });
    }

    private void OnTranslateFacing(GameClient client, PlayerTranslateFacingMessage message)
    {
        SetFacingRotation(message.Angle);

        World.BroadcastExclusive(plr => new ACDTranslateFacingMessage
        {
            ActorId = DynamicID(plr),
            Angle = message.Angle,
            TurnImmediately = message.TurnImmediately
        }, this);
    }

    private void OnAssignActiveSkill(GameClient client, AssignSkillMessage message)
    {
        var old_skills = SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
        foreach (var skill in old_skills)
        {
            var power = PowerLoader.CreateImplementationForPowerSNO(skill);
            if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
                World.BuffManager.RemoveBuffs(this, power.EvalTag(PowerKeys.SynergyPower));
        }

        var oldSNOSkill = SkillSet.ActiveSkills[message.SkillIndex].snoSkill; // find replaced skills SNO.
        if (oldSNOSkill != -1)
        {
            Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
            World.BuffManager.RemoveBuffs(this, oldSNOSkill);

            var rem = new List<uint>();
            foreach (var fol in Followers.Where(f =>
                         World.GetActorByGlobalId(f.Key) == null ||
                         World.GetActorByGlobalId(f.Key).Attributes[GameAttribute.Summoned_By_SNO] == oldSNOSkill))
                rem.Add(fol.Key);
            foreach (var rm in rem)
                DestroyFollowerById(rm);
        }

        Attributes[GameAttribute.Skill, message.SNOSkill] = 1;
        //scripted //this.Attributes[GameAttribute.Skill_Total, message.SNOSkill] = 1;
        SkillSet.ActiveSkills[message.SkillIndex].snoSkill = message.SNOSkill;
        SkillSet.ActiveSkills[message.SkillIndex].snoRune = message.RuneIndex;
        SkillSet.SwitchUpdateSkills(message.SkillIndex, message.SNOSkill, message.RuneIndex, Toon);
        SetAttributesSkillSets();

        Attributes.BroadcastChangedIfRevealed();
        UpdateHeroState();

        var cooldownskill = SkillSet.ActiveSkills.GetValue(message.SkillIndex);

        if (SkillSet.HasSkill(460757))
            foreach (var skill in SkillSet.ActiveSkills)
                if (skill.snoSkill == 460757)
                    if (skill.snoRune == 3)
                        World.BuffManager.AddBuff(this, this, new P6_Necro_Devour_Aura());
                    else
                        World.BuffManager.RemoveBuffs(this, 474325);

        if (SkillSet.HasSkill(460870))
            foreach (var skill in SkillSet.ActiveSkills)
                if (skill.snoSkill == 460870)
                    if (skill.snoRune == 4)
                        World.BuffManager.AddBuff(this, this, new P6_Necro_Frailty_Aura());
                    else
                        World.BuffManager.RemoveBuffs(this, 473992);


        //_StartSkillCooldown((cooldownskill as ActiveSkillSavedData).snoSkill, SkillChangeCooldownLength);
    }

    private void OnAssignPassiveSkills(GameClient client, AssignTraitsMessage message)
    {
        for (var i = 0; i < message.SNOPowers.Length; ++i)
        {
            var oldSNOSkill = SkillSet.PassiveSkills[i]; // find replaced skills SNO.
            if (message.SNOPowers[i] != oldSNOSkill)
            {
                if (oldSNOSkill != -1)
                {
                    World.BuffManager.RemoveAllBuffs(this);
                    // switch off old passive skill
                    Attributes[GameAttribute.Trait, oldSNOSkill] = 0;
                    Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
                    //scripted //this.Attributes[GameAttribute.Skill_Total, oldSNOSkill] = 0;
                }

                if (message.SNOPowers[i] != -1)
                {
                    // switch on new passive skill
                    Attributes[GameAttribute.Trait, message.SNOPowers[i]] = 1;
                    Attributes[GameAttribute.Skill, message.SNOPowers[i]] = 1;
                    //scripted //this.Attributes[GameAttribute.Skill_Total, message.SNOPowers[i]] = 1;
                }

                SkillSet.PassiveSkills[i] = message.SNOPowers[i];
            }
        }

        SkillSet.UpdatePassiveSkills(Toon);

        var skills = SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
        foreach (var skill in skills)
            _StartSkillCooldown(skill, SkillChangeCooldownLength);

        SetAttributesByItems();
        SetAttributesByGems();
        SetAttributesByItemSets();
        SetAttributesByPassives();
        SetAttributesByParagon();
        SetAttributesSkillSets();
        Inventory.CheckWeapons(); //Handles removal of Heavenly Strength
        Attributes.BroadcastChangedIfRevealed();
        UpdateHeroState();
        UpdatePercentageHP(PercHPbeforeChange);
    }

    private void OnUnassignActiveSkill(GameClient client, UnassignSkillMessage message)
    {
        var oldSNOSkill = SkillSet.ActiveSkills[message.SkillIndex].snoSkill; // find replaced skills SNO.
        if (oldSNOSkill != -1)
        {
            Attributes[GameAttribute.Skill, oldSNOSkill] = 0;
            World.BuffManager.RemoveBuffs(this, oldSNOSkill);

            var rem = new List<uint>();
            foreach (var fol in Followers.Where(f =>
                         Math.Abs(World.GetActorByGlobalId(f.Key).Attributes[GameAttribute.Summoned_By_SNO] -
                                  oldSNOSkill) < 0.001))
                rem.Add(fol.Key);
            foreach (var rm in rem)
                DestroyFollowerById(rm);
        }

        SkillSet.ActiveSkills[message.SkillIndex].snoSkill = -1;
        SkillSet.ActiveSkills[message.SkillIndex].snoRune = -1;
        SkillSet.SwitchUpdateSkills(message.SkillIndex, -1, -1, Toon);
        SetAttributesSkillSets();

        Attributes.BroadcastChangedIfRevealed();
        UpdateHeroState();
    }

    public void SetNewAttributes()
    {
        //this.Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
        //this.Attributes[GameAttribute.Attacks_Per_Second_Bonus] = 1.0f;
        //this.Attributes[GameAttribute.Gold] = 1;
        //[GameAttribute.Damage_Weapon_Min_Total, 0]
        Attributes[GameAttribute.Attacks_Per_Second_Percent] = 0;
        Attributes[GameAttribute.Attacks_Per_Second_Percent_Uncapped] = 0;
        Attributes[GameAttribute.Attacks_Per_Second_Percent_Reduction] = 0;
        Attributes[GameAttribute.Attacks_Per_Second_Percent_Cap] = 0;
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
        World.BuffManager.AddBuff(this, this,
            new CooldownBuff(snoPower, seconds));
    }

    //private void OnPlayerChangeHotbarButtonMessage(GameClient client, PlayerChangeHotbarButtonMessage message)
    //{
    //	this.SkillSet.HotBarSkills[message.BarIndex] = message.ButtonData;
    //}

    private void OnObjectTargeted(GameClient client, TargetMessage message)
    {
        if (message.TargetID != 0xffffffff)
            message.TargetID = World.GetGlobalId(this, message.TargetID);

        if (Toon.Class == ToonClass.Crusader)
            if (World.BuffManager.HasBuff<CrusaderSteedCharge.PonyBuff>(this)) //Crusader -> cancel Steed Charge
                World.BuffManager.RemoveBuffs(this, 243853);

        var powerHandled =
            World.PowerManager.RunPower(this, message.PowerSNO, message.TargetID, message.Place.Position, message);

        if (!powerHandled)
        {
            var actor = World.GetActorByGlobalId(message.TargetID);
            if (actor == null) return;


#if DEBUG
            Logger.Warn("OnTargetedActor ID: {0}, Name: {1}, NumInWorld: {2}", actor.SNO, actor.Name,
                actor.NumberInWorld);
#else
#endif
            if (actor.GBHandle.Type == 1 && actor.Attributes[GameAttribute.TeamID] == 10)
                ExpBonusData.MonsterAttacked(InGameClient.Game.TickCounter);
            actor.OnTargeted(this, message);
        }

        ExpBonusData.Check(2);
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
        Attributes.BroadcastChangedIfRevealed();
        var a = GetActorsInRange(15f);

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

        if (World == null) return;

        if (Dead)
        {
            World.BroadcastIfRevealed(ACDWorldPositionMessage, this);
            return;
        }

        if (World.Game.Paused || BetweenWorlds) return;

        if (message.MovementSpeed > Attributes[GameAttribute.Running_Rate_Total] * 1.5f && !SpeedCheckDisabled)
        {
            _hackCounter++;
            if (_hackCounter > 5) _hackCounter = 0;
            World.BroadcastIfRevealed(ACDWorldPositionMessage, this);
            return;
        }

        if (message.Position != null)
        {
            if (PowerMath.Distance2D(message.Position, Position) > 300f)
            {
                World.BroadcastIfRevealed(ACDWorldPositionMessage, this);
                InGameClient.SendMessage(new ACDTranslateSyncMessage()
                {
                    ActorId = DynamicID(this),
                    Position = Position
                });
                return;
            }

            Position = message.Position;
        }

        SetFacingRotation(message.Angle);

        if (IsCasting) StopCasting();
        World.BuffManager.RemoveBuffs(this, 298038);

        RevealScenesToPlayer();
        RevealPlayersToPlayer();
        RevealActorsToPlayer();

        World.BroadcastExclusive(plr => new ACDTranslateNormalMessage
        {
            ActorId = DynamicID(plr),
            Position = Position,
            Angle = message.Angle,
            SnapFacing = false,
            MovementSpeed = message.MovementSpeed,
            AnimationTag = message.AnimationTag
        }, this, true);

        foreach (var actor in GetActorsInRange())
            actor.OnPlayerApproaching(this);

        VacuumPickup();
        if (World.Game.OnLoadWorldActions.ContainsKey(World.SNO))
        {
            Logger.MethodTrace($"OnLoadWorldActions: {World.SNO}");
            lock (World.Game.OnLoadWorldActions[World.SNO])
            {
                foreach (var action in World.Game.OnLoadWorldActions[World.SNO])
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Logger.WarnException(ex, "OnLoadWorldActions");
                    }
                }
                
                World.Game.OnLoadWorldActions[World.SNO].Clear();
            }
        }

        if (World.Game.OnLoadSceneActions.ContainsKey(CurrentScene.SceneSNO.Id))
        {
            Logger.MethodTrace($"OnLoadSceneActions: {CurrentScene.SceneSNO.Id}");
            
            Logger.MethodTrace(World.SNO.ToString());
            lock (World.Game.OnLoadSceneActions[CurrentScene.SceneSNO.Id])
            {
                foreach (var action in World.Game.OnLoadSceneActions[CurrentScene.SceneSNO.Id])
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Logger.WarnException(ex, "OnLoadSceneActions");
                    }
                }

                World.Game.OnLoadSceneActions[CurrentScene.SceneSNO.Id].Clear();
            }
        }

        if (CurrentScene.SceneSNO.Id != PreSceneId)
        {
            PreSceneId = CurrentScene.SceneSNO.Id;
            var levelArea = CurrentScene.Specification.SNOLevelAreas[0];
            if (World.Game.QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
            {
                var trigger = World.Game.QuestProgress.QuestTriggers[levelArea];
                if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
                    try
                    {
                        Logger.MethodTrace($"EnterLevelArea: {levelArea}");
                        trigger.questEvent.Execute(World); // launch a questEvent
                    }
                    catch (Exception e)
                    {
                        Logger.WarnException(e, "questEvent()");
                    }
            }
            // Reset resurrection charges on zone change - TODO: do not reset charges on reentering the same zone
            Attributes[GameAttribute.Corpse_Resurrection_Charges] = Config.Instance.ResurrectionCharges; 

#if DEBUG
            Logger.Warn($"Player Location {Toon.Name}, Scene: {CurrentScene.SceneSNO.Name} SNO: {CurrentScene.SceneSNO.Id} LevelArea: {CurrentScene.Specification.SNOLevelAreas[0]}");
#endif
        }

        LastMovementTick = World.Game.TickCounter;
    }

    private void OnCancelChanneledSkill(GameClient client, CancelChanneledSkillMessage message)
    {
        World.PowerManager.CancelChanneledSkill(this, message.PowerSNO);
    }

    private void OnRequestBuffCancel(GameClient client, RequestBuffCancelMessage message)
    {
        World.BuffManager.RemoveBuffs(this, message.PowerSNOId);
    }

    private void OnSecondaryPowerMessage(GameClient client, SecondaryAnimationPowerMessage message)
    {
        World.PowerManager.RunPower(this, message.PowerSNO, (uint)message.annTarget);
    }

    private void OnMiscPowerMessage(GameClient client, MiscPowerMessage message)
    {
        World.PowerManager.RunPower(this, message.PowerSNO);
    }

    private void OnLoopingAnimationPowerMessage(GameClient client, LoopingAnimationPowerMessage message)
    {
        StartCasting(150, new Action(() =>
        {
            try
            {
                World.PowerManager.RunPower(this, message.snoPower);
            }
            catch
            {
            }
        }), message.snoPower);
    }

    private void OnTryWaypoint(GameClient client, TryWaypointMessage tryWaypointMessage)
    {
        var wpWorld = World.Game.GetWayPointWorldById(tryWaypointMessage.nWaypoint);
        var wayPoint = wpWorld.GetWayPointById(tryWaypointMessage.nWaypoint);
        if (wayPoint == null) return;
        Logger.Debug("---Waypoint Debug---");
        var proximity = new RectangleF(wayPoint.Position.X - 1f, wayPoint.Position.Y - 1f, 2f, 2f);
        var scenes = wpWorld.QuadTree.Query<Scene>(proximity);
        if (scenes.Count == 0) return; // cork (is it real?)

        var scene = scenes[0]; // Parent scene /fasbat

        if (scenes.Count == 2) // What if it's a subscene?
            if (scenes[1].ParentChunkID != 0xFFFFFFFF)
                scene = scenes[1];

        var levelArea = scene.Specification.SNOLevelAreas[0];
        Logger.Debug(
            $"OnTryWaypoint: Id: {tryWaypointMessage.nWaypoint}, WorldId: {wpWorld.SNO}, levelArea: {levelArea}");
        Logger.Debug($"WpWorld: {wpWorld}, wayPoint: {wayPoint}");
        InGameClient.SendMessage(new SimpleMessage(Opcodes.LoadingWarping));
        if (wpWorld == World)
            Teleport(wayPoint.Position);
        else
            ChangeWorld(wpWorld, wayPoint.Position);

        //handling quest triggers
        if (World.Game.QuestProgress.QuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
        {
            var trigger = World.Game.QuestProgress.QuestTriggers[levelArea];
            if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
                try
                {
                    trigger.questEvent.Execute(World); // launch a questEvent
                }
                catch (Exception e)
                {
                    Logger.WarnException(e, "questEvent()");
                }
        }

        foreach (var bounty in World.Game.QuestManager.Bounties)
            bounty.CheckLevelArea(levelArea);

        InGameClient.SendMessage(new PortedToWaypointMessage
        {
            PlayerIndex = PlayerIndex,
            LevelAreaSNO = levelArea
        });
        Logger.Debug("---Waypoint Debug End---");
    }

    public void RefreshReveal()
    {
        var range = 200f;
        if (InGameClient.Game.CurrentEncounter.Activated)
            range = 360f;

        foreach (var actor in GetActorsInRange(range).Where(actor => actor is not Player))
            actor.Unreveal(this);
        RevealActorsToPlayer();
    }

    private void OnRequestBuyItem(GameClient client, RequestBuyItemMessage requestBuyItemMessage)
    {
        var vendor = SelectedNPC as Vendor;
        if (vendor == null)
            return;
        vendor.OnRequestBuyItem(this, requestBuyItemMessage.ItemId);
    }

    private void OnRequestSellItem(GameClient client, RequestSellItemMessage requestSellItemMessage)
    {
        var vendor = SelectedNPC as Vendor;
        if (vendor == null)
            return;
        vendor.OnRequestSellItem(this, (int)requestSellItemMessage.ItemId);
    }

    private void OnHirelingRetrainMessage()
    {
        if (ActiveHireling == null) return;

        switch (ActiveHireling.Attributes[GameAttribute.Hireling_Class])
        {
            case 1:
                if (ActiveHireling is Templar)
                    (ActiveHireling as Templar).Retrain(this);
                break;
            case 2:
                if (ActiveHireling is Scoundrel)
                    (ActiveHireling as Scoundrel).Retrain(this);
                break;
            case 3:
                if (ActiveHireling is Enchantress)
                    (ActiveHireling as Enchantress).Retrain(this);
                break;
            default:
                return;
        }
    }

    //*
    private void OnHirelingDismiss(GameClient client, PetAwayMessage message)
    {
        Logger.MethodTrace(message.ActorID.ToString());
        var petId = World.GetGlobalId(this, message.ActorID);
        var pet = World.GetActorByGlobalId(petId);
        if (pet is Hireling)
            ActiveHireling = null;
        else
            DestroyFollowersBySnoId(pet.SNO);
    }

    private void OnHirelingRequestLearnSkill(GameClient client, HirelingRequestLearnSkillMessage message)
    {
        Logger.MethodTrace($"{message.HirelingID} - {message.PowerSNOId}");
        var hireling = World.GetActorByGlobalId(World.GetGlobalId(this, message.HirelingID));
        if (hireling == null) return;
        switch (hireling.Attributes[GameAttribute.Hireling_Class])
        {
            case 1:
                if (hireling is not Templar templar) return;
                templar.SetSkill(this, message.PowerSNOId);
                break;
            case 2:
                if (hireling is not Scoundrel scoundrel) return;
                scoundrel.SetSkill(this, message.PowerSNOId);
                break;
            case 3:
                if (hireling is not Enchantress enchantress) return;
                enchantress.SetSkill(this, message.PowerSNOId);
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
                Revive(Position);
                ChangeWorld(World.Game.StartingWorld, World.Game.StartPosition);
                break;
            case 1:
                Revive(CheckPointPosition);
                break;
            case 2:
                if (Attributes[GameAttribute.Corpse_Resurrection_Charges] > 0)
                {
                    Revive(Position);
                    Attributes[GameAttribute.Corpse_Resurrection_Charges]--;
                }

                break;
        }
    }

    //*/
    private void OnEquipPotion(GameClient client, ChangeUsableItemMessage message)
    {
        var activeSkills = Toon.DBActiveSkills;
        activeSkills.PotionGBID = message.Field1;
        World.Game.GameDbSession.SessionUpdate(activeSkills);
    }

    public void ToonStateChanged()
    {
        try
        {
            ClientSystem.GameServer.GSBackend.ToonStateChanged(Toon.PersistentID);
        }
        catch (Exception e)
        {
            Logger.WarnException(e, "Exception on ToonStateChanged(): ");
        }
    }

    private void OnArtisanWindowClosed()
    {
        CurrentArtisan = null;
    }

    //*
    private void TrainArtisan(GameClient client, RequestTrainArtisanMessage message)
    {
        if (CurrentArtisan == null || !artisanTrainHelpers.ContainsKey(CurrentArtisan.Value))
        {
            Logger.Warn("Training for artisan {} is not supported", CurrentArtisan);
            return;
        }

        var trainHelper = artisanTrainHelpers[CurrentArtisan.Value];
        if (trainHelper.HasMaxLevel)
            return;

        var recipeDefinition = ItemGenerator.GetRecipeDefinition(trainHelper.TrainRecipeName);
        if (Inventory.GetGoldAmount() < recipeDefinition.Gold)
            return;

        var requiredIngridients = recipeDefinition.Ingredients.Where(x => x.ItemsGBID > 0);
        // FIXME: Inventory.HaveEnough doesn't work for some craft consumables
        var haveEnoughIngredients = requiredIngridients.All(x => Inventory.HaveEnough(x.ItemsGBID, x.Count));
        if (!haveEnoughIngredients)
            return;

        Inventory.RemoveGoldAmount(recipeDefinition.Gold);
        foreach (var ingr in requiredIngridients)
            // FIXME: Inventory.GrabSomeItems doesn't work for some craft consumables
            Inventory.GrabSomeItems(ingr.ItemsGBID, ingr.Count);

        trainHelper.DbRef.Level++;
        World.Game.GameDbSession.SessionUpdate(trainHelper.DbRef);

        if (trainHelper.Achievement is not null)
            GrantAchievement(trainHelper.Achievement.Value);
        if (trainHelper.Criteria is not null)
            GrantCriteria(trainHelper.Criteria.Value);

        if (artisanTrainHelpers.All(x => x.Value.HasMaxLevel))
            GrantCriteria(74987249993545);

        client.SendMessage(new CrafterLevelUpMessage
        {
            Type = trainHelper.Type,
            AnimTag = trainHelper.AnimationTag,
            NewIdle = trainHelper.IdleAnimationTag,
            Level = trainHelper.DbRef.Level
        });

        LoadCrafterData();


        /**/
    }

    public void UnlockTransmog(int transmogGBID)
    {
        if (learnedTransmogs.Contains(transmogGBID)) return;
        InGameClient.SendMessage(new UnlockTransmogMessage() { TransmogGBID = transmogGBID });

        Logger.Trace("Learning transmog #{0}", transmogGBID);
        learnedTransmogs.Add(transmogGBID);
        mystic_data.LearnedRecipes = SerializeBytes(learnedTransmogs);
        World.Game.GameDbSession.SessionUpdate(mystic_data);

        LoadCrafterData();
    }

    #endregion

    #region update-logic

    private int PreviousLevelArea = -1;

    private List<TickTimer> TimedActions = new();

    public int VaultsDone = 0;
    public int SpikeTrapsKilled = 0;

    public void AddTimedAction(float seconds, Action<int> onTimeout)
    {
        TimedActions.Add(TickTimer.WaitSeconds(World.Game, seconds, onTimeout));
    }

    public void Update(int tickCounter)
    {
        if (BetweenWorlds) return;

#if DEBUG
#else
			if ((this.InGameClient.Game.TickCounter - this.LastMovementTick) > 54000) //15m AFK
			{

				this.InGameClient.SendMessage(new SimpleMessage(Opcodes.CloseGameMessage));
			}
#endif

        // Check the gold
        if (InGameClient.Game.TickCounter % 120 == 0 && World != null && GoldCollectedTempCount > 0)
        {
            Toon.GameAccount.Gold += (ulong)GoldCollectedTempCount;
            Toon.CollectedGold += (ulong)GoldCollectedTempCount;

            if (World.Game.IsHardcore)
                Toon.CollectedGoldSeasonal += GoldCollectedTempCount;

            UpdateAchievementCounter(10, (uint)GoldCollectedTempCount);

            GoldCollectedTempCount = 0;
        }

        // Check the blood shards
        if (InGameClient.Game.TickCounter % 120 == 0 && World != null && BloodShardsCollectedTempCount > 0)
        {
            Toon.GameAccount.BloodShards += BloodShardsCollectedTempCount;
            Toon.GameAccount.TotalBloodShards += BloodShardsCollectedTempCount;
            BloodShardsCollectedTempCount = 0;
        }

        if (World != null && SkillSet.HasPassive(298038) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new UnwaveringWillBuff());


        if (World != null && SkillSet.HasSkill(312736) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new MonkDashingStrike.DashingStrikeCountBuff());
        else if (!SkillSet.HasSkill(312736))
            Attributes[GameAttribute.Skill_Charges, 312736] = 0;

        if (World != null && SkillSet.HasSkill(129217) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new Sentry.SentryCountBuff());
        else if (!SkillSet.HasSkill(129217))
            Attributes[GameAttribute.Skill_Charges, 129217] = 0;

        if (World != null && SkillSet.HasSkill(75301) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new SpikeTrap.SpikeCountBuff());
        else if (!SkillSet.HasSkill(75301))
            Attributes[GameAttribute.Skill_Charges, 75301] = 0;

        if (World != null && SkillSet.HasSkill(464896) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new BoneSpirit.SpiritCountBuff());
        else if (!SkillSet.HasSkill(464896))
            Attributes[GameAttribute.Skill_Charges, 464896] = 0;

        if (World != null && SkillSet.HasSkill(97435) && InGameClient.Game.TickCounter - LastMovementTick > 90)
            World.BuffManager.AddBuff(this, this, new FuriousCharge.FuriousChargeCountBuff());
        else if (!SkillSet.HasSkill(97435))
            Attributes[GameAttribute.Skill_Charges, 97435] = 0;

        Attributes.BroadcastChangedIfRevealed();
        lock (TimedActions)
        {
            foreach (var timed_action in TimedActions)
                timed_action.Update(tickCounter);
        }

        foreach (var timed_out in TimedActions.Where(t => t.TimedOut).ToList())
            TimedActions.Remove(timed_out);

        // Check the Killstreaks
        ExpBonusData.Check(0);
        ExpBonusData.Check(1);

        // Check if there is an conversation to close in this tick
        Conversations.Update(World.Game.TickCounter);

        foreach (var proximityGizmo in GetObjectsInRange<Actor>(20f, true))
        {
            if (proximityGizmo == null || proximityGizmo.SNO == ActorSno.__NONE) continue;
            if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)proximityGizmo.SNO) &&
                proximityGizmo.Visible) //EnterTrigger
            {
                var trigger = World.Game.QuestProgress.QuestTriggers[(int)proximityGizmo.SNO];
                if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
                    //this.World.Game.Quests.NotifyQuest(this.World.Game.CurrentQuest, Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, proximityGizmo.ActorSNO.Id);
                    try
                    {
                        trigger.questEvent.Execute(World); // launch a questEvent
                    }
                    catch (Exception e)
                    {
                        Logger.WarnException(e, "questEvent()");
                    }
            }
            else if (World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)proximityGizmo.SNO))
            {
                var trigger = World.Game.SideQuestProgress.QuestTriggers[(int)proximityGizmo.SNO];
                if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
                {
                    World.Game.SideQuestProgress.UpdateSideCounter((int)proximityGizmo.SNO);
                    if (trigger.count == World.Game.SideQuestProgress.QuestTriggers[(int)proximityGizmo.SNO].counter)
                        trigger.questEvent.Execute(World); // launch a questEvent
                }
            }

            if (World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey((int)proximityGizmo.SNO) &&
                proximityGizmo.Visible) //EnterTrigger
            {
                var trigger = World.Game.SideQuestProgress.GlobalQuestTriggers[(int)proximityGizmo.SNO];
                if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger)
                    //this.World.Game.Quests.NotifyQuest(this.World.Game.CurrentQuest, Mooege.Common.MPQ.FileFormats.QuestStepObjectiveType.EnterTrigger, proximityGizmo.ActorSNO.Id);
                    try
                    {
                        trigger.questEvent.Execute(World); // launch a questEvent
                        World.Game.SideQuestProgress.GlobalQuestTriggers.Remove((int)proximityGizmo.SNO);
                    }
                    catch (Exception e)
                    {
                        Logger.WarnException(e, "questEvent()");
                    }
            }
        }

        _UpdateResources();

        if (IsCasting) UpdateCastState();

        if (InGameClient.Game.TickCounter % 60 == 0 && World != null)
        {
            var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
            var scenes = World.QuadTree.Query<Scene>(proximity);
            if (scenes.Count == 0) return;
            var scene = scenes[0];
            if (PreviousLevelArea != scene.Specification.SNOLevelAreas[0])
            {
                PreviousLevelArea = scene.Specification.SNOLevelAreas[0];
                World.Game.WorldGenerator.CheckLevelArea(World, PreviousLevelArea);
                if (InGameClient.Game.TickCounter % 600 == 0)
                    CheckLevelAreaCriteria(PreviousLevelArea);
            }
        }

        if (InGameClient.Game.TickCounter % 600 == 0 && World != null)
        {
            if (KilledMonstersTempCount != 0)
            {
                Toon.TotalKilled += (ulong)KilledMonstersTempCount;
                KilledMonstersTempCount = 0;

                if (KilledElitesTempCount != 0)
                {
                    Toon.ElitesKilled += (ulong)KilledElitesTempCount;
                    KilledElitesTempCount = 0;
                }

                if (KilledSeasonalTempCount != 0)
                {
                    Toon.SeasonalKills += KilledSeasonalTempCount;
                    KilledSeasonalTempCount = 0;
                }
            }

            CheckAchievementCounters();
        }

        #region Necromancer summons

        var switchertobool = false;
        var switchertoboolTwo = false;
        ActiveSkillSavedData NowSkillGolem = null;
        foreach (var skill in SkillSet.ActiveSkills)
            if (skill.snoSkill == 453801)
                switchertobool = true;
        foreach (var skill in SkillSet.ActiveSkills)
            if (skill.snoSkill == 451537)
            {
                switchertoboolTwo = true;
                NowSkillGolem = skill;
            }

        ActiveSkeletons = switchertobool;
        EnableGolem = switchertoboolTwo;


        var Killer = new PowerContext();
        Killer.User = this;
        Killer.World = World;
        Killer.PowerSNO = -1;

        if (ActiveSkeletons)
        {
            if (Followers.All(s => s.Value != ActorSno._p6_necro_commandskeletons_a) && NecroSkeletons.Any())
            {
                foreach (var skeleton in NecroSkeletons)
                {
                    try
                    {
                        InGameClient.SendMessage(new PetDetachMessage()
                        {
                            PetId = skeleton.GlobalID
                        });
                        World.Leave(skeleton);
                    }
                    catch{}
                }

                NecroSkeletons.Clear();
            }
            while (NecroSkeletons.Count < 7)
            {
                var Skeleton = new NecromancerSkeleton_A(World, ActorSno._p6_necro_commandskeletons_a, this);
                Skeleton.Brain.DeActivate();
                Skeleton.Scale = 1.2f;

                Skeleton.EnterWorld(PowerContext.RandomDirection(Position, 3f, 8f));
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
                InGameClient.SendMessage(new PetDetachMessage()
                {
                    PetId = skel.GlobalID
                });
                World.Leave(skel);
            }

            NecroSkeletons.Clear();
        }

        if (EnableGolem || ActiveGolem != null)
        {
            var runeActorSno = RuneSelect(451537, ActorSno._p6_necro_revive_golem, ActorSno._p6_bonegolem,
                ActorSno._p6_bloodgolem, ActorSno._p6_consumefleshgolem, ActorSno._p6_decaygolem,
                ActorSno._p6_icegolem);
            if (ActiveGolem != null)
            {
                if (ActiveGolem.SNO != runeActorSno || !SkillSet.HasSkill(451537))
                {
                    if (ActiveGolem.World != null)
                    {
                        if (!ActiveGolem.IsRevealedToPlayer(this))
                            InGameClient.SendMessage(new PetDetachMessage()
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
                if (Attributes[GameAttribute.Power_Cooldown, 451537] > InGameClient.Game.TickCounter)
                {
                }
                else
                {
                    switch (runeActorSno)
                    {
                        case ActorSno._p6_necro_revive_golem:
                            var NGolem = new BaseGolem(World, this);
                            NGolem.Brain.DeActivate();
                            NGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
                            NGolem.Attributes[GameAttribute.Untargetable] = true;
                            NGolem.EnterWorld(NGolem.Position);


                            //(NGolem as BaseGolem).Brain.Activate();
                            NGolem.Attributes[GameAttribute.Untargetable] = false;
                            NGolem.Attributes.BroadcastChangedIfRevealed();
                            ActiveGolem = NGolem;
                            break;
                        case ActorSno._p6_consumefleshgolem:
                            var CFGolem = new ConsumeFleshGolem(World, this);
                            CFGolem.Brain.DeActivate();
                            CFGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
                            CFGolem.Attributes[GameAttribute.Untargetable] = true;
                            CFGolem.EnterWorld(CFGolem.Position);


                            //(CFGolem as ConsumeFleshGolem).Brain.Activate();
                            CFGolem.Attributes[GameAttribute.Untargetable] = false;
                            CFGolem.Attributes.BroadcastChangedIfRevealed();
                            ActiveGolem = CFGolem;

                            break;
                        case ActorSno._p6_icegolem:
                            var IGolem = new IceGolem(World, this);
                            IGolem.Brain.DeActivate();
                            IGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
                            IGolem.Attributes[GameAttribute.Untargetable] = true;
                            IGolem.EnterWorld(IGolem.Position);


                            //(IGolem as IceGolem).Brain.Activate();
                            IGolem.Attributes[GameAttribute.Untargetable] = false;
                            IGolem.Attributes.BroadcastChangedIfRevealed();
                            ActiveGolem = IGolem;
                            break;
                        case ActorSno._p6_bonegolem:
                            var BGolem = new BoneGolem(World, this);
                            BGolem.Brain.DeActivate();
                            BGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
                            BGolem.Attributes[GameAttribute.Untargetable] = true;
                            BGolem.EnterWorld(BGolem.Position);


                            //(BGolem as BoneGolem).Brain.Activate();
                            BGolem.Attributes[GameAttribute.Untargetable] = false;
                            BGolem.Attributes.BroadcastChangedIfRevealed();
                            ActiveGolem = BGolem;
                            break;
                        case ActorSno._p6_decaygolem:
                            var DGolem = new DecayGolem(World, this);
                            DGolem.Brain.DeActivate();
                            DGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
                            DGolem.Attributes[GameAttribute.Untargetable] = true;
                            DGolem.EnterWorld(DGolem.Position);


                            //(DGolem as DecayGolem).Brain.Activate();
                            DGolem.Attributes[GameAttribute.Untargetable] = false;
                            DGolem.Attributes.BroadcastChangedIfRevealed();
                            ActiveGolem = DGolem;
                            break;
                        case ActorSno._p6_bloodgolem:
                            var BlGolem = new BloodGolem(World, this);
                            BlGolem.Brain.DeActivate();
                            BlGolem.Position =
                                RandomDirection(Position, 3f,
                                    8f); //Kind of hacky until we get proper collisiondetection
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
                    ActiveGolem.PlayActionAnimation(AnimationSno.p6_bloodgolem_spawn_01);
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
        var Rune_A = Attributes[GameAttribute.Rune_A, PowerSNO];
        var Rune_B = Attributes[GameAttribute.Rune_B, PowerSNO];
        var Rune_C = Attributes[GameAttribute.Rune_C, PowerSNO];
        var Rune_D = Attributes[GameAttribute.Rune_D, PowerSNO];
        var Rune_E = Attributes[GameAttribute.Rune_E, PowerSNO];

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
        var scenes_around = World.Scenes.Values.ToList();
        if (!World.worldData.DynamicWorld)
            scenes_around = GetScenesInRegion(DefaultQueryProximityLenght * 3);

        foreach (var scene in scenes_around) // reveal scenes in player's proximity.
        {
            if (scene.IsRevealedToPlayer(this)) // if the actors is already revealed skip it.
                continue; // if the scene is already revealed, skip it.

            if (scene.Parent !=
                null) // if it's a subscene, always make sure it's parent get reveals first and then it reveals his childs.
                scene.Parent.Reveal(this);
            else
                scene.Reveal(this);
        }

        foreach (var scene in World.Scenes.Values) // unreveal far scenes
        {
            if (!scene.IsRevealedToPlayer(this) || scenes_around.Contains(scene))
                continue;

            if (scene.Parent !=
                null) // if it's a subscene, always make sure it's parent get reveals first and then it reveals his childs.
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
        var Range = 200f;
        if (InGameClient.Game.CurrentEncounter.Activated)
            Range = 360f;

        var specialWorlds = new WorldSno[]
        {
            WorldSno.x1_pand_batteringram,
            WorldSno.gluttony_boss,
            WorldSno.a3dun_hub_adria_tower,
            WorldSno.x1_malthael_boss_arena,
            WorldSno.a1trdun_level05_templar
        };

        var actors_around = specialWorlds.Contains(World.SNO) ? World.Actors.Values.ToList() : GetActorsInRange(Range);

        foreach (var actor in actors_around) // reveal actors in player's proximity.
        {
            if (actor is Player) // if the actors is already revealed, skip it.
                continue;

            if (World.SNO == WorldSno.x1_tristram_adventure_mode_hub && actor is Portal portal)
                if (portal.Destination.WorldSNO == (int)WorldSno.x1_tristram_adventure_mode_hub)
                    continue;
            if (World.SNO == WorldSno.trout_town && actor is Portal portal1)
                if (portal1.Destination.WorldSNO == (int)WorldSno.trout_town &&
                    portal1.Destination.DestLevelAreaSNO == 19947)
                {
                    portal1.Destination.WorldSNO = (int)WorldSno.x1_tristram_adventure_mode_hub;
                    portal1.Destination.StartingPointActorTag = 483;
                }

            if (actor.ActorType != ActorType.ClientEffect && actor.ActorType != ActorType.AxeSymbol &&
                actor.ActorType != ActorType.CustomBrain) actor.Reveal(this);
        }

        foreach (var actor in World.Actors.Values) // unreveal far actors
        {
            if ((actor is Player && (!World.IsPvP || actor == this)) ||
                actors_around.Contains(actor)) // if the actors is already revealed, skip it.
                continue;

            actor.Unreveal(this);
        }
    }

    /// <summary>
    /// Reveals other players in player's proximity.
    /// </summary>
    public void RevealPlayersToPlayer()
    {
        var actors = GetActorsInRange<Player>(100f);

        foreach (var actor in actors) // reveal actors in player's proximity.
        {
            if (actor.IsRevealedToPlayer(this)) // if the actors is already revealed, skip it.
                continue;

            actor.Reveal(this);

            if (!IsRevealedToPlayer(actor))
                Reveal(actor);
        }
    }

    public void ReRevealPlayersToPlayer()
    {
        var actors = GetActorsInRange<Player>(100f);

        foreach (var actor in actors) // reveal actors in player's proximity.
        {
            if (actor.IsRevealedToPlayer(this)) // if the actors is already revealed, skip it.
                actor.Unreveal(this);

            actor.Reveal(this);

            if (!IsRevealedToPlayer(actor))
            {
                Reveal(actor);
            }
            else
            {
                Unreveal(actor);
                Reveal(actor);
            }
        }
    }

    public void ClearDoorAnimations()
    {
        var doors = GetActorsInRange<Door>(100f);
        foreach (var door in doors)
            if (door.IsRevealedToPlayer(this))
                InGameClient.SendMessage(new SetIdleAnimationMessage
                {
                    ActorID = door.DynamicID(this),
                    AnimationSNO = AnimationSetKeys.Open.ID
                });
    }

    private bool _motdSent;
    public override void OnEnter(World world)
    {
        world.Reveal(this);
        Unreveal(this);

        if (_CurrentHPValue == -1f)
            DefaultQueryProximityRadius = 60;

        InGameClient.SendMessage(new EnterWorldMessage()
        {
            EnterPosition = Position,
            WorldID = world.GlobalID,
            WorldSNO = (int)world.SNO,
            PlayerIndex = PlayerIndex,
            EnterLookUsed = true,
            EnterKnownLookOverrides = new EnterKnownLookOverrides { Field0 = new int[] { -1, -1, -1, -1, -1, -1 } }
        });

        switch (world.SNO)
        {
            case WorldSno.x1_westmarch_overlook_d:
                InGameClient.SendMessage(new PlayerSetCameraObserverMessage()
                {
                    Field0 = 309026,
                    Field1 = new WorldPlace() { Position = new Vector3D(), WorldID = 0 }
                });
                break;
            case WorldSno.x1_westm_intro:
                InGameClient.SendMessage(new PlayerSetCameraObserverMessage()
                {
                    Field0 = 1541,
                    Field1 = new WorldPlace() { Position = new Vector3D(), WorldID = 0 }
                });
                break;
        }

        if (_CurrentHPValue == -1f)
            AddPercentageHP(100);

        DefaultQueryProximityRadius = 100;

        RevealScenesToPlayer();
        RevealPlayersToPlayer();

        // load all inventory items
        if (!Inventory.Loaded)
        {
            //why reload if already loaded?
            Inventory.LoadFromDB();
            Inventory.LoadStashFromDB();
        }
        else
        {
            Inventory.RefreshInventoryToClient();
        }

        // generate visual update message
        //this.Inventory.SendVisualInventory(this);
        SetAllStatsInCorrectOrder();
        SetAttributesSkillSets();
        if (World.IsPvP)
            DisableStoneOfRecall();
        else
            EnableStoneOfRecall();

        Reveal(this);

        System.Threading.Tasks.Task.Delay(3).Wait();
        RevealActorsToPlayer();
        if (!_motdSent && LoginServer.Config.Instance.MotdEnabled)
            InGameClient.BnetClient.SendMotd();
        //
    }

    public override void OnTeleport()
    {
        Unreveal(this);
        BeforeChangeWorld();
        RevealScenesToPlayer(); // reveal scenes in players proximity.
        RevealPlayersToPlayer();
        RevealActorsToPlayer(); // reveal actors in players proximity.
        //TickTimer.WaitSeconds(this.World.Game, 5.0f, new Action<int>((x) => Logger.Debug("Timer")));
        Reveal(this);
        AfterChangeWorld();
        // load all inventory items
        if (!Inventory.Loaded)
        {
            //why reload if already loaded?
            Inventory.LoadFromDB();
            Inventory.LoadStashFromDB();
        }
        else
        {
            Inventory.RefreshInventoryToClient();
        }
    }

    public override void OnLeave(World world)
    {
        Conversations.StopAll();

        // save visual equipment
        Toon.HeroVisualEquipmentField.Value = Inventory.GetVisualEquipment();
        //this.Toon.HeroLevelField.Value = this.Attributes[GameAttribute.Level];
        Toon.GameAccount.ChangedFields.SetPresenceFieldValue(Toon.HeroVisualEquipmentField);
        Toon.GameAccount.ChangedFields.SetPresenceFieldValue(Toon.HeroLevelField);
        Toon.GameAccount.ChangedFields.SetPresenceFieldValue(Toon.HeroParagonLevelField);
        world.Unreveal(this);
    }

    public override bool Reveal(Player player)
    {
        if (!base.Reveal(player))
            return false;

        if (!World.IsPvP || this == player)
            player.InGameClient.SendMessage(new PlayerEnterKnownMessage()
            {
                PlayerIndex = PlayerIndex,
                ActorId = DynamicID(player)
            });

        Inventory.SendVisualInventory(player);

        if (this == player) // only send this to player itself. Warning: don't remove this check or you'll make the game start crashing! /raist.
            player.InGameClient.SendMessage(new PlayerActorSetInitialMessage()
            {
                ActorId = DynamicID(player),
                PlayerIndex = PlayerIndex
            });

        if (!base.Reveal(player))
            Inventory.Reveal(player);

        if (this == player) // only send this when player's own actor being is revealed. /raist.
            player.InGameClient.SendMessage(new PlayerWarpedMessage()
            {
                WarpReason = 9,
                WarpFadeInSecods = 0f
            });

        if (SkillSet.HasSkill(460757)) // P6_Necro_Devour
            foreach (var skill in SkillSet.ActiveSkills)
                if (skill.snoSkill == 460757) // P6_Necro_Devour
                    if (skill.snoRune == 3)
                        World.BuffManager.AddBuff(this, this, new P6_Necro_Devour_Aura());
                    else
                        World.BuffManager.RemoveBuffs(this, 474325);

        return true;
    }

    public override bool Unreveal(Player player)
    {
        if (!base.Unreveal(player))
            return false;

        Inventory.Unreveal(player);

        return true;
    }

    public Dictionary<Buff, int> AllBuffs = new();

    public bool BetweenWorlds = false;

    public override void BeforeChangeWorld()
    {
        ClearDoorAnimations();
        World.Game.QuestManager.UnsetBountyMarker(this);
        BetweenWorlds = true;
        AllBuffs = World.BuffManager.GetAllBuffs(this);
        World.BuffManager.RemoveAllBuffs(this);
        //this.Inventory.Unreveal(this);
        //this.InGameClient.TickingEnabled = false;
        /*this.InGameClient.SendMessage(new FreezeGameMessage
        {
            Field0 = true
        });*/

        InGameClient.SendMessage(new ACDTranslateSyncMessage()
        {
            ActorId = DynamicID(this),
            Position = Position
        });

        _CurrentHPValue = Attributes[GameAttribute.Hitpoints_Cur];
        _CurrentResourceValue = Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1];
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
        Inventory.Reveal(this);

        foreach (var buff in AllBuffs)
            World.BuffManager.CopyBuff(this, this, buff.Key, buff.Value);
        AllBuffs.Clear();
        BetweenWorlds = false;

        if (Math.Abs(_CurrentHPValue - (-1)) > 0.0001)
        {
            Attributes[GameAttribute.Hitpoints_Cur] = _CurrentHPValue;
            Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource + 1] = _CurrentResourceValue;
            Attributes.BroadcastChangedIfRevealed();
            _CurrentHPValue = -1;
        }

        World.Game.QuestManager.SetBountyMarker(this);


        //System.Threading.Tasks.Task.Delay(1000).ContinueWith(a => {this.BetweenWorlds = false;});
    }

    #endregion

    #region hero-state

    [Obsolete("Does this make sense?")]
    private void WTF()
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
        InGameClient.SendMessage(new HeroStateMessage
        {
            State = GetStateData(),
            PlayerIndex = PlayerIndex
        });
    }

    public HeroStateData GetStateData()
    {
        return new HeroStateData()
        {
            LastPlayedAct = 400, //LastPlayedAct
            HighestUnlockedAct = 400, //HighestUnlockedAct
            PlayedFlags = (int)Toon.Flags,
            PlayerSavedData = GetSavedData(),
            //QuestRewardHistoryEntriesCount = QuestRewardHistory.Count,
            tQuestRewardHistory = QuestRewardHistory.ToArray()
        };
    }

    #endregion

    #region player attribute handling

    public void QueueDeath(bool state)
    {
        //this.World.BroadcastIfRevealed(this.ACDWorldPositionMessage, this);
        InGameClient.SendMessage(new ACDTranslateSyncMessage()
        {
            ActorId = DynamicID(this),
            Position = Position
        });
        Attributes[GameAttribute.QueueDeath] = state;
        Attributes[GameAttribute.Disabled] = state;
        Attributes[GameAttribute.Waiting_To_Accept_Resurrection] = false;
        Attributes[GameAttribute.Invulnerable] = state;
        //this.Attributes[GameAttribute.Stunned] = state;
        Attributes[GameAttribute.Immobolize] = state;
        Attributes[GameAttribute.Hidden] = state;
        Attributes[GameAttribute.Untargetable] = state;
        Attributes[GameAttribute.CantStartDisplayedPowers] = state;
        Attributes[GameAttribute.IsContentRestrictedActor] = state;

        Attributes[GameAttribute.Rest_Experience_Lo] = 0;
        Attributes[GameAttribute.Rest_Experience_Bonus_Percent] = 0;

        Attributes.BroadcastChangedIfRevealed();
        if (World.Game.PvP) Attributes[GameAttribute.Resurrect_As_Observer] = state;
        //this.Attributes[GameAttribute.Observer] = !state;
        //this.Attributes[GameAttribute.Corpse_Resurrection_Charges] = 1;	// Enable this to allow unlimited resurrection at corpse
        //this.Attributes[GameAttribute.Corpse_Resurrection_Allowed_Game_Time] = this.World.Game.TickCounter + 300; // Timer for auto-revive (seems to be broken?)
        Attributes.BroadcastChangedIfRevealed();
    }

    public void Resurrect()
    {
        Attributes[GameAttribute.Waiting_To_Accept_Resurrection] = true;
        Attributes.BroadcastChangedIfRevealed();
    }

    public void Revive(Vector3D spawnPosition)
    {
        if (World == null) return;
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
        QueueDeath(false);
        Dead = false;
        AddPercentageHP(100);

        World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
        {
            ActorID = DynamicID(plr),
            AnimationSNO = AnimationSetKeys.IdleDefault.ID
        }, this);

        //removing tomb
        try
        {
            GetObjectsInRange<Headstone>(100.0f).Where(h => h.playerIndex == PlayerIndex).First().Destroy();
        }
        catch
        {
        }

        Teleport(spawnPosition);
        World.BuffManager.AddBuff(this, this, new ActorGhostedBuff());

        var old_skills = SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
        foreach (var skill in old_skills)
        {
            var power = PowerLoader.CreateImplementationForPowerSNO(skill);
            if (power != null && power.EvalTag(PowerKeys.SynergyPower) != -1)
                World.BuffManager.RemoveBuffs(this, power.EvalTag(PowerKeys.SynergyPower));
        }

        SetAttributesByItems();
        SetAttributesByItemProcs();
        SetAttributesByGems();
        SetAttributesByItemSets();
        SetAttributesByPassives();
        SetAttributesByParagon();
        SetAttributesSkillSets();

        Attributes[GameAttribute.Resource_Cur, PrimaryResourceID] = 0f;
        if (Toon.Class == ToonClass.DemonHunter)
            Attributes[GameAttribute.Resource_Cur, SecondaryResourceID] = 0f;
        Attributes.BroadcastChangedIfRevealed();

        var skills = SkillSet.ActiveSkills.Select(s => s.snoSkill).ToList();
        var cooldowns = World.BuffManager.GetBuffs<CooldownBuff>(this);
        foreach (var skill in skills)
        {
            var inCooldown = false;
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

        Inventory.RefreshInventoryToClient();
        UpdatePercentageHP(PercHPbeforeChange);
    }


    public float Strength
    {
        get
        {
            var baseStrength = 0.0f;

            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Strength)
                baseStrength = Toon.HeroTable.Strength + (Level - 1) * 3;
            else
                baseStrength = Toon.HeroTable.Strength + (Level - 1);

            return baseStrength;
        }
    }

    public float TotalStrength =>
        Attributes[GameAttribute.Strength] + Inventory.GetItemBonus(GameAttribute.Strength_Item);

    public float Dexterity
    {
        get
        {
            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Dexterity)
                return Toon.HeroTable.Dexterity + (Level - 1) * 3;
            else
                return Toon.HeroTable.Dexterity + (Level - 1);
        }
    }

    public float TotalDexterity =>
        Attributes[GameAttribute.Dexterity] + Inventory.GetItemBonus(GameAttribute.Dexterity_Item);

    public float Vitality => Toon.HeroTable.Vitality + (Level - 1) * 2;

    public float TotalVitality =>
        Attributes[GameAttribute.Vitality] + Inventory.GetItemBonus(GameAttribute.Vitality_Item);

    public float Intelligence
    {
        get
        {
            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Intelligence)
                return Toon.HeroTable.Intelligence + (Level - 1) * 3;
            else
                return Toon.HeroTable.Intelligence + (Level - 1);
        }
    }

    public float TotalIntelligence => Attributes[GameAttribute.Intelligence] +
                                      Inventory.GetItemBonus(GameAttribute.Intelligence_Item);

    public float PrimaryAttribute
    {
        get
        {
            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Strength) return TotalStrength;
            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Dexterity) return TotalDexterity;
            if (Toon.HeroTable.CoreAttribute == GameBalance.PrimaryAttribute.Intelligence) return TotalIntelligence;
            return 0f;
        }
    }

    public float DodgeChance
    {
        get
        {
            var dex = TotalDexterity;
            var dodgeChance = dex / (250f * Attributes[GameAttribute.Level] + dex);

            if (dex > 7500f) dodgeChance += 0.04f;
            else if (dex > 6500f) dodgeChance += 0.02f;
            else if (dex > 5500f) dodgeChance += 0.01f;

            dodgeChance = 1f - (1f - dodgeChance) * (1f - Attributes[GameAttribute.Dodge_Chance_Bonus]);

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
            HotBarButtons = SkillSet.HotBarSkills,
            HotBarButton = new HotbarButtonData
            {
                SNOSkill = -1, RuneType = -1, ItemGBId =
                    StringHashHelper.HashItemName(
                        "HealthPotionBottomless") //2142362846//this.Toon.DBActiveSkills.PotionGBID
                ,
                ItemAnn = -1
            },
            SkillSlotEverAssigned = 0x0F, //0xB4,
            PlaytimeTotal = Toon.TimePlayed,
#if DEBUG
            WaypointFlags = 0x0000ffff,
#else
					WaypointFlags = this.World.Game.WaypointFlags,
#endif

            HirelingData = new HirelingSavedData()
            {
                HirelingInfos = HirelingInfo,
                ActiveHireling = 0x00000000,
                AvailableHirelings = 0x00000004
            },

            TimeLastLevel = 0,
            LearnedLore = LearnedLore,

            ActiveSkills = SkillSet.ActiveSkills,
            snoTraits = SkillSet.PassiveSkills,
            GBIDLegendaryPowers = new int[4] { -1, -1, -1, -1 },

            SavePointData = new SavePointData { snoWorld = -1, SavepointId = -1 },
            EventFlags = 0
        };
    }

    public SavePointData SavePointData { get; set; }

    public LearnedLore LearnedLore = new()
    {
        Count = 0x00000000,
        m_snoLoreLearned = new int[512]
        {
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000
        }
    };

    public void SaveStats() //Save 6 primary stats into DB for showing on hero screen
    {
        //Logger.MethodTrace("Strength {0}", this.Inventory.GetItemBonus(GameAttribute.Strength_Item).ToString("F0"));
        var damageFromWeapon =
            (Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Min_Total, 0) +
             Inventory.GetItemBonus(GameAttribute.Damage_Weapon_Delta_Total, 0)) * (1f + PrimaryAttribute / 100f);

        var totalDamage =
            (damageFromWeapon
             + damageFromWeapon * Inventory.GetItemBonus(GameAttribute.Weapon_Crit_Chance) *
             (1.5f + Inventory.GetItemBonus(GameAttribute.Crit_Damage_Percent)))
            * Inventory.GetItemBonus(GameAttribute.Attacks_Per_Second_Total);

        var serialized = "";
        serialized += Inventory.GetItemBonus(GameAttribute.Strength_Item).ToString("F0");
        serialized += ";";
        serialized += Inventory.GetItemBonus(GameAttribute.Dexterity_Item).ToString("F0");
        serialized += ";";
        serialized += Inventory.GetItemBonus(GameAttribute.Intelligence_Item).ToString("F0");
        serialized += ";";
        serialized += Inventory.GetItemBonus(GameAttribute.Vitality_Item).ToString("F0");
        serialized += ";";
        serialized += Inventory.GetItemBonus(GameAttribute.Armor_Item).ToString("F0");
        serialized += ";";
        serialized += totalDamage.ToString("F0");
        var dbStats = Toon.DBToon;
        dbStats.Stats = serialized;
        World.Game.GameDbSession.SessionUpdate(dbStats);
    }

    public List<PlayerQuestRewardHistoryEntry> QuestRewardHistory
    {
        get
        {
            var result = new List<PlayerQuestRewardHistoryEntry>();
            var quests = InGameClient.Game.QuestManager.Quests.Where(q => q.Value.Completed).ToList();
            foreach (var quest in quests)
            {
                InGameClient.SendMessage(new QuestUpdateMessage()
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
                    Field2 = (PlayerQuestRewardHistoryEntry.Difficulty)InGameClient.Game.Difficulty
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
                for (var i = 0; i < 4; i++)
                    _hirelingInfo[i] = GetHirelingInfo(i);
            }

            return _hirelingInfo;
        }
        set => _hirelingInfo = value;
    }

    #endregion

    #region cooked messages

    public void StopMoving()
    {
        World.BroadcastIfRevealed(plr => new ACDTranslateNormalMessage
        {
            ActorId = DynamicID(plr),
            Position = Position,
            SnapFacing = false,
            MovementSpeed = 0,
            AnimationTag = -1
        }, this);
    }

    public void CheckBonusSets()
    {
        var sets = World.Game.GameDbSession
            .SessionQueryWhere<DBBonusSets>(dbi => dbi.DBAccount.Id == Toon.GameAccount.AccountId).ToList();
        foreach (var bonusSet in sets)
        {
            if (World.Game.IsHardcore)
            {
                if (bonusSet.ClaimedHardcore) continue;
            }
            else
            {
                if (bonusSet.Claimed) continue;
            }

            //if (!BonusSetsList.CollectionEditions.ContainsKey(bonusSet.SetId)) continue;

            if (bonusSet.SetId == 6 && World.Game.IsHardcore) continue;

            //if (!(bonusSet.Claimed || bonusSet.ClaimedHardcore))
            //	BonusSetsList.CollectionEditions[bonusSet.SetId].ClaimOnce(this);

            if (World.Game.IsHardcore)
            {
                bonusSet.ClaimedHardcore = true;
            }
            else
            {
                bonusSet.Claimed = true;
                bonusSet.ClaimedToon = Toon.DBToon;
            }

            //BonusSetsList.CollectionEditions[bonusSet.SetId].Claim(this);
            World.Game.GameDbSession.SessionUpdate(bonusSet);
            //this.InGameClient.SendMessage(new BroadcastTextMessage() { Field0 = "You have been granted with gifts from bonus pack!" });
        }
    }

    public HirelingInfo GetHirelingInfo(int type)
    {
        var query = World.Game.GameDbSession
            .SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == Toon.PersistentID && dbh.Class == type).ToList();
        if (query.Count == 0)
        {
            //returns empty data
            var hireling_empty = new HirelingInfo
            {
                HirelingIndex = type, GbidName = 0x0000, Dead = false, Skill1SNOId = -1, Skill2SNOId = -1,
                Skill3SNOId = -1, Skill4SNOId = -1, annItems = -1
            };
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
        var recparts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        var ret = new List<int>();
        foreach (var recid in recparts) ret.Add(Convert.ToInt32(recid, 10));
        return ret;
    }

    private string Serialize(List<int> data)
    {
        var serialized = "";
        foreach (var id in data)
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

    public void LearnRecipe(ArtisanType? artisan, int recipe)
    {
        Logger.Trace("Learning recipe #{0}, Artisan type: {1}", recipe, artisan);
        /*var query = this.World.Game.GameDBSession.SessionQuerySingle<DBCraft>(
                dbi =>
                dbi.DBGameAccount.Id == this.Toon.GameAccount.PersistentID &&
                dbi.Artisan == artisan &&
                dbi.isHardcore == this.World.Game.IsHardcore);*/
        if (artisan == ArtisanType.Blacksmith)
        {
            learnedBlacksmithRecipes.Add(recipe);
            blacksmith_data.LearnedRecipes = SerializeBytes(learnedBlacksmithRecipes);
            World.Game.GameDbSession.SessionUpdate(blacksmith_data);
            UpdateAchievementCounter(404, 1, 0);
        }
        else if (artisan == ArtisanType.Jeweler)
        {
            learnedJewelerRecipes.Add(recipe);
            jeweler_data.LearnedRecipes = SerializeBytes(learnedJewelerRecipes);
            World.Game.GameDbSession.SessionUpdate(jeweler_data);
            UpdateAchievementCounter(404, 1, 1);
        }

        LoadCrafterData();
    }

    public bool RecipeAvailable(GameBalance.RecipeTable recipe_definition)
    {
        if (recipe_definition.Flags == 0) return true;
        return learnedBlacksmithRecipes.Contains(recipe_definition.Hash) ||
               learnedJewelerRecipes.Contains(recipe_definition.Hash);
    }

    public PlayerBannerMessage GetPlayerBanner()
    {
        var playerBanner = D3.GameMessage.PlayerBanner.CreateBuilder()
            .SetPlayerIndex((uint)PlayerIndex)
            .SetBanner(Toon.GameAccount.BannerConfigurationField.Value)
            .Build();

        return new PlayerBannerMessage() { PlayerBanner = playerBanner };
    }

    private List<int> learnedBlacksmithRecipes = new();
    private List<int> learnedJewelerRecipes = new();
    private List<int> learnedTransmogs = new();

    private DBCraft blacksmith_data = null;
    private DBCraft jeweler_data = null;
    private DBCraft mystic_data = null;
    private Dictionary<ArtisanType, ArtisanTrainHelper> artisanTrainHelpers = new();

    public void LoadCrafterData()
    {
        if (blacksmith_data == null)
        {
            var craft_data =
                World.Game.GameDbSession.SessionQueryWhere<DBCraft>(dbc =>
                    dbc.DBGameAccount.Id == Toon.GameAccount.PersistentID);

            blacksmith_data = craft_data.Single(dbc =>
                dbc.Artisan == "Blacksmith" && dbc.isHardcore == World.Game.IsHardcore &&
                dbc.isSeasoned == World.Game.IsSeasoned);
            jeweler_data = craft_data.Single(dbc =>
                dbc.Artisan == "Jeweler" && dbc.isHardcore == World.Game.IsHardcore &&
                dbc.isSeasoned == World.Game.IsSeasoned);
            mystic_data = craft_data.Single(dbc =>
                dbc.Artisan == "Mystic" && dbc.isHardcore == World.Game.IsHardcore &&
                dbc.isSeasoned == World.Game.IsSeasoned);

            artisanTrainHelpers[ArtisanType.Blacksmith] =
                new ArtisanTrainHelper(blacksmith_data, ArtisanType.Blacksmith);
            artisanTrainHelpers[ArtisanType.Jeweler] = new ArtisanTrainHelper(jeweler_data, ArtisanType.Jeweler);
            artisanTrainHelpers[ArtisanType.Mystic] = new ArtisanTrainHelper(mystic_data, ArtisanType.Mystic);
        }


        var blacksmith = D3.ItemCrafting.CrafterData.CreateBuilder()
            .SetLevel(InGameClient.Game.CurrentAct == 3000
                ? BlacksmithUnlocked == false && blacksmith_data.Level < 1 ? 1 : blacksmith_data.Level
                : blacksmith_data.Level)
            .SetCooldownEnd(0)
            .AddRangeRecipes(UnserializeBytes(blacksmith_data.LearnedRecipes))
            .Build();
        learnedBlacksmithRecipes = UnserializeBytes(blacksmith_data.LearnedRecipes);
        var jeweler = D3.ItemCrafting.CrafterData.CreateBuilder()
            .SetLevel(InGameClient.Game.CurrentAct == 3000
                ? JewelerUnlocked == false && jeweler_data.Level < 1 ? 1 : jeweler_data.Level
                : jeweler_data.Level)
            .SetCooldownEnd(0)
            .AddRangeRecipes(UnserializeBytes(jeweler_data.LearnedRecipes))
            .Build();
        learnedJewelerRecipes = UnserializeBytes(jeweler_data.LearnedRecipes);
        var mystic = D3.ItemCrafting.CrafterData.CreateBuilder()
            .SetLevel(InGameClient.Game.CurrentAct == 3000
                ? MysticUnlocked == false && mystic_data.Level < 1 ? 1 : mystic_data.Level
                : mystic_data.Level)
            .SetCooldownEnd(0)
            .Build();

        var transmog = D3.ItemCrafting.CrafterSavedData.CreateBuilder()
            .SetTransmogData(D3.GameBalance.BitPackedGbidArray.CreateBuilder()
                .SetBitfield(ByteString.CopyFrom(mystic_data.LearnedRecipes)))
            //.AddRangeUnlockedTransmogs(this.UnserializeBytes(mystic_data.LearnedRecipes))
            .Build();
        learnedTransmogs = UnserializeBytes(mystic_data.LearnedRecipes);

        if (BlacksmithUnlocked || InGameClient.Game.CurrentAct == 3000)
            InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataBlacksmithInitialMessage)
                { Data = blacksmith.ToByteArray() });

        if (JewelerUnlocked || InGameClient.Game.CurrentAct == 3000)
            InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataJewelerInitialMessage)
                { Data = jeweler.ToByteArray() });

        if (MysticUnlocked || InGameClient.Game.CurrentAct == 3000)
        {
            InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataMysticInitialMessage)
                { Data = mystic.ToByteArray() });
            InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CraftingDataTransmogInitialMessage)
                { Data = transmog.ToByteArray() });
        }
    }

    public void LoadCurrencyData()
    {
        var bloodShards = 0;
        bloodShards = Toon.GameAccount.BloodShards;
        Inventory.UpdateCurrencies();
    }

    public void LoadMailData()
    {
        var mail_data =
            World.Game.GameDbSession.SessionQueryWhere<DBMail>(dbm =>
                dbm.DBToon.Id == Toon.PersistentID && dbm.Claimed == false);
        var mails = D3.Items.Mails.CreateBuilder();
        foreach (var mail in mail_data)
        {
            var mail_row = D3.Items.Mail.CreateBuilder()
                .SetAccountTo(Toon.D3EntityID)
                .SetAccountFrom(Toon.D3EntityID)
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
                                .SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(mail.ItemGBID)
                                    .SetGameBalanceType(2))
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

        InGameClient.SendMessage(new MailDigestMessage() { MailContents = mail_contents });
    }

    //*/
    public void LoadStashIconsData()
    {
        var dbGAcc = Toon.GameAccount.DBGameAccount;
        if (dbGAcc.StashIcons == null) return;

        //this.InGameClient.SendMessage(new StashIconStateMessage() { StashIcons = dbGAcc.StashIcons });
    }

    public void NotifyMaintenance()
    {
        if (GameServer.ClientSystem.GameServer.MaintenanceTime > 0 &&
            GameServer.ClientSystem.GameServer.MaintenanceTime > (int)DateTime.Now.ToUnixTime())
            InGameClient.SendMessage(new LogoutTickTimeMessage()
            {
                Field0 = false, // true - logout with party?
                Ticks = 0, // delay 1, make this equal to 0 for instant logout
                Field2 = 10000, // delay 2
                Field3 = (GameServer.ClientSystem.GameServer.MaintenanceTime - (int)DateTime.Now.ToUnixTime()) *
                         60 //maintenance counter
            });
    }

    public void LoadShownTutorials()
    {
        var tutorials = new List<byte>();
        tutorials.Add(64);
        for (var i = 0; i < 15; i++)
            tutorials.Add(0);
        var seenTutorials = Toon.GameAccount.DBGameAccount.SeenTutorials;

        var state = D3.GameMessage.TutorialState.CreateBuilder()
            .SetSeenTutorials(ByteString.CopyFrom(seenTutorials))
            .Build();
        InGameClient.SendMessage(new GenericBlobMessage(Opcodes.TutorialStateMessage) { Data = state.ToByteArray() });
    }

    private List<ulong> _unlockedAchievements = new();
    private List<ulong> _unlockedCriterias = new();

    private Dictionary<ulong, uint> AchievementCounters = new();

    public int DodgesInARow = 0;
    public int BlocksInARow = 0;

    public void GrantAchievement(ulong id)
    {
        if (_unlockedAchievements.Contains(id)) return;
        if (InGameClient.BnetClient.Account.GameAccount.Achievements.Any(a => a.AchievementId == id && a.Completion != -1)) return;
        if (_unlockedAchievements.Contains(id)) return;
        _unlockedAchievements.Add(id);
        try
        {
            var Achievement = AchievementSystem.AchievementManager.GetAchievementById(id);
            long Platinum = -1;
            foreach (var attr in Achievement.AttributesList)
                if (attr.Key == "Reward Currency Quantity")
                    Platinum = long.Parse(attr.Value);
            InGameClient.SendMessage(new MessageSystem.Message.Definitions.Platinum.PlatinumAchievementAwardedMessage
            {
                CurrentPlatinum = InGameClient.BnetClient.Account.GameAccount.Platinum,
                idAchievement = id,
                PlatinumIncrement = Platinum
            });
            if (Platinum > 0)
            {
                InGameClient.BnetClient.Account.GameAccount.Platinum += (int)Platinum;
                Inventory.UpdateCurrencies();
            }

            ClientSystem.GameServer.GSBackend.GrantAchievement(Toon.GameAccount.PersistentID, id);
        }
        catch (Exception e)
        {
            Logger.WarnException(e, "Exception on GrantAchievement(): ");
        }
    }

    public void AddAchievementCounter(ulong id, uint count)
    {
        lock (AchievementCounters)
        {
            if (!AchievementCounters.ContainsKey(id))
                AchievementCounters.Add(id, count);
            else
                AchievementCounters[id] += count;
        }
    }

    public void CheckAchievementCounters()
    {
        lock (AchievementCounters)
        {
            foreach (var counter in AchievementCounters)
            {
                if (counter.Value == 0) continue;
                UpdateSingleAchievementCounter(counter.Key, counter.Value);
            }

            AchievementCounters.Clear();
        }
    }

    public void GrantCriteria(ulong id)
    {
        if (_unlockedCriterias.Contains(id)) return;
        _unlockedCriterias.Add(id);
        try
        {
            GameServer.ClientSystem.GameServer.GSBackend.GrantCriteria(Toon.GameAccount.PersistentID, id);
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
            GameServer.ClientSystem.GameServer.GSBackend.UpdateQuantity(Toon.GameAccount.PersistentID, id, counter);
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
            GameServer.ClientSystem.GameServer.GSBackend.UpdateAchievementCounter(Toon.GameAccount.PersistentID, type,
                addCounter, comparand, achiId);
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
            GameServer.ClientSystem.GameServer.GSBackend.UpdateSingleAchievementCounter(Toon.GameAccount.PersistentID,
                achievementId, addCounter);
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
            GameServer.ClientSystem.GameServer.GSBackend.CheckQuestCriteria(Toon.GameAccount.PersistentID, questId,
                World.Game.Players.Count > 1);
        }
        catch (Exception e)
        {
            Logger.WarnException(e, "Exception on CheckQuestCriteria(): ");
        }
    }

    public void CheckKillMonsterCriteria(ActorSno actorSno, int type)
    {
        try
        {
            ClientSystem.GameServer.GSBackend.CheckKillMonsterCriteria(Toon.GameAccount.PersistentID, (int)actorSno,
                type, World.Game.IsHardcore);
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
            GameServer.ClientSystem.GameServer.GSBackend.CheckLevelCap(Toon.GameAccount.PersistentID);
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
            GameServer.ClientSystem.GameServer.GSBackend.CheckSalvageItemCriteria(Toon.GameAccount.PersistentID,
                itemId);
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
            GameServer.ClientSystem.GameServer.GSBackend.CheckConversationCriteria(Toon.GameAccount.PersistentID,
                convId);
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
            GameServer.ClientSystem.GameServer.GSBackend.CheckLevelAreaCriteria(Toon.GameAccount.PersistentID, laId);
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
            GameServer.ClientSystem.GameServer.GSBackend.ParagonLevelUp(Toon.GameAccount.PersistentID);
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
            GameServer.ClientSystem.GameServer.GSBackend.UniqueItemIdentified(Toon.GameAccount.PersistentID, itemId);
        }
        catch (Exception e)
        {
            Logger.WarnException(e, "Exception on UniqueItemIdentified(): ");
        }
    }

    public void SetProgress(int act, int difficulty)
    {
        if (act > 400) return;
        var dbGAcc = World.Game.GameDbSession.SessionGet<DBGameAccount>(Toon.GameAccount.PersistentID);
        var progress = dbGAcc.BossProgress;
        if (progress[act / 100] == 0xff || progress[act / 100] < (byte)difficulty)
        {
            progress[act / 100] = (byte)difficulty;

            dbGAcc.BossProgress = progress;
            World.Game.GameDbSession.SessionUpdate(dbGAcc);
        }
    }

    public int CastingSnoPower = -1;

    public void StartCasting(int durationTicks, Action result, int skillsno = -1)
    {
        IsCasting = true;
        CastResult = result;
        Attributes[GameAttribute.Looping_Animation_Start_Time] = World.Game.TickCounter;
        Attributes[GameAttribute.Looping_Animation_End_Time] = World.Game.TickCounter + durationTicks;
        CastingSnoPower = skillsno;
        if (CastingSnoPower != -1)
        {
            Attributes[GameAttribute.Buff_Icon_Start_Tick0, CastingSnoPower] = World.Game.TickCounter;
            Attributes[GameAttribute.Buff_Icon_End_Tick0, CastingSnoPower] = World.Game.TickCounter + durationTicks;
            Attributes[GameAttribute.Buff_Icon_Count0, CastingSnoPower] = 1;
            Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, CastingSnoPower] = true;
        }

        Attributes.BroadcastChangedIfRevealed();
    }

    public void StopCasting()
    {
        IsCasting = false;
        Attributes[GameAttribute.Looping_Animation_Start_Time] = -1;
        Attributes[GameAttribute.Looping_Animation_End_Time] = -1;
        if (CastingSnoPower != -1)
        {
            Attributes[GameAttribute.Buff_Icon_Start_Tick0, CastingSnoPower] = -1;
            Attributes[GameAttribute.Buff_Icon_End_Tick0, CastingSnoPower] = -1;
            Attributes[GameAttribute.Buff_Icon_Count0, CastingSnoPower] = 0;
            Attributes[GameAttribute.Power_Buff_0_Visual_Effect_None, CastingSnoPower] = false;
        }

        Attributes.BroadcastChangedIfRevealed();
    }

    private void UpdateCastState()
    {
        if (Attributes[GameAttribute.Looping_Animation_End_Time] <= World.Game.TickCounter)
        {
            StopCasting();
            CastResult.Invoke();
            CastResult = null;
        }
    }

    public void ShowConfirmation(uint actorId, Action result)
    {
        ConfirmationResult = result;

        InGameClient.SendMessage(new ConfirmMessage()
        {
            ActorID = actorId
        });
    }

    #endregion

    #region generic properties

    public int ClassSNO => Toon.Gender == 0 ? Toon.HeroTable.SNOMaleActor : Toon.HeroTable.SNOFemaleActor;

    public int AdditionalLootItems
    {
        get
        {
            if (World.BuffManager.HasBuff<NephalemValorBuff>(this))
                return Math.Max(World.BuffManager.GetFirstBuff<NephalemValorBuff>(this).StackCount - 3, 0);
            else return 0;
        }
    }

    public float ModelScale =>
        Toon.Class switch
        {
            ToonClass.Barbarian => 1.2f,
            ToonClass.Crusader => 1.2f,
            ToonClass.DemonHunter => 1.35f,
            ToonClass.Monk => 1.43f,
            ToonClass.WitchDoctor => 1.1f,
            ToonClass.Wizard => 1.3f,
            _ => 1.43f
        };

    public int PrimaryResourceID => (int)Toon.HeroTable.PrimaryResource;

    public int SecondaryResourceID => (int)Toon.HeroTable.SecondaryResource;

    [Obsolete]
    public bool IsInTown
    {
        get
        {
            var townAreas = new List<int> { 19947, 168314, 92945, 197101 };
            var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
            var scenes = World.QuadTree.Query<Scene>(proximity);
            if (scenes.Count == 0) return false;

            var scene = scenes[0];

            if (scenes.Count == 2) // What if it's a subscene?
                if (scenes[1].ParentChunkID != 0xFFFFFFFF)
                    scene = scenes[1];

            return townAreas.Contains(scene.Specification.SNOLevelAreas[0]);
        }
    }

    #endregion

    #region experience handling

    //Max((Resource_Max + ((Level#NONE - 1) * Resource_Factor_Level) + Resource_Max_Bonus) * (Resource_Max_Percent_Bonus + 1), 0)
    private float GetMaxResource(int resourceId)
    {
        if (resourceId == 2) return 0;
        return Math.Max(
            (Attributes[GameAttribute.Resource_Max, resourceId] +
             (Attributes[GameAttribute.Level] - 1) * Attributes[GameAttribute.Resource_Factor_Level, resourceId] +
             Attributes[GameAttribute.Resource_Max_Bonus, resourceId]) *
            (Attributes[GameAttribute.Resource_Max_Percent_Bonus, resourceId] + 1), 0);
    }

    public static List<long> LevelBorders = new()
    {
        0, 280, 2700, 4500, 6600, 9000, 11700, 14000, 16500, 19200, 22100, /* Level 0-10 */
        25200, 28500, 32000, 35700, 39600, 43700, 48000, 52500, 57200, 62100, /* Level 11-20 */
        67200, 72500, 78000, 83700, 89600, 95700, 102000, 108500, 115200, 122100, /* Level 21-30 */
        150000, 157500, 180000, 203500, 228000, 273000, 320000, 369000, 420000, 473000, /* Level 31-40 */
        528000, 585000, 644000, 705000, 768000, 833000, 900000, 1453500, 2080000, 3180000, /* Level 41-50 */
        4050000, 5005000, 6048000, 7980000, 10092000, 12390000, 14880000, 17019000, 20150000,
        24586000, /* Level 51-60 */
        27000000, 29400000, 31900000, 39100000, 46800000, 55000000, 63700000, 72900000, 82600000,
        100000000 /* Level 61-70 */
    };

    public static List<long> ParagonLevelBorders = new()
    {
        //7200000,8640000,10800000,11520000,12960000,14400000,15840000,17280000,18720000,20160000,21600000,23040000,24480000,25920000,27360000,
        //28800000,30240000,31680000,33120000,34560000,36000000,37440000,38880000,40320,41760000,43200000,41760000,43200000,44640000,46080000,
        //47520000,48960000,50400000,51840000,53280000,54720000,56160000,57600000,59040000,60480000,61920000,63360000,64800000,66240000,67680000,
        //69120000,70560000,72000000,73440000,76320000,77760000,79200000,80640000,82080000,83520000,84960000,86400000,87840000
    };

    public static void GeneratePLB()
    {
        long previousExp = 7200000;
        ParagonLevelBorders.Add(7200000);
        for (var i = 0; i < 59; i++)
        {
            previousExp += 1440000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 10; i++)
        {
            previousExp += 2880000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 3; i++)
        {
            previousExp += 5040000;
            ParagonLevelBorders.Add(previousExp);
        }

        previousExp += 3660000;
        ParagonLevelBorders.Add(previousExp);
        for (var i = 0; i < 75; i++)
        {
            previousExp += 1020000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 101; i++)
        {
            previousExp += 2040000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 100; i++)
        {
            previousExp += 4080000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 99; i++)
        {
            previousExp += 6120000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 51; i++)
        {
            previousExp += 8160000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 50; i++)
        {
            previousExp += 20400000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 50; i++)
        {
            previousExp += 40800000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 50; i++)
        {
            previousExp += 61200000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 50; i++)
        {
            previousExp += 81600000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 50; i++)
        {
            previousExp += 102000000;
            ParagonLevelBorders.Add(previousExp);
        }

        for (var i = 0; i < 1500; i++)
        {
            previousExp += 122400000;
            ParagonLevelBorders.Add(previousExp);
        }

        long boosterofup = 229500000;
        for (var i = 0; i < 17750; i++)
        {
            boosterofup += 102000;
            previousExp += boosterofup;
            ParagonLevelBorders.Add(previousExp);
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
        if (Attributes[GameAttribute.Level] == Attributes[GameAttribute.Level_Cap])
            exp_needed = ParagonLevelBorders[Attributes[GameAttribute.Alt_Level]];
        else
            exp_needed = LevelBorders[Attributes[GameAttribute.Level]];

        Attributes[GameAttribute.Rest_Experience_Lo] =
            Math.Min(Attributes[GameAttribute.Rest_Experience_Lo] + (int)(exp_needed / 10), (int)exp_needed);
        Attributes[GameAttribute.Rest_Experience_Bonus_Percent] = 0.25f;
        Attributes.BroadcastChangedIfRevealed();
    }

    private object _XPlock = new();

    public void UpdateExp(int addedExp)
    {
        lock (_XPlock)
        {
            if (Dead) return;
            if (World.Game.IsHardcore && Attributes[GameAttribute.Level] >= 70)
                addedExp *= 5;

            if (Attributes[GameAttribute.Alt_Level] >= 515)
            {
                var XPcap = 91.262575239831f * Math.Pow(Attributes[GameAttribute.Alt_Level], 3) -
                            44301.083380565047f * Math.Pow(Attributes[GameAttribute.Alt_Level], 2) +
                            3829010.395566940308f * Attributes[GameAttribute.Alt_Level] + 322795582.543823242188f;
                addedExp = (int)((float)(ParagonLevelBorders[Attributes[GameAttribute.Alt_Level]] / XPcap) * addedExp);
            }

            if (Attributes[GameAttribute.Rest_Experience_Lo] > 0)
            {
                var multipliedExp = (int)Math.Min(addedExp * Attributes[GameAttribute.Rest_Experience_Bonus_Percent],
                    Attributes[GameAttribute.Rest_Experience_Lo]);
                addedExp += multipliedExp;
                Attributes[GameAttribute.Rest_Experience_Lo] -= multipliedExp;
            }

            if (Attributes[GameAttribute.Level] == Attributes[GameAttribute.Level_Cap])
                Attributes[GameAttribute.Alt_Experience_Next_Lo] -= addedExp;
            else
                Attributes[GameAttribute.Experience_Next_Lo] -= addedExp;

            // Levelup
            while (Attributes[GameAttribute.Level] >= Attributes[GameAttribute.Level_Cap]
                       ? Attributes[GameAttribute.Alt_Experience_Next_Lo] <= 0
                       : Attributes[GameAttribute.Experience_Next_Lo] <= 0)
            {
                // No more levelup at Level_Cap
                if (Attributes[GameAttribute.Level] >= Attributes[GameAttribute.Level_Cap])
                {
                    ParagonLevel++;
                    Toon.ParagonLevelUp();
                    ParagonLevelUp();
                    Attributes[GameAttribute.Alt_Level]++;
                    InGameClient.SendMessage(new ParagonLevel()
                    {
                        PlayerIndex = PlayerIndex,
                        Level = ParagonLevel
                    });
                    Conversations.StartConversation(0x0002A777); //LevelUp Conversation

                    Attributes[GameAttribute.Alt_Experience_Next_Lo] =
                        Attributes[GameAttribute.Alt_Experience_Next_Lo] +
                        (int)ParagonLevelBorders[Attributes[GameAttribute.Alt_Level]];
                    // On level up, health is set to max
                    Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
                    // set resources to max as well
                    Attributes[GameAttribute.Resource_Cur, Attributes[GameAttribute.Resource_Type_Primary] - 1] =
                        Attributes[GameAttribute.Resource_Max_Total,
                            Attributes[GameAttribute.Resource_Type_Primary] - 1];
                    Attributes[GameAttribute.Resource_Cur, Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                        Attributes[GameAttribute.Resource_Max_Total,
                            Attributes[GameAttribute.Resource_Type_Secondary] - 1];

                    ExperienceNext = Attributes[GameAttribute.Alt_Experience_Next_Lo];
                    Attributes.BroadcastChangedIfRevealed();

                    PlayEffect(Effect.ParagonLevelUp, null, false);
                    World.PowerManager.RunPower(this, 252038); //g_LevelUp_AA.pow 252038
                    return;
                }

                Level++;
                Attributes[GameAttribute.Level]++;
                Toon.LevelUp();
                if (World.Game.MonsterLevel + 1 ==
                    Attributes[GameAttribute.Level]) //if this is suitable level to update
                    World.Game.UpdateLevel(Attributes[GameAttribute.Level]);

                InGameClient.SendMessage(new PlayerLevel()
                {
                    PlayerIndex = PlayerIndex,
                    Level = Level
                });


                //Test Update Monster Level
                if (PlayerIndex == 0)
                {
                    InGameClient.Game.InitialMonsterLevel = Level;
                    InGameClient.SendMessage(new GameSyncedDataMessage
                    {
                        SyncedData = new GameSyncedData
                        {
                            GameSyncedFlags = InGameClient.Game.IsSeasoned ? InGameClient.Game.IsHardcore ? 3 : 2 :
                                InGameClient.Game.IsHardcore ? 1 : 0,
                            Act = Math.Min(InGameClient.Game.CurrentAct, 3000), //act id
                            InitialMonsterLevel = InGameClient.Game.InitialMonsterLevel, //InitialMonsterLevel
                            MonsterLevel = 0x64E4425E, //MonsterLevel
                            RandomWeatherSeed = InGameClient.Game.WeatherSeed, //RandomWeatherSeed
                            OpenWorldMode = InGameClient.Game.CurrentAct == 3000 ? 1 : 0, //OpenWorldMode
                            OpenWorldModeAct = -1, //OpenWorldModeAct
                            OpenWorldModeParam = -1, //OpenWorldModeParam
                            OpenWorldTransitionTime = 0x00000064, //OpenWorldTransitionTime
                            OpenWorldDefaultAct = 100, //OpenWorldDefaultAct
                            OpenWorldBonusAct = -1, //OpenWorldBonusAct
                            SNODungeonFinderLevelArea = 0x00000001, //SNODungeonFinderLevelArea
                            LootRunOpen =
                                InGameClient.Game.GameMode == Game.Mode.Portals
                                    ? 0
                                    : -1, //LootRunOpen //0 - Великий Портал
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
                            PartyGuideHeroId =
                                0x0, //PartyGuideHeroId //new EntityId() { High = 0, Low = (long)this.Players.Values.First().Toon.PersistentID }
                            TiredRiftPaticipatingHeroID =
                                new long[] { 0x0, 0x0, 0x0, 0x0 } //TiredRiftPaticipatingHeroID
                        }
                    });
                }

                Conversations.StartConversation(0x0002A777); //LevelUp Conversation

                if (Attributes[GameAttribute.Level] >= Attributes[GameAttribute.Level_Cap])
                {
                    Attributes[GameAttribute.Alt_Experience_Next_Lo] = (int)ParagonLevelBorders[Toon.ParagonLevel];
                    Toon.ExperienceNext = (int)ParagonLevelBorders[Toon.ParagonLevel];
                }
                else
                {
                    Attributes[GameAttribute.Experience_Next_Lo] += (int)LevelBorders[Attributes[GameAttribute.Level]];
                    Toon.ExperienceNext = Attributes[GameAttribute.Experience_Next_Lo];
                }

                // 4 main attributes are incremented according to class
                Attributes[GameAttribute.Strength] = Strength;
                Attributes[GameAttribute.Intelligence] = Intelligence;
                Attributes[GameAttribute.Vitality] = Vitality;
                Attributes[GameAttribute.Dexterity] = Dexterity;

                // On level up, health is set to max
                Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];

                // force GameAttributeMap to re-calc resources for the active resource types
                Attributes[GameAttribute.Resource_Max, Attributes[GameAttribute.Resource_Type_Primary] - 1] =
                    Attributes[GameAttribute.Resource_Max, Attributes[GameAttribute.Resource_Type_Primary] - 1];
                Attributes[GameAttribute.Resource_Max, Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                    Attributes[GameAttribute.Resource_Max, Attributes[GameAttribute.Resource_Type_Secondary] - 1];

                // set resources to max as well
                Attributes[GameAttribute.Resource_Cur, Attributes[GameAttribute.Resource_Type_Primary] - 1] =
                    Attributes[GameAttribute.Resource_Max_Total, Attributes[GameAttribute.Resource_Type_Primary] - 1];
                Attributes[GameAttribute.Resource_Cur, Attributes[GameAttribute.Resource_Type_Secondary] - 1] =
                    Attributes[GameAttribute.Resource_Max_Total, Attributes[GameAttribute.Resource_Type_Secondary] - 1];

                Attributes[GameAttribute.Hitpoints_Factor_Vitality] = 10f + Math.Max(Level - 35, 0);

                Attributes.BroadcastChangedIfRevealed();

                PlayEffect(Effect.LevelUp, null, false);
                World.PowerManager.RunPower(this, 85954); //g_LevelUp.pow 85954


                switch (Level)
                {
                    case 10:
                        GrantAchievement(World.Game.IsHardcore ? (ulong)74987243307034 : (ulong)74987243307105);
                        break;
                    case 20:
                        GrantAchievement(World.Game.IsHardcore ? (ulong)74987243307035 : (ulong)74987243307104);
                        break;
                    case 30:
                        GrantAchievement(World.Game.IsHardcore ? (ulong)74987243307036 : (ulong)74987243307103);
                        break;
                    case 40:
                        GrantAchievement(World.Game.IsHardcore ? (ulong)74987243307037 : (ulong)74987243307102);
                        break;
                    case 50:
                        GrantAchievement(World.Game.IsHardcore ? (ulong)74987243307038 : (ulong)74987243307101);
                        if (World.Game.IsSeasoned)
                            GrantCriteria(74987250038929);
                        break;
                    case 60:
                        if (World.Game.IsHardcore)
                        {
                            GrantAchievement(74987243307039);
                            if (!Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.HardcoreTormentUnlocked))
                                Toon.GameAccount.Flags |= GameAccount.GameAccountFlags.HardcoreTormentUnlocked;
                        }
                        else
                        {
                            GrantAchievement(74987243307100);
                            if (!Toon.GameAccount.Flags.HasFlag(GameAccount.GameAccountFlags.TormentUnlocked))
                                Toon.GameAccount.Flags |= GameAccount.GameAccountFlags.TormentUnlocked;
                        }

                        CheckLevelCap();
                        break;
                    case 70:
                        GrantCriteria(74987254853541);
                        break;
                    default:
                        break;
                }
            }

            ExperienceNext = Attributes[GameAttribute.Level] == 70
                ? Attributes[GameAttribute.Alt_Experience_Next_Lo]
                : Attributes[GameAttribute.Experience_Next_Lo];
            Attributes.BroadcastChangedIfRevealed();
            Toon.GameAccount.NotifyUpdate();

            //this.Attributes.SendMessage(this.InGameClient, this.DynamicID); kills the player atm
        }
    }

    #endregion

    #region gold, heath-glob collection

    public void VacuumPickupHealthOrb(float radius = -1)
    {
        if (Math.Abs(radius - -1) < 0.001)
            radius = Attributes[GameAttribute.Gold_PickUp_Radius];
        var itemList = GetItemsInRange(radius);
        foreach (var item in itemList)
            if (Item.IsHealthGlobe(item.ItemType))
            {
                var playersAffected = GetPlayersInRange(26f);
                foreach (var player in playersAffected)
                {
                    foreach (var targetAffected in playersAffected)
                        player.InGameClient.SendMessage(new PlayEffectMessage()
                        {
                            ActorId = targetAffected.DynamicID(player),
                            Effect = Effect.HealthOrbPickup
                        });

                    //every summon and mercenary owned by you must broadcast their green text to you /H_DANILO
                    player.AddPercentageHP((int)item.Attributes[GameAttribute.Health_Globe_Bonus_Health]);
                    //passive abilities
                    if (player.SkillSet.HasPassive(208478)) //wizard PowerHungry
                        player.World.BuffManager.AddBuff(this, this, new HungryBuff());
                    if (player.SkillSet.HasPassive(208594)) //wd GruesomeFeast
                    {
                        player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total,
                            (int)player.Toon.HeroTable.PrimaryResource + 1] * 0.1f);
                        player.World.BuffManager.AddBuff(player, player, new GruesomeFeastIntBuff());
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

    public void VacuumPickup()
    {
        var itemList = GetItemsInRange(Attributes[GameAttribute.Gold_PickUp_Radius]);
        foreach (var item in itemList)
            if (Item.IsGold(item.ItemType))
            {
                if (!GroundItems.ContainsKey(item.GlobalID)) continue;

                InGameClient.SendMessage(new FloatingAmountMessage()
                {
                    Place = new WorldPlace()
                    {
                        Position = Position,
                        WorldID = World.DynamicID(this)
                    },

                    Amount = item.Attributes[GameAttribute.Gold],
                    Type = FloatingAmountMessage.FloatType.Gold
                });
                InGameClient.SendMessage(new PlayEffectMessage()
                {
                    ActorId = DynamicID(this),
                    Effect = Effect.GoldPickup,
                    PlayerId = 0
                });
                PlayEffect(Effect.Sound, 36726);
                Inventory.PickUpGold(item);
                GroundItems.Remove(item.GlobalID);
                item.Destroy();
            }

            else if (Item.IsBloodShard(item.ItemType) || item.ItemDefinition.Name == "HoradricRelic")
            {
                if (!GroundItems.ContainsKey(item.GlobalID)) continue;

                InGameClient.SendMessage(new FloatingAmountMessage()
                {
                    Place = new WorldPlace()
                    {
                        Position = Position,
                        WorldID = World.DynamicID(this)
                    },

                    Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
                    Type = FloatingAmountMessage.FloatType.BloodStone
                });

                Inventory.PickUpBloodShard(item);
                GroundItems.Remove(item.GlobalID);
                item.Destroy();
            }

            else if (item.ItemDefinition.Name == "Platinum")
            {
                InGameClient.SendMessage(new FloatingAmountMessage()
                {
                    Place = new WorldPlace()
                    {
                        Position = Position,
                        WorldID = World.DynamicID(this)
                    },

                    Amount = item.Attributes[GameAttribute.ItemStackQuantityLo],
                    Type = FloatingAmountMessage.FloatType.Platinum
                });
                PlayEffect(Effect.Sound, 433266);

                Inventory.PickUpPlatinum(item);
                GroundItems.Remove(item.GlobalID);
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
                Toon.GameAccount.CraftItem4++;
                Inventory.UpdateCurrencies();
                item.Destroy();
            }

            else if (Item.IsHealthGlobe(item.ItemType))
            {
                var playersAffected = GetPlayersInRange(26f);
                foreach (var player in playersAffected)
                {
                    foreach (var targetAffected in playersAffected)
                        player.InGameClient.SendMessage(new PlayEffectMessage()
                        {
                            ActorId = targetAffected.DynamicID(player),
                            Effect = Effect.HealthOrbPickup
                        });

                    //every summon and mercenary owned by you must broadcast their green text to you /H_DANILO
                    player.AddPercentageHP((int)item.Attributes[GameAttribute.Health_Globe_Bonus_Health]);
                    //passive abilities
                    if (player.SkillSet.HasPassive(208478)) //wizard PowerHungry
                        player.World.BuffManager.AddBuff(this, this, new HungryBuff());
                    if (player.SkillSet.HasPassive(208594)) //wd GruesomeFeast
                    {
                        player.GeneratePrimaryResource(player.Attributes[GameAttribute.Resource_Max_Total,
                            (int)player.Toon.HeroTable.PrimaryResource + 1] * 0.1f);
                        player.World.BuffManager.AddBuff(player, player, new GruesomeFeastIntBuff());
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

            else if (item.ItemDefinition.Name == "ArcaneGlobe" && Toon.Class == ToonClass.Wizard)
            {
                GeneratePrimaryResource(50f);
                item.Destroy();
            }

            else if (item.ItemDefinition.Name == "p1_normal_rifts_Orb" ||
                     item.ItemDefinition.Name == "p1_tiered_rifts_Orb")
            {
                if (InGameClient.Game.ActiveNephalemTimer && InGameClient.Game.ActiveNephalemKilledMobs == false)
                {
                    InGameClient.Game.ActiveNephalemProgress += 15f;
                    foreach (var plr in InGameClient.Game.Players.Values)
                    {
                        plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DunggeonFinderProgressGlyphPickUp)
                        {
                            Field0 = InGameClient.Game.ActiveNephalemProgress
                        });

                        plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.KillCounterRefresh)
                        {
                        });
                        plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
                        {
                            Field0 = InGameClient.Game.ActiveNephalemProgress
                        });
                    }

                    if (InGameClient.Game.ActiveNephalemProgress > 650)
                    {
                        InGameClient.Game.ActiveNephalemKilledMobs = true;
                        foreach (var plr in InGameClient.Game.Players.Values)
                        {
                            if (InGameClient.Game.NephalemGreater)
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

                            plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameChatTextMessage)
                                { Message = "Messages:LR_BossSpawned" });
                            plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameTextMessage)
                                { Message = "Messages:LR_BossSpawned" });
                        }

                        StartConversation(World, 366542);
                        SpawnNephalemBoss(World);
                        //358489
                    }
                }

                item.Destroy();
            }

            else if (item.ItemDefinition.Name == "PowerGlobe_v2_x1_NoFlippy")
            {
                World.BuffManager.AddBuff(this, this, new NephalemValorBuff());
                item.Destroy();
            }

            else if (Item.IsPotion(item.ItemType))
            {
                if ((!GroundItems.ContainsKey(item.GlobalID) && World.Game.Players.Count > 1) ||
                    !Inventory.HasInventorySpace(item)) continue;
                Inventory.PickUp(item);
            }

        //
        foreach (var skill in SkillSet.ActiveSkills)
            if (skill.snoSkill == 460757 && skill.snoRune == 3)
            {
                //Play Aura - 472217
                //this.PlayEffectGroup(472217);
                var Fleshes =
                    GetActorsInRange<NecromancerFlesh>(15f + Attributes[GameAttribute.Gold_PickUp_Radius] *
                        0.5f); //454066
                foreach (var flesh in Fleshes)
                {
                    InGameClient.SendMessage(new EffectGroupACDToACDMessage()
                    {
                        EffectSNOId = 470480,
                        TargetID = DynamicID(this),
                        ActorID = flesh.DynamicID(this)
                    });
                    flesh.PlayEffectGroup(470482);
                    Attributes[GameAttribute.Resource_Cur, (int)Toon.HeroTable.PrimaryResource] += 11f;
                    Attributes.BroadcastChangedIfRevealed();
                    flesh.Destroy();
                }
            }
    }

    public Actor SpawnNephalemBoss(World world)
    {
        var boss = world.SpawnMonster(ActorSnoExtensions.nephalemPortalBosses.PickRandom(), Position);
        boss.Attributes[GameAttribute.Bounty_Objective] = true;
        boss.Attributes[GameAttribute.Is_Loot_Run_Boss] = true;
        boss.Attributes.BroadcastChangedIfRevealed();

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


        return boss;
    }

    public bool StartConversation(World world, int conversationId)
    {
        foreach (var plr in world.Players)
            plr.Value.Conversations.StartConversation(conversationId);
        return true;
    }

    public void AddPercentageHP(int percentage, bool GuidingLight = false)
    {
        var quantity = percentage * Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
        AddHP(quantity, GuidingLight);
    }

    public void AddPercentageHP(float percentage, bool GuidingLight = false)
    {
        var quantity = percentage * Attributes[GameAttribute.Hitpoints_Max_Total] / 100;
        AddHP(quantity, GuidingLight);
    }

    public override void AddHP(float quantity, bool guidingLight = false)
    {
        if (Dead) return;
        if (quantity == 0) return;
        if (quantity > 0)
        {
            if (Attributes[GameAttribute.Hitpoints_Cur] < Attributes[GameAttribute.Hitpoints_Max_Total])
            {
                if (Toon.Class == ToonClass.Barbarian)
                    if (SkillSet.HasPassive(205217))
                        quantity += 0.01f * Attributes[GameAttribute.Health_Globe_Bonus_Health];

                if (guidingLight) //Monk -> Guiding Light
                {
                    var missingHP =
                        (Attributes[GameAttribute.Hitpoints_Max_Total] - Attributes[GameAttribute.Hitpoints_Cur]) /
                        Attributes[GameAttribute.Hitpoints_Max_Total];
                    if (missingHP > 0.05f)
                        if (!World.BuffManager.HasBuff<GuidingLightBuff>(this))
                            World.BuffManager.AddBuff(this, this,
                                new GuidingLightBuff(Math.Min(missingHP, 0.3f),
                                    TickTimer.WaitSeconds(World.Game, 10.0f)));
                }

                Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
                    Attributes[GameAttribute.Hitpoints_Cur] + quantity,
                    Attributes[GameAttribute.Hitpoints_Max_Total]);

                Attributes.BroadcastChangedIfRevealed();
                InGameClient.SendMessage(new FloatingNumberMessage
                {
                    ActorID = DynamicID(this),
                    Number = quantity,
                    Type = FloatingNumberMessage.FloatType.Green
                });
            }
        }
        else
        {
            Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(
                Attributes[GameAttribute.Hitpoints_Cur] + quantity,
                0);

            Attributes.BroadcastChangedIfRevealed();
        }
    }

    //only for WD passive
    public void RestoreMana(float quantity, int secs)
    {
        Attributes[GameAttribute.Resource_Regen_Per_Second, 0] += quantity / secs;
        System.Threading.Tasks.Task.Delay(1000 * secs).ContinueWith(t =>
        {
            Attributes[GameAttribute.Resource_Regen_Per_Second, 0] -= quantity / secs;
        });
    }

    #endregion

    #region Resource Generate/Use

    private int _DisciplineSpent = 0;
    private int _HatredSpent = 0;
    private int _WrathSpent = 0;

    public void GeneratePrimaryResource(float amount)
    {
        if (Toon.Class == ToonClass.Barbarian)
            if (World.BuffManager.HasBuff<WrathOfTheBerserker.BerserkerBuff>(this))
                World.BuffManager.GetFirstBuff<WrathOfTheBerserker.BerserkerBuff>(this).GainedFury += amount;

        if (Toon.Class == ToonClass.Monk)
            if (World.BuffManager.HasBuff<GuardiansPathBuff>(this)) //Monk -> The Guardian's Path 2H
                amount *= 1.35f;

        _ModifyResourceAttribute(PrimaryResourceID, amount);
    }

    public void UsePrimaryResource(float amount, bool tick = false)
    {
        amount = Math.Max(
            (amount - Attributes[GameAttribute.Resource_Cost_Reduction_Amount]) * (1f -
                Attributes[GameAttribute.Resource_Cost_Reduction_Percent_Total,
                    (int)Toon.HeroTable.PrimaryResource + 1]), 0);
        amount = amount * (1f - DecreaseUseResourcePercent);
        if (Toon.Class == ToonClass.Crusader)
        {
            _WrathSpent += (int)amount;
            if (!tick && SkillSet.HasPassive(310775)) //Wrathful passive
                AddHP(_WrathSpent * 15f * Attributes[GameAttribute.Level]);

            //Laws of Hope -> Faith's reward
            if (!tick && World.BuffManager.HasBuff<CrusaderLawsOfHope.LawsShieldBuff>(this))
                if (World.BuffManager.GetFirstBuff<CrusaderLawsOfHope.LawsShieldBuff>(this).HealPerWrath)
                    AddHP(_WrathSpent * 15f * Attributes[GameAttribute.Level]);

            if (_WrathSpent >= 20) //Akarat Champion -> Fire Starter
                if (!tick && World.BuffManager.HasBuff<CrusaderAkaratChampion.AkaratBuff>(this))
                    World.BuffManager.GetFirstBuff<CrusaderAkaratChampion.AkaratBuff>(this).wrathBlast = true;
        }

        if (Toon.Class == ToonClass.DemonHunter)
        {
            _HatredSpent += (int)amount;

            if (_HatredSpent >= 150 && _DisciplineSpent >= 50)
                GrantAchievement(74987243307068);

            AddTimedAction(6f, new Action<int>((q) => _HatredSpent -= (int)amount));
        }

        if (Toon.Class == ToonClass.Barbarian)
        {
            if (SkillSet.HasPassive(105217) && !tick) //Bloodthirst (Burb)
                AddHP(amount * 1.93f * Attributes[GameAttribute.Level]);

            if (!tick && World.BuffManager.HasBuff<IgnorePain.IgnorePainBuff>(this))
                if (Attributes[GameAttribute.Rune_E, 79528] > 0) //IgnorePain
                    AddHP(amount * 13.76f * Attributes[GameAttribute.Level]);
        }

        if (Toon.Class == ToonClass.Wizard)
            if (World.BuffManager.HasBuff<HungryBuff>(this)) //Power Hungry
            {
                amount = 0f;
                World.BuffManager.RemoveStackFromBuff(this, World.BuffManager.GetFirstBuff<HungryBuff>(this));
            }

        if (Toon.Class == ToonClass.Monk)
            if (SkillSet.HasPassive(209250)) //Transcendence (Monk)
                AddHP(amount * (50f + Attributes[GameAttribute.Health_Globe_Bonus_Health] * 0.004f));

        if (SkillSet.HasPassive(208628)) //PierceTheVeil (WD)
            amount *= 1.3f;
        if (SkillSet.HasPassive(208568)) //BloodRitual (WD)
        {
            amount *= 0.9f;
            AddHP(amount * -0.1f);
        }

        if (SkillSet.HasPassive(205398) && Attributes[GameAttribute.Hitpoints_Cur] <
            Attributes[GameAttribute.Hitpoints_Max_Total] * 0.35f) //Relentless (Barbarian)
            amount *= 0.25f;
        _ModifyResourceAttribute(PrimaryResourceID, -amount);
    }

    public void GenerateSecondaryResource(float amount)
    {
        _ModifyResourceAttribute(SecondaryResourceID, amount);
    }

    public void UseSecondaryResource(float amount)
    {
        amount = Math.Max(
            (amount - Attributes[GameAttribute.Resource_Cost_Reduction_Amount]) * (1f -
                Attributes[GameAttribute.Resource_Cost_Reduction_Percent_Total, (int)Toon.HeroTable.SecondaryResource]),
            0);

        if (SkillSet.HasPassive(155722)) //dh - Perfectionist
            amount *= 0.9f;

        if (Toon.Class == ToonClass.DemonHunter)
        {
            _DisciplineSpent += (int)amount;

            if (_HatredSpent >= 150 && _DisciplineSpent >= 50)
                GrantAchievement(74987243307068);

            AddTimedAction(6f, new Action<int>((q) => _DisciplineSpent -= (int)amount));
        }

        _ModifyResourceAttribute(SecondaryResourceID, -amount);
    }

    private void _ModifyResourceAttribute(int resourceID, float amount)
    {
        if (resourceID == -1 || amount == 0) return;
        var current = Attributes[GameAttribute.Resource_Cur, resourceID];
        if (amount > 0f)
            Attributes[GameAttribute.Resource_Cur, resourceID] = Math.Min(
                Attributes[GameAttribute.Resource_Cur, resourceID] + amount,
                Attributes[GameAttribute.Resource_Max_Total, resourceID]);
        else
            Attributes[GameAttribute.Resource_Cur, resourceID] = Math.Max(
                Attributes[GameAttribute.Resource_Cur, resourceID] + amount,
                0f);

        if (current == Attributes[GameAttribute.Resource_Cur, resourceID]) return;

        Attributes.BroadcastChangedIfRevealed();
    }


    private int _fullFuryFirstTick = 0;
    private int _ArmorFirstTick = 0;

    private void _UpdateResources()
    {
        // will crash client when loading if you try to update resources too early
        if (World == null) return;

        if (InGameClient.Game.Paused) return;

        if (!(InGameClient.Game.TickCounter % 30 == 0)) return;

        if (InGameClient.Game.TickCounter % 60 == 0)
            LastSecondCasts = 0;

        if (Toon.Class == ToonClass.Barbarian)
        {
            if (Attributes[GameAttribute.Resource_Cur, 2] < Attributes[GameAttribute.Resource_Max_Total, 2])
                _fullFuryFirstTick = InGameClient.Game.TickCounter;

            if (InGameClient.Game.TickCounter - _fullFuryFirstTick >= 18000)
                GrantAchievement(74987243307047);
        }

        if (Toon.Class == ToonClass.Wizard)
        {
            if (!World.BuffManager.HasBuff<IceArmor.IceArmorBuff>(this) &&
                !World.BuffManager.HasBuff<StormArmor.StormArmorBuff>(this) &&
                !World.BuffManager.HasBuff<EnergyArmor.EnergyArmorBuff>(this))
                _ArmorFirstTick = InGameClient.Game.TickCounter;

            if (InGameClient.Game.TickCounter - _ArmorFirstTick >= 72000)
                GrantAchievement(74987243307588);
        }

        // 1 tick = 1/60s, so multiply ticks in seconds against resource regen per-second to get the amount to update
        var tickSeconds = 1f / 60f * (InGameClient.Game.TickCounter - _lastResourceUpdateTick);
        _lastResourceUpdateTick = InGameClient.Game.TickCounter;

        GeneratePrimaryResource(Math.Max(
            tickSeconds * Attributes[GameAttribute.Resource_Regen_Total,
                Attributes[GameAttribute.Resource_Type_Primary] - 1], 0));
        GenerateSecondaryResource(Math.Max(
            tickSeconds * Attributes[GameAttribute.Resource_Regen_Total,
                Attributes[GameAttribute.Resource_Type_Secondary] - 1], 0));

        var totalHPregen = //(this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] +
            Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] //)
            * (1 + Attributes[GameAttribute.Hitpoints_Regen_Bonus_Percent]);
        if (!Dead && !World.Game.PvP) AddHP(Math.Max(tickSeconds * totalHPregen, 0));

        if (Toon.Class == ToonClass.Barbarian)
            if (SkillSet.HasPassive(205300)) //barbarian fury
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
                CheckConversationCriteria(
                    (MPQStorage.Data.Assets[SNOGroup.Lore][loreSNOId].Data as DiIiS_NA.Core.MPQ.FileFormats.Lore)
                    .SNOConversation);
        }
    }

    /// <summary>
    /// Adds lore to player's state
    /// </summary>
    /// <param name="loreSNOId"></param>
    public void AddLore(int loreSNOId)
    {
        if (LearnedLore.Count < LearnedLore.m_snoLoreLearned.Length)
        {
            LearnedLore.m_snoLoreLearned[LearnedLore.Count] = loreSNOId;
            LearnedLore.Count++; // Count
            UpdateHeroState();
            Logger.Trace("Learning lore #{0}", loreSNOId);
            var dbToon = Toon.DBToon;
            dbToon.Lore = SerializeBytes(LearnedLore.m_snoLoreLearned.Take(LearnedLore.Count).ToList());
            World.Game.GameDbSession.SessionUpdate(dbToon);
        }
    }

    #endregion

    #region StoneOfRecall

    public void EnableStoneOfRecall()
    {
        Attributes[GameAttribute.Skill, 0x0002EC66] = 1;
        Attributes.SendChangedMessage(InGameClient);
    }

    public void DisableStoneOfRecall()
    {
        Attributes[GameAttribute.Skill, 0x0002EC66] = 0;
        Attributes.SendChangedMessage(InGameClient);
    }

    #endregion

    #region Minions and Hirelings handling

    public void AddFollower(Actor source)
    {
        //Rangged Power - 30599
        if (source == null) return;

        var minion = new Minion(World, source.SNO, this, source.Tags, true);
        minion.SetBrain(new MinionBrain(minion));
        minion.Brain.DeActivate();
        minion.WalkSpeed *= 4;
        minion.Position = Position;
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
        minion.Attributes[GameAttribute.Effect_Owner_ANN] = (int)DynamicID(this);
        minion.EnterWorld(minion.Position);
        (minion as Minion).Brain.Activate();

        source.Hidden = true;
        source.SetVisible(false);

        minion.SetVisible(true);
        minion.Hidden = false;

        if (minion.SNO == ActorSno._leah) // (4580) Act I Leah
        {
            (minion.Brain as MinionBrain).PresetPowers.Clear();
            (minion.Brain as MinionBrain).AddPresetPower(30599);
        }

        if ((Followers.Count >= 6 && ActiveHireling != null) || Followers.Count >= 7)
            if (Toon.Class == ToonClass.WitchDoctor)
                GrantAchievement(74987243307563);
    }

    public bool HaveFollower(ActorSno sno)
    {
        return Followers.ContainsValue(sno);
    }

    public void DestroyFollower(ActorSno sno)
    {
        if (Followers.ContainsValue(sno))
        {
            var dynId = Followers.Where(x => x.Value == sno).First().Key;
            var actor = World.GetActorByGlobalId(dynId);
            if (actor != null)
                actor.Destroy();
            Followers.Remove(dynId);
        }
    }

    public void SetFollowerIndex(ActorSno sno)
    {
        if (!HaveFollower(sno))
            for (var i = 1; i < 8; i++)
                if (!_followerIndexes.ContainsKey(i))
                {
                    _followerIndexes.Add(i, sno);
                    return;
                }
    }

    public void FreeFollowerIndex(ActorSno sno)
    {
        if (!HaveFollower(sno)) _followerIndexes.Remove(FindFollowerIndex(sno));
    }

    private Dictionary<int, ActorSno> _followerIndexes = new();

    public int FindFollowerIndex(ActorSno sno)
    {
        if (HaveFollower(sno))
            return _followerIndexes.FirstOrDefault(i => i.Value == sno).Key;
        return 0;
    }

    public int CountFollowers(ActorSno sno)
    {
        return Followers.Values.Count(f => f == sno);
    }

    public int CountAllFollowers()
    {
        return Followers.Count();
    }

    public void DestroyFollowerById(uint dynId)
    {
        if (Followers.ContainsKey(dynId))
        {
            var actor = World.GetActorByGlobalId(dynId);
            Followers.Remove(dynId);
            if (actor != null)
            {
                FreeFollowerIndex(actor.SNO);
                actor.Destroy();
            }
        }
    }

    public void DestroyFollowersBySnoId(ActorSno sno)
    {
        var fol_list = Followers.Where(f => f.Value == sno).Select(f => f.Key).ToList();
        foreach (var fol in fol_list)
            DestroyFollowerById(fol);
    }

    #endregion

    public void Heal()
    {
        Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
        Attributes[GameAttribute.Hitpoints_Total_From_Level] = Attributes[GameAttribute.Hitpoints_Max_Total];
        Attributes.BroadcastChangedIfRevealed();
    }

    public ImmutableArray<Door> GetNearDoors(float distance = 50f)
    {
        var doors = World.GetAllDoors();
        List<Door> doorList = doors.Where(door => door.Position.IsNear(Position, distance)).ToList();
        return doorList.ToImmutableArray();
    }

    public ImmutableArray<Door> OpenNearDoors(float distance = 50f)
    {
        List<Door> openedDoors = new();
        foreach (var door in GetNearDoors(distance))
        {
            openedDoors.Add(door);
            door.Open();
        }

        return openedDoors.ToImmutableArray();
    }
}