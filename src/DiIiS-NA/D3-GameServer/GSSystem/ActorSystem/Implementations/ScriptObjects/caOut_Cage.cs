//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

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

			PlayAnimation(5, AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening]);
			SetIdleAnimation(AnimationSetKeys.Open.ID);

			Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			Attributes.BroadcastChangedIfRevealed();

			base.OnTargeted(player, message);
			Attributes[GameAttribute.Disabled] = true;


		}
	}
}
