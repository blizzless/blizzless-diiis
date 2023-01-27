using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(ActorSno._caout_cage)]
    class CaOut_Cage : LootContainer
	{
		
		public CaOut_Cage(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{

			if (Attributes[GameAttribute.Disabled]) return;

			PlayAnimation(5, (AnimationSno)AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
			SetIdleAnimation((AnimationSno)AnimationSetKeys.Open.ID);

			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			Attributes.BroadcastChangedIfRevealed();

			base.OnTargeted(player, message);
			Attributes[GameAttribute.Disabled] = true;


		}
	}
}
