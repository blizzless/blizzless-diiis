//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class XPPool : Gizmo
	{
		public XPPool(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
			//Attributes[GameAttribute.MinimapIconOverride] = 376779;
			Attributes[GameAttribute.Gizmo_State] = 0;
		}


		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttribute.Gizmo_State] = 1;
			Attributes.BroadcastChangedIfRevealed();
			//this.World.BroadcastIfRevealed(plr => new XPPoolActivatedMessage { ActorID = this.DynamicID(plr) }, this);
			foreach (var plr in GetPlayersInRange(100f))
				plr.AddRestExperience();
		}
	}
}
