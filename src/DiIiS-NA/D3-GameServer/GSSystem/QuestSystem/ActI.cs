//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations.Act_I;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class ActI : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		private uint LeahId = 0;

		private uint LeahTempId = 0;

        public List<ActorSystem.Monster> Prisoners = new List<ActorSystem.Monster>() { };

        public EffectActor ProxyObject = null;

        public ActI(Game game) : base(game)
		{
		}

        public override void SetQuests()
        {
            #region Fallen Star
            Game.QuestManager.Quests.Add(87700, new Quest
            {
                RewardXp = 1125,
                RewardGold = 370,
                Completed = false,
                Saveable = true,
                NextQuest = 72095,
                Steps = new Dictionary<int, QuestStep>
                {

                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 66,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    var world = Game.GetWorld(WorldSno.trout_town);
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 87700 & Game.CurrentStep == -1)
                        {
                            var leah = world.GetActorBySNO(ActorSno._leah, true);
                            if (leah != null) leah.Hidden = true;
                        }
                    });
                    SetActorOperable(world, ActorSno._trout_newtristram_gate_town, false);
                    ListenConversation(151087, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(66, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 42,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    var world = Game.GetWorld(WorldSno.trout_town);
                    script = new SurviveTheWaves();
                    script.Execute(world);
                    var Leah = world.GetActorBySNO(ActorSno._leah, true);
                    if (Leah != null) Leah.Hidden = true;
                    ListenKill(ActorSno._zombieskinny_a, 6, new SecondWave());
                    ListenKill(ActorSno._zombiecrawler_a, 7, new Advance());


                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(42, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 75,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go and talk to Leah
                    var world = Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 151102);
                    
                    try
                    {
                        SetActorOperable(world, ActorSno._captainrumfoord, true);
                        if (world.GetActorBySNO(ActorSno._tristram_mayor, true) != null)
                            world.GetActorBySNO(ActorSno._tristram_mayor, true).Hidden = true;
                        if (world.GetActorBySNO(ActorSno._trout_newtristram_blocking_cart, true) != null)
                            world.GetActorBySNO(ActorSno._trout_newtristram_blocking_cart, true).Hidden = true;
                    }
                    catch { }
                    UnlockTeleport(0);
                    if (world.GetActorsBySNO(ActorSno._trout_newtristram_gate_town).FirstOrDefault(d => d.Visible) != null)
                        Open(world, ActorSno._trout_newtristram_gate_town);
                    ActiveArrow(world, ActorSno._g_portal_rectangle_orange_icondoor, WorldSno.trout_tristram_inn);
                    ListenConversation(151123, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(75, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 46,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //fighting zombies
                    script = new LeahInn();
                    script.Execute(Game.GetWorld(WorldSno.trout_tristram_inn));

                    ListenKill(ActorSno._zombieskinny_a_leahinn, 5, new LaunchConversation(151156));
                    ListenConversation(151156, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(46, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 50,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Leah again
                    ListenConversation(151167, new Advance());
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 87700)
                            ActiveArrow(Game.GetWorld(WorldSno.trout_town), ActorSno._captainrumfoord);
                    });
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(50, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 60,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go and talk to Rumford				
                    //ListenProximity(3739, new LaunchConversation(198503));
                    
                    ListenConversation(198503, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(60, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 27,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill wretched mother
                    var world = Game.GetWorld(WorldSno.trout_town);
                    Break(world, ActorSno._trout_wagon_barricade);

                    foreach (var sp in world.GetActorsBySNO(ActorSno._spawner_zombieskinny_a_immediate))
                    {
                        if (sp.CurrentScene.SceneSNO.Id == 33348)
                            if (sp is ActorSystem.Spawner)
                                //(sp as ActorSystem.Spawner).Spawn();
                                world.SpawnMonster(ActorSno._zombieskinny_a, sp.Position);
                    }

                    ActivateQuestMonsters(world, ActorSno._zombiefemale_a_tristramquest_unique);
                    ListenKill(ActorSno._zombiefemale_a_tristramquest_unique, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(27, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 55,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //MOAR wretched mothers
                    StartConversation(Game.GetWorld(WorldSno.trout_town), 156223);
                    ListenKill(ActorSno._zombiefemale_unique_wretchedqueen, 1, new Advance());
                    ListenKillBonus(ActorSno._zombiefemale_a_tristramquest_unique, 3, new SideTarget());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(55, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
                OnAdvance = () =>
                { //return to New Tristram and talk to Rumford
                    DeactivateQuestMonsters(Game.GetWorld(WorldSno.trout_town), ActorSno._zombiefemale_a_tristramquest_unique);
                    ListenInteract(ActorSno._waypoint_oldtristram, 1, new CompleteObjective(0));
                    ListenConversation(198521, new Advance());
                }
            });

            Game.QuestManager.Quests[87700].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
            #region Rescue Cain
            Game.QuestManager.Quests.Add(72095, new Quest { RewardXp = 3630, RewardGold = 190, Completed = false, Saveable = true, NextQuest = 72221, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72095].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { 
                    StartConversation(Game.GetWorld(WorldSno.trout_town), 198541);
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 28,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //use Tristram Portal
                    UnlockTeleport(1);
                    ListenTeleport(101351, new Advance());
                    //AddFollower(this.Game.GetWorld(71150), 4580);
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        // TODO: CHeck for possible removing outer adding
                        Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                        {
                            if (Game.CurrentQuest == 72095)
                                if (Game.CurrentStep == -1 || Game.CurrentStep == 7)
                                {
                                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                                }
                        });
                        
                    });
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 49,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to gates
                    var world = Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 166678);
                    ListenProximity(ActorSno._trout_oldtristram_exit_gate, new Advance());
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 72095)
                            if (Game.CurrentStep == 28 || Game.CurrentStep == 7 || Game.CurrentStep == -1)
                                ActiveArrow(world, ActorSno._trout_oldtristram_exit_gate);
                      
                    });
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(49, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 39,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Adria house
                    var world = Game.GetWorld(WorldSno.trout_town);
                    if (world.GetActorsBySNO(ActorSno._trout_oldtristram_exit_gate).Where(d => d.Visible).FirstOrDefault() != null)
                        Open(world, ActorSno._trout_oldtristram_exit_gate);
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 72095)
                            if (Game.CurrentStep == 49 || Game.CurrentStep == 39)
                                ActiveArrow(world, ActorSno._g_portal_square_blue_cellar, WorldSno.trout_adriascellar);
                    });
                    ListenProximity(ActorSno._g_portal_square_blue_cellar, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(39, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //inspect house
                    ListenProximity(ActorSno._g_portal_square_blue_cellar, new Advance());

                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to cave
                    //DestroyFollower(4580);
                    ListenTeleport(62968, new Advance());
                    Game.AddOnLoadWorldAction(WorldSno.trout_adriascellar, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trout_adriascellar);
                        foreach (var lh in world.GetActorsBySNO(ActorSno._leah_adriacellar))
                        {
                            lh.SetVisible(false);
                            lh.Hidden = true;
                        }

                        if (Game.CurrentQuest == 72095)
                            ActiveArrow(world, ActorSno._trout_oldtristram_adriacellar_cauldron);
                    });
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 43,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //inspect cave

                    ListenInteract(ActorSno._trout_oldtristram_adriacellar_cauldron, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 45,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Daltin (156801)
                    ActorSystem.Actor CapitanDaltyn = null;
                    Vector3D[] Zombies = new Vector3D[4];
                    Zombies[0] = new Vector3D(50.00065f, 125.4087f, 0.1000305f);
                    Zombies[1] = new Vector3D(54.88688f, 62.24541f, 0.1000305f);
                    Zombies[2] = new Vector3D(86.45869f, 77.09571f, 0.1000305f);
                    Zombies[3] = new Vector3D(102.117f, 97.59058f, 0.1000305f);



                    Game.AddOnLoadWorldAction(WorldSno.trout_adriascellar, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trout_adriascellar);
                        CapitanDaltyn = world.SpawnMonster(ActorSno._unique_captaindaltyn, new Vector3D { X = 52.587f, Y = 103.368f, Z = 0.1f });
                        CapitanDaltyn.Attributes[GameAttribute.Quest_Monster] = true;
                        CapitanDaltyn.PlayAnimation(5, AnimationSno.zombie_male_skinny_spawn);
                        foreach (Vector3D point in Zombies)
                        {
                            var Zombie = world.SpawnMonster(ActorSno._zombieskinny_a, point);
                            Zombie.Attributes[GameAttribute.Quest_Monster] = true;
                            Zombie.PlayAnimation(5, AnimationSno.zombie_male_skinny_spawn);
                        }
                    });
                    ListenKill(ActorSno._unique_captaindaltyn, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(45, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 47,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Leah in cave
                    var world = Game.GetWorld(WorldSno.trout_adriascellar);
                    foreach (var host in world.GetActorsBySNO(ActorSno._leah))
                    {
                        foreach (var lh in world.GetActorsBySNO(ActorSno._leah_adriacellar))
                        {
                            lh.SetVisible(true);
                            lh.Hidden = false;
                            lh.Teleport(host.Position);
                            lh.Position = host.Position;
                        }
                        //this.Game.GetWorld(62751).SpawnMonster(203030, lh.Position);
                    }
                    DestroyFollower(ActorSno._leah);
                    ListenConversation(198588, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(47, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 23,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to church				
                    var world = Game.GetWorld(WorldSno.trout_town);
                    ListenProximity(ActorSno._trdun_cath_cathedraldoorexterior, new Advance());
                    var leah = world.GetActorByGlobalId(LeahTempId);
                    if (leah != null)
                        leah.Hidden = false;
                    SetActorVisible(world, ActorSno._tristram_mayor, false);
                    var cart = world.GetActorBySNO(ActorSno._trout_newtristram_blocking_cart, true);
                    if (cart != null)
                        cart.Hidden = true;
                    //this.Game.GetWorld(71150).GetActorBySNO(196224, true).Hidden = true;
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(23, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to 1st floor of church
                    ListenTeleport(19780, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 15,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Cain in 1st floor
                    ListenTeleport(60714, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(15, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 17,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill skeletons //115403 elite
                    //this.Game.GetWorld(60713).SpawnMonster(115403, new Vector3D{X = 99.131f, Y = 211.501f, Z = 0.1f});
                    Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        SetActorOperable(Game.GetWorld(WorldSno.trdun_cain_intro), ActorSno._trdun_skeletonking_intro_sealed_door, false);
                    });
                    ListenKill(ActorSno._skeleton_a_cain_unique, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 19,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Cain
                    //this.Game.GetWorld(60713).GetActorBySNO(5723, true).Hidden = true;
                    SetActorOperable(Game.GetWorld(WorldSno.trout_town), ActorSno._trout_newtristram_gate_town_nw, false);
                    ListenConversation(17667, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 32,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go with Cain
                    Game.CurrentEncounter.activated = false;
                    StartConversation(Game.GetWorld(WorldSno.trdun_cain_intro), 72496);
                    ListenTeleport(19938, new Advance());
                }
            });

            // TODO: this quest seems to be broken. Leah is not spawned in tristram.
            Game.QuestManager.Quests[72095].Steps.Add(32, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Leah in New Tristram
                    var tristramWorld = Game.GetWorld(WorldSno.trout_town);
                    Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        Open(Game.GetWorld(WorldSno.trdun_cain_intro), ActorSno._trdun_cath_bookcaseshelf_door_reverse);
                    });
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        StartConversation(tristramWorld, 72498);
                    });
                    //StartConversation(this.Game.GetWorld(71150), 72496);
                    var leah = tristramWorld.GetActorBySNO(ActorSno._leah, true);
                    if (leah == null)
                    {
                        leah = tristramWorld.GetActorBySNO(ActorSno._leah, false);
                        if (leah != null)
                        {
                            leah.Hidden = false;
                            leah.SetVisible(true);
                        }
                    }

                    // SetActorVisible(tristramWorld, ActorSno._leah, true);
                    ListenConversation(198617, new Advance());
                }
            });

            Game.QuestManager.Quests[72095].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                    //this.Game.GetWorld(71150).GetActorBySNO(196224).Destroy();
                    UnlockTeleport(2);
                    PlayCutscene(1);
                }
            });
            #endregion
            #region Shattered Crown
            Game.QuestManager.Quests.Add(72221, new Quest { RewardXp = 900, RewardGold = 195, Completed = false, Saveable = true, NextQuest = 72061, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72221].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 72221)
                            if (Game.CurrentStep == -1 || Game.CurrentStep == 41)
                            {
                                /*
                                //3533
                                ActorSystem.InteractiveNPC Cain = this.Game.GetWorld(71150).GetActorBySNO(3533,true) as ActorSystem.InteractiveNPC;
                                Cain.Conversations.Clear();
                                Cain.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(198691));
                                Cain.ForceConversationSNO = 198691;
                                Cain.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
                                Cain.Attributes.BroadcastChangedIfRevealed();
                                //*/
                                StartConversation(Game.GetWorld(WorldSno.trout_town), 198691);
                            }
                    });
                    //ListenConversation(198691, new Advance());
                }
            }); //Указать цель

            Game.QuestManager.Quests[72221].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 43,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Hedric
                    var world = Game.GetWorld(WorldSno.trout_town);
                    ActiveArrow(world, ActorSno._pt_blacksmith_nonvendor);
                    var Cain = world.GetActorBySNO(ActorSno._cain, true) as ActorSystem.InteractiveNPC;
                    Cain.Conversations.Clear();
                    Cain.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
                    Cain.Attributes.BroadcastChangedIfRevealed();

                    ListenConversation(198292, new Advance());
                    
                }
            }); //Поговорить с Хэдриком

            Game.QuestManager.Quests[72221].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to cellar and kill zombies
                    var tristramWorld = Game.GetWorld(WorldSno.trout_town);
                    DisableArrow(tristramWorld, tristramWorld.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor));
                    //136441
                    //*
                    Game.AddOnLoadWorldAction(WorldSno.trout_oldtristram_cellar_f, () =>
                    {
                        //ТОЧНО ПРЯЧЕМ КУЗНЕЦА
                        var questWorld = Game.GetWorld(WorldSno.trout_oldtristram_cellar_f);
                        var questActor = questWorld.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor);
                        questActor.Hidden = true;
                        questActor.SetVisible(false);
                        foreach (var plr in questWorld.Players.Values)
                            questActor.Unreveal(plr);
                        //Добавляем
                        AddFollower(questWorld, ActorSno._pt_blacksmith_nonvendor);
                        //Даём мощ
                        foreach (var Smith in questWorld.GetActorsBySNO(ActorSno._pt_blacksmith_nonvendor))
                        {
                            var monsterLevels = (DiIiS_NA.Core.MPQ.FileFormats.GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.GameBalance][19760].Data;
                            float DamageMin = monsterLevels.MonsterLevel[Game.MonsterLevel].Dmg * 0.5f;
                            float DamageDelta = DamageMin * 0.3f;
                            Smith.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * Game.DmgModifier;
                            Smith.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;
                        }
                        
                    });
                    //*/
                    ListenInteract(ActorSno._trdun_blacksmith_cellardoor_breakable, 1, new CellarZombies()); // Октрыть дверь
                    ListenConversation(131339, new LaunchConversation(131774));
                    ListenKill(ActorSno._zombieskinny_a_leahinn, 14, new Advance()); // Убить всех 
                }
            }); //Событие в подвале

            Game.QuestManager.Quests[72221].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 45,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Mira Imon
                    ListenProximity(ActorSno._blacksmithwife, new LaunchConversation(131345));
                    ListenConversation(131345, new LaunchConversation(193264));
                    ListenConversation(193264, new SpawnMiraImon());
                    ListenKill(ActorSno._zombiefemale_a_blacksmitha, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(45, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 35,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk to Hedric
                    var world = Game.GetWorld(WorldSno.trout_oldtristram_cellar_f);
                    var Hedric = world.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor, true);
                    if (Hedric != null)
                    {
                        Vector3D PositionToSpawn = Hedric.Position;
                        DestroyFollower(ActorSno._pt_blacksmith_nonvendor);
                        world.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor).Teleport(PositionToSpawn);
                    }
                    world.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor).Hidden = false;
                    world.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor).SetVisible(true);
                    foreach (var plr in world.Players.Values) world.GetActorBySNO(ActorSno._pt_blacksmith_nonvendor).Reveal(plr);
                    ListenConversation(198312, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(35, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 25,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //open north gates
                    ListenInteract(ActorSno._trout_newtristram_gate_town_nw, 1, new Advance());
                    if (!Game.Empty)
                        foreach (var plr in Game.Players.Values)
                        {
                            if (!plr.BlacksmithUnlocked)
                            {
                                plr.BlacksmithUnlocked = true;
                                plr.GrantAchievement(74987243307766);
                                //plr.UpdateAchievementCounter(403, 1, 0);
                                plr.LoadCrafterData();
                            }
                        }
                    SetActorOperable(Game.GetWorld(WorldSno.trout_town), ActorSno._trout_newtristram_gate_town_nw, true);
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(25, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 37,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to graveyard
                    ListenProximity(ActorSno._cemetary_gate_trout_wilderness_no_lock, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(37, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 59,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find crown holder
                    var world = Game.GetWorld(WorldSno.trout_town);
                    script = new CryptPortals();
                    script.Execute(world);
                    if (Game.Players.Count == 0) UnlockTeleport(6);
                    if (world.GetActorsBySNO(ActorSno._cemetary_gate_trout_wilderness_no_lock).Where(d => d.Visible).FirstOrDefault() != null)
                        Open(world, ActorSno._cemetary_gate_trout_wilderness_no_lock);
                    ListenInteract(ActorSno._a1dun_crypts_leoric_crown_holder, 1, new Advance());
                    //199642 - holder
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(59, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 61,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Imon advisor
                    if (!Game.Players.IsEmpty) UnlockTeleport(6);
                    Game.AddOnLoadWorldAction(WorldSno.trdun_crypt_skeletonkingcrown_02, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trdun_crypt_skeletonkingcrown_02);
                        world.SpawnMonster(ActorSno._ghost_a_unique_chancellor, world.GetActorBySNO(ActorSno._ghost_a_unique_chancellor_spawner).Position);// or 156381
                    });
                    ListenKill(ActorSno._ghost_a_unique_chancellor, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(61, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 54,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //get Leoric crown
                    ListenInteract(ActorSno._a1dun_crypts_leoric_crown_holder_crowntreasureclass, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(54, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Tristram (by town portal) and talk to Hedric
                    ListenConversation(196041, new Advance());
                }
            });

            Game.QuestManager.Quests[72221].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
            #region Reign of Black King
            Game.QuestManager.Quests.Add(72061, new Quest { RewardXp = 5625, RewardGold = 810, Completed = false, Saveable = true, NextQuest = 117779, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72061].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 30,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    StartConversation(Game.GetWorld(WorldSno.trout_town), 80681);
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(30, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 28,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to cathedral garden
                    ListenTeleport(19938, new BackToCath());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 3,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //enter Hall of Leoric
                    Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        SetActorOperable(Game.GetWorld(WorldSno.trdun_cain_intro), ActorSno._trdun_skeletonking_intro_sealed_door, true);
                    });
                    ListenTeleport(60714, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(3, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 6,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to 2nd level of Cath
                    ListenTeleport(19783, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(6, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 37,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //we need to go deeper (to 3rd level of Cath)
                    ListenTeleport(87907, new Advance());
                }
            });

            
            Game.QuestManager.Quests[72061].Steps.Add(37, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 40,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //help Cormac(kill cultists)
                    var Kormak_Imprisoned = Game.GetWorld(WorldSno.a1trdun_level05_templar).GetActorBySNO(ActorSno._templarnpc_imprisoned);
                    foreach (var act in Kormak_Imprisoned.GetActorsInRange(80)) 
                        if (act.SNO == ActorSno._triunecultist_a_templar)
                        {
                            Prisoners.Add(act as ActorSystem.Monster);
                            (act as ActorSystem.Monster).Brain.DeActivate();
                            act.SetFacingRotation(ActorSystem.Movement.MovementHelpers.GetFacingAngle(act, Kormak_Imprisoned));
                        }

                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        try
                        {
                            foreach (var act in Prisoners)
                            {       //act.AddRopeEffect(182614, Kormak_Imprisoned); //[111529] triuneSummoner_Summon_rope
                                Kormak_Imprisoned.AddRopeEffect(182614, act); //[111529] triuneSummoner_Summon_rope
                                act.SetFacingRotation(ActorSystem.Movement.MovementHelpers.GetFacingAngle(act, Kormak_Imprisoned));
                                act.PlayActionAnimation(AnimationSno.triunecultist_emote_outraisedhands);
                                act.SetIdleAnimation(AnimationSno.triunecultist_emote_outraisedhands);
                            }
                        }
                        catch { }
                    });
                    if (Game.Players.IsEmpty) UnlockTeleport(3);
                    //if (this.Game.Players.Count > 0) this.Game.GetWorld(105406).GetActorBySNO(104813, true).Hidden = true;
                    ListenKill(ActorSno._triunecultist_a_templar, 7, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(40, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 42,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Kormac's stuff 178657
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (Game.CurrentQuest == 72061 && Game.CurrentStep == 40)
                        {
                            var world = Game.GetWorld(WorldSno.a1trdun_level05_templar);
                            foreach (var act in Prisoners)
                                act.Brain.Activate();
                            if (ProxyObject != null)
                                ProxyObject.Destroy();
                            AddFollower(world, ActorSno._templarnpc_imprisoned);
                            StartConversation(world, 104782);
                        }
                    });
                    ListenInteract(ActorSno._templarintro_stash, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(42, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 56,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find and kill Jondar
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (Game.CurrentQuest == 72061 && Game.CurrentStep == 42)
                        {
                            var world = Game.GetWorld(WorldSno.a1trdun_level05_templar);
                            DestroyFollower(ActorSno._templarnpc_imprisoned);
                            AddFollower(world, ActorSno._templarnpc_imprisoned);
                            StartConversation(world, 168278);
                            Game.AddOnLoadSceneAction(32993, () =>
                            {
                                StartConversation(world, 168282);
                            });
                        }
                    });
                    ListenKill(ActorSno._adventurer_d_templarintrounique, 1, new LaunchConversation(104676));
                    ListenConversation(104676, new JondarDeath());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(56, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 44,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //join templar(wtf?)
                    var world = Game.GetWorld(WorldSno.a1trdun_level05_templar);
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (Game.CurrentQuest == 72061 && Game.CurrentStep == 56)
                        {
                            DestroyFollower(ActorSno._templarnpc_imprisoned);
                            //AddFollower(this.Game.GetWorld(105406), 104813);
                        }
                        foreach (var Wall in world.GetActorsBySNO(ActorSno._trdun_cath_bonewall_a_door))
                        {
                            Wall.PlayAnimation(11, AnimationSno.trdun_cath_bonewall_a_death);
                            Wall.Attributes[GameAttribute.Deleted_On_Server] = true;
                            Wall.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
                            Wall.Attributes.BroadcastChangedIfRevealed();
                            Wall.Destroy();
                        }

                    });
                    UnlockTeleport(3);
                    script = new Advance();
                    script.Execute(world);
                    if (!Game.Empty)
                        foreach (var plr in Game.Players.Values)
                        {
                            if (!plr.HirelingTemplarUnlocked)
                            {
                                plr.HirelingTemplarUnlocked = true;
                                plr.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 1 });
                                plr.GrantAchievement(74987243307073);
                            }
                        }

                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(44, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 66,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //enter king's crypt
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (Game.CurrentQuest == 72061 && Game.CurrentStep == 44)
                        {
                            //DestroyFollower(104813);
                        }
                    });
                    ListenTeleport(19787, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(66, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 16,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Leoric's crypt
                    ListenProximity(ActorSno._trdun_skeletonking_sealed_door, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 58,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //enter crypt
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        Game.GetWorld(WorldSno.a1trdun_king_level08).GetActorBySNO(ActorSno._trdun_skeletonking_bridge_active, true).Hidden = true;
                    });
                    UnlockTeleport(4);
                    ListenTeleport(19789, new Advance());
                    //if (!this.Game.Empty) this.Game.GetWorld(73261).GetActorBySNO(461, true).Hidden = true;
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(58, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 19,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill skeletons
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        script = new SpawnSkeletons();
                        script.Execute(Game.GetWorld(WorldSno.a1trdun_king_level08));

                    });

                    ListenKill(ActorSno._skeletonking_shield_skeleton, 4, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 21,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //take crown on Leoric's head
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        Open(Game.GetWorld(WorldSno.a1trdun_king_level08), ActorSno._trdun_cath_gate_b_skeletonking);
                    });
                    //Open(this.Game.GetWorld(73261), 172645);
                    ListenInteract(ActorSno._skeletonkinggizmo, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 48,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Leoric
                    ListenKill(ActorSno._skeletonking, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(48, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 50,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to fallen star room
                    Game.CurrentEncounter.activated = false;
                    ListenTeleport(117411, new Advance());
                    Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        Open(Game.GetWorld(WorldSno.a1trdun_king_level08), ActorSno._trdun_crypt_skeleton_king_throne_parts);
                    });
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(50, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 52,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Tyrael convList 117403 qr 176870
                    ListenInteract(ActorSno._stranger_crater, 1, new LaunchConversation(181910)); //cork
                    ListenConversation(181910, new LaunchConversation(181912));
                    ListenConversation(181912, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(52, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 54,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //return to New Tristram
                    ListenTeleport(19947, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(54, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 24,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Cain
                    UnlockTeleport(5);
                    ListenConversation(117371, new Advance());
                }
            });

            Game.QuestManager.Quests[72061].Steps.Add(24, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                    PlayCutscene(2);
                }
            });
            #endregion
            #region Tyrael Sword
            Game.QuestManager.Quests.Add(117779, new Quest { RewardXp = 4125, RewardGold = 630, Completed = false, Saveable = true, NextQuest = 72738, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[117779].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    var world = Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 198706);
                    world.GetActorBySNO(ActorSno._cemetary_gate_trout_wilderness_static).Hidden = true;
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 10,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Wild Fields
                    var world = Game.GetWorld(WorldSno.trout_town);
                    ListenTeleport(19952, new Advance());
                    ListenProximity(ActorSno._woodfencee_fields_trout, new Advance()); //if going through graveyard
                    var Gate = world.GetActorBySNO(ActorSno._cemetary_gate_trout_wilderness_no_lock);
                    Gate.Field2 = 16;
                    Gate.PlayAnimation(5, (AnimationSno)Gate.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                    world.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDCollFlagsMessage
                    {
                        ActorID = Gate.DynamicID(plr),
                        CollFlags = 0
                    }, Gate);

                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(10, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 3,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Hazra cave
                    ListenTeleport(119893, new Advance());
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(3, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 18,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find piece of sword
                    Game.AddOnLoadWorldAction(WorldSno.fields_cave_swordofjustice_level01, () =>
                    {
                        if (Game.CurrentQuest == 117779 && Game.CurrentStep == 3)
                        {
                            StartConversation(Game.GetWorld(WorldSno.fields_cave_swordofjustice_level01), 130225);
                        }
                    });
                    //if (!this.Game.Empty) StartConversation(this.Game.GetWorld(119888), 130225);
                    ListenProximity(ActorSno._triunecultist_e, new Advance());
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(18, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 13,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill cultists
                    UnlockTeleport(7);
                    ListenKill(ActorSno._triunecultist_e, 6, new LaunchConversation(131144));
                    ListenConversation(131144, new LaunchConversation(194412));
                    ListenConversation(194412, new LaunchConversation(141778));
                    ListenConversation(141778, new Advance());
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(13, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 5,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //get piece of sword
                    ListenInteract(ActorSno._trdun_cave_swordofjustice_blade, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(5, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //take piece to Cain
                    ListenConversation(118037, new Advance());
                }
            });

            Game.QuestManager.Quests[117779].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                    StartConversation(Game.GetWorld(WorldSno.trout_town), 198713);
                }
            });
            #endregion
            #region Broken Blade
            Game.QuestManager.Quests.Add(72738, new Quest { RewardXp = 6205, RewardGold = 1065, Completed = false, Saveable = true, NextQuest = 73236, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72738].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 86,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    var world = Game.GetWorld(WorldSno.trout_town);
                    var leah = world.GetActorBySNO(ActorSno._leah, true);
                    LeahTempId = leah.GlobalID;
                    leah.Hidden = true;
                    StartConversation(world, 198713);
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(86, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 88,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Sunken Temple
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    ListenProximity(ActorSno._scoundrelnpc, new LaunchConversation(111893));
                    ListenConversation(111893, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(88, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 90,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //follow Scoundrel NPC
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    AddFollower(world, ActorSno._scoundrelnpc);
                    //Open(this.Game.GetWorld(71150), 170913);
                    StartConversation(world, 167656);
                    ListenConversation(167656, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(90, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 92,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with bandits
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    try { (world.FindAt(ActorSno._trout_tristramfield_field_gate, new Vector3D { X = 1523.13f, Y = 857.71f, Z = 39.26f }, 5.0f) as Door).Open(); } catch { }
                    StartConversation(world, 167677);
                    ListenConversation(167677, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(92, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 94,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill the bandits
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    world.SpawnMonster(ActorSno._graverobber_c_nigel, new Vector3D { X = 1471.473f, Y = 747.4875f, Z = 40.1f });
                    ListenKill(ActorSno._graverobber_c_nigel, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(94, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 112,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Scoundrel
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    ListenProximity(ActorSno._scoundrelnpc, new LaunchConversation(111899));
                    ListenConversation(111899, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(112, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //lead Scoundrel to waypoint
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    try { (world.FindAt(ActorSno._trout_tristramfield_field_gate, new Vector3D { X = 1444.1f, Y = 786.64f, Z = 39.7f }, 4.0f) as Door).Open(); } catch { }
                    SetActorOperable(world, ActorSno._keybox_trout_tristramfield_02, false);
                    SetActorOperable(world, ActorSno._keybox_trout_tristramfield, false);
                    ListenProximity(ActorSno._waypoint, new Advance());
                    if (!Game.Empty)
                        foreach (var plr in Game.Players.Values)
                        {
                            if (!plr.HirelingScoundrelUnlocked)
                            {
                                plr.HirelingScoundrelUnlocked = true;
                                plr.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 2 });
                                plr.GrantAchievement(74987243307147);
                            }
                            if (Game.Players.Count > 1)
                                plr.InGameClient.SendMessage(new HirelingNoSwapMessage() { NewClass = 2 }); //Призвать нельзя!
                            else
                                plr.InGameClient.SendMessage(new HirelingSwapMessage() { NewClass = 2 }); //Возможность призвать
                        }
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Sunken Temple
                    var world = Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 223934);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    DestroyFollower(ActorSno._scoundrelnpc);
                    ListenProximity(ActorSno._ghostknight1_festering, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 28,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Alaric
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    UnlockTeleport(8);
                    ListenConversation(81576, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 12,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Rotten forest
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    Open(world, ActorSno._a1dun_caves_neph_waterbridge_a); //bridge
                    ListenProximity(ActorSno._a1dun_caves_neph_waterbridge_a, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(12, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 14,
                Objectives = new List<Objective> { Objective.Default(), Objective.Default() },
                OnAdvance = () =>
                { //find 2 Orbs
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    ListenInteract(ActorSno._a1dun_caves_nephalem_altar_a_chest_03, 1, new CompleteObjective(0));
                    ListenInteract(ActorSno._a1dun_caves_nephalem_altar_a_chest_03_b, 1, new CompleteObjective(1));
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(14, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 30,
                Objectives = new List<Objective> { new Objective { Limit = 2, Counter = 0 } },
                OnAdvance = () =>
                { //use 2 stones
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    UnlockTeleport(9);
                    SetActorOperable(world, ActorSno._keybox_trout_tristramfield_02, true);
                    SetActorOperable(world, ActorSno._keybox_trout_tristramfield, true);
                    ListenInteract(ActorSno._keybox_trout_tristramfield_02, 1, new CompleteObjective(0));
                    ListenInteract(ActorSno._keybox_trout_tristramfield, 1, new CompleteObjective(0));
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(30, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 38,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //enter the temple
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    Open(world, ActorSno._a1dun_caves_neph_waterbridge_a_short); //bridge
                    Open(world, ActorSno._trout_nephalem_door_head_a);
                    ListenTeleport(60398, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(38, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 69,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //explore the temple
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    //60395 - trdun_cave_nephalem_03

                    ListenProximity(ActorSno._a1dun_caves_ropebridge_b_destructable, new DrownedTemple1());
                    ListenKill(ActorSno._skeleton_b, 14, new LaunchConversation(108256));
                    ListenConversation(108256, new DrownedTemple2());//new Advance());

                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(69, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 99,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill prophet Ezek and skeletons
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);

                    var world = Game.GetWorld(WorldSno.trdun_cave_nephalem_03);
                    foreach (var act in world.GetActorsBySNO(ActorSno._nephalem_ghost_a_drownedtemple_martyr_skeleton)) act.Destroy();
                    world.SpawnMonster(ActorSno._nephalem_ghost_a_drownedtemple_martyr_skeleton, new Vector3D(292f, 275f, -76f));

                    ListenKill(ActorSno._nephalem_ghost_a_drownedtemple_martyr_skeleton, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(99, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 103,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Alaric in temple
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    StartConversation(Game.GetWorld(WorldSno.trdun_cave_nephalem_03), 133372);
                    ListenConversation(133372, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(103, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 71,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //defend the sword piece
                    DestroyFollower(ActorSno._leah);
                    AddFollower(Game.GetWorld(WorldSno.trout_town), ActorSno._leah);
                    Game.AddOnLoadWorldAction(WorldSno.trdun_cave_nephalem_03, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trdun_cave_nephalem_03);
                        Open(world, ActorSno._a1dun_caves_drownedtemple_walldoor);
                        if (Game.CurrentQuest == 72738 && Game.CurrentStep == 103)
                        {
                            StartConversation(world, 108256);
                        }
                    });
                    ListenProximity(ActorSno._trdun_cave_swordofjustice_shard, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(71, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 56,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //use the piece of sword
                    var world = Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(ActorSno._leah);
                    AddFollower(world, ActorSno._leah);
                    ListenInteract(ActorSno._trdun_cave_swordofjustice_shard, 1, new LaunchConversation(198925));
                    ListenConversation(198925, new LaunchConversation(133487));
                    ListenConversation(133487, new Advance());
                    world.GetActorByGlobalId(LeahTempId).Hidden = false;
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(56, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //return to Tristram
                    Game.AddOnLoadWorldAction(WorldSno.trdun_cave_nephalem_03, () =>
                    {
                        if (Game.CurrentQuest == 72738 && Game.CurrentStep == 56)
                        {
                            StartConversation(Game.GetWorld(WorldSno.trdun_cave_nephalem_03), 202967);
                        }
                    });
                    DestroyFollower(ActorSno._leah);
                    ListenProximity(ActorSno._cain, new Advance());
                }
            });

            Game.QuestManager.Quests[72738].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
            #region Doom of Vortham
            Game.QuestManager.Quests.Add(73236, new Quest { RewardXp = 4950, RewardGold = 670, Completed = false, Saveable = true, NextQuest = 72546, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[73236].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    StartConversation(Game.GetWorld(WorldSno.trout_town), 120357);
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 20,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with parom man
                    ListenConversation(72817, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(20, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 59,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //go to Vortem square
                    var AttackedTown = Game.GetWorld(WorldSno.trout_townattack);
                    var Maghda = AttackedTown.GetActorBySNO(ActorSno._maghda_a_tempprojection);
                    AttackedTown.Leave(Maghda);

                    ListenProximity(ActorSno._townattack_cultist, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(59, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //kill all cultists
                    int Count = 0;
                    foreach (var cultist in Game.GetWorld(WorldSno.trout_townattack).GetActorsBySNO(ActorSno._townattackcultistmelee))
                        if (cultist.CurrentScene.SceneSNO.Id == 76000)
                        {
                            cultist.Attributes[GameAttribute.Quest_Monster] = true;
                            cultist.Attributes.BroadcastChangedIfRevealed();
                            Count++;
                        }

                    ListenKill(ActorSno._townattackcultistmelee, Count, new AttackTownKilled());
                    ListenConversation(194933, new LaunchConversation(194942));
                    ListenConversation(194942, new Advance());

                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 16,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  
                    Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (Game.CurrentQuest == 73236 && Game.CurrentStep == 11)
                        {
                            Game.GetWorld(WorldSno.trout_townattack).SpawnMonster(ActorSno._townattack_summoner_unique, new Vector3D { X = 581.237f, Y = 584.346f, Z = 70.1f });
                        }
                        ListenKill(ActorSno._townattack_summoner_unique, 1, new Advance());
                    });
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 63,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //kill 3 berserkers
                    Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (Game.CurrentQuest == 73236 && Game.CurrentStep == 16)
                        {
                            var world = Game.GetWorld(WorldSno.trout_townattack);
                            world.SpawnMonster(ActorSno._townattack_berserker, new Vector3D { X = 577.724f, Y = 562.869f, Z = 70.1f });
                            world.SpawnMonster(ActorSno._townattack_berserker, new Vector3D { X = 565.886f, Y = 577.66f, Z = 70.1f });
                            world.SpawnMonster(ActorSno._townattack_berserker, new Vector3D { X = 581.308f, Y = 581.079f, Z = 70.1f });
                        }
                    });
                    ListenKill(ActorSno._townattack_berserker, 3, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(63, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 65,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //talk with priest
                    Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (Game.CurrentQuest == 73236 && Game.CurrentStep == 63)
                        {
                            StartConversation(Game.GetWorld(WorldSno.trout_townattack), 120372);
                        }
                    });
                    ListenConversation(120372, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(65, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 67,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //go to church cellar
                    //this.Game.AddOnLoadAction(72882, () =>
                    //{
                    //this.Game.GetWorld(72882).GetActorBySNO(91162).Destroy();
                    //});
                    ListenTeleport(119870, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(67, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 69,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //find piece of sword
                    ListenInteract(ActorSno._trout_townattack_cellar_altar, 1, new LaunchConversation(165080));
                    ListenConversation(165080, new LaunchConversation(165101));
                    ListenConversation(165101, new Advance());
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(69, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 9,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {  //go to Cain's house
                     
                    if (!Game.Empty) StartConversation(Game.GetWorld(WorldSno.fields_cave_swordofjustice_level01), 130225);
                    ListenTeleport(130163, new StartSceneinHome());
                    //ListenTeleport(130163, new LaunchConversation(165125));
                    ListenConversation(165125, new LaunchConversation(190199));
                    ListenConversation(190199, new LaunchConversation(190201));
                    ListenConversation(190201, new AttackTownBoominHome());
                    ListenConversation(165428, new LaunchConversation(129640));
                    ListenConversation(129640, new LaunchConversation(178394));
                    ListenConversation(178394, new LaunchConversation(165161));
                    ListenConversation(165161, new LaunchConversation(165170));
                    ListenConversation(165170, new LaunchConversation(120382));
                    ListenConversation(120382, new LaunchConversation(121703));

                    //Активируем сцену
                    //ListenConversation(165125, new LaunchConversation(143386));
                    //ListenConversation(143386, new LaunchConversation(120382));
                    //ListenConversation(120382, new LaunchConversation(121703));
                    ListenConversation(121703, new EndSceneinHome());  
                }
            });

            Game.QuestManager.Quests[73236].Steps.Add(9, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    UnlockTeleport(10);
                }
            });
            #endregion
            #region To the Black Cult
            Game.QuestManager.Quests.Add(72546, new Quest { RewardXp = 8275, RewardGold = 455, Completed = false, Saveable = true, NextQuest = 72801, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72546].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                    Game.AddOnLoadWorldAction(WorldSno.trout_townattack_chapelcellar_a, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trout_townattack_chapelcellar_a);
                        foreach (var Table in world.GetActorsBySNO(ActorSno._trout_townattack_cellar_altar)) {
                            Table.SetUsable(false);
                            Table.SetIdleAnimation((AnimationSno)Table.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Open]); 
                        }
                        foreach (var Maghda in world.GetActorsBySNO(ActorSno._maghda_a_tempprojection)) Maghda.Destroy();
                    });
                    var tristramWorld = Game.GetWorld(WorldSno.trout_town);
                    var Leah = tristramWorld.GetActorBySNO(ActorSno._leah);
                    var LeahAfterEvent = tristramWorld.SpawnMonster(ActorSno._leah_afterevent31_exit, Leah.Position);
                    
                    //ListenProximity(4580, new LaunchConversation(93337)); //cork
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Conversations.Clear();
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(93337));
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 2;
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
                    ListenConversation(93337, new Advance());
                    Game.CurrentEncounter.activated = false;
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Aranea Cave
                    var LeahAfterEvent = Game.GetWorld(WorldSno.trout_town).GetActorBySNO(ActorSno._leah_afterevent31_exit);
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 1;
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
                    ListenTeleport(78572, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 31,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Aranea Queen lair
                    Game.GetWorld(WorldSno.trout_town).GetActorBySNO(ActorSno._leah_afterevent31_exit, true).Hidden = true;
                    //this.Game.GetWorld(71150).GetActorBySNO(138271,true).SetVisible(false);
                    ListenTeleport(62726, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(31, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 19,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with woman in web
                    var world = Game.GetWorld(WorldSno.a1dun_spidercave_02);
                    SetActorOperable(world, ActorSno._a2dun_spider_venom_pool, false);
                    SetActorOperable(world, ActorSno._a2dun_spider_queen_web_door, false);
                    ListenProximity(ActorSno._a2dun_spider_queen_web_door, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Aranea Queen
                    Game.GetWorld(WorldSno.a1dun_spidercave_02).SpawnMonster(ActorSno._spiderqueen, new Vector3D { X = 149.439f, Y = 121.452f, Z = 13.794f });
                    ListenKill(ActorSno._spiderqueen, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 23,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //grab Aranea acid
                    SetActorOperable(Game.GetWorld(WorldSno.a1dun_spidercave_02), ActorSno._a2dun_spider_venom_pool, true);
                    ListenInteract(ActorSno._a2dun_spider_venom_pool, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(23, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //use acid on Karina
                    SetActorOperable(Game.GetWorld(WorldSno.a1dun_spidercave_02), ActorSno._a2dun_spider_queen_web_door, true);
                    ListenInteract(ActorSno._a2dun_spider_queen_web_door, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 47,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Southern Highlands
                    ListenTeleport(93632, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(47, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 29,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Karina
                    SetActorOperable(Game.GetWorld(WorldSno.trout_town), ActorSno._trout_highlands_goatmen_chokepoint_gate, false);
                    ListenProximity(ActorSno._mystic_b, new LaunchConversation(191511)); //cork
                    ListenConversation(191511, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(29, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 36,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Hazra staff
                    ListenInteract(ActorSno._trout_highlands_mystic_wagon, 1, new Advance());
                    if (Game.Empty) UnlockTeleport(11);
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(36, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 10,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Hazra wall
                    SetActorOperable(Game.GetWorld(WorldSno.trout_town), ActorSno._trout_highlands_goatmen_chokepoint_gate, true);
                    UnlockTeleport(11);
                    ListenInteract(ActorSno._trout_highlands_goatmen_chokepoint_gate, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(10, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to the Leoric's Manor
                    ListenInteract(ActorSno._trout_highlands_manor_front_gate, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //enter the Leoric's Manor
                    ListenTeleport(100854, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 43,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //explore the Leoric's Manor
                    UnlockTeleport(12);
                    ListenInteract(ActorSno._a1dun_leor_manor_deathofcain_door, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 16,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill cultists
                    if (!Game.Empty) StartConversation(Game.GetWorld(WorldSno.a1dun_leor_manor), 134968);
                    ListenConversation(134968, new LaunchConversation(134565));
                    var world = Game.GetWorld(WorldSno.trout_town);
                    var leah = world.GetActorBySNO(ActorSno._leah);
                    leah.Hidden = false;
                    leah.SetVisible(true);
                    ListenKill(ActorSno._triunecultist_a, 7, new Advance());
                }
            });

            Game.QuestManager.Quests[72546].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
            #region Captived Angel
            Game.QuestManager.Quests.Add(72801, new Quest { RewardXp = 10925, RewardGold = 1465, Completed = false, Saveable = true, NextQuest = 136656, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[72801].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to 1st level of Torture Rooms
                    UnlockTeleport(13);
                    ListenTeleport(19774, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 65,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to 2nd level of Torture Rooms
                    ListenTeleport(19775, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(65, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 2,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Highlands Bridge
                    if (Game.Empty) UnlockTeleport(14);
                    ListenTeleport(87832, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(2, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //go to Leoric's Jail
                    UnlockTeleport(14);
                    ListenTeleport(94672, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Asilla Queen (npc 103381)
                    Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        SetActorOperable(Game.GetWorld(WorldSno.trdun_jail_level01), ActorSno._a1dun_leor_jail_door_a, false);
                    });
                    ListenConversation(103388, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 19,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //free 6 souls
                    //spawn souls on 104104, 104106, 104108
                    Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trdun_jail_level01);
                        SetActorOperable(world, ActorSno._a1dun_leor_jail_door_a, true);
                        script = new SpawnSouls();
                        script.Execute(world);
                    });
                    ListenInteract(ActorSno._ghost_jail_prisoner, 6, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 36,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Overseer
                    Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        Game.GetWorld(WorldSno.trdun_jail_level01).SpawnMonster(ActorSno._gravedigger_warden, new Vector3D { X = 360.236f, Y = 840.47f, Z = 0.1f });
                    });
                    ListenKill(ActorSno._gravedigger_warden, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(36, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Butcher's Room		
                    if (Game.Empty) UnlockTeleport(15);
                    Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        Open(Game.GetWorld(WorldSno.trdun_jail_level01), ActorSno._a1dun_leor_jail_door_a_exit);
                    });
                    ListenTeleport(90881, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill Butcher
                    Game.AddOnLoadWorldAction(WorldSno.trdun_butcherslair_02, () =>
                    {
                        var world = Game.GetWorld(WorldSno.trdun_butcherslair_02);
                        SetActorOperable(world, ActorSno._a1dun_leor_gate_a, false);
                        if (world.GetActorBySNO(ActorSno._butcher) == null)
                            world.SpawnMonster(ActorSno._butcher, new Vector3D { X = 93.022f, Y = 89.86f, Z = 0.1f });

                    });
                    UnlockTeleport(15);
                    ListenKill(ActorSno._butcher, 1, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 39,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //find Tyrael
                    Game.CurrentEncounter.activated = false;
                    Game.AddOnLoadWorldAction(WorldSno.trdun_butcherslair_02, () =>
                    {
                        SetActorOperable(Game.GetWorld(WorldSno.trdun_butcherslair_02), ActorSno._a1dun_leor_gate_a, true);
                    });
                    ListenTeleport(148551, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(39, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //kill cultists
                    ListenKill(ActorSno._triunevessel_event31, 6, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 13,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Tyrael (npc 183117)
                    ListenProximity(ActorSno._stranger_ritual, new LaunchConversation(120220)); //cork
                    ListenConversation(120220, new Advance());
                }
            });

            Game.QuestManager.Quests[72801].Steps.Add(13, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
            #region Return to New Tristram
            Game.QuestManager.Quests.Add(136656, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

            Game.QuestManager.Quests[136656].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                {
                }
            });

            Game.QuestManager.Quests[136656].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with Tyrael

                    ListenProximity(ActorSno._tyrael, new LaunchConversation(72897)); //cork
                    ListenConversation(72897, new Advance());
                }
            });

            Game.QuestManager.Quests[136656].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 4,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //talk with caravan leader
                    
                    ListenConversation(177564, new ChangeAct(100));
                }
            });

            Game.QuestManager.Quests[136656].Steps.Add(4, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { Objective.Default() },
                OnAdvance = () =>
                { //complete
                }
            });
            #endregion
        }

        public static bool Break(MapSystem.World world, ActorSno sno)
		{
			var actor = world.GetActorBySNO(sno);
			(actor as DesctructibleLootContainer).Die();
			return true;
		}
	}
}
