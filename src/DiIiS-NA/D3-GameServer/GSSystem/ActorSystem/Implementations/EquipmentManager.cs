using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._equipmentmanagertest)] //EquipmentManagerTest
    class EquipmentManager : InteractiveNPC
    {
        public EquipmentManager(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttributes.MinimapActive] = true;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            //            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.ANNDataMessage23) - Бафф (шрайн)
            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.ANNDataMessage45)
            {
                ActorID = DynamicID(player)
            });
        }

        public override bool Reveal(Player player)
        {
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
            {
                HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("EquipmentManagerTest"),
                Place = new MessageSystem.Message.Fields.WorldPlace { Position = Position, WorldID = World.GlobalID },
                ImageInfo = -1,
                Label = -1,
                snoStringList = -1,
                snoKnownActorOverride = -1,
                snoQuestSource = -1,
                Image = -1,
                Active = true,
                CanBecomeArrow = false,
                RespectsFoW = false,
                IsPing = true,
                PlayerUseFlags = 0
            });

            return base.Reveal(player);
        }
    }
}
