//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(346878)]
	class A5BogBeacon : Gizmo
	{
		public bool isOpened = false;

		public A5BogBeacon(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (this.isOpened == true)
			{
				player.InGameClient.SendMessage(new SetIdleAnimationMessage
				{
					ActorID = this.DynamicID(player),
					AnimationSNO = AnimationSetKeys.Open.ID
				});
			}
			return true;
		}

		public void Open()
		{
			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			this.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			this.Attributes[GameAttribute.Gizmo_State] = 1;
			this.CollFlags = 0;
			this.isOpened = true;

			Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (this.Attributes[GameAttribute.Disabled]) return;
			this.Open();

			base.OnTargeted(player, message);
			this.Attributes[GameAttribute.Disabled] = true;
		}
	}
}
