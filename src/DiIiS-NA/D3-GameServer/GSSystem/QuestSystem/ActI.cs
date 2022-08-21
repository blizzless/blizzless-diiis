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
            this.Game.QuestManager.Quests.Add(87700, new Quest
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

            this.Game.QuestManager.Quests[87700].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 66,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 87700 & Game.CurrentStep == -1)
                        {
                            //Указывает куда идти
                            //ActiveArrow(this.Game.GetWorld(71150), 3739);

                            //Убираем лишнюю Лею
                            var Leah = world.GetActorBySNO(4580, true);
                            if (Leah != null) Leah.Hidden = true;
                        }
                    });
                    setActorOperable(world, 90419, false);
                    ListenConversation(151087, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(66, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 42,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    script = new SurviveTheWaves();
                    script.Execute(world);
                    var Leah = world.GetActorBySNO(4580, true);
                    if (Leah != null) Leah.Hidden = true;
                    ListenKill(6644, 6, new SecondWave());
                    ListenKill(6632, 7, new Advance());


                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(42, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 75,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go and talk to Leah
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 151102);
                    
                    try
                    {
                        setActorOperable(world, 3739, true);
                        if (world.GetActorBySNO(141508, true) != null)
                            world.GetActorBySNO(141508, true).Hidden = true;
                        if (world.GetActorBySNO(112131, true) != null)
                            world.GetActorBySNO(112131, true).Hidden = true;
                    }
                    catch { }
                    UnlockTeleport(0);
                    if (world.GetActorsBySNO(90419).Where(d => d.Visible).FirstOrDefault() != null)
                        Open(world, 90419);
                    ActiveArrow(world, 178293, WorldSno.trout_tristram_inn);
                    ListenConversation(151123, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(75, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 46,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //fighting zombies
                    script = new LeahInn();
                    script.Execute(this.Game.GetWorld(WorldSno.trout_tristram_inn));

                    ListenKill(203121, 5, new LaunchConversation(151156));
                    ListenConversation(151156, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(46, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 50,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Leah again
                    ListenConversation(151167, new Advance());
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 87700)
                            ActiveArrow(this.Game.GetWorld(WorldSno.trout_town), 3739);
                    });
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(50, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 60,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go and talk to Rumford				
                  //ListenProximity(3739, new LaunchConversation(198503));
                    
                    ListenConversation(198503, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(60, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 27,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill wretched mother
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    Break(world, 81699);

                    foreach (var sp in world.GetActorsBySNO(89957))
                    {
                        if (sp.CurrentScene.SceneSNO.Id == 33348)
                            if (sp is ActorSystem.Spawner)
                                //(sp as ActorSystem.Spawner).Spawn();
                                world.SpawnMonster(6644, sp.Position);
                    }

                    ActivateQuestMonsters(world, 219725);
                    ListenKill(219725, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(27, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 55,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //MOAR wretched mothers
                    StartConversation(this.Game.GetWorld(WorldSno.trout_town), 156223);
                    ListenKill(176889, 1, new Advance());
                    ListenKillBonus(219725, 3, new SideTarget());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(55, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 }, new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //return to New Tristram and talk to Rumford
                    DeactivateQuestMonsters(this.Game.GetWorld(WorldSno.trout_town), 219725);
                    ListenInteract(192164, 1, new CompleteObjective(0));
                    ListenConversation(198521, new Advance());
                })
            });

            this.Game.QuestManager.Quests[87700].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
            #region Rescue Cain
            this.Game.QuestManager.Quests.Add(72095, new Quest { RewardXp = 3630, RewardGold = 190, Completed = false, Saveable = true, NextQuest = 72221, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72095].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { 
                    StartConversation(this.Game.GetWorld(WorldSno.trout_town), 198541);
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 28,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //use Tristram Portal
                    UnlockTeleport(1);
                    ListenTeleport(101351, new Advance());
                    //AddFollower(this.Game.GetWorld(71150), 4580);
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        // TODO: CHeck for possible removing outer adding
                        this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                        {
                            if (Game.CurrentQuest == 72095)
                                if (Game.CurrentStep == -1 || Game.CurrentStep == 7)
                                {
                                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                                }
                        });
                        
                    });
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 49,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to gates
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 166678);
                    ListenProximity(108466, new Advance());
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 72095)
                            if (Game.CurrentStep == 28 || Game.CurrentStep == 7 || Game.CurrentStep == -1)
                                ActiveArrow(world, 108466);
                      
                    });
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(49, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 39,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Adria house
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    if (world.GetActorsBySNO(108466).Where(d => d.Visible).FirstOrDefault() != null)
                        Open(world, 108466);
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        if (Game.CurrentQuest == 72095)
                            if (Game.CurrentStep == 49 || Game.CurrentStep == 39)
                                ActiveArrow(world, 191886, WorldSno.trout_adriascellar);
                    });
                    ListenProximity(191886, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(39, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //inspect house
                    ListenProximity(191886, new Advance());

                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to cave
                    //DestroyFollower(4580);
                    ListenTeleport(62968, new Advance());
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_adriascellar, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trout_adriascellar);
                        foreach (var lh in world.GetActorsBySNO(203030))
                        {
                            lh.SetVisible(false);
                            lh.Hidden = true;
                        }

                        if (Game.CurrentQuest == 72095)
                            ActiveArrow(world, 131123);
                    });
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 43,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //inspect cave

                    ListenInteract(131123, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 45,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Daltin (156801)
                    ActorSystem.Actor CapitanDaltyn = null;
                    Vector3D[] Zombies = new Vector3D[4];
                    Zombies[0] = new Vector3D(50.00065f, 125.4087f, 0.1000305f);
                    Zombies[1] = new Vector3D(54.88688f, 62.24541f, 0.1000305f);
                    Zombies[2] = new Vector3D(86.45869f, 77.09571f, 0.1000305f);
                    Zombies[3] = new Vector3D(102.117f, 97.59058f, 0.1000305f);



                    this.Game.AddOnLoadWorldAction(WorldSno.trout_adriascellar, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trout_adriascellar);
                        CapitanDaltyn = world.SpawnMonster(156801, new Vector3D { X = 52.587f, Y = 103.368f, Z = 0.1f });
                        CapitanDaltyn.Attributes[GameAttribute.Quest_Monster] = true;
                        CapitanDaltyn.PlayAnimation(5, 11523);
                        foreach (Vector3D point in Zombies)
                        {
                            var Zombie = world.SpawnMonster(6644, point);
                            Zombie.Attributes[GameAttribute.Quest_Monster] = true;
                            Zombie.PlayAnimation(5, 11523);
                        }
                    });
                    ListenKill(156801, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(45, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 47,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Leah in cave
                    var world = this.Game.GetWorld(WorldSno.trout_adriascellar);
                    foreach (var host in world.GetActorsBySNO(4580))
                    {
                        foreach (var lh in world.GetActorsBySNO(203030))
                        {
                            lh.SetVisible(true);
                            lh.Hidden = false;
                            lh.Teleport(host.Position);
                            lh.Position = host.Position;
                        }
                        //this.Game.GetWorld(62751).SpawnMonster(203030, lh.Position);
                    }
                    DestroyFollower(4580);
                    ListenConversation(198588, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(47, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 23,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to church				
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    ListenProximity(167289, new Advance());
                    if (world.GetActorByGlobalId(LeahTempId) != null)
                        world.GetActorByGlobalId(LeahTempId).Hidden = false;
                    setActorVisible(world, 141508, false);
                    if (world.GetActorBySNO(112131, true) != null)
                        world.GetActorBySNO(112131, true).Hidden = true;
                    //this.Game.GetWorld(71150).GetActorBySNO(196224, true).Hidden = true;
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(23, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to 1st floor of church
                    ListenTeleport(19780, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 15,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Cain in 1st floor
                    ListenTeleport(60714, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(15, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 17,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill skeletons //115403 elite
                  //this.Game.GetWorld(60713).SpawnMonster(115403, new Vector3D{X = 99.131f, Y = 211.501f, Z = 0.1f});
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        setActorOperable(this.Game.GetWorld(WorldSno.trdun_cain_intro), 156058, false);
                    });
                    ListenKill(115403, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 19,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Cain
                  //this.Game.GetWorld(60713).GetActorBySNO(5723, true).Hidden = true;
                    setActorOperable(this.Game.GetWorld(WorldSno.trout_town), 121241, false);
                    ListenConversation(17667, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 32,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go with Cain
                    this.Game.CurrentEncounter.activated = false;
                    StartConversation(this.Game.GetWorld(WorldSno.trdun_cain_intro), 72496);
                    ListenTeleport(19938, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(32, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Leah in New Tristram
                    var tristramWorld = this.Game.GetWorld(WorldSno.trout_town);
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        Open(this.Game.GetWorld(WorldSno.trdun_cain_intro), 5723);
                    });
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
                    {
                        StartConversation(tristramWorld, 72498);
                    });
                    //StartConversation(this.Game.GetWorld(71150), 72496);
                    var CheckLeah = tristramWorld.GetActorBySNO(4580, true);
                    if (CheckLeah == null)
                    {
                        var Leah = tristramWorld.GetActorBySNO(4580, false);
                        if (Leah != null)
                        {
                            Leah.Hidden = false;
                            Leah.SetVisible(true);
                        }
                    }
                    ListenConversation(198617, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72095].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                  //this.Game.GetWorld(71150).GetActorBySNO(196224).Destroy();
                    UnlockTeleport(2);
                    PlayCutscene(1);
                })
            });
            #endregion
            #region Shattered Crown
            this.Game.QuestManager.Quests.Add(72221, new Quest { RewardXp = 900, RewardGold = 195, Completed = false, Saveable = true, NextQuest = 72061, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72221].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_town, () =>
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
                                StartConversation(this.Game.GetWorld(WorldSno.trout_town), 198691);
                            }
                    });
                    //ListenConversation(198691, new Advance());
                })
            }); //Указать цель

            this.Game.QuestManager.Quests[72221].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 43,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Hedric
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    ActiveArrow(world, 65036);
                    var Cain = world.GetActorBySNO(3533, true) as ActorSystem.InteractiveNPC;
                    Cain.Conversations.Clear();
                    Cain.Attributes[GameAttribute.Conversation_Icon, 0] = 1;
                    Cain.Attributes.BroadcastChangedIfRevealed();

                    ListenConversation(198292, new Advance());
                    
                })
            }); //Поговорить с Хэдриком

            this.Game.QuestManager.Quests[72221].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to cellar and kill zombies
                    var tristramWorld = this.Game.GetWorld(WorldSno.trout_town);
                    DisableArrow(tristramWorld, tristramWorld.GetActorBySNO(65036));
                    //136441
                    //*
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_oldtristram_cellar_f, () =>
                    {
                        //ТОЧНО ПРЯЧЕМ КУЗНЕЦА
                        var questWorld = this.Game.GetWorld(WorldSno.trout_oldtristram_cellar_f);
                        var questActor = questWorld.GetActorBySNO(65036);
                        questActor.Hidden = true;
                        questActor.SetVisible(false);
                        foreach (var plr in questWorld.Players.Values)
                            questActor.Unreveal(plr);
                        //Добавляем
                        AddFollower(questWorld, 65036);
                        //Даём мощ
                        foreach (var Smith in questWorld.GetActorsBySNO(65036))
                        {
                            var monsterLevels = (DiIiS_NA.Core.MPQ.FileFormats.GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.GameBalance][19760].Data;
                            float DamageMin = monsterLevels.MonsterLevel[this.Game.MonsterLevel].Dmg * 0.5f;
                            float DamageDelta = DamageMin * 0.3f;
                            Smith.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * this.Game.DmgModifier;
                            Smith.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;
                        }
                        
                    });
                    //*/
                    ListenInteract(157541, 1, new CellarZombies()); // Октрыть дверь
                    ListenConversation(131339, new LaunchConversation(131774));
                    ListenKill(203121, 14, new Advance()); // Убить всех 
                })
            }); //Событие в подвале

            this.Game.QuestManager.Quests[72221].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 45,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Mira Imon
                    ListenProximity(98888, new LaunchConversation(131345));
                    ListenConversation(131345, new LaunchConversation(193264));
                    ListenConversation(193264, new SpawnMiraImon());
                    ListenKill(85900, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(45, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 35,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk to Hedric
                    var world = this.Game.GetWorld(WorldSno.trout_oldtristram_cellar_f);
                    var Hedric = world.GetActorBySNO(65036, true);
                    if (Hedric != null)
                    {
                        Vector3D PositionToSpawn = Hedric.Position;
                        DestroyFollower(65036);
                        world.GetActorBySNO(65036).Teleport(PositionToSpawn);
                    }
                    world.GetActorBySNO(65036).Hidden = false;
                    world.GetActorBySNO(65036).SetVisible(true);
                    foreach (var plr in world.Players.Values) world.GetActorBySNO(65036).Reveal(plr);
                    ListenConversation(198312, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(35, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 25,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //open north gates
                    ListenInteract(121241, 1, new Advance());
                    if (!this.Game.Empty)
                        foreach (var plr in this.Game.Players.Values)
                        {
                            if (!plr.BlacksmithUnlocked)
                            {
                                plr.BlacksmithUnlocked = true;
                                plr.GrantAchievement(74987243307766);
                                //plr.UpdateAchievementCounter(403, 1, 0);
                                plr.LoadCrafterData();
                            }
                        }
                    setActorOperable(this.Game.GetWorld(WorldSno.trout_town), 121241, true);
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(25, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 37,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to graveyard
                    ListenProximity(230324, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(37, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 59,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find crown holder
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    script = new CryptPortals();
                    script.Execute(world);
                    if (this.Game.Players.Count == 0) UnlockTeleport(6);
                    if (world.GetActorsBySNO(230324).Where(d => d.Visible).FirstOrDefault() != null)
                        Open(world, 230324);
                    ListenInteract(159446, 1, new Advance());
                    //199642 - holder
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(59, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 61,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Imon advisor
                    if (!Game.Players.IsEmpty) UnlockTeleport(6);
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_crypt_skeletonkingcrown_02, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trdun_crypt_skeletonkingcrown_02);
                        world.SpawnMonster(156353, world.GetActorBySNO(156381).Position);// or 156381
                    });
                    ListenKill(156353, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(61, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 54,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //get Leoric crown
                    ListenInteract(199642, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(54, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Tristram (by town portal) and talk to Hedric
                    ListenConversation(196041, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72221].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
            #region Reign of Black King
            this.Game.QuestManager.Quests.Add(72061, new Quest { RewardXp = 5625, RewardGold = 810, Completed = false, Saveable = true, NextQuest = 117779, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72061].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 30,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    StartConversation(this.Game.GetWorld(WorldSno.trout_town), 80681);
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(30, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 28,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to cathedral garden
                    ListenTeleport(19938, new BackToCath());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 3,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //enter Hall of Leoric
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_cain_intro, () =>
                    {
                        setActorOperable(this.Game.GetWorld(WorldSno.trdun_cain_intro), 156058, true);
                    });
                    ListenTeleport(60714, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(3, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 6,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to 2nd level of Cath
                    ListenTeleport(19783, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(6, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 37,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //we need to go deeper (to 3rd level of Cath)
                    ListenTeleport(87907, new Advance());
                })
            });

            
            this.Game.QuestManager.Quests[72061].Steps.Add(37, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 40,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //help Cormac(kill cultists)
                    var Kormak_Imprisoned = this.Game.GetWorld(WorldSno.a1trdun_level05_templar).GetActorBySNO(104813);
                    foreach (var act in Kormak_Imprisoned.GetActorsInRange(80)) 
                        if (act.ActorSNO.Id == 145745)
                        {
                            Prisoners.Add(act as ActorSystem.Monster);
                            (act as ActorSystem.Monster).Brain.DeActivate();
                            act.SetFacingRotation(ActorSystem.Movement.MovementHelpers.GetFacingAngle(act, Kormak_Imprisoned));
                        }

                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        try
                        {
                            foreach (var act in Prisoners)
                            {       //act.AddRopeEffect(182614, Kormak_Imprisoned); //[111529] triuneSummoner_Summon_rope
                                Kormak_Imprisoned.AddRopeEffect(182614, act); //[111529] triuneSummoner_Summon_rope
                                act.SetFacingRotation(ActorSystem.Movement.MovementHelpers.GetFacingAngle(act, Kormak_Imprisoned));
                                act.PlayActionAnimation(158306);
                                act.SetIdleAnimation(158306);
                            }
                        }
                        catch { }
                    });
                    if (Game.Players.IsEmpty) UnlockTeleport(3);
                    //if (this.Game.Players.Count > 0) this.Game.GetWorld(105406).GetActorBySNO(104813, true).Hidden = true;
                    ListenKill(145745, 7, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(40, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 42,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Kormac's stuff 178657
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (this.Game.CurrentQuest == 72061 && this.Game.CurrentStep == 40)
                        {
                            var world = this.Game.GetWorld(WorldSno.a1trdun_level05_templar);
                            foreach (var act in Prisoners)
                                act.Brain.Activate();
                            if (ProxyObject != null)
                                ProxyObject.Destroy();
                            AddFollower(world, 104813);
                            StartConversation(world, 104782);
                        }
                    });
                    ListenInteract(178657, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(42, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 56,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find and kill Jondar
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (this.Game.CurrentQuest == 72061 && this.Game.CurrentStep == 42)
                        {
                            var world = this.Game.GetWorld(WorldSno.a1trdun_level05_templar);
                            DestroyFollower(104813);
                            AddFollower(world, 104813);
                            StartConversation(world, 168278);
                            this.Game.AddOnLoadSceneAction(32993, () =>
                            {
                                StartConversation(world, 168282);
                            });
                        }
                    });
                    ListenKill(86624, 1, new LaunchConversation(104676));
                    ListenConversation(104676, new JondarDeath());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(56, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 44,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //join templar(wtf?)
                    var world = this.Game.GetWorld(WorldSno.a1trdun_level05_templar);
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (this.Game.CurrentQuest == 72061 && this.Game.CurrentStep == 56)
                        {
                            DestroyFollower(104813);
                            //AddFollower(this.Game.GetWorld(105406), 104813);
                        }
                        foreach (var Wall in world.GetActorsBySNO(109209))
                        {
                            Wall.PlayAnimation(11, 108568);
                            Wall.Attributes[GameAttribute.Deleted_On_Server] = true;
                            Wall.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
                            Wall.Attributes.BroadcastChangedIfRevealed();
                            Wall.Destroy();
                        }

                    });
                    UnlockTeleport(3);
                    script = new Advance();
                    script.Execute(world);
                    if (!this.Game.Empty)
                        foreach (var plr in this.Game.Players.Values)
                        {
                            if (!plr.HirelingTemplarUnlocked)
                            {
                                plr.HirelingTemplarUnlocked = true;
                                plr.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 1 });
                                plr.GrantAchievement(74987243307073);
                            }
                        }

                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(44, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 66,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //enter king's crypt
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_level05_templar, () =>
                    {
                        if (this.Game.CurrentQuest == 72061 && this.Game.CurrentStep == 44)
                        {
                            //DestroyFollower(104813);
                        }
                    });
                    ListenTeleport(19787, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(66, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 16,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Leoric's crypt
                    ListenProximity(5944, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 58,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //enter crypt
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        this.Game.GetWorld(WorldSno.a1trdun_king_level08).GetActorBySNO(461, true).Hidden = true;
                    });
                    UnlockTeleport(4);
                    ListenTeleport(19789, new Advance());
                    //if (!this.Game.Empty) this.Game.GetWorld(73261).GetActorBySNO(461, true).Hidden = true;
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(58, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 19,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill skeletons
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        script = new SpawnSkeletons();
                        script.Execute(this.Game.GetWorld(WorldSno.a1trdun_king_level08));

                    });

                    ListenKill(51339, 4, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 21,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //take crown on Leoric's head
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        Open(this.Game.GetWorld(WorldSno.a1trdun_king_level08), 5765);
                    });
                    //Open(this.Game.GetWorld(73261), 172645);
                    ListenInteract(5354, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 48,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Leoric
                    ListenKill(5350, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(48, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 50,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to fallen star room
                    this.Game.CurrentEncounter.activated = false;
                    ListenTeleport(117411, new Advance());
                    this.Game.AddOnLoadWorldAction(WorldSno.a1trdun_king_level08, () =>
                    {
                        Open(this.Game.GetWorld(WorldSno.a1trdun_king_level08), 175181);
                    });
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(50, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 52,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Tyrael convList 117403 qr 176870
                    ListenInteract(180900, 1, new LaunchConversation(181910)); //cork
                    ListenConversation(181910, new LaunchConversation(181912));
                    ListenConversation(181912, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(52, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 54,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //return to New Tristram
                    ListenTeleport(19947, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(54, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 24,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Cain
                    UnlockTeleport(5);
                    ListenConversation(117371, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72061].Steps.Add(24, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                    PlayCutscene(2);
                })
            });
            #endregion
            #region Tyrael Sword
            this.Game.QuestManager.Quests.Add(117779, new Quest { RewardXp = 4125, RewardGold = 630, Completed = false, Saveable = true, NextQuest = 72738, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[117779].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 198706);
                    world.GetActorBySNO(216574).Hidden = true;
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 10,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Wild Fields
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    ListenTeleport(19952, new Advance());
                    ListenProximity(60665, new Advance()); //if going through graveyard
                    var Gate = world.GetActorBySNO(230324);
                    Gate.Field2 = 16;
                    Gate.PlayAnimation(5, Gate.AnimationSet.TagMapAnimDefault[DiIiS_NA.GameServer.Core.Types.TagMap.AnimationSetKeys.Opening]);
                    world.BroadcastIfRevealed(plr => new DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD.ACDCollFlagsMessage
                    {
                        ActorID = Gate.DynamicID(plr),
                        CollFlags = 0
                    }, Gate);

                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(10, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 3,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Hazra cave
                    ListenTeleport(119893, new Advance());
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(3, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 18,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find piece of sword
                    this.Game.AddOnLoadWorldAction(WorldSno.fields_cave_swordofjustice_level01, () =>
                    {
                        if (this.Game.CurrentQuest == 117779 && this.Game.CurrentStep == 3)
                        {
                            StartConversation(this.Game.GetWorld(WorldSno.fields_cave_swordofjustice_level01), 130225);
                        }
                    });
                    //if (!this.Game.Empty) StartConversation(this.Game.GetWorld(119888), 130225);
                    ListenProximity(178213, new Advance());
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(18, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 13,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill cultists
                    UnlockTeleport(7);
                    ListenKill(178213, 6, new LaunchConversation(131144));
                    ListenConversation(131144, new LaunchConversation(194412));
                    ListenConversation(194412, new LaunchConversation(141778));
                    ListenConversation(141778, new Advance());
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(13, new QuestStep
            {
                Completed = false,
                Saveable = false,
                NextStep = 5,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //get piece of sword
                    ListenInteract(206527, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(5, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //take piece to Cain
                    ListenConversation(118037, new Advance());
                })
            });

            this.Game.QuestManager.Quests[117779].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                    StartConversation(this.Game.GetWorld(WorldSno.trout_town), 198713);
                })
            });
            #endregion
            #region Broken Blade
            this.Game.QuestManager.Quests.Add(72738, new Quest { RewardXp = 6205, RewardGold = 1065, Completed = false, Saveable = true, NextQuest = 73236, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72738].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 86,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    LeahTempId = world.GetActorBySNO(4580, true).GlobalID;
                    world.GetActorBySNO(4580, true).Hidden = true;
                    StartConversation(world, 198713);
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(86, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 88,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Sunken Temple
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    ListenProximity(80812, new LaunchConversation(111893));
                    ListenConversation(111893, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(88, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 90,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //follow Scoundrel NPC
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    AddFollower(world, 80812);
                    //Open(this.Game.GetWorld(71150), 170913);
                    StartConversation(world, 167656);
                    ListenConversation(167656, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(90, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 92,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with bandits
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    try { (world.FindAt(170913, new Vector3D { X = 1523.13f, Y = 857.71f, Z = 39.26f }, 5.0f) as Door).Open(); } catch { }
                    StartConversation(world, 167677);
                    ListenConversation(167677, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(92, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 94,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill the bandits
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    world.SpawnMonster(174013, new Vector3D { X = 1471.473f, Y = 747.4875f, Z = 40.1f });
                    ListenKill(174013, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(94, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 112,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Scoundrel
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    ListenProximity(80812, new LaunchConversation(111899));
                    ListenConversation(111899, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(112, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //lead Scoundrel to waypoint
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    try { (world.FindAt(170913, new Vector3D { X = 1444.1f, Y = 786.64f, Z = 39.7f }, 4.0f) as Door).Open(); } catch { }
                    setActorOperable(world, 63114, false);
                    setActorOperable(world, 61459, false);
                    ListenProximity(6442, new Advance());
                    if (!this.Game.Empty)
                        foreach (var plr in this.Game.Players.Values)
                        {
                            if (!plr.HirelingScoundrelUnlocked)
                            {
                                plr.HirelingScoundrelUnlocked = true;
                                plr.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 2 });
                                plr.GrantAchievement(74987243307147);
                            }
                            if (this.Game.Players.Count > 1)
                                plr.InGameClient.SendMessage(new HirelingNoSwapMessage() { NewClass = 2 }); //Призвать нельзя!
                            else
                                plr.InGameClient.SendMessage(new HirelingSwapMessage() { NewClass = 2 }); //Возможность призвать
                        }
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Sunken Temple
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    StartConversation(world, 223934);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    DestroyFollower(80812);
                    ListenProximity(108882, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 28,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Alaric
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    UnlockTeleport(8);
                    ListenConversation(81576, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(28, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 12,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Rotten forest
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    Open(world, 100849); //bridge
                    ListenProximity(100849, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(12, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 14,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 }, new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find 2 Orbs
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    ListenInteract(215434, 1, new CompleteObjective(0));
                    ListenInteract(215512, 1, new CompleteObjective(1));
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(14, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 30,
                Objectives = new List<Objective> { new Objective { Limit = 2, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //use 2 stones
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    UnlockTeleport(9);
                    setActorOperable(world, 63114, true);
                    setActorOperable(world, 61459, true);
                    ListenInteract(63114, 1, new CompleteObjective(0));
                    ListenInteract(61459, 1, new CompleteObjective(0));
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(30, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 38,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //enter the temple
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    Open(world, 144149); //bridge
                    Open(world, 100967);
                    ListenTeleport(60398, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(38, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 69,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //explore the temple
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    //60395 - trdun_cave_nephalem_03

                    ListenProximity(98799, new DrownedTemple1());
                    ListenKill(5395, 14, new LaunchConversation(108256));
                    ListenConversation(108256, new DrownedTemple2());//new Advance());

                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(69, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 99,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill prophet Ezek and skeletons
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);

                    var world = this.Game.GetWorld(WorldSno.trdun_cave_nephalem_03);
                    foreach (var act in world.GetActorsBySNO(139757)) act.Destroy();
                    world.SpawnMonster(139757, new Vector3D(292f, 275f, -76f));

                    ListenKill(139757, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(99, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 103,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Alaric in temple
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    StartConversation(this.Game.GetWorld(WorldSno.trdun_cave_nephalem_03), 133372);
                    ListenConversation(133372, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(103, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 71,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //defend the sword piece
                    DestroyFollower(4580);
                    AddFollower(this.Game.GetWorld(WorldSno.trout_town), 4580);
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_cave_nephalem_03, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trdun_cave_nephalem_03);
                        Open(world, 177439);
                        if (this.Game.CurrentQuest == 72738 && this.Game.CurrentStep == 103)
                        {
                            StartConversation(world, 108256);
                        }
                    });
                    ListenProximity(206461, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(71, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 56,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //use the piece of sword
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    DestroyFollower(4580);
                    AddFollower(world, 4580);
                    ListenInteract(206461, 1, new LaunchConversation(198925));
                    ListenConversation(198925, new LaunchConversation(133487));
                    ListenConversation(133487, new Advance());
                    world.GetActorByGlobalId(LeahTempId).Hidden = false;
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(56, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //return to Tristram
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_cave_nephalem_03, () =>
                    {
                        if (this.Game.CurrentQuest == 72738 && this.Game.CurrentStep == 56)
                        {
                            StartConversation(this.Game.GetWorld(WorldSno.trdun_cave_nephalem_03), 202967);
                        }
                    });
                    DestroyFollower(4580);
                    ListenProximity(3533, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72738].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
            #region Doom of Vortham
            this.Game.QuestManager.Quests.Add(73236, new Quest { RewardXp = 4950, RewardGold = 670, Completed = false, Saveable = true, NextQuest = 72546, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[73236].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    StartConversation(this.Game.GetWorld(WorldSno.trout_town), 120357);
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 20,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with parom man
                    ListenConversation(72817, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(20, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 59,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //go to Vortem square
                    var AttackedTown = this.Game.GetWorld(WorldSno.trout_townattack);
                    var Maghda = AttackedTown.GetActorBySNO(129345);
                    AttackedTown.Leave(Maghda);

                    ListenProximity(90367, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(59, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //kill all cultists
                    int Count = 0;
                    foreach (var cultist in this.Game.GetWorld(WorldSno.trout_townattack).GetActorsBySNO(90008))
                        if (cultist.CurrentScene.SceneSNO.Id == 76000)
                        {
                            cultist.Attributes[GameAttribute.Quest_Monster] = true;
                            cultist.Attributes.BroadcastChangedIfRevealed();
                            Count++;
                        }

                    ListenKill(90008, Count, new AttackTownKilled());
                    ListenConversation(194933, new LaunchConversation(194942));
                    ListenConversation(194942, new Advance());

                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 16,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (this.Game.CurrentQuest == 73236 && this.Game.CurrentStep == 11)
                        {
                            this.Game.GetWorld(WorldSno.trout_townattack).SpawnMonster(178619, new Vector3D { X = 581.237f, Y = 584.346f, Z = 70.1f });
                        }
                        ListenKill(178619, 1, new Advance());
                    });
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 63,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //kill 3 berserkers
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (this.Game.CurrentQuest == 73236 && this.Game.CurrentStep == 16)
                        {
                            var world = this.Game.GetWorld(WorldSno.trout_townattack);
                            world.SpawnMonster(178300, new Vector3D { X = 577.724f, Y = 562.869f, Z = 70.1f });
                            world.SpawnMonster(178300, new Vector3D { X = 565.886f, Y = 577.66f, Z = 70.1f });
                            world.SpawnMonster(178300, new Vector3D { X = 581.308f, Y = 581.079f, Z = 70.1f });
                        }
                    });
                    ListenKill(178300, 3, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(63, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 65,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //talk with priest
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_townattack, () =>
                    {
                        if (this.Game.CurrentQuest == 73236 && this.Game.CurrentStep == 63)
                        {
                           StartConversation(this.Game.GetWorld(WorldSno.trout_townattack), 120372);
                        }
                    });
                    ListenConversation(120372, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(65, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 67,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //go to church cellar
                   //this.Game.AddOnLoadAction(72882, () =>
                   //{
                   //this.Game.GetWorld(72882).GetActorBySNO(91162).Destroy();
                   //});
                    ListenTeleport(119870, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(67, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 69,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //find piece of sword
                    ListenInteract(153260, 1, new LaunchConversation(165080));
                    ListenConversation(165080, new LaunchConversation(165101));
                    ListenConversation(165101, new Advance());
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(69, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 9,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {  //go to Cain's house
                     
                    if (!this.Game.Empty) StartConversation(this.Game.GetWorld(WorldSno.fields_cave_swordofjustice_level01), 130225);
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
                })
            });

            this.Game.QuestManager.Quests[73236].Steps.Add(9, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    UnlockTeleport(10);
                })
            });
            #endregion
            #region To the Black Cult
            this.Game.QuestManager.Quests.Add(72546, new Quest { RewardXp = 8275, RewardGold = 455, Completed = false, Saveable = true, NextQuest = 72801, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72546].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                    this.Game.AddOnLoadWorldAction(WorldSno.trout_townattack_chapelcellar_a, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trout_townattack_chapelcellar_a);
                        foreach (var Table in world.GetActorsBySNO(153260)) { Table.SetUsable(false); Table.SetIdleAnimation(Table.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Open]); }
                        foreach (var Maghda in world.GetActorsBySNO(129345)) Maghda.Destroy();
                    });
                    var tristramWorld = this.Game.GetWorld(WorldSno.trout_town);
                    var Leah = tristramWorld.GetActorBySNO(4580);
                    var LeahAfterEvent = tristramWorld.SpawnMonster(138271, Leah.Position);
                    
                    //ListenProximity(4580, new LaunchConversation(93337)); //cork
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Conversations.Clear();
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(93337));
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 2;
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
                    ListenConversation(93337, new Advance());
                    this.Game.CurrentEncounter.activated = false;
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Aranea Cave
                    var LeahAfterEvent = this.Game.GetWorld(WorldSno.trout_town).GetActorBySNO(138271);
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes[GameAttribute.Conversation_Icon, 0] = 1;
                    (LeahAfterEvent as ActorSystem.InteractiveNPC).Attributes.BroadcastChangedIfRevealed();
                    ListenTeleport(78572, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 31,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Aranea Queen lair
                    this.Game.GetWorld(WorldSno.trout_town).GetActorBySNO(138271, true).Hidden = true;
                    //this.Game.GetWorld(71150).GetActorBySNO(138271,true).SetVisible(false);
                    ListenTeleport(62726, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(31, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 19,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with woman in web
                    var world = this.Game.GetWorld(WorldSno.a1dun_spidercave_02);
                    setActorOperable(world, 213490, false);
                    setActorOperable(world, 104545, false);
                    ListenProximity(104545, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Aranea Queen
                    this.Game.GetWorld(WorldSno.a1dun_spidercave_02).SpawnMonster(51341, new Vector3D { X = 149.439f, Y = 121.452f, Z = 13.794f });
                    ListenKill(51341, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 23,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //grab Aranea acid
                    setActorOperable(this.Game.GetWorld(WorldSno.a1dun_spidercave_02), 213490, true);
                    ListenInteract(213490, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(23, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 26,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //use acid on Karina
                    setActorOperable(this.Game.GetWorld(WorldSno.a1dun_spidercave_02), 104545, true);
                    ListenInteract(104545, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(26, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 47,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Southern Highlands
                    ListenTeleport(93632, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(47, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 29,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Karina
                    setActorOperable(this.Game.GetWorld(WorldSno.trout_town), 167311, false);
                    ListenProximity(194263, new LaunchConversation(191511)); //cork
                    ListenConversation(191511, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(29, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 36,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Hazra staff
                    ListenInteract(178151, 1, new Advance());
                    if (this.Game.Empty) UnlockTeleport(11);
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(36, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 10,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Hazra wall
                    setActorOperable(this.Game.GetWorld(WorldSno.trout_town), 167311, true);
                    UnlockTeleport(11);
                    ListenInteract(167311, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(10, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 51,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to the Leoric's Manor
                    ListenInteract(103316, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(51, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //enter the Leoric's Manor
                    ListenTeleport(100854, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 43,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //explore the Leoric's Manor
                    UnlockTeleport(12);
                    ListenInteract(99304, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(43, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 16,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill cultists
                    if (!this.Game.Empty) StartConversation(this.Game.GetWorld(WorldSno.a1dun_leor_manor), 134968);
                    ListenConversation(134968, new LaunchConversation(134565));
                    var world = this.Game.GetWorld(WorldSno.trout_town);
                    world.GetActorBySNO(4580).Hidden = false;
                    world.GetActorBySNO(4580).SetVisible(true);
                    ListenKill(6024, 7, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72546].Steps.Add(16, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
            #region Captived Angel
            this.Game.QuestManager.Quests.Add(72801, new Quest { RewardXp = 10925, RewardGold = 1465, Completed = false, Saveable = true, NextQuest = 136656, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[72801].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 21,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to 1st level of Torture Rooms
                    UnlockTeleport(13);
                    ListenTeleport(19774, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(21, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 65,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to 2nd level of Torture Rooms
                    ListenTeleport(19775, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(65, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 2,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Highlands Bridge
                    if (this.Game.Empty) UnlockTeleport(14);
                    ListenTeleport(87832, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(2, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 34,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //go to Leoric's Jail
                    UnlockTeleport(14);
                    ListenTeleport(94672, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(34, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 17,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Asilla Queen (npc 103381)
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        setActorOperable(this.Game.GetWorld(WorldSno.trdun_jail_level01), 95571, false);
                    });
                    ListenConversation(103388, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(17, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 19,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //free 6 souls
                  //spawn souls on 104104, 104106, 104108
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trdun_jail_level01);
                        setActorOperable(world, 95571, true);
                        script = new SpawnSouls();
                        script.Execute(world);
                     });
                    ListenInteract(102927, 6, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(19, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 36,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Overseer
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        this.Game.GetWorld(WorldSno.trdun_jail_level01).SpawnMonster(98879, new Vector3D { X = 360.236f, Y = 840.47f, Z = 0.1f });
                    });
                    ListenKill(98879, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(36, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 7,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Butcher's Room		
                    if (this.Game.Empty) UnlockTeleport(15);
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_jail_level01, () =>
                    {
                        Open(this.Game.GetWorld(WorldSno.trdun_jail_level01), 100862);
                    });
                    ListenTeleport(90881, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(7, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 41,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill Butcher
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_butcherslair_02, () =>
                    {
                        var world = this.Game.GetWorld(WorldSno.trdun_butcherslair_02);
                        setActorOperable(world, 105361, false);
                        if (world.GetActorBySNO(3526) == null)
                            world.SpawnMonster(3526, new Vector3D { X = 93.022f, Y = 89.86f, Z = 0.1f });

                    });
                    UnlockTeleport(15);
                    ListenKill(3526, 1, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(41, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 39,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //find Tyrael
                    this.Game.CurrentEncounter.activated = false;
                    this.Game.AddOnLoadWorldAction(WorldSno.trdun_butcherslair_02, () =>
                    {
                        setActorOperable(this.Game.GetWorld(WorldSno.trdun_butcherslair_02), 105361, true);
                    });
                    ListenTeleport(148551, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(39, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 11,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //kill cultists
                    ListenKill(102452, 6, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(11, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 13,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Tyrael (npc 183117)
                    ListenProximity(183117, new LaunchConversation(120220)); //cork
                    ListenConversation(120220, new Advance());
                })
            });

            this.Game.QuestManager.Quests[72801].Steps.Add(13, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
            #region Return to New Tristram
            this.Game.QuestManager.Quests.Add(136656, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = true, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

            this.Game.QuestManager.Quests[136656].Steps.Add(-1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                {
                })
            });

            this.Game.QuestManager.Quests[136656].Steps.Add(1, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 8,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with Tyrael

                    ListenProximity(6353, new LaunchConversation(72897)); //cork
                    ListenConversation(72897, new Advance());
                })
            });

            this.Game.QuestManager.Quests[136656].Steps.Add(8, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = 4,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //talk with caravan leader
                    
                    ListenConversation(177564, new ChangeAct(100));
                })
            });

            this.Game.QuestManager.Quests[136656].Steps.Add(4, new QuestStep
            {
                Completed = false,
                Saveable = true,
                NextStep = -1,
                Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
                OnAdvance = new Action(() =>
                { //complete
                })
            });
            #endregion
        }

        public static bool Break(MapSystem.World world, Int32 snoId)
		{
			var actor = world.GetActorBySNO(snoId);
			(actor as DesctructibleLootContainer).Die();
			return true;
		}
	}
}
