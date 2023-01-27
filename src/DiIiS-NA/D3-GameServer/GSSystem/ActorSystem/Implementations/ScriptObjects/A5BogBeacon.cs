using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._x1_bog_catacombsportal_beaconloc_first)]
	class A5BogBeacon : Gizmo
	{
		public bool isOpened = false;

		public A5BogBeacon(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (isOpened == true)
			{
				player.InGameClient.SendMessage(new SetIdleAnimationMessage
				{
					ActorID = DynamicID(player),
					AnimationSNO = AnimationSetKeys.Open.ID
				});
			}
			return true;
		}

		public void Open()
		{
			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttribute.Gizmo_State] = 1;
			CollFlags = 0;
			isOpened = true;

			Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (Attributes[GameAttribute.Disabled]) return;
			Open();

			base.OnTargeted(player, message);
			Attributes[GameAttribute.Disabled] = true;
		}
	}
}
