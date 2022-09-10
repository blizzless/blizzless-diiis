//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
    class DrownedTemple1 : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public DrownedTemple1()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
			var DrownedTempleWorld = world.Game.GetWorld(WorldSno.trdun_cave_nephalem_03);
            
            List<uint> SkeletonsList = new List<uint> { };
            List<uint> Skeletons2List = new List<uint> { };

            var AllSkeletons1 = DrownedTempleWorld.GetActorsBySNO(ActorSno._skeletonarcher_b, ActorSno._shield_skeleton_b, ActorSno._skeletonsummoner_b);
			var AllSkeletons3 = DrownedTempleWorld.GetActorsBySNO(ActorSno._skeleton_b);
			var AllSkeletons5 = DrownedTempleWorld.GetActorsBySNO(ActorSno._nephalem_ghost_a_drownedtemple_martyr_skeleton);
            Vector3D PositionBoss = null;

            #region Варим солянку
            //Кидаем в солянку
            foreach (var Skelet in AllSkeletons1)
            {
                //SkeletonsList.Add(Skelet.GlobalID);
                DrownedTempleWorld.SpawnMonster(ActorSno._skeleton_b, Skelet.Position);
                Skelet.Destroy();
            }
            foreach (var Skelet in AllSkeletons3)
            {
                //SkeletonsList.Add(Skelet.GlobalID);
                
            }
            //PositionBoss = AllSkeletons5[0].Position;
            foreach (var Skelet in AllSkeletons5)
            {
                Skelet.Destroy();
            }
            //to5276


            #endregion

            PositionBoss = new Vector3D(292f, 275f, -76f);

            /*
            var ListenerSkeletons = Task<bool>.Factory.StartNew(() => OnKillListener(SkeletonsList, DrownedTempleWorld));
            
            //Ждём пока убьют
            ListenerSkeletons.ContinueWith(delegate
            {
                var AllTablets = DrownedTempleWorld.GetActorsBySNO(92387);
                foreach (var Tablet in AllTablets)
                {
                    Tablet.PlayAnimation(5, Tablet.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                    DrownedTempleWorld.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
                    {
                        ActorID = Tablet.DynamicID(plr),
                        AnimationSNO = AnimationSetKeys.Open.ID,
                    }, Tablet);
                }
                
                DrownedTempleWorld.SpawnMonster(139713, AllTablets[0].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139713).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139713).Attributes[GameAttribute.Quest_Monster] = true;

                DrownedTempleWorld.SpawnMonster(139715, AllTablets[1].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139715).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139715).Attributes[GameAttribute.Quest_Monster] = true;

                DrownedTempleWorld.SpawnMonster(139756, AllTablets[2].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139756).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139756).Attributes[GameAttribute.Quest_Monster] = true;

                StartConversation(DrownedTempleWorld, 133356);

                var ListenerSecondSkeletons = Task<bool>.Factory.StartNew(() => OnKillListener(Skeletons2List, DrownedTempleWorld));
                ListenerSecondSkeletons.ContinueWith(delegate
                {
                    StartConversation(DrownedTempleWorld, 108256);
                    //PositionBoss = PowerSystem.PowerContext.RandomDirection(world.Players[0].Position, 3f, 8f);

                    Task.Delay(3000).ContinueWith(delegate
                    {
                        DrownedTempleWorld.SpawnMonster(139757, PositionBoss);
                    });
                });
                //Спауним гадов
            });
            //*/
		}

        private bool OnKillListener(List<uint> monstersAlive, MapSystem.World world)
        {
            int monstersKilled = 0;
            var monsterCount = monstersAlive.Count;

            while (monstersKilled != monsterCount)
            {
                for (int i = monstersAlive.Count - 1; i >= 0; i--)
                {
                    if (world.HasMonster(monstersAlive[i]))
                    {

                    }
                    else
                    {
                        Logger.Debug(monstersAlive[i] + " убит");
                        monstersAlive.RemoveAt(i);
                        monstersKilled++;
                    }
                }
            }
            return true;
        }

        private bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
			{
				player.Value.Conversations.StartConversation(conversationId);
			}
			return true;
		}
	}

    class DrownedTemple2 : QuestEvent
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public DrownedTemple2()
            : base(0)
        {

        }

        public override void Execute(MapSystem.World world)
        {
            var DrownedTempleWorld = world.Game.GetWorld(WorldSno.trdun_cave_nephalem_03);

            List<uint> SkeletonsList = new List<uint> { };
            List<uint> Skeletons2List = new List<uint> { };
            
            var PositionBoss = new Vector3D(292f, 275f, -76f);

            var AllTablets = DrownedTempleWorld.GetActorsBySNO(ActorSno._a1dun_caves_nephalem_altar_tablet_a);
            foreach (var Tablet in AllTablets)
            {
                Tablet.PlayAnimation(5, Tablet.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                DrownedTempleWorld.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
                {
                    ActorID = Tablet.DynamicID(plr),
                    AnimationSNO = AnimationSetKeys.Open.ID,
                }, Tablet);
            }

            var actorSnos = new ActorSno[]
            {
                ActorSno._nephalem_ghost_a_drownedtemple_martyr1_skeleton,
                ActorSno._nephalem_ghost_a_drownedtemple_martyr2_skeleton,
                ActorSno._nephalem_ghost_a_drownedtemple_martyr3_skeleton
            };

            for (var i = 0; i < actorSnos.Length; i++)
            {
                var sno = actorSnos[i];
                DrownedTempleWorld.SpawnMonster(sno, AllTablets[i].Position);
                var actor = DrownedTempleWorld.GetActorBySNO(sno);
                actor.Attributes[GameAttribute.Quest_Monster] = true;
                Skeletons2List.Add(actor.GlobalID);
            }

            StartConversation(DrownedTempleWorld, 133356);

            world.Game.QuestManager.Advance();
            /*
            var ListenerSkeletons = Task<bool>.Factory.StartNew(() => OnKillListener(SkeletonsList, DrownedTempleWorld));
            
            //Ждём пока убьют
            ListenerSkeletons.ContinueWith(delegate
            {
                var AllTablets = DrownedTempleWorld.GetActorsBySNO(92387);
                foreach (var Tablet in AllTablets)
                {
                    Tablet.PlayAnimation(5, Tablet.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
                    DrownedTempleWorld.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
                    {
                        ActorID = Tablet.DynamicID(plr),
                        AnimationSNO = AnimationSetKeys.Open.ID,
                    }, Tablet);
                }
                
                DrownedTempleWorld.SpawnMonster(139713, AllTablets[0].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139713).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139713).Attributes[GameAttribute.Quest_Monster] = true;

                DrownedTempleWorld.SpawnMonster(139715, AllTablets[1].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139715).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139715).Attributes[GameAttribute.Quest_Monster] = true;

                DrownedTempleWorld.SpawnMonster(139756, AllTablets[2].Position);
                Skeletons2List.Add(DrownedTempleWorld.GetActorBySNO(139756).GlobalID);
                DrownedTempleWorld.GetActorBySNO(139756).Attributes[GameAttribute.Quest_Monster] = true;

                StartConversation(DrownedTempleWorld, 133356);

                var ListenerSecondSkeletons = Task<bool>.Factory.StartNew(() => OnKillListener(Skeletons2List, DrownedTempleWorld));
                ListenerSecondSkeletons.ContinueWith(delegate
                {
                    StartConversation(DrownedTempleWorld, 108256);
                    //PositionBoss = PowerSystem.PowerContext.RandomDirection(world.Players[0].Position, 3f, 8f);

                    Task.Delay(3000).ContinueWith(delegate
                    {
                        DrownedTempleWorld.SpawnMonster(139757, PositionBoss);
                    });
                });
                //Спауним гадов
            });
            //*/
        }

        private bool StartConversation(MapSystem.World world, Int32 conversationId)
        {
            foreach (var player in world.Players)
            {
                player.Value.Conversations.StartConversation(conversationId);
            }
            return true;
        }
    }
}
