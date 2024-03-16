using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._a2dun_aqd_act_waterwheel_lever_a_01_waterpuzzle)]
	public class ActIITombLever : Gizmo
	{
		public ActIITombLever(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.MinimapActive] = true;
		}


		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (Attributes[GameAttributes.Disabled] == true) return;
			try
			{
				Door waterfall = World.FindActorAt(ActorSno._caout_oasis_door_aqueduct_a_top, Position, 80.0f) as Door;
				if (waterfall == null)
				{
					Door gate = World.FindActorAt(ActorSno._caout_oasis_door_aqueduct_a, Position, 80.0f) as Door;
					if (gate == null)
						(World.FindActorAt(ActorSno._caout_oasis_cenote_door, Position, 80.0f) as Door).Open();
					else
						gate.Open();
				}
				else
				{
					waterfall.Open();
				}
				Attributes[GameAttributes.Disabled] = true;
				Attributes[GameAttributes.Gizmo_Has_Been_Operated] = true;
				Attributes[GameAttributes.Gizmo_State] = 1;
				Attributes.BroadcastChangedIfRevealed();
			}
			catch { }
		}
	}
}
