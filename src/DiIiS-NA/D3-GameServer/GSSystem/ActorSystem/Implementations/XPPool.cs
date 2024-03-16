using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class XPPool : Gizmo
	{
		public XPPool(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.MinimapActive] = true;
			//Attributes[GameAttribute.MinimapIconOverride] = 376779;
			Attributes[GameAttributes.Gizmo_State] = 0;
		}


		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			Attributes[GameAttributes.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttributes.Gizmo_State] = 1;
			Attributes.BroadcastChangedIfRevealed();
			//this.World.BroadcastIfRevealed(plr => new XPPoolActivatedMessage { ActorID = this.DynamicID(plr) }, this);
			foreach (var plr in GetPlayersInRange(100f))
				plr.AddRestExperience();
		}
	}
}
