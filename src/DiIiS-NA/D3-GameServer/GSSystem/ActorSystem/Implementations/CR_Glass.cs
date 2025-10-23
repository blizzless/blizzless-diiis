using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._challenge_rift_inspect_armorrack)]
    class CR_Glass : Gizmo
    {
        public CR_Glass(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttributes.TeamID] = 2;
            Attributes[GameAttributes.MinimapActive] = true;
            Attributes.BroadcastChangedIfRevealed();
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.ANNDataMessage46)
            {
                ActorID = DynamicID(player)
            });
        }
    }
}
