//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
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
    [HandledSNO(ActorSno._x1_openworld_lootrunobelisk_b /* x1_OpenWorld_LootRunObelisk_B.acr */)]
    public sealed class NephalemStone : Gizmo
    {
        public NephalemStone(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            this.Attributes[GameAttribute.TeamID] = 2;
            this.Attributes[GameAttribute.MinimapActive] = true;
            this.Attributes[GameAttribute.Untargetable] = false;
            this.Attributes.BroadcastChangedIfRevealed();
            this.Attributes[GameAttribute.MinimapIconOverride] = 221224;//327066;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.RiftStartEncounterMessage) { ActorID = this.DynamicID(player) });
        }

        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;

            if (Attributes[GameAttribute.Untargetable])
            {
                this.PlayAnimation(5, AnimationSet.TagMapAnimDefault[AnimationSetKeys.Open]);
                this.SetIdleAnimation(AnimationSet.TagMapAnimDefault[AnimationSetKeys.Open]);
            }
            else
            {
                this.PlayAnimation(5, AnimationSet.TagMapAnimDefault[AnimationSetKeys.IdleDefault]);
                this.SetIdleAnimation(AnimationSet.TagMapAnimDefault[AnimationSetKeys.IdleDefault]);
            }
            
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
            {

                HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("x1_OpenWorld_LootRunObelisk_B"),
                Place = new MessageSystem.Message.Fields.WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
                ImageInfo = 221224,
                Label = -1,
                snoStringList = 0x0000F063,
                snoKnownActorOverride = -1,
                snoQuestSource = -1,
                Image = -1,
                Active = true,
                CanBecomeArrow = false,
                RespectsFoW = false,
                IsPing = true,
                PlayerUseFlags = 0
            });
            //*/
            return true;
        }
    }
}
