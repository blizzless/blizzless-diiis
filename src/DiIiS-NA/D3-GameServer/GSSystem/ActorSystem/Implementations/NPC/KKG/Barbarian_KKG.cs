//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	//[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 449323 Name: Barbarian_KKG_Event, NumInWorld: 0
	//[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 435818 Name: Barbarian_KKG, NumInWorld: 0
	[HandledSNO(ActorSno._barbarian_kkg, ActorSno._barbarian_kkg_event)] //Barbarian_KKG
	public class Barbarian_KKG : NPC
	{
		public Barbarian_KKG(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.PlayActionAnimation(449259);
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
	}
}
