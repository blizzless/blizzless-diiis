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
			if (this.Attributes[GameAttribute.Disabled] == true) return;
			try
			{
				Door waterfall = this.World.FindAt(ActorSno._caout_oasis_door_aqueduct_a_top, this.Position, 80.0f) as Door;
				if (waterfall == null)
				{
					Door gate = this.World.FindAt(ActorSno._caout_oasis_door_aqueduct_a, this.Position, 80.0f) as Door;
					if (gate == null)
						(this.World.FindAt(ActorSno._caout_oasis_cenote_door, this.Position, 80.0f) as Door).Open();
					else
						gate.Open();
				}
				else
				{
					waterfall.Open();
				}
				this.Attributes[GameAttribute.Disabled] = true;
				this.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
				this.Attributes[GameAttribute.Gizmo_State] = 1;
				this.Attributes.BroadcastChangedIfRevealed();
			}
			catch { }
		}
	}
}
