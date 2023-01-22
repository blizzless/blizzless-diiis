﻿//Blizzless Project 2022 
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._x1_openworld_lootrunobelisk_b /* x1_OpenWorld_LootRunObelisk_B.acr */)]
    public sealed class NephalemStone : Gizmo
    {
        public NephalemStone(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttribute.TeamID] = 2;
            Attributes[GameAttribute.MinimapActive] = true;
            Attributes[GameAttribute.Untargetable] = false;
            Attributes.BroadcastChangedIfRevealed();
            Attributes[GameAttribute.MinimapIconOverride] = 221224;//327066;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.RiftStartEncounterMessage) { ActorID = DynamicID(player) });
        }

        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;

            var animation = Attributes[GameAttribute.Untargetable] ? AnimationSet.Animations[AnimationSetKeys.Open.ID] : AnimationSet.Animations[AnimationSetKeys.IdleDefault.ID];
            PlayAnimation(5, animation);
            SetIdleAnimation(animation);
            
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
            {

                HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("x1_OpenWorld_LootRunObelisk_B"),
                Place = new MessageSystem.Message.Fields.WorldPlace { Position = Position, WorldID = World.GlobalID },
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
