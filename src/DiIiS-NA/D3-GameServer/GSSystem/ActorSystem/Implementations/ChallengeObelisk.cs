//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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
    [HandledSNO(416137 /* x1_OpenWorld_LootRunObelisk_B.acr */)]
    public sealed class ChallengeObelisk : Gizmo
    {
        public ChallengeObelisk(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Attributes[GameAttribute.TeamID] = 2;
            this.Attributes[GameAttribute.MinimapActive] = true;
            this.Attributes.BroadcastChangedIfRevealed();
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            bool Activated = false;

            this.PlayAnimation(5, AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
            Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
            Attributes[GameAttribute.Untargetable] = !Activated;
            Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
            Attributes[GameAttribute.Operatable] = Activated;
            Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
            Attributes[GameAttribute.Disabled] = !Activated;
            Attributes[GameAttribute.Immunity] = !Activated;
            Attributes.BroadcastChangedIfRevealed();
            CollFlags = 0;

            TickTimer Timeout = new SecondsTickTimer(this.World.Game, 3.5f);
            var Boom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(Timeout));
            Boom.ContinueWith(delegate
            {
                this.World.GetActorBySNO(473334).SetVisible(true);
                this.World.GetActorBySNO(473334).Reveal(player);

                World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage()
                {
                    ActorID = DynamicID(plr),
                    CollFlags = 0
                }, this);
            });

        }

        public override bool Reveal(Player player)
        {
             if (!base.Reveal(player))
                return false;
            if (!Attributes[GameAttribute.Operatable])
            {
                this.World.GetActorBySNO(473334).SetVisible(false);
                this.World.GetActorBySNO(473334).Unreveal(player);
            }
            else
            {
                this.PlayAnimation(5, AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
            }
            return true;
        }

        private bool WaitToSpawn(TickTimer timer)
        {
            while (timer.TimedOut != true)
            {

            }
            return true;
        }
    }
}
